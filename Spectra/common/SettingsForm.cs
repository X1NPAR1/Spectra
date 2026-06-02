using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Spectra.Localization;
using Spectra.UI;

namespace Spectra.common
{
    public partial class SettingsForm : Form
    {
        private readonly MainForm       _mainForm;
        private readonly IVibranceProxy _proxy;
        private readonly int _minLevel;
        private readonly int _maxLevel;

        private CheckBox      _chkSchedule;
        private NumericUpDown _numDayLevel, _numNightLevel;
        private DateTimePicker _dtDayStart, _dtNightStart;
        private Label _lblScheduleSection, _lblDayLevel, _lblNightLevel, _lblDayStart, _lblNightStart;
        private Panel _sepSchedule;

        private TrackBar      _trackBlueLight;
        private Label         _labelBlueLightVal;
        private Button[]      _btnColorBlind;

        private CheckBox      _chkObsIntegration;
        private NumericUpDown _numObsLevel;
        private Label         _lblObsSection, _lblObsLevelLbl;
        private Panel         _sepObs;

        private CheckBox      _chkTransition;
        private NumericUpDown _numTransitionDuration;
        private Label         _lblTransitionSection, _lblTransitionDurationLbl;
        private Panel         _sepTransition;

        private Label         _lblBlueLightSection, _lblBlueLightLbl;
        private Label         _lblColorBlindSection;

        private bool _loading;

        public SettingsForm(MainForm mainForm, IVibranceProxy proxy,
            int minLevel, int maxLevel, int defaultLevel, int currentLevel,
            Func<int, string> resolveLabel)
        {
            _mainForm = mainForm;
            _proxy    = proxy;
            _minLevel = minLevel;
            _maxLevel = maxLevel;

            InitializeComponent();

            Icon = IconFactory.GetAppIcon(16);

            cboMonitorTarget.Items.Add(LocalizationManager.Get("MonitorAll"));
            cboMonitorTarget.Items.Add(LocalizationManager.Get("MonitorPrimary"));
            foreach (Screen s in Screen.AllScreens)
                if (!s.Primary) cboMonitorTarget.Items.Add(s.DeviceName);

            string currentTarget = _proxy.GetVibranceInfo().targetMonitorDeviceName;
            cboMonitorTarget.SelectedIndex = 0;
            if (currentTarget == "PRIMARY") cboMonitorTarget.SelectedIndex = 1;
            else if (currentTarget != null)
                for (int i = 2; i < cboMonitorTarget.Items.Count; i++)
                    if (cboMonitorTarget.Items[i].ToString() == currentTarget) { cboMonitorTarget.SelectedIndex = i; break; }
            cboMonitorTarget.SelectedIndexChanged += cboMonitorTarget_SelectedIndexChanged;

            lblSysInfo.Text = string.Format(".NET {0}   |   OS: {1}",
                Environment.Version, Environment.OSVersion.Version);

            BuildScheduleControls();
            BuildObsControls();
            BuildTransitionControls();
            BuildDisplayEnhancementControls();
            LoadSettings();
            UpdateProfileList();

            LocalizationManager.LanguageChanged += OnLanguageChanged;
            ApplyLocalization();
        }

        private void BuildScheduleControls()
        {
            _lblScheduleSection = MakeSection(0, 224);
            _sepSchedule        = MakeSep(244);

            _chkSchedule = new CheckBox { Location = new Point(0, 252), Size = new Size(488, 22),
                Font = new Font("Segoe UI", 9f), ForeColor = ThemeManager.Text, BackColor = Color.Transparent };
            _chkSchedule.CheckedChanged += (s, e) => SaveSchedule();

            _lblDayLevel = MakeLabel(0, 284);
            _numDayLevel = MakeLevelNum(120, 280);
            _lblNightLevel = MakeLabel(250, 284);
            _numNightLevel = MakeLevelNum(370, 280);

            _lblDayStart = MakeLabel(0, 320);
            _dtDayStart  = MakeTimePicker(120, 316);
            _lblNightStart = MakeLabel(250, 320);
            _dtNightStart  = MakeTimePicker(370, 316);

            _numDayLevel.ValueChanged   += (s, e) => SaveSchedule();
            _numNightLevel.ValueChanged += (s, e) => SaveSchedule();
            _dtDayStart.ValueChanged    += (s, e) => SaveSchedule();
            _dtNightStart.ValueChanged  += (s, e) => SaveSchedule();

            tabBehavior.Controls.Add(_lblScheduleSection);
            tabBehavior.Controls.Add(_sepSchedule);
            tabBehavior.Controls.Add(_chkSchedule);
            tabBehavior.Controls.Add(_lblDayLevel);
            tabBehavior.Controls.Add(_numDayLevel);
            tabBehavior.Controls.Add(_lblNightLevel);
            tabBehavior.Controls.Add(_numNightLevel);
            tabBehavior.Controls.Add(_lblDayStart);
            tabBehavior.Controls.Add(_dtDayStart);
            tabBehavior.Controls.Add(_lblNightStart);
            tabBehavior.Controls.Add(_dtNightStart);
        }

        private void BuildDisplayEnhancementControls()
        {
            int yBase = chkNeverResize.Bottom + 20;

            var sep = MakeSep(yBase);

            _lblBlueLightSection = MakeSection(0, yBase + 8);

            _lblBlueLightLbl = MakeLabel(0, yBase + 34);

            _trackBlueLight = new TrackBar
            {
                Location  = new Point(100, yBase + 30),
                Size      = new Size(290, 30),
                Minimum   = 0, Maximum = 100, Value = 0,
                TickStyle = TickStyle.None,
                BackColor = ThemeManager.Surface
            };
            _trackBlueLight.Scroll += (s, e) =>
            {
                if (_loading) return;
                DisplayGammaController.SetBlueLight(_trackBlueLight.Value);
                _labelBlueLightVal.Text = _trackBlueLight.Value + "%";
                new SettingsController().SetVibranceSetting("blueLight", _trackBlueLight.Value.ToString());
            };

            _labelBlueLightVal = new Label
            {
                Location  = new Point(396, yBase + 34),
                Size      = new Size(44, 20),
                Font      = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = ThemeManager.Accent,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft
            };

            int yBtn = yBase + 74;
            _lblColorBlindSection = MakeSection(0, yBtn);

            string[] cbLabels = { "Normal", "Protanopia", "Deuteranopia", "Tritanopia" };
            string[] cbTags   = { "normal", "protan",     "deutan",       "tritan"     };
            _btnColorBlind = new Button[4];
            int bx = 0;
            for (int i = 0; i < 4; i++)
            {
                int idx = i;
                var btn = new Button
                {
                    Text      = cbLabels[i],
                    Font      = new Font("Segoe UI", 7.5f),
                    ForeColor = ThemeManager.TextSub,
                    BackColor = ThemeManager.Surface2,
                    FlatStyle = FlatStyle.Flat,
                    Location  = new Point(bx, yBtn + 22),
                    Size      = new Size(112, 24),
                    Cursor    = Cursors.Hand,
                    Tag       = cbTags[i]
                };
                btn.FlatAppearance.BorderColor = ThemeManager.Border;
                btn.Click += (s, e) => OnColorBlindClick((string)((Button)s).Tag);
                _btnColorBlind[i] = btn;
                bx += 118;
            }

            tabDisplay.Controls.Add(sep);
            tabDisplay.Controls.Add(_lblBlueLightSection);
            tabDisplay.Controls.Add(_lblBlueLightLbl);
            tabDisplay.Controls.Add(_trackBlueLight);
            tabDisplay.Controls.Add(_labelBlueLightVal);
            tabDisplay.Controls.Add(_lblColorBlindSection);
            foreach (var btn in _btnColorBlind) tabDisplay.Controls.Add(btn);
        }

        private void OnColorBlindClick(string tag)
        {
            var mode = ColorBlindMode.Normal;
            switch (tag)
            {
                case "protan": mode = ColorBlindMode.Protanopia;   break;
                case "deutan": mode = ColorBlindMode.Deuteranopia; break;
                case "tritan": mode = ColorBlindMode.Tritanopia;   break;
            }
            DisplayGammaController.SetColorBlindMode(mode);
            new SettingsController().SetVibranceSetting("colorBlind", tag);
            UpdateColorBlindButtons(mode);
        }

        private void UpdateColorBlindButtons(ColorBlindMode mode)
        {
            if (_btnColorBlind == null) return;
            string[] tags = { "normal", "protan", "deutan", "tritan" };
            for (int i = 0; i < _btnColorBlind.Length; i++)
            {
                bool active = tags[i] == ColorBlindModeToTag(mode);
                _btnColorBlind[i].BackColor = active ? ThemeManager.Accent : ThemeManager.Surface2;
                _btnColorBlind[i].ForeColor = active ? Color.White : ThemeManager.TextSub;
                _btnColorBlind[i].FlatAppearance.BorderColor = active ? ThemeManager.Accent : ThemeManager.Border;
            }
        }

        private static string ColorBlindModeToTag(ColorBlindMode mode)
        {
            switch (mode)
            {
                case ColorBlindMode.Protanopia:   return "protan";
                case ColorBlindMode.Deuteranopia: return "deutan";
                case ColorBlindMode.Tritanopia:   return "tritan";
                default:                          return "normal";
            }
        }

        private void BuildObsControls()
        {
            _lblObsSection = MakeSection(0, 368);
            _sepObs = MakeSep(388);

            _chkObsIntegration = new CheckBox
            {
                Location = new Point(0, 396), Size = new Size(488, 22),
                Font = new Font("Segoe UI", 9f), ForeColor = ThemeManager.Text, BackColor = Color.Transparent
            };
            _chkObsIntegration.CheckedChanged += (s, e) =>
            {
                if (_loading) return;
                new SettingsController().SetVibranceSetting("obsEnabled", _chkObsIntegration.Checked ? "true" : "false");
            };

            _lblObsLevelLbl = MakeLabel(0, 428);
            _numObsLevel = new NumericUpDown
            {
                Location = new Point(160, 424), Size = new Size(80, 24), Font = new Font("Segoe UI", 9f),
                Minimum = _minLevel, Maximum = _maxLevel, BackColor = ThemeManager.Surface2, ForeColor = ThemeManager.Text
            };
            _numObsLevel.ValueChanged += (s, e) =>
            {
                if (_loading) return;
                new SettingsController().SetVibranceSetting("obsLevel", ((int)_numObsLevel.Value).ToString());
            };

            tabBehavior.Controls.Add(_lblObsSection);
            tabBehavior.Controls.Add(_sepObs);
            tabBehavior.Controls.Add(_chkObsIntegration);
            tabBehavior.Controls.Add(_lblObsLevelLbl);
            tabBehavior.Controls.Add(_numObsLevel);
        }

        private void BuildTransitionControls()
        {
            _lblTransitionSection = MakeSection(0, 464);
            _sepTransition = MakeSep(484);

            _chkTransition = new CheckBox
            {
                Location = new Point(0, 492), Size = new Size(488, 22),
                Font = new Font("Segoe UI", 9f), ForeColor = ThemeManager.Text, BackColor = Color.Transparent
            };
            _chkTransition.CheckedChanged += (s, e) =>
            {
                if (_loading) return;
                var sc = new SettingsController();
                sc.SetVibranceSetting("transitionEnabled", _chkTransition.Checked ? "true" : "false");
                _proxy?.SetTransitionEnabled(_chkTransition.Checked);
            };

            _lblTransitionDurationLbl = MakeLabel(0, 524);
            _numTransitionDuration = new NumericUpDown
            {
                Location = new Point(120, 520), Size = new Size(80, 24), Font = new Font("Segoe UI", 9f),
                Minimum = 50, Maximum = 2000, Increment = 50,
                BackColor = ThemeManager.Surface2, ForeColor = ThemeManager.Text
            };
            _numTransitionDuration.ValueChanged += (s, e) =>
            {
                if (_loading) return;
                int d = (int)_numTransitionDuration.Value;
                new SettingsController().SetVibranceSetting("transitionDuration", d.ToString());
                _proxy?.SetTransitionDuration(d);
            };

            tabBehavior.Controls.Add(_lblTransitionSection);
            tabBehavior.Controls.Add(_sepTransition);
            tabBehavior.Controls.Add(_chkTransition);
            tabBehavior.Controls.Add(_lblTransitionDurationLbl);
            tabBehavior.Controls.Add(_numTransitionDuration);
        }

        private Label MakeSection(int x, int y) => new Label {
            Location = new Point(x, y), AutoSize = true,
            Font = new Font("Segoe UI", 8f, FontStyle.Bold), ForeColor = ThemeManager.Accent, BackColor = Color.Transparent };
        private Label MakeLabel(int x, int y) => new Label {
            Location = new Point(x, y), Size = new Size(116, 20),
            Font = new Font("Segoe UI", 9f), ForeColor = ThemeManager.Text, BackColor = Color.Transparent };
        private Panel MakeSep(int y) => new Panel { Location = new Point(0, y), Size = new Size(488, 1), BackColor = ThemeManager.Border };
        private NumericUpDown MakeLevelNum(int x, int y) => new NumericUpDown {
            Location = new Point(x, y), Size = new Size(100, 24), Font = new Font("Segoe UI", 9f),
            Minimum = _minLevel, Maximum = _maxLevel, BackColor = ThemeManager.Surface2, ForeColor = ThemeManager.Text };
        private DateTimePicker MakeTimePicker(int x, int y) => new DateTimePicker {
            Location = new Point(x, y), Size = new Size(100, 24), Format = DateTimePickerFormat.Custom,
            CustomFormat = "HH:mm", ShowUpDown = true, Font = new Font("Segoe UI", 9f) };

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            LocalizationManager.LanguageChanged -= OnLanguageChanged;
            base.OnFormClosed(e);
        }

        private void OnLanguageChanged(object s, EventArgs e)
        {
            if (!IsDisposed && IsHandleCreated) Invoke((Action)ApplyLocalization);
        }

        private void LoadSettings()
        {
            _loading = true;
            var sc = new SettingsController();

            chkAutostart.Checked     = new RegistryController().IsProgramRegistered("Spectra");
            chkMinToTray.Checked     = sc.GetSetting("minimizeToTray",    "false") == "true";
            chkNotifications.Checked = sc.GetSetting("showNotifications", "true")  == "true";
            chkResetOnExit.Checked   = sc.GetSetting("resetOnExit",       "false") == "true";
            numDelay.Value           = Clamp(ParseInt(sc.GetSetting("applyDelay", "500"), 500), (int)numDelay.Minimum, (int)numDelay.Maximum);

            chkAutostart.CheckedChanged += (s, e) =>
            {
                if (_loading) return;
                var reg  = new RegistryController();
                string p = $"\"{System.Windows.Forms.Application.ExecutablePath}\" -minimized";
                if (chkAutostart.Checked)
                {
                    if (!reg.IsProgramRegistered("Spectra") || !reg.IsStartupPathUnchanged("Spectra", p))
                        reg.RegisterProgram("Spectra", p);
                }
                else
                {
                    reg.UnregisterProgram("Spectra");
                }
            };

            chkMinToTray.CheckedChanged     += (s, e) => sc.SetVibranceSetting("minimizeToTray",    chkMinToTray.Checked     ? "true" : "false");
            chkNotifications.CheckedChanged += (s, e) => sc.SetVibranceSetting("showNotifications", chkNotifications.Checked ? "true" : "false");
            chkResetOnExit.CheckedChanged   += (s, e) => sc.SetVibranceSetting("resetOnExit",       chkResetOnExit.Checked   ? "true" : "false");
            numDelay.ValueChanged           += (s, e) => sc.SetVibranceSetting("applyDelay", ((int)numDelay.Value).ToString());

            int bl = Clamp(ParseInt(sc.GetSetting("blueLight", "0"), 0), 0, 100);
            _trackBlueLight.Value = bl;
            _labelBlueLightVal.Text = bl + "%";

            string cbTag = sc.GetSetting("colorBlind", "normal");
            var cbMode = ColorBlindMode.Normal;
            switch (cbTag)
            {
                case "protan": cbMode = ColorBlindMode.Protanopia;   break;
                case "deutan": cbMode = ColorBlindMode.Deuteranopia; break;
                case "tritan": cbMode = ColorBlindMode.Tritanopia;   break;
            }
            UpdateColorBlindButtons(cbMode);

            _chkObsIntegration.Checked = sc.GetSetting("obsEnabled", "false") == "true";
            int obsLvl = Clamp(ParseInt(sc.GetSetting("obsLevel", _maxLevel.ToString()), _maxLevel), _minLevel, _maxLevel);
            _numObsLevel.Value = obsLvl;

            _chkTransition.Checked  = sc.GetSetting("transitionEnabled", "true") == "true";
            int transDur = Clamp(ParseInt(sc.GetSetting("transitionDuration", "300"), 300), 50, 2000);
            _numTransitionDuration.Value = transDur;

            _chkSchedule.Checked   = sc.GetSetting("scheduleEnabled", "false") == "true";
            _numDayLevel.Value     = Clamp(ParseInt(sc.GetSetting("scheduleDayLevel", _maxLevel.ToString()), _maxLevel), _minLevel, _maxLevel);
            _numNightLevel.Value   = Clamp(ParseInt(sc.GetSetting("scheduleNightLevel", _minLevel.ToString()), _minLevel), _minLevel, _maxLevel);
            _dtDayStart.Value      = TimeToday(sc.GetSetting("scheduleDayStart", "08:00"), 8);
            _dtNightStart.Value    = TimeToday(sc.GetSetting("scheduleNightStart", "20:00"), 20);

            _loading = false;
        }

        private void SaveSchedule()
        {
            if (_loading) return;
            var sc = new SettingsController();
            sc.SetVibranceSetting("scheduleEnabled",   _chkSchedule.Checked ? "true" : "false");
            sc.SetVibranceSetting("scheduleDayLevel",  ((int)_numDayLevel.Value).ToString());
            sc.SetVibranceSetting("scheduleNightLevel",((int)_numNightLevel.Value).ToString());
            sc.SetVibranceSetting("scheduleDayStart",  _dtDayStart.Value.ToString("HH:mm", CultureInfo.InvariantCulture));
            sc.SetVibranceSetting("scheduleNightStart",_dtNightStart.Value.ToString("HH:mm", CultureInfo.InvariantCulture));
        }

        private static int ParseInt(string s, int def)
            => int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out int v) ? v : def;
        private static int Clamp(int v, int lo, int hi) => v < lo ? lo : (v > hi ? hi : v);
        private static DateTime TimeToday(string hhmm, int defHour)
        {
            if (TimeSpan.TryParse(hhmm, CultureInfo.InvariantCulture, out var ts))
                return DateTime.Today.Add(ts);
            return DateTime.Today.AddHours(defHour);
        }

        private void UpdateProfileList()
        {
            var profiles = _mainForm?.GetProfiles();
            int count = profiles?.Count ?? 0;
            lblProfileCountVal.Text = count.ToString();
            lbProfiles.Items.Clear();
            if (profiles == null || count == 0) { lbProfiles.Items.Add("—"); return; }
            foreach (var p in profiles)
                lbProfiles.Items.Add($"  {p.Name}   —   Level: {p.IngameLevel}");
        }

        private void ApplyLocalization()
        {
            Text = LocalizationManager.Get("SettingsTitle");
            tabBehavior.Text = LocalizationManager.Get("TabBehavior");
            tabDisplay.Text  = LocalizationManager.Get("TabDisplay");
            tabData.Text     = LocalizationManager.Get("TabData");
            tabAbout.Text    = LocalizationManager.Get("TabAbout");
            tabControl.Invalidate();

            lblStartupSection.Text = LocalizationManager.Get("BehaviorSection");
            chkAutostart.Text      = LocalizationManager.Get("Autostart");
            chkMinToTray.Text      = LocalizationManager.Get("MinimizeToTray");
            chkNotifications.Text  = LocalizationManager.Get("ShowNotifications");
            lblDelaySection.Text   = LocalizationManager.Get("ApplyDelay");
            lblDelayNote.Text      = LocalizationManager.Get("ApplyDelayNote");

            _lblScheduleSection.Text = LocalizationManager.Get("ScheduleSection");
            _chkSchedule.Text        = LocalizationManager.Get("ScheduleEnable");
            _lblDayLevel.Text        = LocalizationManager.Get("DayLevel");
            _lblNightLevel.Text      = LocalizationManager.Get("NightLevel");
            _lblDayStart.Text        = LocalizationManager.Get("DayStart");
            _lblNightStart.Text      = LocalizationManager.Get("NightStart");

            lblMonitorSection.Text = LocalizationManager.Get("DisplaySection");
            lblMonitorTarget.Text  = LocalizationManager.Get("MonitorTarget");
            lblResSection.Text     = LocalizationManager.Get("ResolutionSection");
            chkNeverResize.Text    = LocalizationManager.Get("NeverResize");
            chkResetOnExit.Text    = LocalizationManager.Get("ResetOnExit");

            if (cboMonitorTarget.Items.Count >= 2)
            {
                cboMonitorTarget.Items[0] = LocalizationManager.Get("MonitorAll");
                cboMonitorTarget.Items[1] = LocalizationManager.Get("MonitorPrimary");
            }

            lblProfileSection.Text = LocalizationManager.Get("ProfilesSection");
            lblProfileCount.Text   = LocalizationManager.Get("ProfileCount");
            btnExport.Text         = LocalizationManager.Get("ExportProfiles");
            btnImport.Text         = LocalizationManager.Get("ImportProfiles");
            btnClearProfiles.Text  = LocalizationManager.Get("ClearProfiles");
            lblDataSection.Text    = "DATA & DIAGNOSTICS";
            lblDataNote.Text       = LocalizationManager.Get("DataNote");
            btnOpenLog.Text        = LocalizationManager.Get("OpenLogFolder");
            btnResetAll.Text       = LocalizationManager.Get("ResetAll");

            if (_lblObsSection         != null) _lblObsSection.Text         = LocalizationManager.Get("ObsSection");
            if (_chkObsIntegration     != null) _chkObsIntegration.Text     = LocalizationManager.Get("ObsEnable");
            if (_lblObsLevelLbl        != null) _lblObsLevelLbl.Text         = LocalizationManager.Get("ObsVibrance");
            if (_lblTransitionSection  != null) _lblTransitionSection.Text   = LocalizationManager.Get("TransitionSection");
            if (_chkTransition         != null) _chkTransition.Text          = LocalizationManager.Get("TransitionEnable");
            if (_lblTransitionDurationLbl != null) _lblTransitionDurationLbl.Text = LocalizationManager.Get("TransitionDuration");
            if (_lblBlueLightSection   != null) _lblBlueLightSection.Text    = LocalizationManager.Get("BlueLightSection");
            if (_lblBlueLightLbl       != null) _lblBlueLightLbl.Text        = LocalizationManager.Get("BlueLightLabel");
            if (_lblColorBlindSection  != null) _lblColorBlindSection.Text   = LocalizationManager.Get("ColorBlindSection");
            if (_btnColorBlind != null && _btnColorBlind.Length == 4)
            {
                _btnColorBlind[0].Text = LocalizationManager.Get("CbNormal");
                _btnColorBlind[1].Text = LocalizationManager.Get("CbProtanopia");
                _btnColorBlind[2].Text = LocalizationManager.Get("CbDeuteranopia");
                _btnColorBlind[3].Text = LocalizationManager.Get("CbTritanopia");
            }

            lblAboutDesc.Text    = LocalizationManager.Get("AboutDesc");
            lblSupportTitle.Text = LocalizationManager.Get("AboutSupport");
            lblGpuLine1.Text     = LocalizationManager.Get("AboutGpuLine1");
            lblGpuLine2.Text     = LocalizationManager.Get("AboutGpuLine2");
            lblSysTitle.Text     = LocalizationManager.Get("SysInfo");
            btnGitHub.Text       = LocalizationManager.Get("ViewOnGitHub");
            btnOpenLogShort.Text = LocalizationManager.Get("OpenLogFolderShort");
            btnClose.Text        = LocalizationManager.Get("OK");
        }

        private void tabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            var tab = tabControl.TabPages[e.Index];
            bool selected = e.Index == tabControl.SelectedIndex;
            var g = e.Graphics;
            g.SmoothingMode     = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            using (var bg = new SolidBrush(selected ? ThemeManager.Surface : ThemeManager.Surface2))
                g.FillRectangle(bg, e.Bounds);
            if (selected)
                using (var pen = new Pen(ThemeManager.Accent, 2))
                    g.DrawLine(pen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
            using (var brush = new SolidBrush(selected ? ThemeManager.Accent : ThemeManager.TextSub))
            using (var font  = new Font("Segoe UI", 8.5f, selected ? FontStyle.Bold : FontStyle.Regular))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(tab.Text, font, brush, e.Bounds, sf);
            }
        }

        private void panelHeader_Paint(object sender, PaintEventArgs e)
        {
            var r = panelHeader.ClientRectangle;
            using (var grad = new LinearGradientBrush(r, ThemeManager.GradStart, ThemeManager.GradEnd, LinearGradientMode.Horizontal))
                e.Graphics.FillRectangle(grad, r);
        }

        private void panelLogo_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode     = SmoothingMode.AntiAlias;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            try { using (var bmp = IconFactory.GetAppBitmap(60)) g.DrawImage(bmp, 2, 2, 60, 60); }
            catch { }
        }

        private void cboMonitorTarget_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = cboMonitorTarget.SelectedIndex;
            if (idx == 0)      _proxy.SetTargetMonitorDeviceName(null);
            else if (idx == 1) _proxy.SetTargetMonitorDeviceName("PRIMARY");
            else if (idx >= 2 && idx < cboMonitorTarget.Items.Count)
                _proxy.SetTargetMonitorDeviceName(cboMonitorTarget.Items[idx].ToString());
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            using (var dlg = new SaveFileDialog { Filter = "JSON files (*.json)|*.json", FileName = "spectra-profiles.json" })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;
                try
                {
                    File.WriteAllText(dlg.FileName, BuildProfilesJson(_mainForm.GetProfiles()), System.Text.Encoding.UTF8);
                    MessageBox.Show("OK", "Spectra", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog { Filter = "XML/JSON files (*.xml;*.json)|*.xml;*.json" })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;
                try
                {
                    var xml = new System.Xml.Serialization.XmlSerializer(typeof(List<ApplicationSetting>));
                    using (var sr = new StreamReader(dlg.FileName))
                        _mainForm.GetProfiles().AddRange((List<ApplicationSetting>)xml.Deserialize(sr));
                    UpdateProfileList();
                    MessageBox.Show("OK", "Spectra", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        private static string BuildProfilesJson(List<ApplicationSetting> profiles)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("[");
            for (int i = 0; i < profiles.Count; i++)
            {
                var p = profiles[i];
                sb.Append("  { ");
                sb.Append($"\"name\": {J(p.Name)}, \"fileName\": {J(p.FileName)}, \"ingameLevel\": {p.IngameLevel}, \"changeResolution\": {(p.IsResolutionChangeNeeded ? "true" : "false")}");
                sb.Append(" }");
                if (i < profiles.Count - 1) sb.Append(",");
                sb.AppendLine();
            }
            sb.Append("]");
            return sb.ToString();
        }
        private static string J(string s) => s == null ? "null" : "\"" + s.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";

        private void btnClearProfiles_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(LocalizationManager.Get("ConfirmClear"), "Spectra",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            _mainForm.GetProfiles()?.Clear();
            UpdateProfileList();
        }

        private void btnOpenLog_Click(object sender, EventArgs e)
        {
            string logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Spectra");
            if (!Directory.Exists(logDir)) Directory.CreateDirectory(logDir);
            Process.Start("explorer.exe", logDir);
        }

        private void btnResetAll_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(LocalizationManager.Get("ConfirmReset"), "Spectra",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Spectra");
            try
            {
                foreach (var f in new[] { "Spectra.ini", "applicationData.xml" })
                {
                    string p = Path.Combine(dir, f);
                    if (File.Exists(p)) File.Delete(p);
                }
                _mainForm.GetProfiles()?.Clear();
                UpdateProfileList();
                MessageBox.Show("All settings reset. Restart Spectra.", "Spectra", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Reset Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnGitHub_Click(object sender, EventArgs e)
            => Process.Start("https://github.com/X1NPAR1/Spectra");

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
