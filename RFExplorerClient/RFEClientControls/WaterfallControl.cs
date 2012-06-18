//============================================================================
//RF Explorer PC Client - A Handheld Spectrum Analyzer for everyone!
//Copyright © 2010-11 Ariel Rocholl, www.rf-explorer.com
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
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RFEClientControls
{
    public partial class WaterfallControl : UserControl
    {
        private int m_nZoom;                    //Zoom value 1-7

        //Graphis objects cached to reduce drawing overhead
        Brush m_BrushBackground;
        Pen m_PenDarkBlue;
        Pen m_PenRed;
        Brush m_BrushDarkBlue;
        Rectangle m_AdjustedClientRect;
        QueueSet<Dictionary<double, double>> lastValues;
        int m_contrast;
        int m_sensitivity;

        public WaterfallControl()
        {
            InitializeComponent();
            lastValues = new QueueSet<Dictionary<double, double>>(450);
            m_nZoom = 7;
            m_contrast = 215;
            m_sensitivity = 100;
        }

        public void UpdateZoom(int nNewZoom)
        {
            //TODO: Actually support zooming etc.
            m_nZoom = nNewZoom;
            m_AdjustedClientRect = ClientRectangle;
            m_AdjustedClientRect.Height = m_AdjustedClientRect.Height - 2;
            m_AdjustedClientRect.Width = m_AdjustedClientRect.Width - 2;
        }

        public void UpdateContrast(int newContrast)
        {
            m_contrast = newContrast;
        }

        public void UpdateSensitivity(int newSensitivity)
        {
            m_sensitivity = newSensitivity;
        }
        
        public void DrawWaterfall(Dictionary<double, double> values)
        {
            //Add incoming data to our waterfall history
            lastValues.Enqueue(values);
        }

        public void ClearWaterfall()
        {
            lastValues.Clear();
        }

        private void WaterfallControl_Load(object sender, EventArgs e)
        {
            m_PenDarkBlue = new Pen(Color.DarkBlue, 1);
            m_PenRed = new Pen(Color.Red, 1);
            m_BrushDarkBlue = new SolidBrush(Color.DarkBlue);
            m_BrushBackground = new SolidBrush(Color.Black);
        }

        private void WaterfallControl_Paint(object sender, PaintEventArgs e)
        {
            if (e == null)
                return;

            e.Graphics.FillRectangle(m_BrushBackground, m_AdjustedClientRect);
            e.Graphics.DrawRectangle(m_PenDarkBlue, m_AdjustedClientRect);
            int y = 0;
            int widthFactor = 8; //Defines how wide each pixel of the waterfall display is
            //Copy the current waterfall data queue into a snapshot array
            Dictionary<double, double>[] snapshotValues = new Dictionary<double, double>[lastValues.Count];
            lastValues.CopyTo(snapshotValues, 0);
            //Loop through the snapshot of the waterfall history and draw each line
            foreach (Dictionary<double, double> currentValues in snapshotValues)
            {
                int x = 0;
                foreach (KeyValuePair<double, double> kvp in currentValues)
                {
                    int r = 0;
                    int g = 0;
                    int b = 0;
                    int alpha = (int)(((float)m_sensitivity + (float)kvp.Value) / (255.0 - (float)m_contrast) * 255);

                    alpha = Math.Min(Math.Max(alpha, 0), 255);

                    // Heatmap coloring code from http://www.patrick-wied.at/blog/real-time-heatmap-explained
                    // Licensed under MIT-License (http://www.opensource.org/licenses/mit-license.php)
                    if (alpha <= 255 && alpha >= 235)
                    {
                        int tmp = 255 - alpha;
                        r = 255 - tmp;
                        g = tmp * 12;
                    }
                    else if (alpha <= 234 && alpha >= 200)
                    {
                        int tmp = 234 - alpha;
                        r = 255 - (tmp * 8);
                        g = 255;
                    }
                    else if (alpha <= 199 && alpha >= 150)
                    {
                        int tmp = 199 - alpha;
                        g = 255;
                        b = tmp * 5;
                    }
                    else if (alpha <= 149 && alpha >= 100)
                    {
                        int tmp = 149 - alpha;
                        g = 255 - (tmp * 5);
                        b = 255;
                    }
                    else
                        b = 255;

                    r = Math.Min(Math.Max(r, 0), 255);
                    g = Math.Min(Math.Max(g, 0), 255);
                    b = Math.Min(Math.Max(b, 0), 255);

                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(alpha, r, g, b)), x + 5, y, widthFactor, 1);
                    x += widthFactor;
                }
                y += 1;
            }
        }
    }

    // Queue with fixed maximum size
    class QueueSet<T> : ICollection<T>
    {
        List<T> queue = new List<T>();
        int maximumSize;

        public QueueSet(int maximumSize)
        {
            if (maximumSize < 0)
                throw new ArgumentOutOfRangeException("maximumSize");
            this.maximumSize = maximumSize;
        }

        public T Dequeue()
        {
            if (queue.Count > 0)
            {
                T value = queue[0];
                queue.RemoveAt(0);
                return value;
            }
            return default(T);
        }

        public T Peek()
        {
            if (queue.Count > 0)
            {
                return queue[0];
            }
            return default(T);
        }

        public void Enqueue(T item)
        {
            if (queue.Contains(item))
            {
                queue.Remove(item);
            }
            queue.Add(item);
            while (queue.Count > maximumSize)
            {
                Dequeue();
            }
        }

        public int Count
        {
            get
            {
                return queue.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void Add(T item)
        {
            Enqueue(item);
        }

        public void Clear()
        {
            queue.Clear();
        }

        public bool Contains(T item)
        {
            return queue.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            foreach (T value in queue)
            {
                if (arrayIndex >= array.Length) break;
                if (arrayIndex >= 0)
                {
                    array[arrayIndex] = value;
                }
                arrayIndex++;
            }
        }

        public bool Remove(T item)
        {
            if (Object.Equals(item, Peek()))
            {
                Dequeue();
                return true;
            }
            else
            {
                return false;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return queue.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return queue.GetEnumerator();
        }
    }


}
