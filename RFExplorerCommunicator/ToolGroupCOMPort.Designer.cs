namespace RFExplorerCommunicator
{
    partial class ToolGroupCOMPort
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
            this.m_groupControl_Connection = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.m_btnRescan = new System.Windows.Forms.Button();
            this.m_ComboBaudRate = new System.Windows.Forms.ComboBox();
            this.m_btnConnect = new System.Windows.Forms.Button();
            this.m_btnDisconnect = new System.Windows.Forms.Button();
            this.m_comboCOMPort = new System.Windows.Forms.ComboBox();
            this.m_groupControl_Connection.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_groupControl_Connection
            // 
            this.m_groupControl_Connection.AutoSize = true;
            this.m_groupControl_Connection.Controls.Add(this.tableLayoutPanel6);
            this.m_groupControl_Connection.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.m_groupControl_Connection.Location = new System.Drawing.Point(0, 0);
            this.m_groupControl_Connection.Name = "m_groupControl_Connection";
            this.m_groupControl_Connection.Size = new System.Drawing.Size(195, 114);
            this.m_groupControl_Connection.TabIndex = 51;
            this.m_groupControl_Connection.TabStop = false;
            this.m_groupControl_Connection.Text = "COM Port";
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.AutoSize = true;
            this.tableLayoutPanel6.ColumnCount = 3;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel6.Controls.Add(this.m_btnRescan, 1, 0);
            this.tableLayoutPanel6.Controls.Add(this.m_ComboBaudRate, 2, 0);
            this.tableLayoutPanel6.Controls.Add(this.m_btnConnect, 0, 1);
            this.tableLayoutPanel6.Controls.Add(this.m_btnDisconnect, 2, 1);
            this.tableLayoutPanel6.Controls.Add(this.m_comboCOMPort, 0, 0);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 2;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(189, 95);
            this.tableLayoutPanel6.TabIndex = 0;
            // 
            // m_btnRescan
            // 
            this.m_btnRescan.AutoSize = true;
            this.m_btnRescan.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.m_btnRescan.Location = new System.Drawing.Point(74, 3);
            this.m_btnRescan.Name = "m_btnRescan";
            this.m_btnRescan.Size = new System.Drawing.Size(23, 23);
            this.m_btnRescan.TabIndex = 11;
            this.m_btnRescan.Text = "*";
            this.m_btnRescan.UseVisualStyleBackColor = true;
            this.m_btnRescan.Click += new System.EventHandler(this.OnRescan_Click);
            // 
            // m_ComboBaudRate
            // 
            this.m_ComboBaudRate.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.m_ComboBaudRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_ComboBaudRate.Items.AddRange(new object[] {
            "2400",
            "500000"});
            this.m_ComboBaudRate.Location = new System.Drawing.Point(103, 5);
            this.m_ComboBaudRate.Name = "m_ComboBaudRate";
            this.m_ComboBaudRate.Size = new System.Drawing.Size(83, 21);
            this.m_ComboBaudRate.TabIndex = 12;
            // 
            // m_btnConnect
            // 
            this.m_btnConnect.AutoSize = true;
            this.tableLayoutPanel6.SetColumnSpan(this.m_btnConnect, 2);
            this.m_btnConnect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_btnConnect.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.m_btnConnect.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_btnConnect.Location = new System.Drawing.Point(3, 39);
            this.m_btnConnect.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.m_btnConnect.Name = "m_btnConnect";
            this.m_btnConnect.Size = new System.Drawing.Size(94, 46);
            this.m_btnConnect.TabIndex = 13;
            this.m_btnConnect.Text = "Connect";
            this.m_btnConnect.Click += new System.EventHandler(this.OnConnect_Click);
            // 
            // m_btnDisconnect
            // 
            this.m_btnDisconnect.AutoSize = true;
            this.m_btnDisconnect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_btnDisconnect.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.m_btnDisconnect.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_btnDisconnect.Location = new System.Drawing.Point(103, 39);
            this.m_btnDisconnect.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.m_btnDisconnect.Name = "m_btnDisconnect";
            this.m_btnDisconnect.Size = new System.Drawing.Size(83, 46);
            this.m_btnDisconnect.TabIndex = 41;
            this.m_btnDisconnect.Text = "Disconnect";
            this.m_btnDisconnect.Click += new System.EventHandler(this.OnDisconnect_Click);
            // 
            // m_comboCOMPort
            // 
            this.m_comboCOMPort.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.m_comboCOMPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_comboCOMPort.Location = new System.Drawing.Point(3, 5);
            this.m_comboCOMPort.Name = "m_comboCOMPort";
            this.m_comboCOMPort.Size = new System.Drawing.Size(65, 21);
            this.m_comboCOMPort.TabIndex = 10;
            // 
            // ToolGroupCOMPort
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_groupControl_Connection);
            this.Name = "ToolGroupCOMPort";
            this.Size = new System.Drawing.Size(200, 119);
            this.m_groupControl_Connection.ResumeLayout(false);
            this.m_groupControl_Connection.PerformLayout();
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox m_groupControl_Connection;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.Button m_btnRescan;
        private System.Windows.Forms.ComboBox m_ComboBaudRate;
        private System.Windows.Forms.Button m_btnConnect;
        private System.Windows.Forms.Button m_btnDisconnect;
        private System.Windows.Forms.ComboBox m_comboCOMPort;
    }
}
