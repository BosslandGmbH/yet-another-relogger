using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using YetAnotherRelogger.Helpers.Tools;

namespace YetAnotherRelogger.Helpers.Hotkeys.Actions
{
    public class ResizeCurrent : IHotkeyAction
    {
        private Hotkey _hotkey;

        public string Name => "ResizeCurrent";

        public string Author => "sinterlkaas";

        public string Description => "Resize Current Window to 800x600 in center of screen";

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
                    // Calculate window position
                    var posX = (screen.Bounds.Width*0.5) - 400 + screen.Bounds.X;
                    var posY = (screen.Bounds.Height*0.5) - 300 + screen.Bounds.Y;
                    // Set window position and size
                    AutoPosition.ManualPositionWindow(hwnd, (int) posX, (int) posY, 800, 600);
                }

                return;
            }
            Logger.Instance.WriteGlobal("Resize Current Failed");
        }

        public bool Equals(IHotkeyAction other)
        {
            return (other?.Name == Name) && (other?.Version == Version);
        }
    }
}
