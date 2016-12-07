namespace RFEClientControls
{
    partial class ToolGroupAnalyzerFreqSettings
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
            this.m_groupControl = new RFEClientControls.GroupControl_FreqSettings();
            this.btnAnalyzerFreqSettingsReset = new System.Windows.Forms.Button();
            this.btnAnalyzerSend = new System.Windows.Forms.Button();
            this.m_labAnalyzerFreqSpan = new System.Windows.Forms.Label();
            this.m_sAnalyzerFreqSpan = new System.Windows.Forms.TextBox();
            this.m_sAnalyzerEndFreq = new System.Windows.Forms.TextBox();
            this.m_sAnalyzerTopAmplitude = new System.Windows.Forms.TextBox();
            this.m_labAnalyzerEndFreq = new System.Windows.Forms.Label();
            this.m_sAnalyzerBottomAmplitude = new System.Windows.Forms.TextBox();
            this.m_labAnalyzerBottomAmplitude = new System.Windows.Forms.Label();
            this.m_labAnalyzerTopAmplitude = new System.Windows.Forms.Label();
            this.m_labAnalyzerCenterFreq = new System.Windows.Forms.Label();
            this.m_sAnalyzerStartFreq = new System.Windows.Forms.TextBox();
            this.m_labAnalyzerStartFreq = new System.Windows.Forms.Label();
            this.m_sAnalyzerCenterFreq = new System.Windows.Forms.TextBox();
            this.m_groupControl.SuspendLayout();
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
            // m_groupControl
            // 
            this.m_groupControl.AutoSize = true;
            this.m_groupControl.Controls.Add(this.btnAnalyzerFreqSettingsReset);
            this.m_groupControl.Controls.Add(this.btnAnalyzerSend);
            this.m_groupControl.Controls.Add(this.m_labAnalyzerFreqSpan);
            this.m_groupControl.Controls.Add(this.m_sAnalyzerFreqSpan);
            this.m_groupControl.Controls.Add(this.m_sAnalyzerEndFreq);
            this.m_groupControl.Controls.Add(this.m_sAnalyzerTopAmplitude);
            this.m_groupControl.Controls.Add(this.m_labAnalyzerEndFreq);
            this.m_groupControl.Controls.Add(this.m_sAnalyzerBottomAmplitude);
            this.m_groupControl.Controls.Add(this.m_labAnalyzerBottomAmplitude);
            this.m_groupControl.Controls.Add(this.m_labAnalyzerTopAmplitude);
            this.m_groupControl.Controls.Add(this.m_labAnalyzerCenterFreq);
            this.m_groupControl.Controls.Add(this.m_sAnalyzerStartFreq);
            this.m_groupControl.Controls.Add(this.m_labAnalyzerStartFreq);
            this.m_groupControl.Controls.Add(this.m_sAnalyzerCenterFreq);
            this.m_groupControl.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.m_groupControl.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.m_groupControl.Location = new System.Drawing.Point(0, 0);
            this.m_groupControl.Margin = new System.Windows.Forms.Padding(0);
            this.m_groupControl.Name = "m_groupControl";
            this.m_groupControl.Padding = new System.Windows.Forms.Padding(0);
            this.m_groupControl.Size = new System.Drawing.Size(390, 116);
            this.m_groupControl.TabIndex = 48;
            this.m_groupControl.TabStop = false;
            this.m_groupControl.Text = "Spectrum Analyzer Frequency and Power control";
            // 
            // btnAnalyzerFreqSettingsReset
            // 
            this.btnAnalyzerFreqSettingsReset.AutoSize = true;
            this.btnAnalyzerFreqSettingsReset.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.btnAnalyzerFreqSettingsReset.Location = new System.Drawing.Point(326, 76);
            this.btnAnalyzerFreqSettingsReset.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.btnAnalyzerFreqSettingsReset.MinimumSize = new System.Drawing.Size(58, 26);
            this.btnAnalyzerFreqSettingsReset.Name = "btnAnalyzerFreqSettingsReset";
            this.btnAnalyzerFreqSettingsReset.Size = new System.Drawing.Size(58, 26);
            this.btnAnalyzerFreqSettingsReset.TabIndex = 27;
            this.btnAnalyzerFreqSettingsReset.Text = "Reset";
            this.m_ToolTip.SetToolTip(this.btnAnalyzerFreqSettingsReset, "Ignores all changes and restore last values received from the RF Explorer device");
            this.btnAnalyzerFreqSettingsReset.UseVisualStyleBackColor = true;
            this.btnAnalyzerFreqSettingsReset.Click += new System.EventHandler(this.OnReset_Click);
            // 
            // btnAnalyzerSend
            // 
            this.btnAnalyzerSend.AutoSize = true;
            this.btnAnalyzerSend.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.btnAnalyzerSend.Location = new System.Drawing.Point(326, 19);
            this.btnAnalyzerSend.Name = "btnAnalyzerSend";
            this.btnAnalyzerSend.Size = new System.Drawing.Size(58, 55);
            this.btnAnalyzerSend.TabIndex = 26;
            this.btnAnalyzerSend.Text = "Send";
            this.m_ToolTip.SetToolTip(this.btnAnalyzerSend, "Send the updated values to the RF Explorer device");
            this.btnAnalyzerSend.UseVisualStyleBackColor = true;
            this.btnAnalyzerSend.Click += new System.EventHandler(this.OnSendAnalyzerConfiguration_Click);
            // 
            // m_labAnalyzerFreqSpan
            // 
            this.m_labAnalyzerFreqSpan.AutoSize = true;
            this.m_labAnalyzerFreqSpan.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.m_labAnalyzerFreqSpan.ForeColor = System.Drawing.Color.DarkBlue;
            this.m_labAnalyzerFreqSpan.Location = new System.Drawing.Point(179, 25);
            this.m_labAnalyzerFreqSpan.Name = "m_labAnalyzerFreqSpan";
            this.m_labAnalyzerFreqSpan.Size = new System.Drawing.Size(42, 16);
            this.m_labAnalyzerFreqSpan.TabIndex = 20;
            this.m_labAnalyzerFreqSpan.Text = "SPAN";
            this.m_labAnalyzerFreqSpan.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_sAnalyzerFreqSpan
            // 
            this.m_sAnalyzerFreqSpan.BackColor = System.Drawing.Color.RoyalBlue;
            this.m_sAnalyzerFreqSpan.Font = new System.Drawing.Font("Digital-7", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_sAnalyzerFreqSpan.ForeColor = System.Drawing.Color.White;
            this.m_sAnalyzerFreqSpan.Location = new System.Drawing.Point(225, 20);
            this.m_sAnalyzerFreqSpan.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.m_sAnalyzerFreqSpan.Name = "m_sAnalyzerFreqSpan";
            this.m_sAnalyzerFreqSpan.Size = new System.Drawing.Size(98, 26);
            this.m_sAnalyzerFreqSpan.TabIndex = 16;
            this.m_sAnalyzerFreqSpan.Text = "4.000";
            this.m_sAnalyzerFreqSpan.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_ToolTip.SetToolTip(this.m_sAnalyzerFreqSpan, "Frequency span in MHZ");
            this.m_sAnalyzerFreqSpan.Leave += new System.EventHandler(this.OnFreqSpan_Leave);
            // 
            // m_sAnalyzerEndFreq
            // 
            this.m_sAnalyzerEndFreq.BackColor = System.Drawing.Color.RoyalBlue;
            this.m_sAnalyzerEndFreq.Font = new System.Drawing.Font("Digital-7", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_sAnalyzerEndFreq.ForeColor = System.Drawing.Color.White;
            this.m_sAnalyzerEndFreq.Location = new System.Drawing.Point(225, 48);
            this.m_sAnalyzerEndFreq.Name = "m_sAnalyzerEndFreq";
            this.m_sAnalyzerEndFreq.Size = new System.Drawing.Size(98, 26);
            this.m_sAnalyzerEndFreq.TabIndex = 19;
            this.m_sAnalyzerEndFreq.Text = "2437.000";
            this.m_sAnalyzerEndFreq.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_ToolTip.SetToolTip(this.m_sAnalyzerEndFreq, "Stop frequency in MHZ - Center/Start will be recalculated");
            this.m_sAnalyzerEndFreq.Leave += new System.EventHandler(this.OnEndFreq_Leave);
            // 
            // m_sAnalyzerTopAmplitude
            // 
            this.m_sAnalyzerTopAmplitude.BackColor = System.Drawing.Color.RoyalBlue;
            this.m_sAnalyzerTopAmplitude.Font = new System.Drawing.Font("Digital-7", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_sAnalyzerTopAmplitude.ForeColor = System.Drawing.Color.White;
            this.m_sAnalyzerTopAmplitude.Location = new System.Drawing.Point(225, 76);
            this.m_sAnalyzerTopAmplitude.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.m_sAnalyzerTopAmplitude.Name = "m_sAnalyzerTopAmplitude";
            this.m_sAnalyzerTopAmplitude.Size = new System.Drawing.Size(98, 26);
            this.m_sAnalyzerTopAmplitude.TabIndex = 21;
            this.m_sAnalyzerTopAmplitude.Text = "-20";
            this.m_sAnalyzerTopAmplitude.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_ToolTip.SetToolTip(this.m_sAnalyzerTopAmplitude, "Visual amplitude at the top in dBm");
            this.m_sAnalyzerTopAmplitude.Leave += new System.EventHandler(this.OnAmplitudeLeave);
            // 
            // m_labAnalyzerEndFreq
            // 
            this.m_labAnalyzerEndFreq.AutoSize = true;
            this.m_labAnalyzerEndFreq.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.m_labAnalyzerEndFreq.ForeColor = System.Drawing.Color.DarkBlue;
            this.m_labAnalyzerEndFreq.Location = new System.Drawing.Point(179, 53);
            this.m_labAnalyzerEndFreq.Name = "m_labAnalyzerEndFreq";
            this.m_labAnalyzerEndFreq.Size = new System.Drawing.Size(40, 16);
            this.m_labAnalyzerEndFreq.TabIndex = 23;
            this.m_labAnalyzerEndFreq.Text = "STOP";
            this.m_labAnalyzerEndFreq.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_sAnalyzerBottomAmplitude
            // 
            this.m_sAnalyzerBottomAmplitude.BackColor = System.Drawing.Color.RoyalBlue;
            this.m_sAnalyzerBottomAmplitude.Font = new System.Drawing.Font("Digital-7", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_sAnalyzerBottomAmplitude.ForeColor = System.Drawing.Color.White;
            this.m_sAnalyzerBottomAmplitude.Location = new System.Drawing.Point(66, 76);
            this.m_sAnalyzerBottomAmplitude.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.m_sAnalyzerBottomAmplitude.Name = "m_sAnalyzerBottomAmplitude";
            this.m_sAnalyzerBottomAmplitude.Size = new System.Drawing.Size(105, 26);
            this.m_sAnalyzerBottomAmplitude.TabIndex = 22;
            this.m_sAnalyzerBottomAmplitude.Text = "-120";
            this.m_sAnalyzerBottomAmplitude.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_ToolTip.SetToolTip(this.m_sAnalyzerBottomAmplitude, "Visual amplitude at the bottom in dBm");
            this.m_sAnalyzerBottomAmplitude.Leave += new System.EventHandler(this.OnAmplitudeLeave);
            // 
            // m_labAnalyzerBottomAmplitude
            // 
            this.m_labAnalyzerBottomAmplitude.AutoSize = true;
            this.m_labAnalyzerBottomAmplitude.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.m_labAnalyzerBottomAmplitude.ForeColor = System.Drawing.Color.DarkBlue;
            this.m_labAnalyzerBottomAmplitude.Location = new System.Drawing.Point(3, 81);
            this.m_labAnalyzerBottomAmplitude.Name = "m_labAnalyzerBottomAmplitude";
            this.m_labAnalyzerBottomAmplitude.Size = new System.Drawing.Size(59, 16);
            this.m_labAnalyzerBottomAmplitude.TabIndex = 25;
            this.m_labAnalyzerBottomAmplitude.Text = "BOTTOM";
            this.m_labAnalyzerBottomAmplitude.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_labAnalyzerTopAmplitude
            // 
            this.m_labAnalyzerTopAmplitude.AutoSize = true;
            this.m_labAnalyzerTopAmplitude.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.m_labAnalyzerTopAmplitude.ForeColor = System.Drawing.Color.DarkBlue;
            this.m_labAnalyzerTopAmplitude.Location = new System.Drawing.Point(179, 81);
            this.m_labAnalyzerTopAmplitude.Name = "m_labAnalyzerTopAmplitude";
            this.m_labAnalyzerTopAmplitude.Size = new System.Drawing.Size(32, 16);
            this.m_labAnalyzerTopAmplitude.TabIndex = 24;
            this.m_labAnalyzerTopAmplitude.Text = "TOP";
            this.m_labAnalyzerTopAmplitude.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_labAnalyzerCenterFreq
            // 
            this.m_labAnalyzerCenterFreq.AutoSize = true;
            this.m_labAnalyzerCenterFreq.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_labAnalyzerCenterFreq.ForeColor = System.Drawing.Color.DarkBlue;
            this.m_labAnalyzerCenterFreq.Location = new System.Drawing.Point(3, 25);
            this.m_labAnalyzerCenterFreq.Name = "m_labAnalyzerCenterFreq";
            this.m_labAnalyzerCenterFreq.Size = new System.Drawing.Size(54, 16);
            this.m_labAnalyzerCenterFreq.TabIndex = 14;
            this.m_labAnalyzerCenterFreq.Text = "CENTER";
            this.m_labAnalyzerCenterFreq.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_sAnalyzerStartFreq
            // 
            this.m_sAnalyzerStartFreq.BackColor = System.Drawing.Color.RoyalBlue;
            this.m_sAnalyzerStartFreq.Font = new System.Drawing.Font("Digital-7", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_sAnalyzerStartFreq.ForeColor = System.Drawing.Color.White;
            this.m_sAnalyzerStartFreq.Location = new System.Drawing.Point(66, 48);
            this.m_sAnalyzerStartFreq.Name = "m_sAnalyzerStartFreq";
            this.m_sAnalyzerStartFreq.Size = new System.Drawing.Size(105, 26);
            this.m_sAnalyzerStartFreq.TabIndex = 17;
            this.m_sAnalyzerStartFreq.Text = "2433.000";
            this.m_sAnalyzerStartFreq.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_ToolTip.SetToolTip(this.m_sAnalyzerStartFreq, "Start frequency in MHZ - Center will be recalculated");
            this.m_sAnalyzerStartFreq.Leave += new System.EventHandler(this.OnStartFreq_Leave);
            // 
            // m_labAnalyzerStartFreq
            // 
            this.m_labAnalyzerStartFreq.AutoSize = true;
            this.m_labAnalyzerStartFreq.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.m_labAnalyzerStartFreq.ForeColor = System.Drawing.Color.DarkBlue;
            this.m_labAnalyzerStartFreq.Location = new System.Drawing.Point(3, 53);
            this.m_labAnalyzerStartFreq.Name = "m_labAnalyzerStartFreq";
            this.m_labAnalyzerStartFreq.Size = new System.Drawing.Size(49, 16);
            this.m_labAnalyzerStartFreq.TabIndex = 18;
            this.m_labAnalyzerStartFreq.Text = "START";
            this.m_labAnalyzerStartFreq.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_sAnalyzerCenterFreq
            // 
            this.m_sAnalyzerCenterFreq.BackColor = System.Drawing.Color.RoyalBlue;
            this.m_sAnalyzerCenterFreq.Font = new System.Drawing.Font("Digital-7", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_sAnalyzerCenterFreq.ForeColor = System.Drawing.Color.White;
            this.m_sAnalyzerCenterFreq.Location = new System.Drawing.Point(66, 20);
            this.m_sAnalyzerCenterFreq.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.m_sAnalyzerCenterFreq.MinimumSize = new System.Drawing.Size(98, 26);
            this.m_sAnalyzerCenterFreq.Name = "m_sAnalyzerCenterFreq";
            this.m_sAnalyzerCenterFreq.Size = new System.Drawing.Size(105, 26);
            this.m_sAnalyzerCenterFreq.TabIndex = 15;
            this.m_sAnalyzerCenterFreq.Text = "2435.000";
            this.m_sAnalyzerCenterFreq.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_ToolTip.SetToolTip(this.m_sAnalyzerCenterFreq, "Center frequency in MHZ - Start/Stop will be recalculated");
            this.m_sAnalyzerCenterFreq.WordWrap = false;
            this.m_sAnalyzerCenterFreq.Leave += new System.EventHandler(this.OnCenterFreq_Leave);
            // 
            // ToolGroupAnalyzerFreqSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.m_groupControl);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ToolGroupAnalyzerFreqSettings";
            this.Size = new System.Drawing.Size(390, 116);
            this.m_groupControl.ResumeLayout(false);
            this.m_groupControl.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GroupControl_FreqSettings m_groupControl;
        internal System.Windows.Forms.Button btnAnalyzerFreqSettingsReset;
        internal System.Windows.Forms.Button btnAnalyzerSend;
        internal System.Windows.Forms.Label m_labAnalyzerFreqSpan;
        internal System.Windows.Forms.TextBox m_sAnalyzerFreqSpan;
        internal System.Windows.Forms.TextBox m_sAnalyzerEndFreq;
        internal System.Windows.Forms.TextBox m_sAnalyzerTopAmplitude;
        internal System.Windows.Forms.Label m_labAnalyzerEndFreq;
        internal System.Windows.Forms.TextBox m_sAnalyzerBottomAmplitude;
        internal System.Windows.Forms.Label m_labAnalyzerBottomAmplitude;
        internal System.Windows.Forms.Label m_labAnalyzerTopAmplitude;
        internal System.Windows.Forms.Label m_labAnalyzerCenterFreq;
        internal System.Windows.Forms.TextBox m_sAnalyzerStartFreq;
        internal System.Windows.Forms.Label m_labAnalyzerStartFreq;
        internal System.Windows.Forms.TextBox m_sAnalyzerCenterFreq;
        private System.Windows.Forms.ToolTip m_ToolTip;
    }
}
