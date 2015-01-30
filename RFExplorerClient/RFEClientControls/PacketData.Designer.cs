namespace RFEClientControls
{
    partial class PacketData
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
            this.m_tablePanel = new System.Windows.Forms.TableLayoutPanel();
            this.m_tablePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.m_tablePanel.ColumnCount = 2;
            this.m_tablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.m_tablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.m_tablePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_tablePanel.Location = new System.Drawing.Point(0, 0);
            this.m_tablePanel.Name = "tableLayoutPanel1";
            this.m_tablePanel.RowCount = 9;
            this.m_tablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.m_tablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.m_tablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.m_tablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.m_tablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.m_tablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.m_tablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.m_tablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.m_tablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.m_tablePanel.Size = new System.Drawing.Size(324, 152);
            this.m_tablePanel.TabIndex = 0;
            // 
            // PacketData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_tablePanel);
            this.Name = "PacketData";
            this.Size = new System.Drawing.Size(324, 152);
            this.m_tablePanel.ResumeLayout(false);
            this.m_tablePanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel m_tablePanel;
    }
}
