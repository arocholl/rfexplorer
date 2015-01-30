using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RFExplorerCommunicator
{
    public partial class ToolGroupCOMPort : UserControl
    {
        private RFECommunicator m_objRFE;        //Reference to the running communicator, it contains data and status
        public RFECommunicator RFExplorer
        {
            get { return m_objRFE; }
            set 
                { 
                    m_objRFE = value;
                    UpdateButtonStatus();
                }
        }

        public string GroupBoxTitle
        {
            set
            {
                m_groupControl_Connection.Text = value;
            }
        }

        private string m_sDefaultCOMSpeed;       //RFExplorerClient.Properties.Settings.Default.COMSpeed or equivalent
        public string DefaultCOMSpeed
        {
            set 
                { 
                    m_sDefaultCOMSpeed = value;
                    m_ComboBaudRate.SelectedItem = m_sDefaultCOMSpeed;
                }
        }
        public bool IsCOMSpeedSelected
        {
            get { return m_ComboBaudRate.SelectedItem != null; }
        }
        public string COMSpeedSelected
        {
            get
                {
                    if (IsCOMSpeedSelected)
                        return m_ComboBaudRate.SelectedItem.ToString();
                    else
                        return "";
                }
        }

        private string m_sDefaultCOMPort;        //RFExplorerClient.Properties.Settings.Default.COMPort or equivalent
        public string DefaultCOMPort
        {
            set 
                { 
                    m_sDefaultCOMPort = value;
                    m_comboCOMPort.SelectedItem = m_sDefaultCOMPort;
                }
        }
        public bool IsCOMPortSelected
        {
            get { return m_comboCOMPort.Items.Count > 0 && m_comboCOMPort.SelectedValue!=null; }
        }
        public string COMPortSelected
        {
            get 
            { 
                if (IsCOMPortSelected)
                    return m_comboCOMPort.SelectedValue.ToString(); 
                else
                    return "";
            }
        }

        public ToolGroupCOMPort()
        {
            InitializeComponent();
            UpdateButtonStatus();
        }

        private void OnConnect_Click(object sender, EventArgs e)
        {
            ConnectPort();
        }

        private void OnDisconnect_Click(object sender, EventArgs e)
        {
            ClosePort();
        }

        public void ClosePort()
        {
            Cursor.Current = Cursors.WaitCursor;
            m_objRFE.ClosePort();
            UpdateButtonStatus();
            OnPortClosed(new EventArgs());
            Cursor.Current = Cursors.Default;
        }

        public event EventHandler PortClosed;
        private void OnPortClosed(EventArgs eventArgs)
        {
            if (PortClosed != null)
            {
                PortClosed(this, eventArgs);
            }
        }

        public void ConnectPort()
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                string csCOMPort = "";
                if (m_objRFE.ValidCP2101Ports != null && m_objRFE.ValidCP2101Ports.Length > 0)
                {
                    //there are valid ports available
                    if (m_objRFE.ValidCP2101Ports.Length == 1)
                    {
                        //if only one, ignore the selection from any combo and use what is available
                        csCOMPort = m_objRFE.ValidCP2101Ports[0];
                        m_sDefaultCOMPort = csCOMPort;
                        m_comboCOMPort.SelectedItem = m_sDefaultCOMPort;
                    }
                    else
                    {
                        //if more than one, try to use the one from the combo and otherwise fail
                        if ((m_comboCOMPort != null) && (m_comboCOMPort.Items.Count > 0) && (m_comboCOMPort.SelectedValue!=null))
                        {
                            foreach (string sTestCOMPort in m_objRFE.ValidCP2101Ports)
                            {
                                if (sTestCOMPort==m_comboCOMPort.SelectedValue.ToString())
                                {
                                    csCOMPort=m_comboCOMPort.SelectedValue.ToString();
                                    break;
                                }
                            }
                        }
                    }
                }

                if (!String.IsNullOrEmpty(csCOMPort))
                {
                    m_objRFE.ConnectPort(csCOMPort, Convert.ToInt32(m_ComboBaudRate.SelectedItem.ToString()));

                    m_objRFE.HoldMode = false;
                    UpdateButtonStatus();
                    OnPortConnected(new EventArgs());
                }
            }
            catch(Exception obEx)
            {
                Console.WriteLine(obEx.ToString());
            }

            Cursor.Current = Cursors.Default;
        }

        public event EventHandler PortConnected;
        private void OnPortConnected(EventArgs eventArgs)
        {
            if (PortConnected != null)
            {
                PortConnected(this, eventArgs);
            }
        }

        public void GetConnectedPorts()
        {
            Cursor.Current = Cursors.WaitCursor;
            m_comboCOMPort.DataSource = null;
            if (m_objRFE.GetConnectedPorts())
            {
                m_comboCOMPort.DataSource = m_objRFE.ValidCP2101Ports;
                m_comboCOMPort.SelectedItem = m_sDefaultCOMPort;
            }
            UpdateButtonStatus();
            Cursor.Current = Cursors.Default;
        }

        private void OnRescan_Click(object sender, EventArgs e)
        {
            GetConnectedPorts();
        }

        public void UpdateButtonStatus()
        {
            if (m_objRFE != null)
            {
                this.Enabled = true;
                m_btnRescan.Enabled = !m_objRFE.PortConnected;
                m_btnConnect.Enabled = !m_objRFE.PortConnected && (m_comboCOMPort.Items.Count > 0);
                m_btnDisconnect.Enabled = m_objRFE.PortConnected;
                m_comboCOMPort.Enabled = !m_objRFE.PortConnected;
                m_ComboBaudRate.Enabled = !m_objRFE.PortConnected;
            }
            else
                this.Enabled = false;
        }
    }
}
