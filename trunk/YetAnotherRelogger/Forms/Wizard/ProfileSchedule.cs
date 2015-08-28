using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using YetAnotherRelogger.Helpers;
using YetAnotherRelogger.Helpers.Bot;
using YetAnotherRelogger.Helpers.Enums;
using YetAnotherRelogger.Helpers.Tools;

namespace YetAnotherRelogger.Forms.Wizard
{
    public partial class ProfileSchedule : UserControl
    {
        private readonly WizardMain WM;

        public ProfileSchedule(WizardMain parent)
        {

            WM = parent;
            InitializeComponent();

            var col = new DataGridViewComboBoxColumn
            {
                Name = "Difficulty",
                DataSource = Enum.GetValues(typeof(Difficulty)),
                ValueType = typeof(Difficulty),
            };
            profileGrid.Columns.Add(col);

            profileGrid.CellClick += profileGrid_CellClick;
            profileGrid.CellValueChanged += profileGrid_CellValueChanged;
            profileGrid.DoubleBuffered(true);

            this.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
        }

        public BindingList<Profile> Profiles { get; set; }

        private void profileGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (profileGrid.Columns[e.ColumnIndex].Name.Equals("Difficulty"))
            {
                Profiles[e.RowIndex].DifficultyLevel =
                    (Difficulty)profileGrid.Rows[e.RowIndex].Cells["Difficulty"].Value;
            }
        }

        private void profileGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && profileGrid.Rows[e.RowIndex].IsNewRow)
            {
                var ofd = new OpenFileDialog {Filter = "Profile file|*.xml", Title = "Browse to profile"};
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    //dataGridView1.Rows.Add(Path.GetFileName(ofd.FileName), ofd.FileName, 0, 0);
                    var p = new Profile
                    {
                        Location = ofd.FileName,
                        Name = Path.GetFileName(ofd.FileName),
                        Runs = 0,
                        Minutes = 0,
                        DifficultyLevel = Difficulty.Disabled,
                        GoldTimer = 0,
                    };
                    profileGrid.DataSource = null;
                    Profiles.Add(p);
                    profileGrid.DataSource = Profiles;
                    profileGrid.Columns["isDone"].Visible = false;
                    UpdateGridview();
                }
            }
        }

        private void ProfileSchedule_Load(object sender, EventArgs e)
        {
            VisibleChanged += ProfileSchedule_VisibleChanged;
            textBox1.KeyPress += NumericCheck;
            textBox2.KeyPress += NumericCheck;
            comboBox1.SelectedItem = "** Global **";
        }

        private void NumericCheck(object sender, KeyPressEventArgs e)
        {
            e.Handled = General.NumericOnly(e.KeyChar);
        }

        private void ProfileSchedule_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                WM.NextStep("Profile Settings");
                UpdateGridview();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Delete record
            if (profileGrid.CurrentRow.IsNewRow)
            {
                return;
            }
            if (
                MessageBox.Show("Are you sure you want to delete this profile from your schedule?",
                    "Delete profile from schedule", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                DialogResult.Yes)
            {
                profileGrid.Rows.Remove(profileGrid.CurrentRow);
            }
        }

        private void dataGridView1_MouseUp(object sender, MouseEventArgs e)
        {
            DataGridView.HitTestInfo hitTestInfo = profileGrid.HitTest(e.X, e.Y);
            if (!profileGrid.CurrentRow.IsNewRow && hitTestInfo.Type == DataGridViewHitTestType.Cell)
            {
                if (e.Button == MouseButtons.Right)
                    contextMenuStrip1.Show(profileGrid, new Point(e.X, e.Y));
            }
        }

        public bool ValidateInput()
        {
            return true;
        }

        public void UpdateGridview()
        {
            if (Profiles == null)
                Profiles = new BindingList<Profile>();

            profileGrid.DataSource = Profiles;
            profileGrid.Refresh();
            profileGrid.ReadOnly = false;
            profileGrid.Columns["isDone"].Visible = false;
            profileGrid.Columns["DifficultyLevel"].Visible = false;

            // GameDifficulty
            for (int i = 0; i < Profiles.Count; i++)
            {
                Difficulty pl = Profiles[i].DifficultyLevel;
                profileGrid.Rows[i].Cells["Difficulty"].Value = pl;
            }
        }
    }
}