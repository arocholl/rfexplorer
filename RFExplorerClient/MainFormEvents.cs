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

//#define CALLSTACK_REALTIME
//#define CALLSTACK
//#define SUPPORT_EXPERIMENTAL
//#define DEBUG

using System;
using System.Windows.Forms;
using System.Threading;
using RFExplorerCommunicator;
using RFEClientControls;

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
                m_ToolGroup_RFEGenTracking.UpdateButtonStatus(m_objRFEGenerator.RFGenPowerON);
                m_ToolGroup_RFGenCW.UpdateButtonStatus();
                //no actual data to display in analyzer screen when working in tracking mode
                return;
            }

            if (m_objRFEAnalyzer.Mode == RFECommunicator.eMode.MODE_WIFI_ANALYZER)
            {
                //objGraph is a bar chart
                UpdateButtonStatus();
                if (m_ToolGroup_AnalyzerDataFeed.Iterations < 100)
                    m_ToolGroup_AnalyzerDataFeed.Iterations = 100; //at least 100 is required for a practical view in this mode
                m_GraphSpectrumAnalyzer.GraphPane.XAxis.Scale.Min = m_objRFEAnalyzer.StartFrequencyMHZ - 5.0f;
                m_GraphSpectrumAnalyzer.GraphPane.XAxis.Scale.Max = m_objRFEAnalyzer.StartFrequencyMHZ + m_objRFEAnalyzer.FreqSpectrumSteps * m_objRFEAnalyzer.StepFrequencyMHZ + 5.0f;
            }

            AutoLoadAmplitudeDataFile();

            if (m_LimitLineAnalyzer_Overload.Count > 0)
            {
                //potentially offset may change
                m_LimitLineAnalyzer_Overload.NewOffset(m_objRFEAnalyzer.AmplitudeOffsetDB);
            }

            SetupSpectrumAnalyzerAxis();
            SaveProperties();

            if (m_MainTab.SelectedTab == m_tabConfiguration)
            {
                m_MainTab.SelectedTab = m_tabSpectrumAnalyzer;
            }

            UpdateConfigControlContents(m_panelSAConfiguration, m_objRFEAnalyzer);
            DisplayGroups(); //TODO Ariel: separate update of rf connections so no full display groups update is needed

            if (m_ToolGroup_Commands.DebugTraces)
            {
                DumpAllReceivedBytes();
            }
            m_ToolGroupRFEGenFreqSweep.UpdateNumericControls();
            
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
                m_ToolGroup_AnalyzerDataFeed.UpdateNumericControls();
                m_WaterfallSweepMaxHold.Add(m_objRFEAnalyzer.SweepData.MaxHoldData.Duplicate());

                if (m_ToolGroup_Commands.DebugTraces)
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
                m_ToolGroup_RemoteScreen.UpdateNumericControls();
                //Update button status but first time only to minimize overhead
                if (objRFE.ScreenIndex == 1)
                    UpdateButtonStatus();

                if (m_MainTab.SelectedTab == m_tabRemoteScreen)
                    m_tabRemoteScreen.Invalidate();

                if (m_ToolGroup_RemoteScreen.CaptureDumpScreen == false)
                    m_ToolGroup_RemoteScreen.CaptureDumpScreen = true;
            }
        }

        string BinaryToInteger(string sValue)
        {
            string sResult = "";
            UInt64 nValue = 0;
            try
            {
                //Set correct format and trail with zeros
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
                ReportLog(obEx.Message, true);
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
            else if ((sLine.Length > RFECommunicator._DEBUG_StringReport.Length) && (sLine.Substring(0, RFECommunicator._DEBUG_StringReport.Length) == RFECommunicator._DEBUG_StringReport))
            {
                ReportLog(sLine, true);
            }
            else
                ReportLog(sLine, false);

            if (m_ToolGroup_Commands.DebugTraces)
            {//limit debug dump to 15 seconds
                if ((DateTime.Now - m_StartDebugTime) > new TimeSpan(0, 0, 15))
                {
                    m_ToolGroup_Commands.DebugTraces = false;
                }
            }
        }

        private void OnAnalyzerPortClosed(object sender, EventArgs e)
        {
            m_objRFEGenerator.PortNameExternal = null;
            m_ToolGroup_COMPortGenerator.UpdateComboBox();
            m_ToolGroup_RemoteScreen.ResetComponents();
            controlRemoteScreen.RFExplorer = m_ToolGroup_RemoteScreen.GetConnectedDeviceByPrecedence();

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
            m_objRFEGenerator.PortNameExternal = m_objRFEAnalyzer.PortName;
            m_ToolGroup_COMPortGenerator.UpdateComboBox();
            UpdateFeedMode();
            SaveProperties(); //to save the last used COM port and speed
            OnAutoLCDOff_Click(null, null);

            m_ToolGroup_RemoteScreen.ResetComponents();
            controlRemoteScreen.RFExplorer = m_ToolGroup_RemoteScreen.GetConnectedDeviceByPrecedence();
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
                ReportLog("Device connected in Spectrum Analyzer port is not the right device model, disconnected. Reconnecting in the right port.", false);
                m_objRFEAnalyzer.ClosePort();
                if (!m_objRFEGenerator.PortConnected)
                {
                    m_ToolGroup_COMPortGenerator.DefaultCOMPort = m_objRFEAnalyzer.PortName;
                    m_ToolGroup_COMPortGenerator.ConnectPort();
                }
                else 
                {
                    if (false==m_objRFEGenerator.IsGenerator(true) && (m_CounterAnalyzerReceivedDeviceModel == 0))
                    {
                        //T0024 - We check here only case where connected object is not a generator (or even not yet known to be a generator due to OnXXXDeviceModel event not received yet)
                        ReportLog("Device connected in Signal Generator port is not the right device model, disconnected. Reconnecting in the right port.", false);
                        m_objRFEGenerator.ClosePort();
                        m_ToolGroup_COMPortAnalyzer.DefaultCOMPort = m_objRFEGenerator.PortName;
                        m_ToolGroup_COMPortAnalyzer.ConnectPort();
                        m_ToolGroup_COMPortGenerator.ConnectPort();                        
                    }
                }
            }
            else
                m_MainTab.SelectedTab = m_tabSpectrumAnalyzer; //only switch tab if actually a different object

            m_CounterAnalyzerReceivedDeviceModel++;
            DisplayRequiredFirmware();
            AutoLoadAmplitudeDataFile();
        }

        private void OnAnalyzerUpdateDataTrakingNormalization(object sender, EventArgs e)
        {
            //normalization step finished, now ask the user to connect actual device to test
            m_TrackingStatus.IsVisible = false;
            m_GraphTrackingGenerator.Refresh();
            ReportLog("Normalization completed:" + m_objRFEAnalyzer.TrackingNormalizedData.Dump(), false);
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
                if (m_ToolGroup_Commands.DebugTraces)
                {
                    DumpAllReceivedBytes();
                }
            }
        }

        private void OnTraceConfigurationChangeEvent(object sender, EventArgs e)
        {
            ToolGroupTraces objGroupTraces = null;

            try
            {
                objGroupTraces = (ToolGroupTraces)sender;//Dinamic cast- Dinamic cast

                if (objGroupTraces != null)
                {
                    if (objGroupTraces.ActiveControl != null)
                    {
                        if (objGroupTraces.ActiveControl.Text != null)
                        {
                            switch (objGroupTraces.ActiveControl.Text) //ActiveControl.text
                            {
                                case "Fill Trace":
                                    menuSignalFill.Checked = m_ToolGroup_AnalyzerTraces.FillTrace;
                                    OnSignalFill_Click(null, null);
                                    break;
                                case "Smooth":
                                    menuSmoothSignals.Checked = m_ToolGroup_AnalyzerTraces.Smooth;
                                    OnSmoothSignals_Click(null, null);
                                    break;
                                case "Thick Trace":
                                    menuThickTrace.Checked = m_ToolGroup_AnalyzerTraces.ThickTrace;
                                    OnThickTrace_Click(null, null);
                                    break;
                                case "Show Grid":
                                    menuShowGrid.Checked = m_ToolGroup_AnalyzerTraces.ShowGrid;
                                    OnShowGrid_Click(null, null);
                                    break;
                                case "Axis Labels":
                                    menuShowAxisLabels.Checked = m_ToolGroup_AnalyzerTraces.AxisLabels;
                                    OnShowAxisLabels_Click(null, null);
                                    break;
                                case "Realtime":
                                    menuRealtimeTrace.Checked = m_ToolGroup_AnalyzerTraces.Realtime;
                                    click_view_mode(null, null);
                                    break;
                                case "Average":
                                    menuAveragedTrace.Checked = m_ToolGroup_AnalyzerTraces.Average;
                                    click_view_mode(null, null);
                                    break;
                                case "Max Peak":
                                    menuMaxTrace.Checked = m_ToolGroup_AnalyzerTraces.MaxPeak;
                                    click_view_mode(null, null);
                                    break;
                                case "Max Hold":
                                    menuMaxHoldTrace.Checked = m_ToolGroup_AnalyzerTraces.MaxHold;
                                    click_view_mode(null, null);
                                    break;
                                case "Minimum":
                                    menuMinTrace.Checked = m_ToolGroup_AnalyzerTraces.Minimum;
                                    click_view_mode(null, null);
                                    break;
                                default:
                                    string sLog = "Error - Not valid option in ToolGroupTraces " + objGroupTraces.ActiveControl.Text;
                                    ReportLog(sLog, false);
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception obEx)
            {
                ReportLog(obEx.ToString(), false);
            }
        }

        private void OnAnalyzerMode(object sender, EventArgs e)
        {

        }
        #endregion

        #region Markers

        private void OnMarkerChanged_SNA(object sender, EventArgs e)
        {

        }
        private void OnMarkerChanged_SA(object sender, EventArgs e)
        {

        }

        private void OnMarkerValueChanged_SNA(object sender, EventArgs e)
        {
            try
            {
                UpdateMenuFromMarkerCollection(true);
                UpdateSNAMarkerControlContents();
                DisplayTrackingData();
                m_ToolGroup_Markers_SNA.UpdateButtonStatus();
            }
            catch (Exception obEx)
            {
                ReportLog(obEx.ToString(), false);
            }
        }

        private void OnMarkerValueChanged_SA(object sender, EventArgs e)
        {
            try
            {
                UpdateMenuFromMarkerCollection(false);

                UpdateSAMarkerControlContents();
                DisplaySpectrumAnalyzerData();
                m_ToolGroup_Markers_SA.UpdateButtonStatus();
            }
            catch (Exception obEx)
            {
                ReportLog(obEx.ToString(), false);
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
                ReportLog("Device connected in Signal Generator port is not the right device model, disconnected. Reconnecting in the right port.", false);
                m_objRFEGenerator.ClosePort();
               if (!m_objRFEAnalyzer.PortConnected)
                {
                    m_ToolGroup_COMPortAnalyzer.DefaultCOMPort = m_objRFEGenerator.PortName;
                    m_ToolGroup_COMPortAnalyzer.ConnectPort();                    
                }
                else
                {
                    if (false == m_objRFEAnalyzer.IsAnalyzer(true))
                    {
                        //T0024 - We check here only case where connected object is not an analyzer (or even not yet known to be an analyzer due to OnXXXDeviceModel event not received yet)
                        ReportLog("Device connected in Spectrum Analyzer port is not the right device model, disconnected. Reconnecting in the right port.", false);
                        m_objRFEAnalyzer.ClosePort();
                        m_ToolGroup_COMPortGenerator.DefaultCOMPort = m_objRFEAnalyzer.PortName;
                        m_ToolGroup_COMPortGenerator.ConnectPort();
                        m_ToolGroup_COMPortAnalyzer.ConnectPort();
                    }
                }
            }
            else //try to connect analyzer if not connected yet
            {
                if ((m_objRFEAnalyzer.ValidCP2101Ports.Length == 2) && (!m_objRFEAnalyzer.PortConnected))
                {
                    m_ToolGroup_COMPortAnalyzer.ConnectPort();
                }
                else
                    m_MainTab.SelectedTab = m_tabRFGen;
            }

            UpdateButtonStatus();
            DisplayGroups_RFGen();
        }

        private void OnGeneratorUpdateCalibration(object sender, EventArgs e)
        {
            m_ToolGroup_RFGenCW.UpdatePowerLevels();
        }

        private void OnGeneratorReceivedConfigData(object sender, EventArgs e)
        {
#if CALLSTACK_REALTIME
            Console.WriteLine("CALLSTACK:OnGeneratorReceivedConfigData " + m_objRFEGenerator.RFGenPowerON);
#endif
            UpdateButtonStatus();
            DisplayGroups_RFGen();
            
            m_ToolGroupRFEGenFreqSweep.UpdateRFGeneratorControlsFromObject(true);
            m_ToolGroup_RFGenCW.UpdateRFGeneratorControlsFromObject();
            m_ToolGroupRFEGenFreqSweep.UpdateNumericControls();
            m_ToolGroupRFEGenAmplSweep.UpdateRFGeneratorControlsFromObject();
        }

        private void OnGeneratorPortClosed(object sender, EventArgs e)
        {
            m_objRFEAnalyzer.PortNameExternal = null;
            m_ToolGroup_COMPortAnalyzer.UpdateComboBox();
            UpdateButtonStatus();
            DisplayGroups_RFGen();

            m_ToolGroup_RemoteScreen.ResetComponents();
            controlRemoteScreen.RFExplorer = m_ToolGroup_RemoteScreen.GetConnectedDeviceByPrecedence();
        }

        private void OnGeneratorPortConnected(object sender, EventArgs e)
        {
            m_objRFEAnalyzer.PortNameExternal = m_objRFEGenerator.PortName;
            m_ToolGroup_COMPortAnalyzer.GetConnectedPorts();//NullPointer ValidCP2102
            m_ToolGroup_COMPortAnalyzer.UpdateComboBox();
            //UpdateButtonStatus(); too early to know the model, so cannot be validated here
            //DisplayGroups_RFGen();
            SaveProperties(); //to save the last used COM port and speed

            controlRemoteScreen.RFExplorer = m_ToolGroup_RemoteScreen.GetConnectedDeviceByPrecedence();
        }

        private void OnGeneratorReportLog(object sender, EventArgs e)
        {
            EventReportInfo objArg = (EventReportInfo)e;

            string sLine = objArg.Data;
            bool bDebugLog = (sLine.Length > RFECommunicator._DEBUG_StringReport.Length) && (sLine.Substring(0, RFECommunicator._DEBUG_StringReport.Length) == RFECommunicator._DEBUG_StringReport);
            sLine = "RFEGen: " + objArg.Data;
            ReportLog(sLine, bDebugLog);
        }

        private void OnGeneratorFrequencyChanged(object sender, EventArgs e)
        {
            m_ToolGroup_RFGenCW.SetFrequency();
            m_ToolGroupRFEGenAmplSweep.UpdatePowerLevels();
        }

        private void OnGeneratorPowerChange(object sender, EventArgs e)
        {
            if ((m_objRFEAnalyzer != null) && (m_objRFEGenerator != null) && (m_objRFEAnalyzer.IsTrackingNormalized()))
            {
                m_objRFEAnalyzer.ResetTrackingNormalizedData();
                DisplayGroups_RFGen();
            }
        }

        private void OnGeneratorStartStopClick(object sender, EventArgs e)
        {
            ToolGroupRFGenCW objGroupGen = null;

            try
            {
                objGroupGen = (ToolGroupRFGenCW)sender;//Dynamic cast

                if ((objGroupGen != null) && (objGroupGen.ActiveControl != null))
                {
                    if (objGroupGen.ActiveControl.Text.StartsWith("Start"))
                    {
                        CleanSweepData();
                        ClearTrackingGraph();
                        HideNormalizatingMessage();

                        m_objRFEGenerator.SendCommand_GeneratorCW();
                        DisplayTrackingData_TextProgress(true);

                        ReportLog("CW at " + m_objRFEGenerator.RFGenStartFrequencyMHZ.ToString("f3") + "MHZ with estimated amplitude: " + m_objRFEGenerator.GetSignalGeneratorEstimatedAmplitude(m_objRFEGenerator.RFGenStartFrequencyMHZ) + "dBm", false);

                        Thread.Sleep(500);
                        Application.DoEvents();
                        DisplayGroups_RFGen();
                    }
                    else if (objGroupGen.ActiveControl.Text == "Stop")
                    {
                        HideNormalizatingMessage();
                        DisplayGroups_RFGen();
                        DisplayTrackingData_TextProgress(true);
                    }
                }
            }
            catch (Exception obEx)
            {
                ReportLog(obEx.ToString(), false);
            }
        }

        private void OnToolGroupRFGenReportInfoChanged(object sender, EventArgs e)
        {
            EventReportInfo objArg = (EventReportInfo)e;
            string sLine = objArg.Data;
            ReportLog(sLine, false);
        }
        #endregion

    }
}
