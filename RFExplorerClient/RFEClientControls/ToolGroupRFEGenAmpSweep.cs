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
    public partial class ToolGroupRFEGenAmpSweep : UserControl
    {
        #region Properties

        private RFECommunicator m_objGenerator = null;
        /// <summary>
        /// Set generator object to use internally
        /// </summary>
        public RFECommunicator RFEGenerator
        {
            set
            {
                m_objGenerator = value;
            }
        }
        #endregion
        
        #region Constructor

        public ToolGroupRFEGenAmpSweep()
        {
            InitializeComponent();
        }
        #endregion

        #region Public Events

        /// <summary>
        /// Event to change start power sweep
        /// </summary>
        public event EventHandler StartAmplSweepEvent;
        private void OnStartAmplSweepEvent(EventArgs eventArgs)
        {
            if (StartAmplSweepEvent != null)
            {
                StartAmplSweepEvent(this, eventArgs);
            }
        }

        /// <summary>
        /// Event to change stop power sweep
        /// </summary>
        public event EventHandler StopAmplSweepEvent;
        private void OnStopAmplSweepEvent(EventArgs eventArgs)
        {
            if (StopAmplSweepEvent != null)
            {
                StopAmplSweepEvent(this, eventArgs);
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

        #region Public Methods
        /// <summary>
        /// Update layout of the internal controls and set the container
        /// </summary>
        public void UpdateUniversalLayout()
        {
            m_GroupControl.m_ContainerForm = this;
            m_GroupControl.SetUniversalLayout();
        }

        /// <summary>
        /// Update button status
        /// </summary>
        public void UpdateButtonStatus()
        {
            if (m_objGenerator.PortConnected)
            {
                bool bGenPowerOFF = !m_objGenerator.RFGenPowerON;

                EnableLabelsAndEditBox(bGenPowerOFF);

                if (m_btnRFEGenStartAmplitudeSweep.Enabled != bGenPowerOFF)
                    m_btnRFEGenStartAmplitudeSweep.Enabled = bGenPowerOFF;

                bool bStopEnabled = (m_objGenerator.Mode == RFECommunicator.eMode.MODE_GEN_SWEEP_AMP) && m_objGenerator.RFGenPowerON;

                if (m_btnRFEGenStopAmplitudeSweep.Enabled != bStopEnabled)
                    m_btnRFEGenStopAmplitudeSweep.Enabled = bStopEnabled;
            }
            else
            {
                m_GroupControl.EnableGroup(false);
            }
            DisplayGroups();
        }

        /// <summary>
        /// Used to set a visible text box when is disabled, used for all blueish numeric edit boxes
        /// </summary>
        public void DisplayGroups()
        {
            ChangeTextBoxColor(m_sRFGenSteps);
            ChangeTextBoxColor(m_sRFGenDelay);
        }

        /// <summary>
        /// Updates all group control values from m_objRFEGenerator object
        /// </summary>
        public void UpdateRFGeneratorControlsFromObject()
        {
            m_sRFGenDelay.Text = m_objGenerator.RFGenStepWaitMS.ToString();

            if (m_objGenerator.RFGenStartHighPowerSwitch)
                m_comboRFGenPowerStart.SelectedIndex = 4 + m_objGenerator.RFGenStartPowerLevel;
            else
                m_comboRFGenPowerStart.SelectedIndex = m_objGenerator.RFGenStartPowerLevel;
            if (m_objGenerator.RFGenStopHighPowerSwitch)
                m_comboRFGenPowerStop.SelectedIndex = 4 + m_objGenerator.RFGenStopPowerLevel;
            else
                m_comboRFGenPowerStop.SelectedIndex = m_objGenerator.RFGenStopPowerLevel;

            UpdatePowerLevels();
        }

        /// <summary>
        /// This function updates available power levels per frequency into the combo box,
        /// based on the actual data received from the Signal Generator object
        /// </summary>
        public void UpdatePowerLevels()
        {
            try
            {
                if (m_objGenerator != null && m_objGenerator.PortConnected)
                {
                    RFE6GEN_CalibrationData objRFEGenCal = m_objGenerator.GetRFE6GENCal();
                    double dFreqMHZ = m_objGenerator.RFGenCWFrequencyMHZ;
                    if ((objRFEGenCal.GetCalSize() > 0) && (dFreqMHZ >= m_objGenerator.MinFreqMHZ) && (dFreqMHZ <= m_objGenerator.MaxFreqMHZ))
                    {
                        int nIndexStart = m_comboRFGenPowerStart.SelectedIndex;
                        int nIndexStop = m_comboRFGenPowerStop.SelectedIndex;
                        double[] arrPower = objRFEGenCal.GetEstimatedAmplitudeArray(dFreqMHZ);
                        if (arrPower != null)
                        {
                            m_comboRFGenPowerStart.Items.Clear();
                            m_comboRFGenPowerStop.Items.Clear();
                            foreach (double dVal in arrPower)
                            {
                                m_comboRFGenPowerStart.Items.Add(dVal.ToString("f1") + "dBm");
                                m_comboRFGenPowerStop.Items.Add(dVal.ToString("f1") + "dBm");
                            }
                        }
                        m_comboRFGenPowerStart.SelectedIndex = nIndexStart;
                        m_comboRFGenPowerStop.SelectedIndex = nIndexStop;
                    }
                }
            }
            catch (Exception obEx)
            {
                ReportLog(obEx.ToString());
            }
        }

        /// <summary>
        /// Set Amplitude data for sweep in object Generator
        /// </summary>
        public void UpdateDevicePower()
        {
            SetPower();
            m_objGenerator.RFGenStepWaitMS = Convert.ToUInt16(m_sRFGenDelay.Text);
        }
        #endregion

        #region Private Events and methods

        private void ReportLog(string sLine)
        {
            OnReportInfo(new EventReportInfo(sLine));
        }

        private void OnStartRFGenAmpSweep_Click(object sender, EventArgs e)
        {
            if (DialogResult.Cancel == MessageBox.Show("Connect Signal Generator output to a load (50 ohm)", "RF Explorer Signal Generator", MessageBoxButtons.OKCancel))
                return;
            try
            {
                UpdateDevicePower();

                OnStartAmplSweepEvent(new EventArgs());

            }
            catch (Exception objEx)
            {
                ReportLog(objEx.ToString());
            }
        }

        private void OnStopRFGenAmpSweep_Click(object sender, EventArgs e)
        {
            m_objGenerator.SendCommand_GeneratorRFPowerOFF();

            OnStopAmplSweepEvent(new EventArgs());
        }

        private void OnRFGenAmplSweepDelay_Leave(object sender, EventArgs e)
        {
            try
            {
                int nDelay = Convert.ToInt32(m_sRFGenDelay.Text);
                if (nDelay < 0)
                    m_sRFGenDelay.Text = "0";
                else if (nDelay > 50000)
                    m_sRFGenDelay.Text = "50000";
            }
            catch (Exception obEx)
            {
                ReportLog(obEx.ToString());
                m_sRFGenDelay.Text = "0";
            }
        }

        private void OnComboRFPowerStopSelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_comboRFGenPowerStop.SelectedIndex == 0)
            {
                m_comboRFGenPowerStop.SelectedIndex++;
            }
            if (m_comboRFGenPowerStop.SelectedIndex <= m_comboRFGenPowerStart.SelectedIndex)
            {
                m_comboRFGenPowerStart.SelectedIndex = m_comboRFGenPowerStop.SelectedIndex - 1;
            }
            int nSteps = m_comboRFGenPowerStop.SelectedIndex - m_comboRFGenPowerStart.SelectedIndex;
            m_sRFGenSteps.Text = nSteps.ToString();
        }

        private void OnComboRFPowerStartSelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_comboRFGenPowerStart.SelectedIndex == 7)
            {
                m_comboRFGenPowerStart.SelectedIndex--;
            }
            if (m_comboRFGenPowerStart.SelectedIndex >= m_comboRFGenPowerStop.SelectedIndex)
            {
                m_comboRFGenPowerStop.SelectedIndex = m_comboRFGenPowerStart.SelectedIndex + 1;//Initial State
            }
            int nSteps = m_comboRFGenPowerStop.SelectedIndex - m_comboRFGenPowerStart.SelectedIndex;
            m_sRFGenSteps.Text = nSteps.ToString();
        }
        
        private void EnableLabelsAndEditBox(bool bGenPowerOFF)
        {
            if (m_labRFGenPowerStart.Enabled != bGenPowerOFF)
            {
                m_labRFGenPowerStart.Enabled = bGenPowerOFF;
                m_labRFGenPowerStop.Enabled = bGenPowerOFF;
                m_labRFGenSteps.Enabled = bGenPowerOFF;
                m_sRFGenSteps.Enabled = bGenPowerOFF;
                m_comboRFGenPowerStart.Enabled = bGenPowerOFF;
                m_comboRFGenPowerStop.Enabled = bGenPowerOFF;
                m_labRFGenDelay.Enabled = bGenPowerOFF;
                m_sRFGenDelay.Enabled = bGenPowerOFF;
            }
        }

        private void ChangeTextBoxColor(TextBox objTextBox)
        {
            if (objTextBox != null && !objTextBox.IsDisposed)
            {
                if (objTextBox.Enabled)
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
        }

        /// <summary>
        /// Update the internal RF Generator power based on current selected combo box value
        /// </summary>
        private void SetPower()
        {
            switch (m_comboRFGenPowerStart.SelectedIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    m_objGenerator.RFGenStartHighPowerSwitch = false;
                    m_objGenerator.RFGenStartPowerLevel = Convert.ToByte(m_comboRFGenPowerStart.SelectedIndex);
                    break;

                case 4:
                case 5:
                case 6:
                case 7:
                    m_objGenerator.RFGenStartHighPowerSwitch = true;
                    m_objGenerator.RFGenStartPowerLevel = Convert.ToByte(m_comboRFGenPowerStart.SelectedIndex - 4);
                    break;
            }

            switch (m_comboRFGenPowerStop.SelectedIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    m_objGenerator.RFGenStopHighPowerSwitch = false;
                    m_objGenerator.RFGenStopPowerLevel = Convert.ToByte(m_comboRFGenPowerStop.SelectedIndex);
                    break;

                case 4:
                case 5:
                case 6:
                case 7:
                    m_objGenerator.RFGenStopHighPowerSwitch = true;
                    m_objGenerator.RFGenStopPowerLevel = Convert.ToByte(m_comboRFGenPowerStop.SelectedIndex - 4);
                    break;
            }
        }
        #endregion
    }

    public class GroupControl_RFGen_AmplitudeSweep : GroupBox
    {
        const int BORDER_MARGIN = 6;
        internal ToolGroupRFEGenAmpSweep m_ContainerForm = null;
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

            int nLeftEditBox = m_ContainerForm.m_labRFGenPowerStart.Right + BORDER_MARGIN;
            m_ContainerForm.m_comboRFGenPowerStart.Left = nLeftEditBox;
            m_ContainerForm.m_comboRFGenPowerStop.Location = new Point(nLeftEditBox, m_ContainerForm.m_comboRFGenPowerStart.Bottom + BORDER_MARGIN);
            m_ContainerForm.m_sRFGenDelay.Location = new Point(nLeftEditBox, m_ContainerForm.m_comboRFGenPowerStop.Bottom + BORDER_MARGIN);

            m_ContainerForm.m_labRFGenPowerStart.Top = m_ContainerForm.m_comboRFGenPowerStart.Top + BORDER_MARGIN - 1;
            m_ContainerForm.m_labRFGenPowerStop.Top = m_ContainerForm.m_comboRFGenPowerStop.Top + BORDER_MARGIN;
            m_ContainerForm.m_labRFGenDelay.Top = m_ContainerForm.m_sRFGenDelay.Top + BORDER_MARGIN + 1;
            m_ContainerForm.m_labRFGenSteps.Top = m_ContainerForm.m_labRFGenDelay.Top;
            m_ContainerForm.m_sRFGenSteps.Top = m_ContainerForm.m_sRFGenDelay.Top;

            int nButtonPos = m_ContainerForm.m_comboRFGenPowerStart.Right + 5;
            m_ContainerForm.m_btnRFEGenStartAmplitudeSweep.Left = nButtonPos;
            m_ContainerForm.m_btnRFEGenStopAmplitudeSweep.Left = nButtonPos;
            m_ContainerForm.m_labRFGenSteps.Left = nButtonPos;
            m_ContainerForm.m_sRFGenSteps.Left = m_ContainerForm.m_labRFGenSteps.Right + BORDER_MARGIN;

            m_ContainerForm.m_btnRFEGenStartAmplitudeSweep.Top = m_ContainerForm.m_comboRFGenPowerStart.Top - 3;
            m_ContainerForm.m_btnRFEGenStopAmplitudeSweep.Top = m_ContainerForm.m_comboRFGenPowerStop.Top - 2;
           
            m_ContainerForm.m_btnRFEGenStartAmplitudeSweep.Height = m_ContainerForm.m_sRFGenSteps.Height;
            m_ContainerForm.m_btnRFEGenStopAmplitudeSweep.Height = m_ContainerForm.m_sRFGenSteps.Height;
            m_ContainerForm.m_sRFGenSteps.Height = m_ContainerForm.m_sRFGenDelay.Height;
           
            m_ContainerForm.m_btnRFEGenStopAmplitudeSweep.Width = m_ContainerForm.m_btnRFEGenStartAmplitudeSweep.Width;
            m_ContainerForm.m_sRFGenSteps.Width = m_ContainerForm.m_btnRFEGenStartAmplitudeSweep.Width - (m_ContainerForm.m_labRFGenSteps.Width + BORDER_MARGIN);

            m_ContainerForm.m_comboRFGenPowerStart.SelectedIndex = 0;
            m_ContainerForm.m_comboRFGenPowerStop.SelectedIndex = 0;

            if (m_CollGroupBox == null)
            {
                m_CollGroupBox = new CollapsibleGroupbox(this);
                CollapseBtn_Click(null, null); //to update status first time
                this.Paint += new PaintEventHandler(Collapse_Paint);
                m_CollGroupBox.CollapseButtonClick += new EventHandler(CollapseBtn_Click);
                m_CollGroupBox.CollapsedCaption = "SWEEP AMP";
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
