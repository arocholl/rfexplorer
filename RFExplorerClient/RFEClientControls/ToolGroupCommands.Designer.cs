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
    partial class ToolGroupCommands
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
            this.m_groupControl_Commands = new RFEClientControls.GroupControl_Commands();
            this.m_comboStdCmd = new System.Windows.Forms.ComboBox();
            this.m_comboCustomCommand = new System.Windows.Forms.ComboBox();
            this.m_btnSendAnalyzerCmd = new System.Windows.Forms.Button();
            this.m_label12 = new System.Windows.Forms.Label();
            this.m_label11 = new System.Windows.Forms.Label();
            this.m_btnSendAnalyzerCustomCmd = new System.Windows.Forms.Button();
            this.m_chkDebugTraces = new System.Windows.Forms.CheckBox();
            this.m_btnSendGenCmd = new System.Windows.Forms.Button();
            this.m_btnSendGenCustomCmd = new System.Windows.Forms.Button();
            this.m_groupControl_Commands.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_groupControl_Commands
            // 
            this.m_groupControl_Commands.Controls.Add(this.m_comboStdCmd);
            this.m_groupControl_Commands.Controls.Add(this.m_comboCustomCommand);
            this.m_groupControl_Commands.Controls.Add(this.m_btnSendAnalyzerCmd);
            this.m_groupControl_Commands.Controls.Add(this.m_label12);
            this.m_groupControl_Commands.Controls.Add(this.m_label11);
            this.m_groupControl_Commands.Controls.Add(this.m_btnSendAnalyzerCustomCmd);
            this.m_groupControl_Commands.Controls.Add(this.m_chkDebugTraces);
            this.m_groupControl_Commands.Controls.Add(this.m_btnSendGenCmd);
            this.m_groupControl_Commands.Controls.Add(this.m_btnSendGenCustomCmd);
            this.m_groupControl_Commands.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.m_groupControl_Commands.Location = new System.Drawing.Point(0, 0);
            this.m_groupControl_Commands.Margin = new System.Windows.Forms.Padding(0);
            this.m_groupControl_Commands.Name = "m_groupControl_Commands";
            this.m_groupControl_Commands.Padding = new System.Windows.Forms.Padding(0);
            this.m_groupControl_Commands.Size = new System.Drawing.Size(572, 116);
            this.m_groupControl_Commands.TabIndex = 51;
            this.m_groupControl_Commands.TabStop = false;
            this.m_groupControl_Commands.Text = "Advanced Remote Command (developer only)";
            // 
            // m_comboStdCmd
            // 
            this.m_comboStdCmd.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_comboStdCmd.FormattingEnabled = true;
            this.m_comboStdCmd.Items.AddRange(new object[] {
            "Baudrate 115200 : c8",
            "Baudrate 19200 : c5",
            "Baudrate 2400 : c2",
            "Baudrate 500K : c0",
            "Dump screen OFF : D0",
            "Dump screen ON : D1",
            "LCD OFF : L0",
            "LCD ON : L1",
            "Request Configuration : C0",
            "Restart RFE: r",
            "RFE on hold : CH",
            "Shutdown RFE : S"});
            this.m_comboStdCmd.Location = new System.Drawing.Point(114, 54);
            this.m_comboStdCmd.Name = "m_comboStdCmd";
            this.m_comboStdCmd.Size = new System.Drawing.Size(200, 21);
            this.m_comboStdCmd.Sorted = true;
            this.m_comboStdCmd.TabIndex = 17;
            // 
            // m_comboCustomCommand
            // 
            this.m_comboCustomCommand.FormattingEnabled = true;
            this.m_comboCustomCommand.Location = new System.Drawing.Point(113, 85);
            this.m_comboCustomCommand.Name = "m_comboCustomCommand";
            this.m_comboCustomCommand.Size = new System.Drawing.Size(200, 21);
            this.m_comboCustomCommand.TabIndex = 14;
            // 
            // m_btnSendAnalyzerCmd
            // 
            this.m_btnSendAnalyzerCmd.Location = new System.Drawing.Point(324, 53);
            this.m_btnSendAnalyzerCmd.Name = "m_btnSendAnalyzerCmd";
            this.m_btnSendAnalyzerCmd.Size = new System.Drawing.Size(120, 23);
            this.m_btnSendAnalyzerCmd.TabIndex = 13;
            this.m_btnSendAnalyzerCmd.Text = "Send Analyzer";
            this.m_btnSendAnalyzerCmd.UseVisualStyleBackColor = true;
            this.m_btnSendAnalyzerCmd.Click += new System.EventHandler(this.OnSendAnalyzerCmd_Click);
            // 
            // m_label12
            // 
            this.m_label12.AutoSize = true;
            this.m_label12.Location = new System.Drawing.Point(7, 56);
            this.m_label12.Name = "m_label12";
            this.m_label12.Size = new System.Drawing.Size(100, 13);
            this.m_label12.TabIndex = 16;
            this.m_label12.Text = "Standard Command";
            this.m_label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // m_label11
            // 
            this.m_label11.AutoSize = true;
            this.m_label11.Location = new System.Drawing.Point(7, 88);
            this.m_label11.Name = "m_label11";
            this.m_label11.Size = new System.Drawing.Size(92, 13);
            this.m_label11.TabIndex = 15;
            this.m_label11.Text = "Custom Command";
            this.m_label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // m_btnSendAnalyzerCustomCmd
            // 
            this.m_btnSendAnalyzerCustomCmd.Location = new System.Drawing.Point(324, 84);
            this.m_btnSendAnalyzerCustomCmd.Name = "m_btnSendAnalyzerCustomCmd";
            this.m_btnSendAnalyzerCustomCmd.Size = new System.Drawing.Size(120, 23);
            this.m_btnSendAnalyzerCustomCmd.TabIndex = 18;
            this.m_btnSendAnalyzerCustomCmd.Text = "Send Analyzer";
            this.m_btnSendAnalyzerCustomCmd.UseVisualStyleBackColor = true;
            this.m_btnSendAnalyzerCustomCmd.Click += new System.EventHandler(this.OnSendAnalyzerCustomCmd_Click);
            // 
            // m_chkDebugTraces
            // 
            this.m_chkDebugTraces.AutoSize = true;
            this.m_chkDebugTraces.Location = new System.Drawing.Point(9, 26);
            this.m_chkDebugTraces.Name = "m_chkDebugTraces";
            this.m_chkDebugTraces.Size = new System.Drawing.Size(285, 17);
            this.m_chkDebugTraces.TabIndex = 19;
            this.m_chkDebugTraces.Text = "Detailed debug info (only if required to diagnose issues)";
            this.m_chkDebugTraces.UseVisualStyleBackColor = true;
            this.m_chkDebugTraces.CheckedChanged += new System.EventHandler(this.OnDebug_CheckedChanged);
            // 
            // m_btnSendGenCmd
            // 
            this.m_btnSendGenCmd.Location = new System.Drawing.Point(450, 53);
            this.m_btnSendGenCmd.Name = "m_btnSendGenCmd";
            this.m_btnSendGenCmd.Size = new System.Drawing.Size(120, 23);
            this.m_btnSendGenCmd.TabIndex = 20;
            this.m_btnSendGenCmd.Text = "Send Generator";
            this.m_btnSendGenCmd.UseVisualStyleBackColor = true;
            this.m_btnSendGenCmd.Click += new System.EventHandler(this.OnSendGenCmd_Click);
            // 
            // m_btnSendGenCustomCmd
            // 
            this.m_btnSendGenCustomCmd.Location = new System.Drawing.Point(450, 84);
            this.m_btnSendGenCustomCmd.Name = "m_btnSendGenCustomCmd";
            this.m_btnSendGenCustomCmd.Size = new System.Drawing.Size(120, 23);
            this.m_btnSendGenCustomCmd.TabIndex = 21;
            this.m_btnSendGenCustomCmd.Text = "Send Generator";
            this.m_btnSendGenCustomCmd.UseVisualStyleBackColor = true;
            this.m_btnSendGenCustomCmd.Click += new System.EventHandler(this.OnSendCustomGenCmd_Click);
            // 
            // ToolGroupCommands
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_groupControl_Commands);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MaximumSize = new System.Drawing.Size(572, 116);
            this.Name = "ToolGroupCommands";
            this.Size = new System.Drawing.Size(572, 116);
            this.m_groupControl_Commands.ResumeLayout(false);
            this.m_groupControl_Commands.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private GroupControl_Commands m_groupControl_Commands;
        internal ComboBox m_comboCustomCommand;
        internal ComboBox m_comboStdCmd;
        internal Button m_btnSendAnalyzerCmd;
        internal Label m_label12;
        internal Label m_label11;
        internal Button m_btnSendAnalyzerCustomCmd;
        internal CheckBox m_chkDebugTraces;
        internal Button m_btnSendGenCmd;
        internal Button m_btnSendGenCustomCmd;
    }
}
