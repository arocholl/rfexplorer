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

namespace RFExplorerClient
{
    public partial class MainForm : Form
    {
        #region Data Members
        const byte m_nFileFormat = 1;               //File format constant
        const int m_nTotalBufferSize = 10240;       //buffer size for the different available collections

        const double MIN_AMPLITUDE_DBM = -120.0;
        const double MAX_AMPLITUDE_DBM = -1.0;
        const double MIN_AMPLITUDE_RANGE_DBM = 10;
        const double MAX_RAW_SAMPLE = 4356 * 8;     //default value for RAW data sample
        const UInt16 MAX_SPECTRUM_STEPS = 1024;

        const string m_sRFExplorerFirmwareCertified = "01.09"; //Firmware version of RF Explorer which was tested and certified with this PC Client

        UInt16 m_nFreqSpectrumSteps = 112;  //$S byte buffer by default
        
        int m_nDrawingIteration = 0;        //Iteration counter to do regular updates on GUI

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

        enum eModulation
        {
            MODULATION_OOK,         //0
            MODULATION_PSK,         //1
            MODULATION_NONE = 0xFF  //0xFF
        };
        eModulation m_eModulation;          //Modulation being used

        //calibration values
        Byte m_nCalibrationCapSi4x;
        Byte m_nCalibrationCapMixer;
        string m_sMainBoardCalibrated = ""; //Date when this was calibrated
        string m_sExpansionBoardCalibrated = ""; //Date when this was calibrated

        //Initializer for 433MHz model, will change later based on settings
        double m_fMinSpanMHZ = 0.112;       //Min valid span in MHZ for connected model
        double m_fMaxSpanMHZ = 100.0;       //Max valid span in MHZ for connected model
        double m_fMinFreqMHZ = 430.0;       //Min valid frequency in MHZ for connected model
        double m_fMaxFreqMHZ = 440.0;       //Max valid frequency in MHZ for connected model

        double m_fRBWKHZ = 0.0;             //RBW in use
        float m_fOffset_dBm = 0.0f;         //Manual offset of the amplitude reading

        string m_sFilenameRFE="";           //RFE data file name
        string m_sReportFilePath="";        //Path and name of the report log file
        string m_sDefaultFolder = "";       //RFE default folder for saving data files

        Boolean m_bPortConnected = false;   //Will be true while COM port is connected, as IsOpen() is not reliable

        float[,] m_arrData;                 //Collection of available spectrum data
        UInt16 m_nDataIndex = 0;            //Index pointing to latest spectrum data received
        UInt16 m_nMaxDataIndex = 0;         //Max value for m_nDataIndex with available data

        byte[,] m_arrRemoteScreenData;      //Collection of available remote screen data
        UInt16 m_nScreenIndex = 0;          //Index pointing to the latest Dump screen received
        UInt16 m_nMaxScreenIndex = 0;       //Max value for m_nScreenIndex with available data

        const float m_fSizeX = 130;         //Size of the dump screen in pixels (128x64 + 2 border)
        const float m_fSizeY = 66;

        UInt16 m_nRAWSnifferIndex=0;        //Index pointing to current RAW data value shown
        UInt16 m_nMaxRAWSnifferIndex=0;     //Index pointing to the last RAW data value available
        string[] m_arrRAWSnifferData;       //Array of strings for sniffer data

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

        Button[] m_arrAnalyzerButtonList=new Button[14];
        
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
            m_RealtimePeak.FontSpec.Size = 8;
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
            groupRemoteScreen.Enabled = !menuAutoLCDOff.Checked;
        }

        private void GetNewFilename()
        {
            //New unique filename to store data based on date and time
            m_sFilenameRFE = "RFExplorer_Client_Data_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".rfe";
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            /*for (byte nInd = 0; nInd < 32; nInd++)
            {
                m_nCalibrationCapMixer = nInd;
                byte nHex = MixerCapacitor2Hex();
                string sReport = nInd.ToString("X2") + ":" + nHex.ToString("X2") + "|";
                Hex2MixerCapacitor(nHex);
                sReport += m_nCalibrationCapMixer.ToString("X2");
                ReportLog(sReport);
            }*/

            m_sDefaultFolder = Environment.GetEnvironmentVariable("APPDATA") + "\\";

            //up to m_nTotalBufferSize pages of m_nFreqSpectrumSteps bytes each
            m_arrData               = new float[m_nTotalBufferSize, MAX_SPECTRUM_STEPS]; 

            m_arrRemoteScreenData   = new byte[m_nTotalBufferSize, 128 * 8];
            m_arrRemoteScreenData.Initialize();
            m_nScreenIndex          = 0;
            m_nMaxScreenIndex       = 0;

            m_arrRAWSnifferData     = new string[m_nTotalBufferSize];
            m_nRAWSnifferIndex      = 0;
            m_nMaxRAWSnifferIndex   = 0;
            numSampleDecoder.Maximum = m_nTotalBufferSize;
            numSampleDecoder.Minimum = 0;
            numSampleDecoder.Value  = 0;

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

                GetConnectedPorts();

                LoadProperties();
                InitializeSpectrumAnalyzerGraph();
                SetupSpectrumAnalyzerAxis();

                InitializeRAWDecoderGraph();

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
                ReportLog("Error in MainForm_Load");
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
                    ReportLog("ERROR: No valid COM ports available\r\nConnect RFExplorer and click on [*]");
            }
            catch (Exception obEx)
            {
                ReportLog("Error scanning COM ports: " + obEx.Message); 
            }
        }

        private void UpdateButtonStatus()
        {
            btnConnect.Enabled = !m_bPortConnected && (COMPortCombo.Items.Count>0);
            btnDisconnect.Enabled = m_bPortConnected;
            COMPortCombo.Enabled = !m_bPortConnected;
            comboBaudRate.Enabled = !m_bPortConnected;
            btnRescan.Enabled = !m_bPortConnected;
            chkDumpScreen.Enabled = m_bPortConnected;

            btnSendCmd.Enabled = m_bPortConnected;

            groupFreqSettings.Enabled = m_bPortConnected;
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

            panelConfiguration.Enabled = m_bPortConnected;

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

                ReportLog("Connected: " + m_serialPortObj.PortName.ToString() + ", " + m_serialPortObj.BaudRate.ToString() + " bauds");

                Thread.Sleep(500);
                StopAPIMode(); //stop api mode, if any
                Thread.Sleep(200);
                AskConfigData();
                Thread.Sleep(500);
                menuAutoLCDOff_Click(null, null);
            }
            catch (Exception obException) 
            { 
                ReportLog("ERROR ConnectPort: " + obException.Message); 
            }
            finally
            {
                Monitor.Exit(m_serialPortObj);
            }
            Cursor.Current = Cursors.Default;
        }

        private void LoadProperties()
        {
            comboBaudRate.SelectedItem  = RFExplorerClient.Properties.Settings.Default.COMSpeed;
            COMPortCombo.SelectedItem   = RFExplorerClient.Properties.Settings.Default.COMPort;
            menuSaveOnClose.Checked     = RFExplorerClient.Properties.Settings.Default.SaveOnClose;
            m_fStartFrequencyMHZ        = RFExplorerClient.Properties.Settings.Default.StartFreq;
            m_fStepFrequencyMHZ         = RFExplorerClient.Properties.Settings.Default.StepFreq;
            numericIterations.Value     = RFExplorerClient.Properties.Settings.Default.Calculator;
            m_fAmplitudeBottom          = RFExplorerClient.Properties.Settings.Default.BottomAmp;
            m_fAmplitudeTop             = RFExplorerClient.Properties.Settings.Default.TopAmp;
            m_bDrawMax                  = RFExplorerClient.Properties.Settings.Default.ViewMax;
            m_bDrawMin                  = RFExplorerClient.Properties.Settings.Default.ViewMin;
            m_bDrawRealtime             = RFExplorerClient.Properties.Settings.Default.ViewRT;
            m_bDrawAverage              = RFExplorerClient.Properties.Settings.Default.ViewAvg;
            numericZoom.Value           = RFExplorerClient.Properties.Settings.Default.ScreenZoom;
            m_bShowPeaks                = RFExplorerClient.Properties.Settings.Default.ViewPeaks;
            menuShowControlArea.Checked = RFExplorerClient.Properties.Settings.Default.ShowControlArea;
            m_bDark                     = RFExplorerClient.Properties.Settings.Default.DarkMode;
            menuAutoLCDOff.Checked      = RFExplorerClient.Properties.Settings.Default.AutoLCDOff;

            comboCustomCommand.DataSource = RFExplorerClient.Properties.Settings.Default.CustomCommands;
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
            RFExplorerClient.Properties.Settings.Default.StartFreq = m_fStartFrequencyMHZ;
            RFExplorerClient.Properties.Settings.Default.StepFreq = m_fStepFrequencyMHZ;
            RFExplorerClient.Properties.Settings.Default.Calculator = (int)numericIterations.Value;
            RFExplorerClient.Properties.Settings.Default.BottomAmp = m_fAmplitudeBottom;
            RFExplorerClient.Properties.Settings.Default.TopAmp = m_fAmplitudeTop;
            RFExplorerClient.Properties.Settings.Default.ViewMax = m_bDrawMax;
            RFExplorerClient.Properties.Settings.Default.ViewMin = m_bDrawMin;
            RFExplorerClient.Properties.Settings.Default.ViewRT = m_bDrawRealtime;
            RFExplorerClient.Properties.Settings.Default.ViewAvg = m_bDrawAverage;
            RFExplorerClient.Properties.Settings.Default.ScreenZoom = (int)numericZoom.Value;
            RFExplorerClient.Properties.Settings.Default.ViewPeaks = m_bShowPeaks;
            RFExplorerClient.Properties.Settings.Default.ShowControlArea = menuShowControlArea.Checked;
            RFExplorerClient.Properties.Settings.Default.DarkMode = m_bDark;
            RFExplorerClient.Properties.Settings.Default.AutoLCDOff = menuAutoLCDOff.Checked;
            RFExplorerClient.Properties.Settings.Default.Save();
        }

        private void ManualConnect()
        {
            ConnectPort(COMPortCombo.SelectedValue.ToString());

            SaveProperties();
        }

        private void ClosePort()
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                Monitor.Enter(m_serialPortObj);

                if (m_serialPortObj.IsOpen)
                {
                    StopAPIMode(); //stop api mode, if any
                    Thread.Sleep(200);
                    SendCommand("L1"); //restore LCD
                    Thread.Sleep(200);
                    SendCommand("CH"); //Switch data dump to off
                    Thread.Sleep(200);
                    //Close the port
                    ReportLog("Disconnected.");
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

        bool m_bModeAPI = false;

        private void ReceiveThreadfunc()
        {
            while (m_bRunReceiveThread)
            {
                string strReceived = "";
                while (m_bPortConnected && !m_bModeAPI)
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
                                  "         to double check there is not a newer version available.\r\n");
                }
                else
                {
                    string sText = "RF Explorer device has an older firmware version " + m_sRFExplorerFirmware +
                        "\r\nPlease upgrade it to required version " + m_sRFExplorerFirmwareCertified +
                        "\r\nVisit www.rf-explorer/download to get required firmware.";
                    MessageBox.Show(sText,"Firmware Warning");
                    ReportLog(sText);
                }
            }
        }

        UInt16 m_nTotalConfigurableSteps = 200;
        UInt32 m_nConfigurableStepHZ = 100000;
        double m_fConfigurableStartFreqMHZ = 1000.0f;

        private void RenderConfigurableAnalyzer()
        {
            string sDummy = m_serialPortObj.ReadExisting(); //clean all previous buffer, just in case

            zedSpectrumAnalyzer.GraphPane.CurveList.Clear();
            PointPairList RTList = new PointPairList();

            for (int nStep = 0; nStep < m_nTotalConfigurableSteps; nStep++)
            {
                string sCommand;
                double fFreqKHZ=m_fConfigurableStartFreqMHZ * 1000.0f + (m_nConfigurableStepHZ * nStep)/1000.0f;
                sCommand = "f" + (Convert.ToUInt32(fFreqKHZ)).ToString("D7") + "," + m_nConfigurableStepHZ.ToString("D7");
                SendCommand(sCommand);

                string sResponse="";
                do
                {
                    sResponse = m_serialPortObj.ReadExisting();
                } while (sResponse.Length == 0);

                double fVal = (double)(Convert.ToByte(sResponse[0]))/-2.0f;
                RTList.Add(fFreqKHZ/1000.0f, fVal, -120.0f);
            }
            ClosePort();

            LineItem RealtimeLine = zedSpectrumAnalyzer.GraphPane.AddCurve("API", RTList, Color.DarkBlue, SymbolType.None);
            RealtimeLine.Line.Width = 3;
            zedSpectrumAnalyzer.Refresh();
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
                                m_nDataIndex = m_nTotalBufferSize;
                                m_bHoldMode = true;
                                UpdateFeedMode();
                                ReportLog("Buffer is full.");
                            }
                            m_nMaxDataIndex = m_nDataIndex;
                            numericSampleSA.Value = m_nDataIndex;
                        }
                    }
                    else if ((sLine.Length > 2) && sLine.Substring(0, 2) == "$R")
                    {
                        if (!m_bHoldMode && m_nRAWSnifferIndex < m_nTotalBufferSize)
                        {
                            if (m_arrRAWSnifferData == null)
                            {
                                //don't allocate memory for this object before actually required as most users may not need this
                                m_arrRAWSnifferData = new string[m_nTotalBufferSize];
                            }
                            m_nRAWSnifferIndex++;
                            if (m_nRAWSnifferIndex >= m_nTotalBufferSize)
                            {
                                m_nRAWSnifferIndex = m_nTotalBufferSize;
                                m_bHoldMode = true;
                                UpdateFeedMode();
                                ReportLog("Buffer is full.");
                            }
                            else
                            {
                                m_arrRAWSnifferData[m_nRAWSnifferIndex] = sLine;
                            }
                            m_nMaxRAWSnifferIndex = m_nRAWSnifferIndex;
                            DrawRAWDecoder();
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
                    else if ((sLine.Length >= 5) && sLine.Substring(0, 3) == "$Cc")
                    {
                        m_nCalibrationCapSi4x = Convert.ToByte(sLine[4]);
                        Hex2MixerCapacitor(Convert.ToByte(sLine[5])); //m_nCalibrationCapMixer set internally by this function
                        ReportLog("Calibration data: 0x" + m_nCalibrationCapSi4x.ToString("x") + ", 0x" + m_nCalibrationCapMixer.ToString("x") + ", " + Convert.ToByte(sLine[6])+"ºC");
                    }
                    else if ((sLine.Length >= 5) && sLine.Substring(0, 3) == "$Cr")
                    {
                        //ignore - internally used by callers to this function, not really used here
                        int nSize=Convert.ToByte(sLine[3]);
                        string sDebugOutput="";
                        for (int nInd=0; nInd<nSize; nInd++)
                        {
                            if ((nInd % 16) == 0)
                            {
                                sDebugOutput += "DUMP: ["+nInd.ToString("X2")+"]:";
                            }
                            sDebugOutput+="["+Convert.ToByte(sLine[4+nInd]).ToString("X2")+"]";
                            if (((nInd + 1) % 16) == 0)
                            {
                                sDebugOutput += Environment.NewLine;
                            }
                        }
                        ReportLog(sDebugOutput);
                    }
                    else if ((sLine.Length > 6) && sLine.Substring(0, 6) == "#C2-M:")
                    {
                        ReportLog("Received RFExplorer device model info:" + sLine);
                        m_eMainBoardModel = (eModel)Convert.ToUInt16(sLine.Substring(6, 3));
                        m_eExpansionBoardModel = (eModel)Convert.ToUInt16(sLine.Substring(10, 3));
                        m_sRFExplorerFirmware = (sLine.Substring(14, 5));
                        DisplayRequiredFirmware();
                    }
                    else if ((sLine.Length > 6) && (sLine.Substring(0, 6) == "#C3-F:"))
                    {
                        ReportLog("Received configuration from RFExplorer device:" + sLine);
                        //#C3-F:<Ref_Freq>, <ExpModuleActive>, <CurrentMode>,  <Sample_Rate>, <Digital_Baudrate>, <Modulation><EOL>
                        //      6           14                 16              20             27                  34
                        if (sLine.Length >= 34)
                        {
                            double fRefMHZ = Convert.ToInt32(sLine.Substring(6, 7)) / 1000.0; //note it comes in KHZ
                            if (Math.Abs(fRefMHZ - m_fRefFrequencyMHZ) >= 0.001)
                            {
                                //We cannot use previous data if ref frequency changed
                                m_fRefFrequencyMHZ = fRefMHZ;
                                m_nMaxRAWSnifferIndex = 0;
                                m_nRAWSnifferIndex = 0;
                                ReportLog("New reference Freq - buffer cleared.");
                            }

                            m_bExpansionBoardActive = (sLine[14] == '1');
                            if (m_bExpansionBoardActive)
                                m_eActiveModel = m_eExpansionBoardModel;
                            else
                                m_eActiveModel = m_eMainBoardModel;

                            m_eMode = (eMode)Convert.ToUInt16(sLine.Substring(16, 3));

                            double fSampleRateKHZ = Convert.ToInt32(sLine.Substring(20, 6));        //note it comes in KHZ=KS/sec
                            double fDigitalBaudrateKHZ = Convert.ToInt32(sLine.Substring(27, 6));   //note it comes in KHZ=Kbauds
                            m_eModulation = (eModulation)Convert.ToUInt16(sLine.Substring(34, 1));
                            MainTab.SelectedTab = tabRAWDecoder;
                        }
                    }
                    else if ((sLine.Length > 6) && (sLine.Substring(0, 6) == "#C2-F:"))
                    {
                        ReportLog("Received configuration from RFExplorer device:" + sLine);
                        if (sLine.Length >= 60)
                        {
                            double fStartMHZ = Convert.ToInt32(sLine.Substring(6, 7)) / 1000.0; //note it comes in KHZ
                            double fStepMHZ = Convert.ToInt32(sLine.Substring(14, 7)) / 1000000.0;  //Note it comes in HZ
                            if ((Math.Abs(m_fStartFrequencyMHZ - fStartMHZ) >= 0.001) || (Math.Abs(m_fStepFrequencyMHZ - fStepMHZ) >= 0.001))
                            {
                                m_fStartFrequencyMHZ = fStartMHZ;
                                m_fStepFrequencyMHZ = fStepMHZ;
                                m_nDataIndex = 0; //we cannot use previous data for avg, etc when new frequency range is selected
                                ReportLog("New Freq range - buffer cleared.");
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
                    else if ((sLine.Length > 4) && (sLine.Substring(0, 5) == "#API0"))
                    {
                        //this may never reach here from the timer, but anyway, sanity code...
                        m_bModeAPI = false;
                    }
                    else if ((sLine.Length > 4) && (sLine.Substring(0, 5) == "#API1"))
                    {
                        m_bModeAPI = true;
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
                        ReportLog(sLine);
                    }
                    if (bWrongFormat)
                    {
                        ReportLog("Received unexpected data from RFExplorer device:" + sLine);
                        ReportLog("Please update your RF Explorer to a recent firmware version and");
                        ReportLog("make sure you are using the latest version of this software.");
                        ReportLog("Visit http://www.rf-explorer/download for latest updates.");
                    }
                } while (bProcessAllEvents && (m_arrReceivedStrings.Count > 0));
            }
            catch (Exception obEx)
            {
                ReportLog("ProcessReceivedString: " + obEx.Message);
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
                    DisplaySpectrumAnalyzerData();
                    UpdateWaterfall();
                }
            }
            catch (Exception obEx)
            {
                ReportLog("timer_receive_Tick: " + obEx.Message);
            }

            if (m_bFirstTick)
            {
                m_bFirstTick = false;
                ReportLog("");
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_bRunReceiveThread = false;
            if (menuSaveOnClose.Checked && (m_nMaxDataIndex > 0) && (m_sFilenameRFE.Length==0))
            {
                GetNewFilename();
                SaveFile(m_sDefaultFolder + m_sFilenameRFE);
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

            if (m_eMode == eMode.MODE_SPECTRUM_ANALYZER)
            {
                //objGraph is a standard graph
            }
            else if (m_eMode == eMode.MODE_WIFI_ANALYZER)
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

        double m_fPeakValueMHZ = 0.0f;
        double m_fPeakValueAmp = -120.0f;

        private void DisplaySpectrumAnalyzerData()
        {
            double fNoiseFloor = -120.0;

            m_fPeakValueMHZ = 0.0;
            m_fPeakValueAmp = fNoiseFloor;

            PointPairList RTList = new PointPairList();
            PointPairList MaxList = new PointPairList();
            PointPairList MinList = new PointPairList();
            PointPairList AvgList = new PointPairList();

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
                    RTList.Add(fFreq, fVal, fNoiseFloor);
                if (m_bDrawMin)
                    MinList.Add(fFreq, fMin, fNoiseFloor);

                if (m_bDrawMax)
                {
                    MaxList.Add(fFreq, fMax, fNoiseFloor);
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
                    AvgList.Add(fFreq, fValAvg, fNoiseFloor);
                    if (fValAvg > fAverageMax_Amp)
                    {
                        fAverageMax_Amp = fValAvg;
                        fAverageMax_Iter = nInd;
                    }
                }
            }

            if (m_bCalibrating)
            {
                m_fPeakValueMHZ = (m_fStartFrequencyMHZ + m_fStepFrequencyMHZ * fAverageMax_Iter);
                m_fPeakValueAmp = fAverageMax_Amp;
            }
            else
            {
                zedSpectrumAnalyzer.GraphPane.CurveList.Clear();
                if (m_bDrawAverage)
                {
                    LineItem AvgLine = zedSpectrumAnalyzer.GraphPane.AddCurve("Avg", AvgList, Color.Brown, SymbolType.None);
                    AvgLine.Line.Width = 2;
                    double fFreqMark = (m_fStartFrequencyMHZ + m_fStepFrequencyMHZ * fAverageMax_Iter);
                    m_fPeakValueMHZ = fFreqMark;
                    if (m_bShowPeaks)
                    {
                        m_AveragePeak.Text = fFreqMark.ToString("0.000") + "MHZ\n" + fAverageMax_Amp.ToString("0.0") + "dBm";
                        m_AveragePeak.Location.X = fFreqMark;
                        m_AveragePeak.Location.Y = fAverageMax_Amp;
                    }
                }
                if (m_bDrawRealtime)
                {
                    LineItem RealtimeLine = zedSpectrumAnalyzer.GraphPane.AddCurve("Realtime", RTList, Color.Blue, SymbolType.None);
                    RealtimeLine.Line.Width = 3;
                    double fFreqMark = (m_fStartFrequencyMHZ + m_fStepFrequencyMHZ * fRealtimeMax_Iter);
                    if (m_fPeakValueMHZ == 0.0f)
                        m_fPeakValueMHZ = fFreqMark;
                    if (m_bShowPeaks)
                    {
                        m_RealtimePeak.Text = fFreqMark.ToString("0.000") + "MHZ\n" + fRealtimeMax_Amp.ToString() + "dBm";
                        m_RealtimePeak.Location.X = fFreqMark;
                        m_RealtimePeak.Location.Y = fRealtimeMax_Amp;
                    }
                }
                if (m_bDrawMax)
                {
                    if (m_eMode == eMode.MODE_SPECTRUM_ANALYZER)
                    {
                        LineItem MaxLine = zedSpectrumAnalyzer.GraphPane.AddCurve("Max", MaxList, Color.Red, SymbolType.None);
                        MaxLine.Line.Width = 2;
                        double fFreqMark = (m_fStartFrequencyMHZ + m_fStepFrequencyMHZ * fMaxMax_Iter);
                        if (m_fPeakValueMHZ == 0.0f)
                            m_fPeakValueMHZ = fFreqMark;
                        if (m_bShowPeaks)
                        {
                            m_MaxPeak.Text = fFreqMark.ToString("0.000") + "MHZ\n" + fMaxMax_Amp.ToString() + "dBm";
                            m_MaxPeak.Location.X = fFreqMark;
                            m_MaxPeak.Location.Y = fMaxMax_Amp;
                        }
                    }
                    else if (m_eMode == eMode.MODE_WIFI_ANALYZER)
                    {
                        BarItem MaxBar = zedSpectrumAnalyzer.GraphPane.AddHiLowBar("Max", MaxList, Color.Red);

                        //MaxBar.Bar.Fill.IsVisible = false;
                        MaxBar.Bar.Border.Color = Color.DarkRed;
                        MaxBar.Bar.Border.Width = 2;
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
                    LineItem MinLine = zedSpectrumAnalyzer.GraphPane.AddCurve("Min", MinList, Color.DarkGreen, SymbolType.None);
                    MinLine.Line.Width = 2;
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
                    //ReportLog("-> Sent to RFE: " + "#["+(sData.Length + 2).ToString()+"]"+sData);
                }
            }
            catch (Exception obEx)
            {
                ReportLog("SendCommand error: " + obEx.Message);
            }
        }

        private void AskConfigData()
        {
            if (m_bPortConnected)
            {
                SendCommand("C0");
            }
        }

        private void StartAPIMode()
        {
            if (m_bPortConnected)
            {
                SendCommand("a");
                //Note m_bModeAPI will be updated by the timer if properly received string is found after this
            }
        }

        private void StopAPIMode()
        {
            if (m_bPortConnected)
            {
                SendCommand("A");
            }
            m_bModeAPI = false;
        }

        private void UpdateRemoteConfigData()
        {
            if (m_bPortConnected)
            {
                double fStartFreq = Convert.ToDouble(m_sStartFreq.Text);
                double fEndFreq = Convert.ToDouble(m_sEndFreq.Text);

                //#[32]C2-F:Sssssss,Eeeeeee,tttt,bbbb
                UInt32 nStartKhz = (UInt32)(fStartFreq * 1000);
                UInt32 nEndKhz = (UInt32)(fEndFreq * 1000);
                Int16 nTopDBM = (Int16)(Convert.ToDouble(m_sTopDBM.Text));
                Int16 nBottomDBM = (Int16)(Convert.ToDouble(m_sBottomDBM.Text));

                string sData = "C2-F:" +
                    nStartKhz.ToString("D7") + "," + nEndKhz.ToString("D7") + "," +
                    nTopDBM.ToString("D3") + "," + nBottomDBM.ToString("D3");
                SendCommand(sData);

                Thread.Sleep(500); //wait some time for the unit to process changes, otherwise may get a different command too soon
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
                ReportLog("Buffer cleared.");
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

        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            menuSaveAsRFE.Enabled = (m_nMaxDataIndex > 0) && (MainTab.SelectedTab == tabSpectrumAnalyzer);
            menuSaveCSV.Enabled = (m_nMaxDataIndex > 0) && (MainTab.SelectedTab == tabSpectrumAnalyzer);
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
                using (StreamWriter myFile = new StreamWriter(sFilename, true))
                {
                    myFile.WriteLine("RF Explorer CSV data file: " + FileHeaderVersioned());
                    myFile.WriteLine("Start Frequency: " + m_fStartFrequencyMHZ.ToString()+
                        "MHZ\r\nStep Frequency: " + (m_fStepFrequencyMHZ*1000).ToString()+
                        "KHZ\r\nTotal data entries: " + m_nMaxDataIndex.ToString()+
                        "\r\nSteps per entry: "+ m_nFreqSpectrumSteps.ToString());

                    for (int nPageInd = 0; nPageInd < m_nMaxDataIndex; nPageInd++)
                    {
                        myFile.Write(nPageInd.ToString()+"\t");

                        for (int nByte = 0; nByte < m_nFreqSpectrumSteps; nByte++)
                        {
                            myFile.Write(((double)m_arrData[nPageInd, nByte]).ToString());
                            if (nByte!=(m_nFreqSpectrumSteps-1))
                                myFile.Write("\t");
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
            ReportLog("Found total ports: " + arrPortIndexes.Length.ToString());

            //List all configured ports and driver versions for CP210x
            foreach (string sPortIndex in arrPortIndexes)
            {
                try
                {
                    RegistryKey regPort = regPortKey.OpenSubKey(sPortIndex, RegistryKeyPermissionCheck.ReadSubTree, System.Security.AccessControl.RegistryRights.QueryValues);

                    ReportLog("COM port index: " + sPortIndex);
                    if (regPort != null)
                    {
                        Object obDriverDesc = regPort.GetValue("DriverDesc");
                        string sDriverDesc = obDriverDesc.ToString();
                        ReportLog("   DriverDesc: " + sDriverDesc);
                        if (!sDriverDesc.Contains("CP210x"))
                            continue; //if it is not a Silicon Labs CP2102, ignore next steps
                        Object obCOMID = regPort.GetValue("AssignedPortForQCDevice");
                        if (obCOMID != null)
                            ReportLog("   AssignedPortForQCDevice: " + obCOMID.ToString());
                        Object obDriverVersion = regPort.GetValue("DriverVersion");
                        ReportLog("   DriverVersion: " + obDriverVersion.ToString());
                        Object obDriverDate = regPort.GetValue("DriverDate");
                        ReportLog("   DriverDate: " + obDriverDate.ToString());
                        Object obMatchingDeviceId = regPort.GetValue("MatchingDeviceId");
                        ReportLog("   MatchingDeviceId: " + obMatchingDeviceId.ToString());
                    }
                }
                catch (Exception obEx) { ReportLog(obEx.Message); };
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
            ReportLog("Found total CP210x entries: " + arrDeviceCP210x.Length.ToString());
            //Iterate all driver for CP210x and get those with a valid connected COM port
            foreach (string sUSBIndex in arrDeviceCP210x)
            {
                try
                {
                    RegistryKey regUSBID = regUSBKey.OpenSubKey(sUSBIndex, RegistryKeyPermissionCheck.ReadSubTree, System.Security.AccessControl.RegistryRights.QueryValues | System.Security.AccessControl.RegistryRights.EnumerateSubKeys);
                    if (regUSBID != null)
                    {
                        Object obFriendlyName = regUSBID.GetValue("FriendlyName");
                        ReportLog("   FriendlyName: " + obFriendlyName.ToString());
                        RegistryKey regDevice = regUSBID.OpenSubKey("Device Parameters", RegistryKeyPermissionCheck.ReadSubTree, System.Security.AccessControl.RegistryRights.QueryValues);
                        if (regDevice != null)
                        {
                            object obPortName = regDevice.GetValue("PortName");
                            string sPortName = obPortName.ToString();
                            ReportLog("   PortName: " + sPortName);
                            if (IsConnectedPort(sPortName) && !IsRepeatedPort(sPortName))
                            {
                                ReportLog(sPortName + " is a valid available port.");
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
                catch (Exception obEx) { ReportLog(obEx.Message); };
            }
            string sTotalPortsFound = "0";
            if (m_arrValidCP2101Ports != null)
                sTotalPortsFound = m_arrValidCP2101Ports.Length.ToString();
            ReportLog("Total ports found: " + sTotalPortsFound);
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

            StopAPIMode();

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

            RelocateRemoteControl();

            btnTop5plus.Top = zedSpectrumAnalyzer.Top;
            btnTop5plus.Left = zedSpectrumAnalyzer.Right + 8;
            if (m_arrAnalyzerButtonList[0] != null)
            {
                for (int nInd = 1; nInd < m_arrAnalyzerButtonList.Length; nInd++)
                {
                    m_arrAnalyzerButtonList[nInd].Top = m_arrAnalyzerButtonList[nInd - 1].Bottom + 3;
                    m_arrAnalyzerButtonList[nInd].Left = m_arrAnalyzerButtonList[0].Left;
                }
            }
        }

        private void tabConfiguration_Enter(object sender, EventArgs e)
        {
            groupCOM.Parent = tabConfiguration;
            groupDataFeed.Parent = tabConfiguration;
            groupFreqSettings.Parent = tabConfiguration;
            DisplayGroups();
            m_edCalibrationFreq_Leave(null, null);
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
                        ReportLog("RFS file loaded: " + sHeader + " with total samples:" + m_nMaxScreenIndex.ToString());
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

        private void ReportLog(string sLine)
        {
            if (m_bFirstText)
            {
                m_sReportFilePath = Environment.GetEnvironmentVariable("APPDATA") + "\\RFExplorerClient_report.log";
                textBox_message.AppendText("Welcome to RFExplorer Client - report being saved to " + m_sReportFilePath + Environment.NewLine);
            }
            else
                sLine = Environment.NewLine + sLine;

            textBox_message.AppendText(sLine);

            using (StreamWriter sr = new StreamWriter(m_sReportFilePath, true))
            {
                if (m_bFirstText)
                {
                    sr.WriteLine(Environment.NewLine + Environment.NewLine +
                        "===========================================");
                    sr.WriteLine(
                        "RFExplorer client session " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                    sr.WriteLine(
                        "===========================================" + Environment.NewLine + Environment.NewLine);
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
                ReportLog("Command sent: " + sCmd);
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
                ReportLog("Command sent: " + sCmd);
            }
            else
            {
                MessageBox.Show("Nothing to send.\nSpecify a command first...");
            }
        }

        #endregion

        #region RAWDecoder Window

        private void InitializeRAWDecoderGraph()
        {
            // Get a reference to the GraphPane instance in the ZedGraphControl
            GraphPane myPane = zedRAWDecoder.GraphPane;
            DefineGraphColors(zedRAWDecoder);

            // Set the titles and axis labels
            //myPane.Title.FontSpec.Size = 10;
            //myPane.Title.IsVisible = false;
            myPane.XAxis.Title.Text = "Data Sample";
            myPane.XAxis.Scale.MajorStep = 1000;
            myPane.XAxis.Scale.MinorStep = 100;

            // Show the x axis grid
            myPane.XAxis.MajorGrid.IsVisible = true;
            myPane.XAxis.Type = AxisType.Linear;

            myPane.YAxis.Title.IsVisible = false;
            myPane.YAxis.Type = AxisType.Linear;
            myPane.YAxis.Scale.Align = AlignP.Inside;
            myPane.YAxis.IsVisible = false;
            // Manually set the axis range
            myPane.YAxis.Scale.Min = -3.8;
            myPane.YAxis.Scale.Max = 1.2;
            myPane.XAxis.Scale.Min = 0;
            myPane.XAxis.Scale.Max = MAX_RAW_SAMPLE;

            //this doesn't work as expected when enabled...
            zedRAWDecoder.HorizontalScroll.Enabled = false;
            zedRAWDecoder.HorizontalScroll.Visible = false;
            zedRAWDecoder.HorizontalScroll.LargeChange = 10;
            zedRAWDecoder.HorizontalScroll.SmallChange = 1;
            zedRAWDecoder.HorizontalScroll.Maximum = 100;
            zedRAWDecoder.HorizontalScroll.Minimum = 0;
            zedRAWDecoder.AutoScroll = true;
            zedRAWDecoder.IsEnableHPan = true;
            //zedRAWDecoder.HorizontalScroll.Value = 20000;

            zedRAWDecoder.IsShowPointValues = true;
            zedRAWDecoder.PointValueEvent += new ZedGraphControl.PointValueHandler(RAWDecoderPointValueHandler);
        }

        private string RAWDecoderPointValueHandler(ZedGraphControl control, GraphPane pane,
            CurveItem curve, int iPt)
        {
            // Get the PointPair that is under the mouse
            PointPair pt = curve[iPt];

            return "Sample: "+pt.X.ToString();
        }


        private void DrawRAWSingleLine(int nPrevOrdinal, Color drawColor)
        {
            Int16 nDrawInd = (Int16)(m_nRAWSnifferIndex - nPrevOrdinal);
            if (nDrawInd > 0)
            {
                string sLine = m_arrRAWSnifferData[nDrawInd];
                PointPairList RAWList = new PointPairList();
                int nSize = sLine.Length - 2;
                Byte nPrevValue = 0;
                Byte nCurrentValue = 0;
                for (int nInd = 0; nInd < nSize; nInd++)
                {
                    Byte nVal = Convert.ToByte(sLine[nInd]);
                    for (int nBitInd = 7; nBitInd >= 0; nBitInd--)
                    {
                        nCurrentValue = (Byte)((nVal >> nBitInd) & 0x01);
                        if (nCurrentValue != nPrevValue)
                        {
                            if ((nInd > 0) || (nBitInd < 7))
                            {
                                RAWList.Add(nInd * 8 + (7 - nBitInd) - 1, nCurrentValue - 1.2*nPrevOrdinal);
                            }
                        }
                        RAWList.Add(nInd * 8 + (7 - nBitInd), nCurrentValue - 1.2*nPrevOrdinal);
                        nPrevValue = nCurrentValue;
                    }
                }
                LineItem RealtimeLine = zedRAWDecoder.GraphPane.AddCurve("RAW " + nDrawInd.ToString(), RAWList, drawColor, SymbolType.None);
            }
        }

        private void DrawRAWDecoder()
        {
            numSampleDecoder.Value = m_nRAWSnifferIndex;

            zedRAWDecoder.GraphPane.CurveList.Clear();
            DrawRAWSingleLine(0, Color.Blue);
            if (numMultiGraph.Value > 1)
                DrawRAWSingleLine(1, Color.DarkMagenta);
            if (numMultiGraph.Value > 2)
                DrawRAWSingleLine(2, Color.Red);
            if (numMultiGraph.Value > 3)
                DrawRAWSingleLine(3, Color.Brown);
            zedRAWDecoder.Refresh();
        }

        private void SaveRAWDecoderInCSV(string sFilename)
        {
            if (m_nMaxRAWSnifferIndex == 0)
            {
                MessageBox.Show("Capture data first...");
                return;
            }

            try
            {
                using (StreamWriter myFile = new StreamWriter(sFilename, true))
                {
                    string sLine = m_arrRAWSnifferData[m_nMaxRAWSnifferIndex];

                    myFile.WriteLine("RF Explorer RAW Decoder CSV data file: " + FileHeaderVersioned());
                    myFile.WriteLine("Reference Frequency: " + m_fStartFrequencyMHZ.ToString() +
                        "MHZ\r\nTotal data page captures: " + (m_nMaxRAWSnifferIndex).ToString()+"\r\n"+
                        "Total bits captured: "+(m_nMaxRAWSnifferIndex*(sLine.Length-2)*8).ToString()+"\r\n\r\n");

                    for (int nPageInd = 1; nPageInd < m_nMaxRAWSnifferIndex; nPageInd++)
                    {
                        myFile.Write(nPageInd.ToString() + "\t");

                        sLine = m_arrRAWSnifferData[nPageInd];
                        int nSize = sLine.Length - 2;
                        for (int nInd = 0; nInd < nSize; nInd++)
                        {
                            Byte nVal = Convert.ToByte(sLine[nInd]);
                            for (int nBitInd = 7; nBitInd >= 0; nBitInd--)
                            {
                                myFile.Write(((nVal >> nBitInd) & 0x01).ToString()+"\t");
                            }
                        }                        
                        myFile.Write("\r\n");
                    }
                }
            }
            catch (Exception obEx) { MessageBox.Show(obEx.Message); }
        }

        private void btnSaveRAWDecoderCSV_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog MySaveFileDialog = new SaveFileDialog())
                {
                    MySaveFileDialog.Filter = "RFExplorer CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                    MySaveFileDialog.FilterIndex = 1;
                    MySaveFileDialog.RestoreDirectory = false;

                    GetNewFilename();
                    MySaveFileDialog.FileName = m_sFilenameRFE.Replace(".rfe", ".RAWDecoder.csv");

                    if (MySaveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        SaveRAWDecoderInCSV(MySaveFileDialog.FileName);
                    }
                }
            }
            catch (Exception obEx) { MessageBox.Show(obEx.Message); }
        }

        private void tabRAWDecoder_Enter(object sender, EventArgs e)
        {
            tabRAWDecoder.BackColor = Color.LightYellow;
            DefineGraphColors(zedRAWDecoder);
            if (m_fRefFrequencyMHZ == 0.0)
            {
                m_fRefFrequencyMHZ = CalculateCenterFrequencyMHZ();
            }

            UpdateDialogFromFreqSettings();

            groupCOM.Parent = tabRAWDecoder;
            UpdateButtonStatus();
            DisplayGroups();
        }

        private void chkRunDecoder_CheckedChanged(object sender, EventArgs e)
        {
            m_bHoldMode = !chkRunDecoder.Checked;
            UpdateFeedMode();
        }

        private void chkHoldDecoder_CheckedChanged(object sender, EventArgs e)
        {
            m_bHoldMode = chkHoldDecoder.Checked;
            UpdateFeedMode();
        }

        private void numMultiGraph_ValueChanged(object sender, EventArgs e)
        {
            DrawRAWDecoder();
        }

        private void numSampleDecoder_ValueChanged(object sender, EventArgs e)
        {
            if (m_bHoldMode)
            {
                m_nRAWSnifferIndex= (UInt16)numSampleDecoder.Value;
                if (m_nRAWSnifferIndex > m_nMaxRAWSnifferIndex)
                {
                    m_nRAWSnifferIndex= m_nMaxRAWSnifferIndex;
                    numSampleDecoder.Value = m_nMaxRAWSnifferIndex;
                }
                DrawRAWDecoder();
            }
        }

        private void m_sRefFrequency_Leave(object sender, EventArgs e)
        {
            m_fRefFrequencyMHZ = Convert.ToDouble(m_sRefFrequency.Text);
            m_fRefFrequencyMHZ = Math.Max(m_fMinFreqMHZ, m_fRefFrequencyMHZ);
            m_fRefFrequencyMHZ = Math.Min(m_fMaxFreqMHZ, m_fRefFrequencyMHZ);
            m_sRefFrequency.Text = m_fRefFrequencyMHZ.ToString("f3");
        }
        #endregion

        #region Calibration
        bool m_bCalibrating = false;

        private bool CalibrateSi4x(int nSpanKHZ, byte nStepSize, int nSeconds, CalibrationProgress wndProgress, out double fDeltaKHZ)
        {
            wndProgress.textLine1.Text = "Variation Range: " + nSpanKHZ.ToString() + "KHz";
            Application.DoEvents();
            Cursor.Current = Cursors.WaitCursor;
            SendCommand("Cc"); //ask for calibration data
            double fCenterFreqKHZ = Convert.ToDouble(m_edCalibrationFreq.Text) * 1000.0;
            double fStart = fCenterFreqKHZ - nSpanKHZ/2;//Find the start frequency for this value to be at the center of a 112KHz min span
            double fEnd = fCenterFreqKHZ + nSpanKHZ / 2;
            ReportLog(wndProgress.textLine1.Text + " Center:" + fCenterFreqKHZ + "KHz");
            Thread.Sleep(1000);
            SendCommand("C2-F" + ((UInt32)fStart).ToString("D7") + "," + ((UInt32)fEnd).ToString("D7") + ",-010,-120");

            bool bCalibrated = false;
            int nSteps = 256 / nStepSize;
            int nDirection = 0;
            do
            {
                Thread.Sleep(1000 * nSeconds);
                string sDummy;
                ProcessReceivedString(true, out sDummy);
                DisplaySpectrumAnalyzerData();
                
                Int32 nDelta = Convert.ToInt32(m_fPeakValueMHZ * 1000 * 1000 - fCenterFreqKHZ * 1000);
                fDeltaKHZ = nDelta / 1000.0;
                if (nDelta > 500000)
                {
                    string sText = "Error: reference signal is too far from expected value.\n" +
                        "Check your RF Signal frequency and amplitude (must be between -20 to -50dBm)\n" +
                        "If you think this is an error, please contact rfexplorer@arocholl.com";
                    ReportLog(sText);
                    MessageBox.Show(sText, "Calibration Error");
                    return false;
                }
                wndProgress.textLine2.Text = "Error delta: " + (nDelta / 1000.0).ToString() + "KHz, CAL:0x" + m_nCalibrationCapSi4x.ToString("X2");
                ReportLog(wndProgress.textLine2.Text + " nDirection:" + nDirection + " nSteps:" + nSteps + " nStepSize:" + nStepSize + " Peak: "+m_fPeakValueAmp);
                Application.DoEvents();
                Cursor.Current = Cursors.WaitCursor;
                GetNewFilename();
                SaveFile(m_sDefaultFolder + m_sFilenameRFE);
                ReportLog("Saved filename: " + m_sDefaultFolder + m_sFilenameRFE);

                if (nDelta == 0)
                {
                    bCalibrated = true;
                    nDirection = 0;
                }
                else if (nDelta < 0)
                {
                    if (nDirection > 0)
                    {
                        bCalibrated = true;
                    }
                    else
                    {
                        nDirection = -1;

                        if (m_nCalibrationCapSi4x == 255)
                        {
                            //finish here
                            nSteps = 1;
                        }
                        else
                        {
                            if ((int)m_nCalibrationCapSi4x + (int)nStepSize > 255)
                                m_nCalibrationCapSi4x = 255;
                            else
                                m_nCalibrationCapSi4x += nStepSize;
                            SendCommand("CC" + Convert.ToChar(m_nCalibrationCapSi4x) + Convert.ToChar(0));
                        }
                    }
                }
                else
                {
                    if (nDirection < 0)
                    {
                        bCalibrated = true;
                    }
                    else
                    {
                        nDirection = 1;
                        if (m_nCalibrationCapSi4x == 0)
                        {
                            //finish here
                            nSteps = 1;
                        }
                        else
                        {
                            if ((int)m_nCalibrationCapSi4x - (int)nStepSize < 0)
                                m_nCalibrationCapSi4x = 0;
                            else
                                m_nCalibrationCapSi4x -= nStepSize;
                            SendCommand("CC" + Convert.ToChar(m_nCalibrationCapSi4x) + Convert.ToChar(0));
                        }
                    }
                }

                nSteps--;
            } while (!bCalibrated && nSteps > 0);

            ReportLog("Calibration step finished with 0x" + m_nCalibrationCapSi4x.ToString("X2") + "," + MixerCapacitor2Hex().ToString("X2") + " Center: " + m_fPeakValueMHZ + "MHz");
            return true;
        }

        //A value from 0-31 is valid for m_nCalibrationCapMixer
        //But the RF2052 will take it as a 0xPQ where P is a 4 bit value for XO_CT and Q is a 1 bit value for XO_CR_S
        //Therefore a simple | and <<8 is required on the RF Explorer to use the HEX value
        private byte MixerCapacitor2Hex()
        {
            byte nHex = 0;

            if ((m_nCalibrationCapMixer & 0x01)==1)
            {
                nHex = 0x02;    //XO_CT
            }
            nHex |= (byte)((m_nCalibrationCapMixer & 0x1e) << 3); //XO_CR_S

            return nHex;
        }

        private void Hex2MixerCapacitor(byte nHex)
        {
            m_nCalibrationCapMixer = (byte)((nHex & 0xf0) >> 3); //XO_CR_S
            if ((nHex & 0x02) == 0x02)
            {
                m_nCalibrationCapMixer |= 0x01; //XO_CT
            }
        }

        //Test code
        //for (byte nInd = 0; nInd < 32; nInd++)
        //{
        //    m_nCalibrationCapMixer = nInd;
        //    byte nHex=MixerCapacitor2Hex();
        //    string sReport=nInd.ToString("X2")+":"+nHex.ToString("X2")+"|";
        //    Hex2MixerCapacitor(nHex);
        //    sReport+=m_nCalibrationCapMixer.ToString("X2");
        //    ReportLog(sReport);
        //}

        
        private bool CalibrateMixer(int nSpanKHZ, int nSeconds, CalibrationProgress wndProgress)
        {
            wndProgress.textLine1.Text = "Variation Range: " + nSpanKHZ.ToString() + "KHz";
            Application.DoEvents();
            Cursor.Current = Cursors.WaitCursor;
            SendCommand("Cc"); //ask for calibration data
            double fCenterFreqKHZ = 540000.0; //hardcoded for internal CALIBRATE_WSUB3G_LO1 mode
            double fStart = fCenterFreqKHZ - nSpanKHZ / 2;//Find the start frequency for this value to be at the center of a 112KHz min span
            double fEnd = fCenterFreqKHZ + nSpanKHZ / 2;
            ReportLog(wndProgress.textLine1.Text + " Center:" + fCenterFreqKHZ + "KHz");
            Thread.Sleep(1000);
            SendCommand("C2-F" + ((UInt32)fStart).ToString("D7") + "," + ((UInt32)fEnd).ToString("D7") + ",-010,-120");

            bool bCalibrated = false;
            int nSteps = 32;
            int nDirection = 0;
            do
            {
                Thread.Sleep(1000 * nSeconds);
                string sDummy;
                ProcessReceivedString(true, out sDummy);
                DisplaySpectrumAnalyzerData();
                Int32 nDelta = Convert.ToInt32(m_fPeakValueMHZ * 1000 * 1000 - fCenterFreqKHZ * 1000);
                if (nDelta > 100000)
                {
                    string sText = "Error: reference signal is too far from expected value.\n"+
                        "If you think this is an error, please contact rfexplorer@arocholl.com";
                    ReportLog(sText);
                    MessageBox.Show(sText,"Calibration Error");
                    return false;
                }
                wndProgress.textLine2.Text = "Error delta: " + (nDelta / 1000.0).ToString() + "KHz, CAL:0x" + m_nCalibrationCapMixer.ToString("X2");
                ReportLog(wndProgress.textLine2.Text + " nDirection:" + nDirection + " nSteps:" + nSteps + " Peak: " + m_fPeakValueAmp);
                Application.DoEvents();
                Cursor.Current = Cursors.WaitCursor;
                GetNewFilename();
                SaveFile(m_sDefaultFolder + m_sFilenameRFE);
                ReportLog("Saved filename: " + m_sDefaultFolder + m_sFilenameRFE);

                if (nDelta == 0)
                {
                    bCalibrated = true;
                    nDirection = 0;
                }
                else if (nDelta < 0)
                {
                    if (nDirection > 0)
                    {
                        bCalibrated = true;
                    }
                    else
                    {
                        nDirection = -1;

                        if (m_nCalibrationCapMixer == 0)
                        {
                            //finish here
                            nSteps = 1;
                        }
                        else
                        {
                            m_nCalibrationCapMixer -= 1;
                            SendCommand("CC" + Convert.ToChar(m_nCalibrationCapSi4x) + Convert.ToChar(MixerCapacitor2Hex()));
                        }
                    }
                }
                else
                {
                    if (nDirection < 0)
                    {
                        bCalibrated = true;
                    }
                    else
                    {
                        nDirection = 1;
                        if (m_nCalibrationCapMixer == 31)
                        {
                            //finish here
                            nSteps = 1;
                        }
                        else
                        {
                            m_nCalibrationCapMixer += 1;
                            SendCommand("CC" + Convert.ToChar(m_nCalibrationCapSi4x) + Convert.ToChar(MixerCapacitor2Hex()));
                        }
                    }
                }

                nSteps--;
            } while (!bCalibrated && nSteps > 0);

            ReportLog("Calibration step finished with 0x" + m_nCalibrationCapSi4x.ToString("X2") + "," + MixerCapacitor2Hex().ToString("X2") + " Center: " + m_fPeakValueMHZ + "MHz");
            return true;
        }

        const byte FLASH_FILE_CALIBRATION_DATE=50;

        private void btnCalibrate_Click(object sender, EventArgs e)
        {
            if (m_eActiveModel == eModel.MODEL_2400)
            {
                MessageBox.Show("This RF Explorer model is not yet supported");
                return;
            }

            double fOldStart = m_fStartFrequencyMHZ;
            double fOldStep = m_fStepFrequencyMHZ;
            double fOldTop = m_fAmplitudeTop;
            double fOldBottom = m_fAmplitudeBottom;

            {
                //DateTime CalibrationTime = DateTime.Now;
                //string sCalibrationTime = "CR" + Convert.ToChar(0) + Convert.ToChar(FLASH_FILE_CALIBRATION_DATE) +
                //    CalibrationTime.Year.ToString("D4") + CalibrationTime.Month.ToString("D2") + CalibrationTime.Day.ToString("D2") +
                //    CalibrationTime.Hour.ToString("D2") + CalibrationTime.Minute.ToString("D2") + CalibrationTime.Second.ToString("D2");
                //SendCommand(sCalibrationTime);
                //Thread.Sleep(1000);
                //SendCommand("CS");
                //Thread.Sleep(1000);
                //SendCommand("Cr" + Convert.ToChar(0) + Convert.ToChar(0) + Convert.ToChar(32));
                //return;
            }

            //if (m_sExpansionBoardCalibrated.Length == 0)
            //{
            //    //ask for calibration date
            //    m_sExpansionBoardCalibrated = "";
            //    SendCommand("Cr" + Convert.ToChar(0) + Convert.ToChar(FLASH_FILE_CALIBRATION_DATE) + Convert.ToChar(14));
            //    Thread.Sleep(500);
            //    for (int nInd = 0; nInd < 10; nInd++)
            //    {
            //        string sOut;
            //        ProcessReceivedString(false, out sOut);
            //        if ((sOut.Length > 14) && (sOut.Substring(0, 3) == "$Cr"))
            //        {
            //            m_sExpansionBoardCalibrated = "Expansion module Calibrated on: " + sOut.Substring(4);
            //            break;
            //        }
            //    }
            //}


            CalibrationProgress wndProgress = new CalibrationProgress();

            try
            {
                m_timer_receive.Enabled = false;

                int nTotalSteps = 3;
                if (m_eActiveModel == eModel.MODEL_WSUB3G)
                {
                    nTotalSteps = 5;
                }

                m_bCalibrating = true;
                if (DialogResult.OK == MessageBox.Show("Connect or power ON the RF Source of " + Convert.ToDouble(m_edCalibrationFreq.Text) + "MHz and (-20 to -50dBm)\n" +
                    "Click to continue only when ready...", "RF Explorer Calibration",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1))
                {
                    wndProgress.Show(this);

                    wndProgress.textLine3.Text = string.Format("Calibrating - Step 1/{0}...", nTotalSteps);
                    wndProgress.ProgressBar.Maximum = nTotalSteps;
                    wndProgress.ProgressBar.Value = 0;
                    Application.DoEvents();
                    Cursor.Current = Cursors.WaitCursor;
                    double fDeltaKHZ;
                    bool bOk = CalibrateSi4x(600, 10, 3, wndProgress, out fDeltaKHZ);

                    if (bOk)
                    {
                        wndProgress.textLine3.Text = string.Format("Calibrating - Step 2/{0}...", nTotalSteps);
                        wndProgress.ProgressBar.Value = 1;
                        Application.DoEvents();
                        Cursor.Current = Cursors.WaitCursor;
                        bOk = CalibrateSi4x(200, 3, 3, wndProgress, out fDeltaKHZ);
                    }

                    if (bOk)
                    {
                        wndProgress.textLine3.Text = string.Format("Calibrating - Step 3/{0}...", nTotalSteps);
                        wndProgress.ProgressBar.Value = 2;
                        Application.DoEvents();
                        Cursor.Current = Cursors.WaitCursor;
                        bOk = CalibrateSi4x(112, 1, 10, wndProgress, out fDeltaKHZ);
                    }

                    if (Math.Abs(fDeltaKHZ) > 2.0f)
                    {
                    }
                    else
                    {
                        if (m_eActiveModel == eModel.MODEL_WSUB3G && bOk)
                        {
                            if (DialogResult.OK == MessageBox.Show("Disconnect or power off the RF Source\nClick to continue only when ready...", "RF Signal Source",
                                MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1))
                            {
                                wndProgress.textLine3.Text = string.Format("Calibrating - Step 4/{0}...", nTotalSteps);
                                wndProgress.ProgressBar.Value = 3;
                                Application.DoEvents();
                                Cursor.Current = Cursors.WaitCursor;
                                bOk = CalibrateMixer(300, 3, wndProgress);

                                if (bOk)
                                {
                                    wndProgress.textLine3.Text = string.Format("Calibrating - Step 5/{0}...", nTotalSteps);
                                    wndProgress.ProgressBar.Value = 4;
                                    Application.DoEvents();
                                    Cursor.Current = Cursors.WaitCursor;
                                    bOk = CalibrateMixer(112, 10, wndProgress);
                                }
                            }
                        }
                    }

                    if (bOk)
                    {
                        if (m_bExpansionBoardActive)
                        {
                            //Date-time : this is only available on expansion ROM at the moment
                            DateTime CalibrationTime = DateTime.Now;
                            string sCalibrationTime = "CR" + Convert.ToChar(0) + Convert.ToChar(FLASH_FILE_CALIBRATION_DATE) + "CD:" +
                                (CalibrationTime.Year - 2000).ToString("D2") + CalibrationTime.Month.ToString("D2") + CalibrationTime.Day.ToString("D2") +
                                CalibrationTime.Hour.ToString("D2") + CalibrationTime.Minute.ToString("D2") + CalibrationTime.Second.ToString("D2");
                            SendCommand(sCalibrationTime);
                        }

                        Thread.Sleep(1000);
                        SendCommand("CS"); //save calibration settings and reinit config
                        Thread.Sleep(2000);
                        AskConfigData();
                    }
                }
            }
            catch (Exception obEx)
            {
                ReportLog(obEx.Message);
            }
            finally
            {
                m_bCalibrating = false;
                Cursor.Current = Cursors.Default;
                wndProgress.Close();
                wndProgress = null;
                m_timer_receive.Enabled = true;

                m_fStartFrequencyMHZ = fOldStart;
                m_fStepFrequencyMHZ = fOldStep;
                m_fAmplitudeTop = fOldTop;
                m_fAmplitudeBottom = fOldBottom;
                SetupSpectrumAnalyzerAxis();
            }
        }

        private void m_edCalibrationFreq_Leave(object sender, EventArgs e)
        {
            double fExtraMargin=0.0;
            if ((m_eActiveModel != eModel.MODEL_WSUB3G) && (m_eActiveModel != eModel.MODEL_WSUB1G))
            {
                fExtraMargin = 10.0;
            }
            double fCenterFreqMHZ = Convert.ToDouble(m_edCalibrationFreq.Text);
            fCenterFreqMHZ = Math.Max(m_fMinFreqMHZ - fExtraMargin, fCenterFreqMHZ);
            fCenterFreqMHZ = Math.Min(m_fMaxFreqMHZ + fExtraMargin, fCenterFreqMHZ);
            m_edCalibrationFreq.Text = fCenterFreqMHZ.ToString("f3");
        }
    #endregion

        private void btnSaveRemoteVideo_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Sorry, this is still under development\nCheck in upcoming versions.");
        }
    }
}
