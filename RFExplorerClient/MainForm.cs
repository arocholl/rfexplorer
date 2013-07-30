//============================================================================
//RF Explorer for Windows - A Handheld Spectrum Analyzer for everyone!
//Copyright © 2010-13 Ariel Rocholl, www.rf-explorer.com
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

namespace RFExplorerClient
{
    public partial class MainForm : Form
    {
        #region Data Members
        RFECommunicator m_objRFE;   //The one and only RFE connected object

        string m_sLastSettingsLoaded = "Default";

        int m_nDrawingIteration = 0;        //Iteration counter to do regular updates on GUI

        DataSet m_DataSettings;             //Settings data collection
        bool m_bPropertiesReadOk = false;   //A global flag to indicate the properties were correctly read from file, otherwise we should not persist them later
                                            //as it may indicate some type of crash and damage default settings
        bool m_bVersionAlerted = false;     //Used to alert about firmware version popup only once per session

#if SUPPORT_EXPERIMENTAL
        enum eModulation
        {
            MODULATION_OOK,         //0
            MODULATION_PSK,         //1
            MODULATION_NONE = 0xFF  //0xFF
        };
        eModulation m_eModulation;          //Modulation being used

        UInt16 m_nRAWSnifferIndex=0;        //Index pointing to current RAW data value shown
        UInt16 m_nMaxRAWSnifferIndex=0;     //Index pointing to the last RAW data value available
        string[] m_arrRAWSnifferData;       //Array of strings for sniffer data
#endif
        string m_sFilenameRFE = "";         //RFE data file name
        string m_sReportFilePath = "";      //Path and name of the report log file

        const float m_fSizeX = 130;         //Size of the dump screen in pixels (128x64 + 2 border) + 20 height for text header
        const float m_fSizeY = 66;

        bool m_bDrawRealtime = true;        //True if realtime data should be displayed
        bool m_bDrawAverage = true;         //True if averaged data should be displayed
        bool m_bDrawMax = true;             //True if max data should be displayed
        bool m_bDrawMaxHold = true;         //True if max hold data should be displayed
        bool m_bDrawMin = true;             //True if min data should be displayed
        bool m_bShowPeaks = true;           //True if peak text with MHZ/dBm should be displayed

        bool m_bDark = false;               //True for a Dark mode combination active

        bool m_bFirstTick = true;           //Used to put some text and guarantee action done once after mainform load
        bool m_bFirstText = true;           //First report text printed

        Pen m_PenDarkBlue;                  //Graphis objects cached to reduce drawing overhead
        Pen m_PenRed;
        Brush m_BrushDarkBlue;

        TextObj m_RealtimePeak, m_AveragePeak, m_MaxPeak; //Max dynamic text
        TextObj[] m_arrWiFiBarText;         //Text for the 13 Wifi channels
        TextObj m_RFEConfig;                //Configuration data received from RF Explorer

        bool m_bIsWinXP = false;            //True if it is a Windows XP platform, which has some GUI differences with Win7/Vista

        LineItem m_AvgLine, m_MinLine, m_RealtimeLine, m_MaxLine, m_MaxHoldLine;   //Line curve item for the analyzer zed graph
        PointPairList m_PointListRealtime, m_PointListMax,
            m_PointListMin, m_PointListAverage, m_PointListMaxHold;  //pair list used by the line curve items
        BarItem m_MaxBar;                   //Bar curve used by the wifi analyzer

        Button[] m_arrAnalyzerButtonList = new Button[14];

        string m_sAppDataFolder = "";       //Default folder based on %APPDATA% to store log and report files
        string m_sDefaultDataFolder = "";   //Default folder to store CSV and RFE or other data files
        string m_sSettingsFile = "";        //Filename and path of the named settings file

        bool m_bPrintModeEnabled = false;   //Will be true when the painting is being done for printing, mainly used to remove black background

        #endregion

        #region RFExplorer Events
        private void OnRFE_ReceivedConfigData(object sender, EventArgs e)
        {
            m_objRFE.SweepData.CleanAll(); //we do not want mixed data sweep values

            if (m_bCalibrating)
                return; //do not actually display data updates while calibration is in place

            if (m_objRFE.Mode == RFECommunicator.eMode.MODE_WIFI_ANALYZER)
            {
                //objGraph is a bar chart
                m_bDrawMax = true; //Max mode is the only one drawn with a bar, so it must be included here
                m_bDrawAverage = false;
                m_bDrawMin = false;
                m_bDrawRealtime = false;
                UpdateButtonStatus();
                if (numericIterations.Value < 100)
                    numericIterations.Value = 100; //at least 100 is required for a practical view in this mode
                zedSpectrumAnalyzer.GraphPane.XAxis.Scale.Min = m_objRFE.StartFrequencyMHZ - 5.0f;
                zedSpectrumAnalyzer.GraphPane.XAxis.Scale.Max = m_objRFE.StartFrequencyMHZ + m_objRFE.FreqSpectrumSteps * m_objRFE.StepFrequencyMHZ + 5.0f;
            }

            SetupSpectrumAnalyzerAxis();
            SaveProperties();

            if (MainTab.SelectedTab == tabConfiguration)
            {
                MainTab.SelectedTab = tabSpectrumAnalyzer;
            }
            DisplayGroups();
        }

        private void OnRFE_UpdateData(object sender, EventArgs e)
        {
            RFESweepData objData = m_objRFE.SweepData.GetData((uint)m_objRFE.SweepData.Count - 1);
            if (objData != null)
            {
                m_RFEConfig.Text = m_objRFE.ModelText + '\n' + m_objRFE.ConfigurationText + '\n' + m_objRFE.SweepInfoText;

                UpdateSweepNumericControls();
            }
        }

        private void OnRFE_UpdateRemoteScreen(object sender, EventArgs e)
        {
            RFEScreenData objData = m_objRFE.ScreenData.GetData((UInt16)m_objRFE.ScreenIndex);
            if (objData != null)
            {
                UpdateScreenNumericControls();
                //Update button status but first time only to minimize overhead
                if (m_objRFE.ScreenIndex == 1)
                    UpdateButtonStatus();

                if (MainTab.SelectedTab == tabRemoteScreen)
                    tabRemoteScreen.Invalidate();
            }
        }

        private void OnRFE_ReportLog(object sender, EventArgs e)
        {
            EventReportInfo objArg = (EventReportInfo)e;

            string sLine = objArg.Data;
            ReportLog(sLine, false);
        }

        private void OnRFE_PortClosed(object sender, EventArgs e)
        {
            UpdateButtonStatus();
            UpdateFeedMode();
        }

        private void OnRFE_UpdateCalibration(object sender, EventArgs e)
        {
        }

        private void OnRFE_ReceivedDeviceModel(object sender, EventArgs e)
        {
            DisplayRequiredFirmware();
        }
        #endregion

        #region Main Window

        public About_RFExplorer m_winAboutModeless;

        public MainForm(string sFile)
        {
            m_sFilenameRFE = sFile;

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            InitializeComponent();
            InitializeOldWaterfall();

            m_objRFE = new RFECommunicator();
            m_objRFE.PortClosed += new EventHandler(OnRFE_PortClosed);
            m_objRFE.ReportInfoAdded += new EventHandler(OnRFE_ReportLog);
            m_objRFE.ReceivedConfigurationData += new EventHandler(OnRFE_ReceivedConfigData);
            m_objRFE.UpdateData += new EventHandler(OnRFE_UpdateData);
            m_objRFE.UpdateRemoteScreen+= new EventHandler(OnRFE_UpdateRemoteScreen);
            m_objRFE.ReceivedDeviceModel+=new EventHandler(OnRFE_ReceivedDeviceModel);

            printMainDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler (this.printDocument_PrintPage);
            printMainDocument.DocumentName = "RF Explorer";
        }

        private void printDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            //PaintScreen(e.Graphics, e.MarginBounds, true);
            e.HasMorePages = false;
        }

        private void menuRFConnections_Click(object sender, EventArgs e)
        {
            DisplayGroups();
        }

        private void CreateSettingsSchema()
        {
            try
            {
                DataTable objTableCommon = m_DataSettings.Tables.Add("Common_Settings");

                objTableCommon.Columns.Add(new DataColumn("Name", System.Type.GetType("System.String")));
                objTableCommon.Columns.Add(new DataColumn("StartFreq", System.Type.GetType("System.Double")));
                objTableCommon.Columns.Add(new DataColumn("StepFreq", System.Type.GetType("System.Double")));
                objTableCommon.Columns.Add(new DataColumn("TopAmp", System.Type.GetType("System.Double")));
                objTableCommon.Columns.Add(new DataColumn("BottomAmp", System.Type.GetType("System.Double")));
                objTableCommon.Columns.Add(new DataColumn("Calculator", System.Type.GetType("System.UInt16")));
                objTableCommon.Columns.Add(new DataColumn("ViewAvg", System.Type.GetType("System.Boolean")));
                objTableCommon.Columns.Add(new DataColumn("ViewRT", System.Type.GetType("System.Boolean")));
                objTableCommon.Columns.Add(new DataColumn("ViewMin", System.Type.GetType("System.Boolean")));
                objTableCommon.Columns.Add(new DataColumn("ViewMax", System.Type.GetType("System.Boolean")));
                objTableCommon.Columns.Add(new DataColumn("ViewMaxHold", System.Type.GetType("System.Boolean")));

                DataRow objRow = objTableCommon.NewRow();
                objRow["Name"] = "Default";
                objRow["StartFreq"] = 430.000;
                objRow["StepFreq"] = 0.500;
                objRow["TopAmp"] = 5;
                objRow["BottomAmp"] = -120;
                objRow["Calculator"] = 10;
                objRow["ViewAvg"] = true;
                objRow["ViewRT"] = true;
                objRow["ViewMin"] = false;
                objRow["ViewMax"] = false;
                objRow["ViewHold"] = false;
                objTableCommon.Rows.Add(objRow);

                m_DataSettings.WriteXml(m_sSettingsFile, XmlWriteMode.WriteSchema);
            }
            catch (Exception objEx)
            {
                MessageBox.Show(objEx.Message);
            }
        }

        private void btnLoadSettings_Click(object sender, EventArgs e)
        {
            RestoreSettingsXML(menuComboSavedOptions.Text);
        }

        private void m_chkDebug_CheckedChanged(object sender, EventArgs e)
        {
        }
        
        private void menuComboSavedOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnDelSettings.Enabled = menuComboSavedOptions.Text != "Default";
        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            string sItem = menuComboSavedOptions.Text;
            SaveSettingsXML(sItem);
            PopulateReadedSettings();
            menuComboSavedOptions.SelectedItem = sItem;
        }

        private void btnDelSettings_Click(object sender, EventArgs e)
        {
            string sItem = menuComboSavedOptions.Text;
            DeleteSettingsXML(sItem);
            menuComboSavedOptions.Items.Remove(sItem);
            menuComboSavedOptions.SelectedItem = "Default";
        }

        private void DefineGraphColors(ZedGraphControl objGraph)
        {
            GraphPane myPane = objGraph.GraphPane;

            if (m_bDark)
            {
                Color DarkColor = Color.Black;
                Color FontColor = Color.White;
                Color BackgroundColor = Color.DarkGray;

                if (m_bPrintModeEnabled)
                {
                    DarkColor = Color.White;
                    FontColor = Color.Black;
                    BackgroundColor = Color.White;
                }

                this.BackColor = BackgroundColor;
                tabSpectrumAnalyzer.BackColor = BackgroundColor;
                tabReport.BackColor = BackgroundColor;
                tabRemoteScreen.BackColor = BackgroundColor;
                tabConfiguration.BackColor = BackgroundColor;

                myPane.Fill = new Fill(DarkColor);
                myPane.Chart.Fill = new Fill(DarkColor);
                myPane.Title.FontSpec.FontColor = FontColor;

                m_RFEConfig.FontSpec.FontColor = Color.LightGray;
                m_RFEConfig.FontSpec.DropShadowColor = Color.DarkRed;

                myPane.YAxis.Title.FontSpec.FontColor = FontColor;
                myPane.XAxis.Title.FontSpec.FontColor = FontColor;
                myPane.YAxis.Scale.FontSpec.FontColor = FontColor;
                myPane.XAxis.Scale.FontSpec.FontColor = FontColor;

                myPane.Chart.Border.Color = Color.Gray;
                myPane.Chart.Border.IsAntiAlias = true;

                myPane.Legend.FontSpec.FontColor = FontColor;
                myPane.Legend.Fill.Color = DarkColor;
                myPane.Legend.Fill.SecondaryValueGradientColor = DarkColor;
                myPane.Legend.Fill = new Fill(DarkColor);
            }
            else
            {
                this.BackColor = Color.LightYellow;
                tabSpectrumAnalyzer.BackColor = Color.LightYellow;
                tabReport.BackColor = Color.LightYellow;
                tabRemoteScreen.BackColor = Color.LightYellow;
                tabConfiguration.BackColor = Color.LightYellow;
                tabWaterfall.BackColor = Color.LightYellow;

                myPane.Fill = new Fill(Color.White, Color.LightYellow, 90.0f);
                myPane.Chart.Fill = new Fill(Color.White, Color.LightYellow, 90.0f);

                m_RFEConfig.FontSpec.FontColor = Color.DarkBlue;
                m_RFEConfig.FontSpec.DropShadowColor = Color.LightGray;

                myPane.Title.FontSpec.FontColor = Color.Black;

                myPane.YAxis.Title.FontSpec.FontColor = Color.Black;
                myPane.XAxis.Title.FontSpec.FontColor = Color.Black;
                myPane.YAxis.Scale.FontSpec.FontColor = Color.Black;
                myPane.XAxis.Scale.FontSpec.FontColor = Color.Black;

                myPane.Chart.Border.Color = Color.Gray;
                myPane.Chart.Border.IsAntiAlias = true;

                myPane.Legend.FontSpec.FontColor = Color.Black;
                myPane.Legend.Fill.Color = Color.LightYellow;
                myPane.Legend.Fill = new Fill(Color.LightYellow);
            }

            myPane.YAxis.MajorGrid.Color = Color.Gray;
            myPane.YAxis.MinorGrid.Color = Color.Gray;
            myPane.XAxis.MajorGrid.Color = Color.Gray;
            myPane.XAxis.MinorGrid.Color = Color.Gray;

            myPane.YAxis.MajorTic.Color = Color.Gray;
            myPane.YAxis.MinorTic.Color = Color.Gray;
            myPane.XAxis.MajorTic.Color = Color.Gray;
            myPane.XAxis.MinorTic.Color = Color.Gray; 
            
            myPane.YAxis.Title.FontSpec.Size = 13;
            myPane.XAxis.Title.FontSpec.Size = 13;
            myPane.YAxis.Scale.FontSpec.Size = 10;
            myPane.XAxis.Scale.FontSpec.Size = 10;

            // Fill the axis background with a gradient
            myPane.Legend.IsHStack = true;
            myPane.Legend.FontSpec.Size = 12;

            // Enable scrollbars if needed
            objGraph.IsAutoScrollRange = true;
        }

        private void InitializeSpectrumAnalyzerGraph()
        {
            // Get a reference to the GraphPane instance in the ZedGraphControl
            GraphPane myPane = zedSpectrumAnalyzer.GraphPane;

            m_PointListRealtime = new PointPairList();
            m_PointListMax = new PointPairList();
            m_PointListMaxHold = new PointPairList();
            m_PointListMin = new PointPairList();
            m_PointListAverage = new PointPairList();

            m_MaxBar = zedSpectrumAnalyzer.GraphPane.AddHiLowBar("Max", m_PointListMax, Color.Red);
            m_MaxBar.Bar.Border.Color = Color.DarkRed;
            m_MaxBar.Bar.Border.Width = 3;
            m_AvgLine = zedSpectrumAnalyzer.GraphPane.AddCurve("Avg", m_PointListAverage, Color.Brown, SymbolType.None);
            m_AvgLine.Line.Width = 3;
            m_AvgLine.Line.SmoothTension = 0.3F;
            m_AvgLine.Line.IsSmooth = true;
            m_MinLine = zedSpectrumAnalyzer.GraphPane.AddCurve("Min", m_PointListMin, Color.DarkGreen, SymbolType.None);
            m_MinLine.Line.Width = 3;
            m_MinLine.Line.SmoothTension = 0.3F;
            m_MinLine.Line.IsSmooth = true;
            m_RealtimeLine = zedSpectrumAnalyzer.GraphPane.AddCurve("Realtime", m_PointListRealtime, Color.Blue, SymbolType.None);
            m_RealtimeLine.Line.Width = 4;
            m_RealtimeLine.Line.SmoothTension = 0.2F;
            m_RealtimeLine.Line.IsSmooth = true;
            m_MaxLine = zedSpectrumAnalyzer.GraphPane.AddCurve("Max", m_PointListMax, Color.Red, SymbolType.None);
            m_MaxLine.Line.Width = 3;
            m_MaxLine.Line.SmoothTension = 0.3F;
            m_MaxLine.Line.IsSmooth = true;
            m_MaxHoldLine = zedSpectrumAnalyzer.GraphPane.AddCurve("Max Hold", m_PointListMaxHold, Color.Salmon, SymbolType.None);
            m_MaxHoldLine.Line.Width = 10;
            m_MaxHoldLine.Line.SmoothTension = 0.3F;
            m_MaxHoldLine.Line.IsSmooth = true;

            foreach (CurveItem objCurve in zedSpectrumAnalyzer.GraphPane.CurveList)
            {
                objCurve.IsVisible = false;
                objCurve.Label.IsVisible = false;
            }

            m_arrAnalyzerButtonList[0] = btnTop5plus;
            m_arrAnalyzerButtonList[1] = btnTop5minus;
            m_arrAnalyzerButtonList[2] = btnMoveFreqDecLarge;
            m_arrAnalyzerButtonList[3] = btnMoveFreqDecSmall;
            m_arrAnalyzerButtonList[4] = btnSpanInc;
            m_arrAnalyzerButtonList[5] = btnSpanMax;
            m_arrAnalyzerButtonList[6] = btnSpanDefault;
            m_arrAnalyzerButtonList[7] = btnCenterMark;
            m_arrAnalyzerButtonList[8] = btnSpanMin;
            m_arrAnalyzerButtonList[9] = btnSpanDec;
            m_arrAnalyzerButtonList[10] = btnMoveFreqIncLarge;
            m_arrAnalyzerButtonList[11] = btnMoveFreqIncSmall;
            m_arrAnalyzerButtonList[12] = btnBottom5plus;
            m_arrAnalyzerButtonList[13] = btnBottom5minus;

            // Set the titles and axis labels
            //myPane.Title.FontSpec.Size = 10;
            //myPane.Title.IsVisible = false;
            myPane.XAxis.Title.Text = "Frequency (MHZ)";
            myPane.XAxis.Scale.MajorStep = 1.0;
            myPane.XAxis.Scale.MinorStep = 0.2;

            myPane.Margin.Left = 20;
            myPane.Margin.Right = -5;

            // Show the x axis grid
            myPane.XAxis.MajorGrid.IsVisible = true;
            myPane.XAxis.Type = AxisType.Linear;

            myPane.YAxis.Title.Text = "Amplitude (dBm)";
            //myPane.YAxis.Scale.FontSpec.FontColor = Color.Yellow;
            //myPane.YAxis.Title.FontSpec.FontColor = Color.Blue;
            myPane.YAxis.Type = AxisType.Linear;
            // turn off the opposite tics so the Y tics don't show up on the Y2 axis
            myPane.YAxis.MajorTic.IsOpposite = false;
            myPane.YAxis.MinorTic.IsOpposite = false;
            myPane.YAxis.MajorGrid.IsVisible = true;
            // Don't display the Y zero line
            myPane.YAxis.MajorGrid.IsZeroLine = false;
            // Align the Y axis labels so they are flush to the axis
            myPane.YAxis.Scale.Align = AlignP.Inside;
            // Manually set the axis range
            myPane.YAxis.Scale.Min = RFECommunicator.MIN_AMPLITUDE_DBM;
            myPane.YAxis.Scale.Max = RFECommunicator.MAX_AMPLITUDE_DBM;
            myPane.YAxis.Scale.MajorStep = 20.0;
            myPane.YAxis.Scale.MinorStep = 5.0;

            zedSpectrumAnalyzer.IsShowPointValues = true;
            zedSpectrumAnalyzer.PointValueEvent += new ZedGraphControl.PointValueHandler(SpectrumAnalyzerPointValueHandler);

            myPane.BarSettings.Type = BarType.Overlay;
            myPane.BarSettings.MinBarGap = 0.1f;
            myPane.BarSettings.MinClusterGap = 0.1f;
            myPane.BarSettings.ClusterScaleWidthAuto = false;
            myPane.BarSettings.ClusterScaleWidth = 5.0f;

            m_RFEConfig = new TextObj("RF Explorer DISCONNECTED", 0.01, 0.02, CoordType.ChartFraction);
            m_RFEConfig.IsClippedToChartRect = true;
            //m_RFEConfig.ZOrder = 0;
            m_RFEConfig.FontSpec.FontColor = Color.DarkGray;
            m_RFEConfig.Location.AlignH = AlignH.Left;
            m_RFEConfig.Location.AlignV = AlignV.Top;
            m_RFEConfig.FontSpec.IsBold = true;
            m_RFEConfig.FontSpec.Size = 8f;
            m_RFEConfig.FontSpec.Border.IsVisible = false;
            m_RFEConfig.FontSpec.Fill.IsVisible = false;
            m_RFEConfig.FontSpec.StringAlignment = StringAlignment.Near;
            m_RFEConfig.FontSpec.IsDropShadow = true;
            m_RFEConfig.FontSpec.DropShadowOffset = 0.1f;
            m_RFEConfig.FontSpec.Family = "Arial Narrow";
            myPane.GraphObjList.Add(m_RFEConfig);

            m_RealtimePeak = new TextObj("", 0, 0, CoordType.AxisXYScale);
            m_RealtimePeak.IsClippedToChartRect = true;
            m_RealtimePeak.Location.AlignH = AlignH.Center;
            m_RealtimePeak.Location.AlignV = AlignV.Bottom;
            m_RealtimePeak.FontSpec.Size = 8f;
            m_RealtimePeak.FontSpec.Border.IsVisible = false;
            m_RealtimePeak.FontSpec.FontColor = Color.Blue;
            m_RealtimePeak.FontSpec.StringAlignment = StringAlignment.Center;
            m_RealtimePeak.FontSpec.Fill.IsVisible = false;
            myPane.GraphObjList.Add(m_RealtimePeak);

            m_MaxPeak = new TextObj("", 0, 0, CoordType.AxisXYScale);
            m_MaxPeak.IsClippedToChartRect = true;
            m_MaxPeak.Location.AlignH = AlignH.Center;
            m_MaxPeak.Location.AlignV = AlignV.Bottom;
            m_MaxPeak.FontSpec.Size = 8;
            m_MaxPeak.FontSpec.Border.IsVisible = false;
            m_MaxPeak.FontSpec.FontColor = Color.Red;
            m_MaxPeak.FontSpec.StringAlignment = StringAlignment.Center;
            m_MaxPeak.FontSpec.Fill.IsVisible = false;
            myPane.GraphObjList.Add(m_MaxPeak);

            m_arrWiFiBarText = new TextObj[13];
            m_arrWiFiBarText.Initialize();
            for (int nInd = 0; nInd < m_arrWiFiBarText.Length; nInd++)
            {
                m_arrWiFiBarText[nInd] = new TextObj("", 0, 0, CoordType.AxisXYScale);
                m_arrWiFiBarText[nInd].IsClippedToChartRect = true;
                m_arrWiFiBarText[nInd].Location.AlignH = AlignH.Center;
                m_arrWiFiBarText[nInd].Location.AlignV = AlignV.Bottom;
                m_arrWiFiBarText[nInd].FontSpec.Size = 7;
                m_arrWiFiBarText[nInd].FontSpec.Border.IsVisible = false;
                m_arrWiFiBarText[nInd].FontSpec.FontColor = Color.Red;
                m_arrWiFiBarText[nInd].FontSpec.StringAlignment = StringAlignment.Center;
                m_arrWiFiBarText[nInd].FontSpec.Fill.IsVisible = false;
                myPane.GraphObjList.Add(m_arrWiFiBarText[nInd]);
            }

            m_AveragePeak = new TextObj("", 0, 0, CoordType.AxisXYScale);
            m_AveragePeak.IsClippedToChartRect = true;
            m_AveragePeak.Location.AlignH = AlignH.Center;
            m_AveragePeak.Location.AlignV = AlignV.Bottom;
            m_AveragePeak.FontSpec.Size = 8;
            m_AveragePeak.FontSpec.Border.IsVisible = false;
            m_AveragePeak.FontSpec.FontColor = Color.Brown;
            m_AveragePeak.FontSpec.StringAlignment = StringAlignment.Center;
            m_AveragePeak.FontSpec.Fill.IsVisible = false;
            myPane.GraphObjList.Add(m_AveragePeak);

            DefineGraphColors(zedSpectrumAnalyzer);
        }

        private void menuAutoLCDOff_Click(object sender, EventArgs e)
        {
            if (menuAutoLCDOff.Checked)
            {
                m_objRFE.SendCommand_ScreenOFF();
                chkDumpScreen.Checked = false;
            }
            else
            {
                m_objRFE.SendCommand_ScreenON();
            }
            UpdateButtonStatus();
        }

        private void menuPrintPreview_Click(object sender, EventArgs e)
        {
            m_bPrintModeEnabled = true;
            try
            {
                if (MainTab.SelectedTab == tabSpectrumAnalyzer)
                {
                    DefineGraphColors(zedSpectrumAnalyzer);
                    //zedSpectrumAnalyzer.DoPrintPreview() - not usable, it makes the dialog modeless and cannot set white color
                    printPreviewDialog.Document = zedSpectrumAnalyzer.PrintDocument;
                }
                else
                {
                    printPreviewDialog.Document = printMainDocument;
                }

                if (printPreviewDialog.ShowDialog() == DialogResult.OK)
                {
                    printMainDocument.Print();
                }
            }
            catch (Exception obEx)
            {
                ReportLog(obEx.Message, false);
            }
            m_bPrintModeEnabled = false;
            DefineGraphColors(zedSpectrumAnalyzer);
        }

        private void menuPrint_Click(object sender, EventArgs e)
        {
            m_bPrintModeEnabled = true;
            try
            {
                DefineGraphColors(zedSpectrumAnalyzer);
                zedSpectrumAnalyzer.DoPrint();
            }
            catch (Exception obEx)
            {
                ReportLog(obEx.Message, false);
            }
            m_bPrintModeEnabled = false;
            DefineGraphColors(zedSpectrumAnalyzer);
        }

        private void menuPageSetup_Click(object sender, EventArgs e)
        {
            zedSpectrumAnalyzer.DoPageSetup();
        }

        private void GetNewFilename()
        {
            //New unique filename to store data based on date and time
            m_sFilenameRFE = "RFExplorer_Client_Data_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".rfe";
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
#if SUPPORT_EXPERIMENTAL
            m_arrRAWSnifferData     = new string[m_nTotalBufferSize];
            m_nRAWSnifferIndex      = 0;
            m_nMaxRAWSnifferIndex   = 0;
            numSampleDecoder.Maximum = m_nTotalBufferSize;
            numSampleDecoder.Minimum = 0;
            numSampleDecoder.Value  = 0;
#endif
                toolStripMemory.Maximum = RFESweepDataCollection.MAX_ELEMENTS;
                toolStripMemory.Step = RFESweepDataCollection.MAX_ELEMENTS / 25;

                numericSampleSA.Minimum = 0;
                numericSampleSA.Value = 0;
                UpdateSweepNumericControls();

                numScreenIndex.Minimum = 0;
                numScreenIndex.Maximum = 0;
                numScreenIndex.Value = 0;

                numericIterations.Maximum = 10000;
                numericIterations.Value = 10;

                m_PenDarkBlue = new Pen(Color.DarkBlue, 1);
                m_PenRed = new Pen(Color.Red, 1);
                m_BrushDarkBlue = new SolidBrush(Color.DarkBlue);

                m_bIsWinXP = (Environment.OSVersion.Version.Major <= 5);

                GetConnectedPorts();
                InitializeSpectrumAnalyzerGraph();

                LoadProperties();
                DefineGraphColors(zedSpectrumAnalyzer);
                SetupSpectrumAnalyzerAxis();

#if SUPPORT_EXPERIMENTAL
                InitializeRAWDecoderGraph();
#endif
                UpdateButtonStatus();

                chkHoldMode.Checked = !chkRunMode.Checked;
                chkHoldDecoder.Checked = !chkRunDecoder.Checked;

                if (m_sFilenameRFE != "")
                {
                    LoadFileRFE(m_sFilenameRFE);
                }
                else
                {
                    if (m_objRFE.ValidCP2101Ports != null && m_objRFE.ValidCP2101Ports.Length == 1)
                    {
                        ConnectPort();
                    }
                }
                m_timer_receive.Enabled = true;
            }
            catch (Exception obEx)
            {
                ReportLog("Error in MainForm_Load: " + obEx.ToString(), true);
            }

            if (m_winAboutModeless != null)
            {
                Thread.Sleep(500);
                m_winAboutModeless.Close();
                m_winAboutModeless.Dispose();
                m_winAboutModeless = null;
            }

            Cursor.Current = Cursors.Default;
        }

        private void GetConnectedPorts()
        {
            Cursor.Current = Cursors.WaitCursor;
            COMPortCombo.DataSource = null;
            if (m_objRFE.GetConnectedPorts())
            {
                COMPortCombo.DataSource = m_objRFE.ValidCP2101Ports;
                COMPortCombo.SelectedItem = RFExplorerClient.Properties.Settings.Default.COMPort;
            }
            UpdateButtonStatus();
            Cursor.Current = Cursors.Default;
        }

        private void UpdateButtonStatus()
        {
            try
            {
                btnConnect.Enabled = !m_objRFE.PortConnected && (COMPortCombo.Items.Count > 0);
                btnDisconnect.Enabled = m_objRFE.PortConnected;
                COMPortCombo.Enabled = !m_objRFE.PortConnected;
                comboBaudRate.Enabled = !m_objRFE.PortConnected;
                btnRescan.Enabled = !m_objRFE.PortConnected;
                chkDumpScreen.Checked = chkDumpScreen.Checked && !menuAutoLCDOff.Checked && m_objRFE.PortConnected;
                chkDumpScreen.Enabled = m_objRFE.PortConnected && !menuAutoLCDOff.Checked;

                btnSendCmd.Enabled = m_objRFE.PortConnected;

                groupFreqSettings.Enabled = m_objRFE.PortConnected;
                groupDemodulator.Enabled = m_objRFE.PortConnected;
                chkHoldMode.Enabled = m_objRFE.PortConnected;
                chkRunMode.Enabled = m_objRFE.PortConnected;
                chkRunDecoder.Enabled = m_objRFE.PortConnected;
                chkHoldDecoder.Enabled = m_objRFE.PortConnected;

                chkCalcRealtime.Checked = m_bDrawRealtime;
                chkCalcAverage.Checked = m_bDrawAverage;
                chkCalcMax.Checked = m_bDrawMax;
                chkCalcMin.Checked = m_bDrawMin;

                btnSaveRemoteBitmap.Enabled = m_objRFE.ScreenData.Count > 1;
                btnSaveRemoteVideo.Enabled = m_objRFE.ScreenData.Count > 1;
                chkDumpColorScreen.Enabled = m_objRFE.ScreenData.Count > 1;
                chkDumpLCDGrid.Enabled = m_objRFE.ScreenData.Count > 1;
                chkDumpHeader.Enabled = m_objRFE.ScreenData.Count > 1;

                panelConfiguration.Enabled = true;
                //calibration is available for all models but 2.4G
                groupCalibration.Enabled = m_objRFE.PortConnected && (m_objRFE.ActiveModel!=RFECommunicator.eModel.MODEL_2400);

                controlRemoteScreen.m_objRFE = m_objRFE;

                for (int nInd = 0; nInd < m_arrAnalyzerButtonList.Length; nInd++)
                {
                    if (m_arrAnalyzerButtonList[nInd] != null)
                    {
                        m_arrAnalyzerButtonList[nInd].Enabled = m_objRFE.PortConnected;
                    }
                }
            }
            catch (Exception obEx)
            {
                ReportLog("ERROR UpdateButtonStatus: " + obEx.ToString(), true);
            }

        }

        private void ConnectPort()
        {
            Cursor.Current = Cursors.WaitCursor;

            if (COMPortCombo.Items.Count > 0)
            {
                m_objRFE.ConnectPort(COMPortCombo.SelectedValue.ToString(), Convert.ToInt32(comboBaudRate.SelectedItem.ToString()));

                UpdateButtonStatus();

                m_objRFE.HoldMode = false;
                UpdateFeedMode();
                SaveProperties();

                menuAutoLCDOff_Click(null, null);
            }

            Cursor.Current = Cursors.Default;
        }

        private void DefineCommonFiles()
        {
            if (m_sAppDataFolder == "")
            {
                //Configuring and loading default folders
                m_sAppDataFolder = Environment.GetEnvironmentVariable("APPDATA") + "\\RFExplorer";
                m_sAppDataFolder = m_sAppDataFolder.Replace("\\\\", "\\");
                if (m_sDefaultDataFolder == "")
                {
                    m_sDefaultDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\RFExplorer";
                    m_sDefaultDataFolder = m_sDefaultDataFolder.Replace("\\\\", "\\");
                    edDefaultFilePath.Text = m_sDefaultDataFolder;
                }

                if (Directory.Exists(m_sAppDataFolder) == false)
                {
                    //Create specific RF Explorer folders if they don't exist, alert with a message box if that fails
                    try
                    {
                        Directory.CreateDirectory(m_sAppDataFolder);
                        Directory.CreateDirectory(m_sDefaultDataFolder);
                    }
                    catch (Exception obEx)
                    {
                        MessageBox.Show(obEx.Message);
                    }
                }
            }
        }

        private void LoadProperties()
        {
            try
            {
                //Load standard WinForm .NET properties
                comboBaudRate.SelectedItem = RFExplorerClient.Properties.Settings.Default.COMSpeed;
                COMPortCombo.SelectedItem = RFExplorerClient.Properties.Settings.Default.COMPort;
                menuSaveOnClose.Checked = RFExplorerClient.Properties.Settings.Default.SaveOnClose;
                numericZoom.Value = RFExplorerClient.Properties.Settings.Default.ScreenZoom;
                m_bShowPeaks = RFExplorerClient.Properties.Settings.Default.ViewPeaks;
                menuShowControlArea.Checked = RFExplorerClient.Properties.Settings.Default.ShowControlArea;
                m_bDark = RFExplorerClient.Properties.Settings.Default.DarkMode;
                menuAutoLCDOff.Checked = RFExplorerClient.Properties.Settings.Default.AutoLCDOff;
                menuContinuousLog.Checked = RFExplorerClient.Properties.Settings.Default.ContinuousLog;
                string sTemp = RFExplorerClient.Properties.Settings.Default.DefaultDataFolder;
                comboCSVFieldSeparator.SelectedIndex = (int)RFExplorerClient.Properties.Settings.Default.CSVDelimiter;
                menuRFConnections.Checked = RFExplorerClient.Properties.Settings.Default.ShowRFConnections;

                DefineCommonFiles();

                m_sSettingsFile = m_sAppDataFolder + "\\RFExplorer_Settings.xml";
                try
                {
                    //Check if old settings file exists, and not the new one, thus move it to reuse it. This "old file" was
                    //created in v1.09.04 and has the problem of some users with limited access rights cannot create it.
                    string sOldSettingsFile = Assembly.GetExecutingAssembly().Location + ".Settings.xml";
                    if (File.Exists(sOldSettingsFile) && !File.Exists(m_sSettingsFile))
                    {
                        //We do not really move it as there may be restricted permissions and want to avoid any problem
                        File.Copy(sOldSettingsFile, m_sSettingsFile);
                        MessageBox.Show("NOTE: Old named-settings file has been migrated to the new location " + m_sSettingsFile);
                        //we try to delete it now, may fail but worth trying. We already copied it, anyway
                        File.Delete(sOldSettingsFile);
                    }
                }
                catch { };
                //open new settings file
                m_DataSettings = new DataSet("RF_Explorer_Settings");
                if (!File.Exists(m_sSettingsFile))
                {
                    CreateSettingsSchema();
                }

                //Load custom name saved properties
                if (m_DataSettings.Tables.Count == 0)
                {
                    //do not load it twice or records will be repeated
                    m_DataSettings.ReadXml(m_sSettingsFile);
                }
                PopulateReadedSettings();
                RestoreSettingsXML("Default");

                if (sTemp != "")
                {
                    m_sDefaultDataFolder = sTemp;
                }
                edDefaultFilePath.Text = m_sDefaultDataFolder;

                comboCustomCommand.DataSource = RFExplorerClient.Properties.Settings.Default.CustomCommands;

                m_bPropertiesReadOk = true;
            }
            catch (Exception obEx)
            {
                MessageBox.Show(obEx.Message);
            }
        }

        private void PopulateReadedSettings()
        {
            menuComboSavedOptions.Items.Clear();
            foreach (DataRow objRow in m_DataSettings.Tables["Common_Settings"].Rows)
            {
                menuComboSavedOptions.Items.Add((string)objRow["Name"]);
            }
            menuComboSavedOptions.SelectedItem = "Default";
        }

        private void SaveProperties()
        {
            if (!m_bPropertiesReadOk)
            {
                ReportLog("Application settings were not persisted due to an earlier error.", false);
                return;
            }

            try
            {
                if (COMPortCombo.Items.Count > 0)
                {
                    RFExplorerClient.Properties.Settings.Default.COMPort = COMPortCombo.SelectedValue.ToString();
                }
                //No need to save it here, it is already saved in send button.
                //RFExplorerClient.Properties.Settings.Default.CustomCommands

                if (comboBaudRate.SelectedItem != null)
                {
                    RFExplorerClient.Properties.Settings.Default.COMSpeed = comboBaudRate.SelectedItem.ToString();
                }
                RFExplorerClient.Properties.Settings.Default.SaveOnClose = menuSaveOnClose.Checked;
                RFExplorerClient.Properties.Settings.Default.ScreenZoom = (int)numericZoom.Value;
                RFExplorerClient.Properties.Settings.Default.ViewPeaks = m_bShowPeaks;
                RFExplorerClient.Properties.Settings.Default.ShowControlArea = menuShowControlArea.Checked;
                RFExplorerClient.Properties.Settings.Default.DarkMode = m_bDark;
                RFExplorerClient.Properties.Settings.Default.AutoLCDOff = menuAutoLCDOff.Checked;
                RFExplorerClient.Properties.Settings.Default.DefaultDataFolder = m_sDefaultDataFolder;
                RFExplorerClient.Properties.Settings.Default.CSVDelimiter = (uint)comboCSVFieldSeparator.SelectedIndex;
                RFExplorerClient.Properties.Settings.Default.ContinuousLog = menuContinuousLog.Checked;
                RFExplorerClient.Properties.Settings.Default.ShowRFConnections = menuRFConnections.Checked;

                RFExplorerClient.Properties.Settings.Default.Save();

                SaveSettingsXML("Default");
            }
            catch (Exception obEx)
            {
                ReportLog("ERROR SaveProperties:" + obEx.Message, true);
            }
        }

        private void DeleteSettingsXML(string sSettingsName)
        {
            try
            {
                DataRow[] objRowCol = m_DataSettings.Tables["Common_Settings"].Select("Name='" + sSettingsName + "'");
                if (objRowCol.Length > 0)
                {
                    m_DataSettings.Tables["Common_Settings"].Rows.Remove(objRowCol[0]);
                    m_DataSettings.WriteXml(m_sSettingsFile, XmlWriteMode.WriteSchema);
                }
            }
            catch (Exception obEx)
            {
                ReportLog("ERROR DeleteSettingsXML:" + obEx.Message, true);
            }
        }

        private void RestoreSettingsXML(string sSettingsName)
        {
            try
            {
                DataRow[] objRowCol = m_DataSettings.Tables["Common_Settings"].Select("Name='" + sSettingsName + "'");
                if (objRowCol.Length > 0)
                {
                    DataRow objRowDefault = objRowCol[0];
                    double fStartFrequencyMHZ = (double)objRowDefault["StartFreq"];
                    double fStepFrequencyMHZ = (double)objRowDefault["StepFreq"];
                    double fAmplitudeTop = (double)objRowDefault["TopAmp"];
                    double fAmplitudeBottom = (double)objRowDefault["BottomAmp"];
                    numericIterations.Value = (UInt16)objRowDefault["Calculator"];
                    m_bDrawAverage = (bool)objRowDefault["ViewAvg"];
                    m_bDrawRealtime = (bool)objRowDefault["ViewRT"];
                    m_bDrawMin = (bool)objRowDefault["ViewMin"];
                    m_bDrawMax = (bool)objRowDefault["ViewMax"];

                    if (m_DataSettings.Tables["Common_Settings"].Columns["ViewMaxHold"] == null)
                    {
                        //Introduced in v1.11.0.1307, may not exist before this date
                        m_DataSettings.Tables["Common_Settings"].Columns.Add(new DataColumn("ViewMaxHold", System.Type.GetType("System.Boolean")));
                    }

                    try
                    {
                        m_bDrawMaxHold = (bool)objRowDefault["ViewMaxHold"];
                    }
                    catch
                    {
                        objRowDefault["ViewMaxHold"]=m_bDrawMaxHold;
                    };

                    if (m_objRFE.PortConnected == false)
                    {
                        //If device is disconnected, we just need to update visible parts of screen as otherwise it won't change
                        m_objRFE.AmplitudeTop = fAmplitudeTop;
                        m_objRFE.AmplitudeBottom = fAmplitudeBottom;

                        //Check to reinitiate buffer here, otherwise after changing it the receive function will not know the data was changed
                        if ((Math.Abs(m_objRFE.StartFrequencyMHZ - fStartFrequencyMHZ) >= 0.001) || (Math.Abs(m_objRFE.StepFrequencyMHZ - fStepFrequencyMHZ) >= 0.001))
                        {
                            m_objRFE.StartFrequencyMHZ = fStartFrequencyMHZ;
                            m_objRFE.StepFrequencyMHZ = fStepFrequencyMHZ;
                            m_objRFE.SweepData.CleanAll();
                            numericSampleSA.Value = 0;
                        }
                        SetupSpectrumAnalyzerAxis(); //will update everything including the edit boxes
                    }
                    else
                    {
                        //if device is connected, we do not need to change anything: just ask the device to reconfigure and the new configuration will come back
                        SendNewConfig(fStartFrequencyMHZ, fStartFrequencyMHZ + fStepFrequencyMHZ * m_objRFE.FreqSpectrumSteps, fAmplitudeTop, fAmplitudeBottom);
                    }
                    m_sLastSettingsLoaded = sSettingsName;
                    UpdateTitleText(); 
                    UpdateButtonStatus();
                }
            }
            catch (Exception obEx)
            {
                ReportLog("ERROR RestoreSettingsXML:" + obEx.ToString(), true);
            }
        }

        private void SaveSettingsXML(string sSettingsName)
        {
            try
            {
                DataRow[] objRowCol = m_DataSettings.Tables["Common_Settings"].Select("Name='" + sSettingsName + "'");
                DataRow objRow = null;

                if (objRowCol.Length == 0)
                {
                    objRow = m_DataSettings.Tables["Common_Settings"].NewRow();
                    objRow["Name"] = sSettingsName;
                    m_DataSettings.Tables["Common_Settings"].Rows.Add(objRow);
                }
                else
                {
                    objRow = objRowCol[0];
                }

                objRow["StartFreq"] = m_objRFE.StartFrequencyMHZ;
                objRow["StepFreq"] = m_objRFE.StepFrequencyMHZ;
                objRow["TopAmp"] = m_objRFE.AmplitudeTop;
                objRow["BottomAmp"] = m_objRFE.AmplitudeBottom;
                objRow["Calculator"] = (int)numericIterations.Value;
                objRow["ViewAvg"] = m_bDrawAverage;
                objRow["ViewRT"] = m_bDrawRealtime;
                objRow["ViewMin"] = m_bDrawMin;
                objRow["ViewMax"] = m_bDrawMax;
                try
                {
                    //Introduced in Jun 2013, may not exist before this date. The column should have been created already in ReadXML function...
                    objRow["ViewMaxHold"] = m_bDrawMaxHold;
                }
                catch {};
                m_DataSettings.WriteXml(m_sSettingsFile, XmlWriteMode.WriteSchema);

                if (!((sSettingsName == "Default") && (m_sLastSettingsLoaded != "Default")))
                {
                    //Only update screen text if
                    m_sLastSettingsLoaded = sSettingsName;
                    UpdateTitleText(); 
                }

                if (sSettingsName == "Default")
                {
                    //If we are saving the default value, that is because we are doing it automatically (e.g. when closing the port)
                    //Therefore select it as the default on screen too
                    menuComboSavedOptions.SelectedItem = "Default";
                }
            }
            catch (Exception obEx)
            {
                ReportLog("ERROR SaveSettingsXML:" + obEx.Message, true);
            }
        }

        private void ClosePort()
        {
            Cursor.Current = Cursors.WaitCursor;
            m_objRFE.ClosePort();
            UpdateButtonStatus();
            UpdateFeedMode();
            DisplayGroups();
            Cursor.Current = Cursors.Default;
        }

        private void DisplayRequiredFirmware()
        {
            if (m_objRFE.RFExplorerFirmwareDetected != m_objRFE.FirmwareCertified)
            {
                UInt16 nMayorVerFound = Convert.ToUInt16(m_objRFE.RFExplorerFirmwareDetected.Substring(0, 2));
                UInt16 nMinorVerFound = Convert.ToUInt16(m_objRFE.RFExplorerFirmwareDetected.Substring(3, 2));
                UInt32 nVersionFound = (UInt32)(nMayorVerFound * 100 + nMinorVerFound);
                UInt16 nMayorVerTested = Convert.ToUInt16(m_objRFE.FirmwareCertified.Substring(0, 2));
                UInt16 nMinorVerTested = Convert.ToUInt16(m_objRFE.FirmwareCertified.Substring(3, 2));
                UInt32 nVersionTested = (UInt32)(nMayorVerTested * 100 + nMinorVerTested);

                if (nVersionFound > nVersionTested)
                {
                    ReportLog("\r\nWARNING: Firmware version connected v" + m_objRFE.RFExplorerFirmwareDetected + " is newer than the one certified v" +
                                m_objRFE.FirmwareCertified + " for this version of RF Explorer for Windows.\r\n" +
                                  "         However, it may be compatible but you should check www.rf-explorer.com website\r\n" +
                                  "         to double check if there is a newer version available.\r\n", false);
                }
                else
                {
                    string sText = "RF Explorer device has an older firmware version " + m_objRFE.RFExplorerFirmwareDetected +
                        "\r\nPlease upgrade it to required version " + m_objRFE.FirmwareCertified +
                        "\r\nVisit www.rf-explorer/download to get required firmware.";
                    if (!m_bVersionAlerted)
                    {
                        m_bVersionAlerted = true;
                        MessageBox.Show(sText, "Firmware Warning");
                    }
                    ReportLog(sText, false);
                }
            }
        }

        private void timer_receive_Tick(object sender, EventArgs e)
        {
            try
            {
                bool bDraw = false;
                string sOut;

                if (m_objRFE.PortConnected)
                {
                    bDraw = m_objRFE.ProcessReceivedString(true, out sOut);
                }

                if (bDraw)
                {
                    if (groupCOM.Parent == tabSpectrumAnalyzer)
                    {
                        DisplaySpectrumAnalyzerData();
                    }
                    else if (groupCOM.Parent == tabWaterfall)
                    {
                        UpdateWaterfall();
                    }
                }
            }
            catch (Exception obEx)
            {
                ReportLog("timer_receive_Tick: " + obEx.Message, true);
            }

            if (m_bFirstTick)
            {
                m_bFirstTick = false;
                ReportLog("", true);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if ((menuSaveOnClose.Checked || menuContinuousLog.Checked) && (m_sFilenameRFE.Length == 0))
            {
                GetNewFilename();
                SaveFile(m_sFilenameRFE);
            }
            SaveProperties();
            m_objRFE.Close();
            Cursor.Current = Cursors.Default;
        }

        private void SetupSpectrumAnalyzerAxis()
        {
            double fStart = m_objRFE.StartFrequencyMHZ;
            double fEnd = CalculateEndFrequencyMHZ() - m_objRFE.StepFrequencyMHZ;
            double fMajorStep = 1.0;

            zedSpectrumAnalyzer.GraphPane.XAxis.Scale.Min = fStart;
            zedSpectrumAnalyzer.GraphPane.XAxis.Scale.Max = fEnd;

            if (m_objRFE.Mode == RFECommunicator.eMode.MODE_WIFI_ANALYZER)
            {
                //objGraph is a bar chart
                zedSpectrumAnalyzer.GraphPane.XAxis.Scale.Min = fStart - 2.5f;
                zedSpectrumAnalyzer.GraphPane.XAxis.Scale.Max = fEnd + 2.5f;
                //objGraph.GraphPane.XAxis.Type = AxisType.Ordinal;
            }

            if ((fEnd - fStart) < 1.0)
            {
                fMajorStep = 0.1;
            }
            else if ((fEnd - fStart) < 10)
            {
                fMajorStep = 1.0;
            }
            else if ((fEnd - fStart) < 100)
            {
                fMajorStep = 10;
            }
            else if ((fEnd - fStart) < 500)
            {
                fMajorStep = 50;
            }
            else if ((fEnd - fStart) < 1000)
            {
                fMajorStep = 100;
            }

            zedSpectrumAnalyzer.GraphPane.XAxis.Scale.MajorStep = fMajorStep;
            zedSpectrumAnalyzer.GraphPane.XAxis.Scale.MinorStep = fMajorStep / 10.0;

            UpdateYAxis();

            UpdateDialogFromFreqSettings();
        }

        private void DisplaySpectrumAnalyzerData()
        {
            m_objRFE.PeakValueMHZ = 0.0;
            m_objRFE.PeakValueAmp = RFECommunicator.MIN_AMPLITUDE_DBM;

            foreach (CurveItem objCurve in zedSpectrumAnalyzer.GraphPane.CurveList)
            {
                objCurve.Clear();
                objCurve.IsVisible = false;
                objCurve.Label.IsVisible = false;
            }
            m_PointListRealtime.Clear();
            m_PointListAverage.Clear();
            m_PointListMin.Clear();
            m_PointListMax.Clear();
            m_PointListMaxHold.Clear();
            m_MaxBar.Clear();
            PointPairList PointNoiseFloor = new PointPairList();

            if (m_objRFE.SweepData.Count == 0)
                return; //nothing to paint

            UInt32 nSweepIndex = (UInt32)numericSampleSA.Value;
            m_nDrawingIteration++;

            UInt32 nTotalCalculatorIterations = (UInt32)numericIterations.Value;
            if (nTotalCalculatorIterations > nSweepIndex)
                nTotalCalculatorIterations = nSweepIndex;

            if ((m_nDrawingIteration & 0xf) == 0)
            {
                //Update screen status every 16 drawing iterations only to reduce overhead
                toolStripMemory.Value = (int)nSweepIndex;
                if (m_objRFE.PortConnected)
                    toolCOMStatus.Text = "Connected";
                else
                    toolCOMStatus.Text = "Disconnected";

                toolStripSamples.Text = "Total Samples in buffer: " + (UInt32)numericSampleSA.Value + "/" + RFESweepDataCollection.MAX_ELEMENTS + " - " + (100 * (double)numericSampleSA.Value / RFESweepDataCollection.MAX_ELEMENTS).ToString("0.0") + "%";
            }

            double fRealtimeMax_Amp = RFECommunicator.MIN_AMPLITUDE_DBM;
            int fRealtimeMax_Iter = 0;
            double fAverageMax_Amp = RFECommunicator.MIN_AMPLITUDE_DBM;
            int fAverageMax_Iter = 0;
            double fMaxMax_Amp = RFECommunicator.MIN_AMPLITUDE_DBM;
            int fMaxMax_Iter = 0;

            m_AveragePeak.Text = "";
            m_RealtimePeak.Text = "";
            m_MaxPeak.Text = "";

            //Use the current data sweep item pointed out by the selected index
            RFESweepData objSweep = m_objRFE.SweepData.GetData(nSweepIndex);

            if (!m_objRFE.PortConnected || m_objRFE.HoldMode)
            {
                m_RFEConfig.Text = m_objRFE.ModelText + '\n' + m_objRFE.ConfigurationText + '\n';
                //If not connected, then rebuild the configuration text to properly show the capture date (if available)
                if (objSweep.CaptureTime.Year > 2000)
                {
                    m_RFEConfig.Text += "Captured:" + objSweep.CaptureTime.ToString("yyyy-MM-dd HH:mm:ss\\.fff");
                }
                else
                {
                    m_RFEConfig.Text += "Unknown Capture Date";
                }
            }

            for (UInt16 nSweepPointInd = 0; objSweep!=null && nSweepPointInd < m_objRFE.FreqSpectrumSteps; nSweepPointInd++)
            {
                if (nSweepPointInd < m_arrWiFiBarText.Length)
                    m_arrWiFiBarText[nSweepPointInd].Text = "";

                double fVal = objSweep.GetAmplitudeDBM(nSweepPointInd);
                if (fVal > fRealtimeMax_Amp)
                {
                    fRealtimeMax_Amp = fVal;
                    fRealtimeMax_Iter = nSweepPointInd;
                }

                double fFreq = m_objRFE.StartFrequencyMHZ + m_objRFE.StepFrequencyMHZ * nSweepPointInd;

                //PointNoiseFloor.Add(fFreq, RFECommunicator.MIN_AMPLITUDE_DBM, RFECommunicator.MIN_AMPLITUDE_DBM);

                double fMax = fVal;
                double fMin = fVal;
                double fValAvg = fVal;

                for (UInt32 nSweepIterator = nSweepIndex - nTotalCalculatorIterations; nSweepIterator < nSweepIndex; nSweepIterator++)
                {
                    //Calculate average, max and min over Calculator range
                    RFESweepData objSweepIter = m_objRFE.SweepData.GetData(nSweepIterator);
                    if (objSweepIter != null)
                    {
                        double fVal2 = objSweepIter.GetAmplitudeDBM(nSweepPointInd);

                        fMax = Math.Max(fMax, fVal2);
                        fMin = Math.Min(fMin, fVal2);
                        fValAvg += fVal2;
                    }
                }

                if (m_bDrawRealtime)
                    m_PointListRealtime.Add(fFreq, fVal, RFECommunicator.MIN_AMPLITUDE_DBM);
                if (m_bDrawMin)
                    m_PointListMin.Add(fFreq, fMin, RFECommunicator.MIN_AMPLITUDE_DBM);

                if (m_bDrawMax)
                {
                    m_PointListMax.Add(fFreq, fMax, RFECommunicator.MIN_AMPLITUDE_DBM);
                    if (fMax > fMaxMax_Amp)
                    {
                        fMaxMax_Amp = fMax;
                        fMaxMax_Iter = nSweepPointInd;
                    }
                    if (m_objRFE.Mode == RFECommunicator.eMode.MODE_WIFI_ANALYZER && m_bShowPeaks)
                    {
                        if (nSweepPointInd < m_arrWiFiBarText.Length)
                        {
                            double fFreqMark = (m_objRFE.StartFrequencyMHZ + m_objRFE.StepFrequencyMHZ * nSweepPointInd);
                            m_arrWiFiBarText[nSweepPointInd].Text = "CH" + (nSweepPointInd + 1).ToString() + "\n" + fFreqMark.ToString("0") + "MHZ\n" + fMax.ToString() + "dBm";
                            m_arrWiFiBarText[nSweepPointInd].Location.X = fFreqMark;
                            m_arrWiFiBarText[nSweepPointInd].Location.Y = fMax;
                            m_arrWiFiBarText[nSweepPointInd].FontSpec.IsBold = false;
                            m_arrWiFiBarText[nSweepPointInd].FontSpec.FontColor = Color.Red;
                        }
                    }
                }

                if (m_bDrawMaxHold && (m_objRFE.SweepData.MaxHoldData!=null))
                {
                    m_PointListMaxHold.Add(fFreq, m_objRFE.SweepData.MaxHoldData.GetAmplitudeDBM(nSweepPointInd), RFECommunicator.MIN_AMPLITUDE_DBM);
                }

                if (m_bDrawAverage || m_bCalibrating)
                {
                    fValAvg = fValAvg / (nTotalCalculatorIterations + 1);
                    m_PointListAverage.Add(fFreq, fValAvg, RFECommunicator.MIN_AMPLITUDE_DBM);
                    if (fValAvg > fAverageMax_Amp)
                    {
                        fAverageMax_Amp = fValAvg;
                        fAverageMax_Iter = nSweepPointInd;
                    }
                }
            }

            //Get the m_objRFE.PeakValueMHZ/m_objRFE.PeakValueAmp based on what is available on calculation
            if (fAverageMax_Amp > RFECommunicator.MIN_AMPLITUDE_DBM)
            {
                m_objRFE.PeakValueMHZ = (m_objRFE.StartFrequencyMHZ + m_objRFE.StepFrequencyMHZ * fAverageMax_Iter);
                m_objRFE.PeakValueAmp = fAverageMax_Amp;
            }
            else if (fMaxMax_Amp > RFECommunicator.MIN_AMPLITUDE_DBM)
            {
                m_objRFE.PeakValueMHZ = (m_objRFE.StartFrequencyMHZ + m_objRFE.StepFrequencyMHZ * fMaxMax_Iter);
                m_objRFE.PeakValueAmp = fMaxMax_Amp;
            }
            else if (fRealtimeMax_Amp > RFECommunicator.MIN_AMPLITUDE_DBM)
            {
                m_objRFE.PeakValueMHZ = (m_objRFE.StartFrequencyMHZ + m_objRFE.StepFrequencyMHZ * fRealtimeMax_Iter);
                m_objRFE.PeakValueAmp = fRealtimeMax_Amp;
            }

            if (!m_bCalibrating)
            {
                //zedSpectrumAnalyzer.GraphPane.CurveList.Clear();
                if (m_bDrawAverage)
                {
                    m_AvgLine.Points = m_PointListAverage;
                    m_AvgLine.IsVisible = true;
                    m_AvgLine.Label.IsVisible = true;
                    double fFreqMark = (m_objRFE.StartFrequencyMHZ + m_objRFE.StepFrequencyMHZ * fAverageMax_Iter);
                    if (m_bShowPeaks)
                    {
                        m_AveragePeak.Text = fFreqMark.ToString("0.000") + "MHZ\n" + fAverageMax_Amp.ToString("0.00") + "dBm";
                        m_AveragePeak.Location.X = fFreqMark;
                        m_AveragePeak.Location.Y = fAverageMax_Amp;
                    }
                }
                if (m_bDrawRealtime)
                {
                    m_RealtimeLine.Points = m_PointListRealtime;
                    m_RealtimeLine.IsVisible = true;
                    m_RealtimeLine.Label.IsVisible = true;
                    double fFreqMark = (m_objRFE.StartFrequencyMHZ + m_objRFE.StepFrequencyMHZ * fRealtimeMax_Iter);
                    if (m_bShowPeaks)
                    {
                        m_RealtimePeak.Text = fFreqMark.ToString("0.000") + "MHZ\n" + fRealtimeMax_Amp.ToString("0.0") + "dBm";
                        m_RealtimePeak.Location.X = fFreqMark;
                        m_RealtimePeak.Location.Y = fRealtimeMax_Amp;
                    }
                }
                if (m_bDrawMax)
                {
                    if (m_objRFE.Mode == RFECommunicator.eMode.MODE_SPECTRUM_ANALYZER)
                    {
                        m_MaxLine.Points = m_PointListMax;
                        m_MaxLine.IsVisible = true;
                        m_MaxLine.Label.IsVisible = true;
                        double fFreqMark = (m_objRFE.StartFrequencyMHZ + m_objRFE.StepFrequencyMHZ * fMaxMax_Iter);
                        if (m_bShowPeaks)
                        {
                            m_MaxPeak.Text = fFreqMark.ToString("0.000") + "MHZ\n" + fMaxMax_Amp.ToString("0.0") + "dBm";
                            m_MaxPeak.Location.X = fFreqMark;
                            m_MaxPeak.Location.Y = fMaxMax_Amp;
                        }
                    }
                    else if (m_objRFE.Mode == RFECommunicator.eMode.MODE_WIFI_ANALYZER)
                    {
                        m_MaxBar.Points = m_PointListMax;
                        m_MaxBar.IsVisible = true;
                        if (m_bShowPeaks)
                        {
                            m_arrWiFiBarText[fMaxMax_Iter].FontSpec.IsBold = true;
                            if (m_bDark)
                                m_arrWiFiBarText[fMaxMax_Iter].FontSpec.FontColor = Color.LightCoral;
                            else
                                m_arrWiFiBarText[fMaxMax_Iter].FontSpec.FontColor = Color.DarkRed;
                        }
                    }
                }
                if (m_bDrawMin)
                {
                    m_MinLine.Points = m_PointListMin;
                    m_MinLine.IsVisible = true;
                    m_MinLine.Label.IsVisible = true;
                }
                if (m_bDrawMaxHold)
                {
                    m_MaxHoldLine.Points = m_PointListMaxHold;
                    m_MaxHoldLine.IsVisible = true;
                    m_MaxHoldLine.Label.IsVisible = true;
                }

                if (!m_bPrintModeEnabled)
                    zedSpectrumAnalyzer.Refresh();
            }
        }

        private string SpectrumAnalyzerPointValueHandler(ZedGraphControl control, GraphPane pane,
                    CurveItem curve, int iPt)
        {
            // Get the PointPair that is under the mouse
            PointPair pt = curve[iPt];

            return pt.X.ToString("f3") + "MHZ\r\n" + pt.Y.ToString("f2") + " dBm";
        }

        private void zedSpectrumAnalyzer_ZoomEvent(ZedGraphControl sender, ZoomState oldState, ZoomState newState)
        {

        }

        private void SendCommand(string sData)
        {
            m_objRFE.SendCommand(sData);
        }

        private void SendNewConfig(double fStartMHZ, double fEndMHZ, double fTopDBM, double fBottomDBM)
        {
            //#[32]C2-F:Sssssss,Eeeeeee,tttt,bbbb
            UInt32 nStartKhz = (UInt32)(fStartMHZ * 1000);
            UInt32 nEndKhz = (UInt32)(fEndMHZ * 1000);
            Int16 nTopDBM = (Int16)fTopDBM;
            Int16 nBottomDBM = (Int16)fBottomDBM;

            string sData = "C2-F:" +
                nStartKhz.ToString("D7") + "," + nEndKhz.ToString("D7") + "," +
                nTopDBM.ToString("D3") + "," + nBottomDBM.ToString("D3");
            SendCommand(sData);

            ResetSettingsTitle();

            Thread.Sleep(500); //wait some time for the unit to process changes, otherwise may get a different command too soon
        }

        private void ResetSettingsTitle()
        {
            if (m_sLastSettingsLoaded != "Default")
            {
                m_sLastSettingsLoaded = "Default"; //if we change the current configuration by hand, then it becomes "Default" again
                UpdateTitleText();
            }
        }

        private void UpdateRemoteConfigData()
        {
            try
            {
                if (m_objRFE.PortConnected)
                {
                    SendNewConfig(Convert.ToDouble(m_sStartFreq.Text), Convert.ToDouble(m_sEndFreq.Text),
                        Convert.ToDouble(m_sTopDBM.Text), Convert.ToDouble(m_sBottomDBM.Text));
                }
            }
            catch(Exception obEx)
            {
                ReportLog(obEx.ToString(), false);
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            ConnectPort();
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            ClosePort();
        }

        private void chkRunMode_CheckedChanged(object sender, EventArgs e)
        {
            m_objRFE.HoldMode = !chkRunMode.Checked;
            if (!m_objRFE.HoldMode && (m_objRFE.SweepData.IsFull()))
            {
                m_objRFE.SweepData.CleanAll();
                ReportLog("Buffer cleared.", false);
            }
            UpdateFeedMode();
        }

        private void chkHoldMode_CheckedChanged(object sender, EventArgs e)
        {
            m_objRFE.HoldMode = chkHoldMode.Checked;
            if (m_objRFE.HoldMode)
            {
                //Send hold mode to RF Explorer to stop RS232 traffic
                m_objRFE.SendCommand_Hold();
            }
            else
            {
                //Not on hold anymore, restore RS232 traffic
                m_objRFE.SendCommand_RequestConfigData();
                Thread.Sleep(50);
            }
            UpdateFeedMode();
        }

        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (m_objRFE.HoldMode || (!m_objRFE.PortConnected && m_objRFE.SweepData.Count>0))
            {
                if (numericSampleSA.Value > m_objRFE.SweepData.Count)
                {
                    numericSampleSA.Value = m_objRFE.SweepData.Count;
                }
                DisplaySpectrumAnalyzerData();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (About_RFExplorer myAbout = new About_RFExplorer())
            {
                myAbout.ShowDialog();
            }
        }

        private void MainMenuView_DropDownOpening(object sender, EventArgs e)
        {
            menuRealtimeData.Checked = m_bDrawRealtime;
            menuAveragedData.Checked = m_bDrawAverage;
            menuMaxData.Checked = m_bDrawMax;
            menuMinData.Checked = m_bDrawMin;
            menuMaxHoldData.Checked = m_bDrawMaxHold;
            menuShowPeak.Checked = m_bShowPeaks;
            menuDarkMode.Checked = m_bDark;
        }

        private void click_view_mode(object sender, EventArgs e)
        {
            m_bDrawRealtime = menuRealtimeData.Checked;
            m_bDrawAverage = menuAveragedData.Checked;
            m_bDrawMax = menuMaxData.Checked;
            m_bDrawMaxHold = menuMaxHoldData.Checked;
            m_bDrawMin = menuMinData.Checked;
            m_bShowPeaks = menuShowPeak.Checked;
            UpdateButtonStatus();
            if (m_objRFE.HoldMode)
                DisplaySpectrumAnalyzerData();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog MySaveFileDialog = new SaveFileDialog())
                {
                    MySaveFileDialog.Filter = "RFExplorer files (*.rfe)|*.rfe|All files (*.*)|*.*";
                    MySaveFileDialog.FilterIndex = 1;
                    MySaveFileDialog.RestoreDirectory = false;
                    MySaveFileDialog.InitialDirectory = m_sDefaultDataFolder;

                    GetNewFilename();
                    MySaveFileDialog.FileName = m_sFilenameRFE;

                    if (MySaveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        SaveFile(MySaveFileDialog.FileName);
                    }
                }
            }
            catch (Exception obEx) { MessageBox.Show(obEx.Message); }
        }

        private void SaveCSVtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog MySaveFileDialog = new SaveFileDialog())
                {
                    MySaveFileDialog.Filter = "RFExplorer CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                    MySaveFileDialog.FilterIndex = 1;
                    MySaveFileDialog.RestoreDirectory = false;
                    MySaveFileDialog.InitialDirectory = m_sDefaultDataFolder;

                    GetNewFilename();
                    MySaveFileDialog.FileName = m_sFilenameRFE.Replace(".rfe", ".csv");

                    if (MySaveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        SaveFileCSV(MySaveFileDialog.FileName);
                    }
                }
            }
            catch (Exception obEx) { MessageBox.Show(obEx.Message); }
        }

        private void menusSaveSimpleCSV_Click(object sender, EventArgs e)
        {
            try
            {
                PointPairList listCurrentPointList = null;

                int nSelectionCounter = 0;
                if (m_bDrawAverage)
                {
                    nSelectionCounter++;
                    listCurrentPointList = m_PointListAverage;
                }
                if (m_bDrawMax)
                {
                    nSelectionCounter++;
                    listCurrentPointList = m_PointListMaxHold;
                }
                if (m_bDrawMax)
                {
                    listCurrentPointList = m_PointListMax;
                    nSelectionCounter++;
                }
                if (m_bDrawMin)
                {
                    nSelectionCounter++;
                    listCurrentPointList = m_PointListMin;
                }
                if (m_bDrawRealtime)
                {
                    listCurrentPointList = m_PointListRealtime;
                    nSelectionCounter++;
                }

                if (nSelectionCounter == 0)
                {
                    MessageBox.Show("Single Signal CSV export needs one active display curve on screen (Avg, Max, Min or Realtime)", "Single Curve CSV Export");
                    return;
                }
                else if (nSelectionCounter > 1)
                {
                    MessageBox.Show("Single Signal CSV export requires one active display curve only on screen (Avg, Max, Min or Realtime)", "Single Curve CSV Export");
                    return;
                }

                using (SaveFileDialog MySaveFileDialog = new SaveFileDialog())
                {
                    MySaveFileDialog.Filter = "RFExplorer CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                    MySaveFileDialog.FilterIndex = 1;
                    MySaveFileDialog.RestoreDirectory = false;
                    MySaveFileDialog.InitialDirectory = m_sDefaultDataFolder;

                    GetNewFilename();
                    MySaveFileDialog.FileName = m_sFilenameRFE.Replace(".rfe", ".csv");

                    if (MySaveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        SaveSimpleCSV(MySaveFileDialog.FileName, listCurrentPointList);
                    }
                }
            }
            catch (Exception obEx) { MessageBox.Show(obEx.Message); }
        }

        private char GetCSVDelimiter()
        {
            char cReturn = ',';

            /*
                Comma (,)
                Division (|)
                Semicolon (;)
                Space ( )
                Tabulator (\t)
             */

            switch (comboCSVFieldSeparator.SelectedIndex)
            {
                default:
                case 0: cReturn = ','; break;
                case 1: cReturn = '|'; break;
                case 2: cReturn = ';'; break;
                case 3: cReturn = ' '; break;
                case 4: cReturn = '\t'; break;
            }

            return cReturn;
        }

        private void SaveSimpleCSV(string sFilename, PointPairList listCurrentPointList)
        {
            try
            {

                //if no file path was explicited, add the default folder
                if (sFilename.IndexOf("\\") < 0)
                {
                    sFilename = m_sDefaultDataFolder + "\\" + sFilename;
                    sFilename = sFilename.Replace("\\\\", "\\");
                }

                char cCSV = GetCSVDelimiter();

                using (StreamWriter myFile = new StreamWriter(sFilename, true))
                {
                    foreach (PointPair objPointPair in listCurrentPointList)
                    {
                        myFile.WriteLine(objPointPair.X.ToString("0.000") + cCSV + objPointPair.Y.ToString("0.00"));
                    }
                }
            }
            catch (Exception obEx) { MessageBox.Show(obEx.Message); }
        }

        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            menuSaveAsRFE.Enabled = (m_objRFE.SweepData.Count > 0) && (MainTab.SelectedTab == tabSpectrumAnalyzer);
            menuSaveCSV.Enabled = (m_objRFE.SweepData.Count > 0) && (MainTab.SelectedTab == tabSpectrumAnalyzer);
            menusSaveSimpleCSV.Enabled = menuSaveCSV.Enabled;
            menuLoadRFE.Enabled = MainTab.SelectedTab == tabSpectrumAnalyzer;

            menuSaveRemoteImage.Enabled = (m_objRFE.ScreenData.Count > 0) && (MainTab.SelectedTab == tabRemoteScreen);
            menuLoadRFS.Enabled = MainTab.SelectedTab == tabRemoteScreen;
            menuSaveRFS.Enabled = (m_objRFE.ScreenData.Count > 0) && (MainTab.SelectedTab == tabRemoteScreen);

            menuPrint.Enabled = MainTab.SelectedTab == tabSpectrumAnalyzer;
            menuPrintPreview.Enabled = MainTab.SelectedTab == tabSpectrumAnalyzer;
            menuPageSetup.Enabled = MainTab.SelectedTab == tabSpectrumAnalyzer;
        }

        private void UpdateTitleText()
        {
            if (m_objRFE.HoldMode)
            {
                if (m_sFilenameRFE.Length > 0)
                {
                    zedSpectrumAnalyzer.GraphPane.Title.Text = "RF Explorer File data";
                }
                else
                {
                    zedSpectrumAnalyzer.GraphPane.Title.Text = "RF Explorer ON HOLD";
                }
                zedRAWDecoder.GraphPane.Title.Text = "RF Explorer Decoder ON HOLD";
            }
            else
            {
                if (m_objRFE.PortConnected)
                {
                    zedSpectrumAnalyzer.GraphPane.Title.Text = "RF Explorer Live data";
                    zedRAWDecoder.GraphPane.Title.Text = "RF Explorer Decoder Live Data";
                }
                else
                {
                    zedSpectrumAnalyzer.GraphPane.Title.Text = "RF Explorer Disconnected";
                    zedRAWDecoder.GraphPane.Title.Text = "RF Explorer Disconnected";
                }
            }

            zedSpectrumAnalyzer.GraphPane.Title.Text += " - " + m_sLastSettingsLoaded;
        }

        private void UpdateFeedMode()
        {
            if (!m_objRFE.PortConnected)
            {
                m_objRFE.HoldMode = true;
            }

            chkRunMode.Checked = !m_objRFE.HoldMode;
            chkRunDecoder.Checked = !m_objRFE.HoldMode;
            chkHoldMode.Checked = m_objRFE.HoldMode;
            chkHoldDecoder.Checked = m_objRFE.HoldMode;
            if (m_objRFE.HoldMode == false)
            {
                toolFile.Text = " - File: none";
                m_sFilenameRFE = "";
            }

            UpdateTitleText();

            zedSpectrumAnalyzer.Refresh();
            zedRAWDecoder.Refresh();
        }

        private void SaveFile(string sFilename)
        {
            try
            {
                //if no file path was explicited, add the default folder
                if (sFilename.IndexOf("\\") < 0)
                {
                    sFilename = m_sDefaultDataFolder + "\\" + sFilename;
                    sFilename = sFilename.Replace("\\\\", "\\");
                }
                m_objRFE.SaveFileRFE(sFilename);
            }
            catch (Exception obEx) { MessageBox.Show(obEx.Message); }
        }

        private void SaveFileCSV(string sFilename)
        {
            try
            {
                //if no file path was explicited, add the default folder
                if (sFilename.IndexOf("\\") < 0)
                {
                    sFilename = m_sDefaultDataFolder + "\\" + sFilename;
                    sFilename = sFilename.Replace("\\\\", "\\");
                }
                m_objRFE.SweepData.SaveFileCSV(sFilename,GetCSVDelimiter());
            }
            catch (Exception obEx) { MessageBox.Show(obEx.Message); }
        }

        private void UpdateSweepNumericControls()
        {
            //Update sweep data
            if (m_objRFE.SweepData.Count < numericSampleSA.Value)
            {
                numericSampleSA.Value = m_objRFE.SweepData.Count-1;
            }
            //we can now safely change the max and the value (if not did already)
            numericSampleSA.Maximum = m_objRFE.SweepData.Count-1;
            numericSampleSA.Value = m_objRFE.SweepData.Count-1;
        }

        private void LoadFileRFE(string sFile)
        {
            if (m_objRFE.PortConnected)
            {
                ClosePort();
            }
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                m_objRFE.SweepData.CleanAll();

                if (m_objRFE.LoadFileRFE(sFile))
                {
                    UpdateSweepNumericControls();

                    ReportLog("File " + sFile + " loaded with total of " + m_objRFE.SweepData.Count + " sweeps.", false);

                    toolFile.Text = " - File: " + sFile;
                    m_sFilenameRFE = sFile;

                    UpdateFeedMode();

                    SetupSpectrumAnalyzerAxis();
                    DisplaySpectrumAnalyzerData();
                }
                else
                {
                    MessageBox.Show("Wrong or unknown file format");
                }
            }
            catch (Exception obEx) { MessageBox.Show(obEx.Message); }
            Cursor.Current = Cursors.Default;
        }

        private void toolStripMenuItemLoad_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog MyOpenFileDialog = new OpenFileDialog())
                {
                    MyOpenFileDialog.Filter = "RFExplorer files (*.rfe)|*.rfe|All files (*.*)|*.*";
                    MyOpenFileDialog.FilterIndex = 1;
                    MyOpenFileDialog.RestoreDirectory = false;
                    MyOpenFileDialog.InitialDirectory = m_sDefaultDataFolder;

                    if (MyOpenFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        LoadFileRFE(MyOpenFileDialog.FileName);
                    }
                }
            }
            catch (Exception obEx) { MessageBox.Show(obEx.Message); }
        }

        private void btnRescan_Click(object sender, EventArgs e)
        {
            GetConnectedPorts();
            UpdateButtonStatus();
        }

        private void objGraph_ContextMenuBuilder(ZedGraphControl sender, ContextMenuStrip menuStrip, Point mousePt, ZedGraphControl.ContextMenuObjectState objState)
        {
             menuStrip.Items[3].Enabled = !m_bDark; //disable print in dark mode configuration as it cannot change background
        }

        protected void UpdateYAxis()
        {
            //check and fix absolute margins
            if (m_objRFE.AmplitudeBottom < RFECommunicator.MIN_AMPLITUDE_DBM)
            {
                m_objRFE.AmplitudeBottom = RFECommunicator.MIN_AMPLITUDE_DBM;
            }
            if (m_objRFE.AmplitudeTop > RFECommunicator.MAX_AMPLITUDE_DBM)
            {
                m_objRFE.AmplitudeTop = RFECommunicator.MAX_AMPLITUDE_DBM;
            }

            //Check and fix relative margins
            if (m_objRFE.AmplitudeTop < (RFECommunicator.MIN_AMPLITUDE_DBM + RFECommunicator.MIN_AMPLITUDE_RANGE_DBM))
            {
                m_objRFE.AmplitudeTop = RFECommunicator.MIN_AMPLITUDE_DBM + RFECommunicator.MIN_AMPLITUDE_RANGE_DBM;
            }
            if (m_objRFE.AmplitudeBottom >= m_objRFE.AmplitudeTop)
            {
                m_objRFE.AmplitudeBottom = m_objRFE.AmplitudeTop - RFECommunicator.MIN_AMPLITUDE_RANGE_DBM;
            }

            zedSpectrumAnalyzer.GraphPane.YAxis.Scale.Min = m_objRFE.AmplitudeBottomNormalized;
            zedSpectrumAnalyzer.GraphPane.YAxis.Scale.Max = m_objRFE.AmplitudeTopNormalized;

            zedSpectrumAnalyzer.GraphPane.YAxis.Scale.MajorStep = 10.0;
            if ((m_objRFE.AmplitudeTop - m_objRFE.AmplitudeBottom) > 30)
            {
                zedSpectrumAnalyzer.GraphPane.YAxis.Scale.MinorStep = 5.0;
            }
            else
            {
                zedSpectrumAnalyzer.GraphPane.YAxis.Scale.MinorStep = 1.0;
            }

            zedSpectrumAnalyzer.Refresh();
        }

        private void toolStripMenuPortInfo_Click(object sender, EventArgs e)
        {
            m_objRFE.ListAllCOMPorts();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            UpdateDialogFromFreqSettings();
        }

        private void UpdateDialogFromFreqSettings()
        {
            m_sBottomDBM.Text = m_objRFE.AmplitudeBottom.ToString();
            m_sTopDBM.Text = m_objRFE.AmplitudeTop.ToString();
            m_sStartFreq.Text = m_objRFE.StartFrequencyMHZ.ToString("f3");
            m_sRefFrequency.Text = m_objRFE.RefFrequencyMHZ.ToString("f3");
            m_sEndFreq.Text = CalculateEndFrequencyMHZ().ToString("f3");
            m_sCenterFreq.Text = CalculateCenterFrequencyMHZ().ToString("f3");
            m_sFreqSpan.Text = CalculateFrequencySpanMHZ().ToString("f3");
        }

        private bool IsDifferent(double d1, double d2, double dEpsilon = 0.001)
        {
            return (Math.Abs(d1 - d2) > dEpsilon);
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            UpdateYAxis();
            UpdateRemoteConfigData();
        }

        private void btnMoveFreqDecLarge_Click(object sender, EventArgs e)
        {
            double fStartFreq = Convert.ToDouble(m_sStartFreq.Text);
            fStartFreq -= CalculateFrequencySpanMHZ() * 0.5;
            double fEndFreq = Convert.ToDouble(m_sEndFreq.Text);
            fEndFreq -= CalculateFrequencySpanMHZ() * 0.5;
            m_sStartFreq.Text = fStartFreq.ToString("f3");
            m_sEndFreq.Text = fEndFreq.ToString("f3");
            m_sStartFreq_Leave(null, null);

            UpdateRemoteConfigData();
        }

        private void btnMoveFreqIncLarge_Click(object sender, EventArgs e)
        {
            double fStartFreq = Convert.ToDouble(m_sStartFreq.Text);
            fStartFreq += CalculateFrequencySpanMHZ() * 0.5;
            double fEndFreq = Convert.ToDouble(m_sEndFreq.Text);
            fEndFreq += CalculateFrequencySpanMHZ() * 0.5;
            m_sStartFreq.Text = fStartFreq.ToString("f3");
            m_sEndFreq.Text = fEndFreq.ToString("f3");
            m_sEndFreq_Leave(null, null);

            UpdateRemoteConfigData();
        }

        private void btnMoveFreqDecSmall_Click(object sender, EventArgs e)
        {
            m_objRFE.StartFrequencyMHZ -= CalculateFrequencySpanMHZ() / 10;
            if (m_objRFE.StartFrequencyMHZ < m_objRFE.MinFreqMHZ)
            {
                m_objRFE.StartFrequencyMHZ = m_objRFE.MinFreqMHZ;
            }

            SetupSpectrumAnalyzerAxis();
            SaveProperties();
            UpdateRemoteConfigData();
        }

        private void btnMoveFreqIncSmall_Click(object sender, EventArgs e)
        {
            m_objRFE.StartFrequencyMHZ += CalculateFrequencySpanMHZ() / 10;
            if (CalculateEndFrequencyMHZ() > m_objRFE.MaxFreqMHZ)
            {
                m_objRFE.StartFrequencyMHZ = m_objRFE.MaxFreqMHZ - CalculateFrequencySpanMHZ();
            }

            SetupSpectrumAnalyzerAxis();
            SaveProperties();
            UpdateRemoteConfigData();
        }

        private void btnSpanDec_Click(object sender, EventArgs e)
        {
            double fFreqSpan = Convert.ToDouble(m_sFreqSpan.Text);
            fFreqSpan -= fFreqSpan * 0.25;
            m_sFreqSpan.Text = fFreqSpan.ToString("f3");
            m_sFreqSpan_Leave(null, null);
            UpdateRemoteConfigData();
        }

        private void btnSpanInc_Click(object sender, EventArgs e)
        {
            double fFreqSpan = Convert.ToDouble(m_sFreqSpan.Text);
            fFreqSpan += fFreqSpan * 0.25;
            m_sFreqSpan.Text = fFreqSpan.ToString("f3");
            m_sFreqSpan_Leave(null, null);
            UpdateRemoteConfigData();
        }

        private void btnSpanMax_Click(object sender, EventArgs e)
        {
            double fFreqSpan = 10000; //just a big number
            m_sFreqSpan.Text = fFreqSpan.ToString("f3");
            m_sFreqSpan_Leave(null, null);
            UpdateRemoteConfigData();
        }

        private void btnSpanDefault_Click(object sender, EventArgs e)
        {
            double fFreqSpan = 10;
            m_sFreqSpan.Text = fFreqSpan.ToString("f3");
            m_sFreqSpan_Leave(null, null);
            UpdateRemoteConfigData();
        }

        private void btnSpanMin_Click(object sender, EventArgs e)
        {
            double fFreqSpan = 0; //just a very small number
            m_sFreqSpan.Text = fFreqSpan.ToString("f3");
            m_sFreqSpan_Leave(null, null);
            UpdateRemoteConfigData();
        }

        private void btnTop5plus_Click(object sender, EventArgs e)
        {
            double fAmplitudeTop = Convert.ToDouble(m_sTopDBM.Text);
            fAmplitudeTop += 5;
            m_sTopDBM.Text = fAmplitudeTop.ToString();
            m_sBottomDBM_Leave(null, null);
            UpdateRemoteConfigData();
        }

        private void btnTop5minus_Click(object sender, EventArgs e)
        {
            double fAmplitudeTop = Convert.ToDouble(m_sTopDBM.Text);
            fAmplitudeTop -= 5;
            if (fAmplitudeTop == -6)
            {
                //Correct the case you are lowering the first -1 by 5dB, in which case it makes sense to go to -5dB not -6dB
                fAmplitudeTop = -5;
            }
            m_sTopDBM.Text = fAmplitudeTop.ToString();
            m_sBottomDBM_Leave(null, null);
            UpdateRemoteConfigData();
        }

        private void btnBottom5plus_Click(object sender, EventArgs e)
        {
            double fAmplitudeBottom = Convert.ToDouble(m_sBottomDBM.Text);
            fAmplitudeBottom += 5;
            m_sBottomDBM.Text = fAmplitudeBottom.ToString();
            m_sBottomDBM_Leave(null, null);
            UpdateRemoteConfigData();
        }

        private void btnBottom5minus_Click(object sender, EventArgs e)
        {
            double fAmplitudeBottom = Convert.ToDouble(m_sBottomDBM.Text);
            fAmplitudeBottom -= 5;
            m_sBottomDBM.Text = fAmplitudeBottom.ToString();
            m_sBottomDBM_Leave(null, null);
            UpdateRemoteConfigData();
        }

        private void btnCenterMark_Click(object sender, EventArgs e)
        {
            if (m_objRFE.PeakValueMHZ > 0.0f)
            {
                m_sCenterFreq.Text = m_objRFE.PeakValueMHZ.ToString("f3");
                m_sCenterFreq_Leave(null, null);

                UpdateRemoteConfigData();
            }
        }

        private void m_sStartFreq_Leave(object sender, EventArgs e)
        {
            double fStartFreq = Convert.ToDouble(m_sStartFreq.Text);
            fStartFreq = Math.Max(m_objRFE.MinFreqMHZ, fStartFreq);
            fStartFreq = Math.Min(m_objRFE.MaxFreqMHZ - m_objRFE.MinSpanMHZ, fStartFreq);

            double fEndFreq = Convert.ToDouble(m_sEndFreq.Text);
            fEndFreq = Math.Max(m_objRFE.MinFreqMHZ + m_objRFE.MinSpanMHZ, fEndFreq);
            fEndFreq = Math.Min(m_objRFE.MaxFreqMHZ, fEndFreq);

            double fFreqSpan = (fEndFreq - fStartFreq);
            fFreqSpan = Math.Max(m_objRFE.MinSpanMHZ, fFreqSpan);
            fFreqSpan = Math.Min(m_objRFE.MaxSpanMHZ, fFreqSpan);

            fEndFreq = fStartFreq + fFreqSpan;

            m_sStartFreq.Text = fStartFreq.ToString("f3");
            m_sEndFreq.Text = fEndFreq.ToString("f3");

            m_sCenterFreq.Text = (fStartFreq + fFreqSpan / 2.0).ToString("f3");
            m_sFreqSpan.Text = (fFreqSpan).ToString("f3");
        }

        private void m_sEndFreq_Leave(object sender, EventArgs e)
        {
            double fStartFreq = Convert.ToDouble(m_sStartFreq.Text);
            fStartFreq = Math.Max(m_objRFE.MinFreqMHZ, fStartFreq);
            fStartFreq = Math.Min(m_objRFE.MaxFreqMHZ - m_objRFE.MinSpanMHZ, fStartFreq);

            double fEndFreq = Convert.ToDouble(m_sEndFreq.Text);
            fEndFreq = Math.Max(m_objRFE.MinFreqMHZ + m_objRFE.MinSpanMHZ, fEndFreq);
            fEndFreq = Math.Min(m_objRFE.MaxFreqMHZ, fEndFreq);

            double fFreqSpan = (fEndFreq - fStartFreq);
            fFreqSpan = Math.Max(m_objRFE.MinSpanMHZ, fFreqSpan);
            fFreqSpan = Math.Min(m_objRFE.MaxSpanMHZ, fFreqSpan);

            fStartFreq = fEndFreq - fFreqSpan;

            m_sStartFreq.Text = fStartFreq.ToString("f3");
            m_sEndFreq.Text = fEndFreq.ToString("f3");

            m_sCenterFreq.Text = (fStartFreq + fFreqSpan / 2.0).ToString("f3");
            m_sFreqSpan.Text = (fFreqSpan).ToString("f3");
        }

        private void m_sFreqSpan_Leave(object sender, EventArgs e)
        {
            double fFreqSpan = Convert.ToDouble(m_sFreqSpan.Text);
            fFreqSpan = Math.Max(m_objRFE.MinSpanMHZ, fFreqSpan);
            fFreqSpan = Math.Min(m_objRFE.MaxSpanMHZ, fFreqSpan);

            double fCenterFreq = Convert.ToDouble(m_sCenterFreq.Text);
            if ((fCenterFreq - (fFreqSpan / 2.0)) < m_objRFE.MinFreqMHZ)
                fCenterFreq = (m_objRFE.MinFreqMHZ + (fFreqSpan / 2.0));
            if ((fCenterFreq + (fFreqSpan / 2.0)) > m_objRFE.MaxFreqMHZ)
                fCenterFreq = (m_objRFE.MaxFreqMHZ - (fFreqSpan / 2.0));

            m_sFreqSpan.Text = fFreqSpan.ToString("f3");
            m_sCenterFreq.Text = fCenterFreq.ToString("f3");

            double fStartMHZ = fCenterFreq - fFreqSpan / 2.0;
            m_sStartFreq.Text = fStartMHZ.ToString("f3");
            m_sEndFreq.Text = (fStartMHZ + fFreqSpan).ToString("f3");
        }

        private void m_sCenterFreq_Leave(object sender, EventArgs e)
        {
            double fCenterFreq = Convert.ToDouble(m_sCenterFreq.Text);
            if (fCenterFreq > (m_objRFE.MaxFreqMHZ - (m_objRFE.MinSpanMHZ / 2.0)))
                fCenterFreq = (m_objRFE.MaxFreqMHZ - (m_objRFE.MinSpanMHZ / 2.0));
            if (fCenterFreq < (m_objRFE.MinFreqMHZ + (m_objRFE.MinSpanMHZ / 2.0)))
                fCenterFreq = (m_objRFE.MinFreqMHZ + (m_objRFE.MinSpanMHZ / 2.0));

            double fFreqSpan = Convert.ToDouble(m_sFreqSpan.Text);
            if ((fCenterFreq - (fFreqSpan / 2.0)) < m_objRFE.MinFreqMHZ)
                fFreqSpan = (fCenterFreq - m_objRFE.MinFreqMHZ) * 2.0;
            if ((fCenterFreq + (fFreqSpan / 2.0)) > m_objRFE.MaxFreqMHZ)
                fFreqSpan = (m_objRFE.MaxFreqMHZ - fCenterFreq) * 2.0;
            m_sFreqSpan.Text = fFreqSpan.ToString("f3");
            m_sCenterFreq.Text = fCenterFreq.ToString("f3");

            double fStartMHZ = fCenterFreq - fFreqSpan / 2.0;
            m_sStartFreq.Text = fStartMHZ.ToString("f3");
            m_sEndFreq.Text = (fStartMHZ + fFreqSpan).ToString("f3");
        }

        private void m_sBottomDBM_Leave(object sender, EventArgs e)
        {
            double fAmplitudeBottom = Convert.ToDouble(m_sBottomDBM.Text);
            double fAmplitudeTop = Convert.ToDouble(m_sTopDBM.Text);

            if (fAmplitudeBottom < RFECommunicator.MIN_AMPLITUDE_DBM)
                fAmplitudeBottom = RFECommunicator.MIN_AMPLITUDE_DBM;
            if (fAmplitudeBottom > (fAmplitudeTop - RFECommunicator.MIN_AMPLITUDE_RANGE_DBM))
                fAmplitudeBottom = (fAmplitudeTop - RFECommunicator.MIN_AMPLITUDE_RANGE_DBM);

            if (fAmplitudeTop > RFECommunicator.MAX_AMPLITUDE_DBM)
                fAmplitudeTop = RFECommunicator.MAX_AMPLITUDE_DBM;
            if (fAmplitudeTop < (fAmplitudeBottom + RFECommunicator.MIN_AMPLITUDE_RANGE_DBM))
                fAmplitudeTop = (fAmplitudeBottom + RFECommunicator.MIN_AMPLITUDE_RANGE_DBM);

            m_sBottomDBM.Text = fAmplitudeBottom.ToString();
            m_sTopDBM.Text = fAmplitudeTop.ToString();
        }

        private double CalculateEndFrequencyMHZ()
        {
            return m_objRFE.StartFrequencyMHZ + m_objRFE.StepFrequencyMHZ * m_objRFE.FreqSpectrumSteps;
        }

        private double CalculateFrequencySpanMHZ()
        {
            return m_objRFE.StepFrequencyMHZ * m_objRFE.FreqSpectrumSteps;
        }

        private double CalculateCenterFrequencyMHZ()
        {
            return m_objRFE.StartFrequencyMHZ + CalculateFrequencySpanMHZ() / 2.0;
        }

        private void tabSpectrumAnalyzer_Enter(object sender, EventArgs e)
        {
            groupCOM.Parent = tabSpectrumAnalyzer;
            groupDataFeed.Parent = tabSpectrumAnalyzer;
            groupFreqSettings.Parent = tabSpectrumAnalyzer;
            zedSpectrumAnalyzer.Parent = tabSpectrumAnalyzer;
            DisplayGroups();
        }

        private void toolStripCleanReport_Click(object sender, EventArgs e)
        {
            textBox_message.Text = "Text cleared." + Environment.NewLine;
        }

        private void chkCalcAverage_CheckedChanged(object sender, EventArgs e)
        {
            m_bDrawAverage = chkCalcAverage.Checked;
            if (m_objRFE.HoldMode)
                DisplaySpectrumAnalyzerData();
        }

        private void chkCalcMax_CheckedChanged(object sender, EventArgs e)
        {
            m_bDrawMax = chkCalcMax.Checked;
            if (m_objRFE.HoldMode)
                DisplaySpectrumAnalyzerData();
        }

        private void chkCalcMin_CheckedChanged(object sender, EventArgs e)
        {
            m_bDrawMin = chkCalcMin.Checked;
            if (m_objRFE.HoldMode)
                DisplaySpectrumAnalyzerData();
        }

        private void chkCalcRealtime_CheckedChanged(object sender, EventArgs e)
        {
            m_bDrawRealtime = chkCalcRealtime.Checked;
            if (m_objRFE.HoldMode)
                DisplaySpectrumAnalyzerData();
        }

        private void mnuItem_ShowPeak_CheckedChanged(object sender, EventArgs e)
        {
            m_bShowPeaks = menuShowPeak.Checked;
            if (m_objRFE.HoldMode)
                DisplaySpectrumAnalyzerData();
        }

        private void menuReinitializeData_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Are you sure to reinitialize data buffer?", "Reinitialize data buffer", MessageBoxButtons.YesNo))
            {
                if (groupCOM.Parent == tabRemoteScreen)
                {
                    m_objRFE.ScreenData.CleanAll();
                    numScreenIndex.Value = 0;
                    MessageBox.Show("Remote Screen buffer cleared.");
                }
                else
                {
                    m_objRFE.SweepData.CleanAll();
                    numericSampleSA.Value = 0;
                    MessageBox.Show("Data buffer cleared.");
                }
            }
        }

        private void menuShowControlArea_Click(object sender, EventArgs e)
        {
            DisplayGroups();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            DisplayGroups();
            tabRemoteScreen_UpdateZoomValues();
        }

        private void DisplayGroups()
        {
            MainTab.Width = Width - 14;
            MainTab.Height = Height - 64;

#if SUPPORT_EXPERIMENTAL
            StopAPIMode();
#endif
            if (groupCOM.Parent == tabSpectrumAnalyzer)
            {
                groupCOM.Visible = menuShowControlArea.Checked;
                groupDataFeed.Visible = menuShowControlArea.Checked;
                groupFreqSettings.Visible = menuShowControlArea.Checked;

                if (menuShowControlArea.Checked)
                {
                    zedSpectrumAnalyzer.Top = groupCOM.Location.Y + groupCOM.Height + 5;
                }
                else
                {
                    zedSpectrumAnalyzer.Top = 5;
                }
                zedSpectrumAnalyzer.Height = MainStatusBar.Top - zedSpectrumAnalyzer.Top - 3;
                zedSpectrumAnalyzer.Width = Width - 124;
                zedSpectrumAnalyzer.Left = 6;
                if (menuRFConnections.Checked)
                {
                    controlRFModuleSelectorConfig.m_objRFE = m_objRFE;
                    controlRFModuleSelectorConfig.SelectImageFromConfiguration();

                    int nWidth = 133;
                    if ((zedSpectrumAnalyzer.Width > 800) && (zedSpectrumAnalyzer.Height>400))
                    {
                        int nExtraWidth = zedSpectrumAnalyzer.Width - 800;
                        nWidth = nExtraWidth/2;
                        if (nWidth < 133)
                            nWidth = 133;
                    }

                    panelRFConnections.Parent = tabSpectrumAnalyzer;
                    panelRFConnections.Height = zedSpectrumAnalyzer.Height; //first do this to get the control calculate aspect ration
                    panelRFConnections.Width = nWidth - 10;
                    panelRFConnections.Height = controlRFModuleSelectorConfig.ActualPictureHeight; //later do this to reuse what it calculated as best height
                    panelRFConnections.Left = 6;
                    panelRFConnections.Top = zedSpectrumAnalyzer.Top + (zedSpectrumAnalyzer.Height - panelRFConnections.Height)/2;

                    zedSpectrumAnalyzer.Width -= controlRFModuleSelectorConfig.ActualPictureWidth;
                    zedSpectrumAnalyzer.Left = controlRFModuleSelectorConfig.ActualPictureWidth;
                }
            }
            else if (groupCOM.Parent == tabWaterfall)
            {
                groupCOM.Visible = menuShowControlArea.Checked;
                groupDataFeed.Visible = menuShowControlArea.Checked;
                if (menuShowControlArea.Checked)
                {
                    panelWaterfall.Top = groupCOM.Location.Y + groupCOM.Height + 5;
                }
                else
                {
                    panelWaterfall.Top = 5;
                }
                panelWaterfall.Height = MainStatusBar.Top - panelWaterfall.Top - 3;
                panelWaterfall.Width = Width - 124;
            }
            else
            {
                groupCOM.Visible = true;
                groupDataFeed.Visible = true;
                groupFreqSettings.Visible = true;
            }

            if (groupCOM.Parent == tabRAWDecoder)
            {
                zedRAWDecoder.Height = MainStatusBar.Top - zedRAWDecoder.Top - 3;
                zedRAWDecoder.Width = Width - 35;
            }

            if (groupCOM.Parent == tabRemoteScreen)
            {
                panelRemoteScreen.Width = Width - 45;
                panelRemoteScreen.Height = MainStatusBar.Top - groupCOM.Height - 35;
                panelRemoteScreen.BorderStyle = BorderStyle.FixedSingle;
            }

            if (groupCOM.Parent == tabReport)
            {
                textBox_message.Width = Width - 35;
                textBox_message.Height = MainTab.Height - groupCOM.Height - 50;
            }

            if (groupCOM.Parent == tabConfiguration)
            {
                controlRFModuleSelectorConfig.m_objRFE = m_objRFE;
                controlRFModuleSelectorConfig.SelectImageFromConfiguration();

                panelRFConnections.Parent = panelConfiguration;
                panelRFConnections.Left = 708;
                panelRFConnections.Top = 3; 
                panelConfiguration.Height = MainStatusBar.Top - panelConfiguration.Top - 3;
                panelConfiguration.Width = Width - 35;
                panelRFConnections.Height = panelConfiguration.Height;
                panelRFConnections.Width = panelConfiguration.Width - (panelRFConnections.Left - panelConfiguration.Left)- 10;
            }

            zedSpectrumAnalyzer.Visible = true;
            RelocateRemoteControl();

            btnTop5plus.Top = zedSpectrumAnalyzer.Top;
            btnTop5plus.Left = zedSpectrumAnalyzer.Right + 8;
            if (m_arrAnalyzerButtonList[0] != null)
            {
                btnTop5plus.Visible = true;
                for (int nInd = 1; nInd < m_arrAnalyzerButtonList.Length; nInd++)
                {
                    m_arrAnalyzerButtonList[nInd].Top = m_arrAnalyzerButtonList[nInd - 1].Bottom + 3;
                    m_arrAnalyzerButtonList[nInd].Left = m_arrAnalyzerButtonList[0].Left;
                    m_arrAnalyzerButtonList[nInd].Visible = true;
                }
            }
        }

        private void menuDarkMode_Click(object sender, EventArgs e)
        {
            m_bDark = menuDarkMode.Checked;
            DefineGraphColors(zedSpectrumAnalyzer);
            Invalidate();
        }
        #endregion

        #region Remote screen

        private void tabRemoteScreen_Enter(object sender, EventArgs e)
        {
            groupCOM.Parent = tabRemoteScreen;
            chkDumpLCDGrid.Checked = controlRemoteScreen.LCDGrid;
            chkDumpColorScreen.Checked = controlRemoteScreen.LCDColor;
            chkDumpHeader.Checked = controlRemoteScreen.HeaderText;
            UpdateButtonStatus();
            tabRemoteScreen_UpdateZoomValues();
            tabRemoteScreen.Invalidate();
            DisplayGroups();
        }

        private void numScreenIndex_ValueChanged(object sender, EventArgs e)
        {
            m_objRFE.ScreenIndex = (UInt16)numScreenIndex.Value;
            numScreenIndex.Value = m_objRFE.ScreenIndex;
            if (m_objRFE.ScreenData.Count > 0)
            {
                controlRemoteScreen.Invalidate();
            }
        }

        private void tabRemoteScreen_UpdateZoomValues()
        {
            int nHeaderSize = 0;
            if (chkDumpHeader.Checked)
            {
                nHeaderSize = 20;
            }

            int nNewZoom = (int)numericZoom.Value;
            int nLastGoodZoom = nNewZoom;

            do
            {
                nLastGoodZoom = nNewZoom;
                controlRemoteScreen.Size = new Size((int)(1.0 + m_fSizeX * (float)(nNewZoom)), (int)(1.0 + (m_fSizeY + nHeaderSize) * (float)(nNewZoom)));
                nNewZoom--;
            }
            while (((controlRemoteScreen.Size.Width > panelRemoteScreen.Size.Width) ||
                    (controlRemoteScreen.Size.Height > panelRemoteScreen.Size.Height)) 
                  && (nNewZoom>1));

            if ((nLastGoodZoom > 0) && (nLastGoodZoom != (int)numericZoom.Value))
            {
                numericZoom.Value = nLastGoodZoom;
            }
            else
            {

                controlRemoteScreen.UpdateZoom((int)(numericZoom.Value));

                labelDumpBitmapSize.Text = "Size:" + (controlRemoteScreen.Width - 2) + "x" + (controlRemoteScreen.Height - 2);

                RelocateRemoteControl();
                controlRemoteScreen.Invalidate();
            }
        }

        private void numericZoom_ValueChanged(object sender, EventArgs e)
        {
            tabRemoteScreen_UpdateZoomValues();
            tabRemoteScreen.Invalidate();
        }

        private void chkDumpScreen_CheckedChanged(object sender, EventArgs e)
        {
            m_objRFE.CaptureRemoteScreen = chkDumpScreen.Checked;
            UpdateButtonStatus();
            if (chkDumpScreen.Checked)
            {
                m_objRFE.SendCommand_EnableScreenDump();
            }
            else
            {
                m_objRFE.SendCommand_DisableScreenDump();
            }
        }

        private void chkDumpColorScreen_CheckedChanged(object sender, EventArgs e)
        {
            controlRemoteScreen.LCDColor = chkDumpColorScreen.Checked;
            controlRemoteScreen.Invalidate();
        }

        private void chkDumpHeader_CheckedChanged(object sender, EventArgs e)
        {
            controlRemoteScreen.HeaderText = chkDumpHeader.Checked;
            tabRemoteScreen_UpdateZoomValues();
        }      

        private void chkDumpLCDGrid_CheckedChanged(object sender, EventArgs e)
        {
            controlRemoteScreen.LCDGrid = chkDumpLCDGrid.Checked;
            controlRemoteScreen.Invalidate();
        }

        private void SavePNG(string sFilename)
        {
            //if no file path was explicited, add the default folder
            if (sFilename.IndexOf("\\") < 0)
            {
                sFilename = m_sDefaultDataFolder + "\\" + sFilename;
                sFilename = sFilename.Replace("\\\\", "\\");
            }

            Rectangle rectArea = controlRemoteScreen.ClientRectangle;
            using (Bitmap objAppBmp = new Bitmap(rectArea.Width, rectArea.Height))
            {
                controlRemoteScreen.DrawToBitmap(objAppBmp, rectArea);

                int nSize = (int)(numericZoom.Value);
                using (Bitmap objImage = new Bitmap(rectArea.Width, rectArea.Height))
                {
                    Graphics.FromImage(objImage).DrawImage(objAppBmp, rectArea);
                    objImage.Save(sFilename, ImageFormat.Png);
                }
            }
        }

        private void SaveImagetoolStrip_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog MySaveFileDialog = new SaveFileDialog())
                {
                    MySaveFileDialog.Filter = "Image PNG files (*.png)|*.png|All files (*.*)|*.*";
                    MySaveFileDialog.FilterIndex = 1;
                    MySaveFileDialog.RestoreDirectory = false;
                    MySaveFileDialog.InitialDirectory = m_sDefaultDataFolder;

                    GetNewFilename();
                    MySaveFileDialog.FileName = m_sFilenameRFE.Replace(".rfe", ".png");

                    if (MySaveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        SavePNG(MySaveFileDialog.FileName);
                    }
                }
            }
            catch (Exception obEx) { MessageBox.Show(obEx.Message); }
        }

        private void SaveFileRFS(string sFilename)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                //if no file path was explicited, add the default folder
                if (sFilename.IndexOf("\\") < 0)
                {
                    sFilename = m_sDefaultDataFolder + "\\" + sFilename;
                    sFilename = sFilename.Replace("\\\\", "\\");
                }

                if (m_objRFE.ScreenData.SaveFile(sFilename))
                {
                    ReportLog("File " + sFilename + " loaded with total of " + m_objRFE.ScreenData.Count + " screen shots.", false);
                }
                else
                {
                    MessageBox.Show("Wrong or unknown file format");
                }
            }
            catch (Exception obEx) { MessageBox.Show(obEx.Message); }
            Cursor.Current = Cursors.Default;
        }

        private void LoadFileRFS(string sFilename)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                if (m_objRFE.ScreenData.LoadFile(sFilename))
                {
                    UpdateScreenNumericControls();
                    UpdateButtonStatus();

                    ReportLog("File " + sFilename + " saved with total of " + m_objRFE.ScreenData.Count + " screen shots.", false);
                }
                else
                {
                    MessageBox.Show("Error saving to file");
                }
            }
            catch (Exception obEx) { MessageBox.Show(obEx.Message); }
            Cursor.Current = Cursors.Default;
        }

        private void menu_SaveRFS_Click(object sender, EventArgs e)
        {
            if (m_objRFE.ScreenData.Count > 0)
            {
                try
                {
                    using (SaveFileDialog MySaveFileDialog = new SaveFileDialog())
                    {
                        MySaveFileDialog.Filter = "RF Explorer RFS Screen files (*.rfs)|*.rfs|All files (*.*)|*.*";
                        MySaveFileDialog.FilterIndex = 1;
                        MySaveFileDialog.RestoreDirectory = false;
                        MySaveFileDialog.InitialDirectory = m_sDefaultDataFolder;

                        GetNewFilename();
                        MySaveFileDialog.FileName = m_sFilenameRFE.Replace(".rfe", ".rfs");

                        if (MySaveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            SaveFileRFS(MySaveFileDialog.FileName);
                        }
                    }
                }
                catch (Exception obEx) { MessageBox.Show(obEx.Message); }
            }
        }

        private void menu_LoadRFS_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog MyOpenFileDialog = new OpenFileDialog())
            {
                MyOpenFileDialog.Filter = "RFExplorer files (*.rfs)|*.rfs|All files (*.*)|*.*";
                MyOpenFileDialog.FilterIndex = 1;
                MyOpenFileDialog.RestoreDirectory = false;
                MyOpenFileDialog.InitialDirectory = m_sDefaultDataFolder;

                if (MyOpenFileDialog.ShowDialog() == DialogResult.OK)
                {
                    LoadFileRFS(MyOpenFileDialog.FileName);
                }
            }
        }

        private void controlRemoteScreen_Load(object sender, EventArgs e)
        {
            controlRemoteScreen.m_objRFE = m_objRFE;
        }

        private void UpdateScreenNumericControls()
        {
            //update screen data
            if (m_objRFE.ScreenData.Count < numScreenIndex.Value)
            {
                numScreenIndex.Value = m_objRFE.ScreenData.Count;
            }
            numScreenIndex.Maximum = m_objRFE.ScreenData.Count - 1;
            numScreenIndex.Value = m_objRFE.ScreenData.Count - 1;
        }

        private void RelocateRemoteControl()
        {
            controlRemoteScreen.Top = (panelRemoteScreen.Height - controlRemoteScreen.Height) / 2;
            controlRemoteScreen.Left = (panelRemoteScreen.Width - controlRemoteScreen.Width) / 2;
            MainStatusBar.Parent = groupCOM.Parent;
        }
        #endregion

        #region Report Window

        private void tabReport_Enter(object sender, EventArgs e)
        {
            groupCOM.Parent = tabReport;
            DisplayGroups();
        }

        private void ReportLog(string sLine, bool bDetailed)
        {
            if (textBox_message.IsDisposed || textBox_message.Disposing)
                return;

            DefineCommonFiles();

            if (m_sReportFilePath=="")
            {
                m_sReportFilePath = m_sAppDataFolder + "\\RFExplorerClient_report_" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                m_sReportFilePath = m_sReportFilePath.Replace("\\\\", "\\");

                labelReportFile.Text = "Report file: " + m_sReportFilePath;

                textBox_message.AppendText("Welcome to RFExplorer Client - report being saved to: " + Environment.NewLine + m_sReportFilePath + Environment.NewLine);
            }
            else
                sLine = Environment.NewLine + sLine;

            if (m_chkDebugTraces.Checked || !bDetailed)
            {
                textBox_message.AppendText(sLine);
            }

            using (StreamWriter sr = new StreamWriter(m_sReportFilePath, true))
            {
                if (m_bFirstText)
                {
                    sr.WriteLine(Environment.NewLine + Environment.NewLine +
                        "===========================================");
                    sr.WriteLine(
                        "RFExplorer client session " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                    sr.WriteLine(
                        "===========================================" + Environment.NewLine);

                    sr.WriteLine("OS:       " + Environment.OSVersion.ToString());
                    sr.WriteLine("Runtime:  " + Environment.Version.ToString());
                    sr.WriteLine("Assembly: " + Assembly.GetExecutingAssembly().ToString());
                    sr.WriteLine("File:     " + Assembly.GetExecutingAssembly().Location);

                    sr.WriteLine("");
                }
                sr.Write(sLine);
            }

            m_bFirstText = false;
        }

        private void btnSendCustomCmd_Click(object sender, EventArgs e)
        {
            string sCmd = "";
            if (comboCustomCommand.Items.Count > 0 && comboCustomCommand.Text.Length > 0)
            {
                sCmd = comboCustomCommand.Text;
                if (!RFExplorerClient.Properties.Settings.Default.CustomCommands.Contains(sCmd))
                {
                    RFExplorerClient.Properties.Settings.Default.CustomCommands.Add(sCmd);

                    //All this to refresh the combo after the string collection is changed, something it doesn't do automatically...
                    //Anybody knows a better way?
                    comboCustomCommand.DataSource = null;
                    comboCustomCommand.DataSource = RFExplorerClient.Properties.Settings.Default.CustomCommands;
                    comboCustomCommand.Text = sCmd;
                }
            }
            if (sCmd.Length > 0)
            {
                SendCommand(sCmd);
                ReportLog("Command sent: " + sCmd, true);
            }
            else
            {
                MessageBox.Show("Nothing to send.\nSpecify a command first...");
            }
        }

        private void btnSendCmd_Click(object sender, EventArgs e)
        {

            string sCmd = "";
            if (comboStdCmd.Text.Length > 0)
            {
                sCmd = comboStdCmd.Text;
                sCmd = sCmd.Substring(sCmd.LastIndexOf(':') + 2);
            }

            if (sCmd.Length > 0)
            {
                SendCommand(sCmd);
                ReportLog("Command sent: " + sCmd, true);
            }
            else
            {
                MessageBox.Show("Nothing to send.\nSpecify a command first...");
            }
        }

        #endregion

        #region Configuration
        private void tabConfiguration_Enter(object sender, EventArgs e)
        {
            groupCOM.Parent = tabConfiguration;
            groupDataFeed.Parent = tabConfiguration;
            groupFreqSettings.Parent = tabConfiguration;
            DisplayGroups();
        }
        private void btnOpenLog_Click(object sender, EventArgs e)
        {
            Process.Start(m_sReportFilePath);
        }

        private void edDefaultFilePath_Leave(object sender, EventArgs e)
        {
            m_sDefaultDataFolder = edDefaultFilePath.Text;
            SaveProperties();
        }

        private void WorkaroundBug_firmware_v1_11_switching_module()
        {
            if (menuAutoLCDOff.Checked == false)
            {
                Thread.Sleep(500);
                m_objRFE.SendCommand_ScreenOFF();
                Thread.Sleep(500);
                m_objRFE.SendCommand_ScreenON();
            }
        }

        private bool m_bInternetFWUpgradeOpenOnce=false; //used to open internet firmware upgrade page only once per session
        private bool CheckRequiredVersionForFeature(double fVersionRequired)
        {
            if (!m_objRFE.IsFirmwareSameOrNewer(fVersionRequired))
            {
                MessageBox.Show("This function can only be used with Firmware v" + fVersionRequired + " or later\nPlease upgrade your RF Explorer!", "Old firmware detected");
                if (!m_bInternetFWUpgradeOpenOnce)
                {
                    Process.Start("www.rf-explorer.com/upgrade");
                }
                m_bInternetFWUpgradeOpenOnce=true;
                ReportLog("Function required with firmware v" + fVersionRequired + " but found older " + m_objRFE.RFExplorerFirmwareDetected,true);
                return false;
            }

            return true;
        }

        private void menuEnableMainboard_Click(object sender, EventArgs e)
        {
            if (!CheckRequiredVersionForFeature(1.11))
                return;

            Cursor.Current = Cursors.WaitCursor;
            if (menuEnableMainboard.Checked == false)
            {
                ResetSettingsTitle();
                m_objRFE.SendCommand_EnableMainboard();
                WorkaroundBug_firmware_v1_11_switching_module();
            }
            controlToolStripMenuItem_DropDownOpening(sender, e);
            Application.DoEvents();
            Thread.Sleep(1000); //this sleep will guarantee the user does not switch over and over very quick creating a stability problem
            Cursor.Current = Cursors.Default;
        }

        private void menuEnableExpansionBoard_Click(object sender, EventArgs e)
        {
            if (!CheckRequiredVersionForFeature(1.11))
                return;

            Cursor.Current = Cursors.WaitCursor;
            if (menuEnableExpansionBoard.Checked == false)
            {
                ResetSettingsTitle();
                m_objRFE.SendCommand_EnableExpansion();
                WorkaroundBug_firmware_v1_11_switching_module();
            } 
            controlToolStripMenuItem_DropDownOpening(sender, e);
            Application.DoEvents();
            Thread.Sleep(1000); //this sleep will guarantee the user does not switch over and over very quick creating a stability problem
            Cursor.Current = Cursors.Default;
        }

        private void controlToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            menuEnableMainboard.Enabled = m_objRFE.PortConnected;
            menuEnableMainboard.Checked = (m_objRFE.ExpansionBoardActive == false);
            menuEnableExpansionBoard.Enabled = m_objRFE.PortConnected && (m_objRFE.ExpansionBoardModel != RFECommunicator.eModel.MODEL_NONE);
            if (menuEnableExpansionBoard.Enabled)
            {
                menuEnableExpansionBoard.Checked = m_objRFE.ExpansionBoardActive;
            }
        }

        private void menuOnlineHelp_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.rf-explorer.com/windows");
        }

        private void menuFirmware_Click(object sender, EventArgs e)
        {
            Process.Start("http://micro.arocholl.com/download/sw/fw/FirmwareReleaseNotes.pdf");
        }

        private void menuDeviceManual_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.rf-explorer.com/manual");
        }

        private void menuWindowsReleaseNotes_Click(object sender, EventArgs e)
        {
            Process.Start("http://micro.arocholl.com/download/sw/win/WindowsClientReleaseNotes.pdf");
        }
        #endregion  //configuration
    }
}
