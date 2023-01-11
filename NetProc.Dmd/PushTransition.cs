using NetProc.Interface;

namespace NetProc.Dmd
{
    public enum PushTransitionDirection
    {
        North,
        South,
        East,
        West
    }
    public class PushTransition : LayerTransitionBase
    {
        public PushTransitionDirection direction;

        public PushTransition(PushTransitionDirection direction = PushTransitionDirection.North)
        {
            this.direction = direction;
            this.progress_per_frame = 1.0 / 15.0;
        }

        public override IFrame transition_frame(IFrame from_frame, IFrame to_frame)
        {
            Frame frame = new Frame(((Frame)from_frame).width, ((Frame)from_frame).height);
            int dst_x = 0;
            int dst_y = 0;
            int dst_x1 = 0;
            int dst_y1 = 0;

            double prog = this.progress;
            double prog1 = this.progress;

            if (this.in_out == true)
                prog = 1.0 - prog;
            else
                prog1 = 1.0 - prog1;

            if (direction == PushTransitionDirection.North)
            {
                dst_x = 0;
                dst_y = (int)(prog * frame.height);
                dst_x1 = 0;
                dst_y1 = (int)(-prog1 * frame.height);
            }
            else if (direction == PushTransitionDirection.South)
            {
                dst_x = 0;
                dst_y = (int)(-prog * frame.height);
                dst_x1 = 0;
                dst_y1 = (int)(prog1 * frame.height);
            }
            else if (direction == PushTransitionDirection.East)
            {
                dst_x = (int)(-prog * frame.width);
                dst_y = 0;
                dst_x1 = (int)(prog1 * frame.width);
                dst_y1 = 0;
            }
            else if (direction == PushTransitionDirection.West)
            {
                dst_x = (int)(prog * frame.width);
                dst_y = 0;
                dst_x1 = (int)(-prog1 * frame.width);
                dst_y1 = 0;
            }
            Frame.copy_rect(frame, dst_x, dst_y, (Frame)to_frame, 0, 0, ((Frame)from_frame).width, ((Frame)from_frame).height, DMDBlendMode.DMDBlendModeCopy);
            Frame.copy_rect(frame, dst_x1, dst_y1, (Frame)from_frame, 0, 0, ((Frame)from_frame).width, ((Frame)from_frame).height, DMDBlendMode.DMDBlendModeCopy);

            return frame;
        }
    }
}
