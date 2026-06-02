using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Spectra.Localization;
using Spectra.UI;

namespace Spectra.common
{
    public partial class MainForm : Form
    {
        // ── Proxy / config ────────────────────────────────────────────────
        private readonly int _defaultWindowsLevel;
        private readonly int _minLevel;
        private readonly int _maxLevel;
        private readonly int _defaultIngameLevel;
        private readonly Func<int, string> _resolveLevelLabel;
        private readonly IVibranceProxy _proxy;
        private IRegistryController _registry;
        private List<ApplicationSetting> _profiles;
        private readonly List<ResolutionModeWrapper> _primaryResolutions;
        private readonly Dictionary<string, Tuple<ResolutionModeWrapper, List<ResolutionModeWrapper>>> _resolutionMap;

        // ── State ─────────────────────────────────────────────────────────
        private bool _allowVisible    = true;
        private bool _vibranceEnabled = true;
        private HotkeyManager _hotkey;
        private bool _capturingHotkey;
        private const string AppName = "Spectra";

        private int _presetDef, _presetLow, _presetHigh, _presetMax;

        // ── Per-monitor vibrance ──────────────────────────────────────────
        private readonly List<string> _monitorDevices = new List<string>();
        private readonly Dictionary<string, TrackBar> _monitorSliders     = new Dictionary<string, TrackBar>();
        private readonly Dictionary<string, Label>    _monitorValueLabels = new Dictionary<string, Label>();
        private readonly Dictionary<string, int>      _monitorLevels      = new Dictionary<string, int>();

        // ── New v2.1 features ─────────────────────────────────────────────
        private ScheduleManager _schedule;
        private bool _gamingMode;                 // true = profiles temporarily disabled
        private bool _minimizeToTray;
        private bool _showNotifications;
        private bool _startedMinimized;

        public MainForm(
            Func<List<ApplicationSetting>,
                 Dictionary<string, Tuple<ResolutionModeWrapper, List<ResolutionModeWrapper>>>,
                 IVibranceProxy> getProxy,
            int defaultWindowsLevel, int minLevel, int maxLevel,
            int defaultIngameLevel, Func<int, string> resolveLevelLabel)
        {
            _defaultWindowsLevel = defaultWindowsLevel;
            _minLevel            = minLevel;
            _maxLevel            = maxLevel;
            _defaultIngameLevel  = defaultIngameLevel;
            _resolveLevelLabel   = resolveLevelLabel;
            _profiles            = new List<ApplicationSetting>();
            _resolutionMap       = new Dictionary<string, Tuple<ResolutionModeWrapper, List<ResolutionModeWrapper>>>();
            _primaryResolutions  = new List<ResolutionModeWrapper>();

            int range   = maxLevel - minLevel;
            _presetDef  = defaultWindowsLevel;
            _presetLow  = minLevel + (int)(range * 0.25);
            _presetHigh = minLevel + (int)(range * 0.75);
            _presetMax  = maxLevel;

            foreach (Screen screen in Screen.AllScreens)
            {
                if (ResolutionHelper.GetCurrentResolutionSettings(out Devmode dm, screen.DeviceName))
                {
                    var avail = ResolutionHelper.EnumerateSupportedResolutionModes(screen.DeviceName);
                    if (screen.Primary) _primaryResolutions.AddRange(avail);
                    _resolutionMap[screen.DeviceName] = Tuple.Create(new ResolutionModeWrapper(dm), avail);
                }
            }

            InitializeComponent();

            Icon            = IconFactory.GetAppIcon(32);
            notifyIcon.Icon = IconFactory.GetAppIcon(16);

            _proxy = getProxy(_profiles, _resolutionMap);

            BuildMonitorSliders();
            LayoutCards();

            backgroundWorker.WorkerReportsProgress = true;
            settingsWorker.WorkerReportsProgress   = true;

            _schedule = new ScheduleManager { DayLevel = _presetDef, NightLevel = _presetLow };
            _schedule.ApplyLevel += OnScheduleApply;

            LocalizationManager.LanguageChanged += (s, e) =>
            {
                if (!IsDisposed && IsHandleCreated) Invoke((Action)ApplyLocalization);
            };

            ApplyLocalization();
            ApplyTheme();
            backgroundWorker.RunWorkerAsync();
        }

        // ── Per-monitor slider construction ───────────────────────────────
        private void BuildMonitorSliders()
        {
            _monitorDevices.Clear();
            panelMonitorSliders.Controls.Clear();

            var screens = Screen.AllScreens.OrderByDescending(s => s.Primary).ToArray();
            bool multi = screens.Length > 1;
            int y = 0, idx = 1;

            foreach (Screen screen in screens)
            {
                string device = screen.DeviceName;
                _monitorDevices.Add(device);
                _monitorLevels[device] = _defaultWindowsLevel;

                var lbl = new Label
                {
                    Text      = multi ? $"Monitor {idx}" + (screen.Primary ? " ★" : "") : "All displays",
                    Font      = new Font("Segoe UI", 8f),
                    ForeColor = ThemeManager.TextSub,
                    BackColor = Color.Transparent,
                    Location  = new Point(0, y + 4),
                    Size      = new Size(78, 18)
                };

                var bar = new TrackBar
                {
                    Minimum   = _minLevel,
                    Maximum   = _maxLevel,
                    Value     = _defaultWindowsLevel,
                    TickStyle = TickStyle.None,
                    BackColor = ThemeManager.Surface,
                    Location  = new Point(80, y),
                    Size      = new Size(290, 30),
                    Tag       = device
                };
                bar.Scroll += MonitorSlider_Scroll;

                var val = new Label
                {
                    Text      = _resolveLevelLabel(_defaultWindowsLevel),
                    Font      = new Font("Segoe UI", 11f, FontStyle.Bold),
                    ForeColor = ThemeManager.Accent,
                    BackColor = Color.Transparent,
                    Location  = new Point(374, y + 4),
                    Size      = new Size(50, 22),
                    TextAlign = ContentAlignment.MiddleLeft
                };

                panelMonitorSliders.Controls.Add(lbl);
                panelMonitorSliders.Controls.Add(bar);
                panelMonitorSliders.Controls.Add(val);

                _monitorSliders[device]     = bar;
                _monitorValueLabels[device] = val;

                y += 36;
                idx++;
            }

            panelMonitorSliders.Size = new Size(424, Math.Max(36, y));
        }

        // Repositions cards vertically so the form grows with the monitor count.
        private void LayoutCards()
        {
            int sliderArea = panelMonitorSliders.Height;
            panelMonitorSliders.Location = new Point(14, 30);
            panelPresets.Location = new Point(14, 30 + sliderArea + 8);
            panelVibrance.Size    = new Size(452, panelPresets.Bottom + 12);

            panelDisplay.Location  = new Point(14, panelVibrance.Bottom + 12);
            panelSettings.Location = new Point(14, panelDisplay.Bottom + 12);
            panelProfiles.Location = new Point(14, panelSettings.Bottom + 12);
            panelStatus.Location   = new Point(0, panelProfiles.Bottom + 12);
            panelStatus.Size       = new Size(480, 40);

            ClientSize = new Size(480, panelStatus.Bottom);
        }

        private void MonitorSlider_Scroll(object sender, EventArgs e)
        {
            var bar = (TrackBar)sender;
            string device = (string)bar.Tag;
            ApplyMonitorLevel(device, bar.Value);
            HighlightActivePreset();
            TriggerSettingsSave();
        }

        private void ApplyMonitorLevel(string device, int level)
        {
            level = Math.Max(_minLevel, Math.Min(_maxLevel, level));
            _monitorLevels[device] = level;
            if (_monitorSliders.TryGetValue(device, out var bar)) bar.Value = level;
            if (_monitorValueLabels.TryGetValue(device, out var lbl)) lbl.Text = _resolveLevelLabel(level);

            if (_vibranceEnabled)
            {
                // Single-monitor systems use the global path; multi-monitor targets the device.
                if (_monitorDevices.Count <= 1) _proxy?.SetVibranceWindowsLevel(level);
                else _proxy?.SetVibranceForMonitor(device, level);
            }
        }

        private int PrimaryLevel =>
            _monitorDevices.Count > 0 && _monitorLevels.TryGetValue(_monitorDevices[0], out var v)
                ? v : _defaultWindowsLevel;

        // ── Visibility ────────────────────────────────────────────────────
        protected override void SetVisibleCore(bool value)
        {
            if (!_allowVisible) { value = false; if (!IsHandleCreated) CreateHandle(); }
            base.SetVisibleCore(value);
        }

        public void SetAllowVisible(bool v) { _allowVisible = v; _startedMinimized = !v; }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            _hotkey = new HotkeyManager(Handle);
            _hotkey.Register(Keys.F9);
            _hotkey.TogglePressed += OnHotkeyToggle;
            UpdateHotkeyLabel();
        }

        protected override void WndProc(ref Message m)
        {
            _hotkey?.ProcessMessage(m);
            base.WndProc(ref m);
        }

        public HotkeyManager GetHotkeyManager() => _hotkey;
        public List<ApplicationSetting> GetProfiles() => _profiles;

        // ── Localization ──────────────────────────────────────────────────
        private void ApplyLocalization()
        {
            if (IsDisposed) return;

            labelSectionVibrance.Text  = LocalizationManager.Get("DesktopVibrance");
            labelSectionDisplay.Text   = LocalizationManager.Get("BrightnessContrast");
            labelBrightness.Text       = LocalizationManager.Get("Brightness");
            labelContrast.Text         = LocalizationManager.Get("Contrast");
            btnResetDisplay.Text       = LocalizationManager.Get("Reset");
            labelSectionSettings.Text  = LocalizationManager.Get("Settings");
            labelSectionProfiles.Text  = LocalizationManager.Get("GameProfiles");
            chkAutostart.Text          = LocalizationManager.Get("Autostart");
            chkPrimaryMonitor.Text     = LocalizationManager.Get("PrimaryMonitor");
            chkNeverResize.Text        = LocalizationManager.Get("NeverResize");
            labelLang.Text             = LocalizationManager.Get("LangLabel");
            labelHotkeyTitle.Text      = LocalizationManager.Get("HotkeyLabel");
            btnAdd.Text                = LocalizationManager.Get("AddFile");
            btnBrowse.Text             = LocalizationManager.Get("BrowseRunning");
            btnDetectGame.Text         = LocalizationManager.Get("DetectGame");
            btnRemove.Text             = LocalizationManager.Get("Remove");

            btnPresetDef.Text  = LocalizationManager.Get("PresetDefault");
            btnPresetLow.Text  = LocalizationManager.Get("PresetLow");
            btnPresetHigh.Text = LocalizationManager.Get("PresetHigh");
            btnPresetMax.Text  = LocalizationManager.Get("PresetMax");

            menuOpenSpectra.Text    = LocalizationManager.Get("OpenSpectra");
            menuTrayToggle.Text     = LocalizationManager.Get("TrayToggle");
            menuTrayPresets.Text    = LocalizationManager.Get("QuickPresets");
            menuTrayPresetDef.Text  = LocalizationManager.Get("PresetDefault");
            menuTrayPresetLow.Text  = LocalizationManager.Get("PresetLow");
            menuTrayPresetHigh.Text = LocalizationManager.Get("PresetHigh");
            menuTrayPresetMax.Text  = LocalizationManager.Get("PresetMax");
            menuExit.Text           = LocalizationManager.Get("Exit");
            notifyIcon.Text         = AppName;

            comboLanguage.SelectedIndexChanged -= comboLanguage_SelectedIndexChanged;
            if (comboLanguage.Items.Count > 0)
                comboLanguage.SelectedIndex = (int)LocalizationManager.Current;
            comboLanguage.SelectedIndexChanged += comboLanguage_SelectedIndexChanged;

            UpdateStatusIndicator();
            UpdateGamingModeButton();
        }

        // ── Theme ─────────────────────────────────────────────────────────
        private void ApplyTheme()
        {
            if (IsDisposed) return;

            BackColor = ThemeManager.Bg;
            foreach (var panel in new[] { panelVibrance, panelDisplay, panelSettings, panelProfiles })
            {
                panel.BackColor = ThemeManager.Surface;
                panel.Invalidate();
            }
            panelStatus.BackColor = Color.FromArgb(18, 21, 28);

            foreach (var l in new[] { labelSectionVibrance, labelSectionDisplay, labelSectionSettings, labelSectionProfiles })
                l.ForeColor = ThemeManager.Accent;
            foreach (var l in new[] { labelBrightness, labelContrast, labelLang, labelHotkeyTitle, labelGpuInfo })
                l.ForeColor = ThemeManager.TextSub;
            labelBrightnessVal.ForeColor = ThemeManager.Accent;
            labelContrastVal.ForeColor   = ThemeManager.Accent;

            foreach (var cb in new[] { chkAutostart, chkPrimaryMonitor, chkNeverResize })
                cb.ForeColor = ThemeManager.Text;

            foreach (var t in new[] { trackBrightness, trackContrast })
                t.BackColor = ThemeManager.Surface;

            comboLanguage.BackColor = ThemeManager.Surface2;
            comboLanguage.ForeColor = ThemeManager.Text;
            listProfiles.BackColor  = ThemeManager.Surface;
            listProfiles.ForeColor  = ThemeManager.Text;

            foreach (var device in _monitorDevices)
            {
                if (_monitorSliders.TryGetValue(device, out var bar)) bar.BackColor = ThemeManager.Surface;
                if (_monitorValueLabels.TryGetValue(device, out var v)) v.ForeColor = ThemeManager.Accent;
            }

            HighlightActivePreset();
            UpdateStatusIndicator();
            UpdateGamingModeButton();
            Invalidate(true);
        }

        // ── Painting ──────────────────────────────────────────────────────
        private void panelHeader_Paint(object sender, PaintEventArgs e)
        {
            var g    = e.Graphics;
            var rect = panelHeader.ClientRectangle;
            g.SmoothingMode     = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            using (var grad = new LinearGradientBrush(rect, ThemeManager.GradStart, ThemeManager.GradEnd, LinearGradientMode.Horizontal))
                g.FillRectangle(grad, rect);
            const int iconSize = 44;
            int iconY = (rect.Height - iconSize) / 2;
            try { using (var bmp = IconFactory.GetAppBitmap(iconSize)) g.DrawImage(bmp, 14, iconY, iconSize, iconSize); }
            catch { }
        }

        private void CardPanel_Paint(object sender, PaintEventArgs e)
        {
            var p = (Panel)sender;
            var g = e.Graphics;
            using (var b = new SolidBrush(ThemeManager.Surface)) g.FillRectangle(b, p.ClientRectangle);
            using (var pen = new Pen(ThemeManager.Accent, 3f)) g.DrawLine(pen, 1, 0, 1, p.Height);
            using (var pen = new Pen(ThemeManager.Border, 1)) g.DrawRectangle(pen, 0, 0, p.Width - 1, p.Height - 1);
        }

        // ── Form events ───────────────────────────────────────────────────
        private void MainForm_Load(object sender, EventArgs e)
        {
            SetControlsEnabled(false);
            comboLanguage.Items.Clear();
            comboLanguage.Items.AddRange(LocalizationManager.LanguageNames);
            comboLanguage.SelectedIndex = (int)LocalizationManager.Current;
            comboLanguage.SelectedIndexChanged += comboLanguage_SelectedIndexChanged;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (_proxy?.GetVibranceInfo().isInitialized == true) SetControlsEnabled(true);
            if (_startedMinimized && _showNotifications)
                try { notifyIcon.ShowBalloonTip(3000, AppName, LocalizationManager.Get("RunningInBackground"), ToolTipIcon.Info); }
                catch { }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Minimize to tray instead of exiting when the user closes the window.
            if (_minimizeToTray && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                _allowVisible = false;
                Hide();
                if (_showNotifications)
                    try { notifyIcon.ShowBalloonTip(2000, AppName, LocalizationManager.Get("RunningInBackground"), ToolTipIcon.Info); }
                    catch { }
                return;
            }
            _hotkey?.Dispose();
            _schedule?.Dispose();
            CleanUp();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized) Hide();
        }

        // ── Workers ───────────────────────────────────────────────────────
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int level = _defaultWindowsLevel;
            bool pOnly = false, nResize = false;

            while (!IsHandleCreated) Thread.Sleep(200);
            Invoke((MethodInvoker)(() => ReadSettings(out level, out pOnly, out nResize)));

            if (_proxy.GetVibranceInfo().isInitialized)
            {
                backgroundWorker.ReportProgress(1);
                SetControlsEnabled(true);
                _proxy.SetApplicationSettings(_profiles);
                _proxy.SetShouldRun(true);
                _proxy.SetAffectPrimaryMonitorOnly(pOnly);
                _proxy.SetNeverSwitchResolution(nResize);
                // Apply each monitor's restored level
                Invoke((MethodInvoker)(() =>
                {
                    foreach (var device in _monitorDevices)
                        ApplyMonitorLevel(device, _monitorLevels[device]);
                }));
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage != 1) return;
            UpdateStatusIndicator();
            var info = _proxy?.GetVibranceInfo();
            labelGpuInfo.Text = info.HasValue ? (info.Value.szGpuName ?? "") : "";
        }

        private void settingsWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(2000);
            ForceSaveSettings();
        }

        private void settingsWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) { }

        // ── Presets ───────────────────────────────────────────────────────
        private void btnPreset_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            int level;
            switch (btn.Tag?.ToString())
            {
                case "def":  level = _presetDef;  break;
                case "low":  level = _presetLow;  break;
                case "high": level = _presetHigh; break;
                case "max":  level = _presetMax;  break;
                default: return;
            }
            ApplyPresetToAll(Math.Max(_minLevel, Math.Min(_maxLevel, level)));
        }

        private void ApplyPresetToAll(int level)
        {
            foreach (var device in _monitorDevices) ApplyMonitorLevel(device, level);
            HighlightActivePreset();
            TriggerSettingsSave();
        }

        private void HighlightActivePreset()
        {
            int cur = PrimaryLevel;
            foreach (var (Btn, Level) in new[] {
                (btnPresetDef, _presetDef), (btnPresetLow, _presetLow),
                (btnPresetHigh, _presetHigh), (btnPresetMax, _presetMax) })
            {
                bool active = Level == cur;
                Btn.BackColor = active ? ThemeManager.Accent : ThemeManager.Surface2;
                Btn.ForeColor = active ? Color.White         : ThemeManager.TextSub;
            }
        }

        // ── Brightness / Contrast ─────────────────────────────────────────
        private void trackBrightness_Scroll(object sender, EventArgs e)
        {
            DisplayGammaController.SetBrightness(trackBrightness.Value);
            labelBrightnessVal.Text = trackBrightness.Value.ToString();
            new SettingsController().SetVibranceSetting("brightness", trackBrightness.Value.ToString());
        }

        private void trackContrast_Scroll(object sender, EventArgs e)
        {
            DisplayGammaController.SetContrast(trackContrast.Value);
            labelContrastVal.Text = trackContrast.Value.ToString();
            new SettingsController().SetVibranceSetting("contrast", trackContrast.Value.ToString());
        }

        private void btnResetDisplay_Click(object sender, EventArgs e)
        {
            DisplayGammaController.Reset();
            trackBrightness.Value = DisplayGammaController.Neutral;
            trackContrast.Value   = DisplayGammaController.Neutral;
            labelBrightnessVal.Text = DisplayGammaController.Neutral.ToString();
            labelContrastVal.Text   = DisplayGammaController.Neutral.ToString();
            var sc = new SettingsController();
            sc.SetVibranceSetting("brightness", DisplayGammaController.Neutral.ToString());
            sc.SetVibranceSetting("contrast",   DisplayGammaController.Neutral.ToString());
        }

        // ── Gaming mode (temporarily disable all game profiles) ───────────
        private void btnGamingMode_Click(object sender, EventArgs e)
        {
            _gamingMode = !_gamingMode;
            // When gaming mode is ON, pass an empty list so all game profiles are suspended.
            // When OFF, restore the full profile list.
            _proxy?.SetApplicationSettings(_gamingMode ? new List<ApplicationSetting>() : _profiles);
            UpdateGamingModeButton();
        }

        private void UpdateGamingModeButton()
        {
            if (btnGamingMode == null) return;
            btnGamingMode.BackColor = _gamingMode ? ThemeManager.Warning : ThemeManager.Accent;
            btnGamingMode.Text      = "🎮";
        }

        // ── Schedule ──────────────────────────────────────────────────────
        private void OnScheduleApply(int level)
        {
            if (IsDisposed) return;
            if (InvokeRequired) { try { Invoke((Action)(() => ApplyPresetToAll(level))); } catch { } return; }
            ApplyPresetToAll(level);
        }

        // ── Checkboxes ────────────────────────────────────────────────────
        private void chkAutostart_CheckedChanged(object sender, EventArgs e)
        {
            var reg  = new RegistryController();
            string p = string.Format("\"{0}\" -minimized", Application.ExecutablePath);
            if (chkAutostart.Checked)
            {
                if (!reg.IsProgramRegistered(AppName) || !reg.IsStartupPathUnchanged(AppName, p))
                    reg.RegisterProgram(AppName, p);
            }
            else reg.UnregisterProgram(AppName);
        }

        private void chkPrimaryMonitor_CheckedChanged(object sender, EventArgs e)
        {
            _proxy?.SetAffectPrimaryMonitorOnly(chkPrimaryMonitor.Checked);
            TriggerSettingsSave();
        }

        private void chkNeverResize_CheckedChanged(object sender, EventArgs e)
        {
            _proxy?.SetNeverSwitchResolution(chkNeverResize.Checked);
            TriggerSettingsSave();
        }

        private void comboLanguage_SelectedIndexChanged(object sender, EventArgs e)
            => LocalizationManager.SetLanguage((Language)comboLanguage.SelectedIndex);

        // ── Settings dialog ───────────────────────────────────────────────
        private void btnOpenSettings_Click(object sender, EventArgs e)
        {
            using (var dlg = new SettingsForm(this, _proxy, _minLevel, _maxLevel,
                _defaultWindowsLevel, PrimaryLevel, _resolveLevelLabel))
                dlg.ShowDialog();
            // Re-read behavior/schedule settings the dialog may have changed.
            LoadBehaviorSettings();
        }

        // ── Hotkey ────────────────────────────────────────────────────────
        private void btnHotkey_Click(object sender, EventArgs e)
        {
            if (_capturingHotkey) return;
            _capturingHotkey    = true;
            btnHotkey.Text      = "...";
            btnHotkey.BackColor = ThemeManager.Accent;
            btnHotkey.ForeColor = Color.White;
            btnHotkey.KeyDown  += BtnHotkey_KeyDown;
            btnHotkey.Focus();
        }

        private void BtnHotkey_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled          = true;
            e.SuppressKeyPress = true;
            btnHotkey.KeyDown -= BtnHotkey_KeyDown;
            _capturingHotkey   = false;

            Keys baseKey = e.KeyCode & ~Keys.Modifiers;
            if (baseKey != Keys.Escape && baseKey != Keys.None)
            {
                _hotkey?.Register(baseKey, e.Modifiers);
                TriggerSettingsSave();
            }
            UpdateHotkeyLabel();
            btnHotkey.BackColor = ThemeManager.Surface2;
            btnHotkey.ForeColor = ThemeManager.Accent;
        }

        public void ApplyHotkeyKey(Keys key, Keys modifiers = Keys.None)
        {
            _hotkey?.Register(key, modifiers);
            UpdateHotkeyLabel();
        }

        private void UpdateHotkeyLabel()
        {
            if (btnHotkey != null && _hotkey != null)
                btnHotkey.Text = _hotkey.GetDisplayString();
        }

        private void OnHotkeyToggle(object sender, EventArgs e)
        {
            _vibranceEnabled = !_vibranceEnabled;
            foreach (var device in _monitorDevices)
            {
                int level = _vibranceEnabled ? _monitorLevels[device] : _defaultWindowsLevel;
                if (_monitorDevices.Count <= 1) _proxy?.SetVibranceWindowsLevel(level);
                else _proxy?.SetVibranceForMonitor(device, level);
            }
            UpdateStatusIndicator();
        }

        private void btnToggleVibrance_Click(object sender, EventArgs e) => OnHotkeyToggle(sender, e);

        // ── Tray ─────────────────────────────────────────────────────────
        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) ShowMainWindow();
        }

        private void menuOpenSpectra_Click(object sender, EventArgs e) => ShowMainWindow();
        private void menuTrayToggle_Click(object sender, EventArgs e) => OnHotkeyToggle(sender, e);
        private void exitMenuItem_Click(object sender, EventArgs e) { _minimizeToTray = false; Close(); }
        private void menuGitHub_Click(object sender, EventArgs e)
            => System.Diagnostics.Process.Start("https://github.com/X1NPAR1/Spectra");

        private void menuTrayPreset_Click(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem)sender;
            int level;
            switch (item.Tag?.ToString())
            {
                case "def":  level = _presetDef;  break;
                case "low":  level = _presetLow;  break;
                case "high": level = _presetHigh; break;
                case "max":  level = _presetMax;  break;
                default: return;
            }
            level = Math.Max(_minLevel, Math.Min(_maxLevel, level));
            if (InvokeRequired) Invoke((MethodInvoker)(() => ApplyPresetToAll(level)));
            else ApplyPresetToAll(level);
        }

        private void ShowMainWindow()
        {
            _allowVisible = true;
            Show();
            WindowState   = FormWindowState.Normal;
            ShowInTaskbar = true;
            Activate();
        }

        // ── Profile management ────────────────────────────────────────────
        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog { Filter = "Executable Files (*.exe)|*.exe" })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;
                if (!File.Exists(dlg.FileName)) return;
                if (_profiles.Any(p => p.FileName.Equals(dlg.FileName, StringComparison.OrdinalIgnoreCase))) return;

                // ExtractAssociatedIcon can return null for certain executable types.
                var icon = Icon.ExtractAssociatedIcon(dlg.FileName);
                if (icon == null) return;

                AddProfileIntern(new ProcessExplorerEntry(dlg.FileName, icon,
                    Path.GetFileNameWithoutExtension(dlg.FileName)));
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e) => new ProcessExplorer(this).Show();

        private void btnDetectGame_Click(object sender, EventArgs e)
        {
            string exe = GameDetector.GetForegroundFullScreenExecutable();
            if (string.IsNullOrEmpty(exe) || !File.Exists(exe))
            {
                MessageBox.Show(LocalizationManager.Get("NoGameDetected"), AppName,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (_profiles.Any(p => p.FileName.Equals(exe, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show(LocalizationManager.Get("ProfileExists"), AppName,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var icon = Icon.ExtractAssociatedIcon(exe);
            if (icon == null) return;
            AddProfileIntern(new ProcessExplorerEntry(exe, icon, Path.GetFileNameWithoutExtension(exe)));
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            // Snapshot SelectedItems before modifying — iterating a live selection while
            // removing items causes InvalidOperationException or missed entries.
            var toRemove = new ListViewItem[listProfiles.SelectedItems.Count];
            listProfiles.SelectedItems.CopyTo(toRemove, 0);

            // Sort descending by index so that removing item N doesn't shift the
            // ImageIndex of later items before we process them.
            Array.Sort(toRemove, (a, b) => b.Index.CompareTo(a.Index));

            foreach (var item in toRemove)
            {
                var s = _profiles.FirstOrDefault(
                    p => p.FileName.Equals(item.Tag?.ToString(), StringComparison.OrdinalIgnoreCase));
                if (s != null) _profiles.Remove(s);
                RemoveProfileItem(item);
            }
            ForceSaveSettings();
        }

        private void listProfiles_DoubleClick(object sender, EventArgs e)
        {
            if (listProfiles.SelectedItems.Count == 0) return;
            var sel      = listProfiles.SelectedItems[0];
            var existing = _profiles.FirstOrDefault(p =>
                p.FileName.Equals(sel.Tag?.ToString(), StringComparison.OrdinalIgnoreCase));

            using (var dlg = new GameSettingsForm(_proxy, _minLevel, _maxLevel, _defaultIngameLevel,
                sel, existing, _primaryResolutions, _resolveLevelLabel))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    var ns  = dlg.GetApplicationSetting();
                    var old = _profiles.FirstOrDefault(p => p.FileName == ns.FileName);
                    if (old != null) _profiles.Remove(old);
                    _profiles.Add(ns);
                    if (!_gamingMode) _proxy?.SetApplicationSettings(_profiles);
                    ForceSaveSettings();
                }
                else if (existing == null) RemoveProfileItem(sel);
            }
        }

        public void AddProgramExtern(ProcessExplorerEntry entry)
        {
            if (InvokeRequired) Invoke((MethodInvoker)(() => AddProfileIntern(entry)));
            else AddProfileIntern(entry);
        }

        private void AddProfileIntern(ProcessExplorerEntry entry)
        {
            EnsureProfileList();
            if (!File.Exists(entry.Path)) return;
            if (_profiles.Any(p => p.FileName.Equals(entry.Path, StringComparison.OrdinalIgnoreCase))) return;

            listProfiles.LargeImageList.Images.Add(entry.Icon);
            var item = new ListViewItem(Path.GetFileNameWithoutExtension(entry.Path))
            {
                ImageIndex = listProfiles.Items.Count,
                Tag        = entry.Path
            };
            listProfiles.Items.Add(item);
            listProfiles.SelectedIndices.Clear();
            item.Selected = true;
            listProfiles_DoubleClick(this, EventArgs.Empty);
        }

        private void RemoveProfileItem(ListViewItem item)
        {
            var img = listProfiles.LargeImageList?.Images[item.ImageIndex];
            listProfiles.LargeImageList?.Images.RemoveAt(item.ImageIndex);
            img?.Dispose();
            listProfiles.Items.Remove(item);
        }

        private void EnsureProfileList()
        {
            if (listProfiles.LargeImageList != null) return;
            listProfiles.LargeImageList = new ImageList
            {
                ImageSize  = new Size(48, 48),
                ColorDepth = ColorDepth.Depth32Bit
            };
            ListViewItem_SetSpacing(listProfiles, 48 + 24, 48 + 6 + 16);
        }

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private void ListViewItem_SetSpacing(ListView lv, short lp, short tp)
        {
            const int LVM_SETICONSPACING = 0x1000 + 53;
            SendMessage(lv.Handle, LVM_SETICONSPACING, IntPtr.Zero,
                (IntPtr)(int)(((ushort)lp) | (uint)(tp << 16)));
        }

        // ── Settings persistence ──────────────────────────────────────────
        private void ReadSettings(out int level, out bool primaryOnly, out bool neverResize)
        {
            _registry = new RegistryController();
            chkAutostart.Checked = _registry.IsProgramRegistered(AppName);

            bool po = false, nr = false;
            Keys hotkey = Keys.F9, hotkeyMods = Keys.None;
            new SettingsController().ReadVibranceSettings(
                _proxy.GraphicsAdapter, out level, out po, out nr, out _profiles,
                out hotkey, out hotkeyMods);
            primaryOnly = po;
            neverResize = nr;

            _hotkey?.Register(hotkey, hotkeyMods);
            UpdateHotkeyLabel();

            int safe = Math.Max(_minLevel, Math.Min(_maxLevel, level));
            foreach (var device in _monitorDevices)
            {
                _monitorLevels[device] = safe;
                if (_monitorSliders.TryGetValue(device, out var bar)) bar.Value = safe;
                if (_monitorValueLabels.TryGetValue(device, out var lbl)) lbl.Text = _resolveLevelLabel(safe);
            }
            chkPrimaryMonitor.Checked = po;
            chkNeverResize.Checked    = nr;
            HighlightActivePreset();

            foreach (var s in _profiles.ToList())
            {
                if (!File.Exists(s.FileName)) { _profiles.Remove(s); continue; }
                EnsureProfileList();
                var icon = Icon.ExtractAssociatedIcon(s.FileName);
                // ExtractAssociatedIcon returns null for some file types. Remove the
                // profile from both the list and memory so the UI stays consistent.
                if (icon == null) { _profiles.Remove(s); continue; }
                listProfiles.LargeImageList.Images.Add(icon);
                listProfiles.Items.Add(new ListViewItem(s.Name)
                {
                    ImageIndex = listProfiles.Items.Count,
                    Tag        = s.FileName
                });
            }

            LoadBehaviorSettings();
        }

        // Reads the v2.x settings (brightness/contrast, behavior, schedule).
        private void LoadBehaviorSettings()
        {
            var sc = new SettingsController();

            int brightness = ParseInt(sc.GetSetting("brightness", "50"), 50);
            int contrast   = ParseInt(sc.GetSetting("contrast",   "50"), 50);
            trackBrightness.Value = Math.Max(0, Math.Min(100, brightness));
            trackContrast.Value   = Math.Max(0, Math.Min(100, contrast));
            labelBrightnessVal.Text = trackBrightness.Value.ToString();
            labelContrastVal.Text   = trackContrast.Value.ToString();
            DisplayGammaController.Set(trackBrightness.Value, trackContrast.Value);

            _minimizeToTray    = sc.GetSetting("minimizeToTray", "false") == "true";
            _showNotifications = sc.GetSetting("showNotifications", "true") == "true";

            if (_schedule != null)
            {
                _schedule.Enabled    = sc.GetSetting("scheduleEnabled", "false") == "true";
                _schedule.DayLevel   = ParseInt(sc.GetSetting("scheduleDayLevel", _presetDef.ToString()), _presetDef);
                _schedule.NightLevel = ParseInt(sc.GetSetting("scheduleNightLevel", _presetLow.ToString()), _presetLow);
                _schedule.DayStart   = ParseTime(sc.GetSetting("scheduleDayStart", "08:00"), new TimeSpan(8, 0, 0));
                _schedule.NightStart = ParseTime(sc.GetSetting("scheduleNightStart", "20:00"), new TimeSpan(20, 0, 0));
                if (_schedule.Enabled) _schedule.EvaluateNow();
            }
        }

        private static int ParseInt(string s, int def)
            => int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out int v) ? v : def;

        private static TimeSpan ParseTime(string s, TimeSpan def)
            => TimeSpan.TryParse(s, CultureInfo.InvariantCulture, out var t) ? t : def;

        private void ForceSaveSettings()
        {
            if (IsDisposed) return; // Form disposed during shutdown — skip
            int level = _defaultWindowsLevel;
            bool po   = false, nr = false;
            Keys hk = Keys.F9, hkMods = Keys.None;
            if (IsHandleCreated)
            {
                MethodInvoker read = () =>
                {
                    if (IsDisposed) return;
                    level  = PrimaryLevel;
                    po     = chkPrimaryMonitor.Checked;
                    nr     = chkNeverResize.Checked;
                    hk     = _hotkey?.CurrentKey ?? Keys.F9;
                    hkMods = _hotkey?.CurrentModifiers ?? Keys.None;
                };
                try { if (InvokeRequired) Invoke(read); else read(); }
                catch (ObjectDisposedException) { return; }
            }
            new SettingsController().SetVibranceSettings(level.ToString(), po.ToString(), nr.ToString(), _profiles, hk, hkMods);
        }

        private void TriggerSettingsSave()
        {
            if (!settingsWorker.IsBusy) settingsWorker.RunWorkerAsync();
        }

        private void SetControlsEnabled(bool flag)
        {
            if (InvokeRequired) { Invoke((MethodInvoker)(() => SetControlsEnabled(flag))); return; }
            foreach (var bar in _monitorSliders.Values) bar.Enabled = flag;
            trackBrightness.Enabled   = flag;
            trackContrast.Enabled     = flag;
            chkAutostart.Enabled      = flag;
            chkPrimaryMonitor.Enabled = flag;
            btnAdd.Enabled            = flag;
            btnBrowse.Enabled         = flag;
            btnDetectGame.Enabled     = flag;
            btnRemove.Enabled         = flag;
        }

        private void UpdateStatusIndicator()
        {
            if (!IsHandleCreated || IsDisposed) return;
            if (InvokeRequired) { Invoke((Action)UpdateStatusIndicator); return; }

            bool running = _proxy?.GetVibranceInfo().isInitialized == true && _vibranceEnabled;
            labelStatus.Text      = LocalizationManager.Get(running ? "StatusRunning" : "StatusStopped");
            labelStatus.ForeColor = running ? ThemeManager.Success : ThemeManager.TextSub;
            btnToggleVibrance.Text      = _vibranceEnabled ? "ON" : "OFF";
            btnToggleVibrance.BackColor = _vibranceEnabled ? ThemeManager.Success : ThemeManager.Danger;
        }

        private void CleanUp()
        {
            try
            {
                if (_proxy?.GetVibranceInfo().isInitialized == true)
                {
                    DisplayGammaController.Reset();

                    bool resetOnExit = new SettingsController().GetSetting("resetOnExit", "false")
                        .Equals("true", StringComparison.OrdinalIgnoreCase);

                    if (resetOnExit)
                    {
                        // Restore the GPU-neutral level (0 for NVIDIA, 100 for AMD)
                        // so no extra vibrance is left after the app closes.
                        _proxy.SetVibranceWindowsLevel(_defaultWindowsLevel);
                    }
                    else
                    {
                        // Restore the user's chosen desktop vibrance level.
                        _proxy.HandleDvcExit();
                    }

                    _proxy.SetShouldRun(false);
                    _proxy.UnloadLibraryEx();
                }
            }
            catch (Exception ex) { Log(ex); }
        }

        public static void Log(Exception ex)
        {
            try
            {
                string dir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Spectra");
                Directory.CreateDirectory(dir);
                File.AppendAllText(Path.Combine(dir, "spectra.log"),
                    string.Format("[{0:yyyy-MM-dd HH:mm:ss}] {1}: {2}\r\n{3}\r\n",
                        DateTime.Now, ex.GetType().Name, ex.Message, ex.StackTrace));
            }
            catch { }
        }
    }
}
