using NetProc.Interface;
using NetProc.Game.Lamps;
using System;

namespace NetProc.Game.Modes
{
    /// <summary>
    /// Mode subclass that manages a single LampShow class updating it in the mode_tick method
    /// </summary>
    public class LampShowMode : Mode, ILampShowMode
    {
        public Delegate Callback;
        public LampShow Lampshow;
        public bool Repeat;
        public bool ShowOver;
        public LampShowMode(IGameController game)
            : base(game, 3)
        {
            this.Lampshow = new LampShow(game);
            this.ShowOver = true;
        }

        /// <summary>
        /// Load a new lamp show
        /// </summary>
        public void Load(string filename, bool repeat = false, Delegate callback = null)
        {
            this.Callback = callback;
            this.Repeat = repeat;
            this.Lampshow.reset();
            this.Lampshow.load(filename);
            this.Restart();
        }

        public override void ModeTick()
        {
            if (this.Lampshow.is_complete() && !ShowOver)
            {
                if (this.Repeat)
                    this.Restart();
                else
                {
                    this.CancelDelayed("show_tick");
                    this.ShowOver = true;
                    if (this.Callback != null)
                        Callback.DynamicInvoke();
                }
            }
            else if (!ShowOver)
            {
                this.Lampshow.tick();
            }
        }

        /// <summary>
        /// Restart the lamp show
        /// </summary>
        public void Restart()
        {
            this.Lampshow.restart();
            this.ShowOver = false;
        }
    }
}
