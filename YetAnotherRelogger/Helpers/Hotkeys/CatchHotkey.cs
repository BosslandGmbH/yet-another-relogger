using System;
using System.Windows.Forms;

namespace YetAnotherRelogger.Helpers.Hotkeys
{
    public partial class CatchHotkey : Form
    {
        private readonly Hotkey _hotkey;
        private readonly NewHotkey _parent;

        public CatchHotkey(NewHotkey parent)
        {
            InitializeComponent();
            _parent = parent;
            _hotkey = parent.HotkeyNew;
        }

        private void CatchHotkey_Load(object sender, EventArgs e)
        {
            KeyPreview = true;
            KeyDown += CatchHotkey_KeyDown;
            KeyUp += CatchHotkey_KeyUp;
            Closed += CatchHotkey_Closed;
        }

        private void CatchHotkey_KeyDown(object sender, KeyEventArgs e)
        {
            label3.Text = $@"{GlobalHotkeys.KeysToModifierKeys(e.Modifiers).ToString().Replace(", ", "+")}+{e.KeyCode}";
            _hotkey.Modifier = GlobalHotkeys.KeysToModifierKeys(e.Modifiers);
            _hotkey.Key = e.KeyCode;
            e.SuppressKeyPress = true;
            e.Handled = false;
        }

        private void CatchHotkey_KeyUp(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            e.Handled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Close
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Ready / Save
            _parent.HotkeyNew = _hotkey;
            Close();
        }

        private void CatchHotkey_Closed(object sender, EventArgs e)
        {
            _parent.textBox2.Text = $@"{_hotkey.Modifier.ToString().Replace(", ", "+")}+{_hotkey.Key}";
        }
    }
}
