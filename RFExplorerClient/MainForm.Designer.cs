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
            this.MainFileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLoadRFE = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSaveAsRFE = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.menuSaveCSV = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.menuSaveRFS = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLoadRFS = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSaveRemoteImage = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.menuAutoLCDOff = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSaveOnClose = new System.Windows.Forms.ToolStripMenuItem();
            this.menuReinitializeData = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuPortInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MainViewMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDarkMode = new System.Windows.Forms.ToolStripMenuItem();
            this.menuShowControlArea = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.menuRealtimeData = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAveragedData = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMaxData = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMinData = new System.Windows.Forms.ToolStripMenuItem();
            this.menuShowPeak = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.menuCleanReport = new System.Windows.Forms.ToolStripMenuItem();
            this.m_timer_receive = new System.Windows.Forms.Timer(this.components);
            this.m_serialPortObj = new System.IO.Ports.SerialPort(this.components);
            this.MainTab = new System.Windows.Forms.TabControl();
            this.tabSpectrumAnalyzer = new System.Windows.Forms.TabPage();
            this.btnCenterMark = new System.Windows.Forms.Button();
            this.btnSpanMin = new System.Windows.Forms.Button();
            this.btnSpanDefault = new System.Windows.Forms.Button();
            this.btnSpanMax = new System.Windows.Forms.Button();
            this.btnBottom5minus = new System.Windows.Forms.Button();
            this.btnBottom5plus = new System.Windows.Forms.Button();
            this.btnTop5minus = new System.Windows.Forms.Button();
            this.btnSpanDec = new System.Windows.Forms.Button();
            this.btnSpanInc = new System.Windows.Forms.Button();
            this.btnMoveFreqDecSmall = new System.Windows.Forms.Button();
            this.btnMoveFreqIncSmall = new System.Windows.Forms.Button();
            this.btnTop5plus = new System.Windows.Forms.Button();
            this.btnMoveFreqDecLarge = new System.Windows.Forms.Button();
            this.btnMoveFreqIncLarge = new System.Windows.Forms.Button();
            this.groupDataFeed = new System.Windows.Forms.GroupBox();
            this.chkCalcMin = new System.Windows.Forms.CheckBox();
            this.chkCalcMax = new System.Windows.Forms.CheckBox();
            this.chkCalcAverage = new System.Windows.Forms.CheckBox();
            this.chkCalcRealtime = new System.Windows.Forms.CheckBox();
            this.numericIterations = new System.Windows.Forms.NumericUpDown();
            this.numericSampleSA = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.chkRunMode = new System.Windows.Forms.CheckBox();
            this.chkHoldMode = new System.Windows.Forms.CheckBox();
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
            this.zedSpectrumAnalyzer = new ZedGraph.ZedGraphControl();
            this.groupFreqSettings = new System.Windows.Forms.GroupBox();
            this.m_sBottomDBM = new System.Windows.Forms.TextBox();
            this.m_sTopDBM = new System.Windows.Forms.TextBox();
            this.m_sEndFreq = new System.Windows.Forms.TextBox();
            this.m_sFreqSpan = new System.Windows.Forms.TextBox();
            this.m_sStartFreq = new System.Windows.Forms.TextBox();
            this.m_sCenterFreq = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.tabWaterfall = new System.Windows.Forms.TabPage();
            this.waterfallGroupBox = new System.Windows.Forms.GroupBox();
            this.checkBoxEnableLCD = new System.Windows.Forms.CheckBox();
            this.numericContrast = new System.Windows.Forms.NumericUpDown();
            this.numericSensitivity = new System.Windows.Forms.NumericUpDown();
            this.labelSensitivity = new System.Windows.Forms.Label();
            this.labelContrast = new System.Windows.Forms.Label();
            this.panelWaterfall = new System.Windows.Forms.Panel();
            this.controlWaterfall = new RFEClientControls.WaterfallControl();
            this.tabRemoteScreen = new System.Windows.Forms.TabPage();
            this.groupRemoteScreen = new System.Windows.Forms.GroupBox();
            this.numVideoFPS = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.numericZoom = new System.Windows.Forms.NumericUpDown();
            this.numScreenIndex = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.btnSaveRemoteVideo = new System.Windows.Forms.Button();
            this.btnSaveRemoteBitmap = new System.Windows.Forms.Button();
            this.chkDumpScreen = new System.Windows.Forms.CheckBox();
            this.panelRemoteScreen = new System.Windows.Forms.Panel();
            this.controlRemoteScreen = new RFEClientControls.RemoteScreenControl();
            this.tabConfiguration = new System.Windows.Forms.TabPage();
            this.panelConfiguration = new System.Windows.Forms.Panel();
            this.groupCalibration = new System.Windows.Forms.GroupBox();
            this.btnCalibrate = new System.Windows.Forms.Button();
            this.m_edCalibrationFreq = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.tabReport = new System.Windows.Forms.TabPage();
            this.groupCommands = new System.Windows.Forms.GroupBox();
            this.btnSendCustomCmd = new System.Windows.Forms.Button();
            this.comboStdCmd = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.comboCustomCommand = new System.Windows.Forms.ComboBox();
            this.btnSendCmd = new System.Windows.Forms.Button();
            this.textBox_message = new System.Windows.Forms.TextBox();
            this.trackBarSensitivity = new System.Windows.Forms.TrackBar();
            this.trackBarContrast = new System.Windows.Forms.TrackBar();
            this.chkPSK = new System.Windows.Forms.RadioButton();
            this.chkOOK = new System.Windows.Forms.RadioButton();
            this.m_sBaudRate = new System.Windows.Forms.TextBox();
            this.m_sRefFrequency = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.groupRAWDecoder = new System.Windows.Forms.GroupBox();
            this.chkRunDecoder = new System.Windows.Forms.CheckBox();
            this.chkHoldDecoder = new System.Windows.Forms.CheckBox();
            this.btnSaveRAWDecoderCSV = new System.Windows.Forms.Button();
            this.numMultiGraph = new System.Windows.Forms.NumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
            this.numSampleDecoder = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.zedRAWDecoder = new ZedGraph.ZedGraphControl();
            this.MainMenu.SuspendLayout();
            this.MainTab.SuspendLayout();
            this.tabSpectrumAnalyzer.SuspendLayout();
            this.groupDataFeed.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericIterations)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericSampleSA)).BeginInit();
            this.MainStatusBar.SuspendLayout();
            this.groupCOM.SuspendLayout();
            this.groupFreqSettings.SuspendLayout();
            this.tabWaterfall.SuspendLayout();
            this.waterfallGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericContrast)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericSensitivity)).BeginInit();
            this.panelWaterfall.SuspendLayout();
            this.tabRemoteScreen.SuspendLayout();
            this.groupRemoteScreen.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numVideoFPS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericZoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numScreenIndex)).BeginInit();
            this.panelRemoteScreen.SuspendLayout();
            this.tabConfiguration.SuspendLayout();
            this.panelConfiguration.SuspendLayout();
            this.groupCalibration.SuspendLayout();
            this.tabReport.SuspendLayout();
            this.groupCommands.SuspendLayout();
            this.tabRAWDecoder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSensitivity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarContrast)).BeginInit();
            this.groupDemodulator.SuspendLayout();
            this.groupRAWDecoder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMultiGraph)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSampleDecoder)).BeginInit();
            this.SuspendLayout();
            // 
            // MainMenu
            // 
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MainFileMenu,
            this.MainViewMenu});
            this.MainMenu.Location = new System.Drawing.Point(0, 0);
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.MainMenu.Size = new System.Drawing.Size(940, 24);
            this.MainMenu.TabIndex = 46;
            this.MainMenu.Text = "menu";
            // 
            // MainFileMenu
            // 
            this.MainFileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuLoadRFE,
            this.menuSaveAsRFE,
            this.toolStripSeparator4,
            this.menuSaveCSV,
            this.toolStripSeparator5,
            this.menuSaveRFS,
            this.menuLoadRFS,
            this.menuSaveRemoteImage,
            this.toolStripSeparator6,
            this.menuAutoLCDOff,
            this.menuSaveOnClose,
            this.menuReinitializeData,
            this.toolStripSeparator2,
            this.menuPortInfo,
            this.menuAbout,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.MainFileMenu.Name = "MainFileMenu";
            this.MainFileMenu.Size = new System.Drawing.Size(37, 20);
            this.MainFileMenu.Text = "&File";
            this.MainFileMenu.DropDownOpening += new System.EventHandler(this.fileToolStripMenuItem_DropDownOpening);
            // 
            // menuLoadRFE
            // 
            this.menuLoadRFE.Name = "menuLoadRFE";
            this.menuLoadRFE.Size = new System.Drawing.Size(211, 22);
            this.menuLoadRFE.Text = "&Load RFE data file...";
            this.menuLoadRFE.Click += new System.EventHandler(this.toolStripMenuItemLoad_Click);
            // 
            // menuSaveAsRFE
            // 
            this.menuSaveAsRFE.Name = "menuSaveAsRFE";
            this.menuSaveAsRFE.Size = new System.Drawing.Size(211, 22);
            this.menuSaveAsRFE.Text = "Sa&ve RFE data As...";
            this.menuSaveAsRFE.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(208, 6);
            // 
            // menuSaveCSV
            // 
            this.menuSaveCSV.Name = "menuSaveCSV";
            this.menuSaveCSV.Size = new System.Drawing.Size(211, 22);
            this.menuSaveCSV.Text = "Export CS&V As...";
            this.menuSaveCSV.Click += new System.EventHandler(this.SaveCSVtoolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(208, 6);
            // 
            // menuSaveRFS
            // 
            this.menuSaveRFS.Name = "menuSaveRFS";
            this.menuSaveRFS.Size = new System.Drawing.Size(211, 22);
            this.menuSaveRFS.Text = "Sav&e RFS Screen file As... ";
            this.menuSaveRFS.Click += new System.EventHandler(this.menu_SaveRFS_Click);
            // 
            // menuLoadRFS
            // 
            this.menuLoadRFS.Name = "menuLoadRFS";
            this.menuLoadRFS.Size = new System.Drawing.Size(211, 22);
            this.menuLoadRFS.Text = "Load &RFS Screen file As...";
            this.menuLoadRFS.Click += new System.EventHandler(this.menu_LoadRFS_Click);
            // 
            // menuSaveRemoteImage
            // 
            this.menuSaveRemoteImage.Name = "menuSaveRemoteImage";
            this.menuSaveRemoteImage.Size = new System.Drawing.Size(211, 22);
            this.menuSaveRemoteImage.Text = "Save Remote Ima&ge As...";
            this.menuSaveRemoteImage.Click += new System.EventHandler(this.SaveImagetoolStrip_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(208, 6);
            // 
            // menuAutoLCDOff
            // 
            this.menuAutoLCDOff.CheckOnClick = true;
            this.menuAutoLCDOff.Name = "menuAutoLCDOff";
            this.menuAutoLCDOff.Size = new System.Drawing.Size(211, 22);
            this.menuAutoLCDOff.Text = "Automatic LCD O&FF";
            this.menuAutoLCDOff.Click += new System.EventHandler(this.menuAutoLCDOff_Click);
            // 
            // menuSaveOnClose
            // 
            this.menuSaveOnClose.CheckOnClick = true;
            this.menuSaveOnClose.Name = "menuSaveOnClose";
            this.menuSaveOnClose.Size = new System.Drawing.Size(211, 22);
            this.menuSaveOnClose.Text = "&Save on Close";
            // 
            // menuReinitializeData
            // 
            this.menuReinitializeData.Name = "menuReinitializeData";
            this.menuReinitializeData.Size = new System.Drawing.Size(211, 22);
            this.menuReinitializeData.Text = "Reinitialize &Data...";
            this.menuReinitializeData.Click += new System.EventHandler(this.menuReinitializeData_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(208, 6);
            // 
            // menuPortInfo
            // 
            this.menuPortInfo.Name = "menuPortInfo";
            this.menuPortInfo.Size = new System.Drawing.Size(211, 22);
            this.menuPortInfo.Text = "Report COM port &info";
            this.menuPortInfo.Click += new System.EventHandler(this.toolStripMenuPortInfo_Click);
            // 
            // menuAbout
            // 
            this.menuAbout.Name = "menuAbout";
            this.menuAbout.Size = new System.Drawing.Size(211, 22);
            this.menuAbout.Text = "A&bout RF Explorer Client...";
            this.menuAbout.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(208, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // MainViewMenu
            // 
            this.MainViewMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuDarkMode,
            this.menuShowControlArea,
            this.toolStripSeparator7,
            this.menuRealtimeData,
            this.menuAveragedData,
            this.menuMaxData,
            this.menuMinData,
            this.menuShowPeak,
            this.toolStripSeparator3,
            this.menuCleanReport});
            this.MainViewMenu.Name = "MainViewMenu";
            this.MainViewMenu.Size = new System.Drawing.Size(44, 20);
            this.MainViewMenu.Text = "&View";
            this.MainViewMenu.DropDownOpening += new System.EventHandler(this.MainMenuView_DropDownOpening);
            // 
            // menuDarkMode
            // 
            this.menuDarkMode.CheckOnClick = true;
            this.menuDarkMode.Name = "menuDarkMode";
            this.menuDarkMode.Size = new System.Drawing.Size(182, 22);
            this.menuDarkMode.Text = "Dar&k Color mode";
            this.menuDarkMode.Click += new System.EventHandler(this.menuDarkMode_Click);
            // 
            // menuShowControlArea
            // 
            this.menuShowControlArea.CheckOnClick = true;
            this.menuShowControlArea.Name = "menuShowControlArea";
            this.menuShowControlArea.Size = new System.Drawing.Size(182, 22);
            this.menuShowControlArea.Text = "&Display Control Area";
            this.menuShowControlArea.Click += new System.EventHandler(this.menuShowControlArea_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(179, 6);
            // 
            // menuRealtimeData
            // 
            this.menuRealtimeData.CheckOnClick = true;
            this.menuRealtimeData.Name = "menuRealtimeData";
            this.menuRealtimeData.Size = new System.Drawing.Size(182, 22);
            this.menuRealtimeData.Text = "&Realtime data";
            this.menuRealtimeData.Click += new System.EventHandler(this.click_view_mode);
            // 
            // menuAveragedData
            // 
            this.menuAveragedData.CheckOnClick = true;
            this.menuAveragedData.Name = "menuAveragedData";
            this.menuAveragedData.Size = new System.Drawing.Size(182, 22);
            this.menuAveragedData.Text = "&Averaged data";
            this.menuAveragedData.Click += new System.EventHandler(this.click_view_mode);
            // 
            // menuMaxData
            // 
            this.menuMaxData.CheckOnClick = true;
            this.menuMaxData.Name = "menuMaxData";
            this.menuMaxData.Size = new System.Drawing.Size(182, 22);
            this.menuMaxData.Text = "Ma&x data";
            this.menuMaxData.Click += new System.EventHandler(this.click_view_mode);
            // 
            // menuMinData
            // 
            this.menuMinData.CheckOnClick = true;
            this.menuMinData.Name = "menuMinData";
            this.menuMinData.Size = new System.Drawing.Size(182, 22);
            this.menuMinData.Text = "M&in data";
            this.menuMinData.Click += new System.EventHandler(this.click_view_mode);
            // 
            // menuShowPeak
            // 
            this.menuShowPeak.CheckOnClick = true;
            this.menuShowPeak.Name = "menuShowPeak";
            this.menuShowPeak.Size = new System.Drawing.Size(182, 22);
            this.menuShowPeak.Text = "Show Peak values";
            this.menuShowPeak.CheckedChanged += new System.EventHandler(this.mnuItem_ShowPeak_CheckedChanged);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(179, 6);
            // 
            // menuCleanReport
            // 
            this.menuCleanReport.Name = "menuCleanReport";
            this.menuCleanReport.Size = new System.Drawing.Size(182, 22);
            this.menuCleanReport.Text = "C&lean Report";
            this.menuCleanReport.Click += new System.EventHandler(this.toolStripCleanReport_Click);
            // 
            // m_timer_receive
            // 
            this.m_timer_receive.Interval = 50;
            this.m_timer_receive.Tick += new System.EventHandler(this.timer_receive_Tick);
            // 
            // MainTab
            // 
            this.MainTab.Controls.Add(this.tabSpectrumAnalyzer);
            this.MainTab.Controls.Add(this.tabWaterfall);
            this.MainTab.Controls.Add(this.tabRemoteScreen);
            this.MainTab.Controls.Add(this.tabConfiguration);
            this.MainTab.Controls.Add(this.tabReport);
            this.MainTab.Location = new System.Drawing.Point(0, 27);
            this.MainTab.Name = "MainTab";
            this.MainTab.Padding = new System.Drawing.Point(16, 5);
            this.MainTab.SelectedIndex = 0;
            this.MainTab.Size = new System.Drawing.Size(940, 540);
            this.MainTab.TabIndex = 49;
            // 
            // tabSpectrumAnalyzer
            // 
            this.tabSpectrumAnalyzer.Controls.Add(this.btnCenterMark);
            this.tabSpectrumAnalyzer.Controls.Add(this.btnSpanMin);
            this.tabSpectrumAnalyzer.Controls.Add(this.btnSpanDefault);
            this.tabSpectrumAnalyzer.Controls.Add(this.btnSpanMax);
            this.tabSpectrumAnalyzer.Controls.Add(this.btnBottom5minus);
            this.tabSpectrumAnalyzer.Controls.Add(this.btnBottom5plus);
            this.tabSpectrumAnalyzer.Controls.Add(this.btnTop5minus);
            this.tabSpectrumAnalyzer.Controls.Add(this.btnSpanDec);
            this.tabSpectrumAnalyzer.Controls.Add(this.btnSpanInc);
            this.tabSpectrumAnalyzer.Controls.Add(this.btnMoveFreqDecSmall);
            this.tabSpectrumAnalyzer.Controls.Add(this.btnMoveFreqIncSmall);
            this.tabSpectrumAnalyzer.Controls.Add(this.btnTop5plus);
            this.tabSpectrumAnalyzer.Controls.Add(this.btnMoveFreqDecLarge);
            this.tabSpectrumAnalyzer.Controls.Add(this.btnMoveFreqIncLarge);
            this.tabSpectrumAnalyzer.Controls.Add(this.groupDataFeed);
            this.tabSpectrumAnalyzer.Controls.Add(this.MainStatusBar);
            this.tabSpectrumAnalyzer.Controls.Add(this.groupCOM);
            this.tabSpectrumAnalyzer.Controls.Add(this.zedSpectrumAnalyzer);
            this.tabSpectrumAnalyzer.Controls.Add(this.groupFreqSettings);
            this.tabSpectrumAnalyzer.Location = new System.Drawing.Point(4, 26);
            this.tabSpectrumAnalyzer.Name = "tabSpectrumAnalyzer";
            this.tabSpectrumAnalyzer.Padding = new System.Windows.Forms.Padding(3);
            this.tabSpectrumAnalyzer.Size = new System.Drawing.Size(932, 510);
            this.tabSpectrumAnalyzer.TabIndex = 0;
            this.tabSpectrumAnalyzer.Text = "Spectrum Analyzer";
            this.tabSpectrumAnalyzer.UseVisualStyleBackColor = true;
            this.tabSpectrumAnalyzer.Enter += new System.EventHandler(this.tabSpectrumAnalyzer_Enter);
            // 
            // btnCenterMark
            // 
            this.btnCenterMark.Location = new System.Drawing.Point(426, 244);
            this.btnCenterMark.Name = "btnCenterMark";
            this.btnCenterMark.Size = new System.Drawing.Size(80, 23);
            this.btnCenterMark.TabIndex = 60;
            this.btnCenterMark.Text = "Center Mark";
            this.btnCenterMark.UseVisualStyleBackColor = true;
            this.btnCenterMark.Click += new System.EventHandler(this.btnCenterMark_Click);
            // 
            // btnSpanMin
            // 
            this.btnSpanMin.Location = new System.Drawing.Point(849, 316);
            this.btnSpanMin.Name = "btnSpanMin";
            this.btnSpanMin.Size = new System.Drawing.Size(80, 23);
            this.btnSpanMin.TabIndex = 59;
            this.btnSpanMin.Text = "Span Min";
            this.btnSpanMin.UseVisualStyleBackColor = true;
            this.btnSpanMin.Click += new System.EventHandler(this.btnSpanMin_Click);
            // 
            // btnSpanDefault
            // 
            this.btnSpanDefault.Location = new System.Drawing.Point(849, 288);
            this.btnSpanDefault.Name = "btnSpanDefault";
            this.btnSpanDefault.Size = new System.Drawing.Size(80, 23);
            this.btnSpanDefault.TabIndex = 58;
            this.btnSpanDefault.Text = "Span 10MHz";
            this.btnSpanDefault.UseVisualStyleBackColor = true;
            this.btnSpanDefault.Click += new System.EventHandler(this.btnSpanDefault_Click);
            // 
            // btnSpanMax
            // 
            this.btnSpanMax.Location = new System.Drawing.Point(849, 260);
            this.btnSpanMax.Name = "btnSpanMax";
            this.btnSpanMax.Size = new System.Drawing.Size(80, 23);
            this.btnSpanMax.TabIndex = 57;
            this.btnSpanMax.Text = "Span Max";
            this.btnSpanMax.UseVisualStyleBackColor = true;
            this.btnSpanMax.Click += new System.EventHandler(this.btnSpanMax_Click);
            // 
            // btnBottom5minus
            // 
            this.btnBottom5minus.Location = new System.Drawing.Point(849, 456);
            this.btnBottom5minus.Name = "btnBottom5minus";
            this.btnBottom5minus.Size = new System.Drawing.Size(80, 23);
            this.btnBottom5minus.TabIndex = 56;
            this.btnBottom5minus.Text = "Bottom -5dB";
            this.btnBottom5minus.UseVisualStyleBackColor = true;
            this.btnBottom5minus.Click += new System.EventHandler(this.btnBottom5minus_Click);
            // 
            // btnBottom5plus
            // 
            this.btnBottom5plus.Location = new System.Drawing.Point(849, 428);
            this.btnBottom5plus.Name = "btnBottom5plus";
            this.btnBottom5plus.Size = new System.Drawing.Size(80, 23);
            this.btnBottom5plus.TabIndex = 55;
            this.btnBottom5plus.Text = "Bottom +5dB";
            this.btnBottom5plus.UseVisualStyleBackColor = true;
            this.btnBottom5plus.Click += new System.EventHandler(this.btnBottom5plus_Click);
            // 
            // btnTop5minus
            // 
            this.btnTop5minus.Location = new System.Drawing.Point(849, 148);
            this.btnTop5minus.Name = "btnTop5minus";
            this.btnTop5minus.Size = new System.Drawing.Size(80, 23);
            this.btnTop5minus.TabIndex = 54;
            this.btnTop5minus.Text = "Top -5dB";
            this.btnTop5minus.UseVisualStyleBackColor = true;
            this.btnTop5minus.Click += new System.EventHandler(this.btnTop5minus_Click);
            // 
            // btnSpanDec
            // 
            this.btnSpanDec.Location = new System.Drawing.Point(849, 345);
            this.btnSpanDec.Name = "btnSpanDec";
            this.btnSpanDec.Size = new System.Drawing.Size(80, 23);
            this.btnSpanDec.TabIndex = 53;
            this.btnSpanDec.Text = "Span -25%";
            this.btnSpanDec.UseVisualStyleBackColor = true;
            this.btnSpanDec.Click += new System.EventHandler(this.btnSpanDec_Click);
            // 
            // btnSpanInc
            // 
            this.btnSpanInc.Location = new System.Drawing.Point(849, 233);
            this.btnSpanInc.Name = "btnSpanInc";
            this.btnSpanInc.Size = new System.Drawing.Size(80, 23);
            this.btnSpanInc.TabIndex = 53;
            this.btnSpanInc.Text = "Span +25%";
            this.btnSpanInc.UseVisualStyleBackColor = true;
            this.btnSpanInc.Click += new System.EventHandler(this.btnSpanInc_Click);
            // 
            // btnMoveFreqDecSmall
            // 
            this.btnMoveFreqDecSmall.Location = new System.Drawing.Point(849, 204);
            this.btnMoveFreqDecSmall.Name = "btnMoveFreqDecSmall";
            this.btnMoveFreqDecSmall.Size = new System.Drawing.Size(80, 23);
            this.btnMoveFreqDecSmall.TabIndex = 53;
            this.btnMoveFreqDecSmall.Text = "Start < 10%";
            this.btnMoveFreqDecSmall.UseVisualStyleBackColor = true;
            this.btnMoveFreqDecSmall.Click += new System.EventHandler(this.btnMoveFreqDecSmall_Click);
            // 
            // btnMoveFreqIncSmall
            // 
            this.btnMoveFreqIncSmall.Location = new System.Drawing.Point(849, 372);
            this.btnMoveFreqIncSmall.Name = "btnMoveFreqIncSmall";
            this.btnMoveFreqIncSmall.Size = new System.Drawing.Size(80, 23);
            this.btnMoveFreqIncSmall.TabIndex = 53;
            this.btnMoveFreqIncSmall.Text = "End > 10%";
            this.btnMoveFreqIncSmall.UseVisualStyleBackColor = true;
            this.btnMoveFreqIncSmall.Click += new System.EventHandler(this.btnMoveFreqIncSmall_Click);
            // 
            // btnTop5plus
            // 
            this.btnTop5plus.Location = new System.Drawing.Point(849, 120);
            this.btnTop5plus.Name = "btnTop5plus";
            this.btnTop5plus.Size = new System.Drawing.Size(80, 23);
            this.btnTop5plus.TabIndex = 53;
            this.btnTop5plus.Text = "Top +5dB";
            this.btnTop5plus.UseVisualStyleBackColor = true;
            this.btnTop5plus.Click += new System.EventHandler(this.btnTop5plus_Click);
            // 
            // btnMoveFreqDecLarge
            // 
            this.btnMoveFreqDecLarge.Location = new System.Drawing.Point(849, 176);
            this.btnMoveFreqDecLarge.Name = "btnMoveFreqDecLarge";
            this.btnMoveFreqDecLarge.Size = new System.Drawing.Size(80, 23);
            this.btnMoveFreqDecLarge.TabIndex = 53;
            this.btnMoveFreqDecLarge.Text = "Start < 50%";
            this.btnMoveFreqDecLarge.UseVisualStyleBackColor = true;
            this.btnMoveFreqDecLarge.Click += new System.EventHandler(this.btnMoveFreqDecLarge_Click);
            // 
            // btnMoveFreqIncLarge
            // 
            this.btnMoveFreqIncLarge.Location = new System.Drawing.Point(849, 400);
            this.btnMoveFreqIncLarge.Name = "btnMoveFreqIncLarge";
            this.btnMoveFreqIncLarge.Size = new System.Drawing.Size(80, 23);
            this.btnMoveFreqIncLarge.TabIndex = 53;
            this.btnMoveFreqIncLarge.Text = "End > 50%";
            this.btnMoveFreqIncLarge.UseVisualStyleBackColor = true;
            this.btnMoveFreqIncLarge.Click += new System.EventHandler(this.btnMoveFreqIncLarge_Click);
            // 
            // groupDataFeed
            // 
            this.groupDataFeed.Controls.Add(this.chkCalcMin);
            this.groupDataFeed.Controls.Add(this.chkCalcMax);
            this.groupDataFeed.Controls.Add(this.chkCalcAverage);
            this.groupDataFeed.Controls.Add(this.chkCalcRealtime);
            this.groupDataFeed.Controls.Add(this.numericIterations);
            this.groupDataFeed.Controls.Add(this.numericSampleSA);
            this.groupDataFeed.Controls.Add(this.label1);
            this.groupDataFeed.Controls.Add(this.chkRunMode);
            this.groupDataFeed.Controls.Add(this.chkHoldMode);
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
            1,
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
            // numericSampleSA
            // 
            this.numericSampleSA.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericSampleSA.Location = new System.Drawing.Point(80, 46);
            this.numericSampleSA.Name = "numericSampleSA";
            this.numericSampleSA.Size = new System.Drawing.Size(60, 23);
            this.numericSampleSA.TabIndex = 16;
            this.numericSampleSA.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericSampleSA.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
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
            this.MainStatusBar.Location = new System.Drawing.Point(3, 485);
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
            // zedSpectrumAnalyzer
            // 
            this.zedSpectrumAnalyzer.EditButtons = System.Windows.Forms.MouseButtons.Left;
            this.zedSpectrumAnalyzer.IsEnableSelection = true;
            this.zedSpectrumAnalyzer.Location = new System.Drawing.Point(6, 120);
            this.zedSpectrumAnalyzer.Name = "zedSpectrumAnalyzer";
            this.zedSpectrumAnalyzer.PanModifierKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)));
            this.zedSpectrumAnalyzer.ScrollGrace = 0D;
            this.zedSpectrumAnalyzer.ScrollMaxX = 0D;
            this.zedSpectrumAnalyzer.ScrollMaxY = 0D;
            this.zedSpectrumAnalyzer.ScrollMaxY2 = 0D;
            this.zedSpectrumAnalyzer.ScrollMinX = 0D;
            this.zedSpectrumAnalyzer.ScrollMinY = 0D;
            this.zedSpectrumAnalyzer.ScrollMinY2 = 0D;
            this.zedSpectrumAnalyzer.Size = new System.Drawing.Size(839, 362);
            this.zedSpectrumAnalyzer.TabIndex = 49;
            this.zedSpectrumAnalyzer.TabStop = false;
            this.zedSpectrumAnalyzer.ContextMenuBuilder += new ZedGraph.ZedGraphControl.ContextMenuBuilderEventHandler(this.objGraph_ContextMenuBuilder);
            // 
            // groupFreqSettings
            // 
            this.groupFreqSettings.Controls.Add(this.m_sBottomDBM);
            this.groupFreqSettings.Controls.Add(this.m_sTopDBM);
            this.groupFreqSettings.Controls.Add(this.m_sEndFreq);
            this.groupFreqSettings.Controls.Add(this.m_sFreqSpan);
            this.groupFreqSettings.Controls.Add(this.m_sStartFreq);
            this.groupFreqSettings.Controls.Add(this.m_sCenterFreq);
            this.groupFreqSettings.Controls.Add(this.label8);
            this.groupFreqSettings.Controls.Add(this.label7);
            this.groupFreqSettings.Controls.Add(this.label6);
            this.groupFreqSettings.Controls.Add(this.label5);
            this.groupFreqSettings.Controls.Add(this.label4);
            this.groupFreqSettings.Controls.Add(this.label3);
            this.groupFreqSettings.Controls.Add(this.btnReset);
            this.groupFreqSettings.Controls.Add(this.btnSend);
            this.groupFreqSettings.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupFreqSettings.Location = new System.Drawing.Point(514, 7);
            this.groupFreqSettings.Name = "groupFreqSettings";
            this.groupFreqSettings.Size = new System.Drawing.Size(410, 107);
            this.groupFreqSettings.TabIndex = 48;
            this.groupFreqSettings.TabStop = false;
            this.groupFreqSettings.Text = "Remote Frequency and Power control";
            // 
            // m_sBottomDBM
            // 
            this.m_sBottomDBM.BackColor = System.Drawing.Color.RoyalBlue;
            this.m_sBottomDBM.Font = new System.Drawing.Font("Digital-7", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_sBottomDBM.ForeColor = System.Drawing.Color.White;
            this.m_sBottomDBM.Location = new System.Drawing.Point(77, 78);
            this.m_sBottomDBM.Name = "m_sBottomDBM";
            this.m_sBottomDBM.Size = new System.Drawing.Size(98, 26);
            this.m_sBottomDBM.TabIndex = 6;
            this.m_sBottomDBM.Text = "-120";
            this.m_sBottomDBM.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_sBottomDBM.Leave += new System.EventHandler(this.m_sBottomDBM_Leave);
            // 
            // m_sTopDBM
            // 
            this.m_sTopDBM.BackColor = System.Drawing.Color.RoyalBlue;
            this.m_sTopDBM.Font = new System.Drawing.Font("Digital-7", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_sTopDBM.ForeColor = System.Drawing.Color.White;
            this.m_sTopDBM.Location = new System.Drawing.Point(242, 78);
            this.m_sTopDBM.Name = "m_sTopDBM";
            this.m_sTopDBM.Size = new System.Drawing.Size(98, 26);
            this.m_sTopDBM.TabIndex = 5;
            this.m_sTopDBM.Text = "-20";
            this.m_sTopDBM.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // m_sEndFreq
            // 
            this.m_sEndFreq.BackColor = System.Drawing.Color.RoyalBlue;
            this.m_sEndFreq.Font = new System.Drawing.Font("Digital-7", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_sEndFreq.ForeColor = System.Drawing.Color.White;
            this.m_sEndFreq.Location = new System.Drawing.Point(242, 49);
            this.m_sEndFreq.Name = "m_sEndFreq";
            this.m_sEndFreq.Size = new System.Drawing.Size(98, 26);
            this.m_sEndFreq.TabIndex = 4;
            this.m_sEndFreq.Text = "437.000";
            this.m_sEndFreq.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_sEndFreq.Leave += new System.EventHandler(this.m_sEndFreq_Leave);
            // 
            // m_sFreqSpan
            // 
            this.m_sFreqSpan.BackColor = System.Drawing.Color.RoyalBlue;
            this.m_sFreqSpan.Font = new System.Drawing.Font("Digital-7", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_sFreqSpan.ForeColor = System.Drawing.Color.White;
            this.m_sFreqSpan.Location = new System.Drawing.Point(242, 20);
            this.m_sFreqSpan.Name = "m_sFreqSpan";
            this.m_sFreqSpan.Size = new System.Drawing.Size(98, 26);
            this.m_sFreqSpan.TabIndex = 2;
            this.m_sFreqSpan.Text = "4.000";
            this.m_sFreqSpan.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_sFreqSpan.Leave += new System.EventHandler(this.m_sFreqSpan_Leave);
            // 
            // m_sStartFreq
            // 
            this.m_sStartFreq.BackColor = System.Drawing.Color.RoyalBlue;
            this.m_sStartFreq.Font = new System.Drawing.Font("Digital-7", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_sStartFreq.ForeColor = System.Drawing.Color.White;
            this.m_sStartFreq.Location = new System.Drawing.Point(77, 49);
            this.m_sStartFreq.Name = "m_sStartFreq";
            this.m_sStartFreq.Size = new System.Drawing.Size(98, 26);
            this.m_sStartFreq.TabIndex = 3;
            this.m_sStartFreq.Text = "433.000";
            this.m_sStartFreq.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_sStartFreq.Leave += new System.EventHandler(this.m_sStartFreq_Leave);
            // 
            // m_sCenterFreq
            // 
            this.m_sCenterFreq.BackColor = System.Drawing.Color.RoyalBlue;
            this.m_sCenterFreq.Font = new System.Drawing.Font("Digital-7", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_sCenterFreq.ForeColor = System.Drawing.Color.White;
            this.m_sCenterFreq.Location = new System.Drawing.Point(77, 21);
            this.m_sCenterFreq.Name = "m_sCenterFreq";
            this.m_sCenterFreq.Size = new System.Drawing.Size(98, 26);
            this.m_sCenterFreq.TabIndex = 1;
            this.m_sCenterFreq.Text = "435.000";
            this.m_sCenterFreq.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_sCenterFreq.Leave += new System.EventHandler(this.m_sCenterFreq_Leave);
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
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.label6.ForeColor = System.Drawing.Color.DarkBlue;
            this.label6.Location = new System.Drawing.Point(194, 55);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 16);
            this.label6.TabIndex = 7;
            this.label6.Text = "STOP";
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
            // tabWaterfall
            // 
            this.tabWaterfall.BackColor = System.Drawing.SystemColors.Control;
            this.tabWaterfall.Controls.Add(this.waterfallGroupBox);
            this.tabWaterfall.Controls.Add(this.panelWaterfall);
            this.tabWaterfall.Location = new System.Drawing.Point(4, 26);
            this.tabWaterfall.Name = "tabWaterfall";
            this.tabWaterfall.Padding = new System.Windows.Forms.Padding(3);
            this.tabWaterfall.Size = new System.Drawing.Size(932, 647);
            this.tabWaterfall.TabIndex = 3;
            this.tabWaterfall.Text = "Waterfall";
            this.tabWaterfall.Paint += new System.Windows.Forms.PaintEventHandler(this.tabWaterfall_Paint);
            this.tabWaterfall.Enter += new System.EventHandler(this.tabWaterfall_Enter);
            // 
            // waterfallGroupBox
            // 
            this.waterfallGroupBox.Controls.Add(this.trackBarContrast);
            this.waterfallGroupBox.Controls.Add(this.trackBarSensitivity);
            this.waterfallGroupBox.Controls.Add(this.checkBoxEnableLCD);
            this.waterfallGroupBox.Controls.Add(this.numericContrast);
            this.waterfallGroupBox.Controls.Add(this.numericSensitivity);
            this.waterfallGroupBox.Controls.Add(this.labelSensitivity);
            this.waterfallGroupBox.Controls.Add(this.labelContrast);
            this.waterfallGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.waterfallGroupBox.Location = new System.Drawing.Point(512, 6);
            this.waterfallGroupBox.Name = "waterfallGroupBox";
            this.waterfallGroupBox.Size = new System.Drawing.Size(410, 128);
            this.waterfallGroupBox.TabIndex = 56;
            this.waterfallGroupBox.TabStop = false;
            this.waterfallGroupBox.Text = "Waterfall Controls";
            // 
            // checkBoxEnableLCD
            // 
            this.checkBoxEnableLCD.Checked = true;
            this.checkBoxEnableLCD.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxEnableLCD.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBoxEnableLCD.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.checkBoxEnableLCD.Location = new System.Drawing.Point(10, 91);
            this.checkBoxEnableLCD.MinimumSize = new System.Drawing.Size(60, 0);
            this.checkBoxEnableLCD.Name = "checkBoxEnableLCD";
            this.checkBoxEnableLCD.Size = new System.Drawing.Size(138, 21);
            this.checkBoxEnableLCD.TabIndex = 55;
            this.checkBoxEnableLCD.Text = "Enable LCD";
            this.checkBoxEnableLCD.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxEnableLCD.UseVisualStyleBackColor = true;
            this.checkBoxEnableLCD.CheckedChanged += new System.EventHandler(this.checkBoxEnableLCD_CheckedChanged);
            // 
            // numericContrast
            // 
            this.numericContrast.Font = new System.Drawing.Font("Digital-7", 18F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericContrast.Location = new System.Drawing.Point(108, 54);
            this.numericContrast.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericContrast.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericContrast.Name = "numericContrast";
            this.numericContrast.Size = new System.Drawing.Size(70, 31);
            this.numericContrast.TabIndex = 54;
            this.numericContrast.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericContrast.Value = new decimal(new int[] {
            215,
            0,
            0,
            0});
            this.numericContrast.ValueChanged += new System.EventHandler(this.numericContrast_ValueChanged);
            // 
            // numericSensitivity
            // 
            this.numericSensitivity.Font = new System.Drawing.Font("Digital-7", 18F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericSensitivity.Location = new System.Drawing.Point(108, 17);
            this.numericSensitivity.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericSensitivity.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericSensitivity.Name = "numericSensitivity";
            this.numericSensitivity.Size = new System.Drawing.Size(70, 31);
            this.numericSensitivity.TabIndex = 53;
            this.numericSensitivity.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericSensitivity.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericSensitivity.ValueChanged += new System.EventHandler(this.numericSensitivity_ValueChanged);
            // 
            // labelSensitivity
            // 
            this.labelSensitivity.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSensitivity.Location = new System.Drawing.Point(6, 23);
            this.labelSensitivity.Name = "labelSensitivity";
            this.labelSensitivity.Size = new System.Drawing.Size(96, 22);
            this.labelSensitivity.TabIndex = 52;
            this.labelSensitivity.Text = "Sensitivity";
            // 
            // labelContrast
            // 
            this.labelContrast.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.labelContrast.Location = new System.Drawing.Point(6, 60);
            this.labelContrast.Name = "labelContrast";
            this.labelContrast.Size = new System.Drawing.Size(96, 21);
            this.labelContrast.TabIndex = 50;
            this.labelContrast.Text = "Contrast";
            // 
            // panelWaterfall
            // 
            this.panelWaterfall.Controls.Add(this.controlWaterfall);
            this.panelWaterfall.Location = new System.Drawing.Point(12, 140);
            this.panelWaterfall.Name = "panelWaterfall";
            this.panelWaterfall.Size = new System.Drawing.Size(910, 462);
            this.panelWaterfall.TabIndex = 55;
            // 
            // controlWaterfall
            // 
            this.controlWaterfall.Location = new System.Drawing.Point(0, 0);
            this.controlWaterfall.Name = "controlWaterfall";
            this.controlWaterfall.Size = new System.Drawing.Size(292, 174);
            this.controlWaterfall.TabIndex = 54;
            this.controlWaterfall.Load += new System.EventHandler(this.controlWaterfall_Load);
            // 
            // tabRemoteScreen
            // 
            this.tabRemoteScreen.Controls.Add(this.groupRemoteScreen);
            this.tabRemoteScreen.Controls.Add(this.panelRemoteScreen);
            this.tabRemoteScreen.Location = new System.Drawing.Point(4, 26);
            this.tabRemoteScreen.Name = "tabRemoteScreen";
            this.tabRemoteScreen.Size = new System.Drawing.Size(932, 510);
            this.tabRemoteScreen.TabIndex = 2;
            this.tabRemoteScreen.Text = "Remote Screen";
            this.tabRemoteScreen.UseVisualStyleBackColor = true;
            this.tabRemoteScreen.Paint += new System.Windows.Forms.PaintEventHandler(this.tabRemoteScreen_Paint);
            this.tabRemoteScreen.Enter += new System.EventHandler(this.tabRemoteScreen_Enter);
            // 
            // groupRemoteScreen
            // 
            this.groupRemoteScreen.Controls.Add(this.numVideoFPS);
            this.groupRemoteScreen.Controls.Add(this.label13);
            this.groupRemoteScreen.Controls.Add(this.numericZoom);
            this.groupRemoteScreen.Controls.Add(this.numScreenIndex);
            this.groupRemoteScreen.Controls.Add(this.label10);
            this.groupRemoteScreen.Controls.Add(this.label9);
            this.groupRemoteScreen.Controls.Add(this.btnSaveRemoteVideo);
            this.groupRemoteScreen.Controls.Add(this.btnSaveRemoteBitmap);
            this.groupRemoteScreen.Controls.Add(this.chkDumpScreen);
            this.groupRemoteScreen.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupRemoteScreen.Location = new System.Drawing.Point(272, 6);
            this.groupRemoteScreen.Name = "groupRemoteScreen";
            this.groupRemoteScreen.Size = new System.Drawing.Size(364, 108);
            this.groupRemoteScreen.TabIndex = 53;
            this.groupRemoteScreen.TabStop = false;
            this.groupRemoteScreen.Text = "Dump Remote Screen";
            // 
            // numVideoFPS
            // 
            this.numVideoFPS.DecimalPlaces = 1;
            this.numScreenIndex.ImeMode = System.Windows.Forms.ImeMode.Katakana;
            this.numVideoFPS.Location = new System.Drawing.Point(209, 76);
            this.numVideoFPS.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numVideoFPS.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numVideoFPS.Name = "numVideoFPS";
            this.numVideoFPS.Size = new System.Drawing.Size(49, 23);
            this.numVideoFPS.TabIndex = 54;
            this.numVideoFPS.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numVideoFPS.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label13
            // 
            this.label13.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(139, 78);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(74, 21);
            this.label13.TabIndex = 55;
            this.label13.Text = "Video FPS";
            // 
            // numericZoom
            // 
            this.numericZoom.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericZoom.Location = new System.Drawing.Point(63, 76);
            this.numericZoom.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.numericZoom.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericZoom.Name = "numericZoom";
            this.numericZoom.Size = new System.Drawing.Size(70, 23);
            this.numericZoom.TabIndex = 17;
            this.numericZoom.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericZoom.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericZoom.ValueChanged += new System.EventHandler(this.numericZoom_ValueChanged);
            // 
            // numScreenIndex
            // 
            this.numScreenIndex.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numScreenIndex.Location = new System.Drawing.Point(63, 52);
            this.numScreenIndex.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.numScreenIndex.Name = "numScreenIndex";
            this.numScreenIndex.Size = new System.Drawing.Size(70, 23);
            this.numScreenIndex.TabIndex = 51;
            this.numScreenIndex.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numScreenIndex.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numScreenIndex.ValueChanged += new System.EventHandler(this.numScreenIndex_ValueChanged);
            // 
            // label10
            // 
            this.label10.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(8, 78);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(48, 21);
            this.label10.TabIndex = 50;
            this.label10.Text = "Zoom";
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(5, 54);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(62, 22);
            this.label9.TabIndex = 52;
            this.label9.Text = "Sample";
            // 
            // btnSaveRemoteVideo
            // 
            this.btnSaveRemoteVideo.Location = new System.Drawing.Point(264, 62);
            this.btnSaveRemoteVideo.Name = "btnSaveRemoteVideo";
            this.btnSaveRemoteVideo.Size = new System.Drawing.Size(91, 37);
            this.btnSaveRemoteVideo.TabIndex = 53;
            this.btnSaveRemoteVideo.Text = "Save Video...";
            this.btnSaveRemoteVideo.UseVisualStyleBackColor = true;
            // 
            // btnSaveRemoteBitmap
            // 
            this.btnSaveRemoteBitmap.Location = new System.Drawing.Point(264, 20);
            this.btnSaveRemoteBitmap.Name = "btnSaveRemoteBitmap";
            this.btnSaveRemoteBitmap.Size = new System.Drawing.Size(91, 37);
            this.btnSaveRemoteBitmap.TabIndex = 53;
            this.btnSaveRemoteBitmap.Text = "Save Bitmap...";
            this.btnSaveRemoteBitmap.UseVisualStyleBackColor = true;
            this.btnSaveRemoteBitmap.Click += new System.EventHandler(this.SaveImagetoolStrip_Click);
            // 
            // chkDumpScreen
            // 
            this.chkDumpScreen.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkDumpScreen.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkDumpScreen.Location = new System.Drawing.Point(6, 17);
            this.chkDumpScreen.MinimumSize = new System.Drawing.Size(60, 0);
            this.chkDumpScreen.Name = "chkDumpScreen";
            this.chkDumpScreen.Size = new System.Drawing.Size(161, 29);
            this.chkDumpScreen.TabIndex = 14;
            this.chkDumpScreen.Text = "Remote Dump active";
            this.chkDumpScreen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkDumpScreen.UseVisualStyleBackColor = true;
            this.chkDumpScreen.CheckedChanged += new System.EventHandler(this.chkDumpScreen_CheckedChanged);
            // 
            // panelRemoteScreen
            // 
            this.panelRemoteScreen.Controls.Add(this.controlRemoteScreen);
            this.panelRemoteScreen.Location = new System.Drawing.Point(12, 140);
            this.panelRemoteScreen.Name = "panelRemoteScreen";
            this.panelRemoteScreen.Size = new System.Drawing.Size(912, 363);
            this.panelRemoteScreen.TabIndex = 55;
            // 
            // controlRemoteScreen
            // 
            this.controlRemoteScreen.Location = new System.Drawing.Point(0, 0);
            this.controlRemoteScreen.Name = "controlRemoteScreen";
            this.controlRemoteScreen.Size = new System.Drawing.Size(292, 174);
            this.controlRemoteScreen.TabIndex = 54;
            // 
            // tabConfiguration
            // 
            this.tabConfiguration.Controls.Add(this.panelConfiguration);
            this.tabConfiguration.Location = new System.Drawing.Point(4, 26);
            this.tabConfiguration.Name = "tabConfiguration";
            this.tabConfiguration.Padding = new System.Windows.Forms.Padding(3);
            this.tabConfiguration.Size = new System.Drawing.Size(932, 510);
            this.tabConfiguration.TabIndex = 3;
            this.tabConfiguration.Text = "Configuration";
            this.tabConfiguration.UseVisualStyleBackColor = true;
            this.tabConfiguration.Enter += new System.EventHandler(this.tabConfiguration_Enter);
            // 
            // panelConfiguration
            // 
            this.panelConfiguration.Controls.Add(this.groupCalibration);
            this.panelConfiguration.Location = new System.Drawing.Point(12, 140);
            this.panelConfiguration.Name = "panelConfiguration";
            this.panelConfiguration.Size = new System.Drawing.Size(912, 363);
            this.panelConfiguration.TabIndex = 56;
            // 
            // groupCalibration
            // 
            this.groupCalibration.Controls.Add(this.btnCalibrate);
            this.groupCalibration.Controls.Add(this.m_edCalibrationFreq);
            this.groupCalibration.Controls.Add(this.label19);
            this.groupCalibration.Location = new System.Drawing.Point(3, 3);
            this.groupCalibration.Name = "groupCalibration";
            this.groupCalibration.Size = new System.Drawing.Size(247, 100);
            this.groupCalibration.TabIndex = 4;
            this.groupCalibration.TabStop = false;
            this.groupCalibration.Text = "Calibration";
            // 
            // btnCalibrate
            // 
            this.btnCalibrate.Location = new System.Drawing.Point(131, 58);
            this.btnCalibrate.Name = "btnCalibrate";
            this.btnCalibrate.Size = new System.Drawing.Size(98, 36);
            this.btnCalibrate.TabIndex = 4;
            this.btnCalibrate.Text = "Calibrate";
            this.btnCalibrate.UseVisualStyleBackColor = true;
            this.btnCalibrate.Click += new System.EventHandler(this.btnCalibrate_Click);
            // 
            // m_edCalibrationFreq
            // 
            this.m_edCalibrationFreq.BackColor = System.Drawing.Color.RoyalBlue;
            this.m_edCalibrationFreq.Font = new System.Drawing.Font("Digital-7", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_edCalibrationFreq.ForeColor = System.Drawing.Color.White;
            this.m_edCalibrationFreq.Location = new System.Drawing.Point(131, 26);
            this.m_edCalibrationFreq.Name = "m_edCalibrationFreq";
            this.m_edCalibrationFreq.Size = new System.Drawing.Size(98, 26);
            this.m_edCalibrationFreq.TabIndex = 3;
            this.m_edCalibrationFreq.Text = "780";
            this.m_edCalibrationFreq.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_edCalibrationFreq.Leave += new System.EventHandler(this.m_edCalibrationFreq_Leave);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.ForeColor = System.Drawing.Color.DarkBlue;
            this.label19.Location = new System.Drawing.Point(6, 32);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(119, 16);
            this.label19.TabIndex = 2;
            this.label19.Text = "REFERENCE (MHz)";
            // 
            // tabReport
            // 
            this.tabReport.Controls.Add(this.groupCommands);
            this.tabReport.Controls.Add(this.textBox_message);
            this.tabReport.Location = new System.Drawing.Point(4, 26);
            this.tabReport.Name = "tabReport";
            this.tabReport.Padding = new System.Windows.Forms.Padding(3);
            this.tabReport.Size = new System.Drawing.Size(932, 510);
            this.tabReport.TabIndex = 1;
            this.tabReport.Text = "Report";
            this.tabReport.UseVisualStyleBackColor = true;
            this.tabReport.Enter += new System.EventHandler(this.tabReport_Enter);
            // 
            // groupCommands
            // 
            this.groupCommands.Controls.Add(this.btnSendCustomCmd);
            this.groupCommands.Controls.Add(this.comboStdCmd);
            this.groupCommands.Controls.Add(this.label12);
            this.groupCommands.Controls.Add(this.label11);
            this.groupCommands.Controls.Add(this.comboCustomCommand);
            this.groupCommands.Controls.Add(this.btnSendCmd);
            this.groupCommands.Location = new System.Drawing.Point(274, 6);
            this.groupCommands.Name = "groupCommands";
            this.groupCommands.Size = new System.Drawing.Size(652, 108);
            this.groupCommands.TabIndex = 50;
            this.groupCommands.TabStop = false;
            this.groupCommands.Text = "Advanced Remote Command (developer only)";
            // 
            // btnSendCustomCmd
            // 
            this.btnSendCustomCmd.Location = new System.Drawing.Point(590, 66);
            this.btnSendCustomCmd.Name = "btnSendCustomCmd";
            this.btnSendCustomCmd.Size = new System.Drawing.Size(56, 23);
            this.btnSendCustomCmd.TabIndex = 18;
            this.btnSendCustomCmd.Text = "Send";
            this.btnSendCustomCmd.UseVisualStyleBackColor = true;
            this.btnSendCustomCmd.Click += new System.EventHandler(this.btnSendCustomCmd_Click);
            // 
            // comboStdCmd
            // 
            this.comboStdCmd.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboStdCmd.FormattingEnabled = true;
            this.comboStdCmd.Items.AddRange(new object[] {
            "Baudrate 115200 : c8",
            "Baudrate 19200 : c5",
            "Baudrate 2400 : c2",
            "Baudrate 500K : c0",
            "Dump screen OFF : D0",
            "Dump screen ON : D1",
            "LCD OFF : L0",
            "LCD ON : L1",
            "Request Configuration : C0",
            "RFE on hold : CH",
            "Shutdown RFE : CS"});
            this.comboStdCmd.Location = new System.Drawing.Point(123, 38);
            this.comboStdCmd.Name = "comboStdCmd";
            this.comboStdCmd.Size = new System.Drawing.Size(461, 21);
            this.comboStdCmd.Sorted = true;
            this.comboStdCmd.TabIndex = 17;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(16, 41);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(101, 13);
            this.label12.TabIndex = 16;
            this.label12.Text = "Standard Command";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(16, 71);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(93, 13);
            this.label11.TabIndex = 15;
            this.label11.Text = "Custom Command";
            // 
            // comboCustomCommand
            // 
            this.comboCustomCommand.FormattingEnabled = true;
            this.comboCustomCommand.Location = new System.Drawing.Point(123, 68);
            this.comboCustomCommand.Name = "comboCustomCommand";
            this.comboCustomCommand.Size = new System.Drawing.Size(461, 21);
            this.comboCustomCommand.TabIndex = 14;
            // 
            // btnSendCmd
            // 
            this.btnSendCmd.Location = new System.Drawing.Point(590, 38);
            this.btnSendCmd.Name = "btnSendCmd";
            this.btnSendCmd.Size = new System.Drawing.Size(56, 22);
            this.btnSendCmd.TabIndex = 13;
            this.btnSendCmd.Text = "Send";
            this.btnSendCmd.UseVisualStyleBackColor = true;
            this.btnSendCmd.Click += new System.EventHandler(this.btnSendCmd_Click);
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
            this.textBox_message.Size = new System.Drawing.Size(920, 383);
            this.textBox_message.TabIndex = 49;
            this.textBox_message.WordWrap = false;
            // 
            // tabRAWDecoder
            // trackBarSensitivity
            // 
            this.tabRAWDecoder.Controls.Add(this.groupDemodulator);
            this.tabRAWDecoder.Controls.Add(this.groupRAWDecoder);
            this.tabRAWDecoder.Controls.Add(this.zedRAWDecoder);
            this.tabRAWDecoder.Location = new System.Drawing.Point(4, 26);
            this.tabRAWDecoder.Name = "tabRAWDecoder";
            this.tabRAWDecoder.Padding = new System.Windows.Forms.Padding(3);
            this.tabRAWDecoder.Size = new System.Drawing.Size(932, 510);
            this.tabRAWDecoder.TabIndex = 4;
            this.tabRAWDecoder.Text = "RAW Decoder";
            this.tabRAWDecoder.UseVisualStyleBackColor = true;
            this.tabRAWDecoder.Enter += new System.EventHandler(this.tabRAWDecoder_Enter);
            // 
            // groupDemodulator
            // 
            this.groupDemodulator.Controls.Add(this.chkPSK);
            this.groupDemodulator.Controls.Add(this.chkOOK);
            this.groupDemodulator.Controls.Add(this.m_sBaudRate);
            this.groupDemodulator.Controls.Add(this.m_sRefFrequency);
            this.groupDemodulator.Controls.Add(this.label18);
            this.trackBarSensitivity.Location = new System.Drawing.Point(184, 17);
            this.trackBarSensitivity.Maximum = 255;
            this.trackBarSensitivity.Minimum = 1;
            this.trackBarSensitivity.Name = "trackBarSensitivity";
            this.trackBarSensitivity.Size = new System.Drawing.Size(220, 45);
            this.trackBarSensitivity.TabIndex = 56;
            this.trackBarSensitivity.Value = 100;
            this.trackBarSensitivity.ValueChanged += new System.EventHandler(this.trackBarSensitivity_ValueChanged);
            // 
            // trackBarContrast
            // 
            this.trackBarContrast.Location = new System.Drawing.Point(184, 54);
            this.trackBarContrast.Maximum = 255;
            this.trackBarContrast.Minimum = 1;
            this.trackBarContrast.Name = "trackBarContrast";
            this.trackBarContrast.Size = new System.Drawing.Size(220, 45);
            this.trackBarContrast.TabIndex = 57;
            this.trackBarContrast.Value = 215;
            this.trackBarContrast.ValueChanged += new System.EventHandler(this.trackBarContrast_ValueChanged);
            this.groupDemodulator.Controls.Add(this.label17);
            this.groupDemodulator.Controls.Add(this.label16);
            this.groupDemodulator.Location = new System.Drawing.Point(527, 6);
            this.groupDemodulator.Name = "groupDemodulator";
            this.groupDemodulator.Size = new System.Drawing.Size(171, 108);
            this.groupDemodulator.TabIndex = 52;
            this.groupDemodulator.TabStop = false;
            this.groupDemodulator.Text = "Demodulator";
            // 
            // chkPSK
            // 
            this.chkPSK.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkPSK.AutoSize = true;
            this.chkPSK.Location = new System.Drawing.Point(126, 80);
            this.chkPSK.Name = "chkPSK";
            this.chkPSK.Size = new System.Drawing.Size(38, 23);
            this.chkPSK.TabIndex = 10;
            this.chkPSK.TabStop = true;
            this.chkPSK.Text = "PSK";
            this.chkPSK.UseVisualStyleBackColor = true;
            // 
            // chkOOK
            // 
            this.chkOOK.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkOOK.Location = new System.Drawing.Point(63, 80);
            this.chkOOK.Name = "chkOOK";
            this.chkOOK.Size = new System.Drawing.Size(60, 23);
            this.chkOOK.TabIndex = 9;
            this.chkOOK.TabStop = true;
            this.chkOOK.Text = "ASK/OOK";
            this.chkOOK.UseVisualStyleBackColor = true;
            // 
            // m_sBaudRate
            // 
            this.m_sBaudRate.BackColor = System.Drawing.Color.RoyalBlue;
            this.m_sBaudRate.Font = new System.Drawing.Font("Digital-7", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_sBaudRate.ForeColor = System.Drawing.Color.White;
            this.m_sBaudRate.Location = new System.Drawing.Point(63, 51);
            this.m_sBaudRate.Name = "m_sBaudRate";
            this.m_sBaudRate.Size = new System.Drawing.Size(98, 26);
            this.m_sBaudRate.TabIndex = 7;
            this.m_sBaudRate.Text = "5600";
            this.m_sBaudRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // m_sRefFrequency
            // 
            this.m_sRefFrequency.BackColor = System.Drawing.Color.RoyalBlue;
            this.m_sRefFrequency.Font = new System.Drawing.Font("Digital-7", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_sRefFrequency.ForeColor = System.Drawing.Color.White;
            this.m_sRefFrequency.Location = new System.Drawing.Point(63, 24);
            this.m_sRefFrequency.Name = "m_sRefFrequency";
            this.m_sRefFrequency.Size = new System.Drawing.Size(98, 26);
            this.m_sRefFrequency.TabIndex = 5;
            this.m_sRefFrequency.Text = "433.920";
            this.m_sRefFrequency.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_sRefFrequency.Leave += new System.EventHandler(this.m_sRefFrequency_Leave);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.label18.ForeColor = System.Drawing.Color.DarkBlue;
            this.label18.Location = new System.Drawing.Point(8, 83);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(37, 16);
            this.label18.TabIndex = 8;
            this.label18.Text = "MOD";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.label17.ForeColor = System.Drawing.Color.DarkBlue;
            this.label17.Location = new System.Drawing.Point(6, 56);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(53, 16);
            this.label17.TabIndex = 6;
            this.label17.Text = "B.RATE";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.label16.ForeColor = System.Drawing.Color.DarkBlue;
            this.label16.Location = new System.Drawing.Point(6, 29);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(39, 16);
            this.label16.TabIndex = 4;
            this.label16.Text = "FREQ";
            // 
            // groupRAWDecoder
            // 
            this.groupRAWDecoder.Controls.Add(this.chkRunDecoder);
            this.groupRAWDecoder.Controls.Add(this.chkHoldDecoder);
            this.groupRAWDecoder.Controls.Add(this.btnSaveRAWDecoderCSV);
            this.groupRAWDecoder.Controls.Add(this.numMultiGraph);
            this.groupRAWDecoder.Controls.Add(this.label15);
            this.groupRAWDecoder.Controls.Add(this.numSampleDecoder);
            this.groupRAWDecoder.Controls.Add(this.label14);
            this.groupRAWDecoder.Location = new System.Drawing.Point(272, 6);
            this.groupRAWDecoder.Name = "groupRAWDecoder";
            this.groupRAWDecoder.Size = new System.Drawing.Size(249, 108);
            this.groupRAWDecoder.TabIndex = 51;
            this.groupRAWDecoder.TabStop = false;
            this.groupRAWDecoder.Text = "RAW Decoder";
            // 
            // chkRunDecoder
            // 
            this.chkRunDecoder.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkRunDecoder.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkRunDecoder.Location = new System.Drawing.Point(12, 20);
            this.chkRunDecoder.MinimumSize = new System.Drawing.Size(60, 0);
            this.chkRunDecoder.Name = "chkRunDecoder";
            this.chkRunDecoder.Size = new System.Drawing.Size(60, 23);
            this.chkRunDecoder.TabIndex = 59;
            this.chkRunDecoder.Text = "RUN";
            this.chkRunDecoder.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkRunDecoder.UseVisualStyleBackColor = true;
            this.chkRunDecoder.CheckedChanged += new System.EventHandler(this.chkRunDecoder_CheckedChanged);
            // 
            // chkHoldDecoder
            // 
            this.chkHoldDecoder.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkHoldDecoder.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkHoldDecoder.Location = new System.Drawing.Point(78, 20);
            this.chkHoldDecoder.MinimumSize = new System.Drawing.Size(60, 0);
            this.chkHoldDecoder.Name = "chkHoldDecoder";
            this.chkHoldDecoder.Size = new System.Drawing.Size(60, 23);
            this.chkHoldDecoder.TabIndex = 60;
            this.chkHoldDecoder.Text = "HOLD";
            this.chkHoldDecoder.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkHoldDecoder.UseVisualStyleBackColor = true;
            this.chkHoldDecoder.CheckedChanged += new System.EventHandler(this.chkHoldDecoder_CheckedChanged);
            // 
            // btnSaveRAWDecoderCSV
            // 
            this.btnSaveRAWDecoderCSV.Location = new System.Drawing.Point(152, 13);
            this.btnSaveRAWDecoderCSV.Name = "btnSaveRAWDecoderCSV";
            this.btnSaveRAWDecoderCSV.Size = new System.Drawing.Size(91, 37);
            this.btnSaveRAWDecoderCSV.TabIndex = 58;
            this.btnSaveRAWDecoderCSV.Text = "Save CSV...";
            this.btnSaveRAWDecoderCSV.UseVisualStyleBackColor = true;
            this.btnSaveRAWDecoderCSV.Click += new System.EventHandler(this.btnSaveRAWDecoderCSV_Click);
            // 
            // numMultiGraph
            // 
            this.numMultiGraph.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numMultiGraph.Location = new System.Drawing.Point(90, 76);
            this.numMultiGraph.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numMultiGraph.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMultiGraph.Name = "numMultiGraph";
            this.numMultiGraph.Size = new System.Drawing.Size(47, 23);
            this.numMultiGraph.TabIndex = 55;
            this.numMultiGraph.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numMultiGraph.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numMultiGraph.ValueChanged += new System.EventHandler(this.numMultiGraph_ValueChanged);
            // 
            // label15
            // 
            this.label15.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(9, 78);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(75, 22);
            this.label15.TabIndex = 56;
            this.label15.Text = "Multi-graph";
            // 
            // numSampleDecoder
            // 
            this.numSampleDecoder.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numSampleDecoder.Location = new System.Drawing.Point(67, 52);
            this.numSampleDecoder.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.numSampleDecoder.Name = "numSampleDecoder";
            this.numSampleDecoder.Size = new System.Drawing.Size(70, 23);
            this.numSampleDecoder.TabIndex = 53;
            this.numSampleDecoder.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numSampleDecoder.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numSampleDecoder.ValueChanged += new System.EventHandler(this.numSampleDecoder_ValueChanged);
            // 
            // label14
            // 
            this.label14.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(9, 54);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(62, 22);
            this.label14.TabIndex = 54;
            this.label14.Text = "Sample";
            // 
            // zedRAWDecoder
            // 
            this.zedRAWDecoder.EditButtons = System.Windows.Forms.MouseButtons.Left;
            this.zedRAWDecoder.IsAntiAlias = true;
            this.zedRAWDecoder.IsShowHScrollBar = true;
            this.zedRAWDecoder.Location = new System.Drawing.Point(6, 120);
            this.zedRAWDecoder.Name = "zedRAWDecoder";
            this.zedRAWDecoder.PanModifierKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)));
            this.zedRAWDecoder.ScrollGrace = 0D;
            this.zedRAWDecoder.ScrollMaxX = 0D;
            this.zedRAWDecoder.ScrollMaxY = 0D;
            this.zedRAWDecoder.ScrollMaxY2 = 0D;
            this.zedRAWDecoder.ScrollMinX = 0D;
            this.zedRAWDecoder.ScrollMinY = 0D;
            this.zedRAWDecoder.ScrollMinY2 = 0D;
            this.zedRAWDecoder.Size = new System.Drawing.Size(920, 383);
            this.zedRAWDecoder.TabIndex = 50;
            this.zedRAWDecoder.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Info;
            this.ClientSize = new System.Drawing.Size(940, 568);
            this.Controls.Add(this.MainTab);
            this.Controls.Add(this.MainMenu);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.MainMenu;
            this.MinimumSize = new System.Drawing.Size(950, 600);
            this.Name = "MainForm";
            this.Text = "  RF Explorer Windows Client";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            this.MainTab.ResumeLayout(false);
            this.tabSpectrumAnalyzer.ResumeLayout(false);
            this.tabSpectrumAnalyzer.PerformLayout();
            this.groupDataFeed.ResumeLayout(false);
            this.groupDataFeed.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericIterations)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericSampleSA)).EndInit();
            this.MainStatusBar.ResumeLayout(false);
            this.MainStatusBar.PerformLayout();
            this.groupCOM.ResumeLayout(false);
            this.groupCOM.PerformLayout();
            this.groupFreqSettings.ResumeLayout(false);
            this.groupFreqSettings.PerformLayout();
            this.tabWaterfall.ResumeLayout(false);
            this.waterfallGroupBox.ResumeLayout(false);
            this.waterfallGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericContrast)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericSensitivity)).EndInit();
            this.panelWaterfall.ResumeLayout(false);
            this.tabRemoteScreen.ResumeLayout(false);
            this.groupRemoteScreen.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numVideoFPS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericZoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numScreenIndex)).EndInit();
            this.panelRemoteScreen.ResumeLayout(false);
            this.tabConfiguration.ResumeLayout(false);
            this.panelConfiguration.ResumeLayout(false);
            this.groupCalibration.ResumeLayout(false);
            this.groupCalibration.PerformLayout();
            this.tabReport.ResumeLayout(false);
            this.tabReport.PerformLayout();
            this.groupCommands.ResumeLayout(false);
            this.groupCommands.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSensitivity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarContrast)).EndInit();
            this.tabRAWDecoder.ResumeLayout(false);
            this.groupDemodulator.ResumeLayout(false);
            this.groupDemodulator.PerformLayout();
            this.groupRAWDecoder.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numMultiGraph)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSampleDecoder)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip MainMenu;
        private System.Windows.Forms.ToolStripMenuItem MainFileMenu;
        private System.Windows.Forms.ToolStripMenuItem menuSaveAsRFE;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Timer m_timer_receive;
        private System.IO.Ports.SerialPort m_serialPortObj;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem menuAbout;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem MainViewMenu;
        private System.Windows.Forms.ToolStripMenuItem menuRealtimeData;
        private System.Windows.Forms.ToolStripMenuItem menuAveragedData;
        private System.Windows.Forms.ToolStripMenuItem menuMaxData;
        private System.Windows.Forms.ToolStripMenuItem menuMinData;
        private System.Windows.Forms.ToolStripMenuItem menuLoadRFE;
        private System.Windows.Forms.ToolStripMenuItem menuSaveOnClose;
        private System.Windows.Forms.ToolStripMenuItem menuPortInfo;
        private System.Windows.Forms.TabControl MainTab;
        private System.Windows.Forms.TabPage tabSpectrumAnalyzer;
        private System.Windows.Forms.GroupBox groupDataFeed;
        private System.Windows.Forms.NumericUpDown numericIterations;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkRunMode;
        private System.Windows.Forms.CheckBox chkHoldMode;
        private System.Windows.Forms.NumericUpDown numericSampleSA;
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
        private ZedGraph.ZedGraphControl zedSpectrumAnalyzer;
        private System.Windows.Forms.GroupBox groupFreqSettings;
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
        private System.Windows.Forms.ToolStripMenuItem menuCleanReport;
        private System.Windows.Forms.CheckBox chkCalcMin;
        private System.Windows.Forms.CheckBox chkCalcMax;
        private System.Windows.Forms.CheckBox chkCalcAverage;
        private System.Windows.Forms.CheckBox chkCalcRealtime;
        private System.Windows.Forms.ToolStripMenuItem menuReinitializeData;
        private System.Windows.Forms.ToolStripMenuItem menuSaveCSV;
        private System.Windows.Forms.TabPage tabRemoteScreen;
        private System.Windows.Forms.GroupBox groupRemoteScreen;
        private System.Windows.Forms.NumericUpDown numericZoom;
        private System.Windows.Forms.CheckBox chkDumpScreen;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown numScreenIndex;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ToolStripMenuItem menuSaveRemoteImage;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem menuSaveRFS;
        private System.Windows.Forms.ToolStripMenuItem menuLoadRFS;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.ToolStripMenuItem menuShowPeak;
        private RFEClientControls.RemoteScreenControl controlRemoteScreen;
        private RFEClientControls.WaterfallControl controlWaterfall;
        private System.Windows.Forms.Panel panelRemoteScreen;
        private System.Windows.Forms.TabPage tabConfiguration;
        private System.Windows.Forms.ToolStripMenuItem menuShowControlArea;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.GroupBox groupCommands;
        private System.Windows.Forms.ComboBox comboStdCmd;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox comboCustomCommand;
        private System.Windows.Forms.Button btnSendCmd;
        private System.Windows.Forms.ToolStripMenuItem menuDarkMode;
        private System.Windows.Forms.Panel panelWaterfall;
        private System.Windows.Forms.TabPage tabWaterfall;
        private System.Windows.Forms.GroupBox waterfallGroupBox;
        private System.Windows.Forms.Label labelSensitivity;
        private System.Windows.Forms.Label labelContrast;
        private System.Windows.Forms.NumericUpDown numericContrast;
        private System.Windows.Forms.NumericUpDown numericSensitivity;
        private System.Windows.Forms.CheckBox checkBoxEnableLCD;
        private System.Windows.Forms.TrackBar trackBarContrast;
        private System.Windows.Forms.TrackBar trackBarSensitivity;
        private System.Windows.Forms.Button btnSaveRemoteVideo;
        private System.Windows.Forms.Button btnSaveRemoteBitmap;
        private System.Windows.Forms.ToolStripMenuItem menuAutoLCDOff;
        private System.Windows.Forms.Button btnSendCustomCmd;
        private System.Windows.Forms.TabPage tabRAWDecoder;
        private ZedGraph.ZedGraphControl zedRAWDecoder;
        private System.Windows.Forms.NumericUpDown numVideoFPS;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.GroupBox groupRAWDecoder;
        private System.Windows.Forms.NumericUpDown numMultiGraph;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.NumericUpDown numSampleDecoder;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox m_sBaudRate;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox m_sRefFrequency;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Button btnSaveRAWDecoderCSV;
        private System.Windows.Forms.RadioButton chkPSK;
        private System.Windows.Forms.RadioButton chkOOK;
        private System.Windows.Forms.GroupBox groupDemodulator;
        private System.Windows.Forms.CheckBox chkRunDecoder;
        private System.Windows.Forms.CheckBox chkHoldDecoder;
        private System.Windows.Forms.Button btnSpanDec;
        private System.Windows.Forms.Button btnSpanInc;
        private System.Windows.Forms.Button btnMoveFreqDecSmall;
        private System.Windows.Forms.Button btnMoveFreqIncSmall;
        private System.Windows.Forms.Button btnMoveFreqDecLarge;
        private System.Windows.Forms.Button btnMoveFreqIncLarge;
        private System.Windows.Forms.Button btnTop5plus;
        private System.Windows.Forms.Button btnTop5minus;
        private System.Windows.Forms.Button btnBottom5minus;
        private System.Windows.Forms.Button btnBottom5plus;
        private System.Windows.Forms.Button btnSpanMin;
        private System.Windows.Forms.Button btnSpanDefault;
        private System.Windows.Forms.Button btnSpanMax;
        private System.Windows.Forms.Button btnCenterMark;
        private System.Windows.Forms.Panel panelConfiguration;
        private System.Windows.Forms.GroupBox groupCalibration;
        private System.Windows.Forms.TextBox m_edCalibrationFreq;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Button btnCalibrate;
    }
}

