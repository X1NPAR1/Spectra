using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Spectra.common
{
    // Singleton that monitors foreground window changes via Windows accessibility API.
    // Registered as WINEVENT_OUTOFCONTEXT (async, no DLL injection).
    class WinEventHook
    {
        [DllImport("user32.dll")]
        private static extern IntPtr SetWinEventHook(
            uint eventMin, uint eventMax, IntPtr hmodWinEventProc,
            WinEventDelegate lpfnWinEventProc,
            uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        private delegate void WinEventDelegate(
            IntPtr hWinEventHook, uint eventType, IntPtr hwnd,
            int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        private const uint EventSystemForeground  = 0x0003;
        private const uint WineventOutofcontext   = 0x0000;

        private static WinEventHook _instance;

        public event EventHandler<WinEventHookEventArgs> WinEventHookHandler;

        // Keep delegate alive as a field — if only stored as a local/temp variable the
        // GC can collect it while the unmanaged hook still holds a function pointer to it.
        private readonly WinEventDelegate _procDelegate;
        private readonly IntPtr _hookHandle;

        private WinEventHook()
        {
            _procDelegate = WinEventProc;
            _hookHandle   = SetWinEventHook(
                EventSystemForeground, EventSystemForeground,
                IntPtr.Zero, _procDelegate, 0, 0, WineventOutofcontext);
        }

        public static WinEventHook GetInstance()
        {
            if (_instance == null)
                _instance = new WinEventHook();
            return _instance;
        }

        public void RemoveWinEventHook()
        {
            try
            {
                if (!UnhookWinEvent(_hookHandle))
                    MainForm.Log(new Exception(
                        $"UnhookWinEvent failed. Handle = {_hookHandle}"));
            }
            catch (Exception ex)
            {
                // Log the original exception with its real stack trace.
                MainForm.Log(ex);
            }
        }

        // Called by the Windows message loop when the foreground window changes.
        // Fetches only the process name — previous code also retrieved window text
        // via two extra Win32 calls (GetWindowTextLength + GetWindowTextA) whose
        // result was never read by any subscriber.
        private static void WinEventProc(
            IntPtr hWinEventHook, uint eventType, IntPtr hwnd,
            int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (hwnd == IntPtr.Zero) return;

            GetWindowThreadProcessId(hwnd, out uint pid);
            if (pid == 0) return;

            try
            {
                using (var p = Process.GetProcessById((int)pid))
                {
                    var args = new WinEventHookEventArgs
                    {
                        Handle      = hwnd,
                        ProcessId   = pid,
                        ProcessName = p.ProcessName
                    };
                    GetInstance().DispatchEvent(args);
                }
            }
            catch (InvalidOperationException) { /* process exited before GetProcessById returned */ }
            catch (ArgumentException)         { /* pid no longer valid */                           }
        }

        private void DispatchEvent(WinEventHookEventArgs e)
            => WinEventHookHandler?.Invoke(this, e);
    }
}
