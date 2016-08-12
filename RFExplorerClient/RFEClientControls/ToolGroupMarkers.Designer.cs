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
    partial class ToolGroupMarkers
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
            this.m_GroupControl = new RFEClientControls.GroupControl_Markers();
            this.m_TrackCombo = new System.Windows.Forms.ComboBox();
            this.m_labTrack = new System.Windows.Forms.Label();
            this.m_labAnalyzerCenterFreq = new System.Windows.Forms.Label();
            this.m_sMarkerFreq = new System.Windows.Forms.TextBox();
            this.m_chkEnabled = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.m_MarkerIndex = new System.Windows.Forms.NumericUpDown();
            this.m_GroupControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_MarkerIndex)).BeginInit();
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
            // m_GroupControl
            // 
            this.m_GroupControl.AutoSize = true;
            this.m_GroupControl.Controls.Add(this.m_TrackCombo);
            this.m_GroupControl.Controls.Add(this.m_labTrack);
            this.m_GroupControl.Controls.Add(this.m_labAnalyzerCenterFreq);
            this.m_GroupControl.Controls.Add(this.m_sMarkerFreq);
            this.m_GroupControl.Controls.Add(this.m_chkEnabled);
            this.m_GroupControl.Controls.Add(this.label1);
            this.m_GroupControl.Controls.Add(this.m_MarkerIndex);
            this.m_GroupControl.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.m_GroupControl.Location = new System.Drawing.Point(0, 0);
            this.m_GroupControl.Margin = new System.Windows.Forms.Padding(0);
            this.m_GroupControl.Name = "m_GroupControl";
            this.m_GroupControl.Padding = new System.Windows.Forms.Padding(0);
            this.m_GroupControl.Size = new System.Drawing.Size(177, 116);
            this.m_GroupControl.TabIndex = 0;
            this.m_GroupControl.TabStop = false;
            this.m_GroupControl.Text = "Markers";
            // 
            // m_TrackCombo
            // 
            this.m_TrackCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_TrackCombo.FormattingEnabled = true;
            this.m_TrackCombo.Items.AddRange(new object[] {
            "Realtime",
            "Average",
            "Max Peak",
            "Minimum",
            "Max Hold"});
            this.m_TrackCombo.Location = new System.Drawing.Point(62, 78);
            this.m_TrackCombo.Name = "m_TrackCombo";
            this.m_TrackCombo.Size = new System.Drawing.Size(105, 21);
            this.m_TrackCombo.TabIndex = 6;
            this.m_ToolTip.SetToolTip(this.m_TrackCombo, "Auto-peak Marker 1 trace type selection");
            this.m_TrackCombo.SelectedIndexChanged += new System.EventHandler(this.OnTrackComboSelectedIndexChanged);
            // 
            // m_labTrack
            // 
            this.m_labTrack.AutoSize = true;
            this.m_labTrack.Location = new System.Drawing.Point(10, 81);
            this.m_labTrack.Name = "m_labTrack";
            this.m_labTrack.Size = new System.Drawing.Size(35, 13);
            this.m_labTrack.TabIndex = 5;
            this.m_labTrack.Text = "Track";
            // 
            // m_labAnalyzerCenterFreq
            // 
            this.m_labAnalyzerCenterFreq.AutoSize = true;
            this.m_labAnalyzerCenterFreq.Enabled = false;
            this.m_labAnalyzerCenterFreq.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_labAnalyzerCenterFreq.ForeColor = System.Drawing.Color.DarkBlue;
            this.m_labAnalyzerCenterFreq.Location = new System.Drawing.Point(10, 51);
            this.m_labAnalyzerCenterFreq.Name = "m_labAnalyzerCenterFreq";
            this.m_labAnalyzerCenterFreq.Size = new System.Drawing.Size(39, 16);
            this.m_labAnalyzerCenterFreq.TabIndex = 3;
            this.m_labAnalyzerCenterFreq.Text = "FREQ";
            this.m_labAnalyzerCenterFreq.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_sMarkerFreq
            // 
            this.m_sMarkerFreq.BackColor = System.Drawing.Color.RoyalBlue;
            this.m_sMarkerFreq.Enabled = false;
            this.m_sMarkerFreq.Font = new System.Drawing.Font("Digital-7", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_sMarkerFreq.ForeColor = System.Drawing.Color.White;
            this.m_sMarkerFreq.Location = new System.Drawing.Point(62, 46);
            this.m_sMarkerFreq.MinimumSize = new System.Drawing.Size(98, 26);
            this.m_sMarkerFreq.Name = "m_sMarkerFreq";
            this.m_sMarkerFreq.Size = new System.Drawing.Size(105, 26);
            this.m_sMarkerFreq.TabIndex = 4;
            this.m_sMarkerFreq.Text = "1000.000";
            this.m_sMarkerFreq.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_ToolTip.SetToolTip(this.m_sMarkerFreq, "Marker frequency in MHZ");
            this.m_sMarkerFreq.WordWrap = false;
            this.m_sMarkerFreq.Leave += new System.EventHandler(this.OnFreqLeave);
            // 
            // m_chkEnabled
            // 
            this.m_chkEnabled.AutoSize = true;
            this.m_chkEnabled.Checked = true;
            this.m_chkEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_chkEnabled.Location = new System.Drawing.Point(109, 23);
            this.m_chkEnabled.Name = "m_chkEnabled";
            this.m_chkEnabled.Size = new System.Drawing.Size(65, 17);
            this.m_chkEnabled.TabIndex = 2;
            this.m_chkEnabled.Text = "Enabled";
            this.m_ToolTip.SetToolTip(this.m_chkEnabled, "Enable or disable current Marker");
            this.m_chkEnabled.UseVisualStyleBackColor = true;
            this.m_chkEnabled.CheckStateChanged += new System.EventHandler(this.OnMarkerEnabledChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Marker ID";
            // 
            // m_MarkerIndex
            // 
            this.m_MarkerIndex.AutoSize = true;
            this.m_MarkerIndex.Location = new System.Drawing.Point(59, 21);
            this.m_MarkerIndex.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.m_MarkerIndex.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.m_MarkerIndex.Name = "m_MarkerIndex";
            this.m_MarkerIndex.Size = new System.Drawing.Size(44, 20);
            this.m_MarkerIndex.TabIndex = 0;
            this.m_MarkerIndex.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_ToolTip.SetToolTip(this.m_MarkerIndex, "Marker identifier - Marker 1 is auto-peak and others are set by specific frequenc" +
        "y");
            this.m_MarkerIndex.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.m_MarkerIndex.ValueChanged += new System.EventHandler(this.OnMarkerIndexChanged);
            // 
            // ToolGroupMarkers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.m_GroupControl);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MinimumSize = new System.Drawing.Size(177, 116);
            this.Name = "ToolGroupMarkers";
            this.Size = new System.Drawing.Size(177, 116);
            this.Load += new System.EventHandler(this.ToolGroupMarkers_Load);
            this.m_GroupControl.ResumeLayout(false);
            this.m_GroupControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_MarkerIndex)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        internal System.Windows.Forms.Label m_labAnalyzerCenterFreq;
        internal System.Windows.Forms.TextBox m_sMarkerFreq;
        internal System.Windows.Forms.NumericUpDown m_MarkerIndex;
        internal System.Windows.Forms.CheckBox m_chkEnabled;
        internal System.Windows.Forms.ComboBox m_TrackCombo;
        internal System.Windows.Forms.Label label1;
        internal System.Windows.Forms.Label m_labTrack;
        private GroupControl_Markers m_GroupControl;
        private System.Windows.Forms.ToolTip m_ToolTip;
    }
}
