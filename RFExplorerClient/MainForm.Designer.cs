//============================================================================
//RF Explorer PC Client - A Handheld Spectrum Analyzer for everyone!
//Copyright © 2010-11 Ariel Rocholl, www.rf-explorer.com
//
//This application is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 3.0 of the License, or (at your option) any later version.
//
//This software is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//General Public License for more details.
//
//You should have received a copy of the GNU General Public
//License along with this library; if not, write to the Free Software
//Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//=============================================================================

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
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.SaveCSVtoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.menu_SaveRFS = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_LoadRFS = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveImagetoolStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.saveOnCloseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
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
            this.mnuItem_ShowPeak = new System.Windows.Forms.ToolStripMenuItem();
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
            this.btnReset = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
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
            this.tabRemoteScreen = new System.Windows.Forms.TabPage();
            this.groupRemoteScreen = new System.Windows.Forms.GroupBox();
            this.numScreenIndex = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.numericZoom = new System.Windows.Forms.NumericUpDown();
            this.chkDumpScreen = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.panelRemoteScreen = new System.Windows.Forms.Panel();
            this.tabReport = new System.Windows.Forms.TabPage();
            this.textBox_message = new System.Windows.Forms.TextBox();
            this.controlRemoteScreen = new RFEClientControls.RemoteScreenControl();
            this.MainMenu.SuspendLayout();
            this.MainTab.SuspendLayout();
            this.tabSpectrumAnalyzer.SuspendLayout();
            this.groupDataFeed.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericIterations)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown)).BeginInit();
            this.MainStatusBar.SuspendLayout();
            this.groupCOM.SuspendLayout();
            this.groupSettings.SuspendLayout();
            this.tabRemoteScreen.SuspendLayout();
            this.groupRemoteScreen.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numScreenIndex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericZoom)).BeginInit();
            this.panelRemoteScreen.SuspendLayout();
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
            this.toolStripSeparator4,
            this.SaveCSVtoolStripMenuItem,
            this.toolStripSeparator5,
            this.menu_SaveRFS,
            this.menu_LoadRFS,
            this.SaveImagetoolStrip,
            this.toolStripSeparator6,
            this.saveOnCloseToolStripMenuItem,
            this.toolStripMenuItem1,
            this.toolStripSeparator2,
            this.toolStripMenuPortInfo,
            this.aboutToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            this.fileToolStripMenuItem.DropDownOpening += new System.EventHandler(this.fileToolStripMenuItem_DropDownOpening);
            // 
            // toolStripMenuItemLoad
            // 
            this.toolStripMenuItemLoad.Name = "toolStripMenuItemLoad";
            this.toolStripMenuItemLoad.Size = new System.Drawing.Size(205, 22);
            this.toolStripMenuItemLoad.Text = "&Load RFE data file...";
            this.toolStripMenuItemLoad.Click += new System.EventHandler(this.toolStripMenuItemLoad_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.saveAsToolStripMenuItem.Text = "Sa&ve RFE data As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(202, 6);
            // 
            // SaveCSVtoolStripMenuItem
            // 
            this.SaveCSVtoolStripMenuItem.Name = "SaveCSVtoolStripMenuItem";
            this.SaveCSVtoolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.SaveCSVtoolStripMenuItem.Text = "Export CS&V As...";
            this.SaveCSVtoolStripMenuItem.Click += new System.EventHandler(this.SaveCSVtoolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(202, 6);
            // 
            // menu_SaveRFS
            // 
            this.menu_SaveRFS.Name = "menu_SaveRFS";
            this.menu_SaveRFS.Size = new System.Drawing.Size(205, 22);
            this.menu_SaveRFS.Text = "Save RFS Screen file As... ";
            this.menu_SaveRFS.Click += new System.EventHandler(this.menu_SaveRFS_Click);
            // 
            // menu_LoadRFS
            // 
            this.menu_LoadRFS.Name = "menu_LoadRFS";
            this.menu_LoadRFS.Size = new System.Drawing.Size(205, 22);
            this.menu_LoadRFS.Text = "Load RFS Screen file As...";
            this.menu_LoadRFS.Click += new System.EventHandler(this.menu_LoadRFS_Click);
            // 
            // SaveImagetoolStrip
            // 
            this.SaveImagetoolStrip.Name = "SaveImagetoolStrip";
            this.SaveImagetoolStrip.Size = new System.Drawing.Size(205, 22);
            this.SaveImagetoolStrip.Text = "Save Remote Ima&ge As...";
            this.SaveImagetoolStrip.Click += new System.EventHandler(this.SaveImagetoolStrip_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(202, 6);
            // 
            // saveOnCloseToolStripMenuItem
            // 
            this.saveOnCloseToolStripMenuItem.CheckOnClick = true;
            this.saveOnCloseToolStripMenuItem.Name = "saveOnCloseToolStripMenuItem";
            this.saveOnCloseToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.saveOnCloseToolStripMenuItem.Text = "&Save on Close";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(205, 22);
            this.toolStripMenuItem1.Text = "Reinitialize &Data...";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(202, 6);
            // 
            // toolStripMenuPortInfo
            // 
            this.toolStripMenuPortInfo.Name = "toolStripMenuPortInfo";
            this.toolStripMenuPortInfo.Size = new System.Drawing.Size(205, 22);
            this.toolStripMenuPortInfo.Text = "Report COM port &info";
            this.toolStripMenuPortInfo.Click += new System.EventHandler(this.toolStripMenuPortInfo_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.aboutToolStripMenuItem.Text = "&About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(202, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
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
            this.mnuItem_ShowPeak,
            this.toolStripSeparator3,
            this.toolStripCleanReport});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "&View";
            this.viewToolStripMenuItem.DropDownOpening += new System.EventHandler(this.viewToolStripMenuItem_DropDownOpening);
            // 
            // realtimeDataToolStripMenuItem
            // 
            this.realtimeDataToolStripMenuItem.CheckOnClick = true;
            this.realtimeDataToolStripMenuItem.Name = "realtimeDataToolStripMenuItem";
            this.realtimeDataToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.realtimeDataToolStripMenuItem.Text = "&Realtime data";
            this.realtimeDataToolStripMenuItem.Click += new System.EventHandler(this.click_view_mode);
            // 
            // averagedDataToolStripMenuItem
            // 
            this.averagedDataToolStripMenuItem.CheckOnClick = true;
            this.averagedDataToolStripMenuItem.Name = "averagedDataToolStripMenuItem";
            this.averagedDataToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.averagedDataToolStripMenuItem.Text = "&Averaged data";
            this.averagedDataToolStripMenuItem.Click += new System.EventHandler(this.click_view_mode);
            // 
            // maxDataToolStripMenuItem
            // 
            this.maxDataToolStripMenuItem.CheckOnClick = true;
            this.maxDataToolStripMenuItem.Name = "maxDataToolStripMenuItem";
            this.maxDataToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.maxDataToolStripMenuItem.Text = "Ma&x data";
            this.maxDataToolStripMenuItem.Click += new System.EventHandler(this.click_view_mode);
            // 
            // minDataToolStripMenuItem
            // 
            this.minDataToolStripMenuItem.CheckOnClick = true;
            this.minDataToolStripMenuItem.Name = "minDataToolStripMenuItem";
            this.minDataToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.minDataToolStripMenuItem.Text = "M&in data";
            this.minDataToolStripMenuItem.Click += new System.EventHandler(this.click_view_mode);
            // 
            // mnuItem_ShowPeak
            // 
            this.mnuItem_ShowPeak.CheckOnClick = true;
            this.mnuItem_ShowPeak.Name = "mnuItem_ShowPeak";
            this.mnuItem_ShowPeak.Size = new System.Drawing.Size(167, 22);
            this.mnuItem_ShowPeak.Text = "Show Peak values";
            this.mnuItem_ShowPeak.CheckedChanged += new System.EventHandler(this.mnuItem_ShowPeak_CheckedChanged);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(164, 6);
            // 
            // toolStripCleanReport
            // 
            this.toolStripCleanReport.Name = "toolStripCleanReport";
            this.toolStripCleanReport.Size = new System.Drawing.Size(167, 22);
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
            this.MainTab.Controls.Add(this.tabRemoteScreen);
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
            this.chkCalcMin.Size = new System.Drawing.Size(66, 17);
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
            this.chkCalcMax.Size = new System.Drawing.Size(72, 17);
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
            this.chkCalcAverage.Size = new System.Drawing.Size(67, 17);
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
            this.toolCOMStatus.Size = new System.Drawing.Size(79, 17);
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
            this.toolFile.Size = new System.Drawing.Size(58, 17);
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
            this.groupSettings.Controls.Add(this.btnReset);
            this.groupSettings.Controls.Add(this.btnSend);
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
            this.groupSettings.Size = new System.Drawing.Size(410, 107);
            this.groupSettings.TabIndex = 48;
            this.groupSettings.TabStop = false;
            this.groupSettings.Text = "Remote Frequency and Power control";
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(348, 21);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(56, 34);
            this.btnReset.TabIndex = 13;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(348, 61);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(56, 34);
            this.btnSend.TabIndex = 12;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.label8.ForeColor = System.Drawing.Color.DarkBlue;
            this.label8.Location = new System.Drawing.Point(10, 83);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 16);
            this.label8.TabIndex = 11;
            this.label8.Text = "BOTTOM";
            // 
            // m_sBottomDBM
            // 
            this.m_sBottomDBM.BackColor = System.Drawing.Color.RoyalBlue;
            this.m_sBottomDBM.Font = new System.Drawing.Font("Digital-7", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.m_sBottomDBM.ForeColor = System.Drawing.Color.White;
            this.m_sBottomDBM.Location = new System.Drawing.Point(77, 78);
            this.m_sBottomDBM.Name = "m_sBottomDBM";
            this.m_sBottomDBM.Size = new System.Drawing.Size(98, 26);
            this.m_sBottomDBM.TabIndex = 6;
            this.m_sBottomDBM.Text = "-120";
            this.m_sBottomDBM.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_sBottomDBM.Leave += new System.EventHandler(this.m_sBottomDBM_Leave);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.label7.ForeColor = System.Drawing.Color.DarkBlue;
            this.label7.Location = new System.Drawing.Point(204, 83);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(32, 16);
            this.label7.TabIndex = 9;
            this.label7.Text = "TOP";
            // 
            // m_sTopDBM
            // 
            this.m_sTopDBM.BackColor = System.Drawing.Color.RoyalBlue;
            this.m_sTopDBM.Font = new System.Drawing.Font("Digital-7", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.m_sTopDBM.ForeColor = System.Drawing.Color.White;
            this.m_sTopDBM.Location = new System.Drawing.Point(242, 78);
            this.m_sTopDBM.Name = "m_sTopDBM";
            this.m_sTopDBM.Size = new System.Drawing.Size(98, 26);
            this.m_sTopDBM.TabIndex = 5;
            this.m_sTopDBM.Text = "-20";
            this.m_sTopDBM.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.label6.ForeColor = System.Drawing.Color.DarkBlue;
            this.label6.Location = new System.Drawing.Point(204, 54);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 16);
            this.label6.TabIndex = 7;
            this.label6.Text = "END";
            // 
            // m_sEndFreq
            // 
            this.m_sEndFreq.BackColor = System.Drawing.Color.RoyalBlue;
            this.m_sEndFreq.Font = new System.Drawing.Font("Digital-7", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.m_sEndFreq.ForeColor = System.Drawing.Color.White;
            this.m_sEndFreq.Location = new System.Drawing.Point(242, 49);
            this.m_sEndFreq.Name = "m_sEndFreq";
            this.m_sEndFreq.Size = new System.Drawing.Size(98, 26);
            this.m_sEndFreq.TabIndex = 4;
            this.m_sEndFreq.Text = "437.000";
            this.m_sEndFreq.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_sEndFreq.Leave += new System.EventHandler(this.m_sEndFreq_Leave);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.label5.ForeColor = System.Drawing.Color.DarkBlue;
            this.label5.Location = new System.Drawing.Point(194, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(42, 16);
            this.label5.TabIndex = 5;
            this.label5.Text = "SPAN";
            // 
            // m_sFreqSpan
            // 
            this.m_sFreqSpan.BackColor = System.Drawing.Color.RoyalBlue;
            this.m_sFreqSpan.Font = new System.Drawing.Font("Digital-7", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.m_sFreqSpan.ForeColor = System.Drawing.Color.White;
            this.m_sFreqSpan.Location = new System.Drawing.Point(242, 20);
            this.m_sFreqSpan.Name = "m_sFreqSpan";
            this.m_sFreqSpan.Size = new System.Drawing.Size(98, 26);
            this.m_sFreqSpan.TabIndex = 2;
            this.m_sFreqSpan.Text = "4.000";
            this.m_sFreqSpan.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_sFreqSpan.Leave += new System.EventHandler(this.m_sFreqSpan_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.DarkBlue;
            this.label4.Location = new System.Drawing.Point(20, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 16);
            this.label4.TabIndex = 3;
            this.label4.Text = "START";
            // 
            // m_sStartFreq
            // 
            this.m_sStartFreq.BackColor = System.Drawing.Color.RoyalBlue;
            this.m_sStartFreq.Font = new System.Drawing.Font("Digital-7", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.m_sStartFreq.ForeColor = System.Drawing.Color.White;
            this.m_sStartFreq.Location = new System.Drawing.Point(77, 49);
            this.m_sStartFreq.Name = "m_sStartFreq";
            this.m_sStartFreq.Size = new System.Drawing.Size(98, 26);
            this.m_sStartFreq.TabIndex = 3;
            this.m_sStartFreq.Text = "433.000";
            this.m_sStartFreq.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_sStartFreq.Leave += new System.EventHandler(this.m_sStartFreq_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.DarkBlue;
            this.label3.Location = new System.Drawing.Point(15, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 16);
            this.label3.TabIndex = 1;
            this.label3.Text = "CENTER";
            // 
            // m_sCenterFreq
            // 
            this.m_sCenterFreq.BackColor = System.Drawing.Color.RoyalBlue;
            this.m_sCenterFreq.Font = new System.Drawing.Font("Digital-7", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_sCenterFreq.ForeColor = System.Drawing.Color.White;
            this.m_sCenterFreq.Location = new System.Drawing.Point(77, 21);
            this.m_sCenterFreq.Name = "m_sCenterFreq";
            this.m_sCenterFreq.Size = new System.Drawing.Size(98, 26);
            this.m_sCenterFreq.TabIndex = 1;
            this.m_sCenterFreq.Text = "435.000";
            this.m_sCenterFreq.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_sCenterFreq.Leave += new System.EventHandler(this.m_sCenterFreq_Leave);
            // 
            // tabRemoteScreen
            // 
            this.tabRemoteScreen.Controls.Add(this.groupRemoteScreen);
            this.tabRemoteScreen.Controls.Add(this.panelRemoteScreen);
            this.tabRemoteScreen.Location = new System.Drawing.Point(4, 26);
            this.tabRemoteScreen.Name = "tabRemoteScreen";
            this.tabRemoteScreen.Size = new System.Drawing.Size(932, 647);
            this.tabRemoteScreen.TabIndex = 2;
            this.tabRemoteScreen.Text = "Remote Screen";
            this.tabRemoteScreen.Paint += new System.Windows.Forms.PaintEventHandler(this.tabRemoteScreen_Paint);
            this.tabRemoteScreen.Enter += new System.EventHandler(this.tabRemoteScreen_Enter);
            // 
            // groupRemoteScreen
            // 
            this.groupRemoteScreen.Controls.Add(this.numScreenIndex);
            this.groupRemoteScreen.Controls.Add(this.label9);
            this.groupRemoteScreen.Controls.Add(this.numericZoom);
            this.groupRemoteScreen.Controls.Add(this.chkDumpScreen);
            this.groupRemoteScreen.Controls.Add(this.label10);
            this.groupRemoteScreen.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupRemoteScreen.Location = new System.Drawing.Point(272, 6);
            this.groupRemoteScreen.Name = "groupRemoteScreen";
            this.groupRemoteScreen.Size = new System.Drawing.Size(451, 108);
            this.groupRemoteScreen.TabIndex = 53;
            this.groupRemoteScreen.TabStop = false;
            this.groupRemoteScreen.Text = "Dump Remote Screen";
            // 
            // numScreenIndex
            // 
            this.numScreenIndex.Font = new System.Drawing.Font("Digital-7", 18F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numScreenIndex.Location = new System.Drawing.Point(93, 55);
            this.numScreenIndex.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.numScreenIndex.Name = "numScreenIndex";
            this.numScreenIndex.Size = new System.Drawing.Size(70, 31);
            this.numScreenIndex.TabIndex = 51;
            this.numScreenIndex.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numScreenIndex.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numScreenIndex.ValueChanged += new System.EventHandler(this.numScreenIndex_ValueChanged);
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(10, 61);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(77, 22);
            this.label9.TabIndex = 52;
            this.label9.Text = "Sample";
            // 
            // numericZoom
            // 
            this.numericZoom.Font = new System.Drawing.Font("Digital-7", 18F, System.Drawing.FontStyle.Italic);
            this.numericZoom.Location = new System.Drawing.Point(241, 55);
            this.numericZoom.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.numericZoom.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericZoom.Name = "numericZoom";
            this.numericZoom.Size = new System.Drawing.Size(60, 31);
            this.numericZoom.TabIndex = 17;
            this.numericZoom.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericZoom.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericZoom.ValueChanged += new System.EventHandler(this.numericZoom_ValueChanged);
            // 
            // chkDumpScreen
            // 
            this.chkDumpScreen.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkDumpScreen.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.chkDumpScreen.Location = new System.Drawing.Point(14, 20);
            this.chkDumpScreen.MinimumSize = new System.Drawing.Size(60, 0);
            this.chkDumpScreen.Name = "chkDumpScreen";
            this.chkDumpScreen.Size = new System.Drawing.Size(204, 29);
            this.chkDumpScreen.TabIndex = 14;
            this.chkDumpScreen.Text = "Remote Dump active";
            this.chkDumpScreen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkDumpScreen.UseVisualStyleBackColor = true;
            this.chkDumpScreen.CheckedChanged += new System.EventHandler(this.chkDumpScreen_CheckedChanged);
            // 
            // label10
            // 
            this.label10.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.label10.Location = new System.Drawing.Point(176, 61);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(59, 21);
            this.label10.TabIndex = 50;
            this.label10.Text = "Zoom";
            // 
            // panelRemoteScreen
            // 
            this.panelRemoteScreen.Controls.Add(this.controlRemoteScreen);
            this.panelRemoteScreen.Location = new System.Drawing.Point(12, 140);
            this.panelRemoteScreen.Name = "panelRemoteScreen";
            this.panelRemoteScreen.Size = new System.Drawing.Size(910, 462);
            this.panelRemoteScreen.TabIndex = 55;
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
            // controlRemoteScreen
            // 
            this.controlRemoteScreen.Location = new System.Drawing.Point(0, 0);
            this.controlRemoteScreen.Name = "controlRemoteScreen";
            this.controlRemoteScreen.Size = new System.Drawing.Size(292, 174);
            this.controlRemoteScreen.TabIndex = 54;
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
            this.tabRemoteScreen.ResumeLayout(false);
            this.groupRemoteScreen.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numScreenIndex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericZoom)).EndInit();
            this.panelRemoteScreen.ResumeLayout(false);
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
        private System.Windows.Forms.TabPage tabRemoteScreen;
        private System.Windows.Forms.GroupBox groupRemoteScreen;
        private System.Windows.Forms.NumericUpDown numericZoom;
        private System.Windows.Forms.CheckBox chkDumpScreen;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown numScreenIndex;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ToolStripMenuItem SaveImagetoolStrip;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem menu_SaveRFS;
        private System.Windows.Forms.ToolStripMenuItem menu_LoadRFS;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.ToolStripMenuItem mnuItem_ShowPeak;
        private RFEClientControls.RemoteScreenControl controlRemoteScreen;
        private System.Windows.Forms.Panel panelRemoteScreen;
    }
}

