using System;

namespace NetProcGame.Dmd
{
    /// <summary>
    /// Transition base class
    /// </summary>
    public class LayerTransitionBase
    {
        /// <summary>
        /// Transition progress from 0.0 (100% from frame, 0% to frame) to 1.0 (0% from frame, 100% to frame)
        /// updated by 'next_frame()'
        /// </summary>
        public double progress = 0.0;

        /// <summary>
        /// Progress increment for each frame. Default to 1/60 or 60fps
        /// </summary>
        public double progress_per_frame = 1.0 / 60.0;

        public int progress_mult = 0; // Not moving, -1 for B to A, 1 for A to B. play/pause manipulates this

        /// <summary>
        /// Function to be called once the transition has completed
        /// </summary>
        public Delegate completed_handler = null;

        /// <summary>
        /// If true, transition is moving from 'from' to 'to', if false, the transition is moving from 'to' to 'from'
        /// </summary>
        public bool in_out = true;

        /// <summary>
        /// Start the transition
        /// </summary>
        public void start()
        {
            this.reset();
            this.progress_mult = 1;
        }

        /// <summary>
        /// Pauses the transition at the current position
        /// </summary>
        public void pause()
        {
            this.progress_mult = 0;
        }

        /// <summary>
        /// Reset the transition to the beginning
        /// </summary>
        public void reset()
        {
            this.progress_mult = 0;
            this.progress = 0;
        }

        /// <summary>
        /// Applies the transition and increments the progress if the transition is running.
        /// Returns the resulting frame object.
        /// </summary>
        public virtual Frame next_frame(Frame from_frame, Frame to_frame)
        {
            this.progress = Math.Max(0.0, Math.Min(1.0, this.progress + this.progress_mult * this.progress_per_frame));
            if (this.progress <= 0.0)
            {
                if (this.in_out == true)
                    return from_frame;
                else
                    return to_frame;
            }
            if (this.progress >= 1.0)
            {
                if (this.completed_handler != null)
                    this.completed_handler.DynamicInvoke();
                if (this.in_out == true)
                    return to_frame;
                else
                    return from_frame;
            }
            return this.transition_frame(from_frame, to_frame);
        }

        /// <summary>
        /// Applies the transition at the current progress value.
        /// 
        /// Subclasses should override this method to provide more interesting transition effects.
        /// base implementation simply returns the from_frame
        /// </summary>
        public virtual Frame transition_frame(Frame from_frame, Frame to_frame)
        {
            return from_frame;
        }


    }
}
