namespace RFEClientControls
{
    partial class ToolGroupRFEGenAmpSweep
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
            this.m_GroupControl = new RFEClientControls.GroupControl_RFGen_AmplitudeSweep();
            this.m_sRFGenDelay = new System.Windows.Forms.TextBox();
            this.m_labRFGenDelay = new System.Windows.Forms.Label();
            this.m_comboRFGenPowerStop = new System.Windows.Forms.ComboBox();
            this.m_labRFGenPowerStop = new System.Windows.Forms.Label();
            this.m_btnRFEGenStartAmplitudeSweep = new System.Windows.Forms.Button();
            this.m_labRFGenSteps = new System.Windows.Forms.Label();
            this.m_sRFGenSteps = new System.Windows.Forms.TextBox();
            this.m_btnRFEGenStopAmplitudeSweep = new System.Windows.Forms.Button();
            this.m_comboRFGenPowerStart = new System.Windows.Forms.ComboBox();
            this.m_labRFGenPowerStart = new System.Windows.Forms.Label();
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
            // m_GroupControl
            // 
            this.m_GroupControl.AutoSize = true;
            this.m_GroupControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.m_GroupControl.Controls.Add(this.m_sRFGenDelay);
            this.m_GroupControl.Controls.Add(this.m_labRFGenDelay);
            this.m_GroupControl.Controls.Add(this.m_comboRFGenPowerStop);
            this.m_GroupControl.Controls.Add(this.m_labRFGenPowerStop);
            this.m_GroupControl.Controls.Add(this.m_btnRFEGenStartAmplitudeSweep);
            this.m_GroupControl.Controls.Add(this.m_labRFGenSteps);
            this.m_GroupControl.Controls.Add(this.m_sRFGenSteps);
            this.m_GroupControl.Controls.Add(this.m_btnRFEGenStopAmplitudeSweep);
            this.m_GroupControl.Controls.Add(this.m_comboRFGenPowerStart);
            this.m_GroupControl.Controls.Add(this.m_labRFGenPowerStart);
            this.m_GroupControl.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.m_GroupControl.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.m_GroupControl.Location = new System.Drawing.Point(0, 0);
            this.m_GroupControl.Margin = new System.Windows.Forms.Padding(0);
            this.m_GroupControl.Name = "m_GroupControl";
            this.m_GroupControl.Padding = new System.Windows.Forms.Padding(0);
            this.m_GroupControl.Size = new System.Drawing.Size(282, 126);
            this.m_GroupControl.TabIndex = 0;
            this.m_GroupControl.TabStop = false;
            this.m_GroupControl.Text = "Signal Generator Amplitude Sweep";
            this.m_ToolTip.SetToolTip(this.m_GroupControl, "Sweep amplitude settings");
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
            this.m_sRFGenDelay.TabIndex = 46;
            this.m_sRFGenDelay.Text = "0";
            this.m_sRFGenDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_ToolTip.SetToolTip(this.m_sRFGenDelay, "Delay in seconds between Sweep changes. Every sweep value will remain this select" +
        "ed time active before moving to the next value. Max 50 sec");
            this.m_sRFGenDelay.WordWrap = false;
            this.m_sRFGenDelay.Leave += new System.EventHandler(this.OnRFGenAmplSweepDelay_Leave);
            // 
            // m_labRFGenDelay
            // 
            this.m_labRFGenDelay.AutoSize = true;
            this.m_labRFGenDelay.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_labRFGenDelay.ForeColor = System.Drawing.Color.DarkBlue;
            this.m_labRFGenDelay.Location = new System.Drawing.Point(5, 87);
            this.m_labRFGenDelay.Name = "m_labRFGenDelay";
            this.m_labRFGenDelay.Size = new System.Drawing.Size(50, 16);
            this.m_labRFGenDelay.TabIndex = 45;
            this.m_labRFGenDelay.Text = "DELAY";
            this.m_labRFGenDelay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_comboRFGenPowerStop
            // 
            this.m_comboRFGenPowerStop.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_comboRFGenPowerStop.FormattingEnabled = true;
            this.m_comboRFGenPowerStop.Items.AddRange(new object[] {
            "-40dBm",
            "-37dBm",
            "-34dBm",
            "-30dBm",
            "-10dBm",
            "-7dBm",
            "-4dBm",
            "0dBm"});
            this.m_comboRFGenPowerStop.Location = new System.Drawing.Point(61, 49);
            this.m_comboRFGenPowerStop.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.m_comboRFGenPowerStop.Name = "m_comboRFGenPowerStop";
            this.m_comboRFGenPowerStop.Size = new System.Drawing.Size(90, 21);
            this.m_comboRFGenPowerStop.TabIndex = 44;
            this.m_ToolTip.SetToolTip(this.m_comboRFGenPowerStop, "Stop power value for Amplitude Sweep (always higher than Start value)");
            this.m_comboRFGenPowerStop.SelectedIndexChanged += new System.EventHandler(this.OnComboRFPowerStopSelectedIndexChanged);
            // 
            // m_labRFGenPowerStop
            // 
            this.m_labRFGenPowerStop.AutoSize = true;
            this.m_labRFGenPowerStop.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.m_labRFGenPowerStop.ForeColor = System.Drawing.Color.DarkBlue;
            this.m_labRFGenPowerStop.Location = new System.Drawing.Point(5, 50);
            this.m_labRFGenPowerStop.Name = "m_labRFGenPowerStop";
            this.m_labRFGenPowerStop.Size = new System.Drawing.Size(40, 16);
            this.m_labRFGenPowerStop.TabIndex = 43;
            this.m_labRFGenPowerStop.Text = "STOP";
            this.m_labRFGenPowerStop.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_btnRFEGenStartAmplitudeSweep
            // 
            this.m_btnRFEGenStartAmplitudeSweep.AutoSize = true;
            this.m_btnRFEGenStartAmplitudeSweep.Location = new System.Drawing.Point(161, 22);
            this.m_btnRFEGenStartAmplitudeSweep.Name = "m_btnRFEGenStartAmplitudeSweep";
            this.m_btnRFEGenStartAmplitudeSweep.Size = new System.Drawing.Size(118, 26);
            this.m_btnRFEGenStartAmplitudeSweep.TabIndex = 41;
            this.m_btnRFEGenStartAmplitudeSweep.Text = "Start Sweep...";
            this.m_ToolTip.SetToolTip(this.m_btnRFEGenStartAmplitudeSweep, "Start Amplitude Sweep");
            this.m_btnRFEGenStartAmplitudeSweep.UseVisualStyleBackColor = true;
            this.m_btnRFEGenStartAmplitudeSweep.Click += new System.EventHandler(this.OnStartRFGenAmpSweep_Click);
            // 
            // m_labRFGenSteps
            // 
            this.m_labRFGenSteps.AutoSize = true;
            this.m_labRFGenSteps.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_labRFGenSteps.ForeColor = System.Drawing.Color.DarkBlue;
            this.m_labRFGenSteps.Location = new System.Drawing.Point(158, 89);
            this.m_labRFGenSteps.Name = "m_labRFGenSteps";
            this.m_labRFGenSteps.Size = new System.Drawing.Size(46, 16);
            this.m_labRFGenSteps.TabIndex = 37;
            this.m_labRFGenSteps.Text = "STEPS";
            this.m_labRFGenSteps.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_sRFGenSteps
            // 
            this.m_sRFGenSteps.BackColor = System.Drawing.Color.RoyalBlue;
            this.m_sRFGenSteps.Font = new System.Drawing.Font("Digital-7", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_sRFGenSteps.ForeColor = System.Drawing.Color.White;
            this.m_sRFGenSteps.Location = new System.Drawing.Point(210, 83);
            this.m_sRFGenSteps.MaxLength = 1;
            this.m_sRFGenSteps.Name = "m_sRFGenSteps";
            this.m_sRFGenSteps.ReadOnly = true;
            this.m_sRFGenSteps.Size = new System.Drawing.Size(63, 26);
            this.m_sRFGenSteps.TabIndex = 38;
            this.m_sRFGenSteps.Text = "1";
            this.m_sRFGenSteps.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_ToolTip.SetToolTip(this.m_sRFGenSteps, "Sweep steps between Start and Stop amplitude values");
            this.m_sRFGenSteps.WordWrap = false;
            // 
            // m_btnRFEGenStopAmplitudeSweep
            // 
            this.m_btnRFEGenStopAmplitudeSweep.AutoSize = true;
            this.m_btnRFEGenStopAmplitudeSweep.Enabled = false;
            this.m_btnRFEGenStopAmplitudeSweep.Location = new System.Drawing.Point(161, 51);
            this.m_btnRFEGenStopAmplitudeSweep.Name = "m_btnRFEGenStopAmplitudeSweep";
            this.m_btnRFEGenStopAmplitudeSweep.Size = new System.Drawing.Size(89, 26);
            this.m_btnRFEGenStopAmplitudeSweep.TabIndex = 40;
            this.m_btnRFEGenStopAmplitudeSweep.Text = "Stop Sweep";
            this.m_ToolTip.SetToolTip(this.m_btnRFEGenStopAmplitudeSweep, "Stop active sweep");
            this.m_btnRFEGenStopAmplitudeSweep.UseVisualStyleBackColor = true;
            this.m_btnRFEGenStopAmplitudeSweep.Click += new System.EventHandler(this.OnStopRFGenAmpSweep_Click);
            // 
            // m_comboRFGenPowerStart
            // 
            this.m_comboRFGenPowerStart.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_comboRFGenPowerStart.FormattingEnabled = true;
            this.m_comboRFGenPowerStart.Items.AddRange(new object[] {
            "-40dBm",
            "-37dBm",
            "-34dBm",
            "-30dBm",
            "-10dBm",
            "-7dBm",
            "-4dBm",
            "0dBm"});
            this.m_comboRFGenPowerStart.Location = new System.Drawing.Point(61, 24);
            this.m_comboRFGenPowerStart.Name = "m_comboRFGenPowerStart";
            this.m_comboRFGenPowerStart.Size = new System.Drawing.Size(90, 21);
            this.m_comboRFGenPowerStart.TabIndex = 42;
            this.m_ToolTip.SetToolTip(this.m_comboRFGenPowerStart, "Start power value for Amplitude Sweep (always lower than Stop value)");
            this.m_comboRFGenPowerStart.SelectedIndexChanged += new System.EventHandler(this.OnComboRFPowerStartSelectedIndexChanged);
            // 
            // m_labRFGenPowerStart
            // 
            this.m_labRFGenPowerStart.AutoSize = true;
            this.m_labRFGenPowerStart.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.m_labRFGenPowerStart.ForeColor = System.Drawing.Color.DarkBlue;
            this.m_labRFGenPowerStart.Location = new System.Drawing.Point(5, 24);
            this.m_labRFGenPowerStart.Name = "m_labRFGenPowerStart";
            this.m_labRFGenPowerStart.Size = new System.Drawing.Size(49, 16);
            this.m_labRFGenPowerStart.TabIndex = 39;
            this.m_labRFGenPowerStart.Text = "START";
            this.m_labRFGenPowerStart.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ToolGroupRFEGenAmpSweep
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.m_GroupControl);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ToolGroupRFEGenAmpSweep";
            this.Size = new System.Drawing.Size(282, 130);
            this.m_GroupControl.ResumeLayout(false);
            this.m_GroupControl.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GroupControl_RFGen_AmplitudeSweep m_GroupControl;
        internal System.Windows.Forms.TextBox m_sRFGenDelay;
        internal System.Windows.Forms.Label m_labRFGenDelay;
        internal System.Windows.Forms.ComboBox m_comboRFGenPowerStop;
        internal System.Windows.Forms.Label m_labRFGenPowerStop;
        internal System.Windows.Forms.Button m_btnRFEGenStartAmplitudeSweep;
        internal System.Windows.Forms.Label m_labRFGenSteps;
        internal System.Windows.Forms.TextBox m_sRFGenSteps;
        internal System.Windows.Forms.Button m_btnRFEGenStopAmplitudeSweep;
        internal System.Windows.Forms.ComboBox m_comboRFGenPowerStart;
        internal System.Windows.Forms.Label m_labRFGenPowerStart;
        private System.Windows.Forms.ToolTip m_ToolTip;
    }
}
