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
        const byte m_nFileFormat = 1;       //File format constant
        const int m_nTotalBufferSize = 10240;       //buffer size
        const UInt16 m_nFreqSpectrumSteps = 112;    //$S byte buffer

        const double MIN_AMPLITUDE_DBM = -110.0;
        const double MAX_AMPLITUDE_DBM = -1.0;
        const double MIN_AMPLITUDE_RANGE_DBM = 10;
        
        int m_nDrawingIteration = 0;        //Iteration counter to do regular updates on GUI

        string m_sFilename="";              //Value
        string m_sReportFilePath="";        //Path and name of the report log file

        Boolean m_bPortConnected = false;   //Will be true while COM port is connected, as IsOpen() is not reliable
        float[,] m_arrData;                 //Collection of available spectrum data
        UInt16 m_nDataIndex = 0;            //Index pointing to latest image data received
        UInt16 m_nMaxDataIndex = 0;         //Max value for m_nDataIndex with available data

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

        double m_fAmplitudeTop = -30;       //dBm for top graph limit
        double m_fAmplitudeBottom = MIN_AMPLITUDE_DBM;   //dBm for bottom graph limit

        bool m_bFirstTick = true;           //Used to put some text and guarantee action done once after mainform load
        bool m_bFirstText = true;           //First report text printed

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeGraph()
        {
            this.BackColor = Color.LightYellow;
            tabSpectrumAnalyzer.BackColor = Color.LightYellow;
            tabReport.BackColor = Color.LightYellow;

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

        }

        private void GetNewFilename()
        {
            //New unique filename to store data based on date and time
            m_sFilename = "RFExplorer_Client_Data_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".rfe";
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

        private void MainForm_Load(object sender, EventArgs e)
        {
            m_arrData               = new float[m_nTotalBufferSize, m_nFreqSpectrumSteps]; //up to m_nTotalBufferSize pages of 128 bytes each

            toolStripMemory.Maximum = m_nTotalBufferSize;
            toolStripMemory.Step    = m_nTotalBufferSize / 25;

            numericUpDown.Maximum   = m_nTotalBufferSize;
            numericUpDown.Value     = 0;

            numericIterations.Maximum = m_nTotalBufferSize;
            numericIterations.Value = 10;

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
            };
        }

        private void UpdateButtonStatus()
        {
            btnConnect.Enabled = !m_bPortConnected && (COMPortCombo.Items.Count>0);
            btnDisconnect.Enabled = m_bPortConnected;
            COMPortCombo.Enabled = !m_bPortConnected;
            comboBaudRate.Enabled = !m_bPortConnected;
            btnRescan.Enabled = !m_bPortConnected;

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
                    if (strReceived.Length > 0)
                    {
                        if (strReceived[0] == '$')
                        {
                            if ((strReceived.Length > 1) && (strReceived[1] == 'D'))
                            {
                                if (strReceived.Length >= (2 + 128 * 8 * 2))
                                {
                                    //ignore this string
                                    string sLeftOver = strReceived.Substring(1 + 128 * 8);
                                    strReceived = sLeftOver;
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
                while (m_arrReceivedStrings.Count > 0)
                {
                    string sLine = m_arrReceivedStrings.Dequeue().ToString();

                    if (sLine.Contains("$S") && (m_fStartFrequencyMHZ > 100.0))
                    {
                        if (!m_bHoldMode && m_nDataIndex < m_nTotalBufferSize)
                        {
                            for (int nInd = 0; nInd < m_nFreqSpectrumSteps; nInd++)
                            {
                                byte nVal = Convert.ToByte(sLine[2 + nInd]);
                                float fVal = 0;
                                if (nVal > 53)
                                    fVal = 0.5f * (nVal - 35) - 119.0f;
                                else
                                    fVal = -110.0f; //noise floor

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
                    else if (sLine.Contains("#C1-F:"))
                    {
                        ReportLog("Received configuration from RFExplorer device:" + sLine);
                        double fStart = Convert.ToInt32(sLine.Substring(6, 6)) / 1000.0; //note it comes in KHZ
                        double fStep = Convert.ToInt32(sLine.Substring(13, 5)) / 1000000.0;  //Note it comes in HZ
                        if ((Math.Abs(m_fStartFrequencyMHZ - fStart) >= 0.001) || (Math.Abs(m_fStepFrequencyMHZ - fStep) >= 0.001))
                        {
                            m_fStartFrequencyMHZ = fStart;
                            m_fStepFrequencyMHZ = fStep;
                            m_nDataIndex = 0; //we cannot use previous data for avg, etc when new frequency range is selected
                            ReportLog("New Freq range - buffer cleared.");
                        }
                        m_fAmplitudeTop=Convert.ToInt32(sLine.Substring(19, 4));
                        m_fAmplitudeBottom = Convert.ToInt32(sLine.Substring(24, 4));
                        SetupAxis();
                        SaveProperties();
                    }
                    else
                    {
                        ReportLog(sLine);
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

        private double CalculateEndFrequencyMHZ()
        {
            return m_fStartFrequencyMHZ + m_fStepFrequencyMHZ * (m_nFreqSpectrumSteps - 1);
        }

        private void SetupAxis()
        {
            double fStart = m_fStartFrequencyMHZ;
            double fEnd = CalculateEndFrequencyMHZ();
            double fMajorStep = 1.0;

            objGraph.GraphPane.XAxis.Scale.Min = fStart;
            objGraph.GraphPane.XAxis.Scale.Max = fEnd;

            if ((fEnd - fStart) < 1.0)
                fMajorStep = 0.1;

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

            for (int nInd = 0; nInd < m_nFreqSpectrumSteps; nInd++)
            {
                double fVal = m_arrData[nIndex, nInd];

                double fFreq=m_fStartFrequencyMHZ + m_fStepFrequencyMHZ * nInd;

                double fMax = fVal;
                double fMin = fVal;
                double fValAvg = fVal;

                for (int nIterator = nIndex - nCalculatorMax; nIterator < nIndex; nIterator++)
                {
                    //Calculate average, max and min over Calculator range
                    double fVal2 = m_arrData[nIterator, nInd];

                    if (fVal2 > fMax)
                        fMax = fVal2;
                    if (fVal2 < fMin)
                        fMin = fVal2;

                    fValAvg += fVal2;
                }

                if (m_bDrawRealtime)
                    RTList.Add(fFreq, fVal);
                if (m_bDrawMax)
                    MaxList.Add(fFreq, fMax);
                if (m_bDrawMin)
                    MinList.Add(fFreq, fMin);
                if (m_bDrawAverage)
                {
                    fValAvg = fValAvg / (nCalculatorMax + 1);
                    AvgList.Add(fFreq, fValAvg);
                }
            }

            objGraph.GraphPane.CurveList.Clear();
            if (m_bDrawRealtime)
            {
                LineItem RealtimeLine = objGraph.GraphPane.AddCurve("Realtime", RTList, Color.Blue, SymbolType.None);
                RealtimeLine.Line.Width = 3;
            }
            if (m_bDrawMax)
            {
                LineItem MaxLine = objGraph.GraphPane.AddCurve("Max", MaxList, Color.Red, SymbolType.None);
                MaxLine.Line.Width = 2;
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
                //#C[30]-F:ssssss,eeeeee,tttt,bbbb
                UInt32 nStartKhz = (UInt32)(m_fStartFrequencyMHZ * 1000);
                UInt32 nEndKhz = (UInt32)(CalculateEndFrequencyMHZ() * 1000);
                Int16 nTopDBM = (Int16)(m_fAmplitudeTop);
                Int16 nBottomDBM = (Int16)(m_fAmplitudeBottom);

                string sData = "#\x001EC1-F:" +
                    nStartKhz.ToString("D6") + "," + nEndKhz.ToString("D6") + "," +
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
        }

        private void click_view_mode(object sender, EventArgs e)
        {
            m_bDrawRealtime = realtimeDataToolStripMenuItem.Checked;
            m_bDrawAverage = averagedDataToolStripMenuItem.Checked;
            m_bDrawMax = maxDataToolStripMenuItem.Checked;
            m_bDrawMin = minDataToolStripMenuItem.Checked;
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

        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            saveAsToolStripMenuItem.Enabled = m_nMaxDataIndex > 0;
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

        private void UpdateSettingsFromDialog(object sender, EventArgs e)
        {
            bool bNewAmplitude = false;
            bool bNewFrequency = false;

            double fBottomDBM   = Convert.ToDouble(m_sBottomDBM.Text);
            double fTopDBM      = Convert.ToDouble(m_sTopDBM.Text);
            double fStartFreq   = Convert.ToDouble(m_sStartFreq.Text);
            double fEndFreq     = Convert.ToDouble(m_sEndFreq.Text);
            double fCenterFreq  = Convert.ToDouble(m_sCenterFreq.Text);
            double fFreqSpan    = Convert.ToDouble(m_sFreqSpan.Text);

            if (IsDifferent(fBottomDBM, m_fAmplitudeBottom, 0.1))
            {
                m_fAmplitudeBottom = fBottomDBM;
                bNewAmplitude = true;
            }
            if (IsDifferent(fTopDBM, m_fAmplitudeTop, 1.0))
            {
                m_fAmplitudeTop = fTopDBM;
                bNewAmplitude = true;
            }
            if (IsDifferent(fStartFreq, m_fStartFrequencyMHZ))
            {
                m_fStartFrequencyMHZ=fStartFreq;
                bNewFrequency = true;
            }
            if (IsDifferent(fEndFreq, CalculateEndFrequencyMHZ()))
            {
                m_fStepFrequencyMHZ=(fEndFreq-m_fStartFrequencyMHZ)/(m_nFreqSpectrumSteps-1);
                bNewFrequency = true;
            }
            if (!bNewFrequency)
            {
                //only recalculate center+span if start/end weren't manually changed
                if (IsDifferent(fCenterFreq, CalculateCenterFrequencyMHZ()))
                {
                    m_fStartFrequencyMHZ = fCenterFreq - CalculateFrequencySpanMHZ() / 2.0;
                    bNewFrequency = true;
                }
                if (IsDifferent(fFreqSpan, CalculateFrequencySpanMHZ()))
                {
                    m_fStartFrequencyMHZ = fCenterFreq - fFreqSpan / 2.0;
                    m_fStepFrequencyMHZ = fFreqSpan / (m_nFreqSpectrumSteps - 1);
                    bNewFrequency = true;
                }
            }

            if (bNewAmplitude)
            {
                UpdateYAxis();
                UpdateDialogFromFreqSettings();
            }

            if (bNewFrequency)
            {
                ReportLog("New Freq range - buffer cleared.");
                bNewFrequency = true;
                m_nDataIndex = 0; //we cannot use previous data for avg, etc when new frequency range is selected
                UpdateRemoteConfigData();
            }
        }

        private double CalculateFrequencySpanMHZ()
        {
            return m_fStepFrequencyMHZ * (m_nFreqSpectrumSteps - 1);
        }

        private double CalculateCenterFrequencyMHZ()
        {
            return m_fStartFrequencyMHZ + CalculateFrequencySpanMHZ()/2.0;
        }

        private void tabReport_Enter(object sender, EventArgs e)
        {
            groupCOM.Parent = tabReport;
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
    }
}
