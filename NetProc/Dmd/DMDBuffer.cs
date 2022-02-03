using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetProc.Dmd
{
    public class DMDBuffer
    {
        public DMDFrame frame;

        public DMDBuffer(int width, int height)
        {
            frame = DMDGlobals.DMDFrameCreate(new DMDSize() { height = height, width = width });
        }
        ~DMDBuffer()
        {
            frame.buffer = null;
        }

        public void clear()
        {
            Array.Clear(frame.buffer, 0, 0);
        }

        public void set_data(byte[,] data)
        {
            int frame_size = DMDGlobals.DMDFrameGetBufferSize(ref frame);
            if (data.Length != frame_size)
            {
                throw new Exception("Buffer length is incorrect (" + data.Length.ToString() + " != " + frame_size.ToString() + ")");
            }
            Buffer.BlockCopy(data, 0, frame.buffer, 0, (int)frame_size);
        }
        public void set_data(byte[] data)
        {
            int frame_size = DMDGlobals.DMDFrameGetBufferSize(ref frame);
            if (data.Length != frame_size)
            {
                throw new Exception("Buffer length is incorrect");
            }
            Buffer.BlockCopy(data, 0, frame.buffer, 0, (int)frame_size);
        }
        public byte[] get_data()
        {
            return frame.buffer;
        }
        public byte get_dot(int x, int y)
        {
            if (x >= frame.size.width || y >= frame.size.height)
            {
                throw new Exception("X or Y are out of range");
            }
            return DMDGlobals.DMDFrameGetDot(ref frame, x, y);
        }
        public void set_dot(int x, int y, byte value)
        {
            if (x >= frame.size.width || y >= frame.size.height)
            {
                throw new Exception("X or Y are out of range");
            }
            DMDGlobals.DMDFrameSetDot(ref frame, x, y, value);
        }

        public void fill_rect(int x, int y, int width, int height, byte value)
        {
            DMDGlobals.DMDFrameFillRect(ref frame,
                DMDGlobals.DMDRectMake(x, y, width, height),
                value);
        }

        public void copy_to_rect(DMDBuffer dst, int dst_x, int dst_y, int src_x, int src_y, int width, int height, DMDBlendMode mode = DMDBlendMode.DMDBlendModeCopy)
        {
            DMDRect srcRect = DMDGlobals.DMDRectMake(src_x, src_y, width, height);
            DMDPoint dstPoint = DMDGlobals.DMDPointMake(dst_x, dst_y);
            DMDGlobals.DMDFrameCopyRect(ref frame, srcRect, ref dst.frame, dstPoint, mode);            
        }

        public string ascii()
        {
            string output = "";
            char[] table = { ' ', '.', '.', '.', ',', ',', ',', '-', '-', '=', '=', '=', '*', '*', '#', '#' };
            byte dot = 0;
            for (int y = 0; y < this.frame.size.height; y++)
            {
                for (int x = 0; x < this.frame.size.width; x++)
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
