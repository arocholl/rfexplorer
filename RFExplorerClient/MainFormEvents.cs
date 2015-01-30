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

#define CALLSTACK_REALTIME
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
    public partial class MainForm : Form
    {
        #region RF Explorer Analyzer Events
        private void OnAnalyzerReceivedConfigData(object sender, EventArgs e)
        {
#if CALLSTACK
            Console.WriteLine("CALLSTACK:OnRFE_ReceivedConfigData");
#endif
            CleanSweepData(); //we do not want mixed data sweep values

            if (m_bCalibrating)
                return; //do not actually display data updates while calibration is in place

            if (m_objRFEAnalyzer.Mode == RFECommunicator.eMode.MODE_TRACKING)
            {
                if (m_MainTab.SelectedTab != m_tabRFGen)
                {
                    m_MainTab.SelectedTab = m_tabRFGen;
                }
                //no actual data to display in analyzer screen when working in tracking mode
                return;
            }

            if (m_objRFEAnalyzer.Mode == RFECommunicator.eMode.MODE_WIFI_ANALYZER)
            {
                //objGraph is a bar chart
                UpdateButtonStatus();
                if (numericIterations.Value < 100)
                    numericIterations.Value = 100; //at least 100 is required for a practical view in this mode
                m_GraphSpectrumAnalyzer.GraphPane.XAxis.Scale.Min = m_objRFEAnalyzer.StartFrequencyMHZ - 5.0f;
                m_GraphSpectrumAnalyzer.GraphPane.XAxis.Scale.Max = m_objRFEAnalyzer.StartFrequencyMHZ + m_objRFEAnalyzer.FreqSpectrumSteps * m_objRFEAnalyzer.StepFrequencyMHZ + 5.0f;
            }

            AutoLoadAmplitudeDataFile();

            if (m_LimitLineOverload.Count>0)
            {
                //potentially offset may change
                m_LimitLineOverload.NewOffset(m_objRFEAnalyzer.AmplitudeOffsetDB);
            }

            SetupSpectrumAnalyzerAxis();
            SaveProperties();

            if (m_MainTab.SelectedTab == m_tabConfiguration)
            {
                m_MainTab.SelectedTab = m_tabSpectrumAnalyzer;
            }

            UpdateConfigControlContents();
            DisplayGroups();

            if (m_chkDebugTraces.Checked)
            {
                DumpAllReceivedBytes();
            }

            ValidateRFGenSweepFreqRanges(true);
        }

        private void CheckSomeTraceModeIsEnabled()
        {
            if (!menuRealtimeTrace.Checked && !menuMaxTrace.Checked && !menuMaxHoldTrace.Checked && !menuAveragedTrace.Checked && !menuMinTrace.Checked)
            {
                menuRealtimeTrace.Checked = true;
            }
        }

        private void OnAnalyzerUpdateData(object sender, EventArgs e)
        {
#if CALLSTACK_REALTIME
            Console.WriteLine("CALLSTACK:OnAnalyzerUpdateData");
#endif
            RFESweepData objData = m_objRFEAnalyzer.SweepData.GetData((uint)m_objRFEAnalyzer.SweepData.Count - 1);
            if (objData != null)
            {
                m_StatusGraphText_Analyzer.Text = m_objRFEAnalyzer.SweepInfoText;

                CheckSomeTraceModeIsEnabled();
                UpdateSweepNumericControls();
                m_WaterfallSweepMaxHold.Add(m_objRFEAnalyzer.SweepData.MaxHoldData.Duplicate());

                if (m_chkDebugTraces.Checked)
                {
                    DumpAllReceivedBytes();
                }
            }
        }

        private void OnAnalyzerUpdateRemoteScreen(object sender, EventArgs e)
        {
            RFECommunicator objRFE = (RFECommunicator)sender;

            RFEScreenData objData = objRFE.ScreenData.GetData((UInt16)objRFE.ScreenIndex);
            if (objData != null)
            {
                UpdateScreenNumericControls();
                //Update button status but first time only to minimize overhead
                if (objRFE.ScreenIndex == 1)
                    UpdateButtonStatus();

                if (m_MainTab.SelectedTab == m_tabRemoteScreen)
                    m_tabRemoteScreen.Invalidate();
            }
        }

        string BinaryToInteger(string sValue)
        {
            string sResult = "";
            UInt64 nValue = 0;
            try
            {
                //Set correct format and trail with zeroes
                int nByte = 0;
                foreach (char cVal in sValue)
                {
                    nValue += (UInt64)(Convert.ToByte(cVal) << (nByte * 8));
                    nByte++;
                }
                sResult = "0x" + nValue.ToString("X") + " " + nValue.ToString();
            }
            catch (Exception obEx)
            {
                ReportLog(obEx.Message,true);
            }

            return sResult;
        }

        private void OnAnalyzerReportLog(object sender, EventArgs e)
        {
            EventReportInfo objArg = (EventReportInfo)e;

            string sLine = objArg.Data;

            if ((sLine.Length > 2) && (sLine.Substring(0, 2) == ":*"))
            {
                //This is a binary set of bytes (can be grouped in 8, 16, 32 or 64 bytes)
                ReportLog("Integer: " + BinaryToInteger(sLine.Substring(2, sLine.Length - 2)), true);
            }
            else
                ReportLog(sLine, false);
        }

        private void OnAnalyzerPortClosed(object sender, EventArgs e)
        {
            UpdateButtonStatus();
            UpdateFeedMode();
            DisplayGroups();
        }

        private void OnAnalyzerButtons_PortClosed(object sender, EventArgs e)
        {
            //nothing to do, already handled in OnRFE_PortClosed
        }

        private void OnAnalyzerPortConnected(object sender, EventArgs e)
        {
            UpdateButtonStatus();
            UpdateFeedMode();
            SaveProperties(); //to save the last used COM port and speed

            OnAutoLCDOff_Click(null, null);
        }

        private void OnAnalyzerButtons_PortConnected(object sender, EventArgs e)
        {
            //nothing to do, already handled in OnRFE_PortConnected
        }

        private void OnAnalyzerUpdateCalibration(object sender, EventArgs e)
        {
        }

        private void OnAnalyzerReceivedDeviceModel(object sender, EventArgs e)
        {
            if (m_objRFEAnalyzer.IsGenerator())
            {
                ReportLog("Device connected in Spectrum Analyzer port is not the right device model, disconnected. Please reconnect in the right port.", false);
                m_objRFEAnalyzer.ClosePort();
            }
            DisplayRequiredFirmware();
            AutoLoadAmplitudeDataFile();
        }

        private void OnAnalyzerUpdateDataTrakingNormalization(object sender, EventArgs e)
        {
            //normalization step finished, now ask the user to connect actual device to test
            m_TrackingStatus.IsVisible = false;
            m_GraphTrackingGenerator.Refresh();
            ReportLog(m_objRFEAnalyzer.TrackingNormalizedData.Dump(),false);
            if (!m_objRFEAnalyzer.IsTrackingNormalized())
            {
                m_objRFEAnalyzer.ResetTrackingNormalizedData();
                if (m_nNormalizationRetries == 0)
                {
                    ReportLog("Normalization failed, retrying", false);
                    m_nNormalizationRetries++; //retry once
                    m_objRFEAnalyzer.StartTrackingSequence(true);
                }
                else
                {
                    MessageBox.Show("ERROR: Normalization failed, values lower than -80dBm detected\nMake sure you have selected the correct Analyzer module\nReview your cable setup, connectors,\nthen restart Spectrum Analyzer and try again.", "RF Explorer SNA Tracking");
                }
            }
            else
            {
                MessageBox.Show("Setup is now fully normalized.\nPlease connect cables to DUT.\nClick on [Start SNA] button to start tracking", "RF Explorer SNA Tracking");
            }
        }

        private void OnAnalyzerUpdateTrackingData(object sender, EventArgs e)
        {
#if CALLSTACK_REALTIME
            Console.WriteLine("CALLSTACK:OnAnalyzerUpdateTrackingData");
#endif
            RFESweepData objData = m_objRFEAnalyzer.TrackingData.GetData((uint)m_objRFEAnalyzer.TrackingData.Count - 1);
            ReportLog(DateTime.Now.ToString("HH:mm:ss.fff"), false);
            if (objData != null)
            {
                if (m_chkDebugTraces.Checked)
                {
                    DumpAllReceivedBytes();
                }
            }
        }
        #endregion

        #region RF Explorer Generator Events
        private void OnGeneratorUpdateDataTrakingNormalization(object sender, EventArgs e)
        {
#if CALLSTACK_REALTIME
            Console.WriteLine("CALLSTACK:OnGeneratorUpdateDataTrakingNormalization");
#endif
        }

        private void OnGeneratorReceivedDeviceModel(object sender, EventArgs e)
        {
            if (m_objRFEGenerator.IsAnalyzer())
            {
                ReportLog("Device connected in Signal Generator port is not the right device model, desconnected. Please reconnect in the right port.",false);
                m_objRFEGenerator.ClosePort();
            }
            UpdateButtonStatus();
            DisplayGroups_RFGen();
        }

        private void OnGeneratorReceivedConfigData(object sender, EventArgs e)
        {
#if CALLSTACK_REALTIME
            Console.WriteLine("CALLSTACK:OnGeneratorReceivedConfigData " + m_objRFEGenerator.RFGenPowerON);
#endif
            UpdateButtonStatus();
            DisplayGroups_RFGen();

            UpdateRFGeneratorControlsFromObject(true);
        }

        private void OnGeneratorPortClosed(object sender, EventArgs e)
        {
            UpdateButtonStatus();
            DisplayGroups_RFGen();
        }

        private void OnGeneratorPortConnected(object sender, EventArgs e)
        {
            //UpdateButtonStatus(); too early to know the model, so cannot be validated here
            //DisplayGroups_RFGen();
            SaveProperties(); //to save the last used COM port and speed
        }

        private void OnGeneratorReportLog(object sender, EventArgs e)
        {
            EventReportInfo objArg = (EventReportInfo)e;

            string sLine = "RFEGen: " + objArg.Data;
            ReportLog(sLine, false);
        }
        #endregion
    }
}
