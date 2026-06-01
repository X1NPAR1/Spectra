using System;
using System.Drawing;
using System.Windows.Forms;

namespace Spectra.UI
{
    public enum AppTheme { Dark, Light }

    public static class ThemeManager
    {
        public static AppTheme Current { get; private set; } = AppTheme.Dark;

        public static event EventHandler ThemeChanged;

        // ── Dark palette ──────────────────────────────────────────────────
        private static readonly Color _darkBg         = Color.FromArgb(13,  17,  23);
        private static readonly Color _darkSurface    = Color.FromArgb(22,  27,  34);
        private static readonly Color _darkSurface2   = Color.FromArgb(33,  38,  45);
        private static readonly Color _darkBorder     = Color.FromArgb(48,  54,  61);
        private static readonly Color _darkAccent     = Color.FromArgb(88,  166, 255);
        private static readonly Color _darkAccent2    = Color.FromArgb(123, 47,  247);
        private static readonly Color _darkText       = Color.FromArgb(201, 209, 217);
        private static readonly Color _darkTextSub    = Color.FromArgb(110, 118, 129);
        private static readonly Color _darkSuccess    = Color.FromArgb(63,  185, 80);
        private static readonly Color _darkDanger     = Color.FromArgb(248, 81,  73);

        // ── Light palette ─────────────────────────────────────────────────
        private static readonly Color _lightBg        = Color.FromArgb(246, 248, 250);
        private static readonly Color _lightSurface   = Color.FromArgb(255, 255, 255);
        private static readonly Color _lightSurface2  = Color.FromArgb(234, 238, 242);
        private static readonly Color _lightBorder    = Color.FromArgb(208, 215, 222);
        private static readonly Color _lightAccent    = Color.FromArgb(9,   105, 218);
        private static readonly Color _lightAccent2   = Color.FromArgb(130, 80,  223);
        private static readonly Color _lightText      = Color.FromArgb(31,  35,  40);
        private static readonly Color _lightTextSub   = Color.FromArgb(101, 109, 118);
        private static readonly Color _lightSuccess   = Color.FromArgb(26,  127, 55);
        private static readonly Color _lightDanger    = Color.FromArgb(207, 34,  46);

        // ── Public accessors ──────────────────────────────────────────────
        public static bool IsDark    => Current == AppTheme.Dark;
        public static Color Bg       => IsDark ? _darkBg      : _lightBg;
        public static Color Surface  => IsDark ? _darkSurface : _lightSurface;
        public static Color Surface2 => IsDark ? _darkSurface2: _lightSurface2;
        public static Color Border   => IsDark ? _darkBorder  : _lightBorder;
        public static Color Accent   => IsDark ? _darkAccent  : _lightAccent;
        public static Color Accent2  => IsDark ? _darkAccent2 : _lightAccent2;
        public static Color Text     => IsDark ? _darkText    : _lightText;
        public static Color TextSub  => IsDark ? _darkTextSub : _lightTextSub;
        public static Color Success  => IsDark ? _darkSuccess : _lightSuccess;
        public static Color Danger   => IsDark ? _darkDanger  : _lightDanger;

        public static Font FontRegular => new Font("Segoe UI", 9f,  FontStyle.Regular);
        public static Font FontBold    => new Font("Segoe UI", 9f,  FontStyle.Bold);
        public static Font FontLarge   => new Font("Segoe UI", 12f, FontStyle.Bold);
        public static Font FontHeader  => new Font("Segoe UI", 20f, FontStyle.Bold);
        public static Font FontSmall   => new Font("Segoe UI", 7.5f,FontStyle.Regular);
        public static Font FontMono    => new Font("Consolas",  9f,  FontStyle.Regular);

        public static void SetTheme(AppTheme theme)
        {
            Current = theme;
            ThemeChanged?.Invoke(null, EventArgs.Empty);
        }

        // Apply theme colors to a form and all its child controls
        public static void Apply(Control root)
        {
            root.BackColor = Bg;
            root.ForeColor = Text;
            foreach (Control c in root.Controls)
                ApplyControl(c);
        }

        private static void ApplyControl(Control c)
        {
            switch (c)
            {
                case Panel p:
                    p.BackColor = p.Tag is string tag && tag == "header" ? Color.Transparent : Surface;
                    p.ForeColor = Text;
                    break;
                case Label lbl:
                    lbl.ForeColor = lbl.Tag is string t && t == "sub" ? TextSub : Text;
                    lbl.BackColor = Color.Transparent;
                    break;
                case Button btn:
                    if (btn.Tag is string bt)
                    {
                        if (bt == "accent")  { btn.BackColor = Accent;   btn.ForeColor = Color.White; }
                        else if (bt == "danger") { btn.BackColor = Danger; btn.ForeColor = Color.White; }
                        else { btn.BackColor = Surface2; btn.ForeColor = Text; }
                    }
                    else { btn.BackColor = Surface2; btn.ForeColor = Text; }
                    break;
                case CheckBox cb:
                    cb.BackColor = Color.Transparent;
                    cb.ForeColor = Text;
                    break;
                case ComboBox cmb:
                    cmb.BackColor = Surface2;
                    cmb.ForeColor = Text;
                    break;
                case ListBox lb:
                    lb.BackColor = Surface;
                    lb.ForeColor = Text;
                    break;
                case ListView lv:
                    lv.BackColor = Surface;
                    lv.ForeColor = Text;
                    break;
                case TrackBar _:
                    c.BackColor = Surface;
                    break;
                case GroupBox gb:
                    gb.BackColor = Surface;
                    gb.ForeColor = TextSub;
                    break;
                default:
                    c.BackColor = Surface;
                    c.ForeColor = Text;
                    break;
            }

            foreach (Control child in c.Controls)
                ApplyControl(child);
        }
    }
}
