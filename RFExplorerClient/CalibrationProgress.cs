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
    public partial class CalibrationProgress : Form
    {
        public CalibrationProgress()
        {
            InitializeComponent();
            textLine1.Text = "";
            textLine2.Text = "";
            textLine3.Text = "";
            ProgressBar.Value = 0;
        }
    }
}
