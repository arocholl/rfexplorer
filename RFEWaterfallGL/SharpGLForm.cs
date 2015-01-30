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

//#define TESTMODE 
//#define KEYCAPTURE

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SharpGL;
using System.Runtime.InteropServices;
using SharpGL.Enumerations;
using RFExplorerCommunicator;

namespace RFEWaterfallGL
{
    /// <summary>
    /// The main form class.
    /// </summary>
    public partial class SharpGLForm : UserControl
    {
        const UInt16 m_nTotalSweeps = 100;
        const UInt16 m_nSweepSteps = 112;
        const UInt16 m_nAmplitudeSteps = 500;

        const float m_fX_scale = (float)(1.0 / m_nSweepSteps);
        const float m_fY_scale = (float)(1.0 / m_nAmplitudeSteps);
        const float m_fZ_Scale = (float)(1.0 / m_nTotalSweeps);

        const float m_fX_max = (float)(-1.0 + m_fX_scale);
        const float m_fY_max = (float)(1.0 - m_fY_scale);
        const float m_fZ_max = (float)(1.0 - m_fZ_Scale);

        public static UInt16 TotalDrawingSweeps
        {
            get { return SharpGLForm.m_nTotalSweeps; }
            //set { SharpGLForm.m_nTotalSweeps = value; } //not supported yet
        }

        private bool m_bDarkMode = true;
        public bool DarkMode
        {
            get { return m_bDarkMode; }
            set {
                m_bDarkMode = value;
                if (m_ImageLogo != null)
                {
                    m_ImageLogo.BackColor = Color.Transparent;
                    if (m_bDarkMode)
                    {
                        m_ImageLogo.Image = global::RFEWaterfallGL.Properties.Resources.LogoBlackShadow;
                    }
                    else
                    {
                        m_ImageLogo.Image = global::RFEWaterfallGL.Properties.Resources.LogoBlueShadow;
                    }
                }
            }
        }

        bool m_bDisplayFloor = true;
        public bool DisplayFloor
        {
            get { return m_bDisplayFloor; }
            set { m_bDisplayFloor = value; }
        }

        bool m_bDrawFloor = true;
        public bool DrawFloor
        {
            get { return m_bDrawFloor; }
            set { m_bDrawFloor = value; }
        }

        bool m_bDrawTitle = false;
        public bool DrawTitle
        {
            get { return m_bDrawTitle; }
            set { m_bDrawTitle = value; }
        }

        PictureBox m_ImageLogo;

        double m_fMinFrecuency, m_fMaxFrecuency;
        double m_fMinAmplitude = RFECommunicator.MIN_AMPLITUDE_DBM;
        double m_fMaxAmplitude = RFECommunicator.MAX_AMPLITUDE_DBM;
        bool m_bRangeSet = false;
        /// no matter what input data we have, we map it into 100x100 float array
        /// this is what we represent always
        float[,] m_arrSweepData = new float[m_nSweepSteps, m_nTotalSweeps];
        DateTime[] m_arrTimeData = new DateTime[m_nTotalSweeps];
        //The collection of data to be used for drawing
        RFESweepDataCollection m_SweepCollection = null;
        public RFESweepDataCollection SweepCollection
        {
            set { m_SweepCollection = value; }
        }

        bool m_bTransparent;    //True if waterfall curves are transparent
        /// <summary>
        /// Transparent curves
        /// </summary>
        public bool Transparent
        {
            get { return m_bTransparent; }
            set { m_bTransparent = value; }
        }

        public OpenGLControl InternalOpenGLControl
        {
            get { return openGLControl; }
        }

        //check whether OpenGL has initialized without incidents, otherwise OpenGL may not be supported
        bool m_bOpenGL_Ok = false;
        public bool OpenGL_Supported
        {
            get { return m_bOpenGL_Ok; }
        }
        //Description string of the exception when trying to initialize OpenGL
        string m_sInitializationError = "";
        public string InitializationError
        {
            get { return m_sInitializationError; }
        }

        //Define FPS as visible text (usually only for debug)
        public bool DrawFPS
        {
            get
            {
                if (openGLControl != null)
                    return openGLControl.DrawFPS;
                else
                    return false;
            }
            set 
            {
                if (openGLControl != null)
                    openGLControl.DrawFPS = value;
            }
        }

        const string m_sFontName = "Arial";     //Default font name, may be configurable some day

        public enum WaterfallPerspectives { Perspective1, Perspective2, ISO, Pers_2D};
#if KEYCAPTURE
        // playing with point of view
        double eyex, eyey, eyez;
        double centerx, centery, centerz;
        [DllImport("user32.dll")]        private static extern short GetAsyncKeyState(Keys key);
#endif
        private WaterfallPerspectives CurrentPerspective;

        public WaterfallPerspectives PerspectiveView
        {
            get { return CurrentPerspective; }
            set { CurrentPerspective = value;
                  SetPerspective();
            }
        }

        /// <summary>
        /// Sets all internal stored values to min amplitude
        /// </summary>
        public void CleanAll()
        {
            if (m_bRangeSet)
            {
                for (UInt16 nIndX = 0; nIndX < m_nSweepSteps; nIndX++)
                {
                    for (UInt16 nIndY = 0; nIndY < m_nTotalSweeps; nIndY++)
                    {
                        AddValue(nIndX, nIndY, m_fMinAmplitude);
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SharpGLForm"/> class.
        /// </summary>
        public SharpGLForm()
        {
            try
            {
                CurrentPerspective = WaterfallPerspectives.Perspective1;
                InitializeComponent();

                m_ImageLogo = new PictureBox();
                m_ImageLogo.BackColor = System.Drawing.Color.Transparent;
                m_ImageLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
                m_ImageLogo.Image = global::RFEWaterfallGL.Properties.Resources.rfexplorer_logo;
                m_ImageLogo.Location = new System.Drawing.Point(10, 10);
                m_ImageLogo.Name = "RFExplorer_logo";
                m_ImageLogo.Size = new System.Drawing.Size(m_ImageLogo.Image.Width, m_ImageLogo.Image.Height);
                m_ImageLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
                m_ImageLogo.TabIndex = 0;
                m_ImageLogo.TabStop = false;
                m_ImageLogo.Visible = false;
                m_ImageLogo.Enabled = true;
                m_ImageLogo.BorderStyle = BorderStyle.None;
                openGLControl.Controls.Add(m_ImageLogo);

                m_bTransparent = true;
                m_bOpenGL_Ok = true;
            }
            catch (Exception objEx)
            {
                m_sInitializationError = objEx.ToString();

                Controls.Remove(openGLControl);
                openGLControl = null;
            }
#if TESTMODE
            /// Create random array
            Random rand = new Random(1);// known seed
            int x, z;
            for(x=0;x<nSteps;x++)
                for (z = 0; z < nSteps; z++)
                {
                    fData[x, z] = (float)rand.NextDouble() * (float)nSteps;// this value is the y
                }

#endif
#if KEYCAPTURE
            eyex = -1.69;
           eyey = 0.74;
           eyez = -0.96;
           centerx = -0.54;
           centery = 0.6;
           centerz = 0.4;
#endif
        }

        /// <summary>
        /// Initializes the scale of the spectrum display
        /// </summary>
        /// 
        public void InitSpectrumRange(double minFrec, double maxFrec, double minAmp, double maxAmp)
        {
            m_fMinFrecuency = minFrec;
            m_fMaxFrecuency = maxFrec;
            m_fMinAmplitude = minAmp;
            m_fMaxAmplitude = maxAmp;
            m_bRangeSet = true;
        }

        /// <summary>
        /// Adds a value
        /// </summary>
        /// 
        public void AddValue(uint nStep, uint nSweepIndex, double fAmplitudeDBM)
        {
#if !TESTMODE
            if (nStep >= m_nSweepSteps)
                return; //we do not currently support more than hardwired limit sweep steps

            if (m_bRangeSet)
            {
                double value = Math.Round( (fAmplitudeDBM - m_fMinAmplitude) / ((m_fMaxAmplitude - m_fMinAmplitude) / (double)(m_nTotalSweeps-1)));
                m_arrSweepData[nStep, nSweepIndex] = (float)value;
            }
            else
                throw new ArgumentOutOfRangeException("Spectrum range not set, out of range");
#endif
        }        

        /// <summary>
        /// Adds a timestamp to a whole row
        /// </summary>
        public void AddTimeStamp(uint nSweepIndex, DateTime timeStamp)
        {
            m_arrTimeData[nSweepIndex] = timeStamp;
        }

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, RenderEventArgs e)
        {
            try
            {

                //  Get the OpenGL object.
                OpenGL gl = openGLControl.OpenGL;

                if (m_bDarkMode)
                {
                    gl.ClearColor(0f, 0f, 0f, 1f);
                }
                else
                {
                    gl.ClearColor(1f, 1f, 0.95f, 1f);
                }
                //  Clear the color and depth buffer.
                gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

                //  Load the identity matrix.
                gl.LoadIdentity();

                //  Rotate around the Y axis.
                gl.Rotate(rotation, 0.0f, 1.0f, 0.0f);

                // draw axles
                float[] AxleTextColor = new float[4];
                float[] grey = new float[4];
                if (m_bDarkMode)
                {
                    AxleTextColor[0] = 1.0f;
                    AxleTextColor[1] = 1.0f;
                    AxleTextColor[2] = 1.0f;
                    AxleTextColor[3] = 1.0f;
                    grey[0] = 0.4f;
                    grey[1] = 0.4f;
                    grey[2] = 0.4f;
                    grey[3] = 1.0f;
                }
                else
                {
                    AxleTextColor[0] = 0.0f;
                    AxleTextColor[1] = 0.0f;
                    AxleTextColor[2] = 0.0f;
                    AxleTextColor[3] = 1.0f;
                    grey[0] = 0.7f;
                    grey[1] = 0.7f;
                    grey[2] = 0.7f;
                    grey[3] = 1.0f;
                }

                // Calculate steps for X axis
                double fStart = m_fMinFrecuency;
                double fEnd = m_fMaxFrecuency;
                double fMajorStep = 1.0;
                double fSpan = (fEnd - fStart);

                if (fSpan <= 1.0)
                {
                    fMajorStep = 0.1;
                }
                else if (fSpan <= 10)
                {
                    fMajorStep = 1.0;
                }
                else if (fSpan <= 100)
                {
                    fMajorStep = 10;
                }
                else if (fSpan <= 500)
                {
                    fMajorStep = 50;
                }
                else if (fSpan <= 1000)
                {
                    fMajorStep = 100;
                }

                double XMajorStep = fMajorStep;
                double XMinorStep = fMajorStep / 10.0;

                double XOffset = (((int)((fStart + XMajorStep) / XMajorStep)) * XMajorStep) - fStart;
                if (XOffset > (XMajorStep * 1.0001))
                    throw new ArgumentOutOfRangeException("Offset bigger than step, not possible");
                if (XOffset == XMajorStep)
                    XOffset = 0;// if no rounding that means we are spot on, so no offset needed
                // Calculate steps for Y axis
                double YMajorStep = 10.0;
                double YMinorStep = 1.0;
                if ((m_fMaxAmplitude - m_fMinAmplitude) > 30)
                {
                    YMinorStep = 5.0;
                }

                //Draw floor and axis lines
                double xd;
                double minor = YMinorStep / (m_fMaxAmplitude - m_fMinAmplitude);
                double major = YMajorStep / (m_fMaxAmplitude - m_fMinAmplitude);
                double dXMajorAdjustedSteps = fSpan / XMajorStep;

                if (m_bDrawFloor)
                {
                    {
                        gl.Begin(OpenGL.GL_TRIANGLES);
                        gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE, grey);
                        gl.Vertex(0f, 0f, 0f);
                        gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE, grey);
                        gl.Vertex(m_fX_max, 0f, 0f);
                        gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE, grey);
                        gl.Vertex(m_fX_max, 0f, m_fZ_max);
                        gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE, grey);
                        gl.Vertex(0f, 0f, 0f);
                        gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE, grey);
                        gl.Vertex(0f, 0f, m_fZ_max);
                        gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE, grey);
                        gl.Vertex(m_fX_max, 0f, m_fZ_max);
                        gl.End();
                    }
                }

                gl.Begin(OpenGL.GL_LINES);
                {

                    if (CurrentPerspective != WaterfallPerspectives.Pers_2D)
                    {
                        //Y axis lines
                        gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE, AxleTextColor);
                        gl.Vertex(0f, 0f, 0f);
                        gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE, AxleTextColor);
                        gl.Vertex(0f, 1f, 0f);

                        //Draw Y axis steps
                        minor = YMinorStep / (m_fMaxAmplitude - m_fMinAmplitude);
                        major = YMajorStep / (m_fMaxAmplitude - m_fMinAmplitude);
                        for (xd = 0; xd <= 1.0f; xd += minor)
                        {
                            gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE, AxleTextColor);
                            gl.Vertex(0f, xd, -0.005f);
                            gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE, AxleTextColor);
                            gl.Vertex(0f, xd, 0.005f);
                        }
                        for (xd = 0; xd <= 1.0f; xd += major)
                        {
                            gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE, AxleTextColor);
                            gl.Vertex(0f, xd, -0.01f);
                            gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE, AxleTextColor);
                            gl.Vertex(0f, xd, 0.01f);
                        }
                    }

                    //X axis grid
                    for (xd = -(XOffset / fSpan); xd >= m_fX_max; xd += (m_fX_max / dXMajorAdjustedSteps))
                    {
                        gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE, AxleTextColor);
                        gl.Vertex(xd, 0f, 0f);
                        gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE, AxleTextColor);
                        gl.Vertex(xd, 0f, m_fZ_max);
                    }
                    gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE, AxleTextColor);
                    gl.Vertex(m_fX_max, 0f, 0f);
                    gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE, AxleTextColor);
                    gl.Vertex(m_fX_max, 0f, m_fZ_max);
                    gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE, AxleTextColor);
                    gl.Vertex(0f, 0f, 0f);
                    gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE, AxleTextColor);
                    gl.Vertex(0f, 0f, m_fZ_max);

                    //Z axis grid
                    for (xd = 0; xd <= m_fZ_max; xd += (m_fZ_max / 10.0f))
                    {
                        gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE, AxleTextColor);
                        gl.Vertex(0f, 0f, xd);
                        gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE, AxleTextColor);
                        gl.Vertex(m_fX_max, 0f, xd);
                    }
                    gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE, AxleTextColor);
                    gl.Vertex(0f, 0f, m_fZ_max);
                    gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE, AxleTextColor);
                    gl.Vertex(m_fX_max, 0f, m_fZ_max);
                }
                gl.End();

                //labels for axles
                // amplitude axle
                float ratio = (float)Height / 60.0f;
                float FontSize = (float)Width / 120.0f;// this is kind of an aproximation to size of font based on size of screen
                if (ratio > FontSize)
                    FontSize = ratio;
                if (CurrentPerspective != WaterfallPerspectives.Pers_2D)
                {
                    float xsLabelPos, startXd;
                    if (CurrentPerspective == WaterfallPerspectives.Perspective1)
                    {
                        xsLabelPos = 0.15f;
                        startXd = 0f;
                    }
                    else if (CurrentPerspective == WaterfallPerspectives.Perspective2)
                    {
                        xsLabelPos = 0.07f;
                        startXd = -0.02f;
                    }
                    else
                    {
                        xsLabelPos = 0.12f;
                        startXd = -0.08f;
                    }
                    minor = YMinorStep / (m_fMaxAmplitude - m_fMinAmplitude);
                    major = YMajorStep / (m_fMaxAmplitude - m_fMinAmplitude);
                    double AmpScale = Math.Round(m_fMinAmplitude / YMajorStep) * (int)YMajorStep;
                    double AmpStep = YMajorStep;
                    for (xd = startXd; xd <= 1.0f; xd += major)
                    {
                        double[] modelview = new double[16];
                        double[] projection = new double[16];
                        int[] viewport = new int[4];
                        gl.GetDouble(OpenGL.GL_MODELVIEW_MATRIX, modelview);
                        gl.GetDouble(OpenGL.GL_PROJECTION_MATRIX, projection);
                        gl.GetInteger(OpenGL.GL_VIEWPORT, viewport);
                        double[] xs = new double[1];
                        double[] ys = new double[1];
                        double[] zs = new double[1];
                        gl.Project(xsLabelPos, xd, -0.01f, modelview, projection, viewport, xs, ys, zs);
                        string text;
                        text = AmpScale.ToString() + " dBm";
                        gl.DrawText((int)xs[0], (int)ys[0], AxleTextColor[0], AxleTextColor[1], AxleTextColor[2], m_sFontName, FontSize, text);
                        AmpScale += AmpStep;
                    }
                }

                // frecuency axle
                minor = XMinorStep / (m_fMaxFrecuency - m_fMinFrecuency);
                major = XMajorStep / (m_fMaxFrecuency - m_fMinFrecuency);
                double FrecScale = m_fMinFrecuency + XOffset;
                double FrecStep = XMajorStep;
                float fScale = 0.7f;
                if ((CurrentPerspective != WaterfallPerspectives.Perspective1))
                {
                    fScale = 1.0f;// only scale in perspective
                }
                for (xd = -(XOffset / fSpan); xd >= m_fX_max; xd += (m_fX_max / dXMajorAdjustedSteps))
                {
                    double[] modelview = new double[16];
                    double[] projection = new double[16];
                    int[] viewport = new int[4];
                    gl.GetDouble(OpenGL.GL_MODELVIEW_MATRIX, modelview);
                    gl.GetDouble(OpenGL.GL_PROJECTION_MATRIX, projection);
                    gl.GetInteger(OpenGL.GL_VIEWPORT, viewport);
                    double[] xs = new double[1];
                    double[] ys = new double[1];
                    double[] zs = new double[1];
                    if (CurrentPerspective == WaterfallPerspectives.ISO)
                        gl.Project(xd + 0.01, -0.05f, -0.07f, modelview, projection, viewport, xs, ys, zs);
                    else if (CurrentPerspective == WaterfallPerspectives.Pers_2D)
                        gl.Project(xd + 0.01, -0.05f, -0.05, modelview, projection, viewport, xs, ys, zs);
                    else
                        gl.Project(xd + 0.01, -0.05f, -0.02, modelview, projection, viewport, xs, ys, zs);

                    string text;
                    FrecScale = Math.Round(FrecScale, 1);
                    text = FrecScale.ToString() + " MHz";
                    gl.DrawText((int)xs[0], (int)ys[0], AxleTextColor[0], AxleTextColor[1], AxleTextColor[2], m_sFontName, FontSize * fScale, text);
                    FrecScale += FrecStep;
                    if ((CurrentPerspective == WaterfallPerspectives.Perspective1))
                    {
                        fScale += 0.3f * (float)major; //for perspective, use increasing scale, smallest around the axis
                    }
                }

                // time stamps
                fScale = 1.0f;
                for (xd = 0; xd <= 1.0f; xd += 0.1f)
                {
                    double[] modelview = new double[16];
                    double[] projection = new double[16];
                    int[] viewport = new int[4];
                    gl.GetDouble(OpenGL.GL_MODELVIEW_MATRIX, modelview);
                    gl.GetDouble(OpenGL.GL_PROJECTION_MATRIX, projection);
                    gl.GetInteger(OpenGL.GL_VIEWPORT, viewport);
                    double[] xs = new double[1];
                    double[] ys = new double[1];
                    double[] zs = new double[1];
                    if (CurrentPerspective == WaterfallPerspectives.Pers_2D)
                        gl.Project(-1.0f, 0f, xd, modelview, projection, viewport, xs, ys, zs);
                    else
                        gl.Project(-1.03f, 0f, xd, modelview, projection, viewport, xs, ys, zs);
                    string text;
                    int index = (int)(xd * 100);
                    text = m_arrTimeData[index].ToString("HH:mm:ss\\.fff");
                    gl.DrawText((int)xs[0], (int)ys[0], AxleTextColor[0], AxleTextColor[1], AxleTextColor[2], m_sFontName, FontSize * fScale, text);
                    FrecScale += FrecStep;
                    if ((CurrentPerspective == WaterfallPerspectives.Perspective1) || (CurrentPerspective == WaterfallPerspectives.Perspective2))
                    {
                        fScale -= 0.04f; //for non-parallel perspectives, use a decreasing scale for far horizon text
                    }
                }

                DrawDataTriangles(gl);

                //  Nudge the rotation.
                //rotation += 0.3f;
#if KEYCAPTURE
            double delta = 0.01;
            bool shift = GetAsyncKeyState(Keys.ShiftKey)<0;
            bool reset = false;

            if (GetAsyncKeyState(Keys.X)<0)
            {
                if (shift)
                    eyex -= delta;
                else
                    eyex += delta;
                reset = true;
            }
            else if (GetAsyncKeyState(Keys.Y)<0)
            {
                if (shift)
                    eyey -= delta;
                else
                    eyey += delta;
                reset = true;
            }
            else if (GetAsyncKeyState(Keys.Z)<0)
            {
                if (shift)
                    eyez -= delta;
                else
                    eyez += delta;
                reset = true;
            }
            else if (GetAsyncKeyState(Keys.Q)<0)
            {
                if (shift)
                    centerx -= delta;
                else
                    centerx += delta;
                reset = true;
            }
            else if (GetAsyncKeyState(Keys.W)<0)
            {
                if (shift)
                    centery -= delta;
                else
                    centery += delta;
                reset = true;
            }
            else if (GetAsyncKeyState(Keys.E)<0)
            {
                if (shift)
                    centerz -= delta;
                else
                    centerz += delta;
                reset = true;
            }
            if (reset)
                SetPerspective();
#endif
            }
            catch (Exception objEx)
            {
                Console.WriteLine(objEx.ToString());
            }
        }

        private void DrawDataTriangles(OpenGL gl)
        {
            // draw samples
            //  Draw
            gl.Begin(OpenGL.GL_TRIANGLES);

            int x, z;
            float fCurrent;

            uint nType = OpenGL.GL_AMBIENT_AND_DIFFUSE;

            for (x = 0; x < m_nSweepSteps - 1; x++)
            {
                for (z = 0; z < m_nTotalSweeps - 1; z++)
                {
                    // Current value, I need one more, to create triangles
                    fCurrent = m_arrSweepData[x, z] / (float)m_nTotalSweeps;
                    //gl.Color(ColorMap(fCurrent));
                    gl.Material(OpenGL.GL_FRONT, nType, ColorMap(fCurrent));
                    gl.Vertex(-(float)x / (float)m_nSweepSteps, fCurrent, (float)z / (float)m_nTotalSweeps);
                    fCurrent = m_arrSweepData[x, z + 1] / (float)m_nTotalSweeps;
                    gl.Material(OpenGL.GL_FRONT, nType, ColorMap(fCurrent));
                    gl.Vertex(-(float)x / (float)m_nSweepSteps, fCurrent, (float)(z + 1) / (float)m_nTotalSweeps);
                    fCurrent = m_arrSweepData[x + 1, z] / (float)m_nTotalSweeps;
                    gl.Material(OpenGL.GL_FRONT, nType, ColorMap(fCurrent));
                    gl.Vertex(-(float)(x + 1) / (float)m_nSweepSteps, fCurrent, (float)z / (float)m_nTotalSweeps);
                    // done 1st triangle
                    fCurrent = m_arrSweepData[x, z + 1] / (float)m_nTotalSweeps;
                    gl.Material(OpenGL.GL_FRONT, nType, ColorMap(fCurrent));
                    gl.Vertex(-(float)x / (float)m_nSweepSteps, fCurrent, (float)(z + 1) / (float)m_nTotalSweeps);
                    fCurrent = m_arrSweepData[x + 1, z] / (float)m_nTotalSweeps;
                    gl.Material(OpenGL.GL_FRONT, nType, ColorMap(fCurrent));
                    gl.Vertex(-(float)(x + 1) / (float)m_nSweepSteps, fCurrent, (float)z / (float)m_nTotalSweeps);
                    fCurrent = m_arrSweepData[x + 1, z + 1] / (float)m_nTotalSweeps;
                    gl.Material(OpenGL.GL_FRONT, nType, ColorMap(fCurrent));
                    gl.Vertex(-(float)(x + 1) / (float)m_nSweepSteps, fCurrent, (float)(z + 1) / (float)m_nTotalSweeps);
                }
            }

            gl.End();
        }

        /// <sumary>
        /// Maps color
        /// </sumary>
        /// 
        private float[] ColorMap(float Value)
        {
            float[] retValue = new float[4];
            retValue[0] = 0.2f;
            retValue[1] = 0.2f;
            retValue[2] = 0.2f;
            if (m_bTransparent)
                retValue[3] = 0.7f;
            else
                retValue[3] = 1.0f;
            if (Value > 0.5f)
                retValue[0] = Value;
            else if (Value > 0.4f)
            {
                retValue[0] = 0.3f;
                retValue[1] = Value + 0.5f;
            }
            else if (Value > 0.3f)
            {
                retValue[1] = Value + 0.6f;
            }
            else if (Value > 0.2f)
            {
                retValue[1] = 0.3f;
                retValue[2] = Value + 0.8f;
            }
            else
                retValue[2] = Value + 0.9f;

            return retValue;
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, EventArgs e)
        {
            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            //  Set the clear color.
            gl.ClearColor(0, 0, 0, 0);

            // light
            float[] light = {1.0f,1.0f,1.0f,0.8f};
            float[] amblight = { 0.5f, 0.5f, 0.5f, 0.8f };
            gl.LightModel(LightModelParameter.Ambient, amblight);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light);
            gl.ShadeModel(OpenGL.GL_SMOOTH);

            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_LINE_SMOOTH);
            gl.Hint(HintTarget.LineSmooth, HintMode.Nicest);
            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(BlendingSourceFactor.SourceAlpha, BlendingDestinationFactor.OneMinusSourceAlpha);
            //gl.Enable(OpenGL.GL_NORMALIZE);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, EventArgs e)
        {
            SetPerspective();
        }

        private void SetPerspective()
        {
            if (openGLControl == null)
                return;

            double ratio = 1;

            switch( CurrentPerspective)
            {
                case WaterfallPerspectives.Perspective1:
                    {
                        //  Get the OpenGL object.
                        OpenGL gl = openGLControl.OpenGL;

                        gl.Viewport(0, 0, Width, Height);

                        //  Set the projection matrix.
                        gl.MatrixMode(OpenGL.GL_PROJECTION);

                        //  Load the identity.
                        gl.LoadIdentity();

                        //  Create a perspective transformation.
                        gl.Perspective(60.0f, ratio, 0.01, 100.0);

                        //  Use the 'look at' helper function to position and aim the camera.
                        gl.LookAt(-1.50, 0.44, -0.79, -0.48, 0.37, 0.48, 0, 1, 0);
                        //gl.LookAt(eyex, eyey, eyez, centerx, centery, centerz, 0, 1, 0);  
                        //  Set the modelview matrix.
                        gl.MatrixMode(OpenGL.GL_MODELVIEW);
                        break;
                    }
                case WaterfallPerspectives.Perspective2:
                    {
                        //  Get the OpenGL object.
                        OpenGL gl = openGLControl.OpenGL;
                        gl.Viewport(0, 0, Width, Height);
                        //  Set the projection matrix.
                        gl.MatrixMode(OpenGL.GL_PROJECTION);

                        //  Load the identity.
                        gl.LoadIdentity();

                        //  Create a perspective transformation.
                        gl.Perspective(60.0f, ratio, 0.01, 100.0);

                        //  Use the 'look at' helper function to position and aim the camera.
                        gl.LookAt(-0.43, 0.74, -1.41, -0.55, -0.02, 3.4, 0, 1, 0);
                        //gl.LookAt(eyex, eyey, eyez, centerx, centery, centerz, 0, 1, 0); 
                        
                        //  Set the modelview matrix.
                        gl.MatrixMode(OpenGL.GL_MODELVIEW);
                        break;
                    }
                case WaterfallPerspectives.ISO:
                    {
                        //  Get the OpenGL object.
                        OpenGL gl = openGLControl.OpenGL;
                        gl.Viewport(0, 0, Width, Height);
                        //  Set the projection matrix.
                        gl.MatrixMode(OpenGL.GL_PROJECTION);

                        //  Load the identity.
                        gl.LoadIdentity();

                        //  Create a perspective transformation.
                       // gl.Perspective(45.0f, (double)Width / (double)Height, 0.1, 100000.0);
                        gl.Ortho(0f, -2.0f, 0f, 2.0f, 0.0f, -2.0f);

                    
                        gl.Rotate(-35.264f, 1.0f, 0.0f, 0.0f);
                        gl.Rotate(-45.0f, 0.0f, 1.0f, 0.0f);
                        gl.Translate(0.59, 0.02, 1.1);
                        //  Set the modelview matrix.
                        gl.MatrixMode(OpenGL.GL_MODELVIEW);
                        //  Load the identity.
                        //gl.LoadIdentity();
                        
                        //  Use the 'look at' helper function to position and aim the camera. This can be used to fine tune the camera position
                        // enable KEYCAPTURE to use this
                        //gl.LookAt(eyex, eyey, eyez, centerx, centery, centerz, 0, 1, 0);    

                        break;
                    }
                case WaterfallPerspectives.Pers_2D:
                    {
                        //  Get the OpenGL object.
                        OpenGL gl = openGLControl.OpenGL;
                        gl.Viewport(0, 0, Width, Height);
                        //  Set the projection matrix.
                        gl.MatrixMode(OpenGL.GL_PROJECTION);

                        //  Load the identity.
                        gl.LoadIdentity();

                        //  Create a perspective transformation.
                        //gl.Perspective(60.0f, (double)Width / (double)Height, 0.01, 100.0);
                        gl.Ortho(0.1f, -1.1f, -0.1f, 1.1f, 1.1f, -0.1f);

                        //  Use the 'look at' helper function to position and aim the camera.
                        //gl.LookAt(-0.5, 2.0, +0.5, -0.5, 0, +0.5, 0, 0, 1);
                        gl.Rotate(-90.0f, 1.0f, 0f, 0f);
                        //gl.LookAt(-0, -0.3, -0.009, -0.001, 2.149, -0.019, 0, 0, 1);
                        //  Set the modelview matrix.
                        gl.MatrixMode(OpenGL.GL_MODELVIEW);
                        break;
                    }
            }
            Invalidate();
        }

        /// <summary>
        /// The current rotation.
        /// </summary>
        private float rotation = 0.0f;

        public void Perspective1()
        {
            CurrentPerspective = WaterfallPerspectives.Perspective1;
            SetPerspective();
        }

        public void Perspective2()
        {
            CurrentPerspective = WaterfallPerspectives.Perspective2;
            SetPerspective();
        }

        public void ISO()
        {
            CurrentPerspective = WaterfallPerspectives.ISO;
            SetPerspective();
        }

        public void Pers_2D()
        {
            CurrentPerspective = WaterfallPerspectives.Pers_2D;
            SetPerspective();
        }

        private void SharpGLForm_Paint(object sender, PaintEventArgs e)
        {
            //Draw RF Explorer title
            m_ImageLogo.Visible = DrawTitle;
            if (DrawTitle)
            {
                if (CurrentPerspective == WaterfallPerspectives.Pers_2D)
                {
                    m_ImageLogo.Width = Width / 5;
                    m_ImageLogo.Location = new Point(2*Width / 5, 0);
                }
                else
                {
                    m_ImageLogo.Width = Width / 3;
                    m_ImageLogo.Location = new Point(Width / 3, 0);
                }
                m_ImageLogo.Height = (int)(m_ImageLogo.Width*(double)m_ImageLogo.Image.Height/(double)m_ImageLogo.Image.Width);
                //m_ImageLogo.BringToFront();
            }
        }
    }
}
