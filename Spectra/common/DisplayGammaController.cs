using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Spectra.common
{
    public static class DisplayGammaController
    {
        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool SetDeviceGammaRamp(IntPtr hdc, ref RAMP lpRamp);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern IntPtr CreateDC(string lpszDriver, string lpszDevice, string lpszOutput, IntPtr lpInitData);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteDC(IntPtr hdc);

        [StructLayout(LayoutKind.Sequential)]
        private struct RAMP
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)] public ushort[] Red;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)] public ushort[] Green;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)] public ushort[] Blue;
        }

        public const int Neutral = 50;

        private static int _brightness = Neutral;
        private static int _contrast   = Neutral;

        public static int Brightness => _brightness;
        public static int Contrast   => _contrast;

        public static void SetBrightness(int value) { _brightness = Clamp(value); Apply(); }
        public static void SetContrast(int value)   { _contrast   = Clamp(value); Apply(); }

        public static void Set(int brightness, int contrast)
        {
            _brightness = Clamp(brightness);
            _contrast   = Clamp(contrast);
            Apply();
        }

        public static void Reset()
        {
            _brightness = Neutral;
            _contrast   = Neutral;
            Apply();
        }

        private static int Clamp(int v) => v < 0 ? 0 : (v > 100 ? 100 : v);

        private static void Apply()
        {
            double bright   = (_brightness - Neutral) / 50.0;
            double contrast = 1.0 + (_contrast - Neutral) / 100.0;

            RAMP ramp = BuildRamp(bright, contrast);

            foreach (Screen screen in Screen.AllScreens)
            {
                IntPtr hdc = CreateDC(null, screen.DeviceName, null, IntPtr.Zero);
                if (hdc == IntPtr.Zero) continue;
                try   { SetDeviceGammaRamp(hdc, ref ramp); }
                finally { DeleteDC(hdc); }
            }
        }

        private static RAMP BuildRamp(double bright, double contrast)
        {
            var ramp = new RAMP
            {
                Red   = new ushort[256],
                Green = new ushort[256],
                Blue  = new ushort[256]
            };

            for (int i = 0; i < 256; i++)
            {
                double v = (i / 255.0 - 0.5) * contrast + 0.5 + bright;
                if (v < 0.0) v = 0.0;
                if (v > 1.0) v = 1.0;
                ushort val = (ushort)(v * 65535.0);
                ramp.Red[i] = ramp.Green[i] = ramp.Blue[i] = val;
            }

            return ramp;
        }
    }
}
