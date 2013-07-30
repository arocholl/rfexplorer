//============================================================================
//RF Explorer for Windows - A Handheld Spectrum Analyzer for everyone!
//Copyright © 2010-13 Ariel Rocholl, www.rf-explorer.com
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
using System.Text;

using System.IO.Ports;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Collections;
using Microsoft.Win32;
using System.Diagnostics;
using System.Security;

namespace RFExplorerCommunicator
{
    /// <summary>
    /// Custom event class to report strings to external listeneres
    /// </summary>
    public class EventReportInfo : EventArgs
    {
        string m_sData = "";
        public EventReportInfo(string sText)
        {
            m_sData = sText;
        }
        public string Data
        {
            get { return m_sData; }
            set { m_sData = value; }
        }
    }

    /// <summary>
    /// Main API class to support all basic low level operations with RF Explorer
    /// </summary>
    public class RFECommunicator
    {
        #region constants
        public const float MIN_AMPLITUDE_DBM = -120.0f;
        public const float MAX_AMPLITUDE_DBM = -1.0f;
        public const double MIN_AMPLITUDE_RANGE_DBM = 10;
        public const UInt16 MAX_SPECTRUM_STEPS = 1024;
        public const double MAX_RAW_SAMPLE = 4356 * 8;     //default value for RAW data sample

        const string m_sRFExplorerFirmwareCertified = "01.11"; //Firmware version of RF Explorer which was tested and certified with this PC Client
        public string FirmwareCertified
        {
            get { return m_sRFExplorerFirmwareCertified; }
        }
        #endregion

        #region Member variables
        UInt16 m_nFreqSpectrumSteps = 112;  //$S byte buffer by default
        public UInt16 FreqSpectrumSteps
        {
            get { return m_nFreqSpectrumSteps; }
            set { m_nFreqSpectrumSteps = value; }
        }

        //All posible RF Explorer values
        public enum eModel
        {
            MODEL_433 = 0,    //0
            MODEL_868,      //1
            MODEL_915,      //2
            MODEL_WSUB1G,   //3
            MODEL_2400,     //4
            MODEL_WSUB3G,   //5
            MODEL_NONE = 0xFF //0xFF
        };

        //used to create readable text together with eModel enum
        string[] arrModels = { "433M", "868M", "915M", "WSUB1G", "2.4G", "WSUB3G" };

        //The RF model installed in main board
        eModel m_eMainBoardModel;
        public eModel MainBoardModel
        {
            get { return m_eMainBoardModel; }
        }

        //The RF model installed in the expansion board
        eModel m_eExpansionBoardModel;
        public eModel ExpansionBoardModel
        {
            get { return m_eExpansionBoardModel; }
        }

        //The model active, regardless being main or expansion board
        eModel m_eActiveModel;
        public eModel ActiveModel
        {
            get { return m_eActiveModel; }
        }

        //True when the expansion board is active, false otherwise
        bool m_bExpansionBoardActive;
        public bool ExpansionBoardActive
        {
            get { return m_bExpansionBoardActive; }
        }

        string m_sRFExplorerFirmware;       //Detected firmware
        public string RFExplorerFirmwareDetected
        {
            get { return m_sRFExplorerFirmware; }
        }
        public bool IsFirmwareSameOrNewer(double fVersionWanted)
        {
            if (m_sRFExplorerFirmware.Length > 0)
            {
                double fDetected = Convert.ToDouble(m_sRFExplorerFirmware);
                if (fDetected >= fVersionWanted)
                    return true;
            }

            return false;
        }

        string m_sModelText;                //Human readable text with current HW/firmware configuration received from device
        public string ModelText
        {
            get { return m_sModelText; }
        }

        string m_sConfigurationText;        //Human readable text with current running configuration received from device
        public string ConfigurationText
        {
            get { return m_sConfigurationText; }
        }

        DateTime m_LastCaptureTime;     //Time where prior capture was received. Used to calculate sweep elapsed time.
        string m_sSweepInfoText;        //Human readable text with time of last capture as well as average sweep time and sweeps / second
        public string SweepInfoText
        {
            get { return m_sSweepInfoText; }
        }

        //The current operational mode
        public enum eMode
        {
            MODE_SPECTRUM_ANALYZER, //0
            MODE_TRANSMITTER,       //1
            MODE_WIFI_ANALYZER,     //2
            MODE_NONE = 0xFF          //0xFF
        };
        eMode m_eMode;
        public eMode Mode
        {
            get { return m_eMode; }
        }

        //Initializer for 433MHz model, will change later based on settings
        double m_fMinSpanMHZ = 0.112;       //Min valid span in MHZ for connected model

        public double MinSpanMHZ
        {
            get { return m_fMinSpanMHZ; }
            set { m_fMinSpanMHZ = value; }
        }
        double m_fMaxSpanMHZ = 100.0;       //Max valid span in MHZ for connected model

        public double MaxSpanMHZ
        {
            get { return m_fMaxSpanMHZ; }
            set { m_fMaxSpanMHZ = value; }
        }
        double m_fMinFreqMHZ = 430.0;       //Min valid frequency in MHZ for connected model

        public double MinFreqMHZ
        {
            get { return m_fMinFreqMHZ; }
            set { m_fMinFreqMHZ = value; }
        }
        double m_fMaxFreqMHZ = 440.0;       //Max valid frequency in MHZ for connected model

        public double MaxFreqMHZ
        {
            get { return m_fMaxFreqMHZ; }
            set { m_fMaxFreqMHZ = value; }
        }

        double m_fPeakValueMHZ = 0.0f;      //Last drawing iteration peak value MHZ read
        public double PeakValueMHZ
        {
            get { return m_fPeakValueMHZ; }
            set { m_fPeakValueMHZ = value; }
        }

        double m_fPeakValueAmp = -120.0f;   //Last drawing iteration peak value dBm read
        public double PeakValueAmp
        {
            get { return m_fPeakValueAmp; }
            set { m_fPeakValueAmp = value; }
        }

        double m_fAmplitudeTop = -30;       //dBm for top graph limit
        public double AmplitudeTop
        {
            get { return m_fAmplitudeTop; }
            set { m_fAmplitudeTop = value; }
        }
        public double AmplitudeTopNormalized
        {
            get { return m_fAmplitudeTop + m_fOffset_dBm; }
            set { m_fAmplitudeTop = value - m_fOffset_dBm; }
        }
        double m_fAmplitudeBottom = MIN_AMPLITUDE_DBM;   //dBm for bottom graph limit
        public double AmplitudeBottom
        {
            get { return m_fAmplitudeBottom; }
            set { m_fAmplitudeBottom = value; }
        }
        public double AmplitudeBottomNormalized
        {
            get { return m_fAmplitudeBottom + m_fOffset_dBm; }
            set { m_fAmplitudeBottom = value - m_fOffset_dBm; }
        }

        bool m_bAcknowledge = false;        //Acknowledge used for checking synchronous messages
        public bool Acknowledged            //Everytime we check the acknowledge, it reset itself to false
        {
            get
            {
                bool bTemp = m_bAcknowledge;
                m_bAcknowledge = false;
                return bTemp;
            }
        }

        /// <summary>
        /// Auto configure is true by default and is used for the communicator to auto request config data to RFE upon port connection
        /// </summary>
        bool m_bAutoConfigure = true;
        public bool AutoConfigure
        {
            get { return m_bAutoConfigure; }
            set { m_bAutoConfigure = value; }
        }

        double m_fRBWKHZ = 0.0;             //RBW in use
        public double RBW_KHZ
        {
            get { return m_fRBWKHZ; }
        }
        float m_fOffset_dBm = 0.0f;         //Manual offset of the amplitude reading

        //Calibration variables
        Byte m_nCalibrationCapSi4x;
        public Byte CalibrationCapSi4x
        {
            get { return m_nCalibrationCapSi4x; }
            set { m_nCalibrationCapSi4x = value; }
        }
        Byte m_nCalibrationCapMixer;
        public Byte CalibrationCapMixer
        {
            get { return m_nCalibrationCapMixer; }
            set { m_nCalibrationCapMixer = value; }
        }

        bool m_bPortConnected = false;   //Will be true while COM port is connected, as IsOpen() is not reliable
        public bool PortConnected
        {
            get { return m_bPortConnected; }
        }

        /// <summary>
        /// The main and only data collection with all the Sweep acumulated data
        /// </summary>
        RFESweepDataCollection m_SweepDataContainer;
        public RFESweepDataCollection SweepData
        {
            get { return m_SweepDataContainer; }
        }

        /// <summary>
        /// The main and only collection of screen data
        /// </summary>
        RFEScreenDataCollection m_ScreenDataContainer;
        public RFEScreenDataCollection ScreenData
        {
            get { return m_ScreenDataContainer; }
        }

        UInt16 m_nScreenIndex = 0;                  //Index pointing to the latest Dump screen received
        /// <summary>
        /// Current remote screen data position
        /// </summary>
        public UInt16 ScreenIndex
        {
            get { return m_nScreenIndex; }
            set
            {
                if (value > m_ScreenDataContainer.Count)
                {
                    m_nScreenIndex = (UInt16)m_ScreenDataContainer.Count;
                }
                else
                {
                    m_nScreenIndex = value;
                }
            }
        }

        bool m_bCaptureRemoteScreen = false;        //True only if we want to capture remote screen data
        public bool CaptureRemoteScreen
        {
            get { return m_bCaptureRemoteScreen; }
            set { m_bCaptureRemoteScreen = value; }
        }

        string[] m_arrConnectedPorts;               //Collection of available COM ports
        string[] m_arrValidCP2101Ports;             //Collection of true CP2102 COM ports
        public string[] ValidCP2101Ports
        {
            get { return m_arrValidCP2101Ports; }
        }

        double m_fStartFrequencyMHZ = 0.0;  //In MHZ
        public double StartFrequencyMHZ
        {
            get { return m_fStartFrequencyMHZ; }
            set { m_fStartFrequencyMHZ = value; }
        }
        double m_fStepFrequencyMHZ = 0.0;   //In MHZ
        public double StepFrequencyMHZ
        {
            get { return m_fStepFrequencyMHZ; }
            set { m_fStepFrequencyMHZ = value; }
        }
        double m_fRefFrequencyMHZ = 0.0;    //Reference frequency used for decoder and other zero span functions
        public double RefFrequencyMHZ
        {
            get { return m_fRefFrequencyMHZ; }
            set { m_fRefFrequencyMHZ = value; }
        }

        Queue m_arrReceivedStrings;         //Queue of strings received from COM port

        Mutex m_ReceivedBytesMutex = new Mutex();
        string m_sAllReceivedBytes = "";         //Debug string for all received bytes record.

        public string AllReceivedBytes
        {
            get
            {
                m_ReceivedBytesMutex.WaitOne();
                string sReturn = m_sAllReceivedBytes;
                m_ReceivedBytesMutex.ReleaseMutex();
                return sReturn;
            }
            set
            {
                m_ReceivedBytesMutex.WaitOne();
                m_sAllReceivedBytes = value;
                m_ReceivedBytesMutex.ReleaseMutex();
            }
        }

        System.Threading.Thread m_ReceiveThread;    //Thread to process received RS232 activity
        volatile bool m_bRunReceiveThread;          //Run thread (true) or temporarily stop it (false)

        bool m_bHoldMode = false;                   //True when HOLD is active

        public bool HoldMode
        {
            get { return m_bHoldMode; }
            set { m_bHoldMode = value; }
        }

        SerialPort m_serialPortObj;                 //serial port object

        volatile bool m_bDebugTraces = false;         //True when the low level detailed debug traces should be included too
        public bool EnableDebugTraces
        {
            get { return m_bDebugTraces; }
            set { m_bDebugTraces = value; }
        }

        public RFEMemoryBlock[] m_arrFLASH = new RFEMemoryBlock[512];
        public RFEMemoryBlock[] m_arrRAM1 = new RFEMemoryBlock[8];
        public RFEMemoryBlock[] m_arrRAM2 = new RFEMemoryBlock[8];

        #endregion

        #region main code
        public RFECommunicator()
        {
            m_LastCaptureTime = new DateTime(2000, 1, 1);

            m_SweepDataContainer = new RFESweepDataCollection(100 * 1024);
            m_ScreenDataContainer = new RFEScreenDataCollection();

            m_nScreenIndex = 0;

            m_arrReceivedStrings = new Queue();

            m_bRunReceiveThread = true;
            ThreadStart threadDelegate = new ThreadStart(this.ReceiveThreadfunc);
            m_ReceiveThread = new Thread(threadDelegate);
            m_ReceiveThread.Start();

            ReportLog("RFECommunicator library started.");

            for (int nInd = 0; nInd < m_arrFLASH.Length; nInd++)
            {
                m_arrFLASH[nInd] = new RFEMemoryBlock();
                m_arrFLASH[nInd].Address = (UInt32)(nInd * RFEMemoryBlock.MAX_BLOCK_SIZE);
            }

            try
            {
                m_serialPortObj = new SerialPort();
            }
            catch (Exception obEx)
            {
                ReportLog("Error in RFECommunicator constructor: " + obEx.ToString());
            }
        }

        public void Close()
        {
            if (m_bRunReceiveThread)
            {
                m_bRunReceiveThread = false;
                Thread.Sleep(1000);
                m_ReceiveThread.Abort();
            }
            ClosePort();
        }

        ~RFECommunicator()
        {
            Close();
        }

        /// <summary>
        /// Calculates the END or STOP frequency of the span, based on Start / Step values.
        /// </summary>
        /// <returns></returns>
        private double CalculateEndFrequencyMHZ()
        {
            return StartFrequencyMHZ + StepFrequencyMHZ * FreqSpectrumSteps;
        }

        private double CalculateFrequencySpanMHZ()
        {
            return StepFrequencyMHZ * FreqSpectrumSteps;
        }

        private double CalculateCenterFrequencyMHZ()
        {
            return StartFrequencyMHZ + CalculateFrequencySpanMHZ() / 2.0;
        }

        public void UpdateDeviceConfig(double fStartMHZ, double fEndMHZ, double fTopDBM, double fBottomDBM)
        {
            if (m_bPortConnected)
            {
                //#[32]C2-F:Sssssss,Eeeeeee,tttt,bbbb
                UInt32 nStartKhz = (UInt32)(fStartMHZ * 1000);
                UInt32 nEndKhz = (UInt32)(fEndMHZ * 1000);
                Int16 nTopDBM = (Int16)(fTopDBM);
                Int16 nBottomDBM = (Int16)(fBottomDBM);

                string sData = "C2-F:" +
                    nStartKhz.ToString("D7") + "," + nEndKhz.ToString("D7") + "," +
                    nTopDBM.ToString("D3") + "," + nBottomDBM.ToString("D3");
                SendCommand(sData);

                Thread.Sleep(500); //wait some time for the unit to process changes, otherwise may get a different command too soon
            }
        }

        /// <summary>
        /// The secondary thread used to get data from USB/RS232 COM port
        /// </summary>
        private void ReceiveThreadfunc()
        {
            while (m_bRunReceiveThread)
            {
                string strReceived = "";
#if SUPPORT_EXPERIMENTAL
                while (m_bPortConnected && !m_bModeAPI)
#else
                while (m_bPortConnected && m_bRunReceiveThread)
#endif
                {
                    string sNewText = "";

                    try
                    {
                        Monitor.Enter(m_serialPortObj);
                        if (m_serialPortObj.IsOpen && m_serialPortObj.BytesToRead > 0)
                        {
                            sNewText = m_serialPortObj.ReadExisting();
                        }
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
                        if (m_bDebugTraces)
                        {
                            //Debug only, do not enable this in production
                            m_ReceivedBytesMutex.WaitOne();
                            m_sAllReceivedBytes += sNewText;
                            m_ReceivedBytesMutex.ReleaseMutex();
                        }
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
                                UInt16 nSize = 2; //account for CR+LF

                                switch (strReceived[2])
                                {
                                    case 'c':
                                        {
                                            //Calibration data
                                            nSize += (byte)(Convert.ToByte(strReceived[3]) + 4);
                                            break;
                                        }
                                    case 'b':
                                        {
                                            //Memory data dump
                                            nSize += (UInt16)((Convert.ToByte(strReceived[4]) + 1) * 16 + 10);
                                            break;
                                        }
                                }
                                //This is standard for all 'C' data dumps
                                if ((nSize > 2) && (strReceived.Length >= nSize))
                                {
                                    string sNewLine = strReceived.Substring(0, nSize);
                                    string sLeftOver = strReceived.Substring(nSize);
                                    strReceived = sLeftOver;
                                    Monitor.Enter(m_arrReceivedStrings);
                                    m_arrReceivedStrings.Enqueue(sNewLine);
                                    Monitor.Exit(m_arrReceivedStrings);
                                }
                            }
                            else if ((strReceived.Length > 1) && (strReceived[1] == 'D'))
                            {
                                //This is dump screen data
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
                            else if ((strReceived.Length > 2) && (strReceived[1] == 'S'))
                            {
                                //Standard spectrum analyzer data
                                ushort nReceivedLength = (byte)strReceived[2];

                                bool bLengthOK = (strReceived.Length >= (3 + nReceivedLength + 2));
                                bool bFullStringOK = false;
                                if (bLengthOK && (strReceived.Substring(3 + nReceivedLength, 2) == "\r\n"))
                                {
                                    bFullStringOK = true;
                                }

                                if (bFullStringOK)
                                {
                                    //So we are here because received the full set of chars expected, and all them are apparently of valid characters
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
                                        m_arrReceivedStrings.Enqueue("Ignored $S of size " + nReceivedLength.ToString() + " expected " + FreqSpectrumSteps.ToString());
                                        Monitor.Exit(m_arrReceivedStrings);
                                    }
                                    strReceived = strReceived.Substring(3 + nReceivedLength + 2);
                                }
                                else if (bLengthOK)
                                {
                                    //so we are here because the string doesn't end with the expected chars, but has the right length. 
                                    //The most likely cause is a truncated string was received, and some chars are from next string, not this one
                                    int nPosNextLine = strReceived.IndexOf("\r\n");
                                    if (nPosNextLine >= 0)
                                    {
                                        strReceived = strReceived.Substring(nPosNextLine + 2);
                                    }
                                }
                            }
                            else if ((strReceived.Length > 3) && (strReceived[1] == 's'))
                            {
                                //Extended API spectrum analyzer data
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
                                        m_arrReceivedStrings.Enqueue("Ignored $S of size " + nReceivedLength.ToString() + " expected " + FreqSpectrumSteps.ToString());
                                        Monitor.Exit(m_arrReceivedStrings);
                                    }
                                    string sLeftOver = strReceived.Substring(4 + nReceivedLength + 2);
                                    strReceived = sLeftOver;
                                }
                            }
                            else if ((strReceived.Length > 10) && (strReceived[1] == 'R'))
                            {
                                //Raw data
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
                                if (m_bDebugTraces)
                                {
                                    Monitor.Enter(m_arrReceivedStrings);
                                    m_arrReceivedStrings.Enqueue("DEBUG partial:" + strReceived);
                                    Monitor.Exit(m_arrReceivedStrings);
                                }
                            }
                        }
                    }
                    Thread.Sleep(10);
                }

                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// Processes all strings received and queued by the ReceiveThreadFunc
        /// </summary>
        /// <param name="bProcessAllEvents">If bProcessAllEvents==false then only one event will be processed, otherwise will do all that are waiting on the queue</param>
        /// <param name="sReceivedString">sReceivedString will have the last processed string from the queue</param>
        /// <returns>Returns true if an event was received requiring redraw</returns>
        int m_nAverageSweepSpeedIterator = 0;
        TimeSpan m_spanAverageSpeedAcumulator;
        double m_fAverageSweepTime = 0.0;
        public bool ProcessReceivedString(bool bProcessAllEvents, out string sReceivedString)
        {
            bool bDraw = false;
            sReceivedString = "";

            try
            {
                do
                {
                    bool bWrongFormat = false;
                    string sLine = "";
                    long nCount = 0;

                    try
                    {

                        Monitor.Enter(m_arrReceivedStrings);
                        nCount = m_arrReceivedStrings.Count;

                        if (nCount == 0)
                            break;

                        sLine = m_arrReceivedStrings.Dequeue().ToString();
                        sReceivedString = sLine;
                    }
                    catch (Exception obEx)
                    {
                        ReportLog("m_arrReceivedStrings processing: " + obEx.ToString());
                    }
                    finally
                    {
                        Monitor.Exit(m_arrReceivedStrings);
                    }

                    if ((sLine.Length > 2) && (sLine.Substring(0, 2) == "$S") && (StartFrequencyMHZ > 10.0))
                    {
                        //new data sweep received, just store it in a new object
                        if (!HoldMode)
                        {
                            RFESweepData objSweep = new RFESweepData((float)StartFrequencyMHZ, (float)StepFrequencyMHZ, FreqSpectrumSteps);
                            for (ushort nInd = 0; nInd < FreqSpectrumSteps; nInd++)
                            {
                                byte nVal = Convert.ToByte(sLine[2 + nInd]);
                                float fVal = nVal / -2.0f;

                                objSweep.SetAmplitudeDBM(nInd, fVal + m_fOffset_dBm);
                            }
                            m_SweepDataContainer.Add(objSweep);
                            bDraw = true;
                            if (m_SweepDataContainer.IsFull())
                            {
                                HoldMode = true;
                                OnUpdateFeedMode(new EventArgs());
                                ReportLog("RAM Buffer is full.");
                            }
                            m_sSweepInfoText = "Captured:" + objSweep.CaptureTime.ToString("yyyy-MM-dd HH:mm:ss\\.fff");
                            if (m_LastCaptureTime.Year == DateTime.Now.Year)
                            {
                                m_nAverageSweepSpeedIterator++;
                                m_spanAverageSpeedAcumulator += (objSweep.CaptureTime - m_LastCaptureTime);
                                if (m_fAverageSweepTime > 0.0)
                                {
                                    m_sSweepInfoText += " - Sweep time: " + m_fAverageSweepTime.ToString("0.000") + " seconds - Avg Sweeps/second: " + (1.0 / m_fAverageSweepTime).ToString("0.0");
                                }
                                if (m_nAverageSweepSpeedIterator >= 10)
                                {
                                    m_fAverageSweepTime = m_spanAverageSpeedAcumulator.TotalSeconds / m_nAverageSweepSpeedIterator;
                                    m_nAverageSweepSpeedIterator = 0;
                                    m_spanAverageSpeedAcumulator = m_LastCaptureTime - m_LastCaptureTime; //set it to zero and start average all over again
                                }
                            }
                            m_LastCaptureTime = objSweep.CaptureTime;

                            OnUpdateData(new EventArgs());
                        }
                    }
#if SUPPORT_EXPERIMENTAL
                    else if ((sLine.Length > 2) && sLine.Substring(0, 2) == "$R")
                    {
                        if (!HoldMode && m_nRAWSnifferIndex < m_nTotalBufferSize)
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
                                HoldMode = true;
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
#endif
                    else if ((sLine.Length > 2) && sLine.Substring(0, 2) == "$D")
                    {
                        if ((CaptureRemoteScreen) && (m_ScreenDataContainer.IsFull() == false))
                        {
                            RFEScreenData objScreen = new RFEScreenData(sLine);
                            objScreen.Model = ActiveModel;
                            m_ScreenDataContainer.Add(objScreen);
                            ScreenIndex = (UInt16)m_ScreenDataContainer.UpperBound;
                            OnUpdateRemoteScreen(new EventArgs());
                        }
                        else
                        {
                            //receiving Dump screen strings but it was disabled, resend now
                            SendCommand("D0");
                        }
                    }
                    else if ((sLine.Length > 5) && sLine.Substring(0, 6) == "#C2-M:")
                    {
                        ReportLog("Received RF Explorer device model info:" + sLine);
                        m_eMainBoardModel = (eModel)Convert.ToUInt16(sLine.Substring(6, 3));
                        m_eExpansionBoardModel = (eModel)Convert.ToUInt16(sLine.Substring(10, 3));
                        m_sRFExplorerFirmware = (sLine.Substring(14, 5));
                        OnReceivedDeviceModel(new EventArgs());
                    }
                    else if ((sLine.Length > 5) && (sLine.Substring(0, 6) == "#C2-F:"))
                    {
                        ReportLog("Received configuration from RFExplorer device:" + sLine);
                        if (sLine.Length >= 60)
                        {
                            double fStartMHZ = Convert.ToInt32(sLine.Substring(6, 7)) / 1000.0; //note it comes in KHZ
                            double fStepMHZ = Convert.ToInt32(sLine.Substring(14, 7)) / 1000000.0;  //Note it comes in HZ
                            if ((Math.Abs(StartFrequencyMHZ - fStartMHZ) >= 0.001) || (Math.Abs(StepFrequencyMHZ - fStepMHZ) >= 0.001))
                            {
                                StartFrequencyMHZ = fStartMHZ;
                                StepFrequencyMHZ = fStepMHZ;
                                ReportLog("New Freq range - buffer cleared.");
                            }
                            AmplitudeTop = Convert.ToInt32(sLine.Substring(22, 4));
                            AmplitudeBottom = Convert.ToInt32(sLine.Substring(27, 4));
                            FreqSpectrumSteps = Convert.ToUInt16(sLine.Substring(32, 4));
                            m_bExpansionBoardActive = (sLine[37] == '1');
                            if (m_bExpansionBoardActive)
                            {
                                m_eActiveModel = m_eExpansionBoardModel;
                                if (ExpansionBoardModel == RFECommunicator.eModel.MODEL_WSUB3G)
                                {
                                    //If it is a MODEL_WSUB3G, make sure we use the MAX HOLD mode to account for proper DSP
                                    Thread.Sleep(500);
                                    ReportLog("Updated remote mode to Max Hold for reliable DSP calculations with fast signals");
                                    SendCommand("C+\x4");
                                }
                            }
                            else
                            {
                                m_eActiveModel = m_eMainBoardModel;
                            }
                            m_eMode = (eMode)Convert.ToUInt16(sLine.Substring(39, 3));

                            MinFreqMHZ = Convert.ToInt32(sLine.Substring(43, 7)) / 1000.0;
                            MaxFreqMHZ = Convert.ToInt32(sLine.Substring(51, 7)) / 1000.0;
                            MaxSpanMHZ = Convert.ToInt32(sLine.Substring(59, 7)) / 1000.0;

                            if (sLine.Length > 66)
                            {
                                m_fRBWKHZ = Convert.ToInt32(sLine.Substring(67, 5));
                            }
                            if (sLine.Length > 72)
                            {
                                m_fOffset_dBm = Convert.ToInt32(sLine.Substring(73, 4));
                            }

                            if (m_eActiveModel == eModel.MODEL_2400)
                            {
                                MinSpanMHZ = 2.0;
                            }
                            else
                            {
                                MinSpanMHZ = 0.112;
                            }

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

                            m_sModelText = "Client v" + Assembly.GetExecutingAssembly().GetName().Version.ToString() + " - Firmware v" + m_sRFExplorerFirmware +
                            " - Model:" + sModel + sExpansion +
                            " - Active range:" + m_fMinFreqMHZ.ToString() + "-" + m_fMaxFreqMHZ.ToString() + "MHz";

                            m_sConfigurationText = "Start: " + m_fStartFrequencyMHZ.ToString("f3") + "MHz - Stop:" + CalculateEndFrequencyMHZ().ToString("f3") +
                                "MHz - Center:" + CalculateCenterFrequencyMHZ().ToString("f3") + "MHz - Span:" + CalculateFrequencySpanMHZ().ToString("f3") +
                                "MHz - Sweep Step:" + (m_fStepFrequencyMHZ * 1000.0).ToString("f0") + "KHz";

                            if (m_fRBWKHZ > 0.0)
                            {
                                m_sConfigurationText += " - RBW:" + m_fRBWKHZ.ToString("f0") + "KHz";
                            }
                            m_sConfigurationText += " - Amp Offset:" + m_fOffset_dBm.ToString("f0") + "dBm";

                            OnReceivedConfigurationData(new EventArgs());
                        }
                        else
                            bWrongFormat = true;
                    }
                    else if ((sLine.Length > 18) && (sLine.Substring(0, 18) == "(C) Ariel Rocholl "))
                    {
                        //RF Explorer device was reset for some reason, reconfigure client based on new configuration
                        OnDeviceReset(new EventArgs());
                    }
                    else if ((sLine.Length > 5) && (sLine.Substring(0, 6) == "#C1-F:"))
                    {
                        bWrongFormat = true;
                    }
                    else if ((sLine.Length > 3) && (sLine.Substring(0, 4) == "#ACK"))
                    {
                        m_bAcknowledge = true;
                    }
                    else
                    {
                        ReportLog(sLine);
                    }
                    if (bWrongFormat)
                    {
                        ReportLog("Received unexpected data from RFExplorer device:" + sLine);
                        ReportLog("Please update your RF Explorer to a recent firmware version and");
                        ReportLog("make sure you are using the latest version of RF Explorer for Windows.");
                        ReportLog("Visit http://www.rf-explorer/download for latest updates.");

                        OnWrongFormatData(new EventArgs());
                    }
                } while (bProcessAllEvents && (m_arrReceivedStrings.Count > 0));
            }
            catch (Exception obEx)
            {
                ReportLog("ProcessReceivedString: " + obEx.Message);
            }

            return bDraw;
        }


        public void SendCommand_RequestConfigData()
        {
            SendCommand("C0");
        }

        public void SendCommand_EnableMainboard()
        {
            SendCommand("CM\x0");
        }

        public void SendCommand_Hold()
        {
            SendCommand("CH");
        }

        public void SendCommand_EnableExpansion()
        {
            SendCommand("CM\x1");
        }

        public void SendCommand_ScreenON()
        {
            SendCommand("L1");
        }

        public void SendCommand_ScreenOFF()
        {
            SendCommand("L0");
        }

        public void SendCommand_DisableScreenDump()
        {
            SendCommand("D0");
        }

        public void SendCommand_EnableScreenDump()
        {
            SendCommand("D1");
        }

        /// <summary>
        /// Format and send command - for instance to reboot just use "r", the '#' decorator and byte length char will be included within
        /// </summary>
        /// <param name="sData">unformatted command from http://code.google.com/p/rfexplorer/wiki/RFExplorerRS232Interface </param>
        public void SendCommand(string sData)
        {
            if (!m_bPortConnected)
                return;

            try
            {
                Monitor.Enter(m_serialPortObj);
                m_serialPortObj.Write("#" + Convert.ToChar(sData.Length + 2) + sData);
            }
            catch (Exception obEx)
            {
                ReportLog("SendCommand error: " + obEx.Message);
            }
            finally
            {
                Monitor.Exit(m_serialPortObj);
            }
            if (m_bDebugTraces)
            {
                string sText = "";
                foreach (char cChar in sData)
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
                }

                ReportLog("DEBUG Sent to RFE: " + "#[0x" + (sData.Length + 2).ToString("X2") + "]" + sText);
            }
        }

        /// <summary>
        /// Raw basic data write to Serial Port - use only if you know what you are doing, otherwise use SendCommand
        /// </summary>
        public void WriteRAW(byte[] arrData, int nSize)
        {
            if (!m_bPortConnected)
                return;

            try
            {
                Monitor.Enter(m_serialPortObj);
                m_serialPortObj.Write(arrData, 0, nSize);
            }
            catch (Exception obEx)
            {
                ReportLog("WriteRAW error: " + obEx.Message);
            }
            finally
            {
                Monitor.Exit(m_serialPortObj);
            }
        }

        /// <summary>
        /// Ask RF Explorer for currently configured data, as well as enable (if it wasn't already) the data dump from RFE -> PC
        /// </summary>
        public void AskConfigData()
        {
            if (m_bPortConnected)
            {
                SendCommand("C0");
            }
        }

        private void ReportLog(string sLine)
        {
            OnReportInfoAdded(new EventReportInfo(sLine));
        }

        public bool SaveFileRFE(string sFilename)
        {
            SweepData.SaveFile(sFilename, m_sModelText, m_sConfigurationText);
            return true;
        }

        public bool LoadFileRFE(string sFilename)
        {
            if (SweepData.LoadFile(sFilename, out m_sModelText, out m_sConfigurationText))
            {
                HoldMode = true;
                double fAmplitudeTop, fAmplitudeBottom;
                SweepData.GetTopBottomDataRange(out fAmplitudeTop, out fAmplitudeBottom);
                AmplitudeBottom = fAmplitudeBottom - 5;
                AmplitudeTop = fAmplitudeTop + 15;
                StartFrequencyMHZ = SweepData.GetData(0).StartFrequencyMHZ;
                StepFrequencyMHZ = SweepData.GetData(0).StepFrequencyMHZ;
                FreqSpectrumSteps = SweepData.GetData(0).TotalSteps;

                if (SweepData.GetData(0).TotalSteps == 13)
                {
                    m_eMode = RFECommunicator.eMode.MODE_WIFI_ANALYZER;
                }
                else
                {
                    m_eMode = RFECommunicator.eMode.MODE_SPECTRUM_ANALYZER;
                }
            }
            else
                return false;

            return true;
        }


        #endregion

        #region COM port low level details
        public bool GetConnectedPorts()
        {
            try
            {
                m_arrConnectedPorts = System.IO.Ports.SerialPort.GetPortNames();

                GetValidCOMPorts();
                if (m_arrValidCP2101Ports != null && m_arrValidCP2101Ports.Length > 0)
                {
                    string sPorts = m_arrValidCP2101Ports.ToString();
                    ReportLog("RF Explorer Valid Ports found: " + sPorts);
                    return true;
                }
                else
                {
                    ReportLog("ERROR: No valid RF Explorer COM ports available\r\nConnect RFExplorer and click on [*]");
                }
            }
            catch (Exception obEx)
            {
                ReportLog("Error scanning COM ports: " + obEx.Message);
            }
            return false;
        }

        public void ConnectPort(string PortName, int nBaudRate)
        {
            try
            {
                Monitor.Enter(m_serialPortObj);

                m_serialPortObj.BaudRate = nBaudRate;
                m_serialPortObj.DataBits = 8;
                m_serialPortObj.StopBits = StopBits.One;
                m_serialPortObj.Parity = Parity.None;
                m_serialPortObj.PortName = PortName;
                m_serialPortObj.ReadTimeout = 100;
                m_serialPortObj.WriteBufferSize = 1024;
                m_serialPortObj.ReadBufferSize = 8192;
                m_serialPortObj.Open();
                m_serialPortObj.Handshake = Handshake.None;
                m_serialPortObj.Encoding = Encoding.GetEncoding(28591); //this is the great trick to use ASCII and binary together

                m_bPortConnected = true;

                m_LastCaptureTime = DateTime.Now;

                HoldMode = false;

                ReportLog("Connected: " + m_serialPortObj.PortName.ToString() + ", " + m_serialPortObj.BaudRate.ToString() + " bauds");

#if SUPPORT_EXPERIMENTAL
                Thread.Sleep(500);
                StopAPIMode(); //stop api mode, if any
#endif
                Thread.Sleep(500);
                if (m_bAutoConfigure)
                {
                    AskConfigData();
                    Thread.Sleep(500);
                }
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

        public void ClosePort()
        {
            try
            {
                Monitor.Enter(m_serialPortObj);

                if (m_serialPortObj.IsOpen)
                {
#if SUPPORT_EXPERIMENTAL
                    StopAPIMode(); //stop api mode, if any
#endif
                    Thread.Sleep(200);
                    SendCommand("L1"); //restore LCD
                    Thread.Sleep(200);
                    SendCommand("CH"); //Switch data dump to off
                    Thread.Sleep(200);
                    //Close the port
                    ReportLog("Disconnected.");
                    m_serialPortObj.Close();
                    OnPortClosed(new EventArgs());
                }
            }
            catch { }
            finally
            {
                Monitor.Exit(m_serialPortObj);
            }
            m_bPortConnected = false;

            m_LastCaptureTime = new DateTime(2000, 1, 1);

            GetConnectedPorts();
        }

        public void ListAllCOMPorts()
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
                catch (SecurityException) { }
                catch (Exception obEx) { ReportLog(obEx.ToString()); };
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

        public void GetValidCOMPorts()
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
                        if (obFriendlyName != null)
                        {
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
                }
                catch (SecurityException) { }
                catch (Exception obEx) { ReportLog(obEx.ToString()); };
            }
            string sTotalPortsFound = "0";
            if (m_arrValidCP2101Ports != null)
                sTotalPortsFound = m_arrValidCP2101Ports.Length.ToString();
            ReportLog("Total ports found: " + sTotalPortsFound);
        }
        #endregion

        #region Events

        public event EventHandler MemoryBlockUpdate;
        private void OnMemoryBlockUpdate(EventReportInfo eventArgs)
        {
            if (MemoryBlockUpdate != null)
            {
                MemoryBlockUpdate(this, eventArgs);
            }
        }

        /// <summary>
        /// Optional
        /// This event will fire everytime there is some information human readable to display        /// 
        /// </summary>
        public event EventHandler ReportInfoAdded;
        private void OnReportInfoAdded(EventReportInfo eventArgs)
        {
            if (ReportInfoAdded != null)
            {
                ReportInfoAdded(this, eventArgs);
            }
        }

        /// <summary>
        /// Optional
        /// This event will fire when a wrong format response is received from RFE
        /// </summary>
        public event EventHandler WrongFormatData;
        private void OnWrongFormatData(EventArgs eventArgs)
        {
            if (WrongFormatData != null)
            {
                WrongFormatData(this, eventArgs);
            }
        }

        /// <summary>
        /// Optional
        /// This event will fire when a string identified as reset is received from RFE
        /// </summary>
        public event EventHandler DeviceReset;
        private void OnDeviceReset(EventArgs eventArgs)
        {
            if (DeviceReset != null)
            {
                DeviceReset(this, eventArgs);
            }
        }

        /// <summary>
        /// Required
        /// This event will fire when RFE is sending its configuration back. This always come before any data dump starts.
        /// </summary>
        public event EventHandler ReceivedConfigurationData;
        private void OnReceivedConfigurationData(EventArgs eventArgs)
        {
            if (ReceivedConfigurationData != null)
            {
                ReceivedConfigurationData(this, eventArgs);
            }
        }

        /// <summary>
        /// Required
        /// This event will fire when RFE is sending global configuration back.
        /// </summary>
        public event EventHandler ReceivedDeviceModel;
        private void OnReceivedDeviceModel(EventArgs eventArgs)
        {
            if (ReceivedDeviceModel != null)
            {
                ReceivedDeviceModel(this, eventArgs);
            }
        }


        /// <summary>
        /// Required
        /// This event indicates new data dump has been received from RF Explorer an is ready to be used
        /// </summary>
        public event EventHandler UpdateData;
        protected virtual void OnUpdateData(EventArgs e)
        {
            if (UpdateData != null)
            {
                UpdateData(this, e);
            }
        }

        /// <summary>
        /// Optional
        /// This event indicates new screen dump bitmap has been received from RF Explorer
        /// </summary>
        public event EventHandler UpdateRemoteScreen;
        protected virtual void OnUpdateRemoteScreen(EventArgs e)
        {
            if (UpdateRemoteScreen != null)
            {
                UpdateRemoteScreen(this, e);
            }
        }

        /// <summary>
        /// This event will fire when the feed mode (real time or hold) has changed
        /// </summary>
        public event EventHandler UpdateFeedMode;
        protected virtual void OnUpdateFeedMode(EventArgs e)
        {
            if (UpdateFeedMode != null)
            {
                UpdateFeedMode(this, e);
            }
        }

        /// <summary>
        /// This event will fire in the event of a communication port is closed, either by manual user intervention or by a link error
        /// </summary>
        public event EventHandler PortClosed;
        protected virtual void OnPortClosed(EventArgs e)
        {
            if (PortClosed != null)
            {
                PortClosed(this, e);
            }
        }
        #endregion
    }
}
