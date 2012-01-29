using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetProcGame.dmd
{
    public enum FontJustify
    {
        Left,
        Right,
        Center
    }
    /// <summary>
    /// A layer that displays text
    /// </summary>
    public class TextLayer : Layer
    {
        public TextLayer(uint x, uint y, object font, FontJustify justify, bool opaque = false)
            : base(opaque)
        {
            this.set_target_position(x, y);

        }
    }
}
