using System;
using System.Windows.Forms;

namespace YetAnotherRelogger.Helpers.Hotkeys.Actions
{
    public class RepositionAll : IHotkeyAction
    {
        private Hotkey _hotkey;

        public string Name => "RepositionAll";

        public string Author => "sinterlkaas";

        public string Description => "Reposition All Windows";

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
            AutoPosition.PositionWindows();
        }

        public bool Equals(IHotkeyAction other)
        {
            return (other?.Name == Name) && (other?.Version == Version);
        }
    }
}
