using System.Drawing;
using System.Windows.Forms;

namespace Spectra.common
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            // Header
            panelHeader     = new Panel();
            labelAppName    = new Label();
            labelVersion    = new Label();
            labelGpuBadge   = new Label();
            btnOpenSettings = new Button();

            // Vibrance section
            panelVibrance        = new Panel();
            labelSectionVibrance = new Label();
            trackBarVibrance     = new TrackBar();
            labelVibranceValue   = new Label();

            // Settings section
            panelSettings        = new Panel();
            labelSectionSettings = new Label();
            chkAutostart         = new CheckBox();
            chkPrimaryMonitor    = new CheckBox();
            chkNeverResize       = new CheckBox();

            // Quick-action row (2 rows inside a panel)
            panelQuickRow    = new Panel();
            panelRow1        = new Panel();
            labelTheme       = new Label();
            btnDark          = new Button();
            btnLight         = new Button();
            panelRowSep1     = new Panel();
            labelLang        = new Label();
            comboLanguage    = new ComboBox();
            panelRow2        = new Panel();
            labelHotkeyTitle = new Label();
            btnHotkey        = new Button();

            // Profiles section
            panelProfiles        = new Panel();
            labelSectionProfiles = new Label();
            listProfiles         = new ListView();
            btnAdd               = new Button();
            btnBrowse            = new Button();
            btnRemove            = new Button();

            // Status bar
            panelStatus  = new Panel();
            labelStatus  = new Label();
            labelGpuInfo = new Label();

            // Tray
            notifyIcon      = new NotifyIcon(components);
            contextMenu     = new ContextMenuStrip(components);
            menuOpenSpectra = new ToolStripMenuItem();
            menuTrayToggle  = new ToolStripMenuItem();
            menuExit        = new ToolStripMenuItem();

            // Workers
            backgroundWorker = new System.ComponentModel.BackgroundWorker();
            settingsWorker   = new System.ComponentModel.BackgroundWorker();

            SuspendLayout();
            panelHeader.SuspendLayout();
            panelVibrance.SuspendLayout();
            panelSettings.SuspendLayout();
            panelQuickRow.SuspendLayout();
            panelRow1.SuspendLayout();
            panelRow2.SuspendLayout();
            panelProfiles.SuspendLayout();
            panelStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarVibrance).BeginInit();

            // ── FORM ─────────────────────────────────────────────────────
            AutoScaleDimensions = new SizeF(6f, 13f);
            AutoScaleMode       = AutoScaleMode.Font;
            ClientSize          = new Size(480, 660);
            FormBorderStyle     = FormBorderStyle.FixedSingle;
            MaximizeBox         = false;
            Text                = "Spectra";
            StartPosition       = FormStartPosition.CenterScreen;
            Name                = "MainForm";
            Load               += MainForm_Load;
            Shown              += MainForm_Shown;
            FormClosing        += MainForm_FormClosing;
            Resize             += MainForm_Resize;

            // ── HEADER (gradient) ─────────────────────────────────────────
            panelHeader.Location = new Point(0, 0);
            panelHeader.Size     = new Size(480, 76);
            panelHeader.Tag      = "header";
            panelHeader.Paint   += panelHeader_Paint;

            labelAppName.Text      = "SPECTRA";
            labelAppName.Font      = new Font("Segoe UI", 22f, FontStyle.Bold);
            labelAppName.ForeColor = Color.White;
            labelAppName.BackColor = Color.Transparent;
            labelAppName.Location  = new Point(16, 10);
            labelAppName.AutoSize  = true;

            labelVersion.Text      = "v1.7.0";
            labelVersion.Font      = new Font("Segoe UI", 7.5f);
            labelVersion.ForeColor = Color.FromArgb(190, 225, 255);
            labelVersion.BackColor = Color.Transparent;
            labelVersion.Location  = new Point(18, 52);
            labelVersion.AutoSize  = true;

            labelGpuBadge.Text      = "";
            labelGpuBadge.Font      = new Font("Segoe UI", 8f);
            labelGpuBadge.ForeColor = Color.FromArgb(210, 240, 255);
            labelGpuBadge.BackColor = Color.Transparent;
            labelGpuBadge.TextAlign = ContentAlignment.MiddleRight;
            labelGpuBadge.Location  = new Point(200, 8);
            labelGpuBadge.Size      = new Size(232, 40);

            btnOpenSettings.Text      = "⚙";
            btnOpenSettings.Font      = new Font("Segoe UI", 12f);
            btnOpenSettings.ForeColor = Color.White;
            btnOpenSettings.BackColor = Color.FromArgb(50, 255, 255, 255);
            btnOpenSettings.FlatStyle = FlatStyle.Flat;
            btnOpenSettings.FlatAppearance.BorderSize           = 0;
            btnOpenSettings.FlatAppearance.MouseOverBackColor   = Color.FromArgb(80, 255, 255, 255);
            btnOpenSettings.FlatAppearance.MouseDownBackColor   = Color.FromArgb(100, 255, 255, 255);
            btnOpenSettings.Location  = new Point(438, 8);
            btnOpenSettings.Size      = new Size(34, 34);
            btnOpenSettings.Cursor    = Cursors.Hand;
            btnOpenSettings.Click    += btnOpenSettings_Click;

            panelHeader.Controls.Add(labelAppName);
            panelHeader.Controls.Add(labelVersion);
            panelHeader.Controls.Add(labelGpuBadge);
            panelHeader.Controls.Add(btnOpenSettings);

            // ── DESKTOP VIBRANCE ─────────────────────────────────────────
            panelVibrance.Location = new Point(12, 88);
            panelVibrance.Size     = new Size(456, 74);
            panelVibrance.Paint   += SectionPanel_Paint;

            labelSectionVibrance.Text     = "DESKTOP VIBRANCE";
            labelSectionVibrance.Font     = new Font("Segoe UI", 7.5f, FontStyle.Bold);
            labelSectionVibrance.Location = new Point(12, 10);
            labelSectionVibrance.AutoSize = true;

            trackBarVibrance.Location  = new Point(10, 32);
            trackBarVibrance.Size      = new Size(384, 30);
            trackBarVibrance.TickStyle = TickStyle.None;
            trackBarVibrance.Scroll   += trackBarVibrance_Scroll;

            labelVibranceValue.Text      = "—";
            labelVibranceValue.Font      = new Font("Segoe UI", 13f, FontStyle.Bold);
            labelVibranceValue.Location  = new Point(400, 33);
            labelVibranceValue.Size      = new Size(50, 26);
            labelVibranceValue.TextAlign = ContentAlignment.MiddleLeft;

            panelVibrance.Controls.Add(labelSectionVibrance);
            panelVibrance.Controls.Add(trackBarVibrance);
            panelVibrance.Controls.Add(labelVibranceValue);

            // ── SETTINGS ─────────────────────────────────────────────────
            panelSettings.Location = new Point(12, 174);
            panelSettings.Size     = new Size(456, 178);
            panelSettings.Paint   += SectionPanel_Paint;

            labelSectionSettings.Text     = "SETTINGS";
            labelSectionSettings.Font     = new Font("Segoe UI", 7.5f, FontStyle.Bold);
            labelSectionSettings.Location = new Point(12, 10);
            labelSectionSettings.AutoSize = true;

            chkAutostart.Text            = "Launch on startup";
            chkAutostart.Location        = new Point(14, 32);
            chkAutostart.Size            = new Size(210, 20);
            chkAutostart.CheckedChanged += chkAutostart_CheckedChanged;

            chkPrimaryMonitor.Text            = "Primary monitor only";
            chkPrimaryMonitor.Location        = new Point(14, 56);
            chkPrimaryMonitor.Size            = new Size(210, 20);
            chkPrimaryMonitor.CheckedChanged += chkPrimaryMonitor_CheckedChanged;

            chkNeverResize.Text              = "Never change resolution";
            chkNeverResize.Location          = new Point(14, 80);
            chkNeverResize.Size              = new Size(210, 20);
            chkNeverResize.CheckedChanged   += chkNeverResize_CheckedChanged;

            // Quick-action outer panel
            panelQuickRow.Location  = new Point(0, 106);
            panelQuickRow.Size      = new Size(456, 64);
            panelQuickRow.BackColor = Color.Transparent;

            // Row 1: THEME buttons + LANGUAGE dropdown
            panelRow1.Location  = new Point(0, 0);
            panelRow1.Size      = new Size(456, 30);
            panelRow1.BackColor = Color.Transparent;

            labelTheme.Text      = "THEME";
            labelTheme.Font      = new Font("Segoe UI", 7f, FontStyle.Bold);
            labelTheme.Location  = new Point(14, 8);
            labelTheme.AutoSize  = true;

            btnDark.Text       = "Dark";
            btnDark.Location   = new Point(62, 3);
            btnDark.Size       = new Size(54, 22);
            btnDark.FlatStyle  = FlatStyle.Flat;
            btnDark.FlatAppearance.BorderSize = 1;
            btnDark.Cursor     = Cursors.Hand;
            btnDark.Click     += btnDark_Click;

            btnLight.Text      = "Light";
            btnLight.Location  = new Point(120, 3);
            btnLight.Size      = new Size(54, 22);
            btnLight.FlatStyle = FlatStyle.Flat;
            btnLight.FlatAppearance.BorderSize = 1;
            btnLight.Cursor    = Cursors.Hand;
            btnLight.Click    += btnLight_Click;

            // Vertical separator between THEME and LANG
            panelRowSep1.Location  = new Point(183, 5);
            panelRowSep1.Size      = new Size(1, 16);
            panelRowSep1.Tag       = "separator";

            labelLang.Text      = "LANG";
            labelLang.Font      = new Font("Segoe UI", 7f, FontStyle.Bold);
            labelLang.Location  = new Point(192, 8);
            labelLang.AutoSize  = true;

            comboLanguage.Location      = new Point(228, 3);
            comboLanguage.Size          = new Size(136, 22);
            comboLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
            comboLanguage.FlatStyle     = FlatStyle.Flat;

            panelRow1.Controls.Add(labelTheme);
            panelRow1.Controls.Add(btnDark);
            panelRow1.Controls.Add(btnLight);
            panelRow1.Controls.Add(panelRowSep1);
            panelRow1.Controls.Add(labelLang);
            panelRow1.Controls.Add(comboLanguage);

            // Row 2: HOTKEY button
            panelRow2.Location  = new Point(0, 34);
            panelRow2.Size      = new Size(456, 30);
            panelRow2.BackColor = Color.Transparent;

            labelHotkeyTitle.Text      = "HOTKEY";
            labelHotkeyTitle.Font      = new Font("Segoe UI", 7f, FontStyle.Bold);
            labelHotkeyTitle.Location  = new Point(14, 8);
            labelHotkeyTitle.AutoSize  = true;

            btnHotkey.Text      = "F9";
            btnHotkey.Font      = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            btnHotkey.Location  = new Point(62, 3);
            btnHotkey.Size      = new Size(90, 22);
            btnHotkey.FlatStyle = FlatStyle.Flat;
            btnHotkey.FlatAppearance.BorderSize = 1;
            btnHotkey.Cursor    = Cursors.Hand;
            btnHotkey.Click    += btnHotkey_Click;

            panelRow2.Controls.Add(labelHotkeyTitle);
            panelRow2.Controls.Add(btnHotkey);

            panelQuickRow.Controls.Add(panelRow1);
            panelQuickRow.Controls.Add(panelRow2);

            panelSettings.Controls.Add(labelSectionSettings);
            panelSettings.Controls.Add(chkAutostart);
            panelSettings.Controls.Add(chkPrimaryMonitor);
            panelSettings.Controls.Add(chkNeverResize);
            panelSettings.Controls.Add(panelQuickRow);

            // ── GAME PROFILES ─────────────────────────────────────────────
            panelProfiles.Location = new Point(12, 364);
            panelProfiles.Size     = new Size(456, 236);
            panelProfiles.Paint   += SectionPanel_Paint;

            labelSectionProfiles.Text     = "GAME PROFILES";
            labelSectionProfiles.Font     = new Font("Segoe UI", 7.5f, FontStyle.Bold);
            labelSectionProfiles.Location = new Point(12, 10);
            labelSectionProfiles.AutoSize = true;

            listProfiles.Location = new Point(8, 34);
            listProfiles.Size     = new Size(440, 158);
            listProfiles.View     = View.LargeIcon;
            listProfiles.UseCompatibleStateImageBehavior = false;
            listProfiles.BorderStyle   = BorderStyle.None;
            listProfiles.DoubleClick  += listProfiles_DoubleClick;

            btnAdd.Text      = "Add File";
            btnAdd.Location  = new Point(8, 200);
            btnAdd.Size      = new Size(110, 28);
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.FlatAppearance.BorderSize = 1;
            btnAdd.Cursor    = Cursors.Hand;
            btnAdd.Click    += btnAdd_Click;

            btnBrowse.Text      = "Browse Running";
            btnBrowse.Location  = new Point(126, 200);
            btnBrowse.Size      = new Size(126, 28);
            btnBrowse.FlatStyle = FlatStyle.Flat;
            btnBrowse.FlatAppearance.BorderSize = 1;
            btnBrowse.Cursor    = Cursors.Hand;
            btnBrowse.Click    += btnBrowse_Click;

            btnRemove.Text      = "Remove";
            btnRemove.Location  = new Point(260, 200);
            btnRemove.Size      = new Size(86, 28);
            btnRemove.FlatStyle = FlatStyle.Flat;
            btnRemove.FlatAppearance.BorderSize = 1;
            btnRemove.Cursor    = Cursors.Hand;
            btnRemove.Click    += btnRemove_Click;

            panelProfiles.Controls.Add(labelSectionProfiles);
            panelProfiles.Controls.Add(listProfiles);
            panelProfiles.Controls.Add(btnAdd);
            panelProfiles.Controls.Add(btnBrowse);
            panelProfiles.Controls.Add(btnRemove);

            // ── STATUS BAR ────────────────────────────────────────────────
            panelStatus.Location = new Point(0, 612);
            panelStatus.Size     = new Size(480, 48);

            labelStatus.Text     = "Initializing...";
            labelStatus.Font     = new Font("Segoe UI", 9f);
            labelStatus.Location = new Point(14, 14);
            labelStatus.AutoSize = true;

            labelGpuInfo.Text      = "";
            labelGpuInfo.Font      = new Font("Segoe UI", 8f);
            labelGpuInfo.Location  = new Point(180, 16);
            labelGpuInfo.Size      = new Size(290, 18);
            labelGpuInfo.TextAlign = ContentAlignment.MiddleRight;

            panelStatus.Controls.Add(labelStatus);
            panelStatus.Controls.Add(labelGpuInfo);

            // ── TRAY ──────────────────────────────────────────────────────
            menuOpenSpectra.Text  = "Open Spectra";
            menuOpenSpectra.Font  = new Font("Segoe UI", 9f, FontStyle.Bold);
            menuOpenSpectra.Click += menuOpenSpectra_Click;

            menuTrayToggle.Text  = "Toggle Vibrance";
            menuTrayToggle.Click += menuTrayToggle_Click;

            menuExit.Text  = "Exit";
            menuExit.Click += exitMenuItem_Click;

            contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                menuOpenSpectra,
                new ToolStripSeparator(),
                menuTrayToggle,
                new ToolStripSeparator(),
                menuExit
            });

            notifyIcon.ContextMenuStrip = contextMenu;
            notifyIcon.Text             = "Spectra";
            notifyIcon.Visible          = true;
            notifyIcon.MouseClick      += notifyIcon_MouseClick;

            // ── WORKERS ───────────────────────────────────────────────────
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork           += backgroundWorker_DoWork;
            backgroundWorker.ProgressChanged  += backgroundWorker_ProgressChanged;
            settingsWorker.DoWork             += settingsWorker_DoWork;
            settingsWorker.RunWorkerCompleted += settingsWorker_RunWorkerCompleted;

            Controls.Add(panelHeader);
            Controls.Add(panelVibrance);
            Controls.Add(panelSettings);
            Controls.Add(panelProfiles);
            Controls.Add(panelStatus);

            ((System.ComponentModel.ISupportInitialize)trackBarVibrance).EndInit();
            panelHeader.ResumeLayout(false);
            panelVibrance.ResumeLayout(false);
            panelSettings.ResumeLayout(false);
            panelRow1.ResumeLayout(false);
            panelRow2.ResumeLayout(false);
            panelQuickRow.ResumeLayout(false);
            panelProfiles.ResumeLayout(false);
            panelStatus.ResumeLayout(false);
            ResumeLayout(false);
        }

        // ── FIELD DECLARATIONS ────────────────────────────────────────────
        private Panel          panelHeader;
        private Label          labelAppName;
        private Label          labelVersion;
        private Label          labelGpuBadge;
        private Button         btnOpenSettings;

        private Panel          panelVibrance;
        private Label          labelSectionVibrance;
        private TrackBar       trackBarVibrance;
        private Label          labelVibranceValue;

        private Panel          panelSettings;
        private Label          labelSectionSettings;
        private CheckBox       chkAutostart;
        private CheckBox       chkPrimaryMonitor;
        private CheckBox       chkNeverResize;
        private Panel          panelQuickRow;
        private Panel          panelRow1;
        private Label          labelTheme;
        private Button         btnDark;
        private Button         btnLight;
        private Panel          panelRowSep1;
        private Label          labelLang;
        private ComboBox       comboLanguage;
        private Panel          panelRow2;
        private Label          labelHotkeyTitle;
        private Button         btnHotkey;

        private Panel          panelProfiles;
        private Label          labelSectionProfiles;
        private ListView       listProfiles;
        private Button         btnAdd;
        private Button         btnBrowse;
        private Button         btnRemove;

        private Panel          panelStatus;
        private Label          labelStatus;
        private Label          labelGpuInfo;

        private NotifyIcon        notifyIcon;
        private ContextMenuStrip  contextMenu;
        private ToolStripMenuItem menuOpenSpectra;
        private ToolStripMenuItem menuTrayToggle;
        private ToolStripMenuItem menuExit;

        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.ComponentModel.BackgroundWorker settingsWorker;
    }
}
