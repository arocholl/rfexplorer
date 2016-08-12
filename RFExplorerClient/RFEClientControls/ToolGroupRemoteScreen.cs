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
    public partial class ToolGroupRemoteScreen : UserControl
    {
        #region Data Members and Properties
        private RFECommunicator m_objAnalyzer = null;
        public RFECommunicator Analyzer
        {
            set
            {
                m_objAnalyzer = value;
            }
        }

        private RFECommunicator m_objGenerator = null;
        public RFECommunicator Generator
        {
            set
            {
                m_objGenerator = value;
            }
        }

        private RemoteScreenControl m_controlRemoteScreen = null;
        /// <summary>
        /// Set Control Remote object to use internally
        /// </summary>
        public RemoteScreenControl ControlRemoteScreen
        {
            set
            {
                m_controlRemoteScreen = value;
            }
        }

        /// <summary>
        /// Get or Set to check to enable remote screen
        /// </summary>
        public bool CaptureDumpScreen
        {
            get
            {
                return m_chkDumpScreen.Checked;
            }
            set
            {
                m_chkDumpScreen.Checked = value;
            }
        }

        /// <summary>
        /// Get check header text on screen
        /// </summary>
        public bool DumpHeaderEnabled
        {
            get
            {
                return m_chkDumpHeader.Checked;
            }
        }

        /// <summary>
        /// Get or Set sweep index value
        /// </summary>
        public int ScreenIndex
        {
            get
            {
                return (int)m_numScreenIndex.Value;
            }
            set
            {
                m_numScreenIndex.Value = value;
            }
        }

        /// <summary>
        /// Get or Set Zoom of Screen
        /// </summary>
        public int ScreenZoom
        {
            get
            {
                return (int)m_numericZoom.Value;
            }
            set
            {
                m_numericZoom.Value = value;
            }
        }
        #endregion

        #region Constructor

        public ToolGroupRemoteScreen()
        {
            InitializeComponent();
            ResetComponents();
        }
        #endregion

        #region Public Events

        /// <summary>
        /// This events appears when zoom is change
        /// </summary>
        public event EventHandler NumeriZoomChangeEvent; 
        private void OnNumeriZoomChangeEvent(EventArgs eventArgs)
        {
            if (NumeriZoomChangeEvent != null)
            {
                NumeriZoomChangeEvent(this, eventArgs);
            }
        }

        /// <summary>
        /// This events appears when remote screen is enable/disable
        /// </summary>
        public event EventHandler ChkDumpScreenChangeEvent;
        private void OnChkDumpScreenChangeEvent(EventArgs eventArgs)
        {
            if (ChkDumpScreenChangeEvent != null)
            {
                ChkDumpScreenChangeEvent(this, eventArgs);
            }
        }

        /// <summary>
        /// This events appears when header text is changed
        /// </summary>
        public event EventHandler ChkDumpHeaderChangeEvent;
        private void OnchkDumpHeaderChangeEvent(EventArgs eventArgs)
        {
            if (ChkDumpHeaderChangeEvent != null)
            {
                ChkDumpHeaderChangeEvent(this, eventArgs);
            }
        }

        /// <summary>
        /// This events appears when save image button is clicked
        /// </summary>
        public event EventHandler SaveImageChangeEvent;
        private void OnSaveImageChangeEvent(EventArgs eventArgs)
        {
            if (SaveImageChangeEvent != null)
            {
                SaveImageChangeEvent(this, eventArgs);
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Update initial internal status and layout of controls
        /// </summary>
        public void UpdateUniversalLayout(Size ReferenceControlSize)
        {
            try
            {
                m_groupControl_RemoteScreen.m_ContainerFormRemoteScreen = this;
                m_groupControl_RemoteScreen.SetUniversalLayout(ReferenceControlSize);
            }
            catch { } //TODO: change in a future
        }

        /// <summary>
        /// Update status of all internal buttons based on analyzer object status
        /// </summary>
        /// <param name="menuAutoLCDOffChecked">Whether LCD screen of the device is on(true) or off(false).</param>
        public void UpdateButtonStatus(bool menuAutoLCDOffChecked)
        {
            if (m_controlRemoteScreen != null)
            {
                m_chkDumpLCDGrid.Checked = m_controlRemoteScreen.LCDGrid;
                m_chkDumpColorScreen.Checked = m_controlRemoteScreen.LCDColor;
                m_chkDumpHeader.Checked = m_controlRemoteScreen.HeaderText;
            }

            RFECommunicator objRFE = GetConnectedDeviceByPrecedence();
            if ( objRFE != null)
            {
                CaptureDumpScreen = CaptureDumpScreen && !menuAutoLCDOffChecked && (objRFE.PortConnected);
                m_chkDumpScreen.Enabled = !menuAutoLCDOffChecked && (objRFE.PortConnected);
                m_btnSaveRemoteBitmap.Enabled = objRFE.ScreenData.Count > 1;

                m_chkDumpColorScreen.Enabled = m_chkDumpScreen.Enabled;
                m_chkDumpLCDGrid.Enabled = m_chkDumpScreen.Enabled;
                m_chkDumpHeader.Enabled = m_chkDumpScreen.Enabled;
            }
        }

        /// <summary>
        /// Get analyzer or generator object deppending which one is connected. Analyzer has preference
        /// </summary>
        /// <returns>Analyzer or Generator object</returns>
        public RFECommunicator GetConnectedDeviceByPrecedence()
        {
            RFECommunicator objRFE = m_objAnalyzer;

            if (m_objGenerator != null && m_objGenerator.PortConnected && m_objAnalyzer != null && !m_objAnalyzer.PortConnected)
            {
                objRFE = m_objGenerator;
            }

            if ((objRFE != null) && (m_controlRemoteScreen.RFExplorer != objRFE))
            {
                objRFE.CleanScreenData();
            }

            return objRFE;
        }

        /// <summary>
        /// Set remote screen bitmap size 
        /// </summary>
        public void SetBitmapSizeLabel()
        {
            if (m_controlRemoteScreen != null)
                m_labBitmapSize.Text = "Size:" + (m_controlRemoteScreen.Width - 2) + "x" + (m_controlRemoteScreen.Height - 2);
        }

        /// <summary>
        /// Update values for all internal sweep numeric controls based on analyzer data
        /// </summary>
        public void UpdateNumericControls()
        {
            RFECommunicator objRFE = GetConnectedDeviceByPrecedence();
            if (objRFE == null)
                return; 

            if (objRFE.ScreenData.Count != 0 || objRFE.PortConnected)
            {
                //update screen data
                if (objRFE.ScreenData.Count < m_numScreenIndex.Value)
                {
                    //This is to avoid exception due to Maximum allowed value
                    m_numScreenIndex.Value = objRFE.ScreenData.Count;
                }
                m_numScreenIndex.Maximum = objRFE.ScreenData.Count - 1;
                m_numScreenIndex.Value = objRFE.ScreenData.Count - 1;
            }
            else
            {
                m_numScreenIndex.Value = 0;
                m_numScreenIndex.Minimum = 0;
                m_numScreenIndex.Maximum = 0;
            }
        }

        /// <summary>
        /// Restore ScreenIndex and Enable checkbox of Toolgroup like start of application
        /// </summary>
        public void ResetComponents()
        {
            CaptureDumpScreen = false;
            UpdateNumericControls();
        }

        #endregion

        #region Private Events and methods

        private void OnNumScreenIndex_ValueChanged(object sender, EventArgs e)
        {
            RFECommunicator objRFE = m_controlRemoteScreen.RFExplorer;
            if (objRFE == null)
                return;

            objRFE.ScreenIndex = (UInt16)m_numScreenIndex.Value;
            m_numScreenIndex.Value = objRFE.ScreenIndex;
            if (objRFE.ScreenData.Count > 0)
            {
                m_controlRemoteScreen.Invalidate();
            }
        }

        private void OnNumericZoom_ValueChanged(object sender, EventArgs e)
        {
            OnNumeriZoomChangeEvent(new EventArgs());
        }

        private void OnDumpScreen_CheckedChanged(object sender, EventArgs e)
        {
            RFECommunicator objRFE = m_controlRemoteScreen.RFExplorer;
            if (objRFE == null)
                return; 

            objRFE.CaptureRemoteScreen = CaptureDumpScreen;
            
            OnChkDumpScreenChangeEvent(new EventArgs());

            if (CaptureDumpScreen)
            {
                objRFE.SendCommand_EnableScreenDump();
            }
            else
            {
                objRFE.SendCommand_DisableScreenDump();
            }
        }

        private void OnDumpColorScreen_CheckedChanged(object sender, EventArgs e)
        {
            m_controlRemoteScreen.LCDColor = m_chkDumpColorScreen.Checked;
            m_controlRemoteScreen.Invalidate();
        }

        private void OnDumpHeader_CheckedChanged(object sender, EventArgs e)
        {
            m_controlRemoteScreen.HeaderText = m_chkDumpHeader.Checked;

            OnchkDumpHeaderChangeEvent(new EventArgs());
        }

        private void OnDumpLCDGrid_CheckedChanged(object sender, EventArgs e)
        {
            m_controlRemoteScreen.LCDGrid = m_chkDumpLCDGrid.Checked;
            m_controlRemoteScreen.Invalidate();
        }

        private void OnSaveImage_Click(object sender, EventArgs e)
        {
            OnSaveImageChangeEvent(new EventArgs());        
        }
        #endregion
    }

    public class GroupControl_RemoteScreen : GroupBox
    {
        internal ToolGroupRemoteScreen m_ContainerFormRemoteScreen = null;
        internal CollapsibleGroupbox m_CollGroupBoxRemoteScreen = null;

        /// <summary>
        /// Defines layout of the components regardless their prior position
        /// </summary>
        public void SetUniversalLayout(Size ReferenceControlSize)
        {
            if (m_ContainerFormRemoteScreen == null)
                return;

            this.AutoSize = true;

            m_ContainerFormRemoteScreen.m_numScreenIndex.MinimumSize = ReferenceControlSize;
            m_ContainerFormRemoteScreen.m_numericZoom.MinimumSize = m_ContainerFormRemoteScreen.m_numScreenIndex.Size;
            m_ContainerFormRemoteScreen.m_btnSaveRemoteBitmap.MinimumSize = m_ContainerFormRemoteScreen.m_numScreenIndex.Size;

            m_ContainerFormRemoteScreen.m_btnSaveRemoteBitmap.Top = m_ContainerFormRemoteScreen.m_chkDumpScreen.Bottom + 2;
            m_ContainerFormRemoteScreen.m_numScreenIndex.Top = m_ContainerFormRemoteScreen.m_chkDumpScreen.Bottom + 2;
            m_ContainerFormRemoteScreen.m_labRemoteScreenSample.Top = m_ContainerFormRemoteScreen.m_chkDumpScreen.Bottom + 4;

            m_ContainerFormRemoteScreen.m_numericZoom.Top = m_ContainerFormRemoteScreen.m_btnSaveRemoteBitmap.Bottom + 4;
            m_ContainerFormRemoteScreen.m_labRemoteScreenZoom.Top = m_ContainerFormRemoteScreen.m_btnSaveRemoteBitmap.Bottom + 4;
            m_ContainerFormRemoteScreen.m_labBitmapSize.Top = m_ContainerFormRemoteScreen.m_btnSaveRemoteBitmap.Bottom + 4;

            int nLeftPos = m_ContainerFormRemoteScreen.m_labRemoteScreenSample.Right + 2;
            m_ContainerFormRemoteScreen.m_numScreenIndex.Left = nLeftPos;
            m_ContainerFormRemoteScreen.m_numericZoom.Left = nLeftPos;

            int nRightPos = m_ContainerFormRemoteScreen.m_numScreenIndex.Right + 2;
            m_ContainerFormRemoteScreen.m_btnSaveRemoteBitmap.Left = nRightPos;
            m_ContainerFormRemoteScreen.m_labBitmapSize.Left = nRightPos;

            int nCheckPos = m_ContainerFormRemoteScreen.m_btnSaveRemoteBitmap.Right + 6;
            m_ContainerFormRemoteScreen.m_chkDumpHeader.Left = nCheckPos;
            m_ContainerFormRemoteScreen.m_chkDumpColorScreen.Left = nCheckPos;
            m_ContainerFormRemoteScreen.m_chkDumpLCDGrid.Left = nCheckPos;
            m_ContainerFormRemoteScreen.m_chkDumpColorScreen.Top = m_ContainerFormRemoteScreen.m_chkDumpHeader.Bottom + 3;
            m_ContainerFormRemoteScreen.m_chkDumpLCDGrid.Top = m_ContainerFormRemoteScreen.m_chkDumpColorScreen.Bottom + 3;

            this.MinimumSize = new Size(m_ContainerFormRemoteScreen.m_chkDumpLCDGrid.Right + 5, m_ContainerFormRemoteScreen.m_chkDumpScreen.Bottom + 2);

            if (m_CollGroupBoxRemoteScreen == null)
            {
                m_CollGroupBoxRemoteScreen = new CollapsibleGroupbox(this);
                m_CollGroupBoxRemoteScreen.CollapsedCaption = "REMOTE SCREEN"; 
                CollapseBtn_Click(null, null); //to update status first time
                this.Paint += new System.Windows.Forms.PaintEventHandler(this.Collapse_Paint);
            }
        }

        private void CollapseBtn_Click(object sender, EventArgs e)
        {
            m_ContainerFormRemoteScreen.MinimumSize = MinimumSize;
            m_ContainerFormRemoteScreen.MaximumSize = MaximumSize;
        }

        private void Collapse_Paint(object sender, PaintEventArgs e)
        {
            m_CollGroupBoxRemoteScreen.Paint(e.Graphics);
        }
    }
}
