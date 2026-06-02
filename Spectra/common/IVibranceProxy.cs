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
        void SetTargetMonitorDeviceName(string deviceName);
        void SetVibranceForMonitor(string deviceName, int level);
        VibranceInfo GetVibranceInfo();
        GraphicsAdapter GraphicsAdapter { get; }
        void SetNeverSwitchResolution(bool neverSwitchResolution);
        void SetTransitionEnabled(bool enabled);
        void SetTransitionDuration(int durationMs);
    }
}
