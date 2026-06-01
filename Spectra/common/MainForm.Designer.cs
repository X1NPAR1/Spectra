using System.Drawing;
using System.Windows.Forms;
using Spectra.UI;

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

            panelHeader      = new Panel();
            labelAppName     = new Label();
            labelVersion     = new Label();
            labelGpuBadge    = new Label();
            btnOpenSettings  = new Button();

            panelVibrance        = new Panel();
            labelSectionVibrance = new Label();
            trackBarVibrance     = new TrackBar();
            labelVibranceValue   = new Label();

            panelSettings        = new Panel();
            labelSectionSettings = new Label();
            chkAutostart         = new CheckBox();
            chkPrimaryMonitor    = new CheckBox();
            chkNeverResize       = new CheckBox();
            panelQuickRow        = new Panel();
            labelLang            = new Label();
            comboLanguage        = new ComboBox();
            labelHotkeyTitle     = new Label();
            btnHotkey            = new Button();

            panelProfiles        = new Panel();
            labelSectionProfiles = new Label();
            listProfiles         = new ListView();
            btnAdd               = new Button();
            btnBrowse            = new Button();
            btnRemove            = new Button();

            panelStatus  = new Panel();
            labelStatus  = new Label();
            labelGpuInfo = new Label();

            notifyIcon      = new NotifyIcon(components);
            contextMenu     = new ContextMenuStrip(components);
            menuOpenSpectra = new ToolStripMenuItem();
            menuTrayToggle  = new ToolStripMenuItem();
            menuExit        = new ToolStripMenuItem();

            backgroundWorker = new System.ComponentModel.BackgroundWorker();
            settingsWorker   = new System.ComponentModel.BackgroundWorker();

            SuspendLayout();
            panelHeader.SuspendLayout();
            panelVibrance.SuspendLayout();
            panelSettings.SuspendLayout();
            panelQuickRow.SuspendLayout();
            panelProfiles.SuspendLayout();
            panelStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarVibrance).BeginInit();

            // ── FORM ─────────────────────────────────────────────────────
            AutoScaleDimensions = new SizeF(6f, 13f);
            AutoScaleMode       = AutoScaleMode.Font;
            ClientSize          = new Size(480, 628);
            FormBorderStyle     = FormBorderStyle.FixedSingle;
            MaximizeBox         = false;
            Text                = "Spectra";
            StartPosition       = FormStartPosition.CenterScreen;
            BackColor           = ThemeManager.Bg;
            Name                = "MainForm";
            Load               += MainForm_Load;
            Shown              += MainForm_Shown;
            FormClosing        += MainForm_FormClosing;
            Resize             += MainForm_Resize;

            // ── HEADER (gradient, painted) ────────────────────────────────
            panelHeader.Location = new Point(0, 0);
            panelHeader.Size     = new Size(480, 80);
            panelHeader.BackColor= Color.Transparent;
            panelHeader.Paint   += panelHeader_Paint;

            // Small "S" icon drawn in header paint — labels go next to it
            labelAppName.Text      = "SPECTRA";
            labelAppName.Font      = new Font("Segoe UI", 21f, FontStyle.Bold);
            labelAppName.ForeColor = Color.White;
            labelAppName.BackColor = Color.Transparent;
            labelAppName.Location  = new Point(72, 12);
            labelAppName.AutoSize  = true;

            labelVersion.Text      = "v1.8.0";
            labelVersion.Font      = new Font("Segoe UI", 7.5f);
            labelVersion.ForeColor = Color.FromArgb(200, 230, 255);
            labelVersion.BackColor = Color.Transparent;
            labelVersion.Location  = new Point(74, 52);
            labelVersion.AutoSize  = true;

            labelGpuBadge.Text      = "";
            labelGpuBadge.Font      = new Font("Segoe UI", 8f);
            labelGpuBadge.ForeColor = Color.FromArgb(220, 240, 255);
            labelGpuBadge.BackColor = Color.Transparent;
            labelGpuBadge.TextAlign = ContentAlignment.MiddleRight;
            labelGpuBadge.Location  = new Point(200, 10);
            labelGpuBadge.Size      = new Size(230, 36);

            btnOpenSettings.Text      = "⚙";
            btnOpenSettings.Font      = new Font("Segoe UI", 12f);
            btnOpenSettings.ForeColor = Color.White;
            btnOpenSettings.BackColor = Color.FromArgb(50, 255, 255, 255);
            btnOpenSettings.FlatStyle = FlatStyle.Flat;
            btnOpenSettings.FlatAppearance.BorderSize         = 0;
            btnOpenSettings.FlatAppearance.MouseOverBackColor = Color.FromArgb(80, 255, 255, 255);
            btnOpenSettings.Location  = new Point(436, 8);
            btnOpenSettings.Size      = new Size(36, 36);
            btnOpenSettings.Cursor    = Cursors.Hand;
            btnOpenSettings.Click    += btnOpenSettings_Click;

            panelHeader.Controls.Add(labelAppName);
            panelHeader.Controls.Add(labelVersion);
            panelHeader.Controls.Add(labelGpuBadge);
            panelHeader.Controls.Add(btnOpenSettings);

            // ── DESKTOP VIBRANCE (card panel) ─────────────────────────────
            panelVibrance.Location  = new Point(14, 92);
            panelVibrance.Size      = new Size(452, 76);
            panelVibrance.BackColor = ThemeManager.Surface;
            panelVibrance.Paint    += CardPanel_Paint;

            labelSectionVibrance.Text      = "DESKTOP VIBRANCE";
            labelSectionVibrance.Font      = new Font("Segoe UI", 7.5f, FontStyle.Bold);
            labelSectionVibrance.ForeColor = ThemeManager.Accent;
            labelSectionVibrance.BackColor = Color.Transparent;
            labelSectionVibrance.Location  = new Point(16, 10);
            labelSectionVibrance.AutoSize  = true;

            trackBarVibrance.Location  = new Point(14, 33);
            trackBarVibrance.Size      = new Size(374, 30);
            trackBarVibrance.TickStyle = TickStyle.None;
            trackBarVibrance.BackColor = ThemeManager.Surface;
            trackBarVibrance.Scroll   += trackBarVibrance_Scroll;

            labelVibranceValue.Text      = "—";
            labelVibranceValue.Font      = new Font("Segoe UI", 14f, FontStyle.Bold);
            labelVibranceValue.ForeColor = ThemeManager.Accent;
            labelVibranceValue.BackColor = Color.Transparent;
            labelVibranceValue.Location  = new Point(392, 34);
            labelVibranceValue.Size      = new Size(54, 28);
            labelVibranceValue.TextAlign = ContentAlignment.MiddleLeft;

            panelVibrance.Controls.Add(labelSectionVibrance);
            panelVibrance.Controls.Add(trackBarVibrance);
            panelVibrance.Controls.Add(labelVibranceValue);

            // ── SETTINGS (card panel) ─────────────────────────────────────
            panelSettings.Location  = new Point(14, 180);
            panelSettings.Size      = new Size(452, 156);
            panelSettings.BackColor = ThemeManager.Surface;
            panelSettings.Paint    += CardPanel_Paint;

            labelSectionSettings.Text      = "SETTINGS";
            labelSectionSettings.Font      = new Font("Segoe UI", 7.5f, FontStyle.Bold);
            labelSectionSettings.ForeColor = ThemeManager.Accent;
            labelSectionSettings.BackColor = Color.Transparent;
            labelSectionSettings.Location  = new Point(16, 10);
            labelSectionSettings.AutoSize  = true;

            chkAutostart.Text            = "Launch on startup";
            chkAutostart.Font            = new Font("Segoe UI", 9f);
            chkAutostart.ForeColor       = ThemeManager.Text;
            chkAutostart.BackColor       = Color.Transparent;
            chkAutostart.Location        = new Point(16, 34);
            chkAutostart.Size            = new Size(220, 22);
            chkAutostart.CheckedChanged += chkAutostart_CheckedChanged;

            chkPrimaryMonitor.Text            = "Primary monitor only";
            chkPrimaryMonitor.Font            = new Font("Segoe UI", 9f);
            chkPrimaryMonitor.ForeColor       = ThemeManager.Text;
            chkPrimaryMonitor.BackColor       = Color.Transparent;
            chkPrimaryMonitor.Location        = new Point(16, 60);
            chkPrimaryMonitor.Size            = new Size(220, 22);
            chkPrimaryMonitor.CheckedChanged += chkPrimaryMonitor_CheckedChanged;

            chkNeverResize.Text              = "Never change resolution";
            chkNeverResize.Font              = new Font("Segoe UI", 9f);
            chkNeverResize.ForeColor         = ThemeManager.Text;
            chkNeverResize.BackColor         = Color.Transparent;
            chkNeverResize.Location          = new Point(16, 86);
            chkNeverResize.Size              = new Size(230, 22);
            chkNeverResize.CheckedChanged   += chkNeverResize_CheckedChanged;

            // Quick row: LANGUAGE + HOTKEY side by side (plenty of room)
            panelQuickRow.Location  = new Point(0, 116);
            panelQuickRow.Size      = new Size(452, 34);
            panelQuickRow.BackColor = Color.Transparent;

            labelLang.Text      = "LANGUAGE";
            labelLang.Font      = new Font("Segoe UI", 7f, FontStyle.Bold);
            labelLang.ForeColor = ThemeManager.TextSub;
            labelLang.BackColor = Color.Transparent;
            labelLang.Location  = new Point(16, 10);
            labelLang.AutoSize  = true;

            comboLanguage.Location      = new Point(76, 6);
            comboLanguage.Size          = new Size(148, 22);
            comboLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
            comboLanguage.FlatStyle     = FlatStyle.Flat;
            comboLanguage.Font          = new Font("Segoe UI", 9f);
            comboLanguage.BackColor     = ThemeManager.Surface2;
            comboLanguage.ForeColor     = ThemeManager.Text;

            labelHotkeyTitle.Text      = "HOTKEY";
            labelHotkeyTitle.Font      = new Font("Segoe UI", 7f, FontStyle.Bold);
            labelHotkeyTitle.ForeColor = ThemeManager.TextSub;
            labelHotkeyTitle.BackColor = Color.Transparent;
            labelHotkeyTitle.Location  = new Point(240, 10);
            labelHotkeyTitle.AutoSize  = true;

            btnHotkey.Text      = "F9";
            btnHotkey.Font      = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            btnHotkey.ForeColor = ThemeManager.Accent;
            btnHotkey.BackColor = ThemeManager.Surface2;
            btnHotkey.FlatStyle = FlatStyle.Flat;
            btnHotkey.FlatAppearance.BorderColor = ThemeManager.Accent;
            btnHotkey.FlatAppearance.BorderSize  = 1;
            btnHotkey.Location  = new Point(292, 4);
            btnHotkey.Size      = new Size(92, 24);
            btnHotkey.Cursor    = Cursors.Hand;
            btnHotkey.Click    += btnHotkey_Click;

            panelQuickRow.Controls.Add(labelLang);
            panelQuickRow.Controls.Add(comboLanguage);
            panelQuickRow.Controls.Add(labelHotkeyTitle);
            panelQuickRow.Controls.Add(btnHotkey);

            panelSettings.Controls.Add(labelSectionSettings);
            panelSettings.Controls.Add(chkAutostart);
            panelSettings.Controls.Add(chkPrimaryMonitor);
            panelSettings.Controls.Add(chkNeverResize);
            panelSettings.Controls.Add(panelQuickRow);

            // ── GAME PROFILES (card panel) ────────────────────────────────
            panelProfiles.Location  = new Point(14, 348);
            panelProfiles.Size      = new Size(452, 226);
            panelProfiles.BackColor = ThemeManager.Surface;
            panelProfiles.Paint    += CardPanel_Paint;

            labelSectionProfiles.Text      = "GAME PROFILES";
            labelSectionProfiles.Font      = new Font("Segoe UI", 7.5f, FontStyle.Bold);
            labelSectionProfiles.ForeColor = ThemeManager.Accent;
            labelSectionProfiles.BackColor = Color.Transparent;
            labelSectionProfiles.Location  = new Point(16, 10);
            labelSectionProfiles.AutoSize  = true;

            listProfiles.Location = new Point(10, 34);
            listProfiles.Size     = new Size(432, 152);
            listProfiles.View     = View.LargeIcon;
            listProfiles.BackColor= ThemeManager.Surface;
            listProfiles.ForeColor= ThemeManager.Text;
            listProfiles.UseCompatibleStateImageBehavior = false;
            listProfiles.BorderStyle   = BorderStyle.None;
            listProfiles.DoubleClick  += listProfiles_DoubleClick;

            btnAdd.Text      = "Add File";
            btnAdd.Font      = new Font("Segoe UI", 8.5f);
            btnAdd.ForeColor = ThemeManager.Text;
            btnAdd.BackColor = ThemeManager.Surface2;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.FlatAppearance.BorderColor = ThemeManager.Border;
            btnAdd.Location  = new Point(10, 194);
            btnAdd.Size      = new Size(108, 26);
            btnAdd.Cursor    = Cursors.Hand;
            btnAdd.Click    += btnAdd_Click;

            btnBrowse.Text      = "Browse Running";
            btnBrowse.Font      = new Font("Segoe UI", 8.5f);
            btnBrowse.ForeColor = ThemeManager.Text;
            btnBrowse.BackColor = ThemeManager.Surface2;
            btnBrowse.FlatStyle = FlatStyle.Flat;
            btnBrowse.FlatAppearance.BorderColor = ThemeManager.Border;
            btnBrowse.Location  = new Point(124, 194);
            btnBrowse.Size      = new Size(120, 26);
            btnBrowse.Cursor    = Cursors.Hand;
            btnBrowse.Click    += btnBrowse_Click;

            btnRemove.Text      = "Remove";
            btnRemove.Font      = new Font("Segoe UI", 8.5f);
            btnRemove.ForeColor = ThemeManager.Danger;
            btnRemove.BackColor = ThemeManager.Surface2;
            btnRemove.FlatStyle = FlatStyle.Flat;
            btnRemove.FlatAppearance.BorderColor = ThemeManager.Danger;
            btnRemove.Location  = new Point(250, 194);
            btnRemove.Size      = new Size(86, 26);
            btnRemove.Cursor    = Cursors.Hand;
            btnRemove.Click    += btnRemove_Click;

            panelProfiles.Controls.Add(labelSectionProfiles);
            panelProfiles.Controls.Add(listProfiles);
            panelProfiles.Controls.Add(btnAdd);
            panelProfiles.Controls.Add(btnBrowse);
            panelProfiles.Controls.Add(btnRemove);

            // ── STATUS BAR ────────────────────────────────────────────────
            panelStatus.Location  = new Point(0, 586);
            panelStatus.Size      = new Size(480, 42);
            panelStatus.BackColor = Color.FromArgb(224, 226, 236);

            labelStatus.Text      = "Initializing...";
            labelStatus.Font      = new Font("Segoe UI", 9f);
            labelStatus.ForeColor = ThemeManager.TextSub;
            labelStatus.Location  = new Point(14, 12);
            labelStatus.AutoSize  = true;

            labelGpuInfo.Text      = "";
            labelGpuInfo.Font      = new Font("Segoe UI", 8f);
            labelGpuInfo.ForeColor = ThemeManager.TextSub;
            labelGpuInfo.Location  = new Point(180, 14);
            labelGpuInfo.Size      = new Size(292, 18);
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
            panelQuickRow.ResumeLayout(false);
            panelProfiles.ResumeLayout(false);
            panelStatus.ResumeLayout(false);
            ResumeLayout(false);
        }

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
        private ToolStripMenuItem menuTrayToggle;
        private ToolStripMenuItem menuExit;

        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.ComponentModel.BackgroundWorker settingsWorker;
    }
}
