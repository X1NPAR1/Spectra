using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Spectra.AMD.vendor;
using Spectra.common;
using Spectra.NVIDIA;

namespace Spectra.AMD
{
    public class AmdDynamicVibranceProxy : IVibranceProxy
    {
        #region Win32
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        #endregion

        private readonly IAmdAdapter _amdAdapter;
        private List<ApplicationSetting> _applicationSettings;
        private readonly Dictionary<string, Tuple<ResolutionModeWrapper, List<ResolutionModeWrapper>>> _windowsResolutionSettings;
        private VibranceInfo _vibranceInfo;
        private WinEventHook _hook;
        private Screen _gameScreen;

        // Polling backup — tracks which profile (if any) is currently active
        private volatile string _lastProfileApplied;
        private System.Threading.Timer _pollTimer;

        // AMD neutral saturation (no extra colour, driver default)
        private const int AmdNeutralSaturation = 100;

        public AmdDynamicVibranceProxy(
            IAmdAdapter amdAdapter,
            List<ApplicationSetting> applicationSettings,
            Dictionary<string, Tuple<ResolutionModeWrapper, List<ResolutionModeWrapper>>> windowsResolutionSettings)
        {
            _amdAdapter                = amdAdapter;
            _applicationSettings       = applicationSettings;
            _windowsResolutionSettings = windowsResolutionSettings;

            try
            {
                _vibranceInfo = new VibranceInfo();
                if (amdAdapter.IsAvailable())
                {
                    _vibranceInfo.isInitialized = true;
                    amdAdapter.Init();
                }

                if (_vibranceInfo.isInitialized)
                {
                    _hook = WinEventHook.GetInstance();
                    _hook.WinEventHookHandler += OnWinEventHook;

                    // 500 ms polling timer as a reliable fallback for events
                    // WinEventHook may miss (launchers, fullscreen transitions…)
                    _pollTimer = new System.Threading.Timer(_ => PollAndApply(), null, 1000, 500);
                }
            }
            catch (Exception ex)
            {
                MainForm.Log(ex);
                DialogResult result = MessageBox.Show(NvidiaDynamicVibranceProxy.NvapiErrorInitFailed, "Spectra Error",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                    Process.Start(NvidiaDynamicVibranceProxy.GuideLink);
            }
        }

        // ── Profile matching ─────────────────────────────────────────────────
        // Uses the executable filename as the primary key (always reliable) and
        // the profile display-name as a fallback for manually-renamed entries.
        private static bool MatchesProfile(ApplicationSetting s, string processName)
            => string.Equals(Path.GetFileNameWithoutExtension(s.FileName), processName, StringComparison.OrdinalIgnoreCase)
            || string.Equals(s.Name, processName, StringComparison.OrdinalIgnoreCase);

        // ── Polling backup ───────────────────────────────────────────────────
        private void PollAndApply()
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
                    ApplyVibranceToTarget(match.IngameLevel);
                }
                else
                {
                    if (_lastProfileApplied == null) return;
                    _lastProfileApplied = null;
                    ApplyVibranceToTarget(_vibranceInfo.userVibranceSettingDefault);
                }
            }
            catch { }
        }

        // ── WinEvent handler ─────────────────────────────────────────────────
        private void OnWinEventHook(object sender, WinEventHookEventArgs e)
        {
            var match = _applicationSettings?.FirstOrDefault(x => MatchesProfile(x, e.ProcessName));

            if (match != null)
            {
                _lastProfileApplied = e.ProcessName;

                Screen screen = Screen.FromHandle(e.Handle);

                // Optional resolution change
                if (!_vibranceInfo.neverChangeResolution
                    && match.IsResolutionChangeNeeded
                    && IsResolutionChangeNeeded(screen, match.ResolutionSettings)
                    && _windowsResolutionSettings.ContainsKey(screen.DeviceName)
                    && _windowsResolutionSettings[screen.DeviceName].Item2.Contains(match.ResolutionSettings))
                {
                    _gameScreen = screen;
                    ResolutionHelper.ChangeResolutionEx(match.ResolutionSettings, screen.DeviceName);
                }

                // Apply ingame vibrance directly — no pre-reset, no ForegroundWindow gate
                ApplyVibranceToTarget(match.IngameLevel);
            }
            else
            {
                _lastProfileApplied = null;

                // Restore previous resolution if the game had changed it
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

                // Always restore — no GetForegroundWindow gate that could silently skip restore
                ApplyVibranceToTarget(_vibranceInfo.userVibranceSettingDefault);
            }
        }

        private static bool IsResolutionChangeNeeded(Screen screen, ResolutionModeWrapper settings)
            => settings != null
            && ResolutionHelper.GetCurrentResolutionSettings(out Devmode mode, screen.DeviceName)
            && !settings.Equals(mode);

        // ── IVibranceProxy ───────────────────────────────────────────────────
        public void SetApplicationSettings(List<ApplicationSetting> refApplicationSettings)
        {
            _applicationSettings = refApplicationSettings;
            _lastProfileApplied  = null; // re-evaluate on next event/poll
        }

        public void SetShouldRun(bool shouldRun)         { _vibranceInfo.shouldRun = shouldRun; }
        public void SetNeverSwitchResolution(bool never) { _vibranceInfo.neverChangeResolution = never; }
        public void SetVibranceIngameLevel(int level)    { _vibranceInfo.userVibranceSettingActive = level; }
        public GraphicsAdapter GraphicsAdapter { get; }  = GraphicsAdapter.Amd;

        public void SetVibranceWindowsLevel(int level)
        {
            _vibranceInfo.userVibranceSettingDefault = level;
            if (_vibranceInfo.isInitialized)
                ApplyVibranceToTarget(level);
        }

        public void SetAffectPrimaryMonitorOnly(bool primary)
        {
            _vibranceInfo.affectPrimaryMonitorOnly = primary;
            _vibranceInfo.targetMonitorDeviceName  = primary ? "PRIMARY" : null;
            if (_vibranceInfo.isInitialized)
                ApplyVibranceToTarget(_vibranceInfo.userVibranceSettingDefault);
        }

        public void SetTargetMonitorDeviceName(string deviceName)
        {
            _vibranceInfo.targetMonitorDeviceName  = deviceName;
            _vibranceInfo.affectPrimaryMonitorOnly = (deviceName == "PRIMARY");
            if (_vibranceInfo.isInitialized)
                ApplyVibranceToTarget(_vibranceInfo.userVibranceSettingDefault);
        }

        public void SetVibranceForMonitor(string deviceName, int level)
        {
            if (_vibranceInfo.isInitialized && !string.IsNullOrEmpty(deviceName))
                _amdAdapter.SetSaturationOnDisplay(level, deviceName);
        }

        public VibranceInfo GetVibranceInfo() => _vibranceInfo;

        public bool UnloadLibraryEx()
        {
            _pollTimer?.Dispose();
            _pollTimer = null;
            _hook?.RemoveWinEventHook();
            return true;
        }

        public void HandleDvcExit()
        {
            _amdAdapter.SetSaturationOnAllDisplays(_vibranceInfo.userVibranceSettingDefault);
        }

        // Applies a vibrance level to the user-selected monitor target.
        // Non-target monitors are reset to the neutral ADL saturation (100).
        private void ApplyVibranceToTarget(int level)
        {
            string target = _vibranceInfo.targetMonitorDeviceName;

            if (target == null)
            {
                _amdAdapter.SetSaturationOnAllDisplays(level);
            }
            else if (target == "PRIMARY")
            {
                _amdAdapter.SetSaturationOnAllDisplays(AmdNeutralSaturation);
                string primary = Screen.PrimaryScreen?.DeviceName;
                if (primary != null) _amdAdapter.SetSaturationOnDisplay(level, primary);
            }
            else
            {
                _amdAdapter.SetSaturationOnAllDisplays(AmdNeutralSaturation);
                _amdAdapter.SetSaturationOnDisplay(level, target);
            }
        }
    }
}
