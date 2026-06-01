using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        // ── Proxy / state ─────────────────────────────────────────────────
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

        private bool _allowVisible    = true;
        private bool _vibranceEnabled = true;
        private int  _savedLevel;
        private HotkeyManager _hotkey;
        private bool _capturingHotkey;

        private const string AppName = "Spectra";

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

            ThemeManager.ThemeChanged          += (s, e) => { ApplyTheme(); ApplyLocalization(); };
            LocalizationManager.LanguageChanged += (s, e) => ApplyLocalization();

            ApplyTheme();
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

        // ── Handle & hotkey ───────────────────────────────────────────────
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

        // ── Theme ─────────────────────────────────────────────────────────
        private void ApplyTheme()
        {
            if (InvokeRequired) { Invoke((Action)ApplyTheme); return; }

            bool dark = ThemeManager.IsDark;

            BackColor = ThemeManager.Bg;
            ForeColor = ThemeManager.Text;

            // Header: always gradient painted, text always white
            panelHeader.BackColor = Color.Transparent;

            // Section panels
            foreach (Panel p in new[] { panelVibrance, panelSettings, panelProfiles })
            {
                p.BackColor = ThemeManager.Surface;
                p.ForeColor = ThemeManager.Text;
            }

            // Quick row transparent
            panelQuickRow.BackColor = Color.Transparent;

            // Status bar
            panelStatus.BackColor = dark
                ? Color.FromArgb(8, 12, 18)
                : Color.FromArgb(215, 220, 228);
            panelStatus.ForeColor = ThemeManager.TextSub;

            // Accent labels
            labelSectionVibrance.ForeColor  = ThemeManager.Accent;
            labelSectionSettings.ForeColor  = ThemeManager.Accent;
            labelSectionProfiles.ForeColor  = ThemeManager.Accent;
            labelVibranceValue.ForeColor    = ThemeManager.Accent;

            // Checkboxes
            foreach (CheckBox cb in new[] { chkAutostart, chkPrimaryMonitor, chkNeverResize })
            {
                cb.ForeColor = ThemeManager.Text;
                cb.BackColor = Color.Transparent;
            }

            // Theme toggle buttons
            btnDark.BackColor  = dark ? ThemeManager.Accent  : ThemeManager.Surface2;
            btnDark.ForeColor  = dark ? Color.White            : ThemeManager.TextSub;
            btnLight.BackColor = dark ? ThemeManager.Surface2  : ThemeManager.Accent;
            btnLight.ForeColor = dark ? ThemeManager.TextSub   : Color.White;
            btnDark.FlatAppearance.BorderColor  = ThemeManager.Accent;
            btnLight.FlatAppearance.BorderColor = ThemeManager.Accent;

            // Label colors in quick row
            foreach (Label lbl in new[] { labelTheme, labelLang, labelHotkeyTitle })
            {
                lbl.ForeColor = ThemeManager.TextSub;
                lbl.BackColor = Color.Transparent;
            }

            // Hotkey button
            btnHotkey.BackColor = ThemeManager.Surface2;
            btnHotkey.ForeColor = ThemeManager.Text;
            btnHotkey.FlatAppearance.BorderColor = ThemeManager.Border;

            // Action buttons
            foreach (Button b in new[] { btnAdd, btnBrowse, btnRemove })
            {
                b.BackColor = ThemeManager.Surface2;
                b.ForeColor = ThemeManager.Text;
                b.FlatAppearance.BorderColor = ThemeManager.Border;
            }

            // ListView
            listProfiles.BackColor = ThemeManager.Surface;
            listProfiles.ForeColor = ThemeManager.Text;

            // TrackBar
            trackBarVibrance.BackColor = ThemeManager.Surface;

            // ComboBox
            comboLanguage.BackColor = ThemeManager.Surface2;
            comboLanguage.ForeColor = ThemeManager.Text;

            // Status label color (updated separately by UpdateStatusIndicator)
            UpdateStatusIndicator();
        }

        // ── Localization ──────────────────────────────────────────────────
        private void ApplyLocalization()
        {
            if (InvokeRequired) { Invoke((Action)ApplyLocalization); return; }

            labelSectionVibrance.Text  = LocalizationManager.Get("DesktopVibrance");
            labelSectionSettings.Text  = LocalizationManager.Get("Settings");
            labelSectionProfiles.Text  = LocalizationManager.Get("GameProfiles");
            chkAutostart.Text          = LocalizationManager.Get("Autostart");
            chkPrimaryMonitor.Text     = LocalizationManager.Get("PrimaryMonitor");
            chkNeverResize.Text        = LocalizationManager.Get("NeverResize");
            labelTheme.Text            = LocalizationManager.Get("Theme");
            btnDark.Text               = LocalizationManager.Get("Dark");
            btnLight.Text              = LocalizationManager.Get("Light");
            labelLang.Text             = LocalizationManager.Get("Language");
            labelHotkeyTitle.Text      = LocalizationManager.Get("Hotkey");
            btnAdd.Text                = LocalizationManager.Get("AddFile");
            btnBrowse.Text             = LocalizationManager.Get("BrowseRunning");
            btnRemove.Text             = LocalizationManager.Get("Remove");

            // Tray menu — always localized
            menuOpenSpectra.Text = LocalizationManager.Get("OpenSpectra");
            menuExit.Text        = LocalizationManager.Get("Exit");
            notifyIcon.Text      = AppName;
        }

        // ── Header gradient paint ─────────────────────────────────────────
        private void panelHeader_Paint(object sender, PaintEventArgs e)
        {
            var g    = e.Graphics;
            var rect = panelHeader.ClientRectangle;
            using (var grad = new LinearGradientBrush(rect,
                Color.FromArgb(100, 30, 220),
                Color.FromArgb(0, 175, 225),
                LinearGradientMode.Horizontal))
            {
                g.FillRectangle(grad, rect);
            }
        }

        // ── Section border paint ──────────────────────────────────────────
        private void SectionPanel_Paint(object sender, PaintEventArgs e)
        {
            using (var pen = new Pen(ThemeManager.Border, 1))
                e.Graphics.DrawRectangle(pen, 0, 0,
                    ((Panel)sender).Width - 1, ((Panel)sender).Height - 1);
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
            if (_proxy?.GetVibranceInfo().isInitialized == true)
                SetControlsEnabled(true);
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

        // ── Background workers ────────────────────────────────────────────
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int level = _defaultWindowsLevel;
            bool primaryOnly = false, neverResize = false;

            while (!IsHandleCreated) Thread.Sleep(200);

            if (InvokeRequired)
                Invoke((MethodInvoker)(() => ReadSettings(out level, out primaryOnly, out neverResize)));
            else
                ReadSettings(out level, out primaryOnly, out neverResize);

            if (_proxy.GetVibranceInfo().isInitialized)
            {
                backgroundWorker.ReportProgress(1);
                SetControlsEnabled(true);
                _proxy.SetApplicationSettings(_profiles);
                _proxy.SetShouldRun(true);
                _proxy.SetVibranceWindowsLevel(level);
                _proxy.SetAffectPrimaryMonitorOnly(primaryOnly);
                _proxy.SetNeverSwitchResolution(neverResize);
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage != 1) return;
            UpdateStatusIndicator();
            var info = _proxy?.GetVibranceInfo();
            if (info.HasValue)
                labelGpuInfo.Text = info.Value.szGpuName ?? "";
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
            TriggerSettingsSave();
        }

        // ── Checkboxes ────────────────────────────────────────────────────
        private void chkAutostart_CheckedChanged(object sender, EventArgs e)
        {
            var reg     = new RegistryController();
            string path = string.Format("\"{0}\" -minimized", Application.ExecutablePath);
            if (chkAutostart.Checked)
            {
                if (!reg.IsProgramRegistered(AppName))
                    reg.RegisterProgram(AppName, path);
                else if (!reg.IsStartupPathUnchanged(AppName, path))
                    reg.RegisterProgram(AppName, path);
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

        // ── Theme / Language ──────────────────────────────────────────────
        private void btnDark_Click(object sender, EventArgs e)  => ThemeManager.SetTheme(AppTheme.Dark);
        private void btnLight_Click(object sender, EventArgs e) => ThemeManager.SetTheme(AppTheme.Light);

        private void comboLanguage_SelectedIndexChanged(object sender, EventArgs e)
            => LocalizationManager.SetLanguage((Language)comboLanguage.SelectedIndex);

        // ── Settings dialog ───────────────────────────────────────────────
        private void btnOpenSettings_Click(object sender, EventArgs e)
        {
            using (var dlg = new SettingsForm(this, _proxy, _minLevel, _maxLevel,
                _defaultWindowsLevel, trackBarVibrance.Value, _resolveLevelLabel))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    // Apply any changes returned from settings dialog
                    int newLevel = dlg.DesktopVibranceLevel;
                    trackBarVibrance.Value = Math.Max(_minLevel, Math.Min(_maxLevel, newLevel));
                    labelVibranceValue.Text = _resolveLevelLabel(trackBarVibrance.Value);
                    _proxy?.SetVibranceWindowsLevel(trackBarVibrance.Value);
                    _savedLevel = trackBarVibrance.Value;
                    ForceSaveSettings();
                }
            }
        }

        // ── Hotkey ────────────────────────────────────────────────────────
        private void btnHotkey_Click(object sender, EventArgs e)
        {
            if (_capturingHotkey) return;
            _capturingHotkey = true;
            btnHotkey.Text      = "...";
            btnHotkey.BackColor = Color.FromArgb(123, 47, 247);
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

            if (e.KeyCode != Keys.Escape && e.KeyCode != Keys.None)
                _hotkey?.Register(e.KeyCode);

            UpdateHotkeyLabel();
            ApplyTheme();
        }

        private void UpdateHotkeyLabel()
        {
            if (btnHotkey != null && _hotkey != null)
                btnHotkey.Text = _hotkey.CurrentKey.ToString();
        }

        public void ApplyHotkeyKey(Keys key)
        {
            _hotkey?.Register(key);
            UpdateHotkeyLabel();
        }

        private void OnHotkeyToggle(object sender, EventArgs e)
        {
            _vibranceEnabled = !_vibranceEnabled;
            _proxy?.SetVibranceWindowsLevel(_vibranceEnabled ? _savedLevel : _defaultWindowsLevel);
            UpdateStatusIndicator();
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
                var entry = new ProcessExplorerEntry(dlg.FileName,
                    Icon.ExtractAssociatedIcon(dlg.FileName),
                    Path.GetFileNameWithoutExtension(dlg.FileName));
                AddProfileIntern(entry);
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            new ProcessExplorer(this).Show();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listProfiles.SelectedItems)
            {
                for (int i = item.Index + 1; i < listProfiles.Items.Count; i++)
                    listProfiles.Items[i].ImageIndex--;

                var s = _profiles.FirstOrDefault(
                    p => p.FileName.Equals(item.Tag?.ToString(), StringComparison.OrdinalIgnoreCase));
                if (s != null) _profiles.Remove(s);

                var img = listProfiles.LargeImageList?.Images[item.ImageIndex];
                listProfiles.LargeImageList?.Images.RemoveAt(item.ImageIndex);
                img?.Dispose();
                listProfiles.Items.Remove(item);
            }
            ForceSaveSettings();
        }

        private void listProfiles_DoubleClick(object sender, EventArgs e)
        {
            if (listProfiles.SelectedItems.Count == 0) return;
            var selected = listProfiles.SelectedItems[0];
            var existing = _profiles.FirstOrDefault(p => p.FileName == selected.Tag?.ToString());

            using (var dlg = new GameSettingsForm(_proxy, _minLevel, _maxLevel, _defaultIngameLevel,
                selected, existing, _primaryResolutions, _resolveLevelLabel))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    var ns = dlg.GetApplicationSetting();
                    var old = _profiles.FirstOrDefault(p => p.FileName == ns.FileName);
                    if (old != null) _profiles.Remove(old);
                    _profiles.Add(ns);
                    ForceSaveSettings();
                }
                else if (existing == null)
                {
                    RemoveProfileItem(selected);
                }
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
            var il = new ImageList { ImageSize = new Size(48, 48), ColorDepth = ColorDepth.Depth32Bit };
            listProfiles.LargeImageList = il;
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

        // ── Tray ─────────────────────────────────────────────────────────
        private void notifyIcon_DoubleClick(object sender, MouseEventArgs e)
        {
            ShowMainWindow();
        }

        private void menuOpenSpectra_Click(object sender, EventArgs e)
        {
            ShowMainWindow();
        }

        private void ShowMainWindow()
        {
            _allowVisible = true;
            Show();
            WindowState   = FormWindowState.Normal;
            ShowInTaskbar = true;
            Activate();
        }

        private void exitMenuItem_Click(object sender, EventArgs e) => Close();

        // ── Settings persistence ──────────────────────────────────────────
        private void ReadSettings(out int level, out bool primaryOnly, out bool neverResize)
        {
            _registry = new RegistryController();
            Invoke((MethodInvoker)(() => chkAutostart.Checked = _registry.IsProgramRegistered(AppName)));

            bool pOnly = false, nResize = false;
            new SettingsController().ReadVibranceSettings(
                _proxy.GraphicsAdapter, out level, out pOnly, out nResize, out _profiles);
            primaryOnly = pOnly;
            neverResize = nResize;

            if (!IsHandleCreated) return;

            int safeLevel = Math.Max(_minLevel, Math.Min(_maxLevel, level));
            bool capPOnly = pOnly, capNResize = nResize;
            Invoke((MethodInvoker)(() =>
            {
                labelVibranceValue.Text   = _resolveLevelLabel(safeLevel);
                trackBarVibrance.Value    = safeLevel;
                chkPrimaryMonitor.Checked = capPOnly;
                chkNeverResize.Checked    = capNResize;
                _savedLevel               = safeLevel;

                foreach (var setting in _profiles.ToList())
                {
                    if (!File.Exists(setting.FileName)) { _profiles.Remove(setting); continue; }
                    EnsureProfileList();
                    var icon = Icon.ExtractAssociatedIcon(setting.FileName);
                    if (icon == null) continue;
                    listProfiles.LargeImageList.Images.Add(icon);
                    var lvi = new ListViewItem(setting.Name)
                    {
                        ImageIndex = listProfiles.Items.Count,
                        Tag        = setting.FileName
                    };
                    listProfiles.Items.Add(lvi);
                }
            }));
        }

        private void ForceSaveSettings()
        {
            int level = _defaultWindowsLevel;
            bool primaryOnly = false, neverResize = false;
            if (InvokeRequired)
                Invoke((MethodInvoker)(() =>
                {
                    level       = trackBarVibrance.Value;
                    primaryOnly = chkPrimaryMonitor.Checked;
                    neverResize = chkNeverResize.Checked;
                }));
            else
            {
                level       = trackBarVibrance.Value;
                primaryOnly = chkPrimaryMonitor.Checked;
                neverResize = chkNeverResize.Checked;
            }
            new SettingsController().SetVibranceSettings(
                level.ToString(), primaryOnly.ToString(), neverResize.ToString(), _profiles);
        }

        private void TriggerSettingsSave()
        {
            if (!settingsWorker.IsBusy) settingsWorker.RunWorkerAsync();
        }

        // ── UI helpers ────────────────────────────────────────────────────
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
            if (InvokeRequired) { Invoke((Action)UpdateStatusIndicator); return; }
            bool running = _proxy?.GetVibranceInfo().isInitialized == true && _vibranceEnabled;
            labelStatus.Text      = LocalizationManager.Get(running ? "StatusRunning" : "StatusStopped");
            labelStatus.ForeColor = running ? ThemeManager.Success : ThemeManager.TextSub;
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
                    string.Format("\r\n[{0:yyyy-MM-dd HH:mm:ss}] {1}: {2}\r\n{3}\r\n",
                        DateTime.Now, ex.GetType().Name, ex.Message, ex.StackTrace));
            }
            catch { }
        }
    }
}
