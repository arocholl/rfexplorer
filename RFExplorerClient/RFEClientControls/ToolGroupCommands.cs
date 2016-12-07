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
    public partial class ToolGroupCommands : UserControl
    {
        #region Properties
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
        /// Get or Set confirmation of detailed debug info
        /// </summary>
        public bool DebugTraces
        {
            get
            {
                return m_chkDebugTraces.Checked;
            }

            set
            {
                m_chkDebugTraces.Checked = value;
            }
        }

        /// <summary>
        /// Get the name associated with this control
        /// </summary>
        public string CustomCommandText
        {
            get
            {
                return m_comboCustomCommand.Text; ;
            }
        }

        /// <summary>
        /// Set a new Item into Custom ComboBox list
        /// </summary>
        public string CustomCommandAddItem
        {
            set
            {
                m_comboCustomCommand.Items.Add(value);
            }
        }

        /// <summary>
        /// Event for detailed debug info
        /// </summary>
        public event EventHandler DebugChangeEvent;
        private void OnDebugChangeEvent(EventArgs eventArgs)
        {
            if (DebugChangeEvent != null)
            {
                DebugChangeEvent(this, eventArgs);
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

        /// <summary>
        /// Use this event to store properties
        /// </summary>
        public event EventHandler CustomCommandEvent;
        private void OnCustomCommandEvent(EventArgs eventArgs)
        {
            if (CustomCommandEvent != null)
            {
                CustomCommandEvent(this, eventArgs);
            }
        }
        #endregion

        public ToolGroupCommands()
        {
            InitializeComponent();
        }

        #region Functions
        /// <summary>
        /// Update initial internal status and layout of controls
        /// </summary>
        public void UpdateUniversalLayout()
        {
            try
            {
                m_groupControl_Commands.m_ContainerForm = this;
                m_groupControl_Commands.SetUniversalLayout();
            }
            catch { }
        }

        public void UpdateButtonStatus()
        {
            if (m_objAnalyzer == null || m_objGenerator == null)
                return;

            m_btnSendAnalyzerCmd.Enabled = m_objAnalyzer.PortConnected;
            m_btnSendAnalyzerCustomCmd.Enabled = m_objAnalyzer.PortConnected;
            m_btnSendGenCmd.Enabled = m_objGenerator.PortConnected;
            m_btnSendGenCustomCmd.Enabled = m_objGenerator.PortConnected;
        }

        /// <summary>
        /// Send standard command to analyzer or generator
        /// </summary>
        /// <param name="bAnalyzer">true for analyzer, false for generator</param>
        private void SendStandardCommand(bool bAnalyzer)
        {
            try
            {
                string sCmd = "";
                if (m_comboStdCmd.Text.Length > 0)
                {
                    sCmd = m_comboStdCmd.Text;
                    sCmd = sCmd.Substring(sCmd.LastIndexOf(':') + 2);
                }

                if (sCmd.Length > 0)
                {
                    if (bAnalyzer)
                        m_objAnalyzer.SendCommand(sCmd);
                    else
                        m_objGenerator.SendCommand(sCmd);
                    ReportLog("Command sent: " + sCmd);
                }
                else
                {
                    MessageBox.Show("Nothing to send.\nSpecify a command first...");
                }
            }
            catch (Exception obEx)
            {
                ReportLog(obEx.ToString());
            }
        }

        /// <summary>
        /// Sends a custom command to analyzer or generator
        /// </summary>
        /// <param name="bAnalyzer">true for analyzer, false for generator</param>
        private void SendCustomCommand(bool bAnalyzer)
        {
            try
            {
                string sCmd = m_comboCustomCommand.Text;
                if (sCmd.Length > 0)
                {
                    if (m_comboCustomCommand.FindStringExact(sCmd) == -1)
                    {
                        m_comboCustomCommand.Items.Add(sCmd);
                        m_comboCustomCommand.Text = sCmd;

                        OnCustomCommandEvent(new EventArgs());
                    }

                    string sParsedCmd = "";
                    for (int nCharInd = 0; nCharInd < sCmd.Length; nCharInd++)
                    {
                        if (('\\' == sCmd[nCharInd]) && ('x' == sCmd[nCharInd + 1]))
                        {
                            //An hex byte value is coming
                            string sHexVal = "";
                            sHexVal += (char)sCmd[nCharInd + 2];
                            sHexVal += (char)sCmd[nCharInd + 3];
                            byte nVal = Convert.ToByte(sHexVal, 16);
                            sParsedCmd += Convert.ToChar(nVal);
                            nCharInd += 3;
                        }
                        else
                        {
                            //Normal text is coming
                            sParsedCmd += sCmd[nCharInd];
                        }
                    }
                    if (bAnalyzer)
                        m_objAnalyzer.SendCommand(sParsedCmd);
                    else
                        m_objGenerator.SendCommand(sParsedCmd);
                    ReportLog("Command sent: " + sCmd);
                }
                else
                {
                    MessageBox.Show("Nothing to send.\nSpecify a command first...");
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

        private void OnDebug_CheckedChanged(object sender, EventArgs e)
        {
            OnDebugChangeEvent(new EventArgs());

            if (m_objAnalyzer != null)
            {
                m_objAnalyzer.DebugTracesEnabled = m_chkDebugTraces.Checked;
                m_objAnalyzer.ShowDetailedCOMPortInfo = m_chkDebugTraces.Checked;
            }
        }

        private void OnSendAnalyzerCmd_Click(object sender, EventArgs e)
        {
            SendStandardCommand(true);
        }

        private void OnSendGenCmd_Click(object sender, EventArgs e)
        {
            SendStandardCommand(false);
        }

        private void OnSendAnalyzerCustomCmd_Click(object sender, EventArgs e)
        {
            SendCustomCommand(true);
        }

        private void OnSendCustomGenCmd_Click(object sender, EventArgs e)
        {
            SendCustomCommand(false);
        }
        #endregion
    }

    public class GroupControl_Commands : GroupBox
    {
        const int BORDER_MARGIN = 4;
        internal ToolGroupCommands m_ContainerForm = null;
        internal CollapsibleGroupbox m_CollGroupBoxCommands = null;

        /// <summary>
        /// Defines layout of the components regardless their prior position
        /// </summary>
        public void SetUniversalLayout()
        {
            const int BORDER_MARGIN = 4;
            if (m_ContainerForm == null)
                return;

            if (Parent.Parent == null)
                return;

            if (Parent.Height > Parent.Parent.Height)
            {
                Parent.MinimumSize = new Size(this.Width + 1, Parent.Parent.Height - 1);
                Parent.MaximumSize = new Size(this.Width + 2, Parent.Parent.Height);
                Parent.Height = Parent.Parent.Height;
            }
            this.MinimumSize = new Size(this.Width, this.Height);
            this.MaximumSize = new Size(this.Width, this.Height);

            m_ContainerForm.m_comboStdCmd.Left = m_ContainerForm.m_label12.Right + BORDER_MARGIN;
            m_ContainerForm.m_comboCustomCommand.Left = m_ContainerForm.m_label12.Right + BORDER_MARGIN;
            m_ContainerForm.m_btnSendAnalyzerCmd.Left = m_ContainerForm.m_comboCustomCommand.Right + 8 ;
            m_ContainerForm.m_btnSendAnalyzerCustomCmd.Left = m_ContainerForm.m_comboCustomCommand.Right + 8;
            m_ContainerForm.m_btnSendGenCmd.Left = m_ContainerForm.m_btnSendAnalyzerCmd.Right + BORDER_MARGIN;
            m_ContainerForm.m_btnSendGenCustomCmd.Left = m_ContainerForm.m_btnSendAnalyzerCmd.Right + BORDER_MARGIN;

            if (m_CollGroupBoxCommands == null)
            {
                m_CollGroupBoxCommands = new CollapsibleGroupbox(this);
                CollapseBtn_Click(null, null); //to update status first time
                this.Paint += new PaintEventHandler(this.Collapse_Paint);
                m_CollGroupBoxCommands.CollapseButtonClick += new EventHandler(CollapseBtn_Click);
                m_CollGroupBoxCommands.CollapsedCaption = "Commands";
            }
        }

        private void CollapseBtn_Click(object sender, EventArgs e)
        {
            m_ContainerForm.MinimumSize = MinimumSize;
            m_ContainerForm.MaximumSize = MaximumSize;
        }

        private void Collapse_Paint(object sender, PaintEventArgs e)
        {
            m_CollGroupBoxCommands.Paint(e.Graphics);
        }
    }
}
