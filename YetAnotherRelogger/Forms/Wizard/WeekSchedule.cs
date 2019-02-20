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
        public readonly WizardMain Wm = new WizardMain();

        private bool _isDone;

        public WeekSchedule(WizardMain parent)
        {
            Wm = parent;
            InitializeComponent();
            this.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
        }

        public static ClickBox[] GetSchedule => ScheduleBox;

        private void Schedule_Load(object sender, EventArgs e)
        {
            VisibleChanged += WeekSchedule_VisibleChanged;
            var generator = new Thread(GenerateSchedule) { Name = "WeekScheduleGenerator" };

            textBox1.KeyPress += NumericCheck;
            textBox2.KeyPress += NumericCheck;
            generator.Start();
        }

        private void NumericCheck(object sender, KeyPressEventArgs e)
        {
            e.Handled = General.NumericOnly(e.KeyChar);
        }

        private void ScheduleLoader(object bot)
        {
            var b = bot as Bot;

            while (!_isDone)
                Thread.Sleep(250);

            var n = 0; // Box number
            var md = new DaySchedule();
            for (var d = 1; d <= 7; d++)
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
                for (var h = 0; h <= 23; h++)
                {
                    GetSchedule[n].IsEnabled = md.Hours[h];
                    n++; // increase box number
                }
            }
        }

        public void LoadSchedule(Bot bot)
        {
            var loadScheduleThread = new Thread(ScheduleLoader) { Name = "ScheduleLoader" };
            loadScheduleThread.Start(bot);
        }

        private void WeekSchedule_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
                Wm.NextStep("Week Schedule");
            //WizardMain.ActiveForm.Text = "Week Schedule (Step 3/5)";
        }

        private void GenerateSchedule()
        {
            var x = 0; // start at X-Axis
            var y = 14; // start at Y-Axis
            var n = 0; // box number
            var bHeader = false;
            try
            {
                for (var d = 1; d <= 7; d++)
                {
                    for (var h = 0; h <= 23; h++)
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

                        Invoke(new Action(() => BoxPanel.Controls.Add(ScheduleBox[n].Box)));
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
            _isDone = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Fill week
            foreach (var x in ScheduleBox)
            {
                if (x.Box.BackColor != Color.LightGreen)
                    x.Box.BackColor = Color.LightGreen;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Clear week
            foreach (var x in ScheduleBox)
            {
                if (x.Box.BackColor != DefaultBackColor)
                    x.Box.BackColor = DefaultBackColor;
            }
        }

        public bool ValidateInput()
        {
            return true;
        }

        public class ClickBox
        {
            public PictureBox Box;

            public ClickBox(string name, int x, int y)
            {
                Box = new PictureBox
                {
                    BorderStyle = BorderStyle.FixedSingle,
                    Location = new Point(x, y),
                    Name = name,
                    Size = new Size(21, 21)
                };
                Box.Click += box_Click;
                Box.MouseEnter += box_MouseEnter;
            }

            public bool IsEnabled
            {
                get => (Box.BackColor == Color.LightGreen);
                set
                {
                    if (value)
                        Box.BackColor = Color.LightGreen;
                }
            }

            private void box_MouseEnter(object sender, EventArgs e)
            {
                if (WinApi.IsKeyDown(WinApi.VirtualKeyStates.VkShift))
                    Box.BackColor = Color.LightGreen;
            }

            public void box_Click(object sender, EventArgs e)
            {
                Box.BackColor = Box.BackColor == Color.LightGreen ? DefaultBackColor : Color.LightGreen;
            }
        }
    }
}