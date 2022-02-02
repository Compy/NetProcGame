using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetProcGame.Dmd
{
    public class Frame : DMDBuffer
    {
        public int width = 0;
        public int height = 0;
        public Frame(int width, int height)
            : base(width, height)
        {
            this.width = width;
            this.height = height;
        }

        public static void copy_rect(DMDBuffer dst, int dst_x, int dst_y, DMDBuffer src, int src_x, int src_y, int width, int height, DMDBlendMode mode = DMDBlendMode.DMDBlendModeCopy)
        {
            src.copy_to_rect(dst, dst_x, dst_y, src_x, src_y, width, height, mode);
        }

        public Frame subframe(int x, int y, int width, int height)
        {
            // Generates a new frame based on a sub rectangle of this frame
            Frame subframe = new Frame(width, height);
            Frame.copy_rect(subframe, 0, 0, this, x, y, width, height, DMDBlendMode.DMDBlendModeCopy);
            return subframe;
        }

        public Frame copy()
        {
            Frame frame = new Frame(width, height);
            frame.set_data(this.get_data());
            return frame;
        }
    }
}
