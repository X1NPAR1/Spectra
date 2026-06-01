using System;
using System.IO;
using System.Reflection;

namespace Spectra.AMD.vendor.utils
{
    public static class CommonUtils
    {
        public static string GetAppDataPath()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Spectra");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        public static string LoadUnmanagedLibraryFromResource(Assembly assembly, string libraryResourceName, string libraryName)
        {
            using (Stream s = assembly.GetManifestResourceStream(libraryResourceName))
            {
                byte[] data = new BinaryReader(s).ReadBytes((int)s.Length);
                string tempDllPath = Path.Combine(GetAppDataPath(), libraryName);
                File.WriteAllBytes(tempDllPath, data);
            }
            NativeMethods.LoadLibrary(libraryName);
            return Path.Combine(GetAppDataPath(), libraryName);
        }
    }
}
