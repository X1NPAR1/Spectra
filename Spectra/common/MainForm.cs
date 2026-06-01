using System;
using System.Collections.Generic;
using System.ComponentModel;
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

            Icon            = IconFactory.GetAppIcon(32);
            notifyIcon.Icon = IconFactory.GetAppIcon(16);

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

            ThemeManager.ThemeChanged          += (s, e) => SafeInvoke(ApplyAll);
            LocalizationManager.LanguageChanged += (s, e) => SafeInvoke(ApplyLocalization);

            ApplyAll();
            backgroundWorker.RunWorkerAsync();
        }

        // ── Core helpers ──────────────────────────────────────────────────
        private void SafeInvoke(Action a)
        {
            if (IsHandleCreated && InvokeRequired) Invoke(a);
            else a();
        }

        private void ApplyAll()
        {
            ApplyTheme();
            ApplyLocalization();
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

        // ── Theme — every control explicit ───────────────────────────────
        private void ApplyTheme()
        {
            if (!IsHandleCreated || IsDisposed) return;
            bool dark = ThemeManager.IsDark;

            BackColor = ThemeManager.Bg;

            // Header (always gradient painted, text always white)
            panelHeader.BackColor  = Color.Transparent;
            labelAppName.ForeColor = Color.White;
            labelAppName.BackColor = Color.Transparent;
            labelVersion.ForeColor = Color.FromArgb(190, 225, 255);
            labelVersion.BackColor = Color.Transparent;
            labelGpuBadge.ForeColor= Color.FromArgb(210, 240, 255);
            labelGpuBadge.BackColor= Color.Transparent;

            // Section panels
            panelVibrance.BackColor  = ThemeManager.Surface;
            panelSettings.BackColor  = ThemeManager.Surface;
            panelProfiles.BackColor  = ThemeManager.Surface;

            // Status bar
            panelStatus.BackColor    = dark ? Color.FromArgb(8, 12, 18) : Color.FromArgb(210, 215, 222);

            // Accent section labels
            labelSectionVibrance.ForeColor  = ThemeManager.Accent;
            labelSectionVibrance.BackColor  = Color.Transparent;
            labelSectionSettings.ForeColor  = ThemeManager.Accent;
            labelSectionSettings.BackColor  = Color.Transparent;
            labelSectionProfiles.ForeColor  = ThemeManager.Accent;
            labelSectionProfiles.BackColor  = Color.Transparent;

            // Vibrance value
            labelVibranceValue.ForeColor = ThemeManager.Accent;
            labelVibranceValue.BackColor = Color.Transparent;
            trackBarVibrance.BackColor   = ThemeManager.Surface;

            // Checkboxes
            chkAutostart.ForeColor      = ThemeManager.Text;
            chkAutostart.BackColor      = Color.Transparent;
            chkPrimaryMonitor.ForeColor = ThemeManager.Text;
            chkPrimaryMonitor.BackColor = Color.Transparent;
            chkNeverResize.ForeColor    = ThemeManager.Text;
            chkNeverResize.BackColor    = Color.Transparent;

            // Quick rows
            panelQuickRow.BackColor = Color.Transparent;
            panelRow1.BackColor     = Color.Transparent;
            panelRow2.BackColor     = Color.Transparent;
            panelRowSep1.BackColor  = ThemeManager.Border;

            // Theme toggle buttons
            btnDark.BackColor  = dark ? ThemeManager.Accent  : ThemeManager.Surface2;
            btnDark.ForeColor  = dark ? Color.White            : ThemeManager.TextSub;
            btnLight.BackColor = dark ? ThemeManager.Surface2  : ThemeManager.Accent;
            btnLight.ForeColor = dark ? ThemeManager.TextSub   : Color.White;
            btnDark.FlatAppearance.BorderColor  = ThemeManager.Accent;
            btnLight.FlatAppearance.BorderColor = ThemeManager.Accent;

            // Labels in quick rows
            labelTheme.ForeColor       = ThemeManager.TextSub;
            labelTheme.BackColor       = Color.Transparent;
            labelLang.ForeColor        = ThemeManager.TextSub;
            labelLang.BackColor        = Color.Transparent;
            labelHotkeyTitle.ForeColor = ThemeManager.TextSub;
            labelHotkeyTitle.BackColor = Color.Transparent;

            // Language combo
            comboLanguage.BackColor = ThemeManager.Surface2;
            comboLanguage.ForeColor = ThemeManager.Text;

            // Hotkey button (accent border)
            btnHotkey.BackColor = ThemeManager.Surface2;
            btnHotkey.ForeColor = ThemeManager.Accent;
            btnHotkey.FlatAppearance.BorderColor = ThemeManager.Accent;

            // Profile list
            listProfiles.BackColor = ThemeManager.Surface;
            listProfiles.ForeColor = ThemeManager.Text;

            // Action buttons
            Color btnBg  = ThemeManager.Surface2;
            Color btnFg  = ThemeManager.Text;
            Color btnBdr = ThemeManager.Border;
            foreach (Button b in new[] { btnAdd, btnBrowse, btnRemove })
            {
                b.BackColor = btnBg;
                b.ForeColor = btnFg;
                b.FlatAppearance.BorderColor = btnBdr;
            }

            // Status label
            UpdateStatusIndicator();
            labelGpuInfo.ForeColor = ThemeManager.TextSub;

            // Tray context menu (ToolStrip uses system renderer — set colors)
            contextMenu.BackColor = ThemeManager.Surface;
            contextMenu.ForeColor = ThemeManager.Text;
            foreach (ToolStripItem item in contextMenu.Items)
            {
                item.BackColor = ThemeManager.Surface;
                item.ForeColor = ThemeManager.Text;
            }

            Refresh();
        }

        // ── Localization — every string explicit ──────────────────────────
        private void ApplyLocalization()
        {
            if (!IsHandleCreated || IsDisposed) return;

            labelSectionVibrance.Text  = LocalizationManager.Get("DesktopVibrance");
            labelSectionSettings.Text  = LocalizationManager.Get("Settings");
            labelSectionProfiles.Text  = LocalizationManager.Get("GameProfiles");
            chkAutostart.Text          = LocalizationManager.Get("Autostart");
            chkPrimaryMonitor.Text     = LocalizationManager.Get("PrimaryMonitor");
            chkNeverResize.Text        = LocalizationManager.Get("NeverResize");
            labelTheme.Text            = LocalizationManager.Get("Theme");
            btnDark.Text               = LocalizationManager.Get("Dark");
            btnLight.Text              = LocalizationManager.Get("Light");
            labelLang.Text             = LocalizationManager.Get("LangShort");
            labelHotkeyTitle.Text      = LocalizationManager.Get("HotkeyShort");
            btnAdd.Text                = LocalizationManager.Get("AddFile");
            btnBrowse.Text             = LocalizationManager.Get("BrowseRunning");
            btnRemove.Text             = LocalizationManager.Get("Remove");
            menuOpenSpectra.Text       = LocalizationManager.Get("OpenSpectra");
            menuTrayToggle.Text        = LocalizationManager.Get("TrayToggle");
            menuExit.Text              = LocalizationManager.Get("Exit");
            notifyIcon.Text            = AppName;
        }

        // ── Header gradient ───────────────────────────────────────────────
        private void panelHeader_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            var r = panelHeader.ClientRectangle;
            using (var g = new LinearGradientBrush(r,
                Color.FromArgb(100, 30, 220), Color.FromArgb(0, 175, 225),
                LinearGradientMode.Horizontal))
                e.Graphics.FillRectangle(g, r);
        }

        // ── Section border ────────────────────────────────────────────────
        private void SectionPanel_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            var p = (Panel)sender;
            using (var pen = new Pen(ThemeManager.Border, 1))
                e.Graphics.DrawRectangle(pen, 0, 0, p.Width - 1, p.Height - 1);
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

        // ── Vibrance ──────────────────────────────────────────────────────
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
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    int nv = dlg.DesktopVibranceLevel;
                    trackBarVibrance.Value  = Math.Max(_minLevel, Math.Min(_maxLevel, nv));
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
            _capturingHotkey      = true;
            btnHotkey.Text        = "...";
            btnHotkey.BackColor   = Color.FromArgb(123, 47, 247);
            btnHotkey.ForeColor   = Color.White;
            btnHotkey.KeyDown    += BtnHotkey_KeyDown;
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

        public void ApplyHotkeyKey(Keys key)
        {
            _hotkey?.Register(key);
            UpdateHotkeyLabel();
        }

        private void UpdateHotkeyLabel()
        {
            if (btnHotkey != null && _hotkey != null)
                btnHotkey.Text = _hotkey.CurrentKey.ToString();
        }

        private void OnHotkeyToggle(object sender, EventArgs e)
        {
            _vibranceEnabled = !_vibranceEnabled;
            _proxy?.SetVibranceWindowsLevel(_vibranceEnabled ? _savedLevel : _defaultWindowsLevel);
            UpdateStatusIndicator();
        }

        // ── Tray ─────────────────────────────────────────────────────────
        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                ShowMainWindow();
        }

        private void menuOpenSpectra_Click(object sender, EventArgs e) => ShowMainWindow();

        private void menuTrayToggle_Click(object sender, EventArgs e) => OnHotkeyToggle(sender, e);

        private void exitMenuItem_Click(object sender, EventArgs e) => Close();

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
                if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                if (!File.Exists(dlg.FileName)) return;
                if (_profiles.Any(p => p.FileName.Equals(dlg.FileName, StringComparison.OrdinalIgnoreCase))) return;
                var entry = new ProcessExplorerEntry(dlg.FileName,
                    Icon.ExtractAssociatedIcon(dlg.FileName),
                    Path.GetFileNameWithoutExtension(dlg.FileName));
                AddProfileIntern(entry);
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
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
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
            new SettingsController().ReadVibranceSettings(
                _proxy.GraphicsAdapter, out level, out po, out nr, out _profiles);
            primaryOnly = po;
            neverResize = nr;

            int safe = Math.Max(_minLevel, Math.Min(_maxLevel, level));
            labelVibranceValue.Text   = _resolveLevelLabel(safe);
            trackBarVibrance.Value    = safe;
            chkPrimaryMonitor.Checked = po;
            chkNeverResize.Checked    = nr;
            _savedLevel               = safe;

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
            if (IsHandleCreated)
            {
                if (InvokeRequired)
                    Invoke((MethodInvoker)(() => { level = trackBarVibrance.Value; po = chkPrimaryMonitor.Checked; nr = chkNeverResize.Checked; }));
                else { level = trackBarVibrance.Value; po = chkPrimaryMonitor.Checked; nr = chkNeverResize.Checked; }
            }
            new SettingsController().SetVibranceSettings(level.ToString(), po.ToString(), nr.ToString(), _profiles);
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
            if (!IsHandleCreated) return;
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
                string dir = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Spectra");
                System.IO.Directory.CreateDirectory(dir);
                System.IO.File.AppendAllText(System.IO.Path.Combine(dir, "spectra.log"),
                    string.Format("[{0:yyyy-MM-dd HH:mm:ss}] {1}: {2}\r\n{3}\r\n",
                        DateTime.Now, ex.GetType().Name, ex.Message, ex.StackTrace));
            }
            catch { }
        }
    }
}
