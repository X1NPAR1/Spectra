using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Spectra.common
{
    partial class ProcessExplorer
    {
        private IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components       = new Container();
            listView         = new ListView();
            iconList         = new ImageList(components);
            btnReload        = new Button();
            backgroundWorker = new BackgroundWorker();

            SuspendLayout();

            // Form
            AutoScaleDimensions = new SizeF(6f, 13f);
            AutoScaleMode       = AutoScaleMode.Font;
            ClientSize          = new Size(680, 340);
            FormBorderStyle     = FormBorderStyle.FixedSingle;
            MaximizeBox         = false;
            StartPosition       = FormStartPosition.CenterScreen;
            Text                = "Spectra — Running Processes";
            Name                = "ProcessExplorer";

            // listView
            listView.Location  = new Point(12, 48);
            listView.Size      = new Size(656, 280);
            listView.BorderStyle = BorderStyle.None;
            listView.UseCompatibleStateImageBehavior = false;
            listView.DoubleClick += listView_DoubleClick;

            // iconList
            iconList.ColorDepth      = ColorDepth.Depth32Bit;
            iconList.ImageSize       = new Size(32, 32);
            iconList.TransparentColor= System.Drawing.Color.Transparent;

            // btnReload
            btnReload.Text     = "Reload Processes";
            btnReload.Location = new Point(12, 12);
            btnReload.Size     = new Size(140, 28);
            btnReload.FlatStyle= FlatStyle.Flat;
            btnReload.FlatAppearance.BorderSize = 1;
            btnReload.Click   += btnReload_Click;

            // worker
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork           += backgroundWorker_DoWork;
            backgroundWorker.ProgressChanged  += backgroundWorker_ProgressChanged;

            Controls.Add(listView);
            Controls.Add(btnReload);
            ResumeLayout(false);
        }

        private ListView   listView;
        private ImageList  iconList;
        private Button     btnReload;
        private BackgroundWorker backgroundWorker;
    }
}
