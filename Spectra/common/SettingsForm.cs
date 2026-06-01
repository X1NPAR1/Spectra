using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using Spectra.Localization;
using Spectra.UI;

namespace Spectra.common
{
    public partial class SettingsForm : Form
    {
        private readonly MainForm       _mainForm;
        private readonly IVibranceProxy _proxy;
        private readonly int            _minLevel;
        private readonly int            _maxLevel;
        private readonly int            _defaultLevel;
        private readonly Func<int, string> _resolveLabel;
        private bool _capturingHotkey;

        public int DesktopVibranceLevel { get; private set; }

        public SettingsForm(MainForm mainForm, IVibranceProxy proxy,
            int minLevel, int maxLevel, int defaultLevel, int currentLevel,
            Func<int, string> resolveLabel)
        {
            _mainForm     = mainForm;
            _proxy        = proxy;
            _minLevel     = minLevel;
            _maxLevel     = maxLevel;
            _defaultLevel = defaultLevel;
            _resolveLabel = resolveLabel;
            DesktopVibranceLevel = currentLevel;

            InitializeComponent();

            Icon = IconFactory.GetAppIcon(16);

            // Init vibrance slider
            trackDesktop.Minimum = minLevel;
            trackDesktop.Maximum = maxLevel;
            trackDesktop.Value   = Math.Max(minLevel, Math.Min(maxLevel, currentLevel));
            labelDesktopVal.Text = resolveLabel(trackDesktop.Value);

            // Init hotkey
            var hm = _mainForm.GetHotkeyManager();
            if (hm != null) btnHotkeyPicker.Text = hm.CurrentKey.ToString();

            // Init language combo
            cboLanguage.Items.AddRange(LocalizationManager.LanguageNames);
            cboLanguage.SelectedIndex = (int)LocalizationManager.Current;

            // Init theme radios
            radioDark.Checked  = ThemeManager.IsDark;
            radioLight.Checked = !ThemeManager.IsDark;

            // Apply
            ApplyTheme();
            ApplyLocalization();

            // Subscribe to changes
            ThemeManager.ThemeChanged          += OnThemeChanged;
            LocalizationManager.LanguageChanged += OnLanguageChanged;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            ThemeManager.ThemeChanged          -= OnThemeChanged;
            LocalizationManager.LanguageChanged -= OnLanguageChanged;
            base.OnFormClosed(e);
        }

        private void OnThemeChanged(object s, EventArgs e)
        {
            if (!IsDisposed && IsHandleCreated) Invoke((Action)ApplyTheme);
        }

        private void OnLanguageChanged(object s, EventArgs e)
        {
            if (!IsDisposed && IsHandleCreated) Invoke((Action)ApplyLocalization);
        }

        // ── THEME — every control explicitly themed ───────────────────────
        private void ApplyTheme()
        {
            bool dark = ThemeManager.IsDark;

            BackColor       = ThemeManager.Bg;
            ForeColor       = ThemeManager.Text;

            // Tab control
            tabControl.BackColor = ThemeManager.Surface;

            // All tab pages
            foreach (TabPage tp in new[] { tabGeneral, tabVibrance, tabHotkey, tabAbout })
            {
                tp.BackColor = ThemeManager.Surface;
                tp.ForeColor = ThemeManager.Text;
            }

            // ── General ──────────────────────────────────────────────────
            ThemeAccentLabel(lblAppearanceTitle);
            ThemeSep(sepAppearance);
            ThemeLabel(lblThemeTitle);
            ThemeRadio(radioDark);
            ThemeRadio(radioLight);
            ThemeLabel(lblLangTitle);
            cboLanguage.BackColor = ThemeManager.Surface2;
            cboLanguage.ForeColor = ThemeManager.Text;
            ThemeAccentLabel(lblBehaviorTitle);
            ThemeSep(sepBehavior);
            ThemeCheck(chkSettingsAutostart);
            ThemeCheck(chkSettingsMinToTray);
            ThemeCheck(chkSettingsNotify);

            // ── Vibrance ──────────────────────────────────────────────────
            ThemeAccentLabel(lblVibDefault);
            ThemeSep(sepVib1);
            ThemeSubLabel(lblVibNote);
            trackDesktop.BackColor  = ThemeManager.Surface;
            labelDesktopVal.ForeColor = ThemeManager.Accent;
            labelDesktopVal.BackColor = Color.Transparent;
            ThemeButton(btnResetDesktop);
            ThemeAccentLabel(lblMonitorTitle);
            ThemeSep(sepVib2);
            ThemeCheck(chkPrimaryOnly);
            ThemeCheck(chkNoResize);

            // ── Hotkey ────────────────────────────────────────────────────
            ThemeAccentLabel(lblHotkeyTitle);
            ThemeSep(sepHotkey1);
            ThemeSubLabel(lblHotkeyNote);
            // Hotkey picker button — accent style
            btnHotkeyPicker.BackColor = ThemeManager.Surface2;
            btnHotkeyPicker.ForeColor = ThemeManager.Accent;
            btnHotkeyPicker.FlatAppearance.BorderColor = ThemeManager.Accent;
            // Clear = danger
            btnClearHotkey.BackColor = ThemeManager.Danger;
            btnClearHotkey.ForeColor = Color.White;
            btnClearHotkey.FlatAppearance.BorderColor = ThemeManager.Danger;
            ThemeAccentLabel(lblHotkeyBehTitle);
            ThemeSep(sepHotkey2);
            ThemeRadio(radioToggle);
            radioHold.ForeColor = ThemeManager.TextSub;
            radioHold.BackColor = Color.Transparent;

            // ── About ─────────────────────────────────────────────────────
            panelAboutLogo.BackColor = Color.Transparent;
            panelAboutLogo.Invalidate();
            lblAboutVersion.ForeColor = ThemeManager.Text;
            lblAboutVersion.BackColor = Color.Transparent;
            ThemeSubLabel(lblAboutDesc);
            ThemeSep(sepAbout);
            lblAboutGpu.ForeColor = ThemeManager.Text;
            lblAboutGpu.BackColor = Color.Transparent;
            btnGitHub.BackColor   = ThemeManager.Accent;
            btnGitHub.ForeColor   = Color.White;
            btnGitHub.FlatAppearance.BorderColor = ThemeManager.Accent;

            // ── Bottom buttons ────────────────────────────────────────────
            btnApply.BackColor  = ThemeManager.Accent;
            btnApply.ForeColor  = Color.White;
            btnApply.FlatAppearance.BorderColor = ThemeManager.Accent;
            btnCancel.BackColor = ThemeManager.Surface2;
            btnCancel.ForeColor = ThemeManager.Text;
            btnCancel.FlatAppearance.BorderColor = ThemeManager.Border;

            tabControl.Invalidate();
            Refresh();
        }

        private void ThemeAccentLabel(Label l)
        {
            l.ForeColor = ThemeManager.Accent;
            l.BackColor = Color.Transparent;
        }

        private void ThemeSubLabel(Label l)
        {
            l.ForeColor = ThemeManager.TextSub;
            l.BackColor = Color.Transparent;
        }

        private void ThemeLabel(Label l)
        {
            l.ForeColor = ThemeManager.Text;
            l.BackColor = Color.Transparent;
        }

        private void ThemeSep(Panel p)
        {
            p.BackColor = ThemeManager.Border;
        }

        private void ThemeCheck(CheckBox cb)
        {
            cb.ForeColor = ThemeManager.Text;
            cb.BackColor = Color.Transparent;
        }

        private void ThemeRadio(RadioButton rb)
        {
            rb.ForeColor = ThemeManager.Text;
            rb.BackColor = Color.Transparent;
        }

        private void ThemeButton(Button b)
        {
            b.BackColor = ThemeManager.Surface2;
            b.ForeColor = ThemeManager.Text;
            b.FlatAppearance.BorderColor = ThemeManager.Border;
        }

        // ── LOCALIZATION — every string explicit ─────────────────────────
        private void ApplyLocalization()
        {
            Text = LocalizationManager.Get("SettingsTitle");

            // Tab names
            tabGeneral.Text  = LocalizationManager.Get("TabGeneral");
            tabVibrance.Text = LocalizationManager.Get("TabVibrance");
            tabHotkey.Text   = LocalizationManager.Get("TabHotkey");
            tabAbout.Text    = LocalizationManager.Get("TabAbout");
            tabControl.Invalidate();

            // General
            lblAppearanceTitle.Text   = LocalizationManager.Get("Appearance");
            lblThemeTitle.Text        = LocalizationManager.Get("Theme");
            radioDark.Text            = LocalizationManager.Get("Dark");
            radioLight.Text           = LocalizationManager.Get("Light");
            lblLangTitle.Text         = LocalizationManager.Get("Language");
            lblBehaviorTitle.Text     = LocalizationManager.Get("Behavior");
            chkSettingsAutostart.Text = LocalizationManager.Get("Autostart");
            chkSettingsMinToTray.Text = LocalizationManager.Get("MinimizeToTray");
            chkSettingsNotify.Text    = LocalizationManager.Get("ShowNotifications");

            // Vibrance
            lblVibDefault.Text   = LocalizationManager.Get("DesktopDefault");
            lblVibNote.Text      = LocalizationManager.Get("DesktopDefaultNote");
            btnResetDesktop.Text = LocalizationManager.Get("ResetDefault");
            lblMonitorTitle.Text = LocalizationManager.Get("MonitorConfig");
            chkPrimaryOnly.Text  = LocalizationManager.Get("PrimaryMonitor");
            chkNoResize.Text     = LocalizationManager.Get("NeverResize");

            // Hotkey
            lblHotkeyTitle.Text    = LocalizationManager.Get("GlobalHotkey");
            lblHotkeyNote.Text     = LocalizationManager.Get("HotkeyNote");
            lblHotkeyBehTitle.Text = LocalizationManager.Get("HotkeyBehavior");
            radioToggle.Text       = LocalizationManager.Get("HotkeyToggle");
            radioHold.Text         = LocalizationManager.Get("HotkeyHold");
            btnClearHotkey.Text    = LocalizationManager.Get("ClearHotkey");

            // About
            lblAboutVersion.Text = "Spectra v1.7.0";
            lblAboutDesc.Text    = LocalizationManager.Get("AboutDesc");
            lblAboutGpu.Text     = LocalizationManager.Get("AboutGpuFull");

            // Buttons
            btnApply.Text  = LocalizationManager.Get("Apply");
            btnCancel.Text = LocalizationManager.Get("Cancel");
        }

        // ── Owner-draw tabs ───────────────────────────────────────────────
        private void tabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            var tab      = tabControl.TabPages[e.Index];
            bool selected= e.Index == tabControl.SelectedIndex;
            var g        = e.Graphics;
            g.SmoothingMode     = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Background
            Color bg = selected ? ThemeManager.Surface : ThemeManager.Bg;
            using (var bgBrush = new SolidBrush(bg))
                g.FillRectangle(bgBrush, e.Bounds);

            // Bottom accent line for selected tab
            if (selected)
            {
                using (var accentPen = new Pen(ThemeManager.Accent, 2))
                    g.DrawLine(accentPen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
            }

            // Text
            Color fg = selected ? ThemeManager.Accent : ThemeManager.TextSub;
            using (var fgBrush = new SolidBrush(fg))
            using (var font    = new Font("Segoe UI", 9f, selected ? FontStyle.Bold : FontStyle.Regular))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(tab.Text, font, fgBrush, e.Bounds, sf);
            }
        }

        // ── Paints ───────────────────────────────────────────────────────
        private void panelSettingsHeader_Paint(object sender, PaintEventArgs e)
        {
            var r = panelSettingsHeader.ClientRectangle;
            using (var grad = new LinearGradientBrush(r,
                Color.FromArgb(100, 30, 220), Color.FromArgb(0, 175, 225),
                LinearGradientMode.Horizontal))
                e.Graphics.FillRectangle(grad, r);
        }

        private void panelAboutLogo_Paint(object sender, PaintEventArgs e)
        {
            var g    = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(0, 0, 62, 62);
            var path = new GraphicsPath();
            int r = 10, d = r * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            using (var grad = new LinearGradientBrush(rect,
                Color.FromArgb(100, 30, 220), Color.FromArgb(0, 175, 225),
                LinearGradientMode.ForwardDiagonal))
                g.FillPath(grad, path);
            using (var font = new Font("Segoe UI", 28f, FontStyle.Bold, GraphicsUnit.Pixel))
            using (var br   = new SolidBrush(Color.FromArgb(240, 248, 255)))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("S", font, br, new RectangleF(0, 0, 64, 64), sf);
            }
        }

        // ── Vibrance slider ───────────────────────────────────────────────
        private void trackDesktop_Scroll(object sender, EventArgs e)
        {
            DesktopVibranceLevel = trackDesktop.Value;
            labelDesktopVal.Text = _resolveLabel(trackDesktop.Value);
            _proxy?.SetVibranceWindowsLevel(trackDesktop.Value);
        }

        private void btnResetDesktop_Click(object sender, EventArgs e)
        {
            trackDesktop.Value   = _defaultLevel;
            DesktopVibranceLevel = _defaultLevel;
            labelDesktopVal.Text = _resolveLabel(_defaultLevel);
            _proxy?.SetVibranceWindowsLevel(_defaultLevel);
        }

        // ── Theme radios ──────────────────────────────────────────────────
        private void radioDark_CheckedChanged(object sender, EventArgs e)
        {
            if (radioDark.Checked) ThemeManager.SetTheme(AppTheme.Dark);
        }

        private void radioLight_CheckedChanged(object sender, EventArgs e)
        {
            if (radioLight.Checked) ThemeManager.SetTheme(AppTheme.Light);
        }

        // ── Language ──────────────────────────────────────────────────────
        private void cboLanguage_SelectedIndexChanged(object sender, EventArgs e)
            => LocalizationManager.SetLanguage((Language)cboLanguage.SelectedIndex);

        // ── Hotkey capture ────────────────────────────────────────────────
        private void btnHotkeyPicker_Click(object sender, EventArgs e)
        {
            if (_capturingHotkey) return;
            _capturingHotkey = true;
            btnHotkeyPicker.Text      = LocalizationManager.Get("PressHotkey");
            btnHotkeyPicker.BackColor = Color.FromArgb(123, 47, 247);
            btnHotkeyPicker.ForeColor = Color.White;
            btnHotkeyPicker.KeyDown  += OnHotkeyPickerKeyDown;
            btnHotkeyPicker.Focus();
        }

        private void OnHotkeyPickerKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled          = true;
            e.SuppressKeyPress = true;
            btnHotkeyPicker.KeyDown -= OnHotkeyPickerKeyDown;
            _capturingHotkey = false;

            if (e.KeyCode != Keys.Escape && e.KeyCode != Keys.None)
            {
                _mainForm.ApplyHotkeyKey(e.KeyCode);
                btnHotkeyPicker.Text = e.KeyCode.ToString();
            }
            else
            {
                var hm = _mainForm.GetHotkeyManager();
                btnHotkeyPicker.Text = hm != null ? hm.CurrentKey.ToString() : "F9";
            }

            ApplyTheme();
        }

        private void btnClearHotkey_Click(object sender, EventArgs e)
        {
            _mainForm.GetHotkeyManager()?.Unregister();
            btnHotkeyPicker.Text = LocalizationManager.Get("None");
        }

        // ── About ─────────────────────────────────────────────────────────
        private void btnGitHub_Click(object sender, EventArgs e)
            => Process.Start("https://github.com/X1NPAR1/Spectra");

        // ── Apply / Cancel ────────────────────────────────────────────────
        private void btnApply_Click(object sender, EventArgs e)
        {
            DesktopVibranceLevel = trackDesktop.Value;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
