using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
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
        // ── Vibrance proxy ────────────────────────────────────────────────
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
        private int  _savedLevel;
        private HotkeyManager _hotkey;
        private bool _capturingHotkey;
        private const string AppName = "Spectra";

        // Preset level fractions indexed to [def, low, high, max]
        // Calculated at initialization based on min/max range
        private int _presetDef, _presetLow, _presetHigh, _presetMax;

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

            // Preset levels: def=default, low=25%, high=75%, max=100%
            int range   = maxLevel - minLevel;
            _presetDef  = defaultWindowsLevel;
            _presetLow  = minLevel + (int)(range * 0.25);
            _presetHigh = minLevel + (int)(range * 0.75);
            _presetMax  = maxLevel;

            InitializeComponent();

            Icon              = IconFactory.GetAppIcon(32);
            notifyIcon.Icon   = IconFactory.GetAppIcon(16);

            trackBarVibrance.Minimum = minLevel;
            trackBarVibrance.Maximum = maxLevel;

            foreach (Screen screen in Screen.AllScreens)
            {
                if (ResolutionHelper.GetCurrentResolutionSettings(out Devmode dm, screen.DeviceName))
                {
                    var avail = ResolutionHelper.EnumerateSupportedResolutionModes(screen.DeviceName);
                    if (screen.Primary) _primaryResolutions.AddRange(avail);
                    _resolutionMap[screen.DeviceName] = Tuple.Create(new ResolutionModeWrapper(dm), avail);
                }
            }

            _proxy = getProxy(_profiles, _resolutionMap);

            backgroundWorker.WorkerReportsProgress = true;
            settingsWorker.WorkerReportsProgress   = true;

            LocalizationManager.LanguageChanged += (s, e) =>
            {
                if (!IsDisposed && IsHandleCreated) Invoke((Action)ApplyLocalization);
            };

            ApplyLocalization();
            backgroundWorker.RunWorkerAsync();
        }

        // ── Visibility ────────────────────────────────────────────────────
        protected override void SetVisibleCore(bool value)
        {
            if (!_allowVisible) { value = false; if (!IsHandleCreated) CreateHandle(); }
            base.SetVisibleCore(value);
        }

        public void SetAllowVisible(bool v) => _allowVisible = v;

        // ── Handle / hotkey ───────────────────────────────────────────────
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
            labelSectionSettings.Text  = LocalizationManager.Get("Settings");
            labelSectionProfiles.Text  = LocalizationManager.Get("GameProfiles");
            chkAutostart.Text          = LocalizationManager.Get("Autostart");
            chkPrimaryMonitor.Text     = LocalizationManager.Get("PrimaryMonitor");
            chkNeverResize.Text        = LocalizationManager.Get("NeverResize");
            labelLang.Text             = LocalizationManager.Get("LangLabel");
            labelHotkeyTitle.Text      = LocalizationManager.Get("HotkeyLabel");
            btnAdd.Text                = LocalizationManager.Get("AddFile");
            btnBrowse.Text             = LocalizationManager.Get("BrowseRunning");
            btnRemove.Text             = LocalizationManager.Get("Remove");

            // Vibrance preset buttons — localized in every language
            btnPresetDef.Text  = LocalizationManager.Get("PresetDefault");
            btnPresetLow.Text  = LocalizationManager.Get("PresetLow");
            btnPresetHigh.Text = LocalizationManager.Get("PresetHigh");
            btnPresetMax.Text  = LocalizationManager.Get("PresetMax");

            // Tray menu — all items localized
            menuOpenSpectra.Text    = LocalizationManager.Get("OpenSpectra");
            menuTrayToggle.Text     = LocalizationManager.Get("TrayToggle");
            menuTrayPresets.Text    = LocalizationManager.Get("QuickPresets");
            menuTrayPresetDef.Text  = LocalizationManager.Get("PresetDefault");
            menuTrayPresetLow.Text  = LocalizationManager.Get("PresetLow");
            menuTrayPresetHigh.Text = LocalizationManager.Get("PresetHigh");
            menuTrayPresetMax.Text  = LocalizationManager.Get("PresetMax");
            menuExit.Text           = LocalizationManager.Get("Exit");
            notifyIcon.Text         = AppName;

            // Sync combo without firing change event
            comboLanguage.SelectedIndexChanged -= comboLanguage_SelectedIndexChanged;
            if (comboLanguage.Items.Count > 0)
                comboLanguage.SelectedIndex = (int)LocalizationManager.Current;
            comboLanguage.SelectedIndexChanged += comboLanguage_SelectedIndexChanged;

            // Re-apply status (language-aware)
            UpdateStatusIndicator();
        }

        // ── Header painting — real icon + gradient ────────────────────────
        private void panelHeader_Paint(object sender, PaintEventArgs e)
        {
            var g    = e.Graphics;
            var rect = panelHeader.ClientRectangle;
            g.SmoothingMode     = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // Navy gradient background
            using (var grad = new LinearGradientBrush(rect,
                ThemeManager.GradStart, ThemeManager.GradEnd,
                LinearGradientMode.Horizontal))
                g.FillRectangle(grad, rect);

            // Draw the actual application icon (setting.ico embedded in exe)
            const int iconSize = 44;
            int iconY = (rect.Height - iconSize) / 2;
            try
            {
                using (var bmp = IconFactory.GetAppBitmap(iconSize))
                    g.DrawImage(bmp, 14, iconY, iconSize, iconSize);
            }
            catch { /* icon not yet available */ }
        }

        // ── Card panel (white card + 3px navy left border) ────────────────
        private void CardPanel_Paint(object sender, PaintEventArgs e)
        {
            var p = (Panel)sender;
            var g = e.Graphics;
            g.FillRectangle(Brushes.White, p.ClientRectangle);
            using (var pen = new Pen(ThemeManager.Accent, 3f))
                g.DrawLine(pen, 1, 0, 1, p.Height);
            using (var pen = new Pen(ThemeManager.Border, 1))
                g.DrawRectangle(pen, 0, 0, p.Width - 1, p.Height - 1);
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
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _hotkey?.Dispose();
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
                _proxy.SetVibranceWindowsLevel(level);
                _proxy.SetAffectPrimaryMonitorOnly(pOnly);
                _proxy.SetNeverSwitchResolution(nResize);
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
            Thread.Sleep(3000);
            ForceSaveSettings();
        }

        private void settingsWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) { }

        // ── Vibrance slider ───────────────────────────────────────────────
        private void trackBarVibrance_Scroll(object sender, EventArgs e)
        {
            _proxy?.SetVibranceWindowsLevel(trackBarVibrance.Value);
            labelVibranceValue.Text = _resolveLevelLabel(trackBarVibrance.Value);
            _savedLevel = trackBarVibrance.Value;
            HighlightActivePreset();
            TriggerSettingsSave();
        }

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
                default:     return;
            }
            level = Math.Max(_minLevel, Math.Min(_maxLevel, level));
            trackBarVibrance.Value  = level;
            labelVibranceValue.Text = _resolveLevelLabel(level);
            _proxy?.SetVibranceWindowsLevel(level);
            _savedLevel = level;
            HighlightActivePreset();
            TriggerSettingsSave();
        }

        private void HighlightActivePreset()
        {
            var presets = new (Button Btn, int Level)[]
            {
                (btnPresetDef,  _presetDef),
                (btnPresetLow,  _presetLow),
                (btnPresetHigh, _presetHigh),
                (btnPresetMax,  _presetMax),
            };
            int cur = trackBarVibrance.Value;
            foreach (var p in presets)
            {
                bool active = p.Level == cur;
                p.Btn.BackColor = active ? ThemeManager.Accent  : ThemeManager.Surface2;
                p.Btn.ForeColor = active ? Color.White           : ThemeManager.TextSub;
            }
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

        // ── Language ──────────────────────────────────────────────────────
        private void comboLanguage_SelectedIndexChanged(object sender, EventArgs e)
            => LocalizationManager.SetLanguage((Language)comboLanguage.SelectedIndex);

        // ── Settings dialog ───────────────────────────────────────────────
        private void btnOpenSettings_Click(object sender, EventArgs e)
        {
            using (var dlg = new SettingsForm(this, _proxy, _minLevel, _maxLevel,
                _defaultWindowsLevel, trackBarVibrance.Value, _resolveLevelLabel))
                dlg.ShowDialog();
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
            _proxy?.SetVibranceWindowsLevel(_vibranceEnabled ? _savedLevel : _defaultWindowsLevel);
            UpdateStatusIndicator();
        }

        // ── Toggle button (status bar) ────────────────────────────────────
        private void btnToggleVibrance_Click(object sender, EventArgs e) => OnHotkeyToggle(sender, e);

        // ── Tray ─────────────────────────────────────────────────────────
        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) ShowMainWindow();
        }

        private void menuOpenSpectra_Click(object sender, EventArgs e) => ShowMainWindow();
        private void menuTrayToggle_Click(object sender, EventArgs e) => OnHotkeyToggle(sender, e);
        private void exitMenuItem_Click(object sender, EventArgs e) => Close();
        private void menuGitHub_Click(object sender, EventArgs e)
            => System.Diagnostics.Process.Start("https://github.com/X1NPAR1/Spectra");

        private void menuTrayPreset_Click(object sender, EventArgs e)
        {
            var item = (System.Windows.Forms.ToolStripMenuItem)sender;
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
            if (InvokeRequired) Invoke((MethodInvoker)(() => ApplyPresetLevel(level)));
            else ApplyPresetLevel(level);
        }

        private void ApplyPresetLevel(int level)
        {
            trackBarVibrance.Value  = level;
            labelVibranceValue.Text = _resolveLevelLabel(level);
            _proxy?.SetVibranceWindowsLevel(level);
            _savedLevel = level;
            HighlightActivePreset();
            TriggerSettingsSave();
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
            EnsureProfileList();
            using (var dlg = new OpenFileDialog { Filter = "Executable Files (*.exe)|*.exe" })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;
                if (!File.Exists(dlg.FileName)) return;
                if (_profiles.Any(p => p.FileName.Equals(dlg.FileName, StringComparison.OrdinalIgnoreCase))) return;
                AddProfileIntern(new ProcessExplorerEntry(dlg.FileName,
                    Icon.ExtractAssociatedIcon(dlg.FileName),
                    Path.GetFileNameWithoutExtension(dlg.FileName)));
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e) => new ProcessExplorer(this).Show();

        private void btnRemove_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listProfiles.SelectedItems)
            {
                for (int i = item.Index + 1; i < listProfiles.Items.Count; i++)
                    listProfiles.Items[i].ImageIndex--;
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
            var existing = _profiles.FirstOrDefault(p => p.FileName == sel.Tag?.ToString());

            using (var dlg = new GameSettingsForm(_proxy, _minLevel, _maxLevel, _defaultIngameLevel,
                sel, existing, _primaryResolutions, _resolveLevelLabel))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    var ns  = dlg.GetApplicationSetting();
                    var old = _profiles.FirstOrDefault(p => p.FileName == ns.FileName);
                    if (old != null) _profiles.Remove(old);
                    _profiles.Add(ns);
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

            // Restore saved hotkey
            _hotkey?.Register(hotkey, hotkeyMods);
            UpdateHotkeyLabel();

            int safe = Math.Max(_minLevel, Math.Min(_maxLevel, level));
            labelVibranceValue.Text   = _resolveLevelLabel(safe);
            trackBarVibrance.Value    = safe;
            chkPrimaryMonitor.Checked = po;
            chkNeverResize.Checked    = nr;
            _savedLevel               = safe;
            HighlightActivePreset();

            foreach (var s in _profiles.ToList())
            {
                if (!File.Exists(s.FileName)) { _profiles.Remove(s); continue; }
                EnsureProfileList();
                var icon = Icon.ExtractAssociatedIcon(s.FileName);
                if (icon == null) continue;
                listProfiles.LargeImageList.Images.Add(icon);
                listProfiles.Items.Add(new ListViewItem(s.Name)
                {
                    ImageIndex = listProfiles.Items.Count,
                    Tag        = s.FileName
                });
            }
        }

        private void ForceSaveSettings()
        {
            int level = _defaultWindowsLevel;
            bool po   = false, nr = false;
            Keys hk = Keys.F9, hkMods = Keys.None;
            if (IsHandleCreated)
            {
                if (InvokeRequired)
                    Invoke((MethodInvoker)(() =>
                    {
                        level  = trackBarVibrance.Value;
                        po     = chkPrimaryMonitor.Checked;
                        nr     = chkNeverResize.Checked;
                        hk     = _hotkey?.CurrentKey ?? Keys.F9;
                        hkMods = _hotkey?.CurrentModifiers ?? Keys.None;
                    }));
                else
                {
                    level  = trackBarVibrance.Value;
                    po     = chkPrimaryMonitor.Checked;
                    nr     = chkNeverResize.Checked;
                    hk     = _hotkey?.CurrentKey ?? Keys.F9;
                    hkMods = _hotkey?.CurrentModifiers ?? Keys.None;
                }
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
            trackBarVibrance.Enabled  = flag;
            chkAutostart.Enabled      = flag;
            chkPrimaryMonitor.Enabled = flag;
            btnAdd.Enabled            = flag;
            btnBrowse.Enabled         = flag;
            btnRemove.Enabled         = flag;
        }

        private void UpdateStatusIndicator()
        {
            if (!IsHandleCreated || IsDisposed) return;
            if (InvokeRequired) { Invoke((Action)UpdateStatusIndicator); return; }

            bool running = _proxy?.GetVibranceInfo().isInitialized == true && _vibranceEnabled;

            labelStatus.Text      = LocalizationManager.Get(running ? "StatusRunning" : "StatusStopped");
            labelStatus.ForeColor = running ? ThemeManager.Success : ThemeManager.TextSub;

            // Toggle button reflects state
            btnToggleVibrance.Text      = _vibranceEnabled ? "ON" : "OFF";
            btnToggleVibrance.BackColor = _vibranceEnabled ? ThemeManager.Success : ThemeManager.Danger;
        }

        private void CleanUp()
        {
            try
            {
                if (_proxy?.GetVibranceInfo().isInitialized == true)
                {
                    _proxy.HandleDvcExit();
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
