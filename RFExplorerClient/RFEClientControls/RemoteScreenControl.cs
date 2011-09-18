//============================================================================
//RF Explorer PC Client - A Handheld Spectrum Analyzer for everyone!
//Copyright © 2010-11 Ariel Rocholl, www.rf-explorer.com
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

namespace RFEClientControls
{
    public partial class RemoteScreenControl : UserControl
    {
        public byte[] m_arrRemoteScreenData;    //Data for a single screen to draw
        private int m_nZoom;                    //Zoom value 1-7
        private double m_fXStep, m_fYStep;      //This is 1/7 of the client area, defined to be used by the zoom

        //Graphis objects cached to reduce drawing overhead
        LinearGradientBrush m_BrushlinGrBrush;
        Pen m_PenDarkBlue;
        Pen m_PenRed;
        Brush m_BrushDarkBlue;
        Rectangle m_AdjustedClientRect;

        public RemoteScreenControl()
        {
            InitializeComponent();
            m_nZoom = -1;
        }

        public void UpdateZoom(int nNewZoom)
        {
            m_nZoom = nNewZoom;
            m_BrushlinGrBrush = new LinearGradientBrush(
               new Point(0, -10),
               new Point(0, ClientRectangle.Height + m_nZoom*20),
               Color.White,
               Color.LightBlue);
            m_AdjustedClientRect = ClientRectangle;
            m_AdjustedClientRect.Height = m_AdjustedClientRect.Height - 2;
            m_AdjustedClientRect.Width = m_AdjustedClientRect.Width - 2;
        }

        private void RemoteScreenControl_Load(object sender, EventArgs e)
        {
            m_arrRemoteScreenData = new byte[128 * 8];
            m_arrRemoteScreenData.Initialize();

            m_PenDarkBlue = new Pen(Color.DarkBlue, 1);
            m_PenRed = new Pen(Color.Red, 1);
            m_BrushDarkBlue = new SolidBrush(Color.DarkBlue);
        }

        private void RemoteScreenControl_Paint(object sender, PaintEventArgs e)
        {
            if (m_nZoom == -1)
                return; //uninitialized

            int nGap = 1;
            if (m_nZoom <= 4)
                nGap = 0;
            /*
             * only for video, too blurry for static image
        else
            objGraphics.SmoothingMode = SmoothingMode.AntiAlias;
             */

            e.Graphics.FillRectangle(m_BrushlinGrBrush, m_AdjustedClientRect);
            e.Graphics.DrawRectangle(m_PenDarkBlue, m_AdjustedClientRect);

            for (int nIndY = 0; nIndY < 8; nIndY++)
            {
                for (int nIndX = 0; nIndX < 128; nIndX++)
                {
                    for (byte nBit = 0; nBit < 8; nBit++)
                    {
                        byte nVal = 0x01;
                        nVal = (byte)(nVal << nBit);
                        byte nData = m_arrRemoteScreenData[nIndX + 128 * nIndY];
                        nVal = (byte)(nVal & nData);
                        if (nVal != 0)
                            e.Graphics.FillRectangle(m_BrushDarkBlue, (nIndX + 1) * m_nZoom, (nIndY * 8 + nBit + 1) * m_nZoom, m_nZoom - nGap, m_nZoom - nGap);
                    }
                }
            }
        }
    }
}
