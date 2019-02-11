using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace YetAnotherRelogger.Forms.Wizard
{
    public partial class SetAffinity : Form
    {
        private const int CpNocloseButton = 0x200;
        public List<CheckBox> Cpus = new List<CheckBox>();

        public SetAffinity()
        {
            InitializeComponent();

            for (var i = 0; i < Environment.ProcessorCount; ++i)
            {
                var cpuBox = new CheckBox();
                panel1.Controls.Add(cpuBox);
                cpuBox.AutoSize = true;
                cpuBox.Location = new Point(4, 4 + i*23);
                cpuBox.Name = $"checkBoxCpu{i}";
                cpuBox.Size = new Size(80, 17);
                cpuBox.TabIndex = 0;
                cpuBox.Text = $"cpu {i}";
                cpuBox.UseVisualStyleBackColor = true;
                cpuBox.Checked = true;

                Cpus.Add(cpuBox);
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CpNocloseButton;
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
            foreach (var box in Cpus)
                box.Checked = true;
        }

        // Disable Close button
    }
}