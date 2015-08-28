namespace YetAnotherRelogger.Forms.SettingsTree
{
    partial class ConnectionCheck
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbCheck60AndClose = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbPingHost2 = new System.Windows.Forms.TextBox();
            this.tbPingHost1 = new System.Windows.Forms.TextBox();
            this.cbCheckConnection = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(459, 326);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Internet Connection Check";
            // 
            // textBox2
            // 
            this.textBox2.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::YetAnotherRelogger.Properties.Settings.Default, "ConnectionCheckPingHost2", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox2.Location = new System.Drawing.Point(307, 29);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(144, 20);
            this.textBox2.TabIndex = 3;
            this.textBox2.Text = global::YetAnotherRelogger.Properties.Settings.Default.ConnectionCheckPingHost2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(224, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Or alternatively";
            // 
            // textBox1
            // 
            this.textBox1.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::YetAnotherRelogger.Properties.Settings.Default, "ConnectionCheckPingHost1", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox1.Location = new System.Drawing.Point(74, 29);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(144, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = global::YetAnotherRelogger.Properties.Settings.Default.ConnectionCheckPingHost1;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = global::YetAnotherRelogger.Properties.Settings.Default.ConnectionCheckPing;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::YetAnotherRelogger.Properties.Settings.Default, "ConnectionCheckPing", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox1.Location = new System.Drawing.Point(6, 31);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(62, 17);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "Ping to:";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.cbCheck60AndClose);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.tbPingHost2);
            this.groupBox2.Controls.Add(this.tbPingHost1);
            this.groupBox2.Controls.Add(this.cbCheckConnection);
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(363, 102);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Ping Check";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Ping to";
            // 
            // cbCheck60AndClose
            // 
            this.cbCheck60AndClose.AutoSize = true;
            this.cbCheck60AndClose.Checked = global::YetAnotherRelogger.Properties.Settings.Default.ConnectionCheckCloseBots;
            this.cbCheck60AndClose.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbCheck60AndClose.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::YetAnotherRelogger.Properties.Settings.Default, "ConnectionCheckCloseBots", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cbCheck60AndClose.Location = new System.Drawing.Point(10, 42);
            this.cbCheck60AndClose.Name = "cbCheck60AndClose";
            this.cbCheck60AndClose.Size = new System.Drawing.Size(275, 17);
            this.cbCheck60AndClose.TabIndex = 4;
            this.cbCheck60AndClose.Text = "Check every 60 seconds and close all bots on failure";
            this.cbCheck60AndClose.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(194, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(16, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "or";
            // 
            // tbPingHost2
            // 
            this.tbPingHost2.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::YetAnotherRelogger.Properties.Settings.Default, "ConnectionCheckPingHost2", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tbPingHost2.Location = new System.Drawing.Point(216, 68);
            this.tbPingHost2.Name = "tbPingHost2";
            this.tbPingHost2.Size = new System.Drawing.Size(135, 20);
            this.tbPingHost2.TabIndex = 2;
            this.tbPingHost2.Text = global::YetAnotherRelogger.Properties.Settings.Default.ConnectionCheckPingHost2;
            this.tbPingHost2.TextChanged += new System.EventHandler(this.textBox4_TextChanged);
            // 
            // tbPingHost1
            // 
            this.tbPingHost1.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::YetAnotherRelogger.Properties.Settings.Default, "ConnectionCheckPingHost1", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tbPingHost1.Location = new System.Drawing.Point(53, 68);
            this.tbPingHost1.Name = "tbPingHost1";
            this.tbPingHost1.Size = new System.Drawing.Size(135, 20);
            this.tbPingHost1.TabIndex = 1;
            this.tbPingHost1.Text = global::YetAnotherRelogger.Properties.Settings.Default.ConnectionCheckPingHost1;
            this.tbPingHost1.TextChanged += new System.EventHandler(this.textBox3_TextChanged);
            // 
            // cbCheckConnection
            // 
            this.cbCheckConnection.AutoSize = true;
            this.cbCheckConnection.Checked = global::YetAnotherRelogger.Properties.Settings.Default.ConnectionCheckPing;
            this.cbCheckConnection.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbCheckConnection.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::YetAnotherRelogger.Properties.Settings.Default, "ConnectionCheckPing", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cbCheckConnection.Location = new System.Drawing.Point(10, 19);
            this.cbCheckConnection.Name = "cbCheckConnection";
            this.cbCheckConnection.Size = new System.Drawing.Size(136, 17);
            this.cbCheckConnection.TabIndex = 0;
            this.cbCheckConnection.Text = "Check before bot starts";
            this.cbCheckConnection.UseVisualStyleBackColor = true;
            this.cbCheckConnection.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // ConnectionCheck
            // 
            this.Controls.Add(this.groupBox2);
            this.Name = "ConnectionCheck";
            this.Size = new System.Drawing.Size(445, 339);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbPingHost2;
        private System.Windows.Forms.TextBox tbPingHost1;
        private System.Windows.Forms.CheckBox cbCheckConnection;
        private System.Windows.Forms.CheckBox cbCheck60AndClose;
        private System.Windows.Forms.Label label3;

    }
}
