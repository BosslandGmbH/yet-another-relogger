using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace YetAnotherRelogger.Forms.Wizard
{
    public partial class SetAffinity : Form
    {
        private const int CP_NOCLOSE_BUTTON = 0x200;
        public List<CheckBox> cpus = new List<CheckBox>();

        public SetAffinity()
        {
            InitializeComponent();

            for (int i = 0; i < Environment.ProcessorCount; ++i)
            {
                var cpuBox = new CheckBox();
                panel1.Controls.Add(cpuBox);
                cpuBox.AutoSize = true;
                cpuBox.Location = new Point(4, 4 + i*23);
                cpuBox.Name = string.Format("checkBoxCpu{0}", i);
                cpuBox.Size = new Size(80, 17);
                cpuBox.TabIndex = 0;
                cpuBox.Text = string.Format("cpu {0}", i);
                cpuBox.UseVisualStyleBackColor = true;
                cpuBox.Checked = true;

                cpus.Add(cpuBox);
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        private void SetAffinity_Load(object sender, EventArgs e)
        {
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (CheckBox box in cpus)
                box.Checked = true;
        }

        // Disable Close button
    }
}