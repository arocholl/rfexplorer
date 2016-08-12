using System.Windows.Forms;

namespace RFEClientControls
{
    partial class ToolGroupRemoteScreen
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
            this.m_groupControl_RemoteScreen = new RFEClientControls.GroupControl_RemoteScreen();
            this.m_chkDumpScreen = new System.Windows.Forms.CheckBox();
            this.m_chkDumpLCDGrid = new System.Windows.Forms.CheckBox();
            this.m_numScreenIndex = new System.Windows.Forms.NumericUpDown();
            this.m_chkDumpColorScreen = new System.Windows.Forms.CheckBox();
            this.m_numericZoom = new System.Windows.Forms.NumericUpDown();
            this.m_chkDumpHeader = new System.Windows.Forms.CheckBox();
            this.m_labBitmapSize = new System.Windows.Forms.Label();
            this.m_labRemoteScreenSample = new System.Windows.Forms.Label();
            this.m_labRemoteScreenZoom = new System.Windows.Forms.Label();
            this.m_btnSaveRemoteBitmap = new System.Windows.Forms.Button();
            this.m_groupControl_RemoteScreen.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_numScreenIndex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_numericZoom)).BeginInit();
            this.SuspendLayout();
            // 
            // m_groupControl_RemoteScreen
            // 
            this.m_groupControl_RemoteScreen.AutoSize = true;
            this.m_groupControl_RemoteScreen.Controls.Add(this.m_chkDumpScreen);
            this.m_groupControl_RemoteScreen.Controls.Add(this.m_chkDumpLCDGrid);
            this.m_groupControl_RemoteScreen.Controls.Add(this.m_numScreenIndex);
            this.m_groupControl_RemoteScreen.Controls.Add(this.m_chkDumpColorScreen);
            this.m_groupControl_RemoteScreen.Controls.Add(this.m_numericZoom);
            this.m_groupControl_RemoteScreen.Controls.Add(this.m_chkDumpHeader);
            this.m_groupControl_RemoteScreen.Controls.Add(this.m_labBitmapSize);
            this.m_groupControl_RemoteScreen.Controls.Add(this.m_labRemoteScreenSample);
            this.m_groupControl_RemoteScreen.Controls.Add(this.m_labRemoteScreenZoom);
            this.m_groupControl_RemoteScreen.Controls.Add(this.m_btnSaveRemoteBitmap);
            this.m_groupControl_RemoteScreen.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.m_groupControl_RemoteScreen.Location = new System.Drawing.Point(0, 0);
            this.m_groupControl_RemoteScreen.MaximumSize = new System.Drawing.Size(369, 116);
            this.m_groupControl_RemoteScreen.Name = "m_groupControl_RemoteScreen";
            this.m_groupControl_RemoteScreen.Size = new System.Drawing.Size(369, 116);
            this.m_groupControl_RemoteScreen.TabIndex = 56;
            this.m_groupControl_RemoteScreen.TabStop = false;
            this.m_groupControl_RemoteScreen.Text = "Dump Remote Screen";
            // 
            // chkDumpScreen
            // 
            this.m_chkDumpScreen.AutoSize = true;
            this.m_chkDumpScreen.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.m_chkDumpScreen.Location = new System.Drawing.Point(9, 25);
            this.m_chkDumpScreen.MinimumSize = new System.Drawing.Size(60, 0);
            this.m_chkDumpScreen.Name = "chkDumpScreen";
            this.m_chkDumpScreen.Size = new System.Drawing.Size(153, 17);
            this.m_chkDumpScreen.TabIndex = 14;
            this.m_chkDumpScreen.Text = "Enable Screen Capture";
            this.m_chkDumpScreen.UseVisualStyleBackColor = true;
            this.m_chkDumpScreen.CheckedChanged += new System.EventHandler(this.OnDumpScreen_CheckedChanged);
            // 
            // chkDumpLCDGrid
            // 
            this.m_chkDumpLCDGrid.AutoSize = true;
            this.m_chkDumpLCDGrid.Location = new System.Drawing.Point(250, 82);
            this.m_chkDumpLCDGrid.Name = "chkDumpLCDGrid";
            this.m_chkDumpLCDGrid.Size = new System.Drawing.Size(92, 17);
            this.m_chkDumpLCDGrid.TabIndex = 54;
            this.m_chkDumpLCDGrid.Text = "View LCD Grid";
            this.m_chkDumpLCDGrid.UseVisualStyleBackColor = true;
            this.m_chkDumpLCDGrid.CheckedChanged += new System.EventHandler(this.OnDumpLCDGrid_CheckedChanged);
            // 
            // numScreenIndex
            // 
            this.m_numScreenIndex.AutoSize = true;
            this.m_numScreenIndex.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_numScreenIndex.Location = new System.Drawing.Point(60, 52);
            this.m_numScreenIndex.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.m_numScreenIndex.Name = "numScreenIndex";
            this.m_numScreenIndex.Size = new System.Drawing.Size(87, 23);
            this.m_numScreenIndex.TabIndex = 51;
            this.m_numScreenIndex.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_numScreenIndex.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.m_numScreenIndex.ValueChanged += new System.EventHandler(this.OnNumScreenIndex_ValueChanged);
            // 
            // chkDumpColorScreen
            // 
            this.m_chkDumpColorScreen.AutoSize = true;
            this.m_chkDumpColorScreen.Location = new System.Drawing.Point(250, 53);
            this.m_chkDumpColorScreen.Name = "chkDumpColorScreen";
            this.m_chkDumpColorScreen.Size = new System.Drawing.Size(87, 17);
            this.m_chkDumpColorScreen.TabIndex = 54;
            this.m_chkDumpColorScreen.Text = "Color Screen";
            this.m_chkDumpColorScreen.UseVisualStyleBackColor = true;
            this.m_chkDumpColorScreen.CheckedChanged += new System.EventHandler(this.OnDumpColorScreen_CheckedChanged);
            // 
            // numericZoom
            // 
            this.m_numericZoom.AutoSize = true;
            this.m_numericZoom.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_numericZoom.Location = new System.Drawing.Point(60, 81);
            this.m_numericZoom.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.m_numericZoom.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.m_numericZoom.Name = "numericZoom";
            this.m_numericZoom.Size = new System.Drawing.Size(87, 23);
            this.m_numericZoom.TabIndex = 17;
            this.m_numericZoom.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_numericZoom.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.m_numericZoom.ValueChanged += new System.EventHandler(this.OnNumericZoom_ValueChanged);
            // 
            // chkDumpHeader
            // 
            this.m_chkDumpHeader.AutoSize = true;
            this.m_chkDumpHeader.Location = new System.Drawing.Point(250, 38);
            this.m_chkDumpHeader.Name = "chkDumpHeader";
            this.m_chkDumpHeader.Size = new System.Drawing.Size(86, 17);
            this.m_chkDumpHeader.TabIndex = 54;
            this.m_chkDumpHeader.Text = "Header Text";
            this.m_chkDumpHeader.UseVisualStyleBackColor = true;
            this.m_chkDumpHeader.CheckedChanged += new System.EventHandler(this.OnDumpHeader_CheckedChanged);
            // 
            // m_labBitmapSize
            // 
            this.m_labBitmapSize.AutoSize = true;
            this.m_labBitmapSize.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_labBitmapSize.Location = new System.Drawing.Point(153, 83);
            this.m_labBitmapSize.Name = "m_labBitmapSize";
            this.m_labBitmapSize.Size = new System.Drawing.Size(82, 16);
            this.m_labBitmapSize.TabIndex = 50;
            this.m_labBitmapSize.Text = "Size: 128x64";
            this.m_labBitmapSize.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_labRemoteScreenSample
            // 
            this.m_labRemoteScreenSample.AutoSize = true;
            this.m_labRemoteScreenSample.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_labRemoteScreenSample.Location = new System.Drawing.Point(6, 54);
            this.m_labRemoteScreenSample.Name = "m_labRemoteScreenSample";
            this.m_labRemoteScreenSample.Size = new System.Drawing.Size(51, 16);
            this.m_labRemoteScreenSample.TabIndex = 52;
            this.m_labRemoteScreenSample.Text = "Sample";
            this.m_labRemoteScreenSample.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_labRemoteScreenZoom
            // 
            this.m_labRemoteScreenZoom.AutoSize = true;
            this.m_labRemoteScreenZoom.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_labRemoteScreenZoom.Location = new System.Drawing.Point(6, 83);
            this.m_labRemoteScreenZoom.Name = "m_labRemoteScreenZoom";
            this.m_labRemoteScreenZoom.Size = new System.Drawing.Size(40, 16);
            this.m_labRemoteScreenZoom.TabIndex = 50;
            this.m_labRemoteScreenZoom.Text = "Zoom";
            this.m_labRemoteScreenZoom.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnSaveRemoteBitmap
            // 
            this.m_btnSaveRemoteBitmap.AutoSize = true;
            this.m_btnSaveRemoteBitmap.Location = new System.Drawing.Point(153, 49);
            this.m_btnSaveRemoteBitmap.Name = "btnSaveRemoteBitmap";
            this.m_btnSaveRemoteBitmap.Size = new System.Drawing.Size(104, 26);
            this.m_btnSaveRemoteBitmap.TabIndex = 53;
            this.m_btnSaveRemoteBitmap.Text = "Save Bitmap...";
            this.m_btnSaveRemoteBitmap.UseVisualStyleBackColor = true;
            this.m_btnSaveRemoteBitmap.Click += new System.EventHandler(this.OnSaveImage_Click);
            // 
            // ToolGroupRemoteScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.m_groupControl_RemoteScreen);
            this.Name = "ToolGroupRemoteScreen";
            this.Size = new System.Drawing.Size(372, 119);
            this.m_groupControl_RemoteScreen.ResumeLayout(false);
            this.m_groupControl_RemoteScreen.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_numScreenIndex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_numericZoom)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.CheckBox m_chkDumpScreen;
        internal System.Windows.Forms.CheckBox m_chkDumpHeader;
        internal System.Windows.Forms.Button m_btnSaveRemoteBitmap;
        internal System.Windows.Forms.CheckBox m_chkDumpColorScreen;
        internal System.Windows.Forms.CheckBox m_chkDumpLCDGrid;
        internal System.Windows.Forms.NumericUpDown m_numericZoom;
        internal System.Windows.Forms.NumericUpDown m_numScreenIndex;
        internal System.Windows.Forms.Label m_labBitmapSize;
        internal System.Windows.Forms.Label m_labRemoteScreenZoom;
        internal System.Windows.Forms.Label m_labRemoteScreenSample;
        private GroupControl_RemoteScreen m_groupControl_RemoteScreen;


    }
}
