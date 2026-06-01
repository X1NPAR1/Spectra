using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Spectra.Localization;
using Spectra.UI;

namespace Spectra.common
{
    public partial class SettingsForm : Form
    {
        private readonly MainForm    _mainForm;
        private readonly IVibranceProxy _proxy;
        private readonly int         _minLevel;
        private readonly int         _maxLevel;
        private readonly int         _defaultLevel;
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

            // Wire vibrance slider
            trackDesktop.Minimum = minLevel;
            trackDesktop.Maximum = maxLevel;
            trackDesktop.Value   = Math.Max(minLevel, Math.Min(maxLevel, currentLevel));
            labelDesktopVal.Text = resolveLabel(trackDesktop.Value);

            // Wire hotkey display
            var hm = _mainForm.GetHotkeyManager();
            if (hm != null) btnHotkeyPicker.Text = hm.CurrentKey.ToString();

            // Wire language
            cboLanguage.Items.AddRange(LocalizationManager.LanguageNames);
            cboLanguage.SelectedIndex = (int)LocalizationManager.Current;

            // Wire theme
            bool dark = ThemeManager.IsDark;
            radioDark.Checked  = dark;
            radioLight.Checked = !dark;

            ApplyTheme();
            ApplyLocalization();

            ThemeManager.ThemeChanged          += (s, e) => { ApplyTheme(); ApplyLocalization(); };
            LocalizationManager.LanguageChanged += (s, e) => ApplyLocalization();
        }

        // ── Theme ─────────────────────────────────────────────────────────
        private void ApplyTheme()
        {
            if (InvokeRequired) { Invoke((Action)ApplyTheme); return; }

            bool dark = ThemeManager.IsDark;
            BackColor       = ThemeManager.Bg;
            ForeColor       = ThemeManager.Text;
            tabControl.BackColor  = ThemeManager.Surface;

            // Header paint
            panelSettingsHeader.Invalidate();

            // Tab pages
            foreach (TabPage tp in tabControl.TabPages)
            {
                tp.BackColor = ThemeManager.Surface;
                tp.ForeColor = ThemeManager.Text;
                ApplyControlsInPage(tp);
            }

            // Buttons
            btnApply.BackColor = ThemeManager.Accent;
            btnApply.ForeColor = Color.White;
            btnCancel.BackColor = ThemeManager.Surface2;
            btnCancel.ForeColor = ThemeManager.Text;
            btnApply.FlatAppearance.BorderColor  = ThemeManager.Accent;
            btnCancel.FlatAppearance.BorderColor = ThemeManager.Border;

            Refresh();
        }

        private void ApplyControlsInPage(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                switch (c)
                {
                    case Label lbl:
                        lbl.BackColor = Color.Transparent;
                        lbl.ForeColor = lbl.Tag != null && lbl.Tag.ToString() == "accent"
                            ? ThemeManager.Accent
                            : lbl.Tag != null && lbl.Tag.ToString() == "sub"
                                ? ThemeManager.TextSub
                                : ThemeManager.Text;
                        break;
                    case CheckBox cb:
                        cb.BackColor = Color.Transparent;
                        cb.ForeColor = ThemeManager.Text;
                        break;
                    case RadioButton rb:
                        rb.BackColor = Color.Transparent;
                        rb.ForeColor = ThemeManager.Text;
                        break;
                    case TrackBar tb:
                        tb.BackColor = ThemeManager.Surface;
                        break;
                    case ComboBox cb:
                        cb.BackColor = ThemeManager.Surface2;
                        cb.ForeColor = ThemeManager.Text;
                        break;
                    case Button btn:
                        if (btn.Tag != null && btn.Tag.ToString() == "accent")
                        {
                            btn.BackColor = ThemeManager.Accent;
                            btn.ForeColor = Color.White;
                        }
                        else if (btn.Tag != null && btn.Tag.ToString() == "danger")
                        {
                            btn.BackColor = ThemeManager.Danger;
                            btn.ForeColor = Color.White;
                        }
                        else
                        {
                            btn.BackColor = ThemeManager.Surface2;
                            btn.ForeColor = ThemeManager.Text;
                            btn.FlatAppearance.BorderColor = ThemeManager.Border;
                        }
                        break;
                    case Panel pnl:
                        pnl.BackColor = pnl.Tag != null && pnl.Tag.ToString() == "separator"
                            ? ThemeManager.Border
                            : Color.Transparent;
                        ApplyControlsInPage(pnl);
                        break;
                    default:
                        ApplyControlsInPage(c);
                        break;
                }
            }
        }

        private void ApplyLocalization()
        {
            if (InvokeRequired) { Invoke((Action)ApplyLocalization); return; }
            Text = LocalizationManager.Get("SettingsTitle");

            // Tabs
            tabGeneral.Text  = LocalizationManager.Get("TabGeneral");
            tabVibrance.Text = LocalizationManager.Get("TabVibrance");
            tabHotkey.Text   = LocalizationManager.Get("TabHotkey");
            tabAbout.Text    = LocalizationManager.Get("TabAbout");

            // General tab
            grpAppearance.Text  = LocalizationManager.Get("Appearance");
            lblThemeTitle.Text  = LocalizationManager.Get("Theme");
            radioDark.Text      = LocalizationManager.Get("Dark");
            radioLight.Text     = LocalizationManager.Get("Light");
            lblLangTitle.Text   = LocalizationManager.Get("Language");

            grpBehavior.Text         = LocalizationManager.Get("Behavior");
            chkSettingsAutostart.Text  = LocalizationManager.Get("Autostart");
            chkSettingsMinToTray.Text  = LocalizationManager.Get("MinimizeToTray");
            chkSettingsNotify.Text     = LocalizationManager.Get("ShowNotifications");

            // Vibrance tab
            lblDesktopDefault.Text   = LocalizationManager.Get("DesktopDefault");
            lblDesktopNote.Text      = LocalizationManager.Get("DesktopDefaultNote");
            lblMonitorSection.Text   = LocalizationManager.Get("MonitorConfig");
            chkPrimaryOnly.Text      = LocalizationManager.Get("PrimaryMonitor");
            chkNoResize.Text         = LocalizationManager.Get("NeverResize");
            btnResetDesktop.Text     = LocalizationManager.Get("ResetDefault");

            // Hotkey tab
            lblHotkeySection.Text    = LocalizationManager.Get("GlobalHotkey");
            lblHotkeyNote.Text       = LocalizationManager.Get("HotkeyNote");
            lblHotkeyBehavior.Text   = LocalizationManager.Get("HotkeyBehavior");
            radioToggle.Text         = LocalizationManager.Get("HotkeyToggle");
            radioHold.Text           = LocalizationManager.Get("HotkeyHold");
            btnClearHotkey.Text      = LocalizationManager.Get("ClearHotkey");

            // About tab
            lblAboutVersion.Text     = "Spectra v1.7.0";
            lblAboutDesc.Text        = LocalizationManager.Get("AboutDesc");
            lblAboutGpu.Text         = LocalizationManager.Get("AboutGpu");

            // Bottom buttons
            btnApply.Text  = LocalizationManager.Get("Apply");
            btnCancel.Text = LocalizationManager.Get("Cancel");
        }

        // ── Header paint ──────────────────────────────────────────────────
        private void panelSettingsHeader_Paint(object sender, PaintEventArgs e)
        {
            var g    = e.Graphics;
            var rect = panelSettingsHeader.ClientRectangle;
            using (var grad = new LinearGradientBrush(rect,
                Color.FromArgb(100, 30, 220),
                Color.FromArgb(0, 175, 225),
                LinearGradientMode.Horizontal))
            {
                g.FillRectangle(grad, rect);
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
            btnHotkeyPicker.KeyDown  += BtnHotkeyPicker_KeyDown;
            btnHotkeyPicker.Focus();
        }

        private void BtnHotkeyPicker_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled          = true;
            e.SuppressKeyPress = true;
            btnHotkeyPicker.KeyDown -= BtnHotkeyPicker_KeyDown;
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

        // ── About buttons ─────────────────────────────────────────────────
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
