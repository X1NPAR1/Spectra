using System;
using System.Drawing;
using System.Windows.Forms;

namespace Spectra.common
{
    public static class IconFactory
    {
        private static Icon _source;

        public static Icon GetAppIcon(int size = 32)
        {
            EnsureLoaded();
            try   { return new Icon(_source, size, size); }
            catch { return new Icon(_source, new Size(size, size)); }
        }

        public static Bitmap GetAppBitmap(int size = 32)
        {
            using (var icon = GetAppIcon(size))
                return icon.ToBitmap();
        }

        private static void EnsureLoaded()
        {
            if (_source != null) return;
            try   { _source = Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
            catch { _source = SystemIcons.Application; }
        }
    }
}
