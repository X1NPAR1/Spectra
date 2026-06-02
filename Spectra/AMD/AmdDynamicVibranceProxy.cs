using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Spectra.AMD.vendor;
using Spectra.common;
using Spectra.NVIDIA;

namespace Spectra.AMD
{
    public class AmdDynamicVibranceProxy : IVibranceProxy
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        private readonly IAmdAdapter _amdAdapter;
        private List<ApplicationSetting> _applicationSettings;
        private readonly Dictionary<string, Tuple<ResolutionModeWrapper, List<ResolutionModeWrapper>>> _windowsResolutionSettings;
        private VibranceInfo _vibranceInfo;
        private WinEventHook _hook;
        private Screen _gameScreen;

        private readonly ConcurrentDictionary<string, int> _monitorDesktopLevels
            = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        private bool _defaultLevelSet;
        private int  _currentDisplayedLevel;
        private volatile string _lastProfileApplied;
        private System.Threading.Timer _pollTimer;
        private LevelAnimator _animator;

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
                _currentDisplayedLevel = AmdNeutralSaturation;

                if (amdAdapter.IsAvailable())
                {
                    _vibranceInfo.isInitialized = true;
                    amdAdapter.Init();
                }

                if (_vibranceInfo.isInitialized)
                {
                    _animator = new LevelAnimator(ApplyLevelForAnimation);
                    _hook = WinEventHook.GetInstance();
                    _hook.WinEventHookHandler += OnWinEventHook;
                    _pollTimer = new System.Threading.Timer(_ => PollAndApply(), null, 0, 500);
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

        private static bool MatchesProfile(ApplicationSetting s, string processName)
            => string.Equals(Path.GetFileNameWithoutExtension(s.FileName), processName, StringComparison.OrdinalIgnoreCase)
            || string.Equals(s.Name, processName, StringComparison.OrdinalIgnoreCase);

        private void ApplyLevelForAnimation(int level)
        {
            _currentDisplayedLevel = level;
            _amdAdapter.SetSaturationOnAllDisplays(level);
        }

        private void RestoreDesktopVibrance()
        {
            if (!_monitorDesktopLevels.IsEmpty)
                foreach (var kvp in _monitorDesktopLevels)
                    _amdAdapter.SetSaturationOnDisplay(kvp.Value, kvp.Key);
            else
                ApplyVibranceToTarget(_vibranceInfo.userVibranceSettingDefault);
        }

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
                    if (_animator != null && _animator.Enabled)
                        _animator.AnimateTo(_currentDisplayedLevel, match.IngameLevel);
                    else
                    { _currentDisplayedLevel = match.IngameLevel; ApplyVibranceToTarget(match.IngameLevel); }
                }
                else
                {
                    if (_lastProfileApplied == null) return;
                    _lastProfileApplied = null;
                    if (_animator != null && _animator.Enabled)
                        _animator.AnimateTo(_currentDisplayedLevel, _vibranceInfo.userVibranceSettingDefault);
                    else
                        RestoreDesktopVibrance();
                }
            }
            catch { }
        }

        private void OnWinEventHook(object sender, WinEventHookEventArgs e)
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

                if (_animator != null && _animator.Enabled)
                    _animator.AnimateTo(_currentDisplayedLevel, match.IngameLevel);
                else
                { _currentDisplayedLevel = match.IngameLevel; ApplyVibranceToTarget(match.IngameLevel); }
            }
            else
            {
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

                if (_animator != null && _animator.Enabled)
                    _animator.AnimateTo(_currentDisplayedLevel, _vibranceInfo.userVibranceSettingDefault);
                else
                    RestoreDesktopVibrance();
            }
        }

        private static bool IsResolutionChangeNeeded(Screen screen, ResolutionModeWrapper settings)
            => settings != null
            && ResolutionHelper.GetCurrentResolutionSettings(out Devmode mode, screen.DeviceName)
            && !settings.Equals(mode);

        public void SetApplicationSettings(List<ApplicationSetting> refApplicationSettings)
        {
            _applicationSettings = refApplicationSettings;
            _lastProfileApplied  = null;
        }

        public void SetShouldRun(bool shouldRun)         { _vibranceInfo.shouldRun = shouldRun; }
        public void SetNeverSwitchResolution(bool never) { _vibranceInfo.neverChangeResolution = never; }
        public void SetVibranceIngameLevel(int level)    { _vibranceInfo.userVibranceSettingActive = level; }
        public GraphicsAdapter GraphicsAdapter { get; }  = GraphicsAdapter.Amd;

        public void SetTransitionEnabled(bool enabled)    { if (_animator != null) _animator.Enabled  = enabled; }
        public void SetTransitionDuration(int durationMs) { if (_animator != null) _animator.Duration = durationMs; }

        public void SetVibranceWindowsLevel(int level)
        {
            _vibranceInfo.userVibranceSettingDefault = level;
            _currentDisplayedLevel = level;
            _defaultLevelSet = true;
            _monitorDesktopLevels.Clear();
            if (_vibranceInfo.isInitialized)
                ApplyVibranceToTarget(level);
        }

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
                _currentDisplayedLevel = level;
                _defaultLevelSet = true;
            }

            if (_vibranceInfo.isInitialized)
                _amdAdapter.SetSaturationOnDisplay(level, deviceName);
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

        public VibranceInfo GetVibranceInfo() => _vibranceInfo;

        public bool UnloadLibraryEx()
        {
            _pollTimer?.Dispose();
            _pollTimer = null;
            _animator?.Dispose();
            _animator = null;
            _hook?.RemoveWinEventHook();
            return true;
        }

        public void HandleDvcExit()
        {
            _amdAdapter.SetSaturationOnAllDisplays(_vibranceInfo.userVibranceSettingDefault);
        }

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
