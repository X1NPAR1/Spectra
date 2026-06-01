using System.Drawing;
using System.Windows.Forms;

namespace Spectra.common
{
    partial class GameSettingsForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            picGame            = new PictureBox();
            labelTitle         = new Label();
            labelSectionVibrance = new Label();
            trackBarLevel      = new TrackBar();
            labelLevelValue    = new Label();
            labelSectionRes    = new Label();
            labelResNote       = new Label();
            chkResolution      = new CheckBox();
            cboResolution      = new ComboBox();
            btnSave            = new Button();

            SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picGame).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarLevel).BeginInit();

            // Form
            AutoScaleDimensions = new SizeF(6f, 13f);
            AutoScaleMode       = AutoScaleMode.Font;
            ClientSize          = new Size(320, 310);
            FormBorderStyle     = FormBorderStyle.FixedSingle;
            MaximizeBox         = false;
            StartPosition       = FormStartPosition.CenterParent;
            Text                = "Spectra";
            Name                = "GameSettingsForm";

            // Game icon
            picGame.Location  = new Point(12, 12);
            picGame.Size      = new Size(48, 48);
            picGame.SizeMode  = PictureBoxSizeMode.StretchImage;

            // Title
            labelTitle.Text      = "Settings for";
            labelTitle.Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            labelTitle.Location  = new Point(68, 12);
            labelTitle.Size      = new Size(240, 48);
            labelTitle.TextAlign = ContentAlignment.MiddleLeft;

            // Vibrance section
            labelSectionVibrance.Text     = "IN-GAME VIBRANCE";
            labelSectionVibrance.Font     = new Font("Segoe UI", 7.5f, FontStyle.Bold);
            labelSectionVibrance.Location = new Point(12, 72);
            labelSectionVibrance.AutoSize = true;

            trackBarLevel.Location  = new Point(12, 92);
            trackBarLevel.Size      = new Size(240, 36);
            trackBarLevel.TickStyle = TickStyle.None;
            trackBarLevel.Scroll   += trackBarLevel_Scroll;

            labelLevelValue.Text      = "—";
            labelLevelValue.Font      = new Font("Segoe UI", 11f, FontStyle.Bold);
            labelLevelValue.Location  = new Point(258, 95);
            labelLevelValue.Size      = new Size(54, 26);
            labelLevelValue.TextAlign = ContentAlignment.MiddleLeft;

            // Resolution section
            labelSectionRes.Text     = "IN-GAME RESOLUTION";
            labelSectionRes.Font     = new Font("Segoe UI", 7.5f, FontStyle.Bold);
            labelSectionRes.Location = new Point(12, 142);
            labelSectionRes.AutoSize = true;

            labelResNote.Text     = "For borderless windowed mode only";
            labelResNote.Font     = new Font("Segoe UI", 7.5f, FontStyle.Italic);
            labelResNote.Location = new Point(12, 162);
            labelResNote.AutoSize = true;

            chkResolution.Text     = "Change resolution in-game";
            chkResolution.Location = new Point(12, 184);
            chkResolution.Size     = new Size(220, 20);
            chkResolution.CheckedChanged += chkResolution_CheckedChanged;

            cboResolution.Location      = new Point(12, 208);
            cboResolution.Size          = new Size(296, 22);
            cboResolution.Enabled       = false;
            cboResolution.DropDownStyle = ComboBoxStyle.DropDownList;

            // Save button
            btnSave.Text       = "Save";
            btnSave.Location   = new Point(118, 264);
            btnSave.Size       = new Size(80, 32);
            btnSave.FlatStyle  = FlatStyle.Flat;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click     += btnSave_Click;

            Controls.Add(picGame);
            Controls.Add(labelTitle);
            Controls.Add(labelSectionVibrance);
            Controls.Add(trackBarLevel);
            Controls.Add(labelLevelValue);
            Controls.Add(labelSectionRes);
            Controls.Add(labelResNote);
            Controls.Add(chkResolution);
            Controls.Add(cboResolution);
            Controls.Add(btnSave);

            ((System.ComponentModel.ISupportInitialize)picGame).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarLevel).EndInit();
            ResumeLayout(false);
        }

        private PictureBox picGame;
        private Label      labelTitle;
        private Label      labelSectionVibrance;
        private TrackBar   trackBarLevel;
        private Label      labelLevelValue;
        private Label      labelSectionRes;
        private Label      labelResNote;
        private CheckBox   chkResolution;
        private ComboBox   cboResolution;
        private Button     btnSave;
    }
}
