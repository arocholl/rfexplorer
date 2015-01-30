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

namespace RFEClientControls
{
    public partial class PacketData : UserControl
    {
        public void AddLabel(string sText, string sName, int nCol, int nRow)
        {
            System.Windows.Forms.Label objLabel = new System.Windows.Forms.Label();

            objLabel.AutoSize = true;
            objLabel.Location = new System.Drawing.Point(3, 0);
            objLabel.Name = sName;
            objLabel.Size = new System.Drawing.Size(35, 13);
            objLabel.TabIndex = 0;
            objLabel.Text = sText;
            objLabel.TextAlign = ContentAlignment.MiddleCenter;
            objLabel.BackColor = Color.Transparent;
            objLabel.Dock = DockStyle.Fill;
            this.m_tablePanel.Controls.Add(objLabel, nCol, nRow);
        }

        public PacketData()
        {
            InitializeComponent();

            AddLabel("Type:", "Type", 0, 1);
            AddLabel("Samples:", "Samples", 0, 2);
            AddLabel("Sampling:", "Sampling", 0, 3);
            AddLabel("Time Span:", "TimeSpan", 0, 4);
        }
    }
}
