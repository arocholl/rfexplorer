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

namespace RFExplorerClient
{
    public partial class MainForm : Form
    {
        #region Waterfall
        float m_wSizeX;
        float m_wSizeY;

        private RFEClientControls.WaterfallControl controlWaterfall;
        private System.Windows.Forms.Panel panelWaterfall;
        private System.Windows.Forms.TabPage tabWaterfall;
        private System.Windows.Forms.GroupBox waterfallGroupBox;
        private System.Windows.Forms.Label labelSensitivity;
        private System.Windows.Forms.Label labelContrast;
        private System.Windows.Forms.NumericUpDown numericContrast;
        private System.Windows.Forms.NumericUpDown numericSensitivity;
        private System.Windows.Forms.TrackBar trackBarContrast;
        private System.Windows.Forms.TrackBar trackBarSensitivity;

        private void InitializeOldWaterfall()
        {
            this.tabWaterfall = new System.Windows.Forms.TabPage();
            this.waterfallGroupBox = new System.Windows.Forms.GroupBox();
            this.trackBarContrast = new System.Windows.Forms.TrackBar();
            this.trackBarSensitivity = new System.Windows.Forms.TrackBar();
            this.numericContrast = new System.Windows.Forms.NumericUpDown();
            this.numericSensitivity = new System.Windows.Forms.NumericUpDown();
            this.labelSensitivity = new System.Windows.Forms.Label();
            this.labelContrast = new System.Windows.Forms.Label();
            this.panelWaterfall = new System.Windows.Forms.Panel();
            this.controlWaterfall = new RFEClientControls.WaterfallControl();

            this.tabWaterfall.SuspendLayout();
            this.waterfallGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarContrast)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSensitivity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericContrast)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericSensitivity)).BeginInit();
            this.panelWaterfall.SuspendLayout();
            this.tabRemoteScreen.SuspendLayout();

            this.MainTab.Controls.Add(this.tabWaterfall);

            // 
            // tabWaterfall
            // 
            this.tabWaterfall.BackColor = System.Drawing.SystemColors.Control;
            this.tabWaterfall.Controls.Add(this.waterfallGroupBox);
            this.tabWaterfall.Controls.Add(this.panelWaterfall);
            this.tabWaterfall.Location = new System.Drawing.Point(4, 26);
            this.tabWaterfall.Name = "tabWaterfall";
            this.tabWaterfall.Padding = new System.Windows.Forms.Padding(3);
            this.tabWaterfall.Size = new System.Drawing.Size(932, 510);
            this.tabWaterfall.TabIndex = 3;
            this.tabWaterfall.Text = "Waterfall";
            this.tabWaterfall.Paint += new System.Windows.Forms.PaintEventHandler(this.tabWaterfall_Paint);
            this.tabWaterfall.Enter += new System.EventHandler(this.tabWaterfall_Enter);
            // 
            // waterfallGroupBox
            // 
            this.waterfallGroupBox.Controls.Add(this.trackBarContrast);
            this.waterfallGroupBox.Controls.Add(this.trackBarSensitivity);
            this.waterfallGroupBox.Controls.Add(this.numericContrast);
            this.waterfallGroupBox.Controls.Add(this.numericSensitivity);
            this.waterfallGroupBox.Controls.Add(this.labelSensitivity);
            this.waterfallGroupBox.Controls.Add(this.labelContrast);
            this.waterfallGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.waterfallGroupBox.Location = new System.Drawing.Point(512, 6);
            this.waterfallGroupBox.Name = "waterfallGroupBox";
            this.waterfallGroupBox.Size = new System.Drawing.Size(410, 108);
            this.waterfallGroupBox.TabIndex = 56;
            this.waterfallGroupBox.TabStop = false;
            this.waterfallGroupBox.Text = "Waterfall Controls";
            // 
            // trackBarContrast
            // 
            this.trackBarContrast.Location = new System.Drawing.Point(184, 54);
            this.trackBarContrast.Maximum = 255;
            this.trackBarContrast.Minimum = 1;
            this.trackBarContrast.Name = "trackBarContrast";
            this.trackBarContrast.Size = new System.Drawing.Size(220, 45);
            this.trackBarContrast.TabIndex = 57;
            this.trackBarContrast.Value = 215;
            this.trackBarContrast.ValueChanged += new System.EventHandler(this.trackBarContrast_ValueChanged);
            // 
            // trackBarSensitivity
            // 
            this.trackBarSensitivity.Location = new System.Drawing.Point(184, 17);
            this.trackBarSensitivity.Maximum = 255;
            this.trackBarSensitivity.Minimum = 1;
            this.trackBarSensitivity.Name = "trackBarSensitivity";
            this.trackBarSensitivity.Size = new System.Drawing.Size(220, 45);
            this.trackBarSensitivity.TabIndex = 56;
            this.trackBarSensitivity.Value = 100;
            this.trackBarSensitivity.ValueChanged += new System.EventHandler(this.trackBarSensitivity_ValueChanged);
            // 
            // numericContrast
            // 
            this.numericContrast.Font = new System.Drawing.Font("Digital-7", 18F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericContrast.Location = new System.Drawing.Point(108, 54);
            this.numericContrast.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericContrast.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericContrast.Name = "numericContrast";
            this.numericContrast.Size = new System.Drawing.Size(70, 31);
            this.numericContrast.TabIndex = 54;
            this.numericContrast.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericContrast.Value = new decimal(new int[] {
            215,
            0,
            0,
            0});
            this.numericContrast.ValueChanged += new System.EventHandler(this.numericContrast_ValueChanged);
            // 
            // numericSensitivity
            // 
            this.numericSensitivity.Font = new System.Drawing.Font("Digital-7", 18F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericSensitivity.Location = new System.Drawing.Point(108, 17);
            this.numericSensitivity.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericSensitivity.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericSensitivity.Name = "numericSensitivity";
            this.numericSensitivity.Size = new System.Drawing.Size(70, 31);
            this.numericSensitivity.TabIndex = 53;
            this.numericSensitivity.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericSensitivity.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericSensitivity.ValueChanged += new System.EventHandler(this.numericSensitivity_ValueChanged);
            // 
            // labelSensitivity
            // 
            this.labelSensitivity.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSensitivity.Location = new System.Drawing.Point(6, 23);
            this.labelSensitivity.Name = "labelSensitivity";
            this.labelSensitivity.Size = new System.Drawing.Size(96, 22);
            this.labelSensitivity.TabIndex = 52;
            this.labelSensitivity.Text = "Sensitivity";
            // 
            // labelContrast
            // 
            this.labelContrast.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.labelContrast.Location = new System.Drawing.Point(6, 60);
            this.labelContrast.Name = "labelContrast";
            this.labelContrast.Size = new System.Drawing.Size(96, 21);
            this.labelContrast.TabIndex = 50;
            this.labelContrast.Text = "Contrast";
            // 
            // panelWaterfall
            // 
            this.panelWaterfall.Controls.Add(this.controlWaterfall);
            this.panelWaterfall.Location = new System.Drawing.Point(12, 120);
            this.panelWaterfall.Name = "panelWaterfall";
            this.panelWaterfall.Size = new System.Drawing.Size(910, 462);
            this.panelWaterfall.TabIndex = 55;
            // 
            // controlWaterfall
            // 
            this.controlWaterfall.Location = new System.Drawing.Point(0, 0);
            this.controlWaterfall.Name = "controlWaterfall";
            this.controlWaterfall.Size = new System.Drawing.Size(292, 174);
            this.controlWaterfall.TabIndex = 54;
            this.controlWaterfall.Load += new System.EventHandler(this.controlWaterfall_Load);

            this.tabWaterfall.ResumeLayout(false);
            this.waterfallGroupBox.ResumeLayout(false);
            this.waterfallGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarContrast)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSensitivity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericContrast)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericSensitivity)).EndInit();
            this.panelWaterfall.ResumeLayout(false);
            this.tabRemoteScreen.ResumeLayout(false);
        }

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
            if (m_objRFE.HoldMode)
                controlWaterfall.Invalidate();
        }

        private void numericContrast_ValueChanged(object sender, EventArgs e)
        {
            int contrast = (UInt16)numericContrast.Value;
            trackBarContrast.Value = contrast;
            controlWaterfall.UpdateContrast(contrast);
            if (m_objRFE.HoldMode)
                controlWaterfall.Invalidate();
        }

        private void trackBarSensitivity_ValueChanged(object sender, EventArgs e)
        {
            int sensitivity = (UInt16)trackBarSensitivity.Value;
            numericSensitivity.Value = sensitivity;
            controlWaterfall.UpdateSensitivity(sensitivity);
            if (m_objRFE.HoldMode)
                controlWaterfall.Invalidate();
        }

        private void trackBarContrast_ValueChanged(object sender, EventArgs e)
        {
            int contrast = (UInt16)trackBarContrast.Value;
            numericContrast.Value = contrast;
            controlWaterfall.UpdateContrast(contrast);
            if (m_objRFE.HoldMode)
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

            if (m_objRFE.SweepData.Count == 0)
                return; //nothing to paint

            UInt32 nSweepIndex = (UInt32)m_objRFE.SweepData.UpperBound;
            m_nDrawingIteration++;

            UInt32 nTotalCalculatorIterations = (UInt32)numericIterations.Value;
            if (nTotalCalculatorIterations > nSweepIndex)
                nTotalCalculatorIterations = nSweepIndex;

            if ((m_nDrawingIteration & 0xf) == 0)
            {
                //Update screen status every 16 drawing iterations only to reduce overhead
                toolStripMemory.Value = (int)nSweepIndex;
                if (m_objRFE.PortConnected)
                    toolCOMStatus.Text = "Connected";
                else
                    toolCOMStatus.Text = "Disconnected";

                toolStripSamples.Text = "Total Samples in buffer: " + (UInt32)numericSampleSA.Value + "/" + RFESweepDataCollection.MAX_ELEMENTS + " - " + (100 * (double)numericSampleSA.Value / RFESweepDataCollection.MAX_ELEMENTS).ToString("0.0") + "%";
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

            RFESweepData objSweep = m_objRFE.SweepData.GetData(nSweepIndex);
            for (UInt16 nSweepPointInd = 0; nSweepPointInd < m_objRFE.FreqSpectrumSteps; nSweepPointInd++)
            {
                double fVal = objSweep.GetAmplitudeDBM(nSweepPointInd);
                if (fVal > fRealtimeMax_Amp)
                {
                    fRealtimeMax_Amp = fVal;
                    fRealtimeMax_Iter = nSweepPointInd;
                }

                double fFreq = m_objRFE.StartFrequencyMHZ + m_objRFE.StepFrequencyMHZ * nSweepPointInd;

                double fMax = fVal;
                double fMin = fVal;
                double fValAvg = fVal;

                for (UInt32 nSweepIterator = nSweepIndex - nTotalCalculatorIterations; nSweepIterator < nSweepIndex; nSweepIterator++)
                {
                    //Calculate average, max and min over Calculator range
                    RFESweepData objSweepIter = m_objRFE.SweepData.GetData(nSweepIterator);
                    if (objSweepIter != null)
                    {
                        double fVal2 = objSweepIter.GetAmplitudeDBM(nSweepPointInd);

                        fMax = Math.Max(fMax, fVal2);
                        fMin = Math.Min(fMin, fVal2);
                        fValAvg += fVal2;
                    }
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
                        fMaxMax_Iter = nSweepPointInd;
                    }
                }

                if (m_bDrawAverage)
                {
                    fValAvg = fValAvg / (nTotalCalculatorIterations + 1);
                    AvgList.Add(fFreq, fValAvg);
                    if (fValAvg > fAverageMax_Amp)
                    {
                        fAverageMax_Amp = fValAvg;
                        fAverageMax_Iter = nSweepPointInd;
                    }
                }

                if (m_bDrawMax)
                {
                    controlWaterfall.DrawWaterfall(MaxList);
                }
                else if (m_bDrawAverage)
                {
                    controlWaterfall.DrawWaterfall(AvgList);
                }
                else if (m_bDrawRealtime)
                {
                    controlWaterfall.DrawWaterfall(RTList);
                }
                else if (m_bDrawMin)
                {
                    controlWaterfall.DrawWaterfall(MinList);
                }

                controlWaterfall.Invalidate();
            }
        }

        private void tabWaterfall_UpdateZoomValues()
        {
            controlWaterfall.Size = new Size((int)(1.0 + m_wSizeX * (float)(numericZoom.Value)), (int)(1.0 + m_wSizeY * (float)(numericZoom.Value)));
            controlWaterfall.UpdateZoom((int)(numericZoom.Value));
            controlWaterfall.Invalidate();
        }

        #endregion
    }
}