using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetProcGame.dmd
{
    public class Frame : DMDBuffer
    {
        public uint width = 0;
        public uint height = 0;
        public Frame(uint width, uint height)
            : base(width, height)
        {
            this.width = width;
            this.height = height;
        }

        public static void copy_rect(DMDBuffer dst, uint dst_x, uint dst_y, DMDBuffer src, uint src_x, uint src_y, uint width, uint height, DMDBlendMode mode = DMDBlendMode.DMDBlendModeCopy)
        {
            src.copy_to_rect(ref dst, dst_x, dst_y, src_x, src_y, width, height, mode);
        }

        public Frame subframe(uint x, uint y, uint width, uint height)
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

        public string ascii()
        {
            string output = "";
            char[] table = {' ', '.', '.', '.', ',', ',', ',', '-', '-', '=', '=', '=', '*', '*', '#', '#'};
            byte dot = 0;
            for (uint y = 0; y < this.height; y++)
            {
                for (uint x = 0; x < this.width; x++)
                {
                    dot = this.get_dot(x, y);
                    output += table[dot & 0xf];
                }
                output += "\n";
            }
            return output;
        }
    }
}
