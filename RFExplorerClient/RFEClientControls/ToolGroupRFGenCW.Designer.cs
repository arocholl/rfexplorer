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
    partial class ToolGroupRFGenCW
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
            this.m_GroupControl = new RFEClientControls.GroupControl_RFGenCW();
            this.m_btnRFEGenCWStart = new System.Windows.Forms.Button();
            this.m_labRFGenCWFreq = new System.Windows.Forms.Label();
            this.m_sRFGenFreqCW = new System.Windows.Forms.TextBox();
            this.m_btnRFEGenCWStop = new System.Windows.Forms.Button();
            this.m_labRFPowerON = new System.Windows.Forms.Label();
            this.m_comboRFGenPowerCW = new System.Windows.Forms.ComboBox();
            this.m_labRFGenPower = new System.Windows.Forms.Label();
            this.m_ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.m_GroupControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_GroupControl
            // 
            this.m_GroupControl.AutoSize = true;
            this.m_GroupControl.Controls.Add(this.m_btnRFEGenCWStart);
            this.m_GroupControl.Controls.Add(this.m_labRFGenCWFreq);
            this.m_GroupControl.Controls.Add(this.m_sRFGenFreqCW);
            this.m_GroupControl.Controls.Add(this.m_btnRFEGenCWStop);
            this.m_GroupControl.Controls.Add(this.m_labRFPowerON);
            this.m_GroupControl.Controls.Add(this.m_comboRFGenPowerCW);
            this.m_GroupControl.Controls.Add(this.m_labRFGenPower);
            this.m_GroupControl.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.m_GroupControl.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.m_GroupControl.Location = new System.Drawing.Point(0, 0);
            this.m_GroupControl.Margin = new System.Windows.Forms.Padding(0);
            this.m_GroupControl.Name = "m_GroupControl";
            this.m_GroupControl.Padding = new System.Windows.Forms.Padding(0);
            this.m_GroupControl.Size = new System.Drawing.Size(226, 116);
            this.m_GroupControl.TabIndex = 0;
            this.m_GroupControl.TabStop = false;
            this.m_GroupControl.Text = "Signal Generator CW carrier";
            // 
            // m_btnRFEGenCWStart
            // 
            this.m_btnRFEGenCWStart.AutoSize = true;
            this.m_btnRFEGenCWStart.Location = new System.Drawing.Point(164, 23);
            this.m_btnRFEGenCWStart.Name = "m_btnRFEGenCWStart";
            this.m_btnRFEGenCWStart.Size = new System.Drawing.Size(53, 26);
            this.m_btnRFEGenCWStart.TabIndex = 21;
            this.m_btnRFEGenCWStart.Text = "Start...";
            this.m_ToolTip.SetToolTip(this.m_btnRFEGenCWStart, "Start Signal Generator with selected values");
            this.m_btnRFEGenCWStart.UseVisualStyleBackColor = true;
            this.m_btnRFEGenCWStart.Click += new System.EventHandler(this.OnStartClick);
            // 
            // m_labRFGenCWFreq
            // 
            this.m_labRFGenCWFreq.AutoSize = true;
            this.m_labRFGenCWFreq.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_labRFGenCWFreq.ForeColor = System.Drawing.Color.DarkBlue;
            this.m_labRFGenCWFreq.Location = new System.Drawing.Point(3, 29);
            this.m_labRFGenCWFreq.Name = "m_labRFGenCWFreq";
            this.m_labRFGenCWFreq.Size = new System.Drawing.Size(64, 16);
            this.m_labRFGenCWFreq.TabIndex = 17;
            this.m_labRFGenCWFreq.Text = "CW FREQ";
            this.m_labRFGenCWFreq.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_sRFGenFreqCW
            // 
            this.m_sRFGenFreqCW.BackColor = System.Drawing.Color.RoyalBlue;
            this.m_sRFGenFreqCW.Font = new System.Drawing.Font("Digital-7", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_sRFGenFreqCW.ForeColor = System.Drawing.Color.White;
            this.m_sRFGenFreqCW.Location = new System.Drawing.Point(71, 24);
            this.m_sRFGenFreqCW.Name = "m_sRFGenFreqCW";
            this.m_sRFGenFreqCW.Size = new System.Drawing.Size(90, 26);
            this.m_sRFGenFreqCW.TabIndex = 18;
            this.m_sRFGenFreqCW.Text = "2430.000";
            this.m_sRFGenFreqCW.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_ToolTip.SetToolTip(this.m_sRFGenFreqCW, "CW frequency value for Signal Generator");
            this.m_sRFGenFreqCW.WordWrap = false;
            this.m_sRFGenFreqCW.TextChanged += new System.EventHandler(this.OnTextChanged);
            this.m_sRFGenFreqCW.Leave += new System.EventHandler(this.OnRFGenFreqLeave);
            // 
            // m_btnRFEGenCWStop
            // 
            this.m_btnRFEGenCWStop.AutoSize = true;
            this.m_btnRFEGenCWStop.Location = new System.Drawing.Point(164, 52);
            this.m_btnRFEGenCWStop.Name = "m_btnRFEGenCWStop";
            this.m_btnRFEGenCWStop.Size = new System.Drawing.Size(39, 26);
            this.m_btnRFEGenCWStop.TabIndex = 20;
            this.m_btnRFEGenCWStop.Text = "Stop";
            this.m_ToolTip.SetToolTip(this.m_btnRFEGenCWStop, "Stop Signal Generator output");
            this.m_btnRFEGenCWStop.UseVisualStyleBackColor = true;
            this.m_btnRFEGenCWStop.Click += new System.EventHandler(this.OnStopClick);
            // 
            // m_labRFPowerON
            // 
            this.m_labRFPowerON.AutoSize = true;
            this.m_labRFPowerON.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_labRFPowerON.Location = new System.Drawing.Point(43, 80);
            this.m_labRFPowerON.Name = "m_labRFPowerON";
            this.m_labRFPowerON.Size = new System.Drawing.Size(137, 22);
            this.m_labRFPowerON.TabIndex = 23;
            this.m_labRFPowerON.Text = "RF Power OFF";
            this.m_labRFPowerON.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.m_ToolTip.SetToolTip(this.m_labRFPowerON, "This label will indicate whether the Signal Generator has active power or not");
            // 
            // m_comboRFGenPowerCW
            // 
            this.m_comboRFGenPowerCW.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_comboRFGenPowerCW.FormattingEnabled = true;
            this.m_comboRFGenPowerCW.Items.AddRange(new object[] {
            "-40dBm",
            "-37dBm",
            "-34dBm",
            "-30dBm",
            "-10dBm",
            "-7dBm",
            "-4dBm",
            "0dBm"});
            this.m_comboRFGenPowerCW.Location = new System.Drawing.Point(71, 55);
            this.m_comboRFGenPowerCW.Name = "m_comboRFGenPowerCW";
            this.m_comboRFGenPowerCW.Size = new System.Drawing.Size(90, 21);
            this.m_comboRFGenPowerCW.TabIndex = 22;
            this.m_ToolTip.SetToolTip(this.m_comboRFGenPowerCW, "Nominal power selection for Signal Generator output");
            this.m_comboRFGenPowerCW.SelectedIndexChanged += new System.EventHandler(this.OnComboRFPowerSelectedIndexChanged);
            // 
            // m_labRFGenPower
            // 
            this.m_labRFGenPower.AutoSize = true;
            this.m_labRFGenPower.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.m_labRFGenPower.ForeColor = System.Drawing.Color.DarkBlue;
            this.m_labRFGenPower.Location = new System.Drawing.Point(3, 57);
            this.m_labRFGenPower.Name = "m_labRFGenPower";
            this.m_labRFGenPower.Size = new System.Drawing.Size(54, 16);
            this.m_labRFGenPower.TabIndex = 19;
            this.m_labRFGenPower.Text = "POWER";
            this.m_labRFGenPower.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            // ToolGroupRFGenCW
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.m_GroupControl);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ToolGroupRFGenCW";
            this.Size = new System.Drawing.Size(226, 116);
            this.Load += new System.EventHandler(this.ToolGroupRFGenCw_Load);
            this.m_GroupControl.ResumeLayout(false);
            this.m_GroupControl.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GroupControl_RFGenCW m_GroupControl;
        internal System.Windows.Forms.Button m_btnRFEGenCWStart;
        internal System.Windows.Forms.Label m_labRFGenCWFreq;
        internal System.Windows.Forms.TextBox m_sRFGenFreqCW;
        internal System.Windows.Forms.Button m_btnRFEGenCWStop;
        internal System.Windows.Forms.Label m_labRFPowerON;
        internal System.Windows.Forms.ComboBox m_comboRFGenPowerCW;
        internal System.Windows.Forms.Label m_labRFGenPower;
        private System.Windows.Forms.ToolTip m_ToolTip;
    }
}
