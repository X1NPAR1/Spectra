using System;
using System.Runtime.InteropServices;

namespace Spectra.common
{
    // GPU-agnostic brightness & contrast control via the Windows GDI gamma ramp.
    // Works on both NVIDIA and AMD because it operates at the OS display level
    // rather than through a vendor driver SDK.
    //
    // brightness / contrast are expressed as 0..100 where 50 = neutral (no change).
    public static class DisplayGammaController
    {
        [DllImport("gdi32.dll")]
        private static extern bool SetDeviceGammaRamp(IntPtr hdc, ref RAMP lpRamp);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [StructLayout(LayoutKind.Sequential)]
        private struct RAMP
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256 * 3)]
            public ushort[] Values; // R[256], G[256], B[256]
        }

        public const int Neutral = 50;

        private static int _brightness = Neutral;
        private static int _contrast   = Neutral;

        public static int Brightness => _brightness;
        public static int Contrast   => _contrast;

        public static void SetBrightness(int value)
        {
            _brightness = Clamp(value);
            Apply();
        }

        public static void SetContrast(int value)
        {
            _contrast = Clamp(value);
            Apply();
        }

        public static void Set(int brightness, int contrast)
        {
            _brightness = Clamp(brightness);
            _contrast   = Clamp(contrast);
            Apply();
        }

        // Restores the default (neutral) gamma ramp.
        public static void Reset()
        {
            _brightness = Neutral;
            _contrast   = Neutral;
            Apply();
        }

        private static int Clamp(int v) => v < 0 ? 0 : (v > 100 ? 100 : v);

        private static void Apply()
        {
            // Map 0..100 to usable factors. Neutral (50) yields the identity ramp.
            // brightness: -1.0 .. +1.0 offset   contrast: 0.5 .. 1.5 multiplier
            double bright   = (_brightness - Neutral) / 100.0;        // -0.5 .. +0.5
            double contrast = 1.0 + (_contrast - Neutral) / 100.0;    // 0.5 .. 1.5

            var ramp = new RAMP { Values = new ushort[256 * 3] };
            for (int i = 0; i < 256; i++)
            {
                double normalized = i / 255.0;

                // Apply contrast around the 0.5 midpoint, then brightness offset.
                double v = (normalized - 0.5) * contrast + 0.5 + bright;
                if (v < 0.0) v = 0.0;
                if (v > 1.0) v = 1.0;

                ushort val = (ushort)(v * 65535.0);
                ramp.Values[i]           = val; // R
                ramp.Values[256 + i]     = val; // G
                ramp.Values[512 + i]     = val; // B
            }

            IntPtr hdc = GetDC(IntPtr.Zero);
            if (hdc == IntPtr.Zero) return;
            try { SetDeviceGammaRamp(hdc, ref ramp); }
            finally { ReleaseDC(IntPtr.Zero, hdc); }
        }
    }
}
