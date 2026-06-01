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
            // Header
            panelSettingsHeader = new Panel();
            labelSettingsTitle  = new Label();

            // TabControl
            tabControl  = new TabControl();
            tabGeneral  = new TabPage();
            tabVibrance = new TabPage();
            tabHotkey   = new TabPage();
            tabAbout    = new TabPage();

            // ── General tab ───────────────────────────────────────────────
            lblAppearanceTitle    = new Label();
            sepAppearance         = new Panel();
            lblThemeTitle         = new Label();
            radioDark             = new RadioButton();
            radioLight            = new RadioButton();
            lblLangTitle          = new Label();
            cboLanguage           = new ComboBox();
            lblBehaviorTitle      = new Label();
            sepBehavior           = new Panel();
            chkSettingsAutostart  = new CheckBox();
            chkSettingsMinToTray  = new CheckBox();
            chkSettingsNotify     = new CheckBox();

            // ── Vibrance tab ──────────────────────────────────────────────
            lblVibDefault    = new Label();
            sepVib1          = new Panel();
            lblVibNote       = new Label();
            trackDesktop     = new TrackBar();
            labelDesktopVal  = new Label();
            btnResetDesktop  = new Button();
            lblMonitorTitle  = new Label();
            sepVib2          = new Panel();
            chkPrimaryOnly   = new CheckBox();
            chkNoResize      = new CheckBox();

            // ── Hotkey tab ────────────────────────────────────────────────
            lblHotkeyTitle   = new Label();
            sepHotkey1       = new Panel();
            lblHotkeyNote    = new Label();
            btnHotkeyPicker  = new Button();
            btnClearHotkey   = new Button();
            lblHotkeyBehTitle= new Label();
            sepHotkey2       = new Panel();
            radioToggle      = new RadioButton();
            radioHold        = new RadioButton();

            // ── About tab ─────────────────────────────────────────────────
            panelAboutLogo    = new Panel();
            lblAboutVersion   = new Label();
            lblAboutDesc      = new Label();
            sepAbout          = new Panel();
            lblAboutGpu       = new Label();
            btnGitHub         = new Button();

            // Bottom buttons
            btnApply  = new Button();
            btnCancel = new Button();

            SuspendLayout();
            panelSettingsHeader.SuspendLayout();
            tabControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackDesktop).BeginInit();

            // ── FORM ─────────────────────────────────────────────────────
            AutoScaleDimensions = new SizeF(6f, 13f);
            AutoScaleMode       = AutoScaleMode.Font;
            ClientSize          = new Size(480, 530);
            FormBorderStyle     = FormBorderStyle.FixedSingle;
            MaximizeBox         = false;
            StartPosition       = FormStartPosition.CenterParent;
            Text                = "Spectra — Settings";
            Name                = "SettingsForm";

            // ── HEADER ───────────────────────────────────────────────────
            panelSettingsHeader.Location = new Point(0, 0);
            panelSettingsHeader.Size     = new Size(480, 52);
            panelSettingsHeader.Paint   += panelSettingsHeader_Paint;

            labelSettingsTitle.Text      = "⚙  SETTINGS";
            labelSettingsTitle.Font      = new Font("Segoe UI", 14f, FontStyle.Bold);
            labelSettingsTitle.ForeColor = Color.White;
            labelSettingsTitle.BackColor = Color.Transparent;
            labelSettingsTitle.Location  = new Point(14, 12);
            labelSettingsTitle.AutoSize  = true;
            panelSettingsHeader.Controls.Add(labelSettingsTitle);

            // ── TAB CONTROL ──────────────────────────────────────────────
            tabControl.Location    = new Point(10, 60);
            tabControl.Size        = new Size(460, 418);
            tabControl.DrawMode    = TabDrawMode.OwnerDrawFixed;
            tabControl.ItemSize    = new Size(114, 28);
            tabControl.SizeMode    = TabSizeMode.Fixed;
            tabControl.DrawItem   += tabControl_DrawItem;
            tabControl.TabPages.AddRange(new TabPage[] { tabGeneral, tabVibrance, tabHotkey, tabAbout });

            // ── GENERAL TAB ──────────────────────────────────────────────
            tabGeneral.Text    = "General";
            tabGeneral.Padding = new Padding(14, 12, 14, 12);
            tabGeneral.UseVisualStyleBackColor = false;

            // --- Appearance section ---
            lblAppearanceTitle.Text      = "APPEARANCE";
            lblAppearanceTitle.Font      = new Font("Segoe UI", 8f, FontStyle.Bold);
            lblAppearanceTitle.Location  = new Point(0, 0);
            lblAppearanceTitle.AutoSize  = true;

            sepAppearance.Location = new Point(0, 20);
            sepAppearance.Size     = new Size(430, 1);

            lblThemeTitle.Text      = "Theme";
            lblThemeTitle.Font      = new Font("Segoe UI", 9f);
            lblThemeTitle.Location  = new Point(0, 30);
            lblThemeTitle.AutoSize  = true;

            radioDark.Text             = "Dark";
            radioDark.Font             = new Font("Segoe UI", 9f);
            radioDark.Location         = new Point(90, 28);
            radioDark.Size             = new Size(70, 22);
            radioDark.CheckedChanged  += radioDark_CheckedChanged;

            radioLight.Text             = "Light";
            radioLight.Font             = new Font("Segoe UI", 9f);
            radioLight.Location         = new Point(164, 28);
            radioLight.Size             = new Size(70, 22);
            radioLight.CheckedChanged  += radioLight_CheckedChanged;

            lblLangTitle.Text      = "Language";
            lblLangTitle.Font      = new Font("Segoe UI", 9f);
            lblLangTitle.Location  = new Point(0, 60);
            lblLangTitle.AutoSize  = true;

            cboLanguage.Location          = new Point(90, 58);
            cboLanguage.Size              = new Size(170, 24);
            cboLanguage.DropDownStyle     = ComboBoxStyle.DropDownList;
            cboLanguage.FlatStyle         = FlatStyle.Flat;
            cboLanguage.SelectedIndexChanged += cboLanguage_SelectedIndexChanged;

            // --- Behavior section ---
            lblBehaviorTitle.Text      = "BEHAVIOR";
            lblBehaviorTitle.Font      = new Font("Segoe UI", 8f, FontStyle.Bold);
            lblBehaviorTitle.Location  = new Point(0, 100);
            lblBehaviorTitle.AutoSize  = true;

            sepBehavior.Location = new Point(0, 120);
            sepBehavior.Size     = new Size(430, 1);

            chkSettingsAutostart.Text     = "Launch on startup";
            chkSettingsAutostart.Font     = new Font("Segoe UI", 9f);
            chkSettingsAutostart.Location = new Point(0, 130);
            chkSettingsAutostart.Size     = new Size(250, 22);

            chkSettingsMinToTray.Text     = "Minimize to tray on close";
            chkSettingsMinToTray.Font     = new Font("Segoe UI", 9f);
            chkSettingsMinToTray.Location = new Point(0, 158);
            chkSettingsMinToTray.Size     = new Size(270, 22);

            chkSettingsNotify.Text     = "Show tray notifications";
            chkSettingsNotify.Font     = new Font("Segoe UI", 9f);
            chkSettingsNotify.Location = new Point(0, 186);
            chkSettingsNotify.Size     = new Size(250, 22);

            tabGeneral.Controls.Add(lblAppearanceTitle);
            tabGeneral.Controls.Add(sepAppearance);
            tabGeneral.Controls.Add(lblThemeTitle);
            tabGeneral.Controls.Add(radioDark);
            tabGeneral.Controls.Add(radioLight);
            tabGeneral.Controls.Add(lblLangTitle);
            tabGeneral.Controls.Add(cboLanguage);
            tabGeneral.Controls.Add(lblBehaviorTitle);
            tabGeneral.Controls.Add(sepBehavior);
            tabGeneral.Controls.Add(chkSettingsAutostart);
            tabGeneral.Controls.Add(chkSettingsMinToTray);
            tabGeneral.Controls.Add(chkSettingsNotify);

            // ── VIBRANCE TAB ─────────────────────────────────────────────
            tabVibrance.Text    = "Vibrance";
            tabVibrance.Padding = new Padding(14, 12, 14, 12);
            tabVibrance.UseVisualStyleBackColor = false;

            lblVibDefault.Text      = "DESKTOP VIBRANCE DEFAULT";
            lblVibDefault.Font      = new Font("Segoe UI", 8f, FontStyle.Bold);
            lblVibDefault.Location  = new Point(0, 0);
            lblVibDefault.AutoSize  = true;

            sepVib1.Location = new Point(0, 20);
            sepVib1.Size     = new Size(430, 1);

            lblVibNote.Text      = "Applied when no game profile is active";
            lblVibNote.Font      = new Font("Segoe UI", 8.5f, FontStyle.Italic);
            lblVibNote.Location  = new Point(0, 28);
            lblVibNote.AutoSize  = true;

            trackDesktop.Location  = new Point(0, 50);
            trackDesktop.Size      = new Size(340, 32);
            trackDesktop.TickStyle = TickStyle.None;
            trackDesktop.Scroll   += trackDesktop_Scroll;

            labelDesktopVal.Text      = "—";
            labelDesktopVal.Font      = new Font("Segoe UI", 13f, FontStyle.Bold);
            labelDesktopVal.Location  = new Point(346, 52);
            labelDesktopVal.Size      = new Size(60, 26);
            labelDesktopVal.TextAlign = ContentAlignment.MiddleLeft;

            btnResetDesktop.Text      = "Reset";
            btnResetDesktop.Font      = new Font("Segoe UI", 8.5f);
            btnResetDesktop.Location  = new Point(0, 88);
            btnResetDesktop.Size      = new Size(80, 26);
            btnResetDesktop.FlatStyle = FlatStyle.Flat;
            btnResetDesktop.FlatAppearance.BorderSize = 1;
            btnResetDesktop.Click    += btnResetDesktop_Click;

            lblMonitorTitle.Text      = "MONITOR CONFIGURATION";
            lblMonitorTitle.Font      = new Font("Segoe UI", 8f, FontStyle.Bold);
            lblMonitorTitle.Location  = new Point(0, 130);
            lblMonitorTitle.AutoSize  = true;

            sepVib2.Location = new Point(0, 150);
            sepVib2.Size     = new Size(430, 1);

            chkPrimaryOnly.Text     = "Apply vibrance to primary monitor only";
            chkPrimaryOnly.Font     = new Font("Segoe UI", 9f);
            chkPrimaryOnly.Location = new Point(0, 160);
            chkPrimaryOnly.Size     = new Size(320, 22);

            chkNoResize.Text     = "Never change resolution automatically";
            chkNoResize.Font     = new Font("Segoe UI", 9f);
            chkNoResize.Location = new Point(0, 188);
            chkNoResize.Size     = new Size(320, 22);

            tabVibrance.Controls.Add(lblVibDefault);
            tabVibrance.Controls.Add(sepVib1);
            tabVibrance.Controls.Add(lblVibNote);
            tabVibrance.Controls.Add(trackDesktop);
            tabVibrance.Controls.Add(labelDesktopVal);
            tabVibrance.Controls.Add(btnResetDesktop);
            tabVibrance.Controls.Add(lblMonitorTitle);
            tabVibrance.Controls.Add(sepVib2);
            tabVibrance.Controls.Add(chkPrimaryOnly);
            tabVibrance.Controls.Add(chkNoResize);

            // ── HOTKEY TAB ────────────────────────────────────────────────
            tabHotkey.Text    = "Hotkey";
            tabHotkey.Padding = new Padding(14, 12, 14, 12);
            tabHotkey.UseVisualStyleBackColor = false;

            lblHotkeyTitle.Text      = "GLOBAL TOGGLE HOTKEY";
            lblHotkeyTitle.Font      = new Font("Segoe UI", 8f, FontStyle.Bold);
            lblHotkeyTitle.Location  = new Point(0, 0);
            lblHotkeyTitle.AutoSize  = true;

            sepHotkey1.Location = new Point(0, 20);
            sepHotkey1.Size     = new Size(430, 1);

            lblHotkeyNote.Text      = "Press this key anywhere to toggle vibrance on/off instantly";
            lblHotkeyNote.Font      = new Font("Segoe UI", 8.5f, FontStyle.Italic);
            lblHotkeyNote.Location  = new Point(0, 28);
            lblHotkeyNote.AutoSize  = true;

            btnHotkeyPicker.Text      = "F9";
            btnHotkeyPicker.Font      = new Font("Segoe UI", 13f, FontStyle.Bold);
            btnHotkeyPicker.Location  = new Point(0, 56);
            btnHotkeyPicker.Size      = new Size(130, 44);
            btnHotkeyPicker.FlatStyle = FlatStyle.Flat;
            btnHotkeyPicker.FlatAppearance.BorderSize = 2;
            btnHotkeyPicker.Cursor    = Cursors.Hand;
            btnHotkeyPicker.Click    += btnHotkeyPicker_Click;

            btnClearHotkey.Text      = "Clear";
            btnClearHotkey.Font      = new Font("Segoe UI", 9f);
            btnClearHotkey.Location  = new Point(142, 68);
            btnClearHotkey.Size      = new Size(72, 28);
            btnClearHotkey.FlatStyle = FlatStyle.Flat;
            btnClearHotkey.FlatAppearance.BorderSize = 1;
            btnClearHotkey.Click    += btnClearHotkey_Click;

            lblHotkeyBehTitle.Text      = "HOTKEY BEHAVIOR";
            lblHotkeyBehTitle.Font      = new Font("Segoe UI", 8f, FontStyle.Bold);
            lblHotkeyBehTitle.Location  = new Point(0, 120);
            lblHotkeyBehTitle.AutoSize  = true;

            sepHotkey2.Location = new Point(0, 140);
            sepHotkey2.Size     = new Size(430, 1);

            radioToggle.Text     = "Toggle — Press once to activate, press again to deactivate";
            radioToggle.Font     = new Font("Segoe UI", 9f);
            radioToggle.Location = new Point(0, 150);
            radioToggle.Size     = new Size(420, 22);
            radioToggle.Checked  = true;

            radioHold.Text     = "Hold — Active only while key is held (coming soon)";
            radioHold.Font     = new Font("Segoe UI", 9f);
            radioHold.Location = new Point(0, 178);
            radioHold.Size     = new Size(420, 22);
            radioHold.Enabled  = false;

            tabHotkey.Controls.Add(lblHotkeyTitle);
            tabHotkey.Controls.Add(sepHotkey1);
            tabHotkey.Controls.Add(lblHotkeyNote);
            tabHotkey.Controls.Add(btnHotkeyPicker);
            tabHotkey.Controls.Add(btnClearHotkey);
            tabHotkey.Controls.Add(lblHotkeyBehTitle);
            tabHotkey.Controls.Add(sepHotkey2);
            tabHotkey.Controls.Add(radioToggle);
            tabHotkey.Controls.Add(radioHold);

            // ── ABOUT TAB ─────────────────────────────────────────────────
            tabAbout.Text    = "About";
            tabAbout.Padding = new Padding(14, 12, 14, 12);
            tabAbout.UseVisualStyleBackColor = false;

            // Logo panel (painted in code)
            panelAboutLogo.Location = new Point(0, 0);
            panelAboutLogo.Size     = new Size(64, 64);
            panelAboutLogo.Paint   += panelAboutLogo_Paint;

            lblAboutVersion.Text      = "Spectra v1.7.0";
            lblAboutVersion.Font      = new Font("Segoe UI", 15f, FontStyle.Bold);
            lblAboutVersion.Location  = new Point(80, 4);
            lblAboutVersion.AutoSize  = true;

            lblAboutDesc.Text      = "Professional Digital Vibrance Controller";
            lblAboutDesc.Font      = new Font("Segoe UI", 9f);
            lblAboutDesc.Location  = new Point(80, 38);
            lblAboutDesc.AutoSize  = true;

            sepAbout.Location = new Point(0, 76);
            sepAbout.Size     = new Size(430, 1);

            lblAboutGpu.Text      = "Supported GPUs:\r\n\r\n✓ NVIDIA — Digital Vibrance Control (NVAPI)\r\n✓ AMD — Saturation Control (ADL 32/64-bit)";
            lblAboutGpu.Font      = new Font("Segoe UI", 9f);
            lblAboutGpu.Location  = new Point(0, 86);
            lblAboutGpu.AutoSize  = true;

            btnGitHub.Text      = "★  View on GitHub";
            btnGitHub.Font      = new Font("Segoe UI", 9f);
            btnGitHub.Location  = new Point(0, 176);
            btnGitHub.Size      = new Size(170, 34);
            btnGitHub.FlatStyle = FlatStyle.Flat;
            btnGitHub.FlatAppearance.BorderSize = 1;
            btnGitHub.Cursor    = Cursors.Hand;
            btnGitHub.Click    += btnGitHub_Click;

            tabAbout.Controls.Add(panelAboutLogo);
            tabAbout.Controls.Add(lblAboutVersion);
            tabAbout.Controls.Add(lblAboutDesc);
            tabAbout.Controls.Add(sepAbout);
            tabAbout.Controls.Add(lblAboutGpu);
            tabAbout.Controls.Add(btnGitHub);

            // ── BOTTOM BUTTONS ────────────────────────────────────────────
            btnApply.Text      = "Apply";
            btnApply.Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            btnApply.Location  = new Point(264, 486);
            btnApply.Size      = new Size(100, 34);
            btnApply.FlatStyle = FlatStyle.Flat;
            btnApply.FlatAppearance.BorderSize = 0;
            btnApply.Cursor    = Cursors.Hand;
            btnApply.Click    += btnApply_Click;

            btnCancel.Text      = "Cancel";
            btnCancel.Font      = new Font("Segoe UI", 9.5f);
            btnCancel.Location  = new Point(370, 486);
            btnCancel.Size      = new Size(100, 34);
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderSize = 1;
            btnCancel.Cursor    = Cursors.Hand;
            btnCancel.Click    += btnCancel_Click;

            Controls.Add(panelSettingsHeader);
            Controls.Add(tabControl);
            Controls.Add(btnApply);
            Controls.Add(btnCancel);

            ((System.ComponentModel.ISupportInitialize)trackDesktop).EndInit();
            panelSettingsHeader.ResumeLayout(false);
            tabControl.ResumeLayout(false);
            ResumeLayout(false);
        }

        // ── FIELD DECLARATIONS ─────────────────────────────────────────────
        private Panel      panelSettingsHeader;
        private Label      labelSettingsTitle;
        private TabControl tabControl;
        private TabPage    tabGeneral, tabVibrance, tabHotkey, tabAbout;

        // General
        private Label       lblAppearanceTitle;
        private Panel       sepAppearance;
        private Label       lblThemeTitle;
        private RadioButton radioDark, radioLight;
        private Label       lblLangTitle;
        private ComboBox    cboLanguage;
        private Label       lblBehaviorTitle;
        private Panel       sepBehavior;
        private CheckBox    chkSettingsAutostart, chkSettingsMinToTray, chkSettingsNotify;

        // Vibrance
        private Label       lblVibDefault, lblVibNote;
        private Panel       sepVib1;
        private TrackBar    trackDesktop;
        private Label       labelDesktopVal;
        private Button      btnResetDesktop;
        private Label       lblMonitorTitle;
        private Panel       sepVib2;
        private CheckBox    chkPrimaryOnly, chkNoResize;

        // Hotkey
        private Label       lblHotkeyTitle, lblHotkeyNote;
        private Panel       sepHotkey1;
        private Button      btnHotkeyPicker, btnClearHotkey;
        private Label       lblHotkeyBehTitle;
        private Panel       sepHotkey2;
        private RadioButton radioToggle, radioHold;

        // About
        private Panel  panelAboutLogo;
        private Label  lblAboutVersion, lblAboutDesc, lblAboutGpu;
        private Panel  sepAbout;
        private Button btnGitHub;

        // Bottom
        private Button btnApply, btnCancel;
    }
}
