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
    partial class ToolGroupRFEGenFreqSweep
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
            this.m_ToolTips = new System.Windows.Forms.ToolTip(this.components);
            this.m_sRFGenDelay = new System.Windows.Forms.TextBox();
            this.m_btnStopFreqSweep = new System.Windows.Forms.Button();
            this.m_sRFGenFreqSweepSteps = new System.Windows.Forms.TextBox();
            this.m_btnStartFreqSweepContinuous = new System.Windows.Forms.Button();
            this.m_sRFGenFreqSweepStart = new System.Windows.Forms.TextBox();
            this.m_sRFGenFreqSweepStop = new System.Windows.Forms.TextBox();
            this.m_groupControl_RFEGen_FrequencySweep = new RFEClientControls.GroupControl_RFEGen_FrequencySweep();
            this.m_labRFGenDelay = new System.Windows.Forms.Label();
            this.m_labRFGenStartFreq = new System.Windows.Forms.Label();
            this.m_labRFGenStopFreq = new System.Windows.Forms.Label();
            this.m_labRFGenSteps = new System.Windows.Forms.Label();
            this.m_groupControl_RFEGen_FrequencySweep.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_ToolTips
            // 
            this.m_ToolTips.AutomaticDelay = 1500;
            this.m_ToolTips.AutoPopDelay = 15000;
            this.m_ToolTips.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.m_ToolTips.ForeColor = System.Drawing.Color.Blue;
            this.m_ToolTips.InitialDelay = 500;
            this.m_ToolTips.ReshowDelay = 300;
            // 
            // m_sRFGenDelay
            // 
            this.m_sRFGenDelay.BackColor = System.Drawing.Color.RoyalBlue;
            this.m_sRFGenDelay.Font = new System.Drawing.Font("Digital-7", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_sRFGenDelay.ForeColor = System.Drawing.Color.White;
            this.m_sRFGenDelay.Location = new System.Drawing.Point(61, 81);
            this.m_sRFGenDelay.MaxLength = 5;
            this.m_sRFGenDelay.Name = "m_sRFGenDelay";
            this.m_sRFGenDelay.Size = new System.Drawing.Size(90, 26);
            this.m_sRFGenDelay.TabIndex = 28;
            this.m_sRFGenDelay.Text = "0";
            this.m_sRFGenDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_ToolTips.SetToolTip(this.m_sRFGenDelay, "Delay in seconds between Sweep changes. Every sweep value will remain this select" +
        "ed time active before moving to the next value. Max 50 sec");
            this.m_sRFGenDelay.WordWrap = false;
            this.m_sRFGenDelay.Leave += new System.EventHandler(this.OnRFGenFreqSweepDelay_Leave);
            // 
            // m_btnStopFreqSweep
            // 
            this.m_btnStopFreqSweep.AutoSize = true;
            this.m_btnStopFreqSweep.Enabled = false;
            this.m_btnStopFreqSweep.Location = new System.Drawing.Point(157, 50);
            this.m_btnStopFreqSweep.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.m_btnStopFreqSweep.Name = "m_btnStopFreqSweep";
            this.m_btnStopFreqSweep.Size = new System.Drawing.Size(89, 26);
            this.m_btnStopFreqSweep.TabIndex = 13;
            this.m_btnStopFreqSweep.Text = "Stop sweep";
            this.m_ToolTips.SetToolTip(this.m_btnStopFreqSweep, "Stop Frequency Sweep");
            this.m_btnStopFreqSweep.UseVisualStyleBackColor = true;
            this.m_btnStopFreqSweep.Click += new System.EventHandler(this.OnStopFreqSweep_Click);
            // 
            // m_sRFGenFreqSweepSteps
            // 
            this.m_sRFGenFreqSweepSteps.BackColor = System.Drawing.Color.RoyalBlue;
            this.m_sRFGenFreqSweepSteps.Font = new System.Drawing.Font("Digital-7", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_sRFGenFreqSweepSteps.ForeColor = System.Drawing.Color.White;
            this.m_sRFGenFreqSweepSteps.Location = new System.Drawing.Point(212, 80);
            this.m_sRFGenFreqSweepSteps.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.m_sRFGenFreqSweepSteps.MaxLength = 4;
            this.m_sRFGenFreqSweepSteps.Name = "m_sRFGenFreqSweepSteps";
            this.m_sRFGenFreqSweepSteps.Size = new System.Drawing.Size(63, 26);
            this.m_sRFGenFreqSweepSteps.TabIndex = 6;
            this.m_sRFGenFreqSweepSteps.Text = "1";
            this.m_sRFGenFreqSweepSteps.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_ToolTips.SetToolTip(this.m_sRFGenFreqSweepSteps, "Total steps to run in Sweep and Tracking mode");
            this.m_sRFGenFreqSweepSteps.Leave += new System.EventHandler(this.OnRFGenFreqSweepSteps_Leave);
            // 
            // m_btnStartFreqSweepContinuous
            // 
            this.m_btnStartFreqSweepContinuous.AutoSize = true;
            this.m_btnStartFreqSweepContinuous.Location = new System.Drawing.Point(157, 24);
            this.m_btnStartFreqSweepContinuous.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.m_btnStartFreqSweepContinuous.Name = "m_btnStartFreqSweepContinuous";
            this.m_btnStartFreqSweepContinuous.Size = new System.Drawing.Size(142, 26);
            this.m_btnStartFreqSweepContinuous.TabIndex = 12;
            this.m_btnStartFreqSweepContinuous.Text = "Start Sweep...";
            this.m_ToolTips.SetToolTip(this.m_btnStartFreqSweepContinuous, "Start Frequency Sweep");
            this.m_btnStartFreqSweepContinuous.UseVisualStyleBackColor = true;
            this.m_btnStartFreqSweepContinuous.Click += new System.EventHandler(this.OnStartFreqSweep_Click);
            // 
            // m_sRFGenFreqSweepStart
            // 
            this.m_sRFGenFreqSweepStart.BackColor = System.Drawing.Color.RoyalBlue;
            this.m_sRFGenFreqSweepStart.Font = new System.Drawing.Font("Digital-7", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_sRFGenFreqSweepStart.ForeColor = System.Drawing.Color.White;
            this.m_sRFGenFreqSweepStart.Location = new System.Drawing.Point(61, 24);
            this.m_sRFGenFreqSweepStart.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.m_sRFGenFreqSweepStart.Name = "m_sRFGenFreqSweepStart";
            this.m_sRFGenFreqSweepStart.Size = new System.Drawing.Size(90, 26);
            this.m_sRFGenFreqSweepStart.TabIndex = 1;
            this.m_sRFGenFreqSweepStart.Text = "2430.000";
            this.m_sRFGenFreqSweepStart.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_ToolTips.SetToolTip(this.m_sRFGenFreqSweepStart, "Start frequency in MHZ - Sweep and Tracking mode (always lower than Stop value)");
            this.m_sRFGenFreqSweepStart.WordWrap = false;
            this.m_sRFGenFreqSweepStart.Leave += new System.EventHandler(this.OnRFGenFreqSweepStart_Leave);
            // 
            // m_sRFGenFreqSweepStop
            // 
            this.m_sRFGenFreqSweepStop.BackColor = System.Drawing.Color.RoyalBlue;
            this.m_sRFGenFreqSweepStop.Font = new System.Drawing.Font("Digital-7", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_sRFGenFreqSweepStop.ForeColor = System.Drawing.Color.White;
            this.m_sRFGenFreqSweepStop.Location = new System.Drawing.Point(61, 49);
            this.m_sRFGenFreqSweepStop.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.m_sRFGenFreqSweepStop.Name = "m_sRFGenFreqSweepStop";
            this.m_sRFGenFreqSweepStop.Size = new System.Drawing.Size(90, 26);
            this.m_sRFGenFreqSweepStop.TabIndex = 3;
            this.m_sRFGenFreqSweepStop.Text = "2435.000";
            this.m_sRFGenFreqSweepStop.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_ToolTips.SetToolTip(this.m_sRFGenFreqSweepStop, "Stop frequency in MHZ - Sweep and Tracking mode (always higher than Start value)");
            this.m_sRFGenFreqSweepStop.Leave += new System.EventHandler(this.OnRFGenFreqSweepStop_Leave);
            // 
            // m_groupControl_RFEGen_FrequencySweep
            // 
            this.m_groupControl_RFEGen_FrequencySweep.AutoSize = true;
            this.m_groupControl_RFEGen_FrequencySweep.Controls.Add(this.m_sRFGenDelay);
            this.m_groupControl_RFEGen_FrequencySweep.Controls.Add(this.m_labRFGenDelay);
            this.m_groupControl_RFEGen_FrequencySweep.Controls.Add(this.m_btnStopFreqSweep);
            this.m_groupControl_RFEGen_FrequencySweep.Controls.Add(this.m_labRFGenStartFreq);
            this.m_groupControl_RFEGen_FrequencySweep.Controls.Add(this.m_sRFGenFreqSweepSteps);
            this.m_groupControl_RFEGen_FrequencySweep.Controls.Add(this.m_btnStartFreqSweepContinuous);
            this.m_groupControl_RFEGen_FrequencySweep.Controls.Add(this.m_labRFGenStopFreq);
            this.m_groupControl_RFEGen_FrequencySweep.Controls.Add(this.m_sRFGenFreqSweepStart);
            this.m_groupControl_RFEGen_FrequencySweep.Controls.Add(this.m_sRFGenFreqSweepStop);
            this.m_groupControl_RFEGen_FrequencySweep.Controls.Add(this.m_labRFGenSteps);
            this.m_groupControl_RFEGen_FrequencySweep.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.m_groupControl_RFEGen_FrequencySweep.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.m_groupControl_RFEGen_FrequencySweep.Location = new System.Drawing.Point(0, 0);
            this.m_groupControl_RFEGen_FrequencySweep.Margin = new System.Windows.Forms.Padding(0);
            this.m_groupControl_RFEGen_FrequencySweep.Name = "m_groupControl_RFEGen_FrequencySweep";
            this.m_groupControl_RFEGen_FrequencySweep.Padding = new System.Windows.Forms.Padding(0);
            this.m_groupControl_RFEGen_FrequencySweep.Size = new System.Drawing.Size(302, 126);
            this.m_groupControl_RFEGen_FrequencySweep.TabIndex = 61;
            this.m_groupControl_RFEGen_FrequencySweep.TabStop = false;
            this.m_groupControl_RFEGen_FrequencySweep.Text = "Signal Generator Frequency Sweep";
            // 
            // m_labRFGenDelay
            // 
            this.m_labRFGenDelay.AutoSize = true;
            this.m_labRFGenDelay.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_labRFGenDelay.ForeColor = System.Drawing.Color.DarkBlue;
            this.m_labRFGenDelay.Location = new System.Drawing.Point(5, 86);
            this.m_labRFGenDelay.Name = "m_labRFGenDelay";
            this.m_labRFGenDelay.Size = new System.Drawing.Size(50, 16);
            this.m_labRFGenDelay.TabIndex = 27;
            this.m_labRFGenDelay.Text = "DELAY";
            this.m_labRFGenDelay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_labRFGenStartFreq
            // 
            this.m_labRFGenStartFreq.AutoSize = true;
            this.m_labRFGenStartFreq.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_labRFGenStartFreq.ForeColor = System.Drawing.Color.DarkBlue;
            this.m_labRFGenStartFreq.Location = new System.Drawing.Point(5, 24);
            this.m_labRFGenStartFreq.Name = "m_labRFGenStartFreq";
            this.m_labRFGenStartFreq.Size = new System.Drawing.Size(49, 16);
            this.m_labRFGenStartFreq.TabIndex = 1;
            this.m_labRFGenStartFreq.Text = "START";
            this.m_labRFGenStartFreq.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_labRFGenStopFreq
            // 
            this.m_labRFGenStopFreq.AutoSize = true;
            this.m_labRFGenStopFreq.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.m_labRFGenStopFreq.ForeColor = System.Drawing.Color.DarkBlue;
            this.m_labRFGenStopFreq.Location = new System.Drawing.Point(5, 50);
            this.m_labRFGenStopFreq.Name = "m_labRFGenStopFreq";
            this.m_labRFGenStopFreq.Size = new System.Drawing.Size(40, 16);
            this.m_labRFGenStopFreq.TabIndex = 3;
            this.m_labRFGenStopFreq.Text = "STOP";
            this.m_labRFGenStopFreq.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_labRFGenSteps
            // 
            this.m_labRFGenSteps.AutoSize = true;
            this.m_labRFGenSteps.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.m_labRFGenSteps.ForeColor = System.Drawing.Color.DarkBlue;
            this.m_labRFGenSteps.Location = new System.Drawing.Point(160, 85);
            this.m_labRFGenSteps.Name = "m_labRFGenSteps";
            this.m_labRFGenSteps.Size = new System.Drawing.Size(46, 16);
            this.m_labRFGenSteps.TabIndex = 11;
            this.m_labRFGenSteps.Text = "STEPS";
            this.m_labRFGenSteps.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ToolGroupRFEGenFreqSweep
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.m_groupControl_RFEGen_FrequencySweep);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ToolGroupRFEGenFreqSweep";
            this.Size = new System.Drawing.Size(302, 126);
            this.m_ToolTips.SetToolTip(this, "Sweep frequency settings");
            this.m_groupControl_RFEGen_FrequencySweep.ResumeLayout(false);
            this.m_groupControl_RFEGen_FrequencySweep.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
        internal System.Windows.Forms.Button m_btnStopFreqSweep;
        internal System.Windows.Forms.TextBox m_sRFGenFreqSweepSteps;
        internal System.Windows.Forms.Label m_labRFGenStartFreq;
        internal System.Windows.Forms.TextBox m_sRFGenFreqSweepStart;
        internal System.Windows.Forms.Label m_labRFGenSteps;
        internal System.Windows.Forms.TextBox m_sRFGenFreqSweepStop;
        internal System.Windows.Forms.Label m_labRFGenStopFreq;
        internal System.Windows.Forms.Button m_btnStartFreqSweepContinuous;
        private ToolTip m_ToolTips;
        internal TextBox m_sRFGenDelay;
        internal Label m_labRFGenDelay;
        private GroupControl_RFEGen_FrequencySweep m_groupControl_RFEGen_FrequencySweep;
    }
}
