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
            panelPresets         = new Panel();
            btnPresetDef         = new Button();
            btnPresetLow         = new Button();
            btnPresetHigh        = new Button();
            btnPresetMax         = new Button();

            // Settings section
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

            // Profiles section
            panelProfiles        = new Panel();
            labelSectionProfiles = new Label();
            listProfiles         = new ListView();
            btnAdd               = new Button();
            btnBrowse            = new Button();
            btnRemove            = new Button();

            // Status bar
            panelStatus    = new Panel();
            labelStatus    = new Label();
            labelGpuInfo   = new Label();
            btnToggleVibrance = new Button();

            // Tray
            notifyIcon           = new NotifyIcon(components);
            contextMenu          = new ContextMenuStrip(components);
            menuOpenSpectra      = new ToolStripMenuItem();
            menuTrayToggle       = new ToolStripMenuItem();
            menuTrayPresets      = new ToolStripMenuItem();
            menuTrayPresetDef    = new ToolStripMenuItem();
            menuTrayPresetLow    = new ToolStripMenuItem();
            menuTrayPresetHigh   = new ToolStripMenuItem();
            menuTrayPresetMax    = new ToolStripMenuItem();
            menuExit             = new ToolStripMenuItem();
            menuGitHub           = new ToolStripMenuItem();

            // Workers
            backgroundWorker = new System.ComponentModel.BackgroundWorker();
            settingsWorker   = new System.ComponentModel.BackgroundWorker();

            SuspendLayout();
            panelHeader.SuspendLayout();
            panelVibrance.SuspendLayout();
            panelPresets.SuspendLayout();
            panelSettings.SuspendLayout();
            panelQuickRow.SuspendLayout();
            panelProfiles.SuspendLayout();
            panelStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarVibrance).BeginInit();

            // ── FORM ─────────────────────────────────────────────────────
            AutoScaleDimensions = new SizeF(6f, 13f);
            AutoScaleMode       = AutoScaleMode.Font;
            ClientSize          = new Size(480, 670);
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

            // ── HEADER ───────────────────────────────────────────────────
            // Icon is drawn in panelHeader_Paint using IconFactory.GetAppBitmap
            panelHeader.Location = new Point(0, 0);
            panelHeader.Size     = new Size(480, 78);
            panelHeader.BackColor= Color.Transparent;
            panelHeader.Paint   += panelHeader_Paint;

            // App name — positioned to leave room for 44px icon on left
            labelAppName.Text      = "SPECTRA";
            labelAppName.Font      = new Font("Segoe UI", 20f, FontStyle.Bold);
            labelAppName.ForeColor = Color.White;
            labelAppName.BackColor = Color.Transparent;
            labelAppName.Location  = new Point(70, 10);
            labelAppName.AutoSize  = true;

            labelVersion.Text      = "v1.9.5";
            labelVersion.Font      = new Font("Segoe UI", 7.5f);
            labelVersion.ForeColor = Color.FromArgb(180, 215, 255);
            labelVersion.BackColor = Color.Transparent;
            labelVersion.Location  = new Point(72, 50);
            labelVersion.AutoSize  = true;

            labelGpuBadge.Text      = "";
            labelGpuBadge.Font      = new Font("Segoe UI", 8f);
            labelGpuBadge.ForeColor = Color.FromArgb(200, 230, 255);
            labelGpuBadge.BackColor = Color.Transparent;
            labelGpuBadge.TextAlign = ContentAlignment.MiddleRight;
            labelGpuBadge.Location  = new Point(190, 8);
            labelGpuBadge.Size      = new Size(244, 36);

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

            // ── DESKTOP VIBRANCE ─────────────────────────────────────────
            panelVibrance.Location  = new Point(14, 90);
            panelVibrance.Size      = new Size(452, 122);   // +14px: prevents trackbar thumb from overlapping presets
            panelVibrance.BackColor = ThemeManager.Surface;
            panelVibrance.Paint    += CardPanel_Paint;

            labelSectionVibrance.Text      = "DESKTOP VIBRANCE";
            labelSectionVibrance.Font      = new Font("Segoe UI", 7.5f, FontStyle.Bold);
            labelSectionVibrance.ForeColor = ThemeManager.Accent;
            labelSectionVibrance.BackColor = Color.Transparent;
            labelSectionVibrance.Location  = new Point(16, 10);
            labelSectionVibrance.AutoSize  = true;

            trackBarVibrance.Location  = new Point(14, 32);
            trackBarVibrance.Size      = new Size(374, 30);
            trackBarVibrance.TickStyle = TickStyle.None;
            trackBarVibrance.BackColor = ThemeManager.Surface;
            trackBarVibrance.Scroll   += trackBarVibrance_Scroll;

            labelVibranceValue.Text      = "—";
            labelVibranceValue.Font      = new Font("Segoe UI", 14f, FontStyle.Bold);
            labelVibranceValue.ForeColor = ThemeManager.Accent;
            labelVibranceValue.BackColor = Color.Transparent;
            labelVibranceValue.Location  = new Point(392, 33);
            labelVibranceValue.Size      = new Size(52, 26);
            labelVibranceValue.TextAlign = ContentAlignment.MiddleLeft;

            // Presets sub-panel — Y=84 leaves ≥22px gap below trackbar (which ends at ~62) so the
            // Windows thumb control never visually overlaps the buttons at any DPI setting
            panelPresets.Location  = new Point(14, 84);
            panelPresets.Size      = new Size(430, 28);
            panelPresets.BackColor = Color.Transparent;

            // Helper to create preset button
            int bx = 0;
            foreach (var pair in new[]
            {
                new { Btn = btnPresetDef,  Text = "Default", Tag = "def" },
                new { Btn = btnPresetLow,  Text = "Low",     Tag = "low" },
                new { Btn = btnPresetHigh, Text = "High",    Tag = "high" },
                new { Btn = btnPresetMax,  Text = "Max",     Tag = "max" },
            })
            {
                pair.Btn.Text      = pair.Text;
                pair.Btn.Font      = new Font("Segoe UI", 7.5f);
                pair.Btn.ForeColor = ThemeManager.TextSub;
                pair.Btn.BackColor = ThemeManager.Surface2;
                pair.Btn.FlatStyle = FlatStyle.Flat;
                pair.Btn.FlatAppearance.BorderColor = ThemeManager.Border;
                pair.Btn.FlatAppearance.BorderSize  = 1;
                pair.Btn.Location  = new Point(bx, 0);
                pair.Btn.Size      = new Size(78, 22);
                pair.Btn.Tag       = pair.Tag;
                pair.Btn.Cursor    = Cursors.Hand;
                pair.Btn.Click    += btnPreset_Click;
                panelPresets.Controls.Add(pair.Btn);
                bx += 84;
            }

            panelVibrance.Controls.Add(labelSectionVibrance);
            panelVibrance.Controls.Add(trackBarVibrance);
            panelVibrance.Controls.Add(labelVibranceValue);
            panelVibrance.Controls.Add(panelPresets);

            // ── SETTINGS ─────────────────────────────────────────────────
            panelSettings.Location  = new Point(14, 224);
            panelSettings.Size      = new Size(452, 152);
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
            chkAutostart.Size            = new Size(220, 20);
            chkAutostart.CheckedChanged += chkAutostart_CheckedChanged;

            chkPrimaryMonitor.Text            = "Primary monitor only";
            chkPrimaryMonitor.Font            = new Font("Segoe UI", 9f);
            chkPrimaryMonitor.ForeColor       = ThemeManager.Text;
            chkPrimaryMonitor.BackColor       = Color.Transparent;
            chkPrimaryMonitor.Location        = new Point(16, 58);
            chkPrimaryMonitor.Size            = new Size(220, 20);
            chkPrimaryMonitor.CheckedChanged += chkPrimaryMonitor_CheckedChanged;

            chkNeverResize.Text              = "Never change resolution";
            chkNeverResize.Font              = new Font("Segoe UI", 9f);
            chkNeverResize.ForeColor         = ThemeManager.Text;
            chkNeverResize.BackColor         = Color.Transparent;
            chkNeverResize.Location          = new Point(16, 82);
            chkNeverResize.Size              = new Size(220, 20);
            chkNeverResize.CheckedChanged   += chkNeverResize_CheckedChanged;

            // Quick row: LANGUAGE | HOTKEY  — fixed widths, no autosize → no overflow in any language
            panelQuickRow.Location  = new Point(0, 110);
            panelQuickRow.Size      = new Size(452, 36);
            panelQuickRow.BackColor = Color.Transparent;

            // LANG label — FIXED width 72px (accommodates "LANGUAGE" 8 chars, "SNELTOETS" not here)
            labelLang.Text      = "LANGUAGE";
            labelLang.Font      = new Font("Segoe UI", 7f, FontStyle.Bold);
            labelLang.ForeColor = ThemeManager.TextSub;
            labelLang.BackColor = Color.Transparent;
            labelLang.Location  = new Point(16, 10);
            labelLang.Size      = new Size(72, 16);   // FIXED — no AutoSize
            labelLang.AutoSize  = false;

            // Language combo — starts after fixed label
            comboLanguage.Location      = new Point(92, 6);
            comboLanguage.Size          = new Size(140, 22);
            comboLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
            comboLanguage.FlatStyle     = FlatStyle.Flat;
            comboLanguage.Font          = new Font("Segoe UI", 9f);
            comboLanguage.BackColor     = ThemeManager.Surface2;
            comboLanguage.ForeColor     = ThemeManager.Text;

            // HOTKEY label — FIXED width 76px (accommodates "КЛАВИША" ~63px, "SNELTOETS" ~68px)
            labelHotkeyTitle.Text      = "HOTKEY";
            labelHotkeyTitle.Font      = new Font("Segoe UI", 7f, FontStyle.Bold);
            labelHotkeyTitle.ForeColor = ThemeManager.TextSub;
            labelHotkeyTitle.BackColor = Color.Transparent;
            labelHotkeyTitle.Location  = new Point(248, 10);
            labelHotkeyTitle.Size      = new Size(76, 16);   // FIXED — no AutoSize
            labelHotkeyTitle.AutoSize  = false;

            // Hotkey button — starts after fixed label area
            btnHotkey.Text      = "F9";
            btnHotkey.Font      = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            btnHotkey.ForeColor = ThemeManager.Accent;
            btnHotkey.BackColor = ThemeManager.Surface2;
            btnHotkey.FlatStyle = FlatStyle.Flat;
            btnHotkey.FlatAppearance.BorderColor = ThemeManager.Accent;
            btnHotkey.FlatAppearance.BorderSize  = 1;
            btnHotkey.Location  = new Point(330, 4);
            btnHotkey.Size      = new Size(96, 24);
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

            // ── GAME PROFILES ─────────────────────────────────────────────
            panelProfiles.Location  = new Point(14, 388);
            panelProfiles.Size      = new Size(452, 230);
            panelProfiles.BackColor = ThemeManager.Surface;
            panelProfiles.Paint    += CardPanel_Paint;

            labelSectionProfiles.Text      = "GAME PROFILES";
            labelSectionProfiles.Font      = new Font("Segoe UI", 7.5f, FontStyle.Bold);
            labelSectionProfiles.ForeColor = ThemeManager.Accent;
            labelSectionProfiles.BackColor = Color.Transparent;
            labelSectionProfiles.Location  = new Point(16, 10);
            labelSectionProfiles.AutoSize  = true;

            listProfiles.Location = new Point(10, 34);
            listProfiles.Size     = new Size(432, 156);
            listProfiles.View     = View.LargeIcon;
            listProfiles.BackColor= ThemeManager.Surface;
            listProfiles.ForeColor= ThemeManager.Text;
            listProfiles.UseCompatibleStateImageBehavior = false;
            listProfiles.BorderStyle   = BorderStyle.None;
            listProfiles.DoubleClick  += listProfiles_DoubleClick;

            // Profile buttons — 3 equal columns aligned to listProfiles edges (X=10..442, W=432)
            // Each button: (432 - 2*8 gap) / 3 = 138px. Fits longest translations (French ~120px + padding).
            btnAdd.Text      = "Add File";
            btnAdd.Font      = new Font("Segoe UI", 8.5f);
            btnAdd.ForeColor = ThemeManager.Text;
            btnAdd.BackColor = ThemeManager.Surface2;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.FlatAppearance.BorderColor = ThemeManager.Border;
            btnAdd.Location  = new Point(10, 198);
            btnAdd.Size      = new Size(138, 26);
            btnAdd.Cursor    = Cursors.Hand;
            btnAdd.Click    += btnAdd_Click;

            btnBrowse.Text      = "Browse Running";
            btnBrowse.Font      = new Font("Segoe UI", 8.5f);
            btnBrowse.ForeColor = ThemeManager.Text;
            btnBrowse.BackColor = ThemeManager.Surface2;
            btnBrowse.FlatStyle = FlatStyle.Flat;
            btnBrowse.FlatAppearance.BorderColor = ThemeManager.Border;
            btnBrowse.Location  = new Point(156, 198);
            btnBrowse.Size      = new Size(138, 26);
            btnBrowse.Cursor    = Cursors.Hand;
            btnBrowse.Click    += btnBrowse_Click;

            btnRemove.Text      = "Remove";
            btnRemove.Font      = new Font("Segoe UI", 8.5f);
            btnRemove.ForeColor = ThemeManager.Danger;
            btnRemove.BackColor = ThemeManager.Surface2;
            btnRemove.FlatStyle = FlatStyle.Flat;
            btnRemove.FlatAppearance.BorderColor = ThemeManager.Danger;
            btnRemove.Location  = new Point(302, 198);
            btnRemove.Size      = new Size(138, 26);
            btnRemove.Cursor    = Cursors.Hand;
            btnRemove.Click    += btnRemove_Click;

            panelProfiles.Controls.Add(labelSectionProfiles);
            panelProfiles.Controls.Add(listProfiles);
            panelProfiles.Controls.Add(btnAdd);
            panelProfiles.Controls.Add(btnBrowse);
            panelProfiles.Controls.Add(btnRemove);

            // ── STATUS BAR ────────────────────────────────────────────────
            panelStatus.Location  = new Point(0, 630);
            panelStatus.Size      = new Size(480, 40);
            panelStatus.BackColor = Color.FromArgb(210, 222, 238);

            labelStatus.Text      = "Initializing...";
            labelStatus.Font      = new Font("Segoe UI", 9f);
            labelStatus.ForeColor = ThemeManager.TextSub;
            labelStatus.Location  = new Point(14, 11);
            labelStatus.AutoSize  = true;

            labelGpuInfo.Text      = "";
            labelGpuInfo.Font      = new Font("Segoe UI", 8f);
            labelGpuInfo.ForeColor = ThemeManager.TextSub;
            labelGpuInfo.Location  = new Point(170, 13);
            labelGpuInfo.Size      = new Size(220, 16);
            labelGpuInfo.TextAlign = ContentAlignment.MiddleRight;

            // Toggle ON/OFF button — far right of status bar
            btnToggleVibrance.Text      = "ON";
            btnToggleVibrance.Font      = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            btnToggleVibrance.ForeColor = Color.White;
            btnToggleVibrance.BackColor = ThemeManager.Success;
            btnToggleVibrance.FlatStyle = FlatStyle.Flat;
            btnToggleVibrance.FlatAppearance.BorderSize = 0;
            btnToggleVibrance.Location  = new Point(402, 6);
            btnToggleVibrance.Size      = new Size(66, 26);
            btnToggleVibrance.Cursor    = Cursors.Hand;
            btnToggleVibrance.Click    += btnToggleVibrance_Click;

            panelStatus.Controls.Add(labelStatus);
            panelStatus.Controls.Add(labelGpuInfo);
            panelStatus.Controls.Add(btnToggleVibrance);

            // ── TRAY ──────────────────────────────────────────────────────
            menuOpenSpectra.Text  = "Open Spectra";
            menuOpenSpectra.Font  = new Font("Segoe UI", 9f, FontStyle.Bold);
            menuOpenSpectra.Click += menuOpenSpectra_Click;

            menuTrayToggle.Text  = "Toggle Vibrance";
            menuTrayToggle.Click += menuTrayToggle_Click;

            menuTrayPresets.Text = "Quick Presets";
            menuTrayPresetDef.Text  = "Default"; menuTrayPresetDef.Tag  = "def";  menuTrayPresetDef.Click  += menuTrayPreset_Click;
            menuTrayPresetLow.Text  = "Low";     menuTrayPresetLow.Tag  = "low";  menuTrayPresetLow.Click  += menuTrayPreset_Click;
            menuTrayPresetHigh.Text = "High";    menuTrayPresetHigh.Tag = "high"; menuTrayPresetHigh.Click += menuTrayPreset_Click;
            menuTrayPresetMax.Text  = "Max";     menuTrayPresetMax.Tag  = "max";  menuTrayPresetMax.Click  += menuTrayPreset_Click;
            menuTrayPresets.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                menuTrayPresetDef, menuTrayPresetLow, menuTrayPresetHigh, menuTrayPresetMax
            });

            menuExit.Text  = "Exit";
            menuExit.Click += exitMenuItem_Click;

            // GitHub link — underlined, below Exit
            menuGitHub.Text      = "⭐  GitHub";
            menuGitHub.Font      = new Font("Segoe UI", 8.5f, FontStyle.Underline);
            menuGitHub.ForeColor = System.Drawing.Color.FromArgb(22, 68, 148); // ThemeManager.Accent
            menuGitHub.Click    += menuGitHub_Click;

            contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                menuOpenSpectra,
                new ToolStripSeparator(),
                menuTrayToggle,
                menuTrayPresets,
                new ToolStripSeparator(),
                menuExit,
                new ToolStripSeparator(),
                menuGitHub
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
            panelPresets.ResumeLayout(false);
            panelSettings.ResumeLayout(false);
            panelQuickRow.ResumeLayout(false);
            panelProfiles.ResumeLayout(false);
            panelStatus.ResumeLayout(false);
            ResumeLayout(false);
        }

        // ── Fields ────────────────────────────────────────────────────────
        private Panel     panelHeader;
        private Label     labelAppName;
        private Label     labelVersion;
        private Label     labelGpuBadge;
        private Button    btnOpenSettings;

        private Panel     panelVibrance;
        private Label     labelSectionVibrance;
        private TrackBar  trackBarVibrance;
        private Label     labelVibranceValue;
        private Panel     panelPresets;
        private Button    btnPresetDef, btnPresetLow, btnPresetHigh, btnPresetMax;

        private Panel     panelSettings;
        private Label     labelSectionSettings;
        private CheckBox  chkAutostart;
        private CheckBox  chkPrimaryMonitor;
        private CheckBox  chkNeverResize;
        private Panel     panelQuickRow;
        private Label     labelLang;
        private ComboBox  comboLanguage;
        private Label     labelHotkeyTitle;
        private Button    btnHotkey;

        private Panel     panelProfiles;
        private Label     labelSectionProfiles;
        private ListView  listProfiles;
        private Button    btnAdd;
        private Button    btnBrowse;
        private Button    btnRemove;

        private Panel     panelStatus;
        private Label     labelStatus;
        private Label     labelGpuInfo;
        private Button    btnToggleVibrance;

        private NotifyIcon        notifyIcon;
        private ContextMenuStrip  contextMenu;
        private ToolStripMenuItem menuOpenSpectra;
        private ToolStripMenuItem menuTrayToggle;
        private ToolStripMenuItem menuTrayPresets;
        private ToolStripMenuItem menuTrayPresetDef, menuTrayPresetLow, menuTrayPresetHigh, menuTrayPresetMax;
        private ToolStripMenuItem menuExit;
        private ToolStripMenuItem menuGitHub;

        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.ComponentModel.BackgroundWorker settingsWorker;
    }
}
