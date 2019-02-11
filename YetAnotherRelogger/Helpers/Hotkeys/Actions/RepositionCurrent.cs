﻿using System;
using System.Linq;
using System.Windows.Forms;
using YetAnotherRelogger.Helpers.Tools;

namespace YetAnotherRelogger.Helpers.Hotkeys.Actions
{
    public class RepositionCurrent : IHotkeyAction
    {
        private Hotkey _hotkey;

        public string Name => "RepositionCurrent";

        public string Author => "sinterlkaas";

        public string Description => "Reposition Current Window";

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
                AutoPosition.ManualPositionWindow(hwnd, diablo.X, diablo.Y, diablo.W, diablo.H);
                return;
            }
            test = BotSettings.Instance.Bots.FirstOrDefault(x => x.Demonbuddy.MainWindowHandle == hwnd);
            if (test != null)
            {
                var demonbuddy = test.Demonbuddy;
                if (demonbuddy == null)
                    return;
                AutoPosition.ManualPositionWindow(hwnd, demonbuddy.X, demonbuddy.Y, demonbuddy.W, demonbuddy.H);
                return;
            }
            Logger.Instance.WriteGlobal("Reposition Current Failed");
        }

        public bool Equals(IHotkeyAction other)
        {
            return (other?.Name == Name) && (other?.Version == Version);
        }
    }
}
