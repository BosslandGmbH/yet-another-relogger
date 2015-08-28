namespace YetAnotherRelogger.Forms.SettingsTree
{
    partial class AntiIdle
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelAntiIdleStats2 = new System.Windows.Forms.Label();
            this.labelAntiIdleStats1 = new System.Windows.Forms.Label();
            this.antiIdleStats = new System.Windows.Forms.NumericUpDown();
            this.cbAllowPulseFix = new System.Windows.Forms.CheckBox();
            this.cbAllowKillDB = new System.Windows.Forms.CheckBox();
            this.cbAllowKillGame = new System.Windows.Forms.CheckBox();
            this.cbStartBotIfStopped = new System.Windows.Forms.CheckBox();
            this.cbLogGoldInactivityInfo = new System.Windows.Forms.CheckBox();
            this.checkboxGoldTimer = new System.Windows.Forms.CheckBox();
            this.goldTimerLabel2 = new System.Windows.Forms.Label();
            this.goldTimerLabel1 = new System.Windows.Forms.Label();
            this.goldTimerMaxDuration = new System.Windows.Forms.NumericUpDown();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.antiIdleStats)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.goldTimerMaxDuration)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelAntiIdleStats2);
            this.groupBox1.Controls.Add(this.labelAntiIdleStats1);
            this.groupBox1.Controls.Add(this.antiIdleStats);
            this.groupBox1.Controls.Add(this.cbAllowPulseFix);
            this.groupBox1.Controls.Add(this.cbAllowKillDB);
            this.groupBox1.Controls.Add(this.cbAllowKillGame);
            this.groupBox1.Controls.Add(this.cbStartBotIfStopped);
            this.groupBox1.Controls.Add(this.cbLogGoldInactivityInfo);
            this.groupBox1.Controls.Add(this.checkboxGoldTimer);
            this.groupBox1.Controls.Add(this.goldTimerLabel2);
            this.groupBox1.Controls.Add(this.goldTimerLabel1);
            this.groupBox1.Controls.Add(this.goldTimerMaxDuration);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(449, 211);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "AntiIdle";
            // 
            // labelAntiIdleStats2
            // 
            this.labelAntiIdleStats2.AutoSize = true;
            this.labelAntiIdleStats2.Location = new System.Drawing.Point(233, 183);
            this.labelAntiIdleStats2.Name = "labelAntiIdleStats2";
            this.labelAntiIdleStats2.Size = new System.Drawing.Size(27, 13);
            this.labelAntiIdleStats2.TabIndex = 11;
            this.labelAntiIdleStats2.Text = "sec.";
            // 
            // labelAntiIdleStats1
            // 
            this.labelAntiIdleStats1.AutoSize = true;
            this.labelAntiIdleStats1.Location = new System.Drawing.Point(6, 183);
            this.labelAntiIdleStats1.Name = "labelAntiIdleStats1";
            this.labelAntiIdleStats1.Size = new System.Drawing.Size(155, 13);
            this.labelAntiIdleStats1.TabIndex = 10;
            this.labelAntiIdleStats1.Text = "Plugin Communication Timeout ";
            this.toolTip1.SetToolTip(this.labelAntiIdleStats1, "If gold does not change in this time, AntiIdle actions will be taken.");
            // 
            // antiIdleStats
            // 
            this.antiIdleStats.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::YetAnotherRelogger.Properties.Settings.Default, "AntiIdleStatsDuration", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.antiIdleStats.Increment = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.antiIdleStats.Location = new System.Drawing.Point(167, 181);
            this.antiIdleStats.Maximum = new decimal(new int[] {
            86400,
            0,
            0,
            0});
            this.antiIdleStats.Name = "antiIdleStats";
            this.antiIdleStats.Size = new System.Drawing.Size(60, 20);
            this.antiIdleStats.TabIndex = 9;
            this.toolTip1.SetToolTip(this.antiIdleStats, "Maximum timeout of bot stats being received from DB");
            this.antiIdleStats.Value = global::YetAnotherRelogger.Properties.Settings.Default.AntiIdleStatsDuration;
            // 
            // cbAllowPulseFix
            // 
            this.cbAllowPulseFix.AutoSize = true;
            this.cbAllowPulseFix.Checked = global::YetAnotherRelogger.Properties.Settings.Default.AllowPulseFix;
            this.cbAllowPulseFix.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::YetAnotherRelogger.Properties.Settings.Default, "AllowPulseFix", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cbAllowPulseFix.Location = new System.Drawing.Point(6, 89);
            this.cbAllowPulseFix.Name = "cbAllowPulseFix";
            this.cbAllowPulseFix.Size = new System.Drawing.Size(106, 17);
            this.cbAllowPulseFix.TabIndex = 8;
            this.cbAllowPulseFix.Text = "Allow \"Pulse Fix\"";
            this.toolTip1.SetToolTip(this.cbAllowPulseFix, "If gold does not change in this time, AntiIdle actions will be taken.");
            this.cbAllowPulseFix.UseVisualStyleBackColor = true;
            // 
            // cbAllowKillDB
            // 
            this.cbAllowKillDB.AutoSize = true;
            this.cbAllowKillDB.Checked = global::YetAnotherRelogger.Properties.Settings.Default.AllowKillDemonbuddy;
            this.cbAllowKillDB.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::YetAnotherRelogger.Properties.Settings.Default, "AllowKillDemonbuddy", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cbAllowKillDB.Location = new System.Drawing.Point(6, 158);
            this.cbAllowKillDB.Name = "cbAllowKillDB";
            this.cbAllowKillDB.Size = new System.Drawing.Size(223, 17);
            this.cbAllowKillDB.TabIndex = 7;
            this.cbAllowKillDB.Text = "Allow Killing Demonbuddy if Unresponsive";
            this.toolTip1.SetToolTip(this.cbAllowKillDB, "If gold does not change in this time, AntiIdle actions will be taken.");
            this.cbAllowKillDB.UseVisualStyleBackColor = true;
            // 
            // cbAllowKillGame
            // 
            this.cbAllowKillGame.AutoSize = true;
            this.cbAllowKillGame.Checked = global::YetAnotherRelogger.Properties.Settings.Default.AllowKillGame;
            this.cbAllowKillGame.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::YetAnotherRelogger.Properties.Settings.Default, "AllowKillGame", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cbAllowKillGame.Location = new System.Drawing.Point(6, 135);
            this.cbAllowKillGame.Name = "cbAllowKillGame";
            this.cbAllowKillGame.Size = new System.Drawing.Size(190, 17);
            this.cbAllowKillGame.TabIndex = 6;
            this.cbAllowKillGame.Text = "Allow Killing Diablo if Unresponsive";
            this.toolTip1.SetToolTip(this.cbAllowKillGame, "If gold does not change in this time, AntiIdle actions will be taken.");
            this.cbAllowKillGame.UseVisualStyleBackColor = true;
            // 
            // cbStartBotIfStopped
            // 
            this.cbStartBotIfStopped.AutoSize = true;
            this.cbStartBotIfStopped.Checked = global::YetAnotherRelogger.Properties.Settings.Default.StartBotIfStopped;
            this.cbStartBotIfStopped.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::YetAnotherRelogger.Properties.Settings.Default, "StartBotIfStopped", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cbStartBotIfStopped.Location = new System.Drawing.Point(6, 112);
            this.cbStartBotIfStopped.Name = "cbStartBotIfStopped";
            this.cbStartBotIfStopped.Size = new System.Drawing.Size(173, 17);
            this.cbStartBotIfStopped.TabIndex = 5;
            this.cbStartBotIfStopped.Text = "Start bot if Stopped for too long";
            this.toolTip1.SetToolTip(this.cbStartBotIfStopped, "If gold does not change in this time, AntiIdle actions will be taken.");
            this.cbStartBotIfStopped.UseVisualStyleBackColor = true;
            // 
            // cbLogGoldInactivityInfo
            // 
            this.cbLogGoldInactivityInfo.AutoSize = true;
            this.cbLogGoldInactivityInfo.Checked = global::YetAnotherRelogger.Properties.Settings.Default.GoldInfoLogging;
            this.cbLogGoldInactivityInfo.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::YetAnotherRelogger.Properties.Settings.Default, "GoldInfoLogging", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cbLogGoldInactivityInfo.Location = new System.Drawing.Point(6, 42);
            this.cbLogGoldInactivityInfo.Name = "cbLogGoldInactivityInfo";
            this.cbLogGoldInactivityInfo.Size = new System.Drawing.Size(151, 17);
            this.cbLogGoldInactivityInfo.TabIndex = 4;
            this.cbLogGoldInactivityInfo.Text = "Log info every 30 seconds";
            this.cbLogGoldInactivityInfo.UseVisualStyleBackColor = true;
            // 
            // checkboxGoldTimer
            // 
            this.checkboxGoldTimer.AutoSize = true;
            this.checkboxGoldTimer.Checked = global::YetAnotherRelogger.Properties.Settings.Default.UseGoldTimer;
            this.checkboxGoldTimer.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::YetAnotherRelogger.Properties.Settings.Default, "UseGoldTimer", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkboxGoldTimer.Location = new System.Drawing.Point(6, 19);
            this.checkboxGoldTimer.Name = "checkboxGoldTimer";
            this.checkboxGoldTimer.Size = new System.Drawing.Size(93, 17);
            this.checkboxGoldTimer.TabIndex = 3;
            this.checkboxGoldTimer.Text = "Use gold timer";
            this.toolTip1.SetToolTip(this.checkboxGoldTimer, "If gold does not change in this time, AntiIdle actions will be taken.");
            this.checkboxGoldTimer.UseVisualStyleBackColor = true;
            // 
            // goldTimerLabel2
            // 
            this.goldTimerLabel2.AutoSize = true;
            this.goldTimerLabel2.Location = new System.Drawing.Point(132, 65);
            this.goldTimerLabel2.Name = "goldTimerLabel2";
            this.goldTimerLabel2.Size = new System.Drawing.Size(27, 13);
            this.goldTimerLabel2.TabIndex = 2;
            this.goldTimerLabel2.Text = "sec.";
            // 
            // goldTimerLabel1
            // 
            this.goldTimerLabel1.AutoSize = true;
            this.goldTimerLabel1.Location = new System.Drawing.Point(6, 65);
            this.goldTimerLabel1.Name = "goldTimerLabel1";
            this.goldTimerLabel1.Size = new System.Drawing.Size(54, 13);
            this.goldTimerLabel1.TabIndex = 1;
            this.goldTimerLabel1.Text = "Gold timer";
            this.toolTip1.SetToolTip(this.goldTimerLabel1, "If gold does not change in this time, AntiIdle actions will be taken.");
            // 
            // goldTimerMaxDuration
            // 
            this.goldTimerMaxDuration.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::YetAnotherRelogger.Properties.Settings.Default, "GoldTimer", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.goldTimerMaxDuration.Increment = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.goldTimerMaxDuration.Location = new System.Drawing.Point(66, 63);
            this.goldTimerMaxDuration.Maximum = new decimal(new int[] {
            86400,
            0,
            0,
            0});
            this.goldTimerMaxDuration.Name = "goldTimerMaxDuration";
            this.goldTimerMaxDuration.Size = new System.Drawing.Size(60, 20);
            this.goldTimerMaxDuration.TabIndex = 0;
            this.toolTip1.SetToolTip(this.goldTimerMaxDuration, "If gold does not change in this time, AntiIdle actions will be taken.");
            this.goldTimerMaxDuration.Value = global::YetAnotherRelogger.Properties.Settings.Default.GoldTimer;
            // 
            // AntiIdle
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "AntiIdle";
            this.Size = new System.Drawing.Size(503, 411);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.antiIdleStats)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.goldTimerMaxDuration)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label goldTimerLabel2;
        private System.Windows.Forms.Label goldTimerLabel1;
        private System.Windows.Forms.NumericUpDown goldTimerMaxDuration;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox checkboxGoldTimer;
        private System.Windows.Forms.CheckBox cbLogGoldInactivityInfo;
        private System.Windows.Forms.CheckBox cbAllowPulseFix;
        private System.Windows.Forms.CheckBox cbAllowKillDB;
        private System.Windows.Forms.CheckBox cbAllowKillGame;
        private System.Windows.Forms.CheckBox cbStartBotIfStopped;
        private System.Windows.Forms.Label labelAntiIdleStats2;
        private System.Windows.Forms.Label labelAntiIdleStats1;
        private System.Windows.Forms.NumericUpDown antiIdleStats;

    }
}
