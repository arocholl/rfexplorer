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
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RFExplorerCommunicator;
using System.Threading;

namespace RFEClientControls
{
    public partial class ToolGroupAnalyzerFreqSettings : UserControl
    {
        #region public properties

        private RFECommunicator m_objRFEAnalyzer = null;

        /// <summary>
        /// Set to assign object of RFECommunicator for internal use
        /// </summary>
        public RFECommunicator RFCommunicator
        {
            set
            {
                m_objRFEAnalyzer = value;
            }
        }

        /// <summary>
        /// Access to textBox FreqSpan. In case Set update other controls and send command to Analyzer  
        /// </summary>
        public double FreqSpan
        {
            get
            {
                return Convert.ToDouble(m_sAnalyzerFreqSpan.Text);
            }
            set
            {
                m_sAnalyzerFreqSpan.Text = value.ToString();
                OnFreqSpan_Leave(null, null);
                UpdateRemoteConfigData();
            }
        }

        /// <summary>
        /// Access to textBox FreqStart. In case Set update other controls and send command to Analyzer  
        /// </summary>
        public double FreqStart
        {
            get
            {
                return Convert.ToDouble(m_sAnalyzerStartFreq.Text);
            }
            set
            {
                if (m_objRFEAnalyzer != null)
                {
                    m_objRFEAnalyzer.StartFrequencyMHZ = value;
                    if (m_objRFEAnalyzer.CalculateEndFrequencyMHZ() > m_objRFEAnalyzer.MaxFreqMHZ)
                    {
                        m_objRFEAnalyzer.StartFrequencyMHZ = m_objRFEAnalyzer.MaxFreqMHZ - m_objRFEAnalyzer.CalculateFrequencySpanMHZ();
                    }
                    m_sAnalyzerStartFreq.Text = m_objRFEAnalyzer.StartFrequencyMHZ.ToString("f3");
                    m_sAnalyzerEndFreq.Text = m_objRFEAnalyzer.CalculateEndFrequencyMHZ().ToString("f3");
                    OnEndFreq_Leave(null, null);
                    UpdateRemoteConfigData();
                }
            }
        }

        /// <summary>
        /// Set Center Frecuency and update other controls. Also send command to Analyzer  
        /// </summary>
        public double FreqCenter
        {
            set
            {
                m_sAnalyzerCenterFreq.Text = value.ToString();
                OnCenterFreq_Leave(null, null);
                UpdateRemoteConfigData();
            }
        }

        /// <summary>
        /// Get the visual reference size used by all Frequency edit boxes
        /// </summary>
        public Size ReferenceSize
        {
            get
            {
                return m_sAnalyzerCenterFreq.Size;
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Create new ToolGroupAnalyzerFreqSettings
        /// </summary>
        public ToolGroupAnalyzerFreqSettings()
        {
            InitializeComponent();
        }

        #endregion

        #region Public Events

        /// <summary>
        /// Event for click over button Send. 
        /// </summary>
        public event EventHandler SendAnalyzerConfigurationEvent;

        private void OnSendAnalyzerConfigurationEvent(EventArgs eventArgs)
        {
            if (SendAnalyzerConfigurationEvent != null)
            {
                SendAnalyzerConfigurationEvent(this, eventArgs);
            }
        }

        private void ReportLog(string sLine)
        {
            OnReportInfo(new EventReportInfo(sLine));
        }
        /// <summary>
        /// Use this event to receive error or info notifications
        /// </summary>
        public event EventHandler ReportInfoEvent;
        private void OnReportInfo(EventReportInfo eventArgs)
        {
            if (ReportInfoEvent != null)
            {
                ReportInfoEvent(this, eventArgs);
            }
        }

        #endregion

        #region public functions
        /// <summary>
        /// Update layout of the internal controls and set the container
        /// </summary>
        public void UpdateUniversalLayout()
        {
            try
            {
                m_groupControl.m_ContainerForm = this;
                m_groupControl.SetUniversalLayout();
            }
            catch { }
        }

        /// <summary>
        /// Update color and enable status of all controls
        /// </summary>
        /// <param name="bUpdateSettingsFromDevice">if true, will update all control data from analyzer device, otherwise will not change data</param>
        public void UpdateButtonStatus(bool bUpdateSettingsFromDevice)
        {
            try
            {
                if (m_objRFEAnalyzer != null)
                {
                    m_groupControl.EnableGroup(m_objRFEAnalyzer.PortConnected);
                    if (bUpdateSettingsFromDevice)
                        UpdateDialogFromDevice();

                    ChangeTextBoxColor(m_sAnalyzerBottomAmplitude);
                    ChangeTextBoxColor(m_sAnalyzerCenterFreq);
                    ChangeTextBoxColor(m_sAnalyzerEndFreq);
                    ChangeTextBoxColor(m_sAnalyzerFreqSpan);
                    ChangeTextBoxColor(m_sAnalyzerTopAmplitude);
                    ChangeTextBoxColor(m_sAnalyzerStartFreq);
                }
            }
            catch { }
        }

        /// <summary>
        /// Send Data to the Analyzer contained in the TextBox
        /// </summary>
        public void UpdateRemoteConfigData()
        {
            try
            {
                if ((m_objRFEAnalyzer != null) && (m_objRFEAnalyzer.PortConnected))
                {
                    SendNewConfig(Convert.ToDouble(m_sAnalyzerStartFreq.Text), Convert.ToDouble(m_sAnalyzerEndFreq.Text),
                        ConvertFromCurrentAmplitudeUnit(m_sAnalyzerTopAmplitude.Text), ConvertFromCurrentAmplitudeUnit(m_sAnalyzerBottomAmplitude.Text));
                }
            }
            catch (Exception obEx)
            {
                ReportLog(obEx.ToString());
            }
        }

        /// <summary>
        /// Set new top and bottom amplitude in dBm and update the analyzer object if connected
        /// </summary>
        /// <param name="fBottomDBM">new bottom dBm</param>
        /// <param name="fTopDBM">new top dBm</param>
        public void SetNewAmplitude(double fBottomDBM, double fTopDBM)
        {
            m_sAnalyzerBottomAmplitude.Text = ConvertToCurrentAmplitudeString(fBottomDBM);
            m_sAnalyzerTopAmplitude.Text = ConvertToCurrentAmplitudeString(fTopDBM);
            OnAmplitudeLeave(null, null);
        }

        #endregion

        #region Private Events 
        private void OnSendAnalyzerConfiguration_Click(object sender, EventArgs e)
        {
            UpdateRemoteConfigData();

            OnSendAnalyzerConfigurationEvent(new EventArgs());
        }

        private void OnReset_Click(object sender, EventArgs e)
        {
            UpdateDialogFromDevice();
        }

        private void OnStartFreq_Leave(object sender, EventArgs e)
        {
            if (m_objRFEAnalyzer != null)
            {
                try
                {
                    double fStartFreq = Convert.ToDouble(m_sAnalyzerStartFreq.Text);
                    fStartFreq = Math.Max(m_objRFEAnalyzer.MinFreqMHZ, fStartFreq);
                    fStartFreq = Math.Min(m_objRFEAnalyzer.MaxFreqMHZ - m_objRFEAnalyzer.MinSpanMHZ, fStartFreq);

                    double fEndFreq = Convert.ToDouble(m_sAnalyzerEndFreq.Text);
                    fEndFreq = Math.Max(m_objRFEAnalyzer.MinFreqMHZ + m_objRFEAnalyzer.MinSpanMHZ, fEndFreq);
                    fEndFreq = Math.Min(m_objRFEAnalyzer.MaxFreqMHZ, fEndFreq);

                    double fFreqSpan = (fEndFreq - fStartFreq);
                    fFreqSpan = Math.Max(m_objRFEAnalyzer.MinSpanMHZ, fFreqSpan);
                    fFreqSpan = Math.Min(m_objRFEAnalyzer.MaxSpanMHZ, fFreqSpan);

                    fEndFreq = fStartFreq + fFreqSpan;

                    m_sAnalyzerStartFreq.Text = fStartFreq.ToString("f3");
                    m_sAnalyzerEndFreq.Text = fEndFreq.ToString("f3");

                    m_sAnalyzerCenterFreq.Text = (fStartFreq + fFreqSpan / 2.0).ToString("f3");
                    m_sAnalyzerFreqSpan.Text = (fFreqSpan).ToString("f3");
                }
                catch (Exception obEx)
                {
                    ReportLog(obEx.ToString());
                }
            }
        }

        private void OnEndFreq_Leave(object sender, EventArgs e)
        {
            if (m_objRFEAnalyzer != null)
            {
                try
                {
                    double fStartFreq = Convert.ToDouble(m_sAnalyzerStartFreq.Text);
                    fStartFreq = Math.Max(m_objRFEAnalyzer.MinFreqMHZ, fStartFreq);
                    fStartFreq = Math.Min(m_objRFEAnalyzer.MaxFreqMHZ - m_objRFEAnalyzer.MinSpanMHZ, fStartFreq);

                    double fEndFreq = Convert.ToDouble(m_sAnalyzerEndFreq.Text);
                    fEndFreq = Math.Max(m_objRFEAnalyzer.MinFreqMHZ + m_objRFEAnalyzer.MinSpanMHZ, fEndFreq);
                    fEndFreq = Math.Min(m_objRFEAnalyzer.MaxFreqMHZ, fEndFreq);

                    double fFreqSpan = (fEndFreq - fStartFreq);
                    fFreqSpan = Math.Max(m_objRFEAnalyzer.MinSpanMHZ, fFreqSpan);
                    fFreqSpan = Math.Min(m_objRFEAnalyzer.MaxSpanMHZ, fFreqSpan);

                    fStartFreq = fEndFreq - fFreqSpan;

                    m_sAnalyzerStartFreq.Text = fStartFreq.ToString("f3");
                    m_sAnalyzerEndFreq.Text = fEndFreq.ToString("f3");

                    m_sAnalyzerCenterFreq.Text = (fStartFreq + fFreqSpan / 2.0).ToString("f3");
                    m_sAnalyzerFreqSpan.Text = (fFreqSpan).ToString("f3");
                }
                catch (Exception obEx)
                {
                    ReportLog(obEx.ToString());
                }
            }
        }

        private void OnCenterFreq_Leave(object sender, EventArgs e)
        {
            try
            {
                if (m_objRFEAnalyzer != null)
                {
                    double fCenterFreq = Convert.ToDouble(m_sAnalyzerCenterFreq.Text);
                    if (fCenterFreq > (m_objRFEAnalyzer.MaxFreqMHZ - (m_objRFEAnalyzer.MinSpanMHZ / 2.0)))
                        fCenterFreq = (m_objRFEAnalyzer.MaxFreqMHZ - (m_objRFEAnalyzer.MinSpanMHZ / 2.0));
                    if (fCenterFreq < (m_objRFEAnalyzer.MinFreqMHZ + (m_objRFEAnalyzer.MinSpanMHZ / 2.0)))
                        fCenterFreq = (m_objRFEAnalyzer.MinFreqMHZ + (m_objRFEAnalyzer.MinSpanMHZ / 2.0));

                    double fFreqSpan = Convert.ToDouble(m_sAnalyzerFreqSpan.Text);
                    if ((fCenterFreq - (fFreqSpan / 2.0)) < m_objRFEAnalyzer.MinFreqMHZ)
                        fFreqSpan = (fCenterFreq - m_objRFEAnalyzer.MinFreqMHZ) * 2.0;
                    if ((fCenterFreq + (fFreqSpan / 2.0)) > m_objRFEAnalyzer.MaxFreqMHZ)
                        fFreqSpan = (m_objRFEAnalyzer.MaxFreqMHZ - fCenterFreq) * 2.0;
                    m_sAnalyzerFreqSpan.Text = fFreqSpan.ToString("f3");
                    m_sAnalyzerCenterFreq.Text = fCenterFreq.ToString("f3");

                    double fStartMHZ = fCenterFreq - fFreqSpan / 2.0;
                    m_sAnalyzerStartFreq.Text = fStartMHZ.ToString("f3");
                    m_sAnalyzerEndFreq.Text = (fStartMHZ + fFreqSpan).ToString("f3");
                }
            }
            catch (Exception obEx)
            {
                ReportLog(obEx.ToString());
            }
        }

        private void OnFreqSpan_Leave(object sender, EventArgs e)
        {
            if (m_objRFEAnalyzer != null)
            {
                try
                {
                    double fFreqSpan = Convert.ToDouble(m_sAnalyzerFreqSpan.Text);
                    fFreqSpan = Math.Max(m_objRFEAnalyzer.MinSpanMHZ, fFreqSpan);
                    fFreqSpan = Math.Min(m_objRFEAnalyzer.MaxSpanMHZ, fFreqSpan);

                    double fCenterFreq = Convert.ToDouble(m_sAnalyzerCenterFreq.Text);
                    if ((fCenterFreq - (fFreqSpan / 2.0)) < m_objRFEAnalyzer.MinFreqMHZ)
                        fCenterFreq = (m_objRFEAnalyzer.MinFreqMHZ + (fFreqSpan / 2.0));
                    if ((fCenterFreq + (fFreqSpan / 2.0)) > m_objRFEAnalyzer.MaxFreqMHZ)
                        fCenterFreq = (m_objRFEAnalyzer.MaxFreqMHZ - (fFreqSpan / 2.0));

                    m_sAnalyzerFreqSpan.Text = fFreqSpan.ToString("f3");
                    m_sAnalyzerCenterFreq.Text = fCenterFreq.ToString("f3");

                    double fStartMHZ = fCenterFreq - fFreqSpan / 2.0;
                    m_sAnalyzerStartFreq.Text = fStartMHZ.ToString("f3");
                    m_sAnalyzerEndFreq.Text = (fStartMHZ + fFreqSpan).ToString("f3");
                }
                catch (Exception obEx)
                {
                    ReportLog(obEx.ToString());
                }
            }
        }

        private void OnAmplitudeLeave(object sender, EventArgs e)
        {
            if (m_objRFEAnalyzer != null)
            {
                try
                {
                    bool bUpdateAnalyzer = false;

                    double fAmplitudeBottom = Convert.ToDouble(m_sAnalyzerBottomAmplitude.Text);
                    double fAmplitudeTop = Convert.ToDouble(m_sAnalyzerTopAmplitude.Text);

                    //If not in dBm convert them to dBm
                    fAmplitudeTop = ConvertFromCurrentAmplitudeUnit(fAmplitudeTop);
                    fAmplitudeBottom = ConvertFromCurrentAmplitudeUnit(fAmplitudeBottom);

                    if (fAmplitudeBottom - m_objRFEAnalyzer.AmplitudeOffsetDB < RFECommunicator.MIN_AMPLITUDE_DBM)
                    {
                        fAmplitudeBottom = RFECommunicator.MIN_AMPLITUDE_DBM + m_objRFEAnalyzer.AmplitudeOffsetDB;
                        bUpdateAnalyzer = true;
                    }
                    if (fAmplitudeTop  > RFECommunicator.MAX_AMPLITUDE_DBM + m_objRFEAnalyzer.AmplitudeOffsetDB)
                    {
                        fAmplitudeTop = RFECommunicator.MAX_AMPLITUDE_DBM + m_objRFEAnalyzer.AmplitudeOffsetDB;
                        bUpdateAnalyzer = true;
                    }

                    if (fAmplitudeBottom > (fAmplitudeTop - RFECommunicator.MIN_AMPLITUDE_RANGE_DBM))
                    {
                        fAmplitudeBottom = (fAmplitudeTop - RFECommunicator.MIN_AMPLITUDE_RANGE_DBM);
                        bUpdateAnalyzer = true;
                    }
                    if (fAmplitudeTop - m_objRFEAnalyzer.AmplitudeOffsetDB > RFECommunicator.MAX_AMPLITUDE_DBM)
                    {
                        fAmplitudeTop = RFECommunicator.MAX_AMPLITUDE_DBM + m_objRFEAnalyzer.AmplitudeOffsetDB;
                        bUpdateAnalyzer = true;
                    }
                    if (fAmplitudeTop < (fAmplitudeBottom + RFECommunicator.MIN_AMPLITUDE_RANGE_DBM))
                    {
                        fAmplitudeTop = (fAmplitudeBottom + RFECommunicator.MIN_AMPLITUDE_RANGE_DBM);
                        bUpdateAnalyzer = true;
                    }

                    //Convert them to back to used measurement units
                    m_sAnalyzerBottomAmplitude.Text = ConvertToCurrentAmplitudeString(fAmplitudeBottom);
                    m_sAnalyzerTopAmplitude.Text = ConvertToCurrentAmplitudeString(fAmplitudeTop);
                    if (bUpdateAnalyzer)
                    {
                        m_objRFEAnalyzer.AmplitudeTopDBM = fAmplitudeTop;
                        m_objRFEAnalyzer.AmplitudeBottomDBM = fAmplitudeBottom;
                    }
                }
                catch (Exception obEx)
                {
                    ReportLog(obEx.ToString());
                }
            }
        }

        #endregion

        #region Private methods

        private void ChangeTextBoxColor(TextBox objTextBox)
        {
            if (m_objRFEAnalyzer.PortConnected)
            {
                objTextBox.BackColor = Color.RoyalBlue;
                objTextBox.ForeColor = Color.White;
            }
            else
            {
                objTextBox.BackColor = Color.LightBlue;
                objTextBox.ForeColor = Color.DarkBlue;
            }
        }

        private void SendNewConfig(double fStartMHZ, double fEndMHZ, double fTopDBM, double fBottomDBM)
        {
            //#[32]C2-F:Sssssss,Eeeeeee,tttt,bbbb
            UInt32 nStartKhz = (UInt32)(fStartMHZ * 1000);
            UInt32 nEndKhz = (UInt32)(fEndMHZ * 1000);
            Int16 nTopDBM = (Int16)fTopDBM;
            Int16 nBottomDBM = (Int16)fBottomDBM;

            string sTopDBM = "";
            if (nTopDBM >= 0)
                sTopDBM = nTopDBM.ToString("D4");
            else
                sTopDBM = nTopDBM.ToString("D3");

            string sData = "C2-F:" +
                nStartKhz.ToString("D7") + "," + nEndKhz.ToString("D7") + "," +
                sTopDBM + "," + nBottomDBM.ToString("D3");
            m_objRFEAnalyzer.SendCommand(sData);

            //ResetSettingsTitle();

            Thread.Sleep(500); //wait some time for the unit to process changes, otherwise may get a different command too soon
        }

        private void UpdateDialogFromDevice()
        {
            m_sAnalyzerBottomAmplitude.Text = ConvertToCurrentAmplitudeString(m_objRFEAnalyzer.AmplitudeBottomDBM);
            m_sAnalyzerTopAmplitude.Text = ConvertToCurrentAmplitudeString(m_objRFEAnalyzer.AmplitudeTopDBM);
            m_sAnalyzerStartFreq.Text = m_objRFEAnalyzer.StartFrequencyMHZ.ToString("f3");
            m_sAnalyzerEndFreq.Text = m_objRFEAnalyzer.CalculateEndFrequencyMHZ().ToString("f3");
            m_sAnalyzerCenterFreq.Text = m_objRFEAnalyzer.CalculateCenterFrequencyMHZ().ToString("f3");
            m_sAnalyzerFreqSpan.Text = m_objRFEAnalyzer.CalculateFrequencySpanMHZ().ToString("f3");
        }

        #endregion

        #region Private tools for amplitude
        /// <summary>
        /// If the user have configured a different unit scale (dBuV or Watt) this function will
        /// return the amplitude in those units (from dBm) input
        /// </summary>
        /// <param name="dAmplitudeDBM">standard amplitude dBm value</param>
        /// <returns></returns>
        private double ConvertToCurrentAmplitudeUnit(double dAmplitudeDBM)
        {
            double fResult = 0.0f;

            switch (m_objRFEAnalyzer.CurrentAmplitudeUnit)
            {
                case RFECommunicator.eAmplitudeUnit.dBm:
                    fResult = dAmplitudeDBM;
                    break;

                case RFECommunicator.eAmplitudeUnit.dBuV:
                    fResult = RFECommunicator.Convert_dBm_2_dBuV(dAmplitudeDBM);
                    break;

                case RFECommunicator.eAmplitudeUnit.Watt:
                    fResult = RFECommunicator.Convert_dBm_2_Watt(dAmplitudeDBM);
                    break;
            }
            return fResult;
        }

        /// <summary>
        /// Returns amplitude in dBm from whatever dAmplitude current value is specified
        /// </summary>
        /// <param name="dAmplitude"></param>
        /// <returns></returns>
        private double ConvertFromCurrentAmplitudeUnit(double dAmplitude)
        {
            double fResult = 0.0f;

            switch (m_objRFEAnalyzer.CurrentAmplitudeUnit)
            {
                case RFECommunicator.eAmplitudeUnit.dBm:
                    fResult = dAmplitude;
                    break;

                case RFECommunicator.eAmplitudeUnit.dBuV:
                    fResult = RFECommunicator.Convert_dBuV_2_dBm(dAmplitude);
                    break;

                case RFECommunicator.eAmplitudeUnit.Watt:
                    fResult = RFECommunicator.Convert_Watt_2_dBm(dAmplitude);
                    break;
            }
            return fResult;
        }

        private double ConvertFromCurrentAmplitudeUnit(string sAmplitude)
        {
            return ConvertFromCurrentAmplitudeUnit(Convert.ToDouble(sAmplitude));
        }

        private string GetCurrentAmplitudeUnitFormat()
        {
            if (RFECommunicator.eAmplitudeUnit.Watt == m_objRFEAnalyzer.CurrentAmplitudeUnit)
            {
                return "E3";
            }
            else
            {
                return "0.00";
            }
        }

        /// <summary>
        /// Based on the currently used amplitude unit, it will convert to the right string representation format
        /// </summary>
        /// <param name="dBm"></param>
        /// <returns></returns>
        private string ConvertToCurrentAmplitudeString(double dBm)
        {
            return ConvertToCurrentAmplitudeUnit(dBm).ToString(GetCurrentAmplitudeUnitFormat());
        }

        #endregion
    }
    public class GroupControl_FreqSettings : GroupBox
    {
        const int BORDER_MARGIN = 4;
        internal ToolGroupAnalyzerFreqSettings m_ContainerForm = null;
        internal CollapsibleGroupbox m_CollGroupBox = null;

        /// <summary>
        /// Defines layout of the components regardless their prior position
        /// </summary>
        public void SetUniversalLayout()
        {
            if (m_ContainerForm == null)
                return;

            if (Parent.Parent == null)
                return;

            this.AutoSize = true;

            if (Parent.Height > Parent.Parent.Height)
            {
                Parent.MinimumSize = new Size(this.Width + 1, Parent.Parent.Height - 1);
                Parent.MaximumSize = new Size(this.Width + 2, Parent.Parent.Height);
                Parent.Height = Parent.Parent.Height;
            }
            this.MinimumSize = new Size(this.Width, this.Parent.Height);
            this.MaximumSize = new Size(this.Width, this.Parent.Height);

            Size SizeMinEditbox = new Size((int)(m_ContainerForm.m_labAnalyzerCenterFreq.Width * 1.4), 26);
            Size SizeMinLeftText = m_ContainerForm.m_labAnalyzerBottomAmplitude.Size;
            Size SizeMinRightText = m_ContainerForm.m_labAnalyzerFreqSpan.Size;
            int nLeftEditBox = m_ContainerForm.m_labAnalyzerBottomAmplitude.Right + 3;

            m_ContainerForm.m_sAnalyzerCenterFreq.MinimumSize = SizeMinEditbox;
            m_ContainerForm.m_sAnalyzerFreqSpan.MinimumSize = SizeMinEditbox;
            m_ContainerForm.m_sAnalyzerTopAmplitude.MinimumSize = SizeMinEditbox;
            m_ContainerForm.m_sAnalyzerBottomAmplitude.MinimumSize = SizeMinEditbox;
            m_ContainerForm.m_sAnalyzerStartFreq.MinimumSize = SizeMinEditbox;
            m_ContainerForm.m_sAnalyzerEndFreq.MinimumSize = SizeMinEditbox;

            m_ContainerForm.m_sAnalyzerCenterFreq.Location = new Point(nLeftEditBox, m_ContainerForm.m_sAnalyzerCenterFreq.Top);
            m_ContainerForm.m_sAnalyzerStartFreq.Location = new Point(nLeftEditBox, m_ContainerForm.m_sAnalyzerCenterFreq.Bottom + 2);
            m_ContainerForm.m_sAnalyzerBottomAmplitude.Location = new Point(nLeftEditBox, m_ContainerForm.m_sAnalyzerStartFreq.Bottom + 2);

            m_ContainerForm.m_labAnalyzerCenterFreq.Top = m_ContainerForm.m_sAnalyzerCenterFreq.Top + 4;
            m_ContainerForm.m_labAnalyzerStartFreq.Top = m_ContainerForm.m_sAnalyzerStartFreq.Top + 4;
            m_ContainerForm.m_labAnalyzerBottomAmplitude.Top = m_ContainerForm.m_sAnalyzerBottomAmplitude.Top + 4;

            m_ContainerForm.m_labAnalyzerCenterFreq.MinimumSize = SizeMinLeftText;
            m_ContainerForm.m_labAnalyzerStartFreq.MinimumSize = SizeMinLeftText;

            int nRightText = m_ContainerForm.m_sAnalyzerCenterFreq.Right + 5;
            m_ContainerForm.m_labAnalyzerFreqSpan.Location = new Point(nRightText, m_ContainerForm.m_labAnalyzerCenterFreq.Top);
            m_ContainerForm.m_labAnalyzerEndFreq.Location = new Point(nRightText, m_ContainerForm.m_labAnalyzerStartFreq.Top);
            m_ContainerForm.m_labAnalyzerTopAmplitude.Location = new Point(nRightText, m_ContainerForm.m_labAnalyzerBottomAmplitude.Top);

            int nRightEditBox = m_ContainerForm.m_labAnalyzerFreqSpan.Right + 3;
            m_ContainerForm.m_sAnalyzerFreqSpan.Location = new Point(nRightEditBox, m_ContainerForm.m_sAnalyzerCenterFreq.Top);
            m_ContainerForm.m_sAnalyzerEndFreq.Location = new Point(nRightEditBox, m_ContainerForm.m_sAnalyzerCenterFreq.Bottom + 2);
            m_ContainerForm.m_sAnalyzerTopAmplitude.Location = new Point(nRightEditBox, m_ContainerForm.m_sAnalyzerStartFreq.Bottom + 2);

            int nButtonPos = m_ContainerForm.m_sAnalyzerTopAmplitude.Right + 5;
            m_ContainerForm.btnAnalyzerSend.Left = nButtonPos;
            m_ContainerForm.btnAnalyzerFreqSettingsReset.Location = new Point(nButtonPos, m_ContainerForm.m_sAnalyzerBottomAmplitude.Top);
            m_ContainerForm.btnAnalyzerFreqSettingsReset.Height = m_ContainerForm.m_sAnalyzerStartFreq.Height;
            m_ContainerForm.btnAnalyzerFreqSettingsReset.Width = SizeMinRightText.Width;
            m_ContainerForm.btnAnalyzerSend.MinimumSize = new Size(m_ContainerForm.btnAnalyzerFreqSettingsReset.Width, m_ContainerForm.m_sAnalyzerStartFreq.Bottom - m_ContainerForm.m_sAnalyzerCenterFreq.Top);
            m_ContainerForm.btnAnalyzerSend.MaximumSize = m_ContainerForm.btnAnalyzerSend.MinimumSize;

            this.MinimumSize = new Size(m_ContainerForm.btnAnalyzerSend.Right + 6, m_ContainerForm.m_sAnalyzerBottomAmplitude.Bottom + 2);

            if (m_CollGroupBox == null)
            {
                m_CollGroupBox = new CollapsibleGroupbox(this);
                m_CollGroupBox.CollapsedCaption = "FREQ";
                CollapseBtn_Click(null, null); //to update status first time
                this.Paint += new PaintEventHandler(Collapse_Paint);
                m_CollGroupBox.CollapseButtonClick += new EventHandler(CollapseBtn_Click);
            }
        }

        private void CollapseBtn_Click(object sender, EventArgs e)
        {
            m_ContainerForm.MinimumSize = MinimumSize;
            m_ContainerForm.MaximumSize = MaximumSize;
        }

        private void Collapse_Paint(object sender, PaintEventArgs e)
        {
            m_CollGroupBox.Paint(e.Graphics);
        }

        public void EnableGroup(bool bEnable)
        {
            if (m_CollGroupBox == null)
                return;

            for (int nInd = 0; nInd < Controls.Count; nInd++)
            {
                Control objControl = Controls[nInd];
                if (objControl != m_CollGroupBox.CollapseButton)
                {
                    objControl.Enabled = bEnable;
                }
            }
            if (!m_CollGroupBox.CollapseButton.Enabled)
                m_CollGroupBox.CollapseButton.Enabled = true;
        }
    }
}
