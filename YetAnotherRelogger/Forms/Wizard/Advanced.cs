using System;
using System.Windows.Forms;

namespace YetAnotherRelogger.Forms.Wizard
{
    public partial class Advanced : UserControl
    {
        private readonly WizardMain _wm;

        public Advanced(WizardMain parent)
        {
            InitializeComponent();
            _wm = parent;
            this.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
        }

        private void Advanced_Load(object sender, EventArgs e)
        {
            VisibleChanged += Advanced_VisibleChanged;
        }

        private void Advanced_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
                _wm.NextStep("Advanced Settings");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "D3Prefs.txt|*.txt",
                FileName = "D3Prefs.txt",
                Title = "Browse to D3Prefs.txt"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
                textBox3.Text = ofd.FileName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog {Description = "Select Diablo III clone location"};
            if (fbd.ShowDialog() == DialogResult.OK)
                textBox2.Text = fbd.SelectedPath;
        }
    }
}