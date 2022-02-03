namespace NetProc.Dmd
{
    /// <summary>
    /// Displays a single frame
    /// </summary>
    public class FrameLayer : Layer
    {
        /// <summary>
        /// Number of frame times to turn on and off
        /// </summary>
        public int blink_frames = 0;
        private int blink_frames_counter = 0;
        private Frame frame_old = null;

        private Frame frame;

        public FrameLayer(bool opaque = false, Frame frame = null)
            : base(opaque)
        {
            this.frame = frame;
        }

        public override Frame next_frame()
        {
            if (this.blink_frames > 0)
            {
                if (this.blink_frames_counter == 0)
                {
                    this.blink_frames_counter = this.blink_frames;
                    if (this.frame == null)
                        this.frame = this.frame_old;
                    else
                    {
                        this.frame_old = this.frame;
                        this.frame = null;
                    }
                }
                else
                    this.blink_frames_counter--;
            }
            return this.frame;
        }
    }
}
