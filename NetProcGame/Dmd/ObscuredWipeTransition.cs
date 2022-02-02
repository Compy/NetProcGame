using NetProcGame.Dmd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetProcGame.Dmd
{
    public enum ObscuredWipeTransitionDirection
    {
        North,
        South,
        East,
        West
    }
    public class ObscuredWipeTransition : LayerTransitionBase
    {
        public DMDBlendMode composite_op;
        public ObscuredWipeTransitionDirection direction;
        public Frame obs_frame;

        public ObscuredWipeTransition(Frame obscuring_frame, DMDBlendMode composite_op, ObscuredWipeTransitionDirection direction = ObscuredWipeTransitionDirection.North)
        {
            this.composite_op = composite_op;
            this.direction = direction;
            this.progress_per_frame = 1.0 / 15.0;
            this.obs_frame = obscuring_frame;
        }

        public override Frame transition_frame(Frame from_frame, Frame to_frame)
        {
            Frame frame = new Frame(from_frame.width, from_frame.height);
            double prog0 = this.progress;
            double prog1 = this.progress;
            if (this.in_out == false)
                prog0 = 1.0 - prog0;
            else
                prog1 = 1.0 - prog1;

            int src_x, src_y, ovr_x, ovr_y;
            // TODO: Improve src_x/y so that it moves at the same speed as ovr_x/y with the midpoint
            if (direction == ObscuredWipeTransitionDirection.North)
            {
                src_x = 0;
                src_y = (int)(prog1 * frame.height);
                ovr_x = 0;
                ovr_y = (int)(frame.height - prog0 * (this.obs_frame.height + 2 * frame.height));
            }
            else if (direction == ObscuredWipeTransitionDirection.South)
            {
                src_x = 0;
                src_y = (int)(prog0 * frame.height);
                ovr_x = 0;
                ovr_y = (int)(frame.height - prog1 * (this.obs_frame.height + 2 * frame.height));
            }
            else if (direction == ObscuredWipeTransitionDirection.East)
            {
                src_x = (int)(prog0 * frame.width);
                src_y = 0;
                ovr_x = (int)(frame.width - prog1 * (this.obs_frame.width + 2 * frame.width));
                ovr_y = 0;
            }
            else
            {
                src_x = (int)(prog1 * frame.width);
                src_y = 0;
                ovr_x = (int)(frame.width - prog0 * (this.obs_frame.width + 2 * frame.width));
                ovr_y = 0;
            }

            if (this.direction == ObscuredWipeTransitionDirection.East || this.direction == ObscuredWipeTransitionDirection.South)
            {
                Frame tmpFrame = from_frame;
                from_frame = to_frame;
                to_frame = tmpFrame;
            }

            Frame.copy_rect(frame, 0, 0, from_frame, 0, 0, from_frame.width, from_frame.height, DMDBlendMode.DMDBlendModeCopy);
            Frame.copy_rect(frame, src_x, src_y, to_frame, src_x, src_y, from_frame.width - src_x, from_frame.height - src_y, DMDBlendMode.DMDBlendModeCopy);
            Frame.copy_rect(frame, ovr_x, ovr_y, obs_frame, 0, 0, this.obs_frame.width, this.obs_frame.height, this.composite_op);

            return frame;
        }
    }
}
