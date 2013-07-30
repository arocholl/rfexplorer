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
            this.pictureMain = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureMain)).BeginInit();
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
            // RFModuleSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureMain);
            this.Name = "RFModuleSelector";
            this.Size = new System.Drawing.Size(140, 285);
            this.Load += new System.EventHandler(this.RFModuleSelector_Load);
            this.SizeChanged += new System.EventHandler(this.RFModuleSelector_SizeChanged);
            this.Click += new System.EventHandler(this.RFModuleSelector_Click);
            ((System.ComponentModel.ISupportInitialize)(this.pictureMain)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureMain;
    }
}
