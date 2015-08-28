using System;
using System.Windows.Forms;
using YetAnotherRelogger.Properties;

namespace YetAnotherRelogger.Forms.SettingsTree
{
    public partial class IpHostCheck : UserControl
    {
        public IpHostCheck()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.ConnectionCheckIpHostList = tbAddressList.Text;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.ConnectionCheckIpHost = cbCheckBeforeBotStart.Checked;
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.ConnectionCheckCloseBots = cbCheck60AndClose.Checked;
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.ConnectionCheckIpCheck = cbEnableIPCheck.Checked;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.ConnectionCheckHostCheck = cbEnableHostCheck.Checked;
        }
    }
}