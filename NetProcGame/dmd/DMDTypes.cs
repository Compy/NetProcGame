using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetProcGame.dmd
{
    public struct DMDPoint
    {
        public uint x, y;
    }

    public struct DMDSize
    {
        public uint width, height;
    }

    public struct DMDRect
    {
        public DMDPoint origin;
        public DMDSize size;
    }

    public struct DMDFrame
    {
        public DMDSize size;
        public byte[] buffer;
    }

    public enum DMDBlendMode
    {
        DMDBlendModeCopy = 0,
        DMDBlendModeAdd = 1,
        DMDBlendModeSubtract = 2,
        DMDBlendModeBlackSource = 3,
        DMDBlendModeAlpha = 4,
        DMDBlendModeAlphaBoth = 5
    }
}
