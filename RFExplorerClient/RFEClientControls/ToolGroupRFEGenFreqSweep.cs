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
using System.Windows.Forms;
using RFExplorerCommunicator;
using System.Drawing;

namespace RFEClientControls
{
    public partial class ToolGroupRFEGenFreqSweep : UserControl
    {
        #region Properties
        private enum eFreqRangeOptions
        {
            STARTFREQ,
            STOPFREQ,
            NONE
        };

        private RFECommunicator m_objAnalyzer = null;
        /// <summary>
        /// Set analyzer object to use internally
        /// </summary>
        public RFECommunicator RFEAnalyzer
        {
            set
            {
                m_objAnalyzer = value;
            }
        }

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

        /// <summary>
        /// Get or Set start Frequency sweep
        /// </summary>
        public double Start
        {
            get
            {
                return Convert.ToDouble(m_sRFGenFreqSweepStart.Text);
            }
            set
            {
                m_sRFGenFreqSweepStart.Text = value.ToString("f3");
                ValidateRFGenSweepFreqRanges(true, eFreqRangeOptions.STARTFREQ);
            }
        }

        /// <summary>
        /// Get or Set stop sweep frequency
        /// </summary>
        public double Stop
        {
            get
            {
                return Convert.ToDouble(m_sRFGenFreqSweepStop.Text);
            }

            set
            {
                m_sRFGenFreqSweepStop.Text = value.ToString("f3");
                ValidateRFGenSweepFreqRanges(true, eFreqRangeOptions.STOPFREQ);
            }
        }

        /// <summary>
        /// Get or Set steps sweep frequency
        /// </summary>
        public UInt16 Steps
        {
            get
            {
                return Convert.ToUInt16(m_sRFGenFreqSweepSteps.Text);
            }

            set
            {
                m_sRFGenFreqSweepSteps.Text = value.ToString();
                OnRFGenFreqSweepSteps_Leave(null, null);
            }
        }
        #endregion

        #region Constructor

        public ToolGroupRFEGenFreqSweep()
        {
            InitializeComponent();
        }
        #endregion

        #region Public Events

        /// <summary>
        /// Event to change start frequency sweep
        /// </summary>
        public event EventHandler StartFreqSweepEvent;
        private void OnStartFreqSweepEvent(EventArgs eventArgs)
        {
            if (StartFreqSweepEvent != null)
            {
                StartFreqSweepEvent(this, eventArgs);
            }
        }

        /// <summary>
        /// Event to change stop sweep frequency 
        /// </summary>
        public event EventHandler StopFreqSweepEvent;
        private void OnStopFreqSweepEvent(EventArgs eventArgs)
        {
            if (StopFreqSweepEvent != null)
            {
                StopFreqSweepEvent(this, eventArgs);
            }
        }

        /// <summary>
        /// Event to change step sweep frequency
        /// </summary>
        public event EventHandler RFGenFreqSweepStepsLeaveEvent;
        private void OnRFGenFreqSweepStepsLeaveEvent(EventArgs eventArgs)
        {
            if (RFGenFreqSweepStepsLeaveEvent != null)
            {
                RFGenFreqSweepStepsLeaveEvent(this, eventArgs);
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

        #region Public Methods
        /// <summary>
        /// Update layout of the internal controls and set the container
        /// </summary>
        public void UpdateUniversalLayout()
        {
            m_groupControl_RFEGen_FrequencySweep.m_ContainerForm = this;
            m_groupControl_RFEGen_FrequencySweep.SetUniversalLayout();
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

                if (m_btnStartFreqSweepContinuous.Enabled != bGenPowerOFF)
                    m_btnStartFreqSweepContinuous.Enabled = bGenPowerOFF;
                
                bool bStopEnabled = (m_objGenerator.Mode == RFECommunicator.eMode.MODE_GEN_SWEEP_FREQ) && m_objGenerator.RFGenPowerON;

                if (m_btnStopFreqSweep.Enabled != bStopEnabled)
                    m_btnStopFreqSweep.Enabled = bStopEnabled;
            }
            else
            {
                m_groupControl_RFEGen_FrequencySweep.EnableGroup(false);
            }
            DisplayGroups();
       }

        /// <summary>
        /// Set Sweep frequency data
        /// </summary>
        public void UpdateDeviceFrequency()
        {
            m_objGenerator.RFGenStartFrequencyMHZ = Convert.ToDouble(m_sRFGenFreqSweepStart.Text);
            m_objGenerator.RFGenStopFrequencyMHZ = Convert.ToDouble(m_sRFGenFreqSweepStop.Text);
            m_objGenerator.RFGenSweepSteps = Convert.ToUInt16(m_sRFGenFreqSweepSteps.Text);
            m_objGenerator.RFGenStepWaitMS = Convert.ToUInt16(m_sRFGenDelay.Text);
        }

        /// <summary>
        /// Used to set a visible text box when is disabled, used for all blueish numeric edit boxes
        /// </summary>
        public void DisplayGroups()
        {
            ChangeTextBoxColor(m_sRFGenFreqSweepStart);
            ChangeTextBoxColor(m_sRFGenFreqSweepStop);
            ChangeTextBoxColor(m_sRFGenFreqSweepSteps);
            ChangeTextBoxColor(m_sRFGenDelay);
        }

        /// <summary>
        /// Update numeric control with receiving data from device
        /// </summary>
        public void UpdateNumericControls()
        {
            ValidateRFGenSweepFreqRanges(true, eFreqRangeOptions.NONE);
        }

        /// <summary>
        /// updates all group control values from m_objRFEGenerator object
        /// </summary>
        public void UpdateRFGeneratorControlsFromObject(bool bResetNormalizationData)
        {
            m_sRFGenFreqSweepStart.Text = m_objGenerator.RFGenStartFrequencyMHZ.ToString("f3");
            m_sRFGenFreqSweepStop.Text = m_objGenerator.RFGenStopFrequencyMHZ.ToString("f3");
            m_sRFGenFreqSweepSteps.Text = m_objGenerator.RFGenSweepSteps.ToString();
            m_sRFGenDelay.Text = m_objGenerator.RFGenStepWaitMS.ToString();

            ValidateRFGenSweepFreqRanges(bResetNormalizationData, eFreqRangeOptions.NONE);
        }
        #endregion

        #region Private Events and methods

        private void OnStartFreqSweep_Click(object sender, EventArgs e)
        {
            if (DialogResult.Cancel == MessageBox.Show("Connect Signal Generator output to a load (50 ohm)", "RF Explorer Signal Generator", MessageBoxButtons.OKCancel))
                return;
            //m_objGenerator.Mode = RFECommunicator.eMode.MODE_GEN_SWEEP_FREQ;
            OnStartFreqSweepEvent(new EventArgs());
        }

        private void OnStopFreqSweep_Click(object sender, EventArgs e)
        {
            m_objGenerator.SendCommand_GeneratorRFPowerOFF();

            OnStopFreqSweepEvent(new EventArgs());
        }

        private void OnRFGenFreqSweepSteps_Leave(object sender, EventArgs e)
        {
            try
            {
                int nSteps = Convert.ToInt32(m_sRFGenFreqSweepSteps.Text);
                if (nSteps < 2)
                    m_sRFGenFreqSweepSteps.Text = "1";
                else if (nSteps > 9999)
                    m_sRFGenFreqSweepSteps.Text = "9999";
                ValidateRFGenSweepFreqRanges(true, eFreqRangeOptions.NONE);
            }
            catch (Exception obEx)
            {
                m_sRFGenFreqSweepSteps.Text = "1";
                ReportLog(obEx.ToString());
            }
        }

        private void OnRFGenFreqSweepStart_Leave(object sender, EventArgs e)
        {
            try
            {                
                ValidateRFGenSweepFreqRanges(true, eFreqRangeOptions.STARTFREQ);
            }
            catch (Exception obEx)
            {
                if (m_objAnalyzer.PortConnected)
                {
                    m_sRFGenFreqSweepStart.Text = m_objAnalyzer.MinFreqMHZ.ToString("f3");
                }
                else
                {
                    double dFreqMinMHZ = RFECommunicator.RFGEN_MIN_FREQ_MHZ;
                    m_sRFGenFreqSweepStart.Text = dFreqMinMHZ.ToString("f3");
                }
                ReportLog(obEx.ToString());
            }
        }

        private void OnRFGenFreqSweepStop_Leave(object sender, EventArgs e)
        {
            try
            {
                ValidateRFGenSweepFreqRanges(true, eFreqRangeOptions.STOPFREQ);
            }
            catch (Exception obEx)
            {
                if (m_objAnalyzer.PortConnected)
                {
                    m_sRFGenFreqSweepStop.Text = m_objAnalyzer.MaxFreqMHZ.ToString("f3");
                }
                else
                {
                    double dFreqMaxMHZ = RFECommunicator.RFGEN_MAX_FREQ_MHZ;
                    m_sRFGenFreqSweepStop.Text = dFreqMaxMHZ.ToString("f3");
                }
                ReportLog(obEx.ToString());
            }
        }
        
        private void OnRFGenFreqSweepDelay_Leave(object sender, EventArgs e)
        {
            try
            {
                    int nDelay = Convert.ToInt32(m_sRFGenDelay.Text);
                if (nDelay < 0)
                    m_sRFGenDelay.Text = "0";
                else if (nDelay > 50000 )
                    m_sRFGenDelay.Text = "50000";
            }
            catch (Exception obEx)
            {
                ReportLog(obEx.ToString());
                m_sRFGenDelay.Text = "0";
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

        private void EnableLabelsAndEditBox(bool bGenPowerOFF)
        {
            if (m_labRFGenStartFreq.Enabled != bGenPowerOFF)
            {
                m_labRFGenStartFreq.Enabled = bGenPowerOFF;
                m_labRFGenStopFreq.Enabled = bGenPowerOFF;
                m_labRFGenSteps.Enabled = bGenPowerOFF;
                m_sRFGenFreqSweepStart.Enabled = bGenPowerOFF;
                m_sRFGenFreqSweepStop.Enabled = bGenPowerOFF;
                m_sRFGenFreqSweepSteps.Enabled = bGenPowerOFF;
                m_labRFGenDelay.Enabled = bGenPowerOFF;
                m_sRFGenDelay.Enabled = bGenPowerOFF;
            }
        }

        /// <summary>
        /// To validate generator sweep frequency ranges  
        /// </summary>
        /// <param name="bResetNormalizeData">boolean to normalize sweep data</param>
        private void ValidateRFGenSweepFreqRanges(bool bResetNormalizeData, eFreqRangeOptions eOption)
        {
            if (m_objGenerator == null || !m_objGenerator.PortConnected)
                return;

            int nSteps = Convert.ToInt32(m_sRFGenFreqSweepSteps.Text);
            double dStartMHZ = Convert.ToDouble(m_sRFGenFreqSweepStart.Text);
            double dStopMHZ = Convert.ToDouble(m_sRFGenFreqSweepStop.Text);

            if (dStartMHZ <= m_objGenerator.MinFreqMHZ)
                dStartMHZ = m_objGenerator.MinFreqMHZ;
            else if (dStartMHZ >= m_objGenerator.MaxFreqMHZ)
                dStartMHZ = m_objGenerator.MaxFreqMHZ - (nSteps / 1000.0);
            if (dStartMHZ >= dStopMHZ && dStopMHZ > (m_objGenerator.MinFreqMHZ + (nSteps / 1000.0)))
            {
                if (eOption == eFreqRangeOptions.STARTFREQ)
                    dStopMHZ = dStartMHZ + (nSteps / 1000.0);
                else if (eOption == eFreqRangeOptions.STOPFREQ)
                    dStartMHZ = dStopMHZ - (nSteps / 1000.0);
            }

            if (dStopMHZ > m_objGenerator.MaxFreqMHZ)
                dStopMHZ = m_objGenerator.MaxFreqMHZ;
            else if (dStopMHZ <= m_objGenerator.MinFreqMHZ)
                dStopMHZ = m_objGenerator.MinFreqMHZ - (nSteps / 1000.0);
            if (dStopMHZ <= dStartMHZ)
                dStartMHZ = dStopMHZ - (nSteps / 1000.0);

            if (m_objAnalyzer.PortConnected)
            {
                if (dStartMHZ < m_objAnalyzer.MinFreqMHZ)
                {
                    dStartMHZ = m_objAnalyzer.MinFreqMHZ;
                    if (dStopMHZ < m_objAnalyzer.MinFreqMHZ)
                        dStopMHZ = dStartMHZ + (nSteps / 1000.0);
                }
                else if (dStartMHZ >= (m_objAnalyzer.MaxFreqMHZ - (nSteps / 1000.0)))
                {
                    dStartMHZ = m_objAnalyzer.MaxFreqMHZ - (nSteps / 1000.0);
                    dStopMHZ = m_objAnalyzer.MaxFreqMHZ;
                }

                if (dStopMHZ > m_objAnalyzer.MaxFreqMHZ)
                    dStopMHZ = m_objAnalyzer.MaxFreqMHZ;
            }
            m_sRFGenFreqSweepStop.Text = dStopMHZ.ToString("f3");
            m_sRFGenFreqSweepStart.Text = dStartMHZ.ToString("f3");


            if (bResetNormalizeData && (m_objAnalyzer != null) && (m_objAnalyzer.PortConnected))
            {
                //if previous setup was normalized, reset as it will no longer be valid
                if (m_objAnalyzer.IsTrackingNormalized())
                {
                    if ((m_objGenerator.RFGenSweepSteps != nSteps) ||
                        (m_objGenerator.RFGenStartFrequencyMHZ != dStartMHZ) ||
                        (m_objGenerator.RFGenStopFrequencyMHZ != dStopMHZ)
                       )
                    {
                        m_objAnalyzer.ResetTrackingNormalizedData();
                        OnRFGenFreqSweepStepsLeaveEvent(new EventArgs());
                    }
                }
            }
        }
        #endregion

    }

    public class GroupControl_RFEGen_FrequencySweep : GroupBox
    {
        const int BORDER_MARGIN = 4;
        internal ToolGroupRFEGenFreqSweep m_ContainerForm = null;
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
            this.MinimumSize = new Size(Parent.Width, Parent.Height);
            this.MaximumSize = new Size(Parent.Width, Parent.Height);

            int nLeftEditBox = m_ContainerForm.m_labRFGenStartFreq.Right + 2;
            m_ContainerForm.m_sRFGenFreqSweepStart.Left = nLeftEditBox;
            m_ContainerForm.m_sRFGenFreqSweepStop.Location = new Point(nLeftEditBox, m_ContainerForm.m_sRFGenFreqSweepStart.Bottom + 2);
            m_ContainerForm.m_sRFGenDelay.Location = new Point(nLeftEditBox, m_ContainerForm.m_sRFGenFreqSweepStop.Bottom + 2);

            m_ContainerForm.m_labRFGenStartFreq.Top = m_ContainerForm.m_sRFGenFreqSweepStart.Top + 4;
            m_ContainerForm.m_labRFGenStopFreq.Top = m_ContainerForm.m_sRFGenFreqSweepStop.Top + 4;
            m_ContainerForm.m_labRFGenDelay.Top = m_ContainerForm.m_sRFGenDelay.Top + 4;
            m_ContainerForm.m_labRFGenSteps.Top = m_ContainerForm.m_labRFGenDelay.Top;
            m_ContainerForm.m_sRFGenFreqSweepSteps.Top = m_ContainerForm.m_sRFGenDelay.Top;

            int nButtonPos = m_ContainerForm.m_sRFGenFreqSweepStart.Right + 5;
            m_ContainerForm.m_btnStartFreqSweepContinuous.Left = nButtonPos;
            m_ContainerForm.m_btnStopFreqSweep.Left = nButtonPos;
            m_ContainerForm.m_labRFGenSteps.Left = nButtonPos;
            m_ContainerForm.m_sRFGenFreqSweepSteps.Left = m_ContainerForm.m_labRFGenSteps.Right+ 4;

            m_ContainerForm.m_btnStartFreqSweepContinuous.Top = m_ContainerForm.m_sRFGenFreqSweepStart.Top;
            m_ContainerForm.m_btnStopFreqSweep.Top = m_ContainerForm.m_sRFGenFreqSweepStop.Top;

            m_ContainerForm.m_btnStartFreqSweepContinuous.Height = m_ContainerForm.m_sRFGenFreqSweepStart.Height;
            m_ContainerForm.m_btnStopFreqSweep.Height = m_ContainerForm.m_sRFGenFreqSweepStart.Height;
            m_ContainerForm.m_labRFGenDelay.Height = m_ContainerForm.m_sRFGenFreqSweepStart.Height;
            m_ContainerForm.m_sRFGenDelay.Width = m_ContainerForm.m_sRFGenFreqSweepStart.Width;
            m_ContainerForm.m_sRFGenFreqSweepSteps.Width = m_ContainerForm.m_btnStartFreqSweepContinuous.Width - (m_ContainerForm.m_labRFGenSteps.Width + BORDER_MARGIN);
            m_ContainerForm.m_sRFGenFreqSweepSteps.Height = m_ContainerForm.m_sRFGenDelay.Height;

            m_ContainerForm.m_btnStopFreqSweep.Top = m_ContainerForm.m_btnStartFreqSweepContinuous.Bottom + 2;

            m_ContainerForm.m_btnStopFreqSweep.MinimumSize = m_ContainerForm.m_btnStartFreqSweepContinuous.Size;

            if (m_CollGroupBox == null)
            {
                m_CollGroupBox = new CollapsibleGroupbox(this);
                CollapseBtn_Click(null, null); //to update status first time
                m_CollGroupBox.CollapsedCaption = "SWEEP FREQ";
                this.Paint += new System.Windows.Forms.PaintEventHandler(this.Collapse_Paint);
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
