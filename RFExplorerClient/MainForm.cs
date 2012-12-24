//============================================================================
//RF Explorer PC Client - A Handheld Spectrum Analyzer for everyone!
//Copyright © 2010-12 Ariel Rocholl, www.rf-explorer.com
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

namespace RFExplorerClient
{
    public partial class MainForm : Form
    {

        #region Data Members
        const byte m_nFileFormat = 1;               //File format constant
        const int m_nTotalBufferSize = 30000;       //buffer size for the different available collections

        const double MIN_AMPLITUDE_DBM = -120.0;
        const double MAX_AMPLITUDE_DBM = -1.0;
        const double MIN_AMPLITUDE_RANGE_DBM = 10;
        const double MAX_RAW_SAMPLE = 4356 * 8;     //default value for RAW data sample
        const UInt16 MAX_SPECTRUM_STEPS = 1024;

        const string m_sRFExplorerFirmwareCertified = "01.10"; //Firmware version of RF Explorer which was tested and certified with this PC Client

        UInt16 m_nFreqSpectrumSteps = 112;  //$S byte buffer by default
        
        int m_nDrawingIteration = 0;        //Iteration counter to do regular updates on GUI

        DataSet m_DataSettings;             //Settings data collection
        bool m_bVersionAlerted = false;     //Used to alert about firmware version popup only once per session
        bool m_bCalibrating = false;        //True when the application is calibrating

        enum eModel
        {
            MODEL_433=0,    //0
            MODEL_868,      //1
            MODEL_915,      //2
            MODEL_WSUB1G,   //3
            MODEL_2400,     //4
            MODEL_WSUB3G,   //5
            MODEL_NONE=0xFF //0xFF
        };

        string[] arrModels = {"433M","868M","915M","WSUB1G","2.4G","WSUB3G"};

        eModel m_eMainBoardModel;           //The RF model installed in main board
        eModel m_eExpansionBoardModel;      //The RF model installed in the expansion board
        eModel m_eActiveModel;              //The model active, regardless being main or expansion board

        enum eMode
        {
            MODE_SPECTRUM_ANALYZER, //0
            MODE_TRANSMITTER,       //1
            MODE_WIFI_ANALYZER,     //2
            MODE_NONE=0xFF          //0xFF
        };
        
        eMode m_eMode;                      //The current operational mode
        bool m_bExpansionBoardActive;       //True when the expansion board is active, false otherwise

        double m_fPeakValueMHZ = 0.0f;      //Last drawing iteration peak value MHZ read
        double m_fPeakValueAmp = -120.0f;   //Last drawing iteration peak value dBm read

        //Initializer for 433MHz model, will change later based on settings
        double m_fMinSpanMHZ = 0.112;       //Min valid span in MHZ for connected model
        double m_fMaxSpanMHZ = 100.0;       //Max valid span in MHZ for connected model
        double m_fMinFreqMHZ = 430.0;       //Min valid frequency in MHZ for connected model
        double m_fMaxFreqMHZ = 440.0;       //Max valid frequency in MHZ for connected model

        double m_fRBWKHZ = 0.0;             //RBW in use
        float m_fOffset_dBm = 0.0f;         //Manual offset of the amplitude reading

        string m_sFilenameRFE="";           //RFE data file name
        string m_sReportFilePath="";        //Path and name of the report log file

        Boolean m_bPortConnected = false;   //Will be true while COM port is connected, as IsOpen() is not reliable

        float[,] m_arrData;                 //Collection of available spectrum data
        UInt16 m_nDataIndex = 0;            //Index pointing to latest spectrum data received
        UInt16 m_nMaxDataIndex = 0;         //Max value for m_nDataIndex with available data

        byte[,] m_arrRemoteScreenData;      //Collection of available remote screen data
        UInt16 m_nScreenIndex = 0;          //Index pointing to the latest Dump screen received
        UInt16 m_nMaxScreenIndex = 0;       //Max value for m_nScreenIndex with available data

        const float m_fSizeX = 130;         //Size of the dump screen in pixels (128x64 + 2 border)
        const float m_fSizeY = 66;

        string[] m_arrConnectedPorts;      //Collection of available COM ports
        string[] m_arrValidCP2101Ports;    //Collection of true CP2102 COM ports

        double m_fStartFrequencyMHZ = 0.0;  //In MHZ
        double m_fStepFrequencyMHZ = 0.0;   //In MHZ
        double m_fRefFrequencyMHZ = 0.0;    //Reference frequency used for decoder and other zero span functions

        Queue m_arrReceivedStrings;         //Queue of strings received from COM port

        System.Threading.Thread m_ReceiveThread;    //Thread to process received RS232 activity
        Boolean m_bRunReceiveThread;        //Run thread (true) or temporarily stop it (false)

        bool m_bHoldMode = false;           //True when HOLD is active

        bool m_bDrawRealtime = true;        //True if realtime data should be displayed
        bool m_bDrawAverage = true;         //True if averaged data should be displayed
        bool m_bDrawMax = true;             //True if max data should be displayed
        bool m_bDrawMin = true;             //True if min data should be displayed
        bool m_bShowPeaks = true;           //True if peak text with MHZ/dBm should be displayed

        bool m_bDark = false;               //True for a Dark mode combination active

        double m_fAmplitudeTop = -30;       //dBm for top graph limit
        double m_fAmplitudeBottom = MIN_AMPLITUDE_DBM;   //dBm for bottom graph limit

        bool m_bFirstTick = true;           //Used to put some text and guarantee action done once after mainform load
        bool m_bFirstText = true;           //First report text printed

        Pen m_PenDarkBlue;                  //Graphis objects cached to reduce drawing overhead
        Pen m_PenRed;
        Brush m_BrushDarkBlue;

        TextObj m_RealtimePeak, m_AveragePeak, m_MaxPeak; //Max dynamic text
        TextObj[] m_arrWiFiBarText;         //Text for the 13 Wifi channels
        TextObj m_RFEConfig;                //Configuration data received from RF Explorer
        string m_sRFExplorerFirmware;       //Firmware version of the connected RF Explorer

        bool m_bIsWinXP = false;            //True if it is a Windows XP platform, which has some GUI differences with Win7/Vista

        LineItem m_AvgLine, m_MinLine, m_RealtimeLine, m_MaxLine;   //Line curve item for the analyzer zed graph
        PointPairList m_PointListRealtime, m_PointListMax, m_PointListMin, m_PointListAverage;  //pair list used by the line curve items
        BarItem m_MaxBar;                   //Bar curve used by the wifi analyzer

        Button[] m_arrAnalyzerButtonList=new Button[14];

        string m_sAppDataFolder = "";       //Default folder based on %APPDATA% to store log and report files
        string m_sDefaultDataFolder = "";   //Default folder to store CSV and RFE or other data files
        string m_sSettingsFile = "";        //Filename and path of the named settings file
        #endregion

        #region Main Window
        public MainForm(string sFile)
        {
            m_sFilenameRFE = sFile;

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            InitializeComponent();
        }

        private void CreateSettingsSchema()
        {
            try
            {
                DataTable objTableCommon = m_DataSettings.Tables.Add("Common_Settings");

                objTableCommon.Columns.Add(new DataColumn("Name", System.Type.GetType("System.String")));
                objTableCommon.Columns.Add(new DataColumn("StartFreq", System.Type.GetType("System.Double")));
                objTableCommon.Columns.Add(new DataColumn("StepFreq", System.Type.GetType("System.Double")));
                objTableCommon.Columns.Add(new DataColumn("TopAmp", System.Type.GetType("System.Double")));
                objTableCommon.Columns.Add(new DataColumn("BottomAmp", System.Type.GetType("System.Double")));
                objTableCommon.Columns.Add(new DataColumn("Calculator", System.Type.GetType("System.UInt16")));
                objTableCommon.Columns.Add(new DataColumn("ViewAvg", System.Type.GetType("System.Boolean")));
                objTableCommon.Columns.Add(new DataColumn("ViewRT", System.Type.GetType("System.Boolean")));
                objTableCommon.Columns.Add(new DataColumn("ViewMin", System.Type.GetType("System.Boolean")));
                objTableCommon.Columns.Add(new DataColumn("ViewMax", System.Type.GetType("System.Boolean")));

                DataRow objRow=objTableCommon.NewRow();
                objRow["Name"] = "Default";
                objRow["StartFreq"] = 430.000;
                objRow["StepFreq"] = 0.500;
                objRow["TopAmp"] = 5;
                objRow["BottomAmp"] = -120;
                objRow["Calculator"] = 10;
                objRow["ViewAvg"] = true;
                objRow["ViewRT"] = true;
                objRow["ViewMin"] = false;
                objRow["ViewMax"] = false;
                objTableCommon.Rows.Add(objRow);

                m_DataSettings.WriteXml(m_sSettingsFile,XmlWriteMode.WriteSchema);
            }
            catch (Exception objEx)
            {
                MessageBox.Show(objEx.Message);
            }
        }

        private void btnLoadSettings_Click(object sender, EventArgs e)
        {
            RestoreSettingsXML(menuComboSavedOptions.Text);
        }
        
        private void menuComboSavedOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnDelSettings.Enabled = menuComboSavedOptions.Text != "Default";
        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            string sItem = menuComboSavedOptions.Text;
            SaveSettingsXML(sItem);
            PopulateReadedSettings();
            menuComboSavedOptions.SelectedItem = sItem;
        }

        private void btnDelSettings_Click(object sender, EventArgs e)
        {
            string sItem = menuComboSavedOptions.Text;
            DeleteSettingsXML(sItem);
            menuComboSavedOptions.Items.Remove(sItem);
            menuComboSavedOptions.SelectedItem = "Default";
        }

        private void DefineGraphColors(ZedGraphControl objGraph)
        {
            GraphPane myPane = objGraph.GraphPane;

            if (m_bDark)
            {
                this.BackColor = Color.DarkGray;
                tabSpectrumAnalyzer.BackColor = Color.DarkGray;
                tabReport.BackColor = Color.DarkGray;
                tabRemoteScreen.BackColor = Color.DarkGray;
                tabConfiguration.BackColor = Color.DarkGray;

                myPane.Fill = new Fill(Color.Black);
                myPane.Chart.Fill = new Fill(Color.Black);
                myPane.Title.FontSpec.FontColor = Color.White;

                m_RFEConfig.FontSpec.FontColor = Color.LightGray;
                m_RFEConfig.FontSpec.DropShadowColor = Color.DarkRed;

                myPane.YAxis.Title.FontSpec.FontColor = Color.White;
                myPane.XAxis.Title.FontSpec.FontColor = Color.White;
                myPane.YAxis.Scale.FontSpec.FontColor = Color.White;
                myPane.XAxis.Scale.FontSpec.FontColor = Color.White;

                myPane.YAxis.MajorGrid.Color = Color.Gray;
                myPane.YAxis.MinorGrid.Color = Color.Gray;
                myPane.XAxis.MajorGrid.Color = Color.Gray;
                myPane.XAxis.MinorGrid.Color = Color.Gray;

                myPane.YAxis.MajorTic.Color = Color.Gray;
                myPane.YAxis.MinorTic.Color = Color.Gray;
                myPane.XAxis.MajorTic.Color = Color.Gray;
                myPane.XAxis.MinorTic.Color = Color.Gray;

                myPane.Chart.Border.Color = Color.Gray;
                myPane.Chart.Border.IsAntiAlias = true;

                myPane.Legend.FontSpec.FontColor = Color.White;
                myPane.Legend.Fill.Color = Color.Black;
                myPane.Legend.Fill.SecondaryValueGradientColor = Color.Black;
                myPane.Legend.Fill = new Fill(Color.Black);
            }
            else
            {
                this.BackColor = Color.LightYellow;
                tabSpectrumAnalyzer.BackColor = Color.LightYellow;
                tabReport.BackColor = Color.LightYellow;
                tabRemoteScreen.BackColor = Color.LightYellow;
                tabConfiguration.BackColor = Color.LightYellow;
                tabWaterfall.BackColor = Color.LightYellow;

                myPane.Fill = new Fill(Color.White, Color.LightYellow, 90.0f);
                myPane.Chart.Fill = new Fill(Color.White, Color.LightYellow, 90.0f);

                m_RFEConfig.FontSpec.FontColor = Color.DarkBlue;
                m_RFEConfig.FontSpec.DropShadowColor = Color.LightGray;

                myPane.Title.FontSpec.FontColor = Color.Black;

                myPane.YAxis.Title.FontSpec.FontColor = Color.Black;
                myPane.XAxis.Title.FontSpec.FontColor = Color.Black;
                myPane.YAxis.Scale.FontSpec.FontColor = Color.Black;
                myPane.XAxis.Scale.FontSpec.FontColor = Color.Black;

                myPane.YAxis.MajorGrid.Color = Color.Gray;
                myPane.YAxis.MinorGrid.Color = Color.Gray;
                myPane.XAxis.MajorGrid.Color = Color.Gray;
                myPane.XAxis.MinorGrid.Color = Color.Gray;

                myPane.YAxis.MajorTic.Color = Color.Gray;
                myPane.YAxis.MinorTic.Color = Color.Gray;
                myPane.XAxis.MajorTic.Color = Color.Gray;
                myPane.XAxis.MinorTic.Color = Color.Gray;

                myPane.Chart.Border.Color = Color.Gray;
                myPane.Chart.Border.IsAntiAlias = true;

                myPane.Legend.FontSpec.FontColor = Color.Black;
                myPane.Legend.Fill.Color = Color.LightYellow;
                myPane.Legend.Fill = new Fill(Color.LightYellow);            
            }

            myPane.YAxis.Title.FontSpec.Size = 13;
            myPane.XAxis.Title.FontSpec.Size = 13;
            myPane.YAxis.Scale.FontSpec.Size = 10;
            myPane.XAxis.Scale.FontSpec.Size = 10;

            // Fill the axis background with a gradient
            myPane.Legend.IsHStack = true;
            myPane.Legend.FontSpec.Size = 12;

            // Enable scrollbars if needed
            objGraph.IsAutoScrollRange = true;
        }

        private void InitializeSpectrumAnalyzerGraph()
        {
            // Get a reference to the GraphPane instance in the ZedGraphControl
            GraphPane myPane = zedSpectrumAnalyzer.GraphPane;

            m_PointListRealtime = new PointPairList();
            m_PointListMax = new PointPairList();
            m_PointListMin = new PointPairList();
            m_PointListAverage = new PointPairList();

            m_MaxBar = zedSpectrumAnalyzer.GraphPane.AddHiLowBar("Max", m_PointListMax, Color.Red);
            m_MaxBar.Bar.Border.Color = Color.DarkRed;
            m_MaxBar.Bar.Border.Width = 2;
            m_AvgLine = zedSpectrumAnalyzer.GraphPane.AddCurve("Avg", m_PointListAverage, Color.Brown, SymbolType.None);
            m_AvgLine.Line.Width = 2;
            m_AvgLine.Line.SmoothTension = 0.3F;
            m_AvgLine.Line.IsSmooth = true;
            m_MinLine = zedSpectrumAnalyzer.GraphPane.AddCurve("Min", m_PointListMin, Color.DarkGreen, SymbolType.None);
            m_MinLine.Line.Width = 2;
            m_MinLine.Line.SmoothTension = 0.3F;
            m_MinLine.Line.IsSmooth = true; 
            m_RealtimeLine = zedSpectrumAnalyzer.GraphPane.AddCurve("Realtime", m_PointListRealtime, Color.Blue, SymbolType.None);
            m_RealtimeLine.Line.Width = 3;
            m_RealtimeLine.Line.SmoothTension = 0.2F;
            m_RealtimeLine.Line.IsSmooth = true;
            m_MaxLine = zedSpectrumAnalyzer.GraphPane.AddCurve("Max", m_PointListMax, Color.Red, SymbolType.None);
            m_MaxLine.Line.Width = 2;
            m_MaxLine.Line.SmoothTension = 0.3F;
            m_MaxLine.Line.IsSmooth = true;

            foreach (CurveItem objCurve in zedSpectrumAnalyzer.GraphPane.CurveList)
            {
                objCurve.IsVisible = false;
                objCurve.Label.IsVisible = false;
            }

            m_arrAnalyzerButtonList[0] = btnTop5plus;
            m_arrAnalyzerButtonList[1] = btnTop5minus;
            m_arrAnalyzerButtonList[2] = btnMoveFreqDecLarge;
            m_arrAnalyzerButtonList[3] = btnMoveFreqDecSmall;
            m_arrAnalyzerButtonList[4] = btnSpanInc;
            m_arrAnalyzerButtonList[5] = btnSpanMax;
            m_arrAnalyzerButtonList[6] = btnSpanDefault;
            m_arrAnalyzerButtonList[7] = btnCenterMark;
            m_arrAnalyzerButtonList[8] = btnSpanMin;
            m_arrAnalyzerButtonList[9] = btnSpanDec;
            m_arrAnalyzerButtonList[10] = btnMoveFreqIncLarge;
            m_arrAnalyzerButtonList[11] = btnMoveFreqIncSmall;
            m_arrAnalyzerButtonList[12] = btnBottom5plus;
            m_arrAnalyzerButtonList[13] = btnBottom5minus;

            // Set the titles and axis labels
            //myPane.Title.FontSpec.Size = 10;
            //myPane.Title.IsVisible = false;
            myPane.XAxis.Title.Text = "Frequency (MHZ)";
            myPane.XAxis.Scale.MajorStep = 1.0;
            myPane.XAxis.Scale.MinorStep = 0.2;

            myPane.Margin.Left = 20;
            myPane.Margin.Right = -5;

            // Show the x axis grid
            myPane.XAxis.MajorGrid.IsVisible = true;
            myPane.XAxis.Type = AxisType.Linear;

            myPane.YAxis.Title.Text = "Amplitude (dBm)";
            //myPane.YAxis.Scale.FontSpec.FontColor = Color.Yellow;
            //myPane.YAxis.Title.FontSpec.FontColor = Color.Blue;
            myPane.YAxis.Type = AxisType.Linear;
            // turn off the opposite tics so the Y tics don't show up on the Y2 axis
            myPane.YAxis.MajorTic.IsOpposite = false;
            myPane.YAxis.MinorTic.IsOpposite = false;
            myPane.YAxis.MajorGrid.IsVisible = true;
            // Don't display the Y zero line
            myPane.YAxis.MajorGrid.IsZeroLine = false;
            // Align the Y axis labels so they are flush to the axis
            myPane.YAxis.Scale.Align = AlignP.Inside;
            // Manually set the axis range
            myPane.YAxis.Scale.Min = MIN_AMPLITUDE_DBM;
            myPane.YAxis.Scale.Max = MAX_AMPLITUDE_DBM;
            myPane.YAxis.Scale.MajorStep = 20.0;
            myPane.YAxis.Scale.MinorStep = 5.0;

            zedSpectrumAnalyzer.IsShowPointValues = true;
            zedSpectrumAnalyzer.PointValueEvent += new ZedGraphControl.PointValueHandler(SpectrumAnalyzerPointValueHandler);

            myPane.BarSettings.Type = BarType.Overlay;
            myPane.BarSettings.MinBarGap = 0.1f;
            myPane.BarSettings.MinClusterGap = 0.1f;
            myPane.BarSettings.ClusterScaleWidthAuto = false;
            myPane.BarSettings.ClusterScaleWidth = 5.0f;

            m_RFEConfig = new TextObj("RF Explorer DISCONNECTED", 0.01, 0.02, CoordType.ChartFraction);
            m_RFEConfig.IsClippedToChartRect = true;
            //m_RFEConfig.ZOrder = 0;
            m_RFEConfig.FontSpec.FontColor = Color.DarkGray;
            m_RFEConfig.Location.AlignH = AlignH.Left;
            m_RFEConfig.Location.AlignV = AlignV.Top;
            m_RFEConfig.FontSpec.IsBold = true;
            m_RFEConfig.FontSpec.Size = 8f;
            m_RFEConfig.FontSpec.Border.IsVisible = false;
            m_RFEConfig.FontSpec.Fill.IsVisible = false;
            m_RFEConfig.FontSpec.StringAlignment = StringAlignment.Near;
            m_RFEConfig.FontSpec.IsDropShadow = true;
            m_RFEConfig.FontSpec.DropShadowOffset = 0.1f;
            m_RFEConfig.FontSpec.Family = "Arial Narrow";
            myPane.GraphObjList.Add(m_RFEConfig);

            m_RealtimePeak = new TextObj("", 0, 0, CoordType.AxisXYScale);
            m_RealtimePeak.IsClippedToChartRect = true;
            m_RealtimePeak.Location.AlignH = AlignH.Center;
            m_RealtimePeak.Location.AlignV = AlignV.Bottom;
            m_RealtimePeak.FontSpec.Size = 8f;
            m_RealtimePeak.FontSpec.Border.IsVisible = false;
            m_RealtimePeak.FontSpec.FontColor = Color.Blue;
            m_RealtimePeak.FontSpec.StringAlignment = StringAlignment.Center;
            m_RealtimePeak.FontSpec.Fill.IsVisible = false;
            myPane.GraphObjList.Add(m_RealtimePeak);

            m_MaxPeak= new TextObj("", 0, 0, CoordType.AxisXYScale);
            m_MaxPeak.IsClippedToChartRect = true;
            m_MaxPeak.Location.AlignH = AlignH.Center;
            m_MaxPeak.Location.AlignV = AlignV.Bottom;
            m_MaxPeak.FontSpec.Size = 8;
            m_MaxPeak.FontSpec.Border.IsVisible = false;
            m_MaxPeak.FontSpec.FontColor = Color.Red;
            m_MaxPeak.FontSpec.StringAlignment = StringAlignment.Center;
            m_MaxPeak.FontSpec.Fill.IsVisible = false;
            myPane.GraphObjList.Add(m_MaxPeak);

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

            m_AveragePeak= new TextObj("", 0, 0, CoordType.AxisXYScale);
            m_AveragePeak.IsClippedToChartRect = true;
            m_AveragePeak.Location.AlignH = AlignH.Center;
            m_AveragePeak.Location.AlignV = AlignV.Bottom;
            m_AveragePeak.FontSpec.Size = 8;
            m_AveragePeak.FontSpec.Border.IsVisible = false;
            m_AveragePeak.FontSpec.FontColor = Color.Brown;
            m_AveragePeak.FontSpec.StringAlignment = StringAlignment.Center;
            m_AveragePeak.FontSpec.Fill.IsVisible = false;
            myPane.GraphObjList.Add(m_AveragePeak);

            DefineGraphColors(zedSpectrumAnalyzer);
        }

        private void menuAutoLCDOff_Click(object sender, EventArgs e)
        {
            if (menuAutoLCDOff.Checked)
            {
                SendCommand("L0");
                chkDumpScreen.Checked = false;
            }
            else
            {
                SendCommand("L1");
            }
        }

        private void GetNewFilename()
        {
            //New unique filename to store data based on date and time
            m_sFilenameRFE = "RFExplorer_Client_Data_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".rfe";
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //up to m_nTotalBufferSize pages of m_nFreqSpectrumSteps bytes each
            m_arrData               = new float[m_nTotalBufferSize, MAX_SPECTRUM_STEPS]; 

            m_arrRemoteScreenData   = new byte[m_nTotalBufferSize, 128 * 8];
            m_arrRemoteScreenData.Initialize();
            m_nScreenIndex          = 0;
            m_nMaxScreenIndex       = 0;

            toolStripMemory.Maximum = m_nTotalBufferSize;
            toolStripMemory.Step    = m_nTotalBufferSize / 25;

            numericSampleSA.Minimum = 0;
            numericSampleSA.Maximum = m_nTotalBufferSize;
            numericSampleSA.Value   = 0;

            numScreenIndex.Minimum  = 0;
            numScreenIndex.Maximum  = m_nTotalBufferSize;
            numScreenIndex.Value    = 0;

            numericIterations.Maximum = m_nTotalBufferSize;
            numericIterations.Value = 10;

            m_PenDarkBlue           = new Pen(Color.DarkBlue, 1);
            m_PenRed                = new Pen(Color.Red, 1);
            m_BrushDarkBlue         = new SolidBrush(Color.DarkBlue);

            m_bIsWinXP = (Environment.OSVersion.Version.Major <= 5);

            try
            {
                m_serialPortObj             = new SerialPort();

                LoadProperties();

                GetConnectedPorts();
                InitializeSpectrumAnalyzerGraph();
                SetupSpectrumAnalyzerAxis();

                UpdateButtonStatus();
                m_arrReceivedStrings        = new Queue();
                m_bRunReceiveThread         = true;
                ThreadStart threadDelegate  = new ThreadStart(this.ReceiveThreadfunc);
                m_ReceiveThread             = new Thread(threadDelegate);
                m_ReceiveThread.Start();

                chkHoldMode.Checked = ! chkRunMode.Checked;
                chkHoldDecoder.Checked = !chkRunDecoder.Checked;

                if (m_sFilenameRFE != "")
                {
                    LoadFileRFE(m_sFilenameRFE);
                }
                else
                {
                    if (m_arrValidCP2101Ports != null && m_arrValidCP2101Ports.Length == 1)
                    {
                        ManualConnect();
                    }
                }
                m_timer_receive.Enabled = true;
            }
            catch
            {
                ReportLog("Error in MainForm_Load", true);
            }
            textBox_message.Focus();
        }

        private void GetConnectedPorts()
        {
            try
            {
                COMPortCombo.DataSource = null;

                m_arrConnectedPorts = System.IO.Ports.SerialPort.GetPortNames();

                GetValidCOMPorts();
                if (m_arrValidCP2101Ports != null && m_arrValidCP2101Ports.Length > 0)
                {
                    COMPortCombo.DataSource = m_arrValidCP2101Ports;
                    COMPortCombo.SelectedItem = RFExplorerClient.Properties.Settings.Default.COMPort;
                }
                else
                    ReportLog("ERROR: No valid COM ports available\r\nConnect RFExplorer and click on [*]", false);
            }
            catch (Exception obEx)
            {
                ReportLog("Error scanning COM ports: " + obEx.Message, true); 
            }
        }

        private void UpdateButtonStatus()
        {
            btnConnect.Enabled = !m_bPortConnected && (COMPortCombo.Items.Count>0);
            btnDisconnect.Enabled = m_bPortConnected;
            COMPortCombo.Enabled = !m_bPortConnected;
            comboBaudRate.Enabled = !m_bPortConnected;
            btnRescan.Enabled = !m_bPortConnected;
            chkDumpScreen.Checked = chkDumpScreen.Checked && !menuAutoLCDOff.Checked && m_bPortConnected;
            chkDumpScreen.Enabled = m_bPortConnected && !menuAutoLCDOff.Checked;

            btnSendCmd.Enabled = m_bPortConnected;

            groupFreqSettings.Enabled = m_bPortConnected;
            groupDemodulator.Enabled = m_bPortConnected;
            chkHoldMode.Enabled = m_bPortConnected;
            chkRunMode.Enabled = m_bPortConnected;
            chkRunDecoder.Enabled = m_bPortConnected;
            chkHoldDecoder.Enabled = m_bPortConnected;

            chkCalcRealtime.Checked = m_bDrawRealtime;
            chkCalcAverage.Checked = m_bDrawAverage;
            chkCalcMax.Checked = m_bDrawMax;
            chkCalcMin.Checked = m_bDrawMin;

            btnSaveRemoteBitmap.Enabled = m_nMaxScreenIndex > 0;
            btnSaveRemoteVideo.Enabled = m_nMaxScreenIndex > 0;

            panelConfiguration.Enabled = true;
            groupCalibration.Enabled = m_bPortConnected;

            for (int nInd = 0; nInd < m_arrAnalyzerButtonList.Length; nInd++)
            {
                m_arrAnalyzerButtonList[nInd].Enabled = m_bPortConnected;
            }
        }

        private void ConnectPort(string PortName)
        {
            Cursor.Current=Cursors.WaitCursor;
            try
            {
                Monitor.Enter(m_serialPortObj);

                m_serialPortObj.BaudRate        = Convert.ToInt32(comboBaudRate.SelectedItem.ToString());
                m_serialPortObj.DataBits        = 8;
                m_serialPortObj.StopBits        = StopBits.One;
                m_serialPortObj.Parity          = Parity.None;
                m_serialPortObj.PortName        = PortName;
                m_serialPortObj.ReadTimeout     = 100;
                m_serialPortObj.WriteBufferSize = 1024;
                m_serialPortObj.ReadBufferSize  = 2048;
                m_serialPortObj.Open();
                m_serialPortObj.Handshake       = Handshake.None;
                m_serialPortObj.Encoding        = Encoding.GetEncoding(28591); //this is the great trick to use ASCII and binary together

                m_bPortConnected = true;
                UpdateButtonStatus();

                m_bHoldMode = false;
                UpdateFeedMode();
                SaveProperties();

                ReportLog("Connected: " + m_serialPortObj.PortName.ToString() + ", " + m_serialPortObj.BaudRate.ToString() + " bauds", true);

                Thread.Sleep(500);
                Thread.Sleep(200);
                AskConfigData();
                Thread.Sleep(500);
                menuAutoLCDOff_Click(null, null);
            }
            catch (Exception obException) 
            { 
                ReportLog("ERROR ConnectPort: " + obException.Message, false); 
            }
            finally
            {
                Monitor.Exit(m_serialPortObj);
            }
            Cursor.Current = Cursors.Default;
        }

        private void LoadProperties()
        {
            //Load standard WinForm .NET properties
            comboBaudRate.SelectedItem  = RFExplorerClient.Properties.Settings.Default.COMSpeed;
            COMPortCombo.SelectedItem   = RFExplorerClient.Properties.Settings.Default.COMPort;
            menuSaveOnClose.Checked     = RFExplorerClient.Properties.Settings.Default.SaveOnClose;
            numericZoom.Value           = RFExplorerClient.Properties.Settings.Default.ScreenZoom;
            m_bShowPeaks                = RFExplorerClient.Properties.Settings.Default.ViewPeaks;
            menuShowControlArea.Checked = RFExplorerClient.Properties.Settings.Default.ShowControlArea;
            m_bDark                     = RFExplorerClient.Properties.Settings.Default.DarkMode;
            menuAutoLCDOff.Checked      = RFExplorerClient.Properties.Settings.Default.AutoLCDOff;
            menuContinuousLog.Checked   = RFExplorerClient.Properties.Settings.Default.ContinuousLog;
            string sTemp                = RFExplorerClient.Properties.Settings.Default.DefaultDataFolder;
            comboCSVFieldSeparator.SelectedIndex = (int)RFExplorerClient.Properties.Settings.Default.CSVDelimiter;

            //Configuring and loading default folders
            m_sAppDataFolder = Environment.GetEnvironmentVariable("APPDATA") + "\\RFExplorer";
            m_sAppDataFolder = m_sAppDataFolder.Replace("\\\\", "\\");
            if (m_sDefaultDataFolder == "")
            {
                m_sDefaultDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\RFExplorer";
                m_sDefaultDataFolder = m_sDefaultDataFolder.Replace("\\\\", "\\");
                edDefaultFilePath.Text = m_sDefaultDataFolder;
            }
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


            m_sSettingsFile = m_sAppDataFolder + "\\RFExplorer_Settings.xml";
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
            RestoreSettingsXML("Default");

            if (sTemp != "")
            {
                m_sDefaultDataFolder = sTemp;
            }
            edDefaultFilePath.Text = m_sDefaultDataFolder;

            comboCustomCommand.DataSource = RFExplorerClient.Properties.Settings.Default.CustomCommands;
        }

        private void PopulateReadedSettings()
        {
            menuComboSavedOptions.Items.Clear();
            foreach (DataRow objRow in m_DataSettings.Tables["Common_Settings"].Rows)
            {
                menuComboSavedOptions.Items.Add((string)objRow["Name"]);
            }
            menuComboSavedOptions.SelectedItem = "Default";
        }

        private void SaveProperties()
        {
            if (COMPortCombo.Items.Count > 0)
            {
                RFExplorerClient.Properties.Settings.Default.COMPort = COMPortCombo.SelectedValue.ToString();
            }
            //No need to save it here, it is already saved in send button.
            //RFExplorerClient.Properties.Settings.Default.CustomCommands

            RFExplorerClient.Properties.Settings.Default.COMSpeed = comboBaudRate.SelectedItem.ToString();
            RFExplorerClient.Properties.Settings.Default.SaveOnClose = menuSaveOnClose.Checked;
            RFExplorerClient.Properties.Settings.Default.ScreenZoom = (int)numericZoom.Value;
            RFExplorerClient.Properties.Settings.Default.ViewPeaks = m_bShowPeaks;
            RFExplorerClient.Properties.Settings.Default.ShowControlArea = menuShowControlArea.Checked;
            RFExplorerClient.Properties.Settings.Default.DarkMode = m_bDark;
            RFExplorerClient.Properties.Settings.Default.AutoLCDOff = menuAutoLCDOff.Checked;
            RFExplorerClient.Properties.Settings.Default.DefaultDataFolder = m_sDefaultDataFolder;
            RFExplorerClient.Properties.Settings.Default.CSVDelimiter = (uint)comboCSVFieldSeparator.SelectedIndex;
            RFExplorerClient.Properties.Settings.Default.ContinuousLog = menuContinuousLog.Checked;
            RFExplorerClient.Properties.Settings.Default.Save();

            SaveSettingsXML("Default");
        }

        private void DeleteSettingsXML(string sSettingsName)
        {
            try
            {
                DataRow[] objRowCol = m_DataSettings.Tables["Common_Settings"].Select("Name='" + sSettingsName + "'");
                if (objRowCol.Length > 0)
                {
                    m_DataSettings.Tables["Common_Settings"].Rows.Remove(objRowCol[0]);
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
                DataRow[] objRowCol = m_DataSettings.Tables["Common_Settings"].Select("Name='" + sSettingsName + "'");
                if (objRowCol.Length > 0)
                {
                    DataRow objRowDefault = objRowCol[0];
                    double fStartFrequencyMHZ = (double)objRowDefault["StartFreq"];
                    double fStepFrequencyMHZ = (double)objRowDefault["StepFreq"];
                    double fAmplitudeTop = (double)objRowDefault["TopAmp"];
                    double fAmplitudeBottom = (double)objRowDefault["BottomAmp"];
                    numericIterations.Value = (UInt16)objRowDefault["Calculator"];
                    m_bDrawAverage = (bool)objRowDefault["ViewAvg"];
                    m_bDrawRealtime = (bool)objRowDefault["ViewRT"];
                    m_bDrawMin = (bool)objRowDefault["ViewMin"];
                    m_bDrawMax = (bool)objRowDefault["ViewMax"];

                    if (m_bPortConnected == false)
                    {
                        //If device is disconnected, we just need to update visible parts of screen as otherwise it won't change
                        m_fAmplitudeTop = fAmplitudeTop;
                        m_fAmplitudeBottom = fAmplitudeBottom;

                        //Check to reinitiate buffer here, otherwise after changing it the receive function will not know the data was changed
                        if ((Math.Abs(m_fStartFrequencyMHZ - fStartFrequencyMHZ) >= 0.001) || (Math.Abs(m_fStepFrequencyMHZ - fStepFrequencyMHZ) >= 0.001))
                        {
                            m_fStartFrequencyMHZ = fStartFrequencyMHZ;
                            m_fStepFrequencyMHZ = fStepFrequencyMHZ;
                            m_nDataIndex = 0; //we cannot use previous data for avg, etc when new frequency range is selected
                            m_nMaxDataIndex = 0;
                        }
                        SetupSpectrumAnalyzerAxis(); //will update everything including the edit boxes
                    }
                    else
                    {
                        //if device is connected, we do not need to change anything: just ask the device to reconfigure and the new configuration will come back
                        SendNewConfig(fStartFrequencyMHZ, fStartFrequencyMHZ + fStepFrequencyMHZ * m_nFreqSpectrumSteps, fAmplitudeTop, fAmplitudeBottom);
                    }
                    UpdateButtonStatus();
                }
            }
            catch (Exception obEx)
            {
                ReportLog("ERROR RestoreSettingsXML:" + obEx.Message, true);
            }
        }

        private void SaveSettingsXML(string sSettingsName)
        {
            try
            {
                DataRow[] objRowCol = m_DataSettings.Tables["Common_Settings"].Select("Name='" + sSettingsName + "'");
                DataRow objRow = null;

                if (objRowCol.Length == 0)
                {
                    objRow = m_DataSettings.Tables["Common_Settings"].NewRow();
                    objRow["Name"] = sSettingsName;
                    m_DataSettings.Tables["Common_Settings"].Rows.Add(objRow);
                }
                else
                {
                    objRow = objRowCol[0];
                }

                objRow["StartFreq"] = m_fStartFrequencyMHZ;
                objRow["StepFreq"] = m_fStepFrequencyMHZ;
                objRow["TopAmp"] = m_fAmplitudeTop;
                objRow["BottomAmp"] = m_fAmplitudeBottom;
                objRow["Calculator"] = (int)numericIterations.Value;
                objRow["ViewAvg"] = m_bDrawAverage;
                objRow["ViewRT"] = m_bDrawRealtime;
                objRow["ViewMin"] = m_bDrawMin;
                objRow["ViewMax"] = m_bDrawMax;
                m_DataSettings.WriteXml(m_sSettingsFile, XmlWriteMode.WriteSchema);

                if (sSettingsName == "Default")
                {
                    //If we are saving the default value, that is because we are doing it automatically (e.g. when closing the port)
                    //Therefore select it as the default on screen too
                    menuComboSavedOptions.SelectedItem = "Default";
                }
            }
            catch (Exception obEx)
            {
                ReportLog("ERROR SaveSettingsXML:" + obEx.Message, true);
            }
        }

        private void ManualConnect()
        {
            ConnectPort(COMPortCombo.SelectedValue.ToString());
        }

        private void ClosePort()
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                Monitor.Enter(m_serialPortObj);

                if (m_serialPortObj.IsOpen)
                {
                    Thread.Sleep(200);
                    SendCommand("L1"); //restore LCD
                    Thread.Sleep(200);
                    SendCommand("CH"); //Switch data dump to off
                    Thread.Sleep(200);
                    //Close the port
                    ReportLog("Disconnected.", true);
                    m_serialPortObj.Close();
                }
            }
            catch { }
            finally
            {
                Monitor.Exit(m_serialPortObj);
            }
            m_bPortConnected = false;
            UpdateButtonStatus();
            GetConnectedPorts();
            UpdateFeedMode();

            Cursor.Current = Cursors.Default;
        }

        private void ReceiveThreadfunc()
        {
            while (m_bRunReceiveThread)
            {
                string strReceived = "";
                while (m_bPortConnected)
                {
                    string sNewText = "";

                    try
                    {
                        Monitor.Enter(m_serialPortObj);
                        sNewText = m_serialPortObj.ReadExisting();
                    }
                    catch (IOException) { }
                    catch (TimeoutException) { }
                    catch (Exception obExeption)
                    {
                        Monitor.Enter(m_arrReceivedStrings);
                        m_arrReceivedStrings.Enqueue(obExeption);
                        Monitor.Exit(m_arrReceivedStrings);
                    }
                    finally { Monitor.Exit(m_serialPortObj); }

                    if (sNewText.Length > 0)
                    {
                        strReceived += sNewText;
                        sNewText = "";
                    }
                    if (strReceived.Length > 10240)
                    {
                        //Safety code, some error prevented the string from being processed in several loop cycles. Reset it.
                        strReceived = "";
                    }
                    if (strReceived.Length > 0)
                    {
                        if (strReceived[0] == '$')
                        {
                            if ((strReceived.Length > 4) && (strReceived[1] == 'C'))
                            {
                                Byte nSize = Convert.ToByte(strReceived[3]);
                                if (strReceived.Length >= (nSize + 4))
                                {
                                    string sNewLine = strReceived.Substring(0, nSize + 4);
                                    string sLeftOver = strReceived.Substring(nSize + 4);
                                    strReceived = sLeftOver;
                                    Monitor.Enter(m_arrReceivedStrings);
                                    m_arrReceivedStrings.Enqueue(sNewLine);
                                    Monitor.Exit(m_arrReceivedStrings);
                                }
                            }
                            if ((strReceived.Length > 1) && (strReceived[1] == 'D'))
                            {
                                if (strReceived.Length >= (4 + 128 * 8))
                                {
                                    string sNewLine = "$D" + strReceived.Substring(2, 128 * 8);
                                    string sLeftOver = strReceived.Substring(4 + 128 * 8);
                                    strReceived = sLeftOver;
                                    Monitor.Enter(m_arrReceivedStrings);
                                    m_arrReceivedStrings.Enqueue(sNewLine);
                                    Monitor.Exit(m_arrReceivedStrings);
                                }
                            }
                            if ((strReceived.Length > 2) && (strReceived[1] == 'S'))
                            {
                                ushort nReceivedLength = (byte)strReceived[2];

                                if (strReceived.Length >= (3 + nReceivedLength + 2))
                                {
                                    if (nReceivedLength <= MAX_SPECTRUM_STEPS)
                                    {
                                        string sNewLine = "$S" + strReceived.Substring(3, nReceivedLength);
                                        Monitor.Enter(m_arrReceivedStrings);
                                        m_arrReceivedStrings.Enqueue(sNewLine);
                                        Monitor.Exit(m_arrReceivedStrings);
                                    }
                                    else
                                    {
                                        Monitor.Enter(m_arrReceivedStrings);
                                        m_arrReceivedStrings.Enqueue("Ignored $S of size " + nReceivedLength.ToString() + " expected " + m_nFreqSpectrumSteps.ToString());
                                        Monitor.Exit(m_arrReceivedStrings);
                                    }
                                    string sLeftOver = strReceived.Substring(3 + nReceivedLength + 2);
                                    strReceived = sLeftOver;
                                }
                            }
                            if ((strReceived.Length > 3) && (strReceived[1] == 's'))
                            {
                                ushort nReceivedLength = (byte)strReceived[2];
                                nReceivedLength = (ushort)(nReceivedLength * 0x100 + (byte)strReceived[3]);

                                if (strReceived.Length >= (4 + nReceivedLength + 2))
                                {
                                    if (nReceivedLength <= MAX_SPECTRUM_STEPS)
                                    {
                                        string sNewLine = "$S" + strReceived.Substring(4, nReceivedLength);
                                        Monitor.Enter(m_arrReceivedStrings);
                                        m_arrReceivedStrings.Enqueue(sNewLine);
                                        Monitor.Exit(m_arrReceivedStrings);
                                    }
                                    else
                                    {
                                        Monitor.Enter(m_arrReceivedStrings);
                                        m_arrReceivedStrings.Enqueue("Ignored $S of size " + nReceivedLength.ToString() + " expected " + m_nFreqSpectrumSteps.ToString());
                                        Monitor.Exit(m_arrReceivedStrings);
                                    }
                                    string sLeftOver = strReceived.Substring(4 + nReceivedLength + 2);
                                    strReceived = sLeftOver;
                                }
                            }
                            if ((strReceived.Length > 10) && (strReceived[1] == 'R'))
                            {
                                int nPos = strReceived.IndexOf('-');
                                if ((nPos > 2) && (nPos < 10))
                                {
                                    int nSize = Convert.ToInt32(strReceived.Substring(2, nPos - 2));
                                    if (strReceived.Length > (nSize + nPos + 2))
                                    {
                                        Monitor.Enter(m_arrReceivedStrings);
                                        m_arrReceivedStrings.Enqueue("Received RAW data " + nSize.ToString());
                                        m_arrReceivedStrings.Enqueue("$R" + strReceived.Substring(nPos + 1, nSize));
                                        Monitor.Exit(m_arrReceivedStrings);
                                        strReceived = strReceived.Substring(nSize + nPos + 1 + 2);
                                    }
                                }
                            }
                        }
                        else
                        {
                            int nEndPos = strReceived.IndexOf("\r\n");
                            if (nEndPos >= 0)
                            {
                                string sNewLine = strReceived.Substring(0, nEndPos);
                                string sLeftOver = strReceived.Substring(nEndPos + 2);
                                strReceived = sLeftOver;
                                Monitor.Enter(m_arrReceivedStrings);
                                m_arrReceivedStrings.Enqueue(sNewLine);
                                Monitor.Exit(m_arrReceivedStrings);
                            }
                            else
                            {
                                //diagnosis only
                                //Monitor.Enter(m_arrMessageStrings);
                                //m_arrMessageStrings.Enqueue("partial:"+strReceived);
                                //Monitor.Exit(m_arrMessageStrings);
                            }
                        }
                    }
                    Thread.Sleep(10);
                }

                Thread.Sleep(500);
            }
        }

        private void DisplayRequiredFirmware()
        {
            if (m_sRFExplorerFirmware != m_sRFExplorerFirmwareCertified)
            {
                UInt16 nMayorVerFound = Convert.ToUInt16(m_sRFExplorerFirmware.Substring(0, 2));
                UInt16 nMinorVerFound = Convert.ToUInt16(m_sRFExplorerFirmware.Substring(3, 2));
                UInt32 nVersionFound = (UInt32)(nMayorVerFound * 100 + nMinorVerFound);
                UInt16 nMayorVerTested = Convert.ToUInt16(m_sRFExplorerFirmwareCertified.Substring(0, 2));
                UInt16 nMinorVerTested = Convert.ToUInt16(m_sRFExplorerFirmwareCertified.Substring(3, 2));
                UInt32 nVersionTested = (UInt32)(nMayorVerTested * 100 + nMinorVerTested);

                if (nVersionFound > nVersionTested)
                {
                    ReportLog("\r\nWARNING: Firmware version connected v" + m_sRFExplorerFirmware + " is newer than the one certified v" + m_sRFExplorerFirmwareCertified + " for this version of PC Client.\r\n" +
                                  "         However, it may be compatible but you should check www.rf-explorer.com website\r\n"+
                                  "         to double check there is not a newer version available.\r\n", false);
                }
                else
                {
                    string sText = "RF Explorer device has an older firmware version " + m_sRFExplorerFirmware +
                        "\r\nPlease upgrade it to required version " + m_sRFExplorerFirmwareCertified +
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

        //Returns true if an event was received requiring redraw
        private bool ProcessReceivedString(bool bProcessAllEvents, out string sReceivedString)
        {
            bool bDraw = false;
            sReceivedString = "";

            try
            {
                Monitor.Enter(m_arrReceivedStrings);
                bool bWrongFormat = false;

                do
                {
                    if (m_arrReceivedStrings.Count == 0)
                        break;

                    string sLine = m_arrReceivedStrings.Dequeue().ToString();
                    sReceivedString = sLine;

                    if ((sLine.Length > 2) && (sLine.Substring(0, 2) == "$S") && (m_fStartFrequencyMHZ > 10.0))
                    {
                        if (!m_bHoldMode && m_nDataIndex < m_nTotalBufferSize)
                        {
                            for (int nInd = 0; nInd < m_nFreqSpectrumSteps; nInd++)
                            {
                                byte nVal = Convert.ToByte(sLine[2 + nInd]);
                                float fVal = nVal / -2.0f;

                                m_arrData[m_nDataIndex, nInd] = fVal + m_fOffset_dBm;
                            }
                            bDraw = true;
                            m_nDataIndex++;
                            if (m_nDataIndex >= m_nTotalBufferSize)
                            {
                                if (menuContinuousLog.Checked)
                                {
                                    GetNewFilename();
                                    SaveFile(m_sFilenameRFE);
                                    ReportLog("Buffer is full but continuous data log is active - Auto saved data file: " + m_sFilenameRFE, false);
                                }
                                else
                                {
                                    ReportLog("Buffer is full, previous data was lost - use menu option File->Continuous log to RFE data file.", false);
                                }
                                m_nDataIndex = 0;
                                m_nMaxDataIndex = 0;
                            }
                            m_nMaxDataIndex = m_nDataIndex;
                            numericSampleSA.Value = m_nDataIndex;
                        }
                    }
                    else if ((sLine.Length > 2) && sLine.Substring(0, 2) == "$D")
                    {
                        if (chkDumpScreen.Checked)
                        {
                            if (m_nScreenIndex <= m_nMaxScreenIndex)
                            {
                                //force to draw in a new position
                                m_nScreenIndex = m_nMaxScreenIndex;
                                m_nScreenIndex++;
                            }

                            //Update button status but first time only to minimize overhead
                            if (m_nMaxScreenIndex == 1)
                                UpdateButtonStatus();

                            if (m_nScreenIndex < m_nTotalBufferSize)
                            {
                                //Capture only if we are inside bounds
                                for (int nInd = 0; nInd < 128 * 8; nInd++)
                                {
                                    m_arrRemoteScreenData[m_nScreenIndex, nInd] = Convert.ToByte(sLine[nInd + 2]);
                                }
                                tabRemoteScreen.Invalidate();
                                m_nMaxScreenIndex = m_nScreenIndex;
                                m_nScreenIndex++;
                                numScreenIndex.Value = m_nScreenIndex;
                            }
                        }
                        else
                        {
                            //receiving Dump screen strings but it was disabled, resend now
                            SendCommand("D0");
                        }
                    }
                    else if ((sLine.Length > 6) && sLine.Substring(0, 6) == "#C2-M:")
                    {
                        ReportLog("Received RFExplorer device model info:" + sLine, true);
                        m_eMainBoardModel = (eModel)Convert.ToUInt16(sLine.Substring(6, 3));
                        m_eExpansionBoardModel = (eModel)Convert.ToUInt16(sLine.Substring(10, 3));
                        m_sRFExplorerFirmware = (sLine.Substring(14, 5));
                        DisplayRequiredFirmware();
                    }
                    else if ((sLine.Length > 6) && (sLine.Substring(0, 6) == "#C3-F:"))
                    {
                        ReportLog("Received configuration from RFExplorer device:" + sLine, true);
                        //#C3-F:<Ref_Freq>, <ExpModuleActive>, <CurrentMode>,  <Sample_Rate>, <Digital_Baudrate>, <Modulation><EOL>
                        //      6           14                 16              20             27                  34
                        if (sLine.Length >= 34)
                        {
                            double fRefMHZ = Convert.ToInt32(sLine.Substring(6, 7)) / 1000.0; //note it comes in KHZ
                            if (Math.Abs(fRefMHZ - m_fRefFrequencyMHZ) >= 0.001)
                            {
                                //We cannot use previous data if ref frequency changed
                                m_fRefFrequencyMHZ = fRefMHZ;
                                ReportLog("New reference Freq - buffer cleared.", true);
                            }

                            m_bExpansionBoardActive = (sLine[14] == '1');
                            if (m_bExpansionBoardActive)
                                m_eActiveModel = m_eExpansionBoardModel;
                            else
                                m_eActiveModel = m_eMainBoardModel;

                            m_eMode = (eMode)Convert.ToUInt16(sLine.Substring(16, 3));

                            double fSampleRateKHZ = Convert.ToInt32(sLine.Substring(20, 6));        //note it comes in KHZ=KS/sec
                            MainTab.SelectedTab = tabRAWDecoder;
                        }
                    }
                    else if ((sLine.Length > 6) && (sLine.Substring(0, 6) == "#C2-F:"))
                    {
                        ReportLog("Received configuration from RFExplorer device:" + sLine, true);
                        if (sLine.Length >= 60)
                        {
                            double fStartMHZ = Convert.ToInt32(sLine.Substring(6, 7)) / 1000.0; //note it comes in KHZ
                            double fStepMHZ = Convert.ToInt32(sLine.Substring(14, 7)) / 1000000.0;  //Note it comes in HZ
                            if ((Math.Abs(m_fStartFrequencyMHZ - fStartMHZ) >= 0.001) || (Math.Abs(m_fStepFrequencyMHZ - fStepMHZ) >= 0.001))
                            {
                                m_fStartFrequencyMHZ = fStartMHZ;
                                m_fStepFrequencyMHZ = fStepMHZ;

                                if (menuContinuousLog.Checked)
                                {
                                    GetNewFilename();
                                    SaveFile(m_sFilenameRFE);
                                    ReportLog("Buffer is cleared due to frequency range change, but continuous data log is active - Auto saved data file: " + m_sFilenameRFE, false);
                                }
                                else
                                {
                                    ReportLog("New Freq range - buffer cleared - use continuous data log to capture data automatically.", false);
                                }

                                m_nDataIndex = 0; //we cannot use previous data for avg, etc when new frequency range is selected
                                m_nMaxDataIndex = 0;
                            }
                            m_fAmplitudeTop = Convert.ToInt32(sLine.Substring(22, 4));
                            m_fAmplitudeBottom = Convert.ToInt32(sLine.Substring(27, 4));
                            m_nFreqSpectrumSteps = Convert.ToUInt16(sLine.Substring(32, 4));
                            m_bExpansionBoardActive = (sLine[37] == '1');
                            if (m_bExpansionBoardActive)
                            {
                                m_eActiveModel = m_eExpansionBoardModel;
                            }
                            else
                            {
                                m_eActiveModel = m_eMainBoardModel;
                            }
                            m_eMode = (eMode)Convert.ToUInt16(sLine.Substring(39, 3));

                            if (m_eMode == eMode.MODE_SPECTRUM_ANALYZER)
                            {
                                //objGraph is a standard graph
                            }
                            else if (m_eMode == eMode.MODE_WIFI_ANALYZER)
                            {
                                //objGraph is a bar chart
                                m_bDrawMax = true; //Max mode is the only one drawn with a bar, so it must be included here
                                m_bDrawAverage = false;
                                m_bDrawMin = false;
                                m_bDrawRealtime = false;
                                UpdateButtonStatus();
                                if (numericIterations.Value < 100)
                                    numericIterations.Value = 100; //at least 100 is required for a practical view in this mode
                                zedSpectrumAnalyzer.GraphPane.XAxis.Scale.Min = m_fStartFrequencyMHZ - 5.0f;
                                zedSpectrumAnalyzer.GraphPane.XAxis.Scale.Max = m_fStartFrequencyMHZ + m_nFreqSpectrumSteps * m_fStepFrequencyMHZ + 5.0f;
                            }

                            m_fMinFreqMHZ = Convert.ToInt32(sLine.Substring(43, 7)) / 1000.0;
                            m_fMaxFreqMHZ = Convert.ToInt32(sLine.Substring(51, 7)) / 1000.0;
                            m_fMaxSpanMHZ = Convert.ToInt32(sLine.Substring(59, 7)) / 1000.0;

                            if (sLine.Length > 66)
                            {
                                m_fRBWKHZ = Convert.ToInt32(sLine.Substring(67, 5));
                            }
                            if (sLine.Length > 72)
                            {
                                m_fOffset_dBm = Convert.ToInt32(sLine.Substring(73, 4));
                            }

                            btnCalibrate.Enabled = true; //calibration available for everybody but 2.4G at the moment
                            if (m_eActiveModel == eModel.MODEL_2400)
                            {
                                m_fMinSpanMHZ = 2.0;
                                btnCalibrate.Enabled = false;
                            }
                            else
                            {
                                m_fMinSpanMHZ = 0.112;
                            }

                            if (!m_bCalibrating)
                            {
                                string sModel = arrModels[(int)m_eMainBoardModel];
                                if (m_eActiveModel != m_eExpansionBoardModel)
                                    sModel += "(ACTIVE)";
                                string sExpansion;
                                if (m_eExpansionBoardModel == eModel.MODEL_NONE)
                                    sExpansion = " - No Expansion Module found";
                                else
                                {
                                    sExpansion = " - Expansion Module:" + arrModels[(int)m_eExpansionBoardModel];
                                    if (m_eActiveModel == m_eExpansionBoardModel)
                                        sExpansion += "(ACTIVE)";
                                }

                                m_RFEConfig.Text = "Client v" + Assembly.GetExecutingAssembly().GetName().Version.ToString() + " - Firmware v" + m_sRFExplorerFirmware +
                                " - Model:" + sModel + sExpansion +
                                " - Active range:" + m_fMinFreqMHZ.ToString() + "-" + m_fMaxFreqMHZ.ToString() + "MHz\n";

                                m_RFEConfig.Text += "Start: " + m_fStartFrequencyMHZ.ToString("f3") + "MHz - Stop:" + CalculateEndFrequencyMHZ().ToString("f3") +
                                    "MHz - Center:" + CalculateCenterFrequencyMHZ().ToString("f3") + "MHz - Span:" + CalculateFrequencySpanMHZ().ToString("f3") +
                                    "MHz - Sweep Step:" + (m_fStepFrequencyMHZ * 1000.0).ToString("f0") + "KHz";

                                if (m_fRBWKHZ > 0.0)
                                {
                                    m_RFEConfig.Text += " - RBW:" + m_fRBWKHZ.ToString("f0") + "KHz";
                                }
                                m_RFEConfig.Text += " - Amp Offset:" + m_fOffset_dBm.ToString("f0") + "dBm";

                                SetupSpectrumAnalyzerAxis();
                                SaveProperties();
                                MainTab.SelectedTab = tabSpectrumAnalyzer;
                            }
                        }
                        else
                            bWrongFormat = true;
                    }
                    else if (((sLine.Length > 12) && (sLine.Substring(0, 12) == "RF Explorer ")) ||
                             ((sLine.Length > 18) && (sLine.Substring(0, 18) == "(C) Ariel Rocholl "))
                            )
                    {
                        //RF Explorer device was reset for some reason, reconfigure client based on new configuration
                        AskConfigData();
                    }
                    else if ((sLine.Length > 6) && (sLine.Substring(0, 6) == "#C1-F:"))
                    {
                        bWrongFormat = true;
                    }
                    else
                    {
                        ReportLog(sLine, true);
                    }
                    if (bWrongFormat)
                    {
                        ReportLog("Received unexpected data from RFExplorer device:" + sLine, false);
                        ReportLog("Please update your RF Explorer to a recent firmware version and", false);
                        ReportLog("make sure you are using the latest version of this software.", false);
                        ReportLog("Visit http://www.rf-explorer/download for latest updates.", false);
                    }
                } while (bProcessAllEvents && (m_arrReceivedStrings.Count > 0));
            }
            catch (Exception obEx)
            {
                ReportLog("ProcessReceivedString: " + obEx.Message, true);
            }
            finally
            {
                Monitor.Exit(m_arrReceivedStrings);
            }

            return bDraw;
        }

        private void timer_receive_Tick(object sender, EventArgs e)
        {
            try
            {
                string sOut;
                bool bDraw = ProcessReceivedString(true, out sOut);

                if (bDraw)
                {
                    if (groupCOM.Parent == tabSpectrumAnalyzer)
                    {
                        DisplaySpectrumAnalyzerData();
                    }
                    else if (groupCOM.Parent == tabWaterfall)
                    {
                        UpdateWaterfall();
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
            m_bRunReceiveThread = false;
            if ((menuSaveOnClose.Checked || menuContinuousLog.Checked) && (m_nMaxDataIndex > 0) && (m_sFilenameRFE.Length==0))
            {
                GetNewFilename();
                SaveFile(m_sFilenameRFE);
            }
            ClosePort();
            SaveProperties();
        }

        private void SetupSpectrumAnalyzerAxis()
        {
            double fStart = m_fStartFrequencyMHZ;
            double fEnd = CalculateEndFrequencyMHZ()-m_fStepFrequencyMHZ;
            double fMajorStep = 1.0;

            zedSpectrumAnalyzer.GraphPane.XAxis.Scale.Min = fStart;
            zedSpectrumAnalyzer.GraphPane.XAxis.Scale.Max = fEnd;

            if (m_eMode == eMode.MODE_WIFI_ANALYZER)
            {
                //objGraph is a bar chart
                zedSpectrumAnalyzer.GraphPane.XAxis.Scale.Min = fStart-2.5f;
                zedSpectrumAnalyzer.GraphPane.XAxis.Scale.Max = fEnd+2.5f;
                //objGraph.GraphPane.XAxis.Type = AxisType.Ordinal;
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

            zedSpectrumAnalyzer.GraphPane.XAxis.Scale.MajorStep = fMajorStep;
            zedSpectrumAnalyzer.GraphPane.XAxis.Scale.MinorStep = fMajorStep/10.0;

            UpdateYAxis();

            UpdateDialogFromFreqSettings();
        }

        private void GetTopBottomDataRange(out double dTopRangeDBM, out double dBottomRangeDBM)
        {
            dTopRangeDBM = -120.0;
            dBottomRangeDBM = +5.0;
            for (int nIndSample = 0; nIndSample < m_nMaxDataIndex; nIndSample++)
            {
                for (int nIndStep = 0; nIndStep < m_nFreqSpectrumSteps; nIndStep++)
                {
                    double dValueDBM=m_arrData[nIndSample, nIndStep];
                    if (dTopRangeDBM < dValueDBM)
                        dTopRangeDBM = dValueDBM;
                    if (dBottomRangeDBM > dValueDBM)
                        dBottomRangeDBM = dValueDBM;
                }
            }
        }

        private void DisplaySpectrumAnalyzerData()
        {
            double fNoiseFloor = -120.0;

            m_fPeakValueMHZ = 0.0;
            m_fPeakValueAmp = fNoiseFloor;

            foreach (CurveItem objCurve in zedSpectrumAnalyzer.GraphPane.CurveList)
            {
                objCurve.Clear();
                objCurve.IsVisible = false;
                objCurve.Label.IsVisible = false;
            }
            m_PointListRealtime.Clear();
            m_PointListAverage.Clear();
            m_PointListMin.Clear();
            m_PointListMax.Clear();
            m_MaxBar.Clear();

            int nIndex = m_nDataIndex - 1;
            if (nIndex < 0)
                nIndex = 0;
            m_nDrawingIteration++;

            int nCalculatorMax = (int)numericIterations.Value;
            if (nCalculatorMax > nIndex)
                nCalculatorMax = nIndex;

            if ((m_nDrawingIteration & 0xf) == 0)
            {
                //Update screen status every 16 drawing iterations only to reduce overhead
                toolStripMemory.Value = nIndex;
                if (m_bPortConnected)
                    toolCOMStatus.Text = "Connected";
                else
                    toolCOMStatus.Text = "Disconnected";
            }

            double fRealtimeMax_Amp = fNoiseFloor;
            int fRealtimeMax_Iter = 0;
            double fAverageMax_Amp = fNoiseFloor;
            int fAverageMax_Iter = 0;
            double fMaxMax_Amp = fNoiseFloor;
            int fMaxMax_Iter = 0;

            m_AveragePeak.Text = "";
            m_RealtimePeak.Text = "";
            m_MaxPeak.Text = "";

            for (int nInd = 0; nInd < m_nFreqSpectrumSteps; nInd++)
            {
                if (nInd < m_arrWiFiBarText.Length)
                    m_arrWiFiBarText[nInd].Text = "";
                
                double fVal = m_arrData[nIndex, nInd];
                if (fVal > fRealtimeMax_Amp)
                {
                    fRealtimeMax_Amp = fVal;
                    fRealtimeMax_Iter = nInd;
                }

                double fFreq=m_fStartFrequencyMHZ + m_fStepFrequencyMHZ * nInd;

                double fMax = fVal;
                double fMin = fVal;
                double fValAvg = fVal;

                for (int nIterator = nIndex - nCalculatorMax; nIterator < nIndex; nIterator++)
                {
                    //Calculate average, max and min over Calculator range
                    double fVal2 = m_arrData[nIterator, nInd];

                    fMax = Math.Max(fMax, fVal2);
                    fMin = Math.Min(fMin, fVal2);

                    fValAvg += fVal2;
                }

                if (m_bDrawRealtime)
                    m_PointListRealtime.Add(fFreq, fVal, fNoiseFloor);
                if (m_bDrawMin)
                    m_PointListMin.Add(fFreq, fMin, fNoiseFloor);

                if (m_bDrawMax)
                {
                    m_PointListMax.Add(fFreq, fMax, fNoiseFloor);
                    if (fMax > fMaxMax_Amp)
                    {
                        fMaxMax_Amp = fMax;
                        fMaxMax_Iter = nInd;
                    }
                    if (m_eMode == eMode.MODE_WIFI_ANALYZER && m_bShowPeaks)
                    {
                        if (nInd < m_arrWiFiBarText.Length)
                        {
                            double fFreqMark = (m_fStartFrequencyMHZ + m_fStepFrequencyMHZ * nInd);
                            m_arrWiFiBarText[nInd].Text = "CH"+(nInd+1).ToString() +"\n"+ fFreqMark.ToString("0") + "MHZ\n" + fMax.ToString() + "dBm";
                            m_arrWiFiBarText[nInd].Location.X = fFreqMark;
                            m_arrWiFiBarText[nInd].Location.Y = fMax;
                            m_arrWiFiBarText[nInd].FontSpec.IsBold = false;
                            m_arrWiFiBarText[nInd].FontSpec.FontColor = Color.Red;
                        }
                    }
                }

                if (m_bDrawAverage || m_bCalibrating)
                {
                    fValAvg = fValAvg / (nCalculatorMax + 1);
                    m_PointListAverage.Add(fFreq, fValAvg, fNoiseFloor);
                    if (fValAvg > fAverageMax_Amp)
                    {
                        fAverageMax_Amp = fValAvg;
                        fAverageMax_Iter = nInd;
                    }
                }
            }

            //Get the m_fPeakValueMHZ/m_fPeakValueAmp based on what is available on calculation
            if (fAverageMax_Amp > fNoiseFloor)
            {
                m_fPeakValueMHZ = (m_fStartFrequencyMHZ + m_fStepFrequencyMHZ * fAverageMax_Iter);
                m_fPeakValueAmp = fAverageMax_Amp;
            }
            else if (fMaxMax_Amp > fNoiseFloor)
            {
                m_fPeakValueMHZ = (m_fStartFrequencyMHZ + m_fStepFrequencyMHZ * fMaxMax_Iter);
                m_fPeakValueAmp = fMaxMax_Amp;
            }
            else if (fRealtimeMax_Amp > fNoiseFloor)
            {
                m_fPeakValueMHZ = (m_fStartFrequencyMHZ + m_fStepFrequencyMHZ * fRealtimeMax_Iter);
                m_fPeakValueAmp = fRealtimeMax_Amp;
            }

            if (!m_bCalibrating)
            {
                //zedSpectrumAnalyzer.GraphPane.CurveList.Clear();
                if (m_bDrawAverage)
                {
                    m_AvgLine.Points = m_PointListAverage;
                    m_AvgLine.IsVisible = true;
                    m_AvgLine.Label.IsVisible = true;
                    double fFreqMark = (m_fStartFrequencyMHZ + m_fStepFrequencyMHZ * fAverageMax_Iter);
                    if (m_bShowPeaks)
                    {
                        m_AveragePeak.Text = fFreqMark.ToString("0.000") + "MHZ\n" + fAverageMax_Amp.ToString("0.00") + "dBm";
                        m_AveragePeak.Location.X = fFreqMark;
                        m_AveragePeak.Location.Y = fAverageMax_Amp;
                    }
                }
                if (m_bDrawRealtime)
                {
                    m_RealtimeLine.Points = m_PointListRealtime;
                    m_RealtimeLine.IsVisible = true;
                    m_RealtimeLine.Label.IsVisible = true;
                    double fFreqMark = (m_fStartFrequencyMHZ + m_fStepFrequencyMHZ * fRealtimeMax_Iter);
                    if (m_bShowPeaks)
                    {
                        m_RealtimePeak.Text = fFreqMark.ToString("0.000") + "MHZ\n" + fRealtimeMax_Amp.ToString("0.0") + "dBm";
                        m_RealtimePeak.Location.X = fFreqMark;
                        m_RealtimePeak.Location.Y = fRealtimeMax_Amp;
                    }
                }
                if (m_bDrawMax)
                {
                    if (m_eMode == eMode.MODE_SPECTRUM_ANALYZER)
                    {
                        m_MaxLine.Points = m_PointListMax;
                        m_MaxLine.IsVisible = true;
                        m_MaxLine.Label.IsVisible = true;
                        double fFreqMark = (m_fStartFrequencyMHZ + m_fStepFrequencyMHZ * fMaxMax_Iter);
                        if (m_bShowPeaks)
                        {
                            m_MaxPeak.Text = fFreqMark.ToString("0.000") + "MHZ\n" + fMaxMax_Amp.ToString("0.0") + "dBm";
                            m_MaxPeak.Location.X = fFreqMark;
                            m_MaxPeak.Location.Y = fMaxMax_Amp;
                        }
                    }
                    else if (m_eMode == eMode.MODE_WIFI_ANALYZER)
                    {
                        m_MaxBar.Points = m_PointListMax;
                        m_MaxBar.IsVisible = true;
                        if (m_bShowPeaks)
                        {
                            m_arrWiFiBarText[fMaxMax_Iter].FontSpec.IsBold = true;
                            if (m_bDark)
                                m_arrWiFiBarText[fMaxMax_Iter].FontSpec.FontColor = Color.LightCoral;
                            else
                                m_arrWiFiBarText[fMaxMax_Iter].FontSpec.FontColor = Color.DarkRed;
                        }
                    }
                }
                if (m_bDrawMin)
                {
                    m_MinLine.Points = m_PointListMin;
                    m_MinLine.IsVisible = true;
                    m_MinLine.Label.IsVisible = true;
                }

                zedSpectrumAnalyzer.Refresh();
            }
        }

        private string SpectrumAnalyzerPointValueHandler(ZedGraphControl control, GraphPane pane,
                    CurveItem curve, int iPt)
        {
            // Get the PointPair that is under the mouse
            PointPair pt = curve[iPt];

            return pt.X.ToString("f3") + "MHZ\r\n" + pt.Y.ToString("f2") + " dBm";
        }

        private void SendCommand(string sData)
        {
            try
            {
                if (m_bPortConnected)
                {
                    m_serialPortObj.Write("#" + Convert.ToChar(sData.Length + 2) + sData);
                    ReportLog("-> Sent to RFE: " + "#["+(sData.Length + 2).ToString()+"]"+sData, true);
                }
            }
            catch (Exception obEx)
            {
                ReportLog("SendCommand error: " + obEx.Message, false);
            }
        }

        private void AskConfigData()
        {
            if (m_bPortConnected)
            {
                SendCommand("C0");
            }
        }

        private void SendNewConfig(double fStartMHZ, double fEndMHZ, double fTopDBM, double fBottomDBM)
        {
            //#[32]C2-F:Sssssss,Eeeeeee,tttt,bbbb
            UInt32 nStartKhz = (UInt32)(fStartMHZ * 1000);
            UInt32 nEndKhz = (UInt32)(fEndMHZ * 1000);
            Int16 nTopDBM = (Int16)fTopDBM;
            Int16 nBottomDBM = (Int16)fBottomDBM;

            string sData = "C2-F:" +
                nStartKhz.ToString("D7") + "," + nEndKhz.ToString("D7") + "," +
                nTopDBM.ToString("D3") + "," + nBottomDBM.ToString("D3");
            SendCommand(sData);

            Thread.Sleep(500); //wait some time for the unit to process changes, otherwise may get a different command too soon
        }

        private void UpdateRemoteConfigData()
        {
            if (m_bPortConnected)
            {
                SendNewConfig(Convert.ToDouble(m_sStartFreq.Text), Convert.ToDouble(m_sEndFreq.Text),
                    Convert.ToDouble(m_sTopDBM.Text), Convert.ToDouble(m_sBottomDBM.Text));
            }
        }   

        private void btnConnect_Click(object sender, EventArgs e)
        {
            ManualConnect();
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            ClosePort();
        }

        private void chkRunMode_CheckedChanged(object sender, EventArgs e)
        {
            m_bHoldMode = ! chkRunMode.Checked;
            if (!m_bHoldMode && (m_nDataIndex >= m_nTotalBufferSize))
            {
                m_nDataIndex = 0;
                m_nMaxDataIndex = 0;
                ReportLog("Buffer cleared.", false);
            }
            UpdateFeedMode();
        }

        private void chkHoldMode_CheckedChanged(object sender, EventArgs e)
        {
            m_bHoldMode = chkHoldMode.Checked;
            if (m_bHoldMode)
            {
                //Send hold mode to RF Explorer to stop RS232 traffic
                SendCommand("CH");
            }
            else
            {
                //Not on hold anymore, restore RS232 traffic
                AskConfigData();
                Thread.Sleep(50);
            }
            UpdateFeedMode();
        }

        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (m_bHoldMode)
            {
                m_nDataIndex = (UInt16)numericSampleSA.Value;
                if (m_nDataIndex > m_nMaxDataIndex)
                {
                    m_nDataIndex = m_nMaxDataIndex;
                    numericSampleSA.Value = m_nMaxDataIndex;
                }
                DisplaySpectrumAnalyzerData();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (About_RFExplorer myAbout = new About_RFExplorer())
            {
                myAbout.ShowDialog();
            }
        }

        private void MainMenuView_DropDownOpening(object sender, EventArgs e)
        {
            menuRealtimeData.Checked = m_bDrawRealtime;
            menuAveragedData.Checked = m_bDrawAverage;
            menuMaxData.Checked = m_bDrawMax;
            menuMinData.Checked = m_bDrawMin;
            menuShowPeak.Checked = m_bShowPeaks;
            menuDarkMode.Checked = m_bDark;
        }

        private void click_view_mode(object sender, EventArgs e)
        {
            m_bDrawRealtime = menuRealtimeData.Checked;
            m_bDrawAverage = menuAveragedData.Checked;
            m_bDrawMax = menuMaxData.Checked;
            m_bDrawMin = menuMinData.Checked;
            m_bShowPeaks = menuShowPeak.Checked;
            UpdateButtonStatus();
            if (m_bHoldMode)
                DisplaySpectrumAnalyzerData();
        }

        private string FileHeaderVersioned()
        {
            return "RFExplorer PC Client - Format v" + m_nFileFormat.ToString("D3");
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog MySaveFileDialog = new SaveFileDialog())
                {
                    MySaveFileDialog.Filter = "RFExplorer files (*.rfe)|*.rfe|All files (*.*)|*.*";
                    MySaveFileDialog.FilterIndex = 1;
                    MySaveFileDialog.RestoreDirectory = false;
                    MySaveFileDialog.InitialDirectory = m_sDefaultDataFolder;

                    GetNewFilename();
                    MySaveFileDialog.FileName = m_sFilenameRFE;

                    if (MySaveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        SaveFile(MySaveFileDialog.FileName);
                    }
                }
            }
            catch (Exception obEx) { MessageBox.Show(obEx.Message); }
        }

        private void SaveCSVtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog MySaveFileDialog = new SaveFileDialog())
                {
                    MySaveFileDialog.Filter = "RFExplorer CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                    MySaveFileDialog.FilterIndex = 1;
                    MySaveFileDialog.RestoreDirectory = false;
                    MySaveFileDialog.InitialDirectory = m_sDefaultDataFolder;

                    GetNewFilename();
                    MySaveFileDialog.FileName = m_sFilenameRFE.Replace(".rfe",".csv");

                    if (MySaveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        SaveFileCSV(MySaveFileDialog.FileName);
                    }
                }
            }
            catch (Exception obEx) { MessageBox.Show(obEx.Message); }
        }

        private void menusSaveSimpleCSV_Click(object sender, EventArgs e)
        {
            try
            {
                PointPairList listCurrentPointList = null;

                int nSelectionCounter = 0;
                if (m_bDrawAverage)
                {
                    nSelectionCounter++;
                    listCurrentPointList = m_PointListAverage;
                }
                if (m_bDrawMax)
                {
                    listCurrentPointList = m_PointListMax;
                    nSelectionCounter++;
                }
                if (m_bDrawMin)
                {
                    nSelectionCounter++;
                    listCurrentPointList = m_PointListMin;
                }
                if (m_bDrawRealtime)
                {
                    listCurrentPointList = m_PointListRealtime;
                    nSelectionCounter++;
                }

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
                    MySaveFileDialog.Filter = "RFExplorer CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                    MySaveFileDialog.FilterIndex = 1;
                    MySaveFileDialog.RestoreDirectory = false;
                    MySaveFileDialog.InitialDirectory = m_sDefaultDataFolder;

                    GetNewFilename();
                    MySaveFileDialog.FileName = m_sFilenameRFE.Replace(".rfe", ".csv");

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

        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            menuSaveAsRFE.Enabled = (m_nMaxDataIndex > 0) && (MainTab.SelectedTab == tabSpectrumAnalyzer);
            menuSaveCSV.Enabled = (m_nMaxDataIndex > 0) && (MainTab.SelectedTab == tabSpectrumAnalyzer);
            menusSaveSimpleCSV.Enabled = menuSaveCSV.Enabled;
            menuLoadRFE.Enabled = MainTab.SelectedTab == tabSpectrumAnalyzer;

            menuSaveRemoteImage.Enabled = (m_nMaxScreenIndex > 0) && (MainTab.SelectedTab == tabRemoteScreen);
            menuLoadRFS.Enabled = MainTab.SelectedTab == tabRemoteScreen;
            menuSaveRFS.Enabled = (m_nMaxScreenIndex > 0) && (MainTab.SelectedTab == tabRemoteScreen);
        }

        private void UpdateFeedMode()
        {
            if (!m_bPortConnected)
            {
                m_bHoldMode = true;
            }

            chkRunMode.Checked = !m_bHoldMode;
            chkRunDecoder.Checked = !m_bHoldMode;
            chkHoldMode.Checked = m_bHoldMode;
            chkHoldDecoder.Checked = m_bHoldMode;
            if (m_bHoldMode == false)
            {
                m_nDataIndex = m_nMaxDataIndex;
                toolFile.Text = "File: none";
                m_sFilenameRFE = "";
            }

            if (m_bHoldMode)
            {
                if (m_sFilenameRFE.Length > 0)
                {
                    zedSpectrumAnalyzer.GraphPane.Title.Text = "RF Explorer File data";
                }
                else
                {
                    zedSpectrumAnalyzer.GraphPane.Title.Text = "RF Explorer ON HOLD";
                }
                zedRAWDecoder.GraphPane.Title.Text = "RF Explorer Decoder ON HOLD";
            }
            else
            {
                if (m_bPortConnected)
                {
                    zedSpectrumAnalyzer.GraphPane.Title.Text = "RF Explorer Live data";
                    zedRAWDecoder.GraphPane.Title.Text = "RF Explorer Decoder Live Data";
                }
                else
                {
                    zedSpectrumAnalyzer.GraphPane.Title.Text = "";
                    zedRAWDecoder.GraphPane.Title.Text = "";
                }
            }
            zedSpectrumAnalyzer.Refresh();
            zedRAWDecoder.Refresh();
        }

        private void SaveFile(string sFilename)
        {
            try
            {
                //if no file path was explicited, add the default folder
                if (sFilename.IndexOf("\\") < 0)
                {
                    sFilename=m_sDefaultDataFolder+"\\"+sFilename;
                    sFilename=sFilename.Replace("\\\\", "\\");
                }
                //Save file
                using (FileStream myFile = new FileStream(sFilename, FileMode.Create))
                {
                    using (BinaryWriter binStream = new BinaryWriter(myFile))
                    {
                        binStream.Write(FileHeaderVersioned());
                        binStream.Write(m_fStartFrequencyMHZ);
                        binStream.Write(m_fStepFrequencyMHZ);
                        binStream.Write(m_nMaxDataIndex);
                        binStream.Write(m_nFreqSpectrumSteps);

                        for (int nPageInd = 0; nPageInd < m_nMaxDataIndex; nPageInd++)
                        {
                            for (int nByte = 0; nByte < m_nFreqSpectrumSteps; nByte++)
                            {
                                binStream.Write((double)m_arrData[nPageInd, nByte]);
                            }
                        }
                    }
                }
            }
            catch (Exception obEx) { MessageBox.Show(obEx.Message); }
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

                using (StreamWriter myFile = new StreamWriter(sFilename, true))
                {
                    myFile.WriteLine("RF Explorer CSV data file: " + FileHeaderVersioned());
                    myFile.WriteLine("Start Frequency: " + m_fStartFrequencyMHZ.ToString()+
                        "MHZ\r\nStep Frequency: " + (m_fStepFrequencyMHZ*1000).ToString()+
                        "KHZ\r\nTotal data entries: " + m_nMaxDataIndex.ToString()+
                        "\r\nSteps per entry: "+ m_nFreqSpectrumSteps.ToString());

                    char cCSV = GetCSVDelimiter();

                    for (int nPageInd = 0; nPageInd < m_nMaxDataIndex; nPageInd++)
                    {
                        myFile.Write(nPageInd.ToString() + cCSV);

                        for (int nByte = 0; nByte < m_nFreqSpectrumSteps; nByte++)
                        {
                            myFile.Write(((double)m_arrData[nPageInd, nByte]).ToString());
                            if (nByte!=(m_nFreqSpectrumSteps-1))
                                myFile.Write(cCSV);
                        }
                        myFile.Write("\r\n");
                    }
                }
            }
            catch (Exception obEx) { MessageBox.Show(obEx.Message); }
        }

        private void LoadFileRFE(string sFile)
        {
            bool bFileOk = true;
            try
            {
                using (FileStream myFile = new FileStream(sFile, FileMode.Open))
                {
                    using (BinaryReader binStream = new BinaryReader(myFile))
                    {
                        ClearWaterfall();
                        m_nDataIndex = 0;
                        string sHeader = binStream.ReadString();
                        m_fStartFrequencyMHZ = binStream.ReadDouble();
                        m_fStepFrequencyMHZ = binStream.ReadDouble();
                        m_nMaxDataIndex = binStream.ReadUInt16();
                        if ((m_nFreqSpectrumSteps != binStream.ReadUInt16()) ||
                             (sHeader != FileHeaderVersioned())
                            )
                        {
                            bFileOk = false;
                            MessageBox.Show("Wrong file format: \r\n" + sHeader);
                        }

                        for (int nPageInd = 0; bFileOk && (nPageInd < m_nMaxDataIndex); nPageInd++)
                        {
                            for (int nByte = 0; nByte < m_nFreqSpectrumSteps; nByte++)
                            {
                                m_arrData[nPageInd, nByte] = (float)binStream.ReadDouble();
                            }
                            UpdateWaterfall();
                            m_nDataIndex++;
                        }
                        binStream.Close();

                        if (bFileOk)
                        {
                            m_nDataIndex = m_nMaxDataIndex;
                            numericSampleSA.Value = m_nMaxDataIndex;

                            toolFile.Text = "File: " + sFile;
                            m_sFilenameRFE = sFile;

                            m_bHoldMode = true;
                            UpdateFeedMode();

                            GetTopBottomDataRange(out m_fAmplitudeTop, out m_fAmplitudeBottom);

                            m_fAmplitudeBottom -= 5;
                            m_fAmplitudeTop += 15;

                            SetupSpectrumAnalyzerAxis();
                            DisplaySpectrumAnalyzerData();
                        }
                    }
                }
            }
            catch (Exception obEx) { MessageBox.Show(obEx.Message); }
        }

        private void toolStripMenuItemLoad_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog MyOpenFileDialog = new OpenFileDialog())
                {
                    MyOpenFileDialog.Filter = "RFExplorer files (*.rfe)|*.rfe|All files (*.*)|*.*";
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

        void ListAllCOMPorts()
        {
            string csSubkey = "SYSTEM\\CurrentControlSet\\Control\\Class\\{4D36E978-E325-11CE-BFC1-08002BE10318}";
            RegistryKey regPortKey = Registry.LocalMachine.OpenSubKey(csSubkey, RegistryKeyPermissionCheck.ReadSubTree, System.Security.AccessControl.RegistryRights.QueryValues | System.Security.AccessControl.RegistryRights.EnumerateSubKeys);

            string[] arrPortIndexes = regPortKey.GetSubKeyNames();
            ReportLog("Found total ports: " + arrPortIndexes.Length.ToString(), true);

            //List all configured ports and driver versions for CP210x
            foreach (string sPortIndex in arrPortIndexes)
            {
                try
                {
                    RegistryKey regPort = regPortKey.OpenSubKey(sPortIndex, RegistryKeyPermissionCheck.ReadSubTree, System.Security.AccessControl.RegistryRights.QueryValues);

                    ReportLog("COM port index: " + sPortIndex, true);
                    if (regPort != null)
                    {
                        Object obDriverDesc = regPort.GetValue("DriverDesc");
                        string sDriverDesc = obDriverDesc.ToString();
                        ReportLog("   DriverDesc: " + sDriverDesc, true);
                        if (!sDriverDesc.Contains("CP210x"))
                            continue; //if it is not a Silicon Labs CP2102, ignore next steps
                        Object obCOMID = regPort.GetValue("AssignedPortForQCDevice");
                        if (obCOMID != null)
                            ReportLog("   AssignedPortForQCDevice: " + obCOMID.ToString(), true);
                        Object obDriverVersion = regPort.GetValue("DriverVersion");
                        ReportLog("   DriverVersion: " + obDriverVersion.ToString(), true);
                        Object obDriverDate = regPort.GetValue("DriverDate");
                        ReportLog("   DriverDate: " + obDriverDate.ToString(), true);
                        Object obMatchingDeviceId = regPort.GetValue("MatchingDeviceId");
                        ReportLog("   MatchingDeviceId: " + obMatchingDeviceId.ToString(), true);
                    }
                }
                catch (Exception obEx) { ReportLog(obEx.Message, true); };
            }
        }

        private bool IsConnectedPort(string sPortName)
        {
            foreach (string sPort in m_arrConnectedPorts)
            {
                if (sPort == sPortName)
                    return true;
            }
            return false;
        }

        private bool IsRepeatedPort(string sPortName)
        {
            if (m_arrValidCP2101Ports == null)
                return false;

            foreach (string sPort in m_arrValidCP2101Ports)
            {
                if (sPort == sPortName)
                    return true;
            }
            return false;
        }

        void GetValidCOMPorts()
        {
            m_arrValidCP2101Ports = null;

            string csSubkey = "SYSTEM\\CurrentControlSet\\Enum\\USB\\VID_10C4&PID_EA60";
            RegistryKey regUSBKey = Registry.LocalMachine.OpenSubKey(csSubkey, RegistryKeyPermissionCheck.ReadSubTree, System.Security.AccessControl.RegistryRights.QueryValues | System.Security.AccessControl.RegistryRights.EnumerateSubKeys);

            string[] arrDeviceCP210x = regUSBKey.GetSubKeyNames();
            ReportLog("Found total CP210x entries: " + arrDeviceCP210x.Length.ToString(), true);
            //Iterate all driver for CP210x and get those with a valid connected COM port
            foreach (string sUSBIndex in arrDeviceCP210x)
            {
                try
                {
                    RegistryKey regUSBID = regUSBKey.OpenSubKey(sUSBIndex, RegistryKeyPermissionCheck.ReadSubTree, System.Security.AccessControl.RegistryRights.QueryValues | System.Security.AccessControl.RegistryRights.EnumerateSubKeys);
                    if (regUSBID != null)
                    {
                        Object obFriendlyName = regUSBID.GetValue("FriendlyName");
                        ReportLog("   FriendlyName: " + obFriendlyName.ToString(), true);
                        RegistryKey regDevice = regUSBID.OpenSubKey("Device Parameters", RegistryKeyPermissionCheck.ReadSubTree, System.Security.AccessControl.RegistryRights.QueryValues);
                        if (regDevice != null)
                        {
                            object obPortName = regDevice.GetValue("PortName");
                            string sPortName = obPortName.ToString();
                            ReportLog("   PortName: " + sPortName, true);
                            if (IsConnectedPort(sPortName) && !IsRepeatedPort(sPortName))
                            {
                                ReportLog(sPortName + " is a valid available port.", false);
                                if (m_arrValidCP2101Ports == null)
                                {
                                    m_arrValidCP2101Ports = new string[] { sPortName };
                                }
                                else
                                {
                                    Array.Resize(ref m_arrValidCP2101Ports, m_arrValidCP2101Ports.Length + 1);
                                    m_arrValidCP2101Ports[m_arrValidCP2101Ports.Length - 1] = sPortName;
                                }
                            }
                        }
                    }
                }
                catch (Exception obEx) { ReportLog(obEx.Message, true); };
            }
            string sTotalPortsFound = "0";
            if (m_arrValidCP2101Ports != null)
                sTotalPortsFound = m_arrValidCP2101Ports.Length.ToString();
            ReportLog("Total ports found: " + sTotalPortsFound, false);
        }

        private void btnRescan_Click(object sender, EventArgs e)
        {
            GetConnectedPorts();
            UpdateButtonStatus();
        }

        private void objGraph_ContextMenuBuilder(ZedGraphControl sender, ContextMenuStrip menuStrip, Point mousePt, ZedGraphControl.ContextMenuObjectState objState)
        {
            //todo if something contextual is needed...
        }

        protected void UpdateYAxis()
        {
            //check and fix absolute margins
            if (m_fAmplitudeBottom < MIN_AMPLITUDE_DBM)
            {
                m_fAmplitudeBottom = MIN_AMPLITUDE_DBM;
            }
            if (m_fAmplitudeTop > MAX_AMPLITUDE_DBM)
            {
                m_fAmplitudeTop = MAX_AMPLITUDE_DBM;
            }

            //Check and fix relative margins
            if (m_fAmplitudeTop < (MIN_AMPLITUDE_DBM + MIN_AMPLITUDE_RANGE_DBM))
            {
                m_fAmplitudeTop = MIN_AMPLITUDE_DBM + MIN_AMPLITUDE_RANGE_DBM;
            }
            if (m_fAmplitudeBottom >= m_fAmplitudeTop)
            {
                m_fAmplitudeBottom = m_fAmplitudeTop - MIN_AMPLITUDE_RANGE_DBM;
            }

            zedSpectrumAnalyzer.GraphPane.YAxis.Scale.Min = m_fAmplitudeBottom + m_fOffset_dBm;
            zedSpectrumAnalyzer.GraphPane.YAxis.Scale.Max = m_fAmplitudeTop + m_fOffset_dBm;

            zedSpectrumAnalyzer.GraphPane.YAxis.Scale.MajorStep = 10.0;
            if ((m_fAmplitudeTop - m_fAmplitudeBottom) > 30)
            {
                zedSpectrumAnalyzer.GraphPane.YAxis.Scale.MinorStep = 5.0;
            }
            else
            {
                zedSpectrumAnalyzer.GraphPane.YAxis.Scale.MinorStep = 1.0;
            }

            zedSpectrumAnalyzer.Refresh();
        }

        private void toolStripMenuPortInfo_Click(object sender, EventArgs e)
        {
            ListAllCOMPorts();
        }
        
        private void btnReset_Click(object sender, EventArgs e)
        {
            UpdateDialogFromFreqSettings();
        }

        private void UpdateDialogFromFreqSettings()
        {
            m_sBottomDBM.Text = m_fAmplitudeBottom.ToString();
            m_sTopDBM.Text = m_fAmplitudeTop.ToString();
            m_sStartFreq.Text = m_fStartFrequencyMHZ.ToString("f3");
            m_sRefFrequency.Text = m_fRefFrequencyMHZ.ToString("f3");
            m_sEndFreq.Text = CalculateEndFrequencyMHZ().ToString("f3");
            m_sCenterFreq.Text = CalculateCenterFrequencyMHZ().ToString("f3");
            m_sFreqSpan.Text = CalculateFrequencySpanMHZ().ToString("f3");
        }

        private bool IsDifferent(double d1, double d2, double dEpsilon = 0.001)
        {
            return (Math.Abs(d1 - d2) > dEpsilon);
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            UpdateYAxis();
            UpdateRemoteConfigData();
        }

        private void btnMoveFreqDecLarge_Click(object sender, EventArgs e)
        {
            double fStartFreq = Convert.ToDouble(m_sStartFreq.Text);
            fStartFreq -= CalculateFrequencySpanMHZ() * 0.5;
            double fEndFreq = Convert.ToDouble(m_sEndFreq.Text);
            fEndFreq -= CalculateFrequencySpanMHZ() * 0.5;
            m_sStartFreq.Text = fStartFreq.ToString("f3");
            m_sEndFreq.Text = fEndFreq.ToString("f3");
            m_sStartFreq_Leave(null, null);

            UpdateRemoteConfigData();
        }

        private void btnMoveFreqIncLarge_Click(object sender, EventArgs e)
        {
            double fStartFreq = Convert.ToDouble(m_sStartFreq.Text);
            fStartFreq += CalculateFrequencySpanMHZ() * 0.5;
            double fEndFreq = Convert.ToDouble(m_sEndFreq.Text);
            fEndFreq += CalculateFrequencySpanMHZ() * 0.5;
            m_sStartFreq.Text = fStartFreq.ToString("f3");
            m_sEndFreq.Text = fEndFreq.ToString("f3");
            m_sEndFreq_Leave(null, null);

            UpdateRemoteConfigData();
        }

        private void btnMoveFreqDecSmall_Click(object sender, EventArgs e)
        {
            m_fStartFrequencyMHZ -= CalculateFrequencySpanMHZ() / 10;
            if (m_fStartFrequencyMHZ < m_fMinFreqMHZ)
            {
                m_fStartFrequencyMHZ = m_fMinFreqMHZ;
            }

            SetupSpectrumAnalyzerAxis();
            SaveProperties();
            UpdateRemoteConfigData();
        }

        private void btnMoveFreqIncSmall_Click(object sender, EventArgs e)
        {
            m_fStartFrequencyMHZ += CalculateFrequencySpanMHZ() / 10;
            if (CalculateEndFrequencyMHZ() > m_fMaxFreqMHZ)
            {
                m_fStartFrequencyMHZ = m_fMaxFreqMHZ - CalculateFrequencySpanMHZ();
            }

            SetupSpectrumAnalyzerAxis();
            SaveProperties();
            UpdateRemoteConfigData();
        }

        private void btnSpanDec_Click(object sender, EventArgs e)
        {
            double fFreqSpan = Convert.ToDouble(m_sFreqSpan.Text);
            fFreqSpan -= fFreqSpan * 0.25;
            m_sFreqSpan.Text = fFreqSpan.ToString("f3");
            m_sFreqSpan_Leave(null, null);
            UpdateRemoteConfigData();
        }

        private void btnSpanInc_Click(object sender, EventArgs e)
        {
            double fFreqSpan = Convert.ToDouble(m_sFreqSpan.Text);
            fFreqSpan += fFreqSpan * 0.25;
            m_sFreqSpan.Text = fFreqSpan.ToString("f3");
            m_sFreqSpan_Leave(null, null);
            UpdateRemoteConfigData();
        }

        private void btnSpanMax_Click(object sender, EventArgs e)
        {
            double fFreqSpan = 10000; //just a big number
            m_sFreqSpan.Text = fFreqSpan.ToString("f3");
            m_sFreqSpan_Leave(null, null);
            UpdateRemoteConfigData();
        }

        private void btnSpanDefault_Click(object sender, EventArgs e)
        {
            double fFreqSpan = 10;
            m_sFreqSpan.Text = fFreqSpan.ToString("f3");
            m_sFreqSpan_Leave(null, null);
            UpdateRemoteConfigData();
        }

        private void btnSpanMin_Click(object sender, EventArgs e)
        {
            double fFreqSpan = 0; //just a very small number
            m_sFreqSpan.Text = fFreqSpan.ToString("f3");
            m_sFreqSpan_Leave(null, null);
            UpdateRemoteConfigData();
        }

        private void btnTop5plus_Click(object sender, EventArgs e)
        {
            double fAmplitudeTop = Convert.ToDouble(m_sTopDBM.Text);
            fAmplitudeTop += 5;
            m_sTopDBM.Text = fAmplitudeTop.ToString();
            m_sBottomDBM_Leave(null, null);
            UpdateRemoteConfigData();
        }

        private void btnTop5minus_Click(object sender, EventArgs e)
        {
            double fAmplitudeTop = Convert.ToDouble(m_sTopDBM.Text);
            fAmplitudeTop -= 5;
            if (fAmplitudeTop == -6)
            {
                //Correct the case you are lowering the first -1 by 5dB, in which case it makes sense to go to -5dB not -6dB
                fAmplitudeTop = -5;
            }
            m_sTopDBM.Text = fAmplitudeTop.ToString();
            m_sBottomDBM_Leave(null, null);
            UpdateRemoteConfigData();
        }

        private void btnBottom5plus_Click(object sender, EventArgs e)
        {
            double fAmplitudeBottom = Convert.ToDouble(m_sBottomDBM.Text);
            fAmplitudeBottom += 5;
            m_sBottomDBM.Text = fAmplitudeBottom.ToString();
            m_sBottomDBM_Leave(null, null);
            UpdateRemoteConfigData();
        }

        private void btnBottom5minus_Click(object sender, EventArgs e)
        {
            double fAmplitudeBottom = Convert.ToDouble(m_sBottomDBM.Text);
            fAmplitudeBottom -= 5;
            m_sBottomDBM.Text = fAmplitudeBottom.ToString();
            m_sBottomDBM_Leave(null, null);
            UpdateRemoteConfigData();
        }

        private void btnCenterMark_Click(object sender, EventArgs e)
        {
            if (m_fPeakValueMHZ>0.0f)
            {
                m_sCenterFreq.Text = m_fPeakValueMHZ.ToString("f3");
                m_sCenterFreq_Leave(null, null);

                UpdateRemoteConfigData();
            }
        }

        private void m_sStartFreq_Leave(object sender, EventArgs e)
        {
            double fStartFreq = Convert.ToDouble(m_sStartFreq.Text);
            fStartFreq = Math.Max(m_fMinFreqMHZ, fStartFreq);
            fStartFreq = Math.Min(m_fMaxFreqMHZ - m_fMinSpanMHZ, fStartFreq);

            double fEndFreq = Convert.ToDouble(m_sEndFreq.Text);
            fEndFreq = Math.Max(m_fMinFreqMHZ + m_fMinSpanMHZ, fEndFreq);
            fEndFreq = Math.Min(m_fMaxFreqMHZ, fEndFreq);

            double fFreqSpan = (fEndFreq - fStartFreq);
            fFreqSpan = Math.Max(m_fMinSpanMHZ, fFreqSpan);
            fFreqSpan = Math.Min(m_fMaxSpanMHZ, fFreqSpan);

            fEndFreq = fStartFreq + fFreqSpan;

            m_sStartFreq.Text = fStartFreq.ToString("f3");
            m_sEndFreq.Text = fEndFreq.ToString("f3");

            m_sCenterFreq.Text = (fStartFreq + fFreqSpan / 2.0).ToString("f3");
            m_sFreqSpan.Text = (fFreqSpan).ToString("f3");
        }

        private void m_sEndFreq_Leave(object sender, EventArgs e)
        {
            double fStartFreq = Convert.ToDouble(m_sStartFreq.Text);
            fStartFreq = Math.Max(m_fMinFreqMHZ, fStartFreq);
            fStartFreq = Math.Min(m_fMaxFreqMHZ - m_fMinSpanMHZ, fStartFreq);

            double fEndFreq = Convert.ToDouble(m_sEndFreq.Text);
            fEndFreq = Math.Max(m_fMinFreqMHZ + m_fMinSpanMHZ, fEndFreq);
            fEndFreq = Math.Min(m_fMaxFreqMHZ, fEndFreq);

            double fFreqSpan = (fEndFreq - fStartFreq);
            fFreqSpan = Math.Max(m_fMinSpanMHZ, fFreqSpan);
            fFreqSpan = Math.Min(m_fMaxSpanMHZ, fFreqSpan);

            fStartFreq = fEndFreq - fFreqSpan;

            m_sStartFreq.Text = fStartFreq.ToString("f3");
            m_sEndFreq.Text = fEndFreq.ToString("f3");

            m_sCenterFreq.Text = (fStartFreq + fFreqSpan / 2.0).ToString("f3");
            m_sFreqSpan.Text = (fFreqSpan).ToString("f3");
        }

        private void m_sFreqSpan_Leave(object sender, EventArgs e)
        {
            double fFreqSpan = Convert.ToDouble(m_sFreqSpan.Text);
            fFreqSpan = Math.Max(m_fMinSpanMHZ,fFreqSpan);
            fFreqSpan = Math.Min(m_fMaxSpanMHZ, fFreqSpan);

            double fCenterFreq = Convert.ToDouble(m_sCenterFreq.Text);
            if ((fCenterFreq - (fFreqSpan / 2.0)) < m_fMinFreqMHZ)
                fCenterFreq = (m_fMinFreqMHZ + (fFreqSpan / 2.0));
            if ((fCenterFreq + (fFreqSpan / 2.0)) > m_fMaxFreqMHZ)
                fCenterFreq = (m_fMaxFreqMHZ - (fFreqSpan / 2.0));

            m_sFreqSpan.Text = fFreqSpan.ToString("f3");
            m_sCenterFreq.Text = fCenterFreq.ToString("f3");

            double fStartMHZ = fCenterFreq - fFreqSpan / 2.0;
            m_sStartFreq.Text = fStartMHZ.ToString("f3");
            m_sEndFreq.Text = (fStartMHZ + fFreqSpan).ToString("f3");
        }

        private void m_sCenterFreq_Leave(object sender, EventArgs e)
        {
            double fCenterFreq = Convert.ToDouble(m_sCenterFreq.Text);
            if (fCenterFreq > (m_fMaxFreqMHZ-(m_fMinSpanMHZ/2.0)))
                fCenterFreq = (m_fMaxFreqMHZ-(m_fMinSpanMHZ/2.0));
            if (fCenterFreq < (m_fMinFreqMHZ+(m_fMinSpanMHZ/2.0)))
                fCenterFreq = (m_fMinFreqMHZ+(m_fMinSpanMHZ/2.0));

            double fFreqSpan = Convert.ToDouble(m_sFreqSpan.Text);
            if ((fCenterFreq-(fFreqSpan/2.0))<m_fMinFreqMHZ)
                fFreqSpan=(fCenterFreq-m_fMinFreqMHZ)*2.0;
            if ((fCenterFreq+(fFreqSpan/2.0))>m_fMaxFreqMHZ)
                fFreqSpan=(m_fMaxFreqMHZ-fCenterFreq)*2.0;
            m_sFreqSpan.Text = fFreqSpan.ToString("f3");
            m_sCenterFreq.Text = fCenterFreq.ToString("f3");

            double fStartMHZ = fCenterFreq - fFreqSpan / 2.0;
            m_sStartFreq.Text = fStartMHZ.ToString("f3");
            m_sEndFreq.Text = (fStartMHZ + fFreqSpan).ToString("f3");
        }

        private void m_sBottomDBM_Leave(object sender, EventArgs e)
        {
            double fAmplitudeBottom = Convert.ToDouble(m_sBottomDBM.Text);
            double fAmplitudeTop = Convert.ToDouble(m_sTopDBM.Text);

            if (fAmplitudeBottom < MIN_AMPLITUDE_DBM)
                fAmplitudeBottom = MIN_AMPLITUDE_DBM;
            if (fAmplitudeBottom > (fAmplitudeTop - MIN_AMPLITUDE_RANGE_DBM))
                fAmplitudeBottom = (fAmplitudeTop - MIN_AMPLITUDE_RANGE_DBM);

            if (fAmplitudeTop > MAX_AMPLITUDE_DBM)
                fAmplitudeTop = MAX_AMPLITUDE_DBM;
            if (fAmplitudeTop < (fAmplitudeBottom + MIN_AMPLITUDE_RANGE_DBM))
                fAmplitudeTop = (fAmplitudeBottom + MIN_AMPLITUDE_RANGE_DBM);

            m_sBottomDBM.Text = fAmplitudeBottom.ToString();
            m_sTopDBM.Text = fAmplitudeTop.ToString();
        }

        private double CalculateEndFrequencyMHZ()
        {
            return m_fStartFrequencyMHZ + m_fStepFrequencyMHZ * m_nFreqSpectrumSteps;
        }

        private double CalculateFrequencySpanMHZ()
        {
            return m_fStepFrequencyMHZ * m_nFreqSpectrumSteps;
        }

        private double CalculateCenterFrequencyMHZ()
        {
            return m_fStartFrequencyMHZ + CalculateFrequencySpanMHZ()/2.0;
        }

        private void tabSpectrumAnalyzer_Enter(object sender, EventArgs e)
        {
            groupCOM.Parent = tabSpectrumAnalyzer;
            groupDataFeed.Parent = tabSpectrumAnalyzer;
            groupFreqSettings.Parent = tabSpectrumAnalyzer;
            zedSpectrumAnalyzer.Parent = tabSpectrumAnalyzer;
            DisplayGroups();
        }

        private void toolStripCleanReport_Click(object sender, EventArgs e)
        {
            textBox_message.Text = "Text cleared." + Environment.NewLine;
        }

        private void chkCalcAverage_CheckedChanged(object sender, EventArgs e)
        {
            m_bDrawAverage = chkCalcAverage.Checked;
            if (m_bHoldMode)
                DisplaySpectrumAnalyzerData();
        }

        private void chkCalcMax_CheckedChanged(object sender, EventArgs e)
        {
            m_bDrawMax = chkCalcMax.Checked;
            if (m_bHoldMode)
                DisplaySpectrumAnalyzerData();
        }

        private void chkCalcMin_CheckedChanged(object sender, EventArgs e)
        {
            m_bDrawMin = chkCalcMin.Checked;
            if (m_bHoldMode)
                DisplaySpectrumAnalyzerData();
        }

        private void chkCalcRealtime_CheckedChanged(object sender, EventArgs e)
        {
            m_bDrawRealtime = chkCalcRealtime.Checked;
            if (m_bHoldMode)
                DisplaySpectrumAnalyzerData();
        }

        private void mnuItem_ShowPeak_CheckedChanged(object sender, EventArgs e)
        {
            m_bShowPeaks = menuShowPeak.Checked;
            if (m_bHoldMode)
                DisplaySpectrumAnalyzerData();
        }

        private void menuReinitializeData_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Are you sure to reinitialize data buffer?", "Reinitialize data buffer", MessageBoxButtons.YesNo))
            {
                m_nDataIndex = 0;
                m_nMaxDataIndex = 0;
                numericSampleSA.Value = 0;
            }
        }

        private void menuShowControlArea_Click(object sender, EventArgs e)
        {
            DisplayGroups();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            DisplayGroups();
        }

        private void DisplayGroups()
        {
            MainTab.Width = Width - 14;
            MainTab.Height = Height - 64;


            if (groupCOM.Parent == tabSpectrumAnalyzer)
            {
                //double fPaneWidth = zedSpectrumAnalyzer.GraphPane.Rect.Width;
                //while ((m_RFEConfig.Location.Width > 0.0f) && (m_RFEConfig.Location.Width < (0.7f * fPaneWidth)))
                //{
                //    m_RFEConfig.FontSpec.Size += m_RFEConfig.FontSpec.Size * 1.25f;
                //}

                groupCOM.Visible = menuShowControlArea.Checked;
                groupDataFeed.Visible = menuShowControlArea.Checked;
                groupFreqSettings.Visible = menuShowControlArea.Checked;

                if (menuShowControlArea.Checked)
                {
                    zedSpectrumAnalyzer.Top = groupCOM.Location.Y + groupCOM.Height + 5;
                }
                else
                {
                    zedSpectrumAnalyzer.Top = 5;
                }
                zedSpectrumAnalyzer.Height = MainStatusBar.Top - zedSpectrumAnalyzer.Top - 3;
                zedSpectrumAnalyzer.Width = Width - 124;
            }
            else
            {
                groupCOM.Visible = true;
                groupDataFeed.Visible = true;
                groupFreqSettings.Visible = true;
            }

            if (groupCOM.Parent == tabRAWDecoder)
            {
                zedRAWDecoder.Height = MainStatusBar.Top - zedRAWDecoder.Top - 3;
                zedRAWDecoder.Width = Width - 35;
            }

            if (groupCOM.Parent == tabRemoteScreen)
            {
                panelRemoteScreen.Width = Width - 45;
                panelRemoteScreen.Height = MainStatusBar.Top - groupCOM.Height - 35;
                panelRemoteScreen.BorderStyle = BorderStyle.FixedSingle;
            }

            if (groupCOM.Parent == tabReport)
            {
                textBox_message.Width = Width - 35;
                textBox_message.Height = MainTab.Height - groupCOM.Height - 50;
            }

            zedSpectrumAnalyzer.Visible = true;
            RelocateRemoteControl();

            btnTop5plus.Top = zedSpectrumAnalyzer.Top;
            btnTop5plus.Left = zedSpectrumAnalyzer.Right + 8;
            if (m_arrAnalyzerButtonList[0] != null)
            {
                btnTop5plus.Visible = true;
                for (int nInd = 1; nInd < m_arrAnalyzerButtonList.Length; nInd++)
                {
                    m_arrAnalyzerButtonList[nInd].Top = m_arrAnalyzerButtonList[nInd - 1].Bottom + 3;
                    m_arrAnalyzerButtonList[nInd].Left = m_arrAnalyzerButtonList[0].Left;
                    m_arrAnalyzerButtonList[nInd].Visible = true;
                }
            }
        }

        private void menuDarkMode_Click(object sender, EventArgs e)
        {
            m_bDark = menuDarkMode.Checked;
            DefineGraphColors(zedSpectrumAnalyzer);
            Invalidate();
        }        
        #endregion

        #region Waterfall
        float m_wSizeX;
        float m_wSizeY;

        private void controlWaterfall_Load(object sender, EventArgs e)
        {

        }

        private void tabWaterfall_Enter(object sender, EventArgs e)
        {
            Rectangle rectArea = panelWaterfall.ClientRectangle;
            m_wSizeX = (float)(rectArea.Width / 7.0);
            m_wSizeY = (float)(rectArea.Height / 7.0);
            groupDataFeed.Parent = tabWaterfall;
            groupCOM.Parent = tabWaterfall;

            tabWaterfall_UpdateZoomValues();
        }

        private void numericSensitivity_ValueChanged(object sender, EventArgs e)
        {
            int sensitivity = (UInt16)numericSensitivity.Value;
            trackBarSensitivity.Value = sensitivity;
            controlWaterfall.UpdateSensitivity(sensitivity);
            if (m_bHoldMode)
                controlWaterfall.Invalidate();
        }

        private void numericContrast_ValueChanged(object sender, EventArgs e)
        {
            int contrast = (UInt16)numericContrast.Value;
            trackBarContrast.Value = contrast;
            controlWaterfall.UpdateContrast(contrast);
            if (m_bHoldMode)
                controlWaterfall.Invalidate();
        }

        private void trackBarSensitivity_ValueChanged(object sender, EventArgs e)
        {
            int sensitivity = (UInt16)trackBarSensitivity.Value;
            numericSensitivity.Value = sensitivity;
            controlWaterfall.UpdateSensitivity(sensitivity);
            if (m_bHoldMode)
                controlWaterfall.Invalidate();
        }

        private void trackBarContrast_ValueChanged(object sender, EventArgs e)
        {
            int contrast = (UInt16)trackBarContrast.Value;
            numericContrast.Value = contrast;
            controlWaterfall.UpdateContrast(contrast);
            if (m_bHoldMode)
                controlWaterfall.Invalidate();
        }

        private void tabWaterfall_Paint(object sender, PaintEventArgs e)
        {
        }

        private void ClearWaterfall()
        {
            controlWaterfall.ClearWaterfall();
        }

        private void UpdateWaterfall()
        {
            Dictionary<double, double> RTList = new Dictionary<double, double>();
            Dictionary<double, double> MaxList = new Dictionary<double, double>();
            Dictionary<double, double> MinList = new Dictionary<double, double>();
            Dictionary<double, double> AvgList = new Dictionary<double, double>();

            //TODO: This code is duplicated from DisplayData, needs to be abstracted

            int nIndex = m_nDataIndex - 1;
            if (nIndex < 0)
                nIndex = 0;
            m_nDrawingIteration++;

            int nCalculatorMax = (int)numericIterations.Value;
            if (nCalculatorMax > nIndex)
                nCalculatorMax = nIndex;

            double fRealtimeMax_Amp = -200.0;
            int fRealtimeMax_Iter = 0;
            double fAverageMax_Amp = -200.0;
            int fAverageMax_Iter = 0;
            double fMaxMax_Amp = -200.0;
            int fMaxMax_Iter = 0;

            m_AveragePeak.Text = "";
            m_RealtimePeak.Text = "";
            m_MaxPeak.Text = "";

            for (int nInd = 0; nInd < m_nFreqSpectrumSteps; nInd++)
            {
                double fVal = m_arrData[nIndex, nInd];
                if (fVal > fRealtimeMax_Amp)
                {
                    fRealtimeMax_Amp = fVal;
                    fRealtimeMax_Iter = nInd;
                }

                double fFreq = m_fStartFrequencyMHZ + m_fStepFrequencyMHZ * nInd;

                double fMax = fVal;
                double fMin = fVal;
                double fValAvg = fVal;

                for (int nIterator = nIndex - nCalculatorMax; nIterator < nIndex; nIterator++)
                {
                    //Calculate average, max and min over Calculator range
                    double fVal2 = m_arrData[nIterator, nInd];

                    fMax = Math.Max(fMax, fVal2);
                    fMin = Math.Min(fMin, fVal2);

                    fValAvg += fVal2;
                }

                if (m_bDrawRealtime)
                    RTList.Add(fFreq, fVal);
                if (m_bDrawMin)
                    MinList.Add(fFreq, fMin);

                if (m_bDrawMax)
                {
                    MaxList.Add(fFreq, fMax);
                    if (fMax > fMaxMax_Amp)
                    {
                        fMaxMax_Amp = fMax;
                        fMaxMax_Iter = nInd;
                    }
                }

                if (m_bDrawAverage)
                {
                    fValAvg = fValAvg / (nCalculatorMax + 1);
                    AvgList.Add(fFreq, fValAvg);
                    if (fValAvg > fAverageMax_Amp)
                    {
                        fAverageMax_Amp = fValAvg;
                        fAverageMax_Iter = nInd;
                    }
                }
            }
            if ((int)numericIterations.Value > 1)
                controlWaterfall.DrawWaterfall(MaxList);
            else
                controlWaterfall.DrawWaterfall(RTList);

            controlWaterfall.Invalidate();
        }

        private void tabWaterfall_UpdateZoomValues()
        {
            controlWaterfall.Size = new Size((int)(1.0 + m_wSizeX * (float)(numericZoom.Value)), (int)(1.0 + m_wSizeY * (float)(numericZoom.Value)));
            controlWaterfall.UpdateZoom((int)(numericZoom.Value));
            controlWaterfall.Invalidate();
        }

        #endregion

        #region Remote screen

        private void tabRemoteScreen_Enter(object sender, EventArgs e)
        {
            groupCOM.Parent = tabRemoteScreen;
            UpdateButtonStatus();
            tabRemoteScreen_UpdateZoomValues();
            tabRemoteScreen.Invalidate();
            DisplayGroups();
        }

        private void CopyScreenDataArray()
        {
            UInt16 nPage = (UInt16)numScreenIndex.Value;
            for (int nInd = controlRemoteScreen.m_arrRemoteScreenData.GetLowerBound(0); nInd <= controlRemoteScreen.m_arrRemoteScreenData.GetUpperBound(0); nInd++)
                controlRemoteScreen.m_arrRemoteScreenData[nInd] = m_arrRemoteScreenData[nPage, nInd];
        }

        private void numScreenIndex_ValueChanged(object sender, EventArgs e)
        {
            m_nScreenIndex = (UInt16)numScreenIndex.Value;
            if (m_nScreenIndex > m_nMaxScreenIndex)
            {
                m_nScreenIndex = m_nMaxScreenIndex;
                numScreenIndex.Value = m_nScreenIndex;
            }
            if (m_nMaxScreenIndex > 0)
            {
                CopyScreenDataArray();
                controlRemoteScreen.Invalidate();
            }
        }

        private void tabRemoteScreen_Paint(object sender, PaintEventArgs e)
        {
            CopyScreenDataArray();
        }

        private void tabRemoteScreen_UpdateZoomValues()
        {
            controlRemoteScreen.Size = new Size((int)(1.0 + m_fSizeX * (float)(numericZoom.Value)), (int)(1.0 + m_fSizeY * (float)(numericZoom.Value)));
            controlRemoteScreen.UpdateZoom((int)(numericZoom.Value));
            RelocateRemoteControl();
            controlRemoteScreen.Invalidate();
        }

        private void numericZoom_ValueChanged(object sender, EventArgs e)
        {
            tabRemoteScreen_UpdateZoomValues();
            tabRemoteScreen.Invalidate();
        }

        private void chkDumpScreen_CheckedChanged(object sender, EventArgs e)
        {
            UpdateButtonStatus();
            if (chkDumpScreen.Checked)
            {
                SendCommand("D1");
            }
            else
            {
                SendCommand("D0");
            }
        }

        private void SavePNG(string sFilename)
        {
            //if no file path was explicited, add the default folder
            if (sFilename.IndexOf("\\") < 0)
            {
                sFilename = m_sDefaultDataFolder + "\\" + sFilename;
                sFilename = sFilename.Replace("\\\\", "\\");
            }

            Rectangle rectArea = controlRemoteScreen.ClientRectangle;
            using (Bitmap objAppBmp = new Bitmap(rectArea.Width, rectArea.Height))
            {
                controlRemoteScreen.DrawToBitmap(objAppBmp, rectArea);

                int nSize = (int)(numericZoom.Value);
                using (Bitmap objImage = new Bitmap(rectArea.Width, rectArea.Height))
                {
                    Graphics.FromImage(objImage).DrawImage(objAppBmp, rectArea);
                    objImage.Save(sFilename, ImageFormat.Png);
                }
            }
        }

        private void SaveImagetoolStrip_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog MySaveFileDialog = new SaveFileDialog())
                {
                    MySaveFileDialog.Filter = "Image PNG files (*.png)|*.png|All files (*.*)|*.*";
                    MySaveFileDialog.FilterIndex = 1;
                    MySaveFileDialog.RestoreDirectory = false;
                    MySaveFileDialog.InitialDirectory = m_sDefaultDataFolder;

                    GetNewFilename();
                    MySaveFileDialog.FileName = m_sFilenameRFE.Replace(".rfe", ".png");

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
            try
            {
                //if no file path was explicited, add the default folder
                if (sFilename.IndexOf("\\") < 0)
                {
                    sFilename = m_sDefaultDataFolder + "\\" + sFilename;
                    sFilename = sFilename.Replace("\\\\", "\\");
                }

                using (FileStream myFile = new FileStream(sFilename, FileMode.Create))
                {
                    using (BinaryWriter binStream = new BinaryWriter(myFile))
                    {
                        binStream.Write("RF Explorer RFS screen file: " + FileHeaderVersioned());
                        binStream.Write(m_nMaxScreenIndex);
                        for (UInt16 nPageInd = 0; nPageInd <= m_nMaxScreenIndex; nPageInd++)
                        {
                            binStream.Write(nPageInd);

                            for (int nIndY = 0; nIndY < 8; nIndY++)
                            {
                                for (int nIndX = 0; nIndX < 128; nIndX++)
                                {
                                    byte nData = m_arrRemoteScreenData[nPageInd, nIndX + 128 * nIndY];
                                    binStream.Write(nData);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception obEx) { MessageBox.Show(obEx.Message); }
        }

        private void LoadFileRFS(string sFilename)
        {
            try
            {
                using (FileStream myFile = new FileStream(sFilename, FileMode.Open))
                {
                    using (BinaryReader binStream = new BinaryReader(myFile))
                    {
                        string sHeader = binStream.ReadString();
                        m_nMaxScreenIndex = binStream.ReadUInt16();
                        ReportLog("RFS file loaded: " + sHeader + " with total samples:" + m_nMaxScreenIndex.ToString(), false);
                        for (UInt16 nPageInd = 0; nPageInd <= m_nMaxScreenIndex; nPageInd++)
                        {
                            binStream.ReadUInt16(); //page number, can be ignored here

                            for (int nIndY = 0; nIndY < 8; nIndY++)
                            {
                                for (int nIndX = 0; nIndX < 128; nIndX++)
                                {
                                    byte nData = binStream.ReadByte();
                                    m_arrRemoteScreenData[nPageInd, nIndX + 128 * nIndY] = nData;
                                }
                            }
                        }
                        numScreenIndex.Value = m_nMaxScreenIndex;
                        m_nScreenIndex = (ushort)numScreenIndex.Value;
                        UpdateButtonStatus();
                    }
                }
            }
            catch (Exception obEx) { MessageBox.Show(obEx.Message); }
        }

        private void menu_SaveRFS_Click(object sender, EventArgs e)
        {
            if (m_nMaxScreenIndex > 0)
            {
                try
                {
                    using (SaveFileDialog MySaveFileDialog = new SaveFileDialog())
                    {
                        MySaveFileDialog.Filter = "RF Explorer RFS Screen files (*.rfs)|*.rfs|All files (*.*)|*.*";
                        MySaveFileDialog.FilterIndex = 1;
                        MySaveFileDialog.RestoreDirectory = false;
                        MySaveFileDialog.InitialDirectory = m_sDefaultDataFolder;

                        GetNewFilename();
                        MySaveFileDialog.FileName = m_sFilenameRFE.Replace(".rfe", ".rfs");

                        if (MySaveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            SaveFileRFS(MySaveFileDialog.FileName);
                        }
                    }
                }
                catch (Exception obEx) { MessageBox.Show(obEx.Message); }
            }
        }

        private void menu_LoadRFS_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog MyOpenFileDialog = new OpenFileDialog())
            {
                MyOpenFileDialog.Filter = "RFExplorer files (*.rfs)|*.rfs|All files (*.*)|*.*";
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

        }

        private void RelocateRemoteControl()
        {
            controlRemoteScreen.Top = (panelRemoteScreen.Height - controlRemoteScreen.Height) / 2;
            controlRemoteScreen.Left = (panelRemoteScreen.Width - controlRemoteScreen.Width) / 2;
            MainStatusBar.Parent = groupCOM.Parent;
        }
        #endregion

        #region Report Window

        private void tabReport_Enter(object sender, EventArgs e)
        {
            groupCOM.Parent = tabReport;
            DisplayGroups();
        }

        private void ReportLog(string sLine, bool bDetailed)
        {
            if (m_bFirstText)
            {
                m_sReportFilePath = m_sAppDataFolder + "\\RFExplorerClient_report_" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                m_sReportFilePath = m_sReportFilePath.Replace("\\\\", "\\");

                labelReportFile.Text = "Report file: " + m_sReportFilePath;

                textBox_message.AppendText("Welcome to RFExplorer Client - report being saved to: " + Environment.NewLine + m_sReportFilePath + Environment.NewLine);
            }
            else
                sLine = Environment.NewLine + sLine;

            if (m_chkDebugTraces.Checked || !bDetailed)
            {
                textBox_message.AppendText(sLine);
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

                    sr.WriteLine("OS:       " + Environment.OSVersion.ToString());
                    sr.WriteLine("Runtime:  " + Environment.Version.ToString());
                    sr.WriteLine("Assembly: " + Assembly.GetExecutingAssembly().ToString());
                    sr.WriteLine("File:     " + Assembly.GetExecutingAssembly().Location);

                    sr.WriteLine("");
                }
                sr.Write(sLine);
            }

            m_bFirstText = false;
        }

        private void btnSendCustomCmd_Click(object sender, EventArgs e)
        {
            string sCmd = "";
            if (comboCustomCommand.Items.Count > 0 && comboCustomCommand.Text.Length > 0)
            {
                sCmd = comboCustomCommand.Text;
                if (!RFExplorerClient.Properties.Settings.Default.CustomCommands.Contains(sCmd))
                {
                    RFExplorerClient.Properties.Settings.Default.CustomCommands.Add(sCmd);

                    //All this to refresh the combo after the string collection is changed, something it doesn't do automatically...
                    //Anybody knows a better way?
                    comboCustomCommand.DataSource = null;
                    comboCustomCommand.DataSource = RFExplorerClient.Properties.Settings.Default.CustomCommands;
                    comboCustomCommand.Text = sCmd;
                }
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

        private void btnSendCmd_Click(object sender, EventArgs e)
        {

            string sCmd="";
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
            groupCOM.Parent = tabConfiguration;
            groupDataFeed.Parent = tabConfiguration;
            groupFreqSettings.Parent = tabConfiguration;
            DisplayGroups();
        }
        private void btnOpenLog_Click(object sender, EventArgs e)
        {
            Process.Start(m_sReportFilePath);
        }

        private void edDefaultFilePath_Leave(object sender, EventArgs e)
        {
            m_sDefaultDataFolder = edDefaultFilePath.Text;
            SaveProperties();
        }

        #region Calibration 
    #endregion 

        #endregion
    }
}
