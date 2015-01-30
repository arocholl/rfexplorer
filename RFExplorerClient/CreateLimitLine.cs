using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RFExplorerClient
{
    public partial class CreateLimitLine : Form
    {
        public CreateLimitLine()
        {
            InitializeComponent();
        }

        private void m_edOffsetDB_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(m_edOffsetDB.Text))
            {
                btnOK.Enabled = Convert.ToDouble(m_edOffsetDB.Text) >= 0.0f;
            }
            else
            {
                btnOK.Enabled = false;
            }
        }
    }
}
