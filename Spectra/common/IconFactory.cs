using System;
using System.Drawing;
using System.Windows.Forms;

namespace Spectra.common
{
    // Loads the application icon that is compiled into the exe as ApplicationIcon (setting.ico).
    // All forms and UI elements use this single source of truth — no generated graphics.
    public static class IconFactory
    {
        private static Icon _source;  // the raw icon extracted from the exe

        // Returns a sized copy of the application icon.
        // size: target size in pixels (typically 16 or 32).
        public static Icon GetAppIcon(int size = 32)
        {
            EnsureLoaded();
            try
            {
                return new Icon(_source, size, size);
            }
            catch
            {
                return new Icon(_source, new Size(size, size));
            }
        }

        // Returns a Bitmap version of the icon for use in PictureBox / Graphics.DrawImage.
        public static Bitmap GetAppBitmap(int size = 32)
        {
            using (var icon = GetAppIcon(size))
                return icon.ToBitmap();
        }

        private static void EnsureLoaded()
        {
            if (_source != null) return;
            try
            {
                // ExtractAssociatedIcon reads the Win32 icon resource embedded by the compiler
                // from the ApplicationIcon setting in the csproj (setting.ico).
                _source = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            }
            catch
            {
                _source = SystemIcons.Application;
            }
        }
    }
}
