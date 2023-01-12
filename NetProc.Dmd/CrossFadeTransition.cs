﻿using NetProc.Interface;
using System.Collections.Generic;

namespace NetProc.Dmd
{
    /// <summary>
    /// Performs a cross-fade between two layers. As one fades out the other fades in.
    /// </summary>
    public class CrossFadeTransition : LayerTransitionBase
    {
        public int width, height;
        List<Frame> frames;
        public CrossFadeTransition(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.progress_per_frame = 1.0 / 45.0;
            
            // Create the frames that will be used in the composite operations
            this.frames = new List<Frame>();
            for (int i = 0; i < 16; i++)
            {
                Frame frame = new Frame(width, height);
                frame.fill_rect(0, 0, width, height, (byte)i);
                this.frames.Add(frame);
            }
        }

        public override IFrame transition_frame(IFrame from_frame, IFrame to_frame)
        {
            // Calculate the frame index
            int index = 0;
            if (this.in_out == true)
                index = (int)(this.progress * (frames.Count - 1));
            else
                index = (int)((1.0 - this.progress) * (frames.Count - 1));

            // Subtract the respective reference frame from each of the input frames
            from_frame = from_frame.Copy();
            Frame.copy_rect((DMDBuffer)from_frame, 0, 0, this.frames[index], 0, 0, this.width, this.height, DMDBlendMode.DMDBlendModeSubtract);
            to_frame = to_frame.Copy();
            Frame.copy_rect((DMDBuffer)to_frame, 0, 0, this.frames[frames.Count - (index + 1)], 0, 0, this.width, this.height, DMDBlendMode.DMDBlendModeSubtract);
            // Add the results together
            Frame.copy_rect((DMDBuffer)from_frame, 0, 0, (DMDBuffer)to_frame, 0, 0, this.width, this.height, DMDBlendMode.DMDBlendModeAdd);
            return from_frame;
        }
    }
}