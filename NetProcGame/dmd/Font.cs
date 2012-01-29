using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetProcGame.dmd
{
    /// <summary>
    /// Variable width bitmap font.
    /// 
    /// Fonts can be loaded manually, using the load() method or with the font_named() utility
    /// function which supports searching a font path.
    /// </summary>
    public class Font
    {
        /// <summary>
        /// Array of dot widths for each character, 0-indexed from 'space'.
        /// This array is populated by the load() method
        /// </summary>
        public uint[] char_widths = null;

        /// <summary>
        /// Number of dots to adjust the horizontal position between characters, in addition to the last chars width.
        /// </summary>
        public uint tracking = 0;

        /// <summary>
        /// Composite operation used by draw() when calling DMDBuffer.copy_rect()
        /// </summary>
        public DMDBlendMode composite_op = DMDBlendMode.DMDBlendModeCopy;

        

        public Font(string filename = "")
        {

        }
    }
}
