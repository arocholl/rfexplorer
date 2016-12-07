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

using System.Drawing;
using System.Windows.Forms;
using RFExplorerCommunicator;
using System;


namespace RFEClientControls
{
    public partial class ToolGroupRFEGenTracking : UserControl
    {
        /// <summary>
        /// Set an Analyer RFECommunicator object
        /// </summary>
        private RFECommunicator m_objAnalyzer = null;
        public RFECommunicator RFEAnalyzer
        {
            set
            {
                m_objAnalyzer = value;
            }
        }

        /// <summary>
        /// Set a Generator RFECommunicator object
        /// </summary>
        private RFECommunicator m_objGenerator = null;
        public RFECommunicator RFEGenerator
        {
            get
            {
                return m_objGenerator;
            }

            set
            {
                m_objGenerator = value;
            }
        }

        /// <summary>
        /// Get or Set iterations average value
        /// </summary>
        public UInt16 Average
        {
            get
            {
                return (UInt16)m_nRFGENIterationAverage.Value;
            }

            set
            {
                m_nRFGENIterationAverage.Value = value;
            }
        }

        /// <summary>
        /// Get Checked property of checkbox
        /// </summary>
        public bool SNAAutoStop
        {
            get
            {
                return m_chkSNAAutoStop.Checked;
            }
            set
            {
                m_chkSNAAutoStop.Checked = value;
            }
        }

        /// <summary>
        /// Get the name of a item 
        /// </summary>
        public string ListSNAOptions
        {
            get
            {
                return m_listSNAOptions.SelectedItem.ToString();
            }
        }

        /// <summary>
        /// Get the index of item
        /// </summary>
        public int ListSNAOptionsIndex
        {
            get
            {
                return m_listSNAOptions.SelectedIndex;
            }
        }


        /// <summary>
        /// Event to normalize the measure
        /// </summary>
        public event EventHandler NormalizeTrackingStartEvent;
        private void OnNormalizeTrackingStartEvent(EventArgs eventArgs)
        {
            if (NormalizeTrackingStartEvent != null)
            {
                NormalizeTrackingStartEvent(this, eventArgs);
            }
        }

        /// <summary>
        /// Event to stop the measure
        /// </summary>
        public event EventHandler TrackingStopEvent;
        private void OnTrackingStopEvent(EventArgs eventArgs)
        {
            if (TrackingStopEvent != null)
            {
                TrackingStopEvent(this, eventArgs);
            }
        }

        /// <summary>
        /// Event to star the measure
        /// </summary>
        public event EventHandler TrackingStartEvent;
        private void OnTrackingStartEvent(EventArgs eventArgs)
        {
            if (TrackingStartEvent != null)
            {
                TrackingStartEvent(this, eventArgs);
            }
        }

        /// <summary>
        /// Event to select SNA options
        /// </summary>
        public event EventHandler SNAOptionsChangeEvent;
        private void OnSNAOptionsChangeEvent(EventArgs eventArgs)
        {
            if (SNAOptionsChangeEvent != null)
            {
                SNAOptionsChangeEvent(this, eventArgs);
            }
        }


        public ToolGroupRFEGenTracking()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Update layout of the internal controls and set the container
        /// </summary>
        public void UpdateUniversalLayout()
        {
            m_groupControl_RFEGen_Tracking.m_ContainerForm = this;
            m_groupControl_RFEGen_Tracking.SetUniversalLayout();
        }

        /// <summary>
        /// Initialize the index of SNA options list
        /// </summary>
        /// <param name="bInitialize">true to initialize, false otherwise</param>
        public void InitializeSNAOptions(bool bInitialize)
        {
            if (bInitialize && ((m_listSNAOptions.SelectedItem == null) || m_listSNAOptions.SelectedItem.ToString() == ""))
                m_listSNAOptions.SelectedIndex = 0;
        }

        /// <summary>
        /// Update button status
        /// </summary>
        /// <param name="bTransmitting">boolean if generator is power on or transmitting sweep</param>
        public void UpdateButtonStatus(bool TransmittingSweep)
        {
            if (!m_objGenerator.PortConnected)
            {
                this.Enabled = true;
                m_groupControl_RFEGen_Tracking.EnableGroup(m_objGenerator.PortConnected);
            }
            else
            {
                m_nRFGENIterationAverage.Enabled = true;
                m_labRFGenAverage.Enabled = true;
                m_chkSNAAutoStop.Enabled = true;
                m_listSNAOptions.Enabled = true;
                bool bTransmitting = TransmittingSweep || m_objGenerator.RFGenPowerON;
                m_btnNormalizeTracking.Enabled = !bTransmitting;
                m_btnTrackingStart.Enabled = !bTransmitting && m_objAnalyzer.IsTrackingNormalized();
                m_btnTrackingStop.Enabled = ((m_objAnalyzer.Mode == RFECommunicator.eMode.MODE_TRACKING) && m_objGenerator.RFGenPowerON);
            }
        }

        private void OnNormalizeTrackingStart_Click(object sender, EventArgs e)
        {
            if (DialogResult.Cancel == MessageBox.Show("Connect cables without DUT to normalize SNA setup response", "RF Explorer SNA Tracking", MessageBoxButtons.OKCancel))
                return;

            OnNormalizeTrackingStartEvent(new EventArgs());
        }

        private void OnTrackingStop_Click(object sender, EventArgs e)
        {
            OnTrackingStopEvent(new EventArgs());
        }

        private void OnTrackingStart_Click(object sender, EventArgs e)
        {           
            OnTrackingStartEvent(new EventArgs());
        }

        private void OnSNAOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnSNAOptionsChangeEvent(new EventArgs());
        }
    }

    public partial class GroupControl_RFEGen_Tracking : System.Windows.Forms.GroupBox
    {
        internal ToolGroupRFEGenTracking m_ContainerForm = null;
        internal CollapsibleGroupbox m_CollGroupBoxGenTracking = null;

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

            m_ContainerForm.m_btnTrackingStart.MinimumSize = m_ContainerForm.m_btnNormalizeTracking.Size;
            m_ContainerForm.m_btnTrackingStop.MinimumSize = m_ContainerForm.m_btnNormalizeTracking.Size;

            int nButtonHeight = (m_ContainerForm.m_btnNormalizeTracking.Height * 3) + 8;
            int nTop = ((this.Height - nButtonHeight) / 2) + 7;

            m_ContainerForm.m_btnNormalizeTracking.Top = nTop ;
            m_ContainerForm.m_btnTrackingStart.Top = m_ContainerForm.m_btnNormalizeTracking.Bottom + 2;
            m_ContainerForm.m_btnTrackingStop.Top = m_ContainerForm.m_btnTrackingStart.Bottom + 2;
            m_ContainerForm.m_chkSNAAutoStop.Top = m_ContainerForm.m_btnTrackingStart.Top + 4;
            m_ContainerForm.m_listSNAOptions.Top = m_ContainerForm.m_btnTrackingStop.Top + 1;
            m_ContainerForm.m_nRFGENIterationAverage.Top = nTop;
            m_ContainerForm.m_labRFGenAverage.Top = nTop + 4;

            int nRightPos = m_ContainerForm.m_btnNormalizeTracking.Right + 6;
            m_ContainerForm.m_listSNAOptions.Left = nRightPos;
            m_ContainerForm.m_chkSNAAutoStop.Left = nRightPos;
            m_ContainerForm.m_nRFGENIterationAverage.Left = this.Right - (m_ContainerForm.m_nRFGENIterationAverage.Width + 10);
            m_ContainerForm.m_labRFGenAverage.Left = m_ContainerForm.m_nRFGENIterationAverage.Left - m_ContainerForm.m_labRFGenAverage.Width;
            m_ContainerForm.m_listSNAOptions.Width = m_ContainerForm.m_chkSNAAutoStop.Width + 10;
            m_ContainerForm.m_nRFGENIterationAverage.Height = m_ContainerForm.m_listSNAOptions.Height;

            this.MinimumSize = new Size(m_ContainerForm.m_listSNAOptions.Right + 5, m_ContainerForm.m_btnTrackingStop.Bottom + 4);

            if (m_CollGroupBoxGenTracking == null)
            {
                m_CollGroupBoxGenTracking = new CollapsibleGroupbox(this);
                m_CollGroupBoxGenTracking.CollapsedCaption = "SNA TRACK";
                CollapseBtn_Click(null, null); //to update status first time
                m_CollGroupBoxGenTracking.CollapseButtonClick += new EventHandler(CollapseBtn_Click);
                this.Paint += new System.Windows.Forms.PaintEventHandler(this.Collapse_Paint);
            }
        }

        private void CollapseBtn_Click(object sender, EventArgs e)
        {
            m_ContainerForm.MinimumSize = MinimumSize;
            m_ContainerForm.MaximumSize = MaximumSize;
        }

        private void Collapse_Paint(object sender, PaintEventArgs e)
        {
            m_CollGroupBoxGenTracking.Paint(e.Graphics);
        }

        internal void EnableGroup(bool bEnable)
        {
            if (m_CollGroupBoxGenTracking == null)
                return;

            for (int nInd = 0; nInd < Controls.Count; nInd++)
            {
                Control objControl = Controls[nInd];
                if (objControl != m_CollGroupBoxGenTracking.CollapseButton)
                {
                    objControl.Enabled = bEnable;
                }
            }
            if (!m_CollGroupBoxGenTracking.CollapseButton.Enabled)
                m_CollGroupBoxGenTracking.CollapseButton.Enabled = true;
        }
    }

}
