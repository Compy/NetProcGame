namespace NetProc
{
    public struct DMDPoint
    {
        public int x, y;
    }

    public struct DMDSize
    {
        public int width, height;
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
