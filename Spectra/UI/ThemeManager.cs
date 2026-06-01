using System.Drawing;

namespace Spectra.UI
{
    // Professional navy-blue theme tuned to match the setting.ico color palette.
    // The icon is dominated by deep navy (#000020) with teal highlights.
    public static class ThemeManager
    {
        // ── Brand gradient (header, accent fills) ─────────────────────────
        // Navy → medium blue — matches the deep-navy icon character
        public static readonly Color GradStart = Color.FromArgb(18,  46,  96);  // deep navy
        public static readonly Color GradEnd   = Color.FromArgb(30, 110, 180);  // medium blue

        // ── Surfaces ──────────────────────────────────────────────────────
        public static readonly Color Bg       = Color.FromArgb(237, 242, 250);  // ice-blue-white
        public static readonly Color Surface  = Color.FromArgb(255, 255, 255);  // white card
        public static readonly Color Surface2 = Color.FromArgb(224, 234, 248);  // light navy-blue
        public static readonly Color Border   = Color.FromArgb(196, 214, 236);  // blue-gray border

        // ── Accent (navy blue — brand color) ──────────────────────────────
        public static readonly Color Accent      = Color.FromArgb(22,  68, 148);  // navy blue
        public static readonly Color AccentLight = Color.FromArgb(40, 100, 200);  // lighter navy

        // ── Text ──────────────────────────────────────────────────────────
        public static readonly Color Text     = Color.FromArgb(14,  28,  54);  // dark navy
        public static readonly Color TextSub  = Color.FromArgb(72,  96, 136);  // medium blue-gray

        // ── Status / feedback ─────────────────────────────────────────────
        public static readonly Color Success  = Color.FromArgb(16, 162,  78);  // teal-green
        public static readonly Color Danger   = Color.FromArgb(210,  45,  45);  // red
        public static readonly Color Warning  = Color.FromArgb(200, 130,   0);  // amber
    }
}
