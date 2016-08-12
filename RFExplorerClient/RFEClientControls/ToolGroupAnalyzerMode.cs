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
    public partial class ToolGroupAnalyzerMode : UserControl
    {
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

        /// <summary>
        /// Get or Set iterations value 
        /// </summary>
        public UInt16 Iterations
        {
            get
            {
                return (UInt16)m_numericIterations.Value;
            }
            set
            {
                m_numericIterations.Value = value;
            }
        }

        /// <summary>
        /// Get or Set Sweep index value
        /// </summary>
        public UInt32 SweepIndex
        {
            get
            {
                return (UInt32)m_NumericSweepIndex.Value;
            }
            set
            {
                m_NumericSweepIndex.Value = value;
            }
        }

        /// <summary>
        /// Get sweep index size 
        /// </summary>
        public Size SweepIndexSize
        {
            get
            {
                return m_NumericSweepIndex.Size;
            }
        }

        #region Events
        /// <summary>
        /// Event for Run Mode change of ToolGroupAnalyzerMode
        /// </summary>
        public event EventHandler RunModeChangeEvent;
        private void OnRunModeChangeEvent(EventArgs eventArgs)
        {
            if (RunModeChangeEvent != null)
            {
                RunModeChangeEvent(this, eventArgs);
            }
        }

        /// <summary>
        /// Event for Hold Mode change of ToolGroupAnalyzerMode
        /// </summary>
        public event EventHandler HoldModeChangeEvent;
        private void OnHoldModeChangeEvent(EventArgs eventArgs)
        {
            if (HoldModeChangeEvent != null)
            {
                HoldModeChangeEvent(this, eventArgs);
            }
        }

        /// <summary>
        /// Event for Numeric Sample SA change of ToolGroupAnalyzerMode
        /// </summary>
        public event EventHandler NumericSampleSAChangeEvent;
        private void OnSweepIndexChangeEvent(EventArgs eventArgs)
        {
            if (NumericSampleSAChangeEvent != null)
            {
                NumericSampleSAChangeEvent(this, eventArgs);
            }
        }

        private void OnNumericSweepIndex_ValueChanged(object sender, EventArgs e)
        {
            if (m_objAnalyzer!=null && (m_objAnalyzer.HoldMode || (!m_objAnalyzer.PortConnected && m_objAnalyzer.SweepData.Count > 0)))
            {
                if (m_NumericSweepIndex.Value > m_objAnalyzer.SweepData.Count)
                {
                    m_NumericSweepIndex.Value = m_objAnalyzer.SweepData.Count;
                }

                OnSweepIndexChangeEvent(new EventArgs());
            }
        }

        private void OnRunMode_CheckedChanged(object sender, EventArgs e)
        {
            m_objAnalyzer.HoldMode = !m_chkRunMode.Checked;

            OnRunModeChangeEvent(new EventArgs());
        }

        private void OnHoldMode_CheckedChanged(object sender, EventArgs e)
        {
            if (m_objAnalyzer == null)
                return;

            m_objAnalyzer.HoldMode = m_chkHoldMode.Checked;
            if (m_objAnalyzer.HoldMode)
            {
                //Send hold mode to RF Explorer to stop RS232 traffic
                m_objAnalyzer.SendCommand_Hold();
            }
            else
            {
                //Not on hold anymore, restore RS232 traffic
                m_objAnalyzer.SendCommand_RequestConfigData();
            }

            OnHoldModeChangeEvent(new EventArgs());
        }
        #endregion

        public ToolGroupAnalyzerMode()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Update initial internal status and layout of controls
        /// </summary>
        public void UpdateUniversalLayout()
        {
            try
            {
                m_NumericSweepIndex.Minimum = 0;
                m_NumericSweepIndex.Value = 0;
                m_numericIterations.Maximum = 100000;
                m_numericIterations.Value = 10;
                m_chkHoldMode.Checked = !m_chkRunMode.Checked;

                m_groupControl_DataFeed.m_ContainerForm = this;
                m_groupControl_DataFeed.SetUniversalLayout();
                UpdateNumericControls();
            }
            catch { }
        }

        /// <summary>
        /// Update status of all internal buttons based on analyzer object status
        /// </summary>
        public void UpdateButtonStatus()
        {
            if (m_objAnalyzer != null)
            {
                m_chkHoldMode.Enabled = m_objAnalyzer.PortConnected;
                m_chkRunMode.Enabled = m_objAnalyzer.PortConnected;
                m_chkRunMode.Checked = !m_objAnalyzer.HoldMode;
                m_chkHoldMode.Checked = m_objAnalyzer.HoldMode;
            }
        }

        /// <summary>
        /// Update values for all internal sweep numeric controls based on analyzer data
        /// </summary>
        public void UpdateNumericControls()
        {
#if CALLSTACK_REALTIME
            Console.WriteLine("CALLSTACK:UpdateSweepNumericControls");
#endif
            if (m_objAnalyzer != null)
            {
                //Update sweep data
                if (m_objAnalyzer.SweepData.Count < m_NumericSweepIndex.Value)
                {
                    m_NumericSweepIndex.Value = (m_objAnalyzer.SweepData.Count - 1);
                }
                //we can now safely change the max and the value (if not did already)
                m_NumericSweepIndex.Maximum = (m_objAnalyzer.SweepData.Count - 1);
                m_NumericSweepIndex.Value = (m_objAnalyzer.SweepData.Count - 1);
            }
        }

    }

    public class GroupControl_AnalyzerMode : GroupBox
    {
        const int BORDER_MARGIN = 4;
        internal ToolGroupAnalyzerMode m_ContainerForm = null;
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
            this.MinimumSize = new Size(this.Width, this.Height);
            this.MaximumSize = new Size(this.Width, this.Height);

            int nLeftPos = m_ContainerForm.m_labDataSample.Right + 2;
            m_ContainerForm.m_NumericSweepIndex.Left = nLeftPos;
            m_ContainerForm.m_numericIterations.Left = nLeftPos;
            m_ContainerForm.m_NumericSweepIndex.Top = m_ContainerForm.m_chkRunMode.Bottom + 3;
            m_ContainerForm.m_numericIterations.Top = m_ContainerForm.m_NumericSweepIndex.Bottom + 3;

            m_ContainerForm.m_chkRunMode.MinimumSize = m_ContainerForm.m_labDataSample.Size;
            m_ContainerForm.m_chkHoldMode.MinimumSize = m_ContainerForm.m_numericIterations.Size;
            m_ContainerForm.m_NumericSweepIndex.MaximumSize = m_ContainerForm.m_numericIterations.Size;
            m_ContainerForm.m_NumericSweepIndex.MinimumSize = m_ContainerForm.m_numericIterations.Size;
            m_ContainerForm.m_chkHoldMode.Left = m_ContainerForm.m_numericIterations.Left;

            int nRightPos = m_ContainerForm.m_NumericSweepIndex.Right + 5;

            m_ContainerForm.m_labDataSample.Top = m_ContainerForm.m_NumericSweepIndex.Top + 6;
            m_ContainerForm.m_labIterations.Top = m_ContainerForm.m_numericIterations.Top + 6;

            if (m_CollGroupBox == null)
            {
                m_CollGroupBox = new CollapsibleGroupbox(this);
                CollapseBtn_Click(null, null); //to update status first time
                this.Paint += new PaintEventHandler(this.Collapse_Paint);
                m_CollGroupBox.CollapseButtonClick += new EventHandler(CollapseBtn_Click);
                m_CollGroupBox.CollapsedCaption = "Run Mode";
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
    }
}
