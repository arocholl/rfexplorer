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
    public partial class MainForm : Form
    {
        #region RFGen
        private System.Windows.Forms.TabPage m_tabRFGen;

        private void InitializeRFGenTab()
        {
            this.m_tabRFGen = new System.Windows.Forms.TabPage();
            this.m_tabRFGen.SuspendLayout();
            this.m_MainTab.Controls.Add(this.m_tabRFGen);

            // 
            // m_tabRFGen
            // 
            //this.m_tabRFGen.Controls.Add(this.m_groupControl_Commands);
            //this.m_tabRFGen.Controls.Add(this.m_ReportTextBox);
            this.m_tabRFGen.Location = new System.Drawing.Point(4, 26);
            this.m_tabRFGen.Name = "m_tabRFGen";
            this.m_tabRFGen.Padding = new System.Windows.Forms.Padding(3);
            this.m_tabRFGen.Size = new System.Drawing.Size(1084, 510);
            this.m_tabRFGen.TabIndex = 1;
            this.m_tabRFGen.Text = "Signal Generator [BETA]";
            this.m_tabRFGen.UseVisualStyleBackColor = true;
            this.m_tabRFGen.Enter += new System.EventHandler(this.OnTabRFGen_Enter);

            m_GraphTrackingGenerator = new ZedGraph.ZedGraphControl();
            m_tabRFGen.Controls.Add(m_GraphTrackingGenerator);

            InitializeTrackingGeneratorGraph();
        }

        private void OnTabRFGen_Enter(object sender, EventArgs e)
        {
            DisplayGroups();
        }

        private void SetRFGenPower()
        {
            switch (m_comboRFGenPowerCW.SelectedIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    m_objRFEGenerator.RFGenHighPowerSwitch = false;
                    m_objRFEGenerator.RFGenPowerLevel = Convert.ToByte(m_comboRFGenPowerCW.SelectedIndex);
                    break;

                case 4: 
                case 5: 
                case 6: 
                case 7:
                    m_objRFEGenerator.RFGenHighPowerSwitch = true;
                    m_objRFEGenerator.RFGenPowerLevel = Convert.ToByte(m_comboRFGenPowerCW.SelectedIndex - 4);
                    break;
            }
        }

        private void UpdateButtonStatus_RFGen()
        {
            bool bTransmitting = m_bRFGenTransmittingSweep || m_objRFEGenerator.RFGenPowerON;

            if (m_objRFEGenerator.RFGenPowerON)
            {
                m_sRFPowerON.Text = "RF Power ON";
                m_sRFPowerON.ForeColor = Color.Red;
                m_StatusGraphText_Tracking.FontSpec.FontColor = Color.DarkRed;
            }
            else
            {
                m_sRFPowerON.Text = "RF Power OFF";
                m_sRFPowerON.ForeColor = btnStartFreqSweep1.ForeColor;
                m_StatusGraphText_Tracking.FontSpec.FontColor = Color.DarkGray;
            }

            btnStartFreqSweep1.Enabled = !bTransmitting;
            btnStartFreqSweepContinuous.Enabled = !bTransmitting;
            btnStopFreqSweep.Enabled = m_bRFGenTransmittingSweep;
            btnRFEGenCWStart.Enabled = !bTransmitting;
            btnRFEGenCWStop.Enabled = m_objRFEGenerator.RFGenPowerON && !m_bRFGenTransmittingSweep && (m_objRFEAnalyzer.Mode!=RFECommunicator.eMode.MODE_TRACKING);

            btnNormalizeTracking.Enabled = !bTransmitting;
            btnTrackingStart.Enabled = !bTransmitting && m_objRFEAnalyzer.IsTrackingNormalized();
            btnTrackingStop.Enabled = ((m_objRFEAnalyzer.Mode == RFECommunicator.eMode.MODE_TRACKING) && m_objRFEGenerator.RFGenPowerON);
        }

        private void DisplayGroups_RFGen()
        {
            bool bRFGenConnected = m_objRFEGenerator.PortConnected && m_objRFEGenerator.IsGenerator();

            m_groupControl_RFEGen_Tracking.Enabled = bRFGenConnected && m_objRFEAnalyzer.PortConnected;
            m_groupControl_RFEGen_FrequencySweep.Enabled = bRFGenConnected && !m_objRFEGenerator.RFGenPowerON;
            m_groupControl_RFEGen_CW.Enabled = bRFGenConnected;

            if (m_groupControl_RFEGen_CW.Enabled)
            {
                m_sRFGenFreqCW.Enabled = !m_objRFEGenerator.RFGenPowerON;
                m_comboRFGenPowerCW.Enabled = !m_objRFEGenerator.RFGenPowerON;
            }

            ChangeTextBoxColor(m_sRFGenFreqSweepStart);
            ChangeTextBoxColor(m_sRFGenFreqSweepStop);
            ChangeTextBoxColor(m_sRFGenFreqCW);
            ChangeTextBoxColor(m_sRefFrequency);
            ChangeTextBoxColor(m_sRFGenFreqSweepSteps);

            UpdateButtonStatus_RFGen();

            UpdateTrackingStatusText();
        }

        bool m_bRFGenTransmittingSweep = false;
        private void OnStartFreqSweep1_Click(object sender, EventArgs e)
        {
            if (DialogResult.Cancel == MessageBox.Show("Connect Signal Generator output to a load (50 ohm)", "RF Explorer Signal Generator", MessageBoxButtons.OKCancel))
                return;

            CleanSweepData();
            ClearTrackingGraph();
            HideNormalizatingMessage();

            m_objRFEGenerator.RFGenStartFrequencyMHZ = Convert.ToDouble(m_sRFGenFreqSweepStart.Text);
            m_objRFEGenerator.RFGenStopFrequencyMHZ = Convert.ToDouble(m_sRFGenFreqSweepStop.Text);
            m_objRFEGenerator.RFGenSweepSteps = Convert.ToUInt16(m_sRFGenFreqSweepSteps.Text);
            SetRFGenPower();

            m_objRFEGenerator.SendCommand_GeneratorCW();

            DisplayTrackingData_TextProgress(true);

            m_bRFGenTransmittingSweep = true;
            DisplayGroups_RFGen();
            Thread.Sleep(500);
            Application.DoEvents();

            Cursor.Current = Cursors.WaitCursor;

            for (UInt16 nInd = 0; m_bRFGenTransmittingSweep && (nInd < m_objRFEGenerator.RFGenSweepSteps); nInd++)
            {
                m_objRFEGenerator.SendCommand_TrackingStep(nInd);
                Application.DoEvents();
                Thread.Sleep(100);
            }

            m_objRFEGenerator.SendCommand_GeneratorRFPowerOFF();
            m_bRFGenTransmittingSweep = false;
            DisplayGroups_RFGen();
            Cursor.Current = Cursors.Default;
        }

        private void OnStartFreqSweepContinuous_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Under development");
        }

        private void OnStopFreqSweep_Click(object sender, EventArgs e)
        {
            m_bRFGenTransmittingSweep = false;
            m_objRFEGenerator.SendCommand_GeneratorRFPowerOFF();
            HideNormalizatingMessage();
            DisplayGroups_RFGen();
            DisplayTrackingData_TextProgress(true);
        }

        private void OnRFEGenCWStart_Click(object sender, EventArgs e)
        {
            if (DialogResult.Cancel == MessageBox.Show("Connect Signal Generator output to a load (50 ohm)", "RF Explorer Signal Generator", MessageBoxButtons.OKCancel))
                return;

            double dFreqMHZ = Convert.ToDouble(m_sRFGenFreqCW.Text);

            CleanSweepData();
            ClearTrackingGraph();
            HideNormalizatingMessage();

            m_objRFEGenerator.RFGenStartFrequencyMHZ = Convert.ToDouble(m_sRFGenFreqCW.Text);
            m_objRFEGenerator.RFGenStopFrequencyMHZ = Convert.ToDouble(m_sRFGenFreqCW.Text) + 1.0; //any frequency would do
            m_objRFEGenerator.RFGenSweepSteps = Convert.ToUInt16(m_sRFGenFreqSweepSteps.Text);
            SetRFGenPower();

            m_objRFEGenerator.SendCommand_GeneratorCW();
            DisplayTrackingData_TextProgress(true);

            ReportLog("CW at " + dFreqMHZ.ToString("f3") + "MHZ with estimated amplitude: " + m_objRFEGenerator.GetSignalGeneratorEstimatedAmplitude(dFreqMHZ) + "dBm",false);

            Thread.Sleep(500);
            Application.DoEvents();
            DisplayGroups_RFGen();
        }

        private void OnRFEGenCWStop_Click(object sender, EventArgs e)
        {
            m_objRFEGenerator.SendCommand_GeneratorRFPowerOFF();
            HideNormalizatingMessage();
            DisplayGroups_RFGen();
            DisplayTrackingData_TextProgress(true);
        }

        private void OnFreqGenerator_Leave(object sender, EventArgs e)
        {
            double dFreqMHZ = Convert.ToDouble(m_sRFGenFreqCW.Text);
            if (dFreqMHZ < 23.438)
                m_sRFGenFreqCW.Text = "23.438";
            if (dFreqMHZ>6000)
                m_sRFGenFreqCW.Text = "6000.000";
        }

        private void ValidateRFGenSweepFreqRanges(bool bResetNormalizeData)
        {
            if (m_objRFEGenerator == null || !m_objRFEGenerator.PortConnected)
                return;

            int nSteps = Convert.ToInt32(m_sRFGenFreqSweepSteps.Text);
            double dStartMHZ = Convert.ToDouble(m_sRFGenFreqSweepStart.Text);
            double dStopMHZ = Convert.ToDouble(m_sRFGenFreqSweepStop.Text);
            if (dStartMHZ >= dStopMHZ)
                dStopMHZ = dStartMHZ + (nSteps / 1000.0);
            if (dStartMHZ < m_objRFEGenerator.MinFreqMHZ)
                dStartMHZ = m_objRFEGenerator.MinFreqMHZ;
            if (dStopMHZ > m_objRFEGenerator.MaxFreqMHZ)
                dStopMHZ = m_objRFEGenerator.MaxFreqMHZ;
            if (m_objRFEAnalyzer.PortConnected)
            {
                if (dStartMHZ < m_objRFEAnalyzer.MinFreqMHZ)
                    dStartMHZ = m_objRFEAnalyzer.MinFreqMHZ;
                if (dStopMHZ > m_objRFEAnalyzer.MaxFreqMHZ)
                    dStopMHZ = m_objRFEAnalyzer.MaxFreqMHZ;
            }
            m_sRFGenFreqSweepStop.Text = dStopMHZ.ToString("f3");
            m_sRFGenFreqSweepStart.Text = dStartMHZ.ToString("f3");

            if (bResetNormalizeData && (m_objRFEAnalyzer != null) && (m_objRFEAnalyzer.PortConnected))
            {
                //if previous setup was normalized, reset as it will no longer be valid
                if (m_objRFEAnalyzer.IsTrackingNormalized())
                {
                    if ((m_objRFEGenerator.RFGenSweepSteps != nSteps) ||
                        (m_objRFEGenerator.RFGenStartFrequencyMHZ!=dStartMHZ) ||
                        (m_objRFEGenerator.RFGenStopFrequencyMHZ!=dStopMHZ)
                       )
                    {
                        m_objRFEAnalyzer.ResetTrackingNormalizedData();
                        DisplayGroups_RFGen();
                    }
                }
            }
        }

        private void OnRFGenFreqSweepStart_Leave(object sender, EventArgs e)
        {
            ValidateRFGenSweepFreqRanges(true);
        }

        private void OnRFGenFreqSweepStop_Leave(object sender, EventArgs e)
        {
            ValidateRFGenSweepFreqRanges(true);
        }

        private void OnRFGenFreqSweepSteps_Leave(object sender, EventArgs e)
        {
            int nSteps = Convert.ToInt32(m_sRFGenFreqSweepSteps.Text);
            if (nSteps < 2)
                m_sRFGenFreqSweepSteps.Text = "2";
            else if (nSteps>9999)
                m_sRFGenFreqSweepSteps.Text = "9999";
            ValidateRFGenSweepFreqRanges(true);
        }

        private void OnRFGenFreqSweepStart_TextChanged(object sender, EventArgs e)
        {
        }

        private void OnRFGenFreqSweepStop_TextChanged(object sender, EventArgs e)
        {
        }

        private void OnRFGenFreqSweepSteps_TextChanged(object sender, EventArgs e)
        {
        }

        private void OnRFGenFreqCW_TextChanged(object sender, EventArgs e)
        {
        }

        private void OnRFGenPowerCW_SelectedValueChanged(object sender, EventArgs e)
        {
            if ((m_objRFEAnalyzer != null) && (m_objRFEGenerator != null))
            {
                if (m_objRFEAnalyzer.IsTrackingNormalized())
                {
                    m_objRFEAnalyzer.ResetTrackingNormalizedData();
                    DisplayGroups_RFGen();
                }
            }
        }

        private void InitializeTrackingGeneratorGraph()
        {
#if CALLSTACK
            Console.WriteLine("CALLSTACK:InitializeTrackingGeneratorGraph");
#endif

            m_GraphTrackingGenerator.IsAutoScrollRange = true;
            m_GraphTrackingGenerator.EditButtons = System.Windows.Forms.MouseButtons.Left;
            m_GraphTrackingGenerator.IsAntiAlias = true;
            m_GraphTrackingGenerator.IsEnableSelection = true;
            m_GraphTrackingGenerator.Location = new System.Drawing.Point(8, 257);
            m_GraphTrackingGenerator.Name = "zedTracking";
            m_GraphTrackingGenerator.PanModifierKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)));
            m_GraphTrackingGenerator.ScrollGrace = 0D;
            m_GraphTrackingGenerator.ScrollMaxX = 0D;
            m_GraphTrackingGenerator.ScrollMaxY = 0D;
            m_GraphTrackingGenerator.ScrollMaxY2 = 0D;
            m_GraphTrackingGenerator.ScrollMinX = 0D;
            m_GraphTrackingGenerator.ScrollMinY = 0D;
            m_GraphTrackingGenerator.ScrollMinY2 = 0D;
            m_GraphTrackingGenerator.Size = new System.Drawing.Size(123, 54);
            m_GraphTrackingGenerator.TabIndex = 49;
            m_GraphTrackingGenerator.TabStop = false;
            m_GraphTrackingGenerator.UseExtendedPrintDialog = true;
            m_GraphTrackingGenerator.Visible = true;
            m_GraphTrackingGenerator.ContextMenuBuilder += new ZedGraph.ZedGraphControl.ContextMenuBuilderEventHandler(this.objGraph_ContextMenuBuilder);
            //m_graphTrackingGenerator.ZoomEvent += new ZedGraph.ZedGraphControl.ZoomEventHandler(this.zedSpectrumAnalyzer_ZoomEvent);

            // Get a reference to the GraphPane instance in the ZedGraphControl
            GraphPane myPane = m_GraphTrackingGenerator.GraphPane;
            myPane.Fill = new Fill(Color.White, Color.LightBlue, 90.0f);
            myPane.Chart.Fill = new Fill(Color.White, Color.LightBlue, 90.0f);

            m_PointList_Tracking_Normal = new PointPairList();
            m_GraphLine_Tracking_Normal = m_GraphTrackingGenerator.GraphPane.AddCurve("Realtime", m_PointList_Tracking_Normal, Color.Blue, SymbolType.None);
            m_GraphLine_Tracking_Normal.Line.Width = 1;
            m_GraphLine_Tracking_Normal.Line.SmoothTension = 0.2F;

            m_PointList_Tracking_Avg = new PointPairList();
            m_GraphLine_Tracking_Avg = m_GraphTrackingGenerator.GraphPane.AddCurve("Average", m_PointList_Tracking_Avg, Color.DarkRed, SymbolType.None);
            m_GraphLine_Tracking_Avg.Line.Width = 4;
            m_GraphLine_Tracking_Avg.Line.SmoothTension = 0.3F;

            foreach (CurveItem objCurve in myPane.CurveList)
            {
                objCurve.IsVisible = false;
                objCurve.Label.IsVisible = false;
            }

            // Set the titles and axis labels
            //myPane.Title.FontSpec.Size = 10;
            myPane.XAxis.Title.IsVisible = true;
            myPane.XAxis.Title.Text = "Frequency (MHZ)";
            myPane.XAxis.Scale.MajorStepAuto = true;
            myPane.XAxis.Scale.MinorStepAuto = true;
            myPane.XAxis.Type = AxisType.Linear;
            myPane.Margin.Left = 20;
            myPane.Margin.Right = -5;


            m_GraphTrackingGenerator.IsShowPointValues = true;

            // Show the x axis grid
            myPane.XAxis.MajorGrid.IsVisible = true;
            myPane.XAxis.Type = AxisType.Linear;

            myPane.YAxis.Title.IsVisible = true;
            // turn off the opposite tics so the Y tics don't show up on the Y2 axis
            myPane.YAxis.MajorTic.IsOpposite = false;
            myPane.YAxis.MinorTic.IsOpposite = false;
            myPane.YAxis.MajorGrid.IsVisible = true;
            // Don't display the Y zero line
            myPane.YAxis.MajorGrid.IsZeroLine = false;
            // Align the Y axis labels so they are flush to the axis
            myPane.YAxis.Scale.Align = AlignP.Inside;
            myPane.YAxis.Title.Text = "Insertion Loss (dB)";

            myPane.Title.Text = _RFEGEN_TRACKING_TITLE;

            m_GraphTrackingGenerator.IsShowPointValues = true;
            m_GraphTrackingGenerator.PointValueEvent += new ZedGraphControl.PointValueHandler(GraphPointValueHandler);

            m_StatusGraphText_Tracking = new TextObj("Signal Generator DISCONNECTED", 0.01, 0.02, CoordType.ChartFraction);
            m_StatusGraphText_Tracking.IsClippedToChartRect = true;
            m_StatusGraphText_Tracking.FontSpec.FontColor = Color.DarkGray;
            m_StatusGraphText_Tracking.Location.AlignH = AlignH.Left;
            m_StatusGraphText_Tracking.Location.AlignV = AlignV.Top;
            m_StatusGraphText_Tracking.FontSpec.IsBold = false;
            m_StatusGraphText_Tracking.FontSpec.Size = 10f;
            m_StatusGraphText_Tracking.FontSpec.Border.IsVisible = false;
            m_StatusGraphText_Tracking.FontSpec.Fill.IsVisible = false;
            m_StatusGraphText_Tracking.FontSpec.StringAlignment = StringAlignment.Near;
            m_StatusGraphText_Tracking.FontSpec.IsDropShadow = true;
            m_StatusGraphText_Tracking.FontSpec.DropShadowOffset = 0.1f;
            m_StatusGraphText_Tracking.FontSpec.DropShadowColor = Color.LightBlue;
            m_StatusGraphText_Tracking.FontSpec.Family = "Tahoma";
            myPane.GraphObjList.Add(m_StatusGraphText_Tracking);

            m_TrackingStatus = new TextObj("Tracking Normalization in Progress, \nplease wait...", 0.5, 0.5, CoordType.ChartFraction);
            m_TrackingStatus.Location.AlignH = AlignH.Center;
            m_TrackingStatus.Location.AlignV = AlignV.Center;
            m_TrackingStatus.FontSpec.Size = 20;
            m_TrackingStatus.FontSpec.FontColor = Color.DarkRed;
            m_TrackingStatus.FontSpec.IsDropShadow = true;
            m_TrackingStatus.FontSpec.DropShadowOffset = 0.05f;
            m_TrackingStatus.IsVisible = false;
            myPane.GraphObjList.Add(m_TrackingStatus);

            m_TrackingProgressText = new TextObj("Tracking step: 0/0 0%", 0.01, 0.08, CoordType.ChartFraction);
            m_TrackingProgressText.IsClippedToChartRect = true;
            m_TrackingProgressText.FontSpec.FontColor = Color.DarkBlue;
            m_TrackingProgressText.Location.AlignH = AlignH.Left;
            m_TrackingProgressText.Location.AlignV = AlignV.Top;
            m_TrackingProgressText.FontSpec.IsBold = false;
            m_TrackingProgressText.FontSpec.Size = 8f;
            m_TrackingProgressText.FontSpec.Border.IsVisible = false;
            m_TrackingProgressText.FontSpec.Fill.IsVisible = false;
            m_TrackingProgressText.FontSpec.StringAlignment = StringAlignment.Near;
            m_TrackingProgressText.FontSpec.IsDropShadow = true;
            m_TrackingProgressText.FontSpec.DropShadowColor = Color.LightBlue;
            m_TrackingProgressText.FontSpec.DropShadowOffset = 0.1f;
            m_TrackingProgressText.FontSpec.Family = "Tahoma";
            m_TrackingProgressText.IsVisible = false;
            myPane.GraphObjList.Add(m_TrackingProgressText);

            //DefineGraphColors();

            this.m_tabRFGen.ResumeLayout(false);
            this.m_tabRFGen.PerformLayout();

            m_comboRFGenPowerCW.SelectedIndex = 0;
        }

        private void OnGeneratorButtons_PortConnected(object sender, EventArgs e)
        {
            //nothing to do, already handled in OnRFE_PortConnected
        }

        private void OnGeneratorButtons_PortClosed(object sender, EventArgs e)
        {
            //nothing to do, already handled in OnRFE_PortClosed
        }

        UInt16 m_nNormalizationRetries = 0; //used to retry once if normalization fails
        private void OnNormalizeTrackingStart_Click(object sender, EventArgs e)
        {
            if (DialogResult.Cancel == MessageBox.Show("Connect cables without DUT to normalize SNA setup response", "RF Explorer SNA Tracking", MessageBoxButtons.OKCancel))
                return;

            m_nNormalizationRetries = 0;
            Cursor.Current = Cursors.WaitCursor;

            CleanSweepData();
            ClearTrackingGraph();
            ShowNormalizatingMessage();

            m_objRFEAnalyzer.TrackingRFEGen = m_objRFEGenerator;
            //note: for normalization we always use the value, regardless the checkbox used for tracking
            m_objRFEAnalyzer.AutoStopSNATrackingCounter = Convert.ToUInt16(m_nRFGENIterationAverage.Value);

            m_objRFEGenerator.RFGenStartFrequencyMHZ = Convert.ToDouble(m_sRFGenFreqSweepStart.Text);
            m_objRFEGenerator.RFGenStopFrequencyMHZ = Convert.ToDouble(m_sRFGenFreqSweepStop.Text);
            m_objRFEGenerator.RFGenSweepSteps = Convert.ToUInt16(m_sRFGenFreqSweepSteps.Text);
            SetRFGenPower();

            m_objRFEAnalyzer.StartTrackingSequence(true);
            UpdateButtonStatus();

            Cursor.Current = Cursors.Default;
        }

        private void ClearTrackingGraph()
        {
            m_objRFEAnalyzer.TrackingData.CleanAll();
            m_GraphLine_Tracking_Normal.Clear();
            m_PointList_Tracking_Normal.Clear();
            m_GraphLine_Tracking_Avg.Clear();
            m_PointList_Tracking_Avg.Clear();
            m_GraphTrackingGenerator.Refresh();
        }

        /// <summary>
        /// updates all group control values from m_objRFEGenerator object
        /// </summary>
        private void UpdateRFGeneratorControlsFromObject(bool bResetNormalizationData)
        {
            m_sRFGenFreqSweepStart.Text = m_objRFEGenerator.RFGenStartFrequencyMHZ.ToString("f3");
            m_sRFGenFreqSweepStop.Text = m_objRFEGenerator.RFGenStopFrequencyMHZ.ToString("f3");
            m_sRFGenFreqSweepSteps.Text = m_objRFEGenerator.RFGenSweepSteps.ToString();
            m_sRFGenFreqCW.Text = m_objRFEGenerator.RFGenCWFrequencyMHZ.ToString("f3");

            ValidateRFGenSweepFreqRanges(bResetNormalizationData);

            if (m_objRFEGenerator.RFGenHighPowerSwitch)
            {
                m_comboRFGenPowerCW.SelectedIndex = 4 + m_objRFEGenerator.RFGenPowerLevel;
            }
            else
            {
                m_comboRFGenPowerCW.SelectedIndex = m_objRFEGenerator.RFGenPowerLevel;
            }
        }

        private void OnTrackingStart_Click(object sender, EventArgs e)
        {
            if (DialogResult.Cancel == MessageBox.Show("Connect cables to DUT to start tracking", "RF Explorer SNA Tracking", MessageBoxButtons.OKCancel))
                return;

            Cursor.Current = Cursors.WaitCursor;

            ClearTrackingGraph();
            CleanSweepData();
            HideNormalizatingMessage();

            m_objRFEAnalyzer.TrackingRFEGen = m_objRFEGenerator;
            if (chkSNAAutoStop.Checked)
                m_objRFEAnalyzer.AutoStopSNATrackingCounter = Convert.ToUInt16(m_nRFGENIterationAverage.Value);
            else
                m_objRFEAnalyzer.AutoStopSNATrackingCounter = 0;

            m_objRFEGenerator.RFGenStartFrequencyMHZ = Convert.ToDouble(m_sRFGenFreqSweepStart.Text);
            m_objRFEGenerator.RFGenStopFrequencyMHZ = Convert.ToDouble(m_sRFGenFreqSweepStop.Text);
            m_objRFEGenerator.RFGenSweepSteps = Convert.ToUInt16(m_sRFGenFreqSweepSteps.Text);
            SetRFGenPower();

            m_objRFEAnalyzer.StartTrackingSequence(false);
            Application.DoEvents();
            UpdateButtonStatus();

            Cursor.Current = Cursors.Default;
        }

        private void OnTrackingStop_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            HideNormalizatingMessage();
            m_objRFEAnalyzer.StopTracking();
            UpdateButtonStatus_RFGen();
            //DisplayTrackingData_TextProgress(true);
            m_TrackingProgressText.IsVisible = false;
            Cursor.Current = Cursors.Default;
        }

        private void UpdateTrackingStatusText()
        {
            if (!m_objRFEGenerator.PortConnected)
            {
                m_StatusGraphText_Tracking.Text = "Signal Generator DISCONNECTED";
                m_StatusGraphText_Tracking.FontSpec.FontColor = Color.DarkGray;
                m_StatusGraphText_Tracking.FontSpec.IsBold = false;
            }
            else
            {
                if (m_objRFEGenerator.RFGenPowerON)
                {
                    m_StatusGraphText_Tracking.Text = "RF POWER ON";
                    m_StatusGraphText_Tracking.FontSpec.FontColor = Color.Red;
                    m_StatusGraphText_Tracking.FontSpec.IsBold = true;
                }
                else
                {
                    m_StatusGraphText_Tracking.Text = "Signal Generator IDLE";
                    m_StatusGraphText_Tracking.FontSpec.FontColor = Color.DarkBlue;
                    m_StatusGraphText_Tracking.FontSpec.IsBold = false;
                }
            }
            m_GraphTrackingGenerator.Invalidate();
        }

        private void HideNormalizatingMessage()
        {
            m_TrackingStatus.IsVisible = false;
            m_GraphTrackingGenerator.Refresh();
        }

        private void ShowNormalizatingMessage()
        {
            m_TrackingStatus.IsVisible = true;
            m_GraphTrackingGenerator.Refresh();
        }

        private void DisplayTrackingData_TextProgress(bool bRefresh)
        {
            if (m_objRFEAnalyzer.Mode == RFECommunicator.eMode.MODE_TRACKING)
            {
                if (!m_TrackingProgressText.IsVisible)
                    m_TrackingProgressText.IsVisible = true;

                string sTrackingStep = "Tracking step (#" + m_objRFEAnalyzer.TrackingData.Count + "): " + m_objRFEAnalyzer.RFGenTrackingCurrentStep + "/" + m_objRFEGenerator.RFGenSweepSteps;
                string sStepPercentage = (100.0 * m_objRFEAnalyzer.RFGenTrackingCurrentStep / m_objRFEGenerator.RFGenSweepSteps).ToString("f0") + "%";
                m_TrackingProgressText.Text = sTrackingStep + " " + sStepPercentage;
                if (m_objRFEAnalyzer.IsTrackingNormalizing)
                {
                    m_TrackingProgressText.Text += "\nNormalization progress (#" + m_objRFEAnalyzer.TrackingNormalizingPass + "): " +
                        (100.0 * (m_objRFEAnalyzer.RFGenTrackingCurrentStep + m_objRFEAnalyzer.TrackingNormalizingPass * m_objRFEGenerator.RFGenSweepSteps) /
                         (m_objRFEGenerator.RFGenSweepSteps * RFECommunicator.NORMALIZING_AVG_PASSES)).ToString("f0") + "%"; ;
                }
            }
            else
            {
                if (m_TrackingProgressText.IsVisible)
                    m_TrackingProgressText.IsVisible = false;

            }
            if (bRefresh)
                m_GraphTrackingGenerator.Refresh();
        }

        private void DisplayTrackingData()
        {
#if CALLSTACK_REALTIME
            Console.WriteLine("CALLSTACK:DisplayTrackingData");
#endif
            m_PointList_Tracking_Normal.Clear();
            m_GraphLine_Tracking_Normal.Clear();
            m_PointList_Tracking_Avg.Clear();
            m_GraphLine_Tracking_Avg.Clear();

            DisplayTrackingData_TextProgress(false);

            if (m_objRFEAnalyzer.IsTrackingNormalizing != m_TrackingStatus.IsVisible)
            {
                m_TrackingStatus.IsVisible = m_objRFEAnalyzer.IsTrackingNormalizing;
                m_GraphTrackingGenerator.Refresh();
            }

            if (m_objRFEAnalyzer.TrackingData.Count == 0)
            {
                m_GraphTrackingGenerator.Refresh();
                return; //nothing to paint
            }

            //Use the latest data loaded
            uint nLastSample=m_objRFEAnalyzer.TrackingData.Count-1;
            uint nAvailableAverageSamples=1;
            uint nAverageIterations=Convert.ToUInt16(m_nRFGENIterationAverage.Value);
            if (nAverageIterations == 0)
                nAverageIterations = 1;
            if (nLastSample>=(nAverageIterations-1))
                nAvailableAverageSamples=nAverageIterations;
            else
                nAvailableAverageSamples=nLastSample+1;

            RFESweepData objSweep = m_objRFEAnalyzer.TrackingData.GetData(nLastSample);
            RFESweepData objSweepAvg = m_objRFEAnalyzer.TrackingData.GetAverage(nLastSample + 1 - nAvailableAverageSamples, nLastSample);
            objSweep.StepFrequencyMHZ = m_objRFEGenerator.RFGenTrackStepMHZ(); //step tracking frequency is not known by analyzer

            double dMinDB = 100;
            double dMaxDB = -120;

            for (UInt16 nInd=0; nInd<objSweep.TotalSteps; nInd++)
            {
                //normal realtime
                double dDB = objSweep.GetAmplitudeDBM(nInd) - m_objRFEAnalyzer.TrackingNormalizedData.GetAmplitudeDBM(nInd);
                if (dDB > dMaxDB)
                    dMaxDB = dDB;
                if (dDB < dMinDB)
                    dMinDB = dDB;
                m_PointList_Tracking_Normal.Add(objSweep.GetFrequencyMHZ(nInd), dDB, RFECommunicator.MIN_AMPLITUDE_DBM);
                //average
                if (objSweepAvg != null)
                {
                    dDB = objSweepAvg.GetAmplitudeDBM(nInd) - m_objRFEAnalyzer.TrackingNormalizedData.GetAmplitudeDBM(nInd);
                    if (dDB > dMaxDB)
                        dMaxDB = dDB;
                    if (dDB < dMinDB)
                        dMinDB = dDB;
                    m_PointList_Tracking_Avg.Add(objSweepAvg.GetFrequencyMHZ(nInd), dDB, RFECommunicator.MIN_AMPLITUDE_DBM);
                }
            }

            m_GraphLine_Tracking_Normal.Points = m_PointList_Tracking_Normal;
            m_GraphLine_Tracking_Normal.IsVisible = true;
            m_GraphLine_Tracking_Normal.Label.IsVisible = true;

            m_GraphLine_Tracking_Avg.Points = m_PointList_Tracking_Avg;
            m_GraphLine_Tracking_Avg.IsVisible = true;
            m_GraphLine_Tracking_Avg.Label.IsVisible = true;

            m_GraphTrackingGenerator.GraphPane.XAxis.Scale.Min = objSweep.StartFrequencyMHZ;
            m_GraphTrackingGenerator.GraphPane.XAxis.Scale.Max = objSweep.EndFrequencyMHZ;
            double dGraphMin=m_GraphTrackingGenerator.GraphPane.YAxis.Scale.Min;
            if ((dGraphMin < dMinDB - 15) || (dGraphMin > dMinDB - 5))
                m_GraphTrackingGenerator.GraphPane.YAxis.Scale.Min = dMinDB - 10;
            double dGraphMax = m_GraphTrackingGenerator.GraphPane.YAxis.Scale.Max;
            if ((dGraphMax > dMaxDB + 15) || (dGraphMax < dMaxDB + 5))
                m_GraphTrackingGenerator.GraphPane.YAxis.Scale.Max = dMaxDB + 10;
            m_GraphTrackingGenerator.AxisChange();
            m_GraphTrackingGenerator.Refresh();
        }

        #endregion
    }
}
