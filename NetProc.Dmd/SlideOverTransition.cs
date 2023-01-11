using NetProc.Interface;

namespace NetProc.Dmd
{
    public enum SlideOverTransitionDirection
    {
        North,
        South,
        East,
        West
    }
    public class SlideOverTransition : LayerTransitionBase
    {
        public SlideOverTransitionDirection direction;

        public SlideOverTransition(SlideOverTransitionDirection direction = SlideOverTransitionDirection.North)
        {
            this.direction = direction;
            this.progress_per_frame = 1.0 / 15.0;
        }

        public override IFrame transition_frame(IFrame from_frame, IFrame to_frame)
        {
            var frame = (Frame)from_frame.Copy();
            int dst_x = 0;
            int dst_y = 0;

            double prog = this.progress;
            if (this.in_out == true)
            {
                prog = 1.0 - prog;
            }

            if (this.direction == SlideOverTransitionDirection.North)
            {
                dst_x = 0;
                dst_y = (int)(prog * frame.height);
            }
            else if (this.direction == SlideOverTransitionDirection.South)
            {
                dst_x = 0;
                dst_y = (int)(-prog * frame.height);
            }
            else if (this.direction == SlideOverTransitionDirection.East)
            {
                dst_x = (int)(-prog * frame.width);
                dst_y = 0;
            }
            else if (this.direction == SlideOverTransitionDirection.West)
            {
                dst_x = (int)(prog * frame.width);
                dst_y = 0;
            }
            Frame.copy_rect(frame, dst_x, dst_y, (Frame)to_frame, 0, 0, ((Frame)from_frame).width, ((Frame)from_frame).height, DMDBlendMode.DMDBlendModeCopy);
            return frame;
        }
    }
}
