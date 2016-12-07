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
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using ZedGraph;
using RFExplorerCommunicator;

namespace RFExplorerClient
{
    public class LimitLine : PointPairList
    {
        public enum eLimitLineType
        {
            LIMIT_MAX,
            LIMIT_MIN,
            LIMIT_NONE
        };

        eLimitLineType m_eLineType = eLimitLineType.LIMIT_NONE;
        public eLimitLineType LineType
        {
            get { return m_eLineType; }
            set { m_eLineType = value; }
        }

        public LimitLine()
        {
        }

        public LimitLine(PointPairList objList)
        {
            Add(objList);
        }

        RFECommunicator.eAmplitudeUnit m_eUnits;
        public RFECommunicator.eAmplitudeUnit AmplitudeUnits
        {
            get { return m_eUnits; }
            set 
            {
                if (value != m_eUnits)
                {
                    //add requested offset to each point
                    for (int nInd = 0; nInd < Count; nInd++)
                    {
                        this[nInd].Y = RFECommunicator.ConvertAmplitude(m_eUnits,this[nInd].Y,value);
                    }
                }
                m_eUnits = value; 
            }
        }

        /// <summary>
        /// This variable stores offset used last time. Thanks to it, you can remove an old offset and add a new one
        /// transparently. Mainly used for overload limit line, not really on other lines.
        /// </summary>
        double m_dLastOffset = 0.0;

        /// <summary>
        /// Sets a new offset in dBm, adjusting from any previous offset it may have been using so far.
        /// Note: in order to reduce any unwanted performance impact, the offset won't be adjusted if found
        /// to be the same as previously set.
        /// </summary>
        /// <param name="dOffsetDBM">new offset in dBm</param>
        public void NewOffset(double dOffsetDBM)
        {
            if (dOffsetDBM != m_dLastOffset)
            {
                //add requested offset to each point
                for (int nInd = 0; nInd < Count; nInd++)
                {
                    this[nInd].Y += (dOffsetDBM - m_dLastOffset);
                }

                m_dLastOffset = dOffsetDBM;
            }
        }

        //
        // Summary:
        //     Removes all elements from the System.Collections.Generic.List<T>.
        public new void Clear()
        {
            base.Clear();
            m_dLastOffset = 0.0;
            m_eUnits = RFECommunicator.eAmplitudeUnit.dBm; //note we use the variable, not the property, to avoid any calculation
        }

        /// <summary>
        /// It will build a limit line based on an array of compression data (see RFEAmplitudeTablaData) for overload check
        /// </summary>
        /// <param name="arrData">an array of n floats, each one with a Y coordinate in dBm, and entry points in MHZ</param>
        /// <returns>true if data processed as expected, false otherwise</returns>
        public bool CreateFromArray(float[] arrData)
        {
            Clear();

            if ((arrData == null) || (arrData.Length < 2))
                return false;

            for (int nMHZ = 0; nMHZ < arrData.Length; nMHZ++)
            {
                float fAmplitudeDBM = arrData[nMHZ];
                if (fAmplitudeDBM != RFExplorerCommunicator.RFEAmplitudeTableData.INVALID_DATA)
                {
                    Add((double)nMHZ, arrData[nMHZ]);
                }
            }

            return (Count>1);
        }

        /// <summary>
        /// If this object intersects with objList, it means the objList points are higher (bMax==true)
        /// or lower (bMax==false) than interpolated curve positions, one by one.
        /// It is designed for limit lines and is not expected to resolve actual intersections of every nature
        /// </summary>
        /// <param name="objList"></param>
        /// <param name="bMax"></param>
        /// <returns></returns>
        public bool Intersect(PointPairList objList, bool bMax)
        {
            bool bIntersect = false;

            if (Count > 1)
            {
                PointPair PointLeft = this[0];
                PointPair PointRight = this[Count - 1];

                for (int nInd = 0; !bIntersect && (nInd < objList.Count); nInd++)
                {
                    PointPair objPoint = objList[nInd];
                    if (objPoint.X < PointLeft.X)
                        continue; //we are too left, keep going
                    if (objPoint.X > PointRight.X)
                        break; //we are too far right already, done

                    if (bMax)
                    {
                        bIntersect = (objPoint.Y >= InterpolateX(objPoint.X));
                    }
                    else
                    {
                        bIntersect = (objPoint.Y <= InterpolateX(objPoint.X));
                    }
                }
            }
            return bIntersect;
        }

        public bool ReadFile(string sFilename)
        {
            using (StreamReader myFile = new StreamReader(sFilename))
            {
                string sHeader = myFile.ReadLine().TrimStart(' ');
                if (sHeader != FileHeaderVersioned())
                {
                    //unknown format
                    return false;
                }

                Clear();
                string sLine;
                do
                {
                    //Read line, trim and replace all consecutive blanks with a single tab
                    sLine = myFile.ReadLine().TrimStart(' ');
                    sLine = sLine.TrimEnd(' ');
                    sLine = sLine.Replace('\t', ' ');
                    while (sLine.Contains("  "))
                    {
                        sLine = sLine.Replace("  ", " ");
                    }

                    if (sLine.Substring(0, 2) != "--")
                    {
                        string[] arrStrings = sLine.Split(' ');
                        Add(Convert.ToDouble(arrStrings[0]), Convert.ToDouble(arrStrings[1]));
                    }
                }
                while (!myFile.EndOfStream);
            }

            return true;
        }

        private string FileHeaderVersioned()
        {
            return "--RFEL01";
        }

        /// <summary>
        /// Save limit line data to a RFL file, subtracting the offset if defined
        /// </summary>
        /// <param name="sFilename"></param>
        public void SaveFile(string sFilename)
        {
            if (Count < 2)
            {
                return;
            }

            using (StreamWriter myFile = new StreamWriter(sFilename, false))
            {
                myFile.WriteLine(FileHeaderVersioned());
                myFile.WriteLine("--RF Explorer Limit Lines data file version 01");
                myFile.WriteLine("--Sample points: " + Count);
                myFile.WriteLine("--Generated " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());

                foreach (PointPair objPoint in this)
                {
                    string sVal = objPoint.X.ToString("f3") + "\t" + RFECommunicator.ConvertAmplitude(m_eUnits,(objPoint.Y-m_dLastOffset),RFECommunicator.eAmplitudeUnit.dBm).ToString("f1");
                    myFile.WriteLine(sVal);
                }

                myFile.WriteLine("--EOF");
            }
        }

        public string Dump()
        {
            string sResult = "";

            if (Count < 2)
            {
                return sResult;
            }

            sResult = "Limit line " + Count + " steps: ";
            foreach (PointPair objPoint in this)
            {
                sResult += "[" + objPoint.X.ToString("f3") + "," + objPoint.Y.ToString("f1") + "] ";
            }

            return sResult;
        }
    }
}
