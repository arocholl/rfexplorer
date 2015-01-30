//============================================================================
//RF Explorer for Windows - A Handheld Spectrum Analyzer for everyone!
//Copyright © 2010-15 Ariel Rocholl, www.rf-explorer.com
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RFExplorerCommunicator;

namespace RFEClientControls
{
    public partial class RFModuleSelector : UserControl
    {
        double m_fOriginalWidth = 0;
        double m_fOriginalHeight = 0;

        int m_nActualPictureWidth = 0;
        public int ActualPictureWidth
        {
            get { return m_nActualPictureWidth; }
            set { m_nActualPictureWidth = value; }
        }
        int m_nActualPictureHeight = 0;
        public int ActualPictureHeight
        {
            get { return m_nActualPictureHeight; }
            set { m_nActualPictureHeight = value; }
        }

        public bool AllowHideControl
        {
            set
            {
                if (value == true)
                {
                    this.ContextMenuStrip = menuContextDevice;
                }
                else
                {
                    this.ContextMenuStrip = null;
                }
            }
        }

        private RFExplorerCommunicator.RFECommunicator m_objRFE = null;
        public RFExplorerCommunicator.RFECommunicator RFExplorer
        {
            set { m_objRFE = value; }
        }

        public RFModuleSelector()
        {
            InitializeComponent();

            m_fOriginalHeight = pictureMain.Image.Height;
            m_fOriginalWidth = pictureMain.Image.Width;
        }

        private void radioLeftModule_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioRightModule_CheckedChanged(object sender, EventArgs e)
        {

        }

        public void SelectImageFromConfiguration()
        {
            if (m_objRFE==null)
                return;

            if (m_objRFE.PortConnected == false)
            {
                pictureMain.Image = global::RFEClientControls.Properties.Resources.RFExplorer_WhipLeft_Disconnected;
                return;
            }

            switch (m_objRFE.MainBoardModel)
            {
                default:
                    {
                        switch (m_objRFE.ExpansionBoardModel)
                        {
                            default:
                                if (m_objRFE.ExpansionBoardActive)
                                {
                                    pictureMain.Image = global::RFEClientControls.Properties.Resources.RFE_2AWLWR_AR;
                                }
                                else
                                {
                                    pictureMain.Image = global::RFEClientControls.Properties.Resources.RFE_2AWLWR_AL;
                                }
                                break;
                            case RFECommunicator.eModel.MODEL_NONE:
                                pictureMain.Image = global::RFEClientControls.Properties.Resources.RFE_1AWL;
                                break;
                            case RFECommunicator.eModel.MODEL_WSUB1G:
                            case RFECommunicator.eModel.MODEL_WSUB3G:
                                if (m_objRFE.ExpansionBoardActive)
                                {
                                    pictureMain.Image = global::RFEClientControls.Properties.Resources.RFE_2AWLWR_AR;
                                }
                                else
                                {
                                    pictureMain.Image = global::RFEClientControls.Properties.Resources.RFE_2AWLWR_AL;
                                }
                                break;
                        }
                        break;
                    }
                case RFECommunicator.eModel.MODEL_WSUB1G:
                    {
                        switch (m_objRFE.ExpansionBoardModel)
                        {
                            default:
                                if (m_objRFE.ExpansionBoardActive)
                                {
                                    pictureMain.Image = global::RFEClientControls.Properties.Resources.RFE_2ANLWR_AR;
                                }
                                else
                                {
                                    pictureMain.Image = global::RFEClientControls.Properties.Resources.RFE_2ANLWR_AL;
                                }
                                break;
                            case RFECommunicator.eModel.MODEL_NONE:
                                pictureMain.Image = global::RFEClientControls.Properties.Resources.RFE_1ANL;
                                break;
                            case RFECommunicator.eModel.MODEL_WSUB1G:
                            case RFECommunicator.eModel.MODEL_WSUB3G:
                                if (m_objRFE.ExpansionBoardActive)
                                {
                                    pictureMain.Image = global::RFEClientControls.Properties.Resources.RFE_2ANLWR_AR;
                                }
                                else
                                {
                                    pictureMain.Image = global::RFEClientControls.Properties.Resources.RFE_2ANLWR_AL;
                                }
                                break;
                        }
                        break;
                    }
            }
        }

        private void RFModuleSelector_Load(object sender, EventArgs e)
        {
            SelectImageFromConfiguration();
        }

        private void RFModuleSelector_SizeChanged(object sender, EventArgs e)
        {
            if (pictureMain.Image.Height == 0)
                return;

            if (m_fOriginalHeight == 0.0)
                return;

            double fOriginalAspectRatio = m_fOriginalWidth / m_fOriginalHeight;

            double fNewContainerAspectRatio = (double)Width / (double)Height;

            if (fNewContainerAspectRatio > fOriginalAspectRatio)
            {
                //dimension X now is larger than Y
                pictureMain.Height = Height;
                pictureMain.Width = (int)(Height * fOriginalAspectRatio);
            }
            else
            {
                //dimension Y now is equal or larger than X
                pictureMain.Width = Width;
                pictureMain.Height = (int)(Width / fOriginalAspectRatio);
            }

            ActualPictureHeight = pictureMain.Height;
            ActualPictureWidth = pictureMain.Width;
        }

        private void RFModuleSelector_Click(object sender, EventArgs e)
        {
            if (m_objRFE == null)
                return;

            MessageBox.Show(m_objRFE.FullModelText);
        }

        //Notify the container the control wants to hide
        private void menuHide_Click(object sender, EventArgs e)
        {
            OnHideControl(new EventArgs());
        }

        public event EventHandler HideControl;
        private void OnHideControl(EventArgs eventArgs)
        {
            if (HideControl != null)
            {
                HideControl(this, eventArgs);
            }
        }
    }
}
