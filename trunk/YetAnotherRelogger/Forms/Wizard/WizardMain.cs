using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using YetAnotherRelogger.Helpers;
using YetAnotherRelogger.Helpers.Bot;

namespace YetAnotherRelogger.Forms.Wizard
{
    public partial class WizardMain : Form
    {
        private readonly BotClass bot;
        public SetAffinity AffinityDemonbuddy;
        public SetAffinity AffinityDiablo;
        public int FinishCount;
        private int _mainCount;
        private int _stepCount;
        private Advanced _ucAdvanced;
        private DemonbuddyOptions _ucDemonbuddy;
        public DiabloOptions _ucDiablo;
        private Heroes _ucHeroes;
        private ProfileSchedule _ucProfileSchedule;
        private WeekSchedule _ucWeekSchedule;
        private int index = -1;
        private bool shouldClose;

        public WizardMain()
        {
            InitializeComponent();
        }

        public WizardMain(int index)
        {
            this.index = index;
            bot = BotSettings.Instance.Bots[index];
            InitializeComponent();
        }

        public void NextStep(string title)
        {
            Text = string.Format("{0} (Step {1}/{2})", title, _stepCount - 2, FinishCount - 2);
        }

        private void WizardMain_Load(object sender, EventArgs e)
        {
            Closing += WizardMain_Closing;

            _mainCount = Controls.Count;
            _stepCount = _mainCount; // set start point

            _ucDemonbuddy = new DemonbuddyOptions(this);
            _ucDiablo = new DiabloOptions(this);
            _ucWeekSchedule = new WeekSchedule(this);
            _ucHeroes = new Heroes(this);
            _ucProfileSchedule = new ProfileSchedule(this);
            _ucAdvanced = new Advanced(this);


            Controls.Add(_ucDemonbuddy);
            Controls.Add(_ucDiablo);
            Controls.Add(_ucWeekSchedule);
            //Controls.Add(ucHeroes);
            Controls.Add(_ucProfileSchedule);
            Controls.Add(_ucAdvanced);
            _ucDiablo.Visible =
                _ucWeekSchedule.Visible = _ucProfileSchedule.Visible = _ucHeroes.Visible = _ucAdvanced.Visible = false;

            FinishCount = Controls.Count - 1; // Get Finish count

            AffinityDiablo = new SetAffinity();
            AffinityDemonbuddy = new SetAffinity();

            if (bot != null)
                LoadData();
        }

        private void LoadData()
        {
            // Load data
            _ucDemonbuddy.textBox1.Text = bot.Name;
            _ucDemonbuddy.textBox2.Text = bot.Description;

            // Advanced section
            _ucAdvanced.checkBox2.Checked = bot.CreateWindowsUser;
            _ucAdvanced.checkBox1.Checked = bot.UseWindowsUser;
            _ucAdvanced.textBox1.Text = bot.WindowsUserName;
            _ucAdvanced.maskedTextBox1.Text = bot.WindowsUserPassword;
            _ucAdvanced.textBox3.Text = bot.D3PrefsLocation;
            _ucAdvanced.checkBox3.Checked = bot.UseDiabloClone;
            _ucAdvanced.textBox2.Text = bot.DiabloCloneLocation;

            // Demonbuddy
            _ucDemonbuddy.textBox4.Text = bot.Demonbuddy.Location;
            _ucDemonbuddy.textBox3.Text = bot.Demonbuddy.Key;

            _ucDemonbuddy.comboBox1.Text = bot.Demonbuddy.CombatRoutine;
            _ucDemonbuddy.checkBox1.Checked = bot.Demonbuddy.NoFlash;
            _ucDemonbuddy.checkBox2.Checked = bot.Demonbuddy.AutoUpdate;
            _ucDemonbuddy.checkBox3.Checked = bot.Demonbuddy.NoUpdate;
            _ucDemonbuddy.textBox9.Text = bot.Demonbuddy.BuddyAuthUsername;
            _ucDemonbuddy.maskedTextBox2.Text = bot.Demonbuddy.BuddyAuthPassword;
            _ucDemonbuddy.comboBox2.SelectedIndex = bot.Demonbuddy.Priority;
            _ucDemonbuddy.checkBox5.Checked = bot.Demonbuddy.ForceEnableAllPlugins;
            // Demonbuddy manual position
            _ucDemonbuddy.checkBox4.Checked = bot.Demonbuddy.ManualPosSize;
            _ucDemonbuddy.textBox6.Text = bot.Demonbuddy.X.ToString();
            _ucDemonbuddy.textBox5.Text = bot.Demonbuddy.Y.ToString();
            _ucDemonbuddy.textBox10.Text = bot.Demonbuddy.W.ToString();
            _ucDemonbuddy.textBox11.Text = bot.Demonbuddy.H.ToString();

            // Diablo
            _ucDiablo.username.Text = bot.Diablo.Username;
            _ucDiablo.password.Text = bot.Diablo.Password;
            _ucDiablo.diablo3Path.Text = bot.Diablo.Location;
            _ucDiablo.language.SelectedItem = bot.Diablo.Language;
            _ucDiablo.region.SelectedItem = bot.Diablo.Region;
            _ucDiablo.checkBox1.Checked = bot.Diablo.UseAuthenticator;
            _ucDiablo.useInnerSpace.Checked = bot.Diablo.UseIsBoxer;
            _ucDiablo.isBoxerLaunchAll.Checked = bot.Diablo.ISBoxerLaunchCharacterSet;
            _ucDiablo.characterSet.Text = bot.Diablo.CharacterSet;
            _ucDiablo.displaySlot.Text = bot.Diablo.DisplaySlot;
            _ucDiablo.removeWindowFrame.Checked = bot.Diablo.NoFrame;

            // Affinity Diablo
            if (bot.Diablo.CpuCount != Environment.ProcessorCount)
            {
                bot.Diablo.ProcessorAffinity = bot.Diablo.AllProcessors;
                bot.Diablo.CpuCount = Environment.ProcessorCount;
            }

            if (AffinityDiablo.cpus.Count != bot.Diablo.CpuCount)
            {
                Logger.Instance.Write(
                    "For whatever reason Diablo and UI see different number of CPUs, affinity disabled");
            }
            else
            {
                for (int i = 0; i < bot.Diablo.CpuCount; i++)
                {
                    AffinityDiablo.cpus[i].Checked = ((bot.Diablo.ProcessorAffinity & (1 << i)) != 0);
                }
            }
            // Affinity Demonbuddy
            if (bot.Demonbuddy.CpuCount != Environment.ProcessorCount)
            {
                bot.Demonbuddy.ProcessorAffinity = bot.Demonbuddy.AllProcessors;
                bot.Demonbuddy.CpuCount = Environment.ProcessorCount;
            }

            if (AffinityDemonbuddy.cpus.Count != bot.Demonbuddy.CpuCount)
            {
                Logger.Instance.Write(
                    "For whatever reason Demonbuddy and UI see different number of CPUs, affinity disabled");
            }
            else
            {
                for (int i = 0; i < bot.Demonbuddy.CpuCount; i++)
                {
                    AffinityDemonbuddy.cpus[i].Checked = ((bot.Demonbuddy.ProcessorAffinity & (1 << i)) != 0);
                }
            }

            //!!!d.Serial = string.Format("{0}-{1}-{2}-{3}", ucDiablo.textBox4.Text, ucDiablo.textBox5.Text, ucDiablo.textBox7.Text, ucDiablo.textBox6.Text);
            //!!!ucDiablo.textBox8.Text = bot.diablo.RestoreCode;

            /*
             d.Serial = string.Format("{0}-{1}-{2}-{3}", _ucDiablo.textBox4.Text, _ucDiablo.textBox5.Text,
                    _ucDiablo.textBox7.Text, _ucDiablo.textBox6.Text);
                d.RestoreCode = _ucDiablo.textBox8.Text;
             */

            string SerialCode = bot.Diablo.Serial;
            string[] words;
            words = SerialCode.Split('-');

            _ucDiablo.authField1.Text = words[0];
            _ucDiablo.authField2.Text = words[1];
            _ucDiablo.authField3.Text = words[2];
            _ucDiablo.authField4.Text = words[3];
            _ucDiablo.textBox8.Text = bot.Diablo.RestoreCode;


            _ucDiablo.processorAffinity.SelectedIndex = bot.Diablo.Priority;

            // Diablo manual position
            _ucDiablo.manualPositionAndSize.Checked = bot.Diablo.ManualPosSize;
            _ucDiablo.positionX.Text = bot.Diablo.X.ToString();
            _ucDiablo.positionY.Text = bot.Diablo.Y.ToString();
            _ucDiablo.width.Text = bot.Diablo.W.ToString();
            _ucDiablo.height.Text = bot.Diablo.H.ToString();

            // Profile Schedule
            _ucProfileSchedule.Profiles = bot.ProfileSchedule.Profiles;
            _ucProfileSchedule.textBox1.Text = bot.ProfileSchedule.MaxRandomTime.ToString();
            _ucProfileSchedule.textBox2.Text = bot.ProfileSchedule.MaxRandomRuns.ToString();
            _ucProfileSchedule.checkBox1.Checked = bot.ProfileSchedule.Random;

            // Load Weekschedule
            _ucWeekSchedule.textBox1.Text = bot.Week.MinRandom.ToString();
            _ucWeekSchedule.textBox2.Text = bot.Week.MaxRandom.ToString();
            _ucWeekSchedule.checkBox1.Checked = bot.Week.Shuffle;
            _ucWeekSchedule.LoadSchedule(bot);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // NEXT / finish
            if (_stepCount == FinishCount)
            {
                int result;
                var b = new BotClass();
                var db = new DemonbuddyClass();
                var d = new DiabloClass();
                var ps = new ProfileScheduleClass();
                var w = new Helpers.Bot.WeekSchedule();


                b.Name = _ucDemonbuddy.textBox1.Text;
                b.Description = _ucDemonbuddy.textBox2.Text;

                // Advanced
                b.CreateWindowsUser = _ucAdvanced.checkBox2.Checked;
                b.UseWindowsUser = _ucAdvanced.checkBox1.Checked;
                b.WindowsUserName = _ucAdvanced.textBox1.Text;
                b.WindowsUserPassword = _ucAdvanced.maskedTextBox1.Text;
                b.D3PrefsLocation = _ucAdvanced.textBox3.Text;
                b.UseDiabloClone = _ucAdvanced.checkBox3.Checked;
                b.DiabloCloneLocation = _ucAdvanced.textBox2.Text;

                // Demonbuddy
                db.Location = _ucDemonbuddy.textBox4.Text;
                db.Key = _ucDemonbuddy.textBox3.Text;
                db.CombatRoutine = _ucDemonbuddy.comboBox1.SelectedItem != null
                    ? _ucDemonbuddy.comboBox1.SelectedItem.ToString()
                    : _ucDemonbuddy.comboBox1.Text;
                db.NoFlash = _ucDemonbuddy.checkBox1.Checked;
                db.AutoUpdate = _ucDemonbuddy.checkBox2.Checked;
                db.NoUpdate = _ucDemonbuddy.checkBox3.Checked;
                db.BuddyAuthUsername = _ucDemonbuddy.textBox9.Text;
                db.BuddyAuthPassword = _ucDemonbuddy.maskedTextBox2.Text;
                db.Priority = _ucDemonbuddy.comboBox2.SelectedIndex;
                db.ForceEnableAllPlugins = _ucDemonbuddy.checkBox5.Checked;


                db.ManualPosSize = _ucDemonbuddy.checkBox4.Checked;
                int.TryParse(_ucDemonbuddy.textBox6.Text, out result);
                db.X = result;
                int.TryParse(_ucDemonbuddy.textBox5.Text, out result);
                db.Y = result;
                int.TryParse(_ucDemonbuddy.textBox10.Text, out result);
                db.W = result;
                int.TryParse(_ucDemonbuddy.textBox11.Text, out result);
                db.H = result;

                // Diablo
                d.Username = _ucDiablo.username.Text;
                d.Password = _ucDiablo.password.Text;
                d.Location = _ucDiablo.diablo3Path.Text;
                d.Language = _ucDiablo.language.SelectedItem.ToString();
                d.Region = _ucDiablo.region.SelectedItem.ToString();
                d.UseAuthenticator = _ucDiablo.checkBox1.Checked;
                d.Serial = string.Format("{0}-{1}-{2}-{3}", _ucDiablo.authField1.Text, _ucDiablo.authField2.Text,
                    _ucDiablo.authField3.Text, _ucDiablo.authField4.Text);
                d.Serial2 = string.Format("{0}{1}{2}{3}", _ucDiablo.authField1.Text, _ucDiablo.authField2.Text,
                    _ucDiablo.authField3.Text, _ucDiablo.authField4.Text);
                d.RestoreCode = _ucDiablo.textBox8.Text;
                d.Priority = _ucDiablo.processorAffinity.SelectedIndex;
                d.UseIsBoxer = _ucDiablo.useInnerSpace.Checked;
                d.ISBoxerLaunchCharacterSet = _ucDiablo.isBoxerLaunchAll.Checked;
                d.CharacterSet = _ucDiablo.characterSet.Text;
                d.DisplaySlot = _ucDiablo.displaySlot.Text;
                d.NoFrame = _ucDiablo.removeWindowFrame.Checked;

                // Affinity Diablo
                if (d.CpuCount != Environment.ProcessorCount)
                {
                    d.ProcessorAffinity = d.AllProcessors;
                    d.CpuCount = Environment.ProcessorCount;
                }

                if (AffinityDiablo.cpus.Count != d.CpuCount)
                {
                    Logger.Instance.Write(
                        "For whatever reason Diablo and UI see different number of CPUs, affinity disabled");
                }
                else
                {
                    int intProcessorAffinity = 0;
                    for (int i = 0; i < d.CpuCount; i++)
                    {
                        if (AffinityDiablo.cpus[i].Checked)
                            intProcessorAffinity |= (1 << i);
                    }
                    if (intProcessorAffinity == 0)
                        intProcessorAffinity = -1;
                    d.ProcessorAffinity = intProcessorAffinity;
                }
                if (AffinityDiablo != null)
                    AffinityDiablo.Dispose();

                // Affinity Demonbuddy
                if (db.CpuCount != Environment.ProcessorCount)
                {
                    db.ProcessorAffinity = db.AllProcessors;
                    db.CpuCount = Environment.ProcessorCount;
                }

                if (AffinityDemonbuddy.cpus.Count != db.CpuCount)
                {
                    Logger.Instance.Write(
                        "For whatever reason Demonbuddy and UI see different number of CPUs, affinity disabled");
                }
                else
                {
                    int intProcessorAffinity = 0;
                    for (int i = 0; i < db.CpuCount; i++)
                    {
                        if (AffinityDemonbuddy.cpus[i].Checked)
                            intProcessorAffinity |= (1 << i);
                    }
                    if (intProcessorAffinity == 0)
                        intProcessorAffinity = -1;
                    db.ProcessorAffinity = intProcessorAffinity;
                }
                if (AffinityDemonbuddy != null)
                    AffinityDemonbuddy.Dispose();

                d.ManualPosSize = _ucDiablo.manualPositionAndSize.Checked;
                if (d.ManualPosSize)
                {
                    int.TryParse(_ucDiablo.positionX.Text, out result);
                    d.X = result;
                    int.TryParse(_ucDiablo.positionY.Text, out result);
                    d.Y = result;
                    int.TryParse(_ucDiablo.width.Text, out result);
                    d.W = result;
                    int.TryParse(_ucDiablo.height.Text, out result);
                    d.H = result;
                }

                w.GenerateNewSchedule();
                w.Shuffle = _ucWeekSchedule.checkBox1.Checked;
                w.MinRandom = Convert.ToInt32(_ucWeekSchedule.textBox1.Text);
                w.MaxRandom = Convert.ToInt32(_ucWeekSchedule.textBox2.Text);

                ps.Profiles = _ucProfileSchedule.Profiles;
                ps.MaxRandomTime = Convert.ToInt32(_ucProfileSchedule.textBox1.Text);
                ps.MaxRandomRuns = Convert.ToInt32(_ucProfileSchedule.textBox2.Text);
                ps.Random = _ucProfileSchedule.checkBox1.Checked;

                b.Week = w;
                b.Demonbuddy = db;
                b.Diablo = d;
                b.ProfileSchedule = ps;


                if (bot != null && index >= 0)
                {
                    Logger.Instance.WriteGlobal("Editing bot: {0}", b.Name);

                    // Copy some important stuff from old bot

                    b.IsStarted = BotSettings.Instance.Bots[index].IsStarted;
                    b.IsEnabled = BotSettings.Instance.Bots[index].IsEnabled;
                    b.IsRunning = BotSettings.Instance.Bots[index].IsRunning;
                    b.Diablo.Proc = BotSettings.Instance.Bots[index].Diablo.Proc;
                    b.Demonbuddy.Proc = BotSettings.Instance.Bots[index].Demonbuddy.Proc;
                    b.Demonbuddy.MainWindowHandle = BotSettings.Instance.Bots[index].Demonbuddy.MainWindowHandle;
                    b.Diablo.MainWindowHandle = BotSettings.Instance.Bots[index].Diablo.MainWindowHandle;
                    b.AntiIdle = BotSettings.Instance.Bots[index].AntiIdle;
                    b.Week.ForceStart = BotSettings.Instance.Bots[index].Week.ForceStart;
                    b.RunningTime = BotSettings.Instance.Bots[index].RunningTime;

                    BotSettings.Instance.Bots[index] = b;
                }
                else
                {
                    Logger.Instance.WriteGlobal("Adding new bot: {0}", b.Name);
                    BotSettings.Instance.Bots.Add(b);
                }

                BotSettings.Instance.Save();
                shouldClose = true;
                ActiveForm.Close();

                BotSettings.Instance.Save();
                Program.Mainform.UpdateGridView();
                return;
            }

            if (ValidateControl(Controls[_stepCount]))
            {
                Controls[_stepCount].Visible = false; // Hide old
                _stepCount++;
                Controls[_stepCount].Visible = true; // Show new
            }

            if (_stepCount > _mainCount)
                buttonBack.Enabled = true;
            if (_stepCount == FinishCount)
                buttonNext.Text = "Save!";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // BACK

            Controls[_stepCount].Visible = false; // Hide old
            _stepCount--;
            Controls[_stepCount].Visible = true; // Show new
            if (_stepCount == _mainCount)
                buttonBack.Enabled = false;
            if (_stepCount < FinishCount)
                buttonNext.Text = "Next ->";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // CANCEL
            Close();
        }

        private void WizardMain_Closing(object sender, CancelEventArgs e)
        {
            if (!shouldClose &&
                MessageBox.Show("This will close the wizard without saving.\nAre you sure?", "Warning",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                e.Cancel = true;
        }

        #region Validate User Input

        private readonly Color _invalidColor = Color.FromArgb(255, 0, 0);
        private readonly Color _validColor = Color.White;

        private bool ValidateControl(object control)
        {
            if (control.GetType() == typeof (DemonbuddyOptions))
                return ((DemonbuddyOptions) control).ValidateInput();

            if (control.GetType() == typeof (DiabloOptions))
                return ((DiabloOptions) control).ValidateInput();

            if (control.GetType() == typeof (ProfileSchedule))
                return ((ProfileSchedule) control).ValidateInput();

            if (control.GetType() == typeof (WeekSchedule))
                return ((WeekSchedule) control).ValidateInput();

            // Else always return true
            return true;
        }

        public bool ValidateTextbox(TextBox test)
        {
            if (test.Text.Length == 0)
            {
                test.BackColor = _invalidColor;
                return false;
            }

            test.BackColor = _validColor;
            return true;
        }

        public bool ValidateMaskedTextbox(MaskedTextBox test)
        {
            if (test.Text.Length == 0)
            {
                test.BackColor = _invalidColor;
                return false;
            }

            test.BackColor = _validColor;
            return true;
        }

        #endregion
    }
}