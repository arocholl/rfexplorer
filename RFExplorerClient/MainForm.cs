//============================================================================
//RF Explorer for Windows - A Handheld Spectrum Analyzer for everyone!
//Copyright © 2010-15 Ariel Rocholl, www.rf-explorer.com
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

//#define CALLSTACK_REALTIME
//#define CALLSTACK
//#define SUPPORT_EXPERIMENTAL
//#define DEBUG

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using ZedGraph;
using System.IO.Ports;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Collections;
using Microsoft.Win32;
using System.Diagnostics;
using RFExplorerCommunicator;
using SharpGL;
using SharpGL.Enumerations;
using System.Media;

namespace RFExplorerClient
{
    enum RFExplorerSignalType
    {
        Realtime,
        Average,
        MaxPeak,
        Min,
        MaxHold,
        TOTAL_ITEMS
    };

    enum RFExplorerFileType             //Different file types supported by the application
    {
        SweepDataFile,
        RemoteScreenFile,
        RemoteScreenRFSFile,
        WaterfallScreenshotFile,
        AnalyzerScreenshotFile,
        CumulativeCSVDataFile,
        SimpleCSVDataFile,
        LimitLineDataFile,
        PowerChannelScreenshotFile,
        SNANormalizedDataFile,
        S1PDataFile,
        SNATrackingDataFile,
        None
    };

    public partial class MainForm : Form
    {
        #region Constants
        private const uint MAX_MRU_FILES = 15;

        private const float m_fSizeX = 130;         //Size of the dump screen in pixels (128x64 + 2 border) + 20 height for text header
        private const float m_fSizeY = 66;

        private const string _MarkerEnabled = "MarkerEnabled";
        private const string _Common_Settings = "Common_Settings";
        private const string _MarkerFrequency = "MarkerFrequency";
        private const string _MarkerTrackSignal = "MarkerTrackSignal";
        private const string _ViewMaxHold = "ViewMaxHold";
        private const string _ViewMax = "ViewMax";
        private const string _ViewMin = "ViewMin";
        private const string _ViewRT = "ViewRT";
        private const string _ViewAvg = "ViewAvg";
        private const string _Calculator = "Calculator";
        private const string _BottomAmp = "BottomAmp";
        private const string _TopAmp = "TopAmp";
        private const string _StepFreq = "StepFreq";
        private const string _StartFreq = "StartFreq";
        private const string _RFGenSteps = "RFGenSteps";
        private const string _RFGenStartFreq = "RFGenStartFreq";
        private const string _RFGenStopFreq = "RFGenStopFreq";
        private const string _RFGenPower= "RFGenPower";
        private const string _RFGenStepTime = "RFGenStepTime";
        private const string _Default = "Default";
        private const string _Name = "Name";
        private const string _OnlyIfConnected = "OnlyIfConnected"; //used for resources that should be displayed or enabled only if connected

        private const string _CSV_File_Selector = "RF Explorer CSV files (*.csv)|*.csv|All files (*.*)|*.*";
        private const string _Filename_RFExplorer_Settings = "RFExplorer_Settings.xml";
        private const string _RFE_File_Selector = "RF Explorer Data files (*.rfe)|*.rfe|All files (*.*)|*.*";
        private const string _SNANORM_File_Selector = "RF Explorer Normalized Data files (*.snanorm)|*.snanorm|All files (*.*)|*.*";
        private const string _SNA_File_Selector = "RF Explorer SNA Tracking Data files (*.sna)|*.sna|All files (*.*)|*.*";
        private const string _RFS_File_Selector = "RF Explorer Screen files (*.rfs)|*.rfs|All files (*.*)|*.*";
        private const string _PNG_File_Selector = "Image PNG files (*.png)|*.png|All files (*.*)|*.*";
        private const string _RFL_File_Selector = "RF Explorer Limit Line Data files (*.rfl)|*.rfl|All files (*.*)|*.*";
        private const string _RFA_File_Selector = "RF Explorer Amplitude Data files (*.rfa)|*.rfa|All files (*.*)|*.*";

        private const string _Firmware = "Firmware";
        private const string _Windows = "Windows";
        private const string _Freq = "Freq";
        private const string _OffsetDB = "OffsetDB";
        private const string _Cal = "Cal";
        private const string _Span = "Span";
        private const string _Step = "Step";
        private const string _RBW = "RBW";
        private const string _MainBoard = "MainBoard";
        private const string _ExpansionBoard = "ExpansionBoard";
        private const string _RFEGEN_TRACKING_TITLE="RF Explorer SNA Tracking";

        #endregion

#if SUPPORT_EXPERIMENTAL
        enum eModulation
        {
            MODULATION_OOK,         //0
            MODULATION_PSK,         //1
            MODULATION_NONE = 0xFF  //0xFF
        };
        eModulation m_eModulation;          //Modulation being used

        UInt16 m_nRAWSnifferIndex=0;        //Index pointing to current RAW data value shown
        UInt16 m_nMaxRAWSnifferIndex=0;     //Index pointing to the last RAW data value available
        string[] m_arrRAWSnifferData;       //Array of strings for sniffer data
#endif

        #region Data Members
        RFECommunicator m_objRFEAnalyzer;       //The one and only RFE connected object
        RFECommunicator m_objRFEGenerator;    //Optional RFEGen object for generator

        ToolGroupCOMPort m_groupCOMPortAnalyzer;
        ToolGroupCOMPort m_groupCOMPortGenerator; 

        string m_sLastSettingsLoaded = _Default;
        int m_nDrawingIteration = 0;        //Iteration counter to do regular updates on GUI

        DataSet m_DataSettings;             //Settings data collection
        //A global flag to indicate the properties were correctly read from file, otherwise we should not persist them later
        //as it may indicate some type of crash and damage default settings
        bool m_bPropertiesReadOk = false;
        //Used to alert about firmware version popup only once per session
        bool m_bVersionAlerted = false;     

        public About_RFExplorer m_winAboutModeless; //About box modeless dialog

        string m_sStartFile = "";           //File sent as argument while loading application
        string m_sFilenameRFE = "";         //RFE data file name
        string m_sFilenameRFS = "";         //RFS screen file name
        string m_sReportFilePath = "";      //Path and name of the report log file

        bool m_bFirstTick = true;           //Used to put some text and guarantee action done once after mainform load
        bool m_bFirstText = true;           //First report text printed

        //Graphis objects cached to reduce drawing overhead
        Pen m_PenDarkBlue;                  
        Pen m_PenRed;
        Brush m_BrushDarkBlue;

        //Colors used in configuration and marker panels
        Color m_ColorPanelText = Color.DarkBlue;
        Color m_ColorPanelTextHighlight = Color.DarkRed;
        Color m_ColorPanelBackground = Color.White;
        Panel m_panelSAMarkers;             //panel to display marker values
        Panel m_panelSAConfiguration;       //panel to display RF Explorer configuration settings

        TextObj[] m_arrWiFiBarText;         //Text for the 13 Wifi channels
        TextObj m_StatusGraphText_Analyzer; //Configuration data text received from RF Explorer to show on graph
        TextObj m_StatusGraphText_Tracking; //Configuration data text used in Tracking mode to show on graph
        TextObj m_OverloadText;             //Alert message with ADC overload
        TextObj m_TrackingStatus;           //Indicate current status to the user
        TextObj m_TrackingProgressText;     //Indicate current tracking progress and sweep time
        string m_sUserDefinedText = "";     //Display user defined text

        bool m_bIsWinXP = false;            //True if it is a Windows XP platform, which has some GUI differences with Win7/Vista

        //Line curve item for the analyzer zed graph
        LineItem m_GraphLine_Avg, m_GraphLine_Min, m_GraphLine_Realtime, m_GraphLine_Max, m_GraphLine_MaxHold;
        LineItem m_GraphLine_Tracking_Normal, m_GraphLine_Tracking_Avg;
        //pair list used by the line curve items
        PointPairList m_PointList_Realtime, m_PointList_Max, m_PointList_Min, m_PointList_Avg, m_PointList_MaxHold;
        PointPairList m_PointList_Tracking_Normal, m_PointList_Tracking_Avg;
        //Bar curve used by the wifi analyzer
        BarItem m_MaxBar;                   

        //Support for limit lines
        LineItem m_GraphLimitLineMax, m_GraphLimitLineMin, m_GraphLimitLineOverload;
        LimitLine m_LimitLineMax, m_LimitLineMin, m_LimitLineOverload;

        //Button group list for easy handling
        Button[] m_arrAnalyzerButtonList = new Button[15];

        string m_sAppDataFolder = "";       //Default folder based on %APPDATA% to store log and report files
        string m_sDefaultDataFolder = "";   //Default folder to store CSV and RFE or other data files
        string m_sSettingsFile = "";        //Filename and path of the named settings file

        bool m_bPrintModeEnabled = false;   //Will be true when the painting is being done for printing, mainly used to remove black background
        RFESweepDataCollection m_WaterfallSweepMaxHold = null;
        bool m_bLayoutInitialized = false;  //True after Load Frame is completed

        MarkerCollection m_Markers;         //Marker collection
        ToolStripMenuItem[] m_arrMarkersEnabledMenu;  //Menu items with "Enabled" marker entry
        ToolStripTextBox[] m_arrMarkersFrequencyMenu; //Menu items with frequency MHZ editable, note marker 0 entry is empty (not used)
        RFExplorerSignalType m_eTrackSignalPeak = RFExplorerSignalType.Realtime;

        ZedGraphControl m_graphPowerChannel = null;
        GasGaugeRegion m_PowerChannelRegion_Low = null;
        GasGaugeRegion m_PowerChannelRegion_Medium = null;
        GasGaugeRegion m_PowerChannelRegion_High = null;
        GasGaugeNeedle m_PowerChannelNeedle = null;
        TextObj m_PowerChannelText;

        RFExplorerSignalType m_eWaterfallSignal = RFExplorerSignalType.Realtime;

        Bitmap m_ClipboardBitmap = null; //bitmap object used to copy into clipboard

        //The one and only reusable analyzer graph
        private ZedGraph.ZedGraphControl m_GraphSpectrumAnalyzer;
        //The graph for tracking generator
        private ZedGraph.ZedGraphControl m_GraphTrackingGenerator;

        //A multimedia sound player to notify important events
        System.Media.SoundPlayer m_SoundPlayer = new System.Media.SoundPlayer();

        //variable used as flag to make sure we do not restart a playing sound
        bool m_bSoundPlaying = false;

        //Dialog used to define a custom text
        UserDefinedText m_dlgUserDefinedText = new UserDefinedText();
        #endregion

        #region Static utilities

        /// <summary>
        /// Used to set a visible text box when is disabled, used for all blueish numeric edit boxes
        /// </summary>
        /// <param name="objTextBox"></param>
        public static void ChangeTextBoxColor(TextBox objTextBox)
        {
            if (objTextBox != null && !objTextBox.IsDisposed)
            {
                if (objTextBox.Enabled)
                {
                    objTextBox.BackColor = Color.RoyalBlue;
                    objTextBox.ForeColor = Color.White;
                }
                else
                {
                    objTextBox.BackColor = Color.LightBlue;
                    objTextBox.ForeColor = Color.DarkBlue;
                }
            }
        }
        #endregion

        #region Main Window
        public MainForm(string sFile)
        {
#if CALLSTACK
            Console.WriteLine("CALLSTACK:MainForm");
#endif

            try
            {
                if (!String.IsNullOrEmpty(sFile))
                    m_sStartFile = sFile;

                SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                SetStyle(ControlStyles.UserPaint, true);
                InitializeComponent();

                InitializeRFGenTab();

                m_GraphSpectrumAnalyzer = new ZedGraph.ZedGraphControl();
                m_WaterfallSweepMaxHold = new RFESweepDataCollection(100, false);
                m_Markers = new MarkerCollection();
                m_Markers.Initialize();
                CreateMarkerMenu();

                m_objRFEAnalyzer = new RFECommunicator();
                m_objRFEAnalyzer.PortClosedEvent += new EventHandler(OnAnalyzerPortClosed);
                m_objRFEAnalyzer.PortConnectedEvent += new EventHandler(OnAnalyzerPortConnected);
                m_objRFEAnalyzer.ReportInfoAddedEvent += new EventHandler(OnAnalyzerReportLog);
                m_objRFEAnalyzer.ReceivedConfigurationDataEvent += new EventHandler(OnAnalyzerReceivedConfigData);
                m_objRFEAnalyzer.UpdateDataEvent += new EventHandler(OnAnalyzerUpdateData);
                m_objRFEAnalyzer.UpdateRemoteScreenEvent += new EventHandler(OnAnalyzerUpdateRemoteScreen);
                m_objRFEAnalyzer.ReceivedDeviceModelEvent += new EventHandler(OnAnalyzerReceivedDeviceModel);
                m_objRFEAnalyzer.UpdateDataTrakingNormalizationEvent += new EventHandler(OnAnalyzerUpdateDataTrakingNormalization);
                m_objRFEAnalyzer.UpdateDataTrakingEvent += new EventHandler(OnAnalyzerUpdateTrackingData);
                m_objRFEAnalyzer.ShowDetailedCOMPortInfo = false;

                m_groupCOMPortAnalyzer = new ToolGroupCOMPort();
                m_groupCOMPortAnalyzer.RFExplorer = m_objRFEAnalyzer;
                m_groupCOMPortAnalyzer.PortClosed += new EventHandler(OnAnalyzerButtons_PortClosed);
                m_groupCOMPortAnalyzer.PortConnected += new EventHandler(OnAnalyzerButtons_PortConnected);
                m_groupCOMPortAnalyzer.GroupBoxTitle = "COM Port Spectrum Analyzer";

                m_objRFEGenerator = new RFECommunicator();
                m_objRFEGenerator.PortClosedEvent += new EventHandler(OnGeneratorPortClosed);
                m_objRFEGenerator.PortConnectedEvent += new EventHandler(OnGeneratorPortConnected);
                m_objRFEGenerator.ReportInfoAddedEvent += new EventHandler(OnGeneratorReportLog);
                m_objRFEGenerator.ReceivedConfigurationDataEvent += new EventHandler(OnGeneratorReceivedConfigData);
                //m_objRFEGen.UpdateData += new EventHandler(OnRFE_UpdateData);
                m_objRFEGenerator.UpdateRemoteScreenEvent += new EventHandler(OnAnalyzerUpdateRemoteScreen);
                m_objRFEGenerator.ReceivedDeviceModelEvent += new EventHandler(OnGeneratorReceivedDeviceModel);
                m_objRFEGenerator.ShowDetailedCOMPortInfo = false;

                m_groupCOMPortGenerator = new ToolGroupCOMPort();
                m_groupCOMPortGenerator.RFExplorer = m_objRFEGenerator;
                //m_groupCOMPortGenerator.PortClosed += new EventHandler(OnGeneratorButtons_PortClosed);
                //m_groupCOMPortGenerator.PortConnected += new EventHandler(OnGeneratorButtons_PortConnected);
                m_groupCOMPortGenerator.GroupBoxTitle = "COM Port Signal Generator";

                printMainDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocument_PrintPage);
                printMainDocument.DocumentName = "RF Explorer";
            }
            catch (Exception obEx)
            {
                string sReportFile = Environment.GetEnvironmentVariable("APPDATA") + "\\rfexplorer_crash.log";
                using (StreamWriter sr = new StreamWriter(sReportFile, true))
                {
                    sr.WriteLine(Environment.NewLine + Environment.NewLine +
                        "------------------------------------------------------------" + Environment.NewLine +
                        "Report log date " + DateTime.Now.ToString() + Environment.NewLine);
                    sr.WriteLine("Exception: " + obEx.ToString());
                    sr.Flush();
                }
            }
        }
        
        private void OnUserDefinedText_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == m_dlgUserDefinedText.ShowDialog(this))
            {
                m_sUserDefinedText = m_dlgUserDefinedText.m_comboText.Text;
                UpdateTitleText_Analyzer();
                m_GraphSpectrumAnalyzer.Refresh();
                if (m_dlgUserDefinedText.m_comboText.Items.Count > 0)
                {
                    if (!m_dlgUserDefinedText.m_comboText.Items.Contains(m_sUserDefinedText))
                    {
                        m_dlgUserDefinedText.m_comboText.Items.Add(m_sUserDefinedText);
                    }
                }
                else
                {
                    m_dlgUserDefinedText.m_comboText.Items.Add(m_sUserDefinedText);
                }
            }
        }

        private void printDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            //PaintScreen(e.Graphics, e.MarginBounds, true);
            e.HasMorePages = false;
        }

        private void OnRFConnections_Click(object sender, EventArgs e)
        {
            DisplayGroups();
        }

        private void CreateSettingsSchema()
        {
            try
            {
                DataTable objTableCommon = m_DataSettings.Tables.Add(_Common_Settings);

                objTableCommon.Columns.Add(new DataColumn(_Name, System.Type.GetType("System.String")));
                objTableCommon.Columns.Add(new DataColumn(_StartFreq, System.Type.GetType("System.Double")));
                objTableCommon.Columns.Add(new DataColumn(_StepFreq, System.Type.GetType("System.Double")));
                objTableCommon.Columns.Add(new DataColumn(_TopAmp, System.Type.GetType("System.Double")));
                objTableCommon.Columns.Add(new DataColumn(_BottomAmp, System.Type.GetType("System.Double")));
                objTableCommon.Columns.Add(new DataColumn(_Calculator, System.Type.GetType("System.UInt16")));
                objTableCommon.Columns.Add(new DataColumn(_ViewAvg, System.Type.GetType("System.Boolean")));
                objTableCommon.Columns.Add(new DataColumn(_ViewRT, System.Type.GetType("System.Boolean")));
                objTableCommon.Columns.Add(new DataColumn(_ViewMin, System.Type.GetType("System.Boolean")));
                objTableCommon.Columns.Add(new DataColumn(_ViewMax, System.Type.GetType("System.Boolean")));
                objTableCommon.Columns.Add(new DataColumn(_ViewMaxHold, System.Type.GetType("System.Boolean")));
                for (int nInd = 1; nInd <= MarkerCollection.MAX_MARKERS; nInd++)
                {
                    objTableCommon.Columns.Add(new DataColumn(_MarkerEnabled + nInd.ToString(), System.Type.GetType("System.Boolean")));
                    if (nInd != 1)
                    {
                        objTableCommon.Columns.Add(new DataColumn(_MarkerFrequency + nInd.ToString(), System.Type.GetType("System.Double")));
                    }
                    else
                    {
                        objTableCommon.Columns.Add(new DataColumn(_MarkerTrackSignal, System.Type.GetType("System.UInt16")));
                    }
                }

                DataRow objRow = objTableCommon.NewRow();
                objRow[_Name] = _Default;
                objRow[_StartFreq] = 430.000;
                objRow[_StepFreq] = 0.500;
                objRow[_TopAmp] = 5;
                objRow[_BottomAmp] = -120;
                objRow[_Calculator] = 10;
                objRow[_ViewAvg] = true;
                objRow[_ViewRT] = true;
                objRow[_ViewMin] = false;
                objRow[_ViewMax] = false;
                objRow[_ViewMaxHold] = false;
                for (int nInd = 1; nInd <= MarkerCollection.MAX_MARKERS; nInd++)
                {
                    objRow[_MarkerEnabled + nInd.ToString()] = false;
                    if (nInd != 1)
                    {
                        objRow[_MarkerFrequency + nInd.ToString()] = 1000.0;
                    }
                    else
                    {
                        objRow[_MarkerTrackSignal] = 0;
                    }
                } 
                objTableCommon.Rows.Add(objRow);

                m_DataSettings.WriteXml(m_sSettingsFile, XmlWriteMode.WriteSchema);
            }
            catch (Exception objEx)
            {
                MessageBox.Show(objEx.Message);
            }
        }

        private void GetStringArrayFromStringList(string sListInput, out string[] arrStrings)
        {
            arrStrings = null;
            if (sListInput.Length > 0)
            {
                arrStrings = sListInput.Split(';');
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void menuMRU_DropDownOpening(object sender, EventArgs e)
        {
            int nCounter = 0;
            //foreach (ToolStripMenuItem menuItem in menuMRU.DropDownItems)
            //{
            //    menuItem.Dispose();
            //}
            menuMRU.DropDownItems.Clear();
            string[] arrMRU=null;
            string sUsedMRU = ""; //helper string to limit stored files to MAX_MRU_FILES

            GetStringArrayFromStringList(RFExplorerClient.Properties.Settings.Default.MRUList, out arrMRU);

            foreach (string sFile in arrMRU)
            {
                ToolStripMenuItem menuNew = new ToolStripMenuItem(sFile);
                menuNew.Name = "MRUFile_" + nCounter;
                menuNew.Click += new System.EventHandler(this.OnMRU_Click);
                menuMRU.DropDownItems.Add(menuNew);
                nCounter++;
                //check to limit to MAX_MRU_FILES files
                if (sUsedMRU.Length > 0)
                {
                    sUsedMRU = sUsedMRU + ";" + sFile;
                }
                else
                {
                    sUsedMRU = sFile;
                }
                if (nCounter > MAX_MRU_FILES)
                {
                    //too many files, we limit them to MAX_MRU_FILES
                    RFExplorerClient.Properties.Settings.Default.MRUList = sUsedMRU;
                    break;
                }
            }
        }

        private void OnMRU_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuClicked = (ToolStripMenuItem)sender;
            LoadFileRFE(menuClicked.Text);
        }

        private void OnSmoothSignals_Click(object sender, EventArgs e)
        {
            if (m_objRFEAnalyzer.HoldMode)
                DisplaySpectrumAnalyzerData();
        }

        private void OnSignalFill_Click(object sender, EventArgs e)
        {
            if (m_objRFEAnalyzer.HoldMode)
                DisplaySpectrumAnalyzerData();
        }

        private void OnLoadSettings_Click(object sender, EventArgs e)
        {
            RestoreSettingsXML(menuComboSavedOptions.Text);
        }

        private void OnDebug_CheckedChanged(object sender, EventArgs e)
        {
            m_controlWaterfall.DrawFPS = m_chkDebugTraces.Checked;
            if (m_objRFEAnalyzer != null)
            {
                m_objRFEAnalyzer.EnableDebugTraces = m_chkDebugTraces.Checked;
                m_objRFEAnalyzer.ShowDetailedCOMPortInfo = m_chkDebugTraces.Checked;
            }
        }

        private void menuComboSavedOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnDelSettings.Enabled = menuComboSavedOptions.Text != _Default;
        }

        private void OnSaveSettings_Click(object sender, EventArgs e)
        {
            string sItem = menuComboSavedOptions.Text;
            SaveSettingsXML(sItem);
            PopulateReadedSettings();
            menuComboSavedOptions.SelectedItem = sItem;
        }

        private void OnDeleteSettings_Click(object sender, EventArgs e)
        {
            string sItem = menuComboSavedOptions.Text;
            DeleteSettingsXML(sItem);
            menuComboSavedOptions.Items.Remove(sItem);
            menuComboSavedOptions.SelectedItem = _Default;
        }

        private void DefineGraphColors()
        {
            GraphPane myPane = m_GraphSpectrumAnalyzer.GraphPane;

            m_controlWaterfall.DarkMode = menuDarkMode.Checked;

            if (menuDarkMode.Checked)
            {
                Color DarkColor = Color.Black;
                Color FontColor = Color.White;
                Color BackgroundColor = Color.DarkGray;

                m_ColorPanelText = Color.White;
                m_ColorPanelTextHighlight = Color.LightSalmon;
                m_ColorPanelBackground = Color.Black;

                if (m_bPrintModeEnabled)
                {
                    DarkColor = Color.White;
                    FontColor = Color.Black;
                    BackgroundColor = Color.White;
                }

                this.BackColor = BackgroundColor;
                m_tabSpectrumAnalyzer.BackColor = BackgroundColor;
                m_tabReport.BackColor = BackgroundColor;
                m_tabRemoteScreen.BackColor = BackgroundColor;
                m_tabConfiguration.BackColor = BackgroundColor;
                m_tabWaterfall.BackColor = BackgroundColor;
                if (m_tabRFGen != null)
                {
                    m_tabRFGen.BackColor = BackgroundColor;
                }

                myPane.Fill = new Fill(DarkColor);
                myPane.Chart.Fill = new Fill(DarkColor);
                myPane.Title.FontSpec.FontColor = FontColor;

                m_StatusGraphText_Analyzer.FontSpec.FontColor = Color.LightGray;
                m_StatusGraphText_Analyzer.FontSpec.DropShadowColor = Color.DarkRed;
                m_OverloadText.FontSpec.FontColor = Color.LightSalmon;

                myPane.YAxis.Title.FontSpec.FontColor = FontColor;
                myPane.XAxis.Title.FontSpec.FontColor = FontColor;
                myPane.YAxis.Scale.FontSpec.FontColor = FontColor;
                myPane.XAxis.Scale.FontSpec.FontColor = FontColor;

                myPane.Chart.Border.Color = Color.Gray;
                myPane.Chart.Border.IsAntiAlias = true;

                myPane.Legend.FontSpec.FontColor = FontColor;
                myPane.Legend.Fill.Color = DarkColor;
                myPane.Legend.Fill.SecondaryValueGradientColor = DarkColor;
                myPane.Legend.Fill = new Fill(DarkColor);
            }
            else
            {
                m_ColorPanelText = Color.DarkBlue;
                m_ColorPanelTextHighlight = Color.DarkRed;
                m_ColorPanelBackground = Color.White;

                this.BackColor = Color.LightYellow;
                m_tabSpectrumAnalyzer.BackColor = Color.LightYellow;
                m_tabReport.BackColor = Color.LightYellow;
                m_tabRemoteScreen.BackColor = Color.LightYellow;
                m_tabConfiguration.BackColor = Color.LightYellow;
                m_tabWaterfall.BackColor = Color.LightYellow;
                if (m_tabRFGen!=null)
                {
                    m_tabRFGen.BackColor = this.BackColor;
                }

                myPane.Fill = new Fill(Color.White, Color.LightYellow, 90.0f);
                myPane.Chart.Fill = new Fill(Color.White, Color.LightYellow, 90.0f);

                m_StatusGraphText_Analyzer.FontSpec.FontColor = Color.DarkBlue;
                m_StatusGraphText_Analyzer.FontSpec.DropShadowColor = Color.LightGray;
                m_OverloadText.FontSpec.FontColor = Color.DarkRed;

                myPane.Title.FontSpec.FontColor = Color.Black;

                myPane.YAxis.Title.FontSpec.FontColor = Color.Black;
                myPane.XAxis.Title.FontSpec.FontColor = Color.Black;
                myPane.YAxis.Scale.FontSpec.FontColor = Color.Black;
                myPane.XAxis.Scale.FontSpec.FontColor = Color.Black;

                myPane.Chart.Border.Color = Color.Gray;
                myPane.Chart.Border.IsAntiAlias = true;

                myPane.Legend.FontSpec.FontColor = Color.Black;
                myPane.Legend.Fill.Color = Color.LightYellow;
                myPane.Legend.Fill = new Fill(Color.LightYellow);
            }

            myPane.YAxis.MajorGrid.Color = Color.Gray;
            myPane.YAxis.MinorGrid.Color = Color.Gray;
            myPane.XAxis.MajorGrid.Color = Color.Gray;
            myPane.XAxis.MinorGrid.Color = Color.Gray;

            myPane.YAxis.MajorTic.Color = Color.Gray;
            myPane.YAxis.MinorTic.Color = Color.Gray;
            myPane.XAxis.MajorTic.Color = Color.Gray;
            myPane.XAxis.MinorTic.Color = Color.Gray;

            myPane.YAxis.Title.FontSpec.Size = 13;
            myPane.XAxis.Title.FontSpec.Size = 13;
            myPane.YAxis.Scale.FontSpec.Size = 10;
            myPane.XAxis.Scale.FontSpec.Size = 10;

            // Fill the axis background with a gradient
            myPane.Legend.IsHStack = true;
            myPane.Legend.FontSpec.Size = 12;

            // Enable scrollbars if needed
            m_GraphSpectrumAnalyzer.IsAutoScrollRange = true;

            // Update config and marker panels
            UpdateMarkerControlValues();
            UpdateConfigControlContents();
        }

        private void InitializeSpectrumAnalyzerGraph()
        {
#if CALLSTACK
            Console.WriteLine("CALLSTACK:InitializeSpectrumAnalyzerGraph");
#endif
            m_tabSpectrumAnalyzer.Controls.Add(m_GraphSpectrumAnalyzer);
            m_GraphSpectrumAnalyzer.EditButtons = System.Windows.Forms.MouseButtons.Left;
            m_GraphSpectrumAnalyzer.IsAntiAlias = true;
            m_GraphSpectrumAnalyzer.IsEnableSelection = true;
            m_GraphSpectrumAnalyzer.Location = new System.Drawing.Point(8, 257);
            m_GraphSpectrumAnalyzer.Name = "zedSpectrumAnalyzer";
            m_GraphSpectrumAnalyzer.PanModifierKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)));
            m_GraphSpectrumAnalyzer.ScrollGrace = 0D;
            m_GraphSpectrumAnalyzer.ScrollMaxX = 0D;
            m_GraphSpectrumAnalyzer.ScrollMaxY = 0D;
            m_GraphSpectrumAnalyzer.ScrollMaxY2 = 0D;
            m_GraphSpectrumAnalyzer.ScrollMinX = 0D;
            m_GraphSpectrumAnalyzer.ScrollMinY = 0D;
            m_GraphSpectrumAnalyzer.ScrollMinY2 = 0D;
            m_GraphSpectrumAnalyzer.Size = new System.Drawing.Size(123, 54);
            m_GraphSpectrumAnalyzer.TabIndex = 49;
            m_GraphSpectrumAnalyzer.TabStop = false;
            m_GraphSpectrumAnalyzer.UseExtendedPrintDialog = true;
            m_GraphSpectrumAnalyzer.Visible = false;
            m_GraphSpectrumAnalyzer.ContextMenuBuilder += new ZedGraph.ZedGraphControl.ContextMenuBuilderEventHandler(this.objGraph_ContextMenuBuilder);
            m_GraphSpectrumAnalyzer.ZoomEvent += new ZedGraph.ZedGraphControl.ZoomEventHandler(this.zedSpectrumAnalyzer_ZoomEvent);

            // Get a reference to the GraphPane instance in the ZedGraphControl
            GraphPane myPane = m_GraphSpectrumAnalyzer.GraphPane;

            m_PointList_Realtime = new PointPairList();
            m_PointList_Max = new PointPairList();
            m_PointList_MaxHold = new PointPairList();
            m_PointList_Min = new PointPairList();
            m_PointList_Avg = new PointPairList();
            m_LimitLineMax = new LimitLine();
            m_LimitLineMin = new LimitLine();
            m_LimitLineOverload = new LimitLine();

            m_GraphLimitLineMax = m_GraphSpectrumAnalyzer.GraphPane.AddCurve("Limit Max", m_LimitLineMax, Color.Magenta, SymbolType.Circle);
            m_GraphLimitLineMax.Line.Width = 1;
            m_GraphLimitLineMin = m_GraphSpectrumAnalyzer.GraphPane.AddCurve("Limit Min", m_LimitLineMin, Color.DarkMagenta, SymbolType.Circle);
            m_GraphLimitLineMin.Line.Width = 1;
            m_GraphLimitLineOverload = m_GraphSpectrumAnalyzer.GraphPane.AddCurve("Overload", m_LimitLineOverload, Color.DarkRed, SymbolType.None);
            m_GraphLimitLineOverload.Line.Style=DashStyle.DashDot;
            m_GraphLimitLineOverload.Line.Width = 1;

            m_MaxBar = m_GraphSpectrumAnalyzer.GraphPane.AddHiLowBar("Max", m_PointList_Max, Color.Red);
            m_MaxBar.Bar.Border.Color = Color.DarkRed;
            m_MaxBar.Bar.Border.Width = 3;
            m_GraphLine_Realtime = m_GraphSpectrumAnalyzer.GraphPane.AddCurve("Realtime", m_PointList_Realtime, m_Markers.m_arrCurveColors[(int)RFExplorerSignalType.Realtime], SymbolType.None);
            m_GraphLine_Realtime.Line.Width = 4;
            m_GraphLine_Realtime.Line.SmoothTension = 0.2F;
            m_GraphLine_Min = m_GraphSpectrumAnalyzer.GraphPane.AddCurve("Min", m_PointList_Min, m_Markers.m_arrCurveColors[(int)RFExplorerSignalType.Min], SymbolType.None);
            m_GraphLine_Min.Line.Width = 3;
            m_GraphLine_Min.Line.SmoothTension = 0.3F;
            m_GraphLine_Min.Line.Fill = new Fill(Color.DarkGreen, Color.LightGreen, 90);
            m_GraphLine_Avg = m_GraphSpectrumAnalyzer.GraphPane.AddCurve("Avg", m_PointList_Avg, m_Markers.m_arrCurveColors[(int)RFExplorerSignalType.Average], SymbolType.None);
            m_GraphLine_Avg.Line.Width = 3;
            m_GraphLine_Avg.Line.SmoothTension = 0.3F;
            m_GraphLine_Avg.Line.Fill = new Fill(Color.Brown, Color.Salmon, 90);
            m_GraphLine_Max = m_GraphSpectrumAnalyzer.GraphPane.AddCurve("Max", m_PointList_Max, m_Markers.m_arrCurveColors[(int)RFExplorerSignalType.MaxPeak], SymbolType.None);
            m_GraphLine_Max.Line.Width = 3;
            m_GraphLine_Max.Line.SmoothTension = 0.3F;
            m_GraphLine_Max.Line.Fill = new Fill(Color.Red, Color.Salmon, 90);
            m_GraphLine_MaxHold = m_GraphSpectrumAnalyzer.GraphPane.AddCurve("Max Hold", m_PointList_MaxHold, m_Markers.m_arrCurveColors[(int)RFExplorerSignalType.MaxHold], SymbolType.None);
            m_GraphLine_MaxHold.Line.Width = 6;
            m_GraphLine_MaxHold.Line.SmoothTension = 0.3F;
            m_GraphLine_MaxHold.Line.Fill = new Fill(Color.Salmon, Color.LightSalmon, 90);

            foreach (CurveItem objCurve in m_GraphSpectrumAnalyzer.GraphPane.CurveList)
            {
                objCurve.IsVisible = false;
                objCurve.Label.IsVisible = false;
            }

            //Define button bar
            m_arrAnalyzerButtonList[0] = btnAutoscale;
            m_arrAnalyzerButtonList[1] = btnTop5plus;
            m_arrAnalyzerButtonList[2] = btnTop5minus;
            m_arrAnalyzerButtonList[3] = btnMoveFreqDecLarge;
            m_arrAnalyzerButtonList[4] = btnMoveFreqDecSmall;
            m_arrAnalyzerButtonList[5] = btnSpanInc;
            m_arrAnalyzerButtonList[6] = btnSpanMax;
            m_arrAnalyzerButtonList[7] = btnSpanDefault;
            m_arrAnalyzerButtonList[8] = btnCenterMark;
            m_arrAnalyzerButtonList[9] = btnSpanMin;
            m_arrAnalyzerButtonList[10] = btnSpanDec;
            m_arrAnalyzerButtonList[11] = btnMoveFreqIncLarge;
            m_arrAnalyzerButtonList[12] = btnMoveFreqIncSmall;
            m_arrAnalyzerButtonList[13] = btnBottom5plus;
            m_arrAnalyzerButtonList[14] = btnBottom5minus;

            btnCenterMark.Tag = (string)_OnlyIfConnected;
            btnSpanMin.Tag = (string)_OnlyIfConnected;
            btnSpanDefault.Tag = (string)_OnlyIfConnected;
            btnSpanMax.Tag = (string)_OnlyIfConnected;
            btnSpanDec.Tag = (string)_OnlyIfConnected;
            btnSpanInc.Tag = (string)_OnlyIfConnected;
            btnMoveFreqDecSmall.Tag = (string)_OnlyIfConnected;
            btnMoveFreqIncSmall.Tag = (string)_OnlyIfConnected;
            btnMoveFreqDecLarge.Tag = (string)_OnlyIfConnected;
            btnMoveFreqIncLarge.Tag = (string)_OnlyIfConnected;

            //Make all buttons same size, required for scaled text configurations
            for (int nInd = 0; nInd < m_arrAnalyzerButtonList.Length; nInd++)
            {
                m_arrAnalyzerButtonList[nInd].Width = btnBottom5plus.Width; //btnBottom5plus is currently the one with larger text
                m_arrAnalyzerButtonList[nInd].AutoSize = true;
                m_arrAnalyzerButtonList[nInd].AutoSizeMode = AutoSizeMode.GrowOnly;
            }

            CreateMarkerConfigPanel();

            //MessageBox.Show(btnSend.Width.ToString());

            //Define control bar
            m_tableLayoutControlArea.SuspendLayout();
            m_tableLayoutControlArea.AutoSize = true;
            m_tableLayoutControlArea.ColumnCount = 10;
            m_tableLayoutControlArea.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            m_tableLayoutControlArea.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            m_tableLayoutControlArea.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            m_tableLayoutControlArea.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            m_tableLayoutControlArea.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            m_tableLayoutControlArea.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            m_tableLayoutControlArea.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            m_tableLayoutControlArea.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            m_tableLayoutControlArea.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            m_tableLayoutControlArea.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            m_tableLayoutControlArea.Location = new System.Drawing.Point(6, 3);
            m_tableLayoutControlArea.Name = "m_tableLayoutControlArea";
            m_tableLayoutControlArea.RowCount = 1;
            m_tableLayoutControlArea.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, m_groupControl_FreqSettings.Height));
            m_tableLayoutControlArea.Size = new System.Drawing.Size(10, 122);
            m_tableLayoutControlArea.MaximumSize = new System.Drawing.Size(Width, 122);
            m_tableLayoutControlArea.TabIndex = 61;
            m_tableLayoutControlArea.Visible = false;
            int nSaveBtnWidth = btnAnalyzerSend.Width;
            btnAnalyzerSend.AutoSize = false;
            btnAnalyzerSend.Dock = DockStyle.None;
            btnAnalyzerFreqSettingsReset.Dock = DockStyle.None;
            btnAnalyzerFreqSettingsReset.AutoSize = false;
            m_sAnalyzerCenterFreq.Width = (int)(label_sCenter.Width * 1.4);
            m_sAnalyzerStartFreq.Width = (int)(label_sCenter.Width * 1.4);
            m_sAnalyzerEndFreq.Width = (int)(label_sCenter.Width * 1.4);
            m_sAnalyzerBottomAmplitude.Width = (int)(label_sCenter.Width * 1.4);
            m_sAnalyzerTopAmplitude.Width = (int)(label_sCenter.Width * 1.4);
            m_sAnalyzerFreqSpan.Width = (int)(label_sCenter.Width * 1.4);
            btnAnalyzerSend.Width = nSaveBtnWidth;
            btnAnalyzerFreqSettingsReset.Width = nSaveBtnWidth;

            m_groupControl_DataFeed.Height = m_groupControl_FreqSettings.Height;
            m_groupControl_RemoteScreen.Height = m_groupControl_FreqSettings.Height;
            m_groupControl_Commands.Height = m_groupControl_FreqSettings.Height;
            m_groupControl_RFEGen_CW.Height = m_groupControl_FreqSettings.Height;
            m_groupControl_RFEGen_FrequencySweep.Height = m_groupControl_FreqSettings.Height;
            m_groupCOMPortAnalyzer.Height = m_groupControl_FreqSettings.Height;
            m_groupCOMPortGenerator.Height = m_groupControl_FreqSettings.Height;
            m_groupControl_RFEGen_Tracking.Height = m_groupControl_FreqSettings.Height;
            m_tableLayoutControlArea.Controls.Clear();
            Controls.Remove(m_groupControl_DataFeed);
            Controls.Remove(m_groupControl_FreqSettings);
            Controls.Remove(m_groupControl_RemoteScreen);
            Controls.Remove(m_groupControl_Commands);
            Controls.Remove(m_groupControl_RFEGen_CW);
            Controls.Remove(m_groupControl_RFEGen_FrequencySweep);
            Controls.Remove(m_groupControl_RFEGen_Tracking);
            m_tableLayoutControlArea.Controls.Add(m_groupCOMPortAnalyzer, 0, 0);
            m_tableLayoutControlArea.Controls.Add(m_groupCOMPortGenerator, 1, 0);
            m_tableLayoutControlArea.Controls.Add(m_groupControl_DataFeed, 2, 0);
            m_tableLayoutControlArea.Controls.Add(m_groupControl_FreqSettings, 3, 0);
            m_tableLayoutControlArea.Controls.Add(m_groupControl_RemoteScreen, 4, 0);
            m_tableLayoutControlArea.Controls.Add(m_groupControl_Commands, 5, 0);
            m_tableLayoutControlArea.Controls.Add(m_groupControl_RFEGen_CW, 6, 0);
            m_tableLayoutControlArea.Controls.Add(m_groupControl_RFEGen_FrequencySweep, 7, 0);
            m_tableLayoutControlArea.Controls.Add(m_groupControl_RFEGen_Tracking, 8, 0);
            m_groupControl_FreqSettings.Dock = DockStyle.Fill;
            m_groupControl_RemoteScreen.Dock = DockStyle.Fill;
            //m_tableLayoutControlArea.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            m_tableLayoutControlArea.ResumeLayout();

            // Set the titles and axis labels
            //myPane.Title.FontSpec.Size = 10;
            myPane.XAxis.Title.IsVisible = menuShowAxisLabels.Checked;
            myPane.XAxis.Title.Text = "Frequency (MHZ)";
            myPane.XAxis.Scale.MajorStep = 1.0;
            myPane.XAxis.Scale.MinorStep = 0.2;

            myPane.Margin.Left = 20;
            myPane.Margin.Right = -5;

            // Show the x axis grid
            myPane.XAxis.MajorGrid.IsVisible = true;
            myPane.XAxis.Type = AxisType.Linear;

            myPane.YAxis.Title.IsVisible = menuShowAxisLabels.Checked;
            // turn off the opposite tics so the Y tics don't show up on the Y2 axis
            myPane.YAxis.MajorTic.IsOpposite = false;
            myPane.YAxis.MinorTic.IsOpposite = false;
            myPane.YAxis.MajorGrid.IsVisible = true;
            // Don't display the Y zero line
            myPane.YAxis.MajorGrid.IsZeroLine = false;
            // Align the Y axis labels so they are flush to the axis
            myPane.YAxis.Scale.Align = AlignP.Inside;

            m_GraphSpectrumAnalyzer.IsShowPointValues = true;
            m_GraphSpectrumAnalyzer.PointValueEvent += new ZedGraphControl.PointValueHandler(GraphPointValueHandler);

            myPane.BarSettings.Type = BarType.Overlay;
            myPane.BarSettings.MinBarGap = 0.1f;
            myPane.BarSettings.MinClusterGap = 0.1f;
            myPane.BarSettings.ClusterScaleWidthAuto = false;
            myPane.BarSettings.ClusterScaleWidth = 5.0f;

            m_StatusGraphText_Analyzer = new TextObj("RF Explorer DISCONNECTED", 0.01, 0.02, CoordType.ChartFraction);
            m_StatusGraphText_Analyzer.IsClippedToChartRect = true;
            //m_RFEConfig.ZOrder = 0;
            m_StatusGraphText_Analyzer.FontSpec.FontColor = Color.DarkGray;
            m_StatusGraphText_Analyzer.Location.AlignH = AlignH.Left;
            m_StatusGraphText_Analyzer.Location.AlignV = AlignV.Top;
            m_StatusGraphText_Analyzer.FontSpec.IsBold = false;
            m_StatusGraphText_Analyzer.FontSpec.Size = 10f;
            m_StatusGraphText_Analyzer.FontSpec.Border.IsVisible = false;
            m_StatusGraphText_Analyzer.FontSpec.Fill.IsVisible = false;
            m_StatusGraphText_Analyzer.FontSpec.StringAlignment = StringAlignment.Near;
            m_StatusGraphText_Analyzer.FontSpec.IsDropShadow = true;
            m_StatusGraphText_Analyzer.FontSpec.DropShadowOffset = 0.1f;
            m_StatusGraphText_Analyzer.FontSpec.Family = "Tahoma";
            myPane.GraphObjList.Add(m_StatusGraphText_Analyzer);

            m_OverloadText = new TextObj("RF LEVEL OVERLOAD - COMPRESSION", 0.5, 0.05, CoordType.ChartFraction);
            m_OverloadText.IsClippedToChartRect = true;
            //m_RFEConfig.ZOrder = 0;
            m_OverloadText.FontSpec.FontColor = Color.DarkRed;
            m_OverloadText.Location.AlignH = AlignH.Center;
            m_OverloadText.Location.AlignV = AlignV.Top;
            m_OverloadText.FontSpec.IsBold = true;
            m_OverloadText.FontSpec.Size = 12f;
            m_OverloadText.FontSpec.Border.IsVisible = false;
            m_OverloadText.FontSpec.Fill.IsVisible = false;
            m_OverloadText.FontSpec.StringAlignment = StringAlignment.Center;
            m_OverloadText.FontSpec.IsDropShadow = false;
            m_OverloadText.FontSpec.Family = "Tahoma";
            m_OverloadText.IsVisible = false;
            myPane.GraphObjList.Add(m_OverloadText);

            m_arrWiFiBarText = new TextObj[13];
            m_arrWiFiBarText.Initialize();
            for (int nInd = 0; nInd < m_arrWiFiBarText.Length; nInd++)
            {
                m_arrWiFiBarText[nInd] = new TextObj("", 0, 0, CoordType.AxisXYScale);
                m_arrWiFiBarText[nInd].IsClippedToChartRect = true;
                m_arrWiFiBarText[nInd].Location.AlignH = AlignH.Center;
                m_arrWiFiBarText[nInd].Location.AlignV = AlignV.Bottom;
                m_arrWiFiBarText[nInd].FontSpec.Size = 7;
                m_arrWiFiBarText[nInd].FontSpec.Border.IsVisible = false;
                m_arrWiFiBarText[nInd].FontSpec.FontColor = Color.Red;
                m_arrWiFiBarText[nInd].FontSpec.StringAlignment = StringAlignment.Center;
                m_arrWiFiBarText[nInd].FontSpec.Fill.IsVisible = false;
                myPane.GraphObjList.Add(m_arrWiFiBarText[nInd]);
            }

            m_Markers.ConnectToGraph(myPane);

            DefineGraphColors();
        }

        private void OnAutoLCDOff_Click(object sender, EventArgs e)
        {
            if (menuAutoLCDOff.Checked)
            {
                m_objRFEAnalyzer.SendCommand_ScreenOFF();
                chkDumpScreen.Checked = false;
            }
            else
            {
                m_objRFEAnalyzer.SendCommand_ScreenON();
            }
            UpdateButtonStatus();
        }

        private void OnPrintPreview_Click(object sender, EventArgs e)
        {
            m_bPrintModeEnabled = true;
            try
            {
                if (m_MainTab.SelectedTab == m_tabSpectrumAnalyzer)
                {
                    DefineGraphColors();
                    //zedSpectrumAnalyzer.DoPrintPreview() - not usable, it makes the dialog modeless and cannot set white color
                    printPreviewDialog.Document = m_GraphSpectrumAnalyzer.PrintDocument;
                }
                else
                {
                    printPreviewDialog.Document = printMainDocument;
                }

                if (printPreviewDialog.ShowDialog() == DialogResult.OK)
                {
                    printMainDocument.Print();
                }
            }
            catch (Exception obEx)
            {
                ReportLog(obEx.Message, false);
            }
            m_bPrintModeEnabled = false;
            DefineGraphColors();
        }

        /// <summary>
        /// A threaded version of the copy method to avoid crash with MTA
        /// </summary>         
        private void ClipboardCopyThread()
        {
            if (m_ClipboardBitmap != null)
            {
                Clipboard.SetDataObject(m_ClipboardBitmap, true);
                m_ClipboardBitmap.Dispose();
                m_ClipboardBitmap = null;
            }
        }

        private void OnClipboard_Click(object sender, EventArgs e)
        {
            //create bitmap
            SavePNG("");
            //Use thread to send bitmap to clipboard
            Thread ct = new Thread(new ThreadStart(this.ClipboardCopyThread));
            ct.SetApartmentState(ApartmentState.STA);
            ct.Start();
            ct.Join();
        }

        private void OnPrint_Click(object sender, EventArgs e)
        {
            m_bPrintModeEnabled = true;
            try
            {
                DefineGraphColors();
                m_GraphSpectrumAnalyzer.DoPrint();
            }
            catch (Exception obEx)
            {
                ReportLog(obEx.Message, false);
            }
            m_bPrintModeEnabled = false;
            DefineGraphColors();
        }

        private void OnPageSetup_Click(object sender, EventArgs e)
        {
            m_GraphSpectrumAnalyzer.DoPageSetup();
        }

        private string GetNewFilename(RFExplorerFileType FileType)
        {
            string sDate = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");

            string sResult = "";

            //New unique filename to store data based on date and time
            switch (FileType)
            {
                case RFExplorerFileType.None:
                case RFExplorerFileType.SweepDataFile:
                default:
                    m_sFilenameRFE = "RFExplorer_SweepData_" + sDate + RFECommunicator._RFE_File_Extension;
                    sResult = m_sFilenameRFE;
                    break;
                case RFExplorerFileType.WaterfallScreenshotFile:
                    sResult = "RFExplorer_Waterfall_" + sDate + RFECommunicator._PNG_File_Extension;
                    break;
                case RFExplorerFileType.RemoteScreenFile:
                    sResult = "RFExplorer_RemoteScreen_" + sDate + RFECommunicator._PNG_File_Extension;
                    break;
                case RFExplorerFileType.AnalyzerScreenshotFile:
                    sResult = "RFExplorer_SpectrumAnalyzer_" + sDate + RFECommunicator._PNG_File_Extension;
                    break;
                case RFExplorerFileType.PowerChannelScreenshotFile:
                    sResult = "RFExplorer_PowerChannelMeter_" + sDate + RFECommunicator._PNG_File_Extension;
                    break;
                case RFExplorerFileType.RemoteScreenRFSFile:
                    m_sFilenameRFS = "RFExplorer_RemoteScreen_" + sDate + RFECommunicator._RFS_File_Extension;
                    sResult = m_sFilenameRFS;
                    break;
                case RFExplorerFileType.CumulativeCSVDataFile:
                    sResult = "RFExplorer_MultipleSweepData_" + sDate + RFECommunicator._CSV_File_Extension;
                    break;
                case RFExplorerFileType.SimpleCSVDataFile:
                    sResult = "RFExplorer_SingleSweepData_" + sDate + RFECommunicator._CSV_File_Extension;
                    break;
                case RFExplorerFileType.LimitLineDataFile:
                    sResult = "RFExplorer_LimitLine_" + sDate + RFECommunicator._RFL_File_Extension;
                    break;
                case RFExplorerFileType.SNANormalizedDataFile:
                    sResult = "RFExplorer_SNANorm_" + sDate + RFECommunicator._SNANORM_File_Extension;
                    break;
                case RFExplorerFileType.SNATrackingDataFile:
                    sResult = "RFExplorer_SNA_" + sDate + RFECommunicator._SNA_File_Extension;
                    break;
                case RFExplorerFileType.S1PDataFile:
                    sResult = "RFExplorer_S1Port_" + sDate + RFECommunicator._S1P_File_Extension;
                    break;
            }

            return sResult;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void MainForm_Load(object sender, EventArgs e)
        {
#if CALLSTACK
            Console.WriteLine("CALLSTACK:MainForm_Load");
#endif
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                //Check OpenGL and disable Waterfall if not supported
                if (m_controlWaterfall.OpenGL_Supported == false)
                {
                    ReportLog(m_controlWaterfall.InitializationError, true);
                    ReportLog("ERROR: OpenGL 3D graphics are not supported in this system. Please check with your Graphics Card vendor. " + Environment.NewLine +
                        "Waterfall graphics require OpenGL. Waterfall graphics are disabled", false);
                    m_panelWaterfall.Visible = false;
                    m_panelWaterfall.Enabled = false;
                    System.Windows.Forms.Label warningLabel = new System.Windows.Forms.Label();
                    warningLabel.Text = "Invalid graphics mode - 3D OpenGL not supported - check your video card";
                    warningLabel.Location = m_panelWaterfall.Location;
                    warningLabel.AutoSize = true;
                    m_tabWaterfall.Controls.Add(warningLabel);
                }
                else
                {
                    ReportLog("OpenGL 3D mode: " + m_controlWaterfall.InternalOpenGLControl.RenderContextType +
                                  " - version: " + m_controlWaterfall.InternalOpenGLControl.OpenGL.Version +
                                  " - vendor:" + m_controlWaterfall.InternalOpenGLControl.OpenGL.Vendor +
                                  " - renderer:" + m_controlWaterfall.InternalOpenGLControl.OpenGL.Renderer
                             , false);
                    //ReportLog("OpenGL supported extensions: " + controlWaterfall.InternalOpenGLControl.OpenGL.Extensions, true);
                }

                menuUseAmplitudeCorrection.Enabled = false; //disable this boy till a valid file is loaded

#if SUPPORT_EXPERIMENTAL
            m_arrRAWSnifferData     = new string[m_nTotalBufferSize];
            m_nRAWSnifferIndex      = 0;
            m_nMaxRAWSnifferIndex   = 0;
            numSampleDecoder.Maximum = m_nTotalBufferSize;
            numSampleDecoder.Minimum = 0;
            numSampleDecoder.Value  = 0;
#endif
                toolStripMemory.Maximum = RFESweepDataCollection.MAX_ELEMENTS;
                toolStripMemory.Step = RFESweepDataCollection.MAX_ELEMENTS / 25;

                numericSampleSA.Minimum = 0;
                numericSampleSA.Value = 0;
                UpdateSweepNumericControls();

                numScreenIndex.Minimum = 0;
                numScreenIndex.Maximum = 0;
                numScreenIndex.Value = 0;

                numericIterations.Maximum = 10000;
                numericIterations.Value = 10;

                m_PenDarkBlue = new Pen(Color.DarkBlue, 1);
                m_PenRed = new Pen(Color.Red, 1);
                m_BrushDarkBlue = new SolidBrush(Color.DarkBlue);

                m_bIsWinXP = (Environment.OSVersion.Version.Major <= 5);

                btnLoadSettings.Left = menuComboSavedOptions.Control.Right + 5;
                btnSaveSettings.Left = btnLoadSettings.Right + 5;
                btnDelSettings.Left = btnSaveSettings.Right + 5;

                m_groupCOMPortAnalyzer.GetConnectedPorts();
                m_groupCOMPortGenerator.GetConnectedPorts();
                LoadProperties();

                InitializeSpectrumAnalyzerGraph();
                DefineGraphColors();
                //make sure vertical size is limited as otherwise sometimes the editor may screw up things
                m_groupControl_DataFeed.MaximumSize = new Size(m_groupControl_DataFeed.Width, 116);
                m_groupControl_Commands.MaximumSize = new Size(m_groupControl_Commands.Width, 116);
                m_groupControl_FreqSettings.MaximumSize = new Size(m_groupControl_FreqSettings.Width, 116);
                m_groupControl_RemoteScreen.MaximumSize = new Size(m_groupControl_RemoteScreen.Width, 116);
                m_groupCOMPortGenerator.MaximumSize = new Size(m_groupCOMPortGenerator.Width, 116);
                m_groupControl_RFEGen_CW.MaximumSize = new Size(m_groupControl_RFEGen_CW.Width, 116);
                m_groupControl_RFEGen_FrequencySweep.MaximumSize = new Size(m_groupControl_RFEGen_FrequencySweep.Width, 116);
                m_groupCOMPortAnalyzer.MaximumSize = new Size(m_groupCOMPortAnalyzer.Width, 116);
                m_groupControl_RFEGen_Tracking.MaximumSize = new Size(m_groupControl_RFEGen_Tracking.Width, 116);

                SetupSpectrumAnalyzerAxis();

#if SUPPORT_EXPERIMENTAL
                InitializeRAWDecoderGraph();
#endif
                UpdateButtonStatus();
                m_bLayoutInitialized = true;
                DisplayGroups();

                try
                {
                    m_SoundPlayer.SoundLocation = m_sAppDataFolder + "\\notify.wav";
                    m_SoundPlayer.LoadAsync();
                }
                catch (Exception obSoundException)
                {
                    ReportLog("Error accessing sound alarm file " + m_SoundPlayer.SoundLocation, false);
                    ReportLog(obSoundException.ToString(), true);
                }

                chkHoldMode.Checked = !chkRunMode.Checked;
                chkHoldDecoder.Checked = !chkRunDecoder.Checked;
#if DEBUG
                menuDebug.Visible = true;
                menuDebug.Enabled = true;
#endif

                if (!String.IsNullOrEmpty(m_sStartFile))
                {
                    if (RFECommunicator.IsFileExtensionType(m_sStartFile, RFECommunicator._RFL_File_Extension))
                    {
                        m_sFilenameRFE = m_sStartFile;
                        LoadFileRFE(m_sFilenameRFE);
                    }
                    else if (RFECommunicator.IsFileExtensionType(m_sStartFile, RFECommunicator._RFS_File_Extension))
                    {
                        m_sFilenameRFS = m_sStartFile;
                        LoadFileRFS(m_sFilenameRFS);
                        m_MainTab.SelectedTab = m_tabRemoteScreen;
                    }
                }
                else
                {
                    //try to reconnect analyzer
                    m_groupCOMPortAnalyzer.ConnectPort();
                    if (!m_objRFEAnalyzer.PortConnected)
                    {
                        //To guarantee all markers are properly hidden when first load with no connection or data
                        DisplaySpectrumAnalyzerData();
                    }
                }
                m_timer_receive.Enabled = true;
            }
            catch (Exception obEx)
            {
                ReportLog("Error in MainForm_Load: " + obEx.ToString(), true);
            }

            if ((m_winAboutModeless != null) && (m_winAboutModeless.IsDisposed == false))
            {
                Thread.Sleep(500);
                m_winAboutModeless.Close();
                m_winAboutModeless.Dispose();
                m_winAboutModeless = null;
            }

            Cursor.Current = Cursors.Default;
        }

        private void UpdateButtonStatus()
        {
#if CALLSTACK
            Console.WriteLine("CALLSTACK:UpdateButtonStatus");
#endif
            try
            {
                m_groupCOMPortAnalyzer.UpdateButtonStatus();
                m_groupCOMPortGenerator.UpdateButtonStatus();
                chkDumpScreen.Checked = chkDumpScreen.Checked && !menuAutoLCDOff.Checked && (m_objRFEAnalyzer.PortConnected || m_objRFEGenerator.PortConnected);
                chkDumpScreen.Enabled = !menuAutoLCDOff.Checked && (m_objRFEAnalyzer.PortConnected || m_objRFEGenerator.PortConnected);

                btnSendCmd.Enabled = m_objRFEAnalyzer.PortConnected;

                m_groupControl_FreqSettings.Enabled = m_objRFEAnalyzer.PortConnected;
                groupDemodulator.Enabled = m_objRFEAnalyzer.PortConnected;
                chkHoldMode.Enabled = m_objRFEAnalyzer.PortConnected;
                chkRunMode.Enabled = m_objRFEAnalyzer.PortConnected;
                chkRunDecoder.Enabled = m_objRFEAnalyzer.PortConnected;
                chkHoldDecoder.Enabled = m_objRFEAnalyzer.PortConnected;

                chkCalcRealtime.Checked = menuRealtimeTrace.Checked;
                chkCalcAverage.Checked = menuAveragedTrace.Checked;
                chkCalcMax.Checked = menuMaxTrace.Checked;
                chkCalcMin.Checked = menuMinTrace.Checked;

                btnSaveRemoteBitmap.Enabled = m_objRFEAnalyzer.ScreenData.Count > 1;
                btnSaveRemoteVideo.Enabled = m_objRFEAnalyzer.ScreenData.Count > 1;
                chkDumpColorScreen.Enabled = chkDumpScreen.Enabled;
                chkDumpLCDGrid.Enabled = chkDumpScreen.Enabled;
                chkDumpHeader.Enabled = chkDumpScreen.Enabled;

                ChangeTextBoxColor(m_sAnalyzerBottomAmplitude);
                ChangeTextBoxColor(m_sAnalyzerCenterFreq);
                ChangeTextBoxColor(m_sAnalyzerEndFreq);
                ChangeTextBoxColor(m_sAnalyzerFreqSpan);
                ChangeTextBoxColor(m_sAnalyzerTopAmplitude);
                ChangeTextBoxColor(m_sAnalyzerStartFreq);

                //m_panelSAConfiguration.Enabled = true;
                //calibration is available for all models but 2.4G
                groupCalibration.Enabled = m_objRFEAnalyzer.PortConnected && (m_objRFEAnalyzer.ActiveModel != RFECommunicator.eModel.MODEL_2400);

                if (controlRemoteScreen.RFExplorer == null)
                {
                    controlRemoteScreen.RFExplorer = m_objRFEAnalyzer;
                }

                for (int nInd = 0; nInd < m_arrAnalyzerButtonList.Length; nInd++)
                {
                    if (m_arrAnalyzerButtonList[nInd] != null)
                    {
                        if ((string)m_arrAnalyzerButtonList[nInd].Tag == (string)_OnlyIfConnected)
                        {
                            m_arrAnalyzerButtonList[nInd].Enabled = m_objRFEAnalyzer.PortConnected;
                        }
                        else
                        {
                            m_arrAnalyzerButtonList[nInd].Enabled = m_objRFEAnalyzer.PortConnected || (m_objRFEAnalyzer.SweepData.Count > 0);
                        }
                    }
                }
            }
            catch (Exception obEx)
            {
                ReportLog("ERROR UpdateButtonStatus: " + obEx.ToString(), true);
            }

        }

        private void DefineCommonFiles()
        {
            if (String.IsNullOrEmpty(m_sAppDataFolder))
            {
                //Configuring and loading default folders
                m_sAppDataFolder = Environment.GetEnvironmentVariable("APPDATA") + "\\RFExplorer";
                m_sAppDataFolder = m_sAppDataFolder.Replace("\\\\", "\\");
                if (String.IsNullOrEmpty(m_sDefaultDataFolder))
                {
                    m_sDefaultDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\RFExplorer";
                    m_sDefaultDataFolder = m_sDefaultDataFolder.Replace("\\\\", "\\");
                    edDefaultFilePath.Text = m_sDefaultDataFolder;
                }

                if (Directory.Exists(m_sAppDataFolder) == false)
                {
                    //Create specific RF Explorer folders if they don't exist, alert with a message box if that fails
                    try
                    {
                        Directory.CreateDirectory(m_sAppDataFolder);
                        Directory.CreateDirectory(m_sDefaultDataFolder);
                    }
                    catch (Exception obEx)
                    {
                        MessageBox.Show(obEx.Message);
                    }
                }
            }
        }

        private void LoadProperties()
        {
#if CALLSTACK
            Console.WriteLine("CALLSTACK:LoadProperties");
#endif
            try
            {
                //Load standard WinForm .NET properties
                m_groupCOMPortAnalyzer.DefaultCOMSpeed = RFExplorerClient.Properties.Settings.Default.COMSpeed;
                m_groupCOMPortAnalyzer.DefaultCOMPort = RFExplorerClient.Properties.Settings.Default.COMPort;
                m_groupCOMPortGenerator.DefaultCOMSpeed = RFExplorerClient.Properties.Settings.Default.COMSpeedRFGen;
                m_groupCOMPortGenerator.DefaultCOMPort = RFExplorerClient.Properties.Settings.Default.COMPortRFGen;
                
                menuSaveOnClose.Checked = RFExplorerClient.Properties.Settings.Default.SaveOnClose;
                numericZoom.Value = RFExplorerClient.Properties.Settings.Default.ScreenZoom;
                menuShowPeak.Checked = RFExplorerClient.Properties.Settings.Default.ViewPeaks;
                menuShowControlArea.Checked = RFExplorerClient.Properties.Settings.Default.ShowControlArea;
                menuDarkMode.Checked = RFExplorerClient.Properties.Settings.Default.DarkMode;
                menuAutoLCDOff.Checked = RFExplorerClient.Properties.Settings.Default.AutoLCDOff;
                menuContinuousLog.Checked = RFExplorerClient.Properties.Settings.Default.ContinuousLog;
                string sTemp = RFExplorerClient.Properties.Settings.Default.DefaultDataFolder;
                comboCSVFieldSeparator.SelectedIndex = (int)RFExplorerClient.Properties.Settings.Default.CSVDelimiter;
                menuRFConnections.Checked = RFExplorerClient.Properties.Settings.Default.ShowRFConnections;
                menuSmoothSignals.Checked = RFExplorerClient.Properties.Settings.Default.SignalSmooth;
                menuThickTrace.Checked = RFExplorerClient.Properties.Settings.Default.ThickTrace;
                menuShowGrid.Checked = RFExplorerClient.Properties.Settings.Default.ShowGrid;
                menuSignalFill.Checked = RFExplorerClient.Properties.Settings.Default.SignalFill;
                m_eWaterfallSignal = (RFExplorerSignalType)RFExplorerClient.Properties.Settings.Default.WaterfallSignalType;
                menuShowAxisLabels.Checked = RFExplorerClient.Properties.Settings.Default.ShowAxisTitle;
                menuItemSoundAlarmLimitLine.Checked = RFExplorerClient.Properties.Settings.Default.LimitLinesSoundAlarm;
                menuAutoLoadAmplitudeData.Checked = RFExplorerClient.Properties.Settings.Default.AutoLoadCorrectionModel;
                menuRemoteAmplitudeUpdate.Checked = RFExplorerClient.Properties.Settings.Default.AutoAmplitudeRemoteUpdate;
                menuRemoteMaxHold.Checked = RFExplorerClient.Properties.Settings.Default.AutoMaxHold;
                if (m_objRFEAnalyzer != null)
                {
                    m_objRFEAnalyzer.UseMaxHold = menuRemoteMaxHold.Checked;
                }

                m_controlWaterfall.DrawFloor = RFExplorerClient.Properties.Settings.Default.WaterfallFloor;
                m_controlWaterfall.Transparent = RFExplorerClient.Properties.Settings.Default.TransparentWaterfall;
                m_controlWaterfall.PerspectiveView = (RFEWaterfallGL.SharpGLForm.WaterfallPerspectives)RFExplorerClient.Properties.Settings.Default.WaterfallView;
                UpdateAllWaterfallMenuItems();

                menuPlaceWaterfallAtBottom.Checked = RFExplorerClient.Properties.Settings.Default.WaterfallBottom;
                menuPlaceWaterfallOnTheRight.Checked = RFExplorerClient.Properties.Settings.Default.WaterfallRight;
                menuPlaceWaterfallNone.Checked = RFExplorerClient.Properties.Settings.Default.WaterfallNoSA;
                m_nRFGENIterationAverage.Value = RFExplorerClient.Properties.Settings.Default.TrackingAverage;


                DefineCommonFiles();

                m_sSettingsFile = m_sAppDataFolder + "\\" + _Filename_RFExplorer_Settings;
                try
                {
                    //Check if old settings file exists, and not the new one, thus move it to reuse it. This "old file" was
                    //created in v1.09.04 and has the problem of some users with limited access rights cannot create it.
                    string sOldSettingsFile = Assembly.GetExecutingAssembly().Location + ".Settings.xml";
                    if (File.Exists(sOldSettingsFile) && !File.Exists(m_sSettingsFile))
                    {
                        //We do not really move it as there may be restricted permissions and want to avoid any problem
                        File.Copy(sOldSettingsFile, m_sSettingsFile);
                        MessageBox.Show("NOTE: Old named-settings file has been migrated to the new location " + m_sSettingsFile);
                        //we try to delete it now, may fail but worth trying. We already copied it, anyway
                        File.Delete(sOldSettingsFile);
                    }
                }
                catch { };
                //open new settings file
                m_DataSettings = new DataSet("RF_Explorer_Settings");
                if (!File.Exists(m_sSettingsFile))
                {
                    CreateSettingsSchema();
                }

                //Load custom name saved properties
                if (m_DataSettings.Tables.Count == 0)
                {
                    //do not load it twice or records will be repeated
                    m_DataSettings.ReadXml(m_sSettingsFile);
                }
                PopulateReadedSettings();
                RestoreSettingsXML(_Default);

                if (!String.IsNullOrEmpty(sTemp))
                {
                    m_sDefaultDataFolder = sTemp;
                }
                edDefaultFilePath.Text = m_sDefaultDataFolder;

                if (RFExplorerClient.Properties.Settings.Default.CustomCommandList.Length > 0)
                {
                    string[] arrCustomCommands = null;
                    GetStringArrayFromStringList(RFExplorerClient.Properties.Settings.Default.CustomCommandList, out arrCustomCommands);
                    foreach (string sCmd in arrCustomCommands)
                    {
                        comboCustomCommand.Items.Add(sCmd);
                    }
                }

                if (RFExplorerClient.Properties.Settings.Default.UserDefinedTitleList.Length > 0)
                {
                    string[] arrStrings = null;
                    GetStringArrayFromStringList(RFExplorerClient.Properties.Settings.Default.UserDefinedTitleList, out arrStrings);
                    foreach (string sText in arrStrings)
                    {
                        if (sText.Length > 0)
                        {
                            m_dlgUserDefinedText.m_comboText.Items.Add(sText);
                        }
                    }
                }

                menuItemDBM.Checked = true;

                m_bPropertiesReadOk = true;
            }
            catch (Exception obEx)
            {
                MessageBox.Show(obEx.Message);
            }
        }

        private void PopulateReadedSettings()
        {
            menuComboSavedOptions.Items.Clear();
            foreach (DataRow objRow in m_DataSettings.Tables[_Common_Settings].Rows)
            {
                menuComboSavedOptions.Items.Add((string)objRow[_Name]);
            }
            menuComboSavedOptions.SelectedItem = _Default;
        }

        private void SaveProperties()
        {
            if (!m_bPropertiesReadOk)
            {
                ReportLog("Application settings were not persisted due to an earlier error.", false);
                return;
            }

            try
            {
                //No need to save CustomCommands here, it is already saved in send button.
                //RFExplorerClient.Properties.Settings.Default.CustomCommands

                if (m_groupCOMPortAnalyzer.IsCOMPortSelected)
                {
                    RFExplorerClient.Properties.Settings.Default.COMPort = m_groupCOMPortAnalyzer.COMPortSelected;
                }
                if (m_groupCOMPortAnalyzer.IsCOMSpeedSelected)
                {
                    RFExplorerClient.Properties.Settings.Default.COMSpeed = m_groupCOMPortAnalyzer.COMSpeedSelected;
                }
                if (m_groupCOMPortGenerator.IsCOMPortSelected)
                {
                    RFExplorerClient.Properties.Settings.Default.COMPortRFGen = m_groupCOMPortGenerator.COMPortSelected;
                }
                if (m_groupCOMPortGenerator.IsCOMSpeedSelected)
                {
                    RFExplorerClient.Properties.Settings.Default.COMSpeedRFGen = m_groupCOMPortGenerator.COMSpeedSelected;
                }

                RFExplorerClient.Properties.Settings.Default.SaveOnClose = menuSaveOnClose.Checked;
                RFExplorerClient.Properties.Settings.Default.ScreenZoom = (int)numericZoom.Value;
                RFExplorerClient.Properties.Settings.Default.ViewPeaks = menuShowPeak.Checked;
                RFExplorerClient.Properties.Settings.Default.ShowControlArea = menuShowControlArea.Checked;
                RFExplorerClient.Properties.Settings.Default.DarkMode = menuDarkMode.Checked;
                RFExplorerClient.Properties.Settings.Default.AutoLCDOff = menuAutoLCDOff.Checked;
                RFExplorerClient.Properties.Settings.Default.DefaultDataFolder = m_sDefaultDataFolder;
                RFExplorerClient.Properties.Settings.Default.CSVDelimiter = (uint)comboCSVFieldSeparator.SelectedIndex;
                RFExplorerClient.Properties.Settings.Default.ContinuousLog = menuContinuousLog.Checked;
                RFExplorerClient.Properties.Settings.Default.ShowRFConnections = menuRFConnections.Checked;
                RFExplorerClient.Properties.Settings.Default.SignalFill = menuSignalFill.Checked;
                RFExplorerClient.Properties.Settings.Default.SignalSmooth = menuSmoothSignals.Checked;
                RFExplorerClient.Properties.Settings.Default.ThickTrace = menuThickTrace.Checked;
                RFExplorerClient.Properties.Settings.Default.ShowGrid = menuShowGrid.Checked;
                RFExplorerClient.Properties.Settings.Default.WaterfallView = (uint)m_controlWaterfall.PerspectiveView;
                RFExplorerClient.Properties.Settings.Default.TransparentWaterfall = m_controlWaterfall.Transparent;
                RFExplorerClient.Properties.Settings.Default.WaterfallFloor = m_controlWaterfall.DrawFloor;
                RFExplorerClient.Properties.Settings.Default.WaterfallSignalType = (int)m_eWaterfallSignal;
                RFExplorerClient.Properties.Settings.Default.ShowAxisTitle = menuShowAxisLabels.Checked;
                RFExplorerClient.Properties.Settings.Default.LimitLinesSoundAlarm = menuItemSoundAlarmLimitLine.Checked;
                RFExplorerClient.Properties.Settings.Default.AutoLoadCorrectionModel = menuAutoLoadAmplitudeData.Checked;
                RFExplorerClient.Properties.Settings.Default.AutoMaxHold = menuRemoteMaxHold.Checked;
                RFExplorerClient.Properties.Settings.Default.AutoAmplitudeRemoteUpdate = menuRemoteAmplitudeUpdate.Checked;

                RFExplorerClient.Properties.Settings.Default.WaterfallBottom = menuPlaceWaterfallAtBottom.Checked;
                RFExplorerClient.Properties.Settings.Default.WaterfallRight = menuPlaceWaterfallOnTheRight.Checked;
                RFExplorerClient.Properties.Settings.Default.WaterfallNoSA = menuPlaceWaterfallNone.Checked;

                RFExplorerClient.Properties.Settings.Default.TrackingAverage = m_nRFGENIterationAverage.Value;

                RFExplorerClient.Properties.Settings.Default.UserDefinedTitleList = "";
                foreach (string sText in m_dlgUserDefinedText.m_comboText.Items)
                {
                    RFExplorerClient.Properties.Settings.Default.UserDefinedTitleList += ";" + sText.Replace(';', '-');
                }

                RFExplorerClient.Properties.Settings.Default.Save();

                SaveSettingsXML(_Default);
            }
            catch (Exception obEx)
            {
                ReportLog("ERROR SaveProperties:" + obEx.Message, true);
            }
        }

        private void DeleteSettingsXML(string sSettingsName)
        {
            try
            {
                DataRow[] objRowCol = m_DataSettings.Tables[_Common_Settings].Select("Name='" + sSettingsName + "'");
                if (objRowCol.Length > 0)
                {
                    m_DataSettings.Tables[_Common_Settings].Rows.Remove(objRowCol[0]);
                    m_DataSettings.WriteXml(m_sSettingsFile, XmlWriteMode.WriteSchema);
                }
            }
            catch (Exception obEx)
            {
                ReportLog("ERROR DeleteSettingsXML:" + obEx.Message, true);
            }
        }

        private void RestoreSettingsXML(string sSettingsName)
        {
            try
            {
                m_Markers.HideAllMarkers();                                

                DataRow[] objRowCol = m_DataSettings.Tables[_Common_Settings].Select("Name='" + sSettingsName + "'");
                if (objRowCol.Length > 0)
                {
                    DataRow objRowDefault = objRowCol[0];
                    double fStartFrequencyMHZ = (double)objRowDefault[_StartFreq];
                    double fStepFrequencyMHZ = (double)objRowDefault[_StepFreq];
                    double fAmplitudeTop = (double)objRowDefault[_TopAmp];
                    double fAmplitudeBottom = (double)objRowDefault[_BottomAmp];
                    numericIterations.Value = (UInt16)objRowDefault[_Calculator];
                    menuAveragedTrace.Checked = (bool)objRowDefault[_ViewAvg];
                    menuRealtimeTrace.Checked = (bool)objRowDefault[_ViewRT];
                    menuMinTrace.Checked = (bool)objRowDefault[_ViewMin];
                    menuMaxTrace.Checked = (bool)objRowDefault[_ViewMax];

                    if (m_DataSettings.Tables[_Common_Settings].Columns[_ViewMaxHold] == null)
                    {
                        //Introduced in v1.11.0.1307, may not exist before this date
                        m_DataSettings.Tables[_Common_Settings].Columns.Add(new DataColumn(_ViewMaxHold, System.Type.GetType("System.Boolean")));
                    }

                    try
                    {
                        if ((objRowDefault[_ViewMaxHold] != null) && (!String.IsNullOrEmpty(objRowDefault[_ViewMaxHold].ToString())))
                        {
                            menuMaxHoldTrace.Checked = (bool)objRowDefault[_ViewMaxHold];
                        }
                        else
                        {
                            objRowDefault[_ViewMaxHold] = menuMaxHoldTrace.Checked;
                        }
                    }
                    catch {}

                    if (m_DataSettings.Tables[_Common_Settings].Columns[_MarkerTrackSignal] == null)
                    {
                        //Introduced in v1.11.0.1402, may not exist before this date
                        for (int nInd = 1; nInd <= MarkerCollection.MAX_MARKERS; nInd++)
                        {
                            m_DataSettings.Tables[_Common_Settings].Columns.Add(new DataColumn(_MarkerEnabled + nInd.ToString(), System.Type.GetType("System.Boolean")));
                            if (nInd != 1)
                            {
                                m_DataSettings.Tables[_Common_Settings].Columns.Add(new DataColumn(_MarkerFrequency + nInd.ToString(), System.Type.GetType("System.Double")));
                            }
                            else
                            {
                                m_DataSettings.Tables[_Common_Settings].Columns.Add(new DataColumn(_MarkerTrackSignal, System.Type.GetType("System.UInt16")));
                            }
                        }
                    }

                    try
                    {
                        if ((objRowDefault[_MarkerTrackSignal] != null) && (!String.IsNullOrEmpty(objRowDefault[_MarkerTrackSignal].ToString())))
                        {
                            for (int nInd = 1; nInd <= MarkerCollection.MAX_MARKERS; nInd++)
                            {
                                if ((bool)objRowDefault[_MarkerEnabled + nInd.ToString()])
                                {
                                    m_Markers.EnableMarker(nInd - 1);
                                }
                                if (nInd != 1)
                                {
                                    m_Markers.SetMarkerFrequency(nInd - 1, (double)objRowDefault[_MarkerFrequency + nInd.ToString()]);
                                }
                                else
                                {
                                    m_eTrackSignalPeak = (RFExplorerSignalType)(UInt16)objRowDefault[_MarkerTrackSignal];
                                }
                            }
                        }
                        else
                        {
                            for (int nInd = 1; nInd <= MarkerCollection.MAX_MARKERS; nInd++)
                            {
                                objRowDefault[_MarkerEnabled + nInd.ToString()] = m_Markers.IsMarkerEnabled(nInd - 1);
                                if (nInd != 1)
                                {
                                    objRowDefault[_MarkerFrequency + nInd.ToString()] = m_Markers.GetMarkerFrequency(nInd - 1);
                                }
                                else
                                {
                                    objRowDefault[_MarkerTrackSignal] = 0;
                                }
                            }
                        }
                    }
                    catch {};

                    if (m_DataSettings.Tables[_Common_Settings].Columns[_RFGenStartFreq] == null)
                    {
                        //Introduced in v1.12.1409, may not exist in all files
                        m_DataSettings.Tables[_Common_Settings].Columns.Add(new DataColumn(_RFGenStartFreq, System.Type.GetType("System.Double")));
                        m_DataSettings.Tables[_Common_Settings].Columns.Add(new DataColumn(_RFGenStopFreq, System.Type.GetType("System.Double")));
                        m_DataSettings.Tables[_Common_Settings].Columns.Add(new DataColumn(_RFGenSteps, System.Type.GetType("System.UInt16")));
                        m_DataSettings.Tables[_Common_Settings].Columns.Add(new DataColumn(_RFGenPower, System.Type.GetType("System.UInt16")));
                        m_DataSettings.Tables[_Common_Settings].Columns.Add(new DataColumn(_RFGenStepTime, System.Type.GetType("System.UInt16")));
                    }

                    try
                    {
                        if ((objRowDefault[_RFGenStartFreq] != null) && (!String.IsNullOrEmpty(objRowDefault[_RFGenStartFreq].ToString())))
                        {
                            double fRFGenStartFrequencyMHZ = (double)objRowDefault[_RFGenStartFreq];
                            m_sRFGenFreqSweepStart.Text = fRFGenStartFrequencyMHZ.ToString("f3");
                            double fRFGenStopFrequencyMHZ = (double)objRowDefault[_RFGenStopFreq];
                            m_sRFGenFreqSweepStop.Text = fRFGenStopFrequencyMHZ.ToString("f3");
                            UInt16 nRFGenSteps = (UInt16)objRowDefault[_RFGenSteps];
                            m_sRFGenFreqSweepSteps.Text = nRFGenSteps.ToString();
                            UInt16 nRFGenPower = (UInt16)objRowDefault[_RFGenPower];
                            UInt16 nRFGenStepTime = (UInt16)objRowDefault[_RFGenStepTime];
                        }
                        else
                        {
                            objRowDefault[_RFGenStartFreq] = Convert.ToDouble(m_sRFGenFreqSweepStart.Text);
                            objRowDefault[_RFGenStopFreq] = Convert.ToDouble(m_sRFGenFreqSweepStop.Text);
                            objRowDefault[_RFGenSteps] = Convert.ToDouble(m_sRFGenFreqSweepSteps.Text);
                            objRowDefault[_RFGenPower] = 0;
                            objRowDefault[_RFGenStepTime] = 0;
                        }
                    }
                    catch {};


                    if (m_objRFEAnalyzer.PortConnected == false)
                    {
                        //If device is disconnected, we just need to update visible parts of screen as otherwise it won't change
                        m_objRFEAnalyzer.AmplitudeTopDBM = fAmplitudeTop;
                        m_objRFEAnalyzer.AmplitudeBottomDBM = fAmplitudeBottom;

                        //Check to reinitiate buffer here, otherwise after changing it the receive function will not know the data was changed
                        if ((Math.Abs(m_objRFEAnalyzer.StartFrequencyMHZ - fStartFrequencyMHZ) >= 0.001) || (Math.Abs(m_objRFEAnalyzer.StepFrequencyMHZ - fStepFrequencyMHZ) >= 0.001))
                        {
                            m_objRFEAnalyzer.StartFrequencyMHZ = fStartFrequencyMHZ;
                            m_objRFEAnalyzer.StepFrequencyMHZ = fStepFrequencyMHZ;
                            CleanSweepData();
                        }
                        SetupSpectrumAnalyzerAxis(); //will update everything including the edit boxes
                    }
                    else
                    {
                        //if device is connected, we do not need to change anything: just ask the device to reconfigure and the new configuration will come back
                        SendNewConfig(fStartFrequencyMHZ, fStartFrequencyMHZ + fStepFrequencyMHZ * m_objRFEAnalyzer.FreqSpectrumSteps, fAmplitudeTop, fAmplitudeBottom);
                    }
                    m_sLastSettingsLoaded = sSettingsName;
                    UpdateTitleText_Analyzer();
                    UpdateButtonStatus();
                    UpdateMenuFromMarkerCollection();
                }
            }
            catch (Exception obEx)
            {
                ReportLog("ERROR RestoreSettingsXML:" + obEx.ToString(), true);
            }
        }

        private void SaveSettingsXML(string sSettingsName)
        {
            try
            {
                DataRow[] objRowCol = m_DataSettings.Tables[_Common_Settings].Select("Name='" + sSettingsName + "'");
                DataRow objRow = null;

                if (objRowCol.Length == 0)
                {
                    objRow = m_DataSettings.Tables[_Common_Settings].NewRow();
                    objRow[_Name] = sSettingsName;
                    m_DataSettings.Tables[_Common_Settings].Rows.Add(objRow);
                }
                else
                {
                    objRow = objRowCol[0];
                }

                objRow[_StartFreq] = m_objRFEAnalyzer.StartFrequencyMHZ;
                objRow[_StepFreq] = m_objRFEAnalyzer.StepFrequencyMHZ;
                objRow[_TopAmp] = m_objRFEAnalyzer.AmplitudeTopDBM;
                objRow[_BottomAmp] = m_objRFEAnalyzer.AmplitudeBottomDBM;
                objRow[_Calculator] = (int)numericIterations.Value;
                objRow[_ViewAvg] = menuAveragedTrace.Checked;
                objRow[_ViewRT] = menuRealtimeTrace.Checked;
                objRow[_ViewMin] = menuMinTrace.Checked;
                objRow[_ViewMax] = menuMaxTrace.Checked;
                try
                {
                    //Introduced in Jun 2013, may not exist before this date. The column should have been created already in ReadXML function...
                    objRow[_ViewMaxHold] = menuMaxHoldTrace.Checked;
                }
                catch { };

                try
                {
                    //Introduced in Feb 2014, may not exist before this date. The column should have been created already in ReadXML function...
                    for (int nInd = 1; nInd <= MarkerCollection.MAX_MARKERS; nInd++)
                    {
                        if (nInd != 1)
                        {
                            objRow[_MarkerFrequency + nInd.ToString()] = m_Markers.GetMarkerFrequency(nInd - 1);
                        }
                        else
                        {
                            objRow[_MarkerTrackSignal] = (UInt16)m_eTrackSignalPeak;
                        }
                        objRow[_MarkerEnabled + nInd.ToString()] = m_arrMarkersEnabledMenu[nInd - 1].Checked;
                    }
                }
                catch { };

                try
                {
                    //Introduced in Sep 2014
                    objRow[_RFGenStartFreq] = Convert.ToDouble(m_sRFGenFreqSweepStart.Text);
                    objRow[_RFGenStopFreq] = Convert.ToDouble(m_sRFGenFreqSweepStop.Text);
                    objRow[_RFGenSteps] = Convert.ToDouble(m_sRFGenFreqSweepSteps.Text);
                    objRow[_RFGenPower] = 0;
                    objRow[_RFGenStepTime] = 0;
                }
                catch { };

                m_DataSettings.WriteXml(m_sSettingsFile, XmlWriteMode.WriteSchema);

                if (!((sSettingsName == _Default) && (m_sLastSettingsLoaded != _Default)))
                {
                    //Only update screen text if
                    m_sLastSettingsLoaded = sSettingsName;
                    UpdateTitleText_Analyzer();
                }

                if (sSettingsName == _Default)
                {
                    //If we are saving the default value, that is because we are doing it automatically (e.g. when closing the port)
                    //Therefore select it as the default on screen too
                    menuComboSavedOptions.SelectedItem = _Default;
                }
            }
            catch (Exception obEx)
            {
                ReportLog("ERROR SaveSettingsXML:" + obEx.Message, true);
            }
        }

        private void DisplayRequiredFirmware()
        {
            if (m_objRFEAnalyzer.RFExplorerFirmwareDetected != m_objRFEAnalyzer.FirmwareCertified)
            {
                UInt16 nMayorVerFound = Convert.ToUInt16(m_objRFEAnalyzer.RFExplorerFirmwareDetected.Substring(0, 2));
                UInt16 nMinorVerFound = Convert.ToUInt16(m_objRFEAnalyzer.RFExplorerFirmwareDetected.Substring(3, 2));
                UInt32 nVersionFound = (UInt32)(nMayorVerFound * 100 + nMinorVerFound);
                UInt16 nMayorVerTested = Convert.ToUInt16(m_objRFEAnalyzer.FirmwareCertified.Substring(0, 2));
                UInt16 nMinorVerTested = Convert.ToUInt16(m_objRFEAnalyzer.FirmwareCertified.Substring(3, 2));
                UInt32 nVersionTested = (UInt32)(nMayorVerTested * 100 + nMinorVerTested);

                if (nVersionFound > nVersionTested)
                {
                    ReportLog("\r\nWARNING: Firmware version connected v" + m_objRFEAnalyzer.RFExplorerFirmwareDetected + " is newer than the one certified v" +
                                m_objRFEAnalyzer.FirmwareCertified + " for this version of RF Explorer for Windows.\r\n" +
                                  "         However, it may be compatible but you should check www.rf-explorer.com website\r\n" +
                                  "         to double check if there is a newer version available.\r\n", false);
                }
                else
                {
                    string sText = "RF Explorer device has an older firmware version " + m_objRFEAnalyzer.RFExplorerFirmwareDetected +
                        "\r\nPlease upgrade it to required version " + m_objRFEAnalyzer.FirmwareCertified +
                        "\r\nVisit www.rf-explorer/download to get required firmware.";
                    if (!m_bVersionAlerted)
                    {
                        m_bVersionAlerted = true;
                        MessageBox.Show(sText, "Firmware Warning");
                    }
                    ReportLog(sText, false);
                }
            }
        }

        int m_nTimerCounter = 0;
        private void timer_receive_Tick(object sender, EventArgs e)
        {
            m_nTimerCounter++; //just used to count certain events, so we can do things less frequently when required

            try
            {
                bool bDraw = false;
                string sOut;

                if (m_objRFEAnalyzer.PortConnected)
                {
                    bDraw = m_objRFEAnalyzer.ProcessReceivedString(true, out sOut);
                }

                if (m_objRFEGenerator.PortConnected)
                {
                    m_objRFEGenerator.ProcessReceivedString(true, out sOut);
                    if ((m_nTimerCounter%5)==0)
                    {
                        //every 500ms
                        UpdateButtonStatus_RFGen();
                    }
                }

                if (bDraw)
                {
#if CALLSTACK_REALTIME
                    Console.WriteLine("CALLSTACK:timer_receive_Tick");
#endif
                    if (m_objRFEAnalyzer.Mode == RFECommunicator.eMode.MODE_TRACKING)
                    {
                        if (m_MainTab.SelectedTab==m_tabRFGen)
                        {
                            DisplayTrackingData();
                        }
                    }
                    else
                    {
                        if ((m_MainTab.SelectedTab == m_tabSpectrumAnalyzer) || (m_MainTab.SelectedTab == m_tabPowerChannel))
                        {
                            DisplaySpectrumAnalyzerData();
                        }
                        if ((m_MainTab.SelectedTab == m_tabWaterfall) || IsWaterfallOnMainScreen())
                        {
                            UpdateWaterfall();
                        }
                    }
                }
                else
                {
                    if (m_objRFEAnalyzer.Mode == RFECommunicator.eMode.MODE_TRACKING)
                    {
                        if (m_MainTab.SelectedTab == m_tabRFGen)
                        {
                            DisplayTrackingData_TextProgress(true);
                        }
                    }
                }
            }
            catch (Exception obEx)
            {
                ReportLog("timer_receive_Tick: " + obEx.Message, true);
            }

            if (m_bFirstTick)
            {
                m_bFirstTick = false;
                ReportLog("", true);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if ((menuSaveOnClose.Checked || menuContinuousLog.Checked) && (m_sFilenameRFE.Length == 0) && m_objRFEAnalyzer.IsAnalyzer())
            {
                GetNewFilename(RFExplorerFileType.SweepDataFile);
                SaveFileRFE(m_sFilenameRFE);
            }
            SaveProperties();
            m_objRFEAnalyzer.Close();
            m_objRFEGenerator.Close();
            Cursor.Current = Cursors.Default;
        }

        private void SetupSpectrumAnalyzerAxis()
        {
            if (m_objRFEAnalyzer == null)
                return;

            if (m_GraphSpectrumAnalyzer == null)
                return;

            #if CALLSTACK
            Console.WriteLine("CALLSTACK:SetupSpectrumAnalyzerAxis");
            #endif
            double fStart = m_objRFEAnalyzer.StartFrequencyMHZ;
            double fEnd = m_objRFEAnalyzer.CalculateEndFrequencyMHZ() - m_objRFEAnalyzer.StepFrequencyMHZ;
            double fMajorStep = 1.0;

            m_GraphSpectrumAnalyzer.GraphPane.XAxis.Scale.Min = fStart;
            m_GraphSpectrumAnalyzer.GraphPane.XAxis.Scale.Max = fEnd;

            if (m_objRFEAnalyzer.Mode == RFECommunicator.eMode.MODE_WIFI_ANALYZER)
            {
                //objGraph is a bar chart
                m_GraphSpectrumAnalyzer.GraphPane.XAxis.Scale.Min = fStart - 2.5f;
                m_GraphSpectrumAnalyzer.GraphPane.XAxis.Scale.Max = fEnd + 2.5f;
            }

            if ((fEnd - fStart) < 1.0)
            {
                fMajorStep = 0.1;
            }
            else if ((fEnd - fStart) < 10)
            {
                fMajorStep = 1.0;
            }
            else if ((fEnd - fStart) < 100)
            {
                fMajorStep = 10;
            }
            else if ((fEnd - fStart) < 500)
            {
                fMajorStep = 50;
            }
            else if ((fEnd - fStart) < 1000)
            {
                fMajorStep = 100;
            }

            m_GraphSpectrumAnalyzer.GraphPane.XAxis.Scale.MajorStep = fMajorStep;
            m_GraphSpectrumAnalyzer.GraphPane.XAxis.Scale.MinorStep = fMajorStep / 10.0;

            UpdateYAxis();

            UpdateDialogFromFreqSettings();
        }

        /// <summary>
        /// If the user have configured a different unit scale (dBuV or Watt) this function will
        /// return the amplitude in those units (from dBm) input
        /// </summary>
        /// <param name="dAmplitudeDBM">standard amplitude dBm value</param>
        /// <returns></returns>
        private double ConvertToCurrentAmplitudeUnit(double dAmplitudeDBM)
        {
            if (menuItemWatt.Checked)
                return RFECommunicator.Convert_dBm_2_Watt(dAmplitudeDBM);
            else if (menuItemDBUV.Checked)
                return RFECommunicator.Convert_dBm_2_dBuV(dAmplitudeDBM);
            else
                return dAmplitudeDBM;
        }

        /// <summary>
        /// Returns amplitude in dBm from whatever dAmplitude current value is specified
        /// </summary>
        /// <param name="dAmplitude"></param>
        /// <returns></returns>
        private double ConvertFromCurrentAmplitudeUnit(double dAmplitude)
        {
            if (menuItemWatt.Checked)
                return RFECommunicator.Convert_Watt_2_dBm(dAmplitude);
            else if (menuItemDBUV.Checked)
                return RFECommunicator.Convert_dBuV_2_dBm(dAmplitude);
            else
                return dAmplitude;
        }

        private double ConvertFromCurrentAmplitudeUnit(string sAmplitude)
        {
            return ConvertFromCurrentAmplitudeUnit(Convert.ToDouble(sAmplitude));
        }

        private string GetCurrentAmplitudeUnitLabel()
        {
            if (menuItemWatt.Checked)
                return "Watt";
            else if (menuItemDBUV.Checked)
                return "dBuV";
            else
                return "dBm";
        }

        private RFECommunicator.eAmplitudeUnit GetCurrentAmplitudeEnum()
        {
            if (menuItemWatt.Checked)
                return RFECommunicator.eAmplitudeUnit.Watt;
            else if (menuItemDBUV.Checked)
                return RFECommunicator.eAmplitudeUnit.dBuV;
            else
                return RFECommunicator.eAmplitudeUnit.dBm;
        }

        private string GetCurrentAmplitudeUnitFormat()
        {
            if (menuItemWatt.Checked)
            {
                return "E3";
            }
            else
            {
                return "0.00";
            }
        }

        /// <summary>
        /// Based on the currently used amplitude unit, it will convert to the right string representation format
        /// </summary>
        /// <param name="dBm"></param>
        /// <returns></returns>
        private string ConvertToCurrentAmplitudeString(double dBm)
        {
            return ConvertToCurrentAmplitudeUnit(dBm).ToString(GetCurrentAmplitudeUnitFormat());
        }

        private void DisplaySpectrumAnalyzerData()
        {
#if CALLSTACK_REALTIME
            Console.WriteLine("CALLSTACK:DisplaySpectrumAnalyzerData");
#endif
            m_objRFEAnalyzer.PeakValueMHZ = 0.0;
            m_objRFEAnalyzer.PeakValueAmplitudeDBM = RFECommunicator.MIN_AMPLITUDE_DBM;
            m_Markers.HideAllMarkers();
            foreach (CurveItem objCurve in m_GraphSpectrumAnalyzer.GraphPane.CurveList)
            {
                if (objCurve.Label.Text.Contains("Limit") || objCurve.Label.Text.Contains("Overload"))
                    continue;

                objCurve.Clear();
                objCurve.IsVisible = false;
                objCurve.Label.IsVisible = false;
                if (objCurve.IsLine)
                {
                    LineItem objLine = (LineItem)objCurve;
                    objLine.Line.IsSmooth = menuSmoothSignals.Checked;
                    if (menuThickTrace.Checked)
                    {
                        if (objLine.Label.Text=="Max Hold")
                            objLine.Line.Width = 6;
                        else
                            objLine.Line.Width = 3;
                    }
                    else
                    {
                        objLine.Line.Width = 1;
                    }
                    objLine.Line.FillFromBottom = menuSignalFill.Checked;
                    objLine.Line.Fill.IsVisible = menuSignalFill.Checked;
                }
            }
            m_GraphSpectrumAnalyzer.GraphPane.YAxis.MajorGrid.IsVisible = menuShowGrid.Checked;
            m_GraphSpectrumAnalyzer.GraphPane.YAxis.MinorGrid.IsVisible = menuShowGrid.Checked;
            m_GraphSpectrumAnalyzer.GraphPane.XAxis.MajorGrid.IsVisible = menuShowGrid.Checked;
            m_GraphSpectrumAnalyzer.GraphPane.XAxis.MinorGrid.IsVisible = menuShowGrid.Checked;
            m_GraphLine_Realtime.Line.Fill.IsVisible = false; //we never fill the realtime curve
            m_PointList_Realtime.Clear();
            m_PointList_Avg.Clear();
            m_PointList_Min.Clear();
            m_PointList_Max.Clear();
            m_PointList_MaxHold.Clear();
            m_MaxBar.Clear();

            if (m_objRFEAnalyzer.SweepData.Count == 0)
            {
                m_GraphSpectrumAnalyzer.Refresh();
                return; //nothing to paint
            }

            UInt32 nSweepIndex = (UInt32)numericSampleSA.Value;
            m_nDrawingIteration++;

            UInt32 nTotalCalculatorIterations = (UInt32)numericIterations.Value;
            if (m_bCalibrating)
            {
                nTotalCalculatorIterations = 5;
            }
            if (nTotalCalculatorIterations > nSweepIndex)
                nTotalCalculatorIterations = nSweepIndex;

            if ((m_nDrawingIteration & 0xf) == 0)
            {
                //Update screen status every 16 drawing iterations just to reduce overhead
                toolStripMemory.Value = (int)nSweepIndex;
                if (m_objRFEAnalyzer.PortConnected)
                    toolCOMStatus.Text = "Connected";
                else
                    toolCOMStatus.Text = "Disconnected";

                toolStripSamples.Text = "Total Samples in buffer: " + (UInt32)numericSampleSA.Value + "/" + RFESweepDataCollection.MAX_ELEMENTS + " - " + (100 * (double)numericSampleSA.Value / RFESweepDataCollection.MAX_ELEMENTS).ToString("0.0") + "%";
            }

            //Use the current data sweep item pointed out by the selected index
            RFESweepData objSweep = m_objRFEAnalyzer.SweepData.GetData(nSweepIndex);

            if (!m_objRFEAnalyzer.PortConnected || m_objRFEAnalyzer.HoldMode)
            {
                //If not connected, then rebuild the configuration text to properly show the capture date (if available)
                if (objSweep.CaptureTime.Year > 2000)
                {
                    m_StatusGraphText_Analyzer.Text = "Captured:" + objSweep.CaptureTime.ToString("yyyy-MM-dd HH:mm:ss\\.fff");
                }
                else
                {
                    m_StatusGraphText_Analyzer.Text = "Legacy file format - Unknown Capture Date";
                }
            }

            for (UInt16 nSweepPointInd = 0; objSweep != null && nSweepPointInd < m_objRFEAnalyzer.FreqSpectrumSteps; nSweepPointInd++)
            {
                if (nSweepPointInd < m_arrWiFiBarText.Length)
                    m_arrWiFiBarText[nSweepPointInd].Text = "";

                double dFreqStepMHZ = m_objRFEAnalyzer.StartFrequencyMHZ + m_objRFEAnalyzer.StepFrequencyMHZ * nSweepPointInd;
                double dRealtimeDBM = objSweep.GetAmplitudeDBM(nSweepPointInd, m_objRFEAnalyzer.m_AmplitudeCalibration, menuUseAmplitudeCorrection.Checked);
                double dMaxDBM = dRealtimeDBM;
                double dMinDBM = dRealtimeDBM;
                double dAverageDBM = dRealtimeDBM;

                for (UInt32 nSweepIterator = nSweepIndex - nTotalCalculatorIterations; nSweepIterator < nSweepIndex; nSweepIterator++)
                {
                    //Calculate average over Calculator range
                    RFESweepData objSweepIter = m_objRFEAnalyzer.SweepData.GetData(nSweepIterator);
                    if (objSweepIter != null)
                    {
                        double dLocalAvgDBM = objSweepIter.GetAmplitudeDBM(nSweepPointInd, m_objRFEAnalyzer.m_AmplitudeCalibration, menuUseAmplitudeCorrection.Checked);
                        dAverageDBM += dLocalAvgDBM;
                        if (dMinDBM > dLocalAvgDBM)
                        {
                            dMinDBM = dLocalAvgDBM;
                        }
                        if (dMaxDBM < dLocalAvgDBM)
                        {
                            dMaxDBM = dLocalAvgDBM;
                        }
                    }
                }

                if (menuUseAmplitudeCorrection.Checked || (menuRealtimeTrace.Checked && m_objRFEAnalyzer.Mode != RFECommunicator.eMode.MODE_WIFI_ANALYZER))
                {
                    //we always add realtime as may be needed for overload check, etc
                    m_PointList_Realtime.Add(dFreqStepMHZ, ConvertToCurrentAmplitudeUnit(dRealtimeDBM), RFECommunicator.MIN_AMPLITUDE_DBM);
                }
                if (menuMinTrace.Checked && m_objRFEAnalyzer.Mode != RFECommunicator.eMode.MODE_WIFI_ANALYZER)
                {
                    m_PointList_Min.Add(dFreqStepMHZ, ConvertToCurrentAmplitudeUnit(dMinDBM), RFECommunicator.MIN_AMPLITUDE_DBM);
                }
                if (menuMaxTrace.Checked || m_objRFEAnalyzer.Mode == RFECommunicator.eMode.MODE_WIFI_ANALYZER)
                {
                    m_PointList_Max.Add(dFreqStepMHZ, ConvertToCurrentAmplitudeUnit(dMaxDBM), RFECommunicator.MIN_AMPLITUDE_DBM);
                    if (m_objRFEAnalyzer.Mode == RFECommunicator.eMode.MODE_WIFI_ANALYZER && menuShowPeak.Checked)
                    {
                        if (nSweepPointInd < m_arrWiFiBarText.Length)
                        {
                            double fFreqMark = (m_objRFEAnalyzer.StartFrequencyMHZ + m_objRFEAnalyzer.StepFrequencyMHZ * nSweepPointInd);
                            m_arrWiFiBarText[nSweepPointInd].Text = "CH" + (nSweepPointInd + 1).ToString() + "\n" + fFreqMark.ToString("0") + "MHZ\n" + ConvertToCurrentAmplitudeString(dMaxDBM) + GetCurrentAmplitudeUnitLabel();
                            m_arrWiFiBarText[nSweepPointInd].Location.X = fFreqMark;
                            m_arrWiFiBarText[nSweepPointInd].Location.Y = ConvertToCurrentAmplitudeUnit(dMaxDBM);
                            m_arrWiFiBarText[nSweepPointInd].FontSpec.IsBold = false;
                            m_arrWiFiBarText[nSweepPointInd].FontSpec.FontColor = Color.Red;
                        }
                    }
                }
                if (menuMaxHoldTrace.Checked && (m_objRFEAnalyzer.SweepData.MaxHoldData != null) && m_objRFEAnalyzer.Mode != RFECommunicator.eMode.MODE_WIFI_ANALYZER)
                {
                    m_PointList_MaxHold.Add(dFreqStepMHZ, ConvertToCurrentAmplitudeUnit(m_objRFEAnalyzer.SweepData.MaxHoldData.GetAmplitudeDBM(nSweepPointInd, m_objRFEAnalyzer.m_AmplitudeCalibration, menuUseAmplitudeCorrection.Checked)), RFECommunicator.MIN_AMPLITUDE_DBM);
                }
                if ((menuAveragedTrace.Checked && m_objRFEAnalyzer.Mode != RFECommunicator.eMode.MODE_WIFI_ANALYZER) || m_bCalibrating)
                {
                    dAverageDBM = dAverageDBM / (nTotalCalculatorIterations + 1);
                    m_PointList_Avg.Add(dFreqStepMHZ, ConvertToCurrentAmplitudeUnit(dAverageDBM), RFECommunicator.MIN_AMPLITUDE_DBM);
                }
            }

            m_OverloadText.IsVisible = false;
            if (!m_bCalibrating)
            {
                //limit lines
                bool bPlaySound = false;
                PointPairList listCheck = null;
                SelectSinglePointPairList(ref listCheck);
                m_GraphLimitLineMax.Points = m_LimitLineMax;
                m_GraphLimitLineMax.IsVisible = m_LimitLineMax.Count > 1;
                if (m_LimitLineMax.Intersect(listCheck, true))
                {
                    bPlaySound = true;
                    m_GraphLimitLineMax.Line.Width = 5;
                }
                else
                {
                    m_GraphLimitLineMax.Line.Width = 1;
                }

                m_GraphLimitLineOverload.Points = m_LimitLineOverload;
                m_GraphLimitLineOverload.IsVisible = (m_LimitLineOverload.Count > 1) && menuUseAmplitudeCorrection.Checked;
                if (m_LimitLineOverload.Intersect(m_PointList_Realtime, true))
                {
                    bPlaySound = true;
                    m_GraphLimitLineOverload.Line.Width = 10;
                    m_OverloadText.IsVisible = true;
                }
                else
                {
                    m_GraphLimitLineOverload.Line.Width = 1;
                }

                m_GraphLimitLineMin.Points = m_LimitLineMin;
                m_GraphLimitLineMin.IsVisible = m_LimitLineMin.Count > 1;
                if (m_LimitLineMin.Intersect(listCheck, false))
                {
                    m_GraphLimitLineMin.Line.Width = 5;
                    bPlaySound = true;
                }
                else
                {
                    m_GraphLimitLineMin.Line.Width = 1;
                }
                if (bPlaySound && menuItemSoundAlarmLimitLine.Checked)
                {
                    PlayNotificationSound();
                }
                else
                {
                    StopNotificationSound();
                }

                if (m_objRFEAnalyzer.Mode == RFECommunicator.eMode.MODE_WIFI_ANALYZER)
                {
                    m_MaxBar.Points = m_PointList_Max;
                    m_MaxBar.IsVisible = true;
                }
                else
                {

                    //draw all marker except tracking 1
                    for (int nMenuInd = 1; nMenuInd < m_arrMarkersEnabledMenu.Length; nMenuInd++)
                    {
                        if (m_arrMarkersEnabledMenu[nMenuInd].Checked)
                        {
                            if (menuRealtimeTrace.Checked)
                            {
                                m_Markers.UpdateMarker(nMenuInd, RFExplorerSignalType.Realtime, m_PointList_Realtime.InterpolateX(m_Markers.GetMarkerFrequency(nMenuInd)));
                            }
                            if (menuAveragedTrace.Checked)
                            {
                                m_Markers.UpdateMarker(nMenuInd, RFExplorerSignalType.Average, m_PointList_Avg.InterpolateX(m_Markers.GetMarkerFrequency(nMenuInd)));
                            }
                            if (menuMaxTrace.Checked)
                            {
                                m_Markers.UpdateMarker(nMenuInd, RFExplorerSignalType.MaxPeak, m_PointList_Max.InterpolateX(m_Markers.GetMarkerFrequency(nMenuInd)));
                            }
                            if (menuMaxHoldTrace.Checked)
                            {
                                m_Markers.UpdateMarker(nMenuInd, RFExplorerSignalType.MaxHold, m_PointList_MaxHold.InterpolateX(m_Markers.GetMarkerFrequency(nMenuInd)));
                            }
                            if (menuMinTrace.Checked)
                            {
                                m_Markers.UpdateMarker(nMenuInd, RFExplorerSignalType.Min, m_PointList_Min.InterpolateX(m_Markers.GetMarkerFrequency(nMenuInd)));
                            }
                        }
                    }

                    double fTrackPeakMHZ = 0.0;
                    if (m_arrMarkersEnabledMenu[0].Checked)
                    {
                        double fTrackDBM = RFECommunicator.MIN_AMPLITUDE_DBM;
                        if ((m_eTrackSignalPeak == RFExplorerSignalType.Realtime) && menuRealtimeTrace.Checked)
                        {
                            fTrackPeakMHZ = m_PointList_Realtime[m_PointList_Realtime.GetIndexMax()].X;
                            fTrackDBM = m_PointList_Realtime[m_PointList_Realtime.GetIndexMax()].Y;
                        }
                        else if ((m_eTrackSignalPeak == RFExplorerSignalType.Average) && menuAveragedTrace.Checked)
                        {
                            fTrackPeakMHZ = m_PointList_Avg[m_PointList_Avg.GetIndexMax()].X;
                            fTrackDBM = m_PointList_Avg[m_PointList_Avg.GetIndexMax()].Y;
                        }
                        else if ((m_eTrackSignalPeak == RFExplorerSignalType.MaxHold) && menuMaxHoldTrace.Checked)
                        {
                            fTrackPeakMHZ = m_PointList_MaxHold[m_PointList_MaxHold.GetIndexMax()].X;
                            fTrackDBM = m_PointList_MaxHold[m_PointList_MaxHold.GetIndexMax()].Y;
                        }
                        else if ((m_eTrackSignalPeak == RFExplorerSignalType.MaxPeak) && menuMaxTrace.Checked)
                        {
                            fTrackPeakMHZ = m_PointList_Max[m_PointList_Max.GetIndexMax()].X;
                            fTrackDBM = m_PointList_Max[m_PointList_Max.GetIndexMax()].Y;
                        }
                        else if ((m_eTrackSignalPeak == RFExplorerSignalType.Min) && menuMinTrace.Checked)
                        {
                            fTrackPeakMHZ = m_PointList_Min[m_PointList_Min.GetIndexMax()].X;
                            fTrackDBM = m_PointList_Min[m_PointList_Min.GetIndexMax()].Y;
                        }
                        else
                        {
                            m_arrMarkersEnabledMenu[0].Checked = false;
                            UpdateMarkerControlContents();
                        }
                        m_Markers.SetMarkerFrequency(0, fTrackPeakMHZ);
                        m_objRFEAnalyzer.PeakValueMHZ = fTrackPeakMHZ;
                        m_objRFEAnalyzer.PeakValueAmplitudeDBM = RFECommunicator.ConvertAmplitude(GetCurrentAmplitudeEnum(), fTrackDBM, RFECommunicator.eAmplitudeUnit.dBm);
                    }
                    //remove old text from all peak track markers
                    m_Markers.CleanAllMarkerText(0);

                    //draw data curves
                    if (menuRealtimeTrace.Checked)
                    {
                        m_GraphLine_Realtime.Points = m_PointList_Realtime;
                        m_GraphLine_Realtime.IsVisible = true;
                        m_GraphLine_Realtime.Label.IsVisible = true;

                        if (m_arrMarkersEnabledMenu[0].Checked)
                        {
                            double dAmplitude = m_PointList_Realtime.InterpolateX(m_Markers.GetMarkerFrequency(0));
                            m_Markers.UpdateMarker(0, RFExplorerSignalType.Realtime, dAmplitude);
                            if ((m_eTrackSignalPeak == RFExplorerSignalType.Realtime) && menuShowPeak.Checked)
                            {
                                m_Markers.SetMarkerText(0, RFExplorerSignalType.Realtime, m_Markers.GetMarkerFrequency(0).ToString("0.000") + "MHZ\n" + dAmplitude.ToString(GetCurrentAmplitudeUnitFormat()) + GetCurrentAmplitudeUnitLabel());
                            }
                        }
                    }

                    if (menuAveragedTrace.Checked)
                    {
                        m_GraphLine_Avg.Points = m_PointList_Avg;
                        m_GraphLine_Avg.IsVisible = true;
                        m_GraphLine_Avg.Label.IsVisible = true;

                        if (m_arrMarkersEnabledMenu[0].Checked)
                        {
                            double dAmplitude = m_PointList_Avg.InterpolateX(m_Markers.GetMarkerFrequency(0));
                            m_Markers.UpdateMarker(0, RFExplorerSignalType.Average, dAmplitude);
                            if ((m_eTrackSignalPeak == RFExplorerSignalType.Average) && menuShowPeak.Checked)
                            {
                                m_Markers.SetMarkerText(0, RFExplorerSignalType.Average, m_Markers.GetMarkerFrequency(0).ToString("0.000") + "MHZ\n" + dAmplitude.ToString(GetCurrentAmplitudeUnitFormat()) + GetCurrentAmplitudeUnitLabel());
                            }
                        }
                    }

                    if (menuMaxTrace.Checked)
                    {
                        if (m_objRFEAnalyzer.Mode == RFECommunicator.eMode.MODE_SPECTRUM_ANALYZER)
                        {
                            m_GraphLine_Max.Points = m_PointList_Max;
                            m_GraphLine_Max.IsVisible = true;
                            m_GraphLine_Max.Label.IsVisible = true;

                            if (m_arrMarkersEnabledMenu[0].Checked)
                            {
                                double dAmplitude = m_PointList_Max.InterpolateX(m_Markers.GetMarkerFrequency(0));
                                m_Markers.UpdateMarker(0, RFExplorerSignalType.MaxPeak, dAmplitude);
                                if ((m_eTrackSignalPeak == RFExplorerSignalType.MaxPeak) && menuShowPeak.Checked)
                                {
                                    m_Markers.SetMarkerText(0, RFExplorerSignalType.MaxPeak, m_Markers.GetMarkerFrequency(0).ToString("0.000") + "MHZ\n" + dAmplitude.ToString(GetCurrentAmplitudeUnitFormat()) + GetCurrentAmplitudeUnitLabel());
                                }
                            }
                        }
                    }
                    if (menuMinTrace.Checked)
                    {
                        m_GraphLine_Min.Points = m_PointList_Min;
                        m_GraphLine_Min.IsVisible = true;
                        m_GraphLine_Min.Label.IsVisible = true;
                        if (m_arrMarkersEnabledMenu[0].Checked)
                        {
                            double dAmplitude = m_PointList_Min.InterpolateX(m_Markers.GetMarkerFrequency(0));
                            m_Markers.UpdateMarker(0, RFExplorerSignalType.Min, dAmplitude);
                            if ((m_eTrackSignalPeak == RFExplorerSignalType.Min) && menuShowPeak.Checked)
                            {
                                m_Markers.SetMarkerText(0, RFExplorerSignalType.Min, m_Markers.GetMarkerFrequency(0).ToString("0.000") + "MHZ\n" + dAmplitude.ToString(GetCurrentAmplitudeUnitFormat()) + GetCurrentAmplitudeUnitLabel());
                            }
                        }
                    }
                    if (menuMaxHoldTrace.Checked)
                    {
                        m_GraphLine_MaxHold.Points = m_PointList_MaxHold;
                        m_GraphLine_MaxHold.IsVisible = true;
                        m_GraphLine_MaxHold.Label.IsVisible = true;
                        if (m_arrMarkersEnabledMenu[0].Checked)
                        {
                            double dAmplitude = m_PointList_MaxHold.InterpolateX(m_Markers.GetMarkerFrequency(0));
                            m_Markers.UpdateMarker(0, RFExplorerSignalType.MaxHold, dAmplitude);
                            if ((m_eTrackSignalPeak == RFExplorerSignalType.MaxHold) && menuShowPeak.Checked)
                            {
                                m_Markers.SetMarkerText(0, RFExplorerSignalType.MaxHold, m_Markers.GetMarkerFrequency(0).ToString("0.000") + "MHZ\n" + dAmplitude.ToString(GetCurrentAmplitudeUnitFormat()) + GetCurrentAmplitudeUnitLabel());
                            }

                        }
                    }
                }

                UpdateMarkerControlValues();

                if (!m_bPrintModeEnabled)
                    m_GraphSpectrumAnalyzer.Refresh();

                if (m_MainTab.SelectedTab == m_tabPowerChannel)
                {
                    double fPowerDBM = 0.0f;
                    string sPowerMode = "Power Data Mode: ";

                    //Note: this is BETA and should be done by looking into sweep data collection, but we are not doing
                    //it here that way to avoid recalculation of max or average which will be costly.
                    if (menuAveragedTrace.Checked)
                    {
                        double fPowerTemp = 0.0f;
                        sPowerMode += "Average";

                        for (UInt16 nInd = 0; nInd < m_PointList_Avg.Count; nInd++)
                        {
                            fPowerTemp += RFECommunicator.ConvertAmplitude(GetCurrentAmplitudeEnum(), m_PointList_Avg[nInd].Y, RFECommunicator.eAmplitudeUnit.Watt);
                        }

                        if (fPowerTemp > 0.0f)
                        {
                            //add here actual RBW calculation in the future - currently we are assuming frequency step is the same
                            //as RBW which is not 100% accurate.
                            fPowerDBM = RFECommunicator.ConvertAmplitude(RFECommunicator.eAmplitudeUnit.Watt, fPowerTemp, RFECommunicator.eAmplitudeUnit.dBm);
                        }
                    }
                    else if (menuMaxTrace.Checked)
                    {
                        double fPowerTemp = 0.0f;
                        sPowerMode += "Max";

                        for (UInt16 nInd = 0; nInd < m_PointList_Max.Count; nInd++)
                        {
                            fPowerTemp += RFECommunicator.ConvertAmplitude(GetCurrentAmplitudeEnum(), m_PointList_Max[nInd].Y, RFECommunicator.eAmplitudeUnit.Watt);
                        }

                        if (fPowerTemp > 0.0f)
                        {
                            //add here actual RBW calculation in the future - currently we are assuming frequency step is the same
                            //as RBW which is not 100% accurate.
                            fPowerDBM = RFECommunicator.ConvertAmplitude(RFECommunicator.eAmplitudeUnit.Watt, fPowerTemp, RFECommunicator.eAmplitudeUnit.dBm);
                        }
                    }
                    else
                    {
                        sPowerMode += "Realtime";
                        fPowerDBM = objSweep.GetChannelPowerDBM();
                    }

                    double fPowerWatt = RFECommunicator.Convert_dBm_2_Watt(fPowerDBM);
                    double fPowerDensityDBM = RFECommunicator.Convert_Watt_2_dBm(fPowerWatt / (objSweep.GetFrequencySpanMHZ() * 1E6));
                    string sWatt = " Watt";
                    if (fPowerWatt < 0.5E-9)
                    {
                        fPowerWatt *= 1E12;
                        sWatt = " pW";
                    }
                    else if (fPowerWatt < 0.5E-6)
                    {
                        fPowerWatt *= 1E9;
                        sWatt = " nW";
                    }
                    else if (fPowerWatt < 0.5E-3)
                    {
                        fPowerWatt *= 1E6;
                        sWatt = " uW";
                    }
                    else if (fPowerWatt < 0.5f)
                    {
                        fPowerWatt *= 1E3;
                        sWatt = " mW";
                    }

                    m_PowerChannelNeedle.NeedleValue = fPowerDBM;

                    m_PowerChannelText.Text = sPowerMode +
                         "\nChannel Power: " + fPowerDBM.ToString("f1") + "dBm " + fPowerWatt.ToString("f1") + sWatt +
                         "\nChannel Power density: " + fPowerDensityDBM.ToString("f1") + " dBm/Hz" +
                         "\nChannel Center: " + objSweep.GetFrequencyMHZ((ushort)(objSweep.TotalSteps / 2)).ToString("f3") + " MHz" +
                         "\nChannel Bandwidth: " + objSweep.GetFrequencySpanMHZ().ToString("f3") + " MHz";

                    m_graphPowerChannel.Refresh();
                }
            }
            else
            {
                m_objRFEAnalyzer.PeakValueMHZ = m_PointList_Avg[m_PointList_Avg.GetIndexMax()].X;
                m_objRFEAnalyzer.PeakValueAmplitudeDBM = RFECommunicator.ConvertAmplitude(GetCurrentAmplitudeEnum(), m_PointList_Avg[m_PointList_Avg.GetIndexMax()].Y, RFECommunicator.eAmplitudeUnit.dBm);
            }
        }

        private string GraphPointValueHandler(ZedGraphControl control, GraphPane pane, CurveItem curve, int iPt)
        {
            // Get the PointPair that is under the mouse
            PointPair pt = curve[iPt];
            return pt.X.ToString("f3") + "MHZ\r\n" + pt.Y.ToString(GetCurrentAmplitudeUnitFormat()) + " " + GetCurrentAmplitudeUnitLabel();
        }

        private void zedSpectrumAnalyzer_ZoomEvent(ZedGraphControl sender, ZoomState oldState, ZoomState newState)
        {

        }

        private void SendCommand(string sData)
        {
            m_objRFEAnalyzer.SendCommand(sData);
        }

        private void SendNewConfig(double fStartMHZ, double fEndMHZ, double fTopDBM, double fBottomDBM)
        {
            //#[32]C2-F:Sssssss,Eeeeeee,tttt,bbbb
            UInt32 nStartKhz = (UInt32)(fStartMHZ * 1000);
            UInt32 nEndKhz = (UInt32)(fEndMHZ * 1000);
            Int16 nTopDBM = (Int16)fTopDBM;
            Int16 nBottomDBM = (Int16)fBottomDBM;

            string sTopDBM = "";
            if (nTopDBM >= 0)
                sTopDBM = nTopDBM.ToString("D4");
            else
                sTopDBM = nTopDBM.ToString("D3");

            string sData = "C2-F:" +
                nStartKhz.ToString("D7") + "," + nEndKhz.ToString("D7") + "," +
                sTopDBM + "," + nBottomDBM.ToString("D3");
            SendCommand(sData);

            ResetSettingsTitle();

            Thread.Sleep(500); //wait some time for the unit to process changes, otherwise may get a different command too soon
        }

        private void ResetSettingsTitle()
        {
            if (m_sLastSettingsLoaded != _Default)
            {
                m_sLastSettingsLoaded = _Default; //if we change the current configuration by hand, then it becomes _Default again
                UpdateTitleText_Analyzer();
            }
        }

        private void UpdateRemoteConfigData()
        {
            try
            {
                if (m_objRFEAnalyzer.PortConnected)
                {
                    SendNewConfig(Convert.ToDouble(m_sAnalyzerStartFreq.Text), Convert.ToDouble(m_sAnalyzerEndFreq.Text),
                        ConvertFromCurrentAmplitudeUnit(m_sAnalyzerTopAmplitude.Text), ConvertFromCurrentAmplitudeUnit(m_sAnalyzerBottomAmplitude.Text));
                }
            }
            catch (Exception obEx)
            {
                ReportLog(obEx.ToString(), false);
            }
        }

        private void CleanSweepData()
        {
            m_objRFEAnalyzer.SweepData.CleanAll();
            m_WaterfallSweepMaxHold.CleanAll();
            m_sFilenameRFE = "";
            UpdateButtonStatus();
            if (m_objRFEAnalyzer.HoldMode)
            {
                UpdateFeedMode();
                SetupSpectrumAnalyzerAxis();
                DisplaySpectrumAnalyzerData();
                m_GraphSpectrumAnalyzer.Invalidate();
            }
            if (m_controlWaterfall != null)
            {
                m_controlWaterfall.CleanAll();
            }
            m_objRFEAnalyzer.ResetInternalBuffers();
            numericSampleSA.Value = 0;
        }

        private void chkRunMode_CheckedChanged(object sender, EventArgs e)
        {
            m_objRFEAnalyzer.HoldMode = !chkRunMode.Checked;
            if (!m_objRFEAnalyzer.HoldMode && (m_objRFEAnalyzer.SweepData.IsFull()))
            {
                CleanSweepData();
                ReportLog("Buffer cleared.", false);
            }
            UpdateFeedMode();
        }

        private void chkHoldMode_CheckedChanged(object sender, EventArgs e)
        {
            m_objRFEAnalyzer.HoldMode = chkHoldMode.Checked;
            if (m_objRFEAnalyzer.HoldMode)
            {
                //Send hold mode to RF Explorer to stop RS232 traffic
                m_objRFEAnalyzer.SendCommand_Hold();
            }
            else
            {
                //Not on hold anymore, restore RS232 traffic
                m_objRFEAnalyzer.SendCommand_RequestConfigData();
                Thread.Sleep(50);
            }
            UpdateFeedMode();
        }

        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (m_objRFEAnalyzer.HoldMode || (!m_objRFEAnalyzer.PortConnected && m_objRFEAnalyzer.SweepData.Count > 0))
            {
                if (numericSampleSA.Value > m_objRFEAnalyzer.SweepData.Count)
                {
                    numericSampleSA.Value = m_objRFEAnalyzer.SweepData.Count;
                }
                if ((m_MainTab.SelectedTab == m_tabSpectrumAnalyzer) || (m_MainTab.SelectedTab == m_tabPowerChannel))
                {
                    DisplaySpectrumAnalyzerData();
                }
                else
                {
                    UpdateWaterfall();
                }
            }
        }

        private void OnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void OnAbout_Click(object sender, EventArgs e)
        {
            using (About_RFExplorer myAbout = new About_RFExplorer())
            {
                myAbout.ShowDialog();
            }
        }

        private void MainMenuView_DropDownOpening(object sender, EventArgs e)
        {
            menuTransparentWaterfall.Checked = m_controlWaterfall.Transparent;
            menuWaterfallFloor.Checked = m_controlWaterfall.DrawFloor;
        }

        private void click_view_mode(object sender, EventArgs e)
        {
            CheckSomeTraceModeIsEnabled();
            if (menuShowPeak.Checked)
            {
                m_arrMarkersEnabledMenu[0].Checked = true;
            }
            UpdateMarkerControlContents();
            UpdateButtonStatus();

            if (m_objRFEAnalyzer.HoldMode)
                DisplaySpectrumAnalyzerData();
        }

        private void OnSaveAsRFE_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog MySaveFileDialog = new SaveFileDialog())
                {
                    MySaveFileDialog.Filter = _RFE_File_Selector;
                    MySaveFileDialog.FilterIndex = 1;
                    MySaveFileDialog.RestoreDirectory = false;
                    MySaveFileDialog.InitialDirectory = m_sDefaultDataFolder;

                    GetNewFilename(RFExplorerFileType.SweepDataFile);
                    MySaveFileDialog.FileName = m_sFilenameRFE;

                    if (MySaveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        SaveFileRFE(MySaveFileDialog.FileName);
                    }
                }
            }
            catch (Exception obEx) { MessageBox.Show(obEx.Message); }
        }

        private void OnSaveCSV_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog MySaveFileDialog = new SaveFileDialog())
                {
                    MySaveFileDialog.Filter = _CSV_File_Selector;
                    MySaveFileDialog.FilterIndex = 1;
                    MySaveFileDialog.RestoreDirectory = false;
                    MySaveFileDialog.InitialDirectory = m_sDefaultDataFolder;

                    MySaveFileDialog.FileName = GetNewFilename(RFExplorerFileType.CumulativeCSVDataFile);

                    if (MySaveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        SaveFileCSV(MySaveFileDialog.FileName);
                    }
                }
            }
            catch (Exception obEx) { MessageBox.Show(obEx.Message); }
        }

        private void OnSaveSimpleCSV_Click(object sender, EventArgs e)
        {
            try
            {
                PointPairList listCurrentPointList = null;
                int nSelectionCounter = SelectSinglePointPairList(ref listCurrentPointList);

                if (nSelectionCounter == 0)
                {
                    MessageBox.Show("Single Signal CSV export needs one active display curve on screen (Avg, Max, Min or Realtime)", "Single Curve CSV Export");
                    return;
                }
                else if (nSelectionCounter > 1)
                {
                    MessageBox.Show("Single Signal CSV export requires one active display curve only on screen (Avg, Max, Min or Realtime)", "Single Curve CSV Export");
                    return;
                }

                using (SaveFileDialog MySaveFileDialog = new SaveFileDialog())
                {
                    MySaveFileDialog.Filter = _CSV_File_Selector;
                    MySaveFileDialog.FilterIndex = 1;
                    MySaveFileDialog.RestoreDirectory = false;
                    MySaveFileDialog.InitialDirectory = m_sDefaultDataFolder;

                    MySaveFileDialog.FileName = GetNewFilename(RFExplorerFileType.SimpleCSVDataFile);

                    if (MySaveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        SaveSimpleCSV(MySaveFileDialog.FileName, listCurrentPointList);
                    }
                }
            }
            catch (Exception obEx) { MessageBox.Show(obEx.Message); }
        }

        private char GetCSVDelimiter()
        {
            char cReturn = ',';

            /*
                Comma (,)
                Division (|)
                Semicolon (;)
                Space ( )
                Tabulator (\t)
             */

            switch (comboCSVFieldSeparator.SelectedIndex)
            {
                default:
                case 0: cReturn = ','; break;
                case 1: cReturn = '|'; break;
                case 2: cReturn = ';'; break;
                case 3: cReturn = ' '; break;
                case 4: cReturn = '\t'; break;
            }

            return cReturn;
        }

        private void SaveSimpleCSV(string sFilename, PointPairList listCurrentPointList)
        {
            try
            {

                //if no file path was explicited, add the default folder
                if (sFilename.IndexOf("\\") < 0)
                {
                    sFilename = m_sDefaultDataFolder + "\\" + sFilename;
                    sFilename = sFilename.Replace("\\\\", "\\");
                }

                char cCSV = GetCSVDelimiter();

                using (StreamWriter myFile = new StreamWriter(sFilename, true))
                {
                    foreach (PointPair objPointPair in listCurrentPointList)
                    {
                        myFile.WriteLine(objPointPair.X.ToString("0.000") + cCSV + objPointPair.Y.ToString("0.00"));
                    }
                }
            }
            catch (Exception obEx) { MessageBox.Show(obEx.Message); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            menuSaveAsRFE.Enabled = (m_objRFEAnalyzer.SweepData.Count > 0) && (m_MainTab.SelectedTab == m_tabSpectrumAnalyzer);
            menuSaveCSV.Enabled = (m_objRFEAnalyzer.SweepData.Count > 0) && (m_MainTab.SelectedTab == m_tabSpectrumAnalyzer);
            menusSaveSimpleCSV.Enabled = menuSaveCSV.Enabled;
            menuLoadRFE.Enabled = m_MainTab.SelectedTab == m_tabSpectrumAnalyzer;

            menuSaveRemoteImage.Enabled = (m_objRFEAnalyzer.ScreenData.Count > 0) && (m_MainTab.SelectedTab == m_tabRemoteScreen);
            menuLoadRFS.Enabled = m_MainTab.SelectedTab == m_tabRemoteScreen;
            menuSaveRFS.Enabled = (m_objRFEAnalyzer.ScreenData.Count > 0) && (m_MainTab.SelectedTab == m_tabRemoteScreen);

            menuPrint.Enabled = m_MainTab.SelectedTab == m_tabSpectrumAnalyzer;
            menuPrintPreview.Enabled = m_MainTab.SelectedTab == m_tabSpectrumAnalyzer;
            menuPageSetup.Enabled = m_MainTab.SelectedTab == m_tabSpectrumAnalyzer;

            menuSaveSNACSV.Enabled = false; //TODO
            menuSaveS1P.Enabled = false; //TODO
            menuSaveSNANormalization.Enabled = m_objRFEAnalyzer.PortConnected && m_objRFEGenerator.PortConnected && m_objRFEAnalyzer.IsTrackingNormalized();
            menuLoadSNANormalization.Enabled = m_objRFEAnalyzer.PortConnected && m_objRFEGenerator.PortConnected;

            if (RFExplorerClient.Properties.Settings.Default.MRUList.Length==0)
            {
                menuMRU.Enabled = false;
                menuMRU.Visible = false;
            }
            else
            {
                menuMRU.Enabled = true;
                menuMRU.Visible = true;

                if (menuMRU.DropDownItems.Count == 0)
                {
                    //add one sub item just to display it properly
                    ToolStripMenuItem menuNew = new ToolStripMenuItem(".");
                    menuNew.Name = "MRUFile_0";
                    menuNew.Click += new System.EventHandler(this.OnMRU_Click);
                    menuMRU.DropDownItems.Add(menuNew);
                }
            }
        }

        private void UpdateTitleText_Analyzer()
        {
            if (m_objRFEAnalyzer.HoldMode)
            {
                if (m_sFilenameRFE.Length > 0)
                {
                    m_GraphSpectrumAnalyzer.GraphPane.Title.Text = "RF Explorer File data";
                }
                else
                {
                    m_GraphSpectrumAnalyzer.GraphPane.Title.Text = "RF Explorer ON HOLD";
                }
                zedRAWDecoder.GraphPane.Title.Text = "RF Explorer Decoder ON HOLD";
            }
            else
            {
                if (m_objRFEAnalyzer.PortConnected)
                {
                    m_GraphSpectrumAnalyzer.GraphPane.Title.Text = "RF Explorer Live data";
                    zedRAWDecoder.GraphPane.Title.Text = "RF Explorer Decoder Live Data";
                }
                else
                {
                    m_GraphSpectrumAnalyzer.GraphPane.Title.Text = "RF Explorer Disconnected";
                    zedRAWDecoder.GraphPane.Title.Text = "RF Explorer Disconnected";
                }
            }

            m_GraphSpectrumAnalyzer.GraphPane.Title.Text += " - " + m_sLastSettingsLoaded + Environment.NewLine + m_sUserDefinedText;
            m_GraphTrackingGenerator.GraphPane.Title.Text = _RFEGEN_TRACKING_TITLE + " - " + m_sLastSettingsLoaded + Environment.NewLine + m_sUserDefinedText;
        }

        private void UpdateFeedMode()
        {
            if (!m_objRFEAnalyzer.PortConnected)
            {
                m_objRFEAnalyzer.HoldMode = true;
            }

            chkRunMode.Checked = !m_objRFEAnalyzer.HoldMode;
            chkRunDecoder.Checked = !m_objRFEAnalyzer.HoldMode;
            chkHoldMode.Checked = m_objRFEAnalyzer.HoldMode;
            chkHoldDecoder.Checked = m_objRFEAnalyzer.HoldMode;
            if ((m_objRFEAnalyzer.HoldMode == false) || (m_objRFEAnalyzer.SweepData.Count==0))
            {
                toolFile.Text = " - File: none";
                m_sFilenameRFE = "";
            }
            else if ((m_objRFEAnalyzer.HoldMode == true) && (m_objRFEAnalyzer.SweepData.Count > 0))
            {
                toolFile.Text = " - File: " + m_sFilenameRFE;
            }

            UpdateTitleText_Analyzer();

            m_GraphSpectrumAnalyzer.Refresh();
            zedRAWDecoder.Refresh();
        }

        private void SaveFileRFE(string sFilename)
        {
            try
            {
                //if no file path was explicited, add the default folder
                if (sFilename.IndexOf("\\") < 0)
                {
                    sFilename = m_sDefaultDataFolder + "\\" + sFilename;
                    sFilename = sFilename.Replace("\\\\", "\\");
                }
                m_objRFEAnalyzer.SaveFileRFE(sFilename, menuUseAmplitudeCorrection.Checked);
            }
            catch (Exception obEx) { MessageBox.Show(obEx.ToString()); }
        }

        private void SaveFileCSV(string sFilename)
        {
            try
            {
                //if no file path was explicited, add the default folder
                if (sFilename.IndexOf("\\") < 0)
                {
                    sFilename = m_sDefaultDataFolder + "\\" + sFilename;
                    sFilename = sFilename.Replace("\\\\", "\\");
                }
                if (menuUseAmplitudeCorrection.Checked)
                    m_objRFEAnalyzer.SweepData.SaveFileCSV(sFilename, GetCSVDelimiter(), m_objRFEAnalyzer.m_AmplitudeCalibration);
                else
                    m_objRFEAnalyzer.SweepData.SaveFileCSV(sFilename, GetCSVDelimiter(), null);

            }
            catch (Exception obEx) { MessageBox.Show(obEx.Message); }
        }

        private void UpdateSweepNumericControls()
        {
#if CALLSTACK_REALTIME
            Console.WriteLine("CALLSTACK:UpdateSweepNumericControls");
#endif
            //Update sweep data
            if (m_objRFEAnalyzer.SweepData.Count < numericSampleSA.Value)
            {
                numericSampleSA.Value = m_objRFEAnalyzer.SweepData.Count - 1;
            }
            //we can now safely change the max and the value (if not did already)
            numericSampleSA.Maximum = m_objRFEAnalyzer.SweepData.Count - 1;
            numericSampleSA.Value = m_objRFEAnalyzer.SweepData.Count - 1;
        }

        private void LoadFileRFE(string sFile)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (m_objRFEAnalyzer.PortConnected)
            {
                m_groupCOMPortAnalyzer.ClosePort();
                m_groupCOMPortGenerator.ClosePort();
            }
            try
            {
                CleanSweepData();

                if (m_objRFEAnalyzer.LoadDataFile(sFile))
                {
                    menuUseAmplitudeCorrection.Checked = false;

                    CheckSomeTraceModeIsEnabled();
                    UpdateSweepNumericControls();
                    UpdateButtonStatus();
                    UpdateConfigControlContents();

                    ReportLog("File " + sFile + " loaded with total of " + m_objRFEAnalyzer.SweepData.Count + " sweeps.", false);
                    m_sFilenameRFE = sFile;

                    UpdateFeedMode();

                    AutoLoadAmplitudeDataFile();

                    if (m_LimitLineOverload.Count > 0)
                    {
                        //potentially offset may change
                        m_LimitLineOverload.NewOffset(m_objRFEAnalyzer.AmplitudeOffsetDB);
                    }

                    SetupSpectrumAnalyzerAxis();
                    DisplaySpectrumAnalyzerData();
                    UpdateWaterfall();

                    AddMRUFile(m_sFilenameRFE);
                }
                else
                {
                    MessageBox.Show("Wrong or unknown file format");
                }
            }
            catch (Exception obEx) { MessageBox.Show(obEx.Message); }
            Cursor.Current = Cursors.Default;
        }

        private void AddMRUFile(string sFile)
        {
            string sStoredMRU = RFExplorerClient.Properties.Settings.Default.MRUList;

            if (sStoredMRU.Contains(sFile))
            {
                //avoid the file to be included twice, but we want it to be added on top so added it even if it was before
                //remove it in all posible combinations it can be included
                sStoredMRU = sStoredMRU.Replace(sFile + ";", "");
                sStoredMRU = sStoredMRU.Replace(";" + sFile, "");
                sStoredMRU = sStoredMRU.Replace(sFile, "");
            }

            if (sStoredMRU.Length > 1)
            {
                sStoredMRU = sFile + ";" + sStoredMRU;
            }
            else
            {
                sStoredMRU = sFile;
            }

            //add the new file on top of the list
            RFExplorerClient.Properties.Settings.Default.MRUList = sStoredMRU;
        }

        private void OnLoadFileRFE_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog MyOpenFileDialog = new OpenFileDialog())
                {
                    MyOpenFileDialog.Filter = _RFE_File_Selector;
                    MyOpenFileDialog.FilterIndex = 1;
                    MyOpenFileDialog.RestoreDirectory = false;
                    MyOpenFileDialog.InitialDirectory = m_sDefaultDataFolder;

                    if (MyOpenFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        LoadFileRFE(MyOpenFileDialog.FileName);
                    }
                }
            }
            catch (Exception obEx) { MessageBox.Show(obEx.Message); }
        }

        private void objGraph_ContextMenuBuilder(ZedGraphControl sender, ContextMenuStrip menuStrip, Point mousePt, ZedGraphControl.ContextMenuObjectState objState)
        {
            menuStrip.Items["save_as"].Click += new System.EventHandler(OnSaveImage_Click);
            menuStrip.Items["copy"].Click += new System.EventHandler(OnClipboard_Click);
        }

        protected void UpdateYAxis()
        {
#if CALLSTACK
            Console.WriteLine("CALLSTACK:UpdateYAxis");
#endif
            //check and fix absolute margins
            if (m_objRFEAnalyzer.AmplitudeBottomNormalizedDBM < RFECommunicator.MIN_AMPLITUDE_DBM)
            {
                m_objRFEAnalyzer.AmplitudeBottomNormalizedDBM = RFECommunicator.MIN_AMPLITUDE_DBM;
            }
            if (m_objRFEAnalyzer.AmplitudeTopNormalizedDBM > RFECommunicator.MAX_AMPLITUDE_DBM)
            {
                m_objRFEAnalyzer.AmplitudeTopNormalizedDBM = RFECommunicator.MAX_AMPLITUDE_DBM;
            }

            //Check and fix relative margins
            if (m_objRFEAnalyzer.AmplitudeTopNormalizedDBM < (RFECommunicator.MIN_AMPLITUDE_DBM + RFECommunicator.MIN_AMPLITUDE_RANGE_DBM))
            {
                m_objRFEAnalyzer.AmplitudeTopNormalizedDBM = RFECommunicator.MIN_AMPLITUDE_DBM + RFECommunicator.MIN_AMPLITUDE_RANGE_DBM;
            }
            if (m_objRFEAnalyzer.AmplitudeBottomNormalizedDBM >= m_objRFEAnalyzer.AmplitudeTopNormalizedDBM)
            {
                m_objRFEAnalyzer.AmplitudeBottomNormalizedDBM = m_objRFEAnalyzer.AmplitudeTopNormalizedDBM - RFECommunicator.MIN_AMPLITUDE_RANGE_DBM;
            }

            m_GraphSpectrumAnalyzer.GraphPane.YAxis.Scale.MajorStepAuto = false;
            m_GraphSpectrumAnalyzer.GraphPane.YAxis.Scale.MinorStepAuto = false;
            m_GraphSpectrumAnalyzer.GraphPane.YAxis.Title.Text = "Amplitude (" + GetCurrentAmplitudeUnitLabel() + ")";
            m_GraphSpectrumAnalyzer.GraphPane.YAxis.MinorGrid.IsVisible = menuItemWatt.Checked;

            if (menuItemWatt.Checked)
            {
                //Convert to Watt
                m_GraphSpectrumAnalyzer.GraphPane.YAxis.Type = AxisType.Log;
                m_GraphSpectrumAnalyzer.GraphPane.YAxis.Scale.MajorStep = 1;
                m_GraphSpectrumAnalyzer.GraphPane.YAxis.Scale.MinorStepAuto = true;
            }
            else
            {
                //valid for dBm and dBuV
                m_GraphSpectrumAnalyzer.GraphPane.YAxis.Type = AxisType.Linear;
                m_GraphSpectrumAnalyzer.GraphPane.YAxis.Scale.MajorStep = 10.0;
                if ((m_objRFEAnalyzer.AmplitudeTopDBM - m_objRFEAnalyzer.AmplitudeBottomDBM) > 30)
                {
                    m_GraphSpectrumAnalyzer.GraphPane.YAxis.Scale.MinorStep = 5.0;
                }
                else
                {
                    m_GraphSpectrumAnalyzer.GraphPane.YAxis.Scale.MinorStep = 1.0;
                }
            }
            m_GraphSpectrumAnalyzer.GraphPane.YAxis.Scale.Min = ConvertToCurrentAmplitudeUnit(m_objRFEAnalyzer.AmplitudeBottomDBM);
            m_GraphSpectrumAnalyzer.GraphPane.YAxis.Scale.Max = ConvertToCurrentAmplitudeUnit(m_objRFEAnalyzer.AmplitudeTopDBM);

            m_GraphSpectrumAnalyzer.Refresh();
        }

        private void OnCOMPortInfo_Click(object sender, EventArgs e)
        {
            m_objRFEAnalyzer.ListAllCOMPorts();
        }

        private void OnReset_Click(object sender, EventArgs e)
        {
            UpdateDialogFromFreqSettings();
        }

        private void UpdateDialogFromFreqSettings()
        {
            m_sAnalyzerBottomAmplitude.Text = ConvertToCurrentAmplitudeString(m_objRFEAnalyzer.AmplitudeBottomDBM);
            m_sAnalyzerTopAmplitude.Text = ConvertToCurrentAmplitudeString(m_objRFEAnalyzer.AmplitudeTopDBM);
            m_sAnalyzerStartFreq.Text = m_objRFEAnalyzer.StartFrequencyMHZ.ToString("f3");
            m_sRefFrequency.Text = m_objRFEAnalyzer.RefFrequencyMHZ.ToString("f3");
            m_sAnalyzerEndFreq.Text = m_objRFEAnalyzer.CalculateEndFrequencyMHZ().ToString("f3");
            m_sAnalyzerCenterFreq.Text = m_objRFEAnalyzer.CalculateCenterFrequencyMHZ().ToString("f3");
            m_sAnalyzerFreqSpan.Text = m_objRFEAnalyzer.CalculateFrequencySpanMHZ().ToString("f3");
        }

        private bool IsDifferent(double d1, double d2, double dEpsilon = 0.001)
        {
            return (Math.Abs(d1 - d2) > dEpsilon);
        }

        private void OnSendAnalyzerConfiguration_Click(object sender, EventArgs e)
        {
            UpdateYAxis();
            UpdateRemoteConfigData();
        }

        private void OnMoveFreqDecLarge_Click(object sender, EventArgs e)
        {
            double fStartFreq = Convert.ToDouble(m_sAnalyzerStartFreq.Text);
            fStartFreq -= m_objRFEAnalyzer.CalculateFrequencySpanMHZ() * 0.5;
            double fEndFreq = Convert.ToDouble(m_sAnalyzerEndFreq.Text);
            fEndFreq -= m_objRFEAnalyzer.CalculateFrequencySpanMHZ() * 0.5;
            m_sAnalyzerStartFreq.Text = fStartFreq.ToString("f3");
            m_sAnalyzerEndFreq.Text = fEndFreq.ToString("f3");
            OnStartFreq_Leave(null, null);

            UpdateRemoteConfigData();
        }

        private void OnMoveFreqIncLarge_Click(object sender, EventArgs e)
        {
            double fStartFreq = Convert.ToDouble(m_sAnalyzerStartFreq.Text);
            fStartFreq += m_objRFEAnalyzer.CalculateFrequencySpanMHZ() * 0.5;
            double fEndFreq = Convert.ToDouble(m_sAnalyzerEndFreq.Text);
            fEndFreq += m_objRFEAnalyzer.CalculateFrequencySpanMHZ() * 0.5;
            m_sAnalyzerStartFreq.Text = fStartFreq.ToString("f3");
            m_sAnalyzerEndFreq.Text = fEndFreq.ToString("f3");
            OnEndFreq_Leave(null, null);

            UpdateRemoteConfigData();
        }

        private void OnMoveFreqDecSmall_Click(object sender, EventArgs e)
        {
            m_objRFEAnalyzer.StartFrequencyMHZ -= m_objRFEAnalyzer.CalculateFrequencySpanMHZ() / 10;
            if (m_objRFEAnalyzer.StartFrequencyMHZ < m_objRFEAnalyzer.MinFreqMHZ)
            {
                m_objRFEAnalyzer.StartFrequencyMHZ = m_objRFEAnalyzer.MinFreqMHZ;
            }

            SetupSpectrumAnalyzerAxis();
            SaveProperties();
            UpdateRemoteConfigData();
        }

        private void OnMoveFreqIncSmall_Click(object sender, EventArgs e)
        {
            m_objRFEAnalyzer.StartFrequencyMHZ += m_objRFEAnalyzer.CalculateFrequencySpanMHZ() / 10;
            if (m_objRFEAnalyzer.CalculateEndFrequencyMHZ() > m_objRFEAnalyzer.MaxFreqMHZ)
            {
                m_objRFEAnalyzer.StartFrequencyMHZ = m_objRFEAnalyzer.MaxFreqMHZ - m_objRFEAnalyzer.CalculateFrequencySpanMHZ();
            }

            SetupSpectrumAnalyzerAxis();
            SaveProperties();
            UpdateRemoteConfigData();
        }

        private void OnSpanDec_Click(object sender, EventArgs e)
        {
            double fFreqSpan = Convert.ToDouble(m_sAnalyzerFreqSpan.Text);
            fFreqSpan -= fFreqSpan * 0.25;
            m_sAnalyzerFreqSpan.Text = fFreqSpan.ToString("f3");
            OnFreqSpan_Leave(null, null);
            UpdateRemoteConfigData();
        }

        private void OnSpanInc_Click(object sender, EventArgs e)
        {
            double fFreqSpan = Convert.ToDouble(m_sAnalyzerFreqSpan.Text);
            fFreqSpan += fFreqSpan * 0.25;
            m_sAnalyzerFreqSpan.Text = fFreqSpan.ToString("f3");
            OnFreqSpan_Leave(null, null);
            UpdateRemoteConfigData();
        }

        private void OnSpanMax_Click(object sender, EventArgs e)
        {
            double fFreqSpan = 10000; //just a big number
            m_sAnalyzerFreqSpan.Text = fFreqSpan.ToString("f3");
            OnFreqSpan_Leave(null, null);
            UpdateRemoteConfigData();
        }

        private void OnSpanDefault_Click(object sender, EventArgs e)
        {
            double fFreqSpan = 10;
            m_sAnalyzerFreqSpan.Text = fFreqSpan.ToString("f3");
            OnFreqSpan_Leave(null, null);
            UpdateRemoteConfigData();
        }

        private void OnSpanMin_Click(object sender, EventArgs e)
        {
            double fFreqSpan = 0; //just a very small number
            m_sAnalyzerFreqSpan.Text = fFreqSpan.ToString("f3");
            OnFreqSpan_Leave(null, null);
            UpdateRemoteConfigData();
        }

        private void IncreaseTopAmplitude(double dIncreaseAmplitudeDBM)
        {
            double fAmplitudeTop = ConvertFromCurrentAmplitudeUnit(m_sAnalyzerTopAmplitude.Text);
            fAmplitudeTop += dIncreaseAmplitudeDBM;
            m_sAnalyzerTopAmplitude.Text = ConvertToCurrentAmplitudeString(fAmplitudeTop);
            OnAmplitudeLeave(null, null);
            if (m_objRFEAnalyzer.PortConnected && menuRemoteAmplitudeUpdate.Checked && menuAutoLCDOff.Checked == false)
            {
                UpdateRemoteConfigData();
            }
            else
            {
                m_objRFEAnalyzer.AmplitudeTopDBM = fAmplitudeTop;
                UpdateYAxis();
                DisplaySpectrumAnalyzerData();
            }
        }

        private void IncreaseBottomAmplitude(double dIncreaseAmplitudeDBM)
        {
            double fAmplitudeBottom = ConvertFromCurrentAmplitudeUnit(m_sAnalyzerBottomAmplitude.Text);
            fAmplitudeBottom += dIncreaseAmplitudeDBM;
            m_sAnalyzerBottomAmplitude.Text = ConvertToCurrentAmplitudeString(fAmplitudeBottom);
            OnAmplitudeLeave(null, null);
            if (m_objRFEAnalyzer.PortConnected && menuRemoteAmplitudeUpdate.Checked && menuAutoLCDOff.Checked == false)
            {
                UpdateRemoteConfigData();
            }
            else
            {
                m_objRFEAnalyzer.AmplitudeBottomDBM = fAmplitudeBottom;
                UpdateYAxis();
                DisplaySpectrumAnalyzerData();
            }
        }

        private void OnAutoscale_Click(object sender, EventArgs e)
        {
            double fTop = ConvertToCurrentAmplitudeUnit(RFECommunicator.MIN_AMPLITUDE_DBM);
            double fBottom = ConvertToCurrentAmplitudeUnit(RFECommunicator.MAX_AMPLITUDE_DBM);
            int nVal1, nVal2;

            if (menuRealtimeTrace.Checked)
            {
                m_PointList_Realtime.GetMaxMinValues(ref fBottom, ref fTop, out nVal1, out nVal2);
            }
            if (menuAveragedTrace.Checked)
            {
                m_PointList_Avg.GetMaxMinValues(ref fBottom, ref fTop, out nVal1, out nVal2);
            }
            if (menuMaxTrace.Checked)
            {
                m_PointList_Max.GetMaxMinValues(ref fBottom, ref fTop, out nVal1, out nVal2);
            }
            if (menuMaxHoldTrace.Checked)
            {
                m_PointList_MaxHold.GetMaxMinValues(ref fBottom, ref fTop, out nVal1, out nVal2);
            }
            if (menuMinTrace.Checked)
            {
                m_PointList_Min.GetMaxMinValues(ref fBottom, ref fTop, out nVal1, out nVal2);
            }

            if (menuItemWatt.Checked)
            {
                fTop += fTop * 10; //add a range on top
                fBottom -= fBottom * 0.1; //add a range at the bottom
            }
            else
            {
                if ((fTop - fBottom) < 20)
                {
                    fTop += 10;
                    fBottom -= 10;
                }

                fTop += (fTop - fBottom) * 0.3; //add a 20% on top
                fBottom -= (fTop - fBottom) * 0.1; //add a 10% at the bottom
            }

            m_sAnalyzerBottomAmplitude.Text = fBottom.ToString(GetCurrentAmplitudeUnitFormat());
            m_sAnalyzerTopAmplitude.Text = fTop.ToString(GetCurrentAmplitudeUnitFormat());

            OnAmplitudeLeave(null, null);
            if (m_objRFEAnalyzer.PortConnected && menuRemoteAmplitudeUpdate.Checked && menuAutoLCDOff.Checked==false)
            {
                UpdateRemoteConfigData();
            }

            m_GraphSpectrumAnalyzer.ZoomOutAll(m_GraphSpectrumAnalyzer.GraphPane);
            m_objRFEAnalyzer.AmplitudeTopDBM = ConvertFromCurrentAmplitudeUnit(fTop);
            m_objRFEAnalyzer.AmplitudeBottomDBM = ConvertFromCurrentAmplitudeUnit(fBottom);
            UpdateYAxis();
            DisplaySpectrumAnalyzerData();
        }

        private void OnTop5plus_Click(object sender, EventArgs e)
        {
            IncreaseTopAmplitude(5);
        }

        private void OnTop5minus_Click(object sender, EventArgs e)
        {
            IncreaseTopAmplitude(-5);
        }

        private void OnBottom5plus_Click(object sender, EventArgs e)
        {
            IncreaseBottomAmplitude(5);
        }

        private void OnBottom5minus_Click(object sender, EventArgs e)
        {
            IncreaseBottomAmplitude(-5);
        }

        private void OnCenterMark_Click(object sender, EventArgs e)
        {
            if (m_objRFEAnalyzer.PeakValueMHZ > 0.0f)
            {
                m_sAnalyzerCenterFreq.Text = m_objRFEAnalyzer.PeakValueMHZ.ToString("f3");
                OnCenterFreq_Leave(null, null);

                UpdateRemoteConfigData();
            }
        }

        private void OnStartFreq_Leave(object sender, EventArgs e)
        {
            try
            {
                double fStartFreq = Convert.ToDouble(m_sAnalyzerStartFreq.Text);
                fStartFreq = Math.Max(m_objRFEAnalyzer.MinFreqMHZ, fStartFreq);
                fStartFreq = Math.Min(m_objRFEAnalyzer.MaxFreqMHZ - m_objRFEAnalyzer.MinSpanMHZ, fStartFreq);

                double fEndFreq = Convert.ToDouble(m_sAnalyzerEndFreq.Text);
                fEndFreq = Math.Max(m_objRFEAnalyzer.MinFreqMHZ + m_objRFEAnalyzer.MinSpanMHZ, fEndFreq);
                fEndFreq = Math.Min(m_objRFEAnalyzer.MaxFreqMHZ, fEndFreq);

                double fFreqSpan = (fEndFreq - fStartFreq);
                fFreqSpan = Math.Max(m_objRFEAnalyzer.MinSpanMHZ, fFreqSpan);
                fFreqSpan = Math.Min(m_objRFEAnalyzer.MaxSpanMHZ, fFreqSpan);

                fEndFreq = fStartFreq + fFreqSpan;

                m_sAnalyzerStartFreq.Text = fStartFreq.ToString("f3");
                m_sAnalyzerEndFreq.Text = fEndFreq.ToString("f3");

                m_sAnalyzerCenterFreq.Text = (fStartFreq + fFreqSpan / 2.0).ToString("f3");
                m_sAnalyzerFreqSpan.Text = (fFreqSpan).ToString("f3");
            }
            catch (Exception obEx)
            {
                ReportLog(obEx.ToString(), false);
            }
        }

        private void OnEndFreq_Leave(object sender, EventArgs e)
        {
            try
            {
                double fStartFreq = Convert.ToDouble(m_sAnalyzerStartFreq.Text);
                fStartFreq = Math.Max(m_objRFEAnalyzer.MinFreqMHZ, fStartFreq);
                fStartFreq = Math.Min(m_objRFEAnalyzer.MaxFreqMHZ - m_objRFEAnalyzer.MinSpanMHZ, fStartFreq);

                double fEndFreq = Convert.ToDouble(m_sAnalyzerEndFreq.Text);
                fEndFreq = Math.Max(m_objRFEAnalyzer.MinFreqMHZ + m_objRFEAnalyzer.MinSpanMHZ, fEndFreq);
                fEndFreq = Math.Min(m_objRFEAnalyzer.MaxFreqMHZ, fEndFreq);

                double fFreqSpan = (fEndFreq - fStartFreq);
                fFreqSpan = Math.Max(m_objRFEAnalyzer.MinSpanMHZ, fFreqSpan);
                fFreqSpan = Math.Min(m_objRFEAnalyzer.MaxSpanMHZ, fFreqSpan);

                fStartFreq = fEndFreq - fFreqSpan;

                m_sAnalyzerStartFreq.Text = fStartFreq.ToString("f3");
                m_sAnalyzerEndFreq.Text = fEndFreq.ToString("f3");

                m_sAnalyzerCenterFreq.Text = (fStartFreq + fFreqSpan / 2.0).ToString("f3");
                m_sAnalyzerFreqSpan.Text = (fFreqSpan).ToString("f3");
            }
            catch (Exception obEx)
            {
                ReportLog(obEx.ToString(), false);
            }
        }

        private void OnFreqSpan_Leave(object sender, EventArgs e)
        {
            try
            {
                double fFreqSpan = Convert.ToDouble(m_sAnalyzerFreqSpan.Text);
                fFreqSpan = Math.Max(m_objRFEAnalyzer.MinSpanMHZ, fFreqSpan);
                fFreqSpan = Math.Min(m_objRFEAnalyzer.MaxSpanMHZ, fFreqSpan);

                double fCenterFreq = Convert.ToDouble(m_sAnalyzerCenterFreq.Text);
                if ((fCenterFreq - (fFreqSpan / 2.0)) < m_objRFEAnalyzer.MinFreqMHZ)
                    fCenterFreq = (m_objRFEAnalyzer.MinFreqMHZ + (fFreqSpan / 2.0));
                if ((fCenterFreq + (fFreqSpan / 2.0)) > m_objRFEAnalyzer.MaxFreqMHZ)
                    fCenterFreq = (m_objRFEAnalyzer.MaxFreqMHZ - (fFreqSpan / 2.0));

                m_sAnalyzerFreqSpan.Text = fFreqSpan.ToString("f3");
                m_sAnalyzerCenterFreq.Text = fCenterFreq.ToString("f3");

                double fStartMHZ = fCenterFreq - fFreqSpan / 2.0;
                m_sAnalyzerStartFreq.Text = fStartMHZ.ToString("f3");
                m_sAnalyzerEndFreq.Text = (fStartMHZ + fFreqSpan).ToString("f3");
            }
            catch (Exception obEx)
            {
                ReportLog(obEx.ToString(), false);
            }
        }

        private void OnCenterFreq_Leave(object sender, EventArgs e)
        {
            try
            {
                double fCenterFreq = Convert.ToDouble(m_sAnalyzerCenterFreq.Text);
                if (fCenterFreq > (m_objRFEAnalyzer.MaxFreqMHZ - (m_objRFEAnalyzer.MinSpanMHZ / 2.0)))
                    fCenterFreq = (m_objRFEAnalyzer.MaxFreqMHZ - (m_objRFEAnalyzer.MinSpanMHZ / 2.0));
                if (fCenterFreq < (m_objRFEAnalyzer.MinFreqMHZ + (m_objRFEAnalyzer.MinSpanMHZ / 2.0)))
                    fCenterFreq = (m_objRFEAnalyzer.MinFreqMHZ + (m_objRFEAnalyzer.MinSpanMHZ / 2.0));

                double fFreqSpan = Convert.ToDouble(m_sAnalyzerFreqSpan.Text);
                if ((fCenterFreq - (fFreqSpan / 2.0)) < m_objRFEAnalyzer.MinFreqMHZ)
                    fFreqSpan = (fCenterFreq - m_objRFEAnalyzer.MinFreqMHZ) * 2.0;
                if ((fCenterFreq + (fFreqSpan / 2.0)) > m_objRFEAnalyzer.MaxFreqMHZ)
                    fFreqSpan = (m_objRFEAnalyzer.MaxFreqMHZ - fCenterFreq) * 2.0;
                m_sAnalyzerFreqSpan.Text = fFreqSpan.ToString("f3");
                m_sAnalyzerCenterFreq.Text = fCenterFreq.ToString("f3");

                double fStartMHZ = fCenterFreq - fFreqSpan / 2.0;
                m_sAnalyzerStartFreq.Text = fStartMHZ.ToString("f3");
                m_sAnalyzerEndFreq.Text = (fStartMHZ + fFreqSpan).ToString("f3");
            }
            catch (Exception obEx)
            {
                ReportLog(obEx.ToString(), false);
            }
        }

        private void OnAmplitudeLeave(object sender, EventArgs e)
        {
            try
            {
                double fAmplitudeBottom = Convert.ToDouble(m_sAnalyzerBottomAmplitude.Text);
                double fAmplitudeTop = Convert.ToDouble(m_sAnalyzerTopAmplitude.Text);

                //If not in dBm convert them to dBm
                fAmplitudeTop = ConvertFromCurrentAmplitudeUnit(fAmplitudeTop);
                fAmplitudeBottom = ConvertFromCurrentAmplitudeUnit(fAmplitudeBottom);

                if (fAmplitudeBottom - m_objRFEAnalyzer.AmplitudeOffsetDB < RFECommunicator.MIN_AMPLITUDE_DBM)
                    fAmplitudeBottom = RFECommunicator.MIN_AMPLITUDE_DBM + m_objRFEAnalyzer.AmplitudeOffsetDB;
                if (fAmplitudeBottom > (fAmplitudeTop - RFECommunicator.MIN_AMPLITUDE_RANGE_DBM))
                    fAmplitudeBottom = (fAmplitudeTop - RFECommunicator.MIN_AMPLITUDE_RANGE_DBM);

                if (fAmplitudeTop - m_objRFEAnalyzer.AmplitudeOffsetDB > RFECommunicator.MAX_AMPLITUDE_DBM)
                    fAmplitudeTop = RFECommunicator.MAX_AMPLITUDE_DBM + m_objRFEAnalyzer.AmplitudeOffsetDB;
                if (fAmplitudeTop < (fAmplitudeBottom + RFECommunicator.MIN_AMPLITUDE_RANGE_DBM))
                    fAmplitudeTop = (fAmplitudeBottom + RFECommunicator.MIN_AMPLITUDE_RANGE_DBM);

                //Convert them to back to used measurement units
                m_sAnalyzerBottomAmplitude.Text = ConvertToCurrentAmplitudeString(fAmplitudeBottom);
                m_sAnalyzerTopAmplitude.Text = ConvertToCurrentAmplitudeString(fAmplitudeTop);
            }
            catch(Exception obEx)
            {
                ReportLog(obEx.ToString(), false);
            }
        }

        private void tabSpectrumAnalyzer_Enter(object sender, EventArgs e)
        {
            DisplayGroups();
        }

        private void OnCleanReport_Click(object sender, EventArgs e)
        {
            m_ReportTextBox.Text = "Text cleared." + Environment.NewLine;
        }

        private void OnCalcAverage_CheckedChanged(object sender, EventArgs e)
        {
            menuAveragedTrace.Checked = chkCalcAverage.Checked;
            click_view_mode(null, null);
        }

        private void OnCalcMax_CheckedChanged(object sender, EventArgs e)
        {
            menuMaxTrace.Checked = chkCalcMax.Checked;
            click_view_mode(null, null);
        }

        private void OnCalcMin_CheckedChanged(object sender, EventArgs e)
        {
            menuMinTrace.Checked = chkCalcMin.Checked;
            click_view_mode(null, null);
        }

        private void OnCalcRealtime_CheckedChanged(object sender, EventArgs e)
        {
            menuRealtimeTrace.Checked = chkCalcRealtime.Checked;
            click_view_mode(null, null);
        }

        private void OnReinitializeData_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Are you sure to reinitialize data buffer?", "Reinitialize data buffer", MessageBoxButtons.YesNo))
            {
                if (m_MainTab.SelectedTab == m_tabRemoteScreen)
                {
                    m_objRFEAnalyzer.ScreenData.CleanAll();
                    m_sFilenameRFS = "";
                    numScreenIndex.Value = 0;
                    MessageBox.Show("Remote Screen buffer cleared.");
                }
                else
                {
                    CleanSweepData();
                    MessageBox.Show("Data buffer cleared.");
                }
            }
        }

        private void OnShowControlArea_Click(object sender, EventArgs e)
        {
            DisplayGroups();
        }

        private void MainForm_Layout(object sender, LayoutEventArgs e)
        {
#if CALLSTACK
            Console.WriteLine("CALLSTACK:MainForm_Layout");
#endif
            DisplayGroups();
            tabRemoteScreen_UpdateZoomValues();
        }

        private Size m_SizePriorMainTab = new Size(-1, -1);
        private void MainTab_ClientSizeChanged(object sender, EventArgs e)
        {
            //This is looking for 2 difference in width because some sort of weird behavior makes it repaint more than needed for repeated 2 pixel changes
            //for some reason this does not happen in height but in width only
            if ((m_SizePriorMainTab.Width!=m_MainTab.ClientSize.Width) || (m_SizePriorMainTab.Height != m_MainTab.ClientSize.Height))
            {
#if CALLSTACK
                Console.WriteLine("CALLSTACK:MainTab_ClientSizeChanged " + m_SizePriorMainTab.ToString() + " " + m_MainTab.ClientSize.ToString());
#endif
                //this is to fix the problem of Tabs not resizing their childs automatically
                m_MainTab.Refresh();
                m_panelWaterfall.Refresh();
                m_panelRemoteScreen.Refresh();
                m_SizePriorMainTab = m_MainTab.ClientSize;
            }
        }

        private void OnRFModuleSelectorConfig_HideControl(object sender, EventArgs e)
        {
            menuRFConnections.Checked = false;
            DisplayGroups();
        }

        bool m_bInsideDisplayGroups = false; //avoid recursive calls
        private void DisplayGroups()
        {
            if (!m_bLayoutInitialized)
                return;

            if (m_bInsideDisplayGroups)
            {
                //sanity check to avoid any potential nested call to this expensive redraw method
                #if CALLSTACK
                Console.WriteLine("CALLSTACK:DisplayGroups nested");
                #endif
                return;
            }

            m_bInsideDisplayGroups = true;
            try
            {
                #if CALLSTACK
                Console.WriteLine("CALLSTACK:DisplayGroups");
                #endif

                if (m_MainTab.Width != (Width - 16))
                {
                    //important: do not use anything smaller than 16 or may provoke unnecesary back and forth refresh
                    m_MainTab.Width = Width - 16; 
                }
                if (m_MainTab.Height != Height - 64)
                {
                    m_MainTab.Height = Height - 64;
                }

                if (m_tableLayoutControlArea.Parent != m_MainTab.SelectedTab)
                {
                    m_tableLayoutControlArea.Parent = m_MainTab.SelectedTab;
                }
                m_tableLayoutControlArea.Visible = menuShowControlArea.Checked;

                int nTop = m_tableLayoutControlArea.Top;
                if (menuShowControlArea.Checked)
                {
                    nTop = m_tableLayoutControlArea.Bottom;
                }

                int nButtonPositions = 0;

                m_groupCOMPortAnalyzer.Visible = true;
                m_groupCOMPortGenerator.Visible = false;
                m_groupControl_FreqSettings.Visible = true;
                m_groupControl_DataFeed.Visible = true;
                m_groupControl_Commands.Visible = false;
                m_groupControl_RemoteScreen.Visible = false;
                m_groupControl_RFEGen_FrequencySweep.Visible = false;
                m_groupControl_RFEGen_CW.Visible = false;
                m_groupControl_RFEGen_Tracking.Visible = false;
                m_controlWaterfall.DrawTitle = true;

                if (m_MainTab.SelectedTab == m_tabSpectrumAnalyzer)
                {
                    m_GraphSpectrumAnalyzer.Top = nTop + 5;
                    m_GraphSpectrumAnalyzer.Height = m_MainStatusBar.Top - m_GraphSpectrumAnalyzer.Top - 3;
                    m_GraphSpectrumAnalyzer.Width = Width - (int)(btnBottom5plus.Width * 1.3) - 16;
                    m_GraphSpectrumAnalyzer.Width -= (m_panelSAMarkers.Width + 8);
                    m_GraphSpectrumAnalyzer.Left = 6;

                    if (IsWaterfallOnMainScreen())
                    {
                        m_panelWaterfall.BorderStyle = BorderStyle.FixedSingle;
                        m_panelWaterfall.Parent = m_tabSpectrumAnalyzer;

                        if (menuPlaceWaterfallAtBottom.Checked)
                        {
                            m_controlWaterfall.DrawTitle = false;

                            int nSABottom = m_GraphSpectrumAnalyzer.Bottom;
                            m_GraphSpectrumAnalyzer.Height /= 2;
                            m_panelWaterfall.Left = m_GraphSpectrumAnalyzer.Left;
                            m_panelWaterfall.Width = m_GraphSpectrumAnalyzer.Width;
                            m_panelWaterfall.Top = m_GraphSpectrumAnalyzer.Bottom + 5;
                            m_panelWaterfall.Height = nSABottom - m_panelWaterfall.Top;
                        }
                        else
                        {
                            int nSAWidth = m_GraphSpectrumAnalyzer.Width;
                            m_GraphSpectrumAnalyzer.Width /= 2;
                            m_panelWaterfall.Left = m_GraphSpectrumAnalyzer.Right + 5;
                            m_panelWaterfall.Height = m_GraphSpectrumAnalyzer.Height;
                            m_panelWaterfall.Top = m_GraphSpectrumAnalyzer.Top;
                            m_panelWaterfall.Width = nSAWidth - m_panelWaterfall.Left;
                        }
                    }
                    else
                    {
                        m_panelWaterfall.Parent = m_tabWaterfall;
                    }

                    UpdateConfigControlContents();
                    UpdateMarkerControlContents();
                    nButtonPositions = m_panelSAMarkers.Right + 8;

                    if (menuRFConnections.Checked)
                    {
                        m_controlRFModuleSelectorConfig.AllowHideControl = true;
                        m_controlRFModuleSelectorConfig.RFExplorer = m_objRFEAnalyzer;
                        m_controlRFModuleSelectorConfig.SelectImageFromConfiguration();

                        int nWidth = 133;
                        if ((m_GraphSpectrumAnalyzer.Width > 800) && (m_GraphSpectrumAnalyzer.Height > 400))
                        {
                            int nExtraWidth = m_GraphSpectrumAnalyzer.Width - 800;
                            nWidth = nExtraWidth / 2;
                            if (nWidth < 133)
                                nWidth = 133;
                        }

                        m_panelRFConnections.Visible = true;
                        m_panelRFConnections.Dock = DockStyle.None;
                        m_panelRFConnections.Height = 20; //start with small size in order to grow for proper location
                        m_panelRFConnections.Width = 10;
                        m_panelRFConnections.Parent = m_tabSpectrumAnalyzer;
                        m_panelRFConnections.Height = m_GraphSpectrumAnalyzer.Height; //first do this to get the control calculate aspect ration
                        m_panelRFConnections.Width = nWidth - 10;
                        m_panelRFConnections.Height = m_controlRFModuleSelectorConfig.ActualPictureHeight; //later do this to reuse what it calculated as best height
                        m_panelRFConnections.Left = 6;
                        m_panelRFConnections.Top = m_GraphSpectrumAnalyzer.Top + (m_GraphSpectrumAnalyzer.Height - m_panelRFConnections.Height) / 2;

                        m_GraphSpectrumAnalyzer.Width -= m_controlRFModuleSelectorConfig.ActualPictureWidth;
                        m_GraphSpectrumAnalyzer.Left = m_controlRFModuleSelectorConfig.ActualPictureWidth;
                        m_GraphSpectrumAnalyzer.BringToFront();
                    }
                    else
                    {
                        m_panelRFConnections.Visible = false;
                    }
                }
                else if (m_MainTab.SelectedTab == m_tabWaterfall)
                {
                    m_panelWaterfall.Parent = m_tabWaterfall;
                    m_panelWaterfall.BorderStyle = BorderStyle.None;
                    m_panelWaterfall.Top = nTop + 5;
                    m_panelWaterfall.Height = m_MainStatusBar.Top - m_panelWaterfall.Top - 3;
                    m_panelWaterfall.Left = 6;
                    m_panelWaterfall.Width = Width - 35;
                }
                else if (m_MainTab.SelectedTab == m_tabRFGen)
                {
                    m_groupControl_FreqSettings.Visible = false;
                    m_groupControl_DataFeed.Visible = false;
                    m_groupControl_RFEGen_FrequencySweep.Visible = true;
                    m_groupControl_RFEGen_CW.Visible = true;
                    m_groupControl_RFEGen_Tracking.Visible = true;
                    m_groupCOMPortAnalyzer.Visible = false;
                    m_groupCOMPortGenerator.Visible = true;

                    m_GraphTrackingGenerator.Top = nTop + 5;
                    m_GraphTrackingGenerator.Height = m_MainStatusBar.Top - m_GraphTrackingGenerator.Top - 3;
                    m_GraphTrackingGenerator.Width = Width - 35;
                    m_GraphTrackingGenerator.Left = 6;

                    DisplayGroups_RFGen();
                }

                if (m_MainTab.SelectedTab == m_tabRemoteScreen)
                {
                    m_groupControl_RemoteScreen.Visible = true;
                    m_groupControl_DataFeed.Visible = false;
                    m_groupControl_RemoteScreen.Parent = m_tableLayoutControlArea;
                    m_groupControl_RemoteScreen.Dock = DockStyle.Top;

                    m_panelRemoteScreen.Top = nTop + 5;
                    m_panelRemoteScreen.Width = Width - 45;
                    m_panelRemoteScreen.Height = m_MainStatusBar.Top - nTop - 10;
                    m_panelRemoteScreen.BorderStyle = BorderStyle.FixedSingle;
                }

                if (m_MainTab.SelectedTab == m_tabPowerChannel)
                {
                    m_groupControl_DataFeed.Visible = true;

                    m_panelPowerChannel.Top = nTop + 5;
                    m_panelPowerChannel.Left = 6;
                    m_panelPowerChannel.Width = Width - 35;
                    m_panelPowerChannel.Height = m_MainStatusBar.Top - nTop - 10;
                    m_panelPowerChannel.BorderStyle = BorderStyle.FixedSingle;

                    double dMin=m_objRFEAnalyzer.AmplitudeBottomDBM;
                    double dMax = m_objRFEAnalyzer.AmplitudeTopDBM;

                    m_PowerChannelRegion_Low.MinValue = dMin;
                    m_PowerChannelRegion_Low.MaxValue = dMin + ((dMax - dMin) * 0.25);
                    m_PowerChannelRegion_High.MinValue = dMin + ((dMax - dMin) * 0.75);
                    m_PowerChannelRegion_High.MaxValue = dMax;

                    m_PowerChannelRegion_Medium.MinValue = m_PowerChannelRegion_Low.MaxValue;
                    m_PowerChannelRegion_Medium.MaxValue = m_PowerChannelRegion_High.MinValue;

                    m_panelPowerChannel.Left = 5;
                    //m_graphPowerChannel.Width = m_panelPowerChannel.Width - 10;
                }

                if (m_MainTab.SelectedTab == m_tabReport)
                {
                    m_groupControl_Commands.Visible = true;
                    m_groupControl_Commands.Dock = DockStyle.Top;
                    m_groupControl_Commands.Parent = m_tableLayoutControlArea;

                    m_groupControl_DataFeed.Visible = false;
                    m_groupControl_FreqSettings.Visible = false;
                    m_ReportTextBox.Top = nTop + 5;
                    m_ReportTextBox.Width = Width - 35;
                    m_ReportTextBox.Height = m_MainStatusBar.Top - nTop - 10;
                }

                if (m_MainTab.SelectedTab == m_tabConfiguration)
                {
                    m_controlRFModuleSelectorConfig.AllowHideControl = false;

                    m_panelGeneralConfigTab.Left = m_tableLayoutControlArea.Left;
                    m_panelGeneralConfigTab.Top = nTop + 5;
                    m_controlRFModuleSelectorConfig.RFExplorer = m_objRFEAnalyzer;
                    m_controlRFModuleSelectorConfig.SelectImageFromConfiguration();

                    m_panelRFConnections.Visible = true;
                    m_panelRFConnections.Height = 20; //start with small size in order to grow for proper location
                    m_panelRFConnections.Width = 20;
                    m_panelRFConnections.Parent = m_tableConfiguration;
                    m_panelRFConnections.Dock = DockStyle.Fill;

                    m_groupControl_DataFeed.Visible = false;
                    m_groupControl_FreqSettings.Visible = false;
                }

                m_GraphSpectrumAnalyzer.Visible = true;
                RelocateRemoteControl();

                btnAutoscale.Top = m_GraphSpectrumAnalyzer.Top;
                btnAutoscale.Left = nButtonPositions;
                if (m_arrAnalyzerButtonList[0] != null)
                {
                    btnAutoscale.Visible = true;
                    for (int nInd = 1; nInd < m_arrAnalyzerButtonList.Length; nInd++)
                    {
                        m_arrAnalyzerButtonList[nInd].Top = m_arrAnalyzerButtonList[nInd - 1].Bottom + 3;
                        m_arrAnalyzerButtonList[nInd].Left = m_arrAnalyzerButtonList[0].Left;
                        m_arrAnalyzerButtonList[nInd].Visible = true;
                    }
                }
            }
            catch (Exception obEx)
            {
                ReportLog(obEx.ToString(), false);
            }
            m_bInsideDisplayGroups = false;
        }

        private bool IsWaterfallOnMainScreen()
        {
            return menuPlaceWaterfallAtBottom.Checked || menuPlaceWaterfallOnTheRight.Checked;
        }

        private void OnPlaceWaterfallAtBottom_Click(object sender, EventArgs e)
        {
            menuPlaceWaterfallNone.Checked = false;
            menuPlaceWaterfallOnTheRight.Checked = false;
            menuPlaceWaterfallAtBottom.Checked = true;
            if (m_MainTab.SelectedTab == m_tabSpectrumAnalyzer)
            {
                DisplayGroups();
                UpdateWaterfall();
            }
        }

        private void OnPlaceWaterfallOnTheRight_Click(object sender, EventArgs e)
        {
            menuPlaceWaterfallNone.Checked = false;
            menuPlaceWaterfallAtBottom.Checked = false;
            menuPlaceWaterfallOnTheRight.Checked = true;
            if (m_MainTab.SelectedTab == m_tabSpectrumAnalyzer)
            {
                DisplayGroups();
                UpdateWaterfall();
            }
        }

        private void OnWaterfallPlaceNone_Click(object sender, EventArgs e)
        {
            menuPlaceWaterfallOnTheRight.Checked = false;
            menuPlaceWaterfallAtBottom.Checked = false;
            menuPlaceWaterfallNone.Checked = true;
            if (m_MainTab.SelectedTab == m_tabSpectrumAnalyzer)
            {
                DisplayGroups();
            }
        }

        private void OnDarkMode_Click(object sender, EventArgs e)
        {
            DefineGraphColors();
            Invalidate();
            if ((m_MainTab.SelectedTab == m_tabWaterfall) || IsWaterfallOnMainScreen())
            {
                UpdateWaterfall();
            }
        }

        /// <summary>
        /// based on a predefined hierarchy, it will select the <param name="listCurrentPointList"> and
        /// will tell how many curves are displayed.
        /// </summary>
        /// <param name="listCurrentPointList"></param>
        /// <returns></returns>
        private int SelectSinglePointPairList(ref PointPairList listCurrentPointList)
        {
            int nSelectionCounter = 0;
            if (menuAveragedTrace.Checked)
            {
                nSelectionCounter++;
                listCurrentPointList = m_PointList_Avg;
            }
            if (menuMaxHoldTrace.Checked)
            {
                nSelectionCounter++;
                listCurrentPointList = m_PointList_MaxHold;
            }
            if (menuMaxTrace.Checked)
            {
                listCurrentPointList = m_PointList_Max;
                nSelectionCounter++;
            }
            if (menuMinTrace.Checked)
            {
                nSelectionCounter++;
                listCurrentPointList = m_PointList_Min;
            }
            if (menuRealtimeTrace.Checked)
            {
                listCurrentPointList = m_PointList_Realtime;
                nSelectionCounter++;
            }
            return nSelectionCounter;
        }

        private void OnShowAxisLabels_Click(object sender, EventArgs e)
        {
            m_GraphSpectrumAnalyzer.GraphPane.XAxis.Title.IsVisible = menuShowAxisLabels.Checked;
            m_GraphSpectrumAnalyzer.GraphPane.YAxis.Title.IsVisible = menuShowAxisLabels.Checked;
            m_GraphSpectrumAnalyzer.Refresh();
        }

        private void OnItemAmplitudeUnit_Click(object sender, EventArgs e)
        {
            menuItemDBM.Checked = (sender.ToString() == menuItemDBM.Text);
            menuItemDBUV.Checked = (sender.ToString() == menuItemDBUV.Text);
            menuItemWatt.Checked = (sender.ToString() == menuItemWatt.Text);

            //rescale limit lines according to new units
            m_LimitLineOverload.AmplitudeUnits = GetCurrentAmplitudeEnum();
            m_LimitLineMin.AmplitudeUnits = GetCurrentAmplitudeEnum();
            m_LimitLineMax.AmplitudeUnits = GetCurrentAmplitudeEnum();

            UpdateDialogFromFreqSettings();
            UpdateYAxis();
            DisplaySpectrumAnalyzerData();
            //btnAutoscale_Click(null, null);
        }

        private void PlayNotificationSound()
        {
            try
            {
                if (!m_bSoundPlaying)
                {
                    m_SoundPlayer.PlayLooping();
                    m_bSoundPlaying = true;
                }
            }
            catch (Exception obEx)
            {
                MessageBox.Show(obEx.Message);
                ReportLog(obEx.ToString(), false);
            }
        }

        private void StopNotificationSound()
        {
            try
            {
                if (m_bSoundPlaying)
                {
                    m_SoundPlayer.Stop();
                    m_bSoundPlaying = false;
                }
            }
            catch (Exception obEx)
            {
                MessageBox.Show(obEx.Message);
                ReportLog(obEx.ToString(), false);
            }
        }

        private void OnThickTrace_Click(object sender, EventArgs e)
        {
            if (m_objRFEAnalyzer.HoldMode)
                DisplaySpectrumAnalyzerData();
        }

        private void OnShowGrid_Click(object sender, EventArgs e)
        {
            if (m_objRFEAnalyzer.HoldMode)
                DisplaySpectrumAnalyzerData();
        }
        #endregion

        #region Waterfall

        private void tabWaterfall_Enter(object sender, EventArgs e)
        {
            DisplayGroups();
            UpdateWaterfall();
        }

        /// <summary>
        /// Creates a max hold data from what is in the RFE object. This is useful when reconstructing from file or buffer
        /// because when receiving realtime this is done more efficiently on updata data callback
        /// </summary>
        private void CreateMaxHoldWaterfallData()
        {
            m_WaterfallSweepMaxHold.CleanAll();

            if ((m_objRFEAnalyzer.SweepData == null) || (m_objRFEAnalyzer.SweepData.Count < 5))
            {
                return;
            }

            uint nTotalSweepSteps = m_objRFEAnalyzer.SweepData.GetData(0).TotalSteps;

            uint nStartPos, nTotalSteps;
            if (m_objRFEAnalyzer.SweepData.Count < 100)
            {
                nStartPos = 0;
                nTotalSteps = m_objRFEAnalyzer.SweepData.Count;
            }
            else
            {
                nStartPos = m_objRFEAnalyzer.SweepData.Count - 100;
                nTotalSteps = 100;
            }

            for (UInt16 nMaxHoldInd = 0; nMaxHoldInd < nTotalSteps; nMaxHoldInd++)
            {
                m_WaterfallSweepMaxHold.Add(new RFESweepData(m_objRFEAnalyzer.SweepData.GetData(0).StartFrequencyMHZ,
                    m_objRFEAnalyzer.SweepData.GetData(0).StepFrequencyMHZ, m_objRFEAnalyzer.SweepData.GetData(0).TotalSteps));
                for (UInt16 nSourceInd = 0; nSourceInd <= nStartPos; nSourceInd++)
                {
                    for (UInt16 nSweepDataInd = 0; nSweepDataInd < nTotalSweepSteps; nSweepDataInd++)
                    {
                        if (m_objRFEAnalyzer.SweepData.GetData(nSourceInd).GetAmplitudeDBM(nSweepDataInd,null,false) > m_WaterfallSweepMaxHold.GetData(nMaxHoldInd).GetAmplitudeDBM(nSweepDataInd,null,false))
                        {
                            m_WaterfallSweepMaxHold.GetData(nMaxHoldInd).SetAmplitudeDBM(nSweepDataInd, m_objRFEAnalyzer.SweepData.GetData(nSourceInd).GetAmplitudeDBM(nSweepDataInd,null,false));
                        }
                    }
                }
                nStartPos++;
            }
        }

        /// <summary>
        /// Main drawing / iteration function for Waterfall display
        /// </summary>
        private void UpdateWaterfall()
        {
            m_controlWaterfall.DarkMode = menuDarkMode.Checked;

            if (m_objRFEAnalyzer.SweepData.Count == 0)
                return; //nothing to paint

            UInt32 nSourceSweepIndex = 0;  //Index to position last source sample to use (first sample to draw in front)
            UInt32 nTargetSweepIndex = 0;  //Index to position first target sample to use (first sample to draw in front)

            switch (m_eWaterfallSignal)
            {
                case RFExplorerSignalType.MaxHold:
                    {
                        //Check if data is available but not in the waterfall collection (e.g. was read from a file, etc)
                        if ((m_WaterfallSweepMaxHold.Count == 0) && (m_objRFEAnalyzer.SweepData.MaxHoldData != null))
                        {
                            CreateMaxHoldWaterfallData();
                        }

                        //Get the index to use for painting, we always paint most recent in front, so get the latest value available
                        if (m_WaterfallSweepMaxHold.Count == 0)
                            return; //nothing to paint!

                        nSourceSweepIndex = m_WaterfallSweepMaxHold.Count - 1;
                    }
                    break;
                case RFExplorerSignalType.Realtime:
                    {
                        nSourceSweepIndex = (UInt32)numericSampleSA.Value - 1;
                    }
                    break;
            }

            //Set range to the waterfall control
            m_controlWaterfall.InitSpectrumRange(m_objRFEAnalyzer.StartFrequencyMHZ, m_objRFEAnalyzer.StopFrequencyMHZ, m_objRFEAnalyzer.AmplitudeBottomDBM, m_objRFEAnalyzer.AmplitudeTopDBM);

            //We will use the "iterations" factor as the number of sweeps to use for the Z axis
            UInt32 nTotalCalculatorIterations = RFEWaterfallGL.SharpGLForm.TotalDrawingSweeps; //for the moment we will hardcode to 100, we can change this later on
            if (nTotalCalculatorIterations > nSourceSweepIndex)
                nTotalCalculatorIterations = nSourceSweepIndex;

            // now we populate
            for (int nSweepIterator = (int)nTotalCalculatorIterations; nSweepIterator > 0; nSweepIterator--, nSourceSweepIndex--)
            {
                //for each sweep, we get the amplitude values (from left to right)
                RFESweepData objSweep = null;

                switch (m_eWaterfallSignal)
                {
                    case RFExplorerSignalType.MaxHold:
                        {
                            objSweep = m_WaterfallSweepMaxHold.GetData(nSourceSweepIndex);
                            nTargetSweepIndex = (uint)(nTotalCalculatorIterations - nSourceSweepIndex);
                        }
                        break;
                    case RFExplorerSignalType.Realtime:
                        {
                            objSweep = m_objRFEAnalyzer.SweepData.GetData(nSourceSweepIndex);
                            nTargetSweepIndex = (uint)(nTotalCalculatorIterations - nSweepIterator);
                        }
                        break;
                }

                //build one full sweep data entry
                if (objSweep != null)
                {
                    m_controlWaterfall.AddTimeStamp(nTargetSweepIndex, objSweep.CaptureTime);
                    for (UInt16 nStep = 0; nStep < objSweep.TotalSteps; nStep++)
                    {
                        double fAmplitudeDBM = objSweep.GetAmplitudeDBM(nStep, m_objRFEAnalyzer.m_AmplitudeCalibration, menuUseAmplitudeCorrection.Checked);
                        double fFrequencyMHZ = objSweep.GetFrequencyMHZ(nStep);

                        m_controlWaterfall.AddValue(nStep, nTargetSweepIndex, fAmplitudeDBM);

                        //This is for debug only, it writes all the data points in the "Report" window. It takes a lot to print a large data set so use with care
                        //ReportLog("[x,y,z]=[" +  fFrequencyMHZ + "," + fAmplitudeDBM + "," + nSourceSweepIndex + "]",false);
                        //Console.WriteLine("[x,y,z]=[{0},{1},{2}]", fFrequencyMHZ, fAmplitudeDBM, nSourceSweepIndex);
                    }
                }
            }
        }

        private void OnWaterfallContextRealtime_Click(object sender, EventArgs e)
        {
            m_eWaterfallSignal = RFExplorerSignalType.Realtime;
            UpdateAllWaterfallMenuItems();
            UpdateWaterfall();
            m_controlWaterfall.Invalidate();
        }

        private void OnWaterfallContextMaxHold_Click(object sender, EventArgs e)
        {
            m_eWaterfallSignal = RFExplorerSignalType.MaxHold;
            UpdateAllWaterfallMenuItems();
            UpdateWaterfall();
            m_controlWaterfall.Invalidate();
        }

        private void OnTransparentWaterfall_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem objMenu = (ToolStripMenuItem)sender;
            m_controlWaterfall.Transparent = objMenu.Checked;
            m_controlWaterfall.Invalidate();
            UpdateAllWaterfallMenuItems();
        }

        private void OnWaterfallFloor_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem objMenu = (ToolStripMenuItem)sender;
            m_controlWaterfall.DrawFloor = objMenu.Checked;
            m_controlWaterfall.Invalidate();
            UpdateAllWaterfallMenuItems();
        }

        private void UpdateAllWaterfallMenuItems()
        {
            menuWaterfallContextPerspective1.Checked = false;
            menuWaterfallContextPerspective2.Checked = false;
            menuWaterfallContextISO.Checked = false;
            menuWaterfallContext2D.Checked = false;
            menuWaterfallPerspective1.Checked = false;
            menuWaterfallPerspective2.Checked = false;
            menuWaterfallPerspective2D.Checked = false;
            menuWaterfallPerspectiveIso.Checked = false;
            switch (m_controlWaterfall.PerspectiveView)
            {
                case RFEWaterfallGL.SharpGLForm.WaterfallPerspectives.Perspective1:
                    menuWaterfallContextPerspective1.Checked = true;
                    menuWaterfallPerspective1.Checked = true;
                    break;
                case RFEWaterfallGL.SharpGLForm.WaterfallPerspectives.Perspective2:
                    menuWaterfallContextPerspective2.Checked = true;
                    menuWaterfallPerspective2.Checked = true;
                    break;
                case RFEWaterfallGL.SharpGLForm.WaterfallPerspectives.ISO:
                    menuWaterfallContextISO.Checked = true;
                    menuWaterfallPerspectiveIso.Checked = true;
                    break;
                case RFEWaterfallGL.SharpGLForm.WaterfallPerspectives.Pers_2D:
                    menuWaterfallContext2D.Checked = true;
                    menuWaterfallPerspective2D.Checked = true;
                    break;
            }

            menuWaterfallContextRealtime.Checked = false;
            menuWaterfallContextMaxHold.Checked = false;
            switch (m_eWaterfallSignal)
            {
                case RFExplorerSignalType.Realtime:
                    menuWaterfallContextRealtime.Checked = true;
                    break;
                case RFExplorerSignalType.MaxHold:
                    menuWaterfallContextMaxHold.Checked = true;
                    break;
                case RFExplorerSignalType.Average:
                    break;
                case RFExplorerSignalType.MaxPeak:
                    break;
                case RFExplorerSignalType.Min:
                    break;
            }

            menuWaterfallContextFloor.Checked = m_controlWaterfall.DrawFloor;
            menuWaterfallContextTransparent.Checked = m_controlWaterfall.Transparent;
        }

        private void OnWaterfallPerspective1_Click(object sender, EventArgs e)
        {
            m_controlWaterfall.Perspective1();
            UpdateAllWaterfallMenuItems();
        }

        private void OnWaterfallPerspective2_Click(object sender, EventArgs e)
        {
            m_controlWaterfall.Perspective2();
            UpdateAllWaterfallMenuItems();
        }

        private void OnWaterfallIsometric_Click(object sender, EventArgs e)
        {
            m_controlWaterfall.ISO();
            UpdateAllWaterfallMenuItems();
        }

        private void OnWaterfall2D_Click(object sender, EventArgs e)
        {
            m_controlWaterfall.Pers_2D();
            UpdateAllWaterfallMenuItems();
        }
        #endregion

        #region Power Channel
        private void tabPowerChannel_Enter(object sender, EventArgs e)
        {
            if (m_graphPowerChannel == null)
            {
                m_graphPowerChannel = new ZedGraphControl();
                m_panelPowerChannel.Controls.Add(m_graphPowerChannel);
                m_graphPowerChannel.EditButtons = System.Windows.Forms.MouseButtons.Left;
                m_graphPowerChannel.IsAntiAlias = true;
                m_graphPowerChannel.IsEnableSelection = false;
                m_graphPowerChannel.Location = new System.Drawing.Point(5, 5);
                m_graphPowerChannel.Name = "zedPowerChannel";
                m_graphPowerChannel.ScrollGrace = 0D;
                m_graphPowerChannel.ScrollMaxX = 0D;
                m_graphPowerChannel.ScrollMaxY = 0D;
                m_graphPowerChannel.ScrollMaxY2 = 0D;
                m_graphPowerChannel.ScrollMinX = 0D;
                m_graphPowerChannel.ScrollMinY = 0D;
                m_graphPowerChannel.ScrollMinY2 = 0D;
                m_graphPowerChannel.Size = new System.Drawing.Size(600, 300);
                m_graphPowerChannel.TabIndex = 49;
                m_graphPowerChannel.TabStop = false;
                m_graphPowerChannel.UseExtendedPrintDialog = true;
                m_graphPowerChannel.Visible = true;
                m_graphPowerChannel.GraphPane.Title.Text = "RF Explorer Channel Power Meter";
                m_graphPowerChannel.GraphPane.Title.FontSpec.Size = 18f;
                m_graphPowerChannel.GraphPane.TitleGap = 6;
                m_graphPowerChannel.Dock = DockStyle.Fill;
                m_graphPowerChannel.ContextMenuBuilder += new ZedGraph.ZedGraphControl.ContextMenuBuilderEventHandler(this.objGraphPowerChannel_ContextMenuBuilder);
                //m_graphSpectrumAnalyzer.ZoomEvent += new ZedGraph.ZedGraphControl.ZoomEventHandler(this.zedSpectrumAnalyzer_ZoomEvent);

                m_graphPowerChannel.GraphPane.XAxis.IsVisible = false;
                m_graphPowerChannel.GraphPane.Y2Axis.IsVisible = false;
                m_graphPowerChannel.GraphPane.YAxis.IsVisible = false;
                m_graphPowerChannel.GraphPane.Border.IsVisible = false;

                //Define needles; can add more than one
                m_PowerChannelNeedle = new GasGaugeNeedle("Realtime", -30.0f, Color.Blue);
                m_PowerChannelNeedle.NeedleWidth = 10f;
                m_PowerChannelNeedle.NeedleColor = Color.Blue;
                m_PowerChannelNeedle.Label.IsVisible = false;
                m_PowerChannelNeedle.LineCap = LineCap.ArrowAnchor;
                m_graphPowerChannel.GraphPane.CurveList.Add(m_PowerChannelNeedle);

                //Define all regions
                m_PowerChannelRegion_Low = new GasGaugeRegion("Low", -120.0f, -90.0f, Color.Blue);
                m_PowerChannelRegion_Low.HasLabel = true;
                m_PowerChannelRegion_Medium = new GasGaugeRegion("Medium", -90.0f, -30.0f, Color.LightBlue);
                m_PowerChannelRegion_High = new GasGaugeRegion("High", -30.0f, 0.0f, Color.Red);
                m_PowerChannelRegion_High.HasLabel = true;
                m_PowerChannelRegion_Low.Label.IsVisible = false;
                m_PowerChannelRegion_Medium.Label.IsVisible = false;
                m_PowerChannelRegion_High.Label.IsVisible = false;

                //not working as intended
                //m_PowerChannelRegion_Low.RegionColorStart = Color.LightGreen;
                //m_PowerChannelRegion_Low.RegionColorEnd = Color.LightBlue;
                //m_PowerChannelRegion_Medium.RegionColorStart = Color.LightBlue;
                //m_PowerChannelRegion_Medium.RegionColorEnd = Color.Blue;
                //m_PowerChannelRegion_High.RegionColorStart = Color.Blue;
                //m_PowerChannelRegion_High.RegionColorEnd = Color.Red;

                m_PowerChannelText = new TextObj("No data available", 0.5, 0.25, CoordType.PaneFraction);
                m_PowerChannelText.IsClippedToChartRect = false;
                m_PowerChannelText.FontSpec.FontColor = Color.DarkBlue;
                m_PowerChannelText.Location.AlignH = AlignH.Center;
                m_PowerChannelText.Location.AlignV = AlignV.Center;
                m_PowerChannelText.FontSpec.IsBold = false;
                m_PowerChannelText.FontSpec.Size = 16f;
                m_PowerChannelText.FontSpec.Border.IsVisible = false;
                m_PowerChannelText.FontSpec.Fill.IsVisible = false;
                m_PowerChannelText.FontSpec.StringAlignment = StringAlignment.Center;
                m_PowerChannelText.FontSpec.Family = "Arial";
                m_graphPowerChannel.GraphPane.GraphObjList.Add(m_PowerChannelText);

                // Add the curves
                m_graphPowerChannel.GraphPane.CurveList.Add(m_PowerChannelRegion_Low);
                m_graphPowerChannel.GraphPane.CurveList.Add(m_PowerChannelRegion_Medium);
                m_graphPowerChannel.GraphPane.CurveList.Add(m_PowerChannelRegion_High);
                m_graphPowerChannel.GraphPane.Angle = 150;
                m_graphPowerChannel.GraphPane.GasGaugeRegionWidth = 25;
                m_graphPowerChannel.GraphPane.GasGaugeBorder = false;
                m_graphPowerChannel.GraphPane.ShowGasGaugeValueLabel = false; //needs better code to paint value + unit
                m_graphPowerChannel.GraphPane.HasLabel = false; //needs better code to paint values
                m_graphPowerChannel.GraphPane.Border.IsVisible = false;
                m_graphPowerChannel.GraphPane.Chart.Border.IsVisible = false;
                m_graphPowerChannel.GraphPane.Clockwise = true;
                m_graphPowerChannel.AxisChange();
            }

            DisplayGroups();
            m_tabPowerChannel.Invalidate();
        }

        private void objGraphPowerChannel_ContextMenuBuilder(ZedGraphControl sender, ContextMenuStrip menuStrip, Point mousePt, ZedGraphControl.ContextMenuObjectState objState)
        {
            menuStrip.Items["save_as"].Click += new System.EventHandler(OnSaveImage_Click);
            menuStrip.Items["copy"].Click += new System.EventHandler(OnClipboard_Click);
            menuStrip.Items["show_val"].Visible = false;
            menuStrip.Items["unzoom"].Visible = false;
            menuStrip.Items["undo_all"].Visible = false;
        }

        #endregion

        #region Remote screen

        private void tabRemoteScreen_Enter(object sender, EventArgs e)
        {
            chkDumpLCDGrid.Checked = controlRemoteScreen.LCDGrid;
            chkDumpColorScreen.Checked = controlRemoteScreen.LCDColor;
            chkDumpHeader.Checked = controlRemoteScreen.HeaderText;
            UpdateButtonStatus();
            DisplayGroups();
            tabRemoteScreen_UpdateZoomValues();
            m_tabRemoteScreen.Invalidate();
        }

        private RFECommunicator GetConnectedDeviceByPrecedence()
        {
            RFECommunicator objRFE = null;

            if (m_objRFEAnalyzer!=null && m_objRFEAnalyzer.PortConnected)
            {
                objRFE = m_objRFEAnalyzer;
            }
            else if (m_objRFEGenerator!=null && m_objRFEGenerator.PortConnected)
            {
                objRFE = m_objRFEGenerator;
            }

            return objRFE;
        }

        private void numScreenIndex_ValueChanged(object sender, EventArgs e)
        {
            RFECommunicator objRFE = GetConnectedDeviceByPrecedence();
            if (objRFE == null)
                return; //no device is connected

            objRFE.ScreenIndex = (UInt16)numScreenIndex.Value;
            numScreenIndex.Value = objRFE.ScreenIndex;
            if (objRFE.ScreenData.Count > 0)
            {
                controlRemoteScreen.Invalidate();
            }
        }

        private void tabRemoteScreen_UpdateZoomValues()
        {
            int nHeaderSize = 0;
            if (chkDumpHeader.Checked)
            {
                nHeaderSize = 20;
            }

            int nNewZoom = (int)numericZoom.Value;
            int nLastGoodZoom = nNewZoom;

            do
            {
                nLastGoodZoom = nNewZoom;
                controlRemoteScreen.Size = new Size((int)(1.0 + m_fSizeX * (float)(nNewZoom)), (int)(1.0 + (m_fSizeY + nHeaderSize) * (float)(nNewZoom)));
                nNewZoom--;
            }
            while (((controlRemoteScreen.Size.Width > m_panelRemoteScreen.Size.Width) ||
                    (controlRemoteScreen.Size.Height > m_panelRemoteScreen.Size.Height))
                  && (nNewZoom > 1));

            if ((nLastGoodZoom > 0) && (nLastGoodZoom != (int)numericZoom.Value))
            {
                numericZoom.Value = nLastGoodZoom;
            }
            else
            {

                controlRemoteScreen.UpdateZoom((int)(numericZoom.Value));

                labelDumpBitmapSize.Text = "Size:" + (controlRemoteScreen.Width - 2) + "x" + (controlRemoteScreen.Height - 2);

                RelocateRemoteControl();
                controlRemoteScreen.Invalidate();
            }
            m_panelRemoteScreen.Refresh();
        }

        private void numericZoom_ValueChanged(object sender, EventArgs e)
        {
            tabRemoteScreen_UpdateZoomValues();
            m_tabRemoteScreen.Invalidate();
        }

        private void chkDumpScreen_CheckedChanged(object sender, EventArgs e)
        {
            RFECommunicator objRFE = GetConnectedDeviceByPrecedence();
            if (objRFE == null)
                return; //no device is connected

            controlRemoteScreen.RFExplorer = objRFE;
            objRFE.CaptureRemoteScreen = chkDumpScreen.Checked;
            UpdateButtonStatus();
            if (chkDumpScreen.Checked)
            {
                objRFE.SendCommand_EnableScreenDump();
            }
            else
            {
                objRFE.SendCommand_DisableScreenDump();
            }
        }

        private void chkDumpColorScreen_CheckedChanged(object sender, EventArgs e)
        {
            controlRemoteScreen.LCDColor = chkDumpColorScreen.Checked;
            controlRemoteScreen.Invalidate();
        }

        private void chkDumpHeader_CheckedChanged(object sender, EventArgs e)
        {
            controlRemoteScreen.HeaderText = chkDumpHeader.Checked;
            tabRemoteScreen_UpdateZoomValues();
        }

        private void chkDumpLCDGrid_CheckedChanged(object sender, EventArgs e)
        {
            controlRemoteScreen.LCDGrid = chkDumpLCDGrid.Checked;
            controlRemoteScreen.Invalidate();
        }

        private void SavePNG(string sFilename)
        {
            //if no file path was explicited, add the default folder
            if (!String.IsNullOrEmpty(sFilename) && (sFilename.IndexOf("\\") < 0))
            {
                sFilename = m_sDefaultDataFolder + "\\" + sFilename;
                sFilename = sFilename.Replace("\\\\", "\\");
            }

            Rectangle rectArea;
            Control controlArea;

            if (m_MainTab.SelectedTab == m_tabRemoteScreen)
            {
                rectArea = controlRemoteScreen.ClientRectangle;
                controlArea = controlRemoteScreen;
            }
            else if (m_MainTab.SelectedTab == m_tabWaterfall)
            {
                rectArea = m_panelWaterfall.ClientRectangle;
                controlArea = m_panelWaterfall;
            }
            else if (m_MainTab.SelectedTab == m_tabSpectrumAnalyzer)
            {
                rectArea = new Rectangle(0, 0,
                    m_tabSpectrumAnalyzer.ClientRectangle.Width - (m_tabSpectrumAnalyzer.ClientRectangle.Right-btnAutoscale.Left),
                    m_tabSpectrumAnalyzer.ClientRectangle.Height - m_MainStatusBar.Height -2);
                controlArea = m_tabSpectrumAnalyzer;
            }
            else if (m_MainTab.SelectedTab == m_tabPowerChannel)
            {
                rectArea = m_graphPowerChannel.ClientRectangle;
                controlArea = m_graphPowerChannel;
            }
            else
            {
                return;
            }

            using (Bitmap objAppBmp = new Bitmap(rectArea.Width, rectArea.Height))
            {
                controlArea.DrawToBitmap(objAppBmp, rectArea);

                if (String.IsNullOrEmpty(sFilename))
                {
                    if (m_ClipboardBitmap == null)
                    {
                        m_ClipboardBitmap = new Bitmap(rectArea.Width, rectArea.Height);
                        Graphics.FromImage(m_ClipboardBitmap).DrawImage(objAppBmp, rectArea);
                        ReportLog("Image sent to clipboard: " + rectArea.Width + "x" + rectArea.Height + " pixels", false);
                    }
                    else
                    {
                        ReportLog("Unable to capture clipboard: the resource was still in use", false);
                    }
                }
                else
                {
                    using (Bitmap objImage = new Bitmap(rectArea.Width, rectArea.Height))
                    {
                        Graphics.FromImage(objImage).DrawImage(objAppBmp, rectArea);

                        objImage.Save(sFilename, ImageFormat.Png);
                    }
                }
            }
        }

        private void OnSaveImage_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog MySaveFileDialog = new SaveFileDialog())
                {
                    MySaveFileDialog.Filter = _PNG_File_Selector;
                    MySaveFileDialog.FilterIndex = 1;
                    MySaveFileDialog.RestoreDirectory = false;
                    MySaveFileDialog.InitialDirectory = m_sDefaultDataFolder;

                    if (m_MainTab.SelectedTab == m_tabSpectrumAnalyzer)
                    {
                        MySaveFileDialog.FileName = GetNewFilename(RFExplorerFileType.AnalyzerScreenshotFile);
                    }
                    else if (m_MainTab.SelectedTab == m_tabWaterfall)
                    {
                        MySaveFileDialog.FileName = GetNewFilename(RFExplorerFileType.WaterfallScreenshotFile);
                    }
                    else if (m_MainTab.SelectedTab == m_tabRemoteScreen)
                    {
                        MySaveFileDialog.FileName = GetNewFilename(RFExplorerFileType.RemoteScreenFile);
                    }
                    else if (m_MainTab.SelectedTab == m_tabPowerChannel)
                    {
                        MySaveFileDialog.FileName = GetNewFilename(RFExplorerFileType.PowerChannelScreenshotFile);
                    }

                    if (MySaveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        SavePNG(MySaveFileDialog.FileName);
                    }
                }
            }
            catch (Exception obEx) { MessageBox.Show(obEx.Message); }
        }

        private void SaveFileRFS(string sFilename)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                //if no file path was explicited, add the default folder
                if (sFilename.IndexOf("\\") < 0)
                {
                    sFilename = m_sDefaultDataFolder + "\\" + sFilename;
                    sFilename = sFilename.Replace("\\\\", "\\");
                }

                if (m_objRFEAnalyzer.ScreenData.SaveFile(sFilename))
                {
                    ReportLog("File " + sFilename + " loaded with total of " + m_objRFEAnalyzer.ScreenData.Count + " screen shots.", false);
                }
                else
                {
                    MessageBox.Show("Wrong or unknown file format");
                }
            }
            catch (Exception obEx) { MessageBox.Show(obEx.Message); }
            Cursor.Current = Cursors.Default;
        }

        private void LoadFileRFS(string sFilename)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                if (m_objRFEAnalyzer.ScreenData.LoadFile(sFilename))
                {
                    UpdateScreenNumericControls();
                    UpdateButtonStatus();

                    ReportLog("File " + sFilename + " saved with total of " + m_objRFEAnalyzer.ScreenData.Count + " screen shots.", false);
                }
                else
                {
                    MessageBox.Show("Error saving to file");
                }
            }
            catch (Exception obEx) { MessageBox.Show(obEx.Message); }
            Cursor.Current = Cursors.Default;
        }

        private void OnSaveRFS_Click(object sender, EventArgs e)
        {
            if (m_objRFEAnalyzer.ScreenData.Count > 0)
            {
                try
                {
                    using (SaveFileDialog MySaveFileDialog = new SaveFileDialog())
                    {
                        MySaveFileDialog.Filter = _RFS_File_Selector;
                        MySaveFileDialog.FilterIndex = 1;
                        MySaveFileDialog.RestoreDirectory = false;
                        MySaveFileDialog.InitialDirectory = m_sDefaultDataFolder;

                        GetNewFilename(RFExplorerFileType.RemoteScreenRFSFile);
                        MySaveFileDialog.FileName = m_sFilenameRFS;

                        if (MySaveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            SaveFileRFS(MySaveFileDialog.FileName);
                        }
                    }
                }
                catch (Exception obEx) { MessageBox.Show(obEx.Message); }
            }
        }

        private void OnLoadRFS_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog MyOpenFileDialog = new OpenFileDialog())
            {
                MyOpenFileDialog.Filter = _RFS_File_Selector;
                MyOpenFileDialog.FilterIndex = 1;
                MyOpenFileDialog.RestoreDirectory = false;
                MyOpenFileDialog.InitialDirectory = m_sDefaultDataFolder;

                if (MyOpenFileDialog.ShowDialog() == DialogResult.OK)
                {
                    LoadFileRFS(MyOpenFileDialog.FileName);
                }
            }
        }

        private void controlRemoteScreen_Load(object sender, EventArgs e)
        {
            RFECommunicator objRFE = GetConnectedDeviceByPrecedence();
            if (objRFE == null)
                return; //no device is connected

            controlRemoteScreen.RFExplorer = objRFE;
        }

        private void UpdateScreenNumericControls()
        {
            RFECommunicator objRFE = GetConnectedDeviceByPrecedence();
            if (objRFE == null)
                return; //no device is connected

            //update screen data
            if (objRFE.ScreenData.Count < numScreenIndex.Value)
            {
                numScreenIndex.Value = objRFE.ScreenData.Count;
            }
            numScreenIndex.Maximum = objRFE.ScreenData.Count - 1;
            numScreenIndex.Value = objRFE.ScreenData.Count - 1;
        }

        private void RelocateRemoteControl()
        {
            controlRemoteScreen.SuspendLayout();
            controlRemoteScreen.Top = (m_panelRemoteScreen.Height - controlRemoteScreen.Height) / 2;
            controlRemoteScreen.Left = (m_panelRemoteScreen.Width - controlRemoteScreen.Width) / 2;
            m_MainStatusBar.Parent = m_tableLayoutControlArea.Parent;
            controlRemoteScreen.ResumeLayout();
        }
        #endregion

        #region Report Window

        private void tabReport_Enter(object sender, EventArgs e)
        {
            DisplayGroups();
        }

        private void ReportLog(string sLine, bool bDetailed)
        {
            if (m_ReportTextBox.IsDisposed || m_ReportTextBox.Disposing)
                return;

            DefineCommonFiles();

            if (String.IsNullOrEmpty(m_sReportFilePath))
            {
                m_sReportFilePath = m_sAppDataFolder + "\\RFExplorerClient_report_" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                m_sReportFilePath = m_sReportFilePath.Replace("\\\\", "\\");

                labelReportFile.Text = "Report file: " + m_sReportFilePath;

                m_ReportTextBox.AppendText("Welcome to RFExplorer Client - report being saved to: " + Environment.NewLine + m_sReportFilePath + Environment.NewLine);
            }
            else
                sLine = Environment.NewLine + sLine;

            if (m_chkDebugTraces.Checked || !bDetailed)
            {
                m_ReportTextBox.AppendText(sLine);
            }

            using (StreamWriter sr = new StreamWriter(m_sReportFilePath, true))
            {
                if (m_bFirstText)
                {
                    sr.WriteLine(Environment.NewLine + Environment.NewLine +
                        "===========================================");
                    sr.WriteLine(
                        "RFExplorer client session " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                    sr.WriteLine(
                        "===========================================" + Environment.NewLine);

                    sr.WriteLine("OS:         " + Environment.OSVersion.ToString());
                    sr.WriteLine("Runtime:    " + Environment.Version.ToString());
                    sr.WriteLine("Assembly:   " + Assembly.GetExecutingAssembly().ToString());
                    sr.WriteLine("File:       " + Assembly.GetExecutingAssembly().Location);
                    sr.WriteLine("");
                }
                sr.Write(sLine);
            }

            m_bFirstText = false;
        }

        private void OnSendCustomCmd_Click(object sender, EventArgs e)
        {
            string sCmd = comboCustomCommand.Text;
            if (sCmd.Length > 0)
            {
                if (!RFExplorerClient.Properties.Settings.Default.CustomCommandList.Contains(sCmd))
                {
                    RFExplorerClient.Properties.Settings.Default.CustomCommandList += ";" + sCmd.Replace(';','-');
                    comboCustomCommand.Items.Add(sCmd);
                    comboCustomCommand.Text = sCmd;
                }

                string sParsedCmd = "";
                for (int nCharInd = 0; nCharInd < sCmd.Length; nCharInd++)
                {
                    if (('\\' == sCmd[nCharInd]) && ('x' == sCmd[nCharInd + 1]))
                    {
                        //An hex byte value is coming
                        string sHexVal = "";
                        sHexVal += (char)sCmd[nCharInd + 2];
                        sHexVal += (char)sCmd[nCharInd + 3];
                        byte nVal = Convert.ToByte(sHexVal, 16);
                        sParsedCmd += Convert.ToChar(nVal);
                        nCharInd += 3;
                    }
                    else
                    {
                        //Normal text is coming
                        sParsedCmd += sCmd[nCharInd];
                    }
                }
                SendCommand(sParsedCmd);
                ReportLog("Command sent: " + sCmd, true);
            }
            else
            {
                MessageBox.Show("Nothing to send.\nSpecify a command first...");
            }
        }

        private void OnSendCmd_Click(object sender, EventArgs e)
        {

            string sCmd = "";
            if (comboStdCmd.Text.Length > 0)
            {
                sCmd = comboStdCmd.Text;
                sCmd = sCmd.Substring(sCmd.LastIndexOf(':') + 2);
            }

            if (sCmd.Length > 0)
            {
                SendCommand(sCmd);
                ReportLog("Command sent: " + sCmd, true);
            }
            else
            {
                MessageBox.Show("Nothing to send.\nSpecify a command first...");
            }
        }

        #endregion

        #region Configuration

        private void tabConfiguration_Enter(object sender, EventArgs e)
        {
            DisplayGroups();
        }
        private void OnOpenLog_Click(object sender, EventArgs e)
        {
            Process.Start(m_sReportFilePath);
        }

        private void edDefaultFilePath_Leave(object sender, EventArgs e)
        {
            m_sDefaultDataFolder = edDefaultFilePath.Text;
            SaveProperties();
        }

        private void WorkaroundBug_firmware_v1_11_switching_module()
        {
            if (menuAutoLCDOff.Checked == false)
            {
                Thread.Sleep(500);
                m_objRFEAnalyzer.SendCommand_ScreenOFF();
                Thread.Sleep(500);
                m_objRFEAnalyzer.SendCommand_ScreenON();
            }
        }

        private bool m_bInternetFWUpgradeOpenOnce = false; //used to open internet firmware upgrade page only once per session
        private bool CheckRequiredVersionForFeature(double fVersionRequired)
        {
            if (!m_objRFEAnalyzer.IsFirmwareSameOrNewer(fVersionRequired))
            {
                MessageBox.Show("This function can only be used with Firmware v" + fVersionRequired + " or later\nPlease upgrade your RF Explorer!", "Old firmware detected");
                if (!m_bInternetFWUpgradeOpenOnce)
                {
                    Process.Start("www.rf-explorer.com/upgrade");
                }
                m_bInternetFWUpgradeOpenOnce = true;
                ReportLog("Function required with firmware v" + fVersionRequired + " but found older " + m_objRFEAnalyzer.RFExplorerFirmwareDetected, true);
                return false;
            }

            return true;
        }

        private void OnEnableMainboard_Click(object sender, EventArgs e)
        {
            if (!CheckRequiredVersionForFeature(1.11))
                return;

            Cursor.Current = Cursors.WaitCursor;
            if (menuEnableMainboard.Checked == false)
            {
                ResetSettingsTitle();
                m_objRFEAnalyzer.SendCommand_EnableMainboard();
                WorkaroundBug_firmware_v1_11_switching_module();
            }
            menuDevice_DropDownOpening(sender, e);
            Application.DoEvents();
            Thread.Sleep(1000); //this sleep will guarantee the user does not switch over and over very quick creating a stability problem
            Cursor.Current = Cursors.Default;
        }

        private void OnEnableExpansionBoard_Click(object sender, EventArgs e)
        {
            if (!CheckRequiredVersionForFeature(1.11))
                return;

            Cursor.Current = Cursors.WaitCursor;
            if (menuEnableExpansionBoard.Checked == false)
            {
                ResetSettingsTitle();
                m_objRFEAnalyzer.SendCommand_EnableExpansion();
                WorkaroundBug_firmware_v1_11_switching_module();
            }
            menuDevice_DropDownOpening(sender, e);
            Application.DoEvents();
            Thread.Sleep(1000); //this sleep will guarantee the user does not switch over and over very quick creating a stability problem
            Cursor.Current = Cursors.Default;
        }

        private void menuDevice_DropDownOpening(object sender, EventArgs e)
        {
            menuEnableMainboard.Enabled = m_objRFEAnalyzer.PortConnected;
            menuRemoteMaxHold.Enabled = m_objRFEAnalyzer.PortConnected;
            menuRefreshRemoteMaxHold.Enabled = m_objRFEAnalyzer.PortConnected && menuRemoteMaxHold.Checked;
            menuEnableMainboard.Checked = (m_objRFEAnalyzer.ExpansionBoardActive == false);
            menuEnableExpansionBoard.Enabled = m_objRFEAnalyzer.PortConnected && (m_objRFEAnalyzer.ExpansionBoardModel != RFECommunicator.eModel.MODEL_NONE);
            if (menuEnableExpansionBoard.Enabled)
            {
                menuEnableExpansionBoard.Checked = m_objRFEAnalyzer.ExpansionBoardActive;
            }
        }
        private void menuRemoteMaxHold_Click(object sender, EventArgs e)
        {
            m_objRFEAnalyzer.UseMaxHold = menuRemoteMaxHold.Checked;
        }

        private void menuRefreshRemoteMaxHold_Click(object sender, EventArgs e)
        {
            m_objRFEAnalyzer.ResetInternalBuffers();
        }

        private void OnOnlineHelp_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.rf-explorer.com/windows");
        }

        private void OnFirmware_Click(object sender, EventArgs e)
        {
            Process.Start("http://micro.arocholl.com/download/sw/fw/FirmwareReleaseNotes.pdf");
        }

        private void OnDeviceManual_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.rf-explorer.com/manual");
        }

        private void OnWindowsReleaseNotes_Click(object sender, EventArgs e)
        {
            Process.Start("http://micro.arocholl.com/download/sw/win/WindowsClientReleaseNotes.pdf");
        }
        #endregion  //configuration

        #region Debug_only
        private void CreateWaterfallTestData()
        {
            m_WaterfallSweepMaxHold.CleanAll();

            m_objRFEAnalyzer.AmplitudeBottomDBM = -100;
            m_objRFEAnalyzer.AmplitudeTopDBM = -10;

            UInt16 nTotalSteps = 100;

            for (UInt16 nMaxHoldInd = 0; nMaxHoldInd < nTotalSteps; nMaxHoldInd++)
            {
                RFESweepData objNewSweep = new RFESweepData(100.0f, 10.0f / 112, 112);
                m_WaterfallSweepMaxHold.Add(objNewSweep);

                for (UInt16 nSweepDataInd = 0; nSweepDataInd < objNewSweep.TotalSteps; nSweepDataInd++)
                {
                    float fAmplitude = -100.0f;
                    //a mark on the 6th position
                    if (nSweepDataInd == 5)
                    {
                        fAmplitude = -30.0f;
                    }
                    //every of the 10 divisions (112/10)
                    if ((nSweepDataInd % 12) == 0)
                    {
                        fAmplitude = -40.0f;
                    }
                    //A 15 steps band for the first half of the sweep time
                    if (nMaxHoldInd < 50)
                    {
                        if (nSweepDataInd > 35 && nSweepDataInd < 50)
                        {
                            fAmplitude = -50.0f;
                        }
                    }
                    //every 5 sweeps interval
                    if ((nMaxHoldInd % 10) < 5)
                    {
                        if (nSweepDataInd == 65)
                        {
                            fAmplitude = -30.0f;
                        }
                    }
                    objNewSweep.SetAmplitudeDBM(nSweepDataInd, fAmplitude);
                }
                Thread.Sleep(10); //to force some time difference between samples
            }
            UpdateWaterfall();
        }

        private void OnDebug_datatest_1_Click(object sender, EventArgs e)
        {
            CreateWaterfallTestData();
        }

        private void DumpAllReceivedBytes()
        {
            string sString = m_objRFEAnalyzer.AllReceivedBytes;
            m_objRFEAnalyzer.AllReceivedBytes = "";
            if (String.IsNullOrEmpty(sString))
                return;

            string sText = "";
            char cPrevChar = ' ';
            foreach (char cChar in sString)
            {
                byte nChar = Convert.ToByte(cChar);
                if ((nChar < 0x20) || (cChar > 0x7D))
                {
                    sText += "[0x" + nChar.ToString("X2") + "]";
                }
                else
                {
                    sText += cChar;
                }
                if ((cPrevChar == '\r') && (cChar == '\n'))
                {
                    sText += Environment.NewLine;
                }

                cPrevChar = cChar;
            }
            ReportLog(sText,false);
        }
        #endregion

        #region Limit Lines
        private void OnLimitLineBuildFromSignal_Click(object sender, EventArgs e)
        {
            try
            {
                bool bMin = sender.ToString().Contains("M&in");

                PointPairList listCurrentPointList = null;
                int nSelectionCounter = SelectSinglePointPairList(ref listCurrentPointList);

                if (nSelectionCounter == 0)
                {
                    MessageBox.Show("Condition not met: One active display curve on screen (Avg, Max, Min or Realtime)", "Limit Lines");
                    return;
                }
                else if (nSelectionCounter > 1)
                {
                    MessageBox.Show("Condition not met: One active display curve on screen ONLY (Avg, Max, Min or Realtime)", "Limit Lines");
                    return;
                }

                using (CreateLimitLine objDialog = new CreateLimitLine())
                {
                    if (objDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        double dIncrementAmplitudeDB = Convert.ToDouble(objDialog.m_edOffsetDB.Text);

                        if (bMin)
                            m_LimitLineMin = new LimitLine(listCurrentPointList);
                        else
                            m_LimitLineMax = new LimitLine(listCurrentPointList);

                        //add requested offset to each point - NOTE: This is not the RFE measurement offset so the 
                        //function SetNewOffset is not appropriate here!
                        for (int nInd = 0; nInd < listCurrentPointList.Count; nInd++)
                        {
                            if (bMin)
                                m_LimitLineMin[nInd].Y -= dIncrementAmplitudeDB;
                            else
                                m_LimitLineMax[nInd].Y += dIncrementAmplitudeDB;
                        }

                        if (bMin)
                            m_LimitLineMin.AmplitudeUnits = GetCurrentAmplitudeEnum();
                        else
                            m_LimitLineMax.AmplitudeUnits = GetCurrentAmplitudeEnum();

                        DisplaySpectrumAnalyzerData();
                    }
                }
            }
            catch (Exception obEx) { MessageBox.Show(obEx.Message); }
        }

        private void OnLimitLineSaveToFile_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog MySaveFileDialog = new SaveFileDialog())
            {
                MySaveFileDialog.Filter = _RFL_File_Selector;
                MySaveFileDialog.FilterIndex = 1;
                MySaveFileDialog.RestoreDirectory = false;
                MySaveFileDialog.InitialDirectory = m_sDefaultDataFolder;

                MySaveFileDialog.FileName = GetNewFilename(RFExplorerFileType.LimitLineDataFile);

                if (MySaveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (sender.ToString().Contains("M&in"))
                        m_LimitLineMin.SaveFile(MySaveFileDialog.FileName);
                    else
                        m_LimitLineMax.SaveFile(MySaveFileDialog.FileName);
                }
            }
        }

        private void OnRemoveMaxLimitLine_Click(object sender, EventArgs e)
        {
            m_LimitLineMax.Clear();
            DisplaySpectrumAnalyzerData();
        }

        private void OnRemoveMinLimitLine_Click(object sender, EventArgs e)
        {
            m_LimitLineMin.Clear();
            DisplaySpectrumAnalyzerData();
        }

        private void OnLimitLineReadFromFile_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog MyOpenFileDialog = new OpenFileDialog())
                {
                    MyOpenFileDialog.Filter = _RFL_File_Selector;
                    MyOpenFileDialog.FilterIndex = 1;
                    MyOpenFileDialog.RestoreDirectory = false;
                    MyOpenFileDialog.InitialDirectory = m_sDefaultDataFolder;

                    if (MyOpenFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        bool bOk = false;

                        if (sender.ToString().Contains("M&in"))
                        {
                            bOk = m_LimitLineMin.ReadFile(MyOpenFileDialog.FileName);
                            if (bOk)
                                m_LimitLineMin.AmplitudeUnits = GetCurrentAmplitudeEnum();
                        }
                        else
                        {
                            bOk = m_LimitLineMax.ReadFile(MyOpenFileDialog.FileName);
                            if (bOk)
                                m_LimitLineMax.AmplitudeUnits = GetCurrentAmplitudeEnum();
                        }

                        if (bOk)
                            DisplaySpectrumAnalyzerData();
                        else
                            MessageBox.Show("");
                    }
                }
            }
            catch (Exception obEx) { MessageBox.Show(obEx.Message); }
        }

        private void OnItemSoundAlarmLimitLine_Click(object sender, EventArgs e)
        {
            if (menuItemSoundAlarmLimitLine.Checked == false)
                m_SoundPlayer.Stop();
        }
        #endregion

        #region Markers
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void CreateMarkerMenu()
        {
            m_arrMarkersEnabledMenu = new ToolStripMenuItem[MarkerCollection.MAX_MARKERS];
            m_arrMarkersFrequencyMenu = new ToolStripTextBox[MarkerCollection.MAX_MARKERS];

            for (int nInd = 0; nInd < MarkerCollection.MAX_MARKERS; nInd++)
            {
                ToolStripMenuItem objMarkerMenu = new ToolStripMenuItem((nInd + 1).ToString());
                objMarkerMenu.Name = "menuMarkerEnabled_ID" + (nInd + 1).ToString();
                objMarkerMenu.Size = new System.Drawing.Size(152, 22);
                menuMarkers.DropDownItems.Add(objMarkerMenu);
                ToolStripMenuItem objMarkerVisible = new ToolStripMenuItem("&Enable Marker");
                objMarkerVisible.Name = objMarkerMenu.Name + "_Enable";
                objMarkerVisible.ToolTipText = "Enable or Disable this marker on screen";
                objMarkerVisible.CheckOnClick = true;
                objMarkerVisible.Click += new System.EventHandler(OnMarkerVisible_Click);
                objMarkerMenu.DropDownItems.Add(objMarkerVisible);
                m_arrMarkersEnabledMenu[nInd] = objMarkerVisible;
                if (nInd != 0)
                {
                    ToolStripTextBox objFrequency = new ToolStripTextBox();
                    objFrequency.MaxLength = 12;
                    objFrequency.Name = "menuMarkerFrequency_ID" + (nInd + 1).ToString();
                    objFrequency.BorderStyle = BorderStyle.Fixed3D;
                    objFrequency.ToolTipText = "Specify frequency in MHZ for this marker (only numbers required, the MHZ is filled in automatically)";
                    objFrequency.LostFocus += new System.EventHandler(menuMarkerFrequency_Change);
                    //objFrequency.TextChanged += new System.EventHandler(menuMarkerFrequency_Change);
                    objMarkerMenu.DropDownItems.Add(objFrequency);
                    m_arrMarkersFrequencyMenu[nInd] = objFrequency;
                }
                else
                {
                    //Create specific items of marker 0
                    for (int nInd2 = 0; nInd2 < (int)RFExplorerSignalType.TOTAL_ITEMS; nInd2++)
                    {
                        ToolStripMenuItem objMarkerPeak = new ToolStripMenuItem("Track " + ((RFExplorerSignalType)nInd2).ToString() + " peak");
                        objMarkerPeak.Name = "menuTrackPeak_" + ((RFExplorerSignalType)nInd2).ToString();
                        objMarkerPeak.ToolTipText = "Marker track peak on this trace, all other traces will follow this one";
                        objMarkerPeak.CheckOnClick = false;
                        objMarkerPeak.Tag = nInd2;
                        objMarkerPeak.Click += new System.EventHandler(OnMarkerTrackPeak_Click);
                        objMarkerMenu.DropDownItems.Add(objMarkerPeak);
                    }
                }
            }

            UpdateMenuFromMarkerCollection();
        }

        private void UpdateMenuMarkerPeakTrack()
        {
            ToolStripMenuItem objMarker1Menu = (ToolStripMenuItem)menuMarkers.DropDownItems[0];

            for (int nInd2 = 0; nInd2 < (int)RFExplorerSignalType.TOTAL_ITEMS; nInd2++)
            {
                if (nInd2 == (int)m_eTrackSignalPeak)
                {
                    ((ToolStripMenuItem)objMarker1Menu.DropDownItems[nInd2 + 1]).Checked = true;
                }
                else
                {
                    ((ToolStripMenuItem)objMarker1Menu.DropDownItems[nInd2 + 1]).Checked = false;
                }
            }
        }

        private void OnMarkerTrackPeak_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem objMarkerPeak = (ToolStripMenuItem)sender;
            m_eTrackSignalPeak = (RFExplorerSignalType)objMarkerPeak.Tag;
            UpdateMenuMarkerPeakTrack();
            UpdateMarkerControlContents();

            if (m_MainTab.SelectedTab == m_tabSpectrumAnalyzer)
            {
                DisplaySpectrumAnalyzerData(); //redisplay all for markers update
            }
        }

        private void OnMarkerVisible_Click(object sender, EventArgs e)
        {
            UpdateMarkerControlContents();
            if (m_MainTab.SelectedTab == m_tabSpectrumAnalyzer)
            {
                DisplaySpectrumAnalyzerData(); //redisplay all for markers update
            }
        }

        private void UpdateMenuFromMarkerCollection()
        {
            for (int nInd = 0; nInd < MarkerCollection.MAX_MARKERS; nInd++)
            {
                m_arrMarkersEnabledMenu[nInd].Checked = m_Markers.IsMarkerEnabled(nInd);
                if (nInd > 0)
                {
                    m_arrMarkersFrequencyMenu[nInd].Text = m_Markers.GetMarkerFrequency(nInd).ToString("0000.000 MHZ");
                }
                else
                {
                    UpdateMenuMarkerPeakTrack();
                }
            }
        }

        private void menuMarkerFrequency_Change(object sender, EventArgs e)
        {
            try
            {
                ToolStripTextBox objFrequency = (ToolStripTextBox)sender;

                string sFreq = objFrequency.Text.Replace("MHZ", "");
                sFreq = sFreq.Replace(" ", "");
                double dFreq = Convert.ToDouble(sFreq);
                int nMarker = Convert.ToInt32(objFrequency.Name.Replace("menuMarkerFrequency_ID", ""));
                if (nMarker > 1)
                {
                    m_Markers.SetMarkerFrequency(nMarker-1, dFreq);
                    m_arrMarkersEnabledMenu[nMarker - 1].Checked=true;
                    OnMarkerVisible_Click(null, null);
                }
                UpdateMenuFromMarkerCollection();
            }
            catch (Exception obEx)
            {
                ReportLog(obEx.Message, false);
                ReportLog(obEx.ToString(), true);
            }
        }

        private void CreateMarkerConfigPanel()
        {
            m_panelSAMarkers = new Panel();
            m_panelSAMarkers.Visible = true;
            m_panelSAMarkers.AutoSize = true;
            m_panelSAMarkers.AutoSizeMode = AutoSizeMode.GrowOnly;
            m_panelSAMarkers.BorderStyle = BorderStyle.FixedSingle;
            m_panelSAMarkers.Font = new System.Drawing.Font("Tahoma", 8, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            m_panelSAMarkers.ForeColor = m_ColorPanelText;
            m_panelSAMarkers.Width = 120;

            m_panelSAConfiguration = new Panel();
            m_panelSAConfiguration.Visible = true;
            m_panelSAConfiguration.AutoSize = true;
            m_panelSAConfiguration.AutoSizeMode = AutoSizeMode.GrowOnly;
            m_panelSAConfiguration.BorderStyle = BorderStyle.FixedSingle;
            m_panelSAConfiguration.Font = new System.Drawing.Font("Tahoma", 8, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            m_panelSAConfiguration.ForeColor = m_ColorPanelText;
            m_panelSAConfiguration.Width = m_panelSAMarkers.Width;

            m_tabSpectrumAnalyzer.Controls.Add(m_panelSAMarkers);
            m_tabSpectrumAnalyzer.Controls.Add(m_panelSAConfiguration);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void UpdateConfigControlContents()
        {
            if (m_panelSAConfiguration == null)
                return;

            if (m_panelSAConfiguration.Controls.Count == 0)
            {
                //create labels and initialize
                int nPosX = 3;
                int nPosY = 5;

                System.Windows.Forms.Label objLabelFirmwareNew = new System.Windows.Forms.Label();
                objLabelFirmwareNew.Name = _Firmware;
                objLabelFirmwareNew.Location = new Point(nPosX, nPosY);
                objLabelFirmwareNew.AutoSize = true;
                int nTextSizeHeight = objLabelFirmwareNew.Height / 2 + 2;
                nPosY += nTextSizeHeight;
                m_panelSAConfiguration.Controls.Add(objLabelFirmwareNew);

                System.Windows.Forms.Label objLabelMainboard = new System.Windows.Forms.Label();
                objLabelMainboard.Name = _MainBoard;
                objLabelMainboard.Location = new Point(nPosX, nPosY);
                objLabelMainboard.AutoSize = true;
                nPosY += nTextSizeHeight;
                m_panelSAConfiguration.Controls.Add(objLabelMainboard);

                System.Windows.Forms.Label objLabelExpansion = new System.Windows.Forms.Label();
                objLabelExpansion.Name = _ExpansionBoard;
                objLabelExpansion.Location = new Point(nPosX, nPosY);
                objLabelExpansion.AutoSize = true;
                nPosY += nTextSizeHeight;
                m_panelSAConfiguration.Controls.Add(objLabelExpansion);

                System.Windows.Forms.Label objLabelWindowsNew = new System.Windows.Forms.Label();
                objLabelWindowsNew.Name = _Windows;
                objLabelWindowsNew.Location = new Point(nPosX, nPosY);
                objLabelWindowsNew.AutoSize = true;
                nPosY += nTextSizeHeight;
                objLabelWindowsNew.Text = "Client: " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
                m_panelSAConfiguration.Controls.Add(objLabelWindowsNew);

                System.Windows.Forms.Label objLabelFreq = new System.Windows.Forms.Label();
                objLabelFreq.Name = _Freq;
                objLabelFreq.Location = new Point(nPosX, nPosY);
                objLabelFreq.AutoSize = true;
                nPosY += nTextSizeHeight;
                m_panelSAConfiguration.Controls.Add(objLabelFreq);

                System.Windows.Forms.Label objLabelSpan = new System.Windows.Forms.Label();
                objLabelSpan.Name = _Span;
                objLabelSpan.Location = new Point(nPosX, nPosY);
                objLabelSpan.AutoSize = true;
                nPosY += nTextSizeHeight;
                m_panelSAConfiguration.Controls.Add(objLabelSpan);

                System.Windows.Forms.Label objLabelStep = new System.Windows.Forms.Label();
                objLabelStep.Name = _Step;
                objLabelStep.Location = new Point(nPosX, nPosY);
                objLabelStep.AutoSize = true;
                nPosY += nTextSizeHeight;
                m_panelSAConfiguration.Controls.Add(objLabelStep);

                System.Windows.Forms.Label objLabelRBW = new System.Windows.Forms.Label();
                objLabelRBW.Name = _RBW;
                objLabelRBW.Location = new Point(nPosX, nPosY);
                objLabelRBW.AutoSize = true;
                nPosY += nTextSizeHeight;
                m_panelSAConfiguration.Controls.Add(objLabelRBW);

                System.Windows.Forms.Label objLabelOffsetDBNew = new System.Windows.Forms.Label();
                objLabelOffsetDBNew.Name = _OffsetDB;
                objLabelOffsetDBNew.Location = new Point(nPosX, nPosY);
                objLabelOffsetDBNew.AutoSize = true;
                nPosY += nTextSizeHeight;
                m_panelSAConfiguration.Controls.Add(objLabelOffsetDBNew);

                System.Windows.Forms.Label objLabelCalibration = new System.Windows.Forms.Label();
                objLabelCalibration.Name = _Cal;
                objLabelCalibration.Location = new Point(nPosX, nPosY);
                objLabelCalibration.AutoSize = true;
                nPosY += nTextSizeHeight;
                m_panelSAConfiguration.Controls.Add(objLabelCalibration);

                m_panelSAConfiguration.Height = nPosY + nTextSizeHeight;
            }

            m_panelSAConfiguration.ForeColor = m_ColorPanelText;
            m_panelSAConfiguration.Controls[_MainBoard].ForeColor = m_ColorPanelText;
            m_panelSAConfiguration.Controls[_ExpansionBoard].ForeColor = m_ColorPanelText;
            if (m_objRFEAnalyzer.MainBoardModel != RFECommunicator.eModel.MODEL_NONE)
            {
                if (m_objRFEAnalyzer.ExpansionBoardActive)
                {
                    m_panelSAConfiguration.Controls[_MainBoard].ForeColor = m_ColorPanelText;
                    m_panelSAConfiguration.Controls[_ExpansionBoard].ForeColor = m_ColorPanelTextHighlight;
                }
                else
                {
                    m_panelSAConfiguration.Controls[_MainBoard].ForeColor = m_ColorPanelTextHighlight;
                    m_panelSAConfiguration.Controls[_ExpansionBoard].ForeColor = m_ColorPanelText;
                }
            }

            m_panelSAConfiguration.Controls[_Firmware].Text = "Firmware: " + m_objRFEAnalyzer.RFExplorerFirmwareDetected;
            m_panelSAConfiguration.Controls[_MainBoard].Text = "RF Left: " + RFECommunicator.GetModelTextFromEnum(m_objRFEAnalyzer.MainBoardModel);
            m_panelSAConfiguration.Controls[_ExpansionBoard].Text = "RF Right: " + RFECommunicator.GetModelTextFromEnum(m_objRFEAnalyzer.ExpansionBoardModel);
            m_panelSAConfiguration.Controls[_Freq].Text = "Center: " + m_objRFEAnalyzer.CalculateCenterFrequencyMHZ().ToString("f3") + "MHz";
            m_panelSAConfiguration.Controls[_Span].Text = "Span: " + m_objRFEAnalyzer.CalculateFrequencySpanMHZ().ToString("f3") + "MHz";
            m_panelSAConfiguration.Controls[_OffsetDB].Text = "Offset: " + m_objRFEAnalyzer.AmplitudeOffsetDB + "dB";
            m_panelSAConfiguration.Controls[_Step].Text = "Step: " + (1000*m_objRFEAnalyzer.StepFrequencyMHZ).ToString("f3") + "KHz";
            m_panelSAConfiguration.Controls[_RBW].Text = "RBW: " + m_objRFEAnalyzer.RBW_KHZ + "KHz";

            m_panelSAConfiguration.Controls[_Cal].Text = "Cal: ";
            if (menuUseAmplitudeCorrection.Checked)
            {
                if (m_objRFEAnalyzer.m_AmplitudeCalibration.HasCalibrationData)
                {
                    m_panelSAConfiguration.Controls[_Cal].Text += "OVR ";
                    m_panelSAConfiguration.Controls[_Cal].Text += m_objRFEAnalyzer.m_AmplitudeCalibration.CalibrationID.ToLower();
                    if (m_panelSAConfiguration.Controls[_Cal].Text.Length > 20)
                    {
                        m_panelSAConfiguration.Controls[_Cal].Text = m_panelSAConfiguration.Controls[_Cal].Text.Substring(0,20);
                    }
                }
                else
                {
                    m_panelSAConfiguration.Controls[_Cal].Text += "No";
                }
            }
            else
            {
                m_panelSAConfiguration.Controls[_Cal].Text += "Disabled";
            }

            if (IsWaterfallOnMainScreen())
            {
                m_panelSAConfiguration.Left = m_panelWaterfall.Right + 4;
            }
            else
            {
                m_panelSAConfiguration.Left = m_GraphSpectrumAnalyzer.Right + 4;
            }

            m_panelSAConfiguration.Top = m_GraphSpectrumAnalyzer.Top;

            m_panelSAConfiguration.BackColor = m_ColorPanelBackground;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void UpdateMarkerControlContents()
        {
            if (m_panelSAMarkers==null)
                return;

            //remove any old control setting
            foreach (Control objControl in m_panelSAMarkers.Controls)
            {
                objControl.Dispose();
            }
            m_panelSAMarkers.Controls.Clear();

            m_panelSAMarkers.Top = m_panelSAConfiguration.Bottom + 2;
            m_panelSAMarkers.Left = m_panelSAConfiguration.Left;

            if (menuPlaceWaterfallAtBottom.Checked)
            {
                m_panelSAMarkers.Height = m_GraphSpectrumAnalyzer.Height + m_panelWaterfall.Height - m_panelSAConfiguration.Height - 6;
            }
            else
            {
                m_panelSAMarkers.Height = m_GraphSpectrumAnalyzer.Height - m_panelSAConfiguration.Height - 2;
            }

            int nPosX = 3;
            int nPosY = 5;
            bool bSomeMarkerEnabled = false;
            for (int nInd=0; nInd<MarkerCollection.MAX_MARKERS; nInd++)
            {
                if (m_arrMarkersEnabledMenu[nInd].Checked)
                {
                    bSomeMarkerEnabled = true;
                    System.Windows.Forms.Label objLabelFreq = new System.Windows.Forms.Label();
                    objLabelFreq.Name = "M" + (nInd + 1).ToString() + "Freq";
                    objLabelFreq.Location = new Point(nPosX, nPosY);
                    objLabelFreq.AutoSize = true;
                    int nTextSizeHeight = objLabelFreq.Height/2+2;
                    nPosY += nTextSizeHeight;
                    m_panelSAMarkers.Controls.Add(objLabelFreq);

                    if (menuRealtimeTrace.Checked)
                    {
                        System.Windows.Forms.Label objLabel= new System.Windows.Forms.Label();
                        objLabel.Name = "M" + (nInd + 1).ToString() + "RT";
                        objLabel.Location = new Point(nPosX + 3, nPosY);
                        objLabel.AutoSize = true;
                        nPosY += nTextSizeHeight;
                        m_panelSAMarkers.Controls.Add(objLabel);
                    }
                    if (menuAveragedTrace.Checked)
                    {
                        System.Windows.Forms.Label objLabel= new System.Windows.Forms.Label();
                        objLabel.Name = "M" + (nInd + 1).ToString() + "Avg";
                        objLabel.Location = new Point(nPosX + 3, nPosY);
                        objLabel.AutoSize = true;
                        nPosY += nTextSizeHeight;
                        m_panelSAMarkers.Controls.Add(objLabel);
                    }
                    if (menuMaxTrace.Checked)
                    {
                        System.Windows.Forms.Label objLabel= new System.Windows.Forms.Label();
                        objLabel.Name = "M" + (nInd + 1).ToString() + "Max";
                        objLabel.Location = new Point(nPosX + 3, nPosY);
                        objLabel.AutoSize = true;
                        nPosY += nTextSizeHeight;
                        m_panelSAMarkers.Controls.Add(objLabel);
                    }
                    if (menuMinTrace.Checked)
                    {
                        System.Windows.Forms.Label objLabel= new System.Windows.Forms.Label();
                        objLabel.Name = "M" + (nInd + 1).ToString() + "Min";
                        objLabel.Location = new Point(nPosX + 3, nPosY);
                        objLabel.AutoSize = true;
                        nPosY += nTextSizeHeight;
                        m_panelSAMarkers.Controls.Add(objLabel);
                    }
                    if (menuMaxHoldTrace.Checked)
                    {
                        System.Windows.Forms.Label objLabel= new System.Windows.Forms.Label();
                        objLabel.Name = "M" + (nInd + 1).ToString() + "MxH";
                        objLabel.Location = new Point(nPosX + 3, nPosY);
                        objLabel.AutoSize = true;
                        nPosY += nTextSizeHeight;
                        m_panelSAMarkers.Controls.Add(objLabel);
                    }
                    nPosY += 5;
                }
            }
            if (bSomeMarkerEnabled)
            {
                UpdateMarkerControlValues();
            }
        }

        private void UpdateMarkerControlValues()
        {
            m_panelSAMarkers.BackColor = m_ColorPanelBackground;
            m_panelSAMarkers.ForeColor = m_ColorPanelText;

            if ((m_arrMarkersEnabledMenu == null) || (m_panelSAMarkers.Controls.Count==0))
                return;

            for (int nInd = 0; nInd < MarkerCollection.MAX_MARKERS; nInd++)
            {
                if (m_arrMarkersEnabledMenu[nInd].Checked)
                {
                    System.Windows.Forms.Label objLabelFreq=(System.Windows.Forms.Label)m_panelSAMarkers.Controls["M" + (nInd + 1).ToString() + "Freq"];
                    if (objLabelFreq != null)
                    {
                        objLabelFreq.Text = "M" + (nInd + 1).ToString() + ": " + m_Markers.GetMarkerFrequency(nInd).ToString("0000.000 MHz");

                        //Check if the marker is indeed inside the frequency area
                        if ((m_Markers.GetMarkerFrequency(nInd) > m_objRFEAnalyzer.StopFrequencyMHZ) ||
                            (m_Markers.GetMarkerFrequency(nInd) < m_objRFEAnalyzer.StartFrequencyMHZ))
                        {
                            objLabelFreq.Enabled = false;
                        }
                        else
                        {
                            objLabelFreq.Enabled = true;
                        }
                    }

                    if (menuRealtimeTrace.Checked)
                    {
                        System.Windows.Forms.Label objLabel = (System.Windows.Forms.Label)m_panelSAMarkers.Controls["M" + (nInd + 1).ToString() + "RT"];
                        if (objLabel != null)
                        {
                            if (nInd == 0)
                            {
                                if (m_eTrackSignalPeak == RFExplorerSignalType.Realtime)
                                    objLabel.ForeColor = m_ColorPanelTextHighlight;
                                else
                                    objLabel.ForeColor = m_ColorPanelText;
                            }
                            objLabel.Enabled = objLabelFreq.Enabled && m_PointList_Realtime.Count > 0;
                            if (objLabel.Enabled)
                                objLabel.Text = "RT: " + m_Markers.GetMarkerAmplitude(nInd, RFExplorerSignalType.Realtime).ToString(GetCurrentAmplitudeUnitFormat()) + GetCurrentAmplitudeUnitLabel();
                            else
                                objLabel.Text = "RT: invalid";
                            objLabelFreq.Enabled &= objLabel.Enabled;
                        }
                    }
                    if (menuAveragedTrace.Checked)
                    {
                        System.Windows.Forms.Label objLabel = (System.Windows.Forms.Label)m_panelSAMarkers.Controls["M" + (nInd + 1).ToString() + "Avg"];
                        if (objLabel != null)
                        {
                            if (nInd == 0)
                            {
                                if (m_eTrackSignalPeak == RFExplorerSignalType.Average)
                                    objLabel.ForeColor = m_ColorPanelTextHighlight;
                                else
                                    objLabel.ForeColor = m_ColorPanelText;
                            }
                            objLabel.Enabled = objLabelFreq.Enabled && m_PointList_Avg.Count > 0;
                            if (objLabel.Enabled)
                                objLabel.Text = "Avg: " + m_Markers.GetMarkerAmplitude(nInd, RFExplorerSignalType.Average).ToString(GetCurrentAmplitudeUnitFormat()) + GetCurrentAmplitudeUnitLabel();
                            else
                                objLabel.Text = "Avg: invalid";
                        }
                    }
                    if (menuMaxTrace.Checked)
                    {
                        System.Windows.Forms.Label objLabel = (System.Windows.Forms.Label)m_panelSAMarkers.Controls["M" + (nInd + 1).ToString() + "Max"];
                        if (objLabel != null)
                        {
                            if (nInd == 0)
                            {
                                if (m_eTrackSignalPeak == RFExplorerSignalType.MaxPeak)
                                    objLabel.ForeColor = m_ColorPanelTextHighlight;
                                else
                                    objLabel.ForeColor = m_ColorPanelText;
                            }
                            objLabel.Enabled = objLabelFreq.Enabled && m_PointList_Max.Count > 0;
                            if (objLabel.Enabled)
                                objLabel.Text = "Max: " + m_Markers.GetMarkerAmplitude(nInd, RFExplorerSignalType.MaxPeak).ToString(GetCurrentAmplitudeUnitFormat()) + GetCurrentAmplitudeUnitLabel();
                            else
                                objLabel.Text = "Max: invalid";
                        }
                    }
                    if (menuMinTrace.Checked)
                    {
                        System.Windows.Forms.Label objLabel = (System.Windows.Forms.Label)m_panelSAMarkers.Controls["M" + (nInd + 1).ToString() + "Min"];
                        if (objLabel != null)
                        {
                            if (nInd == 0)
                            {
                                if (m_eTrackSignalPeak == RFExplorerSignalType.Min)
                                    objLabel.ForeColor = m_ColorPanelTextHighlight;
                                else
                                    objLabel.ForeColor = m_ColorPanelText;
                            }
                            objLabel.Enabled = objLabelFreq.Enabled && m_PointList_Min.Count > 0;
                            if (objLabel.Enabled)
                                objLabel.Text = "Min: " + m_Markers.GetMarkerAmplitude(nInd, RFExplorerSignalType.Min).ToString(GetCurrentAmplitudeUnitFormat()) + GetCurrentAmplitudeUnitLabel();
                            else
                                objLabel.Text = "Min: invalid";
                        }
                    }
                    if (menuMaxHoldTrace.Checked)
                    {
                        System.Windows.Forms.Label objLabel = (System.Windows.Forms.Label)m_panelSAMarkers.Controls["M" + (nInd + 1).ToString() + "MxH"];
                        if (objLabel != null)
                        {
                            if (nInd == 0)
                            {
                                if (m_eTrackSignalPeak == RFExplorerSignalType.MaxHold)
                                    objLabel.ForeColor = m_ColorPanelTextHighlight;
                                else
                                    objLabel.ForeColor = m_ColorPanelText;
                            }
                            objLabel.Enabled = objLabelFreq.Enabled && m_PointList_MaxHold.Count > 0;
                            if (objLabel.Enabled)
                                objLabel.Text = "MxH: " + m_Markers.GetMarkerAmplitude(nInd, RFExplorerSignalType.MaxHold).ToString(GetCurrentAmplitudeUnitFormat()) + GetCurrentAmplitudeUnitLabel();
                            else
                                objLabel.Text = "MxH: invalid";
                        }
                    }
                }
            }
        }
        #endregion

        #region AmplitudeCalibration

        private void LoadFileRFA(string sFilename, bool bInteractiveWarning)
        {
            menuUseAmplitudeCorrection.Checked = false;
            if (m_objRFEAnalyzer.LoadFileRFA(sFilename))
            {
                m_LimitLineOverload.Clear();
                if (m_objRFEAnalyzer.m_AmplitudeCalibration.HasCompressionData)
                {
                    m_LimitLineOverload.CreateFromArray(m_objRFEAnalyzer.m_AmplitudeCalibration.m_arrCompressionDataDBM);
                    m_LimitLineOverload.NewOffset(m_objRFEAnalyzer.AmplitudeOffsetDB);
                }
                m_LimitLineOverload.AmplitudeUnits = GetCurrentAmplitudeEnum();
                menuUseAmplitudeCorrection.Enabled = true;
                menuUseAmplitudeCorrection.Checked = m_objRFEAnalyzer.PortConnected;
                DisplaySpectrumAnalyzerData();
            }
            else
            {
                string sWarning = "ERROR: Incorrect format or data - Unable to load file " + sFilename;
                ReportLog(sWarning, false);
                if (bInteractiveWarning)
                    MessageBox.Show(sWarning);
            }
        }

        private void OnLoadAmplitudeFile_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog MyOpenFileDialog = new OpenFileDialog())
                {
                    MyOpenFileDialog.Filter = _RFA_File_Selector;
                    MyOpenFileDialog.FilterIndex = 1;
                    MyOpenFileDialog.RestoreDirectory = false;
                    MyOpenFileDialog.InitialDirectory = m_sDefaultDataFolder;

                    if (MyOpenFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        LoadFileRFA(MyOpenFileDialog.FileName, true);
                    }
                }
            }
            catch (Exception obEx) { MessageBox.Show(obEx.Message); }
        }

        private void OnUseAmplitudeCorrection_Click(object sender, EventArgs e)
        {
            menuUseAmplitudeCorrection.Checked = !menuUseAmplitudeCorrection.Checked;
            UpdateConfigControlContents();
            DisplaySpectrumAnalyzerData();
        }

        private string ModelAmplitudeFileName()
        {
            return m_sAppDataFolder + "\\" + RFECommunicator.GetModelTextFromEnum(m_objRFEAnalyzer.ActiveModel) + ".RFA";
        }

        private void AutoLoadAmplitudeDataFile()
        {
            if (menuAutoLoadAmplitudeData.Checked)
            {
                if (File.Exists(ModelAmplitudeFileName()))
                {
                    LoadFileRFA(ModelAmplitudeFileName(), false);
                    UpdateConfigControlContents();
                    ReportLog("Automatic amplitude calibration data loaded: " + ModelAmplitudeFileName(), false);
                }
                else
                {
                    ReportLog("NO CALIBRATION DATA LOAD: Automatic amplitude calibration data not found: " + ModelAmplitudeFileName(), false);
                }
            }
        }

        private void menuAutoLoadAmplitudeData_Click(object sender, EventArgs e)
        {
            AutoLoadAmplitudeDataFile();
        }
        #endregion

        private void menuSaveSNANormalization_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog MySaveFileDialog = new SaveFileDialog())
                {
                    MySaveFileDialog.Filter = _SNANORM_File_Selector;
                    MySaveFileDialog.FilterIndex = 1;
                    MySaveFileDialog.RestoreDirectory = false;
                    MySaveFileDialog.InitialDirectory = m_sDefaultDataFolder;

                    MySaveFileDialog.FileName = GetNewFilename(RFExplorerFileType.SNANormalizedDataFile); ;

                    if (MySaveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        m_objRFEAnalyzer.SaveFileSNANormalization(MySaveFileDialog.FileName);
                    }
                }
            }
            catch (Exception obEx) { MessageBox.Show(obEx.Message); }
        }

        private void menuLoadSNANormalization_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                CleanSweepData();

                using (OpenFileDialog MyOpenFileDialog = new OpenFileDialog())
                {
                    MyOpenFileDialog.Filter = _SNANORM_File_Selector;
                    MyOpenFileDialog.FilterIndex = 1;
                    MyOpenFileDialog.RestoreDirectory = false;
                    MyOpenFileDialog.InitialDirectory = m_sDefaultDataFolder;

                    if (MyOpenFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        m_objRFEAnalyzer.TrackingRFEGen = m_objRFEGenerator;

                        if (m_objRFEAnalyzer.LoadDataFile(MyOpenFileDialog.FileName))
                        {
                            UpdateSweepNumericControls();
                            UpdateConfigControlContents();
                            ReportLog("Normalization Data File " + MyOpenFileDialog.FileName + " loaded: " + m_objRFEAnalyzer.TrackingNormalizedData.Dump(), false);
                            UpdateFeedMode();

                            SetupSpectrumAnalyzerAxis();
                            DisplaySpectrumAnalyzerData();
                            UpdateWaterfall();

                            UpdateRFGeneratorControlsFromObject(false);
                            UpdateButtonStatus();
                            //we read it again here. The reason is the previous call UpdateRFGeneratorControlsFromObject may have reset TrackingNormalizedData to null due to changes
                            //in controls, so normalization data would have been lost to null. This recovers it and guarantees the controls are already in expected status.
                            m_objRFEAnalyzer.LoadDataFile(MyOpenFileDialog.FileName);
                        }
                        else
                        {
                            m_objRFEAnalyzer.ResetTrackingNormalizedData();
                            MessageBox.Show("Normalization file cannot be loaded or is not valid for the connected models");
                        }
                        UpdateButtonStatus();
                    }
                }
            }
            catch (Exception obEx) 
            {
                ReportLog(obEx.ToString(), false);
                MessageBox.Show(obEx.Message); 
            }

            Cursor.Current = Cursors.Default;
        }
        private void m_btnCalibrate6G_Click(object sender, EventArgs e)
        {
        }

        private void btnCalibratePurge_Click(object sender, EventArgs e)
        {
        }
    }
}
