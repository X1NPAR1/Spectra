using System.Drawing;

namespace Spectra.UI
{
    // Single professional light theme — purple brand accent matching the logo gradient.
    public static class ThemeManager
    {
        // ── Brand gradient (matches the logo S and header) ────────────────
        public static readonly Color GradStart   = Color.FromArgb(108,  47, 230);  // purple
        public static readonly Color GradEnd     = Color.FromArgb(  0, 185, 220);  // cyan

        // ── Surfaces ──────────────────────────────────────────────────────
        public static readonly Color Bg       = Color.FromArgb(238, 240, 248);  // very light blue-gray
        public static readonly Color Surface  = Color.FromArgb(255, 255, 255);  // white card
        public static readonly Color Surface2 = Color.FromArgb(230, 232, 244);  // input/button fill
        public static readonly Color Border   = Color.FromArgb(210, 213, 228);  // card border

        // ── Accent (purple — consistent with logo) ────────────────────────
        public static readonly Color Accent   = Color.FromArgb(108,  47, 230);
        public static readonly Color AccentHover = Color.FromArgb(90, 35, 200);

        // ── Text ──────────────────────────────────────────────────────────
        public static readonly Color Text     = Color.FromArgb( 22,  22,  44);
        public static readonly Color TextSub  = Color.FromArgb( 96,  98, 122);

        // ── Status ────────────────────────────────────────────────────────
        public static readonly Color Success  = Color.FromArgb( 16, 185, 129);
        public static readonly Color Danger   = Color.FromArgb(239,  68,  68);
        public static readonly Color Warning  = Color.FromArgb(245, 158,  11);
    }
}
