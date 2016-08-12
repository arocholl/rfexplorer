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

//#define CALLSTACK_REALTIME
//#define CALLSTACK
//#define SUPPORT_EXPERIMENTAL
//#define DEBUG

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
    public partial class MainForm : Form
    {
        #region Limit Lines
        private void OnLimitLineBuildFromSignal_Click(object sender, EventArgs e)
        {
            try
            {
                bool bAnalyzer = (m_MainTab.SelectedTab != m_tabRFGen);

                bool bMin = sender.ToString().Contains("M&in");

                PointPairList listCurrentPointList = null;

                int nSelectionCounter = SelectSinglePointPairList(ref listCurrentPointList);

                if (nSelectionCounter == 0)
                {
                    if (bAnalyzer)
                        MessageBox.Show("Condition not met: One active display curve on screen (Avg, Max, Min or Realtime)", "Limit Lines");
                    else
                        MessageBox.Show("Condition not met: no tracking signal available", "Limit Lines");
                    return;
                }
                else if (nSelectionCounter > 1)
                {
                    MessageBox.Show("Condition not met: One active display curve on screen ONLY (Avg, Max, Min or Realtime)", "Limit Lines");
                    return;
                }

                using (CreateLimitLine objDialog = new CreateLimitLine())
                {
                    if (objDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        double dIncrementAmplitudeDB = Convert.ToDouble(objDialog.m_edOffsetDB.Text);

                        if (bAnalyzer)
                        {
                            if (bMin)
                                m_LimitLineAnalyzer_Min = new LimitLine(listCurrentPointList);
                            else
                                m_LimitLineAnalyzer_Max = new LimitLine(listCurrentPointList);

                            //add requested offset to each point - NOTE: This is not the RFE measurement offset so the 
                            //function SetNewOffset is not appropriate here!
                            for (int nInd = 0; nInd < listCurrentPointList.Count; nInd++)
                            {
                                if (bMin)
                                    m_LimitLineAnalyzer_Min[nInd].Y -= dIncrementAmplitudeDB;
                                else
                                    m_LimitLineAnalyzer_Max[nInd].Y += dIncrementAmplitudeDB;
                            }

                            if (bMin)
                                m_LimitLineAnalyzer_Min.AmplitudeUnits = GetCurrentAmplitudeEnum();
                            else
                                m_LimitLineAnalyzer_Max.AmplitudeUnits = GetCurrentAmplitudeEnum();

                            DisplaySpectrumAnalyzerData();
                        }
                        else
                        {
                            if (bMin)
                                m_LimitLineGenerator_Min = new LimitLine(listCurrentPointList);
                            else
                                m_LimitLineGenerator_Max = new LimitLine(listCurrentPointList);

                            //add requested offset to each point - NOTE: This is not the RFE measurement offset so the 
                            //function SetNewOffset is not appropriate here!
                            for (int nInd = 0; nInd < listCurrentPointList.Count; nInd++)
                            {
                                if (bMin)
                                    m_LimitLineGenerator_Min[nInd].Y -= dIncrementAmplitudeDB;
                                else
                                    m_LimitLineGenerator_Max[nInd].Y += dIncrementAmplitudeDB;
                            }

                            DisplayTrackingData();
                        }
                    }
                }
            }
            catch (Exception obEx) { MessageBox.Show(obEx.Message); }
        }

        private void OnLimitLineSaveToFile_Click(object sender, EventArgs e)
        {
            bool bAnalyzer = (m_MainTab.SelectedTab != m_tabRFGen);

            using (SaveFileDialog MySaveFileDialog = new SaveFileDialog())
            {
                MySaveFileDialog.Filter = _RFL_File_Selector;
                MySaveFileDialog.FilterIndex = 1;
                MySaveFileDialog.RestoreDirectory = false;
                MySaveFileDialog.InitialDirectory = m_sDefaultUserFolder;

                MySaveFileDialog.FileName = GetNewFilename(RFExplorerFileType.LimitLineDataFile);

                if (MySaveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (bAnalyzer)
                    {
                        if (sender.ToString().Contains("M&in"))
                            m_LimitLineAnalyzer_Min.SaveFile(MySaveFileDialog.FileName);
                        else
                            m_LimitLineAnalyzer_Max.SaveFile(MySaveFileDialog.FileName);
                    }
                    else
                    {
                        if (sender.ToString().Contains("M&in"))
                            m_LimitLineGenerator_Min.SaveFile(MySaveFileDialog.FileName);
                        else
                            m_LimitLineGenerator_Max.SaveFile(MySaveFileDialog.FileName);
                    }
                    m_sDefaultUserFolder = Path.GetDirectoryName(MySaveFileDialog.FileName);
                    edDefaultFilePath.Text = m_sDefaultUserFolder;
                }
            }
        }

        private void OnRemoveMaxLimitLine_Click(object sender, EventArgs e)
        {
            if (m_MainTab.SelectedTab != m_tabRFGen)
            {
                m_LimitLineAnalyzer_Max.Clear();
                DisplaySpectrumAnalyzerData();
            }
            else
            {
                m_LimitLineGenerator_Max.Clear();
                DisplayTrackingData();
            }
        }

        private void OnRemoveMinLimitLine_Click(object sender, EventArgs e)
        {
            if (m_MainTab.SelectedTab != m_tabRFGen)
            {
                m_LimitLineAnalyzer_Min.Clear();
                DisplaySpectrumAnalyzerData();
            }
            else
            {
                m_LimitLineGenerator_Min.Clear();
                DisplayTrackingData();
            }
        }

        private void OnLimitLineReadFromFile_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog MyOpenFileDialog = new OpenFileDialog())
                {
                    MyOpenFileDialog.Filter = _RFL_File_Selector;
                    MyOpenFileDialog.FilterIndex = 1;
                    MyOpenFileDialog.RestoreDirectory = false;
                    MyOpenFileDialog.InitialDirectory = m_sDefaultUserFolder;

                    if (MyOpenFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        bool bOk = false;

                        if (m_MainTab.SelectedTab != m_tabRFGen)
                        {
                            if (sender.ToString().Contains("M&in"))
                            {
                                bOk = m_LimitLineAnalyzer_Min.ReadFile(MyOpenFileDialog.FileName);
                                if (bOk)
                                    m_LimitLineAnalyzer_Min.AmplitudeUnits = GetCurrentAmplitudeEnum();
                            }
                            else
                            {
                                bOk = m_LimitLineAnalyzer_Max.ReadFile(MyOpenFileDialog.FileName);
                                if (bOk)
                                    m_LimitLineAnalyzer_Max.AmplitudeUnits = GetCurrentAmplitudeEnum();
                            }

                            if (bOk)
                                DisplaySpectrumAnalyzerData();
                        }
                        else
                        {
                            if (sender.ToString().Contains("M&in"))
                            {
                                bOk = m_LimitLineGenerator_Min.ReadFile(MyOpenFileDialog.FileName);
                                if (bOk)
                                    m_LimitLineGenerator_Min.AmplitudeUnits = GetCurrentAmplitudeEnum();
                            }
                            else
                            {
                                bOk = m_LimitLineGenerator_Max.ReadFile(MyOpenFileDialog.FileName);
                                if (bOk)
                                    m_LimitLineGenerator_Max.AmplitudeUnits = GetCurrentAmplitudeEnum();
                            }

                            if (bOk)
                                DisplayTrackingData();
                        }

                        if (!bOk)
                        {
                            MessageBox.Show("Failed to read file, check format and file data");
                        }
                        m_sDefaultUserFolder = Path.GetDirectoryName(MyOpenFileDialog.FileName);
                        edDefaultFilePath.Text = m_sDefaultUserFolder;
                    }
                }
            }
            catch (Exception obEx) { MessageBox.Show(obEx.Message); }
        }

        private void OnItemSoundAlarmLimitLine_Click(object sender, EventArgs e)
        {
            if (menuItemSoundAlarmLimitLine.Checked == false)
                m_SoundPlayer.Stop();
        }
        #endregion
    }
}		