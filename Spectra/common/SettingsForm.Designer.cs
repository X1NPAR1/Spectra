using System.Drawing;
using System.Windows.Forms;

namespace Spectra.common
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            panelSettingsHeader = new Panel();
            labelSettingsTitle  = new Label();
            tabControl          = new TabControl();
            tabGeneral          = new TabPage();
            tabVibrance         = new TabPage();
            tabHotkey           = new TabPage();
            tabAbout            = new TabPage();

            // ── General tab controls ──────────────────────────────────────
            grpAppearance       = new GroupBox();
            lblThemeTitle       = new Label();
            radioDark           = new RadioButton();
            radioLight          = new RadioButton();
            sep1                = new Panel();
            lblLangTitle        = new Label();
            cboLanguage         = new ComboBox();

            grpBehavior            = new GroupBox();
            chkSettingsAutostart   = new CheckBox();
            chkSettingsMinToTray   = new CheckBox();
            chkSettingsNotify      = new CheckBox();

            // ── Vibrance tab controls ─────────────────────────────────────
            lblDesktopDefault   = new Label();
            lblDesktopNote      = new Label();
            trackDesktop        = new TrackBar();
            labelDesktopVal     = new Label();
            btnResetDesktop     = new Button();
            sep2                = new Panel();
            lblMonitorSection   = new Label();
            chkPrimaryOnly      = new CheckBox();
            chkNoResize         = new CheckBox();

            // ── Hotkey tab controls ───────────────────────────────────────
            lblHotkeySection    = new Label();
            lblHotkeyNote       = new Label();
            btnHotkeyPicker     = new Button();
            btnClearHotkey      = new Button();
            sep3                = new Panel();
            lblHotkeyBehavior   = new Label();
            radioToggle         = new RadioButton();
            radioHold           = new RadioButton();

            // ── About tab controls ────────────────────────────────────────
            panelAboutLogo      = new Panel();
            lblAboutVersion     = new Label();
            lblAboutDesc        = new Label();
            sep4                = new Panel();
            lblAboutGpu         = new Label();
            btnGitHub           = new Button();

            // ── Bottom buttons ────────────────────────────────────────────
            btnApply            = new Button();
            btnCancel           = new Button();

            SuspendLayout();
            panelSettingsHeader.SuspendLayout();
            tabControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackDesktop).BeginInit();

            // ── Form ──────────────────────────────────────────────────────
            AutoScaleDimensions = new SizeF(6f, 13f);
            AutoScaleMode       = AutoScaleMode.Font;
            ClientSize          = new Size(470, 510);
            FormBorderStyle     = FormBorderStyle.FixedSingle;
            MaximizeBox         = false;
            StartPosition       = FormStartPosition.CenterParent;
            Text                = "Spectra Settings";
            Name                = "SettingsForm";

            // ── Header ────────────────────────────────────────────────────
            panelSettingsHeader.Location = new Point(0, 0);
            panelSettingsHeader.Size     = new Size(470, 52);
            panelSettingsHeader.Paint   += panelSettingsHeader_Paint;

            labelSettingsTitle.Text      = "⚙  SETTINGS";
            labelSettingsTitle.Font      = new Font("Segoe UI", 14f, FontStyle.Bold);
            labelSettingsTitle.ForeColor = Color.White;
            labelSettingsTitle.BackColor = Color.Transparent;
            labelSettingsTitle.Location  = new Point(14, 12);
            labelSettingsTitle.AutoSize  = true;
            panelSettingsHeader.Controls.Add(labelSettingsTitle);

            // ── TabControl ────────────────────────────────────────────────
            tabControl.Location  = new Point(10, 60);
            tabControl.Size      = new Size(450, 390);
            tabControl.TabPages.AddRange(new TabPage[] { tabGeneral, tabVibrance, tabHotkey, tabAbout });

            // ─── GENERAL TAB ──────────────────────────────────────────────
            tabGeneral.Text    = "General";
            tabGeneral.Padding = new Padding(10);

            grpAppearance.Text     = "Appearance";
            grpAppearance.Location = new Point(8, 8);
            grpAppearance.Size     = new Size(426, 130);
            grpAppearance.Font     = new Font("Segoe UI", 8.5f, FontStyle.Bold);

            lblThemeTitle.Text      = "Theme";
            lblThemeTitle.Font      = new Font("Segoe UI", 8.5f, FontStyle.Regular);
            lblThemeTitle.Location  = new Point(12, 28);
            lblThemeTitle.AutoSize  = true;

            radioDark.Text     = "Dark";
            radioDark.Location = new Point(70, 26);
            radioDark.Size     = new Size(70, 20);
            radioDark.CheckedChanged += radioDark_CheckedChanged;

            radioLight.Text     = "Light";
            radioLight.Location = new Point(145, 26);
            radioLight.Size     = new Size(70, 20);
            radioLight.CheckedChanged += radioLight_CheckedChanged;

            sep1.Location  = new Point(12, 56);
            sep1.Size      = new Size(400, 1);
            sep1.Tag       = "separator";

            lblLangTitle.Text      = "Language";
            lblLangTitle.Font      = new Font("Segoe UI", 8.5f, FontStyle.Regular);
            lblLangTitle.Location  = new Point(12, 70);
            lblLangTitle.AutoSize  = true;

            cboLanguage.Location      = new Point(85, 68);
            cboLanguage.Size          = new Size(160, 22);
            cboLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
            cboLanguage.FlatStyle     = FlatStyle.Flat;
            cboLanguage.SelectedIndexChanged += cboLanguage_SelectedIndexChanged;

            grpAppearance.Controls.Add(lblThemeTitle);
            grpAppearance.Controls.Add(radioDark);
            grpAppearance.Controls.Add(radioLight);
            grpAppearance.Controls.Add(sep1);
            grpAppearance.Controls.Add(lblLangTitle);
            grpAppearance.Controls.Add(cboLanguage);

            grpBehavior.Text     = "Behavior";
            grpBehavior.Location = new Point(8, 148);
            grpBehavior.Size     = new Size(426, 110);
            grpBehavior.Font     = new Font("Segoe UI", 8.5f, FontStyle.Bold);

            chkSettingsAutostart.Text     = "Launch on startup";
            chkSettingsAutostart.Location = new Point(12, 28);
            chkSettingsAutostart.Size     = new Size(200, 20);
            chkSettingsAutostart.Font     = new Font("Segoe UI", 8.5f, FontStyle.Regular);

            chkSettingsMinToTray.Text     = "Minimize to tray on close";
            chkSettingsMinToTray.Location = new Point(12, 54);
            chkSettingsMinToTray.Size     = new Size(230, 20);
            chkSettingsMinToTray.Font     = new Font("Segoe UI", 8.5f, FontStyle.Regular);

            chkSettingsNotify.Text     = "Show tray notifications";
            chkSettingsNotify.Location = new Point(12, 80);
            chkSettingsNotify.Size     = new Size(200, 20);
            chkSettingsNotify.Font     = new Font("Segoe UI", 8.5f, FontStyle.Regular);

            grpBehavior.Controls.Add(chkSettingsAutostart);
            grpBehavior.Controls.Add(chkSettingsMinToTray);
            grpBehavior.Controls.Add(chkSettingsNotify);

            tabGeneral.Controls.Add(grpAppearance);
            tabGeneral.Controls.Add(grpBehavior);

            // ─── VIBRANCE TAB ─────────────────────────────────────────────
            tabVibrance.Text    = "Vibrance";
            tabVibrance.Padding = new Padding(10);

            lblDesktopDefault.Text      = "DESKTOP VIBRANCE DEFAULT";
            lblDesktopDefault.Font      = new Font("Segoe UI", 8f, FontStyle.Bold);
            lblDesktopDefault.Location  = new Point(10, 14);
            lblDesktopDefault.AutoSize  = true;
            lblDesktopDefault.Tag       = "accent";

            lblDesktopNote.Text      = "Applied when no game profile is active";
            lblDesktopNote.Font      = new Font("Segoe UI", 8f, FontStyle.Italic);
            lblDesktopNote.Location  = new Point(10, 34);
            lblDesktopNote.AutoSize  = true;
            lblDesktopNote.Tag       = "sub";

            trackDesktop.Location  = new Point(10, 58);
            trackDesktop.Size      = new Size(340, 32);
            trackDesktop.TickStyle = TickStyle.None;
            trackDesktop.Scroll   += trackDesktop_Scroll;

            labelDesktopVal.Text      = "—";
            labelDesktopVal.Font      = new Font("Segoe UI", 12f, FontStyle.Bold);
            labelDesktopVal.Location  = new Point(356, 60);
            labelDesktopVal.Size      = new Size(56, 26);
            labelDesktopVal.TextAlign = ContentAlignment.MiddleLeft;
            labelDesktopVal.Tag       = "accent";

            btnResetDesktop.Text      = "Reset";
            btnResetDesktop.Location  = new Point(10, 96);
            btnResetDesktop.Size      = new Size(80, 26);
            btnResetDesktop.FlatStyle = FlatStyle.Flat;
            btnResetDesktop.FlatAppearance.BorderSize = 1;
            btnResetDesktop.Click    += btnResetDesktop_Click;

            sep2.Location = new Point(10, 132);
            sep2.Size     = new Size(416, 1);
            sep2.Tag      = "separator";

            lblMonitorSection.Text      = "MONITOR CONFIGURATION";
            lblMonitorSection.Font      = new Font("Segoe UI", 8f, FontStyle.Bold);
            lblMonitorSection.Location  = new Point(10, 144);
            lblMonitorSection.AutoSize  = true;
            lblMonitorSection.Tag       = "accent";

            chkPrimaryOnly.Text     = "Apply vibrance to primary monitor only";
            chkPrimaryOnly.Location = new Point(10, 168);
            chkPrimaryOnly.Size     = new Size(300, 20);
            chkPrimaryOnly.Font     = new Font("Segoe UI", 8.5f, FontStyle.Regular);

            chkNoResize.Text     = "Never change resolution automatically";
            chkNoResize.Location = new Point(10, 194);
            chkNoResize.Size     = new Size(300, 20);
            chkNoResize.Font     = new Font("Segoe UI", 8.5f, FontStyle.Regular);

            tabVibrance.Controls.Add(lblDesktopDefault);
            tabVibrance.Controls.Add(lblDesktopNote);
            tabVibrance.Controls.Add(trackDesktop);
            tabVibrance.Controls.Add(labelDesktopVal);
            tabVibrance.Controls.Add(btnResetDesktop);
            tabVibrance.Controls.Add(sep2);
            tabVibrance.Controls.Add(lblMonitorSection);
            tabVibrance.Controls.Add(chkPrimaryOnly);
            tabVibrance.Controls.Add(chkNoResize);

            // ─── HOTKEY TAB ───────────────────────────────────────────────
            tabHotkey.Text    = "Hotkey";
            tabHotkey.Padding = new Padding(10);

            lblHotkeySection.Text      = "GLOBAL VIBRANCE TOGGLE";
            lblHotkeySection.Font      = new Font("Segoe UI", 8f, FontStyle.Bold);
            lblHotkeySection.Location  = new Point(10, 14);
            lblHotkeySection.AutoSize  = true;
            lblHotkeySection.Tag       = "accent";

            lblHotkeyNote.Text      = "Press this key anywhere to toggle vibrance on/off";
            lblHotkeyNote.Font      = new Font("Segoe UI", 8f, FontStyle.Italic);
            lblHotkeyNote.Location  = new Point(10, 34);
            lblHotkeyNote.AutoSize  = true;
            lblHotkeyNote.Tag       = "sub";

            btnHotkeyPicker.Text      = "F9";
            btnHotkeyPicker.Font      = new Font("Segoe UI", 11f, FontStyle.Bold);
            btnHotkeyPicker.Location  = new Point(10, 62);
            btnHotkeyPicker.Size      = new Size(120, 40);
            btnHotkeyPicker.FlatStyle = FlatStyle.Flat;
            btnHotkeyPicker.FlatAppearance.BorderSize = 2;
            btnHotkeyPicker.Cursor    = Cursors.Hand;
            btnHotkeyPicker.Click    += btnHotkeyPicker_Click;

            btnClearHotkey.Text      = "Clear";
            btnClearHotkey.Location  = new Point(140, 72);
            btnClearHotkey.Size      = new Size(70, 26);
            btnClearHotkey.FlatStyle = FlatStyle.Flat;
            btnClearHotkey.FlatAppearance.BorderSize = 1;
            btnClearHotkey.Tag       = "danger";
            btnClearHotkey.Click    += btnClearHotkey_Click;

            sep3.Location = new Point(10, 116);
            sep3.Size     = new Size(416, 1);
            sep3.Tag      = "separator";

            lblHotkeyBehavior.Text      = "HOTKEY BEHAVIOR";
            lblHotkeyBehavior.Font      = new Font("Segoe UI", 8f, FontStyle.Bold);
            lblHotkeyBehavior.Location  = new Point(10, 128);
            lblHotkeyBehavior.AutoSize  = true;
            lblHotkeyBehavior.Tag       = "accent";

            radioToggle.Text     = "Toggle — Press once to enable, press again to disable";
            radioToggle.Location = new Point(10, 152);
            radioToggle.Size     = new Size(400, 20);
            radioToggle.Checked  = true;

            radioHold.Text     = "Hold — Vibrance active only while key is held";
            radioHold.Location = new Point(10, 178);
            radioHold.Size     = new Size(400, 20);
            radioHold.Enabled  = false;

            tabHotkey.Controls.Add(lblHotkeySection);
            tabHotkey.Controls.Add(lblHotkeyNote);
            tabHotkey.Controls.Add(btnHotkeyPicker);
            tabHotkey.Controls.Add(btnClearHotkey);
            tabHotkey.Controls.Add(sep3);
            tabHotkey.Controls.Add(lblHotkeyBehavior);
            tabHotkey.Controls.Add(radioToggle);
            tabHotkey.Controls.Add(radioHold);

            // ─── ABOUT TAB ────────────────────────────────────────────────
            tabAbout.Text    = "About";
            tabAbout.Padding = new Padding(10);

            panelAboutLogo.Location = new Point(10, 14);
            panelAboutLogo.Size     = new Size(60, 60);
            panelAboutLogo.Paint   += (s, e) =>
            {
                var g    = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                var rect = new Rectangle(0, 0, 58, 58);
                var path = new System.Drawing.Drawing2D.GraphicsPath();
                int r = 10, d = r * 2;
                path.AddArc(rect.X, rect.Y, d, d, 180, 90);
                path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
                path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
                path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
                path.CloseFigure();
                using (var grad = new System.Drawing.Drawing2D.LinearGradientBrush(rect,
                    Color.FromArgb(100, 30, 220), Color.FromArgb(0, 175, 225),
                    System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal))
                    g.FillPath(grad, path);
                using (var font = new Font("Segoe UI", 28f, FontStyle.Bold))
                using (var brush = new SolidBrush(Color.FromArgb(240, 248, 255)))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString("S", font, brush, new RectangleF(0, 0, 60, 60), sf);
                }
            };

            lblAboutVersion.Text      = "Spectra v1.7.0";
            lblAboutVersion.Font      = new Font("Segoe UI", 16f, FontStyle.Bold);
            lblAboutVersion.Location  = new Point(82, 14);
            lblAboutVersion.AutoSize  = true;

            lblAboutDesc.Text      = "Professional Digital Vibrance Controller";
            lblAboutDesc.Font      = new Font("Segoe UI", 9f, FontStyle.Regular);
            lblAboutDesc.Location  = new Point(82, 44);
            lblAboutDesc.AutoSize  = true;
            lblAboutDesc.Tag       = "sub";

            sep4.Location = new Point(10, 88);
            sep4.Size     = new Size(416, 1);
            sep4.Tag      = "separator";

            lblAboutGpu.Text      = "✓ NVIDIA Digital Vibrance Control (NVAPI)\r\n✓ AMD Saturation Control (ADL)";
            lblAboutGpu.Font      = new Font("Segoe UI", 8.5f, FontStyle.Regular);
            lblAboutGpu.Location  = new Point(10, 102);
            lblAboutGpu.AutoSize  = true;

            btnGitHub.Text      = "★  View on GitHub";
            btnGitHub.Font      = new Font("Segoe UI", 9f, FontStyle.Regular);
            btnGitHub.Location  = new Point(10, 160);
            btnGitHub.Size      = new Size(160, 32);
            btnGitHub.FlatStyle = FlatStyle.Flat;
            btnGitHub.FlatAppearance.BorderSize = 1;
            btnGitHub.Tag       = "accent";
            btnGitHub.Click    += btnGitHub_Click;

            tabAbout.Controls.Add(panelAboutLogo);
            tabAbout.Controls.Add(lblAboutVersion);
            tabAbout.Controls.Add(lblAboutDesc);
            tabAbout.Controls.Add(sep4);
            tabAbout.Controls.Add(lblAboutGpu);
            tabAbout.Controls.Add(btnGitHub);

            // ── Bottom buttons ────────────────────────────────────────────
            btnApply.Text      = "Apply";
            btnApply.Location  = new Point(256, 462);
            btnApply.Size      = new Size(100, 34);
            btnApply.FlatStyle = FlatStyle.Flat;
            btnApply.FlatAppearance.BorderSize = 0;
            btnApply.Font      = new Font("Segoe UI", 9f, FontStyle.Bold);
            btnApply.Click    += btnApply_Click;

            btnCancel.Text      = "Cancel";
            btnCancel.Location  = new Point(362, 462);
            btnCancel.Size      = new Size(96, 34);
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderSize = 1;
            btnCancel.Font      = new Font("Segoe UI", 9f, FontStyle.Regular);
            btnCancel.Click    += btnCancel_Click;

            // ── Assemble ──────────────────────────────────────────────────
            Controls.Add(panelSettingsHeader);
            Controls.Add(tabControl);
            Controls.Add(btnApply);
            Controls.Add(btnCancel);

            ((System.ComponentModel.ISupportInitialize)trackDesktop).EndInit();
            panelSettingsHeader.ResumeLayout(false);
            tabControl.ResumeLayout(false);
            ResumeLayout(false);
        }

        // ── Fields ────────────────────────────────────────────────────────
        private Panel      panelSettingsHeader;
        private Label      labelSettingsTitle;
        private TabControl tabControl;
        private TabPage    tabGeneral, tabVibrance, tabHotkey, tabAbout;

        // General
        private GroupBox   grpAppearance;
        private Label      lblThemeTitle;
        private RadioButton radioDark, radioLight;
        private Panel      sep1;
        private Label      lblLangTitle;
        private ComboBox   cboLanguage;
        private GroupBox   grpBehavior;
        private CheckBox   chkSettingsAutostart, chkSettingsMinToTray, chkSettingsNotify;

        // Vibrance
        private Label      lblDesktopDefault, lblDesktopNote;
        private TrackBar   trackDesktop;
        private Label      labelDesktopVal;
        private Button     btnResetDesktop;
        private Panel      sep2;
        private Label      lblMonitorSection;
        private CheckBox   chkPrimaryOnly, chkNoResize;

        // Hotkey
        private Label      lblHotkeySection, lblHotkeyNote;
        private Button     btnHotkeyPicker;
        private Button     btnClearHotkey;
        private Panel      sep3;
        private Label      lblHotkeyBehavior;
        private RadioButton radioToggle, radioHold;

        // About
        private Panel      panelAboutLogo;
        private Label      lblAboutVersion, lblAboutDesc, lblAboutGpu;
        private Panel      sep4;
        private Button     btnGitHub;

        // Bottom
        private Button     btnApply, btnCancel;
    }
}
