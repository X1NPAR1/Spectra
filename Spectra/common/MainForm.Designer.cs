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
            btnTheme        = new Button();
            btnOpenSettings = new Button();

            // Vibrance section (per-monitor sliders are built at runtime)
            panelVibrance        = new Panel();
            labelSectionVibrance = new Label();
            panelMonitorSliders  = new Panel();
            panelPresets         = new Panel();
            btnPresetDef         = new Button();
            btnPresetLow         = new Button();
            btnPresetHigh        = new Button();
            btnPresetMax         = new Button();

            // Display (brightness / contrast) section
            panelDisplay         = new Panel();
            labelSectionDisplay  = new Label();
            labelBrightness      = new Label();
            trackBrightness      = new TrackBar();
            labelBrightnessVal   = new Label();
            labelContrast        = new Label();
            trackContrast        = new TrackBar();
            labelContrastVal     = new Label();
            btnResetDisplay      = new Button();

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
            btnDetectGame        = new Button();

            // Status bar
            panelStatus    = new Panel();
            labelStatus    = new Label();
            labelGpuInfo   = new Label();
            btnGamingMode  = new Button();
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

            backgroundWorker = new System.ComponentModel.BackgroundWorker();
            settingsWorker   = new System.ComponentModel.BackgroundWorker();

            SuspendLayout();
            panelHeader.SuspendLayout();
            panelVibrance.SuspendLayout();
            panelPresets.SuspendLayout();
            panelDisplay.SuspendLayout();
            panelSettings.SuspendLayout();
            panelQuickRow.SuspendLayout();
            panelProfiles.SuspendLayout();
            panelStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBrightness).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackContrast).BeginInit();

            // ── FORM (height is finalised at runtime by LayoutCards) ──────
            AutoScaleDimensions = new SizeF(6f, 13f);
            AutoScaleMode       = AutoScaleMode.Font;
            ClientSize          = new Size(480, 760);
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
            panelHeader.Location = new Point(0, 0);
            panelHeader.Size     = new Size(480, 78);
            panelHeader.BackColor= Color.Transparent;
            panelHeader.Paint   += panelHeader_Paint;

            labelAppName.Text      = "SPECTRA";
            labelAppName.Font      = new Font("Segoe UI", 20f, FontStyle.Bold);
            labelAppName.ForeColor = Color.White;
            labelAppName.BackColor = Color.Transparent;
            labelAppName.Location  = new Point(70, 10);
            labelAppName.AutoSize  = true;

            labelVersion.Text      = "v2.1.0";
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
            labelGpuBadge.Location  = new Point(180, 8);
            labelGpuBadge.Size      = new Size(210, 36);

            btnTheme.Text      = "🌙";
            btnTheme.Font      = new Font("Segoe UI", 11f);
            btnTheme.ForeColor = Color.White;
            btnTheme.BackColor = Color.FromArgb(50, 255, 255, 255);
            btnTheme.FlatStyle = FlatStyle.Flat;
            btnTheme.FlatAppearance.BorderSize         = 0;
            btnTheme.FlatAppearance.MouseOverBackColor = Color.FromArgb(80, 255, 255, 255);
            btnTheme.Location  = new Point(398, 8);
            btnTheme.Size      = new Size(36, 36);
            btnTheme.Cursor    = Cursors.Hand;
            btnTheme.Click    += btnTheme_Click;

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
            panelHeader.Controls.Add(btnTheme);
            panelHeader.Controls.Add(btnOpenSettings);

            // ── DESKTOP VIBRANCE (size set at runtime) ────────────────────
            panelVibrance.Location  = new Point(14, 90);
            panelVibrance.Size      = new Size(452, 120);
            panelVibrance.BackColor = ThemeManager.Surface;
            panelVibrance.Paint    += CardPanel_Paint;

            labelSectionVibrance.Text      = "DESKTOP VIBRANCE";
            labelSectionVibrance.Font      = new Font("Segoe UI", 7.5f, FontStyle.Bold);
            labelSectionVibrance.ForeColor = ThemeManager.Accent;
            labelSectionVibrance.BackColor = Color.Transparent;
            labelSectionVibrance.Location  = new Point(16, 10);
            labelSectionVibrance.AutoSize  = true;

            // Container the per-monitor sliders are added into at runtime.
            panelMonitorSliders.Location  = new Point(14, 30);
            panelMonitorSliders.Size      = new Size(424, 40);
            panelMonitorSliders.BackColor = Color.Transparent;

            panelPresets.Location  = new Point(14, 74);
            panelPresets.Size      = new Size(430, 28);
            panelPresets.BackColor = Color.Transparent;

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
            panelVibrance.Controls.Add(panelMonitorSliders);
            panelVibrance.Controls.Add(panelPresets);

            // ── DISPLAY (brightness / contrast) ───────────────────────────
            panelDisplay.Location  = new Point(14, 220);
            panelDisplay.Size      = new Size(452, 116);
            panelDisplay.BackColor = ThemeManager.Surface;
            panelDisplay.Paint    += CardPanel_Paint;

            labelSectionDisplay.Text      = "BRIGHTNESS & CONTRAST";
            labelSectionDisplay.Font      = new Font("Segoe UI", 7.5f, FontStyle.Bold);
            labelSectionDisplay.ForeColor = ThemeManager.Accent;
            labelSectionDisplay.BackColor = Color.Transparent;
            labelSectionDisplay.Location  = new Point(16, 10);
            labelSectionDisplay.AutoSize  = true;

            labelBrightness.Text      = "Brightness";
            labelBrightness.Font      = new Font("Segoe UI", 8.5f);
            labelBrightness.ForeColor = ThemeManager.TextSub;
            labelBrightness.BackColor = Color.Transparent;
            labelBrightness.Location  = new Point(16, 34);
            labelBrightness.Size      = new Size(90, 18);

            trackBrightness.Location  = new Point(108, 30);
            trackBrightness.Size      = new Size(280, 30);
            trackBrightness.Minimum   = 0;
            trackBrightness.Maximum   = 100;
            trackBrightness.Value     = 50;
            trackBrightness.TickStyle = TickStyle.None;
            trackBrightness.BackColor = ThemeManager.Surface;
            trackBrightness.Scroll   += trackBrightness_Scroll;

            labelBrightnessVal.Text      = "50";
            labelBrightnessVal.Font      = new Font("Segoe UI", 9f, FontStyle.Bold);
            labelBrightnessVal.ForeColor = ThemeManager.Accent;
            labelBrightnessVal.BackColor = Color.Transparent;
            labelBrightnessVal.Location  = new Point(394, 34);
            labelBrightnessVal.Size      = new Size(44, 20);
            labelBrightnessVal.TextAlign = ContentAlignment.MiddleLeft;

            labelContrast.Text      = "Contrast";
            labelContrast.Font      = new Font("Segoe UI", 8.5f);
            labelContrast.ForeColor = ThemeManager.TextSub;
            labelContrast.BackColor = Color.Transparent;
            labelContrast.Location  = new Point(16, 66);
            labelContrast.Size      = new Size(90, 18);

            trackContrast.Location  = new Point(108, 62);
            trackContrast.Size      = new Size(280, 30);
            trackContrast.Minimum   = 0;
            trackContrast.Maximum   = 100;
            trackContrast.Value     = 50;
            trackContrast.TickStyle = TickStyle.None;
            trackContrast.BackColor = ThemeManager.Surface;
            trackContrast.Scroll   += trackContrast_Scroll;

            labelContrastVal.Text      = "50";
            labelContrastVal.Font      = new Font("Segoe UI", 9f, FontStyle.Bold);
            labelContrastVal.ForeColor = ThemeManager.Accent;
            labelContrastVal.BackColor = Color.Transparent;
            labelContrastVal.Location  = new Point(394, 66);
            labelContrastVal.Size      = new Size(44, 20);
            labelContrastVal.TextAlign = ContentAlignment.MiddleLeft;

            btnResetDisplay.Text      = "Reset";
            btnResetDisplay.Font      = new Font("Segoe UI", 7.5f);
            btnResetDisplay.ForeColor = ThemeManager.TextSub;
            btnResetDisplay.BackColor = ThemeManager.Surface2;
            btnResetDisplay.FlatStyle = FlatStyle.Flat;
            btnResetDisplay.FlatAppearance.BorderColor = ThemeManager.Border;
            btnResetDisplay.Location  = new Point(16, 92);
            btnResetDisplay.Size      = new Size(72, 20);
            btnResetDisplay.Cursor    = Cursors.Hand;
            btnResetDisplay.Click    += btnResetDisplay_Click;

            panelDisplay.Controls.Add(labelSectionDisplay);
            panelDisplay.Controls.Add(labelBrightness);
            panelDisplay.Controls.Add(trackBrightness);
            panelDisplay.Controls.Add(labelBrightnessVal);
            panelDisplay.Controls.Add(labelContrast);
            panelDisplay.Controls.Add(trackContrast);
            panelDisplay.Controls.Add(labelContrastVal);
            panelDisplay.Controls.Add(btnResetDisplay);

            // ── SETTINGS ─────────────────────────────────────────────────
            panelSettings.Location  = new Point(14, 346);
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
            chkAutostart.Size            = new Size(420, 20);
            chkAutostart.CheckedChanged += chkAutostart_CheckedChanged;

            chkPrimaryMonitor.Text            = "Primary monitor only";
            chkPrimaryMonitor.Font            = new Font("Segoe UI", 9f);
            chkPrimaryMonitor.ForeColor       = ThemeManager.Text;
            chkPrimaryMonitor.BackColor       = Color.Transparent;
            chkPrimaryMonitor.Location        = new Point(16, 58);
            chkPrimaryMonitor.Size            = new Size(420, 20);
            chkPrimaryMonitor.CheckedChanged += chkPrimaryMonitor_CheckedChanged;

            chkNeverResize.Text              = "Never change resolution";
            chkNeverResize.Font              = new Font("Segoe UI", 9f);
            chkNeverResize.ForeColor         = ThemeManager.Text;
            chkNeverResize.BackColor         = Color.Transparent;
            chkNeverResize.Location          = new Point(16, 82);
            chkNeverResize.Size              = new Size(420, 20);
            chkNeverResize.CheckedChanged   += chkNeverResize_CheckedChanged;

            panelQuickRow.Location  = new Point(0, 110);
            panelQuickRow.Size      = new Size(452, 36);
            panelQuickRow.BackColor = Color.Transparent;

            labelLang.Text      = "LANGUAGE";
            labelLang.Font      = new Font("Segoe UI", 7f, FontStyle.Bold);
            labelLang.ForeColor = ThemeManager.TextSub;
            labelLang.BackColor = Color.Transparent;
            labelLang.Location  = new Point(16, 10);
            labelLang.Size      = new Size(72, 16);
            labelLang.AutoSize  = false;

            comboLanguage.Location      = new Point(92, 6);
            comboLanguage.Size          = new Size(140, 22);
            comboLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
            comboLanguage.FlatStyle     = FlatStyle.Flat;
            comboLanguage.Font          = new Font("Segoe UI", 9f);
            comboLanguage.BackColor     = ThemeManager.Surface2;
            comboLanguage.ForeColor     = ThemeManager.Text;

            labelHotkeyTitle.Text      = "HOTKEY";
            labelHotkeyTitle.Font      = new Font("Segoe UI", 7f, FontStyle.Bold);
            labelHotkeyTitle.ForeColor = ThemeManager.TextSub;
            labelHotkeyTitle.BackColor = Color.Transparent;
            labelHotkeyTitle.Location  = new Point(248, 10);
            labelHotkeyTitle.Size      = new Size(76, 16);
            labelHotkeyTitle.AutoSize  = false;

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
            panelProfiles.Location  = new Point(14, 510);
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
            listProfiles.Size     = new Size(432, 148);
            listProfiles.View     = View.LargeIcon;
            listProfiles.BackColor= ThemeManager.Surface;
            listProfiles.ForeColor= ThemeManager.Text;
            listProfiles.UseCompatibleStateImageBehavior = false;
            listProfiles.BorderStyle   = BorderStyle.None;
            listProfiles.DoubleClick  += listProfiles_DoubleClick;

            // 4 equal columns: Add / Browse / Detect game / Remove
            btnAdd.Text      = "Add File";
            btnAdd.Font      = new Font("Segoe UI", 8f);
            btnAdd.ForeColor = ThemeManager.Text;
            btnAdd.BackColor = ThemeManager.Surface2;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.FlatAppearance.BorderColor = ThemeManager.Border;
            btnAdd.Location  = new Point(10, 190);
            btnAdd.Size      = new Size(104, 28);
            btnAdd.Cursor    = Cursors.Hand;
            btnAdd.Click    += btnAdd_Click;

            btnBrowse.Text      = "Browse Running";
            btnBrowse.Font      = new Font("Segoe UI", 8f);
            btnBrowse.ForeColor = ThemeManager.Text;
            btnBrowse.BackColor = ThemeManager.Surface2;
            btnBrowse.FlatStyle = FlatStyle.Flat;
            btnBrowse.FlatAppearance.BorderColor = ThemeManager.Border;
            btnBrowse.Location  = new Point(118, 190);
            btnBrowse.Size      = new Size(104, 28);
            btnBrowse.Cursor    = Cursors.Hand;
            btnBrowse.Click    += btnBrowse_Click;

            btnDetectGame.Text      = "Detect Game";
            btnDetectGame.Font      = new Font("Segoe UI", 8f);
            btnDetectGame.ForeColor = ThemeManager.Text;
            btnDetectGame.BackColor = ThemeManager.Surface2;
            btnDetectGame.FlatStyle = FlatStyle.Flat;
            btnDetectGame.FlatAppearance.BorderColor = ThemeManager.Border;
            btnDetectGame.Location  = new Point(226, 190);
            btnDetectGame.Size      = new Size(104, 28);
            btnDetectGame.Cursor    = Cursors.Hand;
            btnDetectGame.Click    += btnDetectGame_Click;

            btnRemove.Text      = "Remove";
            btnRemove.Font      = new Font("Segoe UI", 8f);
            btnRemove.ForeColor = ThemeManager.Danger;
            btnRemove.BackColor = ThemeManager.Surface2;
            btnRemove.FlatStyle = FlatStyle.Flat;
            btnRemove.FlatAppearance.BorderColor = ThemeManager.Danger;
            btnRemove.Location  = new Point(334, 190);
            btnRemove.Size      = new Size(108, 28);
            btnRemove.Cursor    = Cursors.Hand;
            btnRemove.Click    += btnRemove_Click;

            panelProfiles.Controls.Add(labelSectionProfiles);
            panelProfiles.Controls.Add(listProfiles);
            panelProfiles.Controls.Add(btnAdd);
            panelProfiles.Controls.Add(btnBrowse);
            panelProfiles.Controls.Add(btnDetectGame);
            panelProfiles.Controls.Add(btnRemove);

            // ── STATUS BAR ────────────────────────────────────────────────
            panelStatus.Location  = new Point(0, 752);
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
            labelGpuInfo.Location  = new Point(120, 13);
            labelGpuInfo.Size      = new Size(130, 16);
            labelGpuInfo.TextAlign = ContentAlignment.MiddleRight;

            btnGamingMode.Text      = "🎮";
            btnGamingMode.Font      = new Font("Segoe UI", 9f, FontStyle.Bold);
            btnGamingMode.ForeColor = Color.White;
            btnGamingMode.BackColor = ThemeManager.Accent;
            btnGamingMode.FlatStyle = FlatStyle.Flat;
            btnGamingMode.FlatAppearance.BorderSize = 0;
            btnGamingMode.Location  = new Point(330, 6);
            btnGamingMode.Size      = new Size(66, 26);
            btnGamingMode.Cursor    = Cursors.Hand;
            btnGamingMode.Click    += btnGamingMode_Click;

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
            panelStatus.Controls.Add(btnGamingMode);
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

            menuGitHub.Text      = "⭐  GitHub";
            menuGitHub.Font      = new Font("Segoe UI", 8.5f, FontStyle.Underline);
            menuGitHub.ForeColor = System.Drawing.Color.FromArgb(22, 68, 148);
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

            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork           += backgroundWorker_DoWork;
            backgroundWorker.ProgressChanged  += backgroundWorker_ProgressChanged;
            settingsWorker.DoWork             += settingsWorker_DoWork;
            settingsWorker.RunWorkerCompleted += settingsWorker_RunWorkerCompleted;

            Controls.Add(panelHeader);
            Controls.Add(panelVibrance);
            Controls.Add(panelDisplay);
            Controls.Add(panelSettings);
            Controls.Add(panelProfiles);
            Controls.Add(panelStatus);

            ((System.ComponentModel.ISupportInitialize)trackBrightness).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackContrast).EndInit();
            panelHeader.ResumeLayout(false);
            panelVibrance.ResumeLayout(false);
            panelPresets.ResumeLayout(false);
            panelDisplay.ResumeLayout(false);
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
        private Button    btnTheme;
        private Button    btnOpenSettings;

        private Panel     panelVibrance;
        private Label     labelSectionVibrance;
        private Panel     panelMonitorSliders;
        private Panel     panelPresets;
        private Button    btnPresetDef, btnPresetLow, btnPresetHigh, btnPresetMax;

        private Panel     panelDisplay;
        private Label     labelSectionDisplay;
        private Label     labelBrightness;
        private TrackBar  trackBrightness;
        private Label     labelBrightnessVal;
        private Label     labelContrast;
        private TrackBar  trackContrast;
        private Label     labelContrastVal;
        private Button    btnResetDisplay;

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
        private Button    btnDetectGame;
        private Button    btnRemove;

        private Panel     panelStatus;
        private Label     labelStatus;
        private Label     labelGpuInfo;
        private Button    btnGamingMode;
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
