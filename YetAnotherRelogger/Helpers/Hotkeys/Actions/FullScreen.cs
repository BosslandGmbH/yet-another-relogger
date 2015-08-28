using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using YetAnotherRelogger.Helpers.Bot;
using YetAnotherRelogger.Helpers.Tools;

namespace YetAnotherRelogger.Helpers.Hotkeys.Actions
{
    public class FullScreen : IHotkeyAction
    {
        private Hotkey _hotkey;

        public string Name
        {
            get { return "FullScreen"; }
        }

        public string Author
        {
            get { return "sinterlkaas"; }
        }

        public string Description
        {
            get { return "Make current window Fullscreen"; }
        }

        public Version Version
        {
            get { return new Version(1, 0, 0); }
        }

        public Form ConfigWindow
        {
            get { return null; }
        }

        public void OnInitialize(Hotkey hotkey)
        {
            _hotkey = hotkey;
        }

        public void OnDispose()
        {
        }

        public void OnPressed()
        {
            Logger.Instance.WriteGlobal("Hotkey pressed: {0}+{1} : {2}", _hotkey.Modifier.ToString().Replace(", ", "+"),
                _hotkey.Key, Name);
            // Get active window
            IntPtr hwnd = WinAPI.GetForegroundWindow();

            BotClass test = BotSettings.Instance.Bots.FirstOrDefault(x => x.Diablo.MainWindowHandle == hwnd);
            if (test != null)
            {
                DiabloClass diablo = test.Diablo;
                if (diablo == null)
                    return;

                // Get window rectangle
                WinAPI.RECT rct;
                if (WinAPI.GetWindowRect(new HandleRef(test, hwnd), out rct))
                {
                    // Get screen where window is located
                    var rect = new Rectangle(rct.Left, rct.Top, rct.Width, rct.Heigth);
                    Screen screen = Screen.FromRectangle(rect);
                    // Set window fullscreen to current screen
                    WinAPI.SetWindowPos(hwnd, IntPtr.Zero, screen.Bounds.X, screen.Bounds.Y, screen.Bounds.Width,
                        screen.Bounds.Height,
                        WinAPI.SetWindowPosFlags.SWP_SHOWWINDOW | WinAPI.SetWindowPosFlags.SWP_NOSENDCHANGING);
                }
            }
        }

        public bool Equals(IHotkeyAction other)
        {
            return (other.Name == Name) && (other.Version == Version);
        }
    }
}