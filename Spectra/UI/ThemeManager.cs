using System;
using System.Drawing;

namespace Spectra.UI
{
    // Central palette with runtime-switchable Light / Dark themes.
    // Controls read these as properties, so InitializeComponent picks up the
    // active theme; switching at runtime raises ThemeChanged so open forms repaint.
    public static class ThemeManager
    {
        public static bool IsDark { get; private set; }

        public static event EventHandler ThemeChanged;

        public static void SetDarkMode(bool dark)
        {
            if (IsDark == dark) return;
            IsDark = dark;
            ThemeChanged?.Invoke(null, EventArgs.Empty);
        }

        public static void ToggleTheme() => SetDarkMode(!IsDark);

        // ── Brand gradient (header) — same navy on both themes for brand identity
        public static Color GradStart => Color.FromArgb(18,  46,  96);
        public static Color GradEnd   => Color.FromArgb(30, 110, 180);

        // ── Surfaces ──────────────────────────────────────────────────────
        public static Color Bg       => IsDark ? Color.FromArgb(24,  27,  36)  : Color.FromArgb(237, 242, 250);
        public static Color Surface  => IsDark ? Color.FromArgb(34,  38,  50)  : Color.FromArgb(255, 255, 255);
        public static Color Surface2 => IsDark ? Color.FromArgb(46,  52,  68)  : Color.FromArgb(224, 234, 248);
        public static Color Border   => IsDark ? Color.FromArgb(60,  68,  88)  : Color.FromArgb(196, 214, 236);

        // ── Accent ─────────────────────────────────────────────────────────
        public static Color Accent      => IsDark ? Color.FromArgb(74, 144, 226) : Color.FromArgb(22,  68, 148);
        public static Color AccentLight => IsDark ? Color.FromArgb(96, 165, 245) : Color.FromArgb(40, 100, 200);

        // ── Text ──────────────────────────────────────────────────────────
        public static Color Text    => IsDark ? Color.FromArgb(228, 232, 240) : Color.FromArgb(14,  28,  54);
        public static Color TextSub => IsDark ? Color.FromArgb(150, 160, 180) : Color.FromArgb(72,  96, 136);

        // ── Status / feedback (theme-independent) ──────────────────────────
        public static Color Success => Color.FromArgb(16, 162,  78);
        public static Color Danger  => Color.FromArgb(210,  45,  45);
        public static Color Warning => Color.FromArgb(200, 130,   0);
    }
}
