using NetProcGame.Dmd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetProcGame.Dmd
{
    /// <summary>
    /// Layer subclass that composites several sublayers together.
    /// </summary>
    public class GroupedLayer : Layer
    {
        /// <summary>
        /// List of layers to be composited together whenever this layer's next_frame() is called.
        /// 
        /// Layers are composited first to last using each layer's composite_next() method.
        /// </summary>
        public List<Layer> layers;

        private Frame buffer;

        public GroupedLayer(int width, int height, List<Layer> layers = null)
        {
            this.buffer = new Frame(width, height);
            if (layers == null)
                this.layers = new List<Layer>();
            else
                this.layers = layers;
        }

        public override void reset()
        {
            foreach (Layer layer in this.layers)
                layer.reset();
        }

        public override Frame next_frame()
        {
            this.buffer.clear();
            int composited_count = 0;
            foreach (Layer layer in this.layers)
            {
                Frame frame = null;
                if (layer.enabled)
                    frame = layer.composite_next(this.buffer);
                if (frame != null)
                    composited_count++;
                if (frame != null && layer.opaque) // If an opaque layer doesn't draw anything, dont stop
                    break;
            }

            if (composited_count == 0)
                return null;

            return this.buffer;
        }
    }
}
