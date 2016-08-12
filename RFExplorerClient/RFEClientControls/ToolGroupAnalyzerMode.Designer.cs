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
    partial class ToolGroupAnalyzerMode
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
            this.m_numericIterations = new System.Windows.Forms.NumericUpDown();
            this.m_NumericSweepIndex = new System.Windows.Forms.NumericUpDown();
            this.m_chkRunMode = new System.Windows.Forms.CheckBox();
            this.m_chkHoldMode = new System.Windows.Forms.CheckBox();
            this.m_groupControl_DataFeed = new RFEClientControls.GroupControl_AnalyzerMode();
            this.m_labIterations = new System.Windows.Forms.Label();
            this.m_labDataSample = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.m_numericIterations)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_NumericSweepIndex)).BeginInit();
            this.m_groupControl_DataFeed.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_ToolTip
            // 
            this.m_ToolTip.AutomaticDelay = 1500;
            this.m_ToolTip.AutoPopDelay = 15000;
            this.m_ToolTip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(244)))));
            this.m_ToolTip.ForeColor = System.Drawing.Color.Blue;
            this.m_ToolTip.InitialDelay = 500;
            this.m_ToolTip.ReshowDelay = 300;
            // 
            // m_numericIterations
            // 
            this.m_numericIterations.AutoSize = true;
            this.m_numericIterations.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_numericIterations.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_numericIterations.Location = new System.Drawing.Point(76, 76);
            this.m_numericIterations.MaximumSize = new System.Drawing.Size(86, 0);
            this.m_numericIterations.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.m_numericIterations.Name = "m_numericIterations";
            this.m_numericIterations.Size = new System.Drawing.Size(86, 23);
            this.m_numericIterations.TabIndex = 17;
            this.m_numericIterations.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_ToolTip.SetToolTip(this.m_numericIterations, "Total number of iterations to perform calculations for (Max, Min and Avg)");
            this.m_numericIterations.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.m_numericIterations.ValueChanged += new System.EventHandler(this.OnNumericSweepIndex_ValueChanged);
            // 
            // m_NumericSweepIndex
            // 
            this.m_NumericSweepIndex.AutoSize = true;
            this.m_NumericSweepIndex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_NumericSweepIndex.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.m_NumericSweepIndex.Location = new System.Drawing.Point(76, 51);
            this.m_NumericSweepIndex.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.m_NumericSweepIndex.MaximumSize = new System.Drawing.Size(86, 0);
            this.m_NumericSweepIndex.Name = "m_NumericSweepIndex";
            this.m_NumericSweepIndex.Size = new System.Drawing.Size(86, 23);
            this.m_NumericSweepIndex.TabIndex = 16;
            this.m_NumericSweepIndex.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_ToolTip.SetToolTip(this.m_NumericSweepIndex, "Current sample being displayed on Screen");
            this.m_NumericSweepIndex.ValueChanged += new System.EventHandler(this.OnNumericSweepIndex_ValueChanged);
            // 
            // m_chkRunMode
            // 
            this.m_chkRunMode.Appearance = System.Windows.Forms.Appearance.Button;
            this.m_chkRunMode.AutoSize = true;
            this.m_chkRunMode.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.m_chkRunMode.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.m_chkRunMode.Location = new System.Drawing.Point(3, 23);
            this.m_chkRunMode.MinimumSize = new System.Drawing.Size(72, 0);
            this.m_chkRunMode.Name = "m_chkRunMode";
            this.m_chkRunMode.Size = new System.Drawing.Size(72, 23);
            this.m_chkRunMode.TabIndex = 14;
            this.m_chkRunMode.Text = "RUN";
            this.m_chkRunMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.m_ToolTip.SetToolTip(this.m_chkRunMode, "Restore data receive with RF Explorer");
            this.m_chkRunMode.UseVisualStyleBackColor = true;
            this.m_chkRunMode.CheckedChanged += new System.EventHandler(this.OnRunMode_CheckedChanged);
            // 
            // m_chkHoldMode
            // 
            this.m_chkHoldMode.Appearance = System.Windows.Forms.Appearance.Button;
            this.m_chkHoldMode.AutoSize = true;
            this.m_chkHoldMode.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.m_chkHoldMode.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.m_chkHoldMode.Location = new System.Drawing.Point(76, 23);
            this.m_chkHoldMode.MinimumSize = new System.Drawing.Size(87, 23);
            this.m_chkHoldMode.Name = "m_chkHoldMode";
            this.m_chkHoldMode.Size = new System.Drawing.Size(87, 23);
            this.m_chkHoldMode.TabIndex = 15;
            this.m_chkHoldMode.Text = "HOLD";
            this.m_chkHoldMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.m_ToolTip.SetToolTip(this.m_chkHoldMode, "Stops receiving data from RF Explorer");
            this.m_chkHoldMode.UseVisualStyleBackColor = false;
            this.m_chkHoldMode.CheckedChanged += new System.EventHandler(this.OnHoldMode_CheckedChanged);
            // 
            // m_groupControl_DataFeed
            // 
            this.m_groupControl_DataFeed.AutoSize = true;
            this.m_groupControl_DataFeed.Controls.Add(this.m_numericIterations);
            this.m_groupControl_DataFeed.Controls.Add(this.m_labIterations);
            this.m_groupControl_DataFeed.Controls.Add(this.m_NumericSweepIndex);
            this.m_groupControl_DataFeed.Controls.Add(this.m_chkRunMode);
            this.m_groupControl_DataFeed.Controls.Add(this.m_labDataSample);
            this.m_groupControl_DataFeed.Controls.Add(this.m_chkHoldMode);
            this.m_groupControl_DataFeed.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.m_groupControl_DataFeed.Location = new System.Drawing.Point(0, 0);
            this.m_groupControl_DataFeed.Margin = new System.Windows.Forms.Padding(0);
            this.m_groupControl_DataFeed.Name = "m_groupControl_DataFeed";
            this.m_groupControl_DataFeed.Padding = new System.Windows.Forms.Padding(0);
            this.m_groupControl_DataFeed.Size = new System.Drawing.Size(169, 116);
            this.m_groupControl_DataFeed.TabIndex = 52;
            this.m_groupControl_DataFeed.TabStop = false;
            this.m_groupControl_DataFeed.Text = "Mode";
            // 
            // m_labIterations
            // 
            this.m_labIterations.AutoSize = true;
            this.m_labIterations.CausesValidation = false;
            this.m_labIterations.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.m_labIterations.Location = new System.Drawing.Point(3, 80);
            this.m_labIterations.Name = "m_labIterations";
            this.m_labIterations.Size = new System.Drawing.Size(58, 13);
            this.m_labIterations.TabIndex = 50;
            this.m_labIterations.Text = "Iterations:";
            this.m_labIterations.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // m_labDataSample
            // 
            this.m_labDataSample.AutoSize = true;
            this.m_labDataSample.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.m_labDataSample.Location = new System.Drawing.Point(3, 55);
            this.m_labDataSample.Name = "m_labDataSample";
            this.m_labDataSample.Size = new System.Drawing.Size(71, 13);
            this.m_labDataSample.TabIndex = 49;
            this.m_labDataSample.Text = "Data Sample:";
            this.m_labDataSample.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ToolGroupAnalyzerMode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.m_groupControl_DataFeed);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MinimumSize = new System.Drawing.Size(169, 116);
            this.Name = "ToolGroupAnalyzerMode";
            this.Size = new System.Drawing.Size(169, 116);
            ((System.ComponentModel.ISupportInitialize)(this.m_numericIterations)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_NumericSweepIndex)).EndInit();
            this.m_groupControl_DataFeed.ResumeLayout(false);
            this.m_groupControl_DataFeed.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        internal System.Windows.Forms.NumericUpDown m_numericIterations;
        internal System.Windows.Forms.Label m_labIterations;
        internal System.Windows.Forms.NumericUpDown m_NumericSweepIndex;
        internal System.Windows.Forms.CheckBox m_chkRunMode;
        internal System.Windows.Forms.Label m_labDataSample;
        internal System.Windows.Forms.CheckBox m_chkHoldMode;
        private GroupControl_AnalyzerMode m_groupControl_DataFeed;
        private ToolTip m_ToolTip;
    }
}
