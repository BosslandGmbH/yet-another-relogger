namespace YetAnotherRelogger.Forms.SettingsTree
{
    partial class IpHostCheck
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbEnableHostCheck = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbAddressList = new System.Windows.Forms.TextBox();
            this.cbEnableIPCheck = new System.Windows.Forms.CheckBox();
            this.cbCheck60AndClose = new System.Windows.Forms.CheckBox();
            this.cbCheckBeforeBotStart = new System.Windows.Forms.CheckBox();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.groupBox4);
            this.groupBox3.Controls.Add(this.cbCheck60AndClose);
            this.groupBox3.Controls.Add(this.cbCheckBeforeBotStart);
            this.groupBox3.Location = new System.Drawing.Point(3, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(494, 342);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Validate IP + Host";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.cbEnableHostCheck);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.tbAddressList);
            this.groupBox4.Controls.Add(this.cbEnableIPCheck);
            this.groupBox4.Location = new System.Drawing.Point(6, 64);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(482, 272);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Check list";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 220);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(265, 39);
            this.label3.TabIndex = 6;
            this.label3.Text = "To allow a Host or IP simply place the \'@\' sign in front\r\n(be sure to place all a" +
    "llowed hosts/ips before all others)\r\n- Example: @192.168.1.200";
            // 
            // cbEnableHostCheck
            // 
            this.cbEnableHostCheck.AutoSize = true;
            this.cbEnableHostCheck.Checked = global::YetAnotherRelogger.Properties.Settings.Default.ConnectionCheckHostCheck;
            this.cbEnableHostCheck.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::YetAnotherRelogger.Properties.Settings.Default, "ConnectionCheckHostCheck", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cbEnableHostCheck.Location = new System.Drawing.Point(6, 42);
            this.cbEnableHostCheck.Name = "cbEnableHostCheck";
            this.cbEnableHostCheck.Size = new System.Drawing.Size(117, 17);
            this.cbEnableHostCheck.TabIndex = 5;
            this.cbEnableHostCheck.Text = "Enable Host check";
            this.cbEnableHostCheck.UseVisualStyleBackColor = true;
            this.cbEnableHostCheck.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(296, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(180, 91);
            this.label2.TabIndex = 4;
            this.label2.Text = "IP:\r\n- Range: 192.168.1.0-192.168.3.255\r\n- Wildcard: 192.168.*.*\r\n               " +
    "or: 192.1??.*.*\r\nHost:\r\n- Wildcard: *.dynamic.provider.nl\r\n               or: na" +
    "me.*.provider.nl\r\n";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(296, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Examples\r\n";
            // 
            // tbAddressList
            // 
            this.tbAddressList.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::YetAnotherRelogger.Properties.Settings.Default, "ConnectionCheckIpHostList", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tbAddressList.Location = new System.Drawing.Point(5, 65);
            this.tbAddressList.Multiline = true;
            this.tbAddressList.Name = "tbAddressList";
            this.tbAddressList.Size = new System.Drawing.Size(285, 152);
            this.tbAddressList.TabIndex = 2;
            this.tbAddressList.Text = global::YetAnotherRelogger.Properties.Settings.Default.ConnectionCheckIpHostList;
            this.tbAddressList.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // cbEnableIPCheck
            // 
            this.cbEnableIPCheck.AutoSize = true;
            this.cbEnableIPCheck.Checked = global::YetAnotherRelogger.Properties.Settings.Default.ConnectionCheckIpCheck;
            this.cbEnableIPCheck.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::YetAnotherRelogger.Properties.Settings.Default, "ConnectionCheckIpCheck", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cbEnableIPCheck.Location = new System.Drawing.Point(6, 19);
            this.cbEnableIPCheck.Name = "cbEnableIPCheck";
            this.cbEnableIPCheck.Size = new System.Drawing.Size(105, 17);
            this.cbEnableIPCheck.TabIndex = 1;
            this.cbEnableIPCheck.Text = "Enable IP check";
            this.cbEnableIPCheck.UseVisualStyleBackColor = true;
            this.cbEnableIPCheck.CheckedChanged += new System.EventHandler(this.checkBox7_CheckedChanged);
            // 
            // cbCheck60AndClose
            // 
            this.cbCheck60AndClose.AutoSize = true;
            this.cbCheck60AndClose.Checked = global::YetAnotherRelogger.Properties.Settings.Default.ConnectionCheckIpHostCloseBots;
            this.cbCheck60AndClose.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::YetAnotherRelogger.Properties.Settings.Default, "ConnectionCheckIpHostCloseBots", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cbCheck60AndClose.Location = new System.Drawing.Point(12, 42);
            this.cbCheck60AndClose.Name = "cbCheck60AndClose";
            this.cbCheck60AndClose.Size = new System.Drawing.Size(275, 17);
            this.cbCheck60AndClose.TabIndex = 1;
            this.cbCheck60AndClose.Text = "Check every 60 seconds and close all bots on failure";
            this.cbCheck60AndClose.UseVisualStyleBackColor = true;
            this.cbCheck60AndClose.CheckedChanged += new System.EventHandler(this.checkBox5_CheckedChanged);
            // 
            // cbCheckBeforeBotStart
            // 
            this.cbCheckBeforeBotStart.AutoSize = true;
            this.cbCheckBeforeBotStart.Checked = global::YetAnotherRelogger.Properties.Settings.Default.ConnectionCheckIpHost;
            this.cbCheckBeforeBotStart.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::YetAnotherRelogger.Properties.Settings.Default, "ConnectionCheckIpHost", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cbCheckBeforeBotStart.Location = new System.Drawing.Point(12, 19);
            this.cbCheckBeforeBotStart.Name = "cbCheckBeforeBotStart";
            this.cbCheckBeforeBotStart.Size = new System.Drawing.Size(136, 17);
            this.cbCheckBeforeBotStart.TabIndex = 0;
            this.cbCheckBeforeBotStart.Text = "Check before bot starts";
            this.cbCheckBeforeBotStart.UseVisualStyleBackColor = true;
            this.cbCheckBeforeBotStart.CheckedChanged += new System.EventHandler(this.checkBox4_CheckedChanged);
            // 
            // IpHostCheck
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox3);
            this.Name = "IpHostCheck";
            this.Size = new System.Drawing.Size(506, 358);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox cbEnableIPCheck;
        private System.Windows.Forms.CheckBox cbCheck60AndClose;
        private System.Windows.Forms.CheckBox cbCheckBeforeBotStart;
        private System.Windows.Forms.TextBox tbAddressList;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox cbEnableHostCheck;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}
