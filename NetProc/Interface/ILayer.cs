namespace NetProc.Interface
{
    public interface ILayer
    {
        bool Enabled { get; set; }
        bool Opaque { get; set; }
        IFrame composite_next(IFrame target);
        IFrame next_frame();
        void reset();
        void set_target_position(int x, int y);
    }
}