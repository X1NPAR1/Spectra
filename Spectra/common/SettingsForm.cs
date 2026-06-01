using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Spectra.Localization;
using Spectra.UI;

namespace Spectra.common
{
    public partial class SettingsForm : Form
    {
        private readonly MainForm      _mainForm;
        private readonly IVibranceProxy _proxy;

        public SettingsForm(MainForm mainForm, IVibranceProxy proxy,
            int minLevel, int maxLevel, int defaultLevel, int currentLevel,
            Func<int, string> resolveLabel)
        {
            _mainForm = mainForm;
            _proxy    = proxy;

            InitializeComponent();

            Icon = IconFactory.GetAppIcon(16);

            // Populate monitor dropdown
            cboMonitorTarget.Items.Add(LocalizationManager.Get("MonitorAll"));
            cboMonitorTarget.Items.Add(LocalizationManager.Get("MonitorPrimary"));
            foreach (Screen s in Screen.AllScreens)
            {
                if (!s.Primary)
                    cboMonitorTarget.Items.Add(s.DeviceName);
            }

            // Sync combo to the proxy's current monitor target selection
            string currentTarget = _proxy.GetVibranceInfo().targetMonitorDeviceName;
            cboMonitorTarget.SelectedIndex = 0;          // default: all monitors
            if (currentTarget == "PRIMARY")
            {
                cboMonitorTarget.SelectedIndex = 1;
            }
            else if (currentTarget != null)
            {
                for (int i = 2; i < cboMonitorTarget.Items.Count; i++)
                {
                    if (cboMonitorTarget.Items[i].ToString() == currentTarget)
                    {
                        cboMonitorTarget.SelectedIndex = i;
                        break;
                    }
                }
            }

            cboMonitorTarget.SelectedIndexChanged += cboMonitorTarget_SelectedIndexChanged;

            // Profile count
            UpdateProfileCount();

            // System info
            lblSysInfo.Text = string.Format(".NET {0}   |   OS: {1}",
                Environment.Version, Environment.OSVersion.Version);

            // Populate profile list
            UpdateProfileList();

            // Subscribe localization
            LocalizationManager.LanguageChanged += OnLanguageChanged;

            ApplyLocalization();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            LocalizationManager.LanguageChanged -= OnLanguageChanged;
            base.OnFormClosed(e);
        }

        private void OnLanguageChanged(object s, EventArgs e)
        {
            if (!IsDisposed && IsHandleCreated) Invoke((Action)ApplyLocalization);
        }

        private void UpdateProfileCount() => UpdateProfileList();

        private void UpdateProfileList()
        {
            var profiles = _mainForm.GetProfiles();
            int count = profiles?.Count ?? 0;
            lblProfileCountVal.Text = count.ToString();

            lbProfiles.Items.Clear();
            if (profiles == null || count == 0)
            {
                lbProfiles.Items.Add("— " + LocalizationManager.Get("ProfileCount") + " 0 —");
                return;
            }
            foreach (var p in profiles)
                lbProfiles.Items.Add($"  {p.Name}   —   Level: {p.IngameLevel}");
        }

        // ── Localization ──────────────────────────────────────────────────
        private void ApplyLocalization()
        {
            Text = LocalizationManager.Get("SettingsTitle");

            tabBehavior.Text = LocalizationManager.Get("TabBehavior");
            tabDisplay.Text  = LocalizationManager.Get("TabDisplay");
            tabData.Text     = LocalizationManager.Get("TabData");
            tabAbout.Text    = LocalizationManager.Get("TabAbout");
            tabControl.Invalidate();

            // Behavior tab
            lblStartupSection.Text  = LocalizationManager.Get("BehaviorSection");
            chkAutostart.Text       = LocalizationManager.Get("Autostart");
            chkMinToTray.Text       = LocalizationManager.Get("MinimizeToTray");
            chkNotifications.Text   = LocalizationManager.Get("ShowNotifications");
            lblDelaySection.Text    = LocalizationManager.Get("ApplyDelay");
            lblDelayNote.Text       = LocalizationManager.Get("ApplyDelayNote");

            // Display tab
            lblMonitorSection.Text  = LocalizationManager.Get("DisplaySection");
            lblMonitorTarget.Text   = LocalizationManager.Get("MonitorTarget");
            lblResSection.Text      = LocalizationManager.Get("ResolutionSection");
            chkNeverResize.Text     = LocalizationManager.Get("NeverResize");
            chkResetOnExit.Text     = LocalizationManager.Get("ResetOnExit");

            // Update monitor dropdown items
            if (cboMonitorTarget.Items.Count >= 2)
            {
                cboMonitorTarget.Items[0] = LocalizationManager.Get("MonitorAll");
                cboMonitorTarget.Items[1] = LocalizationManager.Get("MonitorPrimary");
            }

            // Data tab
            lblProfileSection.Text  = LocalizationManager.Get("ProfilesSection");
            lblProfileCount.Text    = LocalizationManager.Get("ProfileCount");
            btnExport.Text          = LocalizationManager.Get("ExportProfiles");
            btnImport.Text          = LocalizationManager.Get("ImportProfiles");
            btnClearProfiles.Text   = LocalizationManager.Get("ClearProfiles");
            lblDataSection.Text     = LocalizationManager.Get("DataSection") != "DataSection"
                ? LocalizationManager.Get("DataSection") : "DATA & DIAGNOSTICS";
            lblDataNote.Text        = LocalizationManager.Get("DataNote");
            btnOpenLog.Text         = LocalizationManager.Get("OpenLogFolder");
            btnResetAll.Text        = LocalizationManager.Get("ResetAll");

            // About tab
            lblAboutDesc.Text       = LocalizationManager.Get("AboutDesc");
            lblSupportTitle.Text    = LocalizationManager.Get("AboutSupport");
            lblGpuLine1.Text        = LocalizationManager.Get("AboutGpuLine1");
            lblGpuLine2.Text        = LocalizationManager.Get("AboutGpuLine2");
            lblSysTitle.Text        = LocalizationManager.Get("SysInfo");
            btnGitHub.Text          = LocalizationManager.Get("ViewOnGitHub");
            btnOpenLogShort.Text    = LocalizationManager.Get("OpenLogFolderShort");

            btnClose.Text           = LocalizationManager.Get("OK");
        }

        // ── Owner-draw tabs ───────────────────────────────────────────────
        private void tabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            var tab      = tabControl.TabPages[e.Index];
            bool selected= e.Index == tabControl.SelectedIndex;
            var g        = e.Graphics;
            g.SmoothingMode     = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            using (var bg = new SolidBrush(selected ? ThemeManager.Surface : Color.FromArgb(230, 232, 244)))
                g.FillRectangle(bg, e.Bounds);

            if (selected)
            {
                using (var pen = new Pen(ThemeManager.Accent, 2))
                    g.DrawLine(pen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
            }

            using (var brush = new SolidBrush(selected ? ThemeManager.Accent : ThemeManager.TextSub))
            using (var font  = new Font("Segoe UI", 8.5f, selected ? FontStyle.Bold : FontStyle.Regular))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(tab.Text, font, brush, e.Bounds, sf);
            }
        }

        // ── Paints ───────────────────────────────────────────────────────
        private void panelHeader_Paint(object sender, PaintEventArgs e)
        {
            var r = panelHeader.ClientRectangle;
            using (var grad = new LinearGradientBrush(r,
                ThemeManager.GradStart, ThemeManager.GradEnd,
                LinearGradientMode.Horizontal))
                e.Graphics.FillRectangle(grad, r);
        }

        private void panelLogo_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode     = SmoothingMode.AntiAlias;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            try
            {
                using (var bmp = IconFactory.GetAppBitmap(60))
                    g.DrawImage(bmp, 2, 2, 60, 60);
            }
            catch { }
        }

        // ── Display tab handlers ──────────────────────────────────────────
        private void cboMonitorTarget_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = cboMonitorTarget.SelectedIndex;
            if (idx == 0)
                _proxy.SetTargetMonitorDeviceName(null);          // all monitors
            else if (idx == 1)
                _proxy.SetTargetMonitorDeviceName("PRIMARY");     // primary only
            else if (idx >= 2 && idx < cboMonitorTarget.Items.Count)
                _proxy.SetTargetMonitorDeviceName(cboMonitorTarget.Items[idx].ToString());
        }

        // ── Behavior tab handlers ─────────────────────────────────────────
        // (Startup checkbox persisted immediately via RegistryController on main form)

        // ── Data tab handlers ─────────────────────────────────────────────
        private void btnExport_Click(object sender, EventArgs e)
        {
            using (var dlg = new SaveFileDialog
            {
                Filter   = "JSON files (*.json)|*.json|XML files (*.xml)|*.xml",
                FileName = "spectra-profiles.json"
            })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;
                try
                {
                    var profiles = _mainForm.GetProfiles();
                    string content;
                    if (dlg.FilterIndex == 1)
                        content = BuildProfilesJson(profiles);
                    else
                    {
                        var xml = new System.Xml.Serialization.XmlSerializer(typeof(System.Collections.Generic.List<ApplicationSetting>));
                        var sb = new System.Text.StringBuilder();
                        using (var sw = new System.IO.StringWriter(sb))
                            xml.Serialize(sw, profiles);
                        content = sb.ToString();
                    }
                    File.WriteAllText(dlg.FileName, content, System.Text.Encoding.UTF8);
                    MessageBox.Show(LocalizationManager.Get("ExportProfiles") + " OK.", "Spectra", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        // Builds a proper JSON array from the profile list — no external libraries required.
        private static string BuildProfilesJson(System.Collections.Generic.List<ApplicationSetting> profiles)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("[");
            for (int i = 0; i < profiles.Count; i++)
            {
                var p = profiles[i];
                sb.Append("  {");
                sb.Append($" \"name\": {JStr(p.Name)},");
                sb.Append($" \"fileName\": {JStr(p.FileName)},");
                sb.Append($" \"ingameLevel\": {p.IngameLevel},");
                sb.Append($" \"changeResolution\": {(p.IsResolutionChangeNeeded ? "true" : "false")}");
                sb.Append(" }");
                if (i < profiles.Count - 1) sb.Append(",");
                sb.AppendLine();
            }
            sb.Append("]");
            return sb.ToString();
        }

        private static string JStr(string s)
        {
            if (s == null) return "null";
            return "\"" + s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "") + "\"";
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog { Filter = "XML/JSON files (*.xml;*.json)|*.xml;*.json" })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;
                try
                {
                    var xml = new System.Xml.Serialization.XmlSerializer(typeof(System.Collections.Generic.List<ApplicationSetting>));
                    using (var sr = new StreamReader(dlg.FileName))
                    {
                        var imported = (System.Collections.Generic.List<ApplicationSetting>)xml.Deserialize(sr);
                        var existing = _mainForm.GetProfiles();
                        existing.AddRange(imported);
                    }
                    UpdateProfileList();
                    MessageBox.Show("Profiles imported.", "Spectra", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        private void btnClearProfiles_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(LocalizationManager.Get("ConfirmClear"), "Spectra",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            _mainForm.GetProfiles()?.Clear();
            UpdateProfileList();
        }

        private void btnOpenLog_Click(object sender, EventArgs e)
        {
            string logDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Spectra");
            if (!Directory.Exists(logDir)) Directory.CreateDirectory(logDir);
            Process.Start("explorer.exe", logDir);
        }

        private void btnResetAll_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(LocalizationManager.Get("ConfirmReset"), "Spectra",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            string dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Spectra");
            try
            {
                string ini = Path.Combine(dir, "Spectra.ini");
                string xml = Path.Combine(dir, "applicationData.xml");
                if (File.Exists(ini)) File.Delete(ini);
                if (File.Exists(xml)) File.Delete(xml);
                _mainForm.GetProfiles()?.Clear();
                UpdateProfileCount();
                MessageBox.Show("All settings reset. Restart Spectra for changes to take effect.", "Spectra",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Reset Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        // ── About handlers ────────────────────────────────────────────────
        private void btnGitHub_Click(object sender, EventArgs e)
            => Process.Start("https://github.com/X1NPAR1/Spectra");

        // ── Close ─────────────────────────────────────────────────────────
        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
