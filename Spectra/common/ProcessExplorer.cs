using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Spectra.Localization;
using Spectra.UI;

namespace Spectra.common
{
    public partial class ProcessExplorer : Form
    {
        private readonly Form _mainForm;

        public ProcessExplorer(Form mainForm)
        {
            InitializeComponent();

            _mainForm = mainForm;

            listView.Columns.Add("Program", 160, HorizontalAlignment.Left);
            listView.Columns.Add("Path",    380, HorizontalAlignment.Left);
            listView.LargeImageList = iconList;
            listView.FullRowSelect  = true;
            listView.View           = View.Tile;
            listView.HeaderStyle    = ColumnHeaderStyle.Nonclickable;

            Icon = IconFactory.GetAppIcon(16);
            Text = LocalizationManager.Get("ProcessExplorer");

            ApplyTheme();
            
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.RunWorkerAsync();
        }

        private void ApplyTheme()
        {
            if (InvokeRequired) { Invoke((Action)ApplyTheme); return; }
            BackColor         = ThemeManager.Bg;
            listView.BackColor= ThemeManager.Surface;
            listView.ForeColor= ThemeManager.Text;
            btnReload.BackColor = ThemeManager.Surface2;
            btnReload.ForeColor = ThemeManager.Text;
            btnReload.FlatAppearance.BorderColor = ThemeManager.Border;
        }

        [DllImport("psapi.dll")]
        private static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule,
            [Out] StringBuilder lpBaseName, [In][MarshalAs(UnmanagedType.U4)] int nSize);

        private string GetProcessPath(Process p)
        {
            var sb = new StringBuilder(1024);
            return GetModuleFileNameEx(p.Handle, IntPtr.Zero, sb, sb.Capacity) > 0 ? sb.ToString() : string.Empty;
        }

        private void EnumerateProcesses()
        {
            int count = 0;
            foreach (Process p in Process.GetProcesses())
            {
                if (p.MainWindowHandle == IntPtr.Zero || p.Id == Process.GetCurrentProcess().Id) continue;
                try
                {
                    string path = GetProcessPath(p);
                    if (!string.IsNullOrEmpty(path) && File.Exists(path))
                    {
                        var entry = new ProcessExplorerEntry(path, Icon.ExtractAssociatedIcon(path), p);
                        backgroundWorker.ReportProgress(++count, entry);
                    }
                }
                catch (Exception ex) { MainForm.Log(ex); }
            }
        }

        private void listView_DoubleClick(object sender, EventArgs e)
        {
            // Game profile feature removed in v1.9.4
            Close();
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            if (backgroundWorker.IsBusy) return;
            listView.Items.Clear();
            iconList.Images.Clear();
            backgroundWorker.RunWorkerAsync();
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e) => EnumerateProcesses();

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (!(e.UserState is ProcessExplorerEntry entry)) return;
            iconList.Images.Add(entry.Icon);
            var item = new ListViewItem(entry.ProcessName, iconList.Images.Count - 1) { Tag = entry };
            item.SubItems.Add(entry.Path);
            listView.Items.Add(item);
        }
    }
}

