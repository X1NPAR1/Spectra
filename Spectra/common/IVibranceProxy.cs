using System.Collections.Generic;
using Spectra.NVIDIA;

namespace Spectra.common
{
    public interface IVibranceProxy
    {
        void SetApplicationSettings(List<ApplicationSetting> refApplicationSettings);
        void SetShouldRun(bool shouldRun);
        void SetVibranceWindowsLevel(int vibranceWindowsLevel);
        void SetVibranceIngameLevel(int vibranceIngameLevel);
        bool UnloadLibraryEx();
        void HandleDvcExit();
        void SetAffectPrimaryMonitorOnly(bool affectPrimaryMonitorOnly);
        // deviceName: null=all, "PRIMARY"=primary only, device path=specific monitor
        void SetTargetMonitorDeviceName(string deviceName);
        // Applies a vibrance level to one specific monitor (per-monitor independent control).
        void SetVibranceForMonitor(string deviceName, int level);
        VibranceInfo GetVibranceInfo();
        GraphicsAdapter GraphicsAdapter { get; }
        void SetNeverSwitchResolution(bool neverSwitchResolution);
    }
}
