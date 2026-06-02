using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Serialization;
using Spectra.NVIDIA;

namespace Spectra.common
{
    class SettingsController : ISettingsController
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern uint GetPrivateProfileString(
            string lpAppName, string lpKeyName, string lpDefault,
            StringBuilder lpReturnedString, uint nSize, string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern bool WritePrivateProfileString(
            string lpAppName, string lpKeyName, string lpString, string lpFileName);

        private const string SzSectionName                    = "Settings";
        private const string SzKeyNameInactive                = "inactiveValue";
        private const string SzKeyNameAffectPrimaryMonitorOnly = "affectPrimaryMonitorOnly";
        private const string SzKeyNameNeverSwitchResolution   = "neverSwitchResolution";
        private const string SzKeyNameHotkey                  = "hotkey";
        private const string SzKeyNameHotkeyModifiers         = "hotkeyModifiers";

        private static readonly string AppDataDir =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Spectra");

        private readonly string _fileName =
            Path.Combine(AppDataDir, "Spectra.ini");
        private readonly string _fileNameApplicationSettings =
            Path.Combine(AppDataDir, "applicationData.xml");

        public bool SetVibranceSettings(string windowsLevel, string affectPrimaryMonitorOnly,
            string neverSwitchResolution, List<ApplicationSetting> applicationSettings,
            System.Windows.Forms.Keys hotkey          = System.Windows.Forms.Keys.F9,
            System.Windows.Forms.Keys hotkeyModifiers = System.Windows.Forms.Keys.None)
        {
            if (!PrepareFile()) return false;

            WritePrivateProfileString(SzSectionName, SzKeyNameInactive,                windowsLevel,             _fileName);
            WritePrivateProfileString(SzSectionName, SzKeyNameAffectPrimaryMonitorOnly, affectPrimaryMonitorOnly, _fileName);
            WritePrivateProfileString(SzSectionName, SzKeyNameNeverSwitchResolution,   neverSwitchResolution,    _fileName);
            WritePrivateProfileString(SzSectionName, SzKeyNameHotkey,                  ((int)hotkey).ToString(),          _fileName);
            WritePrivateProfileString(SzSectionName, SzKeyNameHotkeyModifiers,         ((int)hotkeyModifiers).ToString(), _fileName);

            try
            {
                Directory.CreateDirectory(AppDataDir);
                using (var writer = System.Xml.XmlWriter.Create(_fileNameApplicationSettings))
                {
                    var serializer = new XmlSerializer(typeof(List<ApplicationSetting>));
                    serializer.Serialize(writer, applicationSettings);
                }
            }
            catch { return false; }

            return true;
        }

        public bool SetVibranceSetting(string szKeyName, string value)
        {
            if (!PrepareFile()) return false;
            WritePrivateProfileString(SzSectionName, szKeyName, value, _fileName);
            return true;
        }

        public string GetSetting(string szKeyName, string szDefault)
        {
            if (!File.Exists(_fileName)) return szDefault;
            return ReadIni(szKeyName, szDefault);
        }

        public void ReadVibranceSettings(GraphicsAdapter graphicsAdapter,
            out int vibranceWindowsLevel, out bool affectPrimaryMonitorOnly,
            out bool neverSwitchResolution, out List<ApplicationSetting> applicationSettings,
            out System.Windows.Forms.Keys hotkey, out System.Windows.Forms.Keys hotkeyModifiers)
        {
            int defaultLevel = 0, maxLevel = 0;
            if (graphicsAdapter == GraphicsAdapter.Nvidia)
                { defaultLevel = NvidiaDynamicVibranceProxy.NvapiDefaultLevel; maxLevel = NvidiaDynamicVibranceProxy.NvapiMaxLevel; }
            else if (graphicsAdapter == GraphicsAdapter.Amd)
                { defaultLevel = 100; maxLevel = 300; }

            vibranceWindowsLevel     = defaultLevel;
            affectPrimaryMonitorOnly = false;
            neverSwitchResolution    = false;
            hotkey                   = System.Windows.Forms.Keys.F9;
            hotkeyModifiers          = System.Windows.Forms.Keys.None;

            if (File.Exists(_fileName))
            {
                if (int.TryParse(ReadIni(SzKeyNameInactive, defaultLevel.ToString()), out int parsedLevel))
                    if (parsedLevel >= defaultLevel && parsedLevel <= maxLevel)
                        vibranceWindowsLevel = parsedLevel;

                affectPrimaryMonitorOnly = ReadIni(SzKeyNameAffectPrimaryMonitorOnly, "false")
                    .Equals("true", StringComparison.OrdinalIgnoreCase);
                neverSwitchResolution = ReadIni(SzKeyNameNeverSwitchResolution, "false")
                    .Equals("true", StringComparison.OrdinalIgnoreCase);

                if (int.TryParse(ReadIni(SzKeyNameHotkey, ((int)System.Windows.Forms.Keys.F9).ToString()), out int hkInt))
                    hotkey = (System.Windows.Forms.Keys)hkInt;
                if (int.TryParse(ReadIni(SzKeyNameHotkeyModifiers, "0"), out int hkModsInt))
                    hotkeyModifiers = (System.Windows.Forms.Keys)hkModsInt;
            }

            applicationSettings = new List<ApplicationSetting>();
            if (!File.Exists(_fileNameApplicationSettings)) return;

            try
            {
                using (var reader = System.Xml.XmlReader.Create(_fileNameApplicationSettings))
                {
                    var serializer = new XmlSerializer(typeof(List<ApplicationSetting>));
                    applicationSettings = (List<ApplicationSetting>)serializer.Deserialize(reader);
                }
            }
            catch { applicationSettings = new List<ApplicationSetting>(); }
        }

        private string ReadIni(string key, string def)
        {
            var sb = new StringBuilder(1024);
            GetPrivateProfileString(SzSectionName, key, def, sb, (uint)sb.Capacity, _fileName);
            return sb.ToString();
        }

        private bool PrepareFile()
        {
            if (File.Exists(_fileName)) return true;
            try
            {
                Directory.CreateDirectory(AppDataDir);
                MigrateFromLegacy();
                File.WriteAllText(_fileName, "");
                return File.Exists(_fileName);
            }
            catch { return false; }
        }

        private void MigrateFromLegacy()
        {
            string legacyDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "vibranceGUI");

            string legacyIni = Path.Combine(legacyDir, "vibranceGUI.ini");
            string legacyXml = Path.Combine(legacyDir, "applicationData.xml");

            if (File.Exists(legacyIni) && !File.Exists(_fileName))
                File.Copy(legacyIni, _fileName, overwrite: false);

            if (File.Exists(legacyXml) && !File.Exists(_fileNameApplicationSettings))
                File.Copy(legacyXml, _fileNameApplicationSettings, overwrite: false);
        }
    }
}
