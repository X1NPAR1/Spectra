using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Spectra.common
{
    public static class GameDetector
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("psapi.dll")]
        private static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule,
            [Out] StringBuilder lpBaseName, [In][MarshalAs(UnmanagedType.U4)] int nSize);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT { public int Left, Top, Right, Bottom; }

        public static string GetForegroundFullScreenExecutable()
        {
            IntPtr hWnd = GetForegroundWindow();
            if (hWnd == IntPtr.Zero) return null;

            if (!GetWindowRect(hWnd, out RECT rect)) return null;

            var screen = Screen.FromHandle(hWnd);
            int w = rect.Right - rect.Left;
            int h = rect.Bottom - rect.Top;

            bool fullScreen = w >= screen.Bounds.Width && h >= screen.Bounds.Height;
            if (!fullScreen) return null;

            GetWindowThreadProcessId(hWnd, out uint pid);
            if (pid == 0) return null;

            try
            {
                using (var p = Process.GetProcessById((int)pid))
                {
                    if (p.Id == Process.GetCurrentProcess().Id) return null;
                    string name = p.ProcessName.ToLowerInvariant();
                    if (name == "explorer" || name == "spectra") return null;

                    var sb = new StringBuilder(1024);
                    if (GetModuleFileNameEx(p.Handle, IntPtr.Zero, sb, sb.Capacity) > 0)
                        return sb.ToString();
                }
            }
            catch { }
            return null;
        }
    }
}
