using NetProc.Interface;

namespace NetProc.Dmd
{
    public enum WipeTransitionDirection
    {
        North,
        South,
        East,
        West
    }
    class WipeTransition : LayerTransitionBase
    {
        public WipeTransitionDirection direction;

        public WipeTransition(WipeTransitionDirection direction = WipeTransitionDirection.North)
        {
            this.direction = direction;
            this.progress_per_frame = 1.0 / 15.0;
        }

        public override IFrame transition_frame(IFrame from_frame, IFrame to_frame)
        {
            Frame frame = new Frame(((Frame)from_frame).width, ((Frame)from_frame).height);
            double prog0 = this.progress;
            double prog1 = this.progress;

            int src_x = 0;
            int src_y = 0;

            if (this.in_out == false)
                prog0 = 1.0 - prog0;
            else
                prog1 = 1.0 - prog1;

            if (this.direction == WipeTransitionDirection.North)
            {
                src_x = 0;
                src_y = (int)(prog1 * frame.height);
            }
            else if (this.direction == WipeTransitionDirection.South)
            {
                src_x = 0;
                src_y = (int)(prog0 * frame.height);
            }
            else if (this.direction == WipeTransitionDirection.East)
            {
                src_x = (int)(prog0 * frame.width);
                src_y = 0;
            }
            else if (this.direction == WipeTransitionDirection.East)
            {
                src_x = (int)(prog1 * frame.width);
                src_y = 0;
            }
            if (this.direction == WipeTransitionDirection.East || this.direction == WipeTransitionDirection.South)
            {
                Frame tmpFrame = from_frame as Frame;
                from_frame = to_frame;
                to_frame = tmpFrame;
            }
            Frame.copy_rect(frame, 0, 0, (Frame)from_frame, 0, 0, ((Frame)from_frame).width, ((Frame)from_frame).height, DMDBlendMode.DMDBlendModeCopy);
            Frame.copy_rect(frame, src_x, src_y, (Frame)to_frame, src_x, src_y, ((Frame)from_frame).width, ((Frame)from_frame).height, DMDBlendMode.DMDBlendModeCopy);

            return frame;
        }
    }
}
