using System;
using Microsoft.Win32;

namespace Spectra.common
{
    class RegistryController : IRegistryController
    {
        private const string RunKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        // Every method opens the key inside a using block so:
        //   1. The handle is always closed, even on exception paths.
        //   2. We never dereference a null key — a null check gates each operation.
        // The previous implementation stored the key as a field and called
        // _startupKey.Close() in a finally block; when OpenSubKey returned null,
        // the finally triggered NullReferenceException.

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
                    // throwOnMissingValue: false — silently succeeds when the value is absent
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
