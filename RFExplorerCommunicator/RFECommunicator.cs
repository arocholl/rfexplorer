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
    public partial class RFECommunicator : IDisposable
    {
        #region constants
        public const float MIN_AMPLITUDE_DBM = -120.0f;
        public const float MAX_AMPLITUDE_DBM = 50.0f;
        public const double MIN_AMPLITUDE_RANGE_DBM = 10;
        public const UInt16 MAX_SPECTRUM_STEPS = 1024;
        public const double MAX_RAW_SAMPLE = 4356 * 8;     //default value for RAW data sample
        public const double MIN_AMPLITUDE_TRACKING_NORMALIZE = -80.0; //lower than this is considered too low for accurate measurement
        public const UInt16 NORMALIZING_AVG_PASSES = 3;
        public const UInt16 MAX_EMBEDDED_CALIBRATED_DATA_SIZE = 154; //length of max embedded calibration device data
        public const UInt16 POS_EMBEDDED_CALIBRATED_6G = 134; //start position for 6G model

        public const string _RFE_File_Extension = ".rfe";
        public const string _CSV_File_Extension = ".csv";
        public const string _XML_File_Extension = ".xml";
        public const string _SNANORM_File_Extension = ".snanorm";
        public const string _SNA_File_Extension = ".sna";
        public const string _RFS_File_Extension = ".rfs";
        public const string _PNG_File_Extension = ".png";
        public const string _RFL_File_Extension = ".rfl";
        public const string _RFA_File_Extension = ".rfa";
        public const string _S1P_File_Extension = ".s1p";

        private const string _DISCONNECTED = "DISCONNECTED";
        private const string _ACTIVE = "(ACTIVE)";
        private const string _Acknowldedge = "#ACK";
        private const string _ResetString = "(C) Ariel Rocholl ";
        private const string _RFEGEN_FILE_MODEL_Mark = "[*]RFEGen:";

        private const string m_sRFExplorerFirmwareCertified = "01.12"; //Firmware version of RF Explorer which was tested and certified with this PC Client
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

        /// <summary>
        /// Available amplitude units
        /// </summary>
        public enum eAmplitudeUnit
        {
            dBm = 0,
            dBuV,
            Watt
        };

        /// <summary>
        /// All posible RF Explorer model values
        /// </summary>
        public enum eModel
        {
            MODEL_433 = 0,  //0
            MODEL_868,      //1
            MODEL_915,      //2
            MODEL_WSUB1G,   //3
            MODEL_2400,     //4
            MODEL_WSUB3G,   //5
            MODEL_6G,       //6

            MODEL_RFGEN=60, //60
            MODEL_NONE = 0xFF //0xFF
        };

        //used to create readable text together with eModel enum
        static string[] arrModels = null;
        private void InitializeModels()
        {
            arrModels=new string[256];
            for (int nInd = 0; nInd < arrModels.Length; nInd++ )
            {
                arrModels[nInd] = "UNKWN";
            }

            arrModels[(int)eModel.MODEL_433] = "433M";
            arrModels[(int)eModel.MODEL_868]="868M";
            arrModels[(int)eModel.MODEL_915]="915M";
            arrModels[(int)eModel.MODEL_WSUB1G]="WSUB1G";
            arrModels[(int)eModel.MODEL_2400]="2.4G";
            arrModels[(int)eModel.MODEL_WSUB3G]="WSUB3G";
            arrModels[(int)eModel.MODEL_6G]="6G";
            arrModels[(int)eModel.MODEL_RFGEN]="RFE6GEN";
            arrModels[(int)eModel.MODEL_NONE]="NONE";
        }

        /// <summary>
        /// Returns a human readable and normalized identifier text for the model specified in the enum
        /// </summary>
        /// <param name="model">RFExplorer model</param>
        /// <returns>model text identifier such as 433M or WSUB1G</returns>
        public static string GetModelTextFromEnum(eModel model)
        {
            return arrModels[(int)model];
        }

        /// <summary>
        /// Returns model enumerator based on text provided
        /// </summary>
        /// <param name="sText">One of "433M", "868M", "915M", "WSUB1G", "2.4G", "WSUB3G", "6G"</param>
        /// <returns>Return valid model enumerator or will set to MODEL_NONE if not found</returns>
        public eModel GetModelEnumFromText(string sText)
        {
            eModel eReturn = eModel.MODEL_NONE;

            for (int nInd = 0; nInd < arrModels.Length; nInd++)
            {
                if (sText.ToUpper() == arrModels[nInd])
                {
                    eReturn = (eModel)nInd;
                    break;
                }
            }

            return eReturn;
        }

        //offset values read from spectrum analyzer calibration
        float[] m_arrSpectrumAnalyzerEmbeddedCalibrationOffsetDB = null;

        /// <summary>
        /// Returns by entry position, the internal calibration offset data
        /// </summary>
        /// <param name="nPosInd"></param>
        /// <returns></returns>
        public float GetEmbeddedAnalyzerCalibrationOffsetDB(int nPosInd)
        {
            if (m_arrSpectrumAnalyzerEmbeddedCalibrationOffsetDB != null)
                return m_arrSpectrumAnalyzerEmbeddedCalibrationOffsetDB[nPosInd];
            else
                return 0.0f;
        }
        public bool IsAnalyzerEmbeddedCal()
        {
            if (m_arrSpectrumAnalyzerEmbeddedCalibrationOffsetDB == null)
                return false;

            bool bAnyValue = false;
            foreach (float fVal in m_arrSpectrumAnalyzerEmbeddedCalibrationOffsetDB)
            {
                if (Math.Abs(fVal - 0.0) > 0.001f)
                {
                    bAnyValue = true;
                    break;
                }
            }
            return bAnyValue;
        }

        //actual -30dBm adjusted values read from signal generator
        double[] m_arrSignalGeneratorEmbeddedCalibrationActual30DBM = null;
        //note this is shared with RFEGenTest
        UInt32[] m_arrSignalGeneratorCalRanges_KHZ = { 
                    25000, 33000, 41000, 49000, 57000, 65000, 73000, 81000,
                    89000, 97000, 100000, 116000, 132000, 148000, 164000, 180000,
                    196000, 212000, 228000, 244000, 260000, 276000, 292000, 308000,
                    324000, 340000, 356000, 372000, 388000, 404000, 420000, 436000,
                    452000, 468000, 484000, 500000, 532000, 564000, 596000,
                    628000, 660000, 692000, 724000, 756000, 788000, 820000, 852000,
                    884000, 916000, 948000, 980000, 1012000, 1044000, 1076000, 1108000, 
                    1140000, 1172000, 1204000, 1236000, 1268000, 1300000, 1332000, 1364000, 
                    1396000, 1428000, 1460000, 1492000, 1524000, 1556000, 1588000, 1620000, 
                    1652000, 1684000, 1716000, 1748000, 1780000, 1812000, 1844000, 1876000, 
                    1908000, 1940000, 1972000, 2004000, 2036000, 2068000, 2100000, 2132000, 
                    2164000, 2196000, 2228000, 2260000, 2292000, 2324000, 2356000, 2388000, 
                    2420000, 2452000, 2484000, 2516000, 2548000, 2580000, 2612000, 2644000, 
                    2676000, 2708000, 2740000, 2772000, 2804000, 2836000, 2868000, 2900000, 
                    2932000, 2964000, 2996000, 3000000, 3064000, 3128000, 3192000, 3256000, 
                    3320000, 3384000, 3448000, 3512000, 3576000, 3640000, 3704000, 3768000, 
                    3832000, 3896000, 3960000, 4024000, 4088000, 4152000, 4216000, 4280000, 
                    4344000, 4408000, 4472000, 4536000, 4600000, 4664000, 4728000, 4792000, 
                    4856000, 4920000, 4984000, 5048000, 5112000, 5176000, 5240000, 5304000, 
                    5368000, 5432000, 5496000, 5560000, 5624000, 5688000, 5752000, 5816000, 
                    5880000, 5944000 };

        /// <summary>
        /// Returns best matching amplitude value based on internal -30dBm calibration table, and configured power switch/attenuator
        /// If not available, this returns the estimated value based on hardcoded ideal amplitude
        /// </summary>
        /// <param name="dFrequencyMHZ"></param>
        public double GetSignalGeneratorEstimatedAmplitude(double dFrequencyMHZ)
        {
            double dValueDBM=0;
            double dValue30DBM = -30;

            if (m_arrSignalGeneratorEmbeddedCalibrationActual30DBM != null)
            {
                //search by brute force, if this is considered too slow, can be replace by binary search or something else such a hash
                for (int nInd = m_arrSignalGeneratorCalRanges_KHZ.Length-1; nInd > 0; nInd--)
                {
                    double dStartFreqMHZ = m_arrSignalGeneratorCalRanges_KHZ[nInd] / 1000.0;
                    if (dStartFreqMHZ < dFrequencyMHZ)
                    {
                        dValue30DBM = m_arrSignalGeneratorEmbeddedCalibrationActual30DBM[nInd];
                        break;
                    }
                }
            }

            if (m_bRFGenHighPowerSwitch)
            {
                switch (m_nRFGenPowerLevel)
                {
                    case 3: dValueDBM = dValue30DBM + 30; break;
                    case 2: dValueDBM = dValue30DBM + 27; break;
                    case 1: dValueDBM = dValue30DBM + 24; break;
                    case 0: dValueDBM = dValue30DBM + 21; break;
                }
            }
            else
            {
                switch (m_nRFGenPowerLevel)
                {
                    case 3: dValueDBM = dValue30DBM; break; //dValue30DBM is correct already
                    case 2: dValueDBM = dValue30DBM - 3; break;
                    case 1: dValueDBM = dValue30DBM - 6; break;
                    case 0: dValueDBM = dValue30DBM - 9; break;
                }
            }

            return dValueDBM;
        }

        string m_sSerialNumber="";
        public string SerialNumber
        {
            get
            {
                if (!String.IsNullOrEmpty(m_sSerialNumber))
                {
                    return m_sSerialNumber.Substring(0, 4) + "-" + m_sSerialNumber.Substring(4, 4) + "-" +
                        m_sSerialNumber.Substring(8, 4) + "-" + m_sSerialNumber.Substring(12, 4);
                }
                else
                    return "";
            }
        }

        //if available, points to a RF Explorer Signal Generator to be used for tracking. It is only meaningful and used when the current object is a spectrum analyzer
        private RFECommunicator m_objRFEGen;
        /// <summary>
        /// Connected tracking generator linked to the current spectrum analyzer object.
        /// </summary>
        public RFECommunicator TrackingRFEGen
        {
            get { return m_objRFEGen; }
            set { m_objRFEGen = value; }
        }

        bool m_bUseMaxHold = true;
        public bool UseMaxHold
        {
            get { return m_bUseMaxHold; }
            set
            {
                if (value != m_bUseMaxHold)
                {
                    if (value)
                    {
                        SendCommand_SetMaxHold();
                    }
                    else
                    {
                        SendCommand_Realtime();
                    }
                }
                m_bUseMaxHold = value;
            }
        }

        /// <summary>
        /// Returns the dBuV value assuming 50ohm
        /// </summary>
        /// <param name="dBm"></param>
        /// <returns></returns>
        public static double Convert_dBm_2_dBuV(double dBm)
        {
            return (dBm + 107.0f);
        }

        public static double Convert_dBuV_2_dBm(double dBuV)
        {
            return (dBuV - 107.0f);
        }

        public static double Convert_dBm_2_mW(double dBm)
        {
            return (Math.Pow(10, dBm / 10.0));
        }

        public static double Convert_dBm_2_Watt(double dBm)
        {
            return (Convert_dBm_2_mW(dBm) / 1000.0f);
        }

        public static double Convert_mW_2_dBm(double mW)
        {
            return (10.0f * Math.Log10(mW));
        }

        public static double Convert_Watt_2_dBm(double Watt)
        {
            return (30.0f + Convert_mW_2_dBm(Watt));
        }

        /// <summary>
        /// Will convert from eFrom amplitude unit to eTo amplitude unit
        /// </summary>
        /// <param name="eFrom"></param>
        /// <param name="dFromAmplitude">amplitude value to convert from, in eFrom units</param>
        /// <param name="eTo"></param>
        /// <returns>amplitude value in eTo units</returns>
        public static double ConvertAmplitude(eAmplitudeUnit eFrom, double dFromAmplitude, eAmplitudeUnit eTo)
        {
            if (eTo == eFrom)
                return dFromAmplitude;

            if (eFrom == eAmplitudeUnit.dBm)
            {
                if (eTo == eAmplitudeUnit.dBuV)
                    return Convert_dBm_2_dBuV(dFromAmplitude);
                else
                    return Convert_dBm_2_Watt(dFromAmplitude);
            }
            else if (eFrom == eAmplitudeUnit.dBuV)
            {
                if (eTo == eAmplitudeUnit.dBm)
                    return Convert_dBuV_2_dBm(dFromAmplitude);
                else
                    return Convert_dBm_2_Watt(Convert_dBuV_2_dBm(dFromAmplitude));
            }
            else
            {
                if (eTo == eAmplitudeUnit.dBm)
                    return Convert_Watt_2_dBm(dFromAmplitude);
                else
                    return Convert_dBm_2_dBuV(Convert_Watt_2_dBm(dFromAmplitude));
            }
        }

        //The RF model installed in main board
        eModel m_eMainBoardModel = eModel.MODEL_NONE;
        public eModel MainBoardModel
        {
            get { return m_eMainBoardModel; }
        }

        //The RF model installed in the expansion board
        eModel m_eExpansionBoardModel = eModel.MODEL_NONE;
        public eModel ExpansionBoardModel
        {
            get { return m_eExpansionBoardModel; }
        }

        //The model active, regardless being main or expansion board
        eModel m_eActiveModel=eModel.MODEL_NONE;
        public eModel ActiveModel
        {
            get { return m_eActiveModel; }
        }

        //True when the expansion board is active, false otherwise
        bool m_bExpansionBoardActive = false;
        public bool ExpansionBoardActive
        {
            get { return m_bExpansionBoardActive; }
        }

        string m_sRFExplorerFirmware;       //Detected firmware
        public string RFExplorerFirmwareDetected
        {
            get 
            {
                if (String.IsNullOrEmpty(m_sRFExplorerFirmware))
                    return "N/A";
                else
                    return m_sRFExplorerFirmware; 
            }
        }
        public bool IsFirmwareSameOrNewer(double fVersionWanted)
        {
            if (!String.IsNullOrEmpty(m_sRFExplorerFirmware))
            {
                double fDetected = Convert.ToDouble(m_sRFExplorerFirmware);
                if (fDetected >= fVersionWanted)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Human readable text with current HW/firmware configuration received from device or file
        /// </summary>
        public string FullModelText
        {
            get 
            {
                string sModelText = _DISCONNECTED;
                if (PortConnected || (SweepData.Count > 0))
                {
                    string sModel = arrModels[(int)m_eMainBoardModel];
                    if (m_eActiveModel != m_eExpansionBoardModel)
                        sModel += _ACTIVE;
                    string sExpansion;
                    if (m_eExpansionBoardModel == eModel.MODEL_NONE)
                        sExpansion = " - No Expansion Module found";
                    else
                    {
                        sExpansion = " - Expansion Module:" + arrModels[(int)m_eExpansionBoardModel];
                        if (m_eActiveModel == m_eExpansionBoardModel)
                            sExpansion += _ACTIVE;
                    }

                    sModelText = "Client v" + Assembly.GetExecutingAssembly().GetName().Version.ToString() + " - Firmware v" + m_sRFExplorerFirmware +
                    " - Model:" + sModel + sExpansion +
                    " - Active range:" + m_fMinFreqMHZ.ToString() + "-" + m_fMaxFreqMHZ.ToString() + "MHz";
                }
                return sModelText; 
            }
        }

        /// <summary>
        /// Human readable text with current running configuration received from device or file
        /// </summary>
        public string ConfigurationText
        {
            get
            {
                string sConfiguration = _DISCONNECTED;
                if (IsAnalyzer())
                {
                    if (PortConnected || (SweepData.Count > 0))
                    {
                        sConfiguration = "Start: " + m_fStartFrequencyMHZ.ToString("f3") + "MHz - Stop:" + CalculateEndFrequencyMHZ().ToString("f3") +
                            "MHz - Center:" + CalculateCenterFrequencyMHZ().ToString("f3") + "MHz - Span:" + CalculateFrequencySpanMHZ().ToString("f3") +
                            "MHz - Sweep Step:" + (m_fStepFrequencyMHZ * 1000.0).ToString("f0") + "KHz";

                        if (m_fRBWKHZ > 0.0)
                        {
                            sConfiguration += " - RBW:" + m_fRBWKHZ.ToString("f0") + "KHz";
                        }
                        sConfiguration += " - Amp Offset:" + m_fOffset_dB.ToString("f0") + "dBm";
                    }
                }
                else
                {
                    if (PortConnected)
                    {
                        sConfiguration = "CW: " + m_fRFGenCWFrequencyMHZ.ToString("f3") + "MHz - Start:" + m_fRFGenStartFrequencyMHZ.ToString("f3") + "MHz - Stop:" +
                            m_fRFGenStopFrequencyMHZ.ToString("f3") + "MHz - Step:" + m_fRFGenStepFrequencyMHZ.ToString("f3") + "MHz - PowerLevel:" +
                            m_nRFGenPowerLevel.ToString("D3") + " - HighPowerSwitch:" + m_bRFGenHighPowerSwitch.ToString() + " - SweepSteps:" + 
                            m_nRFGenSweepSteps.ToString("D4") + " - StepWait:" + m_nRFGenStepWaitMS.ToString("D5") + "ms";
                    }
                }
                
                return sConfiguration; 
            }
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
            MODE_SPECTRUM_ANALYZER = 0,
            MODE_TRANSMITTER = 1,
            MODE_WIFI_ANALYZER = 2,
            MODE_TRACKING = 5,
            MODE_NONE = 0xFF
        };
        eMode m_eMode=eMode.MODE_SPECTRUM_ANALYZER;
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
        public double PeakValueAmplitudeDBM
        {
            get { return m_fPeakValueAmp; }
            set { m_fPeakValueAmp = value; }
        }

        double m_fAmplitudeTopDBM = -30;       //dBm for top graph limit
        /// <summary>
        /// This is the highest value that should be selected for display, includes Offset dBm
        /// </summary>
        public double AmplitudeTopDBM
        {
            get { return m_fAmplitudeTopDBM; }
            set { m_fAmplitudeTopDBM = value; }
        }

        /// <summary>
        /// AmplitudeTop property includes the offset dBm, the normalized one does not
        /// </summary>
        public double AmplitudeTopNormalizedDBM
        {
            get { return m_fAmplitudeTopDBM - m_fOffset_dB; }
            set { m_fAmplitudeTopDBM = value + m_fOffset_dB; }
        }
        double m_fAmplitudeBottomDBM = MIN_AMPLITUDE_DBM;   //dBm for bottom graph limit
        /// <summary>
        /// This is the lowest value that should be selected for display, includes Offset dBm
        /// </summary>
        public double AmplitudeBottomDBM
        {
            get { return m_fAmplitudeBottomDBM; }
            set { m_fAmplitudeBottomDBM = value; }
        }
        /// <summary>
        /// AmplitudeBottom property includes the offset dBm, the normalized one does not
        /// </summary>
        public double AmplitudeBottomNormalizedDBM
        {
            get { return m_fAmplitudeBottomDBM - m_fOffset_dB; }
            set { m_fAmplitudeBottomDBM = value + m_fOffset_dB; }
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
        
        float m_fOffset_dB = 0.0f;         //Manual offset of the amplitude reading
        public float AmplitudeOffsetDB
        {
            get { return m_fOffset_dB; }
            set { m_fOffset_dB = value; }
        }

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
        /// The main data collection for all Tracking mode acumulated data (except normalized response)
        /// </summary>
        RFESweepDataCollection m_TrackingDataContainer;
        public RFESweepDataCollection TrackingData
        {
            get { return m_TrackingDataContainer; }
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

        bool m_bCaptureRemoteScreen = false;
        //True only if we want to capture remote screen data
        public bool CaptureRemoteScreen
        {
            get { return (m_bCaptureRemoteScreen && !m_bHoldMode); }
            set { m_bCaptureRemoteScreen = value; }
        }

        string[] m_arrConnectedPorts;               //Collection of available COM ports
        string[] m_arrValidCP2101Ports;             //Collection of true CP2102 COM ports
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public string[] ValidCP2101Ports
        {
            get { return m_arrValidCP2101Ports; }
        }

        //spectrum analyzer configuration
        double m_fStartFrequencyMHZ = 0.0;
        public double StartFrequencyMHZ
        {
            get { return m_fStartFrequencyMHZ; }
            set { m_fStartFrequencyMHZ = value; }
        }
        double m_fStepFrequencyMHZ = 0.0;
        public double StepFrequencyMHZ
        {
            get { return m_fStepFrequencyMHZ; }
            set { m_fStepFrequencyMHZ = value; }
        }
        public double StopFrequencyMHZ
        {
            get { return m_fStartFrequencyMHZ + m_fStepFrequencyMHZ * FreqSpectrumSteps; }
        }
        double m_fRefFrequencyMHZ = 0.0;    //Reference frequency used for decoder and other zero span functions
        public double RefFrequencyMHZ
        {
            get { return m_fRefFrequencyMHZ; }
            set { m_fRefFrequencyMHZ = value; }
        }

        //Signal generator configuration
        double m_fRFGenStartFrequencyMHZ = 0.0;
        public double RFGenStartFrequencyMHZ
        {
            get { return m_fRFGenStartFrequencyMHZ; }
            set { m_fRFGenStartFrequencyMHZ = value; }
        }
        double m_fRFGenCWFrequencyMHZ = 0.0;
        public double RFGenCWFrequencyMHZ
        {
            get { return m_fRFGenCWFrequencyMHZ; }
            set { m_fRFGenCWFrequencyMHZ = value; }
        }
        double m_fRFGenStepFrequencyMHZ = 0.0;
        public double RFGenStepFrequencyMHZ
        {
            get { return m_fRFGenStepFrequencyMHZ; }
            set { m_fRFGenStepFrequencyMHZ = value; }
        }
        double m_fRFGenStopFrequencyMHZ = 0.0;
        public double RFGenStopFrequencyMHZ
        {
            get { return m_fRFGenStopFrequencyMHZ; }
            set { m_fRFGenStopFrequencyMHZ = value; }
        }
        UInt16 m_nRFGenSweepSteps = 2;
        public UInt16 RFGenSweepSteps
        {
            get { return m_nRFGenSweepSteps; }
            set { m_nRFGenSweepSteps = value; }
        }
        UInt16 m_nRFGenStepWaitMS = 0;
        public UInt16 RFGenStepWaitMS
        {
            get { return m_nRFGenStepWaitMS; }
            set { m_nRFGenStepWaitMS = value; }
        }
        bool m_bRFGenHighPowerSwitch = false;
        public bool RFGenHighPowerSwitch
        {
            get { return m_bRFGenHighPowerSwitch; }
            set { m_bRFGenHighPowerSwitch = value; }
        }
        byte m_nRFGenPowerLevel = 0;
        public byte RFGenPowerLevel
        {
            get { return m_nRFGenPowerLevel; }
            set { m_nRFGenPowerLevel = value; }
        }
        bool m_bRFGenPowerON = false;
        public bool RFGenPowerON
        {
            get { return m_bRFGenPowerON; }
            set { m_bRFGenPowerON = value; }
        }

        Queue m_arrReceivedData;         //Queue of strings received from COM port

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

        /// <summary>
        /// Standard Dispose resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        bool m_bDisposed = false;
        /// <summary>
        /// Local dispose method
        /// </summary>
        /// <param name="bDisposing">if disposing is required</param>
        protected virtual void Dispose(bool bDisposing)
        {
            Close();

            if (!m_bDisposed)
            {
                if (bDisposing)
                {
                    if (m_serialPortObj != null)
                    {
                        if (m_serialPortObj.IsOpen)
                        {
                            m_serialPortObj.Close();
                        }
                        m_serialPortObj.Dispose();
                        m_serialPortObj = null;
                    }
                    if (m_ReceivedBytesMutex != null)
                    {
                        m_ReceivedBytesMutex.Dispose();
                        m_ReceivedBytesMutex = null;
                    }
                }
                m_bDisposed = true;
            }
        }

        public RFECommunicator()
        {
            InitializeModels();

            m_LastCaptureTime = new DateTime(2000, 1, 1);

            m_SweepDataContainer = new RFESweepDataCollection(100 * 1024, true);
            m_TrackingDataContainer = new RFESweepDataCollection(1024, true);
            m_ScreenDataContainer = new RFEScreenDataCollection();

            m_nScreenIndex = 0;

            m_arrReceivedData = new Queue();

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
            Dispose(false);
        }

        /// <summary>
        /// will return true if the last chars of the file name are same as file extension, regardless capitals
        /// </summary>
        /// <param name="sFilename"></param>
        /// <param name="sFileExtension"></param>
        /// <returns></returns>
        public static bool IsFileExtensionType(string sFilename, string sFileExtension)
        {
            return (sFilename.EndsWith(sFileExtension, StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>
        /// Calculates the END or STOP frequency of the span, based on Start / Step values.
        /// </summary>
        /// <returns></returns>
        public double CalculateEndFrequencyMHZ()
        {
            return StartFrequencyMHZ + StepFrequencyMHZ * FreqSpectrumSteps;
        }

        public double CalculateFrequencySpanMHZ()
        {
            return StepFrequencyMHZ * FreqSpectrumSteps;
        }

        public double CalculateCenterFrequencyMHZ()
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
            //this is the object used to keep current configuration data
            RFEConfiguration objCurrentConfiguration = null;
            RFESweepData objSweepTracking = null;
            m_bThreadTrackingEnabled = false;
            int nTrackingStepRetry = 0;

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
                        Monitor.Enter(m_arrReceivedData);
                        m_arrReceivedData.Enqueue(obExeption);
                        Monitor.Exit(m_arrReceivedData);
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
                    if (strReceived.Length > 25240)
                    {
                        //Safety code, some error prevented the string from being processed in several loop cycles. Reset it.
                        if (m_bDebugTraces)
                        {
                            Monitor.Enter(m_arrReceivedData);
                            m_arrReceivedData.Enqueue("Received string truncated (" + strReceived.Length + ")");
                            Monitor.Exit(m_arrReceivedData);
                        }
                        strReceived = "";
                    }
                    if (strReceived.Length > 0)
                    {
                        if (strReceived[0] == '#')
                        {
                            int nEndPos = strReceived.IndexOf("\r\n");
                            if (nEndPos >= 0)
                            {
                                string sNewLine = strReceived.Substring(0, nEndPos);
                                string sLeftOver = strReceived.Substring(nEndPos + 2);
                                strReceived = sLeftOver;

                                RFEConfiguration objNewConfiguration = null;
                                if ((sNewLine.Length > 2) && (sNewLine.Substring(0, 3) == "#K1"))
                                {
                                    if (m_bThreadTrackingEnabled==false)
                                    {
                                        //if we are starting tracking here, send the request for first step right away
                                        m_objRFEGen.SendCommand_TrackingStep(0);
                                        SendCommand_TrackingStep(0); 
                                    }
                                    m_bThreadTrackingEnabled=true;
                                    nTrackingStepRetry = 0;

                                    Monitor.Enter(m_arrReceivedData);
                                    m_arrReceivedData.Enqueue(sNewLine);
                                    Monitor.Exit(m_arrReceivedData);
                                }
                                if ((sNewLine.Length > 2) && (sNewLine.Substring(0, 3) == "#K0"))
                                {
                                    m_bThreadTrackingEnabled = false;

                                    Monitor.Enter(m_arrReceivedData);
                                    m_arrReceivedData.Enqueue(sNewLine);
                                    Monitor.Exit(m_arrReceivedData);
                                }
                                else if ((sNewLine.Length > 5) && ((sNewLine.Substring(0, 6) == "#C2-F:") || (sNewLine.Substring(0, 6) == "#C3-G:")))
                                {
                                    m_bThreadTrackingEnabled=false;

                                    if (m_bDebugTraces)
                                    {
                                        Monitor.Enter(m_arrReceivedData);
                                        m_arrReceivedData.Enqueue("Received Config:" + strReceived.Length.ToString("D5"));
                                        Monitor.Exit(m_arrReceivedData);
                                    }

                                    //Standard configuration expected
                                    objNewConfiguration = new RFEConfiguration();
                                    if (objNewConfiguration.ProcessReceivedString(sNewLine))
                                    {
                                        objCurrentConfiguration = new RFEConfiguration(objNewConfiguration);
                                        Monitor.Enter(m_arrReceivedData);
                                        m_arrReceivedData.Enqueue(objNewConfiguration);
                                        Monitor.Exit(m_arrReceivedData);
                                    }
                                }
                                else
                                {
                                    Monitor.Enter(m_arrReceivedData);
                                    m_arrReceivedData.Enqueue(sNewLine);
                                    Monitor.Exit(m_arrReceivedData);
                                }
                            }
                        }
                        else if (strReceived[0] == '$')
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
                                    Monitor.Enter(m_arrReceivedData);
                                    m_arrReceivedData.Enqueue(sNewLine);
                                    Monitor.Exit(m_arrReceivedData);
                                }
                            }
                            else if ((strReceived.Length > 1) && (strReceived[1] == 'q'))
                            {
                                //this is internal calibration data dump
                                ushort nReceivedLength = (byte)strReceived[2];
                                bool bLengthOK = (strReceived.Length >= (3 + nReceivedLength + 2));
                                if (bLengthOK)
                                {
                                    Monitor.Enter(m_arrReceivedData);
                                    m_arrReceivedData.Enqueue(strReceived);
                                    Monitor.Exit(m_arrReceivedData);
                                    strReceived = strReceived.Substring(3 + nReceivedLength + 2);
                                }
                            }
                            else if ((strReceived.Length > 1) && (strReceived[1] == 'D'))
                            {
                                //This is dump screen data
                                if (m_bDebugTraces)
                                {
                                    Monitor.Enter(m_arrReceivedData);
                                    m_arrReceivedData.Enqueue("Received $D" + strReceived.Length.ToString("D5"));
                                    Monitor.Exit(m_arrReceivedData);
                                }

                                if (strReceived.Length >= (4 + 128 * 8))
                                {
                                    string sNewLine = "$D" + strReceived.Substring(2, 128 * 8);
                                    string sLeftOver = strReceived.Substring(4 + 128 * 8);
                                    strReceived = sLeftOver;
                                    RFEScreenData objData = new RFEScreenData();
                                    if (objData.ProcessReceivedString(sNewLine))
                                    {
                                        Monitor.Enter(m_arrReceivedData);
                                        m_arrReceivedData.Enqueue(objData);
                                        Monitor.Exit(m_arrReceivedData);
                                    }
                                    else
                                    {
                                        Monitor.Enter(m_arrReceivedData);
                                        m_arrReceivedData.Enqueue(sNewLine);
                                        Monitor.Exit(m_arrReceivedData);
                                    }
                                }
                            }
                            else if ((strReceived.Length > 2) && (strReceived[1] == 'S'))
                            {
                                //Standard spectrum analyzer data
                                ushort nReceivedLength = (byte)strReceived[2];

                                if (m_bDebugTraces)
                                {
                                    Monitor.Enter(m_arrReceivedData);
                                    m_arrReceivedData.Enqueue("Received $S" + nReceivedLength.ToString("D5"));
                                    Monitor.Exit(m_arrReceivedData);
                                }

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
                                        if (objCurrentConfiguration != null)
                                        {
                                            UInt16 nSweepSteps = objCurrentConfiguration.nFreqSpectrumSteps;
                                            if (m_bThreadTrackingEnabled)
                                            {
                                                nSweepSteps = nReceivedLength;
                                            }

                                            RFESweepData objSweep = new RFESweepData(objCurrentConfiguration.fStartMHZ, objCurrentConfiguration.fStepMHZ, nSweepSteps);
                                            if (objSweep.ProcessReceivedString(sNewLine, objCurrentConfiguration.fOffset_dB))
                                            {
                                                if (!m_bThreadTrackingEnabled)
                                                {
                                                    //Normal spectrum analyzer sweep data
                                                    Monitor.Enter(m_arrReceivedData);
                                                    m_arrReceivedData.Enqueue(objSweep);
                                                    Monitor.Exit(m_arrReceivedData);
                                                }
                                                else
                                                {
                                                    //tracking generator sweep data, we capture a sweep point every time to build a new full objSweepTracking
                                                    if (m_nRFGenTracking_CurrentSweepStep == 0)
                                                    {
                                                        objSweepTracking = new RFESweepData(m_objRFEGen.RFGenStartFrequencyMHZ, m_objRFEGen.RFGenStepFrequencyMHZ, m_objRFEGen.RFGenSweepSteps);
                                                    }
                                                    float fMaxDB = objSweep.GetAmplitudeDBM(objSweep.GetPeakStep());
                                                    objSweepTracking.SetAmplitudeDBM(m_nRFGenTracking_CurrentSweepStep, fMaxDB);

                                                    if (!m_bTrackingNormalizing || (fMaxDB > MIN_AMPLITUDE_TRACKING_NORMALIZE) || !m_bTrackingAllowed)
                                                    {
                                                        //if we are normalizing, make sure the value read is correct or either do not increase step
                                                        m_nRFGenTracking_CurrentSweepStep++;
                                                    }
                                                    else
                                                    {
                                                        nTrackingStepRetry++;
                                                        if (m_bTrackingNormalizing && nTrackingStepRetry > (m_objRFEGen.RFGenSweepSteps / 5))
                                                        {
                                                            //if we retried about the same number of steps the sweep have, then something is really wrong
                                                            m_objRFEGen.SendCommand_GeneratorRFPowerOFF();
                                                            m_bThreadTrackingEnabled = false; //be done with thread tracking activity, so main thread knows
                                                            Monitor.Enter(m_arrReceivedData);
                                                            m_arrReceivedData.Enqueue("Too many retries normalizing data. Review your setup and restart Spectrum Analyzer");
                                                            Monitor.Exit(m_arrReceivedData);
                                                            Monitor.Enter(m_arrReceivedData);
                                                            m_arrReceivedData.Enqueue(objSweepTracking); //send whatever we have, we will detect it outside the thread
                                                            Monitor.Exit(m_arrReceivedData);
                                                        }
                                                    }
                                                    if (m_bThreadTrackingEnabled)
                                                    {
                                                        if (m_nRFGenTracking_CurrentSweepStep <= m_objRFEGen.RFGenSweepSteps)
                                                        {
                                                            m_objRFEGen.SendCommand_TrackingStep(m_nRFGenTracking_CurrentSweepStep);
                                                            SendCommand_TrackingStep(m_nRFGenTracking_CurrentSweepStep);
                                                        }
                                                        else
                                                        {
                                                            //we are done with a tracking sweep capture objSweepTracking, make it available
                                                            m_nTrackingNormalizingPass++;
                                                            m_nTrackingPass++;
                                                            Monitor.Enter(m_arrReceivedData);
                                                            m_arrReceivedData.Enqueue(objSweepTracking);
                                                            Monitor.Exit(m_arrReceivedData);

                                                            //If we need to restart, do it from first step
                                                            m_nRFGenTracking_CurrentSweepStep = 0;

                                                            if ( (m_bTrackingNormalizing && (m_nTrackingNormalizingPass > m_nAutoStopSNATrackingCounter)) || 
                                                                 !m_bTrackingAllowed ||
                                                                 (!m_bTrackingNormalizing && ((m_nAutoStopSNATrackingCounter!=0) && (m_nTrackingPass > m_nAutoStopSNATrackingCounter)))
                                                               )
                                                            {
                                                                //if normalizing is completed, or if we have finished tracking manually or automatically, we are done with RF power
                                                                m_objRFEGen.SendCommand_GeneratorRFPowerOFF();
                                                                m_bThreadTrackingEnabled = false; //be done with thread tracking activity, so main thread knows
                                                            }
                                                            else
                                                            {
                                                                //start all over again a full new sweep
                                                                m_objRFEGen.SendCommand_TrackingStep(m_nRFGenTracking_CurrentSweepStep);
                                                                SendCommand_TrackingStep(m_nRFGenTracking_CurrentSweepStep);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                Monitor.Enter(m_arrReceivedData);
                                                m_arrReceivedData.Enqueue(sNewLine);
                                                Monitor.Exit(m_arrReceivedData);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Monitor.Enter(m_arrReceivedData);
                                        m_arrReceivedData.Enqueue("Ignored $S of size " + nReceivedLength.ToString() + " expected " + FreqSpectrumSteps.ToString());
                                        Monitor.Exit(m_arrReceivedData);
                                    }
                                    strReceived = strReceived.Substring(3 + nReceivedLength + 2);
                                }
                                else if (bLengthOK)
                                {
                                    //So we are here because the string doesn't end with the expected chars, but has the right length. 
                                    //The most likely cause is a truncated string was received, and some chars are from next string, not this one
                                    //therefore we truncate the line to avoid being much larger, and start over again next time.
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
                                        Monitor.Enter(m_arrReceivedData);
                                        m_arrReceivedData.Enqueue(sNewLine);
                                        Monitor.Exit(m_arrReceivedData);
                                    }
                                    else
                                    {
                                        Monitor.Enter(m_arrReceivedData);
                                        m_arrReceivedData.Enqueue("Ignored $S of size " + nReceivedLength.ToString() + " expected " + FreqSpectrumSteps.ToString());
                                        Monitor.Exit(m_arrReceivedData);
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
                                        Monitor.Enter(m_arrReceivedData);
                                        m_arrReceivedData.Enqueue("Received RAW data " + nSize.ToString());
                                        m_arrReceivedData.Enqueue("$R" + strReceived.Substring(nPos + 1, nSize));
                                        Monitor.Exit(m_arrReceivedData);
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
                                Monitor.Enter(m_arrReceivedData);
                                m_arrReceivedData.Enqueue(sNewLine);
                                Monitor.Exit(m_arrReceivedData);
                            }
                            else
                            {
                                //diagnosis only
                                if (m_bDebugTraces)
                                {
                                    Monitor.Enter(m_arrReceivedData);
                                    m_arrReceivedData.Enqueue("DEBUG partial:" + strReceived);
                                    Monitor.Exit(m_arrReceivedData);
                                }
                            }
                        }
                    }
                    if (m_eMode!=eMode.MODE_TRACKING)
                        Thread.Sleep(10);
                    else
                        Thread.Sleep(2); //in tracking mode we want to be as fast as possible
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
                    object objNew = null;
                    long nCount = 0;

                    try
                    {
                        Monitor.Enter(m_arrReceivedData);
                        nCount = m_arrReceivedData.Count;

                        if (nCount == 0)
                            break;
                        objNew = m_arrReceivedData.Dequeue();
                    }
                    catch (Exception obEx)
                    {
                        ReportLog("m_arrReceivedStrings processing: " + obEx.ToString());
                    }
                    finally
                    {
                        Monitor.Exit(m_arrReceivedData);
                    }

                    if (objNew.GetType() == typeof(RFEConfiguration))
                    {
                        RFEConfiguration objConfiguration = (RFEConfiguration)objNew;
                        ReportLog("Received configuration: " + objConfiguration.sLineString);

                        if (m_arrSpectrumAnalyzerEmbeddedCalibrationOffsetDB == null)
                        {
                            //request internal calibration data, if available
                            SendCommand("Cq");
                        }

                        if (IsGenerator())
                        {
                            //signal generator
                            RFGenStartFrequencyMHZ = objConfiguration.fStartMHZ;
                            RFGenStepFrequencyMHZ = objConfiguration.fStepMHZ;
                            RFGenCWFrequencyMHZ = objConfiguration.fRFEGenCWFreqMHZ;
                            RFGenSweepSteps = objConfiguration.nFreqSpectrumSteps;
                            RFGenStopFrequencyMHZ = RFGenStartFrequencyMHZ + RFGenSweepSteps * RFGenStepFrequencyMHZ;
                            RFGenPowerON = objConfiguration.bRFEGenPowerON;
                            RFGenPowerLevel = objConfiguration.nRFEGenPowerLevel;
                            RFGenHighPowerSwitch = objConfiguration.bRFEGenHighPowerSwitch;

                            MaxFreqMHZ = 6000;
                            MinFreqMHZ = 23.438;

                            RFGenStepWaitMS = objConfiguration.nRFEGenSweepWaitMS;

                            OnReceivedConfigurationData(new EventArgs());
                        }
                        else
                        {
                            //spectrum analyzer
                            if ((Math.Abs(StartFrequencyMHZ - objConfiguration.fStartMHZ) >= 0.001) || (Math.Abs(StepFrequencyMHZ - objConfiguration.fStepMHZ) >= 0.001))
                            {
                                StartFrequencyMHZ = objConfiguration.fStartMHZ;
                                StepFrequencyMHZ = objConfiguration.fStepMHZ;
                                ReportLog("New Freq range - buffer cleared.");
                            }
                            AmplitudeTopDBM = objConfiguration.fAmplitudeTopDBM;
                            AmplitudeBottomDBM = objConfiguration.fAmplitudeBottomDBM;
                            FreqSpectrumSteps = objConfiguration.nFreqSpectrumSteps;
                            m_bExpansionBoardActive = objConfiguration.bExpansionBoardActive;
                            if (m_bExpansionBoardActive)
                            {
                                m_eActiveModel = m_eExpansionBoardModel;
                                if (ExpansionBoardModel == RFECommunicator.eModel.MODEL_WSUB3G)
                                {
                                    //If it is a MODEL_WSUB3G, make sure we use the MAX HOLD mode to account for proper DSP
                                    Thread.Sleep(500);
                                    if (m_bUseMaxHold)
                                    {
                                        ReportLog("Updated remote mode to Max Hold for reliable DSP calculations with fast signals");
                                        SendCommand_SetMaxHold();
                                    }
                                    else
                                    {
                                        ReportLog("Remote mode is not Max Hold, some fast signals may not be detected");
                                        SendCommand_Realtime();
                                    }
                                }
                            }
                            else
                            {
                                m_eActiveModel = m_eMainBoardModel;
                            }
                            m_eMode = objConfiguration.eMode;

                            MinFreqMHZ = objConfiguration.fMinFreqMHZ;
                            MaxFreqMHZ = objConfiguration.fMaxFreqMHZ;
                            MaxSpanMHZ = objConfiguration.fMaxSpanMHZ;

                            m_fRBWKHZ = objConfiguration.fRBWKHZ;
                            m_fOffset_dB = objConfiguration.fOffset_dB;

                            if ((m_eActiveModel == eModel.MODEL_2400) || (m_eActiveModel == eModel.MODEL_6G))
                            {
                                MinSpanMHZ = 2.0;
                            }
                            else
                            {
                                MinSpanMHZ = 0.112;
                            }

                            if (objConfiguration.nFreqSpectrumSteps == 13)
                            {
                                m_eMode = RFECommunicator.eMode.MODE_WIFI_ANALYZER;
                            }
                            else
                            {
                                m_eMode = RFECommunicator.eMode.MODE_SPECTRUM_ANALYZER;
                            }

                            OnReceivedConfigurationData(new EventArgs());
                        }
                    }
                    else if (objNew.GetType() == typeof(RFESweepData))
                    {
                        if (m_eMode == eMode.MODE_TRACKING)
                        {
                            if (m_bTrackingNormalizing)
                            {
                                if (m_SweepTrackingNormalizedContainer == null)
                                    m_SweepTrackingNormalizedContainer = new RFESweepDataCollection(3,true);

                                RFESweepData objSweep=(RFESweepData)objNew;
                                m_SweepTrackingNormalizedContainer.Add(objSweep);
                                bool bWrongData=objSweep.GetAmplitudeDBM(objSweep.GetMinStep())<=MIN_AMPLITUDE_TRACKING_NORMALIZE;

                                if (bWrongData || ((m_nAutoStopSNATrackingCounter != 0) && (m_SweepTrackingNormalizedContainer.Count >= m_nAutoStopSNATrackingCounter)))
                                {
                                    StopTracking();

                                    if (bWrongData)
                                        //invalid data, end so it can be restarted
                                        m_SweepTrackingNormalized = objSweep;
                                    else
                                        //if all samples collected, end and get average among them
                                        m_SweepTrackingNormalized = m_SweepTrackingNormalizedContainer.GetAverage(0, m_SweepTrackingNormalizedContainer.Count - 1);

                                    OnUpdateDataTrakingNormalization(new EventArgs());
                                }
                            }
                            else
                            {
                                RFESweepData objSweep = (RFESweepData)objNew;
                                m_TrackingDataContainer.Add(objSweep);
                                bDraw = true;
                                OnUpdateDataTraking(new EventArgs());

                                if ((m_nAutoStopSNATrackingCounter != 0) && (m_nTrackingPass > m_nAutoStopSNATrackingCounter))
                                {
                                    StopTracking();
                                }
                            }
                        }
                        else
                        {
                            if (!HoldMode)
                            {
                                RFESweepData objSweep = (RFESweepData)objNew;
                                m_SweepDataContainer.Add(objSweep);

                                bDraw = true;
                                if (m_SweepDataContainer.IsFull())
                                {
                                    HoldMode = true;
                                    OnUpdateFeedMode(new EventArgs());
                                    ReportLog("RAM Buffer is full.");
                                }
                                m_sSweepInfoText = "Captured:" + objSweep.CaptureTime.ToString("yyyy-MM-dd HH:mm:ss\\.fff");
                                TimeSpan objSpan = new TimeSpan();
                                objSpan = objSweep.CaptureTime - m_LastCaptureTime;
                                if (objSpan.TotalSeconds < 60)
                                {
                                    //if time between captures is less than 60 seconds, we can assume we are getting realtime data
                                    //and therefore can provide average sweep/seconds information, otherwise we were in hold or something
                                    //and data could not be used for these calculations.
                                    m_nAverageSweepSpeedIterator++;
                                    m_spanAverageSpeedAcumulator += (objSweep.CaptureTime - m_LastCaptureTime);
                                    if (m_fAverageSweepTime > 0.0)
                                    {
                                        m_sSweepInfoText += "\nSweep time: " + m_fAverageSweepTime.ToString("0.000") + " seconds - Avg Sweeps/second: " + (1.0 / m_fAverageSweepTime).ToString("0.0");
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
                            else
                            {
                                //if in hold mode, we just record last time came here to make sure we start from most reliable point in time
                                m_LastCaptureTime = DateTime.Now;
                            }
                        }
                    }
                    else if (objNew.GetType() == typeof(RFEScreenData))
                    {
                        if ((CaptureRemoteScreen) && (m_ScreenDataContainer.IsFull() == false))
                        {
                            RFEScreenData objScreen = (RFEScreenData)objNew;
                            objScreen.Model = ActiveModel;
                            m_ScreenDataContainer.Add(objScreen);
                            ScreenIndex = (UInt16)m_ScreenDataContainer.UpperBound;
                            OnUpdateRemoteScreen(new EventArgs());
                        }
                        else
                        {
                            //receiving Screen Dump data but it was intended to be disabled, resend a disable command now
                            SendCommand("D0");
                        }
                    }
                    else
                    {
                        //received a string, so use it along to parse parameters
                        string sLine = (string)objNew;
                        sReceivedString = sLine;

                        if ((sLine.Length > 3) && (sLine.Substring(0, 4) == _Acknowldedge))
                        {
                            m_bAcknowledge = true;
                        }
                        else if ((sLine.Length > 16) && (sLine.Substring(0, 3) == "#Sn"))
                        {
                            m_sSerialNumber = sLine.Substring(3, 16);
                            ReportLog("Device serial number: " + SerialNumber);
                        }
                        else if((sLine.Length > 2) && (sLine.Substring(0, 2) == "$q"))
                        {
                            //calibration data
                            UInt16 nSize=Convert.ToUInt16(sLine[2]);

                            if (IsGenerator())
                            {
                                //signal generator uses a different approach for storing absolute amplitude value offset over an ideal -30dBm response
                                if ((m_arrSignalGeneratorEmbeddedCalibrationActual30DBM == null) || (m_arrSignalGeneratorEmbeddedCalibrationActual30DBM.Length != nSize))
                                {
                                    m_arrSignalGeneratorEmbeddedCalibrationActual30DBM = new double[nSize];
                                    for (int nInd = 0; nInd < nSize; nInd++)
                                    {
                                        m_arrSignalGeneratorEmbeddedCalibrationActual30DBM[nInd] = -30.0;
                                    }

                                    //Values using 10*delta from the value delivered when compared with 30dBm.
                                    //For instance if value delivered for a frequency is -28.5dBm, that is a +1.5dB difference
                                    //therefore a 1.5*10=15 value. If the value delivered is -33.2 that is a -3.2dB difference
                                    //therefore a -32 value.

                                    string sData = "Embedded calibration Signal Generator data received:";
                                    for (int nInd = 0; nInd < nSize; nInd++)
                                    {
                                        m_arrSignalGeneratorEmbeddedCalibrationActual30DBM[nInd] = -30.0 + Convert.ToInt16(sLine[nInd + 3]) / 10.0;
                                        sData += m_arrSignalGeneratorEmbeddedCalibrationActual30DBM[nInd].ToString();
                                        if (nInd < nSize - 1)
                                            sData += ",";
                                    }
                                    ReportLog(sData);
                                }
                            }
                            else
                            {
                                if ((m_arrSpectrumAnalyzerEmbeddedCalibrationOffsetDB == null) || (m_arrSpectrumAnalyzerEmbeddedCalibrationOffsetDB.Length != nSize))
                                {
                                    m_arrSpectrumAnalyzerEmbeddedCalibrationOffsetDB = new float[nSize];
                                    for (int nInd = 0; nInd < nSize; nInd++)
                                    {
                                        m_arrSpectrumAnalyzerEmbeddedCalibrationOffsetDB[nInd] = 0.0f;
                                    }
                                }

                                if (m_eActiveModel == eModel.MODEL_6G)
                                {
                                    string sData = "Embedded calibration Spectrum Analyzer data received:";
                                    for (int nInd = POS_EMBEDDED_CALIBRATED_6G; nInd < nSize; nInd++)
                                    {
                                        int nVal = Convert.ToInt32(sLine[nInd + 3]);
                                        if (nVal > 127)
                                            nVal = -(256 - nVal); //get the right sign
                                        m_arrSpectrumAnalyzerEmbeddedCalibrationOffsetDB[nInd] = nVal / 2.0f; //split by two to get dB
                                        sData += m_arrSpectrumAnalyzerEmbeddedCalibrationOffsetDB[nInd].ToString();
                                        if (nInd < nSize - 1)
                                            sData += ",";
                                    }
                                    ReportLog(sData);
                                }
                            }
                        }
                        else if ((sLine.Length > 18) && (sLine.Substring(0, 18) == _ResetString))
                        {
                            //RF Explorer device was reset for some reason, reconfigure client based on new configuration
                            OnDeviceResetEvent(new EventArgs());
                        }
                        else if ((sLine.Length > 5) && sLine.Substring(0, 6) == "#C2-M:")
                        {
                            ReportLog("Received RF Explorer device model info:" + sLine);
                            m_eMainBoardModel = (eModel)Convert.ToUInt16(sLine.Substring(6, 3));
                            m_eExpansionBoardModel = (eModel)Convert.ToUInt16(sLine.Substring(10, 3));
                            m_sRFExplorerFirmware = (sLine.Substring(14, 5));
                            OnReceivedDeviceModel(new EventArgs());
                        }
                        else if ((sLine.Length > 5) && sLine.Substring(0, 6) == "#C3-M:")
                        {
                            ReportLog("Received RF Explorer Generator device info:" + sLine);
                            m_eMainBoardModel = (eModel)Convert.ToUInt16(sLine.Substring(6, 3));
                            m_eExpansionBoardModel = (eModel)Convert.ToUInt16(sLine.Substring(10, 3));
                            m_sRFExplorerFirmware = (sLine.Substring(14, 5));
                            OnReceivedDeviceModel(new EventArgs());
                        }
                        else if ((sLine.Length > 2) && sLine.Substring(0, 3) == "#K1")
                        {
                            ReportLog("RF Explorer is now in TRACKING mode.");
                            m_eMode = eMode.MODE_TRACKING;
                        }
                        else if ((sLine.Length > 2) && sLine.Substring(0, 3) == "#K0")
                        {
                            ReportLog("RF Explorer is now in ANALYZER mode.");
                            m_eMode = eMode.MODE_SPECTRUM_ANALYZER;
                        }
                        else if ((sLine.Length > 2) && (sLine.Substring(0, 2) == "$S") && (StartFrequencyMHZ > 0.1))
                        {
                            bWrongFormat = true;
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
                        else if ((sLine.Length > 5) && (sLine.Substring(0, 6) == "#C2-F:"))
                        {
                            bWrongFormat = true; //parsed on the thread
                        }
                        else if ((sLine.Length > 5) && (sLine.Substring(0, 6) == "#C1-F:"))
                        {
                            bWrongFormat = true; //obsolete firmware
                        }
                        else
                        {
                            ReportLog(sLine); //report any line we don't understand - it is likely a human readable message
                        }

                        if (bWrongFormat)
                        {
                            ReportLog("Received unexpected data from RFExplorer device:" + sLine);
                            ReportLog("Please update your RF Explorer to a recent firmware version and");
                            ReportLog("make sure you are using the latest version of RF Explorer for Windows.");
                            ReportLog("Visit http://www.rf-explorer/download for latest firmware updates.");

                            OnWrongFormatData(new EventArgs());
                        }
                    }

                } while (bProcessAllEvents && (m_arrReceivedData.Count > 0));
            }
            catch (Exception obEx)
            {
                ReportLog("ProcessReceivedString: " + sReceivedString + Environment.NewLine + obEx.ToString());
            }

            return bDraw;
        }


        /// <summary>
        /// True if the connected object is a Signal Generator model
        /// </summary>
        /// <returns></returns>
        public bool IsGenerator()
        {
            return MainBoardModel == eModel.MODEL_RFGEN;
        }

        /// <summary>
        /// True if the connected object is a Spectrum Analyzer model
        /// </summary>
        /// <returns></returns>
        public bool IsAnalyzer()
        {
            return (!IsGenerator()) && (MainBoardModel!=eModel.MODEL_NONE);
        }

        UInt16 m_nAutoStopSNATrackingCounter = 0;
        /// <summary>
        /// For SNA tracking mode, this setting will indicate how many tracking passes should be done before stop, or 0 for infinite
        /// </summary>
        public UInt16 AutoStopSNATrackingCounter
        {
            get { return m_nAutoStopSNATrackingCounter; }
            set { m_nAutoStopSNATrackingCounter = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sConfigurationString"></param>
        /// <param name="sModel"></param>
        private bool UpdateOfflineConfigurationParameters_Generator(string sConfigurationString, string sModel)
        {
            if (String.IsNullOrEmpty(sConfigurationString))
                return false;

            if (String.IsNullOrEmpty(sModel))
                return false;

            //TODO: enable a smart comparison, only for actual model otherwise string is different due to client model, or (ACTIVE) string, etc
            /*if (sModel != m_objRFEGen.FullModelText)
            {
                Console.WriteLine("Model text is different " + sModel + " ||| " + m_objRFEGen.FullModelText);
                return false;
            }*/

            string sValues = sConfigurationString.Replace(" - ", "@").ToLower(); //get a reliable separator field
            sValues = sValues.Replace("mhz", "");
            sValues = sValues.Replace("start:", "");
            sValues = sValues.Replace("stop:", "");
            sValues = sValues.Replace("stepwait:", "");
            sValues = sValues.Replace("sweepsteps:", "");
            sValues = sValues.Replace("step:", "");
            sValues = sValues.Replace("highpowerswitch:", "");
            sValues = sValues.Replace("powerlevel:", "");
            sValues = sValues.Replace("cw:", "");
            sValues = sValues.Replace("ms", "");
            string[] arrValues = sValues.Split('@');

            m_objRFEGen.RFGenCWFrequencyMHZ = Convert.ToDouble(arrValues[0]);
            m_objRFEGen.RFGenStartFrequencyMHZ = Convert.ToDouble(arrValues[1]);
            m_objRFEGen.RFGenStopFrequencyMHZ = Convert.ToDouble(arrValues[2]);
            m_objRFEGen.RFGenStepFrequencyMHZ = Convert.ToDouble(arrValues[3]);
            m_objRFEGen.RFGenPowerLevel = Convert.ToByte(arrValues[4]);
            m_objRFEGen.RFGenHighPowerSwitch = (arrValues[5] == "true");
            m_objRFEGen.RFGenSweepSteps = Convert.ToUInt16(arrValues[6]);
            m_objRFEGen.RFGenStepWaitMS = Convert.ToUInt16(arrValues[7]);

            return true;
        }

        /// <summary>
        /// This method will parse Spectrum Analyzer configuration and model string read from data file and will update current configuration to match that.
        /// Note: for this to work the device must be disconnected, you cannot change runtime parameters if a device is connected
        /// Rather than parsing, a better way since the begining would have been to store each and everyone of the parameters separately in the file but,
        /// given that was not the case and to keep backward compatibility with files, we keep the string human readable format
        /// and parse it here for machine usability.
        /// </summary>
        private void UpdateOfflineConfigurationParameters_Analyzer(string sConfigurationString, string sModel)
        {
            if (PortConnected)
                return; //we do this only if device is offline

            if (!String.IsNullOrEmpty(sConfigurationString))
            {
                string sValues = sConfigurationString.Replace(" - ", "@").ToLower(); //get a reliable separator field
                sValues = sValues.Replace("from file:", "");
                sValues = sValues.Replace("mhz", "");
                sValues = sValues.Replace("khz", "");
                sValues = sValues.Replace("dbm", "");
                sValues = sValues.Replace("start:", "");
                sValues = sValues.Replace("stop:", "");
                sValues = sValues.Replace("center:", "");
                sValues = sValues.Replace("span:", "");
                sValues = sValues.Replace("sweep step:", "");
                sValues = sValues.Replace("rbw:", "");
                sValues = sValues.Replace("amp offset:", "");
                sValues = sValues.Replace(" ", "");
                string[] arrValues = sValues.Split('@');

                //note, we do not use many of these fiels because already came from sweep data in binary format
                m_fRBWKHZ = 0.0f;
                m_fOffset_dB = 0.0f;
                if (arrValues.Length >= 6)
                {
                    if (arrValues.Length >= 6)
                        m_fRBWKHZ = Convert.ToDouble(arrValues[5]);
                    if (arrValues.Length >= 7)
                        m_fOffset_dB = (float)Convert.ToDouble(arrValues[6]);
                }
            }

            if (!String.IsNullOrEmpty(sModel))
            {
                string sValues = sModel.Replace(" - ", "@").ToLower(); //get a reliable separator field
                sValues = sValues.Replace("-", "@");
                sValues = sValues.Replace("from file:", "");
                sValues = sValues.Replace("expansion module:", "");
                sValues = sValues.Replace("client v", "");
                sValues = sValues.Replace("firmware v", "");
                sValues = sValues.Replace("model:", "");
                sValues = sValues.Replace("active range:", "");
                sValues = sValues.Replace("mhz", "");
                sValues = sValues.Replace("no expansion module found",eModel.MODEL_NONE.ToString().ToLower());
                sValues = sValues.Replace(" ", "");
                string[] arrValues = sValues.Split('@');

                m_sRFExplorerFirmware = "";
                m_eMainBoardModel = eModel.MODEL_NONE;
                m_eExpansionBoardModel= eModel.MODEL_NONE;
                m_eActiveModel = eModel.MODEL_NONE;
                m_bExpansionBoardActive = false;
                if (arrValues.Length > 2)
                {
                    //Update model enumerators from text
                    string sModelMainBoard = arrValues[2];
                    string sModelExpansion = arrValues[3];
                    m_bExpansionBoardActive = false;

                    //First determine what is the active board
                    if (sModelExpansion.Contains(_ACTIVE.ToLower()))
                    {
                        m_bExpansionBoardActive = true;
                    }
                    sModelMainBoard = sModelMainBoard.Replace(_ACTIVE.ToLower(), "");
                    sModelExpansion = sModelExpansion.Replace(_ACTIVE.ToLower(), "");

                    //Now get each board model
                    m_eMainBoardModel = GetModelEnumFromText(sModelMainBoard);
                    m_eExpansionBoardModel = GetModelEnumFromText(sModelExpansion);
                    if (m_bExpansionBoardActive)
                    {
                        m_eActiveModel = m_eExpansionBoardModel;
                    }
                    else
                    {
                        m_eActiveModel = m_eMainBoardModel;
                    }

                    //Get firmware
                    m_sRFExplorerFirmware = arrValues[1];
                    //Get max min frequency
                    MinFreqMHZ = Convert.ToDouble(arrValues[4]);
                    MaxFreqMHZ = Convert.ToDouble(arrValues[5]);
                    MaxSpanMHZ = 0.0; //Unknown span, not saved in file format
                }
            }
        }


        public void ResetInternalBuffers()
        {
            //we use this method to internally restore capture buffers to empty status
            if (m_bUseMaxHold)
            {
                SendCommand_Realtime();
                Thread.Sleep(500);
                SendCommand_SetMaxHold();
            }
        }

        /// <summary>
        /// Step from 0-9999 to set the tracking configuration
        /// </summary>
        /// <param name="nStep">step to select analyzer or tracking generator to work on momentarily</param>
        public void SendCommand_TrackingStep(UInt16 nStep)
        {
            byte nByte1 = Convert.ToByte(nStep >> 8);
            byte nByte2 = Convert.ToByte(nStep & 0x00ff);

            SendCommand("k" + Convert.ToChar(nByte1) + Convert.ToChar(nByte2));
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

        public void SendCommand_SetMaxHold()
        {
            SendCommand("C+\x4");
        }

        public void SendCommand_Realtime()
        {
            SendCommand("C+\x0");
        }

        public void SendCommand_GeneratorRFPowerOFF()
        {
            if (IsGenerator())
            {
                RFGenPowerON = false;
                SendCommand("CP0");
            }
        }

        public void SendCommand_GeneratorRFPowerON()
        {
            if (IsGenerator())
            {
                RFGenPowerON = true;
                SendCommand("CP1");
            }
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

        public bool SaveFileRFE(string sFilename, bool bUseCorrection)
        {
            if (bUseCorrection)
                SweepData.SaveFile(sFilename, FullModelText, ConfigurationText, m_AmplitudeCalibration);
            else
                SweepData.SaveFile(sFilename, FullModelText, ConfigurationText, null);
            return true;
        }

        public bool SaveFileSNANormalization(string sFilename)
        {
            if (!IsTrackingNormalized())
                return false;

            RFESweepDataCollection objCollection = new RFESweepDataCollection(1, true);
            objCollection.Add(TrackingNormalizedData);
            objCollection.SaveFile(sFilename, FullModelText + " - " + _RFEGEN_FILE_MODEL_Mark + " " + m_objRFEGen.FullModelText,
                ConfigurationText + " - " + _RFEGEN_FILE_MODEL_Mark + " " + m_objRFEGen.ConfigurationText, null);

            return true;
        }

        /// <summary>
        /// load a normalization SNA file and reconfigures m_SweepTrackingNormalized based on that
        /// </summary>
        /// <param name="sFilename"></param>
        /// <returns></returns>
        private bool LoadFileSNANormalization(string sFilename)
        {
            RFESweepDataCollection objCollection = new RFESweepDataCollection(1, true);
            string sModel, sConfig;
            if (objCollection.LoadFile(sFilename, out sModel, out sConfig))
            {
                if (!sModel.Contains(_RFEGEN_FILE_MODEL_Mark))
                    return false;

                int nIndRFEGen = sModel.IndexOf(_RFEGEN_FILE_MODEL_Mark);
                string sModelRFEGen = sModel.Substring(nIndRFEGen + 1 + _RFEGEN_FILE_MODEL_Mark.Length);
                nIndRFEGen = sConfig.IndexOf(_RFEGEN_FILE_MODEL_Mark);
                string sConfigRFEGen = sConfig.Substring(nIndRFEGen + 1 + _RFEGEN_FILE_MODEL_Mark.Length);
                m_SweepTrackingNormalized = objCollection.GetData(0);

                return UpdateOfflineConfigurationParameters_Generator(sConfigRFEGen, sModelRFEGen);
            }
            else 
                return false;
        }

        //This variable contains the latest correction file loaded
        public RFEAmplitudeTableData m_AmplitudeCalibration=new RFEAmplitudeTableData();
        /// <summary>
        /// Use this to load a correction file (will replace any prior file loaded)
        /// </summary>
        /// <param name="sFilename"></param>
        /// <returns></returns>
        public bool LoadFileRFA(string sFilename)
        {
            return m_AmplitudeCalibration.LoadFile(sFilename);
        }

        /// <summary>
        /// Returns the current correction amplitude value for a given MHZ frequency
        /// </summary>
        /// <param name="nMHz"></param>
        /// <returns></returns>
        public float GetAmplitudeCorrectionDB(int nMHz)
        {
            return m_AmplitudeCalibration.GetAmplitudeCalibration(nMHz);
        }

        /// <summary>
        /// Loads a sweep data file, it can be a .RFE sweep data file, a .SNA tracking file or a .SNANORM normalization tracking file
        /// This is only valid for analyzer objects. A tracking generator will be updated from SNA if linked to the analyzer, but never call this method
        /// from a generator object itself
        /// </summary>
        /// <param name="sFilename"></param>
        /// <returns></returns>
        public bool LoadDataFile(string sFilename)
        {
            string sConfiguration = "";
            string sModel = "";

            if (IsGenerator())
                return false; //only valid for analyzer

            if (IsFileExtensionType(sFilename,_RFE_File_Extension))
            {
                //normal sweep data file
                if (SweepData.LoadFile(sFilename, out sModel, out sConfiguration))
                {
                    //This is standard sweep data file with spectrum analyzer info only
                    if (sModel.Contains(_RFEGEN_FILE_MODEL_Mark))
                    {
                        return false; //found multi model data in a expected normal sweep file
                    }
                    HoldMode = true;
                    double fAmplitudeTop, fAmplitudeBottom;
                    SweepData.GetTopBottomDataRange(out fAmplitudeTop, out fAmplitudeBottom, m_AmplitudeCalibration);
                    AmplitudeBottomDBM = fAmplitudeBottom - 5;
                    AmplitudeTopDBM = fAmplitudeTop + 15;
                    StartFrequencyMHZ = SweepData.GetData(0).StartFrequencyMHZ;
                    StepFrequencyMHZ = SweepData.GetData(0).StepFrequencyMHZ;
                    FreqSpectrumSteps = SweepData.GetData(0).TotalSteps;

                    //Get offset, RBW and other parameters not saved as individual variables
                    UpdateOfflineConfigurationParameters_Analyzer(sConfiguration, sModel);

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
            }
            else if (IsFileExtensionType(sFilename,_SNANORM_File_Extension))
            {
                if ((m_objRFEGen == null) || (!m_objRFEGen.PortConnected))
                    return false; //we can load and update connected generator only

                return LoadFileSNANormalization(sFilename);
            }
            else
                return false;

            return true;
        }


        #endregion

        #region Tracking Generator
        bool m_bTrackingNormalizing = false; //true if tracking and normalizing
        UInt16 m_nTrackingNormalizingPass = 0; 
        /// <summary>
        /// number of normalization tracking pass completed
        /// </summary>
        public UInt16 TrackingNormalizingPass
        {
            get { return m_nTrackingNormalizingPass; }
        }
        bool m_bTrackingAllowed = false; //true if main thread allows secondary thread to use tracking
        bool m_bThreadTrackingEnabled = false; //true if the thread has tracking enabled, as detected by the thread (may be earlier than other parts of the code)

        UInt16 m_nTrackingPass = 0;
        /// <summary>
        /// number of tracking pass completed
        /// </summary>
        public UInt16 TrackingPass
        {
            get { return m_nTrackingPass; }
        }

        /// <summary>
        /// true if the current tracking mode is for normalization response
        /// </summary>
        public bool IsTrackingNormalizing
        {
            get { return m_bTrackingNormalizing; }
        }
        UInt16 m_nRFGenTracking_CurrentSweepStep = 0; //step used dynamically while doing tracking
        /// <summary>
        /// Current tracking step being measured within the sweep
        /// </summary>
        public UInt16 RFGenTrackingCurrentStep
        {
            get { return m_nRFGenTracking_CurrentSweepStep; }
        }

        private string GetRFGenPowerString()
        {
            string sPower = ",";
            if (RFGenHighPowerSwitch)
                sPower += "1,";
            else
                sPower += "0,";
            sPower += RFGenPowerLevel;

            return sPower;
        }

        //used to temporarily store the configuration of the analyzer before it goes to tracking mode
        double  m_BackupStartMHZ, m_BackupStopMHZ, m_BackupTopDBM, m_BackupBottomDBM; 
        /// <summary>
        /// Start and completes asynchronous tracking sequence, this action is performed on the Analyzer and will internally
        /// drive and handle the Generator.
        /// </summary>
        /// <param name="bNormalize">If true, the sequence will be saved as normalization sequence</param>
        /// <returns>true if sequence started correctly, false otherwise</returns>
        public bool StartTrackingSequence(bool bNormalize)
        {
            bool bOk = true;

            if (IsGenerator())
            {
                ReportLog("Invalid command sent to RF Explorer Signal Generator (StartTrackingSequence)");
                return false; //This can only be used in the analyzer
            }

            if ((m_objRFEGen==null) || (!m_objRFEGen.PortConnected))
            {
                //Signal Generator not connected or available
                ReportLog("RF Explorer Signal Generator not connected");
                return false;
            }

            //enable normalization and save prior analyzer configuration
            m_nTrackingNormalizingPass = 0;
            m_nTrackingPass = 0;
            m_bTrackingNormalizing = bNormalize;
            if (bNormalize)
            {
                ResetTrackingNormalizedData();
            }

            //Backup current configuration
            m_BackupStartMHZ = StartFrequencyMHZ;
            m_BackupStopMHZ = StopFrequencyMHZ;
            m_BackupTopDBM = AmplitudeTopDBM;
            m_BackupBottomDBM = AmplitudeBottomDBM;

            //start actual tracking
            m_bTrackingAllowed = true; //tell thread we allow tracking being enabled
            m_nRFGenTracking_CurrentSweepStep = 0;
            
            m_objRFEGen.SendCommand_GeneratorCW();
            m_objRFEGen.RFGenPowerON = true;
            SendCommand("C3-K:" + ((UInt32)(m_objRFEGen.RFGenStartFrequencyMHZ * 1000)).ToString("D7") + "," + ((UInt32)(m_objRFEGen.RFGenStepFrequencyMHZ * 1000)).ToString("D7"));

            return bOk;
        }

        /// <summary>
        /// Start CW generation using current configuration setting values - only valid for Signal Generator models
        /// </summary>
        public void SendCommand_GeneratorCW()
        {
            if (IsGenerator())
            {
                string sSteps = "," + RFGenSweepSteps.ToString("D4") + ",";

                double dStepMHZ = RFGenTrackStepMHZ();
                if (dStepMHZ < 0)
                {
                    return;
                }

                SendCommand("C3-F:" + ((UInt32)(RFGenStartFrequencyMHZ * 1000)).ToString("D7") + GetRFGenPowerString() + sSteps + ((UInt32)(dStepMHZ * 1000)).ToString("D7"));
            }
        }

        /// <summary>
        /// Configured tracking step size in MHZ
        /// </summary>
        /// <returns></returns>
        public double RFGenTrackStepMHZ()
        {
            return (RFGenStopFrequencyMHZ - RFGenStartFrequencyMHZ) / RFGenSweepSteps;
        }

        public void StopTracking()
        {
            //use backed up configuration to start back in analyzer mode
            m_bTrackingAllowed = false; //tell thread the tracking must stop
            m_bTrackingNormalizing = false;
            m_objRFEGen.SendCommand_GeneratorRFPowerOFF();

            int nWaitInd = 0;
            while (m_bThreadTrackingEnabled)
            {
                //wait till tracking sweep is done before changing unit to spectrum analyzer mode
                Thread.Sleep(100);
                nWaitInd++;
                if (nWaitInd>100)
                {
                    //too much to keep waiting
                    m_bThreadTrackingEnabled = false; //force end of tracking
                    break;
                }
            }
            UpdateDeviceConfig(m_BackupStartMHZ, m_BackupStopMHZ, m_BackupTopDBM, m_BackupBottomDBM);
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
                    string sPorts = "";
                    foreach (string sValue in m_arrValidCP2101Ports)
                    {
                        sPorts += sValue + " ";
                    }
                    ReportLog("RF Explorer Valid Ports found: " + sPorts.Trim());
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

                OnPortConnected(new EventArgs()); 
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
                    m_bPortConnected = false; //do this here so the external event has the right port status
                    OnPortClosed(new EventArgs());
                }
            }
            catch { }
            finally
            {
                Monitor.Exit(m_serialPortObj);
            }
            m_bPortConnected = false; //to be double safe in case of exception

            m_LastCaptureTime = new DateTime(2000, 1, 1);

            m_sSerialNumber = "";
            m_arrSpectrumAnalyzerEmbeddedCalibrationOffsetDB = null;
            m_arrSignalGeneratorEmbeddedCalibrationActual30DBM = null;

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

        bool m_bShowDetailedCOMPortInfo = true;

        public bool ShowDetailedCOMPortInfo
        {
            get { return m_bShowDetailedCOMPortInfo; }
            set { m_bShowDetailedCOMPortInfo = value; }
        }

        public void GetValidCOMPorts()
        {
            m_arrValidCP2101Ports = null;

            string csSubkey = "SYSTEM\\CurrentControlSet\\Enum\\USB\\VID_10C4&PID_EA60";
            RegistryKey regUSBKey = Registry.LocalMachine.OpenSubKey(csSubkey, RegistryKeyPermissionCheck.ReadSubTree, System.Security.AccessControl.RegistryRights.QueryValues | System.Security.AccessControl.RegistryRights.EnumerateSubKeys);

            if (regUSBKey == null)
            {
                ReportLog("Found no CP210x registry entries");
                return;
            }

            string[] arrDeviceCP210x = regUSBKey.GetSubKeyNames();
            if (m_bShowDetailedCOMPortInfo)
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
                            if (m_bShowDetailedCOMPortInfo)
                                ReportLog("   FriendlyName: " + obFriendlyName.ToString());
                            RegistryKey regDevice = regUSBID.OpenSubKey("Device Parameters", RegistryKeyPermissionCheck.ReadSubTree, System.Security.AccessControl.RegistryRights.QueryValues);
                            if (regDevice != null)
                            {
                                object obPortName = regDevice.GetValue("PortName");
                                string sPortName = obPortName.ToString();
                                if (m_bShowDetailedCOMPortInfo)
                                    ReportLog("   PortName: " + sPortName);
                                if (IsConnectedPort(sPortName) && !IsRepeatedPort(sPortName))
                                {
                                    if (m_bShowDetailedCOMPortInfo)
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
            long nTotalPortsFound = 0;
            if (m_arrValidCP2101Ports != null)
                nTotalPortsFound = m_arrValidCP2101Ports.Length;
            ReportLog("Total ports found: " + nTotalPortsFound);
        }
        #endregion

        #region Events

        public event EventHandler MemoryBlockUpdateEvent;
        private void OnMemoryBlockUpdate(EventReportInfo eventArgs)
        {
            if (MemoryBlockUpdateEvent != null)
            {
                MemoryBlockUpdateEvent(this, eventArgs);
            }
        }

        /// <summary>
        /// Optional
        /// This event will fire everytime there is some information human readable to display
        /// </summary>
        public event EventHandler ReportInfoAddedEvent;
        private void OnReportInfoAdded(EventReportInfo eventArgs)
        {
            if (ReportInfoAddedEvent != null)
            {
                ReportInfoAddedEvent(this, eventArgs);
            }
        }

        /// <summary>
        /// Optional
        /// This event will fire when a wrong format response is received from RFE
        /// </summary>
        public event EventHandler WrongFormatDataEvent;
        private void OnWrongFormatData(EventArgs eventArgs)
        {
            if (WrongFormatDataEvent != null)
            {
                WrongFormatDataEvent(this, eventArgs);
            }
        }

        /// <summary>
        /// Optional
        /// This event will fire when a string identified as reset is received from RFE
        /// </summary>
        public event EventHandler DeviceReset;
        private void OnDeviceResetEvent(EventArgs eventArgs)
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
        public event EventHandler ReceivedConfigurationDataEvent;
        private void OnReceivedConfigurationData(EventArgs eventArgs)
        {
            if (ReceivedConfigurationDataEvent != null)
            {
                ReceivedConfigurationDataEvent(this, eventArgs);
            }
        }

        /// <summary>
        /// Required
        /// This event will fire when RFE is sending global configuration back.
        /// </summary>
        public event EventHandler ReceivedDeviceModelEvent;
        private void OnReceivedDeviceModel(EventArgs eventArgs)
        {
            if (ReceivedDeviceModelEvent != null)
            {
                ReceivedDeviceModelEvent(this, eventArgs);
            }
        }


        /// <summary>
        /// Required
        /// This event indicates new data dump has been received from RF Explorer an is ready to be used
        /// </summary>
        public event EventHandler UpdateDataEvent;
        protected virtual void OnUpdateData(EventArgs e)
        {
            if (UpdateDataEvent != null)
            {
                UpdateDataEvent(this, e);
            }
        }

        private RFESweepDataCollection m_SweepTrackingNormalizedContainer;
        private RFESweepData m_SweepTrackingNormalized;
        /// <summary>
        /// Sweep data with values of last normalized tracking scan, valid for current configuration
        /// </summary>
        public RFESweepData TrackingNormalizedData
        {
            get { return m_SweepTrackingNormalized; }
        }

        /// <summary>
        /// returns true if the normalization data is available and no item is lower than MIN_AMPLITUDE_TRACKING_NORMALIZE (considered too low for any valid normalization setup)
        /// </summary>
        /// <returns></returns>
        public bool IsTrackingNormalized()
        {
            bool bReturn = false;

            if (m_SweepTrackingNormalized != null)
            {
                bReturn = (m_SweepTrackingNormalized.TotalSteps > 0) && m_SweepTrackingNormalized.GetAmplitudeDBM(m_SweepTrackingNormalized.GetMinStep()) > MIN_AMPLITUDE_TRACKING_NORMALIZE;
            }

            return bReturn;
        }

        /// <summary>
        /// removes any prior loaeded normalization data
        /// </summary>
        public void ResetTrackingNormalizedData()
        {
            m_SweepTrackingNormalized = null;
            m_SweepTrackingNormalizedContainer = null;
        }

        /// <summary>
        /// Optional
        /// This event indicates Trakcing Normalization data dump has been received from RF Explorer an is ready to be used
        /// </summary>
        public event EventHandler UpdateDataTrakingNormalizationEvent;
        protected virtual void OnUpdateDataTrakingNormalization(EventArgs e)
        {
            if (UpdateDataTrakingNormalizationEvent != null)
            {
                UpdateDataTrakingNormalizationEvent(this, e);
            }
        }

        /// <summary>
        /// Optional
        /// This event indicates Trakcing data dump has been received from RF Explorer an is ready to be used
        /// </summary>
        public event EventHandler UpdateDataTrakingEvent;
        protected virtual void OnUpdateDataTraking(EventArgs e)
        {
            if (UpdateDataTrakingEvent != null)
            {
                UpdateDataTrakingEvent(this, e);
            }
        }

        /// <summary>
        /// Optional
        /// This event indicates new screen dump bitmap has been received from RF Explorer
        /// </summary>
        public event EventHandler UpdateRemoteScreenEvent;
        protected virtual void OnUpdateRemoteScreen(EventArgs e)
        {
            if (UpdateRemoteScreenEvent != null)
            {
                UpdateRemoteScreenEvent(this, e);
            }
        }

        /// <summary>
        /// This event will fire when the feed mode (real time or hold) has changed
        /// </summary>
        public event EventHandler UpdateFeedModeEvent;
        protected virtual void OnUpdateFeedMode(EventArgs e)
        {
            if (UpdateFeedModeEvent != null)
            {
                UpdateFeedModeEvent(this, e);
            }
        }

        /// <summary>
        /// This event will fire in the event of a communication port is connected
        /// </summary>
        public event EventHandler PortConnectedEvent;
        protected virtual void OnPortConnected(EventArgs e)
        {
            if (PortConnectedEvent != null)
            {
                PortConnectedEvent(this, e);
            }
        }

        /// <summary>
        /// This event will fire in the event of a communication port is closed, either by manual user intervention or by a link error
        /// </summary>
        public event EventHandler PortClosedEvent;
        protected virtual void OnPortClosed(EventArgs e)
        {
            if (PortClosedEvent != null)
            {
                PortClosedEvent(this, e);
            }
        }

        #endregion
    }
}
