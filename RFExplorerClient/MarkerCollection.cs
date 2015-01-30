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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using ZedGraph;
using System.IO.Ports;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Collections;
using Microsoft.Win32;
using System.Diagnostics;
using RFExplorerCommunicator;
using SharpGL;
using SharpGL.Enumerations;
using System.Media;

namespace RFExplorerClient
{
    class MarkerCollection
    {
        //Array of colors mapped to RFExplorerSignalType 
        public Color[] m_arrCurveColors = { Color.Blue, Color.Red, Color.Brown, Color.Salmon, Color.DarkGreen };

        public const int MAX_MARKERS = 10;         //Total allowed markers
        public MarkerObj[][] m_arrMarkers;          //List of all arrow markers

        public void Initialize()
        {
            m_arrMarkers = new MarkerObj[MAX_MARKERS][];
            for (int nInd1 = 0; nInd1 < MAX_MARKERS; nInd1++)
            {
                m_arrMarkers[nInd1] = new MarkerObj[(int)RFExplorerSignalType.TOTAL_ITEMS];

                for (int nInd2 = 0; nInd2 < (int)RFExplorerSignalType.TOTAL_ITEMS; nInd2++)
                {
                    m_arrMarkers[nInd1][nInd2] = new MarkerObj(m_arrCurveColors[nInd2], 17, 0, 0);
                    m_arrMarkers[nInd1][nInd2].ZOrder = ZOrder.A_InFront;
                    m_arrMarkers[nInd1][nInd2].IsClippedToChartRect = true;
                    m_arrMarkers[nInd1][nInd2].Line.Style = DashStyle.Solid;
                    m_arrMarkers[nInd1][nInd2].IsVisible = false;
                    m_arrMarkers[nInd1][nInd2].InsideText = (nInd1 + 1).ToString("0");
                    m_arrMarkers[nInd1][nInd2].Location.X = 1000.0; //initialize to 1000MHz, a rather random value
                }
            }
        }

        public void ConnectToGraph(ZedGraph.GraphPane objPane)
        {
            for (int nInd1 = 0; nInd1 < MAX_MARKERS; nInd1++)
            {
                for (int nInd2 = 0; nInd2 < (int)RFExplorerSignalType.TOTAL_ITEMS; nInd2++)
                {
                    objPane.GraphObjList.Add(m_arrMarkers[nInd1][nInd2]);
                }
            }
        }

        /// <summary>
        /// Sets the dominant, main marker frequency in all markers of this ID
        /// </summary>
        /// <param name="nMarkerID"></param>
        /// <param name="dFrequencyMHZ"></param>
        public void SetMarkerFrequency(int nMarkerID, double dFrequencyMHZ)
        {
            if (m_arrMarkers != null)
            {
                foreach (MarkerObj objMarker in m_arrMarkers[nMarkerID])
                {
                    objMarker.Location.X = dFrequencyMHZ;
                }
            }
        }

        /// <summary>
        /// Gets the dominant marker frequency
        /// </summary>
        /// <param name="nMarkerID"></param>
        /// <returns></returns>
        public double GetMarkerFrequency(int nMarkerID)
        {
            if (m_arrMarkers != null)
            {
                return m_arrMarkers[nMarkerID][0].Location.X;
            }
            else
            {
                return 0.0;
            }
        }

        /// <summary>
        /// Gets the dominant marker amplitude in whatever current measurement unit was stored
        /// </summary>
        /// <param name="nMarkerID"></param>
        /// <returns></returns>
        public double GetMarkerAmplitude(int nMarkerID, RFExplorerSignalType eSignalType)
        {
            if (m_arrMarkers != null)
            {
                return m_arrMarkers[nMarkerID][(int)eSignalType].Location.Y;
            }
            else
            {
                return 0.0;
            }
        }

        /// <summary>
        /// Update a marker amplitude based on signal type and marker ID
        /// </summary>
        /// <param name="nMarkerID"></param>
        /// <param name="eType"></param>
        /// <param name="dAmplitude"></param>
        public void UpdateMarker(int nMarkerID, RFExplorerSignalType eType, double dAmplitude)
        {
            m_arrMarkers[nMarkerID][(int)eType].Location.X = m_arrMarkers[nMarkerID][0].Location.X;
            m_arrMarkers[nMarkerID][(int)eType].Location.Y = dAmplitude;
            m_arrMarkers[nMarkerID][(int)eType].IsVisible = true;
        }

        public void CleanAllMarkerText(int nMarkerID)
        {
            foreach (MarkerObj objMarker in m_arrMarkers[nMarkerID])
            {
                objMarker.FullText = "";
            }
        }

        public void SetMarkerText(int nMarkerID, RFExplorerSignalType eType, string sText)
        {
            m_arrMarkers[nMarkerID][(int)eType].FullText = sText;
        }

        /// <summary>
        /// Hide all markers of this ID
        /// </summary>
        /// <param name="nMarkerID"></param>
        public void HideMarker(int nMarkerID)
        {
            foreach (MarkerObj objMarker in m_arrMarkers[nMarkerID])
            {
                objMarker.IsVisible = false;
            }
        }

        public bool IsMarkerEnabled(int nMarkerID)
        {
            return m_arrMarkers[nMarkerID][0].IsVisible;
        }

        public void EnableMarker(int nMarkerID)
        {
            foreach (MarkerObj objMarker in m_arrMarkers[nMarkerID])
            {
                objMarker.IsVisible = true;
            }
        }

        public void HideAllMarkers()
        {
            for (int nInd = 0; nInd < MAX_MARKERS; nInd++)
            {
                HideMarker(nInd);
            }
        }
    }
}
