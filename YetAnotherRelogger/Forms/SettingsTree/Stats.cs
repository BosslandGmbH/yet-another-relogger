using System;
using System.Windows.Forms;

namespace YetAnotherRelogger.Forms.SettingsTree
{
    public partial class Stats : UserControl
    {
        public Stats()
        {
            InitializeComponent();
        }

        private void StatsEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (!StatsEnabled.Checked)
                StatsUpdater.Instance.Stop();
            else
                StatsUpdater.Instance.Start();
        }
    }
}