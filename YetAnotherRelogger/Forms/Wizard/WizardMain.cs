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
        private readonly BotClass _bot;
        public SetAffinity AffinityDemonbuddy;
        public SetAffinity AffinityDiablo;
        public int FinishCount;
        private int _mainCount;
        private int _stepCount;
        private Advanced _ucAdvanced;
        private DemonbuddyOptions _ucDemonbuddy;
        public DiabloOptions UcDiablo;
        private Heroes _ucHeroes;
        private ProfileSchedule _ucProfileSchedule;
        private WeekSchedule _ucWeekSchedule;
        private int _index = -1;
        private bool _shouldClose;

        public WizardMain()
        {
            InitializeComponent();
        }

        public WizardMain(int index)
        {
            this._index = index;
            _bot = BotSettings.Instance.Bots[index];
            InitializeComponent();
        }

        public void NextStep(string title)
        {
            Text = $"{title} (Step {_stepCount - 2}/{FinishCount - 2})";
        }

        private void WizardMain_Load(object sender, EventArgs e)
        {
            Closing += WizardMain_Closing;

            _mainCount = Controls.Count;
            _stepCount = _mainCount; // set start point

            _ucDemonbuddy = new DemonbuddyOptions(this);
            UcDiablo = new DiabloOptions(this);
            _ucWeekSchedule = new WeekSchedule(this);
            _ucHeroes = new Heroes(this);
            _ucProfileSchedule = new ProfileSchedule(this);
            _ucAdvanced = new Advanced(this);


            Controls.Add(_ucDemonbuddy);
            Controls.Add(UcDiablo);
            Controls.Add(_ucWeekSchedule);
            //Controls.Add(ucHeroes);
            Controls.Add(_ucProfileSchedule);
            Controls.Add(_ucAdvanced);
            UcDiablo.Visible =
                _ucWeekSchedule.Visible = _ucProfileSchedule.Visible = _ucHeroes.Visible = _ucAdvanced.Visible = false;

            FinishCount = Controls.Count - 1; // Get Finish count

            AffinityDiablo = new SetAffinity();
            AffinityDemonbuddy = new SetAffinity();

            if (_bot != null)
                LoadData();
        }

        private void LoadData()
        {
            // Load data
            _ucDemonbuddy.textBox1.Text = _bot.Name;
            _ucDemonbuddy.textBox2.Text = _bot.Description;

            // Advanced section
            _ucAdvanced.checkBox2.Checked = _bot.CreateWindowsUser;
            _ucAdvanced.checkBox1.Checked = _bot.UseWindowsUser;
            _ucAdvanced.textBox1.Text = _bot.WindowsUserName;
            _ucAdvanced.maskedTextBox1.Text = _bot.WindowsUserPassword;
            _ucAdvanced.textBox3.Text = _bot.D3PrefsLocation;
            _ucAdvanced.checkBox3.Checked = _bot.UseDiabloClone;
            _ucAdvanced.textBox2.Text = _bot.DiabloCloneLocation;

            // Demonbuddy
            _ucDemonbuddy.textBox4.Text = _bot.Demonbuddy.Location;
            _ucDemonbuddy.textBox3.Text = _bot.Demonbuddy.Key;

            _ucDemonbuddy.comboBox1.Text = _bot.Demonbuddy.CombatRoutine;
            _ucDemonbuddy.checkBox1.Checked = _bot.Demonbuddy.NoFlash;
            _ucDemonbuddy.checkBox2.Checked = _bot.Demonbuddy.AutoUpdate;
            _ucDemonbuddy.checkBox3.Checked = _bot.Demonbuddy.NoUpdate;
            _ucDemonbuddy.textBox9.Text = _bot.Demonbuddy.BuddyAuthUsername;
            _ucDemonbuddy.maskedTextBox2.Text = _bot.Demonbuddy.BuddyAuthPassword;
            _ucDemonbuddy.comboBox2.SelectedIndex = _bot.Demonbuddy.Priority;
            _ucDemonbuddy.checkBox5.Checked = _bot.Demonbuddy.ForceEnableAllPlugins;
            // Demonbuddy manual position
            _ucDemonbuddy.checkBox4.Checked = _bot.Demonbuddy.ManualPosSize;
            _ucDemonbuddy.textBox6.Text = _bot.Demonbuddy.X.ToString();
            _ucDemonbuddy.textBox5.Text = _bot.Demonbuddy.Y.ToString();
            _ucDemonbuddy.textBox10.Text = _bot.Demonbuddy.W.ToString();
            _ucDemonbuddy.textBox11.Text = _bot.Demonbuddy.H.ToString();

            // Diablo
            UcDiablo.username.Text = _bot.Diablo.Username;
            UcDiablo.password.Text = _bot.Diablo.Password;
            UcDiablo.diablo3Path.Text = _bot.Diablo.Location;
            UcDiablo.language.SelectedItem = _bot.Diablo.Language;
            UcDiablo.region.SelectedItem = _bot.Diablo.Region;
            UcDiablo.checkBox1.Checked = _bot.Diablo.UseAuthenticator;
            UcDiablo.useInnerSpace.Checked = _bot.Diablo.UseIsBoxer;
            UcDiablo.isBoxerLaunchAll.Checked = _bot.Diablo.IsBoxerLaunchCharacterSet;
            UcDiablo.characterSet.Text = _bot.Diablo.CharacterSet;
            UcDiablo.displaySlot.Text = _bot.Diablo.DisplaySlot;
            UcDiablo.removeWindowFrame.Checked = _bot.Diablo.NoFrame;

            // Affinity Diablo
            if (_bot.Diablo.CpuCount != Environment.ProcessorCount)
            {
                _bot.Diablo.ProcessorAffinity = _bot.Diablo.AllProcessors;
                _bot.Diablo.CpuCount = Environment.ProcessorCount;
            }

            if (AffinityDiablo.Cpus.Count != _bot.Diablo.CpuCount)
            {
                Logger.Instance.Write(
                    "For whatever reason Diablo and UI see different number of CPUs, affinity disabled");
            }
            else
            {
                for (var i = 0; i < _bot.Diablo.CpuCount; i++)
                {
                    AffinityDiablo.Cpus[i].Checked = ((_bot.Diablo.ProcessorAffinity & (1 << i)) != 0);
                }
            }
            // Affinity Demonbuddy
            if (_bot.Demonbuddy.CpuCount != Environment.ProcessorCount)
            {
                _bot.Demonbuddy.ProcessorAffinity = _bot.Demonbuddy.AllProcessors;
                _bot.Demonbuddy.CpuCount = Environment.ProcessorCount;
            }

            if (AffinityDemonbuddy.Cpus.Count != _bot.Demonbuddy.CpuCount)
            {
                Logger.Instance.Write(
                    "For whatever reason Demonbuddy and UI see different number of CPUs, affinity disabled");
            }
            else
            {
                for (var i = 0; i < _bot.Demonbuddy.CpuCount; i++)
                {
                    AffinityDemonbuddy.Cpus[i].Checked = ((_bot.Demonbuddy.ProcessorAffinity & (1 << i)) != 0);
                }
            }

            //!!!d.Serial = string.Format("{0}-{1}-{2}-{3}", ucDiablo.textBox4.Text, ucDiablo.textBox5.Text, ucDiablo.textBox7.Text, ucDiablo.textBox6.Text);
            //!!!ucDiablo.textBox8.Text = bot.diablo.RestoreCode;

            /*
             d.Serial = string.Format("{0}-{1}-{2}-{3}", _ucDiablo.textBox4.Text, _ucDiablo.textBox5.Text,
                    _ucDiablo.textBox7.Text, _ucDiablo.textBox6.Text);
                d.RestoreCode = _ucDiablo.textBox8.Text;
             */

            var serialCode = _bot.Diablo.Serial;
            string[] words;
            words = serialCode.Split('-');

            UcDiablo.authField1.Text = words[0];
            UcDiablo.authField2.Text = words[1];
            UcDiablo.authField3.Text = words[2];
            UcDiablo.authField4.Text = words[3];
            UcDiablo.textBox8.Text = _bot.Diablo.RestoreCode;


            UcDiablo.processorAffinity.SelectedIndex = _bot.Diablo.Priority;

            // Diablo manual position
            UcDiablo.manualPositionAndSize.Checked = _bot.Diablo.ManualPosSize;
            UcDiablo.positionX.Text = _bot.Diablo.X.ToString();
            UcDiablo.positionY.Text = _bot.Diablo.Y.ToString();
            UcDiablo.width.Text = _bot.Diablo.W.ToString();
            UcDiablo.height.Text = _bot.Diablo.H.ToString();

            // Profile Schedule
            _ucProfileSchedule.Profiles = _bot.ProfileSchedule.Profiles;
            _ucProfileSchedule.textBox1.Text = _bot.ProfileSchedule.MaxRandomTime.ToString();
            _ucProfileSchedule.textBox2.Text = _bot.ProfileSchedule.MaxRandomRuns.ToString();
            _ucProfileSchedule.checkBox1.Checked = _bot.ProfileSchedule.Random;

            // Load Weekschedule
            _ucWeekSchedule.textBox1.Text = _bot.Week.MinRandom.ToString();
            _ucWeekSchedule.textBox2.Text = _bot.Week.MaxRandom.ToString();
            _ucWeekSchedule.checkBox1.Checked = _bot.Week.Shuffle;
            _ucWeekSchedule.LoadSchedule(_bot);
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
                d.Username = UcDiablo.username.Text;
                d.Password = UcDiablo.password.Text;
                d.Location = UcDiablo.diablo3Path.Text;
                d.Language = UcDiablo.language.SelectedItem.ToString();
                d.Region = UcDiablo.region.SelectedItem.ToString();
                d.UseAuthenticator = UcDiablo.checkBox1.Checked;
                d.Serial =
                    $"{UcDiablo.authField1.Text}-{UcDiablo.authField2.Text}-{UcDiablo.authField3.Text}-{UcDiablo.authField4.Text}";
                d.Serial2 =
                    $"{UcDiablo.authField1.Text}{UcDiablo.authField2.Text}{UcDiablo.authField3.Text}{UcDiablo.authField4.Text}";
                d.RestoreCode = UcDiablo.textBox8.Text;
                d.Priority = UcDiablo.processorAffinity.SelectedIndex;
                d.UseIsBoxer = UcDiablo.useInnerSpace.Checked;
                d.IsBoxerLaunchCharacterSet = UcDiablo.isBoxerLaunchAll.Checked;
                d.CharacterSet = UcDiablo.characterSet.Text;
                d.DisplaySlot = UcDiablo.displaySlot.Text;
                d.NoFrame = UcDiablo.removeWindowFrame.Checked;

                // Affinity Diablo
                if (d.CpuCount != Environment.ProcessorCount)
                {
                    d.ProcessorAffinity = d.AllProcessors;
                    d.CpuCount = Environment.ProcessorCount;
                }

                if (AffinityDiablo.Cpus.Count != d.CpuCount)
                {
                    Logger.Instance.Write(
                        "For whatever reason Diablo and UI see different number of CPUs, affinity disabled");
                }
                else
                {
                    var intProcessorAffinity = 0;
                    for (var i = 0; i < d.CpuCount; i++)
                    {
                        if (AffinityDiablo.Cpus[i].Checked)
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

                if (AffinityDemonbuddy.Cpus.Count != db.CpuCount)
                {
                    Logger.Instance.Write(
                        "For whatever reason Demonbuddy and UI see different number of CPUs, affinity disabled");
                }
                else
                {
                    var intProcessorAffinity = 0;
                    for (var i = 0; i < db.CpuCount; i++)
                    {
                        if (AffinityDemonbuddy.Cpus[i].Checked)
                            intProcessorAffinity |= (1 << i);
                    }
                    if (intProcessorAffinity == 0)
                        intProcessorAffinity = -1;
                    db.ProcessorAffinity = intProcessorAffinity;
                }
                if (AffinityDemonbuddy != null)
                    AffinityDemonbuddy.Dispose();

                d.ManualPosSize = UcDiablo.manualPositionAndSize.Checked;
                if (d.ManualPosSize)
                {
                    int.TryParse(UcDiablo.positionX.Text, out result);
                    d.X = result;
                    int.TryParse(UcDiablo.positionY.Text, out result);
                    d.Y = result;
                    int.TryParse(UcDiablo.width.Text, out result);
                    d.W = result;
                    int.TryParse(UcDiablo.height.Text, out result);
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


                if (_bot != null && _index >= 0)
                {
                    Logger.Instance.WriteGlobal("Editing bot: {0}", b.Name);

                    // Copy some important stuff from old bot

                    b.IsStarted = BotSettings.Instance.Bots[_index].IsStarted;
                    b.IsEnabled = BotSettings.Instance.Bots[_index].IsEnabled;
                    b.IsRunning = BotSettings.Instance.Bots[_index].IsRunning;
                    b.Diablo.Proc = BotSettings.Instance.Bots[_index].Diablo.Proc;
                    b.Demonbuddy.Proc = BotSettings.Instance.Bots[_index].Demonbuddy.Proc;
                    b.Demonbuddy.MainWindowHandle = BotSettings.Instance.Bots[_index].Demonbuddy.MainWindowHandle;
                    b.Diablo.MainWindowHandle = BotSettings.Instance.Bots[_index].Diablo.MainWindowHandle;
                    b.AntiIdle = BotSettings.Instance.Bots[_index].AntiIdle;
                    b.Week.ForceStart = BotSettings.Instance.Bots[_index].Week.ForceStart;
                    b.RunningTime = BotSettings.Instance.Bots[_index].RunningTime;

                    BotSettings.Instance.Bots[_index] = b;
                }
                else
                {
                    Logger.Instance.WriteGlobal("Adding new bot: {0}", b.Name);
                    BotSettings.Instance.Bots.Add(b);
                }

                BotSettings.Instance.Save();
                _shouldClose = true;
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
            if (!_shouldClose &&
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