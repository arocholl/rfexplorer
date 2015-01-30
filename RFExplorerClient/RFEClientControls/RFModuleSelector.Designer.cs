namespace RFEClientControls
{
    partial class RFModuleSelector
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
            this.pictureMain = new System.Windows.Forms.PictureBox();
            this.menuContextDevice = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuCloseDeviceIcon = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pictureMain)).BeginInit();
            this.menuContextDevice.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureMain
            // 
            this.pictureMain.BackColor = System.Drawing.Color.Transparent;
            this.pictureMain.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureMain.Image = global::RFEClientControls.Properties.Resources.RFExplorer_WhipLeft_Disconnected;
            this.pictureMain.Location = new System.Drawing.Point(3, 3);
            this.pictureMain.Name = "pictureMain";
            this.pictureMain.Size = new System.Drawing.Size(133, 279);
            this.pictureMain.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureMain.TabIndex = 0;
            this.pictureMain.TabStop = false;
            // 
            // menuContextDevice
            // 
            this.menuContextDevice.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuCloseDeviceIcon});
            this.menuContextDevice.Name = "menuContextDevice";
            this.menuContextDevice.Size = new System.Drawing.Size(153, 48);
            this.menuContextDevice.Text = "RF Explorer device";
            // 
            // menuCloseDeviceIcon
            // 
            this.menuCloseDeviceIcon.Name = "menuCloseDeviceIcon";
            this.menuCloseDeviceIcon.Size = new System.Drawing.Size(152, 22);
            this.menuCloseDeviceIcon.Text = "Hide device";
            this.menuCloseDeviceIcon.Click += new System.EventHandler(this.menuHide_Click);
            // 
            // RFModuleSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.menuContextDevice;
            this.Controls.Add(this.pictureMain);
            this.Name = "RFModuleSelector";
            this.Size = new System.Drawing.Size(140, 285);
            this.Load += new System.EventHandler(this.RFModuleSelector_Load);
            this.SizeChanged += new System.EventHandler(this.RFModuleSelector_SizeChanged);
            this.Click += new System.EventHandler(this.RFModuleSelector_Click);
            ((System.ComponentModel.ISupportInitialize)(this.pictureMain)).EndInit();
            this.menuContextDevice.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureMain;
        private System.Windows.Forms.ContextMenuStrip menuContextDevice;
        private System.Windows.Forms.ToolStripMenuItem menuCloseDeviceIcon;
    }
}
