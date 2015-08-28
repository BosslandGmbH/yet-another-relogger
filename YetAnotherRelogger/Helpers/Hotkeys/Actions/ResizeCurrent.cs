using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using YetAnotherRelogger.Helpers.Bot;
using YetAnotherRelogger.Helpers.Tools;

namespace YetAnotherRelogger.Helpers.Hotkeys.Actions
{
    public class ResizeCurrent : IHotkeyAction
    {
        private Hotkey _hotkey;

        public string Name
        {
            get { return "ResizeCurrent"; }
        }

        public string Author
        {
            get { return "sinterlkaas"; }
        }

        public string Description
        {
            get { return "Resize Current Window to 800x600 in center of screen"; }
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
                    // Calculate window position
                    double posX = (screen.Bounds.Width*0.5) - 400 + screen.Bounds.X;
                    double posY = (screen.Bounds.Height*0.5) - 300 + screen.Bounds.Y;
                    // Set window position and size
                    AutoPosition.ManualPositionWindow(hwnd, (int) posX, (int) posY, 800, 600);
                }

                return;
            }
            Logger.Instance.WriteGlobal("Resize Current Failed");
        }

        public bool Equals(IHotkeyAction other)
        {
            return (other.Name == Name) && (other.Version == Version);
        }
    }
}