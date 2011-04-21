namespace RFExplorerClient
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.MainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveOnCloseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuPortInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.realtimeDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.averagedDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.maxDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.minDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripCleanReport = new System.Windows.Forms.ToolStripMenuItem();
            this.m_timer_receive = new System.Windows.Forms.Timer(this.components);
            this.m_serialPortObj = new System.IO.Ports.SerialPort(this.components);
            this.MainTab = new System.Windows.Forms.TabControl();
            this.tabSpectrumAnalyzer = new System.Windows.Forms.TabPage();
            this.groupDataFeed = new System.Windows.Forms.GroupBox();
            this.chkCalcMin = new System.Windows.Forms.CheckBox();
            this.chkCalcMax = new System.Windows.Forms.CheckBox();
            this.chkCalcAverage = new System.Windows.Forms.CheckBox();
            this.chkCalcRealtime = new System.Windows.Forms.CheckBox();
            this.numericIterations = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.chkRunMode = new System.Windows.Forms.CheckBox();
            this.chkHoldMode = new System.Windows.Forms.CheckBox();
            this.numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.MainStatusBar = new System.Windows.Forms.StatusStrip();
            this.toolCOMStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripMemory = new System.Windows.Forms.ToolStripProgressBar();
            this.toolFile = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupCOM = new System.Windows.Forms.GroupBox();
            this.btnRescan = new System.Windows.Forms.Button();
            this.comboBaudRate = new System.Windows.Forms.ComboBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.COMPortCombo = new System.Windows.Forms.ComboBox();
            this.objGraph = new ZedGraph.ZedGraphControl();
            this.groupSettings = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.m_sBottomDBM = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.m_sTopDBM = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.m_sEndFreq = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.m_sFreqSpan = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.m_sStartFreq = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.m_sCenterFreq = new System.Windows.Forms.TextBox();
            this.tabReport = new System.Windows.Forms.TabPage();
            this.textBox_message = new System.Windows.Forms.TextBox();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveCSVtoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MainMenu.SuspendLayout();
            this.MainTab.SuspendLayout();
            this.tabSpectrumAnalyzer.SuspendLayout();
            this.groupDataFeed.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericIterations)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown)).BeginInit();
            this.MainStatusBar.SuspendLayout();
            this.groupCOM.SuspendLayout();
            this.groupSettings.SuspendLayout();
            this.tabReport.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainMenu
            // 
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.MainMenu.Location = new System.Drawing.Point(0, 0);
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.MainMenu.Size = new System.Drawing.Size(940, 24);
            this.MainMenu.TabIndex = 46;
            this.MainMenu.Text = "menu";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemLoad,
            this.saveAsToolStripMenuItem,
            this.SaveCSVtoolStripMenuItem,
            this.saveOnCloseToolStripMenuItem,
            this.toolStripMenuItem1,
            this.toolStripSeparator2,
            this.toolStripMenuPortInfo,
            this.aboutToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "&File";
            this.fileToolStripMenuItem.DropDownOpening += new System.EventHandler(this.fileToolStripMenuItem_DropDownOpening);
            // 
            // toolStripMenuItemLoad
            // 
            this.toolStripMenuItemLoad.Name = "toolStripMenuItemLoad";
            this.toolStripMenuItemLoad.Size = new System.Drawing.Size(188, 22);
            this.toolStripMenuItemLoad.Text = "&Load...";
            this.toolStripMenuItemLoad.Click += new System.EventHandler(this.toolStripMenuItemLoad_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.saveAsToolStripMenuItem.Text = "Sa&ve As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // saveOnCloseToolStripMenuItem
            // 
            this.saveOnCloseToolStripMenuItem.CheckOnClick = true;
            this.saveOnCloseToolStripMenuItem.Name = "saveOnCloseToolStripMenuItem";
            this.saveOnCloseToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.saveOnCloseToolStripMenuItem.Text = "&Save on Close";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(185, 6);
            // 
            // toolStripMenuPortInfo
            // 
            this.toolStripMenuPortInfo.Name = "toolStripMenuPortInfo";
            this.toolStripMenuPortInfo.Size = new System.Drawing.Size(188, 22);
            this.toolStripMenuPortInfo.Text = "Report COM port &info";
            this.toolStripMenuPortInfo.Click += new System.EventHandler(this.toolStripMenuPortInfo_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.aboutToolStripMenuItem.Text = "&About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(185, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.realtimeDataToolStripMenuItem,
            this.averagedDataToolStripMenuItem,
            this.maxDataToolStripMenuItem,
            this.minDataToolStripMenuItem,
            this.toolStripSeparator3,
            this.toolStripCleanReport});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.viewToolStripMenuItem.Text = "&View";
            this.viewToolStripMenuItem.DropDownOpening += new System.EventHandler(this.viewToolStripMenuItem_DropDownOpening);
            // 
            // realtimeDataToolStripMenuItem
            // 
            this.realtimeDataToolStripMenuItem.CheckOnClick = true;
            this.realtimeDataToolStripMenuItem.Name = "realtimeDataToolStripMenuItem";
            this.realtimeDataToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.realtimeDataToolStripMenuItem.Text = "&Realtime data";
            this.realtimeDataToolStripMenuItem.Click += new System.EventHandler(this.click_view_mode);
            // 
            // averagedDataToolStripMenuItem
            // 
            this.averagedDataToolStripMenuItem.CheckOnClick = true;
            this.averagedDataToolStripMenuItem.Name = "averagedDataToolStripMenuItem";
            this.averagedDataToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.averagedDataToolStripMenuItem.Text = "&Averaged data";
            this.averagedDataToolStripMenuItem.Click += new System.EventHandler(this.click_view_mode);
            // 
            // maxDataToolStripMenuItem
            // 
            this.maxDataToolStripMenuItem.CheckOnClick = true;
            this.maxDataToolStripMenuItem.Name = "maxDataToolStripMenuItem";
            this.maxDataToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.maxDataToolStripMenuItem.Text = "Ma&x data";
            this.maxDataToolStripMenuItem.Click += new System.EventHandler(this.click_view_mode);
            // 
            // minDataToolStripMenuItem
            // 
            this.minDataToolStripMenuItem.CheckOnClick = true;
            this.minDataToolStripMenuItem.Name = "minDataToolStripMenuItem";
            this.minDataToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.minDataToolStripMenuItem.Text = "M&in data";
            this.minDataToolStripMenuItem.Click += new System.EventHandler(this.click_view_mode);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(154, 6);
            // 
            // toolStripCleanReport
            // 
            this.toolStripCleanReport.Name = "toolStripCleanReport";
            this.toolStripCleanReport.Size = new System.Drawing.Size(157, 22);
            this.toolStripCleanReport.Text = "C&lean Report";
            this.toolStripCleanReport.Click += new System.EventHandler(this.toolStripCleanReport_Click);
            // 
            // m_timer_receive
            // 
            this.m_timer_receive.Interval = 50;
            this.m_timer_receive.Tick += new System.EventHandler(this.timer_receive_Tick);
            // 
            // MainTab
            // 
            this.MainTab.Controls.Add(this.tabSpectrumAnalyzer);
            this.MainTab.Controls.Add(this.tabReport);
            this.MainTab.Location = new System.Drawing.Point(0, 27);
            this.MainTab.Name = "MainTab";
            this.MainTab.Padding = new System.Drawing.Point(16, 5);
            this.MainTab.SelectedIndex = 0;
            this.MainTab.Size = new System.Drawing.Size(940, 677);
            this.MainTab.TabIndex = 49;
            // 
            // tabSpectrumAnalyzer
            // 
            this.tabSpectrumAnalyzer.Controls.Add(this.groupDataFeed);
            this.tabSpectrumAnalyzer.Controls.Add(this.MainStatusBar);
            this.tabSpectrumAnalyzer.Controls.Add(this.groupCOM);
            this.tabSpectrumAnalyzer.Controls.Add(this.objGraph);
            this.tabSpectrumAnalyzer.Controls.Add(this.groupSettings);
            this.tabSpectrumAnalyzer.Location = new System.Drawing.Point(4, 26);
            this.tabSpectrumAnalyzer.Name = "tabSpectrumAnalyzer";
            this.tabSpectrumAnalyzer.Padding = new System.Windows.Forms.Padding(3);
            this.tabSpectrumAnalyzer.Size = new System.Drawing.Size(932, 647);
            this.tabSpectrumAnalyzer.TabIndex = 0;
            this.tabSpectrumAnalyzer.Text = "Spectrum Analyzer";
            this.tabSpectrumAnalyzer.UseVisualStyleBackColor = true;
            this.tabSpectrumAnalyzer.Enter += new System.EventHandler(this.tabSpectrumAnalyzer_Enter);
            // 
            // groupDataFeed
            // 
            this.groupDataFeed.Controls.Add(this.chkCalcMin);
            this.groupDataFeed.Controls.Add(this.chkCalcMax);
            this.groupDataFeed.Controls.Add(this.chkCalcAverage);
            this.groupDataFeed.Controls.Add(this.chkCalcRealtime);
            this.groupDataFeed.Controls.Add(this.numericIterations);
            this.groupDataFeed.Controls.Add(this.label1);
            this.groupDataFeed.Controls.Add(this.chkRunMode);
            this.groupDataFeed.Controls.Add(this.chkHoldMode);
            this.groupDataFeed.Controls.Add(this.numericUpDown);
            this.groupDataFeed.Controls.Add(this.label2);
            this.groupDataFeed.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupDataFeed.Location = new System.Drawing.Point(272, 6);
            this.groupDataFeed.Name = "groupDataFeed";
            this.groupDataFeed.Size = new System.Drawing.Size(236, 108);
            this.groupDataFeed.TabIndex = 52;
            this.groupDataFeed.TabStop = false;
            this.groupDataFeed.Text = "Mode";
            // 
            // chkCalcMin
            // 
            this.chkCalcMin.AutoSize = true;
            this.chkCalcMin.Location = new System.Drawing.Point(160, 79);
            this.chkCalcMin.Name = "chkCalcMin";
            this.chkCalcMin.Size = new System.Drawing.Size(67, 17);
            this.chkCalcMin.TabIndex = 51;
            this.chkCalcMin.Text = "Minimum";
            this.chkCalcMin.UseVisualStyleBackColor = true;
            this.chkCalcMin.CheckedChanged += new System.EventHandler(this.chkCalcMin_CheckedChanged);
            // 
            // chkCalcMax
            // 
            this.chkCalcMax.AutoSize = true;
            this.chkCalcMax.Location = new System.Drawing.Point(160, 59);
            this.chkCalcMax.Name = "chkCalcMax";
            this.chkCalcMax.Size = new System.Drawing.Size(74, 17);
            this.chkCalcMax.TabIndex = 51;
            this.chkCalcMax.Text = "Max Peak";
            this.chkCalcMax.UseVisualStyleBackColor = true;
            this.chkCalcMax.CheckedChanged += new System.EventHandler(this.chkCalcMax_CheckedChanged);
            // 
            // chkCalcAverage
            // 
            this.chkCalcAverage.AutoSize = true;
            this.chkCalcAverage.Location = new System.Drawing.Point(160, 39);
            this.chkCalcAverage.Name = "chkCalcAverage";
            this.chkCalcAverage.Size = new System.Drawing.Size(66, 17);
            this.chkCalcAverage.TabIndex = 51;
            this.chkCalcAverage.Text = "Average";
            this.chkCalcAverage.UseVisualStyleBackColor = true;
            this.chkCalcAverage.CheckedChanged += new System.EventHandler(this.chkCalcAverage_CheckedChanged);
            // 
            // chkCalcRealtime
            // 
            this.chkCalcRealtime.AutoSize = true;
            this.chkCalcRealtime.Location = new System.Drawing.Point(160, 19);
            this.chkCalcRealtime.Name = "chkCalcRealtime";
            this.chkCalcRealtime.Size = new System.Drawing.Size(67, 17);
            this.chkCalcRealtime.TabIndex = 51;
            this.chkCalcRealtime.Text = "Realtime";
            this.chkCalcRealtime.UseVisualStyleBackColor = true;
            this.chkCalcRealtime.CheckedChanged += new System.EventHandler(this.chkCalcRealtime_CheckedChanged);
            // 
            // numericIterations
            // 
            this.numericIterations.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericIterations.Location = new System.Drawing.Point(80, 73);
            this.numericIterations.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericIterations.Name = "numericIterations";
            this.numericIterations.Size = new System.Drawing.Size(60, 23);
            this.numericIterations.TabIndex = 17;
            this.numericIterations.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericIterations.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericIterations.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(16, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 21);
            this.label1.TabIndex = 49;
            this.label1.Text = "Sample:";
            // 
            // chkRunMode
            // 
            this.chkRunMode.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkRunMode.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkRunMode.Location = new System.Drawing.Point(14, 20);
            this.chkRunMode.MinimumSize = new System.Drawing.Size(60, 0);
            this.chkRunMode.Name = "chkRunMode";
            this.chkRunMode.Size = new System.Drawing.Size(60, 23);
            this.chkRunMode.TabIndex = 14;
            this.chkRunMode.Text = "RUN";
            this.chkRunMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkRunMode.UseVisualStyleBackColor = true;
            this.chkRunMode.CheckedChanged += new System.EventHandler(this.chkRunMode_CheckedChanged);
            // 
            // chkHoldMode
            // 
            this.chkHoldMode.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkHoldMode.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkHoldMode.Location = new System.Drawing.Point(80, 20);
            this.chkHoldMode.MinimumSize = new System.Drawing.Size(60, 0);
            this.chkHoldMode.Name = "chkHoldMode";
            this.chkHoldMode.Size = new System.Drawing.Size(60, 23);
            this.chkHoldMode.TabIndex = 15;
            this.chkHoldMode.Text = "HOLD";
            this.chkHoldMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkHoldMode.UseVisualStyleBackColor = true;
            this.chkHoldMode.CheckedChanged += new System.EventHandler(this.chkHoldMode_CheckedChanged);
            // 
            // numericUpDown
            // 
            this.numericUpDown.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown.Location = new System.Drawing.Point(80, 46);
            this.numericUpDown.Name = "numericUpDown";
            this.numericUpDown.Size = new System.Drawing.Size(60, 23);
            this.numericUpDown.TabIndex = 16;
            this.numericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 21);
            this.label2.TabIndex = 50;
            this.label2.Text = "Iterations:";
            // 
            // MainStatusBar
            // 
            this.MainStatusBar.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.MainStatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolCOMStatus,
            this.toolStripMemory,
            this.toolFile});
            this.MainStatusBar.Location = new System.Drawing.Point(3, 622);
            this.MainStatusBar.Name = "MainStatusBar";
            this.MainStatusBar.Size = new System.Drawing.Size(926, 22);
            this.MainStatusBar.TabIndex = 51;
            this.MainStatusBar.Text = "statusStrip1";
            // 
            // toolCOMStatus
            // 
            this.toolCOMStatus.Name = "toolCOMStatus";
            this.toolCOMStatus.Size = new System.Drawing.Size(71, 17);
            this.toolCOMStatus.Text = "Disconnected";
            // 
            // toolStripMemory
            // 
            this.toolStripMemory.Name = "toolStripMemory";
            this.toolStripMemory.Size = new System.Drawing.Size(100, 16);
            this.toolStripMemory.ToolTipText = "Buffer memory used";
            // 
            // toolFile
            // 
            this.toolFile.Name = "toolFile";
            this.toolFile.Size = new System.Drawing.Size(54, 17);
            this.toolFile.Text = "File: none";
            // 
            // groupCOM
            // 
            this.groupCOM.Controls.Add(this.btnRescan);
            this.groupCOM.Controls.Add(this.comboBaudRate);
            this.groupCOM.Controls.Add(this.btnConnect);
            this.groupCOM.Controls.Add(this.btnDisconnect);
            this.groupCOM.Controls.Add(this.COMPortCombo);
            this.groupCOM.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupCOM.Location = new System.Drawing.Point(6, 6);
            this.groupCOM.Name = "groupCOM";
            this.groupCOM.Size = new System.Drawing.Size(260, 108);
            this.groupCOM.TabIndex = 50;
            this.groupCOM.TabStop = false;
            this.groupCOM.Text = "COM Port";
            // 
            // btnRescan
            // 
            this.btnRescan.AutoSize = true;
            this.btnRescan.Location = new System.Drawing.Point(110, 19);
            this.btnRescan.Name = "btnRescan";
            this.btnRescan.Size = new System.Drawing.Size(34, 23);
            this.btnRescan.TabIndex = 11;
            this.btnRescan.Text = "*";
            this.btnRescan.UseVisualStyleBackColor = true;
            this.btnRescan.Click += new System.EventHandler(this.btnRescan_Click);
            // 
            // comboBaudRate
            // 
            this.comboBaudRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBaudRate.Items.AddRange(new object[] {
            "2400",
            "500000"});
            this.comboBaudRate.Location = new System.Drawing.Point(150, 20);
            this.comboBaudRate.Name = "comboBaudRate";
            this.comboBaudRate.Size = new System.Drawing.Size(92, 21);
            this.comboBaudRate.TabIndex = 12;
            // 
            // btnConnect
            // 
            this.btnConnect.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnConnect.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConnect.Location = new System.Drawing.Point(22, 50);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(107, 38);
            this.btnConnect.TabIndex = 13;
            this.btnConnect.Text = "Connect";
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnDisconnect.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDisconnect.Location = new System.Drawing.Point(135, 50);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(107, 38);
            this.btnDisconnect.TabIndex = 41;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // COMPortCombo
            // 
            this.COMPortCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.COMPortCombo.Location = new System.Drawing.Point(22, 20);
            this.COMPortCombo.Name = "COMPortCombo";
            this.COMPortCombo.Size = new System.Drawing.Size(82, 21);
            this.COMPortCombo.TabIndex = 10;
            // 
            // objGraph
            // 
            this.objGraph.EditButtons = System.Windows.Forms.MouseButtons.Left;
            this.objGraph.IsEnableSelection = true;
            this.objGraph.Location = new System.Drawing.Point(6, 120);
            this.objGraph.Name = "objGraph";
            this.objGraph.PanModifierKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)));
            this.objGraph.ScrollGrace = 0D;
            this.objGraph.ScrollMaxX = 0D;
            this.objGraph.ScrollMaxY = 0D;
            this.objGraph.ScrollMaxY2 = 0D;
            this.objGraph.ScrollMinX = 0D;
            this.objGraph.ScrollMinY = 0D;
            this.objGraph.ScrollMinY2 = 0D;
            this.objGraph.Size = new System.Drawing.Size(920, 499);
            this.objGraph.TabIndex = 49;
            this.objGraph.ContextMenuBuilder += new ZedGraph.ZedGraphControl.ContextMenuBuilderEventHandler(this.objGraph_ContextMenuBuilder);
            // 
            // groupSettings
            // 
            this.groupSettings.Controls.Add(this.label8);
            this.groupSettings.Controls.Add(this.m_sBottomDBM);
            this.groupSettings.Controls.Add(this.label7);
            this.groupSettings.Controls.Add(this.m_sTopDBM);
            this.groupSettings.Controls.Add(this.label6);
            this.groupSettings.Controls.Add(this.m_sEndFreq);
            this.groupSettings.Controls.Add(this.label5);
            this.groupSettings.Controls.Add(this.m_sFreqSpan);
            this.groupSettings.Controls.Add(this.label4);
            this.groupSettings.Controls.Add(this.m_sStartFreq);
            this.groupSettings.Controls.Add(this.label3);
            this.groupSettings.Controls.Add(this.m_sCenterFreq);
            this.groupSettings.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupSettings.Location = new System.Drawing.Point(514, 7);
            this.groupSettings.Name = "groupSettings";
            this.groupSettings.Size = new System.Drawing.Size(276, 107);
            this.groupSettings.TabIndex = 48;
            this.groupSettings.TabStop = false;
            this.groupSettings.Text = "Frequency and Power Settings";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 77);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(64, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "Bottom dBm";
            // 
            // m_sBottomDBM
            // 
            this.m_sBottomDBM.Location = new System.Drawing.Point(77, 74);
            this.m_sBottomDBM.Name = "m_sBottomDBM";
            this.m_sBottomDBM.Size = new System.Drawing.Size(55, 21);
            this.m_sBottomDBM.TabIndex = 6;
            this.m_sBottomDBM.Text = "-120";
            this.m_sBottomDBM.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_sBottomDBM.Leave += new System.EventHandler(this.UpdateSettingsFromDialog);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(152, 77);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(48, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "Top dBm";
            // 
            // m_sTopDBM
            // 
            this.m_sTopDBM.Location = new System.Drawing.Point(209, 74);
            this.m_sTopDBM.Name = "m_sTopDBM";
            this.m_sTopDBM.Size = new System.Drawing.Size(54, 21);
            this.m_sTopDBM.TabIndex = 5;
            this.m_sTopDBM.Text = "-20";
            this.m_sTopDBM.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_sTopDBM.Leave += new System.EventHandler(this.UpdateSettingsFromDialog);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.DarkBlue;
            this.label6.Location = new System.Drawing.Point(152, 52);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(50, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "End Freq";
            // 
            // m_sEndFreq
            // 
            this.m_sEndFreq.ForeColor = System.Drawing.Color.DarkBlue;
            this.m_sEndFreq.Location = new System.Drawing.Point(209, 48);
            this.m_sEndFreq.Name = "m_sEndFreq";
            this.m_sEndFreq.Size = new System.Drawing.Size(54, 21);
            this.m_sEndFreq.TabIndex = 4;
            this.m_sEndFreq.Text = "437.000";
            this.m_sEndFreq.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_sEndFreq.Leave += new System.EventHandler(this.UpdateSettingsFromDialog);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Green;
            this.label5.Location = new System.Drawing.Point(146, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Freq Span";
            // 
            // m_sFreqSpan
            // 
            this.m_sFreqSpan.ForeColor = System.Drawing.Color.Green;
            this.m_sFreqSpan.Location = new System.Drawing.Point(209, 21);
            this.m_sFreqSpan.Name = "m_sFreqSpan";
            this.m_sFreqSpan.Size = new System.Drawing.Size(54, 21);
            this.m_sFreqSpan.TabIndex = 2;
            this.m_sFreqSpan.Text = "4.000";
            this.m_sFreqSpan.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_sFreqSpan.Leave += new System.EventHandler(this.UpdateSettingsFromDialog);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.DarkBlue;
            this.label4.Location = new System.Drawing.Point(15, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Start Freq";
            // 
            // m_sStartFreq
            // 
            this.m_sStartFreq.ForeColor = System.Drawing.Color.DarkBlue;
            this.m_sStartFreq.Location = new System.Drawing.Point(77, 48);
            this.m_sStartFreq.Name = "m_sStartFreq";
            this.m_sStartFreq.Size = new System.Drawing.Size(55, 21);
            this.m_sStartFreq.TabIndex = 3;
            this.m_sStartFreq.Text = "433.000";
            this.m_sStartFreq.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_sStartFreq.Leave += new System.EventHandler(this.UpdateSettingsFromDialog);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Green;
            this.label3.Location = new System.Drawing.Point(6, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Center Freq";
            // 
            // m_sCenterFreq
            // 
            this.m_sCenterFreq.ForeColor = System.Drawing.Color.Green;
            this.m_sCenterFreq.Location = new System.Drawing.Point(77, 21);
            this.m_sCenterFreq.Name = "m_sCenterFreq";
            this.m_sCenterFreq.Size = new System.Drawing.Size(55, 21);
            this.m_sCenterFreq.TabIndex = 1;
            this.m_sCenterFreq.Text = "435.000";
            this.m_sCenterFreq.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_sCenterFreq.Leave += new System.EventHandler(this.UpdateSettingsFromDialog);
            // 
            // tabReport
            // 
            this.tabReport.Controls.Add(this.textBox_message);
            this.tabReport.Location = new System.Drawing.Point(4, 26);
            this.tabReport.Name = "tabReport";
            this.tabReport.Padding = new System.Windows.Forms.Padding(3);
            this.tabReport.Size = new System.Drawing.Size(932, 647);
            this.tabReport.TabIndex = 1;
            this.tabReport.Text = "Report";
            this.tabReport.UseVisualStyleBackColor = true;
            this.tabReport.Enter += new System.EventHandler(this.tabReport_Enter);
            // 
            // textBox_message
            // 
            this.textBox_message.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.textBox_message.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_message.ForeColor = System.Drawing.Color.DarkBlue;
            this.textBox_message.Location = new System.Drawing.Point(6, 120);
            this.textBox_message.Multiline = true;
            this.textBox_message.Name = "textBox_message";
            this.textBox_message.ReadOnly = true;
            this.textBox_message.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_message.Size = new System.Drawing.Size(920, 519);
            this.textBox_message.TabIndex = 49;
            this.textBox_message.WordWrap = false;
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(188, 22);
            this.toolStripMenuItem1.Text = "Reinitialize &Data...";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // SaveCSVtoolStripMenuItem
            // 
            this.SaveCSVtoolStripMenuItem.Name = "SaveCSVtoolStripMenuItem";
            this.SaveCSVtoolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.SaveCSVtoolStripMenuItem.Text = "Save CS&V As...";
            this.SaveCSVtoolStripMenuItem.Click += new System.EventHandler(this.SaveCSVtoolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.Info;
            this.ClientSize = new System.Drawing.Size(940, 704);
            this.Controls.Add(this.MainTab);
            this.Controls.Add(this.MainMenu);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.MainMenu;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "  RF Explorer Windows Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            this.MainTab.ResumeLayout(false);
            this.tabSpectrumAnalyzer.ResumeLayout(false);
            this.tabSpectrumAnalyzer.PerformLayout();
            this.groupDataFeed.ResumeLayout(false);
            this.groupDataFeed.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericIterations)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown)).EndInit();
            this.MainStatusBar.ResumeLayout(false);
            this.MainStatusBar.PerformLayout();
            this.groupCOM.ResumeLayout(false);
            this.groupCOM.PerformLayout();
            this.groupSettings.ResumeLayout(false);
            this.groupSettings.PerformLayout();
            this.tabReport.ResumeLayout(false);
            this.tabReport.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip MainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Timer m_timer_receive;
        private System.IO.Ports.SerialPort m_serialPortObj;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem realtimeDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem averagedDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem maxDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem minDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemLoad;
        private System.Windows.Forms.ToolStripMenuItem saveOnCloseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuPortInfo;
        private System.Windows.Forms.TabControl MainTab;
        private System.Windows.Forms.TabPage tabSpectrumAnalyzer;
        private System.Windows.Forms.GroupBox groupDataFeed;
        private System.Windows.Forms.NumericUpDown numericIterations;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkRunMode;
        private System.Windows.Forms.CheckBox chkHoldMode;
        private System.Windows.Forms.NumericUpDown numericUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.StatusStrip MainStatusBar;
        private System.Windows.Forms.ToolStripStatusLabel toolCOMStatus;
        private System.Windows.Forms.ToolStripProgressBar toolStripMemory;
        private System.Windows.Forms.ToolStripStatusLabel toolFile;
        private System.Windows.Forms.GroupBox groupCOM;
        private System.Windows.Forms.Button btnRescan;
        private System.Windows.Forms.ComboBox comboBaudRate;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.ComboBox COMPortCombo;
        private ZedGraph.ZedGraphControl objGraph;
        private System.Windows.Forms.GroupBox groupSettings;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox m_sBottomDBM;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox m_sTopDBM;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox m_sEndFreq;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox m_sFreqSpan;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox m_sStartFreq;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox m_sCenterFreq;
        private System.Windows.Forms.TabPage tabReport;
        private System.Windows.Forms.TextBox textBox_message;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem toolStripCleanReport;
        private System.Windows.Forms.CheckBox chkCalcMin;
        private System.Windows.Forms.CheckBox chkCalcMax;
        private System.Windows.Forms.CheckBox chkCalcAverage;
        private System.Windows.Forms.CheckBox chkCalcRealtime;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem SaveCSVtoolStripMenuItem;
    }
}

