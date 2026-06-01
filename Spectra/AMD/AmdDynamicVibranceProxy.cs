using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Spectra.AMD.vendor;
using Spectra.common;
using Spectra.NVIDIA;

namespace Spectra.AMD
{
    public class AmdDynamicVibranceProxy : IVibranceProxy
    {
        private readonly IAmdAdapter _amdAdapter;
        private List<ApplicationSetting> _applicationSettings;
        private readonly Dictionary<string, Tuple<ResolutionModeWrapper, List<ResolutionModeWrapper>>> _windowsResolutionSettings;
        private VibranceInfo _vibranceInfo;
        private WinEventHook _hook;
        private static Screen _gameScreen;

        public AmdDynamicVibranceProxy(IAmdAdapter amdAdapter, List<ApplicationSetting> applicationSettings, Dictionary<string, Tuple<ResolutionModeWrapper, List<ResolutionModeWrapper>>> windowsResolutionSettings)
        {
            _amdAdapter = amdAdapter;
            _applicationSettings = applicationSettings;
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
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                DialogResult result = MessageBox.Show(NvidiaDynamicVibranceProxy.NvapiErrorInitFailed, "vibranceGUI Error",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                {
                    Process.Start(NvidiaDynamicVibranceProxy.GuideLink);
                }
            }
        }

        public void SetApplicationSettings(List<ApplicationSetting> refApplicationSettings)
        {
            _applicationSettings = refApplicationSettings;
        }

        public void SetShouldRun(bool shouldRun)
        {
            _vibranceInfo.shouldRun = shouldRun;
        }

        public void SetNeverSwitchResolution(bool neverChangeResolution)
        {
            _vibranceInfo.neverChangeResolution = neverChangeResolution;
        }

        public void SetVibranceWindowsLevel(int vibranceWindowsLevel)
        {
            _vibranceInfo.userVibranceSettingDefault = vibranceWindowsLevel;
        }

        public void SetVibranceIngameLevel(int vibranceIngameLevel)
        {
            _vibranceInfo.userVibranceSettingActive = vibranceIngameLevel;
        }

        public bool UnloadLibraryEx()
        {
            _hook.RemoveWinEventHook();
            return true;
        }

        public void HandleDvcExit()
        {
            _amdAdapter.SetSaturationOnAllDisplays(_vibranceInfo.userVibranceSettingDefault);
        }

        public void SetAffectPrimaryMonitorOnly(bool affectPrimaryMonitorOnly)
        {
            _vibranceInfo.affectPrimaryMonitorOnly = affectPrimaryMonitorOnly;
            _vibranceInfo.targetMonitorDeviceName  = affectPrimaryMonitorOnly ? "PRIMARY" : null;
        }

        public void SetTargetMonitorDeviceName(string deviceName)
        {
            _vibranceInfo.targetMonitorDeviceName  = deviceName;
            _vibranceInfo.affectPrimaryMonitorOnly = (deviceName == "PRIMARY");
        }

        public VibranceInfo GetVibranceInfo()
        {
            return _vibranceInfo;
        }

        public GraphicsAdapter GraphicsAdapter { get; } = GraphicsAdapter.Amd;

        // Applies the given saturation level to the user-selected monitor target.
        private void ApplyVibranceToTarget(int level)
        {
            string target = _vibranceInfo.targetMonitorDeviceName;
            if (target == null)
            {
                _amdAdapter.SetSaturationOnAllDisplays(level);
            }
            else if (target == "PRIMARY")
            {
                string primary = System.Windows.Forms.Screen.PrimaryScreen?.DeviceName;
                if (primary != null) _amdAdapter.SetSaturationOnDisplay(level, primary);
            }
            else
            {
                _amdAdapter.SetSaturationOnDisplay(level, target);
            }
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        private void OnWinEventHook(object sender, WinEventHookEventArgs e)
        {
            if (_applicationSettings.Count > 0)
            {
                ApplicationSetting applicationSetting = _applicationSettings.FirstOrDefault(x => string.Equals(x.Name, e.ProcessName, StringComparison.OrdinalIgnoreCase));
                if (applicationSetting != null)
                {
                    //test if a resolution change is needed
                    Screen screen = Screen.FromHandle(e.Handle);
                    if (_vibranceInfo.neverChangeResolution == false && 
                        applicationSetting.IsResolutionChangeNeeded && 
                        IsResolutionChangeNeeded(screen, applicationSetting.ResolutionSettings) &&
                        _windowsResolutionSettings.ContainsKey(screen.DeviceName) &&
                        _windowsResolutionSettings[screen.DeviceName].Item2.Contains(applicationSetting.ResolutionSettings))
                    {
                        _gameScreen = screen;
                        PerformResolutionChange(screen, applicationSetting.ResolutionSettings);
                    }

                    // Reset all to desktop level, then apply ingame level to target monitor(s)
                    _amdAdapter.SetSaturationOnAllDisplays(_vibranceInfo.userVibranceSettingDefault);
                    ApplyVibranceToTarget(applicationSetting.IngameLevel);
                }
                else
                {
                    IntPtr processHandle = e.Handle;
                    if (GetForegroundWindow() != processHandle)
                        return;

                    //test if a resolution change is needed
                    Screen screen = Screen.FromHandle(processHandle);
                    if (_vibranceInfo.neverChangeResolution == false &&
                        _gameScreen != null && _gameScreen.Equals(screen) &&
                        _windowsResolutionSettings.ContainsKey(screen.DeviceName) &&
                        IsResolutionChangeNeeded(screen, _windowsResolutionSettings[screen.DeviceName].Item1))
                    {
                        PerformResolutionChange(screen, _windowsResolutionSettings[screen.DeviceName].Item1);
                    }

                    // Restore desktop vibrance to the selected target monitor(s)
                    ApplyVibranceToTarget(_vibranceInfo.userVibranceSettingDefault);
                }
            }
        }

        private static bool IsResolutionChangeNeeded(Screen screen, ResolutionModeWrapper resolutionSettings)
        {
            Devmode mode;
            if (resolutionSettings != null && ResolutionHelper.GetCurrentResolutionSettings(out mode, screen.DeviceName) && !resolutionSettings.Equals(mode))
            {
                return true;
            }
            return false;
        }

        private static void PerformResolutionChange(Screen screen, ResolutionModeWrapper resolutionSettings)
        {
            ResolutionHelper.ChangeResolutionEx(resolutionSettings, screen.DeviceName);
        }
    }
}
