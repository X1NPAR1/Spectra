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

            LocalizationManager.LanguageChanged += (s, e) =>
            {
                if (!IsDisposed && IsHandleCreated) Invoke((Action)ApplyLocalization);
            };

            ApplyLocalization();
            backgroundWorker.RunWorkerAsync();
        }

        protected override void SetVisibleCore(bool value)
        {
            if (!_allowVisible) { value = false; if (!IsHandleCreated) CreateHandle(); }
            base.SetVisibleCore(value);
        }

        public void SetAllowVisible(bool v) => _allowVisible = v;

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
        public void ReloadProfiles() => backgroundWorker.RunWorkerAsync();

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
            menuOpenSpectra.Text       = LocalizationManager.Get("OpenSpectra");
            menuTrayToggle.Text        = LocalizationManager.Get("TrayToggle");
            menuExit.Text              = LocalizationManager.Get("Exit");
            notifyIcon.Text            = AppName;

            // Sync combobox selection without firing the event again
            comboLanguage.SelectedIndexChanged -= comboLanguage_SelectedIndexChanged;
            if (comboLanguage.Items.Count > 0)
                comboLanguage.SelectedIndex = (int)LocalizationManager.Current;
            comboLanguage.SelectedIndexChanged += comboLanguage_SelectedIndexChanged;

            // Re-apply status label in current language
            UpdateStatusIndicator();
        }

        // ── Header painting (gradient + inline S logo) ───────────────────
        private void panelHeader_Paint(object sender, PaintEventArgs e)
        {
            var g    = e.Graphics;
            var rect = panelHeader.ClientRectangle;
            g.SmoothingMode      = SmoothingMode.AntiAlias;
            g.TextRenderingHint  = TextRenderingHint.AntiAlias;

            // Gradient fill
            using (var grad = new LinearGradientBrush(rect,
                ThemeManager.GradStart, ThemeManager.GradEnd,
                LinearGradientMode.Horizontal))
                g.FillRectangle(grad, rect);

            // Small "S" logo (40×40 rounded rect) at left
            DrawLogoMini(g, 14, (rect.Height - 40) / 2, 40);
        }

        private static void DrawLogoMini(Graphics g, int x, int y, int size)
        {
            var rect = new Rectangle(x, y, size, size);
            int r    = size / 6, d = r * 2;

            using (var path = new GraphicsPath())
            {
                path.AddArc(rect.X, rect.Y, d, d, 180, 90);
                path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
                path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
                path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
                path.CloseFigure();

                // Semi-transparent white fill so it's distinct from the gradient
                using (var fill = new SolidBrush(Color.FromArgb(50, 255, 255, 255)))
                    g.FillPath(fill, path);
                using (var border = new Pen(Color.FromArgb(120, 255, 255, 255), 1.5f))
                    g.DrawPath(border, path);
            }

            // "S" text inside
            using (var font = new Font("Segoe UI", size * 0.52f, FontStyle.Bold, GraphicsUnit.Pixel))
            using (var brush = new SolidBrush(Color.White))
            {
                var sf = new StringFormat
                {
                    Alignment     = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString("S", font, brush, new RectangleF(x, y, size, size), sf);
            }
        }

        // ── Card panel painting (white card with accent left border) ──────
        private void CardPanel_Paint(object sender, PaintEventArgs e)
        {
            var p = (Panel)sender;
            var g = e.Graphics;

            // White card fill
            g.FillRectangle(Brushes.White, p.ClientRectangle);

            // 3px accent left border (purple)
            using (var pen = new Pen(ThemeManager.Accent, 3f))
                g.DrawLine(pen, 1, 0, 1, p.Height);

            // Subtle outer border
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

        // ── Language ──────────────────────────────────────────────────────
        private void comboLanguage_SelectedIndexChanged(object sender, EventArgs e)
            => LocalizationManager.SetLanguage((Language)comboLanguage.SelectedIndex);

        // ── Settings dialog ───────────────────────────────────────────────
        private void btnOpenSettings_Click(object sender, EventArgs e)
        {
            using (var dlg = new SettingsForm(this, _proxy, _minLevel, _maxLevel,
                _defaultWindowsLevel, trackBarVibrance.Value, _resolveLevelLabel))
            {
                dlg.ShowDialog();
                // Re-sync checkboxes that may have changed in settings
                ApplyLocalization();
            }
        }

        // ── Hotkey ────────────────────────────────────────────────────────
        private void btnHotkey_Click(object sender, EventArgs e)
        {
            if (_capturingHotkey) return;
            _capturingHotkey      = true;
            btnHotkey.Text        = "...";
            btnHotkey.BackColor   = ThemeManager.Accent;
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
            // Restore colors
            btnHotkey.BackColor = ThemeManager.Surface2;
            btnHotkey.ForeColor = ThemeManager.Accent;
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
            if (e.Button == MouseButtons.Left) ShowMainWindow();
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
                if (dlg.ShowDialog() != DialogResult.OK) return;
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
