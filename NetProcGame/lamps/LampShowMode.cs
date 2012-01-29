using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NetProcGame.game;

namespace NetProcGame.lamps
{
    /// <summary>
    /// Mode subclass that manages a single LampShow class updating it in the mode_tick method
    /// </summary>
    public class LampShowMode : Mode
    {
        public LampShow lampshow;
        public bool show_over;
        public bool repeat;
        public Delegate callback;

        public LampShowMode(GameController game)
            : base(game, 3)
        {
            this.lampshow = new LampShow(game);
            this.show_over = true;
        }

        /// <summary>
        /// Load a new lamp show
        /// </summary>
        public void load(string filename, bool repeat = false, Delegate callback = null)
        {
            this.callback = callback;
            this.repeat = repeat;
            this.lampshow.reset();
            this.lampshow.load(filename);
            this.restart();
        }

        /// <summary>
        /// Restart the lamp show
        /// </summary>
        public void restart()
        {
            this.lampshow.restart();
            this.show_over = false;
        }

        public override void mode_tick()
        {
            if (this.lampshow.is_complete() && !show_over)
            {
                if (this.repeat)
                    this.restart();
                else
                {
                    this.cancel_delayed("show_tick");
                    this.show_over = true;
                    if (this.callback != null)
                        callback.DynamicInvoke();
                }
            }
            else if (!show_over)
            {
                this.lampshow.tick();
            }
        }
    }
}
