using System;
using Microsoft.Win32;

namespace Spectra.common
{
    class RegistryController : IRegistryController
    {
        private const string RunKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        public bool RegisterProgram(string appName, string pathToExe)
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(RunKey, writable: true))
                {
                    if (key == null) return false;
                    key.SetValue(appName, pathToExe);
                    return true;
                }
            }
            catch { return false; }
        }

        public bool UnregisterProgram(string appName)
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(RunKey, writable: true))
                {
                    if (key == null) return false;
                    key.DeleteValue(appName, throwOnMissingValue: false);
                    return true;
                }
            }
            catch { return false; }
        }

        public bool IsProgramRegistered(string appName)
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(RunKey, writable: false))
                    return key?.GetValue(appName) != null;
            }
            catch { return false; }
        }

        public bool IsStartupPathUnchanged(string appName, string pathToExe)
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(RunKey, writable: false))
                {
                    if (key == null) return false;
                    var stored = key.GetValue(appName)?.ToString();
                    return string.Equals(stored, pathToExe, StringComparison.OrdinalIgnoreCase);
                }
            }
            catch { return false; }
        }
    }
}
