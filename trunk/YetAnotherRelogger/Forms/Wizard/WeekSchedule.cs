using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using YetAnotherRelogger.Helpers;
using YetAnotherRelogger.Helpers.Bot;
using YetAnotherRelogger.Helpers.Tools;

namespace YetAnotherRelogger.Forms.Wizard
{
    public partial class WeekSchedule : UserControl
    {
        public static ClickBox[] ScheduleBox = new ClickBox[168];
        public readonly WizardMain WM = new WizardMain();

        private bool isDone;

        public WeekSchedule(WizardMain parent)
        {
            WM = parent;
            InitializeComponent();
            this.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
        }

        public static ClickBox[] getSchedule
        {
            get { return ScheduleBox; }
        }

        private void Schedule_Load(object sender, EventArgs e)
        {
            VisibleChanged += WeekSchedule_VisibleChanged;
            var Generator = new Thread(GenerateSchedule) { Name = "WeekScheduleGenerator" };

            textBox1.KeyPress += NumericCheck;
            textBox2.KeyPress += NumericCheck;
            Generator.Start();
        }

        private void NumericCheck(object sender, KeyPressEventArgs e)
        {
            e.Handled = General.NumericOnly(e.KeyChar);
        }

        private void ScheduleLoader(object bot)
        {
            var b = bot as BotClass;

            while (!isDone)
                Thread.Sleep(250);

            int n = 0; // Box number
            var md = new DaySchedule();
            for (int d = 1; d <= 7; d++)
            {
                switch (d)
                {
                    case 1:
                        md = b.Week.Monday;
                        break;
                    case 2:
                        md = b.Week.Tuesday;
                        break;
                    case 3:
                        md = b.Week.Wednesday;
                        break;
                    case 4:
                        md = b.Week.Thursday;
                        break;
                    case 5:
                        md = b.Week.Friday;
                        break;
                    case 6:
                        md = b.Week.Saturday;
                        break;
                    case 7:
                        md = b.Week.Sunday;
                        break;
                }
                for (int h = 0; h <= 23; h++)
                {
                    getSchedule[n].isEnabled = md.Hours[h];
                    n++; // increase box number
                }
            }
        }

        public void LoadSchedule(BotClass bot)
        {
            var loadScheduleThread = new Thread(ScheduleLoader) { Name = "ScheduleLoader" };
            loadScheduleThread.Start(bot);
        }

        private void WeekSchedule_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
                WM.NextStep("Week Schedule");
            //WizardMain.ActiveForm.Text = "Week Schedule (Step 3/5)";
        }

        private void GenerateSchedule()
        {
            int x = 0; // start at X-Axis
            int y = 14; // start at Y-Axis
            int n = 0; // box number
            bool bHeader = false;
            try
            {
                for (int d = 1; d <= 7; d++)
                {
                    for (int h = 0; h <= 23; h++)
                    {
                        if (!bHeader)
                        {
                            var l = new Label();
                            l.Location = new Point(x + 1, 0);
                            l.Margin = new Padding(0);
                            l.Name = "lbl" + n;
                            l.Text = n.ToString("D2"); // add leading zero when needed (2 digits)
                            l.Size = new Size(20, 13);
                            l.TextAlign = ContentAlignment.MiddleCenter;
                            Invoke(new Action(() => BoxPanel.Controls.Add(l)));
                        }
                        ScheduleBox[n] = new ClickBox("box" + n, x, y);

                        Invoke(new Action(() => BoxPanel.Controls.Add(ScheduleBox[n].box)));
                        n++;
                        x += 20; // X-Axis
                    }
                    bHeader = true;
                    y += 20; // Y-Axis
                    x = 0; // X-Axis
                }
            }
            catch (Exception ex)
            {
                DebugHelper.Exception(ex);
            }
            isDone = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Fill week
            foreach (ClickBox x in ScheduleBox)
            {
                if (x.box.BackColor != Color.LightGreen)
                    x.box.BackColor = Color.LightGreen;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Clear week
            foreach (ClickBox x in ScheduleBox)
            {
                if (x.box.BackColor != DefaultBackColor)
                    x.box.BackColor = DefaultBackColor;
            }
        }

        public bool ValidateInput()
        {
            return true;
        }

        public class ClickBox
        {
            public PictureBox box;

            public ClickBox(string name, int x, int y)
            {
                box = new PictureBox
                {
                    BorderStyle = BorderStyle.FixedSingle,
                    Location = new Point(x, y),
                    Name = name,
                    Size = new Size(21, 21)
                };
                box.Click += box_Click;
                box.MouseEnter += box_MouseEnter;
            }

            public bool isEnabled
            {
                get { return (box.BackColor == Color.LightGreen); }
                set
                {
                    if (value)
                        box.BackColor = Color.LightGreen;
                }
            }

            private void box_MouseEnter(object sender, EventArgs e)
            {
                if (WinAPI.IsKeyDown(WinAPI.VirtualKeyStates.VK_SHIFT))
                    box.BackColor = Color.LightGreen;
            }

            public void box_Click(object sender, EventArgs e)
            {
                box.BackColor = box.BackColor == Color.LightGreen ? DefaultBackColor : Color.LightGreen;
            }
        }
    }
}