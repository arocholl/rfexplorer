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

namespace RFEClientControls
{
    partial class ToolGroupTraces
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.m_ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.m_chkAxisLabels = new System.Windows.Forms.CheckBox();
            this.m_chkShowGrid = new System.Windows.Forms.CheckBox();
            this.m_chkThick = new System.Windows.Forms.CheckBox();
            this.m_chkSmooth = new System.Windows.Forms.CheckBox();
            this.m_chkFillTrace = new System.Windows.Forms.CheckBox();
            this.m_chkCalcMaxHold = new System.Windows.Forms.CheckBox();
            this.m_chkCalcMaxPeak = new System.Windows.Forms.CheckBox();
            this.m_chkCalcMin = new System.Windows.Forms.CheckBox();
            this.m_chkCalcAverage = new System.Windows.Forms.CheckBox();
            this.m_chkCalcRealtime = new System.Windows.Forms.CheckBox();
            this.m_GroupControl = new RFEClientControls.GroupControl_Traces();
            this.m_GroupControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_ToolTip
            // 
            this.m_ToolTip.AutomaticDelay = 1500;
            this.m_ToolTip.AutoPopDelay = 15000;
            this.m_ToolTip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.m_ToolTip.ForeColor = System.Drawing.Color.Blue;
            this.m_ToolTip.InitialDelay = 500;
            this.m_ToolTip.ReshowDelay = 300;
            // 
            // m_chkAxisLabels
            // 
            this.m_chkAxisLabels.AutoSize = true;
            this.m_chkAxisLabels.Font = new System.Drawing.Font("Tahoma", 7F);
            this.m_chkAxisLabels.Location = new System.Drawing.Point(9, 82);
            this.m_chkAxisLabels.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.m_chkAxisLabels.Name = "m_chkAxisLabels";
            this.m_chkAxisLabels.Size = new System.Drawing.Size(72, 16);
            this.m_chkAxisLabels.TabIndex = 58;
            this.m_chkAxisLabels.Text = "Axis Labels";
            this.m_ToolTip.SetToolTip(this.m_chkAxisLabels, "Select to display Amplitude and Frequency Axis labels");
            this.m_chkAxisLabels.UseVisualStyleBackColor = true;
            this.m_chkAxisLabels.CheckStateChanged += new System.EventHandler(this.OnDisplayAxisLabelsChanged);
            // 
            // m_chkShowGrid
            // 
            this.m_chkShowGrid.AutoSize = true;
            this.m_chkShowGrid.Font = new System.Drawing.Font("Tahoma", 7F);
            this.m_chkShowGrid.Location = new System.Drawing.Point(9, 67);
            this.m_chkShowGrid.Name = "m_chkShowGrid";
            this.m_chkShowGrid.Size = new System.Drawing.Size(72, 16);
            this.m_chkShowGrid.TabIndex = 58;
            this.m_chkShowGrid.Text = "Show Grid";
            this.m_ToolTip.SetToolTip(this.m_chkShowGrid, "Select to enable grid in Spectrum Analyzer graph");
            this.m_chkShowGrid.UseVisualStyleBackColor = true;
            this.m_chkShowGrid.CheckStateChanged += new System.EventHandler(this.OnDisplayShowGridChanged);
            // 
            // m_chkThick
            // 
            this.m_chkThick.AutoSize = true;
            this.m_chkThick.Font = new System.Drawing.Font("Tahoma", 7F);
            this.m_chkThick.Location = new System.Drawing.Point(9, 52);
            this.m_chkThick.Name = "m_chkThick";
            this.m_chkThick.Size = new System.Drawing.Size(77, 16);
            this.m_chkThick.TabIndex = 58;
            this.m_chkThick.Text = "Thick Trace";
            this.m_ToolTip.SetToolTip(this.m_chkThick, "Select to display traces with extra width");
            this.m_chkThick.UseVisualStyleBackColor = true;
            this.m_chkThick.CheckStateChanged += new System.EventHandler(this.OnDisplayThickTraceChanged);
            // 
            // m_chkSmooth
            // 
            this.m_chkSmooth.AutoSize = true;
            this.m_chkSmooth.Font = new System.Drawing.Font("Tahoma", 7F);
            this.m_chkSmooth.Location = new System.Drawing.Point(9, 37);
            this.m_chkSmooth.Name = "m_chkSmooth";
            this.m_chkSmooth.Size = new System.Drawing.Size(59, 16);
            this.m_chkSmooth.TabIndex = 58;
            this.m_chkSmooth.Text = "Smooth";
            this.m_ToolTip.SetToolTip(this.m_chkSmooth, "Select to use softed spline type aproximation traces");
            this.m_chkSmooth.UseVisualStyleBackColor = true;
            this.m_chkSmooth.CheckStateChanged += new System.EventHandler(this.OnDisplaySmoothChanged);
            // 
            // m_chkFillTrace
            // 
            this.m_chkFillTrace.AutoSize = true;
            this.m_chkFillTrace.Font = new System.Drawing.Font("Tahoma", 7F);
            this.m_chkFillTrace.Location = new System.Drawing.Point(9, 22);
            this.m_chkFillTrace.Name = "m_chkFillTrace";
            this.m_chkFillTrace.Size = new System.Drawing.Size(64, 16);
            this.m_chkFillTrace.TabIndex = 58;
            this.m_chkFillTrace.Text = "Fill Trace";
            this.m_ToolTip.SetToolTip(this.m_chkFillTrace, "Select to display Spectrum Analyzer traces filled");
            this.m_chkFillTrace.UseVisualStyleBackColor = true;
            this.m_chkFillTrace.CheckStateChanged += new System.EventHandler(this.OnDisplayFillTraceChanged);
            // 
            // m_chkCalcMaxHold
            // 
            this.m_chkCalcMaxHold.AutoSize = true;
            this.m_chkCalcMaxHold.Font = new System.Drawing.Font("Tahoma", 7F);
            this.m_chkCalcMaxHold.Location = new System.Drawing.Point(91, 67);
            this.m_chkCalcMaxHold.Name = "m_chkCalcMaxHold";
            this.m_chkCalcMaxHold.Size = new System.Drawing.Size(66, 16);
            this.m_chkCalcMaxHold.TabIndex = 57;
            this.m_chkCalcMaxHold.Text = "Max Hold";
            this.m_ToolTip.SetToolTip(this.m_chkCalcMaxHold, "Select to display Maximum Hold trace");
            this.m_chkCalcMaxHold.UseVisualStyleBackColor = true;
            this.m_chkCalcMaxHold.CheckStateChanged += new System.EventHandler(this.OnDisplayMaxHoldChanged);
            // 
            // m_chkCalcMaxPeak
            // 
            this.m_chkCalcMaxPeak.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.m_chkCalcMaxPeak.AutoSize = true;
            this.m_chkCalcMaxPeak.Font = new System.Drawing.Font("Tahoma", 7F);
            this.m_chkCalcMaxPeak.Location = new System.Drawing.Point(91, 52);
            this.m_chkCalcMaxPeak.Name = "m_chkCalcMaxPeak";
            this.m_chkCalcMaxPeak.Size = new System.Drawing.Size(67, 16);
            this.m_chkCalcMaxPeak.TabIndex = 55;
            this.m_chkCalcMaxPeak.Text = "Max Peak";
            this.m_ToolTip.SetToolTip(this.m_chkCalcMaxPeak, "Select to display Maximum Peak trace (using Iterations for sample number)");
            this.m_chkCalcMaxPeak.UseVisualStyleBackColor = true;
            this.m_chkCalcMaxPeak.CheckStateChanged += new System.EventHandler(this.OnDisplayMaxPeakChanged);
            // 
            // m_chkCalcMin
            // 
            this.m_chkCalcMin.AutoSize = true;
            this.m_chkCalcMin.Font = new System.Drawing.Font("Tahoma", 7F);
            this.m_chkCalcMin.Location = new System.Drawing.Point(91, 82);
            this.m_chkCalcMin.Margin = new System.Windows.Forms.Padding(0);
            this.m_chkCalcMin.Name = "m_chkCalcMin";
            this.m_chkCalcMin.Size = new System.Drawing.Size(64, 16);
            this.m_chkCalcMin.TabIndex = 53;
            this.m_chkCalcMin.Text = "Minimum";
            this.m_ToolTip.SetToolTip(this.m_chkCalcMin, "Select to display Minimum trace  (using Iterations for sample number)");
            this.m_chkCalcMin.UseVisualStyleBackColor = true;
            this.m_chkCalcMin.CheckStateChanged += new System.EventHandler(this.OnDisplayMinimumChanged);
            // 
            // m_chkCalcAverage
            // 
            this.m_chkCalcAverage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.m_chkCalcAverage.AutoSize = true;
            this.m_chkCalcAverage.Font = new System.Drawing.Font("Tahoma", 7F);
            this.m_chkCalcAverage.Location = new System.Drawing.Point(91, 37);
            this.m_chkCalcAverage.Name = "m_chkCalcAverage";
            this.m_chkCalcAverage.Size = new System.Drawing.Size(62, 16);
            this.m_chkCalcAverage.TabIndex = 56;
            this.m_chkCalcAverage.Text = "Average";
            this.m_ToolTip.SetToolTip(this.m_chkCalcAverage, "Select to display arithmetic Average trace  (using Iterations for sample number)");
            this.m_chkCalcAverage.UseVisualStyleBackColor = true;
            this.m_chkCalcAverage.CheckStateChanged += new System.EventHandler(this.OnDisplayAverageChanged);
            // 
            // m_chkCalcRealtime
            // 
            this.m_chkCalcRealtime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.m_chkCalcRealtime.AutoSize = true;
            this.m_chkCalcRealtime.Font = new System.Drawing.Font("Tahoma", 7F);
            this.m_chkCalcRealtime.Location = new System.Drawing.Point(91, 22);
            this.m_chkCalcRealtime.Name = "m_chkCalcRealtime";
            this.m_chkCalcRealtime.Size = new System.Drawing.Size(60, 16);
            this.m_chkCalcRealtime.TabIndex = 54;
            this.m_chkCalcRealtime.Text = "Realtime";
            this.m_ToolTip.SetToolTip(this.m_chkCalcRealtime, "Select to display normal Realtime trace");
            this.m_chkCalcRealtime.UseVisualStyleBackColor = true;
            this.m_chkCalcRealtime.CheckStateChanged += new System.EventHandler(this.OnDisplayRealtimeChanged);
            // 
            // m_GroupControl
            // 
            this.m_GroupControl.AutoSize = true;
            this.m_GroupControl.Controls.Add(this.m_chkAxisLabels);
            this.m_GroupControl.Controls.Add(this.m_chkShowGrid);
            this.m_GroupControl.Controls.Add(this.m_chkThick);
            this.m_GroupControl.Controls.Add(this.m_chkSmooth);
            this.m_GroupControl.Controls.Add(this.m_chkFillTrace);
            this.m_GroupControl.Controls.Add(this.m_chkCalcMaxHold);
            this.m_GroupControl.Controls.Add(this.m_chkCalcMaxPeak);
            this.m_GroupControl.Controls.Add(this.m_chkCalcMin);
            this.m_GroupControl.Controls.Add(this.m_chkCalcAverage);
            this.m_GroupControl.Controls.Add(this.m_chkCalcRealtime);
            this.m_GroupControl.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.m_GroupControl.Location = new System.Drawing.Point(0, 0);
            this.m_GroupControl.Margin = new System.Windows.Forms.Padding(0);
            this.m_GroupControl.MinimumSize = new System.Drawing.Size(160, 116);
            this.m_GroupControl.Name = "m_GroupControl";
            this.m_GroupControl.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.m_GroupControl.Size = new System.Drawing.Size(164, 116);
            this.m_GroupControl.TabIndex = 58;
            this.m_GroupControl.TabStop = false;
            this.m_GroupControl.Text = "Trace Mode";
            // 
            // ToolGroupTraces
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.m_GroupControl);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MinimumSize = new System.Drawing.Size(164, 116);
            this.Name = "ToolGroupTraces";
            this.Size = new System.Drawing.Size(164, 116);
            this.Load += new System.EventHandler(this.ToolGroupTraces_Load);
            this.m_GroupControl.ResumeLayout(false);
            this.m_GroupControl.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.CheckBox m_chkCalcMaxHold;
        internal System.Windows.Forms.CheckBox m_chkCalcMin;
        internal System.Windows.Forms.CheckBox m_chkCalcRealtime;
        internal System.Windows.Forms.CheckBox m_chkCalcMaxPeak;
        internal System.Windows.Forms.CheckBox m_chkCalcAverage;
        private GroupControl_Traces m_GroupControl;
        internal System.Windows.Forms.CheckBox m_chkAxisLabels;
        internal System.Windows.Forms.CheckBox m_chkShowGrid;
        internal System.Windows.Forms.CheckBox m_chkThick;
        internal System.Windows.Forms.CheckBox m_chkSmooth;
        internal System.Windows.Forms.CheckBox m_chkFillTrace;
        private System.Windows.Forms.ToolTip m_ToolTip;
    }
}
