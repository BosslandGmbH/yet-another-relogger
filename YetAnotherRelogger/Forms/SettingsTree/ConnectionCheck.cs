using System;
using System.Windows.Forms;
using YetAnotherRelogger.Properties;

namespace YetAnotherRelogger.Forms.SettingsTree
{
    public partial class ConnectionCheck : UserControl
    {
        public ConnectionCheck()
        {
            InitializeComponent();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.ConnectionCheckPingHost1 = tbPingHost1.Text;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.ConnectionCheckPingHost2 = tbPingHost2.Text;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.ConnectionCheckPing = cbCheckConnection.Checked;
        }
    }
}