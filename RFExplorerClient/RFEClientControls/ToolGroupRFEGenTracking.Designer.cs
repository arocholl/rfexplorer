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

using System.Windows.Forms;

namespace RFEClientControls
{
    partial class ToolGroupRFEGenTracking
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
            this.m_groupControl_RFEGen_Tracking = new RFEClientControls.GroupControl_RFEGen_Tracking();
            this.m_nRFGENIterationAverage = new System.Windows.Forms.NumericUpDown();
            this.m_btnNormalizeTracking = new System.Windows.Forms.Button();
            this.m_btnTrackingStop = new System.Windows.Forms.Button();
            this.m_btnTrackingStart = new System.Windows.Forms.Button();
            this.m_listSNAOptions = new System.Windows.Forms.ComboBox();
            this.m_chkSNAAutoStop = new System.Windows.Forms.CheckBox();
            this.m_labRFGenAverage = new System.Windows.Forms.Label();
            this.m_ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.m_groupControl_RFEGen_Tracking.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_nRFGENIterationAverage)).BeginInit();
            this.SuspendLayout();
            // 
            // m_groupControl_RFEGen_Tracking
            // 
            this.m_groupControl_RFEGen_Tracking.AutoSize = true;
            this.m_groupControl_RFEGen_Tracking.Controls.Add(this.m_nRFGENIterationAverage);
            this.m_groupControl_RFEGen_Tracking.Controls.Add(this.m_btnNormalizeTracking);
            this.m_groupControl_RFEGen_Tracking.Controls.Add(this.m_btnTrackingStop);
            this.m_groupControl_RFEGen_Tracking.Controls.Add(this.m_btnTrackingStart);
            this.m_groupControl_RFEGen_Tracking.Controls.Add(this.m_listSNAOptions);
            this.m_groupControl_RFEGen_Tracking.Controls.Add(this.m_chkSNAAutoStop);
            this.m_groupControl_RFEGen_Tracking.Controls.Add(this.m_labRFGenAverage);
            this.m_groupControl_RFEGen_Tracking.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.m_groupControl_RFEGen_Tracking.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.m_groupControl_RFEGen_Tracking.Location = new System.Drawing.Point(0, 0);
            this.m_groupControl_RFEGen_Tracking.Margin = new System.Windows.Forms.Padding(0);
            this.m_groupControl_RFEGen_Tracking.MinimumSize = new System.Drawing.Size(268, 116);
            this.m_groupControl_RFEGen_Tracking.Name = "m_groupControl_RFEGen_Tracking";
            this.m_groupControl_RFEGen_Tracking.Padding = new System.Windows.Forms.Padding(0);
            this.m_groupControl_RFEGen_Tracking.Size = new System.Drawing.Size(268, 118);
            this.m_groupControl_RFEGen_Tracking.TabIndex = 52;
            this.m_groupControl_RFEGen_Tracking.TabStop = false;
            this.m_groupControl_RFEGen_Tracking.Text = "Scalar Network Analyzer Tracking";
            this.m_ToolTip.SetToolTip(this.m_groupControl_RFEGen_Tracking, "Scalar Network Analyzer Tracking tool sets options");
            // 
            // m_nRFGENIterationAverage
            // 
            this.m_nRFGENIterationAverage.AutoSize = true;
            this.m_nRFGENIterationAverage.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_nRFGENIterationAverage.Location = new System.Drawing.Point(194, 24);
            this.m_nRFGENIterationAverage.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.m_nRFGENIterationAverage.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.m_nRFGENIterationAverage.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.m_nRFGENIterationAverage.Name = "m_nRFGENIterationAverage";
            this.m_nRFGENIterationAverage.Size = new System.Drawing.Size(64, 23);
            this.m_nRFGENIterationAverage.TabIndex = 68;
            this.m_nRFGENIterationAverage.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_ToolTip.SetToolTip(this.m_nRFGENIterationAverage, "Total number of sweeps to average samples, larger numbers improve noise rejection" +
        "");
            this.m_nRFGENIterationAverage.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // m_btnNormalizeTracking
            // 
            this.m_btnNormalizeTracking.AutoSize = true;
            this.m_btnNormalizeTracking.Location = new System.Drawing.Point(6, 24);
            this.m_btnNormalizeTracking.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.m_btnNormalizeTracking.Name = "m_btnNormalizeTracking";
            this.m_btnNormalizeTracking.Size = new System.Drawing.Size(119, 26);
            this.m_btnNormalizeTracking.TabIndex = 64;
            this.m_btnNormalizeTracking.Text = "Normalize SNA...";
            this.m_ToolTip.SetToolTip(this.m_btnNormalizeTracking, "Start Normalization Process required for tracking");
            this.m_btnNormalizeTracking.UseVisualStyleBackColor = true;
            this.m_btnNormalizeTracking.Click += new System.EventHandler(this.OnNormalizeTrackingStart_Click);
            // 
            // m_btnTrackingStop
            // 
            this.m_btnTrackingStop.AutoSize = true;
            this.m_btnTrackingStop.Location = new System.Drawing.Point(6, 78);
            this.m_btnTrackingStop.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.m_btnTrackingStop.Name = "m_btnTrackingStop";
            this.m_btnTrackingStop.Size = new System.Drawing.Size(119, 26);
            this.m_btnTrackingStop.TabIndex = 65;
            this.m_btnTrackingStop.Text = "Stop SNA";
            this.m_ToolTip.SetToolTip(this.m_btnTrackingStop, "Stop tracking");
            this.m_btnTrackingStop.UseVisualStyleBackColor = true;
            this.m_btnTrackingStop.Click += new System.EventHandler(this.OnTrackingStop_Click);
            // 
            // m_btnTrackingStart
            // 
            this.m_btnTrackingStart.AutoSize = true;
            this.m_btnTrackingStart.Location = new System.Drawing.Point(6, 51);
            this.m_btnTrackingStart.Name = "m_btnTrackingStart";
            this.m_btnTrackingStart.Size = new System.Drawing.Size(119, 26);
            this.m_btnTrackingStart.TabIndex = 66;
            this.m_btnTrackingStart.Text = "Start SNA...";
            this.m_ToolTip.SetToolTip(this.m_btnTrackingStart, "Start full SNA tracking sweep based on current normalization");
            this.m_btnTrackingStart.UseVisualStyleBackColor = true;
            this.m_btnTrackingStart.Click += new System.EventHandler(this.OnTrackingStart_Click);
            // 
            // m_listSNAOptions
            // 
            this.m_listSNAOptions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_listSNAOptions.FormattingEnabled = true;
            this.m_listSNAOptions.Items.AddRange(new object[] {
            "Insertion Loss (dB)",
            "Return Loss (dB)",
            "Return Loss (VSWR)"});
            this.m_listSNAOptions.Location = new System.Drawing.Point(134, 82);
            this.m_listSNAOptions.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.m_listSNAOptions.Name = "m_listSNAOptions";
            this.m_listSNAOptions.Size = new System.Drawing.Size(124, 21);
            this.m_listSNAOptions.TabIndex = 70;
            this.m_ToolTip.SetToolTip(this.m_listSNAOptions, "Select display option to correctly interpret tracking sweep data");
            this.m_listSNAOptions.SelectedIndexChanged += new System.EventHandler(this.OnSNAOptions_SelectedIndexChanged);
            // 
            // m_chkSNAAutoStop
            // 
            this.m_chkSNAAutoStop.AutoSize = true;
            this.m_chkSNAAutoStop.Location = new System.Drawing.Point(134, 57);
            this.m_chkSNAAutoStop.Name = "m_chkSNAAutoStop";
            this.m_chkSNAAutoStop.Size = new System.Drawing.Size(117, 17);
            this.m_chkSNAAutoStop.TabIndex = 69;
            this.m_chkSNAAutoStop.Text = "Stop auto Average";
            this.m_ToolTip.SetToolTip(this.m_chkSNAAutoStop, "Autostop SNA tracking after <Average> specified number of sweeps");
            this.m_chkSNAAutoStop.UseVisualStyleBackColor = true;
            // 
            // m_labRFGenAverage
            // 
            this.m_labRFGenAverage.AutoSize = true;
            this.m_labRFGenAverage.Location = new System.Drawing.Point(136, 28);
            this.m_labRFGenAverage.Name = "m_labRFGenAverage";
            this.m_labRFGenAverage.Size = new System.Drawing.Size(52, 13);
            this.m_labRFGenAverage.TabIndex = 67;
            this.m_labRFGenAverage.Text = "Average:";
            this.m_labRFGenAverage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // m_ToolTip
            // 
            this.m_ToolTip.AutomaticDelay = 1500;
            this.m_ToolTip.AutoPopDelay = 15000;
            this.m_ToolTip.ForeColor = System.Drawing.Color.Blue;
            this.m_ToolTip.InitialDelay = 500;
            this.m_ToolTip.ReshowDelay = 300;
            // 
            // ToolGroupRFEGenTracking
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.m_groupControl_RFEGen_Tracking);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ToolGroupRFEGenTracking";
            this.Size = new System.Drawing.Size(268, 118);
            this.m_groupControl_RFEGen_Tracking.ResumeLayout(false);
            this.m_groupControl_RFEGen_Tracking.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_nRFGENIterationAverage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        internal System.Windows.Forms.NumericUpDown m_nRFGENIterationAverage;
        internal System.Windows.Forms.Button m_btnNormalizeTracking;
        internal System.Windows.Forms.Button m_btnTrackingStop;
        internal System.Windows.Forms.Button m_btnTrackingStart;
        internal System.Windows.Forms.ComboBox m_listSNAOptions;
        internal System.Windows.Forms.CheckBox m_chkSNAAutoStop;
        internal System.Windows.Forms.Label m_labRFGenAverage;
        internal GroupControl_RFEGen_Tracking m_groupControl_RFEGen_Tracking;
        private ToolTip m_ToolTip;
    }
}
