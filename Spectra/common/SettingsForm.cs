using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;
using Spectra.Localization;
using Spectra.UI;

namespace Spectra.common
{
    public partial class SettingsForm : Form
    {
        private readonly IVibranceProxy _proxy;

        public SettingsForm(MainForm mainForm, IVibranceProxy proxy,
            int minLevel, int maxLevel, int defaultLevel, int currentLevel,
            Func<int, string> resolveLabel)
        {
            _proxy = proxy;

            InitializeComponent();

            Icon = IconFactory.GetAppIcon(16);

            // Populate monitor dropdown with all connected screens
            cboMonitorTarget.Items.Add(LocalizationManager.Get("MonitorAll"));
            cboMonitorTarget.Items.Add(LocalizationManager.Get("MonitorPrimary"));
            foreach (Screen s in Screen.AllScreens)
            {
                if (!s.Primary)
                    cboMonitorTarget.Items.Add(s.DeviceName);
            }

            // Sync combo to proxy's current monitor target
            string currentTarget = _proxy.GetVibranceInfo().targetMonitorDeviceName;
            cboMonitorTarget.SelectedIndex = 0;
            if (currentTarget == "PRIMARY")
                cboMonitorTarget.SelectedIndex = 1;
            else if (currentTarget != null)
            {
                for (int i = 2; i < cboMonitorTarget.Items.Count; i++)
                {
                    if (cboMonitorTarget.Items[i].ToString() == currentTarget)
                    { cboMonitorTarget.SelectedIndex = i; break; }
                }
            }
            cboMonitorTarget.SelectedIndexChanged += cboMonitorTarget_SelectedIndexChanged;

            // System info label
            lblSysInfo.Text = string.Format(".NET {0}   |   OS: {1}",
                Environment.Version, Environment.OSVersion.Version);

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

        // ── Localization ──────────────────────────────────────────────────
        private void ApplyLocalization()
        {
            Text = LocalizationManager.Get("SettingsTitle");

            tabBehavior.Text = LocalizationManager.Get("TabBehavior");
            tabDisplay.Text  = LocalizationManager.Get("TabDisplay");
            tabAbout.Text    = LocalizationManager.Get("TabAbout");
            tabControl.Invalidate();

            // Behavior tab
            lblStartupSection.Text = LocalizationManager.Get("BehaviorSection");
            chkAutostart.Text      = LocalizationManager.Get("Autostart");
            chkMinToTray.Text      = LocalizationManager.Get("MinimizeToTray");
            chkNotifications.Text  = LocalizationManager.Get("ShowNotifications");
            lblDelaySection.Text   = LocalizationManager.Get("ApplyDelay");
            lblDelayNote.Text      = LocalizationManager.Get("ApplyDelayNote");

            // Display tab
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

            // About tab
            lblAboutDesc.Text    = LocalizationManager.Get("AboutDesc");
            lblSupportTitle.Text = LocalizationManager.Get("AboutSupport");
            lblGpuLine1.Text     = LocalizationManager.Get("AboutGpuLine1");
            lblGpuLine2.Text     = LocalizationManager.Get("AboutGpuLine2");
            lblSysTitle.Text     = LocalizationManager.Get("SysInfo");
            btnGitHub.Text       = LocalizationManager.Get("ViewOnGitHub");
            btnOpenLogShort.Text = LocalizationManager.Get("OpenLogFolderShort");

            btnClose.Text = LocalizationManager.Get("OK");
        }

        // ── Owner-draw tabs ───────────────────────────────────────────────
        private void tabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            var tab      = tabControl.TabPages[e.Index];
            bool selected = e.Index == tabControl.SelectedIndex;
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
            try { using (var bmp = IconFactory.GetAppBitmap(60)) g.DrawImage(bmp, 2, 2, 60, 60); }
            catch { }
        }

        // ── Display tab — monitor combo ───────────────────────────────────
        private void cboMonitorTarget_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = cboMonitorTarget.SelectedIndex;
            if (idx == 0)      _proxy.SetTargetMonitorDeviceName(null);
            else if (idx == 1) _proxy.SetTargetMonitorDeviceName("PRIMARY");
            else if (idx >= 2 && idx < cboMonitorTarget.Items.Count)
                _proxy.SetTargetMonitorDeviceName(cboMonitorTarget.Items[idx].ToString());
        }

        // ── Data tab stubs (tab is hidden; handlers must exist for designer) ──
        private void btnExport_Click(object sender, EventArgs e) { }
        private void btnImport_Click(object sender, EventArgs e) { }
        private void btnClearProfiles_Click(object sender, EventArgs e) { }
        private void btnResetAll_Click(object sender, EventArgs e) { }

        private void btnOpenLog_Click(object sender, EventArgs e)
        {
            string logDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Spectra");
            if (!Directory.Exists(logDir)) Directory.CreateDirectory(logDir);
            Process.Start("explorer.exe", logDir);
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
