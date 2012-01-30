using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetProcGame.dmd
{
    static class DMDGlobals
    {
        public static DMDPoint DMDPointMake(uint x, uint y)
        {
            DMDPoint p = new DMDPoint() { x = x, y = y };
            return p;
        }

        public static DMDSize DMDSizeMake(uint w, uint h)
        {
            DMDSize s = new DMDSize() { width = w, height = h };
            return s;
        }

        public static DMDRect DMDRectMake(uint x, uint y, uint w, uint h)
        {
            DMDRect r = new DMDRect();
            r.origin = new DMDPoint() { x = x, y = y };
            r.size = new DMDSize() { width = w, height = h };
            return r;
        }

        public static uint DMDRectGetMinX(DMDRect r) { return r.origin.x; }
        public static uint DMDRectGetMinY(DMDRect r) { return r.origin.y; }
        public static uint DMDRectGetMaxX(DMDRect r) { return r.origin.x + r.size.width; }
        public static uint DMDRectGetMaxY(DMDRect r) { return r.origin.y + r.size.height; }

        public static byte DMDFrameGetDot(ref DMDFrame frame, DMDPoint p) { return frame.buffer[p.y * frame.size.width + p.x]; }
        public static byte DMDFrameGetDot(ref DMDFrame frame, uint x, uint y) { return frame.buffer[y * frame.size.width + x]; }
        
        public static void DMDFrameSetDot(ref DMDFrame frame, DMDPoint p, byte c) { frame.buffer[p.y * frame.size.width + p.x] = c; }
        public static void DMDFrameSetDot(ref DMDFrame frame, uint x, uint y, byte c) { frame.buffer[y * frame.size.width + x] = c; }

        public static DMDFrame DMDFrameCreate(DMDSize size)
        {
            DMDFrame result = new DMDFrame();
            result.buffer = new byte[size.width * size.height];
            result.size = size;
            return result;
        }

        public static void DMDFrameDelete(ref DMDFrame frame)
        {
            frame.buffer = null;
        }

        public static DMDFrame DMDFrameCopy(ref DMDFrame frame)
        {
            DMDFrame result = DMDFrameCreate(frame.size);
            Array.Copy(frame.buffer, result.buffer, frame.buffer.Length);
            return result;
        }

        public static DMDRect DMDFrameGetBounds(ref DMDFrame frame)
        {
            return DMDRectMake(0, 0, frame.size.width, frame.size.height);
        }

        public static uint DMDFrameGetBufferSize(ref DMDFrame frame)
        {
            return frame.size.width * frame.size.height;
        }

        public static void DMDFrameFillRect(ref DMDFrame frame, DMDRect rect, byte color)
        {
            rect = DMDRectIntersection(DMDFrameGetBounds(ref frame), rect);

            uint maxX = DMDRectGetMaxX(rect);
            uint maxY = DMDRectGetMaxY(rect);

            uint x, y;
            for (x = DMDRectGetMinX(rect); x < maxX; x++)
                for (y = DMDRectGetMinY(rect); y < maxY; y++)
                    DMDFrameSetDot(ref frame, x, y, color);
        }

        public static void DMDFrameCopyRect(ref DMDFrame src, DMDRect srcRect, ref DMDFrame dst, DMDPoint dstPoint, DMDBlendMode blendMode)
        {
            srcRect = DMDRectIntersection(DMDFrameGetBounds(ref src), srcRect);
            DMDRect dstRect = DMDRectIntersection(DMDFrameGetBounds(ref dst),
                                DMDRectMake(dstPoint.x, dstPoint.y, srcRect.size.width, srcRect.size.height));

            if (srcRect.size.width == 0 || srcRect.size.height == 0)
                return; /* Nothing to do */

            uint width = dstRect.size.width;
            uint height = dstRect.size.height;
            uint x, y;
            byte dot;

            if (blendMode == DMDBlendMode.DMDBlendModeCopy)
            {
                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        dot = DMDFrameGetDot(ref src, srcRect.origin.x + x, srcRect.origin.y + y);
                        DMDFrameSetDot(ref dst, dstRect.origin.x + x, dstRect.origin.y + y,
                            dot);
                    }
                }
            }
            else if (blendMode == DMDBlendMode.DMDBlendModeAdd)
            {
                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        byte srcDot = DMDFrameGetDot(ref src, DMDPointMake(srcRect.origin.x, srcRect.origin.y + y));
                        byte dstDot = DMDFrameGetDot(ref dst, DMDPointMake(dstRect.origin.x, dstRect.origin.y + y));
                        DMDFrameSetDot(ref dst, DMDPointMake(dstRect.origin.x, dstRect.origin.y + y), (byte)Math.Min(srcDot + dstDot, 0xF));
                    }
                }
            }
            else if (blendMode == DMDBlendMode.DMDBlendModeBlackSource)
            {
                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        byte srcDot = DMDFrameGetDot(ref src, DMDPointMake(srcRect.origin.x, srcRect.origin.y + y));
                        byte dstDot = DMDFrameGetDot(ref dst, DMDPointMake(dstRect.origin.x, dstRect.origin.y + y));
                        // Only write dots into black dots
                        if ((srcDot & 0xf) != 0)
                        {
                            DMDFrameSetDot(ref dst, DMDPointMake(dstRect.origin.x, dstRect.origin.y + y), (byte)((dstDot & 0xf0) | (srcDot & 0xf)));
                        }
                    }
                }
            }
            else if (blendMode == DMDBlendMode.DMDBlendModeSubtract)
            {
                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        byte srcDot = DMDFrameGetDot(ref src, DMDPointMake(srcRect.origin.x, srcRect.origin.y + y));
                        byte dstDot = DMDFrameGetDot(ref dst, DMDPointMake(dstRect.origin.x, dstRect.origin.y + y));
                        DMDFrameSetDot(ref dst, DMDPointMake(dstRect.origin.x, dstRect.origin.y + y), (byte)Math.Max(srcDot + dstDot, 0xF));
                    }
                }
            }
            else if (blendMode == DMDBlendMode.DMDBlendModeAlpha)
            {
                byte[] alphaMap = DMDGetAlphaMap();

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        byte srcDot = DMDFrameGetDot(ref src, DMDPointMake(srcRect.origin.x, srcRect.origin.y + y));
                        byte dstDot = DMDFrameGetDot(ref dst, DMDPointMake(dstRect.origin.x, dstRect.origin.y + y));
                        //byte v = alphaMap[(byte)srv
                    }
                }
            }
            else if (blendMode == DMDBlendMode.DMDBlendModeAlphaBoth)
            {
            }

        }

        public static DMDRect DMDRectIntersection(DMDRect rect0, DMDRect rect1)
        {
            DMDRect result;

            if (DMDRectGetMaxX(rect0) <= DMDRectGetMinX(rect1) || DMDRectGetMinX(rect0) >= DMDRectGetMaxX(rect1) ||
                DMDRectGetMaxY(rect0) <= DMDRectGetMinY(rect1) || DMDRectGetMinY(rect0) >= DMDRectGetMaxY(rect1))
                return DMDRectMake(0, 0, 0, 0);

            result.origin.x = Math.Max(DMDRectGetMinX(rect0), DMDRectGetMinX(rect1));
            result.origin.y = Math.Max(DMDRectGetMinY(rect0), DMDRectGetMinY(rect1));
            result.size.width = Math.Min(DMDRectGetMaxX(rect0), DMDRectGetMaxX(rect1)) - result.origin.x;
            result.size.height = Math.Min(DMDRectGetMaxY(rect0), DMDRectGetMaxY(rect1)) - result.origin.y;

            return result;
        }

        public static byte[] DMDGetAlphaMap()
        {
            byte[] gAlphaMap = new byte[256 * 256];
            uint src, dst;
            for (src = 0x00; src <= 0xff; src++)
            {
                for (dst = 0x00; dst <= 0xff; dst++)
                {
                    byte src_dot = (byte)(src & 0xf);
                    byte src_a = (byte)(src >> 4);
                    byte dst_dot = (byte)(dst & 0xf);
                    byte dst_a = (byte)(dst >> 4);

                    float i = (float)(src_dot * src_a) / (15.0f * 15.0f);
                    float j = (float)(dst_dot * dst_a * (0xf - src_a)) / (15.0f * 15.0f * 15.0f);

                    byte dot = (byte)((i + j) * 15.0f);
                    byte a = Math.Max(src_a, dst_a);

                    gAlphaMap[src * 256 + dst] = (byte)((a << 4) | (dot & 0xf));
                }
            }
            return gAlphaMap;
        }

        public static void DMDFrameCopyPROCSubframes(ref DMDFrame frame, byte[] dots, uint width, uint height, uint subframes, byte[] colorMap)
        {
            if (subframes != 4)
            {
                throw new Exception("ERROR in DMDFrameCopyPROCSubframes(): subframes must be 4");
            }
            byte[] defaultColorMap = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            /* Color map specific to P-ROC */
            byte[] procColorMap = { 0, 2, 8, 10, 1, 3, 9, 11, 4, 6, 12, 14, 5, 7, 13, 15 };

            if (colorMap != null)
                colorMap = defaultColorMap;

            uint row, col;

            for (row = 0; row < height; row++)
            {
                for (col = 0; col < width; col++)
                {
                    byte dot = DMDFrameGetDot(ref frame, DMDPointMake(col, row));
                    if (dot == 0)
                        continue;
                    else
                    {
                        dot &= 0x0f;
                        dot = colorMap[(int)dot];
                        dot = procColorMap[(int)dot];
                        if ((dot & 0x1) == 0x1) dots[0 * (width * height / 8) + ((row * width + col) / 8)] |= (byte)(1 << (int)(col % 8));
                        if ((dot & 0x2) == 0x2) dots[1 * (width * height / 8) + ((row * width + col) / 8)] |= (byte)(1 << (int)(col % 8));
                        if ((dot & 0x4) == 0x4) dots[2 * (width * height / 8) + ((row * width + col) / 8)] |= (byte)(1 << (int)(col % 8));
                        if ((dot & 0x8) == 0x8) dots[3 * (width * height / 8) + ((row * width + col) / 8)] |= (byte)(1 << (int)(col % 8));
                    }
                }
            }
        }
    }
}
