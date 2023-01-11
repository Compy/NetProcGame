namespace NetProc.Interface
{
    public interface IFrame
    {
        DMDFrame frame { get; set; }
        IFrame Copy();
        IFrame SubFrame(int x, int y, int width, int height);
    }
}