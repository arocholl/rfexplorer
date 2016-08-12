//============================================================================
//RF Explorer for Windows - A Handheld Spectrum Analyzer for everyone!
//Copyright © 2010-16 Ariel Rocholl, www.rf-explorer.com
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
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                    components = null;
                }
                if (m_BrushDarkBlue != null)
                {
                    m_BrushDarkBlue.Dispose();
                    m_BrushDarkBlue = null;
                }
                if (m_PenDarkBlue != null)
                {
                    m_PenDarkBlue.Dispose();
                    m_PenDarkBlue = null;
                }
                if (m_PenRed != null)
                {
                    m_PenRed.Dispose();
                    m_PenRed = null;
                }
                if (m_ClipboardBitmap != null)
                {
                    m_ClipboardBitmap.Dispose();
                    m_ClipboardBitmap = null;
                }
                if (m_StatusGraphText_Analyzer != null)
                {
                    m_StatusGraphText_Analyzer.Dispose();
                    m_StatusGraphText_Analyzer = null;
                }
                if (m_OverloadText != null)
                {
                    m_OverloadText.Dispose();
                    m_OverloadText = null;
                }
                if (m_objRFEAnalyzer != null)
                {
                    m_objRFEAnalyzer.Dispose();
                    m_objRFEAnalyzer = null;
                }
                if (m_PowerChannelRegion_High != null)
                {
                    m_PowerChannelRegion_High.Dispose();
                    m_PowerChannelRegion_High = null;
                }
                if (m_PowerChannelRegion_Low != null)
                {
                    m_PowerChannelRegion_Low.Dispose();
                    m_PowerChannelRegion_Low = null;
                }
                if (m_PowerChannelRegion_Medium!=null)
                {
                    m_PowerChannelRegion_Medium.Dispose();
                    m_PowerChannelRegion_Medium = null;
                }
                if (m_PowerChannelText!=null)
                {
                    m_PowerChannelText.Dispose();
                    m_PowerChannelText = null;
                }
                if (m_PowerChannelNeedle!=null)
                {
                    m_PowerChannelNeedle.Dispose();
                    m_PowerChannelNeedle = null;
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Mobility", "CA1601:DoNotUseTimersThatPreventPowerStateChanges")]
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.MainMenu = new System.Windows.Forms.MenuStrip();
            this.MainFileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLoadRFE = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMRU = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSaveAsRFE = new System.Windows.Forms.ToolStripMenuItem();
            this.menuContinuousLog = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSaveOnClose = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.menusSaveSimpleCSV = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSaveCSV = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.menuSaveRFS = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLoadRFS = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSaveRemoteImage = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator22 = new System.Windows.Forms.ToolStripSeparator();
            this.menuSaveSNANormalization = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLoadSNANormalization = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSaveS1P = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSaveSNACSV = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator20 = new System.Windows.Forms.ToolStripSeparator();
            this.menuAutoLoadAmplitudeData = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLoadAmplitudeFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuUseAmplitudeCorrection = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.menuSaveBitmap = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.menuPageSetup = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPrintPreview = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPrint = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.menuReinitializeData = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuExitApp = new System.Windows.Forms.ToolStripMenuItem();
            this.MainViewMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDarkMode = new System.Windows.Forms.ToolStripMenuItem();
            this.menuShowControlArea = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAmplitudeUnits = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemDBM = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemDBUV = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemWatt = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.menuLimitLines = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLimitLineBuildFromSignal = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLimitLineMaxBuildFromSignal = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLimitLineMinBuildFromSignal = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLimitLineSaveToFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLimitLineMaxSaveToFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLimitLineMinSaveToFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLimitLineReadFromFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLimitLineMaxReadFromFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLimitLineMinReadFromFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLimitLineRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRemoveMaxLimitLine = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRemoveMinLimitLine = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSoundAlarmLimitLine = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMarkers = new System.Windows.Forms.ToolStripMenuItem();
            this.menuShowPeak = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.menuCalculatorSignalModes = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRealtimeTrace = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAveragedTrace = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMaxTrace = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMinTrace = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMaxHoldTrace = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSignalFill = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSmoothSignals = new System.Windows.Forms.ToolStripMenuItem();
            this.menuThickTrace = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator19 = new System.Windows.Forms.ToolStripSeparator();
            this.menuShowGrid = new System.Windows.Forms.ToolStripMenuItem();
            this.menuShowAxisLabels = new System.Windows.Forms.ToolStripMenuItem();
            this.menuUserDefinedText = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.menuWaterfallInSA = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPlaceWaterfallNone = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPlaceWaterfallAtBottom = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPlaceWaterfallOnTheRight = new System.Windows.Forms.ToolStripMenuItem();
            this.menuWaterfallPerspective = new System.Windows.Forms.ToolStripMenuItem();
            this.menuWaterfallPerspective1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuWaterfallPerspective2 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuWaterfallPerspectiveIso = new System.Windows.Forms.ToolStripMenuItem();
            this.menuWaterfallPerspective2D = new System.Windows.Forms.ToolStripMenuItem();
            this.menuTransparentWaterfall = new System.Windows.Forms.ToolStripMenuItem();
            this.menuWaterfallFloor = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.menuCleanReport = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDevice = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRFConnections = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.menuEnableMainboard = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEnableExpansionBoard = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.menuRemoteMaxHold = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRefreshRemoteMaxHold = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator21 = new System.Windows.Forms.ToolStripSeparator();
            this.menuAutoLCDOff = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRemoteAmplitudeUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.menuComboSavedOptions = new System.Windows.Forms.ToolStripComboBox();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOnlineHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDeviceManual = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.menuReleaseNotes = new System.Windows.Forms.ToolStripMenuItem();
            this.menuWindowsReleaseNotes = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.menuPortInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDebug = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDebug_datatest_1 = new System.Windows.Forms.ToolStripMenuItem();
            this.m_timer_receive = new System.Windows.Forms.Timer(this.components);
            this.m_MainTab = new System.Windows.Forms.TabControl();
            this.m_tabSpectrumAnalyzer = new System.Windows.Forms.TabPage();
            this.m_group_CalibrateAmplitudeAnalyzer = new System.Windows.Forms.GroupBox();
            this.btnCalibratePurge = new System.Windows.Forms.Button();
            this.chkRFE6GEN_CAL = new System.Windows.Forms.CheckBox();
            this.m_btnCalibrate1G = new System.Windows.Forms.Button();
            this.m_btnCalibrate3G = new System.Windows.Forms.Button();
            this.m_btnCalibrate6G = new System.Windows.Forms.Button();
            this.m_tableLayoutControlArea = new System.Windows.Forms.TableLayoutPanel();
            this.btnCenterMark = new System.Windows.Forms.Button();
            this.btnSpanMin = new System.Windows.Forms.Button();
            this.btnSpanDefault = new System.Windows.Forms.Button();
            this.btnSpanMax = new System.Windows.Forms.Button();
            this.btnBottom5minus = new System.Windows.Forms.Button();
            this.btnBottom5plus = new System.Windows.Forms.Button();
            this.btnTop5minus = new System.Windows.Forms.Button();
            this.btnAutoscale = new System.Windows.Forms.Button();
            this.btnSpanDec = new System.Windows.Forms.Button();
            this.btnSpanInc = new System.Windows.Forms.Button();
            this.btnMoveFreqDecSmall = new System.Windows.Forms.Button();
            this.btnMoveFreqIncSmall = new System.Windows.Forms.Button();
            this.btnTop5plus = new System.Windows.Forms.Button();
            this.btnMoveFreqDecLarge = new System.Windows.Forms.Button();
            this.btnMoveFreqIncLarge = new System.Windows.Forms.Button();
            this.m_tabWaterfall = new System.Windows.Forms.TabPage();
            this.menuContextWaterfall = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuWaterfallContextMaxHold = new System.Windows.Forms.ToolStripMenuItem();
            this.menuWaterfallContextRealtime = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
            this.menuWaterfallContextPerspective1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuWaterfallContextPerspective2 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuWaterfallContextISO = new System.Windows.Forms.ToolStripMenuItem();
            this.menuWaterfallContext2D = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.menuWaterfallContextTransparent = new System.Windows.Forms.ToolStripMenuItem();
            this.menuWaterfallContextFloor = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
            this.menuWaterfallContext_saveBitmap = new System.Windows.Forms.ToolStripMenuItem();
            this.m_tabPowerChannel = new System.Windows.Forms.TabPage();
            this.m_panelPowerChannel = new System.Windows.Forms.Panel();
            this.m_tabRemoteScreen = new System.Windows.Forms.TabPage();
            this.m_panelRemoteScreen = new System.Windows.Forms.Panel();
            this.controlRemoteScreen = new RFEClientControls.RemoteScreenControl();
            this.m_tabConfiguration = new System.Windows.Forms.TabPage();
            this.m_panelGeneralConfigTab = new System.Windows.Forms.Panel();
            this.m_tableConfiguration = new System.Windows.Forms.TableLayoutPanel();
            this.groupCalibration = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel10 = new System.Windows.Forms.TableLayoutPanel();
            this.btnCalibrate = new System.Windows.Forms.Button();
            this.m_edCalibrationFreq = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.groupBoxFiles = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel9 = new System.Windows.Forms.TableLayoutPanel();
            this.label20 = new System.Windows.Forms.Label();
            this.comboCSVFieldSeparator = new System.Windows.Forms.ComboBox();
            this.edDefaultFilePath = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.labelReportFile = new System.Windows.Forms.Label();
            this.btnOpenLog = new System.Windows.Forms.Button();
            this.m_panelRFConnections = new System.Windows.Forms.Panel();
            this.m_controlRFModuleSelectorConfig = new RFEClientControls.RFModuleSelector();
            this.m_groupSignalTypeConfiguration = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.label23 = new System.Windows.Forms.Label();
            this.m_chkPeakValue = new System.Windows.Forms.CheckBox();
            this.m_chkFilledGraph = new System.Windows.Forms.CheckBox();
            this.label22 = new System.Windows.Forms.Label();
            this.m_SignalTypeList = new System.Windows.Forms.ComboBox();
            this.m_chkVisibleCurve = new System.Windows.Forms.CheckBox();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.m_bLineColor = new System.Windows.Forms.Button();
            this.m_bFillColor = new System.Windows.Forms.Button();
            this.m_tabReport = new System.Windows.Forms.TabPage();
            this.m_ReportTextBox = new System.Windows.Forms.TextBox();
            this.m_MainStatusBar = new System.Windows.Forms.StatusStrip();
            this.toolCOMStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripMemory = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripSamples = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolFile = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabRAWDecoder = new System.Windows.Forms.TabPage();
            this.groupDemodulator = new System.Windows.Forms.GroupBox();
            this.chkPSK = new System.Windows.Forms.RadioButton();
            this.chkOOK = new System.Windows.Forms.RadioButton();
            this.m_sBaudRate = new System.Windows.Forms.TextBox();
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
            this.m_MainFormTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.btnSaveSettings = new System.Windows.Forms.Button();
            this.btnDelSettings = new System.Windows.Forms.Button();
            this.btnLoadSettings = new System.Windows.Forms.Button();
            this.printMainDocument = new System.Drawing.Printing.PrintDocument();
            this.printPreviewDialog = new System.Windows.Forms.PrintPreviewDialog();
            this.printDialog = new System.Windows.Forms.PrintDialog();
            this.zedRAWDecoder = new ZedGraph.ZedGraphControl();
            this.MainMenu.SuspendLayout();
            this.m_MainTab.SuspendLayout();
            this.m_tabSpectrumAnalyzer.SuspendLayout();
            this.m_group_CalibrateAmplitudeAnalyzer.SuspendLayout();
            this.m_tabWaterfall.SuspendLayout();
            this.menuContextWaterfall.SuspendLayout();
            this.m_tabPowerChannel.SuspendLayout();
            this.m_tabRemoteScreen.SuspendLayout();
            this.m_panelRemoteScreen.SuspendLayout();
            this.m_tabConfiguration.SuspendLayout();
            this.m_panelGeneralConfigTab.SuspendLayout();
            this.m_tableConfiguration.SuspendLayout();
            this.groupCalibration.SuspendLayout();
            this.tableLayoutPanel10.SuspendLayout();
            this.groupBoxFiles.SuspendLayout();
            this.tableLayoutPanel9.SuspendLayout();
            this.m_panelRFConnections.SuspendLayout();
            this.m_groupSignalTypeConfiguration.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.m_tabReport.SuspendLayout();
            this.m_MainStatusBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMultiGraph)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSampleDecoder)).BeginInit();
            this.SuspendLayout();
            // 
            // MainMenu
            // 
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MainFileMenu,
            this.MainViewMenu,
            this.menuDevice,
            this.menuComboSavedOptions,
            this.helpToolStripMenuItem,
            this.menuDebug});
            this.MainMenu.Location = new System.Drawing.Point(0, 0);
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.MainMenu.Size = new System.Drawing.Size(1092, 27);
            this.MainMenu.TabIndex = 46;
            this.MainMenu.Text = "menu";
            // 
            // MainFileMenu
            // 
            this.MainFileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuLoadRFE,
            this.menuMRU,
            this.menuSaveAsRFE,
            this.menuContinuousLog,
            this.menuSaveOnClose,
            this.toolStripSeparator4,
            this.menusSaveSimpleCSV,
            this.menuSaveCSV,
            this.toolStripSeparator5,
            this.menuSaveRFS,
            this.menuLoadRFS,
            this.menuSaveRemoteImage,
            this.toolStripSeparator22,
            this.menuSaveSNANormalization,
            this.menuLoadSNANormalization,
            this.menuSaveS1P,
            this.menuSaveSNACSV,
            this.toolStripSeparator20,
            this.menuAutoLoadAmplitudeData,
            this.menuLoadAmplitudeFile,
            this.menuUseAmplitudeCorrection,
            this.toolStripSeparator6,
            this.menuSaveBitmap,
            this.toolStripSeparator13,
            this.menuPageSetup,
            this.menuPrintPreview,
            this.menuPrint,
            this.toolStripSeparator12,
            this.menuReinitializeData,
            this.toolStripSeparator1,
            this.menuExitApp});
            this.MainFileMenu.Name = "MainFileMenu";
            this.MainFileMenu.Size = new System.Drawing.Size(37, 23);
            this.MainFileMenu.Text = "&File";
            this.MainFileMenu.DropDownOpening += new System.EventHandler(this.fileToolStripMenuItem_DropDownOpening);
            // 
            // menuLoadRFE
            // 
            this.menuLoadRFE.Name = "menuLoadRFE";
            this.menuLoadRFE.Size = new System.Drawing.Size(300, 22);
            this.menuLoadRFE.Text = "&Load RFE data file...";
            this.menuLoadRFE.Click += new System.EventHandler(this.OnLoadFileRFE_Click);
            // 
            // menuMRU
            // 
            this.menuMRU.Name = "menuMRU";
            this.menuMRU.Size = new System.Drawing.Size(300, 22);
            this.menuMRU.Text = "Open recent data files";
            this.menuMRU.DropDownOpening += new System.EventHandler(this.menuMRU_DropDownOpening);
            // 
            // menuSaveAsRFE
            // 
            this.menuSaveAsRFE.Name = "menuSaveAsRFE";
            this.menuSaveAsRFE.Size = new System.Drawing.Size(300, 22);
            this.menuSaveAsRFE.Text = "Sa&ve RFE data As...";
            this.menuSaveAsRFE.Click += new System.EventHandler(this.OnSaveAsRFE_Click);
            // 
            // menuContinuousLog
            // 
            this.menuContinuousLog.CheckOnClick = true;
            this.menuContinuousLog.Name = "menuContinuousLog";
            this.menuContinuousLog.Size = new System.Drawing.Size(300, 22);
            this.menuContinuousLog.Text = "&Continuous log to RFE data file";
            // 
            // menuSaveOnClose
            // 
            this.menuSaveOnClose.CheckOnClick = true;
            this.menuSaveOnClose.Name = "menuSaveOnClose";
            this.menuSaveOnClose.Size = new System.Drawing.Size(300, 22);
            this.menuSaveOnClose.Text = "&Save RFE data on Close";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(297, 6);
            // 
            // menusSaveSimpleCSV
            // 
            this.menusSaveSimpleCSV.Name = "menusSaveSimpleCSV";
            this.menusSaveSimpleCSV.Size = new System.Drawing.Size(300, 22);
            this.menusSaveSimpleCSV.Text = "Export Single Signal CSV &As...";
            this.menusSaveSimpleCSV.Click += new System.EventHandler(this.OnSaveSimpleCSV_Click);
            // 
            // menuSaveCSV
            // 
            this.menuSaveCSV.Name = "menuSaveCSV";
            this.menuSaveCSV.Size = new System.Drawing.Size(300, 22);
            this.menuSaveCSV.Text = "Export Cumulative CS&V As...";
            this.menuSaveCSV.Click += new System.EventHandler(this.OnSaveCSV_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(297, 6);
            // 
            // menuSaveRFS
            // 
            this.menuSaveRFS.Name = "menuSaveRFS";
            this.menuSaveRFS.Size = new System.Drawing.Size(300, 22);
            this.menuSaveRFS.Text = "Sav&e RFS Screen file As... ";
            this.menuSaveRFS.Click += new System.EventHandler(this.OnSaveRFS_Click);
            // 
            // menuLoadRFS
            // 
            this.menuLoadRFS.Name = "menuLoadRFS";
            this.menuLoadRFS.Size = new System.Drawing.Size(300, 22);
            this.menuLoadRFS.Text = "Load &RFS Screen file As...";
            this.menuLoadRFS.Click += new System.EventHandler(this.OnLoadRFS_Click);
            // 
            // menuSaveRemoteImage
            // 
            this.menuSaveRemoteImage.Name = "menuSaveRemoteImage";
            this.menuSaveRemoteImage.Size = new System.Drawing.Size(300, 22);
            this.menuSaveRemoteImage.Text = "Save Remote Ima&ge As...";
            this.menuSaveRemoteImage.Click += new System.EventHandler(this.OnSaveImage_Click);
            // 
            // toolStripSeparator22
            // 
            this.toolStripSeparator22.Name = "toolStripSeparator22";
            this.toolStripSeparator22.Size = new System.Drawing.Size(297, 6);
            // 
            // menuSaveSNANormalization
            // 
            this.menuSaveSNANormalization.Name = "menuSaveSNANormalization";
            this.menuSaveSNANormalization.Size = new System.Drawing.Size(300, 22);
            this.menuSaveSNANormalization.Text = "Save SNA Normalization As...";
            this.menuSaveSNANormalization.Click += new System.EventHandler(this.menuSaveSNANormalization_Click);
            // 
            // menuLoadSNANormalization
            // 
            this.menuLoadSNANormalization.Name = "menuLoadSNANormalization";
            this.menuLoadSNANormalization.Size = new System.Drawing.Size(300, 22);
            this.menuLoadSNANormalization.Text = "Load SNA Normalization...";
            this.menuLoadSNANormalization.Click += new System.EventHandler(this.menuLoadSNANormalization_Click);
            // 
            // menuSaveS1P
            // 
            this.menuSaveS1P.Name = "menuSaveS1P";
            this.menuSaveS1P.Size = new System.Drawing.Size(300, 22);
            this.menuSaveS1P.Text = "Save SNA S1P data As...";
            this.menuSaveS1P.Click += new System.EventHandler(this.menuSaveS1P_Click);
            // 
            // menuSaveSNACSV
            // 
            this.menuSaveSNACSV.Name = "menuSaveSNACSV";
            this.menuSaveSNACSV.Size = new System.Drawing.Size(300, 22);
            this.menuSaveSNACSV.Text = "Save SNA CSV data As...";
            this.menuSaveSNACSV.Click += new System.EventHandler(this.menuSaveSNACSV_Click);
            // 
            // toolStripSeparator20
            // 
            this.toolStripSeparator20.Name = "toolStripSeparator20";
            this.toolStripSeparator20.Size = new System.Drawing.Size(297, 6);
            // 
            // menuAutoLoadAmplitudeData
            // 
            this.menuAutoLoadAmplitudeData.CheckOnClick = true;
            this.menuAutoLoadAmplitudeData.Name = "menuAutoLoadAmplitudeData";
            this.menuAutoLoadAmplitudeData.Size = new System.Drawing.Size(300, 22);
            this.menuAutoLoadAmplitudeData.Text = "Autoload model amplitude correction data";
            this.menuAutoLoadAmplitudeData.Click += new System.EventHandler(this.menuAutoLoadAmplitudeData_Click);
            // 
            // menuLoadAmplitudeFile
            // 
            this.menuLoadAmplitudeFile.Name = "menuLoadAmplitudeFile";
            this.menuLoadAmplitudeFile.Size = new System.Drawing.Size(300, 22);
            this.menuLoadAmplitudeFile.Text = "Load amplitude correction file...";
            this.menuLoadAmplitudeFile.Click += new System.EventHandler(this.OnLoadAmplitudeFile_Click);
            // 
            // menuUseAmplitudeCorrection
            // 
            this.menuUseAmplitudeCorrection.Name = "menuUseAmplitudeCorrection";
            this.menuUseAmplitudeCorrection.Size = new System.Drawing.Size(300, 22);
            this.menuUseAmplitudeCorrection.Text = "Use amplitude correction data";
            this.menuUseAmplitudeCorrection.Click += new System.EventHandler(this.OnUseAmplitudeCorrection_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(297, 6);
            // 
            // menuSaveBitmap
            // 
            this.menuSaveBitmap.Name = "menuSaveBitmap";
            this.menuSaveBitmap.Size = new System.Drawing.Size(300, 22);
            this.menuSaveBitmap.Text = "Save &Bitmap screenshot image...";
            this.menuSaveBitmap.Click += new System.EventHandler(this.OnSaveImage_Click);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new System.Drawing.Size(297, 6);
            // 
            // menuPageSetup
            // 
            this.menuPageSetup.Name = "menuPageSetup";
            this.menuPageSetup.Size = new System.Drawing.Size(300, 22);
            this.menuPageSetup.Text = "Pa&ge Setup...";
            this.menuPageSetup.Click += new System.EventHandler(this.OnPageSetup_Click);
            // 
            // menuPrintPreview
            // 
            this.menuPrintPreview.Name = "menuPrintPreview";
            this.menuPrintPreview.Size = new System.Drawing.Size(300, 22);
            this.menuPrintPreview.Text = "Print Previe&w...";
            this.menuPrintPreview.Click += new System.EventHandler(this.OnPrintPreview_Click);
            // 
            // menuPrint
            // 
            this.menuPrint.Name = "menuPrint";
            this.menuPrint.Size = new System.Drawing.Size(300, 22);
            this.menuPrint.Text = "&Print...";
            this.menuPrint.Click += new System.EventHandler(this.OnPrint_Click);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(297, 6);
            // 
            // menuReinitializeData
            // 
            this.menuReinitializeData.Name = "menuReinitializeData";
            this.menuReinitializeData.Size = new System.Drawing.Size(300, 22);
            this.menuReinitializeData.Text = "Reinitialize &Data Buffer...";
            this.menuReinitializeData.Click += new System.EventHandler(this.OnReinitializeData_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(297, 6);
            // 
            // menuExitApp
            // 
            this.menuExitApp.Name = "menuExitApp";
            this.menuExitApp.Size = new System.Drawing.Size(300, 22);
            this.menuExitApp.Text = "E&xit";
            this.menuExitApp.Click += new System.EventHandler(this.OnExit_Click);
            // 
            // MainViewMenu
            // 
            this.MainViewMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuDarkMode,
            this.menuShowControlArea,
            this.menuAmplitudeUnits,
            this.toolStripSeparator15,
            this.menuLimitLines,
            this.menuMarkers,
            this.menuShowPeak,
            this.toolStripSeparator7,
            this.menuCalculatorSignalModes,
            this.menuSignalFill,
            this.menuSmoothSignals,
            this.menuThickTrace,
            this.toolStripSeparator19,
            this.menuShowGrid,
            this.menuShowAxisLabels,
            this.menuUserDefinedText,
            this.toolStripSeparator3,
            this.menuWaterfallInSA,
            this.menuWaterfallPerspective,
            this.menuTransparentWaterfall,
            this.menuWaterfallFloor,
            this.toolStripSeparator14,
            this.menuCleanReport});
            this.MainViewMenu.Name = "MainViewMenu";
            this.MainViewMenu.Size = new System.Drawing.Size(44, 23);
            this.MainViewMenu.Text = "&View";
            this.MainViewMenu.DropDownOpening += new System.EventHandler(this.MainMenuView_DropDownOpening);
            // 
            // menuDarkMode
            // 
            this.menuDarkMode.CheckOnClick = true;
            this.menuDarkMode.Name = "menuDarkMode";
            this.menuDarkMode.Size = new System.Drawing.Size(243, 22);
            this.menuDarkMode.Text = "Dar&k Color mode";
            this.menuDarkMode.Click += new System.EventHandler(this.OnDarkMode_Click);
            // 
            // menuShowControlArea
            // 
            this.menuShowControlArea.CheckOnClick = true;
            this.menuShowControlArea.Name = "menuShowControlArea";
            this.menuShowControlArea.Size = new System.Drawing.Size(243, 22);
            this.menuShowControlArea.Text = "&Display Control Area";
            this.menuShowControlArea.Click += new System.EventHandler(this.OnShowControlArea_Click);
            // 
            // menuAmplitudeUnits
            // 
            this.menuAmplitudeUnits.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemDBM,
            this.menuItemDBUV,
            this.menuItemWatt});
            this.menuAmplitudeUnits.Name = "menuAmplitudeUnits";
            this.menuAmplitudeUnits.Size = new System.Drawing.Size(243, 22);
            this.menuAmplitudeUnits.Text = "Amplitude &units";
            // 
            // menuItemDBM
            // 
            this.menuItemDBM.AutoToolTip = true;
            this.menuItemDBM.Name = "menuItemDBM";
            this.menuItemDBM.Size = new System.Drawing.Size(102, 22);
            this.menuItemDBM.Text = "d&Bm";
            this.menuItemDBM.ToolTipText = "dB over milliWatt at 50 ohm";
            this.menuItemDBM.Click += new System.EventHandler(this.OnItemAmplitudeUnit_Click);
            // 
            // menuItemDBUV
            // 
            this.menuItemDBUV.AutoToolTip = true;
            this.menuItemDBUV.Name = "menuItemDBUV";
            this.menuItemDBUV.Size = new System.Drawing.Size(102, 22);
            this.menuItemDBUV.Text = "dB&uV";
            this.menuItemDBUV.ToolTipText = "dB over microVolt at 50 ohm";
            this.menuItemDBUV.Click += new System.EventHandler(this.OnItemAmplitudeUnit_Click);
            // 
            // menuItemWatt
            // 
            this.menuItemWatt.Name = "menuItemWatt";
            this.menuItemWatt.Size = new System.Drawing.Size(102, 22);
            this.menuItemWatt.Text = "&Watt";
            this.menuItemWatt.Click += new System.EventHandler(this.OnItemAmplitudeUnit_Click);
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            this.toolStripSeparator15.Size = new System.Drawing.Size(240, 6);
            // 
            // menuLimitLines
            // 
            this.menuLimitLines.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuLimitLineBuildFromSignal,
            this.menuLimitLineSaveToFile,
            this.menuLimitLineReadFromFile,
            this.menuLimitLineRemove,
            this.menuItemSoundAlarmLimitLine});
            this.menuLimitLines.Name = "menuLimitLines";
            this.menuLimitLines.Size = new System.Drawing.Size(243, 22);
            this.menuLimitLines.Text = "&Limit Lines";
            // 
            // menuLimitLineBuildFromSignal
            // 
            this.menuLimitLineBuildFromSignal.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuLimitLineMaxBuildFromSignal,
            this.menuLimitLineMinBuildFromSignal});
            this.menuLimitLineBuildFromSignal.Name = "menuLimitLineBuildFromSignal";
            this.menuLimitLineBuildFromSignal.Size = new System.Drawing.Size(165, 22);
            this.menuLimitLineBuildFromSignal.Text = "&Build from Signal";
            // 
            // menuLimitLineMaxBuildFromSignal
            // 
            this.menuLimitLineMaxBuildFromSignal.Name = "menuLimitLineMaxBuildFromSignal";
            this.menuLimitLineMaxBuildFromSignal.Size = new System.Drawing.Size(105, 22);
            this.menuLimitLineMaxBuildFromSignal.Text = "&Max...";
            this.menuLimitLineMaxBuildFromSignal.Click += new System.EventHandler(this.OnLimitLineBuildFromSignal_Click);
            // 
            // menuLimitLineMinBuildFromSignal
            // 
            this.menuLimitLineMinBuildFromSignal.Name = "menuLimitLineMinBuildFromSignal";
            this.menuLimitLineMinBuildFromSignal.Size = new System.Drawing.Size(105, 22);
            this.menuLimitLineMinBuildFromSignal.Text = "M&in...";
            this.menuLimitLineMinBuildFromSignal.Click += new System.EventHandler(this.OnLimitLineBuildFromSignal_Click);
            // 
            // menuLimitLineSaveToFile
            // 
            this.menuLimitLineSaveToFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuLimitLineMaxSaveToFile,
            this.menuLimitLineMinSaveToFile});
            this.menuLimitLineSaveToFile.Name = "menuLimitLineSaveToFile";
            this.menuLimitLineSaveToFile.Size = new System.Drawing.Size(165, 22);
            this.menuLimitLineSaveToFile.Text = "&Save to File";
            // 
            // menuLimitLineMaxSaveToFile
            // 
            this.menuLimitLineMaxSaveToFile.Name = "menuLimitLineMaxSaveToFile";
            this.menuLimitLineMaxSaveToFile.Size = new System.Drawing.Size(105, 22);
            this.menuLimitLineMaxSaveToFile.Text = "&Max...";
            this.menuLimitLineMaxSaveToFile.Click += new System.EventHandler(this.OnLimitLineSaveToFile_Click);
            // 
            // menuLimitLineMinSaveToFile
            // 
            this.menuLimitLineMinSaveToFile.Name = "menuLimitLineMinSaveToFile";
            this.menuLimitLineMinSaveToFile.Size = new System.Drawing.Size(105, 22);
            this.menuLimitLineMinSaveToFile.Text = "M&in...";
            this.menuLimitLineMinSaveToFile.Click += new System.EventHandler(this.OnLimitLineSaveToFile_Click);
            // 
            // menuLimitLineReadFromFile
            // 
            this.menuLimitLineReadFromFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuLimitLineMaxReadFromFile,
            this.menuLimitLineMinReadFromFile});
            this.menuLimitLineReadFromFile.Name = "menuLimitLineReadFromFile";
            this.menuLimitLineReadFromFile.Size = new System.Drawing.Size(165, 22);
            this.menuLimitLineReadFromFile.Text = "R&ead from File";
            // 
            // menuLimitLineMaxReadFromFile
            // 
            this.menuLimitLineMaxReadFromFile.Name = "menuLimitLineMaxReadFromFile";
            this.menuLimitLineMaxReadFromFile.Size = new System.Drawing.Size(105, 22);
            this.menuLimitLineMaxReadFromFile.Text = "&Max...";
            this.menuLimitLineMaxReadFromFile.Click += new System.EventHandler(this.OnLimitLineReadFromFile_Click);
            // 
            // menuLimitLineMinReadFromFile
            // 
            this.menuLimitLineMinReadFromFile.Name = "menuLimitLineMinReadFromFile";
            this.menuLimitLineMinReadFromFile.Size = new System.Drawing.Size(105, 22);
            this.menuLimitLineMinReadFromFile.Text = "M&in...";
            this.menuLimitLineMinReadFromFile.Click += new System.EventHandler(this.OnLimitLineReadFromFile_Click);
            // 
            // menuLimitLineRemove
            // 
            this.menuLimitLineRemove.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuRemoveMaxLimitLine,
            this.menuRemoveMinLimitLine});
            this.menuLimitLineRemove.Name = "menuLimitLineRemove";
            this.menuLimitLineRemove.Size = new System.Drawing.Size(165, 22);
            this.menuLimitLineRemove.Text = "&Remove";
            // 
            // menuRemoveMaxLimitLine
            // 
            this.menuRemoveMaxLimitLine.Name = "menuRemoveMaxLimitLine";
            this.menuRemoveMaxLimitLine.Size = new System.Drawing.Size(96, 22);
            this.menuRemoveMaxLimitLine.Text = "&Max";
            this.menuRemoveMaxLimitLine.Click += new System.EventHandler(this.OnRemoveMaxLimitLine_Click);
            // 
            // menuRemoveMinLimitLine
            // 
            this.menuRemoveMinLimitLine.Name = "menuRemoveMinLimitLine";
            this.menuRemoveMinLimitLine.Size = new System.Drawing.Size(96, 22);
            this.menuRemoveMinLimitLine.Text = "M&in";
            this.menuRemoveMinLimitLine.Click += new System.EventHandler(this.OnRemoveMinLimitLine_Click);
            // 
            // menuItemSoundAlarmLimitLine
            // 
            this.menuItemSoundAlarmLimitLine.CheckOnClick = true;
            this.menuItemSoundAlarmLimitLine.Name = "menuItemSoundAlarmLimitLine";
            this.menuItemSoundAlarmLimitLine.Size = new System.Drawing.Size(165, 22);
            this.menuItemSoundAlarmLimitLine.Text = "Sound &Alarm";
            this.menuItemSoundAlarmLimitLine.Click += new System.EventHandler(this.OnItemSoundAlarmLimitLine_Click);
            // 
            // menuMarkers
            // 
            this.menuMarkers.Name = "menuMarkers";
            this.menuMarkers.Size = new System.Drawing.Size(243, 22);
            this.menuMarkers.Text = "Mar&kers";
            // 
            // menuShowPeak
            // 
            this.menuShowPeak.CheckOnClick = true;
            this.menuShowPeak.Name = "menuShowPeak";
            this.menuShowPeak.Size = new System.Drawing.Size(243, 22);
            this.menuShowPeak.Text = "Show Auto Peak &values";
            this.menuShowPeak.CheckedChanged += new System.EventHandler(this.click_view_mode);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(240, 6);
            // 
            // menuCalculatorSignalModes
            // 
            this.menuCalculatorSignalModes.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuRealtimeTrace,
            this.menuAveragedTrace,
            this.menuMaxTrace,
            this.menuMinTrace,
            this.menuMaxHoldTrace});
            this.menuCalculatorSignalModes.Name = "menuCalculatorSignalModes";
            this.menuCalculatorSignalModes.Size = new System.Drawing.Size(243, 22);
            this.menuCalculatorSignalModes.Text = "Trace modes";
            // 
            // menuRealtimeTrace
            // 
            this.menuRealtimeTrace.CheckOnClick = true;
            this.menuRealtimeTrace.Name = "menuRealtimeTrace";
            this.menuRealtimeTrace.Size = new System.Drawing.Size(153, 22);
            this.menuRealtimeTrace.Text = "&Realtime data";
            this.menuRealtimeTrace.Click += new System.EventHandler(this.click_view_mode);
            // 
            // menuAveragedTrace
            // 
            this.menuAveragedTrace.CheckOnClick = true;
            this.menuAveragedTrace.Name = "menuAveragedTrace";
            this.menuAveragedTrace.Size = new System.Drawing.Size(153, 22);
            this.menuAveragedTrace.Text = "&Averaged data";
            this.menuAveragedTrace.Click += new System.EventHandler(this.click_view_mode);
            // 
            // menuMaxTrace
            // 
            this.menuMaxTrace.CheckOnClick = true;
            this.menuMaxTrace.Name = "menuMaxTrace";
            this.menuMaxTrace.Size = new System.Drawing.Size(153, 22);
            this.menuMaxTrace.Text = "Ma&x Peak data";
            this.menuMaxTrace.Click += new System.EventHandler(this.click_view_mode);
            // 
            // menuMinTrace
            // 
            this.menuMinTrace.CheckOnClick = true;
            this.menuMinTrace.Name = "menuMinTrace";
            this.menuMinTrace.Size = new System.Drawing.Size(153, 22);
            this.menuMinTrace.Text = "M&inimum data";
            this.menuMinTrace.Click += new System.EventHandler(this.click_view_mode);
            // 
            // menuMaxHoldTrace
            // 
            this.menuMaxHoldTrace.CheckOnClick = true;
            this.menuMaxHoldTrace.Name = "menuMaxHoldTrace";
            this.menuMaxHoldTrace.Size = new System.Drawing.Size(153, 22);
            this.menuMaxHoldTrace.Text = "Max &Hold data";
            this.menuMaxHoldTrace.Click += new System.EventHandler(this.click_view_mode);
            // 
            // menuSignalFill
            // 
            this.menuSignalFill.CheckOnClick = true;
            this.menuSignalFill.Name = "menuSignalFill";
            this.menuSignalFill.Size = new System.Drawing.Size(243, 22);
            this.menuSignalFill.Text = "&Fill Trace";
            this.menuSignalFill.Click += new System.EventHandler(this.OnSignalFill_Click);
            // 
            // menuSmoothSignals
            // 
            this.menuSmoothSignals.CheckOnClick = true;
            this.menuSmoothSignals.Name = "menuSmoothSignals";
            this.menuSmoothSignals.Size = new System.Drawing.Size(243, 22);
            this.menuSmoothSignals.Text = "Sm&ooth Trace";
            this.menuSmoothSignals.Click += new System.EventHandler(this.OnSmoothSignals_Click);
            // 
            // menuThickTrace
            // 
            this.menuThickTrace.Checked = true;
            this.menuThickTrace.CheckOnClick = true;
            this.menuThickTrace.CheckState = System.Windows.Forms.CheckState.Checked;
            this.menuThickTrace.Name = "menuThickTrace";
            this.menuThickTrace.Size = new System.Drawing.Size(243, 22);
            this.menuThickTrace.Text = "Thic&k Trace";
            this.menuThickTrace.Click += new System.EventHandler(this.OnThickTrace_Click);
            // 
            // toolStripSeparator19
            // 
            this.toolStripSeparator19.Name = "toolStripSeparator19";
            this.toolStripSeparator19.Size = new System.Drawing.Size(240, 6);
            // 
            // menuShowGrid
            // 
            this.menuShowGrid.Checked = true;
            this.menuShowGrid.CheckOnClick = true;
            this.menuShowGrid.CheckState = System.Windows.Forms.CheckState.Checked;
            this.menuShowGrid.Name = "menuShowGrid";
            this.menuShowGrid.Size = new System.Drawing.Size(243, 22);
            this.menuShowGrid.Text = "Show &Grid";
            this.menuShowGrid.Click += new System.EventHandler(this.OnShowGrid_Click);
            // 
            // menuShowAxisLabels
            // 
            this.menuShowAxisLabels.CheckOnClick = true;
            this.menuShowAxisLabels.Name = "menuShowAxisLabels";
            this.menuShowAxisLabels.Size = new System.Drawing.Size(243, 22);
            this.menuShowAxisLabels.Text = "Show &Axis Labels";
            this.menuShowAxisLabels.Click += new System.EventHandler(this.OnShowAxisLabels_Click);
            // 
            // menuUserDefinedText
            // 
            this.menuUserDefinedText.Name = "menuUserDefinedText";
            this.menuUserDefinedText.Size = new System.Drawing.Size(243, 22);
            this.menuUserDefinedText.Text = "User defined Text...";
            this.menuUserDefinedText.Click += new System.EventHandler(this.OnUserDefinedText_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(240, 6);
            // 
            // menuWaterfallInSA
            // 
            this.menuWaterfallInSA.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuPlaceWaterfallNone,
            this.menuPlaceWaterfallAtBottom,
            this.menuPlaceWaterfallOnTheRight});
            this.menuWaterfallInSA.Name = "menuWaterfallInSA";
            this.menuWaterfallInSA.Size = new System.Drawing.Size(243, 22);
            this.menuWaterfallInSA.Text = "Include Waterfall in main scree&n";
            // 
            // menuPlaceWaterfallNone
            // 
            this.menuPlaceWaterfallNone.Name = "menuPlaceWaterfallNone";
            this.menuPlaceWaterfallNone.Size = new System.Drawing.Size(171, 22);
            this.menuPlaceWaterfallNone.Text = "None";
            this.menuPlaceWaterfallNone.Click += new System.EventHandler(this.OnWaterfallPlaceNone_Click);
            // 
            // menuPlaceWaterfallAtBottom
            // 
            this.menuPlaceWaterfallAtBottom.Name = "menuPlaceWaterfallAtBottom";
            this.menuPlaceWaterfallAtBottom.Size = new System.Drawing.Size(171, 22);
            this.menuPlaceWaterfallAtBottom.Text = "Bottom alignment";
            this.menuPlaceWaterfallAtBottom.Click += new System.EventHandler(this.OnPlaceWaterfallAtBottom_Click);
            // 
            // menuPlaceWaterfallOnTheRight
            // 
            this.menuPlaceWaterfallOnTheRight.Name = "menuPlaceWaterfallOnTheRight";
            this.menuPlaceWaterfallOnTheRight.Size = new System.Drawing.Size(171, 22);
            this.menuPlaceWaterfallOnTheRight.Text = "Right alignment";
            this.menuPlaceWaterfallOnTheRight.Click += new System.EventHandler(this.OnPlaceWaterfallOnTheRight_Click);
            // 
            // menuWaterfallPerspective
            // 
            this.menuWaterfallPerspective.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuWaterfallPerspective1,
            this.menuWaterfallPerspective2,
            this.menuWaterfallPerspectiveIso,
            this.menuWaterfallPerspective2D});
            this.menuWaterfallPerspective.Name = "menuWaterfallPerspective";
            this.menuWaterfallPerspective.Size = new System.Drawing.Size(243, 22);
            this.menuWaterfallPerspective.Text = "&Waterfall perspective view";
            // 
            // menuWaterfallPerspective1
            // 
            this.menuWaterfallPerspective1.CheckOnClick = true;
            this.menuWaterfallPerspective1.Name = "menuWaterfallPerspective1";
            this.menuWaterfallPerspective1.Size = new System.Drawing.Size(143, 22);
            this.menuWaterfallPerspective1.Text = "Perspective &1";
            this.menuWaterfallPerspective1.Click += new System.EventHandler(this.OnWaterfallPerspective1_Click);
            // 
            // menuWaterfallPerspective2
            // 
            this.menuWaterfallPerspective2.CheckOnClick = true;
            this.menuWaterfallPerspective2.Name = "menuWaterfallPerspective2";
            this.menuWaterfallPerspective2.Size = new System.Drawing.Size(143, 22);
            this.menuWaterfallPerspective2.Text = "Perspective &2";
            this.menuWaterfallPerspective2.Click += new System.EventHandler(this.OnWaterfallPerspective2_Click);
            // 
            // menuWaterfallPerspectiveIso
            // 
            this.menuWaterfallPerspectiveIso.CheckOnClick = true;
            this.menuWaterfallPerspectiveIso.Name = "menuWaterfallPerspectiveIso";
            this.menuWaterfallPerspectiveIso.Size = new System.Drawing.Size(143, 22);
            this.menuWaterfallPerspectiveIso.Text = "&Isometric";
            this.menuWaterfallPerspectiveIso.Click += new System.EventHandler(this.OnWaterfallIsometric_Click);
            // 
            // menuWaterfallPerspective2D
            // 
            this.menuWaterfallPerspective2D.CheckOnClick = true;
            this.menuWaterfallPerspective2D.Name = "menuWaterfallPerspective2D";
            this.menuWaterfallPerspective2D.Size = new System.Drawing.Size(143, 22);
            this.menuWaterfallPerspective2D.Text = "2&D";
            this.menuWaterfallPerspective2D.Click += new System.EventHandler(this.OnWaterfall2D_Click);
            // 
            // menuTransparentWaterfall
            // 
            this.menuTransparentWaterfall.CheckOnClick = true;
            this.menuTransparentWaterfall.Name = "menuTransparentWaterfall";
            this.menuTransparentWaterfall.Size = new System.Drawing.Size(243, 22);
            this.menuTransparentWaterfall.Text = "&Transparent Waterfall";
            this.menuTransparentWaterfall.Click += new System.EventHandler(this.OnTransparentWaterfall_Click);
            // 
            // menuWaterfallFloor
            // 
            this.menuWaterfallFloor.CheckOnClick = true;
            this.menuWaterfallFloor.Name = "menuWaterfallFloor";
            this.menuWaterfallFloor.Size = new System.Drawing.Size(243, 22);
            this.menuWaterfallFloor.Text = "Wat&erfall floor visible";
            this.menuWaterfallFloor.Click += new System.EventHandler(this.OnWaterfallFloor_Click);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Size = new System.Drawing.Size(240, 6);
            // 
            // menuCleanReport
            // 
            this.menuCleanReport.Name = "menuCleanReport";
            this.menuCleanReport.Size = new System.Drawing.Size(243, 22);
            this.menuCleanReport.Text = "C&lean Report";
            this.menuCleanReport.Click += new System.EventHandler(this.OnCleanReport_Click);
            // 
            // menuDevice
            // 
            this.menuDevice.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuRFConnections,
            this.toolStripSeparator11,
            this.menuEnableMainboard,
            this.menuEnableExpansionBoard,
            this.toolStripSeparator10,
            this.menuRemoteMaxHold,
            this.menuRefreshRemoteMaxHold,
            this.toolStripSeparator21,
            this.menuAutoLCDOff,
            this.menuRemoteAmplitudeUpdate});
            this.menuDevice.Name = "menuDevice";
            this.menuDevice.Size = new System.Drawing.Size(54, 23);
            this.menuDevice.Text = "&Device";
            this.menuDevice.DropDownOpening += new System.EventHandler(this.menuDevice_DropDownOpening);
            // 
            // menuRFConnections
            // 
            this.menuRFConnections.CheckOnClick = true;
            this.menuRFConnections.Name = "menuRFConnections";
            this.menuRFConnections.Size = new System.Drawing.Size(330, 22);
            this.menuRFConnections.Text = "Show RF Explorer icon and RF Active &Connection";
            this.menuRFConnections.ToolTipText = "Show visual device icon including connected antennas and enabled RF SMA port";
            this.menuRFConnections.Click += new System.EventHandler(this.OnRFConnections_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(327, 6);
            // 
            // menuEnableMainboard
            // 
            this.menuEnableMainboard.Enabled = false;
            this.menuEnableMainboard.Name = "menuEnableMainboard";
            this.menuEnableMainboard.Size = new System.Drawing.Size(330, 22);
            this.menuEnableMainboard.Text = "Enable &left SMA connector";
            this.menuEnableMainboard.ToolTipText = "Use this option to enable remote device port on the left";
            this.menuEnableMainboard.Click += new System.EventHandler(this.OnEnableMainboard_Click);
            // 
            // menuEnableExpansionBoard
            // 
            this.menuEnableExpansionBoard.Enabled = false;
            this.menuEnableExpansionBoard.Name = "menuEnableExpansionBoard";
            this.menuEnableExpansionBoard.Size = new System.Drawing.Size(330, 22);
            this.menuEnableExpansionBoard.Text = "Enable &right SMA connector";
            this.menuEnableExpansionBoard.ToolTipText = "Use this option to enable remote device port on the right";
            this.menuEnableExpansionBoard.Click += new System.EventHandler(this.OnEnableExpansionBoard_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(327, 6);
            // 
            // menuRemoteMaxHold
            // 
            this.menuRemoteMaxHold.CheckOnClick = true;
            this.menuRemoteMaxHold.Name = "menuRemoteMaxHold";
            this.menuRemoteMaxHold.Size = new System.Drawing.Size(330, 22);
            this.menuRemoteMaxHold.Text = "Enable Remote MaxHold";
            this.menuRemoteMaxHold.ToolTipText = "Enable this to increase chances of fast changing signal capture, such as WiFi. Di" +
    "sable for extra accurate signal capture.";
            this.menuRemoteMaxHold.Click += new System.EventHandler(this.menuRemoteMaxHold_Click);
            // 
            // menuRefreshRemoteMaxHold
            // 
            this.menuRefreshRemoteMaxHold.Name = "menuRefreshRemoteMaxHold";
            this.menuRefreshRemoteMaxHold.Size = new System.Drawing.Size(330, 22);
            this.menuRefreshRemoteMaxHold.Text = "Refresh Remote MaxHold buffer";
            this.menuRefreshRemoteMaxHold.ToolTipText = "When Remote MaxHold mode is enabled for fast signal capture, it is suggested to c" +
    "lean remote capture buffer with this option or by clicking RETURN key in the RF " +
    "Explorer device keyboard";
            this.menuRefreshRemoteMaxHold.Click += new System.EventHandler(this.menuRefreshRemoteMaxHold_Click);
            // 
            // toolStripSeparator21
            // 
            this.toolStripSeparator21.Name = "toolStripSeparator21";
            this.toolStripSeparator21.Size = new System.Drawing.Size(327, 6);
            // 
            // menuAutoLCDOff
            // 
            this.menuAutoLCDOff.CheckOnClick = true;
            this.menuAutoLCDOff.Name = "menuAutoLCDOff";
            this.menuAutoLCDOff.Size = new System.Drawing.Size(330, 22);
            this.menuAutoLCDOff.Text = "Automatic LCD O&FF";
            this.menuAutoLCDOff.ToolTipText = "Automatically switch the LCD off for faster updates and help increase LCD lifetim" +
    "e";
            this.menuAutoLCDOff.Click += new System.EventHandler(this.OnAutoLCDOff_Click);
            // 
            // menuRemoteAmplitudeUpdate
            // 
            this.menuRemoteAmplitudeUpdate.CheckOnClick = true;
            this.menuRemoteAmplitudeUpdate.Name = "menuRemoteAmplitudeUpdate";
            this.menuRemoteAmplitudeUpdate.Size = new System.Drawing.Size(330, 22);
            this.menuRemoteAmplitudeUpdate.Text = "Automatic Update Remote Amplitude";
            this.menuRemoteAmplitudeUpdate.ToolTipText = "Use this option enabled to force the RF Explorer device to update amplitude visua" +
    "l range, or disable for fast local updates with no need to remote updates";
            // 
            // menuComboSavedOptions
            // 
            this.menuComboSavedOptions.AutoSize = false;
            this.menuComboSavedOptions.AutoToolTip = true;
            this.menuComboSavedOptions.MaxLength = 30;
            this.menuComboSavedOptions.Name = "menuComboSavedOptions";
            this.menuComboSavedOptions.Size = new System.Drawing.Size(180, 23);
            this.menuComboSavedOptions.Sorted = true;
            this.menuComboSavedOptions.ToolTipText = "Saved preconfigured options";
            this.menuComboSavedOptions.SelectedIndexChanged += new System.EventHandler(this.menuComboSavedOptions_SelectedIndexChanged);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuOnlineHelp,
            this.menuDeviceManual,
            this.toolStripSeparator8,
            this.menuReleaseNotes,
            this.menuWindowsReleaseNotes,
            this.toolStripSeparator9,
            this.menuPortInfo,
            this.menuAbout});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 23);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // menuOnlineHelp
            // 
            this.menuOnlineHelp.Name = "menuOnlineHelp";
            this.menuOnlineHelp.Size = new System.Drawing.Size(278, 22);
            this.menuOnlineHelp.Text = "RF Explorer for Windows Online &Help";
            this.menuOnlineHelp.Click += new System.EventHandler(this.OnOnlineHelp_Click);
            // 
            // menuDeviceManual
            // 
            this.menuDeviceManual.Name = "menuDeviceManual";
            this.menuDeviceManual.Size = new System.Drawing.Size(278, 22);
            this.menuDeviceManual.Text = "RF Explorer device User Manual";
            this.menuDeviceManual.Click += new System.EventHandler(this.OnDeviceManual_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(275, 6);
            // 
            // menuReleaseNotes
            // 
            this.menuReleaseNotes.Name = "menuReleaseNotes";
            this.menuReleaseNotes.Size = new System.Drawing.Size(278, 22);
            this.menuReleaseNotes.Text = "RF Explorer Firmware Release Notes";
            this.menuReleaseNotes.Click += new System.EventHandler(this.OnFirmware_Click);
            // 
            // menuWindowsReleaseNotes
            // 
            this.menuWindowsReleaseNotes.Name = "menuWindowsReleaseNotes";
            this.menuWindowsReleaseNotes.Size = new System.Drawing.Size(278, 22);
            this.menuWindowsReleaseNotes.Text = "RF Explorer for Windows Release Notes";
            this.menuWindowsReleaseNotes.Click += new System.EventHandler(this.OnWindowsReleaseNotes_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(275, 6);
            // 
            // menuPortInfo
            // 
            this.menuPortInfo.Name = "menuPortInfo";
            this.menuPortInfo.Size = new System.Drawing.Size(278, 22);
            this.menuPortInfo.Text = "Report COM port &info";
            this.menuPortInfo.Click += new System.EventHandler(this.OnCOMPortInfo_Click);
            // 
            // menuAbout
            // 
            this.menuAbout.Name = "menuAbout";
            this.menuAbout.Size = new System.Drawing.Size(278, 22);
            this.menuAbout.Text = "A&bout RF Explorer For Windows...";
            this.menuAbout.Click += new System.EventHandler(this.OnAbout_Click);
            // 
            // menuDebug
            // 
            this.menuDebug.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.menuDebug.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuDebug_datatest_1});
            this.menuDebug.Enabled = false;
            this.menuDebug.Name = "menuDebug";
            this.menuDebug.Size = new System.Drawing.Size(54, 23);
            this.menuDebug.Text = "Debug";
            this.menuDebug.Visible = false;
            // 
            // menuDebug_datatest_1
            // 
            this.menuDebug_datatest_1.Name = "menuDebug_datatest_1";
            this.menuDebug_datatest_1.Size = new System.Drawing.Size(165, 22);
            this.menuDebug_datatest_1.Text = "Create data test 1";
            this.menuDebug_datatest_1.Click += new System.EventHandler(this.OnDebug_datatest_1_Click);
            // 
            // m_timer_receive
            // 
            this.m_timer_receive.Interval = 50;
            this.m_timer_receive.Tick += new System.EventHandler(this.timer_receive_Tick);
            // 
            // m_MainTab
            // 
            this.m_MainTab.Controls.Add(this.m_tabSpectrumAnalyzer);
            this.m_MainTab.Controls.Add(this.m_tabWaterfall);
            this.m_MainTab.Controls.Add(this.m_tabPowerChannel);
            this.m_MainTab.Controls.Add(this.m_tabRemoteScreen);
            this.m_MainTab.Controls.Add(this.m_tabConfiguration);
            this.m_MainTab.Controls.Add(this.m_tabReport);
            this.m_MainTab.Dock = System.Windows.Forms.DockStyle.Top;
            this.m_MainTab.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.m_MainTab.Location = new System.Drawing.Point(0, 27);
            this.m_MainTab.Name = "m_MainTab";
            this.m_MainTab.Padding = new System.Drawing.Point(16, 5);
            this.m_MainTab.SelectedIndex = 0;
            this.m_MainTab.Size = new System.Drawing.Size(1092, 540);
            this.m_MainTab.TabIndex = 49;
            this.m_MainTab.ClientSizeChanged += new System.EventHandler(this.MainTab_ClientSizeChanged);
            this.m_MainTab.SizeChanged += new System.EventHandler(this.MainTab_ClientSizeChanged);
            this.m_MainTab.Resize += new System.EventHandler(this.MainTab_ClientSizeChanged);
            // 
            // m_tabSpectrumAnalyzer
            // 
            this.m_tabSpectrumAnalyzer.Controls.Add(this.m_group_CalibrateAmplitudeAnalyzer);
            this.m_tabSpectrumAnalyzer.Controls.Add(this.m_tableLayoutControlArea);
            this.m_tabSpectrumAnalyzer.Controls.Add(this.btnCenterMark);
            this.m_tabSpectrumAnalyzer.Controls.Add(this.btnSpanMin);
            this.m_tabSpectrumAnalyzer.Controls.Add(this.btnSpanDefault);
            this.m_tabSpectrumAnalyzer.Controls.Add(this.btnSpanMax);
            this.m_tabSpectrumAnalyzer.Controls.Add(this.btnBottom5minus);
            this.m_tabSpectrumAnalyzer.Controls.Add(this.btnBottom5plus);
            this.m_tabSpectrumAnalyzer.Controls.Add(this.btnTop5minus);
            this.m_tabSpectrumAnalyzer.Controls.Add(this.btnAutoscale);
            this.m_tabSpectrumAnalyzer.Controls.Add(this.btnSpanDec);
            this.m_tabSpectrumAnalyzer.Controls.Add(this.btnSpanInc);
            this.m_tabSpectrumAnalyzer.Controls.Add(this.btnMoveFreqDecSmall);
            this.m_tabSpectrumAnalyzer.Controls.Add(this.btnMoveFreqIncSmall);
            this.m_tabSpectrumAnalyzer.Controls.Add(this.btnTop5plus);
            this.m_tabSpectrumAnalyzer.Controls.Add(this.btnMoveFreqDecLarge);
            this.m_tabSpectrumAnalyzer.Controls.Add(this.btnMoveFreqIncLarge);
            this.m_tabSpectrumAnalyzer.Location = new System.Drawing.Point(4, 26);
            this.m_tabSpectrumAnalyzer.Name = "m_tabSpectrumAnalyzer";
            this.m_tabSpectrumAnalyzer.Padding = new System.Windows.Forms.Padding(3);
            this.m_tabSpectrumAnalyzer.Size = new System.Drawing.Size(1084, 510);
            this.m_tabSpectrumAnalyzer.TabIndex = 0;
            this.m_tabSpectrumAnalyzer.Text = "Spectrum Analyzer";
            this.m_tabSpectrumAnalyzer.UseVisualStyleBackColor = true;
            this.m_tabSpectrumAnalyzer.Enter += new System.EventHandler(this.tabSpectrumAnalyzer_Enter);
            // 
            // m_group_CalibrateAmplitudeAnalyzer
            // 
            this.m_group_CalibrateAmplitudeAnalyzer.Controls.Add(this.btnCalibratePurge);
            this.m_group_CalibrateAmplitudeAnalyzer.Controls.Add(this.chkRFE6GEN_CAL);
            this.m_group_CalibrateAmplitudeAnalyzer.Controls.Add(this.m_btnCalibrate1G);
            this.m_group_CalibrateAmplitudeAnalyzer.Controls.Add(this.m_btnCalibrate3G);
            this.m_group_CalibrateAmplitudeAnalyzer.Controls.Add(this.m_btnCalibrate6G);
            this.m_group_CalibrateAmplitudeAnalyzer.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.m_group_CalibrateAmplitudeAnalyzer.Location = new System.Drawing.Point(6, 271);
            this.m_group_CalibrateAmplitudeAnalyzer.Name = "m_group_CalibrateAmplitudeAnalyzer";
            this.m_group_CalibrateAmplitudeAnalyzer.Size = new System.Drawing.Size(314, 114);
            this.m_group_CalibrateAmplitudeAnalyzer.TabIndex = 62;
            this.m_group_CalibrateAmplitudeAnalyzer.TabStop = false;
            this.m_group_CalibrateAmplitudeAnalyzer.Text = "校准装置";
            this.m_MainFormTooltip.SetToolTip(this.m_group_CalibrateAmplitudeAnalyzer, "校准过程完成射频资源管理器中生产测试RF测试所需要的步骤。\r\nCalibration process to complete RF TEST REQUIRED s" +
        "tep in RF Explorer production tests.");
            this.m_group_CalibrateAmplitudeAnalyzer.Visible = false;
            // 
            // btnCalibratePurge
            // 
            this.btnCalibratePurge.BackColor = System.Drawing.Color.Red;
            this.btnCalibratePurge.ForeColor = System.Drawing.Color.White;
            this.btnCalibratePurge.Location = new System.Drawing.Point(209, 17);
            this.btnCalibratePurge.Name = "btnCalibratePurge";
            this.btnCalibratePurge.Size = new System.Drawing.Size(62, 60);
            this.btnCalibratePurge.TabIndex = 2;
            this.btnCalibratePurge.Text = "清洗\r\nPurge\r\n";
            this.m_MainFormTooltip.SetToolTip(this.btnCalibratePurge, "使用此当单位有先前的校准\r\nuse this when the unit has a previous calibration");
            this.btnCalibratePurge.UseVisualStyleBackColor = false;
            this.btnCalibratePurge.Click += new System.EventHandler(this.btnCalibratePurge_Click);
            // 
            // chkRFE6GEN_CAL
            // 
            this.chkRFE6GEN_CAL.AutoSize = true;
            this.chkRFE6GEN_CAL.Checked = true;
            this.chkRFE6GEN_CAL.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRFE6GEN_CAL.Location = new System.Drawing.Point(6, 86);
            this.chkRFE6GEN_CAL.Name = "chkRFE6GEN_CAL";
            this.chkRFE6GEN_CAL.Size = new System.Drawing.Size(166, 17);
            this.chkRFE6GEN_CAL.TabIndex = 1;
            this.chkRFE6GEN_CAL.Text = "Use Generator RFE6GEN CAL";
            this.m_MainFormTooltip.SetToolTip(this.chkRFE6GEN_CAL, "需要的，以便使用来自信号发生器的内部校准 - 这应当在所有情况下被启用\r\nRequired in order to use the internal calibr" +
        "ation from Signal Generator - This should be checked in all cases");
            this.chkRFE6GEN_CAL.UseVisualStyleBackColor = true;
            // 
            // m_btnCalibrate1G
            // 
            this.m_btnCalibrate1G.BackColor = System.Drawing.Color.PeachPuff;
            this.m_btnCalibrate1G.Enabled = false;
            this.m_btnCalibrate1G.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_btnCalibrate1G.Location = new System.Drawing.Point(142, 17);
            this.m_btnCalibrate1G.Name = "m_btnCalibrate1G";
            this.m_btnCalibrate1G.Size = new System.Drawing.Size(62, 60);
            this.m_btnCalibrate1G.TabIndex = 0;
            this.m_btnCalibrate1G.Text = "1G";
            this.m_MainFormTooltip.SetToolTip(this.m_btnCalibrate1G, "用这个来校准射频资源管理器WSUB1G\r\nuse this to calibrate a RF Explorer WSUB1G\r\n");
            this.m_btnCalibrate1G.UseVisualStyleBackColor = false;
            this.m_btnCalibrate1G.Click += new System.EventHandler(this.m_btnCalibrate1G_Click);
            // 
            // m_btnCalibrate3G
            // 
            this.m_btnCalibrate3G.BackColor = System.Drawing.Color.PeachPuff;
            this.m_btnCalibrate3G.Enabled = false;
            this.m_btnCalibrate3G.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_btnCalibrate3G.Location = new System.Drawing.Point(74, 17);
            this.m_btnCalibrate3G.Name = "m_btnCalibrate3G";
            this.m_btnCalibrate3G.Size = new System.Drawing.Size(62, 60);
            this.m_btnCalibrate3G.TabIndex = 0;
            this.m_btnCalibrate3G.Text = "3G";
            this.m_MainFormTooltip.SetToolTip(this.m_btnCalibrate3G, "用这个来校准射频资源管理器WSUB3G\r\nuse this to calibrate a RFEMWSUB3G");
            this.m_btnCalibrate3G.UseVisualStyleBackColor = false;
            this.m_btnCalibrate3G.Click += new System.EventHandler(this.m_btnCalibrate3G_Click);
            // 
            // m_btnCalibrate6G
            // 
            this.m_btnCalibrate6G.BackColor = System.Drawing.Color.PeachPuff;
            this.m_btnCalibrate6G.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_btnCalibrate6G.Location = new System.Drawing.Point(6, 17);
            this.m_btnCalibrate6G.Name = "m_btnCalibrate6G";
            this.m_btnCalibrate6G.Size = new System.Drawing.Size(62, 60);
            this.m_btnCalibrate6G.TabIndex = 0;
            this.m_btnCalibrate6G.Text = "6G";
            this.m_MainFormTooltip.SetToolTip(this.m_btnCalibrate6G, "用这个来校准射频资源管理器6G\r\nuse this to calibrate a RF Explorer 6G");
            this.m_btnCalibrate6G.UseVisualStyleBackColor = false;
            this.m_btnCalibrate6G.Click += new System.EventHandler(this.m_btnCalibrate6G_Click);
            // 
            // m_tableLayoutControlArea
            // 
            this.m_tableLayoutControlArea.AutoSize = true;
            this.m_tableLayoutControlArea.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.m_tableLayoutControlArea.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.m_tableLayoutControlArea.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.m_tableLayoutControlArea.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.AddColumns;
            this.m_tableLayoutControlArea.Location = new System.Drawing.Point(953, 6);
            this.m_tableLayoutControlArea.Name = "m_tableLayoutControlArea";
            this.m_tableLayoutControlArea.Size = new System.Drawing.Size(27, 30);
            this.m_tableLayoutControlArea.TabIndex = 51;
            // 
            // btnCenterMark
            // 
            this.btnCenterMark.AutoSize = true;
            this.btnCenterMark.Location = new System.Drawing.Point(935, 233);
            this.btnCenterMark.MinimumSize = new System.Drawing.Size(80, 23);
            this.btnCenterMark.Name = "btnCenterMark";
            this.btnCenterMark.Size = new System.Drawing.Size(80, 23);
            this.btnCenterMark.TabIndex = 60;
            this.btnCenterMark.Text = "Center Mark";
            this.m_MainFormTooltip.SetToolTip(this.btnCenterMark, "Useful command that will use the current peak frequency and will center it on scr" +
        "een, moving the start/end frequency accordingly");
            this.btnCenterMark.UseVisualStyleBackColor = true;
            this.btnCenterMark.Visible = false;
            this.btnCenterMark.Click += new System.EventHandler(this.OnCenterMark_Click);
            // 
            // btnSpanMin
            // 
            this.btnSpanMin.AutoSize = true;
            this.btnSpanMin.Location = new System.Drawing.Point(849, 316);
            this.btnSpanMin.MinimumSize = new System.Drawing.Size(80, 23);
            this.btnSpanMin.Name = "btnSpanMin";
            this.btnSpanMin.Size = new System.Drawing.Size(80, 23);
            this.btnSpanMin.TabIndex = 59;
            this.btnSpanMin.Text = "Span Min";
            this.m_MainFormTooltip.SetToolTip(this.btnSpanMin, "Decrease the frequency span to the minimum available for the active module");
            this.btnSpanMin.UseVisualStyleBackColor = true;
            this.btnSpanMin.Visible = false;
            this.btnSpanMin.Click += new System.EventHandler(this.OnSpanMin_Click);
            // 
            // btnSpanDefault
            // 
            this.btnSpanDefault.AutoSize = true;
            this.btnSpanDefault.Location = new System.Drawing.Point(849, 288);
            this.btnSpanDefault.MinimumSize = new System.Drawing.Size(80, 23);
            this.btnSpanDefault.Name = "btnSpanDefault";
            this.btnSpanDefault.Size = new System.Drawing.Size(80, 23);
            this.btnSpanDefault.TabIndex = 58;
            this.btnSpanDefault.Text = "Span 10MHz";
            this.m_MainFormTooltip.SetToolTip(this.btnSpanDefault, "Set the frequency span to a common use 10MHz");
            this.btnSpanDefault.UseVisualStyleBackColor = true;
            this.btnSpanDefault.Visible = false;
            this.btnSpanDefault.Click += new System.EventHandler(this.OnSpanDefault_Click);
            // 
            // btnSpanMax
            // 
            this.btnSpanMax.AutoSize = true;
            this.btnSpanMax.Location = new System.Drawing.Point(849, 260);
            this.btnSpanMax.MinimumSize = new System.Drawing.Size(80, 23);
            this.btnSpanMax.Name = "btnSpanMax";
            this.btnSpanMax.Size = new System.Drawing.Size(80, 23);
            this.btnSpanMax.TabIndex = 57;
            this.btnSpanMax.Text = "Span Max";
            this.m_MainFormTooltip.SetToolTip(this.btnSpanMax, "Increase the frequency span to the maximum available for the active module");
            this.btnSpanMax.UseVisualStyleBackColor = true;
            this.btnSpanMax.Visible = false;
            this.btnSpanMax.Click += new System.EventHandler(this.OnSpanMax_Click);
            // 
            // btnBottom5minus
            // 
            this.btnBottom5minus.AutoSize = true;
            this.btnBottom5minus.Location = new System.Drawing.Point(849, 456);
            this.btnBottom5minus.MinimumSize = new System.Drawing.Size(80, 23);
            this.btnBottom5minus.Name = "btnBottom5minus";
            this.btnBottom5minus.Size = new System.Drawing.Size(80, 23);
            this.btnBottom5minus.TabIndex = 56;
            this.btnBottom5minus.Text = "Bottom -5dB";
            this.m_MainFormTooltip.SetToolTip(this.btnBottom5minus, "Decrease the visual amplitude in 5dB at the bottom");
            this.btnBottom5minus.UseVisualStyleBackColor = true;
            this.btnBottom5minus.Visible = false;
            this.btnBottom5minus.Click += new System.EventHandler(this.OnBottom5minus_Click);
            // 
            // btnBottom5plus
            // 
            this.btnBottom5plus.AutoSize = true;
            this.btnBottom5plus.Location = new System.Drawing.Point(849, 428);
            this.btnBottom5plus.MinimumSize = new System.Drawing.Size(80, 23);
            this.btnBottom5plus.Name = "btnBottom5plus";
            this.btnBottom5plus.Size = new System.Drawing.Size(80, 23);
            this.btnBottom5plus.TabIndex = 55;
            this.btnBottom5plus.Text = "Bottom +5dB";
            this.m_MainFormTooltip.SetToolTip(this.btnBottom5plus, "Increase the visual amplitude in 5dB at the bottom");
            this.btnBottom5plus.UseVisualStyleBackColor = true;
            this.btnBottom5plus.Visible = false;
            this.btnBottom5plus.Click += new System.EventHandler(this.OnBottom5plus_Click);
            // 
            // btnTop5minus
            // 
            this.btnTop5minus.AutoSize = true;
            this.btnTop5minus.Location = new System.Drawing.Point(935, 204);
            this.btnTop5minus.MinimumSize = new System.Drawing.Size(80, 23);
            this.btnTop5minus.Name = "btnTop5minus";
            this.btnTop5minus.Size = new System.Drawing.Size(80, 23);
            this.btnTop5minus.TabIndex = 54;
            this.btnTop5minus.Text = "Top -5dB";
            this.m_MainFormTooltip.SetToolTip(this.btnTop5minus, "Decrease the visual amplitude in 5dB at the top");
            this.btnTop5minus.UseVisualStyleBackColor = true;
            this.btnTop5minus.Visible = false;
            this.btnTop5minus.Click += new System.EventHandler(this.OnTop5minus_Click);
            // 
            // btnAutoscale
            // 
            this.btnAutoscale.AutoSize = true;
            this.btnAutoscale.Location = new System.Drawing.Point(935, 204);
            this.btnAutoscale.MinimumSize = new System.Drawing.Size(80, 23);
            this.btnAutoscale.Name = "btnAutoscale";
            this.btnAutoscale.Size = new System.Drawing.Size(80, 23);
            this.btnAutoscale.TabIndex = 54;
            this.btnAutoscale.Text = "Autoscale";
            this.m_MainFormTooltip.SetToolTip(this.btnAutoscale, "Autoscale amplitude to a confortable visual level based on selected visual signal" +
        " modes");
            this.btnAutoscale.UseVisualStyleBackColor = true;
            this.btnAutoscale.Visible = false;
            this.btnAutoscale.Click += new System.EventHandler(this.OnAutoscale_Click);
            // 
            // btnSpanDec
            // 
            this.btnSpanDec.AutoSize = true;
            this.btnSpanDec.Location = new System.Drawing.Point(849, 345);
            this.btnSpanDec.MinimumSize = new System.Drawing.Size(80, 23);
            this.btnSpanDec.Name = "btnSpanDec";
            this.btnSpanDec.Size = new System.Drawing.Size(80, 23);
            this.btnSpanDec.TabIndex = 53;
            this.btnSpanDec.Text = "Span -25%";
            this.btnSpanDec.UseVisualStyleBackColor = true;
            this.btnSpanDec.Visible = false;
            this.btnSpanDec.Click += new System.EventHandler(this.OnSpanDec_Click);
            // 
            // btnSpanInc
            // 
            this.btnSpanInc.AutoSize = true;
            this.btnSpanInc.Location = new System.Drawing.Point(849, 233);
            this.btnSpanInc.MinimumSize = new System.Drawing.Size(80, 23);
            this.btnSpanInc.Name = "btnSpanInc";
            this.btnSpanInc.Size = new System.Drawing.Size(80, 23);
            this.btnSpanInc.TabIndex = 53;
            this.btnSpanInc.Text = "Span +25%";
            this.m_MainFormTooltip.SetToolTip(this.btnSpanInc, "Increase the frequency span by a 25%");
            this.btnSpanInc.UseVisualStyleBackColor = true;
            this.btnSpanInc.Visible = false;
            this.btnSpanInc.Click += new System.EventHandler(this.OnSpanInc_Click);
            // 
            // btnMoveFreqDecSmall
            // 
            this.btnMoveFreqDecSmall.AutoSize = true;
            this.btnMoveFreqDecSmall.Location = new System.Drawing.Point(849, 204);
            this.btnMoveFreqDecSmall.MinimumSize = new System.Drawing.Size(80, 23);
            this.btnMoveFreqDecSmall.Name = "btnMoveFreqDecSmall";
            this.btnMoveFreqDecSmall.Size = new System.Drawing.Size(80, 23);
            this.btnMoveFreqDecSmall.TabIndex = 53;
            this.btnMoveFreqDecSmall.Text = "Start < 10%";
            this.m_MainFormTooltip.SetToolTip(this.btnMoveFreqDecSmall, "Offset to a lower frequency by 10% of the span");
            this.btnMoveFreqDecSmall.UseVisualStyleBackColor = true;
            this.btnMoveFreqDecSmall.Visible = false;
            this.btnMoveFreqDecSmall.Click += new System.EventHandler(this.OnMoveFreqDecSmall_Click);
            // 
            // btnMoveFreqIncSmall
            // 
            this.btnMoveFreqIncSmall.AutoSize = true;
            this.btnMoveFreqIncSmall.Location = new System.Drawing.Point(849, 372);
            this.btnMoveFreqIncSmall.MinimumSize = new System.Drawing.Size(80, 23);
            this.btnMoveFreqIncSmall.Name = "btnMoveFreqIncSmall";
            this.btnMoveFreqIncSmall.Size = new System.Drawing.Size(80, 23);
            this.btnMoveFreqIncSmall.TabIndex = 53;
            this.btnMoveFreqIncSmall.Text = "End > 10%";
            this.m_MainFormTooltip.SetToolTip(this.btnMoveFreqIncSmall, "Offset to a higher frequency by 10% of the span");
            this.btnMoveFreqIncSmall.UseVisualStyleBackColor = true;
            this.btnMoveFreqIncSmall.Visible = false;
            this.btnMoveFreqIncSmall.Click += new System.EventHandler(this.OnMoveFreqIncSmall_Click);
            // 
            // btnTop5plus
            // 
            this.btnTop5plus.AutoSize = true;
            this.btnTop5plus.Location = new System.Drawing.Point(935, 175);
            this.btnTop5plus.MinimumSize = new System.Drawing.Size(80, 23);
            this.btnTop5plus.Name = "btnTop5plus";
            this.btnTop5plus.Size = new System.Drawing.Size(80, 23);
            this.btnTop5plus.TabIndex = 53;
            this.btnTop5plus.Text = "Top +5dB";
            this.m_MainFormTooltip.SetToolTip(this.btnTop5plus, "Increase the visual amplitude in 5dB at the top");
            this.btnTop5plus.UseVisualStyleBackColor = true;
            this.btnTop5plus.Visible = false;
            this.btnTop5plus.Click += new System.EventHandler(this.OnTop5plus_Click);
            // 
            // btnMoveFreqDecLarge
            // 
            this.btnMoveFreqDecLarge.AutoSize = true;
            this.btnMoveFreqDecLarge.Location = new System.Drawing.Point(849, 176);
            this.btnMoveFreqDecLarge.MinimumSize = new System.Drawing.Size(80, 23);
            this.btnMoveFreqDecLarge.Name = "btnMoveFreqDecLarge";
            this.btnMoveFreqDecLarge.Size = new System.Drawing.Size(80, 23);
            this.btnMoveFreqDecLarge.TabIndex = 53;
            this.btnMoveFreqDecLarge.Text = "Start < 50%";
            this.m_MainFormTooltip.SetToolTip(this.btnMoveFreqDecLarge, "Offset to a lower frequency by 50% of the span");
            this.btnMoveFreqDecLarge.UseVisualStyleBackColor = true;
            this.btnMoveFreqDecLarge.Visible = false;
            this.btnMoveFreqDecLarge.Click += new System.EventHandler(this.OnMoveFreqDecLarge_Click);
            // 
            // btnMoveFreqIncLarge
            // 
            this.btnMoveFreqIncLarge.AutoSize = true;
            this.btnMoveFreqIncLarge.Location = new System.Drawing.Point(849, 400);
            this.btnMoveFreqIncLarge.MinimumSize = new System.Drawing.Size(80, 23);
            this.btnMoveFreqIncLarge.Name = "btnMoveFreqIncLarge";
            this.btnMoveFreqIncLarge.Size = new System.Drawing.Size(80, 23);
            this.btnMoveFreqIncLarge.TabIndex = 53;
            this.btnMoveFreqIncLarge.Text = "End > 50%";
            this.m_MainFormTooltip.SetToolTip(this.btnMoveFreqIncLarge, "Offset to a higher frequency by 50% of the span");
            this.btnMoveFreqIncLarge.UseVisualStyleBackColor = true;
            this.btnMoveFreqIncLarge.Visible = false;
            this.btnMoveFreqIncLarge.Click += new System.EventHandler(this.OnMoveFreqIncLarge_Click);
            // 
            // m_tabWaterfall
            // 
            this.m_tabWaterfall.Location = new System.Drawing.Point(4, 26);
            this.m_tabWaterfall.Name = "m_tabWaterfall";
            this.m_tabWaterfall.Padding = new System.Windows.Forms.Padding(3);
            this.m_tabWaterfall.Size = new System.Drawing.Size(1084, 510);
            this.m_tabWaterfall.TabIndex = 3;
            this.m_tabWaterfall.Text = "Waterfall";
            this.m_tabWaterfall.UseVisualStyleBackColor = true;
            this.m_tabWaterfall.SizeChanged += new System.EventHandler(this.MainTab_ClientSizeChanged);
            this.m_tabWaterfall.Enter += new System.EventHandler(this.tabWaterfall_Enter);
            this.m_tabWaterfall.Resize += new System.EventHandler(this.MainTab_ClientSizeChanged);

            // 
            // menuContextWaterfall
            // 
            this.menuContextWaterfall.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuWaterfallContextMaxHold,
            this.menuWaterfallContextRealtime,
            this.toolStripSeparator18,
            this.menuWaterfallContextPerspective1,
            this.menuWaterfallContextPerspective2,
            this.menuWaterfallContextISO,
            this.menuWaterfallContext2D,
            this.toolStripSeparator16,
            this.menuWaterfallContextTransparent,
            this.menuWaterfallContextFloor,
            this.toolStripSeparator17,
            this.menuWaterfallContext_saveBitmap});
            this.menuContextWaterfall.Name = "contextMenuStrip1";
            this.menuContextWaterfall.Size = new System.Drawing.Size(219, 220);
            // 
            // menuWaterfallContextMaxHold
            // 
            this.menuWaterfallContextMaxHold.Name = "menuWaterfallContextMaxHold";
            this.menuWaterfallContextMaxHold.Size = new System.Drawing.Size(218, 22);
            this.menuWaterfallContextMaxHold.Text = "Max Hold";
            this.menuWaterfallContextMaxHold.Click += new System.EventHandler(this.OnWaterfallContextMaxHold_Click);
            // 
            // menuWaterfallContextRealtime
            // 
            this.menuWaterfallContextRealtime.Name = "menuWaterfallContextRealtime";
            this.menuWaterfallContextRealtime.Size = new System.Drawing.Size(218, 22);
            this.menuWaterfallContextRealtime.Text = "Realtime";
            this.menuWaterfallContextRealtime.Click += new System.EventHandler(this.OnWaterfallContextRealtime_Click);
            // 
            // toolStripSeparator18
            // 
            this.toolStripSeparator18.Name = "toolStripSeparator18";
            this.toolStripSeparator18.Size = new System.Drawing.Size(215, 6);
            // 
            // menuWaterfallContextPerspective1
            // 
            this.menuWaterfallContextPerspective1.Checked = true;
            this.menuWaterfallContextPerspective1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.menuWaterfallContextPerspective1.Name = "menuWaterfallContextPerspective1";
            this.menuWaterfallContextPerspective1.Size = new System.Drawing.Size(218, 22);
            this.menuWaterfallContextPerspective1.Text = "Perspective 1";
            this.menuWaterfallContextPerspective1.Click += new System.EventHandler(this.OnWaterfallPerspective1_Click);
            // 
            // menuWaterfallContextPerspective2
            // 
            this.menuWaterfallContextPerspective2.Name = "menuWaterfallContextPerspective2";
            this.menuWaterfallContextPerspective2.Size = new System.Drawing.Size(218, 22);
            this.menuWaterfallContextPerspective2.Text = "Perspective 2";
            this.menuWaterfallContextPerspective2.Click += new System.EventHandler(this.OnWaterfallPerspective2_Click);
            // 
            // menuWaterfallContextISO
            // 
            this.menuWaterfallContextISO.Name = "menuWaterfallContextISO";
            this.menuWaterfallContextISO.Size = new System.Drawing.Size(218, 22);
            this.menuWaterfallContextISO.Text = "ISO";
            this.menuWaterfallContextISO.Click += new System.EventHandler(this.OnWaterfallIsometric_Click);
            // 
            // menuWaterfallContext2D
            // 
            this.menuWaterfallContext2D.Name = "menuWaterfallContext2D";
            this.menuWaterfallContext2D.Size = new System.Drawing.Size(218, 22);
            this.menuWaterfallContext2D.Text = "2D";
            this.menuWaterfallContext2D.Click += new System.EventHandler(this.OnWaterfall2D_Click);
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            this.toolStripSeparator16.Size = new System.Drawing.Size(215, 6);
            // 
            // menuWaterfallContextTransparent
            // 
            this.menuWaterfallContextTransparent.CheckOnClick = true;
            this.menuWaterfallContextTransparent.Name = "menuWaterfallContextTransparent";
            this.menuWaterfallContextTransparent.Size = new System.Drawing.Size(218, 22);
            this.menuWaterfallContextTransparent.Text = "Transparent";
            this.menuWaterfallContextTransparent.Click += new System.EventHandler(this.OnTransparentWaterfall_Click);
            // 
            // menuWaterfallContextFloor
            // 
            this.menuWaterfallContextFloor.CheckOnClick = true;
            this.menuWaterfallContextFloor.Name = "menuWaterfallContextFloor";
            this.menuWaterfallContextFloor.Size = new System.Drawing.Size(218, 22);
            this.menuWaterfallContextFloor.Text = "Visible floor";
            this.menuWaterfallContextFloor.Click += new System.EventHandler(this.OnWaterfallFloor_Click);
            // 
            // toolStripSeparator17
            // 
            this.toolStripSeparator17.Name = "toolStripSeparator17";
            this.toolStripSeparator17.Size = new System.Drawing.Size(215, 6);
            // 
            // menuWaterfallContext_saveBitmap
            // 
            this.menuWaterfallContext_saveBitmap.Name = "menuWaterfallContext_saveBitmap";
            this.menuWaterfallContext_saveBitmap.Size = new System.Drawing.Size(218, 22);
            this.menuWaterfallContext_saveBitmap.Text = "Save Waterfall Screenshot...";
            this.menuWaterfallContext_saveBitmap.Click += new System.EventHandler(this.OnSaveImage_Click);
            // 
            // m_tabPowerChannel
            // 
            this.m_tabPowerChannel.Controls.Add(this.m_panelPowerChannel);
            this.m_tabPowerChannel.Location = new System.Drawing.Point(4, 26);
            this.m_tabPowerChannel.Name = "m_tabPowerChannel";
            this.m_tabPowerChannel.Size = new System.Drawing.Size(1084, 510);
            this.m_tabPowerChannel.TabIndex = 5;
            this.m_tabPowerChannel.Text = "Power Channel";
            this.m_tabPowerChannel.UseVisualStyleBackColor = true;
            this.m_tabPowerChannel.Enter += new System.EventHandler(this.tabPowerChannel_Enter);
            // 
            // m_panelPowerChannel
            // 
            this.m_panelPowerChannel.Location = new System.Drawing.Point(169, 101);
            this.m_panelPowerChannel.Name = "m_panelPowerChannel";
            this.m_panelPowerChannel.Size = new System.Drawing.Size(200, 100);
            this.m_panelPowerChannel.TabIndex = 0;
            // 
            // m_tabRemoteScreen
            // 
            this.m_tabRemoteScreen.Controls.Add(this.m_panelRemoteScreen);
            this.m_tabRemoteScreen.Location = new System.Drawing.Point(4, 26);
            this.m_tabRemoteScreen.Name = "m_tabRemoteScreen";
            this.m_tabRemoteScreen.Size = new System.Drawing.Size(1084, 510);
            this.m_tabRemoteScreen.TabIndex = 2;
            this.m_tabRemoteScreen.Text = "Remote Screen";
            this.m_tabRemoteScreen.UseVisualStyleBackColor = true;
            this.m_tabRemoteScreen.Enter += new System.EventHandler(this.tabRemoteScreen_Enter);
            // 
            // m_panelRemoteScreen
            // 
            this.m_panelRemoteScreen.Controls.Add(this.controlRemoteScreen);
            this.m_panelRemoteScreen.Location = new System.Drawing.Point(12, 140);
            this.m_panelRemoteScreen.Name = "m_panelRemoteScreen";
            this.m_panelRemoteScreen.Size = new System.Drawing.Size(912, 363);
            this.m_panelRemoteScreen.TabIndex = 55;
            // 
            // controlRemoteScreen
            // 
            this.controlRemoteScreen.HeaderText = true;
            this.controlRemoteScreen.LCDColor = true;
            this.controlRemoteScreen.LCDGrid = true;
            this.controlRemoteScreen.Location = new System.Drawing.Point(0, 0);
            this.controlRemoteScreen.Margin = new System.Windows.Forms.Padding(4);
            this.controlRemoteScreen.Name = "controlRemoteScreen";
            this.controlRemoteScreen.RFExplorer = null;
            this.controlRemoteScreen.Size = new System.Drawing.Size(292, 174);
            this.controlRemoteScreen.TabIndex = 54;
            // 
            // m_tabConfiguration
            // 
            this.m_tabConfiguration.Controls.Add(this.m_panelGeneralConfigTab);
            this.m_tabConfiguration.Location = new System.Drawing.Point(4, 26);
            this.m_tabConfiguration.Name = "m_tabConfiguration";
            this.m_tabConfiguration.Padding = new System.Windows.Forms.Padding(3);
            this.m_tabConfiguration.Size = new System.Drawing.Size(1084, 510);
            this.m_tabConfiguration.TabIndex = 3;
            this.m_tabConfiguration.Text = "Configuration";
            this.m_tabConfiguration.UseVisualStyleBackColor = true;
            this.m_tabConfiguration.Enter += new System.EventHandler(this.tabConfiguration_Enter);
            // 
            // m_panelGeneralConfigTab
            // 
            this.m_panelGeneralConfigTab.AutoSize = true;
            this.m_panelGeneralConfigTab.Controls.Add(this.m_tableConfiguration);
            this.m_panelGeneralConfigTab.Location = new System.Drawing.Point(12, 140);
            this.m_panelGeneralConfigTab.Name = "m_panelGeneralConfigTab";
            this.m_panelGeneralConfigTab.Size = new System.Drawing.Size(912, 347);
            this.m_panelGeneralConfigTab.TabIndex = 56;
            // 
            // m_tableConfiguration
            // 
            this.m_tableConfiguration.ColumnCount = 3;
            this.m_tableConfiguration.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 250F));
            this.m_tableConfiguration.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 440F));
            this.m_tableConfiguration.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.m_tableConfiguration.Controls.Add(this.groupCalibration, 0, 1);
            this.m_tableConfiguration.Controls.Add(this.groupBoxFiles, 0, 0);
            this.m_tableConfiguration.Controls.Add(this.m_panelRFConnections, 2, 0);
            this.m_tableConfiguration.Controls.Add(this.m_groupSignalTypeConfiguration, 1, 1);
            this.m_tableConfiguration.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_tableConfiguration.Location = new System.Drawing.Point(0, 0);
            this.m_tableConfiguration.Name = "m_tableConfiguration";
            this.m_tableConfiguration.RowCount = 2;
            this.m_tableConfiguration.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.m_tableConfiguration.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.m_tableConfiguration.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.m_tableConfiguration.Size = new System.Drawing.Size(912, 347);
            this.m_tableConfiguration.TabIndex = 9;
            // 
            // groupCalibration
            // 
            this.groupCalibration.AutoSize = true;
            this.groupCalibration.Controls.Add(this.tableLayoutPanel10);
            this.groupCalibration.Location = new System.Drawing.Point(3, 115);
            this.groupCalibration.MaximumSize = new System.Drawing.Size(244, 110);
            this.groupCalibration.Name = "groupCalibration";
            this.groupCalibration.Size = new System.Drawing.Size(244, 110);
            this.groupCalibration.TabIndex = 4;
            this.groupCalibration.TabStop = false;
            this.groupCalibration.Text = "Frequency Calibration";
            // 
            // tableLayoutPanel10
            // 
            this.tableLayoutPanel10.AutoSize = true;
            this.tableLayoutPanel10.ColumnCount = 2;
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 44.73684F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 55.26316F));
            this.tableLayoutPanel10.Controls.Add(this.btnCalibrate, 1, 1);
            this.tableLayoutPanel10.Controls.Add(this.m_edCalibrationFreq, 1, 0);
            this.tableLayoutPanel10.Controls.Add(this.label19, 0, 0);
            this.tableLayoutPanel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel10.Location = new System.Drawing.Point(3, 17);
            this.tableLayoutPanel10.Name = "tableLayoutPanel10";
            this.tableLayoutPanel10.RowCount = 2;
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel10.Size = new System.Drawing.Size(238, 90);
            this.tableLayoutPanel10.TabIndex = 3;
            // 
            // btnCalibrate
            // 
            this.btnCalibrate.AutoSize = true;
            this.btnCalibrate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCalibrate.Location = new System.Drawing.Point(109, 35);
            this.btnCalibrate.Name = "btnCalibrate";
            this.btnCalibrate.Size = new System.Drawing.Size(126, 52);
            this.btnCalibrate.TabIndex = 4;
            this.btnCalibrate.Text = "Calibrate...";
            this.m_MainFormTooltip.SetToolTip(this.btnCalibrate, "Perform calibration for all models (currently 2.4G model not supported)");
            this.btnCalibrate.UseVisualStyleBackColor = true;
            this.btnCalibrate.Click += new System.EventHandler(this.btnCalibrate_Click);
            // 
            // m_edCalibrationFreq
            // 
            this.m_edCalibrationFreq.BackColor = System.Drawing.Color.RoyalBlue;
            this.m_edCalibrationFreq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_edCalibrationFreq.Font = new System.Drawing.Font("Digital-7", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_edCalibrationFreq.ForeColor = System.Drawing.Color.White;
            this.m_edCalibrationFreq.Location = new System.Drawing.Point(109, 3);
            this.m_edCalibrationFreq.Name = "m_edCalibrationFreq";
            this.m_edCalibrationFreq.Size = new System.Drawing.Size(126, 26);
            this.m_edCalibrationFreq.TabIndex = 3;
            this.m_edCalibrationFreq.Text = "910";
            this.m_edCalibrationFreq.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_MainFormTooltip.SetToolTip(this.m_edCalibrationFreq, "Please read user manual for details on calibration frequency");
            this.m_edCalibrationFreq.Leave += new System.EventHandler(this.m_edCalibrationFreq_Leave);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label19.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.label19.ForeColor = System.Drawing.Color.DarkBlue;
            this.label19.Location = new System.Drawing.Point(3, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(100, 32);
            this.label19.TabIndex = 2;
            this.label19.Text = "REFERENCE (MHz)";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBoxFiles
            // 
            this.groupBoxFiles.AutoSize = true;
            this.groupBoxFiles.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.m_tableConfiguration.SetColumnSpan(this.groupBoxFiles, 2);
            this.groupBoxFiles.Controls.Add(this.tableLayoutPanel9);
            this.groupBoxFiles.Location = new System.Drawing.Point(3, 3);
            this.groupBoxFiles.Name = "groupBoxFiles";
            this.groupBoxFiles.Size = new System.Drawing.Size(670, 106);
            this.groupBoxFiles.TabIndex = 7;
            this.groupBoxFiles.TabStop = false;
            this.groupBoxFiles.Text = "Configuration files and folders";
            // 
            // tableLayoutPanel9
            // 
            this.tableLayoutPanel9.AutoSize = true;
            this.tableLayoutPanel9.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel9.ColumnCount = 3;
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel9.Controls.Add(this.label20, 0, 0);
            this.tableLayoutPanel9.Controls.Add(this.comboCSVFieldSeparator, 1, 2);
            this.tableLayoutPanel9.Controls.Add(this.edDefaultFilePath, 1, 0);
            this.tableLayoutPanel9.Controls.Add(this.label21, 0, 2);
            this.tableLayoutPanel9.Controls.Add(this.labelReportFile, 0, 1);
            this.tableLayoutPanel9.Controls.Add(this.btnOpenLog, 2, 1);
            this.tableLayoutPanel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel9.Location = new System.Drawing.Point(3, 17);
            this.tableLayoutPanel9.MinimumSize = new System.Drawing.Size(664, 84);
            this.tableLayoutPanel9.Name = "tableLayoutPanel9";
            this.tableLayoutPanel9.RowCount = 3;
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel9.Size = new System.Drawing.Size(664, 86);
            this.tableLayoutPanel9.TabIndex = 11;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label20.Location = new System.Drawing.Point(3, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(150, 27);
            this.label20.TabIndex = 5;
            this.label20.Text = "Default Output data file path:";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboCSVFieldSeparator
            // 
            this.comboCSVFieldSeparator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCSVFieldSeparator.FormattingEnabled = true;
            this.comboCSVFieldSeparator.Items.AddRange(new object[] {
            "Comma (,)",
            "Division (|)",
            "Semicolon (;)",
            "Space ( )",
            "Tabulator (\\t)"});
            this.comboCSVFieldSeparator.Location = new System.Drawing.Point(159, 62);
            this.comboCSVFieldSeparator.Name = "comboCSVFieldSeparator";
            this.comboCSVFieldSeparator.Size = new System.Drawing.Size(154, 21);
            this.comboCSVFieldSeparator.Sorted = true;
            this.comboCSVFieldSeparator.TabIndex = 10;
            this.m_MainFormTooltip.SetToolTip(this.comboCSVFieldSeparator, "Delimiter to use for CSV file generation");
            // 
            // edDefaultFilePath
            // 
            this.tableLayoutPanel9.SetColumnSpan(this.edDefaultFilePath, 2);
            this.edDefaultFilePath.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.edDefaultFilePath.Location = new System.Drawing.Point(159, 3);
            this.edDefaultFilePath.Name = "edDefaultFilePath";
            this.edDefaultFilePath.Size = new System.Drawing.Size(502, 21);
            this.edDefaultFilePath.TabIndex = 6;
            this.m_MainFormTooltip.SetToolTip(this.edDefaultFilePath, "Default path for data file output");
            this.edDefaultFilePath.Leave += new System.EventHandler(this.edDefaultFilePath_Leave);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label21.Location = new System.Drawing.Point(3, 59);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(150, 27);
            this.label21.TabIndex = 9;
            this.label21.Text = "CSV field separator:";
            this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelReportFile
            // 
            this.labelReportFile.AutoSize = true;
            this.labelReportFile.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tableLayoutPanel9.SetColumnSpan(this.labelReportFile, 2);
            this.labelReportFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelReportFile.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelReportFile.Location = new System.Drawing.Point(3, 27);
            this.labelReportFile.Name = "labelReportFile";
            this.labelReportFile.Size = new System.Drawing.Size(568, 32);
            this.labelReportFile.TabIndex = 7;
            this.labelReportFile.Text = "Report log file:";
            this.labelReportFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.m_MainFormTooltip.SetToolTip(this.labelReportFile, "Full path of the report log file, you may need this upon Support request");
            // 
            // btnOpenLog
            // 
            this.btnOpenLog.AutoSize = true;
            this.btnOpenLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnOpenLog.Location = new System.Drawing.Point(577, 30);
            this.btnOpenLog.Name = "btnOpenLog";
            this.btnOpenLog.Size = new System.Drawing.Size(84, 26);
            this.btnOpenLog.TabIndex = 8;
            this.btnOpenLog.Text = "Open Log";
            this.m_MainFormTooltip.SetToolTip(this.btnOpenLog, "Open report and diagnosis log file");
            this.btnOpenLog.UseVisualStyleBackColor = true;
            this.btnOpenLog.Click += new System.EventHandler(this.OnOpenLog_Click);
            // 
            // m_panelRFConnections
            // 
            this.m_panelRFConnections.BackColor = System.Drawing.Color.Transparent;
            this.m_panelRFConnections.Controls.Add(this.m_controlRFModuleSelectorConfig);
            this.m_panelRFConnections.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_panelRFConnections.Location = new System.Drawing.Point(693, 3);
            this.m_panelRFConnections.Name = "m_panelRFConnections";
            this.m_tableConfiguration.SetRowSpan(this.m_panelRFConnections, 3);
            this.m_panelRFConnections.Size = new System.Drawing.Size(220, 341);
            this.m_panelRFConnections.TabIndex = 8;
            // 
            // m_controlRFModuleSelectorConfig
            // 
            this.m_controlRFModuleSelectorConfig.ActualPictureHeight = 341;
            this.m_controlRFModuleSelectorConfig.ActualPictureWidth = 162;
            this.m_controlRFModuleSelectorConfig.AutoSize = true;
            this.m_controlRFModuleSelectorConfig.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.m_controlRFModuleSelectorConfig.BackColor = System.Drawing.Color.Transparent;
            this.m_controlRFModuleSelectorConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_controlRFModuleSelectorConfig.Location = new System.Drawing.Point(0, 0);
            this.m_controlRFModuleSelectorConfig.Margin = new System.Windows.Forms.Padding(4);
            this.m_controlRFModuleSelectorConfig.Name = "m_controlRFModuleSelectorConfig";
            this.m_controlRFModuleSelectorConfig.Size = new System.Drawing.Size(220, 341);
            this.m_controlRFModuleSelectorConfig.TabIndex = 0;
            this.m_controlRFModuleSelectorConfig.HideControl += new System.EventHandler(this.OnRFModuleSelectorConfig_HideControl);
            // 
            // m_groupSignalTypeConfiguration
            // 
            this.m_groupSignalTypeConfiguration.AutoSize = true;
            this.m_groupSignalTypeConfiguration.Controls.Add(this.tableLayoutPanel5);
            this.m_groupSignalTypeConfiguration.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_groupSignalTypeConfiguration.Location = new System.Drawing.Point(253, 115);
            this.m_groupSignalTypeConfiguration.Name = "m_groupSignalTypeConfiguration";
            this.m_groupSignalTypeConfiguration.Size = new System.Drawing.Size(434, 110);
            this.m_groupSignalTypeConfiguration.TabIndex = 9;
            this.m_groupSignalTypeConfiguration.TabStop = false;
            this.m_groupSignalTypeConfiguration.Text = "Signal style configuration";
            this.m_groupSignalTypeConfiguration.Visible = false;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 6;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 41F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.Controls.Add(this.label23, 2, 0);
            this.tableLayoutPanel5.Controls.Add(this.m_chkPeakValue, 5, 1);
            this.tableLayoutPanel5.Controls.Add(this.m_chkFilledGraph, 5, 2);
            this.tableLayoutPanel5.Controls.Add(this.label22, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.m_SignalTypeList, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.m_chkVisibleCurve, 5, 0);
            this.tableLayoutPanel5.Controls.Add(this.numericUpDown1, 3, 0);
            this.tableLayoutPanel5.Controls.Add(this.label24, 2, 1);
            this.tableLayoutPanel5.Controls.Add(this.label25, 2, 2);
            this.tableLayoutPanel5.Controls.Add(this.m_bLineColor, 3, 1);
            this.tableLayoutPanel5.Controls.Add(this.m_bFillColor, 3, 2);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 17);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 3;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.Size = new System.Drawing.Size(428, 90);
            this.tableLayoutPanel5.TabIndex = 0;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label23.Location = new System.Drawing.Point(176, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(101, 27);
            this.label23.TabIndex = 1;
            this.label23.Text = "Line Thickness";
            this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // m_chkPeakValue
            // 
            this.m_chkPeakValue.AutoSize = true;
            this.m_chkPeakValue.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.m_chkPeakValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_chkPeakValue.Location = new System.Drawing.Point(344, 30);
            this.m_chkPeakValue.Name = "m_chkPeakValue";
            this.m_chkPeakValue.Size = new System.Drawing.Size(81, 23);
            this.m_chkPeakValue.TabIndex = 3;
            this.m_chkPeakValue.Text = "Peak value";
            this.m_chkPeakValue.UseVisualStyleBackColor = true;
            // 
            // m_chkFilledGraph
            // 
            this.m_chkFilledGraph.AutoSize = true;
            this.m_chkFilledGraph.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.m_chkFilledGraph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_chkFilledGraph.Location = new System.Drawing.Point(344, 59);
            this.m_chkFilledGraph.Name = "m_chkFilledGraph";
            this.m_chkFilledGraph.Size = new System.Drawing.Size(81, 79);
            this.m_chkFilledGraph.TabIndex = 1;
            this.m_chkFilledGraph.Text = "Filled graph";
            this.m_chkFilledGraph.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.m_chkFilledGraph.UseVisualStyleBackColor = true;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label22.Location = new System.Drawing.Point(3, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(60, 27);
            this.label22.TabIndex = 2;
            this.label22.Text = "Signal type";
            this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_SignalTypeList
            // 
            this.m_SignalTypeList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_SignalTypeList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_SignalTypeList.FormattingEnabled = true;
            this.m_SignalTypeList.Items.AddRange(new object[] {
            "Average",
            "Realtime",
            "Minimum",
            "Max Peak",
            "Max Hold"});
            this.m_SignalTypeList.Location = new System.Drawing.Point(69, 3);
            this.m_SignalTypeList.Name = "m_SignalTypeList";
            this.m_SignalTypeList.Size = new System.Drawing.Size(101, 21);
            this.m_SignalTypeList.TabIndex = 0;
            // 
            // m_chkVisibleCurve
            // 
            this.m_chkVisibleCurve.AutoSize = true;
            this.m_chkVisibleCurve.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.m_chkVisibleCurve.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_chkVisibleCurve.Location = new System.Drawing.Point(344, 3);
            this.m_chkVisibleCurve.Name = "m_chkVisibleCurve";
            this.m_chkVisibleCurve.Size = new System.Drawing.Size(81, 21);
            this.m_chkVisibleCurve.TabIndex = 4;
            this.m_chkVisibleCurve.Text = "Visible";
            this.m_chkVisibleCurve.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.m_chkVisibleCurve.UseVisualStyleBackColor = true;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numericUpDown1.Location = new System.Drawing.Point(283, 3);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(35, 21);
            this.numericUpDown1.TabIndex = 0;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label24.Location = new System.Drawing.Point(176, 27);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(101, 29);
            this.label24.TabIndex = 5;
            this.label24.Text = "Line Color";
            this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label25.Location = new System.Drawing.Point(176, 56);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(101, 85);
            this.label25.TabIndex = 6;
            this.label25.Text = "Fill Color";
            this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // m_bLineColor
            // 
            this.m_bLineColor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_bLineColor.Location = new System.Drawing.Point(283, 30);
            this.m_bLineColor.Name = "m_bLineColor";
            this.m_bLineColor.Size = new System.Drawing.Size(35, 23);
            this.m_bLineColor.TabIndex = 7;
            this.m_bLineColor.UseVisualStyleBackColor = true;
            // 
            // m_bFillColor
            // 
            this.m_bFillColor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_bFillColor.Location = new System.Drawing.Point(283, 59);
            this.m_bFillColor.Name = "m_bFillColor";
            this.m_bFillColor.Size = new System.Drawing.Size(35, 79);
            this.m_bFillColor.TabIndex = 8;
            this.m_bFillColor.UseVisualStyleBackColor = true;
            // 
            // m_tabReport
            // 
            this.m_tabReport.Controls.Add(this.m_ReportTextBox);
            this.m_tabReport.Location = new System.Drawing.Point(4, 26);
            this.m_tabReport.Name = "m_tabReport";
            this.m_tabReport.Padding = new System.Windows.Forms.Padding(3);
            this.m_tabReport.Size = new System.Drawing.Size(1084, 510);
            this.m_tabReport.TabIndex = 1;
            this.m_tabReport.Text = "Report";
            this.m_tabReport.UseVisualStyleBackColor = true;
            this.m_tabReport.Enter += new System.EventHandler(this.tabReport_Enter);
            // 
            // m_ReportTextBox
            // 
            this.m_ReportTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.m_ReportTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_ReportTextBox.ForeColor = System.Drawing.Color.DarkBlue;
            this.m_ReportTextBox.Location = new System.Drawing.Point(6, 120);
            this.m_ReportTextBox.Multiline = true;
            this.m_ReportTextBox.Name = "m_ReportTextBox";
            this.m_ReportTextBox.ReadOnly = true;
            this.m_ReportTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.m_ReportTextBox.Size = new System.Drawing.Size(920, 383);
            this.m_ReportTextBox.TabIndex = 49;
            this.m_ReportTextBox.WordWrap = false;
            // 
            // m_MainStatusBar
            // 
            this.m_MainStatusBar.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.m_MainStatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolCOMStatus,
            this.toolStripMemory,
            this.toolStripSamples,
            this.toolFile});
            this.m_MainStatusBar.Location = new System.Drawing.Point(0, 546);
            this.m_MainStatusBar.Name = "m_MainStatusBar";
            this.m_MainStatusBar.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.m_MainStatusBar.Size = new System.Drawing.Size(1092, 22);
            this.m_MainStatusBar.TabIndex = 51;
            this.m_MainStatusBar.Text = "statusStrip1";
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
            // toolStripSamples
            // 
            this.toolStripSamples.Name = "toolStripSamples";
            this.toolStripSamples.Size = new System.Drawing.Size(101, 17);
            this.toolStripSamples.Text = "Samples: 0/0 - 0%";
            // 
            // toolFile
            // 
            this.toolFile.Name = "toolFile";
            this.toolFile.Size = new System.Drawing.Size(58, 17);
            this.toolFile.Text = "File: none";
            // 
            // tabRAWDecoder
            // 
            this.tabRAWDecoder.Location = new System.Drawing.Point(0, 0);
            this.tabRAWDecoder.Name = "tabRAWDecoder";
            this.tabRAWDecoder.Size = new System.Drawing.Size(200, 100);
            this.tabRAWDecoder.TabIndex = 0;
            // 
            // groupDemodulator
            // 
            this.groupDemodulator.Location = new System.Drawing.Point(0, 0);
            this.groupDemodulator.Name = "groupDemodulator";
            this.groupDemodulator.Size = new System.Drawing.Size(200, 100);
            this.groupDemodulator.TabIndex = 0;
            this.groupDemodulator.TabStop = false;
            // 
            // chkPSK
            // 
            this.chkPSK.Location = new System.Drawing.Point(0, 0);
            this.chkPSK.Name = "chkPSK";
            this.chkPSK.Size = new System.Drawing.Size(104, 24);
            this.chkPSK.TabIndex = 0;
            // 
            // chkOOK
            // 
            this.chkOOK.Location = new System.Drawing.Point(0, 0);
            this.chkOOK.Name = "chkOOK";
            this.chkOOK.Size = new System.Drawing.Size(104, 24);
            this.chkOOK.TabIndex = 0;
            // 
            // m_sBaudRate
            // 
            this.m_sBaudRate.Location = new System.Drawing.Point(0, 0);
            this.m_sBaudRate.Name = "m_sBaudRate";
            this.m_sBaudRate.Size = new System.Drawing.Size(100, 20);
            this.m_sBaudRate.TabIndex = 0;
            // 
            // label18
            // 
            this.label18.Location = new System.Drawing.Point(0, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(100, 23);
            this.label18.TabIndex = 0;
            // 
            // label17
            // 
            this.label17.Location = new System.Drawing.Point(0, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(100, 23);
            this.label17.TabIndex = 0;
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(0, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(100, 23);
            this.label16.TabIndex = 0;
            // 
            // groupRAWDecoder
            // 
            this.groupRAWDecoder.Location = new System.Drawing.Point(0, 0);
            this.groupRAWDecoder.Name = "groupRAWDecoder";
            this.groupRAWDecoder.Size = new System.Drawing.Size(200, 100);
            this.groupRAWDecoder.TabIndex = 0;
            this.groupRAWDecoder.TabStop = false;
            // 
            // chkRunDecoder
            // 
            this.chkRunDecoder.Location = new System.Drawing.Point(0, 0);
            this.chkRunDecoder.Name = "chkRunDecoder";
            this.chkRunDecoder.Size = new System.Drawing.Size(104, 24);
            this.chkRunDecoder.TabIndex = 0;
            // 
            // chkHoldDecoder
            // 
            this.chkHoldDecoder.Location = new System.Drawing.Point(0, 0);
            this.chkHoldDecoder.Name = "chkHoldDecoder";
            this.chkHoldDecoder.Size = new System.Drawing.Size(104, 24);
            this.chkHoldDecoder.TabIndex = 0;
            // 
            // btnSaveRAWDecoderCSV
            // 
            this.btnSaveRAWDecoderCSV.Location = new System.Drawing.Point(0, 0);
            this.btnSaveRAWDecoderCSV.Name = "btnSaveRAWDecoderCSV";
            this.btnSaveRAWDecoderCSV.Size = new System.Drawing.Size(75, 23);
            this.btnSaveRAWDecoderCSV.TabIndex = 0;
            // 
            // numMultiGraph
            // 
            this.numMultiGraph.Location = new System.Drawing.Point(0, 0);
            this.numMultiGraph.Name = "numMultiGraph";
            this.numMultiGraph.Size = new System.Drawing.Size(120, 20);
            this.numMultiGraph.TabIndex = 0;
            // 
            // label15
            // 
            this.label15.Location = new System.Drawing.Point(0, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(100, 23);
            this.label15.TabIndex = 0;
            // 
            // numSampleDecoder
            // 
            this.numSampleDecoder.Location = new System.Drawing.Point(0, 0);
            this.numSampleDecoder.Name = "numSampleDecoder";
            this.numSampleDecoder.Size = new System.Drawing.Size(120, 20);
            this.numSampleDecoder.TabIndex = 0;
            // 
            // label14
            // 
            this.label14.Location = new System.Drawing.Point(0, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(100, 23);
            this.label14.TabIndex = 0;
            // 
            // m_MainFormTooltip
            // 
            this.m_MainFormTooltip.AutomaticDelay = 1500;
            this.m_MainFormTooltip.AutoPopDelay = 15000;
            this.m_MainFormTooltip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.m_MainFormTooltip.ForeColor = System.Drawing.Color.Blue;
            this.m_MainFormTooltip.InitialDelay = 500;
            this.m_MainFormTooltip.ReshowDelay = 300;
            // 
            // btnSaveSettings
            // 
            this.btnSaveSettings.AutoSize = true;
            this.btnSaveSettings.Location = new System.Drawing.Point(383, 4);
            this.btnSaveSettings.Name = "btnSaveSettings";
            this.btnSaveSettings.Size = new System.Drawing.Size(46, 23);
            this.btnSaveSettings.TabIndex = 50;
            this.btnSaveSettings.Text = "Save";
            this.m_MainFormTooltip.SetToolTip(this.btnSaveSettings, "Save current settings to the selected name");
            this.btnSaveSettings.UseVisualStyleBackColor = true;
            this.btnSaveSettings.Click += new System.EventHandler(this.OnSaveSettings_Click);
            // 
            // btnDelSettings
            // 
            this.btnDelSettings.AutoSize = true;
            this.btnDelSettings.Location = new System.Drawing.Point(435, 4);
            this.btnDelSettings.Name = "btnDelSettings";
            this.btnDelSettings.Size = new System.Drawing.Size(46, 23);
            this.btnDelSettings.TabIndex = 50;
            this.btnDelSettings.Text = "Del";
            this.m_MainFormTooltip.SetToolTip(this.btnDelSettings, "Delete the selected named settings");
            this.btnDelSettings.UseVisualStyleBackColor = true;
            this.btnDelSettings.Click += new System.EventHandler(this.OnDeleteSettings_Click);
            // 
            // btnLoadSettings
            // 
            this.btnLoadSettings.AutoSize = true;
            this.btnLoadSettings.Location = new System.Drawing.Point(331, 4);
            this.btnLoadSettings.Name = "btnLoadSettings";
            this.btnLoadSettings.Size = new System.Drawing.Size(46, 23);
            this.btnLoadSettings.TabIndex = 51;
            this.btnLoadSettings.Text = "Load";
            this.m_MainFormTooltip.SetToolTip(this.btnLoadSettings, "Load and apply the selected settings");
            this.btnLoadSettings.UseVisualStyleBackColor = true;
            this.btnLoadSettings.Click += new System.EventHandler(this.OnLoadSettings_Click);
            // 
            // printPreviewDialog
            // 
            this.printPreviewDialog.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.printPreviewDialog.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.printPreviewDialog.ClientSize = new System.Drawing.Size(400, 300);
            this.printPreviewDialog.Document = this.printMainDocument;
            this.printPreviewDialog.Enabled = true;
            this.printPreviewDialog.Icon = ((System.Drawing.Icon)(resources.GetObject("printPreviewDialog.Icon")));
            this.printPreviewDialog.Name = "printPreviewDialog";
            this.printPreviewDialog.Visible = false;
            // 
            // printDialog
            // 
            this.printDialog.UseEXDialog = true;
            // 
            // zedRAWDecoder
            // 
            this.zedRAWDecoder.Location = new System.Drawing.Point(0, 0);
            this.zedRAWDecoder.Margin = new System.Windows.Forms.Padding(4);
            this.zedRAWDecoder.Name = "zedRAWDecoder";
            this.zedRAWDecoder.ScrollGrace = 0D;
            this.zedRAWDecoder.ScrollMaxX = 0D;
            this.zedRAWDecoder.ScrollMaxY = 0D;
            this.zedRAWDecoder.ScrollMaxY2 = 0D;
            this.zedRAWDecoder.ScrollMinX = 0D;
            this.zedRAWDecoder.ScrollMinY = 0D;
            this.zedRAWDecoder.ScrollMinY2 = 0D;
            this.zedRAWDecoder.Size = new System.Drawing.Size(150, 150);
            this.zedRAWDecoder.TabIndex = 0;
            this.zedRAWDecoder.UseExtendedPrintDialog = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Info;
            this.ClientSize = new System.Drawing.Size(1092, 568);
            this.Controls.Add(this.m_MainStatusBar);
            this.Controls.Add(this.btnLoadSettings);
            this.Controls.Add(this.btnDelSettings);
            this.Controls.Add(this.btnSaveSettings);
            this.Controls.Add(this.m_MainTab);
            this.Controls.Add(this.MainMenu);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.MainMenu;
            this.MinimumSize = new System.Drawing.Size(950, 600);
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "  RF Explorer for Windows";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Layout += new System.Windows.Forms.LayoutEventHandler(this.MainForm_Layout);
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            this.m_MainTab.ResumeLayout(false);
            this.m_tabSpectrumAnalyzer.ResumeLayout(false);
            this.m_tabSpectrumAnalyzer.PerformLayout();
            this.m_group_CalibrateAmplitudeAnalyzer.ResumeLayout(false);
            this.m_group_CalibrateAmplitudeAnalyzer.PerformLayout();
            this.m_tabWaterfall.ResumeLayout(false);
            this.menuContextWaterfall.ResumeLayout(false);
            this.m_tabPowerChannel.ResumeLayout(false);
            this.m_tabRemoteScreen.ResumeLayout(false);
			this.m_tabRemoteScreen.PerformLayout();            this.m_panelRemoteScreen.ResumeLayout(false);
            this.m_tabConfiguration.ResumeLayout(false);
            this.m_tabConfiguration.PerformLayout();
            this.m_panelGeneralConfigTab.ResumeLayout(false);
            this.m_tableConfiguration.ResumeLayout(false);
            this.m_tableConfiguration.PerformLayout();
            this.groupCalibration.ResumeLayout(false);
            this.groupCalibration.PerformLayout();
            this.tableLayoutPanel10.ResumeLayout(false);
            this.tableLayoutPanel10.PerformLayout();
            this.groupBoxFiles.ResumeLayout(false);
            this.groupBoxFiles.PerformLayout();
            this.tableLayoutPanel9.ResumeLayout(false);
            this.tableLayoutPanel9.PerformLayout();
            this.m_panelRFConnections.ResumeLayout(false);
            this.m_panelRFConnections.PerformLayout();
            this.m_groupSignalTypeConfiguration.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.m_tabReport.ResumeLayout(false);
            this.m_tabReport.PerformLayout();
            this.m_MainStatusBar.ResumeLayout(false);
            this.m_MainStatusBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMultiGraph)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSampleDecoder)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.MenuStrip MainMenu;
        public System.Windows.Forms.ToolStripMenuItem MainFileMenu;
        public System.Windows.Forms.ToolStripMenuItem menuSaveAsRFE;
        public System.Windows.Forms.ToolStripMenuItem menuExitApp;
        public System.Windows.Forms.Timer m_timer_receive;
        public System.Windows.Forms.ToolStripMenuItem menuAbout;
        public System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        public System.Windows.Forms.ToolStripMenuItem MainViewMenu;
        public System.Windows.Forms.ToolStripMenuItem menuRealtimeTrace;
        public System.Windows.Forms.ToolStripMenuItem menuAveragedTrace;
        public System.Windows.Forms.ToolStripMenuItem menuMaxTrace;
        public System.Windows.Forms.ToolStripMenuItem menuMinTrace;
        public System.Windows.Forms.ToolStripMenuItem menuLoadRFE;
        public System.Windows.Forms.ToolStripMenuItem menuSaveOnClose;
        public System.Windows.Forms.TabControl m_MainTab;
        public System.Windows.Forms.TabPage m_tabSpectrumAnalyzer;
        public System.Windows.Forms.StatusStrip m_MainStatusBar;
        public System.Windows.Forms.ToolStripStatusLabel toolCOMStatus;
        public System.Windows.Forms.ToolStripProgressBar toolStripMemory;
        public System.Windows.Forms.ToolStripStatusLabel toolFile;
        public System.Windows.Forms.TabPage m_tabReport;
        public System.Windows.Forms.TextBox m_ReportTextBox;
        public System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        public System.Windows.Forms.ToolStripMenuItem menuCleanReport;
        public System.Windows.Forms.ToolStripMenuItem menuReinitializeData;
        public System.Windows.Forms.ToolStripMenuItem menuSaveCSV;
        public System.Windows.Forms.TabPage m_tabRemoteScreen;
        public System.Windows.Forms.ToolStripMenuItem menuSaveRemoteImage;
        public System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        public System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        public System.Windows.Forms.ToolStripMenuItem menuSaveRFS;
        public System.Windows.Forms.ToolStripMenuItem menuLoadRFS;
        public System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        public System.Windows.Forms.ToolStripMenuItem menuShowPeak;
        public System.Windows.Forms.Panel m_panelRemoteScreen;
        public System.Windows.Forms.TabPage m_tabConfiguration;
        public System.Windows.Forms.ToolStripMenuItem menuShowControlArea;
        public System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        public System.Windows.Forms.ToolStripMenuItem menuDarkMode;
        public System.Windows.Forms.TabPage tabRAWDecoder;
        private ZedGraph.ZedGraphControl zedRAWDecoder;
        public System.Windows.Forms.GroupBox groupRAWDecoder;
        public System.Windows.Forms.NumericUpDown numMultiGraph;
        public System.Windows.Forms.Label label15;
        public System.Windows.Forms.NumericUpDown numSampleDecoder;
        public System.Windows.Forms.Label label14;
        public System.Windows.Forms.Label label18;
        public System.Windows.Forms.TextBox m_sBaudRate;
        public System.Windows.Forms.Label label17;
        public System.Windows.Forms.Label label16;
        public System.Windows.Forms.Button btnSaveRAWDecoderCSV;
        public System.Windows.Forms.RadioButton chkPSK;
        public System.Windows.Forms.RadioButton chkOOK;
        public System.Windows.Forms.GroupBox groupDemodulator;
        public System.Windows.Forms.CheckBox chkRunDecoder;
        public System.Windows.Forms.CheckBox chkHoldDecoder;
        public System.Windows.Forms.Button btnSpanDec;
        public System.Windows.Forms.Button btnSpanInc;
        public System.Windows.Forms.Button btnMoveFreqDecSmall;
        public System.Windows.Forms.Button btnMoveFreqIncSmall;
        public System.Windows.Forms.Button btnMoveFreqDecLarge;
        public System.Windows.Forms.Button btnMoveFreqIncLarge;
        public System.Windows.Forms.Button btnTop5plus;
        public System.Windows.Forms.Button btnTop5minus;
        public System.Windows.Forms.Button btnAutoscale;
        public System.Windows.Forms.Button btnBottom5minus;
        public System.Windows.Forms.Button btnBottom5plus;
        public System.Windows.Forms.Button btnSpanMin;
        public System.Windows.Forms.Button btnSpanDefault;
        public System.Windows.Forms.Button btnSpanMax;
        public System.Windows.Forms.Button btnCenterMark;
        public System.Windows.Forms.Panel m_panelGeneralConfigTab;
        public System.Windows.Forms.GroupBox groupCalibration;
        public System.Windows.Forms.TextBox m_edCalibrationFreq;
        public System.Windows.Forms.Label label19;
        public System.Windows.Forms.Button btnCalibrate;
        public System.Windows.Forms.TextBox edDefaultFilePath;
        public System.Windows.Forms.Label label20;
        public System.Windows.Forms.GroupBox groupBoxFiles;
        public System.Windows.Forms.Button btnOpenLog;
        public System.Windows.Forms.Label labelReportFile;
        public System.Windows.Forms.ToolStripComboBox menuComboSavedOptions;
        public System.Windows.Forms.ToolTip m_MainFormTooltip;
        public System.Windows.Forms.Button btnSaveSettings;
        public System.Windows.Forms.Button btnDelSettings;
        public System.Windows.Forms.ToolStripMenuItem menusSaveSimpleCSV;
        public System.Windows.Forms.Label label21;
        public System.Windows.Forms.ComboBox comboCSVFieldSeparator;
        public System.Windows.Forms.Button btnLoadSettings;
        public System.Windows.Forms.ToolStripMenuItem menuContinuousLog;
        private RFEClientControls.RemoteScreenControl controlRemoteScreen;
        public System.Windows.Forms.Panel m_panelRFConnections;
        private RFEClientControls.RFModuleSelector m_controlRFModuleSelectorConfig;
        public System.Windows.Forms.ToolStripMenuItem menuDevice;
        public System.Windows.Forms.ToolStripMenuItem menuEnableMainboard;
        public System.Windows.Forms.ToolStripMenuItem menuEnableExpansionBoard;
        public System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem menuOnlineHelp;
        public System.Windows.Forms.ToolStripMenuItem menuDeviceManual;
        public System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        public System.Windows.Forms.ToolStripMenuItem menuReleaseNotes;
        public System.Windows.Forms.ToolStripMenuItem menuWindowsReleaseNotes;
        public System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        public System.Windows.Forms.ToolStripStatusLabel toolStripSamples;
        public System.Windows.Forms.ToolStripMenuItem menuMaxHoldTrace;
        public System.Windows.Forms.ToolStripMenuItem menuRFConnections;
        public System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        public System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        public System.Windows.Forms.ToolStripMenuItem menuAutoLCDOff;
        public System.Windows.Forms.ToolStripMenuItem menuPrintPreview;
        public System.Windows.Forms.ToolStripMenuItem menuPrint;
        public System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        public System.Drawing.Printing.PrintDocument printMainDocument;
        public System.Windows.Forms.PrintPreviewDialog printPreviewDialog;
        public System.Windows.Forms.PrintDialog printDialog;
        public System.Windows.Forms.ToolStripMenuItem menuPageSetup;
        public System.Windows.Forms.TabPage m_tabWaterfall;
        public System.Windows.Forms.ContextMenuStrip menuContextWaterfall;
        public System.Windows.Forms.ToolStripMenuItem menuWaterfallContextPerspective1;
        public System.Windows.Forms.ToolStripMenuItem menuWaterfallContextPerspective2;
        public System.Windows.Forms.ToolStripMenuItem menuWaterfallContextISO;
        public System.Windows.Forms.ToolStripMenuItem menuWaterfallContext2D;
        public System.Windows.Forms.ToolStripMenuItem menuSaveBitmap;
        public System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
        public System.Windows.Forms.ToolStripMenuItem menuTransparentWaterfall;
        public System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
        public System.Windows.Forms.TableLayoutPanel m_tableConfiguration;
        public System.Windows.Forms.GroupBox m_groupSignalTypeConfiguration;
        public System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        public System.Windows.Forms.CheckBox m_chkPeakValue;
        public System.Windows.Forms.CheckBox m_chkFilledGraph;
        public System.Windows.Forms.Label label22;
        public System.Windows.Forms.ComboBox m_SignalTypeList;
        public System.Windows.Forms.CheckBox m_chkVisibleCurve;
        public System.Windows.Forms.NumericUpDown numericUpDown1;
        public System.Windows.Forms.Label label23;
        public System.Windows.Forms.Label label24;
        public System.Windows.Forms.Label label25;
        public System.Windows.Forms.Button m_bLineColor;
        public System.Windows.Forms.Button m_bFillColor;
        public System.Windows.Forms.TableLayoutPanel m_tableLayoutControlArea;
        public System.Windows.Forms.ToolStripMenuItem menuMRU;
        public System.Windows.Forms.ToolStripSeparator toolStripSeparator15;
        public System.Windows.Forms.ToolStripMenuItem menuSignalFill;
        public System.Windows.Forms.ToolStripMenuItem menuSmoothSignals;
        public System.Windows.Forms.ToolStripMenuItem menuWaterfallPerspective;
        public System.Windows.Forms.ToolStripMenuItem menuWaterfallPerspective1;
        public System.Windows.Forms.ToolStripMenuItem menuWaterfallPerspective2;
        public System.Windows.Forms.ToolStripMenuItem menuWaterfallPerspectiveIso;
        public System.Windows.Forms.ToolStripMenuItem menuWaterfallPerspective2D;
        public System.Windows.Forms.ToolStripMenuItem menuWaterfallFloor;
        public System.Windows.Forms.ToolStripMenuItem menuDebug;
        public System.Windows.Forms.ToolStripMenuItem menuDebug_datatest_1;
        public System.Windows.Forms.ToolStripSeparator toolStripSeparator16;
        public System.Windows.Forms.ToolStripMenuItem menuWaterfallContext_saveBitmap;
        public System.Windows.Forms.ToolStripMenuItem menuWaterfallContextTransparent;
        public System.Windows.Forms.ToolStripMenuItem menuWaterfallContextFloor;
        public System.Windows.Forms.ToolStripSeparator toolStripSeparator17;
        public System.Windows.Forms.ToolStripMenuItem menuWaterfallContextMaxHold;
        public System.Windows.Forms.ToolStripMenuItem menuWaterfallContextRealtime;
        public System.Windows.Forms.ToolStripSeparator toolStripSeparator18;
        public System.Windows.Forms.ToolStripMenuItem menuWaterfallInSA;
        public System.Windows.Forms.ToolStripMenuItem menuPlaceWaterfallAtBottom;
        public System.Windows.Forms.ToolStripMenuItem menuPlaceWaterfallOnTheRight;
        public System.Windows.Forms.ToolStripMenuItem menuPlaceWaterfallNone;
        public System.Windows.Forms.ToolStripMenuItem menuPortInfo;
        public System.Windows.Forms.ToolStripMenuItem menuUserDefinedText;
        public System.Windows.Forms.TableLayoutPanel tableLayoutPanel9;
        public System.Windows.Forms.TableLayoutPanel tableLayoutPanel10;
        public System.Windows.Forms.ToolStripMenuItem menuLimitLines;
        public System.Windows.Forms.ToolStripMenuItem menuLimitLineBuildFromSignal;
        public System.Windows.Forms.ToolStripMenuItem menuLimitLineSaveToFile;
        public System.Windows.Forms.ToolStripMenuItem menuLimitLineReadFromFile;
        public System.Windows.Forms.ToolStripMenuItem menuLimitLineRemove;
        public System.Windows.Forms.ToolStripSeparator toolStripSeparator19;
        public System.Windows.Forms.ToolStripMenuItem menuRemoveMaxLimitLine;
        public System.Windows.Forms.ToolStripMenuItem menuRemoveMinLimitLine;
        public System.Windows.Forms.ToolStripMenuItem menuLimitLineMaxSaveToFile;
        public System.Windows.Forms.ToolStripMenuItem menuLimitLineMinSaveToFile;
        public System.Windows.Forms.ToolStripMenuItem menuLimitLineMaxReadFromFile;
        public System.Windows.Forms.ToolStripMenuItem menuLimitLineMinReadFromFile;
        public System.Windows.Forms.ToolStripMenuItem menuLimitLineMaxBuildFromSignal;
        public System.Windows.Forms.ToolStripMenuItem menuLimitLineMinBuildFromSignal;
        public System.Windows.Forms.ToolStripMenuItem menuShowAxisLabels;
        public System.Windows.Forms.ToolStripMenuItem menuAmplitudeUnits;
        public System.Windows.Forms.ToolStripMenuItem menuItemDBM;
        public System.Windows.Forms.ToolStripMenuItem menuItemDBUV;
        public System.Windows.Forms.ToolStripMenuItem menuItemWatt;
        public System.Windows.Forms.ToolStripMenuItem menuItemSoundAlarmLimitLine;
        public System.Windows.Forms.ToolStripMenuItem menuMarkers;
        public System.Windows.Forms.ToolStripMenuItem menuCalculatorSignalModes;
        public System.Windows.Forms.ToolStripSeparator toolStripSeparator20;
        public System.Windows.Forms.ToolStripMenuItem menuLoadAmplitudeFile;
        public System.Windows.Forms.ToolStripMenuItem menuUseAmplitudeCorrection;
        public System.Windows.Forms.ToolStripMenuItem menuAutoLoadAmplitudeData;
        public System.Windows.Forms.TabPage m_tabPowerChannel;
        public System.Windows.Forms.Panel m_panelPowerChannel;
        public System.Windows.Forms.ToolStripMenuItem menuRemoteMaxHold;
        public System.Windows.Forms.ToolStripMenuItem menuRefreshRemoteMaxHold;
        public System.Windows.Forms.ToolStripSeparator toolStripSeparator21;
        public System.Windows.Forms.ToolStripMenuItem menuRemoteAmplitudeUpdate;
        public System.Windows.Forms.ToolStripMenuItem menuShowGrid;
        public System.Windows.Forms.ToolStripMenuItem menuThickTrace;
        public System.Windows.Forms.ToolStripSeparator toolStripSeparator22;
        public System.Windows.Forms.ToolStripMenuItem menuSaveSNANormalization;
        public System.Windows.Forms.ToolStripMenuItem menuLoadSNANormalization;
        public System.Windows.Forms.ToolStripMenuItem menuSaveS1P;
        public System.Windows.Forms.ToolStripMenuItem menuSaveSNACSV;
        public System.Windows.Forms.GroupBox m_group_CalibrateAmplitudeAnalyzer;
        public System.Windows.Forms.Button m_btnCalibrate6G;
        public System.Windows.Forms.CheckBox chkRFE6GEN_CAL;
        public System.Windows.Forms.Button btnCalibratePurge;
        public System.Windows.Forms.Button m_btnCalibrate1G;
        public System.Windows.Forms.Button m_btnCalibrate3G;
        private System.ComponentModel.IContainer components;
    }
}

