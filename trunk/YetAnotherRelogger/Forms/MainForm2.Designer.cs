namespace YetAnotherRelogger.Forms
{
    public partial class MainForm2
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("General");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Auto position");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Ping Check");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Ip + Host Check");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Internet Check", new System.Windows.Forms.TreeNode[] {
            treeNode3,
            treeNode4});
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Relogger", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode5});
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Anti Idle");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Hotkeys");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Stats");
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm2));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnClone = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.checkBoxForce = new System.Windows.Forms.CheckBox();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnStopAll = new System.Windows.Forms.Button();
            this.btnStartAll = new System.Windows.Forms.Button();
            this.botGrid = new System.Windows.Forms.DataGridView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.SettingsPanel = new System.Windows.Forms.Panel();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.MemoryUsage = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.CpuUsage = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.TotalGold = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.CurrentCash = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.CashPerHour = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.GoldStats = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.CommConnections = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.btnClose = new System.Windows.Forms.Button();
            this.TrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.labelStats = new System.Windows.Forms.Label();
            this.btnRestartAllDb = new System.Windows.Forms.Button();
            this.btnOpenLog = new System.Windows.Forms.Button();
            this.logDirectoryTT = new System.Windows.Forms.ToolTip(this.components);
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forceStartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.killDemonbuddyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.killDiabloToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cloneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.statsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.moveUpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveDownMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.botGrid)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MemoryUsage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CpuUsage)).BeginInit();
            this.tabPage7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GoldStats)).BeginInit();
            this.tabPage6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CommConnections)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(693, 449);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.splitContainer1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(685, 423);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Relogger";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel1MinSize = 100;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel2MinSize = 75;
            this.splitContainer1.Size = new System.Drawing.Size(679, 417);
            this.splitContainer1.SplitterDistance = 244;
            this.splitContainer1.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnClone);
            this.groupBox1.Controls.Add(this.btnPause);
            this.groupBox1.Controls.Add(this.checkBoxForce);
            this.groupBox1.Controls.Add(this.btnEdit);
            this.groupBox1.Controls.Add(this.btnNew);
            this.groupBox1.Controls.Add(this.btnStopAll);
            this.groupBox1.Controls.Add(this.btnStartAll);
            this.groupBox1.Controls.Add(this.botGrid);
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(679, 242);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Bots";
            // 
            // btnClone
            // 
            this.btnClone.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnClone.Location = new System.Drawing.Point(427, 210);
            this.btnClone.Name = "btnClone";
            this.btnClone.Size = new System.Drawing.Size(75, 23);
            this.btnClone.TabIndex = 7;
            this.btnClone.Text = "Clone";
            this.btnClone.UseVisualStyleBackColor = true;
            this.btnClone.Click += new System.EventHandler(this.btnClone_Click);
            // 
            // btnPause
            // 
            this.btnPause.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnPause.Location = new System.Drawing.Point(227, 211);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(74, 23);
            this.btnPause.TabIndex = 6;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // checkBoxForce
            // 
            this.checkBoxForce.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.checkBoxForce.AutoSize = true;
            this.checkBoxForce.Location = new System.Drawing.Point(11, 214);
            this.checkBoxForce.Name = "checkBoxForce";
            this.checkBoxForce.Size = new System.Drawing.Size(53, 17);
            this.checkBoxForce.TabIndex = 5;
            this.checkBoxForce.Text = "Force";
            this.checkBoxForce.UseVisualStyleBackColor = true;
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnEdit.Location = new System.Drawing.Point(508, 211);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 4;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnNew
            // 
            this.btnNew.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnNew.Location = new System.Drawing.Point(589, 211);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(75, 23);
            this.btnNew.TabIndex = 3;
            this.btnNew.Text = "New";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnStopAll
            // 
            this.btnStopAll.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnStopAll.Location = new System.Drawing.Point(146, 211);
            this.btnStopAll.Name = "btnStopAll";
            this.btnStopAll.Size = new System.Drawing.Size(75, 23);
            this.btnStopAll.TabIndex = 2;
            this.btnStopAll.Text = "Stop All";
            this.btnStopAll.UseVisualStyleBackColor = true;
            this.btnStopAll.Click += new System.EventHandler(this.btnStopAll_Click);
            // 
            // btnStartAll
            // 
            this.btnStartAll.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnStartAll.Location = new System.Drawing.Point(65, 211);
            this.btnStartAll.Name = "btnStartAll";
            this.btnStartAll.Size = new System.Drawing.Size(75, 23);
            this.btnStartAll.TabIndex = 1;
            this.btnStartAll.Text = "Start All";
            this.btnStartAll.UseVisualStyleBackColor = true;
            this.btnStartAll.Click += new System.EventHandler(this.btnStartAll_click);
            // 
            // botGrid
            // 
            this.botGrid.AllowUserToOrderColumns = true;
            this.botGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.botGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.botGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.botGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.botGrid.Location = new System.Drawing.Point(6, 15);
            this.botGrid.Name = "botGrid";
            this.botGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.botGrid.Size = new System.Drawing.Size(667, 190);
            this.botGrid.TabIndex = 0;
            this.botGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.richTextBox1);
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(679, 169);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Log";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Cursor = System.Windows.Forms.Cursors.Default;
            this.richTextBox1.Location = new System.Drawing.Point(6, 15);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(667, 148);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.SettingsPanel);
            this.tabPage2.Controls.Add(this.treeView1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(685, 423);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Settings";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // SettingsPanel
            // 
            this.SettingsPanel.Location = new System.Drawing.Point(185, 0);
            this.SettingsPanel.Name = "SettingsPanel";
            this.SettingsPanel.Size = new System.Drawing.Size(501, 407);
            this.SettingsPanel.TabIndex = 4;
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(6, 6);
            this.treeView1.Name = "treeView1";
            treeNode1.Name = "General";
            treeNode1.Text = "General";
            treeNode2.Name = "AutoPos";
            treeNode2.Text = "Auto position";
            treeNode3.Name = "PingCheck";
            treeNode3.Text = "Ping Check";
            treeNode4.Name = "IpHostCheck";
            treeNode4.Text = "Ip + Host Check";
            treeNode5.Name = "ConnectionCheck";
            treeNode5.Text = "Internet Check";
            treeNode6.Checked = true;
            treeNode6.Name = "Node0";
            treeNode6.Text = "Relogger";
            treeNode7.Name = "AntiIdle";
            treeNode7.Text = "Anti Idle";
            treeNode8.Name = "HotKeys";
            treeNode8.Text = "Hotkeys";
            treeNode9.Name = "Stats";
            treeNode9.Text = "Stats";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode6,
            treeNode7,
            treeNode8,
            treeNode9});
            this.treeView1.Size = new System.Drawing.Size(173, 395);
            this.treeView1.TabIndex = 3;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.tabControl2);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(685, 423);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Stats";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tabControl2
            // 
            this.tabControl2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl2.Controls.Add(this.tabPage5);
            this.tabControl2.Controls.Add(this.tabPage7);
            this.tabControl2.Controls.Add(this.tabPage6);
            this.tabControl2.Location = new System.Drawing.Point(12, 6);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(667, 395);
            this.tabControl2.TabIndex = 3;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.MemoryUsage);
            this.tabPage5.Controls.Add(this.CpuUsage);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(659, 369);
            this.tabPage5.TabIndex = 0;
            this.tabPage5.Text = "System";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // MemoryUsage
            // 
            this.MemoryUsage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea1.Name = "ChartArea1";
            this.MemoryUsage.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.MemoryUsage.Legends.Add(legend1);
            this.MemoryUsage.Location = new System.Drawing.Point(0, 179);
            this.MemoryUsage.MinimumSize = new System.Drawing.Size(650, 140);
            this.MemoryUsage.Name = "MemoryUsage";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.MemoryUsage.Series.Add(series1);
            this.MemoryUsage.Size = new System.Drawing.Size(650, 140);
            this.MemoryUsage.TabIndex = 2;
            this.MemoryUsage.Text = "chart1";
            // 
            // CpuUsage
            // 
            this.CpuUsage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea2.Name = "ChartArea1";
            this.CpuUsage.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.CpuUsage.Legends.Add(legend2);
            this.CpuUsage.Location = new System.Drawing.Point(3, 19);
            this.CpuUsage.MinimumSize = new System.Drawing.Size(650, 140);
            this.CpuUsage.Name = "CpuUsage";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.CpuUsage.Series.Add(series2);
            this.CpuUsage.Size = new System.Drawing.Size(650, 140);
            this.CpuUsage.TabIndex = 1;
            this.CpuUsage.Text = "chart1";
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.TotalGold);
            this.tabPage7.Controls.Add(this.label4);
            this.tabPage7.Controls.Add(this.CurrentCash);
            this.tabPage7.Controls.Add(this.label3);
            this.tabPage7.Controls.Add(this.CashPerHour);
            this.tabPage7.Controls.Add(this.label1);
            this.tabPage7.Controls.Add(this.GoldStats);
            this.tabPage7.Location = new System.Drawing.Point(4, 22);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage7.Size = new System.Drawing.Size(659, 369);
            this.tabPage7.TabIndex = 2;
            this.tabPage7.Text = "Bots";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // TotalGold
            // 
            this.TotalGold.AutoSize = true;
            this.TotalGold.Location = new System.Drawing.Point(95, 157);
            this.TotalGold.Name = "TotalGold";
            this.TotalGold.Size = new System.Drawing.Size(13, 13);
            this.TotalGold.TabIndex = 11;
            this.TotalGold.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 157);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Total Gold:";
            // 
            // CurrentCash
            // 
            this.CurrentCash.AutoSize = true;
            this.CurrentCash.Location = new System.Drawing.Point(95, 179);
            this.CurrentCash.Name = "CurrentCash";
            this.CurrentCash.Size = new System.Drawing.Size(22, 13);
            this.CurrentCash.TabIndex = 9;
            this.CurrentCash.Text = "0 $";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 179);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Gold as cash:";
            // 
            // CashPerHour
            // 
            this.CashPerHour.AutoSize = true;
            this.CashPerHour.Location = new System.Drawing.Point(95, 135);
            this.CashPerHour.Name = "CashPerHour";
            this.CashPerHour.Size = new System.Drawing.Size(22, 13);
            this.CashPerHour.TabIndex = 7;
            this.CashPerHour.Text = "0 $";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 135);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Cash per hour:";
            // 
            // GoldStats
            // 
            chartArea3.Name = "ChartArea1";
            this.GoldStats.ChartAreas.Add(chartArea3);
            legend3.Name = "Legend1";
            this.GoldStats.Legends.Add(legend3);
            this.GoldStats.Location = new System.Drawing.Point(6, 6);
            this.GoldStats.Name = "GoldStats";
            series3.ChartArea = "ChartArea1";
            series3.Legend = "Legend1";
            series3.Name = "Series1";
            this.GoldStats.Series.Add(series3);
            this.GoldStats.Size = new System.Drawing.Size(643, 105);
            this.GoldStats.TabIndex = 5;
            this.GoldStats.Text = "GoldStats";
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.CommConnections);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(659, 369);
            this.tabPage6.TabIndex = 1;
            this.tabPage6.Text = "Relogger";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // CommConnections
            // 
            chartArea4.Name = "ChartArea1";
            this.CommConnections.ChartAreas.Add(chartArea4);
            legend4.Name = "Legend1";
            this.CommConnections.Legends.Add(legend4);
            this.CommConnections.Location = new System.Drawing.Point(6, 6);
            this.CommConnections.Name = "CommConnections";
            series4.ChartArea = "ChartArea1";
            series4.Legend = "Legend1";
            series4.Name = "Series1";
            this.CommConnections.Series.Add(series4);
            this.CommConnections.Size = new System.Drawing.Size(643, 105);
            this.CommConnections.TabIndex = 4;
            this.CommConnections.Text = "chart1";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(621, 467);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // TrayIcon
            // 
            this.TrayIcon.Text = "Yet Another Relogger";
            this.TrayIcon.Visible = true;
            // 
            // labelStats
            // 
            this.labelStats.AutoSize = true;
            this.labelStats.Location = new System.Drawing.Point(25, 448);
            this.labelStats.Name = "labelStats";
            this.labelStats.Size = new System.Drawing.Size(0, 13);
            this.labelStats.TabIndex = 2;
            // 
            // btnRestartAllDb
            // 
            this.btnRestartAllDb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRestartAllDb.Location = new System.Drawing.Point(25, 467);
            this.btnRestartAllDb.Name = "btnRestartAllDb";
            this.btnRestartAllDb.Size = new System.Drawing.Size(83, 23);
            this.btnRestartAllDb.TabIndex = 8;
            this.btnRestartAllDb.Text = "Restart All DB";
            this.btnRestartAllDb.UseVisualStyleBackColor = true;
            this.btnRestartAllDb.Click += new System.EventHandler(this.btnRestartAllDb_Click);
            // 
            // btnOpenLog
            // 
            this.btnOpenLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenLog.Location = new System.Drawing.Point(527, 467);
            this.btnOpenLog.Name = "btnOpenLog";
            this.btnOpenLog.Size = new System.Drawing.Size(88, 23);
            this.btnOpenLog.TabIndex = 9;
            this.btnOpenLog.Text = "Open Log File";
            this.logDirectoryTT.SetToolTip(this.btnOpenLog, "Shift-click to open log directory");
            this.btnOpenLog.UseVisualStyleBackColor = true;
            this.btnOpenLog.Click += new System.EventHandler(this.btnOpenLog_Click);
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.startToolStripMenuItem.Text = "Start";
            this.startToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
            // 
            // forceStartToolStripMenuItem
            // 
            this.forceStartToolStripMenuItem.Name = "forceStartToolStripMenuItem";
            this.forceStartToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.forceStartToolStripMenuItem.Text = "Force Start";
            this.forceStartToolStripMenuItem.Click += new System.EventHandler(this.forceStartToolStripMenuItem_Click);
            // 
            // killDemonbuddyToolStripMenuItem
            // 
            this.killDemonbuddyToolStripMenuItem.Name = "killDemonbuddyToolStripMenuItem";
            this.killDemonbuddyToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.killDemonbuddyToolStripMenuItem.Text = "Kill Demonbuddy";
            this.killDemonbuddyToolStripMenuItem.Click += new System.EventHandler(this.killDemonbuddyToolStripMenuItem_Click);
            // 
            // killDiabloToolStripMenuItem
            // 
            this.killDiabloToolStripMenuItem.Name = "killDiabloToolStripMenuItem";
            this.killDiabloToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.killDiabloToolStripMenuItem.Text = "Kill Diablo";
            this.killDiabloToolStripMenuItem.Click += new System.EventHandler(this.killDiabloToolStripMenuItem_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.stopToolStripMenuItem.Text = "Stop";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // cloneToolStripMenuItem
            // 
            this.cloneToolStripMenuItem.Name = "cloneToolStripMenuItem";
            this.cloneToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.cloneToolStripMenuItem.Text = "Clone";
            this.cloneToolStripMenuItem.Click += new System.EventHandler(this.cloneToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(163, 6);
            // 
            // statsToolStripMenuItem
            // 
            this.statsToolStripMenuItem.Enabled = false;
            this.statsToolStripMenuItem.Name = "statsToolStripMenuItem";
            this.statsToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.statsToolStripMenuItem.Text = "Stats";
            this.statsToolStripMenuItem.Click += new System.EventHandler(this.statsToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(163, 6);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startToolStripMenuItem,
            this.forceStartToolStripMenuItem,
            this.killDemonbuddyToolStripMenuItem,
            this.killDiabloToolStripMenuItem,
            this.moveUpMenuItem,
            this.moveDownMenuItem,
            this.stopToolStripMenuItem,
            this.cloneToolStripMenuItem,
            this.toolStripSeparator1,
            this.statsToolStripMenuItem,
            this.toolStripSeparator2,
            this.editToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(167, 258);
            // 
            // moveUpMenuItem
            // 
            this.moveUpMenuItem.Name = "moveUpMenuItem";
            this.moveUpMenuItem.Size = new System.Drawing.Size(166, 22);
            this.moveUpMenuItem.Text = "Move Up";
            this.moveUpMenuItem.Click += new System.EventHandler(this.moveUpMenuItem_Click);
            // 
            // moveDownMenuItem
            // 
            this.moveDownMenuItem.Name = "moveDownMenuItem";
            this.moveDownMenuItem.Size = new System.Drawing.Size(166, 22);
            this.moveDownMenuItem.Text = "Move Down";
            this.moveDownMenuItem.Click += new System.EventHandler(this.moveDownMenuItem_Click);
            // 
            // MainForm2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(714, 498);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnOpenLog);
            this.Controls.Add(this.btnRestartAllDb);
            this.Controls.Add(this.labelStats);
            this.Controls.Add(this.btnClose);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(730, 536);
            this.Name = "MainForm2";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Yet Another Relogger";
            this.Load += new System.EventHandler(this.MainForm2_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.botGrid)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MemoryUsage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CpuUsage)).EndInit();
            this.tabPage7.ResumeLayout(false);
            this.tabPage7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GoldStats)).EndInit();
            this.tabPage6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.CommConnections)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.NotifyIcon TrayIcon;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Panel SettingsPanel;
        public System.Windows.Forms.Label labelStats;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage5;
        public System.Windows.Forms.DataVisualization.Charting.Chart MemoryUsage;
        public System.Windows.Forms.DataVisualization.Charting.Chart CpuUsage;
        private System.Windows.Forms.TabPage tabPage6;
        public System.Windows.Forms.DataVisualization.Charting.Chart CommConnections;
        private System.Windows.Forms.TabPage tabPage7;
        public System.Windows.Forms.DataVisualization.Charting.Chart GoldStats;
        public System.Windows.Forms.Label CashPerHour;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label CurrentCash;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.Label TotalGold;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.CheckBox checkBoxForce;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnStopAll;
        private System.Windows.Forms.Button btnStartAll;
        public System.Windows.Forms.DataGridView botGrid;
        public System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnClone;
        private System.Windows.Forms.Button btnRestartAllDb;
        private System.Windows.Forms.Button btnOpenLog;
        private System.Windows.Forms.ToolTip logDirectoryTT;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem forceStartToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem killDemonbuddyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem killDiabloToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cloneToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem statsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem moveUpMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveDownMenuItem;
    }
}