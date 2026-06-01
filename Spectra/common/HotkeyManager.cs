using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Spectra.common
{
    public class HotkeyManager : IDisposable
    {
        [DllImport("user32.dll")] private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll")] private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public const int WM_HOTKEY = 0x0312;
        private const int HOTKEY_TOGGLE_ID = 9001;

        private readonly IntPtr _handle;
        private Keys _currentKey = Keys.F9;
        private bool _registered;

        public Keys CurrentKey => _currentKey;
        public event EventHandler TogglePressed;

        public HotkeyManager(IntPtr windowHandle)
        {
            _handle = windowHandle;
        }

        public bool Register(Keys key)
        {
            Unregister();
            _currentKey = key;
            _registered = RegisterHotKey(_handle, HOTKEY_TOGGLE_ID, 0, (uint)key);
            return _registered;
        }

        public void Unregister()
        {
            if (_registered)
            {
                UnregisterHotKey(_handle, HOTKEY_TOGGLE_ID);
                _registered = false;
            }
        }

        public void ProcessMessage(Message m)
        {
            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_TOGGLE_ID)
                TogglePressed?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose() => Unregister();
    }
}
