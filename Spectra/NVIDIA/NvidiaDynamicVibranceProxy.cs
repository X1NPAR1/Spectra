using System;
using System.Collections.Concurrent;
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

        // ConcurrentDictionary for thread-safe access from UI thread (SetVibranceForMonitor)
        // and the timer thread pool (PollAndApply / RestoreDesktopVibrance).
        private static readonly ConcurrentDictionary<string, int> _monitorDesktopLevels
            = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        private static bool _defaultLevelSet;

        // null = desktop (no active game profile), non-null = active game process name.
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
                _defaultLevelSet           = false;

                if (initializeLibrary())
                    InitializeProxy();

                if (_vibranceInfo.isInitialized)
                {
                    _hook = WinEventHook.GetInstance();
                    _hook.WinEventHookHandler += OnWinEventHook;

                    // Start polling immediately (0ms) so a game that is already in focus
                    // when Spectra launches is detected on the very first tick.
                    _pollTimer = new System.Threading.Timer(_ => PollAndApply(), null, 0, 500);
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
                if (gpuHandle != 0 && getGpuSystemType(gpuHandle) == NvSystemType.NvSystemTypeUnknown)
                {
                    MessageBox.Show("VibranceProxy failed to initialize — GPU system type unknown.",
                        "Spectra Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _vibranceInfo.isInitialized = false;
                    return;
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

        // ── Profile matching ──────────────────────────────────────────────────
        // Primary key: executable filename (always reliable).
        // Fallback: profile display-name (for manually renamed entries).
        private static bool MatchesProfile(ApplicationSetting s, string processName)
            => string.Equals(Path.GetFileNameWithoutExtension(s.FileName), processName, StringComparison.OrdinalIgnoreCase)
            || string.Equals(s.Name, processName, StringComparison.OrdinalIgnoreCase);

        // ── Desktop restore ───────────────────────────────────────────────────
        // Restores each monitor to its individual user-chosen desktop level.
        // Falls back to the single userVibranceSettingDefault when per-monitor
        // data isn't available (single-monitor mode or SetVibranceWindowsLevel path).
        private static void RestoreDesktopVibrance()
        {
            if (!_monitorDesktopLevels.IsEmpty)
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
                // Guard: only restore when transitioning OUT of a game profile.
                // Normal program switching (browser, taskbar…) must NOT affect desktop vibrance.
                if (_lastProfileApplied == null) return;
                _lastProfileApplied = null;

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
        public GraphicsAdapter GraphicsAdapter { get; }  = GraphicsAdapter.Nvidia;

        public void SetVibranceWindowsLevel(int level)
        {
            _vibranceInfo.userVibranceSettingDefault = level;
            _defaultLevelSet = true;
            _monitorDesktopLevels.Clear();
            if (_vibranceInfo.isInitialized)
                ApplyDesktopVibranceToTarget(level);
        }

        // Records the per-monitor desktop level for accurate post-game restoration.
        // Also keeps userVibranceSettingDefault in sync with the primary monitor so that
        // tray presets and the schedule apply a sensible single-level fallback.
        public void SetVibranceForMonitor(string deviceName, int level)
        {
            if (string.IsNullOrEmpty(deviceName)) return;

            _monitorDesktopLevels[deviceName] = level;

            Screen primary = Screen.PrimaryScreen;
            bool isPrimary = primary != null &&
                string.Equals(deviceName, primary.DeviceName, StringComparison.OrdinalIgnoreCase);

            if (isPrimary || !_defaultLevelSet)
            {
                _vibranceInfo.userVibranceSettingDefault = level;
                _defaultLevelSet = true;
            }

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

        public void HandleDvcExit() => RestoreDesktopVibrance();

        // Applies a single level to the configured monitor target (used by presets, schedule).
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
                setDVCLevel(h, h == targetHandle ? level : NvapiDefaultLevel));

            if (_vibranceInfo.displayHandles == null && targetHandle != -1)
                setDVCLevel(targetHandle, level);
        }
    }
}
