using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFExplorerCommunicator
{
    public partial class RFECommunicator : IDisposable
    {
        private class RFEConfiguration
        {
            public double fStartMHZ;
            public double fStepMHZ;
            public double fAmplitudeTopDBM;
            public double fAmplitudeBottomDBM;
            public UInt16 nFreqSpectrumSteps;
            public bool bExpansionBoardActive;
            public eMode eMode;
            public double fMinFreqMHZ;
            public double fMaxFreqMHZ;
            public double fMaxSpanMHZ;
            public double fRBWKHZ;
            public float fOffset_dB;
            public string sLineString;

            public bool bRFEGenHighPowerSwitch;
            public byte nRFEGenPowerLevel;
            public double fRFEGenCWFreqMHZ;
            public UInt16 nRFEGenSweepWaitMS;
            public bool bRFEGenPowerON;

            public RFEConfiguration()
            {
                fStartMHZ = 0.0;
                fStepMHZ = 0.0;
                fAmplitudeTopDBM = 0.0;
                fAmplitudeBottomDBM = 0.0;
                nFreqSpectrumSteps = 0;
                bExpansionBoardActive = false;
                eMode = eMode.MODE_NONE;
                fMinFreqMHZ = 0.0;
                fMaxFreqMHZ = 0.0;
                fMaxSpanMHZ = 0.0;
                fRBWKHZ = 0.0;
                fOffset_dB = 0.0f;

                nRFEGenSweepWaitMS = 0;
                bRFEGenHighPowerSwitch=false;
                nRFEGenPowerLevel = 0;
                fRFEGenCWFreqMHZ = 0.0;
                bRFEGenPowerON = false;
            }

            public RFEConfiguration(RFEConfiguration objSource)
            {
                fStartMHZ = objSource.fStartMHZ;
                fStepMHZ = objSource.fStepMHZ;
                fAmplitudeTopDBM = objSource.fAmplitudeTopDBM;
                fAmplitudeBottomDBM = objSource.fAmplitudeBottomDBM;
                nFreqSpectrumSteps = objSource.nFreqSpectrumSteps;
                bExpansionBoardActive = objSource.bExpansionBoardActive;
                eMode = objSource.eMode;
                fMinFreqMHZ = objSource.fMinFreqMHZ;
                fMaxFreqMHZ = objSource.fMaxFreqMHZ;
                fMaxSpanMHZ = objSource.fMaxSpanMHZ;
                fRBWKHZ = objSource.fRBWKHZ;
                fOffset_dB = objSource.fOffset_dB;

                bRFEGenHighPowerSwitch = objSource.bRFEGenHighPowerSwitch;
                nRFEGenPowerLevel = objSource.nRFEGenPowerLevel;
                fRFEGenCWFreqMHZ = objSource.fRFEGenCWFreqMHZ;
                nRFEGenSweepWaitMS = objSource.nRFEGenSweepWaitMS;
                bRFEGenPowerON = objSource.bRFEGenPowerON;
            }

            public bool ProcessReceivedString(string sLine)
            {
                bool bOk = true;

                try
                {
                    sLineString = sLine;

                    if ((sLine.Length >= 60) && (sLine.Substring(0, 6) == "#C2-F:"))
                    {
                        //Spectrum Analyzer mode
                        fStartMHZ = Convert.ToInt32(sLine.Substring(6, 7)) / 1000.0; //note it comes in KHZ
                        fStepMHZ = Convert.ToInt32(sLine.Substring(14, 7)) / 1000000.0;  //Note it comes in HZ
                        fAmplitudeTopDBM = Convert.ToInt32(sLine.Substring(22, 4));
                        fAmplitudeBottomDBM = Convert.ToInt32(sLine.Substring(27, 4));
                        nFreqSpectrumSteps = Convert.ToUInt16(sLine.Substring(32, 4));
                        bExpansionBoardActive = (sLine[37] == '1');
                        eMode = (eMode)Convert.ToUInt16(sLine.Substring(39, 3));
                        fMinFreqMHZ = Convert.ToInt32(sLine.Substring(43, 7)) / 1000.0;
                        fMaxFreqMHZ = Convert.ToInt32(sLine.Substring(51, 7)) / 1000.0;
                        fMaxSpanMHZ = Convert.ToInt32(sLine.Substring(59, 7)) / 1000.0;

                        if (sLine.Length > 66)
                        {
                            fRBWKHZ = Convert.ToInt32(sLine.Substring(67, 5));
                        }
                        if (sLine.Length > 72)
                        {
                            fOffset_dB = Convert.ToInt32(sLine.Substring(73, 4));
                        }
                    }
                    else if ((sLine.Length >= 39) && (sLine.Substring(0, 6) == "#C3-G:"))
                    {
                        //Signal generator mode
                        fStartMHZ = Convert.ToInt32(sLine.Substring(6, 7)) / 1000.0; //note it comes in KHZ
                        fRFEGenCWFreqMHZ = Convert.ToInt32(sLine.Substring(14, 7)) / 1000.0;  //Note it comes in KHZ
                        nFreqSpectrumSteps = Convert.ToUInt16(sLine.Substring(22, 4));
                        fStepMHZ = Convert.ToInt32(sLine.Substring(27, 7)) / 1000.0;  //Note it comes in KHZ
                        bRFEGenHighPowerSwitch = (sLine[35] == '1');
                        nRFEGenPowerLevel = Convert.ToByte(sLine[37]-0x30);
                        bRFEGenPowerON = (sLine[39] == '1');

                        nRFEGenSweepWaitMS = 0;
                        eMode = eMode.MODE_NONE;
                    }
                    else
                        bOk = false;
                }
                catch (Exception)
                {
                    bOk = false;
                }

                return bOk;
            }
        }

    }
}
