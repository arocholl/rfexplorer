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
    public partial class ToolGroupMarkers : UserControl
    {
        #region Properties

        /// <summary>
        /// Signal Trace that RF Explorer is now tracking
        /// </summary>
        public RFECommunicator.RFExplorerSignalType TrackSignalPeak = RFECommunicator.RFExplorerSignalType.Realtime;

        private MarkerCollection m_objMarkerCollection = null;
        /// <summary>
        /// MarkerCollection of this ToolGroup
        /// </summary>
        public MarkerCollection Markers
        {
            get
            {
                return m_objMarkerCollection;
            }
        }

        private RFECommunicator m_objCommunicator = null;

        /// <summary>
        /// Set to assign object of RFECommunicator for internal use
        /// </summary>
        public RFECommunicator RFCommunicator
        {
            set
            {
                m_objCommunicator = value;
            }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of class ToolGroupMarkers
        /// </summary>
        /// <param name="colMarkers">Markers Collection to use and update internally</param>
        public ToolGroupMarkers(MarkerCollection colMarkers)
        {
            InitializeComponent();
            m_objMarkerCollection = colMarkers;
        }
        #endregion

        #region Public Events

        /// <summary>
        /// Event for MarkerChanged. ToolGroup index changed
        /// </summary>
        public event EventHandler MarkerChangedEvent;

        private void OnMarkerChangedEvent(EventArgs eventArgs)
        {
            if (MarkerChangedEvent != null)
            {
                MarkerChangedEvent(this, eventArgs);
            }
        }

        /// <summary>
        /// Event for MarkerValueChanged. Any Value of the ToolGroup is changed
        /// </summary>
        public event EventHandler MarkerValueChangedEvent;

        private void OnMarkerValueChangedEvent(EventArgs eventArgs)
        {
            if (MarkerValueChangedEvent != null)
            {
                MarkerValueChangedEvent(this, eventArgs);
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Update layout of the internal controls and set the container
        /// </summary>
        public void UpdateUniversalLayout()
        {
            try
            {
                m_GroupControl.m_ContainerForm = this;
                if ((m_objCommunicator != null) && m_objCommunicator.IsGenerator())
                {
                    TrackSignalPeak = RFECommunicator.RFExplorerSignalType.Average;
                    m_TrackCombo.Items.RemoveAt(2);
                    m_TrackCombo.Items.RemoveAt(2);
                    m_TrackCombo.Items.RemoveAt(2);
                }
                m_GroupControl.SetUniversalLayout();
            }
            catch {}
        }

        public void UpdateButtonStatus()
        {
            int nMarker = (int)m_MarkerIndex.Value;

            m_chkEnabled.Checked = m_objMarkerCollection.IsMarkerEnabled(nMarker - 1);

            if (nMarker > 1)
            {
                if (m_sMarkerFreq != null && !m_sMarkerFreq.IsDisposed)
                {
                    m_sMarkerFreq.Text = m_objMarkerCollection.GetMarkerFrequency(nMarker - 1).ToString("0000.000");
                }
            }
            else
            {
                m_TrackCombo.SelectedIndex = (int)TrackSignalPeak;
            }
        }

        /// <summary>
        /// Method to identify if the Marker is visible
        /// </summary>
        /// <param name="nMarkerID">Marker Identifier</param>
        public bool IsMarkerEnabled(int nMarkerID)
        {
            return m_objMarkerCollection.IsMarkerEnabled(nMarkerID);
        }
        #endregion

        #region Private Events and methods

        private void OnMarkerIndexChanged(object sender, EventArgs e)
        {
            int nMarker = Convert.ToUInt16(m_MarkerIndex.Value);
            if (nMarker == 1)
            {
                m_sMarkerFreq.Enabled = false;
                m_labAnalyzerCenterFreq.Enabled = false;
                m_TrackCombo.Enabled = true;
                m_labTrack.Enabled = true;
            }
            else
            {
                m_sMarkerFreq.Enabled = true;
                m_labAnalyzerCenterFreq.Enabled = true;
                m_TrackCombo.Enabled = false;
                m_labTrack.Enabled = false;

                m_sMarkerFreq.Text = m_objMarkerCollection.GetMarkerFrequency(nMarker - 1).ToString("0000.000");
            }

            m_chkEnabled.Checked = m_objMarkerCollection.IsMarkerEnabled(nMarker - 1);

            OnMarkerChangedEvent(new EventArgs());
        }

        private void OnMarkerEnabledChanged(object sender, EventArgs e)
        {
            int nMarker = Convert.ToUInt16(m_MarkerIndex.Value);

            if ((m_objMarkerCollection.IsMarkerEnabled(nMarker - 1) != (m_chkEnabled.Checked)))
            {
                if (m_chkEnabled.Checked)
                    m_objMarkerCollection.EnableMarker(nMarker - 1);
                else
                    m_objMarkerCollection.HideMarker(nMarker - 1);

                OnMarkerValueChangedEvent(new EventArgs());
            }
        }

        private void OnTrackComboSelectedIndexChanged(object sender, EventArgs e)
        {
            if (TrackSignalPeak != (RFECommunicator.RFExplorerSignalType)m_TrackCombo.SelectedIndex)
            {
                TrackSignalPeak = (RFECommunicator.RFExplorerSignalType)m_TrackCombo.SelectedIndex;
                OnMarkerValueChangedEvent(new EventArgs());
            }
        }

        private void OnFreqLeave(object sender, EventArgs e)
        {
            int nMarker = Convert.ToUInt16(m_MarkerIndex.Value);
            double dFreq;

            if (nMarker > 1)
            {
                try
                {
                    dFreq = Convert.ToDouble(m_sMarkerFreq.Text);                    
                }
                catch
                {
                    dFreq = 1000.000;
                }
                if (dFreq != m_objMarkerCollection.GetMarkerFrequency(nMarker - 1))
                {
                    m_objMarkerCollection.SetMarkerFrequency(nMarker - 1, dFreq);
                    OnMarkerValueChangedEvent(new EventArgs());
                }
            }
        }

        private void ToolGroupMarkers_Load(object sender, EventArgs e)
        {
        }
        #endregion
    }

    public class GroupControl_Markers : System.Windows.Forms.GroupBox
    {
        const int BORDER_MARGIN = 4;
        internal ToolGroupMarkers m_ContainerForm = null;
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

            m_ContainerForm.m_MarkerIndex.Left = m_ContainerForm.label1.Right + BORDER_MARGIN;
            m_ContainerForm.m_chkEnabled.Left = m_ContainerForm.m_MarkerIndex.Right + BORDER_MARGIN;

            m_ContainerForm.m_sMarkerFreq.Left = m_ContainerForm.label1.Right + BORDER_MARGIN;
            m_ContainerForm.m_TrackCombo.Left = m_ContainerForm.label1.Right + BORDER_MARGIN;

            if (m_CollGroupBox == null)
            {
                m_CollGroupBox = new CollapsibleGroupbox(this);
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
    }
}
