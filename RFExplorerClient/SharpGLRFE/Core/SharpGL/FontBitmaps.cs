using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGL
{
    /// <summary>
    /// A FontOutline entry contains the details of a font face.
    /// </summary>
    internal class FontOutlineEntry
    {
        /// <summary>
        /// Gets or sets the HDC.
        /// </summary>
        /// <value>
        /// The HDC.
        /// </value>
        public IntPtr HDC
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the HRC.
        /// </summary>
        /// <value>
        /// The HRC.
        /// </value>
        public IntPtr HRC
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the face.
        /// </summary>
        /// <value>
        /// The name of the face.
        /// </value>
        public string FaceName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public int Height
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list base.
        /// </summary>
        /// <value>
        /// The list base.
        /// </value>
        public uint ListBase
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list count.
        /// </summary>
        /// <value>
        /// The list count.
        /// </value>
        public uint ListCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the deviation.
        /// </summary>
        /// <value>
        /// The deviation.
        /// </value>
        public float Deviation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the extrusion.
        /// </summary>
        /// <value>
        /// The extrusion.
        /// </value>
        public float Extrusion
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the font outline format.
        /// </summary>
        /// <value>
        /// The font outline format.
        /// </value>
        public FontOutlineFormat FontOutlineFormat
        {
            get;
            set;
        }
    }

    /// <summary>
    /// The font outline format.
    /// </summary>
    public enum FontOutlineFormat
    {
        /// <summary>
        /// Render using lines.
        /// </summary>
        Lines = 0,

        /// <summary>
        /// Render using polygons.
        /// </summary>
        Polygons = 1
    }

    /// <summary>
    /// This class wraps the functionality of the wglUseFontOutlines function to
    /// allow straightforward rendering of text.
    /// </summary>
    public class FontOutlines
    {
        private FontOutlineEntry CreateFontOutlineEntry(OpenGL gl, string faceName, int height,
            float deviation, float extrusion, FontOutlineFormat fontOutlineFormat)
        {
            //  Make the OpenGL instance current.
            gl.MakeCurrent();

            //  Create the font based on the face name.
            var hFont = Win32.CreateFont(height, 0, 0, 0, Win32.FW_DONTCARE, 0, 0, 0, Win32.DEFAULT_CHARSET, 
                Win32.OUT_OUTLINE_PRECIS, Win32.CLIP_DEFAULT_PRECIS, Win32.CLEARTYPE_QUALITY, Win32.VARIABLE_PITCH, faceName);
            
            //  Select the font handle.
            var hOldObject = Win32.SelectObject(gl.RenderContextProvider.DeviceContextHandle, hFont);
            
            //  Create the list base.
            var listBase = gl.GenLists(1);

            //  Create the font bitmaps.
            bool result = Win32.wglUseFontOutlines(gl.RenderContextProvider.DeviceContextHandle, 0, 255, listBase,
                deviation, extrusion, (int)fontOutlineFormat, IntPtr.Zero);

            //  Reselect the old font.
            Win32.SelectObject(gl.RenderContextProvider.DeviceContextHandle, hOldObject);

            //  Free the font.
            Win32.DeleteObject(hFont);

            //  Create the font bitmap entry.
            var foe = new FontOutlineEntry()
            {
                HDC = gl.RenderContextProvider.DeviceContextHandle,
                HRC = gl.RenderContextProvider.RenderContextHandle,
                FaceName = faceName,
                Height = height,
                ListBase = listBase,
                ListCount = 255,
                Deviation = deviation,
                Extrusion = extrusion,
                FontOutlineFormat = fontOutlineFormat
            };

            //  Add the font bitmap entry to the internal list.
            fontOutlineEntries.Add(foe);

            return foe;
        }

        /// <summary>
        /// Draws the text.
        /// </summary>
        /// <param name="gl">The gl.</param>
        /// <param name="faceName">Name of the face.</param>
        /// <param name="fontSize">Size of the font.</param>
        /// <param name="deviation">The deviation.</param>
        /// <param name="extrusion">The extrusion.</param>
        /// <param name="text">The text.</param>
        public void DrawText(OpenGL gl, string faceName, float fontSize, 
            float deviation, float extrusion, string text)
        {
            //  Get the font size in pixels.
            var fontHeight = (int)(fontSize * (16.0f / 12.0f));

            //  Do we have a font bitmap entry for this OpenGL instance and face name?
            var result = (from fbe in fontOutlineEntries
                         where fbe.HDC == gl.RenderContextProvider.DeviceContextHandle
                         && fbe.HRC == gl.RenderContextProvider.RenderContextHandle
                         && String.Compare(fbe.FaceName, faceName, StringComparison.OrdinalIgnoreCase) == 0
                         && fbe.Height == fontHeight
                         && fbe.Deviation == deviation
                         && fbe.Extrusion == extrusion
                         select fbe).ToList();

            //  Get the FBE or null.
            var fontOutlineEntry = result.FirstOrDefault();

            //  If we don't have the FBE, we must create it.
            if (fontOutlineEntry == null)
                fontOutlineEntry = CreateFontOutlineEntry(gl, faceName, fontHeight, deviation, extrusion, FontOutlineFormat.Polygons);

            //  Set the list base.
            gl.ListBase(fontOutlineEntry.ListBase);

            //  Create an array of lists for the glyphs.
            var lists = text.Select(c => (byte) c).ToArray();

            //  Call the lists for the string.
            gl.CallLists(lists.Length, lists);
            gl.Flush();
        }

        /// <summary>
        /// The cache of font outline entries.
        /// </summary>
        private readonly List<FontOutlineEntry> fontOutlineEntries = new List<FontOutlineEntry>();
    }
}
