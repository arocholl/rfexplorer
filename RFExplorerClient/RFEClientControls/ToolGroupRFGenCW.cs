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
using System.Drawing;
using System.Windows.Forms;
using RFExplorerCommunicator;

namespace RFEClientControls
{
    public partial class ToolGroupRFGenCW : UserControl
    {
        #region Properties

        RFECommunicator m_objRFEGen = null;

        /// <summary>
        /// Set to assign object of RFECommunicator
        /// </summary>
        public RFECommunicator RFEGenerator
        {
            set
            {
                m_objRFEGen = value;
                UpdateButtonStatus();
            }
        }

        RFECommunicator m_objRFEAna = null;

        /// <summary>
        /// Set to assign object of RFECommunicator
        /// </summary>
        public RFECommunicator RFEAnalyzer
        {
            set
            {
                m_objRFEAna = value;
                UpdateButtonStatus();
            }
        }

        ///
        /// Get Frequency of ToolGroup
        /// </summary>
        public double FrequencyCW
        {
            get
            {
                return (Convert.ToDouble(m_sRFGenFreqCW.Text));
            }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor that InitializeComponent
        /// </summary>
        public ToolGroupRFGenCW()
        {
            InitializeComponent();
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Update layout of the internal controls and set the container
        /// </summary>
        public void UpdateUniversalLayout()
        {
            m_GroupControl.m_ContainerForm = this;
            m_GroupControl.SetUniversalLayout();
        }

        /// <summary>
        /// updates all group control values from m_objRFEGenerator object
        /// </summary>
        public void UpdateRFGeneratorControlsFromObject()
        {
            m_sRFGenFreqCW.Text = m_objRFEGen.RFGenCWFrequencyMHZ.ToString("f3");
            if (m_objRFEGen.RFGenHighPowerSwitch)
            {
                m_comboRFGenPowerCW.SelectedIndex = 4 + m_objRFEGen.RFGenPowerLevel;
            }
            else
            {
                m_comboRFGenPowerCW.SelectedIndex = m_objRFEGen.RFGenPowerLevel;
            }
        }

        /// <summary>
        /// Update Button and data in RF Generator
        /// </summary>
        public void UpdateButtonStatus()
        {
            if (!m_objRFEGen.PortConnected)
            {
                this.Enabled = true;
                m_GroupControl.EnableGroup(m_objRFEGen.PortConnected);
            }
            else
            {
                m_labRFPowerON.Enabled = true;
                m_labRFGenCWFreq.Enabled = true;
                m_labRFGenPower.Enabled = true;

                bool bRFGenConnected = m_objRFEGen.PortConnected && m_objRFEGen.IsGenerator();
                this.Enabled = bRFGenConnected;
                if (m_GroupControl.Enabled)
                {
                    m_sRFGenFreqCW.Enabled = !m_objRFEGen.RFGenPowerON;
                    m_comboRFGenPowerCW.Enabled = !m_objRFEGen.RFGenPowerON;
                }

                if (m_sRFGenFreqCW != null && !m_sRFGenFreqCW.IsDisposed)
                {
                    if (m_sRFGenFreqCW.Enabled)
                    {
                        m_sRFGenFreqCW.BackColor = Color.RoyalBlue;
                        m_sRFGenFreqCW.ForeColor = Color.White;
                    }
                    else
                    {
                        m_sRFGenFreqCW.BackColor = Color.LightBlue;
                        m_sRFGenFreqCW.ForeColor = Color.DarkBlue;
                    }
                }

                if (m_objRFEGen.RFGenPowerON)
                {
                    m_labRFPowerON.Text = "RF Power ON";
                    m_labRFPowerON.ForeColor = Color.Red;
                }
                else
                {
                    m_labRFPowerON.Text = "RF Power OFF";
                    m_labRFPowerON.ForeColor = m_btnRFEGenCWStart.ForeColor;
                }

                m_btnRFEGenCWStart.Enabled = !m_objRFEGen.RFGenPowerON;
                m_btnRFEGenCWStop.Enabled = m_objRFEGen.RFGenPowerON && (m_objRFEAna.Mode != RFECommunicator.eMode.MODE_TRACKING);
            }
        }

        /// <summary>
        /// This function updates available power levels per frequency into the combo box,
        /// based on the actual data received from the Signal Generator object
        /// </summary>
        public void UpdatePowerLevels()
        {
            OnTextChanged(null, null);
        }

        /// <summary>
        /// Set CW frequency data
        /// </summary>
        public void UpdateDeviceFrequency()
        {
            m_objRFEGen.RFGenCWFrequencyMHZ = Convert.ToDouble(m_sRFGenFreqCW.Text);
        }

        /// <summary>
        /// Update the internal RF Generator power based on current selected combo box value
        /// </summary>
        public void UpdateDevicePower()
        {
            switch (m_comboRFGenPowerCW.SelectedIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    m_objRFEGen.RFGenHighPowerSwitch = false;
                    m_objRFEGen.RFGenPowerLevel = Convert.ToByte(m_comboRFGenPowerCW.SelectedIndex);
                    break;

                case 4:
                case 5:
                case 6:
                case 7:
                    m_objRFEGen.RFGenHighPowerSwitch = true;
                    m_objRFEGen.RFGenPowerLevel = Convert.ToByte(m_comboRFGenPowerCW.SelectedIndex - 4);
                    break;
            }
        }

        /// <summary>
        /// Update the internal RF Generator frequency based on current selected edit box value
        /// </summary>
        /// 
        public void SetFrequency()
        {
            m_objRFEGen.RFGenCWFrequencyMHZ = Convert.ToDouble(m_sRFGenFreqCW.Text);
        }

        #endregion

        #region Public Events 

        /// <summary>
        /// Event for frecuency change of ToolGroupRFGenCW
        /// </summary>
        public event EventHandler FrequencyChangeEvent;

        private void OnFrequencyChangeEvent(EventArgs eventArgs)
        {
            if (FrequencyChangeEvent != null)
            {
                FrequencyChangeEvent(this, eventArgs);
            }
        }

        /// <summary>
        /// Event for change power in the combo box of ToolGroupRFGenCW
        /// </summary>
        public event EventHandler PowerChangeEvent;
        private void OnPowerChangeEvent(EventArgs eventArgs)
        {
            if (PowerChangeEvent != null)
            {
                PowerChangeEvent(this, eventArgs);
            }
        }

        /// <summary>
        /// Event for start and stop push button of ToolGroupRFGenCW
        /// </summary>
        public event EventHandler StartStopEvent;
        private void OnStartStopEvent(EventArgs eventArgs)
        {
            if (StartStopEvent != null)
            {
               StartStopEvent(this, eventArgs);
            }
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

        #region Private Events 

        private void OnRFGenFreqLeave(object sender, EventArgs e)
        {
            double dFreqMinMHZ = RFECommunicator.RFGEN_MIN_FREQ_MHZ;
            double dFreqMaxMHZ = RFECommunicator.RFGEN_MAX_FREQ_MHZ;
            double dFreqMHZ = dFreqMinMHZ;
            try
            {
                dFreqMHZ = Convert.ToDouble(m_sRFGenFreqCW.Text);
            }
            catch (FormatException objEx)
            {
                ReportLog(objEx.ToString());
                m_sRFGenFreqCW.Text = dFreqMinMHZ.ToString("f3");
            }
            if (m_objRFEGen != null)
            {
                dFreqMinMHZ = m_objRFEGen.MinFreqMHZ;
                dFreqMaxMHZ = m_objRFEGen.MaxFreqMHZ;
            }
            if (dFreqMHZ < dFreqMinMHZ)
                m_sRFGenFreqCW.Text = dFreqMinMHZ.ToString("f3");
            if (dFreqMHZ > dFreqMaxMHZ)
                m_sRFGenFreqCW.Text = dFreqMaxMHZ.ToString("f3");
            OnFrequencyChangeEvent(new EventArgs());
        }

        int m_nPreviousRFGenPowerCWSelectedIndex = -1; //T0003
        private void OnComboRFPowerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_nPreviousRFGenPowerCWSelectedIndex == m_comboRFGenPowerCW.SelectedIndex)
                return; //T0003 this is not really changing the index, may come from a simple change in internal text

            m_nPreviousRFGenPowerCWSelectedIndex = m_comboRFGenPowerCW.SelectedIndex;
            OnPowerChangeEvent(new EventArgs());
        }

        private void OnStartClick(object sender, EventArgs e)
        {
            if (DialogResult.Cancel == MessageBox.Show("Connect Signal Generator output to a load (50 ohm)", "RF Explorer Signal Generator", MessageBoxButtons.OKCancel))
                return;
            try
            {
                double dFreqMHZ = Convert.ToDouble(m_sRFGenFreqCW.Text);

                m_objRFEGen.RFGenCWFrequencyMHZ = Convert.ToDouble(m_sRFGenFreqCW.Text);
              
                UpdateDevicePower();

                OnStartStopEvent(new EventArgs());
            }
            catch (Exception objEx)
            {
                ReportLog(objEx.ToString());
            }
        }

        private void OnStopClick(object sender, EventArgs e)
        {
            m_objRFEGen.SendCommand_GeneratorRFPowerOFF();

            OnStartStopEvent(new EventArgs());
        }

        private void ToolGroupRFGenCw_Load(object sender, EventArgs e)
        {
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            try
            {
                if (m_objRFEGen != null && m_objRFEGen.PortConnected)
                {
                    RFE6GEN_CalibrationData objRFEGenCal = m_objRFEGen.GetRFE6GENCal();
                    double dFreqMHZ = Convert.ToDouble(m_sRFGenFreqCW.Text);
                    if ((objRFEGenCal.GetCalSize() > 0) && (dFreqMHZ >= m_objRFEGen.MinFreqMHZ) && (dFreqMHZ <= m_objRFEGen.MaxFreqMHZ))
                    {
                        int nIndex = m_comboRFGenPowerCW.SelectedIndex;
                        double[] arrPower = objRFEGenCal.GetEstimatedAmplitudeArray(dFreqMHZ);
                        if (arrPower != null)
                        {
                            m_comboRFGenPowerCW.Items.Clear();
                            foreach (double dVal in arrPower)
                            {
                                m_comboRFGenPowerCW.Items.Add(dVal.ToString("f1") + "dBm");
                            }
                        }
                        m_comboRFGenPowerCW.SelectedIndex = nIndex;
                    }
                }
            }
            catch (Exception obEx)
            {
                ReportLog(obEx.ToString());
            }
        }
        private void ReportLog(string sLine)
        {
            OnReportInfo(new EventReportInfo(sLine));
        }
        
        #endregion
    }

    public class GroupControl_RFGenCW : System.Windows.Forms.GroupBox
    {
        const int BORDER_MARGIN = 4;
        internal ToolGroupRFGenCW m_ContainerForm = null;
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

            m_ContainerForm.m_comboRFGenPowerCW.SelectedIndex = 0;
            m_ContainerForm.m_btnRFEGenCWStop.Width = m_ContainerForm.m_btnRFEGenCWStart.Width;

            m_ContainerForm.m_sRFGenFreqCW.Left = m_ContainerForm.m_labRFGenCWFreq.Right + BORDER_MARGIN;
            m_ContainerForm.m_btnRFEGenCWStart.Left = m_ContainerForm.m_sRFGenFreqCW.Right + BORDER_MARGIN;
            m_ContainerForm.m_comboRFGenPowerCW.Left = m_ContainerForm.m_labRFGenCWFreq.Right + BORDER_MARGIN;
            m_ContainerForm.m_btnRFEGenCWStop.Left = m_ContainerForm.m_comboRFGenPowerCW.Right + BORDER_MARGIN;

            if (m_CollGroupBox == null)
            {
                m_CollGroupBox = new CollapsibleGroupbox(this);
                CollapseBtn_Click(null, null); //to update status first time
                this.Paint += new PaintEventHandler(Collapse_Paint);
                m_CollGroupBox.CollapseButtonClick += new EventHandler(CollapseBtn_Click);
                m_CollGroupBox.CollapsedCaption = "Signal Gen CW";
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

        internal void EnableGroup(bool bEnable)
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
