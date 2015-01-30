//============================================================================
//
//This class created by Ariel Rocholl (C) 2014
//
//This library is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 2.1 of the License, or (at your option) any later version.
//
//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this library; if not, write to the Free Software
//Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//=============================================================================

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ZedGraph
{
    /// <summary>
    /// A class that represents a marker on the graph.  A list of
    /// MarkerObj objects is maintained by the <see cref="GraphObjList"/> collection class.
    /// </summary>

    [Serializable]
    public class MarkerObj : LineObj, ICloneable, ISerializable, IDisposable
    {
        #region Fields
        /// <summary>
        /// Private field that stores the arrowhead size, measured in points.
        /// Use the public property <see cref="Size"/> to access this value.
        /// </summary>
        private float _size = _DefaultSize;

        Font m_FontInsideText = new Font("Arial", 8);
        Font m_FontFullText = new Font("Arial Black", 11);
        const float _DefaultSize = 12.0F;

        #endregion

        #region Properties
        /// <summary>
        /// The size of the triangle.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <value>Size is in pixels</value>
        public float Size
        {
            get { return _size; }
            set { _size = value; }
        }

        string m_sInsideText = "";
        /// <summary>
        /// Inside text used for marker ordinal
        /// </summary>
        public string InsideText
        {
            get { return m_sInsideText; }
            set { m_sInsideText = value; }
        }

        string m_sFullText = "";
        /// <summary>
        /// Outside full text used for value marker
        /// </summary>
        public string FullText
        {
            get { return m_sFullText; }
            set { m_sFullText = value; }
        }

        #endregion

        #region Constructors
        /// <overloads>Constructors for the <see cref="MarkerObj"/> object</overloads>
        /// <summary>
        /// A constructor that allows the position, color, and size of the
        /// <see cref="MarkerObj"/> to be pre-specified.
        /// </summary>
        /// <param name="color">An arbitrary <see cref="System.Drawing.Color"/> specification
        /// for the arrow</param>
        /// <param name="size">The size of the arrowhead, measured in points.</param>
        /// <param name="x1">The x position of the starting point that defines the
        /// arrow.  The units of this position are specified by the
        /// <see cref="Location.CoordinateFrame"/> property.</param>
        /// <param name="y1">The y position of the starting point that defines the
        /// arrow.  The units of this position are specified by the
        /// <see cref="Location.CoordinateFrame"/> property.</param>
        public MarkerObj( Color color, float size, double x1, double y1)
            : base( color, x1, y1, 0, 0 )
        {
            _size = size;
        }

        /// <summary>
        /// A constructor that allows only the position of the
        /// arrow to be pre-specified.  All other properties are set to
        /// default values
        /// </summary>
        /// <param name="x1">The x position of the starting point that defines the
        /// <see cref="MarkerObj"/>.  The units of this position are specified by the
        /// <see cref="Location.CoordinateFrame"/> property.</param>
        /// <param name="y1">The y position of the starting point that defines the
        /// <see cref="MarkerObj"/>.  The units of this position are specified by the
        /// <see cref="Location.CoordinateFrame"/> property.</param>
        public MarkerObj( double x1, double y1)
            : this( LineBase.Default.Color, _DefaultSize, x1, y1)
        {
        }

        /// <summary>
        /// Default constructor -- places the <see cref="MarkerObj"/> at location
        /// (0,0) to (1,1).  All other values are defaulted.
        /// </summary>
        public MarkerObj()
            :
            this( LineBase.Default.Color, _DefaultSize, 0, 0 )
        {
        }

        /// <summary>
        /// The Copy Constructor
        /// </summary>
        /// <param name="rhs">The <see cref="MarkerObj"/> object from which to copy</param>
        public MarkerObj( MarkerObj rhs )
            : base( rhs )
        {
            _size = rhs.Size;
        }

        /// <summary>
        /// Implement the <see cref="ICloneable" /> interface in a typesafe manner by just
        /// calling the typed version of <see cref="Clone" />
        /// </summary>
        /// <returns>A deep copy of this object</returns>
        object ICloneable.Clone()
        {
            return this.Clone();
        }

        /// <summary>
        /// Typesafe, deep-copy clone method.
        /// </summary>
        /// <returns>A new, independent copy of this class</returns>
        public new MarkerObj Clone()
        {
            return new MarkerObj( this );
        }

        bool m_bDisposed=false;
        /// <summary>
        /// Local dispose method
        /// </summary>
        /// <param name="bDisposing">if disposing is required</param>
        override protected void Dispose(bool bDisposing)
        {
            if (!m_bDisposed)
            {
                if (bDisposing)
                {
                    if (m_FontFullText != null)
                    {
                        m_FontFullText.Dispose();
                        m_FontFullText = null;
                    }
                    if (m_FontInsideText != null)
                    {
                        m_FontInsideText.Dispose();
                        m_FontInsideText = null;
                    }
                }
                base.Dispose(bDisposing);
                m_bDisposed = true;
            }
        }

        #endregion

        #region Serialization
        /// <summary>
        /// Current schema value that defines the version of the serialized file
        /// </summary>
        public const int schema3 = 10;

        /// <summary>
        /// Constructor for deserializing objects
        /// </summary>
        /// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data
        /// </param>
        /// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data
        /// </param>
        protected MarkerObj( SerializationInfo info, StreamingContext context )
            : base( info, context )
        {
            // The schema value is just a file version parameter.  You can use it to make future versions
            // backwards compatible as new member variables are added to classes
            int sch = info.GetInt32( "schema3" );

            _size = info.GetSingle( "size" );
        }
        /// <summary>
        /// Populates a <see cref="SerializationInfo"/> instance with the data needed to serialize the target object
        /// </summary>
        /// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data</param>
        /// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data</param>
        [SecurityPermissionAttribute( SecurityAction.Demand, SerializationFormatter = true )]
        public override void GetObjectData( SerializationInfo info, StreamingContext context )
        {
            base.GetObjectData( info, context );
            info.AddValue( "schema3", schema2 );
            info.AddValue( "size", _size );
        }
        #endregion

        #region Rendering Methods
        /// <summary>
        /// Render this object to the specified <see cref="Graphics"/> device.
        /// </summary>
        /// <remarks>
        /// This method is normally only called by the Draw method
        /// of the parent <see cref="GraphObjList"/> collection object.
        /// </remarks>
        /// <param name="g">
        /// A graphic device object to be drawn into.  This is normally e.Graphics from the
        /// PaintEventArgs argument to the Paint() method.
        /// </param>
        /// <param name="pane">
        /// A reference to the <see cref="PaneBase"/> object that is the parent or
        /// owner of this object.
        /// </param>
        /// <param name="scaleFactor">
        /// The scaling factor to be used for rendering objects.  This is calculated and
        /// passed down by the parent <see cref="GraphPane"/> object using the
        /// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
        /// font sizes, etc. according to the actual size of the graph.
        /// </param>
        override public void Draw( Graphics g, PaneBase pane, float scaleFactor )
        {
            if (!IsVisible)
                return;

            // Convert the arrow coordinates from the user coordinate system
            // to the screen coordinate system
            PointF pix1 = this.Location.TransformTopLeft( pane );
            PointF pix2 = this.Location.TransformBottomRight( pane );

            if ( pix1.X > -10000 && pix1.X < 100000 && pix1.Y > -100000 && pix1.Y < 100000 &&
                pix2.X > -10000 && pix2.X < 100000 && pix2.Y > -100000 && pix2.Y < 100000 )
            {
                // get a scaled size for the arrowhead
                float scaledSize = _size;

                // Save the old transform matrix
                Matrix transform = g.Transform;
                // Move the coordinate system so it is located at the starting point
                // of this arrow
                g.TranslateTransform( pix1.X, pix1.Y );

                // get a pen according to this arrow properties
                using (Pen pen = _line.GetPen(pane, scaleFactor))
                {
                    // Create a polygon representing the arrowhead based on the scaled
                    // size
                    PointF[] polyPt = new PointF[4];
                    float hsize = scaledSize / 2F;
                    polyPt[0].Y = 0;
                    polyPt[0].X = 0;
                    polyPt[1].Y = - scaledSize;
                    polyPt[1].X = hsize;
                    polyPt[2].Y = - scaledSize;
                    polyPt[2].X = -hsize;
                    polyPt[3] = polyPt[0];

                    PointF[] polyPt2 = new PointF[5];
                    polyPt2[0].Y = 1;
                    polyPt2[0].X = 1;
                    polyPt2[1].Y = - scaledSize - 1;
                    polyPt2[1].X = hsize + 1;
                    polyPt2[2].Y = - scaledSize - 1;
                    polyPt2[2].X = -hsize - 1;
                    polyPt2[3].Y = 1;
                    polyPt2[3].X = -1;
                    polyPt2[4] = polyPt2[0];

                    using (SolidBrush WhiteBrush=new SolidBrush(Color.White))
                    {
                        g.FillPolygon(WhiteBrush, polyPt2);
                    }
                    g.DrawPolygon(pen, polyPt);
                    if (!String.IsNullOrEmpty(m_sInsideText))
                    {
                        StringFormat drawFormat = new StringFormat();
                        try
                        {
                            drawFormat.Alignment = StringAlignment.Center;

                            using (SolidBrush brush = new SolidBrush(_line._color))
                            {
                                g.DrawString(m_sInsideText, m_FontInsideText, brush, new RectangleF(-hsize, -scaledSize, 2 * hsize, scaledSize), drawFormat);
                                //g.DrawRectangle(pen, -hsize, -scaledSize, 2 * hsize, scaledSize);
                                if (!String.IsNullOrEmpty(m_sFullText))
                                {
                                    g.DrawString(m_sFullText, m_FontFullText, brush, 0, -4 * scaledSize, drawFormat);
                                }
                            }
                        }
                        finally
                        {
                            drawFormat.Dispose();
                        }
                    }
                }

                // Restore the transform matrix back to its original state
                g.Transform = transform;
            }
        }

        #endregion

    }
}
