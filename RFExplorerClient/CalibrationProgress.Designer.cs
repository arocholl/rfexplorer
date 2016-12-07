namespace RFExplorerClient
{
    partial class CalibrationProgress
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CalibrationProgress));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.textLine1 = new System.Windows.Forms.Label();
            this.textLine2 = new System.Windows.Forms.Label();
            this.textLine3 = new System.Windows.Forms.Label();
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.ImageLocation = "";
            this.pictureBox1.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.InitialImage")));
            this.pictureBox1.Location = new System.Drawing.Point(27, 26);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(154, 136);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // textLine1
            // 
            this.textLine1.AutoSize = true;
            this.textLine1.Font = new System.Drawing.Font("Franklin Gothic Book", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textLine1.Location = new System.Drawing.Point(199, 35);
            this.textLine1.Name = "textLine1";
            this.textLine1.Size = new System.Drawing.Size(52, 21);
            this.textLine1.TabIndex = 1;
            this.textLine1.Text = "label1";
            this.textLine1.UseWaitCursor = true;
            // 
            // textLine2
            // 
            this.textLine2.AutoSize = true;
            this.textLine2.Font = new System.Drawing.Font("Franklin Gothic Book", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textLine2.Location = new System.Drawing.Point(199, 72);
            this.textLine2.Name = "textLine2";
            this.textLine2.Size = new System.Drawing.Size(52, 21);
            this.textLine2.TabIndex = 2;
            this.textLine2.Text = "label1";
            // 
            // textLine3
            // 
            this.textLine3.AutoSize = true;
            this.textLine3.Font = new System.Drawing.Font("Franklin Gothic Book", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textLine3.Location = new System.Drawing.Point(199, 141);
            this.textLine3.Name = "textLine3";
            this.textLine3.Size = new System.Drawing.Size(52, 21);
            this.textLine3.TabIndex = 3;
            this.textLine3.Text = "label2";
            // 
            // ProgressBar
            // 
            this.ProgressBar.Location = new System.Drawing.Point(203, 106);
            this.ProgressBar.Maximum = 3;
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(277, 23);
            this.ProgressBar.Step = 1;
            this.ProgressBar.TabIndex = 4;
            // 
            // CalibrationProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 186);
            this.Controls.Add(this.ProgressBar);
            this.Controls.Add(this.textLine3);
            this.Controls.Add(this.textLine2);
            this.Controls.Add(this.textLine1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CalibrationProgress";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RF Explorer Calibrating Frequency...";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        public System.Windows.Forms.Label textLine1;
        public System.Windows.Forms.Label textLine2;
        public System.Windows.Forms.Label textLine3;
        public System.Windows.Forms.ProgressBar ProgressBar;
    }
}