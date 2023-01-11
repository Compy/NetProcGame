using NetProc.Interface;

namespace NetProc.Dmd
{
    /// <summary>
    /// The Layer class is the basis for the pyprocgame display architecture.
	/// Subclasses override next_frame() to provide a frame for the current moment in time.
	/// Handles compositing of provided frames and applying transitions within a DisplayController context.
    /// </summary>
    public class Layer : ILayer
    {
        /// <summary>
        /// Determines whether layers below this one will be rendered.
        /// If 'true', the DisplayController will not render any layers after this one
        /// (such as from modes with lower priorities)
        /// </summary>
        public bool Opaque { get; set; }

        /// <summary>
        /// Base 'x' component of the coordinates at which this layer will be composited upon a target buffer
        /// </summary>
        public int target_x = 0;

        /// <summary>
        /// Base 'y' component of the coordinates at which this layer will be composited upon a target buffer
        /// </summary>
        public int target_y = 0;

        /// <summary>
        /// Translation component used in addition to 'target_x' as this layer's final compositing position
        /// </summary>
        public int target_x_offset = 0;

        /// <summary>
        /// Translation component used in addition to 'target_y' as this layer's final compositing position
        /// </summary>
        public int target_y_offset = 0;

        /// <summary>
        /// If false, the DisplayController will ignore this layer
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// The composite operation used by composite_next when calling DMDBuffer.copy_rect
        /// </summary>
        public DMDBlendMode composite_op = DMDBlendMode.DMDBlendModeCopy;

        /// <summary>
        /// Transition which composite_next() applies to the result of next_frame prior to compositing upon the target buffer
        /// </summary>
        public LayerTransitionBase transition = null;

        public Layer(bool opaque = false)
        {
            this.Opaque = opaque;
            this.set_target_position(0, 0);
        }

        public virtual void reset()
        {
        }

        public virtual void set_target_position(int x, int y)
        {
            this.target_x = x;
            this.target_y = y;
        }

        /// <summary>
        /// Returns an instance of a Frame object to be shown or null if there is no frame.
        /// The default implementation returns null, subclasses should implement this method.
        /// </summary>
        public virtual IFrame next_frame()
        {
            return null;
        }

        /// <summary>
        /// Composites the next frame of this layer onto the given target buffer.
        /// Called by DisplayController.update()
        /// Generally subclasses should not override this method. Implement next_frame() instead
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public virtual IFrame composite_next(IFrame target)
        {
            var src = next_frame() as Frame;
            if (src != null)
            {
                if (transition != null)
                {
                    src = this.transition.next_frame(target, src) as Frame;
                }
                // src not all zeroes
                // Target = all zeros here
                Frame.copy_rect(target as DMDBuffer, (int)(this.target_x + this.target_x_offset), (int)(this.target_y + this.target_y_offset), src, 0, 0, src.width, src.height, this.composite_op);
            }
            return src;
        }
    }
}
