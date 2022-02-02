using System;
using System.Collections.Generic;
using NetProcGame.Dmd;
using NetProcGame.Tools;

namespace NetProcGame.Dmd
{
    /// <summary>
    /// Displays a set of layers based on a simple script.
    /// 
    /// **Script Format**
    /// 
    /// The script is a list of dictionaries. Each dictionary contains two keys (seconds, layer)
    /// 'seconds' is the number of seconds that 'layer' will be displayed before advancing to the
    /// next script element.
    /// 
    /// If 'layer' is null, no frame will be returned by this layer for the duration of that script element.
    /// </summary>
    public class ScriptedLayer : Layer
    {
        private Frame buffer;
        private List<Pair<int, Layer>> script;
        private int script_index = 0;
        private double frame_start_time = -1;
        private Direction force_direction = Direction.None;
        private Delegate on_complete = null;
        private bool is_new_script_item = true;
        private Layer last_layer = null;

        public ScriptedLayer(int width, int height, List<Pair<int, Layer>> script)
        {
            this.buffer = new Frame(width, height);
            this.script = script;
            this.script_index = 0;
            this.frame_start_time = -1;
            this.force_direction = Direction.None;
            this.on_complete = null;
            this.is_new_script_item = true;
            this.last_layer = null;
        }

        public override Frame next_frame()
        {
            Layer layer;
            if (this.frame_start_time == -1)
                this.frame_start_time = Time.GetTime();

            Pair<int, Layer> script_item = this.script[(int)this.script_index];
            double time_on_frame = Time.GetTime() - this.frame_start_time;

            // If we are being forced to the next frame, or if the current script item has expired
            if (this.force_direction != Direction.None || time_on_frame > script_item.First)
            {
                this.last_layer = script_item.Second;

                // Update the script index
                if (this.force_direction == Direction.Backward)
                    if (this.script_index == 0)
                        this.script_index = (int)this.script.Count - 1;
                    else
                        this.script_index--;
                else
                    if (this.script_index == this.script.Count)
                        this.script_index = 0;
                    else
                        this.script_index++;

                // Only force one item
                this.force_direction = Direction.None;

                // If we are at the end of the script, reset to the beginning
                if (this.script_index == this.script.Count)
                {
                    this.script_index = 0;
                    if (this.on_complete != null)
                        this.on_complete.DynamicInvoke();
                }

                // Assign the new script item
                script_item = this.script[(int)script_index];
                this.frame_start_time = Time.GetTime();

                layer = script_item.Second;
                if (layer != null)
                    layer.reset();
                this.is_new_script_item = true;
            }
            // Composite the current script item's layer
            layer = script_item.Second;
            // Do layer transitions here

            this.is_new_script_item = false;

            if (layer != null)
            {
                //this.buffer.clear();
                Array.Clear(this.buffer.frame.buffer, 0, this.buffer.frame.buffer.Length);

                // If the layer is opaque we can composite the last layer onto our buffer first.
                // This will allow us to do transitions between script frames
                //if (this.last_layer != null && this.opaque)
                //    this.last_layer.composite_next(this.buffer);

                layer.composite_next(this.buffer);
                return this.buffer;
            }
            else
            {
                // if this script item has null set for its layer, return null (transparent)
                return null;
            }

        }
        public void force_next(Direction direction = Direction.Forward)
        {
            this.force_direction = direction;
        }
    }
    public enum Direction
    {
        Forward,
        Backward,
        None
    }
}
