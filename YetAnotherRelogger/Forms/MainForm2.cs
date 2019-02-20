using Serilog;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using YetAnotherRelogger.Forms.SettingsTree;
using YetAnotherRelogger.Forms.Wizard;
using YetAnotherRelogger.Helpers;
using YetAnotherRelogger.Helpers.Hotkeys;
using YetAnotherRelogger.Helpers.Tools;
using YetAnotherRelogger.Properties;
using AutoPosition = YetAnotherRelogger.Forms.SettingsTree.AutoPosition;
using ConnectionCheck = YetAnotherRelogger.Helpers.ConnectionCheck;
using General = YetAnotherRelogger.Forms.SettingsTree.General;
using ProfileKickstart = YetAnotherRelogger.Forms.SettingsTree.ProfileKickstart;

namespace YetAnotherRelogger.Forms
{
    public partial class MainForm2 : Form
    {
        private ILogger _logger;
        private bool _close;
        private Thread _restartBotsThread;
        private ContextMenu _menu;

        public MainForm2()
        {
            InitializeComponent();
            treeView1.NodeMouseClick += treeView1_NodeMouseClick;
        }

        private void MainForm2_Load(object sender, EventArgs e)
        {
            var version = Assembly.GetEntryAssembly().GetName().Version;
            var name = Assembly.GetEntryAssembly().GetName().Name;
#if DEBUG
            name += " - DEBUG";
#elif BETA
            name += " - BETA";
#endif
            name += $" v{version}";

            _logger = Logger.Instance.GetLogger<MainForm2>();
            LogUpdateTimer.Start();

            var screenMaxSize = new Point(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
            if (!CommandLineArgs.SafeMode)
            {
                // Set window location
                if (Settings.Default.WindowLocation != Point.Empty)
                {
                    if (Settings.Default.WindowLocation.X <= screenMaxSize.X && Settings.Default.WindowLocation.Y <= screenMaxSize.Y &&
                        Settings.Default.WindowLocation.Y >= 0 && Settings.Default.WindowLocation.Y >= 0)
                    {
                        Location = Settings.Default.WindowLocation;
                    }
                }

                // Set window size
                if (Settings.Default.WindowSize.Width >= 0 &&
                    Settings.Default.WindowSize.Height >= 0 &&
                    Settings.Default.WindowSize.Width <= screenMaxSize.X &&
                    Settings.Default.WindowSize.Height <= screenMaxSize.Y)
                {
                    Size = Settings.Default.WindowSize;
                }
                splitContainer1.SplitterDistance = Settings.Default.SplitterDistance;
            }

            Resize += MainForm2_Resize;
            
            Text = name;

            _logger.Information($"{name} started", version);
#if DEBUG
            _logger.Information("This is a DEBUG build of YetAnotherRelogger. This is meant for private use, do not share!");
            _logger.Information("This build may have bugs. Use at your own risk!");
#elif BETA
            _logger.Information("This is a BETA build of Demonbuddy. This is not meant for general usage. Please report any issues you may have in the beta thread on our forums.");
            _logger.Information("This build may have bugs. Use at your own risk!");
#endif
            // Check if we are run as admin
            if (!Program.IsRunAsAdmin)
                _logger.Warning("WE DON'T HAVE ADMIN RIGHTS!!");

            // Check if current application path is the same as last saved path
            // this is used for Windows autostart in a sutation where user moved/renamed the relogger
            if (Settings.Default.StartWithWindows && !Settings.Default.Location.Equals(Application.ExecutablePath))
            {
                _logger.Information("Application current path does not match last saved path. Updating registy key.");
                // Update to current location
                Settings.Default.Location = Application.ExecutablePath;
                // Update Regkey
                RegistryClass.WindowsAutoStartAdd();
            }

            // Set stuff for list of bots
            botGrid.DoubleBuffered(true);
            botGrid.AllowUserToAddRows = false;
            botGrid.MultiSelect = false;
            botGrid.MouseUp += dataGridView1_MouseUp;
            botGrid.CellValueChanged += dataGridView1_CellValueChanged;
            UpdateGridView();

            // OnClose
            Closing += MainForm2_Closing;

            // TrayIcon
            ToggleIcon();
            TrayIcon.Icon = Icon;
            TrayIcon.DoubleClick += TrayIcon_DoubleClick;
            _menu = new ContextMenu();
            _menu.MenuItems.Add(0, new MenuItem("Show", Show_Click));
            _menu.MenuItems.Add(1, new MenuItem("Hide", Hide_Click));
            _menu.MenuItems.Add(2, new MenuItem("Exit", Exit_Click));
            TrayIcon.ContextMenu = _menu;

            // Minimize on start
            if (Settings.Default.MinimizeOnStart)
            {
                WindowState = FormWindowState.Minimized;
                if (Settings.Default.MinimizeToTray)
                {
                    HideMe();
                    ToggleIcon();
                    ShowNotification("Yet Another Relogger", "Minimize on start");
                }
            }

            // Load global hotkeys
            GlobalHotkeys.Instance.Load();
        }

        protected void MainForm2_Closing(object sender, CancelEventArgs e)
        {
            if (!_close && Settings.Default.CloseToTray)
            {
                e.Cancel = true;
                HideMe();
                ToggleIcon();
                ShowNotification("Yet Another Relogger", "Is still running");
            }

            SaveWindowState();
        }

        private void MainForm2_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState && Settings.Default.MinimizeToTray)
            {
                ToggleIcon();
                ShowNotification("Yet Another Relogger", "Is still running");
                Hide();
            }
            SaveWindowState();
        }

        private void SaveWindowState()
        {
            // Copy window location to app settings
            Settings.Default.WindowLocation = Location;
            Settings.Default.SplitterDistance = splitContainer1.SplitterDistance;

            // Copy window size to app settings
            Settings.Default.WindowSize = WindowState == FormWindowState.Normal ? Size : RestoreBounds.Size;

            // Save settings
            Settings.Default.Save();
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == botGrid.Columns["isEnabled"].Index)
            {
                try
                {
                    if (e.RowIndex > 0 && e.RowIndex <= BotSettings.Instance.Bots.Count)
                    {
                        BotSettings.Instance.Bots[e.RowIndex].IsEnabled =
                               (bool)botGrid[e.ColumnIndex, e.RowIndex].Value;

                        // BotSettings.Instance.Save();
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Exception in dataGridView1_CellValueChanged");
                }
            }
        }

        private void dataGridView1_MouseUp(object sender, MouseEventArgs e)
        {
            var hitTestInfo = botGrid.HitTest(e.X, e.Y);
            if (hitTestInfo.Type == DataGridViewHitTestType.Cell)
            {
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(botGrid, new Point(e.X, e.Y));
                    SelectRow(hitTestInfo.RowIndex);
                }
                else if (e.Button == MouseButtons.Left)
                {
                    SelectRow(hitTestInfo.RowIndex);
                }
            }
        }

        private void SelectRow(int index)
        {
            foreach (DataGridViewRow row in botGrid.Rows)
                row.Selected = false;
            botGrid.Rows[index].Selected = true;
            botGrid.CurrentCell = botGrid.Rows[index].Cells[0];
        }

        public void UpdateGridView()
        {
            botGrid.DataSource = BotSettings.Instance.Bots;
            botGrid.Refresh();
            botGrid.Columns["week"].Visible = false;
            botGrid.Columns["demonbuddy"].Visible = false;
            botGrid.Columns["diablo"].Visible = false;
            botGrid.Columns["isRunning"].Visible = false;
            botGrid.Columns["isStarted"].Visible = false;
            botGrid.Columns["profileSchedule"].Visible = false;
            botGrid.Columns["AntiIdle"].Visible = false;
            botGrid.Columns["StartTime"].Visible = false;
            botGrid.Columns["UseWindowsUser"].Visible = false;
            botGrid.Columns["CreateWindowsUser"].Visible = false;
            botGrid.Columns["WindowsUserName"].Visible = false;
            botGrid.Columns["WindowsUserPassword"].Visible = false;
            botGrid.Columns["D3PrefsLocation"].Visible = false;
            botGrid.Columns["IsStandby"].Visible = false;
            botGrid.Columns["UseDiabloClone"].Visible = false;
            botGrid.Columns["DiabloCloneLocation"].Visible = false;
            botGrid.Columns["ChartStats"].Visible = false;

            botGrid.Columns["isEnabled"].DisplayIndex = 1;
            botGrid.Columns["isEnabled"].HeaderText = "Enabled";
            botGrid.Columns["isEnabled"].Width = 50;

            botGrid.Columns["Name"].DisplayIndex = 2;
            botGrid.Columns["Name"].ReadOnly = true;

            botGrid.Columns["Description"].DisplayIndex = 3;
            botGrid.Columns["Description"].Width = 200;
            botGrid.Columns["Description"].ReadOnly = true;

            botGrid.Columns["Status"].ReadOnly = true;


            foreach (DataGridViewRow row in botGrid.Rows)
            {
                row.HeaderCell.Value = $"{(row.Index + 1):00}";
            }

            botGrid.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
        }

        private void btnStartAll_click(object sender, EventArgs e)
        {
            //lock (BotSettings.Instance)
            //{
            ConnectionCheck.Reset();
            // Start All
            foreach (
                var row in
                    botGrid.Rows.Cast<DataGridViewRow>().Where(row => (bool)row.Cells["isEnabled"].Value))
            {
                BotSettings.Instance.Bots[row.Index].Start(checkBoxForce.Checked);
            }
            //}
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            lock (BotSettings.Instance)
            {
                // Open new bot wizard
                var wm = new WizardMain { TopMost = true };
                wm.ShowDialog();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            lock (BotSettings.Instance)
            {
                // Edit bot
                if (botGrid.CurrentRow == null || botGrid.CurrentRow.Index < 0)
                    return;
                var wm = new WizardMain(botGrid.CurrentRow.Index) { TopMost = true };

                wm.ShowDialog();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (
                MessageBox.Show(this, @"Are you sure you want to close Yet Another Relogger?",
                    @"Close Yet Another Relogger?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _close = true;
                Close();
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (richTextBox1.Lines.Length > 65535)
                richTextBox1.Clear();
            // scroll down
            richTextBox1.ScrollToCaret();
        }

        private void btnStopAll_Click(object sender, EventArgs e)
        {
            Relogger.Instance.Stop();
            // Stop All
            foreach (var bot in BotSettings.Instance.Bots)
            {
                bot.Stop();
            }
            Relogger.Instance.Start();
        }

        private void btnRestartAllDb_Click(object sender, EventArgs e)
        {
            DisableMainFormButtons();
            _restartBotsThread = new Thread(RestartAllBots) { IsBackground = true, Name = "RestartBotsThread" };
            _restartBotsThread.Start();
            btnRestartAllDb.Enabled = false;
        }

        private void RestartAllBots()
        {
            lock (BotSettings.Instance)
            {

                var runningBots = BotSettings.Instance.Bots.Where(b => b.IsRunning).ToList();

                if (runningBots.Any())
                {
                    Relogger.Instance.Stop();
                    foreach (var bot in runningBots)
                    {
                        var swKill = new Stopwatch();
                        swKill.Start();
                        bot.Demonbuddy.Stop();
                        var pid = Convert.ToInt32(bot.DemonbuddyPid);
                        if (Process.GetProcesses().All(p => p.Id != pid))
                            continue;
                        try
                        {
                            var p = Process.GetProcessById(pid);
                            while (!p.HasExited && swKill.ElapsedMilliseconds < 10000)
                            {
                                Thread.Sleep(10);
                            }
                        }
                        catch (Win32Exception) { Thread.Sleep(250); }
                        catch { Thread.Sleep(250); }
                    }
                    Relogger.Instance.Start();
                }
                EnableMainFormButtons();
            }

            btnRestartAllDb.BeginInvoke(new System.Action(() => btnRestartAllDb.Enabled = true));
            //Application.Current.Dispatcher.BeginInvoke(new System.Action(() => btnRestartAllDb.Enabled = true));

        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Start
            BotSettings.Instance.Bots[botGrid.CurrentRow.Index].Start();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Stop
            if (BotSettings.Instance.Bots[botGrid.CurrentRow.Index].IsStarted)
                BotSettings.Instance.Bots[botGrid.CurrentRow.Index].Stop();
        }

        private void statsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Bot Stats
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lock (BotSettings.Instance)
            {
                // Delete Bot
                if (
                    MessageBox.Show(@"Are you sure you want to delete this bot?", @"Delete bot", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    BotSettings.Instance.Bots.RemoveAt(botGrid.CurrentRow.Index);
                    BotSettings.Instance.Save();
                    UpdateGridView();
                }
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lock (BotSettings.Instance)
            {
                // Edit bot
                var wm = new WizardMain(botGrid.CurrentRow.Index) { TopMost = true };
                wm.ShowDialog();
            }
        }

        private void forceStartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lock (BotSettings.Instance)
            {
                // Force Start single bot
                BotSettings.Instance.Bots[botGrid.CurrentRow.Index].Start(true);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (Program.Pause)
            {
                Program.Pause = false;
                btnPause.Text = @"Pause";
            }
            else
            {
                Program.Pause = true;
                btnPause.Text = @"Unpause";
            }
        }

        protected override void WndProc(ref Message message)
        {
            // Show first instance form
            if (message.Msg == SingleInstance.WmShowfirstinstance)
            {
                Show();
                WinApi.ShowWindow(Handle, WinApi.WindowShowStyle.ShowNormal);
                WinApi.SetForegroundWindow(Handle);
            }
            base.WndProc(ref message);
        }

        private void pictureBoxDonate_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=K7KUXHUE9XUR4&lc=US&item_name=rrrix%20Demonbuddy%20development&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donate_SM%2egif%3aNonHosted");
        }

        private void btnClone_Click(object sender, EventArgs e)
        {
            DoClone();
        }

        private void cloneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoClone();
        }

        private void DoClone()
        {
            lock (BotSettings.Instance)
            {
                try
                {
                    // Clone bot
                    if (botGrid.CurrentRow == null || botGrid.CurrentRow.Index < 0)
                        return;

                    var idx = botGrid.SelectedRows[0].Index;

                    var newIdx = BotSettings.Instance.Clone(idx);
                    BotSettings.Instance.Save();

                    UpdateGridView();
                    botGrid.ClearSelection();

                    botGrid.CurrentCell = botGrid.Rows[newIdx].Cells[0];
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error cloning bot");
                }
            }
        }
        private void moveUpMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                lock (BotSettings.Instance)
                {
                    if (botGrid.CurrentRow == null || botGrid.CurrentRow.Index < 0)
                        return;

                    var idx = botGrid.SelectedRows[0].Index;

                    if (idx == 0)
                        return;

                    var newIdx = BotSettings.Instance.MoveUp(idx);
                    UpdateGridView();
                    botGrid.ClearSelection();
                    botGrid.CurrentCell = botGrid.Rows[newIdx].Cells[0];
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error moving bot up");
            }
            finally
            {
                BotSettings.Instance.Save();
            }
        }

        private void moveDownMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                lock (BotSettings.Instance)
                {
                    if (botGrid.CurrentRow == null || botGrid.CurrentRow.Index < 0)
                        return;

                    var idx = botGrid.SelectedRows[0].Index;

                    if (idx == botGrid.Rows.Count - 1)
                        return;

                    var newIdx = BotSettings.Instance.MoveDown(idx);
                    UpdateGridView();
                    botGrid.ClearSelection();
                    botGrid.CurrentCell = botGrid.Rows[newIdx].Cells[0];

                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error moving bot down");
            }
            finally
            {
                BotSettings.Instance.Save();
            }
        }

        private void btnOpenLog_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(Logger.Instance.LogDirectory);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unable to open log directory {LogDirectory}", Logger.Instance.LogDirectory);
            }
        }

        public void DisableMainFormButtons()
        {
            btnClone.Enabled = false;
            btnClose.Enabled = false;
            btnEdit.Enabled = false;
            btnNew.Enabled = false;
            btnOpenLog.Enabled = false;
            btnPause.Enabled = false;
            btnStartAll.Enabled = false;
            btnStopAll.Enabled = false;

            botGrid.Enabled = false;
            contextMenuStrip1.Enabled = false;
        }

        public void EnableMainFormButtons()
        {
            btnClone.Enabled = true;
            btnClose.Enabled = true;
            btnEdit.Enabled = true;
            btnNew.Enabled = true;
            btnOpenLog.Enabled = true;
            btnPause.Enabled = true;
            btnStartAll.Enabled = true;
            btnStopAll.Enabled = true;

            botGrid.Enabled = true;
            contextMenuStrip1.Enabled = true;
        }

        #region Settings Tree

        public UserControl UcSetting = new UserControl(); // holds current settings user control

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            var tmp = new UserControl();
            switch (e.Node.Name)
            {
                case "General": // General
                    tmp = new General();
                    break;
                case "AutoPos": // Auto postion
                    tmp = new AutoPosition();
                    break;
                case "PingCheck":
                case "ConnectionCheck":
                    tmp = new SettingsTree.ConnectionCheck();
                    break;
                case "IpHostCheck":
                    tmp = new IpHostCheck();
                    break;
                case "AntiIdle":
                    tmp = new AntiIdle();
                    break;
                case "ProfileKickstart":
                    tmp = new ProfileKickstart();
                    break;
                case "HotKeys":
                    tmp = new HotKeys();
                    break;
                case "Stats":
                    tmp = new Stats();
                    break;
            }

            // Check if new user control should be displayed
            if (!tmp.Name.Equals(UcSetting.Name))
            {
                //var c = tabControl1.TabPages[1].Controls;
                var c = SettingsPanel.Controls;
                if (c.Contains(UcSetting))
                    c.Remove(UcSetting);

                UcSetting = tmp;
                //_ucSetting.Left = 180;
                c.Add(UcSetting);
            }
        }
        #endregion

        #region Tray Icon
        public void ShowNotification(string title, string msg, ToolTipIcon icon = ToolTipIcon.None)
        {
            if (!Settings.Default.ShowNotification || !TrayIcon.Visible)
                return;
            TrayIcon.ShowBalloonTip(500, title, msg, icon);
        }

        public void ToggleIcon()
        {
            TrayIcon.Visible = (Settings.Default.AlwaysShowTray ||
                                (!Visible || WindowState == FormWindowState.Minimized));
        }

        protected void Exit_Click(object sender, EventArgs e)
        {
            _close = true;
            Close();
        }

        protected void Hide_Click(object sender, EventArgs e)
        {
            ToggleIcon();
            ShowNotification("Yet Another Relogger", "Is still running");
            HideMe();
        }

        protected void Show_Click(object sender, EventArgs e)
        {
            ShowMe();
            WinApi.ShowWindow(Handle, WinApi.WindowShowStyle.ShowNormal);
            ToggleIcon();
        }

        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            ShowMe();
            WinApi.ShowWindow(Handle, WinApi.WindowShowStyle.ShowNormal);
            ToggleIcon();
        }

        private void ShowMe()
        {
            ShowInTaskbar = true;
            Visible = true;
            Show();
        }

        private void HideMe()
        {
            ShowInTaskbar = false;
            Visible = false;
            Hide();
        }
        #endregion

        private void killDemonbuddyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (BotSettings.Instance.Bots[botGrid.CurrentRow.Index].IsStarted)
                BotSettings.Instance.Bots[botGrid.CurrentRow.Index].KillDemonbuddy();
        }

        private void killDiabloToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (BotSettings.Instance.Bots[botGrid.CurrentRow.Index].IsStarted)
                BotSettings.Instance.Bots[botGrid.CurrentRow.Index].KillDiablo();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }
    }
}
