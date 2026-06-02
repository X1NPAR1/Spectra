using System;

namespace Spectra.common
{
    class WinEventHookEventArgs : EventArgs
    {
        public IntPtr Handle      { get; set; }
        public uint   ProcessId   { get; set; }
        public string ProcessName { get; set; }
    }
}
