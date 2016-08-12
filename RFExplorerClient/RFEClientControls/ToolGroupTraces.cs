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
    public partial class ToolGroupTraces : UserControl
    {
        #region Properties

        /// <summary>
        /// Get or Set the current value of FillTrace setting
        /// </summary>
        public bool FillTrace
        {
            get
            {
                return m_chkFillTrace.Checked;
            }
            set
            {
                m_chkFillTrace.Checked = value;
            }
        }
        /// <summary>
        /// Get or Set the current value of Smooth setting
        /// </summary>
        public bool Smooth
        {
            get
            {
                return m_chkSmooth.Checked;
            }
            set
            {
                m_chkSmooth.Checked = value;
            }
        }
        /// <summary>
        /// Get or Set the current value of ThickTrace setting
        /// </summary>
        public bool ThickTrace
        {
            get
            {
                return m_chkThick.Checked;
            }
            set
            {
                m_chkThick.Checked = value;
            }
        }
        /// <summary>
        /// Get or Set the current value of ShowGrid setting
        /// </summary>
        public bool ShowGrid
        {
            get
            {
                return m_chkShowGrid.Checked;
            }
            set
            {
                m_chkShowGrid.Checked = value;
            }
        }
        /// <summary>
        /// Get or Set the current value of AxisLabels setting
        /// </summary>
        public bool AxisLabels
        {
            get
            {
                return m_chkAxisLabels.Checked;
            }
            set
            {
                m_chkAxisLabels.Checked = value;
            }
        }
        /// <summary>
        /// Get or Set the current value of Realtime setting
        /// </summary>
        public bool Realtime
        {
            get
            {
                return m_chkCalcRealtime.Checked;
            }
            set
            {
                m_chkCalcRealtime.Checked = value;
            }
        }
        /// <summary>
        /// Get or Set the current value of Average setting
        /// </summary>
        public bool Average
        {
            get
            {
                return m_chkCalcAverage.Checked;
            }
            set
            {
                m_chkCalcAverage.Checked = value;
            }
        }
        /// <summary>
        /// Get or Set the current value of MaxPeak setting
        /// </summary>
        public bool MaxPeak
        {
            get
            {
                return m_chkCalcMaxPeak.Checked;
            }
            set
            {
                m_chkCalcMaxPeak.Checked = value;
            }
        }
        /// <summary>
        /// Get or Set the current value of MaxHold setting
        /// </summary>
        public bool MaxHold
        {
            get
            {
                return m_chkCalcMaxHold.Checked;
            }
            set
            {
                m_chkCalcMaxHold.Checked = value;
            }
        }
        /// <summary>
        /// Get or Set the current value of Minimum setting
        /// </summary>
        public bool Minimum
        {
            get
            {
                return m_chkCalcMin.Checked;
            }
            set
            {
                m_chkCalcMin.Checked = value;
            }
        }

        #endregion

        #region Constructor
        public ToolGroupTraces()
        {
            InitializeComponent();
        }
        #endregion

        #region Public Events
        /// <summary>
        /// Event for every changed in any ckeckBox of the ToolGroupTraces
        /// </summary>
        public event EventHandler ConfigurationChangeEvent;

        private void OnConfigurationChangeEvent(EventArgs eventArgs)
        {
            if (ConfigurationChangeEvent != null)
            {
                ConfigurationChangeEvent(this, eventArgs);
            }
        }

        /// <summary>
        /// Update layout of the internal controls and set the container
        /// </summary>
        public void UpdateUniversalLayout()
        {
            m_GroupControl.m_ContainerForm = this;
            m_GroupControl.SetUniversalLayout();
        }
        #endregion

        #region Private Events 
        //Every change in the state of the checkBox generate one event

        private void OnDisplayFillTraceChanged(object sender, EventArgs e)
        {
            OnConfigurationChangeEvent(new EventArgs());
        }

        private void OnDisplaySmoothChanged(object sender, EventArgs e)
        {
            OnConfigurationChangeEvent(new EventArgs());
        }

        private void OnDisplayThickTraceChanged(object sender, EventArgs e)
        {
            OnConfigurationChangeEvent(new EventArgs());
        }

        private void OnDisplayShowGridChanged(object sender, EventArgs e)
        {
            OnConfigurationChangeEvent(new EventArgs());
        }

        private void OnDisplayAxisLabelsChanged(object sender, EventArgs e)
        {
            OnConfigurationChangeEvent(new EventArgs());
        }

        private void OnDisplayRealtimeChanged(object sender, EventArgs e)
        {
            OnConfigurationChangeEvent(new EventArgs());
        }

        private void OnDisplayAverageChanged(object sender, EventArgs e)
        {
            OnConfigurationChangeEvent(new EventArgs());
        }

        private void OnDisplayMaxPeakChanged(object sender, EventArgs e)
        {
            OnConfigurationChangeEvent(new EventArgs());
        }

        private void OnDisplayMaxHoldChanged(object sender, EventArgs e)
        {
            OnConfigurationChangeEvent(new EventArgs());
        }

        private void OnDisplayMinimumChanged(object sender, EventArgs e)
        {
            OnConfigurationChangeEvent(new EventArgs());
        }

        private void ToolGroupTraces_Load(object sender, EventArgs e)
        {
        }
    }
    #endregion

    public class GroupControl_Traces : GroupBox
    {
        const int BORDER_MARGIN = 4;
        internal ToolGroupTraces m_ContainerForm = null;
        internal CollapsibleGroupbox m_CollGroupBoxTraces = null;

        /// <summary>
        /// Defines layout of the components regardless their prior position
        /// </summary>
        internal void SetUniversalLayout()
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

            //Correct Position
            m_ContainerForm.m_chkCalcRealtime.Left = m_ContainerForm.m_chkThick.Right + BORDER_MARGIN;
            m_ContainerForm.m_chkCalcAverage.Left = m_ContainerForm.m_chkThick.Right + BORDER_MARGIN;
            m_ContainerForm.m_chkCalcMaxPeak.Left = m_ContainerForm.m_chkThick.Right + BORDER_MARGIN;
            m_ContainerForm.m_chkCalcMaxHold.Left = m_ContainerForm.m_chkThick.Right + BORDER_MARGIN;
            m_ContainerForm.m_chkCalcMin.Left = m_ContainerForm.m_chkThick.Right + BORDER_MARGIN;

            if (m_CollGroupBoxTraces == null)
            {
                m_CollGroupBoxTraces = new CollapsibleGroupbox(this);
                CollapseBtn_Click(null, null); //to update status first time
                this.Paint += new PaintEventHandler(Collapse_Paint);
                m_CollGroupBoxTraces.CollapseButtonClick += new EventHandler(CollapseBtn_Click);
            }
        }

        private void CollapseBtn_Click(object sender, EventArgs e)
        {
            m_ContainerForm.MinimumSize = MinimumSize;
            m_ContainerForm.MaximumSize = MaximumSize;
        }

        private void Collapse_Paint(object sender, PaintEventArgs e)
        {
            m_CollGroupBoxTraces.Paint(e.Graphics);
        }
    }
}
