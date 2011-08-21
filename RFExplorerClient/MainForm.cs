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
using System.Threading;
using System.Collections;
using Microsoft.Win32;

namespace RFExplorerClient
{
    public partial class MainForm : Form
    {

        #region Data Members
        const byte m_nFileFormat = 1;               //File format constant
        const int m_nTotalBufferSize = 10240;       //buffer size 

        const double MIN_AMPLITUDE_DBM = -120.0;
        const double MAX_AMPLITUDE_DBM = -1.0;
        const double MIN_AMPLITUDE_RANGE_DBM = 10;

        UInt16 m_nFreqSpectrumSteps = 112;  //$S byte buffer by default
        int m_nDrawingIteration = 0;        //Iteration counter to do regular updates on GUI

        enum eModel
        {
            MODEL_434=0,    //0
            MODEL_868,      //1
            MODEL_915,      //2
            MODEL_WSUB1G,   //3
            MODEL_2400,     //4
            MODEL_NONE=0xFF //0xFF
        };

        eModel m_eMainBoardModel;           //The RF model installed in main board
        eModel m_eExpansionBoardModel;      //The RF model installed in the expansion board
        eModel m_eActiveModel;              //The model active, regardless being main or expansion board
        UInt16 m_eMode;                     //The current operational mode
        bool m_bExpansionBoardActive;       //True when the expansion board is active, false otherwise

        double m_fMinSpanMHZ = 0.112;       //Min valid span in MHZ for connected model
        double m_fMaxSpanMHZ = 100.0;       //Max valid span in MHZ for connected model
        double m_fMinFreqMHZ = 430.0;       //Min valid frequency in MHZ for connected model
        double m_fMaxFreqMHZ = 440.0;       //Max valid frequency in MHZ for connected model

        string m_sFilename="";              //Value
        string m_sReportFilePath="";        //Path and name of the report log file

        Boolean m_bPortConnected = false;   //Will be true while COM port is connected, as IsOpen() is not reliable
        float[,] m_arrData;                 //Collection of available spectrum data
        UInt16 m_nDataIndex = 0;            //Index pointing to latest spectrum data received
        UInt16 m_nMaxDataIndex = 0;         //Max value for m_nDataIndex with available data
        UInt16 m_nScreenIndex = 0;          //Index pointing to the latest Dump screen received
        UInt16 m_nMaxScreenIndex = 0;       //Max value for m_nScreenIndex with available data

        string[] m_arrConnectedPorts;      //Collection of available COM ports
        string[] m_arrValidCP2101Ports;    //Collection of true CP2102 COM ports

        double m_fStartFrequencyMHZ = 0.0;  //In MHZ
        double m_fStepFrequencyMHZ = 0.0;   //In MHZ

        Queue m_arrReceivedStrings;         //Queue of strings received from COM port

        System.Threading.Thread m_ReceiveThread;    //Thread to process received RS232 activity
        Boolean m_bRunReceiveThread;        //Run thread (true) or temporarily stop it (false)

        bool m_bHoldMode = false;           //True when HOLD is active

        bool m_bDrawRealtime = true;        //True if realtime data should be displayed
        bool m_bDrawAverage = true;         //True if averaged data should be displayed
        bool m_bDrawMax = true;             //True if max data should be displayed
        bool m_bDrawMin = true;             //True if min data should be displayed
        bool m_bShowPeaks = true;           //True if peak text with MHZ/dBm should be displayed

        double m_fAmplitudeTop = -30;       //dBm for top graph limit
        double m_fAmplitudeBottom = MIN_AMPLITUDE_DBM;   //dBm for bottom graph limit

        bool m_bFirstTick = true;           //Used to put some text and guarantee action done once after mainform load
        bool m_bFirstText = true;           //First report text printed

        Pen m_PenDarkBlue;                  //Graphis objects cached to reduce drawing overhead
        Pen m_PenRed;
        Brush m_BrushDarkBlue;
        TextObj m_RealtimePeak, m_AveragePeak, m_MaxPeak;

        bool m_bIsWinXP = false;            //True if it is a Windows XP platform, which has some GUI differences with Win7/Vista
        #endregion

        #region Main Window
        public MainForm()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);

            InitializeComponent();
        }

        private void InitializeGraph()
        {
            this.BackColor = Color.LightYellow;
            tabSpectrumAnalyzer.BackColor = Color.LightYellow;
            tabReport.BackColor = Color.LightYellow;
            tabRemoteScreen.BackColor = Color.LightYellow;

            // Get a reference to the GraphPane instance in the ZedGraphControl
            GraphPane myPane = objGraph.GraphPane;

            // Set the titles and axis labels
            myPane.Fill = new Fill(Color.White, Color.LightYellow, 90.0f);
            //myPane.Title.FontSpec.Size = 10;
            //myPane.Title.IsVisible = false;
            myPane.XAxis.Title.Text = "Frequency (MHZ)";
            myPane.XAxis.Scale.MajorStep = 1.0;
            myPane.XAxis.Scale.MinorStep = 0.2;

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

            // Fill the axis background with a gradient
            myPane.Chart.Fill = new Fill(Color.White, Color.LightYellow, 90.0f);
            myPane.Legend.IsHStack = true;
            myPane.Legend.FontSpec.Size = 12;

            // Enable scrollbars if needed
            objGraph.IsAutoScrollRange = true;

            objGraph.IsShowPointValues = true;
            objGraph.PointValueEvent += new ZedGraphControl.PointValueHandler(MyPointValueHandler);

            myPane.YAxis.Title.FontSpec.Size = 13;
            myPane.XAxis.Title.FontSpec.Size = 13;
            myPane.YAxis.Scale.FontSpec.Size = 10;
            myPane.XAxis.Scale.FontSpec.Size = 10;

            m_RealtimePeak = new TextObj("", 0, 0, CoordType.AxisXYScale);
            m_RealtimePeak.Location.AlignH = AlignH.Center;
            m_RealtimePeak.Location.AlignV = AlignV.Bottom;
            m_RealtimePeak.FontSpec.Size = 8;
            m_RealtimePeak.FontSpec.Border.IsVisible = false;
            m_RealtimePeak.FontSpec.FontColor = Color.Blue;
            m_RealtimePeak.FontSpec.StringAlignment = StringAlignment.Center;
            myPane.GraphObjList.Add(m_RealtimePeak);

            m_MaxPeak= new TextObj("", 0, 0, CoordType.AxisXYScale);
            m_MaxPeak.Location.AlignH = AlignH.Center;
            m_MaxPeak.Location.AlignV = AlignV.Bottom;
            m_MaxPeak.FontSpec.Size = 8;
            m_MaxPeak.FontSpec.Border.IsVisible = false;
            m_MaxPeak.FontSpec.FontColor = Color.Red;
            m_MaxPeak.FontSpec.StringAlignment = StringAlignment.Center;
            myPane.GraphObjList.Add(m_MaxPeak);

            m_AveragePeak= new TextObj("", 0, 0, CoordType.AxisXYScale);
            m_AveragePeak.Location.AlignH = AlignH.Center;
            m_AveragePeak.Location.AlignV = AlignV.Bottom;
            m_AveragePeak.FontSpec.Size = 8;
            m_AveragePeak.FontSpec.Border.IsVisible = false;
            m_AveragePeak.FontSpec.FontColor = Color.Brown;
            m_AveragePeak.FontSpec.StringAlignment = StringAlignment.Center;
            myPane.GraphObjList.Add(m_AveragePeak);

        
        }

        private void GetNewFilename()
        {
            //New unique filename to store data based on date and time
            m_sFilename = "RFExplorer_Client_Data_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".rfe";
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            m_arrData               = new float[m_nTotalBufferSize, m_nFreqSpectrumSteps]; //up to m_nTotalBufferSize pages of 128 bytes each
            m_arrRemoteScreenData   = new byte[m_nTotalBufferSize, 128 * 8];
            m_arrRemoteScreenData.Initialize();
            m_nScreenIndex          = 0;
            m_nMaxScreenIndex       = 0;

            toolStripMemory.Maximum = m_nTotalBufferSize;
            toolStripMemory.Step    = m_nTotalBufferSize / 25;

            numericUpDown.Minimum   = 0;
            numericUpDown.Maximum   = m_nTotalBufferSize;
            numericUpDown.Value     = 0;

            numScreenIndex.Minimum  = 0;
            numScreenIndex.Maximum  = m_nTotalBufferSize;
            numScreenIndex.Value    = 0;

            numericIterations.Maximum = m_nTotalBufferSize;
            numericIterations.Value = 10;

            m_PenDarkBlue           = new Pen(Color.DarkBlue, 1);
            m_PenRed                = new Pen(Color.Red, 1);
            m_BrushDarkBlue         = new SolidBrush(Color.DarkBlue);

            m_bIsWinXP = (Environment.OSVersion.Version.Major <= 5);

            InitializeGraph();

            try
            {
                m_serialPortObj             = new SerialPort();

                GetConnectedPorts();

                LoadProperties();
                SetupAxis();

                UpdateButtonStatus();
                m_arrReceivedStrings        = new Queue();
                m_bRunReceiveThread         = true;
                ThreadStart threadDelegate  = new ThreadStart(this.ReceiveThreadfunc);
                m_ReceiveThread             = new Thread(threadDelegate);
                m_ReceiveThread.Start();

                chkHoldMode.Checked = ! chkRunMode.Checked;

                if (m_arrValidCP2101Ports != null && m_arrValidCP2101Ports.Length == 1)
                {
                    ManualConnect();
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

            groupSettings.Enabled = m_bPortConnected;
            chkHoldMode.Enabled = m_bPortConnected;
            chkRunMode.Enabled = m_bPortConnected;

            chkCalcRealtime.Checked = m_bDrawRealtime;
            chkCalcAverage.Checked = m_bDrawAverage;
            chkCalcMax.Checked = m_bDrawMax;
            chkCalcMin.Checked = m_bDrawMin;
        }

        private void ConnectPort(string PortName)
        {
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

                AskConfigData();
            }
            catch (Exception obException) 
            { 
                ReportLog("ERROR ConnectPort: " + obException.Message); 
            }
            finally
            {
                Monitor.Exit(m_serialPortObj);
            }
        }

        private void LoadProperties()
        {
            comboBaudRate.SelectedItem  = RFExplorerClient.Properties.Settings.Default.COMSpeed;
            COMPortCombo.SelectedItem   = RFExplorerClient.Properties.Settings.Default.COMPort;
            saveOnCloseToolStripMenuItem.Checked = RFExplorerClient.Properties.Settings.Default.SaveOnClose;
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
        }

        private void SaveProperties()
        {
            if (COMPortCombo.Items.Count > 0)
            {
                RFExplorerClient.Properties.Settings.Default.COMPort = COMPortCombo.SelectedValue.ToString();
            }

            RFExplorerClient.Properties.Settings.Default.COMSpeed = comboBaudRate.SelectedItem.ToString();
            RFExplorerClient.Properties.Settings.Default.SaveOnClose = saveOnCloseToolStripMenuItem.Checked;
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
            RFExplorerClient.Properties.Settings.Default.Save();
        }

        private void ManualConnect()
        {
            ConnectPort(COMPortCombo.SelectedValue.ToString());

            SaveProperties();
        }

        private void ClosePort()
        {
            m_bPortConnected = false;
            try
            {
                Monitor.Enter(m_serialPortObj);

                if (m_serialPortObj.IsOpen)
                {
                    ReportLog("Disconnected.");
                    m_serialPortObj.Close();
                }
            }
            catch { }
            finally
            {
                Monitor.Exit(m_serialPortObj);
            }
            UpdateButtonStatus();
            GetConnectedPorts();
            UpdateFeedMode();
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
                            if ((strReceived.Length > 2) && (strReceived[1] == 'S') && ((byte)strReceived[2] == m_nFreqSpectrumSteps))
                            {
                                if (strReceived.Length >= (3 + m_nFreqSpectrumSteps + 2))
                                {
                                    string sNewLine = "$S" + strReceived.Substring(3, m_nFreqSpectrumSteps);
                                    string sLeftOver = strReceived.Substring(3 + m_nFreqSpectrumSteps + 2);
                                    strReceived = sLeftOver;
                                    Monitor.Enter(m_arrReceivedStrings);
                                    m_arrReceivedStrings.Enqueue(sNewLine);
                                    Monitor.Exit(m_arrReceivedStrings);
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

        private void timer_receive_Tick(object sender, EventArgs e)
        {
            try
            {
                Monitor.Enter(m_arrReceivedStrings);
                bool bDraw = false;
                bool bWrongFormat = false;
                while (m_arrReceivedStrings.Count > 0)
                {
                    string sLine = m_arrReceivedStrings.Dequeue().ToString();

                    if ((sLine.Substring(0,2)=="$S") && (m_fStartFrequencyMHZ > 100.0))
                    {
                        if (!m_bHoldMode && m_nDataIndex < m_nTotalBufferSize)
                        {
                            for (int nInd = 0; nInd < m_nFreqSpectrumSteps; nInd++)
                            {
                                byte nVal = Convert.ToByte(sLine[2 + nInd]);
                                float fVal = nVal / -2.0f;

                                m_arrData[m_nDataIndex, nInd] = fVal;
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
                            numericUpDown.Value = m_nDataIndex;
                        }
                    }
                    else if ( sLine.Substring(0,2)=="$D" )
                    {
                        if (m_nScreenIndex <= m_nMaxScreenIndex)
                        {
                            //force to draw in a new position
                            m_nScreenIndex = m_nMaxScreenIndex;
                            m_nScreenIndex++;
                        }

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
                    else if (sLine.Substring(0, 6) == "#C2-M:")
                    {
                        ReportLog("Received RFExplorer device model info:" + sLine);
                        m_eMainBoardModel = (eModel)Convert.ToUInt16(sLine.Substring(6, 3));
                        m_eExpansionBoardModel = (eModel)Convert.ToUInt16(sLine.Substring(10, 3));
                    }
                    else if (sLine.Substring(0, 6) == "#C2-F:")
                    {
                        ReportLog("Received configuration from RFExplorer device:" + sLine);
                        if (sLine.Length >= 50)
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
                                m_eActiveModel = m_eExpansionBoardModel;
                            else
                                m_eActiveModel = m_eMainBoardModel;
                            m_eMode = Convert.ToUInt16(sLine.Substring(39, 3));

                            m_fMinFreqMHZ = Convert.ToInt32(sLine.Substring(43, 7)) / 1000.0;
                            m_fMaxFreqMHZ = Convert.ToInt32(sLine.Substring(51, 7)) / 1000.0;
                            m_fMaxSpanMHZ = Convert.ToInt32(sLine.Substring(59, 7)) / 1000.0;

                            if (m_eActiveModel == eModel.MODEL_2400)
                            {
                                m_fMinSpanMHZ = 2.0;
                            }
                            else
                            {
                                m_fMinSpanMHZ = 0.112;
                            }

                            SetupAxis();
                            SaveProperties();
                        }
                        else
                            bWrongFormat = true;
                    }
                    else if (sLine.Substring(0, 6) == "#C1-F:")
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
                        ReportLog("Please update your RF Explorer to a recent firmware version.");
                    }
                }
                if (bDraw)
                    DisplayData();
            }
            catch (Exception obEx)
            {
                ReportLog("timer_receive_Tick: " + obEx.Message);
            }
            finally
            {
                Monitor.Exit(m_arrReceivedStrings);
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
            if (saveOnCloseToolStripMenuItem.Checked && (m_nMaxDataIndex > 0) && (m_sFilename.Length==0))
            {
                GetNewFilename();
                SaveFile(m_sFilename);
            }
            ClosePort();
            SaveProperties();
        }

        private void SetupAxis()
        {
            double fStart = m_fStartFrequencyMHZ;
            double fEnd = CalculateEndFrequencyMHZ()-m_fStepFrequencyMHZ;
            double fMajorStep = 1.0;

            objGraph.GraphPane.XAxis.Scale.Min = fStart;
            objGraph.GraphPane.XAxis.Scale.Max = fEnd;

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

            objGraph.GraphPane.XAxis.Scale.MajorStep = fMajorStep;
            objGraph.GraphPane.XAxis.Scale.MinorStep = fMajorStep/10.0;

            UpdateYAxis();

            UpdateDialogFromFreqSettings();
        }

        private void DisplayData()
        {
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

            objGraph.GraphPane.CurveList.Clear();
            if (m_bDrawRealtime)
            {
                LineItem RealtimeLine = objGraph.GraphPane.AddCurve("Realtime", RTList, Color.Blue, SymbolType.None);
                RealtimeLine.Line.Width = 3;
                if (m_bShowPeaks)
                {
                    double fFreqMark = (m_fStartFrequencyMHZ + m_fStepFrequencyMHZ * fRealtimeMax_Iter);
                    m_RealtimePeak.Text = fFreqMark.ToString("0.000") + "MHZ\n" + fRealtimeMax_Amp.ToString() + "dBm";
                    m_RealtimePeak.Location.X = fFreqMark;
                    m_RealtimePeak.Location.Y = fRealtimeMax_Amp;
                }
            }
            if (m_bDrawMax)
            {
                LineItem MaxLine = objGraph.GraphPane.AddCurve("Max", MaxList, Color.Red, SymbolType.None);
                MaxLine.Line.Width = 2;
                if (m_bShowPeaks)
                {
                    double fFreqMark = (m_fStartFrequencyMHZ + m_fStepFrequencyMHZ * fMaxMax_Iter);
                    m_MaxPeak.Text = fFreqMark.ToString("0.000") + "MHZ\n" + fMaxMax_Amp.ToString() + "dBm";
                    m_MaxPeak.Location.X = fFreqMark;
                    m_MaxPeak.Location.Y = fMaxMax_Amp;
                }
            }
            if (m_bDrawMin)
            {
                LineItem MinLine = objGraph.GraphPane.AddCurve("Min", MinList, Color.DarkGreen, SymbolType.None);
                MinLine.Line.Width = 2;
            }
            if (m_bDrawAverage)
            {
                LineItem AvgLine = objGraph.GraphPane.AddCurve("Avg", AvgList, Color.Brown, SymbolType.None);
                AvgLine.Line.Width = 2;
                if (m_bShowPeaks)
                {
                    double fFreqMark = (m_fStartFrequencyMHZ + m_fStepFrequencyMHZ * fAverageMax_Iter);
                    m_AveragePeak.Text = fFreqMark.ToString("0.000") + "MHZ\n" + fAverageMax_Amp.ToString("0.0") + "dBm";
                    m_AveragePeak.Location.X = fFreqMark;
                    m_AveragePeak.Location.Y = fAverageMax_Amp;
                }
            }

            objGraph.Refresh();
        }

        private string MyPointValueHandler(ZedGraphControl control, GraphPane pane,
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
                    m_serialPortObj.Write(sData);
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
                SendCommand("#\x0004C0");
            }
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

                string sData = "#\x001EC2-F:" +
                    nStartKhz.ToString("D7") + "," + nEndKhz.ToString("D7") + "," +
                    nTopDBM.ToString("D3") + "," + nBottomDBM.ToString("D3");
                SendCommand(sData);
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
                SendCommand("#\0004CH");
            }
            else
            {
                //Not on hold anymore, restore RS232 traffic
                AskConfigData();
            }
            UpdateFeedMode();
        }

        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (m_bHoldMode)
            {
                m_nDataIndex = (UInt16)numericUpDown.Value;
                if (m_nDataIndex > m_nMaxDataIndex)
                {
                    m_nDataIndex = m_nMaxDataIndex;
                    numericUpDown.Value = m_nMaxDataIndex;
                }
                DisplayData();
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

        private void viewToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            realtimeDataToolStripMenuItem.Checked = m_bDrawRealtime;
            averagedDataToolStripMenuItem.Checked = m_bDrawAverage;
            maxDataToolStripMenuItem.Checked = m_bDrawMax;
            minDataToolStripMenuItem.Checked = m_bDrawMin;
            mnuItem_ShowPeak.Checked = m_bShowPeaks;
        }

        private void click_view_mode(object sender, EventArgs e)
        {
            m_bDrawRealtime = realtimeDataToolStripMenuItem.Checked;
            m_bDrawAverage = averagedDataToolStripMenuItem.Checked;
            m_bDrawMax = maxDataToolStripMenuItem.Checked;
            m_bDrawMin = minDataToolStripMenuItem.Checked;
            m_bShowPeaks = mnuItem_ShowPeak.Checked;
            UpdateButtonStatus();
            if (m_bHoldMode)
                DisplayData();
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
                    MySaveFileDialog.FileName = m_sFilename;

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
                    MySaveFileDialog.FileName = m_sFilename.Replace(".rfe",".csv");

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
            saveAsToolStripMenuItem.Enabled = (m_nMaxDataIndex > 0) && (MainTab.SelectedTab == tabSpectrumAnalyzer);
            SaveCSVtoolStripMenuItem.Enabled = (m_nMaxDataIndex > 0) && (MainTab.SelectedTab == tabSpectrumAnalyzer);
            toolStripMenuItemLoad.Enabled = MainTab.SelectedTab == tabSpectrumAnalyzer;

            SaveImagetoolStrip.Enabled = (m_nMaxScreenIndex > 0) && (MainTab.SelectedTab == tabRemoteScreen);
            menu_LoadRFS.Enabled = MainTab.SelectedTab == tabRemoteScreen;
            menu_SaveRFS.Enabled = (m_nMaxScreenIndex > 0) && (MainTab.SelectedTab == tabRemoteScreen);
        }

        private void UpdateFeedMode()
        {
            if (!m_bPortConnected)
            {
                m_bHoldMode = true;
            }

            chkRunMode.Checked = !m_bHoldMode;
            chkHoldMode.Checked = m_bHoldMode;
            if (m_bHoldMode == false)
            {
                m_nDataIndex = m_nMaxDataIndex;
                toolFile.Text = "File: none";
                m_sFilename = "";
            }

            if (m_bHoldMode)
            {
                if (m_sFilename.Length > 0)
                {
                    objGraph.GraphPane.Title.Text = "RF Explorer File data";
                }
                else
                {
                    objGraph.GraphPane.Title.Text = "RF Explorer ON HOLD";
                }
            }
            else
            {
                if (m_bPortConnected)
                {
                    objGraph.GraphPane.Title.Text = "RF Explorer Live data";
                }
                else
                    objGraph.GraphPane.Title.Text = "";
            }
            objGraph.Refresh();
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

        private void toolStripMenuItemLoad_Click(object sender, EventArgs e)
        {
            bool bFileOk = true;

            try
            {
                using (OpenFileDialog MyOpenFileDialog = new OpenFileDialog())
                {
                    MyOpenFileDialog.Filter = "RFExplorer files (*.rfe)|*.rfe|All files (*.*)|*.*";
                    MyOpenFileDialog.FilterIndex = 1;
                    MyOpenFileDialog.RestoreDirectory = false;

                    if (MyOpenFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        using (FileStream myFile = new FileStream(MyOpenFileDialog.FileName, FileMode.Open))
                        {
                            using (BinaryReader binStream = new BinaryReader(myFile))
                            {
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
                                }
                                binStream.Close();

                                if (bFileOk)
                                {
                                    m_nDataIndex = m_nMaxDataIndex;
                                    numericUpDown.Value = m_nMaxDataIndex;

                                    toolFile.Text = "File: " + MyOpenFileDialog.FileName;
                                    m_sFilename = MyOpenFileDialog.FileName;

                                    m_bHoldMode = true;
                                    UpdateFeedMode();

                                    SetupAxis();
                                    DisplayData();
                                }
                            }
                        }
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

            objGraph.GraphPane.YAxis.Scale.Min = m_fAmplitudeBottom;
            objGraph.GraphPane.YAxis.Scale.Max = m_fAmplitudeTop;

            objGraph.GraphPane.YAxis.Scale.MajorStep = 10.0;
            if ((m_fAmplitudeTop - m_fAmplitudeBottom) > 30)
            {
                objGraph.GraphPane.YAxis.Scale.MinorStep = 5.0;
            }
            else
            {
                objGraph.GraphPane.YAxis.Scale.MinorStep = 1.0;
            }

            objGraph.Refresh();
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
        }

        private void toolStripCleanReport_Click(object sender, EventArgs e)
        {
            textBox_message.Text = "Text cleared." + Environment.NewLine;
        }

        private void chkCalcAverage_CheckedChanged(object sender, EventArgs e)
        {
            m_bDrawAverage = chkCalcAverage.Checked;
            if (m_bHoldMode)
                DisplayData();
        }

        private void chkCalcMax_CheckedChanged(object sender, EventArgs e)
        {
            m_bDrawMax = chkCalcMax.Checked;
            if (m_bHoldMode)
                DisplayData();
        }

        private void chkCalcMin_CheckedChanged(object sender, EventArgs e)
        {
            m_bDrawMin = chkCalcMin.Checked;
            if (m_bHoldMode)
                DisplayData();
        }

        private void chkCalcRealtime_CheckedChanged(object sender, EventArgs e)
        {
            m_bDrawRealtime = chkCalcRealtime.Checked;
            if (m_bHoldMode)
                DisplayData();
        }

        private void mnuItem_ShowPeak_CheckedChanged(object sender, EventArgs e)
        {
            m_bShowPeaks = mnuItem_ShowPeak.Checked;
            if (m_bHoldMode)
                DisplayData();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Are you sure to reinitialize data buffer?", "Reinitialize data buffer", MessageBoxButtons.YesNo))
            {
                m_nDataIndex = 0;
                m_nMaxDataIndex = 0;
                numericUpDown.Value = 0;
            }
        }
        #endregion

        #region Remote screen

        byte[,] m_arrRemoteScreenData;

        int m_nRSOrigin_X = 10;
        int m_nRSOrigin_Y = 125;

        LinearGradientBrush m_BrushlinGrBrush;

        private void tabRemoteScreen_UpdateZoomValues()
        {
            int nSize = (int)(numericZoom.Value);

            m_nRSOrigin_X = ((Width - 128 * nSize + 9) / 2) - 20;
            m_nRSOrigin_Y = 125 + ((519 - 64 * nSize + 9) / 2);

            m_BrushlinGrBrush = new LinearGradientBrush(
               new Point(0, m_nRSOrigin_Y - 5),
               new Point(0, m_nRSOrigin_Y - 5 + 64 * nSize + 9),
               Color.White,
               Color.LightBlue);
        }

        private void tabRemoteScreen_Enter(object sender, EventArgs e)
        {
            groupCOM.Parent = tabRemoteScreen;

            //TODO Note: automatic double buffer doesn't work for a tab, we need to create a custom control
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            tabRemoteScreen_UpdateZoomValues();
        }

        private void numScreenIndex_ValueChanged(object sender, EventArgs e)
        {
            m_nScreenIndex = (UInt16)numScreenIndex.Value;
            if (m_nScreenIndex > m_nMaxScreenIndex)
            {
                m_nScreenIndex = m_nMaxScreenIndex;
                numScreenIndex.Value = m_nScreenIndex;
            }
            tabRemoteScreen.Invalidate();
        }

        //Remote screen functions - TODO: refactor it to a separate class
        void DrawData(Graphics objGraphics)
        {
            int nSize = (int)(numericZoom.Value);
            int nGap = 1;
            if (nSize <= 3)
                nGap = 0;
                /*
                 * only for video, too blur for static image
            else
                objGraphics.SmoothingMode = SmoothingMode.AntiAlias;
                 */

            objGraphics.FillRectangle(m_BrushlinGrBrush, m_nRSOrigin_X - 4, m_nRSOrigin_Y - 5, 128 * nSize + 9, 64 * nSize + 9);
            objGraphics.DrawRectangle(m_PenDarkBlue, m_nRSOrigin_X - 4, m_nRSOrigin_Y - 5, 128 * nSize + 9, 64 * nSize + 9);

            for (int nIndY = 0; nIndY < 8; nIndY++)
            {
                for (int nIndX = 0; nIndX < 128; nIndX++)
                {
                    for (byte nBit = 0; nBit < 8; nBit++)
                    {
                        byte nVal = 0x01;
                        nVal = (byte)(nVal << nBit);
                        byte nData = m_arrRemoteScreenData[(UInt16)numScreenIndex.Value, nIndX + 128 * nIndY];
                        nVal = (byte)(nVal & nData);
                        if (nVal != 0)
                            objGraphics.FillRectangle(m_BrushDarkBlue, m_nRSOrigin_X + nIndX * nSize, m_nRSOrigin_Y + (nIndY * 8 + nBit) * nSize, nSize - nGap, nSize - nGap);
                    }
                }
            }
        }

        private void tabRemoteScreen_Paint(object sender, PaintEventArgs e)
        {
            Graphics objGraphics = e.Graphics;
            DrawData(objGraphics);
        }

        private void numericZoom_ValueChanged(object sender, EventArgs e)
        {
            tabRemoteScreen_UpdateZoomValues();
            tabRemoteScreen.Invalidate();
        }

        private void chkDumpScreen_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDumpScreen.Checked)
            {
                SendCommand("#\x0004D1");
            }
            else
            {
                //sent twice to guarantee process in high load condition
                SendCommand("#\x0004D0");
                SendCommand("#\x0004D0");
            }
        }

        private void SavePNG(string sFilename)
        {
            Rectangle rectBounds = new Rectangle(0, 0, Width, Height);
            using (Bitmap objAppBmp = new Bitmap(Width, Height))
            {
                DrawToBitmap(objAppBmp, rectBounds);

                int nSize = (int)(numericZoom.Value);
                using (Bitmap objImage = new Bitmap(128 * nSize + 15, 64 * nSize + 15))
                {
                    int nOriginX = -m_nRSOrigin_X-2;
                    int nOriginY = -m_nRSOrigin_Y - 76;
                    if (!m_bIsWinXP)
                        nOriginY += 4; //Difference in Win7 by trial/error. TODO: We need a better method to adjust this
                    Rectangle rectBounds2 = new Rectangle(nOriginX, nOriginY, Width, Height);
                    Graphics.FromImage(objImage).DrawImage(objAppBmp, rectBounds2);
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
                    MySaveFileDialog.FileName = m_sFilename.Replace(".rfe", ".png");

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
                        MySaveFileDialog.FileName = m_sFilename.Replace(".rfe", ".rfs");

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

        #endregion

        #region Report Window

        private void tabReport_Enter(object sender, EventArgs e)
        {
            groupCOM.Parent = tabReport;
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
        #endregion

    }


}
