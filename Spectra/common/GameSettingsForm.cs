using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Spectra.Localization;
using Spectra.UI;

namespace Spectra.common
{
    public partial class GameSettingsForm : Form
    {
        private readonly IVibranceProxy _proxy;
        private readonly ListViewItem _sender;
        private readonly Func<int, string> _resolveLevelLabel;

        public GameSettingsForm(
            IVibranceProxy proxy,
            int minValue, int maxValue, int defaultValue,
            ListViewItem sender,
            ApplicationSetting setting,
            List<ResolutionModeWrapper> resolutionList,
            Func<int, string> resolveLevelLabel)
        {
            InitializeComponent();

            _proxy            = proxy;
            _sender           = sender;
            _resolveLevelLabel = resolveLevelLabel;

            trackBarLevel.Minimum = minValue;
            trackBarLevel.Maximum = maxValue;
            trackBarLevel.Value   = defaultValue;
            labelLevelValue.Text  = _resolveLevelLabel(defaultValue);
            labelTitle.Text       = LocalizationManager.Get("SettingsFor") + $"\"{sender.Text}\"";
            picGame.Image         = sender.ListView?.LargeImageList?.Images[sender.ImageIndex];
            cboResolution.DataSource = resolutionList;

            if (setting != null)
            {
                trackBarLevel.Value       = setting.IngameLevel;
                cboResolution.SelectedItem= setting.ResolutionSettings;
                chkResolution.Checked     = setting.IsResolutionChangeNeeded;
                trackBarLevel_Scroll(null, null);
            }

            ApplyTheme();
            LocalizationManager.LanguageChanged += OnLanguageChanged;

            Icon = IconFactory.GetAppIcon(16);
        }

        private void OnLanguageChanged(object sender, EventArgs e) => ApplyLocalization();

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            LocalizationManager.LanguageChanged -= OnLanguageChanged;
            base.OnFormClosed(e);
        }

        private void ApplyTheme()
        {
            if (InvokeRequired) { Invoke((Action)ApplyTheme); return; }
            BackColor                    = ThemeManager.Surface;
            ForeColor                    = ThemeManager.Text;
            labelTitle.ForeColor         = ThemeManager.Text;
            labelSectionVibrance.ForeColor = ThemeManager.Accent;
            labelSectionRes.ForeColor    = ThemeManager.Accent;
            labelResNote.ForeColor       = ThemeManager.TextSub;
            labelLevelValue.ForeColor    = ThemeManager.Accent;
            trackBarLevel.BackColor      = ThemeManager.Surface;
            chkResolution.ForeColor      = ThemeManager.Text;
            chkResolution.BackColor      = Color.Transparent;
            cboResolution.BackColor      = ThemeManager.Surface2;
            cboResolution.ForeColor      = ThemeManager.Text;
            btnSave.BackColor            = ThemeManager.Accent;
            btnSave.ForeColor            = Color.White;
            btnSave.FlatAppearance.BorderColor = ThemeManager.Accent;
        }

        private void ApplyLocalization()
        {
            if (InvokeRequired) { Invoke((Action)ApplyLocalization); return; }
            labelSectionVibrance.Text = LocalizationManager.Get("IngameVibrance");
            labelSectionRes.Text      = LocalizationManager.Get("IngameResolution");
            labelResNote.Text         = LocalizationManager.Get("BorderlessOnly");
            chkResolution.Text        = LocalizationManager.Get("ChangeResIngame");
            btnSave.Text              = LocalizationManager.Get("SaveProfile");
            Text                      = "Spectra";
        }

        private void trackBarLevel_Scroll(object sender, EventArgs e)
        {
            _proxy?.SetVibranceIngameLevel(trackBarLevel.Value);
            labelLevelValue.Text = _resolveLevelLabel(trackBarLevel.Value);
        }

        private void chkResolution_CheckedChanged(object sender, EventArgs e)
        {
            cboResolution.Enabled = chkResolution.Checked;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        public ApplicationSetting GetApplicationSetting()
        {
            return new ApplicationSetting(
                _sender.Text,
                _sender.Tag?.ToString(),
                trackBarLevel.Value,
                (ResolutionModeWrapper)cboResolution.SelectedItem,
                chkResolution.Checked);
        }
    }
}

