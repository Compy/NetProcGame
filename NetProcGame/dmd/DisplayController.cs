using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NetProcGame.game;

namespace NetProcGame.dmd
{
    public delegate void DMDFrameHandler(Frame frame);
    /// <summary>
    /// Manages the process of obtaining DMD frames from active modes and compositing them together for
    /// display on the DMD.
    /// 
    /// 1. Add a DisplayController instance to your class (GameController subclass)
    /// 
    /// 2. In your subclass' method dmd_event(), call DisplayController.update() (this.dmd.update();)
    /// </summary>
    public class DisplayController
    {
        /// <summary>
        /// If set, frames obtained by update() will be sent to the functions in this list
        /// with the frame as the only parameter.
        /// 
        /// This list is initialized to contain only dmd_draw()
        /// </summary>
        public List<DMDFrameHandler> frame_handlers;

        private GameController game;
        private TextLayer message_layer;
        private uint width = 0;
        private uint height = 0;

        public DisplayController(GameController game, uint width = 128, uint height = 32, Font message_font = null)
        {
            this.game = game;
            this.width = width;
            this.height = height;
            if (message_font != null)
                this.message_layer = new TextLayer(width / 2, height - 2 * 7, message_font, FontJustify.Center);

            // Do two updates to get the "pump primed" ? -- Yeah.
            for (int i = 0; i < 2; i++)
                this.update();

            this.frame_handlers = new List<DMDFrameHandler>();

            this.frame_handlers.Add(new DMDFrameHandler(game.PROC.dmd_draw));
        }

        public void set_message(string message, int seconds)
        {
            if (this.message_layer == null)
                throw new Exception("message_font must be specified in constructor to enable message layer.");

            game.Logger.Log("Setting message layer on DC");
            this.message_layer.set_text(message, seconds);
        }

        /// <summary>
        /// Iterates over 'GameController.Modes' from lowest to highest and composites a DMD image for this
        /// point in time by checking for a layer attribute on each Mode class.
        /// 
        /// If the mode has a layer attribute, that layer's composite_next method is called to apply that layer's
        /// next frame to the frame in progress.
        /// </summary>
        public void update()
        {
            List<Layer> layers = new List<Layer>();
            foreach (Mode mode in this.game.Modes.Modes)
            {
                if (mode.layer != null)
                {
                    layers.Add(mode.layer);
                    if (mode.layer.opaque) break;
                }
            }

            Frame frame = new Frame(this.width, this.height);
            for (int i = layers.Count - 1; i >= 0; i--)
            {
                if (layers[i].enabled)
                    layers[i].composite_next(frame);
            }
            if (this.message_layer != null)
            {
                this.message_layer.composite_next(frame);
            }

            if (frame != null && this.frame_handlers != null)
            {
                foreach (DMDFrameHandler handler in this.frame_handlers)
                {
                    handler.DynamicInvoke(frame);
                }
            }
        }
    }
}
