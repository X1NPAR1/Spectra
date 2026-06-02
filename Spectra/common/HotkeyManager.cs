using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Spectra.common
{
    public class HotkeyManager : IDisposable
    {
        [DllImport("user32.dll")] private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll")] private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const uint MOD_ALT      = 0x0001;
        private const uint MOD_CONTROL  = 0x0002;
        private const uint MOD_SHIFT    = 0x0004;
        private const uint MOD_NOREPEAT = 0x4000;

        public const int WM_HOTKEY = 0x0312;
        private const int HOTKEY_TOGGLE_ID = 9001;

        private readonly IntPtr _handle;
        private Keys _currentKey       = Keys.F9;
        private Keys _currentModifiers = Keys.None;
        private bool _registered;

        public Keys CurrentKey       => _currentKey;
        public Keys CurrentModifiers => _currentModifiers;
        public event EventHandler TogglePressed;

        public HotkeyManager(IntPtr windowHandle)
        {
            _handle = windowHandle;
        }

        public bool Register(Keys key, Keys modifiers = Keys.None)
        {
            Unregister();

            Keys baseKey = key & ~Keys.Modifiers;
            if (baseKey == Keys.None) return false;

            bool isFunctionKey = baseKey >= Keys.F1 && baseKey <= Keys.F24;
            if (!isFunctionKey && modifiers == Keys.None)
                modifiers = Keys.Control;

            _currentKey       = baseKey;
            _currentModifiers = modifiers;

            uint win32Mods = MOD_NOREPEAT;
            if ((modifiers & Keys.Alt)     != 0) win32Mods |= MOD_ALT;
            if ((modifiers & Keys.Control) != 0) win32Mods |= MOD_CONTROL;
            if ((modifiers & Keys.Shift)   != 0) win32Mods |= MOD_SHIFT;

            _registered = RegisterHotKey(_handle, HOTKEY_TOGGLE_ID, win32Mods, (uint)baseKey);
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

        public string GetDisplayString()
        {
            string k = _currentKey.ToString();
            if (_currentModifiers == Keys.None) return k;

            string mods = "";
            if ((_currentModifiers & Keys.Control) != 0) mods += "Ctrl+";
            if ((_currentModifiers & Keys.Alt)     != 0) mods += "Alt+";
            if ((_currentModifiers & Keys.Shift)   != 0) mods += "Shift+";
            return mods + k;
        }

        public void Dispose() => Unregister();
    }
}
