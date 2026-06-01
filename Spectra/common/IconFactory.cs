using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace Spectra.common
{
    public static class IconFactory
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr handle);

        private static Icon _cachedLarge;
        private static Icon _cachedSmall;

        public static Icon GetAppIcon(int size = 32)
        {
            if (size <= 16) return _cachedSmall ?? (_cachedSmall = CreateIcon(16));
            return _cachedLarge ?? (_cachedLarge = CreateIcon(32));
        }

        private static Icon CreateIcon(int size)
        {
            using (var bmp = new Bitmap(size, size))
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode      = SmoothingMode.AntiAlias;
                g.TextRenderingHint  = TextRenderingHint.AntiAlias;
                g.InterpolationMode  = InterpolationMode.HighQualityBicubic;

                // Background — deep space navy
                using (var bg = new SolidBrush(Color.FromArgb(13, 17, 23)))
                    g.FillRectangle(bg, 0, 0, size, size);

                // Rounded rect background
                int pad = Math.Max(1, size / 16);
                var rect = new Rectangle(pad, pad, size - pad * 2, size - pad * 2);
                using (var path = RoundedRect(rect, Math.Max(2, size / 8)))
                using (var grad = new LinearGradientBrush(rect,
                    Color.FromArgb(123, 47, 247),
                    Color.FromArgb(0, 210, 255),
                    LinearGradientMode.ForwardDiagonal))
                {
                    g.FillPath(grad, path);
                }

                // "S" lettermark
                float fontSize = size * 0.52f;
                using (var font = new Font("Segoe UI", fontSize, FontStyle.Bold, GraphicsUnit.Pixel))
                using (var brush = new SolidBrush(Color.FromArgb(240, 248, 255)))
                {
                    var sf = new StringFormat
                    {
                        Alignment     = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString("S", font, brush, new RectangleF(0, 0, size, size), sf);
                }

                IntPtr hIcon = bmp.GetHicon();
                Icon icon = (Icon)Icon.FromHandle(hIcon).Clone();
                DestroyIcon(hIcon);
                return icon;
            }
        }

        private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int d = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
