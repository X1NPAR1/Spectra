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

            panelHeader       = new Panel();
            labelAppName      = new Label();
            labelVersion      = new Label();
            labelGpuBadge     = new Label();
            btnOpenSettings   = new Button();

            panelVibrance     = new Panel();
            labelSectionVibrance = new Label();
            trackBarVibrance  = new TrackBar();
            labelVibranceValue= new Label();

            panelSettings     = new Panel();
            labelSectionSettings = new Label();
            chkAutostart      = new CheckBox();
            chkPrimaryMonitor = new CheckBox();
            chkNeverResize    = new CheckBox();
            panelQuickRow     = new Panel();
            labelTheme        = new Label();
            btnDark           = new Button();
            btnLight          = new Button();
            labelLang         = new Label();
            comboLanguage     = new ComboBox();
            labelHotkeyTitle  = new Label();
            btnHotkey         = new Button();

            panelProfiles     = new Panel();
            labelSectionProfiles = new Label();
            listProfiles      = new ListView();
            btnAdd            = new Button();
            btnBrowse         = new Button();
            btnRemove         = new Button();

            panelStatus       = new Panel();
            labelStatus       = new Label();
            labelGpuInfo      = new Label();

            notifyIcon        = new NotifyIcon(components);
            contextMenu       = new ContextMenuStrip(components);
            menuOpenSpectra   = new ToolStripMenuItem();
            menuExit          = new ToolStripMenuItem();

            backgroundWorker  = new System.ComponentModel.BackgroundWorker();
            settingsWorker    = new System.ComponentModel.BackgroundWorker();

            SuspendLayout();
            panelHeader.SuspendLayout();
            panelVibrance.SuspendLayout();
            panelSettings.SuspendLayout();
            panelQuickRow.SuspendLayout();
            panelProfiles.SuspendLayout();
            panelStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarVibrance).BeginInit();

            // ── Form ──────────────────────────────────────────────────────
            AutoScaleDimensions = new SizeF(6f, 13f);
            AutoScaleMode       = AutoScaleMode.Font;
            ClientSize          = new Size(480, 640);
            FormBorderStyle     = FormBorderStyle.FixedSingle;
            MaximizeBox         = false;
            Text                = "Spectra";
            StartPosition       = FormStartPosition.CenterScreen;
            Name                = "MainForm";
            Load               += MainForm_Load;
            Shown              += MainForm_Shown;
            FormClosing        += MainForm_FormClosing;
            Resize             += MainForm_Resize;

            // ── Header panel (gradient, 80px tall) ───────────────────────
            panelHeader.Location = new Point(0, 0);
            panelHeader.Size     = new Size(480, 80);
            panelHeader.Tag      = "header";
            panelHeader.Paint   += panelHeader_Paint;

            // "SPECTRA" — big title
            labelAppName.Text      = "SPECTRA";
            labelAppName.Font      = new Font("Segoe UI", 22f, FontStyle.Bold);
            labelAppName.ForeColor = Color.White;
            labelAppName.BackColor = Color.Transparent;
            labelAppName.Location  = new Point(16, 12);
            labelAppName.AutoSize  = true;

            // "v1.7.0" — below title
            labelVersion.Text      = "v1.7.0";
            labelVersion.Font      = new Font("Segoe UI", 7.5f, FontStyle.Regular);
            labelVersion.ForeColor = Color.FromArgb(180, 220, 255);
            labelVersion.BackColor = Color.Transparent;
            labelVersion.Location  = new Point(18, 54);
            labelVersion.AutoSize  = true;

            // GPU badge — right-aligned, two lines
            labelGpuBadge.Text      = "";
            labelGpuBadge.Font      = new Font("Segoe UI", 8f, FontStyle.Regular);
            labelGpuBadge.ForeColor = Color.FromArgb(210, 240, 255);
            labelGpuBadge.BackColor = Color.Transparent;
            labelGpuBadge.TextAlign = ContentAlignment.MiddleRight;
            labelGpuBadge.Location  = new Point(240, 8);
            labelGpuBadge.Size      = new Size(170, 40);

            // ⚙ Settings button — top-right of header
            btnOpenSettings.Text      = "⚙";
            btnOpenSettings.Font      = new Font("Segoe UI", 11f, FontStyle.Regular);
            btnOpenSettings.ForeColor = Color.White;
            btnOpenSettings.BackColor = Color.FromArgb(60, 255, 255, 255);
            btnOpenSettings.FlatStyle = FlatStyle.Flat;
            btnOpenSettings.FlatAppearance.BorderSize  = 0;
            btnOpenSettings.FlatAppearance.MouseOverBackColor = Color.FromArgb(80, 255, 255, 255);
            btnOpenSettings.Location  = new Point(440, 8);
            btnOpenSettings.Size      = new Size(32, 32);
            btnOpenSettings.Cursor    = Cursors.Hand;
            btnOpenSettings.Click    += btnOpenSettings_Click;

            panelHeader.Controls.Add(labelAppName);
            panelHeader.Controls.Add(labelVersion);
            panelHeader.Controls.Add(labelGpuBadge);
            panelHeader.Controls.Add(btnOpenSettings);

            // ── Vibrance panel (y=92) ─────────────────────────────────────
            panelVibrance.Location = new Point(12, 92);
            panelVibrance.Size     = new Size(456, 76);
            panelVibrance.Paint   += SectionPanel_Paint;

            labelSectionVibrance.Text      = "DESKTOP VIBRANCE";
            labelSectionVibrance.Font      = new Font("Segoe UI", 7.5f, FontStyle.Bold);
            labelSectionVibrance.Location  = new Point(12, 10);
            labelSectionVibrance.AutoSize  = true;

            trackBarVibrance.Location  = new Point(10, 32);
            trackBarVibrance.Size      = new Size(382, 32);
            trackBarVibrance.TickStyle = TickStyle.None;
            trackBarVibrance.Scroll   += trackBarVibrance_Scroll;

            labelVibranceValue.Text      = "—";
            labelVibranceValue.Font      = new Font("Segoe UI", 12f, FontStyle.Bold);
            labelVibranceValue.Location  = new Point(398, 33);
            labelVibranceValue.Size      = new Size(52, 26);
            labelVibranceValue.TextAlign = ContentAlignment.MiddleLeft;
            labelVibranceValue.AutoSize  = false;

            panelVibrance.Controls.Add(labelSectionVibrance);
            panelVibrance.Controls.Add(trackBarVibrance);
            panelVibrance.Controls.Add(labelVibranceValue);

            // ── Settings panel (y=180) ───────────────────────────────────
            panelSettings.Location = new Point(12, 180);
            panelSettings.Size     = new Size(456, 158);
            panelSettings.Paint   += SectionPanel_Paint;

            labelSectionSettings.Text     = "SETTINGS";
            labelSectionSettings.Font     = new Font("Segoe UI", 7.5f, FontStyle.Bold);
            labelSectionSettings.Location = new Point(12, 10);
            labelSectionSettings.AutoSize = true;

            chkAutostart.Text             = "Launch on startup";
            chkAutostart.Location         = new Point(14, 32);
            chkAutostart.Size             = new Size(200, 20);
            chkAutostart.CheckedChanged  += chkAutostart_CheckedChanged;

            chkPrimaryMonitor.Text            = "Primary monitor only";
            chkPrimaryMonitor.Location        = new Point(14, 56);
            chkPrimaryMonitor.Size            = new Size(200, 20);
            chkPrimaryMonitor.CheckedChanged += chkPrimaryMonitor_CheckedChanged;

            chkNeverResize.Text              = "Never change resolution";
            chkNeverResize.Location          = new Point(14, 80);
            chkNeverResize.Size              = new Size(210, 20);
            chkNeverResize.CheckedChanged   += chkNeverResize_CheckedChanged;

            // Quick-action row: Theme | Language | Hotkey
            panelQuickRow.Location  = new Point(0, 108);
            panelQuickRow.Size      = new Size(456, 42);
            panelQuickRow.BackColor = Color.Transparent;

            labelTheme.Text      = "THEME";
            labelTheme.Font      = new Font("Segoe UI", 7f, FontStyle.Bold);
            labelTheme.Location  = new Point(14, 14);
            labelTheme.AutoSize  = true;

            btnDark.Text       = "Dark";
            btnDark.Location   = new Point(60, 9);
            btnDark.Size       = new Size(52, 24);
            btnDark.FlatStyle  = FlatStyle.Flat;
            btnDark.FlatAppearance.BorderSize = 1;
            btnDark.Cursor     = Cursors.Hand;
            btnDark.Click     += btnDark_Click;

            btnLight.Text      = "Light";
            btnLight.Location  = new Point(116, 9);
            btnLight.Size      = new Size(52, 24);
            btnLight.FlatStyle = FlatStyle.Flat;
            btnLight.FlatAppearance.BorderSize = 1;
            btnLight.Cursor    = Cursors.Hand;
            btnLight.Click    += btnLight_Click;

            labelLang.Text      = "LANG";
            labelLang.Font      = new Font("Segoe UI", 7f, FontStyle.Bold);
            labelLang.Location  = new Point(182, 14);
            labelLang.AutoSize  = true;

            comboLanguage.Location      = new Point(214, 10);
            comboLanguage.Size          = new Size(104, 22);
            comboLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
            comboLanguage.FlatStyle     = FlatStyle.Flat;

            labelHotkeyTitle.Text      = "HOTKEY";
            labelHotkeyTitle.Font      = new Font("Segoe UI", 7f, FontStyle.Bold);
            labelHotkeyTitle.Location  = new Point(330, 14);
            labelHotkeyTitle.AutoSize  = true;

            btnHotkey.Text      = "F9";
            btnHotkey.Location  = new Point(378, 9);
            btnHotkey.Size      = new Size(66, 24);
            btnHotkey.FlatStyle = FlatStyle.Flat;
            btnHotkey.FlatAppearance.BorderSize = 1;
            btnHotkey.Cursor    = Cursors.Hand;
            btnHotkey.Click    += btnHotkey_Click;

            panelQuickRow.Controls.Add(labelTheme);
            panelQuickRow.Controls.Add(btnDark);
            panelQuickRow.Controls.Add(btnLight);
            panelQuickRow.Controls.Add(labelLang);
            panelQuickRow.Controls.Add(comboLanguage);
            panelQuickRow.Controls.Add(labelHotkeyTitle);
            panelQuickRow.Controls.Add(btnHotkey);

            panelSettings.Controls.Add(labelSectionSettings);
            panelSettings.Controls.Add(chkAutostart);
            panelSettings.Controls.Add(chkPrimaryMonitor);
            panelSettings.Controls.Add(chkNeverResize);
            panelSettings.Controls.Add(panelQuickRow);

            // ── Profiles panel (y=350) ───────────────────────────────────
            panelProfiles.Location = new Point(12, 350);
            panelProfiles.Size     = new Size(456, 230);
            panelProfiles.Paint   += SectionPanel_Paint;

            labelSectionProfiles.Text     = "GAME PROFILES";
            labelSectionProfiles.Font     = new Font("Segoe UI", 7.5f, FontStyle.Bold);
            labelSectionProfiles.Location = new Point(12, 10);
            labelSectionProfiles.AutoSize = true;

            listProfiles.Location = new Point(8, 34);
            listProfiles.Size     = new Size(440, 152);
            listProfiles.View     = View.LargeIcon;
            listProfiles.UseCompatibleStateImageBehavior = false;
            listProfiles.BorderStyle    = BorderStyle.None;
            listProfiles.DoubleClick   += listProfiles_DoubleClick;

            btnAdd.Text      = "Add File";
            btnAdd.Location  = new Point(8, 194);
            btnAdd.Size      = new Size(100, 28);
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.FlatAppearance.BorderSize = 1;
            btnAdd.Cursor    = Cursors.Hand;
            btnAdd.Click    += btnAdd_Click;

            btnBrowse.Text      = "Browse Running";
            btnBrowse.Location  = new Point(116, 194);
            btnBrowse.Size      = new Size(120, 28);
            btnBrowse.FlatStyle = FlatStyle.Flat;
            btnBrowse.FlatAppearance.BorderSize = 1;
            btnBrowse.Cursor    = Cursors.Hand;
            btnBrowse.Click    += btnBrowse_Click;

            btnRemove.Text      = "Remove";
            btnRemove.Location  = new Point(244, 194);
            btnRemove.Size      = new Size(80, 28);
            btnRemove.FlatStyle = FlatStyle.Flat;
            btnRemove.FlatAppearance.BorderSize = 1;
            btnRemove.Cursor    = Cursors.Hand;
            btnRemove.Click    += btnRemove_Click;

            panelProfiles.Controls.Add(labelSectionProfiles);
            panelProfiles.Controls.Add(listProfiles);
            panelProfiles.Controls.Add(btnAdd);
            panelProfiles.Controls.Add(btnBrowse);
            panelProfiles.Controls.Add(btnRemove);

            // ── Status bar (y=592) ───────────────────────────────────────
            panelStatus.Location = new Point(0, 594);
            panelStatus.Size     = new Size(480, 46);

            labelStatus.Text      = "Initializing...";
            labelStatus.Font      = new Font("Segoe UI", 9f, FontStyle.Regular);
            labelStatus.Location  = new Point(14, 13);
            labelStatus.AutoSize  = true;

            labelGpuInfo.Text      = "";
            labelGpuInfo.Font      = new Font("Segoe UI", 8f, FontStyle.Regular);
            labelGpuInfo.Location  = new Point(200, 13);
            labelGpuInfo.Size      = new Size(268, 20);
            labelGpuInfo.TextAlign = ContentAlignment.MiddleRight;
            labelGpuInfo.AutoSize  = false;

            panelStatus.Controls.Add(labelStatus);
            panelStatus.Controls.Add(labelGpuInfo);

            // ── Tray / Context menu ───────────────────────────────────────
            menuOpenSpectra.Text  = "Open Spectra";
            menuOpenSpectra.Font  = new Font("Segoe UI", 9f, FontStyle.Bold);
            menuOpenSpectra.Click += menuOpenSpectra_Click;

            menuExit.Text  = "Exit";
            menuExit.Click += exitMenuItem_Click;

            contextMenu.Items.AddRange(new ToolStripItem[] { menuOpenSpectra, new ToolStripSeparator(), menuExit });

            notifyIcon.ContextMenuStrip = contextMenu;
            notifyIcon.Text             = "Spectra";
            notifyIcon.Visible          = true;
            notifyIcon.MouseDoubleClick += notifyIcon_DoubleClick;

            // ── Workers ───────────────────────────────────────────────────
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork           += backgroundWorker_DoWork;
            backgroundWorker.ProgressChanged  += backgroundWorker_ProgressChanged;

            settingsWorker.DoWork             += settingsWorker_DoWork;
            settingsWorker.RunWorkerCompleted += settingsWorker_RunWorkerCompleted;

            // ── Add to form ───────────────────────────────────────────────
            Controls.Add(panelHeader);
            Controls.Add(panelVibrance);
            Controls.Add(panelSettings);
            Controls.Add(panelProfiles);
            Controls.Add(panelStatus);

            ((System.ComponentModel.ISupportInitialize)trackBarVibrance).EndInit();
            panelHeader.ResumeLayout(false);
            panelVibrance.ResumeLayout(false);
            panelSettings.ResumeLayout(false);
            panelQuickRow.ResumeLayout(false);
            panelProfiles.ResumeLayout(false);
            panelStatus.ResumeLayout(false);
            ResumeLayout(false);
        }

        // ── Fields ────────────────────────────────────────────────────────
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
        private Label          labelTheme;
        private Button         btnDark;
        private Button         btnLight;
        private Label          labelLang;
        private ComboBox       comboLanguage;
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
        private ToolStripMenuItem menuExit;

        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.ComponentModel.BackgroundWorker settingsWorker;
    }
}
