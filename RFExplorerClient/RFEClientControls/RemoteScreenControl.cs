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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RFExplorerCommunicator;

namespace RFEClientControls
{
    public partial class RemoteScreenControl : UserControl
    {
        private int m_nZoom;                    //Zoom value 1-7
        private RFECommunicator m_objRFE;        //Reference to the running communicator, it contains data and status
        public RFECommunicator RFExplorer
        {
            set { m_objRFE = value; }
            get { return m_objRFE; }
        }
        public PictureBox m_ImageLogo;

        //Graphis objects cached to reduce drawing overhead
        LinearGradientBrush m_BrushlinGrBrush;
        Pen m_PenDarkBlue;
        Pen m_PenBlack;
        Brush m_BrushDarkBlue;
        Brush m_BrushBlack;
        Brush m_BrushWhite;
        Brush m_BrushLightBlue;
        Rectangle m_AdjustedClientRect;
        Font m_TextFont;

        bool m_bHeaderText;
        public bool HeaderText
        {
            get { return m_bHeaderText; }
            set { m_bHeaderText = value; }
        }

        bool m_bColor;    //true if the control draws color screen
        public bool LCDColor
        {
            get { return m_bColor; }
            set { m_bColor = value; }
        }
        bool m_bGrid;     //true if the control draws LCD grid in large zoom values
        public bool LCDGrid
        {
            get { return m_bGrid; }
            set { m_bGrid = value; }
        }

        public RemoteScreenControl()
        {
            InitializeComponent();
            m_nZoom = -1;
            LCDGrid = true;
            LCDColor = true;
            HeaderText = true;
        }

        public void UpdateZoom(int nNewZoom)
        {
            m_nZoom = nNewZoom;
            if (m_BrushlinGrBrush != null)
                m_BrushlinGrBrush.Dispose();
            m_BrushlinGrBrush = new LinearGradientBrush(
               new Point(0, -10),
               new Point(0, ClientRectangle.Height + m_nZoom * 20),
               Color.White,
               Color.LightBlue);
            m_AdjustedClientRect = ClientRectangle;
            m_AdjustedClientRect.Height = m_AdjustedClientRect.Height - 2;
            m_AdjustedClientRect.Width = m_AdjustedClientRect.Width - 2;

            if (m_TextFont!=null)
                m_TextFont.Dispose();

            m_TextFont = new Font("Arial Bold", 3.0f * m_nZoom);
        }

        private void RemoteScreenControl_Load(object sender, EventArgs e)
        {
            m_PenDarkBlue = new Pen(Color.DarkBlue, 1);
            m_BrushDarkBlue = new SolidBrush(Color.DarkBlue);
            m_PenBlack = new Pen(Color.Black,1);
            m_BrushBlack = new SolidBrush(Color.Black);
            m_BrushWhite = new SolidBrush(Color.White);
            m_BrushLightBlue = new SolidBrush(Color.SteelBlue);

            m_ImageLogo = new PictureBox();
            m_ImageLogo.BackColor = System.Drawing.Color.Transparent;
            m_ImageLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            m_ImageLogo.Image = global::RFEClientControls.Properties.Resources.LogoBlueShadow;
            m_ImageLogo.Location = new System.Drawing.Point(10, 10);
            m_ImageLogo.Name = "RFExplorer_logo";
            m_ImageLogo.Size = new System.Drawing.Size(m_ImageLogo.Image.Width, m_ImageLogo.Image.Height);
            m_ImageLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            m_ImageLogo.TabIndex = 0;
            m_ImageLogo.TabStop = false;
            m_ImageLogo.Visible = false;
            m_ImageLogo.Enabled = true;
            m_ImageLogo.BorderStyle = BorderStyle.None;
            Controls.Add(m_ImageLogo);
        }

        private void RemoteScreenControl_Paint(object sender, PaintEventArgs e)
        {
            if (m_nZoom == -1)
                return; //uninitialized

            Pen objPen = m_PenDarkBlue;
            Brush objBrush = m_BrushDarkBlue;
            if (!LCDColor)
            {
                objBrush = m_BrushBlack;
                objPen = m_PenBlack;
                e.Graphics.FillRectangle(m_BrushWhite, m_AdjustedClientRect);
            }
            else
            {
                e.Graphics.FillRectangle(m_BrushlinGrBrush, m_AdjustedClientRect);
            }

            e.Graphics.DrawRectangle(objPen, m_AdjustedClientRect);

            if (m_objRFE == null)
                return;

            RFEScreenData objScreen = m_objRFE.ScreenData.GetData(m_objRFE.ScreenIndex);
            if ((m_objRFE.ScreenIndex == 0) || (objScreen == null))
            {
                m_ImageLogo.Visible = true;

                m_ImageLogo.Width = (int)(0.8 * m_AdjustedClientRect.Width);
                m_ImageLogo.Height = (int)(m_ImageLogo.Width * (double)m_ImageLogo.Image.Height / (double)m_ImageLogo.Image.Width);
                m_ImageLogo.Location = new System.Drawing.Point((int)(0.1 * m_AdjustedClientRect.Width),
                    (m_AdjustedClientRect.Height - m_ImageLogo.Height) / 2);
                return;
            }

            if ((m_objRFE.ScreenIndex > 0) && (HeaderText))
            {
                m_ImageLogo.Visible = true;

                m_ImageLogo.Width = (int)(0.6 * m_AdjustedClientRect.Width);
                m_ImageLogo.Height = (int)(m_ImageLogo.Width * (double)m_ImageLogo.Image.Height / (double)m_ImageLogo.Image.Width);
                m_ImageLogo.Location = new System.Drawing.Point((int)(0.2 * m_AdjustedClientRect.Width), 0);

                if (objScreen.CaptureTime.Year > 2000)
                {
                    //Draw time and model captured only if information available is current
                    string sText = "Screen captured " + objScreen.CaptureTime.ToString("yyyy-MM-dd HH:mm:ss\\.fff") + " - model " + objScreen.Model.ToString().Replace("MODEL_", "");
                    SizeF objStringSize = e.Graphics.MeasureString(sText, m_TextFont);
                    e.Graphics.DrawString(sText, m_TextFont, m_BrushLightBlue, (m_AdjustedClientRect.Width - objStringSize.Width) / 2, m_ImageLogo.Bottom);
                }
            }
            else
            {
                m_ImageLogo.Visible = false;
            }
            
            int nGap = 1;
            if ((m_nZoom <= 4) || (LCDGrid==false))
                nGap = 0;
            /*
             * only for video, too blurry for static image
        else
            objGraphics.SmoothingMode = SmoothingMode.AntiAlias;
             */

            int nAdjustHeader = 20;
            if (!HeaderText)
            {
                nAdjustHeader = 0;
            }

            for (int nIndY = 0; nIndY < 8; nIndY++)
            {
                for (int nIndX = 0; nIndX < 128; nIndX++)
                {
                    for (byte nBit = 0; nBit < 8; nBit++)
                    {
                        byte nVal = 0x01;
                        nVal = (byte)(nVal << nBit);
                        byte nData = objScreen.GetByte(nIndX + 128 * nIndY);
                        nVal = (byte)(nVal & nData);
                        if (nVal != 0)
                            e.Graphics.FillRectangle(objBrush, (nIndX + 1) * m_nZoom, (nIndY * 8 + nBit + 1 + nAdjustHeader) * m_nZoom, m_nZoom - nGap, m_nZoom - nGap);
                    }
                }
            }
        }
    }
}
