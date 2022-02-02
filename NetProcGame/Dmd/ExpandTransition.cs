using System;

namespace NetProcGame.Dmd
{
    public enum ExpandTransitionDirection
    {
        Vertical,
        Horizontal
    }
    public class ExpandTransition : LayerTransitionBase
    {
        public ExpandTransitionDirection direction;
        public ExpandTransition(ExpandTransitionDirection direction = ExpandTransitionDirection.Vertical)
        {
            this.direction = direction;
            this.progress_per_frame = 1.0 / 11.0;
        }

        public override Frame transition_frame(Frame from_frame, Frame to_frame)
        {
            Frame frame = new Frame(from_frame.width, from_frame.height);
            int dst_x = 0;
            int dst_y = 0;
            int width, height;
            double prog = this.progress;

            if (this.in_out == false)
                prog = 1.0 - prog;

            if (this.direction == ExpandTransitionDirection.Vertical)
            {
                dst_x = 0;
                dst_y = Convert.ToInt32((frame.height / 2 - prog * (frame.height / 2)));

                width = frame.width;
                height = Convert.ToInt32(prog * frame.height);
            }
            else
            {
                dst_x = Convert.ToInt32((frame.width / 2 - prog * (frame.width / 2)));
                dst_y = 0;

                width = Convert.ToInt32(prog * frame.width);
                height = frame.height;
            }
            Frame.copy_rect(frame, dst_x, dst_y, to_frame, dst_x, dst_y, width, height, DMDBlendMode.DMDBlendModeCopy);
            return frame;
        }
    }
}
