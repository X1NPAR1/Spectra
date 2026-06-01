using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Spectra.AMD;
using Spectra.AMD.vendor;
using Spectra.AMD.vendor.utils;
using Spectra.common;
using Spectra.Localization;
using Spectra.NVIDIA;

namespace Spectra
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            bool created;
            var mutex = new Mutex(true, "Spectra~VibControl~Mutex", out created);
            if (!created)
            {
                MessageBox.Show(LocalizationManager.Get("RunOnce"),
                    "Spectra", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            NativeMethods.SetDllDirectory(CommonUtils.GetAppDataPath());

            GraphicsAdapter adapter = GraphicsAdapterHelper.GetAdapter();
            Form mainForm = null;

            switch (adapter)
            {
                case GraphicsAdapter.Amd:
                    mainForm = new MainForm(
                        (profiles, resMap) => new AmdDynamicVibranceProxy(
                            Environment.Is64BitOperatingSystem
                                ? new AmdAdapter64()
                                : (IAmdAdapter)new AmdAdapter32(),
                            profiles, resMap),
                        defaultWindowsLevel: 100,
                        minLevel:            0,
                        maxLevel:            300,
                        defaultIngameLevel:  100,
                        resolveLevelLabel:   v => v.ToString());
                    break;

                case GraphicsAdapter.Nvidia:
                    const string nvDllName   = "vibranceDLL.dll";
                    string       resourceName = typeof(Program).Namespace + ".NVIDIA." + nvDllName;
                    CommonUtils.LoadUnmanagedLibraryFromResource(
                        Assembly.GetExecutingAssembly(), resourceName, nvDllName);
                    Marshal.PrelinkAll(typeof(NvidiaDynamicVibranceProxy));

                    mainForm = new MainForm(
                        (profiles, resMap) => new NvidiaDynamicVibranceProxy(profiles, resMap),
                        defaultWindowsLevel: NvidiaDynamicVibranceProxy.NvapiDefaultLevel,
                        minLevel:            NvidiaDynamicVibranceProxy.NvapiDefaultLevel,
                        maxLevel:            NvidiaDynamicVibranceProxy.NvapiMaxLevel,
                        defaultIngameLevel:  NvidiaDynamicVibranceProxy.NvapiDefaultLevel,
                        resolveLevelLabel:   v => NvidiaVibranceValueWrapper.Find(v).Percentage);
                    break;

                case GraphicsAdapter.Unknown:
                    MessageBox.Show(
                        LocalizationManager.Get("ErrorGpuUnknown") + "\n\n" +
                        new Win32Exception(Marshal.GetLastWin32Error()).Message,
                        LocalizationManager.Get("ErrorCaption"),
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;

                case GraphicsAdapter.Ambiguous:
                    MessageBox.Show(
                        LocalizationManager.Get("ErrorGpuAmbiguous"),
                        LocalizationManager.Get("ErrorCaption"),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
            }

            if (mainForm == null) return;

            if (Array.IndexOf(args, "-minimized") >= 0)
            {
                mainForm.WindowState = FormWindowState.Minimized;
                ((MainForm)mainForm).SetAllowVisible(false);
            }

            // Trim trailing ".0" segments: "1.9.4.0" → "1.9.4"
            string ver = Application.ProductVersion;
            while (ver.EndsWith(".0")) ver = ver.Substring(0, ver.Length - 2);
            mainForm.Text = string.Format("Spectra  [{0}]  v{1}",
                adapter.ToString().ToUpperInvariant(), ver);

            Application.Run(mainForm);
            GC.KeepAlive(mutex);
        }
    }
}
