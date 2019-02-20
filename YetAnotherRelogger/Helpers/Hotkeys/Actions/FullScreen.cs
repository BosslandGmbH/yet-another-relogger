using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using YetAnotherRelogger.Helpers.Tools;

namespace YetAnotherRelogger.Helpers.Hotkeys.Actions
{
    public class FullScreen : IHotkeyAction
    {
        private Hotkey _hotkey;

        public string Name => "FullScreen";

        public string Author => "sinterlkaas";

        public string Description => "Make current window Fullscreen";

        public Version Version => new Version(1, 0, 0);

        public Form ConfigWindow => null;

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
            var hwnd = WinApi.GetForegroundWindow();

            var test = BotSettings.Instance.Bots.FirstOrDefault(x => x.Diablo.MainWindowHandle == hwnd);
            if (test != null)
            {
                var diablo = test.Diablo;
                if (diablo == null)
                    return;

                // Get window rectangle
                if (WinApi.GetWindowRect(new HandleRef(test, hwnd), out var rct))
                {
                    // Get screen where window is located
                    var rect = new Rectangle(rct.Left, rct.Top, rct.Width, rct.Heigth);
                    var screen = Screen.FromRectangle(rect);
                    // Set window fullscreen to current screen
                    WinApi.SetWindowPos(hwnd, IntPtr.Zero, screen.Bounds.X, screen.Bounds.Y, screen.Bounds.Width,
                        screen.Bounds.Height,
                        WinApi.SetWindowPosFlags.SWP_SHOWWINDOW | WinApi.SetWindowPosFlags.SWP_NOSENDCHANGING);
                }
            }
        }

        public bool Equals(IHotkeyAction other)
        {
            return (other?.Name == Name) && (other?.Version == Version);
        }
    }
}