using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;

namespace TestZed
{
    public partial class Form1 : Form
    {
        ZedGraphControl m_graph;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            m_graph = new ZedGraphControl();
            m_graph.Location = new Point(0, 0);
            m_graph.Parent = this;
            m_graph.Dock = DockStyle.Fill;
            CreateChart(m_graph);
        }

        public void CreateChart(ZedGraphControl zgc)
        {
            GraphPane myPane = zgc.GraphPane;

            // Set the titles and axis labels
            myPane.Title.Text = "My Test Date Graph";
            myPane.XAxis.Title.Text = "Date";
            myPane.YAxis.Title.Text = "My Y Axis";

            // Make up some data points from the Sine function
            PointPairList list = new PointPairList();
            PointPairList list2 = new PointPairList();
            for (int i = 0; i < 36; i++)
            {
                double x = new XDate(1995, i + 1, 1);
                double y = Math.Sin((double)i * Math.PI / 15.0);
                double y2 = 2 * y;

                list.Add(x, y);
                list2.Add(x, y2);
            }

            // Generate a blue curve with circle symbols, and "My Curve 2" in the legend
            LineItem myCurve2 = myPane.AddCurve("My Curve 2", list, Color.Blue,
                              SymbolType.Circle);
            // Fill the area under the curve with a white-red gradient at 45 degrees
            myCurve2.Line.Fill = new Fill(Color.Red, Color.Pink, 90F);
            myCurve2.Line.FillFromBottom = true;
            // Make the symbols opaque by filling them with white
            myCurve2.Symbol.Fill = new Fill(Color.White);

            // Generate a red curve with diamond symbols, and "My Curve" in the legend
            LineItem myCurve = myPane.AddCurve("My Curve",
               list2, Color.MediumVioletRed, SymbolType.Diamond);
            // Fill the area under the curve with a white-green gradient
            //myCurve.Line.Fill = new Fill(Color.White, Color.Green);
            // Make the symbols opaque by filling them with white
            myCurve.Symbol.Fill = new Fill(Color.White);

            // Set the XAxis to date type
            myPane.XAxis.Type = AxisType.Date;

            // Fill the axis background with a color gradient
            myPane.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45F);

            // Calculate the Axis Scale Ranges
            zgc.AxisChange();
        }
    }
}
