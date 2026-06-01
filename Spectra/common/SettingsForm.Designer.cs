using System.Drawing;
using System.Windows.Forms;
using Spectra.UI;

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
            panelHeader      = new Panel();
            lblSettingsTitle = new Label();

            tabControl  = new TabControl();
            tabBehavior = new TabPage();
            tabDisplay  = new TabPage();
            tabData     = new TabPage();
            tabAbout    = new TabPage();

            // ── Behavior tab ──────────────────────────────────────────────
            lblStartupSection  = new Label();
            sepStartup         = new Panel();
            chkAutostart       = new CheckBox();
            chkMinToTray       = new CheckBox();
            chkNotifications   = new CheckBox();
            lblDelaySection    = new Label();
            sepDelay           = new Panel();
            lblDelayNote       = new Label();
            numDelay           = new NumericUpDown();
            lblDelayMs         = new Label();

            // ── Display tab ───────────────────────────────────────────────
            lblMonitorSection  = new Label();
            sepMonitor         = new Panel();
            lblMonitorTarget   = new Label();
            cboMonitorTarget   = new ComboBox();
            lblResSection      = new Label();
            sepRes             = new Panel();
            chkResetOnExit     = new CheckBox();
            chkNeverResize     = new CheckBox();

            // ── Data tab ──────────────────────────────────────────────────
            lblProfileSection  = new Label();
            sepProfile         = new Panel();
            lblProfileCount    = new Label();
            lblProfileCountVal = new Label();
            btnExport          = new Button();
            btnImport          = new Button();
            btnClearProfiles   = new Button();
            lblDataSection     = new Label();
            sepData            = new Panel();
            lblDataNote        = new Label();
            btnResetAll        = new Button();
            btnOpenLog         = new Button();

            // ── About tab ─────────────────────────────────────────────────
            panelLogo       = new Panel();
            lblAboutName    = new Label();
            lblAboutVersion = new Label();
            lblAboutDesc    = new Label();
            sepAbout        = new Panel();
            lblSupportTitle = new Label();
            lblGpuLine1     = new Label();
            lblGpuLine2     = new Label();
            sepAbout2       = new Panel();
            lblSysTitle     = new Label();
            lblSysInfo      = new Label();
            btnGitHub       = new Button();
            btnOpenLogShort = new Button();

            btnClose = new Button();

            SuspendLayout();
            panelHeader.SuspendLayout();
            tabControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numDelay).BeginInit();

            // ── FORM ─────────────────────────────────────────────────────
            AutoScaleDimensions = new SizeF(6f, 13f);
            AutoScaleMode       = AutoScaleMode.Font;
            ClientSize          = new Size(480, 520);
            FormBorderStyle     = FormBorderStyle.FixedSingle;
            MaximizeBox         = false;
            StartPosition       = FormStartPosition.CenterParent;
            BackColor           = ThemeManager.Bg;
            Text                = "Spectra — Settings";
            Name                = "SettingsForm";

            // ── HEADER ───────────────────────────────────────────────────
            panelHeader.Location = new Point(0, 0);
            panelHeader.Size     = new Size(480, 52);
            panelHeader.Paint   += panelHeader_Paint;

            lblSettingsTitle.Text      = "⚙  SETTINGS";
            lblSettingsTitle.Font      = new Font("Segoe UI", 14f, FontStyle.Bold);
            lblSettingsTitle.ForeColor = Color.White;
            lblSettingsTitle.BackColor = Color.Transparent;
            lblSettingsTitle.Location  = new Point(14, 12);
            lblSettingsTitle.AutoSize  = true;
            panelHeader.Controls.Add(lblSettingsTitle);

            // ── TAB CONTROL ──────────────────────────────────────────────
            tabControl.Location  = new Point(10, 60);
            tabControl.Size      = new Size(460, 406);
            tabControl.DrawMode  = TabDrawMode.OwnerDrawFixed;
            tabControl.ItemSize  = new Size(114, 28);
            tabControl.SizeMode  = TabSizeMode.Fixed;
            tabControl.DrawItem += tabControl_DrawItem;
            tabControl.TabPages.AddRange(new TabPage[] { tabBehavior, tabDisplay, tabData, tabAbout });

            // Helper — page setup
            foreach (TabPage tp in tabControl.TabPages)
            {
                tp.BackColor = ThemeManager.Surface;
                tp.Padding   = new Padding(16, 14, 16, 14);
                tp.UseVisualStyleBackColor = false;
            }

            tabBehavior.Text = "Behavior";
            tabDisplay.Text  = "Display";
            tabData.Text     = "Profiles";
            tabAbout.Text    = "About";

            // ── BEHAVIOR TAB ─────────────────────────────────────────────
            lblStartupSection.Text      = "STARTUP & BEHAVIOR";
            lblStartupSection.Font      = new Font("Segoe UI", 8f, FontStyle.Bold);
            lblStartupSection.ForeColor = ThemeManager.Accent;
            lblStartupSection.BackColor = Color.Transparent;
            lblStartupSection.Location  = new Point(0, 0);
            lblStartupSection.AutoSize  = true;

            sepStartup.Location  = new Point(0, 20);
            sepStartup.Size      = new Size(428, 1);
            sepStartup.BackColor = ThemeManager.Border;

            chkAutostart.Text     = "Launch on startup";
            chkAutostart.Font     = new Font("Segoe UI", 9f);
            chkAutostart.ForeColor= ThemeManager.Text;
            chkAutostart.BackColor= Color.Transparent;
            chkAutostart.Location = new Point(0, 30);
            chkAutostart.Size     = new Size(260, 22);

            chkMinToTray.Text     = "Minimize to tray when closed";
            chkMinToTray.Font     = new Font("Segoe UI", 9f);
            chkMinToTray.ForeColor= ThemeManager.Text;
            chkMinToTray.BackColor= Color.Transparent;
            chkMinToTray.Location = new Point(0, 58);
            chkMinToTray.Size     = new Size(280, 22);

            chkNotifications.Text     = "Show tray notifications";
            chkNotifications.Font     = new Font("Segoe UI", 9f);
            chkNotifications.ForeColor= ThemeManager.Text;
            chkNotifications.BackColor= Color.Transparent;
            chkNotifications.Location = new Point(0, 86);
            chkNotifications.Size     = new Size(260, 22);

            lblDelaySection.Text      = "VIBRANCE APPLY DELAY";
            lblDelaySection.Font      = new Font("Segoe UI", 8f, FontStyle.Bold);
            lblDelaySection.ForeColor = ThemeManager.Accent;
            lblDelaySection.BackColor = Color.Transparent;
            lblDelaySection.Location  = new Point(0, 126);
            lblDelaySection.AutoSize  = true;

            sepDelay.Location  = new Point(0, 146);
            sepDelay.Size      = new Size(428, 1);
            sepDelay.BackColor = ThemeManager.Border;

            lblDelayNote.Text      = "Wait before applying vibrance when a game launches (ms)";
            lblDelayNote.Font      = new Font("Segoe UI", 8.5f, FontStyle.Italic);
            lblDelayNote.ForeColor = ThemeManager.TextSub;
            lblDelayNote.BackColor = Color.Transparent;
            lblDelayNote.Location  = new Point(0, 154);
            lblDelayNote.AutoSize  = true;

            numDelay.Location     = new Point(0, 178);
            numDelay.Size         = new Size(100, 24);
            numDelay.Font         = new Font("Segoe UI", 9f);
            numDelay.Minimum      = 0;
            numDelay.Maximum      = 5000;
            numDelay.Value        = 500;
            numDelay.Increment    = 100;
            numDelay.BackColor    = ThemeManager.Surface2;
            numDelay.ForeColor    = ThemeManager.Text;

            lblDelayMs.Text      = "ms";
            lblDelayMs.Font      = new Font("Segoe UI", 9f);
            lblDelayMs.ForeColor = ThemeManager.TextSub;
            lblDelayMs.BackColor = Color.Transparent;
            lblDelayMs.Location  = new Point(108, 182);
            lblDelayMs.AutoSize  = true;

            tabBehavior.Controls.Add(lblStartupSection);
            tabBehavior.Controls.Add(sepStartup);
            tabBehavior.Controls.Add(chkAutostart);
            tabBehavior.Controls.Add(chkMinToTray);
            tabBehavior.Controls.Add(chkNotifications);
            tabBehavior.Controls.Add(lblDelaySection);
            tabBehavior.Controls.Add(sepDelay);
            tabBehavior.Controls.Add(lblDelayNote);
            tabBehavior.Controls.Add(numDelay);
            tabBehavior.Controls.Add(lblDelayMs);

            // ── DISPLAY TAB ───────────────────────────────────────────────
            lblMonitorSection.Text      = "MONITOR TARGET";
            lblMonitorSection.Font      = new Font("Segoe UI", 8f, FontStyle.Bold);
            lblMonitorSection.ForeColor = ThemeManager.Accent;
            lblMonitorSection.BackColor = Color.Transparent;
            lblMonitorSection.Location  = new Point(0, 0);
            lblMonitorSection.AutoSize  = true;

            sepMonitor.Location  = new Point(0, 20);
            sepMonitor.Size      = new Size(428, 1);
            sepMonitor.BackColor = ThemeManager.Border;

            lblMonitorTarget.Text      = "Apply vibrance to:";
            lblMonitorTarget.Font      = new Font("Segoe UI", 9f);
            lblMonitorTarget.ForeColor = ThemeManager.Text;
            lblMonitorTarget.BackColor = Color.Transparent;
            lblMonitorTarget.Location  = new Point(0, 30);
            lblMonitorTarget.AutoSize  = true;

            cboMonitorTarget.Location      = new Point(128, 28);
            cboMonitorTarget.Size          = new Size(200, 22);
            cboMonitorTarget.DropDownStyle = ComboBoxStyle.DropDownList;
            cboMonitorTarget.FlatStyle     = FlatStyle.Flat;
            cboMonitorTarget.Font          = new Font("Segoe UI", 9f);
            cboMonitorTarget.BackColor     = ThemeManager.Surface2;
            cboMonitorTarget.ForeColor     = ThemeManager.Text;

            lblResSection.Text      = "RESOLUTION BEHAVIOR";
            lblResSection.Font      = new Font("Segoe UI", 8f, FontStyle.Bold);
            lblResSection.ForeColor = ThemeManager.Accent;
            lblResSection.BackColor = Color.Transparent;
            lblResSection.Location  = new Point(0, 70);
            lblResSection.AutoSize  = true;

            sepRes.Location  = new Point(0, 90);
            sepRes.Size      = new Size(428, 1);
            sepRes.BackColor = ThemeManager.Border;

            chkNeverResize.Text     = "Never change resolution automatically";
            chkNeverResize.Font     = new Font("Segoe UI", 9f);
            chkNeverResize.ForeColor= ThemeManager.Text;
            chkNeverResize.BackColor= Color.Transparent;
            chkNeverResize.Location = new Point(0, 100);
            chkNeverResize.Size     = new Size(320, 22);

            chkResetOnExit.Text     = "Reset vibrance to default on exit";
            chkResetOnExit.Font     = new Font("Segoe UI", 9f);
            chkResetOnExit.ForeColor= ThemeManager.Text;
            chkResetOnExit.BackColor= Color.Transparent;
            chkResetOnExit.Location = new Point(0, 128);
            chkResetOnExit.Size     = new Size(300, 22);

            tabDisplay.Controls.Add(lblMonitorSection);
            tabDisplay.Controls.Add(sepMonitor);
            tabDisplay.Controls.Add(lblMonitorTarget);
            tabDisplay.Controls.Add(cboMonitorTarget);
            tabDisplay.Controls.Add(lblResSection);
            tabDisplay.Controls.Add(sepRes);
            tabDisplay.Controls.Add(chkNeverResize);
            tabDisplay.Controls.Add(chkResetOnExit);

            // ── DATA TAB ──────────────────────────────────────────────────
            lblProfileSection.Text      = "PROFILE MANAGEMENT";
            lblProfileSection.Font      = new Font("Segoe UI", 8f, FontStyle.Bold);
            lblProfileSection.ForeColor = ThemeManager.Accent;
            lblProfileSection.BackColor = Color.Transparent;
            lblProfileSection.Location  = new Point(0, 0);
            lblProfileSection.AutoSize  = true;

            sepProfile.Location  = new Point(0, 20);
            sepProfile.Size      = new Size(428, 1);
            sepProfile.BackColor = ThemeManager.Border;

            lblProfileCount.Text      = "Saved profiles:";
            lblProfileCount.Font      = new Font("Segoe UI", 9f);
            lblProfileCount.ForeColor = ThemeManager.Text;
            lblProfileCount.BackColor = Color.Transparent;
            lblProfileCount.Location  = new Point(0, 30);
            lblProfileCount.AutoSize  = true;

            lblProfileCountVal.Text      = "0";
            lblProfileCountVal.Font      = new Font("Segoe UI", 9f, FontStyle.Bold);
            lblProfileCountVal.ForeColor = ThemeManager.Accent;
            lblProfileCountVal.BackColor = Color.Transparent;
            lblProfileCountVal.Location  = new Point(110, 30);
            lblProfileCountVal.AutoSize  = true;

            btnExport.Text      = "Export to file";
            btnExport.Font      = new Font("Segoe UI", 9f);
            btnExport.ForeColor = ThemeManager.Text;
            btnExport.BackColor = ThemeManager.Surface2;
            btnExport.FlatStyle = FlatStyle.Flat;
            btnExport.FlatAppearance.BorderColor = ThemeManager.Border;
            btnExport.Location  = new Point(0, 58);
            btnExport.Size      = new Size(130, 28);
            btnExport.Cursor    = Cursors.Hand;
            btnExport.Click    += btnExport_Click;

            btnImport.Text      = "Import from file";
            btnImport.Font      = new Font("Segoe UI", 9f);
            btnImport.ForeColor = ThemeManager.Text;
            btnImport.BackColor = ThemeManager.Surface2;
            btnImport.FlatStyle = FlatStyle.Flat;
            btnImport.FlatAppearance.BorderColor = ThemeManager.Border;
            btnImport.Location  = new Point(138, 58);
            btnImport.Size      = new Size(130, 28);
            btnImport.Cursor    = Cursors.Hand;
            btnImport.Click    += btnImport_Click;

            btnClearProfiles.Text      = "Clear all";
            btnClearProfiles.Font      = new Font("Segoe UI", 9f);
            btnClearProfiles.ForeColor = ThemeManager.Danger;
            btnClearProfiles.BackColor = ThemeManager.Surface2;
            btnClearProfiles.FlatStyle = FlatStyle.Flat;
            btnClearProfiles.FlatAppearance.BorderColor = ThemeManager.Danger;
            btnClearProfiles.Location  = new Point(276, 58);
            btnClearProfiles.Size      = new Size(100, 28);
            btnClearProfiles.Cursor    = Cursors.Hand;
            btnClearProfiles.Click    += btnClearProfiles_Click;

            lblDataSection.Text      = "DATA & DIAGNOSTICS";
            lblDataSection.Font      = new Font("Segoe UI", 8f, FontStyle.Bold);
            lblDataSection.ForeColor = ThemeManager.Accent;
            lblDataSection.BackColor = Color.Transparent;
            lblDataSection.Location  = new Point(0, 106);
            lblDataSection.AutoSize  = true;

            sepData.Location  = new Point(0, 126);
            sepData.Size      = new Size(428, 1);
            sepData.BackColor = ThemeManager.Border;

            lblDataNote.Text      = "Data stored in: %APPDATA%\\Spectra";
            lblDataNote.Font      = new Font("Segoe UI", 8.5f, FontStyle.Italic);
            lblDataNote.ForeColor = ThemeManager.TextSub;
            lblDataNote.BackColor = Color.Transparent;
            lblDataNote.Location  = new Point(0, 134);
            lblDataNote.AutoSize  = true;

            btnOpenLog.Text      = "Open log folder";
            btnOpenLog.Font      = new Font("Segoe UI", 9f);
            btnOpenLog.ForeColor = ThemeManager.Text;
            btnOpenLog.BackColor = ThemeManager.Surface2;
            btnOpenLog.FlatStyle = FlatStyle.Flat;
            btnOpenLog.FlatAppearance.BorderColor = ThemeManager.Border;
            btnOpenLog.Location  = new Point(0, 158);
            btnOpenLog.Size      = new Size(150, 28);
            btnOpenLog.Cursor    = Cursors.Hand;
            btnOpenLog.Click    += btnOpenLog_Click;

            btnResetAll.Text      = "Reset all settings";
            btnResetAll.Font      = new Font("Segoe UI", 9f);
            btnResetAll.ForeColor = ThemeManager.Danger;
            btnResetAll.BackColor = ThemeManager.Surface2;
            btnResetAll.FlatStyle = FlatStyle.Flat;
            btnResetAll.FlatAppearance.BorderColor = ThemeManager.Danger;
            btnResetAll.Location  = new Point(160, 158);
            btnResetAll.Size      = new Size(150, 28);
            btnResetAll.Cursor    = Cursors.Hand;
            btnResetAll.Click    += btnResetAll_Click;

            tabData.Controls.Add(lblProfileSection);
            tabData.Controls.Add(sepProfile);
            tabData.Controls.Add(lblProfileCount);
            tabData.Controls.Add(lblProfileCountVal);
            tabData.Controls.Add(btnExport);
            tabData.Controls.Add(btnImport);
            tabData.Controls.Add(btnClearProfiles);
            tabData.Controls.Add(lblDataSection);
            tabData.Controls.Add(sepData);
            tabData.Controls.Add(lblDataNote);
            tabData.Controls.Add(btnOpenLog);
            tabData.Controls.Add(btnResetAll);

            // ── ABOUT TAB ─────────────────────────────────────────────────
            panelLogo.Location = new Point(0, 0);
            panelLogo.Size     = new Size(64, 64);
            panelLogo.Paint   += panelLogo_Paint;
            panelLogo.BackColor= Color.Transparent;

            lblAboutName.Text      = "Spectra";
            lblAboutName.Font      = new Font("Segoe UI", 16f, FontStyle.Bold);
            lblAboutName.ForeColor = ThemeManager.Accent;
            lblAboutName.BackColor = Color.Transparent;
            lblAboutName.Location  = new Point(80, 2);
            lblAboutName.AutoSize  = true;

            lblAboutVersion.Text      = "v1.8.0";
            lblAboutVersion.Font      = new Font("Segoe UI", 9f);
            lblAboutVersion.ForeColor = ThemeManager.TextSub;
            lblAboutVersion.BackColor = Color.Transparent;
            lblAboutVersion.Location  = new Point(80, 32);
            lblAboutVersion.AutoSize  = true;

            lblAboutDesc.Text      = "Professional Digital Vibrance Controller";
            lblAboutDesc.Font      = new Font("Segoe UI", 9f, FontStyle.Italic);
            lblAboutDesc.ForeColor = ThemeManager.TextSub;
            lblAboutDesc.BackColor = Color.Transparent;
            lblAboutDesc.Location  = new Point(80, 50);
            lblAboutDesc.AutoSize  = true;

            sepAbout.Location  = new Point(0, 76);
            sepAbout.Size      = new Size(428, 1);
            sepAbout.BackColor = ThemeManager.Border;

            lblSupportTitle.Text      = "GPU SUPPORT";
            lblSupportTitle.Font      = new Font("Segoe UI", 8f, FontStyle.Bold);
            lblSupportTitle.ForeColor = ThemeManager.Accent;
            lblSupportTitle.BackColor = Color.Transparent;
            lblSupportTitle.Location  = new Point(0, 86);
            lblSupportTitle.AutoSize  = true;

            lblGpuLine1.Text      = "✓  NVIDIA — Digital Vibrance Control (NVAPI)";
            lblGpuLine1.Font      = new Font("Segoe UI", 9f);
            lblGpuLine1.ForeColor = ThemeManager.Text;
            lblGpuLine1.BackColor = Color.Transparent;
            lblGpuLine1.Location  = new Point(0, 108);
            lblGpuLine1.AutoSize  = true;

            lblGpuLine2.Text      = "✓  AMD — Saturation Control (ADL 32/64-bit)";
            lblGpuLine2.Font      = new Font("Segoe UI", 9f);
            lblGpuLine2.ForeColor = ThemeManager.Text;
            lblGpuLine2.BackColor = Color.Transparent;
            lblGpuLine2.Location  = new Point(0, 130);
            lblGpuLine2.AutoSize  = true;

            sepAbout2.Location  = new Point(0, 158);
            sepAbout2.Size      = new Size(428, 1);
            sepAbout2.BackColor = ThemeManager.Border;

            lblSysTitle.Text      = "SYSTEM";
            lblSysTitle.Font      = new Font("Segoe UI", 8f, FontStyle.Bold);
            lblSysTitle.ForeColor = ThemeManager.Accent;
            lblSysTitle.BackColor = Color.Transparent;
            lblSysTitle.Location  = new Point(0, 168);
            lblSysTitle.AutoSize  = true;

            lblSysInfo.Text      = "Loading...";
            lblSysInfo.Font      = new Font("Segoe UI", 9f);
            lblSysInfo.ForeColor = ThemeManager.TextSub;
            lblSysInfo.BackColor = Color.Transparent;
            lblSysInfo.Location  = new Point(0, 188);
            lblSysInfo.AutoSize  = true;

            btnGitHub.Text      = "★  GitHub";
            btnGitHub.Font      = new Font("Segoe UI", 9f);
            btnGitHub.ForeColor = Color.White;
            btnGitHub.BackColor = ThemeManager.Accent;
            btnGitHub.FlatStyle = FlatStyle.Flat;
            btnGitHub.FlatAppearance.BorderColor = ThemeManager.Accent;
            btnGitHub.Location  = new Point(0, 230);
            btnGitHub.Size      = new Size(120, 30);
            btnGitHub.Cursor    = Cursors.Hand;
            btnGitHub.Click    += btnGitHub_Click;

            btnOpenLogShort.Text      = "Open logs";
            btnOpenLogShort.Font      = new Font("Segoe UI", 9f);
            btnOpenLogShort.ForeColor = ThemeManager.Text;
            btnOpenLogShort.BackColor = ThemeManager.Surface2;
            btnOpenLogShort.FlatStyle = FlatStyle.Flat;
            btnOpenLogShort.FlatAppearance.BorderColor = ThemeManager.Border;
            btnOpenLogShort.Location  = new Point(130, 230);
            btnOpenLogShort.Size      = new Size(110, 30);
            btnOpenLogShort.Cursor    = Cursors.Hand;
            btnOpenLogShort.Click    += btnOpenLog_Click;

            tabAbout.Controls.Add(panelLogo);
            tabAbout.Controls.Add(lblAboutName);
            tabAbout.Controls.Add(lblAboutVersion);
            tabAbout.Controls.Add(lblAboutDesc);
            tabAbout.Controls.Add(sepAbout);
            tabAbout.Controls.Add(lblSupportTitle);
            tabAbout.Controls.Add(lblGpuLine1);
            tabAbout.Controls.Add(lblGpuLine2);
            tabAbout.Controls.Add(sepAbout2);
            tabAbout.Controls.Add(lblSysTitle);
            tabAbout.Controls.Add(lblSysInfo);
            tabAbout.Controls.Add(btnGitHub);
            tabAbout.Controls.Add(btnOpenLogShort);

            // ── CLOSE BUTTON ──────────────────────────────────────────────
            btnClose.Text      = "Close";
            btnClose.Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            btnClose.ForeColor = Color.White;
            btnClose.BackColor = ThemeManager.Accent;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Location  = new Point(366, 478);
            btnClose.Size      = new Size(104, 34);
            btnClose.Cursor    = Cursors.Hand;
            btnClose.Click    += btnClose_Click;

            Controls.Add(panelHeader);
            Controls.Add(tabControl);
            Controls.Add(btnClose);

            ((System.ComponentModel.ISupportInitialize)numDelay).EndInit();
            panelHeader.ResumeLayout(false);
            tabControl.ResumeLayout(false);
            ResumeLayout(false);
        }

        // ── Fields ────────────────────────────────────────────────────────
        private Panel  panelHeader;
        private Label  lblSettingsTitle;
        private TabControl tabControl;
        private TabPage tabBehavior, tabDisplay, tabData, tabAbout;

        // Behavior
        private Label          lblStartupSection;
        private Panel          sepStartup;
        private CheckBox       chkAutostart, chkMinToTray, chkNotifications;
        private Label          lblDelaySection, lblDelayNote, lblDelayMs;
        private Panel          sepDelay;
        private NumericUpDown  numDelay;

        // Display
        private Label    lblMonitorSection, lblMonitorTarget;
        private Panel    sepMonitor;
        private ComboBox cboMonitorTarget;
        private Label    lblResSection;
        private Panel    sepRes;
        private CheckBox chkNeverResize, chkResetOnExit;

        // Data
        private Label  lblProfileSection, lblProfileCount, lblProfileCountVal;
        private Panel  sepProfile;
        private Button btnExport, btnImport, btnClearProfiles;
        private Label  lblDataSection, lblDataNote;
        private Panel  sepData;
        private Button btnOpenLog, btnResetAll;

        // About
        private Panel  panelLogo;
        private Label  lblAboutName, lblAboutVersion, lblAboutDesc;
        private Panel  sepAbout, sepAbout2;
        private Label  lblSupportTitle, lblGpuLine1, lblGpuLine2;
        private Label  lblSysTitle, lblSysInfo;
        private Button btnGitHub, btnOpenLogShort;

        private Button btnClose;
    }
}
