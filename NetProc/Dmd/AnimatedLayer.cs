using System;
using System.Collections.Generic;
using NetProc.Tools;

namespace NetProc.Dmd
{
    /// <summary>
    /// Collection of frames displayed sequentially as an animation. Optionally holds the last frame on-screen
    /// </summary>
    public class AnimatedLayer : Layer
    {
        /// <summary>
        /// True if the last frame of the animation should be held on-screen indefinitely
        /// </summary>
        public bool hold = true;

        /// <summary>
        /// True if the animation should be repeated indefinitely
        /// </summary>
        public bool repeat = false;

        /// <summary>
        /// Number of frame times each frame should be shown on screen before advancing to the next frame.
        /// </summary>
        public int frame_time = 1;

        /// <summary>
        /// Index of the next frame to display. Incremented by 'next_frame()'
        /// </summary>
        public int frame_pointer = 0;

        private Frame[] frames;
        private List<Pair<int, Delegate>> frame_listeners;
        private int frame_time_counter = 0;

        public AnimatedLayer(bool opaque = false, bool hold = true, bool repeat = false, int frame_time = 1, Frame[] frames = null)
            : base(opaque)
        {
            this.hold = hold;
            this.repeat = repeat;

            this.frames = frames;

            this.frame_time = frame_time;
            this.frame_time_counter = frame_time;
            this.frame_listeners = new List<Pair<int, Delegate>>();
            this.reset();
        }

        /// <summary>
        /// Resets the animation back to the first frame
        /// </summary>
        public override void reset()
        {
            this.frame_pointer = 0;
        }

        /// <summary>
        /// Registers a listener to be called when a specific frame number (frame_index) in the
        /// animation has been reached.
        /// Negative numbers indicate a number of frames from the last frame. That is a frame
        /// index of -1 will trigger on the last frame of the animation.
        /// </summary>
        /// <param name="frame_index"></param>
        /// <param name="listener"></param>
        public void add_frame_listener(int frame_index, Delegate listener)
        {
            Pair<int, Delegate> v = new Pair<int, Delegate>(frame_index, listener);
            frame_listeners.Add(v);
        }

        private void notify_frame_listeners()
        {
            for (int i = 0; i < frame_listeners.Count; i++)
            {
                Pair<int, Delegate> v = frame_listeners[i];
                if (v.First >= 0 && this.frame_pointer == v.First)
                    v.Second.DynamicInvoke();
                else if (this.frame_pointer == (this.frames.Length + v.First))
                    v.Second.DynamicInvoke();
            }
        }

        /// <summary>
        /// Returns the frame to be shown, or null if there is no frame.
        /// </summary>
        /// <returns></returns>
        public override Frame next_frame()
        {
            if (this.frame_pointer >= this.frames.Length) return null;

            /// Important: Notify the frame listeners before the frame_pointer
            /// has been advanced. Only notify the listeners if this is the first time
            /// this frame has been shown (such as if frame_time is > 1)
            if (this.frame_time_counter == this.frame_time)
                this.notify_frame_listeners();

            Frame frame = this.frames[this.frame_pointer];
            this.frame_time_counter--;

            if (this.frames.Length > 1 && this.frame_time_counter == 0)
            {
                if (this.frame_pointer == this.frames.Length - 1)
                {
                    if (this.repeat)
                        this.frame_pointer = 0;
                    else if (!this.hold)
                        this.frame_pointer++;
                }
                else
                    this.frame_pointer++;
            }

            if (this.frame_time_counter == 0)
                this.frame_time_counter = this.frame_time;

            return frame;
        }
    }
}
