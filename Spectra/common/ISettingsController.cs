using System.Collections.Generic;
using System.Windows.Forms;
using Spectra.NVIDIA;

namespace Spectra.common
{
    internal interface ISettingsController
    {
        bool SetVibranceSettings(string windowsLevel, string affectPrimaryMonitorOnly, string neverSwitchResolution,
            List<ApplicationSetting> applicationSettings, Keys hotkey, Keys hotkeyModifiers);
        bool SetVibranceSetting(string szKeyName, string value);
        void ReadVibranceSettings(GraphicsAdapter graphicsAdapter,
            out int vibranceWindowsLevel, out bool affectPrimaryMonitorOnly,
            out bool neverSwitchResolution, out List<ApplicationSetting> applicationSettings,
            out Keys hotkey, out Keys hotkeyModifiers);
    }
}
