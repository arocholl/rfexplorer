//============================================================================
//RF Explorer for Windows - A Handheld Spectrum Analyzer for everyone!
//Copyright © 2010-15 Ariel Rocholl, www.rf-explorer.com
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
    partial class RemoteScreenControl
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
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                if (m_BrushBlack != null)
                {
                    m_BrushBlack.Dispose();
                    m_BrushBlack = null;
                }
                if (m_BrushDarkBlue!= null)
                {
                    m_BrushDarkBlue.Dispose();
                    m_BrushDarkBlue = null;
                }
                if (m_BrushLightBlue!= null)
                {
                    m_BrushLightBlue.Dispose();
                    m_BrushLightBlue = null;
                }
                if (m_BrushlinGrBrush!= null)
                {
                    m_BrushlinGrBrush.Dispose();
                    m_BrushlinGrBrush = null;
                }
                if (m_BrushWhite!= null)
                {
                    m_BrushWhite.Dispose();
                    m_BrushWhite = null;
                }
                if (m_ImageLogo!= null)
                {
                    m_ImageLogo.Dispose();
                    m_ImageLogo = null;
                }
                if (m_PenDarkBlue!= null)
                {
                    m_PenDarkBlue.Dispose();
                    m_PenDarkBlue = null;
                }
                if (m_PenBlack != null)
                {
                    m_PenBlack.Dispose();
                    m_PenBlack = null;
                }
                if (m_TextFont!= null)
                {
                    m_TextFont.Dispose();
                    m_TextFont = null;
                }
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
            this.SuspendLayout();
            // 
            // RemoteScreenControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.DoubleBuffered = true;
            this.Name = "RemoteScreenControl";
            this.Size = new System.Drawing.Size(905, 457);
            this.Load += new System.EventHandler(this.RemoteScreenControl_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.RemoteScreenControl_Paint);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
