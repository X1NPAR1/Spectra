using System.Drawing;

namespace Spectra.UI
{
    // Single light theme palette — no runtime switching.
    // All colours are named constants; controls read them once on construction.
    public static class ThemeManager
    {
        // ── Brand gradient (header) ───────────────────────────────────────
        public static Color GradStart => Color.FromArgb(18,  46,  96);
        public static Color GradEnd   => Color.FromArgb(30, 110, 180);

        // ── Surfaces ──────────────────────────────────────────────────────
        public static Color Bg       => Color.FromArgb(245, 247, 252);
        public static Color Surface  => Color.FromArgb(255, 255, 255);
        public static Color Surface2 => Color.FromArgb(235, 238, 246);
        public static Color Border   => Color.FromArgb(210, 215, 228);

        // ── Accent ────────────────────────────────────────────────────────
        public static Color Accent      => Color.FromArgb(37,  99, 235);
        public static Color AccentLight => Color.FromArgb(96, 165, 250);

        // ── Text ──────────────────────────────────────────────────────────
        public static Color Text    => Color.FromArgb( 15,  23,  42);
        public static Color TextSub => Color.FromArgb(100, 116, 139);

        // ── Status / feedback ─────────────────────────────────────────────
        public static Color Success => Color.FromArgb(  5, 150,  72);
        public static Color Danger  => Color.FromArgb(220,  38,  38);
        public static Color Warning => Color.FromArgb(180, 100,   0);
    }
}
