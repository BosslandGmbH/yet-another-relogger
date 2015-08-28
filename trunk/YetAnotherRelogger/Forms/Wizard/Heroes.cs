using System;
using System.Windows.Forms;

namespace YetAnotherRelogger.Forms.Wizard
{
    public partial class Heroes : UserControl
    {
        private readonly WizardMain WM;

        public Heroes(WizardMain parent)
        {
            WM = parent;
            InitializeComponent();
            VisibleChanged += Heroes_VisibleChanged;
            this.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
        }

        private void Heroes_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
                WM.NextStep("Heroes");
        }

        private void Heroes_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Launch the stuff
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }
    }
}