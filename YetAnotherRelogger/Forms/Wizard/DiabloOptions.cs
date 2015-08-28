using System;
using System.IO;
using System.Windows.Forms;
using YetAnotherRelogger.Helpers.Tools;
using YetAnotherRelogger.Properties;

namespace YetAnotherRelogger.Forms.Wizard
{
    public partial class DiabloOptions : UserControl
    {
        private readonly WizardMain WM;

        public DiabloOptions(WizardMain parent)
        {
            WM = parent;
            InitializeComponent();
            this.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
        }

        private void DiabloOptions_Load(object sender, EventArgs e)
        {
            VisibleChanged += DiabloOptions_VisibleChanged;
            positionX.KeyPress += NumericCheck;
            positionY.KeyPress += NumericCheck;
            width.KeyPress += NumericCheck;
            height.KeyPress += NumericCheck;
            displaySlot.KeyPress += NumericCheck;
            processorAffinity.SelectedIndex = 2;
            language.SelectedIndex = 0;
            region.SelectedIndex = 1;
        }

        private void NumericCheck(object sender, KeyPressEventArgs e)
        {
            e.Handled = General.NumericOnly(e.KeyChar);
        }

        private void DiabloOptions_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
                WM.NextStep("Diablo Settings");
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                label5.Enabled = label6.Enabled = label7.Enabled = label8.Enabled = label9.Enabled = true;
                authField1.Enabled = authField2.Enabled = authField3.Enabled = authField4.Enabled = textBox8.Enabled = true;
                authenticatorTestButton.Enabled = authenticatorClearButton.Enabled = true;
            }
            else
            {
                label5.Enabled = label6.Enabled = label7.Enabled = label8.Enabled = label9.Enabled = false;
                authField1.Enabled = authField2.Enabled = authField3.Enabled = authField4.Enabled = textBox8.Enabled = false;
                authenticatorTestButton.Enabled = authenticatorClearButton.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "Diablo III.exe|*.exe",
                FileName = "Diablo III.exe",
                Title = "Browse to Diablo III.exe"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
                diablo3Path.Text = ofd.FileName;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            label13.Enabled =
                label14.Enabled =
                    label15.Enabled =
                        label16.Enabled =
                            positionX.Enabled =
                                positionY.Enabled = width.Enabled = height.Enabled = manualPositionAndSize.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            labelISCharacterSetName.Enabled = labelISCharacterSetSlot.Enabled = displaySlot.Enabled = characterSet.Enabled = isBoxerLaunchAll.Enabled = useInnerSpace.Checked;
            if (useInnerSpace.Checked &&
                (string.IsNullOrEmpty(Settings.Default.ISBoxerPath) || !File.Exists(Settings.Default.ISBoxerPath)))
            {
                // Locate Inner space
                var ofd = new OpenFileDialog
                {
                    Filter = "Inner Space.exe|*.exe",
                    FileName = "Inner Space.exe",
                    Title = "Browse to Inner Space.exe"
                };
                if (ofd.ShowDialog() == DialogResult.OK)
                    Settings.Default.ISBoxerPath = ofd.FileName;
                ofd.Dispose();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            WM.AffinityDiablo.ShowDialog(this);
        }

        public bool ValidateInput()
        {
            return (WM.ValidateTextbox(username) &
                    WM.ValidateTextbox(diablo3Path) &
                    WM.ValidateMaskedTextbox(password) &
                    (!useInnerSpace.Checked || (WM.ValidateTextbox(displaySlot) & WM.ValidateTextbox(characterSet)))
                );
        }

        private void button5_Click(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void authenticatorTestButton_Click(object sender, EventArgs e)
        {

            // restore the authenticator
            try
            {
                BattleNetAuthenticator auth = new BattleNetAuthenticator();
                auth.Restore(string.Format("{0}{1}{2}{3}", authField1.Text, authField2.Text,
                    authField3.Text, authField4.Text), textBox8.Text);

                CodeField.Text = Convert.ToString(auth.CurrentCode);
            }
            catch (InvalidRestoreResponseException re)
            {
                MessageBox.Show(this, re.Message, "YAR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }
    }
}