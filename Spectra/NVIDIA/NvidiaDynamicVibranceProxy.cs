using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Spectra.common;

namespace Spectra.NVIDIA
{
    class NvidiaDynamicVibranceProxy : IVibranceProxy
    {
        #region DllImports — vibranceDLL
        [DllImport("vibranceDLL.dll", EntryPoint = "?initializeLibrary@vibrance@vibranceDLL@@QAE_NXZ",
            CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        static extern bool initializeLibrary();

        [DllImport("vibranceDLL.dll", EntryPoint = "?unloadLibrary@vibrance@vibranceDLL@@QAE_NXZ",
            CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        static extern bool unloadLibrary();

        [DllImport("vibranceDLL.dll", EntryPoint = "?getActiveOutputs@vibrance@vibranceDLL@@QAEHQAPAH0@Z",
            CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        static extern int getActiveOutputs([In, Out] int[] gpuHandles, [In, Out] int[] outputIds);

        [DllImport("vibranceDLL.dll", EntryPoint = "?enumeratePhsyicalGPUs@vibrance@vibranceDLL@@QAEXQAPAH@Z",
            CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        static extern void enumeratePhsyicalGPUs([In, Out] int[] gpuHandles);

        [DllImport("vibranceDLL.dll", EntryPoint = "?getGpuName@vibrance@vibranceDLL@@QAE_NQAPAHPAD@Z",
            CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        static extern bool getGpuName([In, Out] int[] gpuHandles, StringBuilder szName);

        [DllImport("vibranceDLL.dll", EntryPoint = "?getDVCInfo@vibrance@vibranceDLL@@QAE_NPAUNV_DISPLAY_DVC_INFO@12@H@Z",
            CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        static extern bool getDVCInfo(ref NvDisplayDvcInfo info, int defaultHandle);

        [DllImport("vibranceDLL.dll", EntryPoint = "?enumerateNvidiaDisplayHandle@vibrance@vibranceDLL@@QAEHH@Z",
            CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        static extern int enumerateNvidiaDisplayHandle(int index);

        [DllImport("vibranceDLL.dll", EntryPoint = "?setDVCLevel@vibrance@vibranceDLL@@QAE_NHH@Z",
            CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        static extern bool setDVCLevel([In] int defaultHandle, [In] int level);

        [DllImport("vibranceDLL.dll", EntryPoint = "?getGpuSystemType@vibrance@vibranceDLL@@QAEHPAH@Z",
            CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        static extern NvSystemType getGpuSystemType(int gpuHandle);

        [DllImport("vibranceDLL.dll", EntryPoint = "?getAssociatedNvidiaDisplayHandle@vibrance@vibranceDLL@@QAEHPBDH@Z",
            CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        static extern int getAssociatedNvidiaDisplayHandle(string deviceName, [In] int length);
        #endregion

        #region DllImports — Win32
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        #endregion

        public const int NvapiMaxPhysicalGpus = 64;
        public const int NvapiMaxLevel        = 63;
        public const int NvapiDefaultLevel    = 0;

        public const string NvapiErrorInitFailed =
            "VibranceProxy failed to initialize! Press Ok to open the vibranceGUI Steam Guide in your browser. " +
            "Scroll down to section \"Troubleshooting, Errors, Q&A\".";
        public const string GuideLink = "https://vibrancegui.com/vibrance/guide";

        private static VibranceInfo _vibranceInfo;
        private static List<ApplicationSetting> _applicationSettings;
        private static Dictionary<string, Tuple<ResolutionModeWrapper, List<ResolutionModeWrapper>>> _windowsResolutionSettings;
        private WinEventHook _hook;
        private static Screen _gameScreen;

        // Per-monitor desktop vibrance levels set by the user.
        // Populated by SetVibranceForMonitor (multi-monitor) and cleared by SetVibranceWindowsLevel (single-level).
        // Restored when a game loses focus so each monitor returns to the user's chosen level.
        private static readonly Dictionary<string, int> _monitorDesktopLevels = new Dictionary<string, int>();

        // null = desktop mode (no profile active), non-null = process name of active profile.
        private static volatile string _lastProfileApplied;
        private static System.Threading.Timer _pollTimer;

        public NvidiaDynamicVibranceProxy(
            List<ApplicationSetting> savedApplicationSettings,
            Dictionary<string, Tuple<ResolutionModeWrapper, List<ResolutionModeWrapper>>> currentWindowsResolutionSettings)
        {
            try
            {
                _applicationSettings       = savedApplicationSettings;
                _windowsResolutionSettings = currentWindowsResolutionSettings;
                _vibranceInfo              = new VibranceInfo();

                if (initializeLibrary())
                    InitializeProxy();

                if (_vibranceInfo.isInitialized)
                {
                    _hook = WinEventHook.GetInstance();
                    _hook.WinEventHookHandler += OnWinEventHook;

                    // 500ms polling backup — catches launches/alt-tabs that WinEventHook misses
                    _pollTimer = new System.Threading.Timer(_ => PollAndApply(), null, 1000, 500);
                }
            }
            catch (Exception ex)
            {
                MainForm.Log(ex);
                DialogResult result = MessageBox.Show(NvapiErrorInitFailed, "Spectra Error",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                    Process.Start(GuideLink);
            }
        }

        private void InitializeProxy()
        {
            int[] gpuHandles = new int[NvapiMaxPhysicalGpus];
            int[] outputIds  = new int[NvapiMaxPhysicalGpus];
            enumeratePhsyicalGPUs(gpuHandles);

            foreach (int gpuHandle in gpuHandles)
            {
                if (gpuHandle != 0)
                {
                    NvSystemType systemType = getGpuSystemType(gpuHandle);
                    if (systemType == NvSystemType.NvSystemTypeUnknown)
                    {
                        MessageBox.Show("VibranceProxy failed to initialize — GPU system type unknown.",
                            "Spectra Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        _vibranceInfo.isInitialized = false;
                        return;
                    }
                }
            }

            EnumerateDisplayHandles();

            _vibranceInfo.activeOutput  = getActiveOutputs(gpuHandles, outputIds);
            var sb = new StringBuilder(64);
            getGpuName(gpuHandles, sb);
            _vibranceInfo.szGpuName     = sb.ToString();
            _vibranceInfo.defaultHandle = enumerateNvidiaDisplayHandle(0);

            NvDisplayDvcInfo info = new NvDisplayDvcInfo();
            if (getDVCInfo(ref info, _vibranceInfo.defaultHandle))
                if (info.currentLevel != _vibranceInfo.userVibranceSettingDefault)
                    setDVCLevel(_vibranceInfo.defaultHandle, _vibranceInfo.userVibranceSettingDefault);

            _vibranceInfo.isInitialized = true;
        }

        // ── Profile matching ─────────────────────────────────────────────────
        private static bool MatchesProfile(ApplicationSetting s, string processName)
            => string.Equals(Path.GetFileNameWithoutExtension(s.FileName), processName, StringComparison.OrdinalIgnoreCase)
            || string.Equals(s.Name, processName, StringComparison.OrdinalIgnoreCase);

        // ── Desktop restore ───────────────────────────────────────────────────
        // Restores each monitor to its saved desktop vibrance level.
        // In single-monitor mode (or when SetVibranceWindowsLevel was used), falls
        // back to userVibranceSettingDefault applied to the configured target.
        private static void RestoreDesktopVibrance()
        {
            if (_monitorDesktopLevels.Count > 0)
            {
                foreach (var kvp in _monitorDesktopLevels)
                {
                    int h = getAssociatedNvidiaDisplayHandle(kvp.Key, kvp.Key.Length);
                    if (h == -1) h = _vibranceInfo.defaultHandle;
                    if (h != -1) setDVCLevel(h, kvp.Value);
                }
            }
            else
            {
                ApplyDesktopVibranceToTarget(_vibranceInfo.userVibranceSettingDefault);
            }
        }

        // ── Polling backup ────────────────────────────────────────────────────
        private static void PollAndApply()
        {
            if (!_vibranceInfo.isInitialized) return;
            try
            {
                IntPtr hwnd = GetForegroundWindow();
                if (hwnd == IntPtr.Zero) return;

                GetWindowThreadProcessId(hwnd, out uint pid);

                string processName;
                try { using (var p = Process.GetProcessById((int)pid)) processName = p.ProcessName; }
                catch { return; }

                var match = _applicationSettings?.FirstOrDefault(s => MatchesProfile(s, processName));

                if (match != null)
                {
                    if (_lastProfileApplied == processName) return;
                    _lastProfileApplied = processName;
                    int handle = GetBestDisplayHandle(hwnd);
                    setDVCLevel(handle, match.IngameLevel);
                }
                else
                {
                    // Only restore when transitioning OUT of a profile — not on every desktop focus event
                    if (_lastProfileApplied == null) return;
                    _lastProfileApplied = null;
                    RestoreDesktopVibrance();
                }
            }
            catch { }
        }

        // ── WinEvent handler ──────────────────────────────────────────────────
        private static void OnWinEventHook(object sender, WinEventHookEventArgs e)
        {
            var match = _applicationSettings?.FirstOrDefault(x => MatchesProfile(x, e.ProcessName));

            if (match != null)
            {
                _lastProfileApplied = e.ProcessName;

                Screen screen = Screen.FromHandle(e.Handle);

                if (!_vibranceInfo.neverChangeResolution
                    && match.IsResolutionChangeNeeded
                    && IsResolutionChangeNeeded(screen, match.ResolutionSettings)
                    && _windowsResolutionSettings.ContainsKey(screen.DeviceName)
                    && _windowsResolutionSettings[screen.DeviceName].Item2.Contains(match.ResolutionSettings))
                {
                    _gameScreen = screen;
                    ResolutionHelper.ChangeResolutionEx(match.ResolutionSettings, screen.DeviceName);
                }

                int handle = GetBestDisplayHandle(e.Handle);
                _vibranceInfo.defaultHandle = handle;
                setDVCLevel(handle, match.IngameLevel);
            }
            else
            {
                // Only restore when transitioning OUT of a game profile.
                // If no profile was ever active (_lastProfileApplied == null) we are in normal
                // desktop usage — do NOT touch the vibrance so switching between programs has
                // no effect on the user's chosen desktop vibrance level.
                if (_lastProfileApplied == null) return;
                _lastProfileApplied = null;

                // Restore resolution if the game had changed it
                if (!_vibranceInfo.neverChangeResolution && _gameScreen != null)
                {
                    Screen current = Screen.FromHandle(e.Handle);
                    if (_gameScreen.Equals(current)
                        && _windowsResolutionSettings.ContainsKey(current.DeviceName)
                        && IsResolutionChangeNeeded(current, _windowsResolutionSettings[current.DeviceName].Item1))
                    {
                        ResolutionHelper.ChangeResolutionEx(_windowsResolutionSettings[current.DeviceName].Item1, current.DeviceName);
                    }
                }

                RestoreDesktopVibrance();
            }
        }

        // Returns the NVIDIA display handle for the screen containing hWnd,
        // falling back to defaultHandle so vibrance is always applied.
        private static int GetBestDisplayHandle(IntPtr hWnd)
        {
            if (hWnd != IntPtr.Zero)
            {
                Screen screen = Screen.FromHandle(hWnd);
                if (screen != null)
                {
                    string device = screen.DeviceName;
                    int id = getAssociatedNvidiaDisplayHandle(device, device.Length);
                    if (id != -1) return id;
                }
            }
            return _vibranceInfo.defaultHandle;
        }

        private static bool IsResolutionChangeNeeded(Screen screen, ResolutionModeWrapper settings)
            => settings != null
            && ResolutionHelper.GetCurrentResolutionSettings(out Devmode mode, screen.DeviceName)
            && !settings.Equals(mode);

        private void EnumerateDisplayHandles()
        {
            for (int i = 0; ; i++)
            {
                if (_vibranceInfo.displayHandles == null)
                    _vibranceInfo.displayHandles = new List<int>();
                int handle = enumerateNvidiaDisplayHandle(i);
                if (handle == -1) break;
                _vibranceInfo.displayHandles.Add(handle);
            }
        }

        // ── IVibranceProxy ────────────────────────────────────────────────────
        public void SetApplicationSettings(List<ApplicationSetting> refApplicationSettings)
        {
            _applicationSettings = refApplicationSettings;
            _lastProfileApplied  = null;
        }

        public void SetShouldRun(bool shouldRun)         { _vibranceInfo.shouldRun = shouldRun; }
        public void SetNeverSwitchResolution(bool never) { _vibranceInfo.neverChangeResolution = never; }
        public void SetVibranceIngameLevel(int level)    { _vibranceInfo.userVibranceSettingActive = level; }
        public void HandleDvc()                          { }
        public void SetSleepInterval(int interval)       { _vibranceInfo.sleepInterval = interval; }
        public GraphicsAdapter GraphicsAdapter { get; }  = GraphicsAdapter.Nvidia;

        public void SetVibranceWindowsLevel(int level)
        {
            _vibranceInfo.userVibranceSettingDefault = level;
            _monitorDesktopLevels.Clear(); // single-level mode: clear per-monitor overrides
            if (_vibranceInfo.isInitialized)
                ApplyDesktopVibranceToTarget(level);
        }

        // Applies a level to a specific monitor and records it so RestoreDesktopVibrance
        // can bring that exact level back after a game session.
        public void SetVibranceForMonitor(string deviceName, int level)
        {
            if (string.IsNullOrEmpty(deviceName)) return;

            // Track per-monitor desktop level for accurate restoration
            _monitorDesktopLevels[deviceName] = level;

            // Keep userVibranceSettingDefault in sync with the primary monitor so single-level
            // fallbacks (e.g. tray presets) use a sensible value.
            Screen primary = Screen.PrimaryScreen;
            if (primary != null && string.Equals(deviceName, primary.DeviceName, StringComparison.OrdinalIgnoreCase))
                _vibranceInfo.userVibranceSettingDefault = level;
            else if (_vibranceInfo.userVibranceSettingDefault == NvapiDefaultLevel)
                _vibranceInfo.userVibranceSettingDefault = level;

            if (!_vibranceInfo.isInitialized) return;
            int h = getAssociatedNvidiaDisplayHandle(deviceName, deviceName.Length);
            if (h == -1) h = _vibranceInfo.defaultHandle;
            if (h != -1) setDVCLevel(h, level);
        }

        public void SetAffectPrimaryMonitorOnly(bool primary)
        {
            _vibranceInfo.affectPrimaryMonitorOnly = primary;
            _vibranceInfo.targetMonitorDeviceName  = primary ? "PRIMARY" : null;
            if (_vibranceInfo.isInitialized)
                ApplyDesktopVibranceToTarget(_vibranceInfo.userVibranceSettingDefault);
        }

        public void SetTargetMonitorDeviceName(string deviceName)
        {
            _vibranceInfo.targetMonitorDeviceName  = deviceName;
            _vibranceInfo.affectPrimaryMonitorOnly = (deviceName == "PRIMARY");
            if (_vibranceInfo.isInitialized)
                ApplyDesktopVibranceToTarget(_vibranceInfo.userVibranceSettingDefault);
        }

        public VibranceInfo GetVibranceInfo() => _vibranceInfo;

        public bool UnloadLibraryEx()
        {
            _pollTimer?.Dispose();
            _pollTimer = null;
            _hook?.RemoveWinEventHook();
            return unloadLibrary();
        }

        public void HandleDvcExit()
        {
            RestoreDesktopVibrance();
        }

        // Applies a single vibrance level to the configured monitor target.
        // Used for single-level operations (presets, schedule, tray menu).
        private static void ApplyDesktopVibranceToTarget(int level)
        {
            string target = _vibranceInfo.targetMonitorDeviceName;

            if (target == null)
            {
                _vibranceInfo.displayHandles?.ForEach(h => setDVCLevel(h, level));
                return;
            }

            int targetHandle = (target == "PRIMARY")
                ? _vibranceInfo.defaultHandle
                : getAssociatedNvidiaDisplayHandle(target, target.Length);

            _vibranceInfo.displayHandles?.ForEach(h =>
            {
                int desired = (h == targetHandle) ? level : NvapiDefaultLevel;
                setDVCLevel(h, desired);
            });

            if (_vibranceInfo.displayHandles == null && targetHandle != -1)
                setDVCLevel(targetHandle, level);
        }
    }
}
