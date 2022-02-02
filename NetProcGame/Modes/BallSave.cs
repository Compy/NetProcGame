using NetProcGame;
using NetProcGame.Events;
using NetProcGame.Game;
using System;

namespace NetProcGame.Modes
{
    public delegate void BallSaveEventHandler();
    public delegate void BallSaveEnable(bool enabled);
    public delegate int GetNumBallsToSaveHandler();
    /// <summary>
    /// Manages a games ball save functionality by keeping track of the ball save timer and the number of balls to be saved.
    /// </summary>
    public class BallSave : Mode
    {
        public string lamp;
        public int num_balls_to_save = 1;
        public int mode_begin = 0;
        public bool allow_multiple_saves = false;
        public int timer = 0;
        public int timer_hold;
        public Delegate callback = null;
        public Delegate trough_enable_ball_save = null;

        public BallSave(IGameController game, string lamp, string delayed_start_switch = "")
            : base(game, 3)
        {
            this.lamp = lamp;
            this.num_balls_to_save = 1;
            this.mode_begin = 0;
            this.allow_multiple_saves = false;
            this.timer = 0;

            if (delayed_start_switch != "")
                AddSwitchHandler(delayed_start_switch, "inactive", 1.0, this.delayed_start_handler);

            // Optional method to be called when a ball is saved. Should be defined externally
            callback = null;

            // Optional method to be called to tell a trough to save balls. Should be linked externally to an enable method for a trough
            trough_enable_ball_save = null;
        }

        public override void ModeStopped()
        {
            this.disable();
        }

        /// <summary>
        /// Disables the ball save logic when multiple saves are not allowed. This is typically linked to a 
        /// trough object so the trough can notify this logic when a ball is being saved.
        /// 
        /// If self.callback is externally defined, that method will be called from here.
        /// </summary>
        public void launch_callback()
        {
            if (!allow_multiple_saves)
                this.disable();

            if (callback != null)
                callback.DynamicInvoke();
        }

        /// <summary>
        /// Starts blinking the ball save lamp. Oftentimes called externally to start blinking a lamp before
        /// the ball is launched.
        /// </summary>
        public void start_lamp()
        {
            Game.Lamps[lamp].Schedule(0xFF00FF00, 0, true);
        }

        public void update_lamp()
        {
            if (timer > 5)
                Game.Lamps[lamp].Schedule(0xFF00FF00, 0, true);
            else if (timer > 2)
                Game.Lamps[lamp].Schedule(0x55555555, 0, true);
            else
                Game.Lamps[lamp].Disable();
        }

        public void add(int add_time, bool allow_multiple_saves = true)
        {
            if (timer >= 1)
            {
                timer += add_time;
                UpdateLamps();
            }
            else
            {
                start(num_balls_to_save, add_time, true, allow_multiple_saves);
            }
        }

        /// <summary>
        /// Disables ball save logic
        /// </summary>
        public void disable()
        {
            if (trough_enable_ball_save != null)
                trough_enable_ball_save.DynamicInvoke(false);
            timer = 0;
            Game.Lamps[lamp].Disable();
        }

        /// <summary>
        /// Activates the ball save logic
        /// </summary>
        public void start(int num_balls_to_save = 1, int time = 12, bool now = true, bool allow_multiple_saves = false)
        {
            this.allow_multiple_saves = allow_multiple_saves;
            this.num_balls_to_save = num_balls_to_save;

            if (time > this.timer) this.timer = time;

            UpdateLamps();

            if (now)
            {
                CancelDelayed("ball_save_timer");
                Delay("ball_save_timer", EventType.None, 1, new AnonDelayedHandler(timer_countdown));
                if (trough_enable_ball_save != null)
                {
                    trough_enable_ball_save.DynamicInvoke(true);
                }
            }
            else
            {
                mode_begin = 1;
                timer_hold = time;
            }

        }

        private void timer_countdown()
        {
            timer--;
            if (timer >= 1)
                Delay("ball_save_timer", EventType.None, 1.0, new AnonDelayedHandler(timer_countdown));
            else
                disable();

            update_lamp();
        }

        public bool is_active()
        {
            return timer > 0;
        }

        /// <summary>
        /// Returns the number of balls to be saved. Typically this is linked to a trough object so the trough
        /// can decide if a draining ball should be saved.
        /// </summary>
        /// <returns></returns>
        public int get_num_balls_to_save()
        {
            return this.num_balls_to_save;
        }

        public void saving_ball()
        {
            if (!allow_multiple_saves)
            {
                timer = 1;
                Game.Lamps[lamp].Disable();
            }
        }

        public bool delayed_start_handler(Switch sw)
        {
            if (mode_begin == 1)
            {
                this.timer = timer_hold;
                this.mode_begin = 0;
                this.UpdateLamps();
                CancelDelayed("ball_save_timer");
                Delay("ball_save_timer", EventType.None, 1.0, new AnonDelayedHandler(timer_countdown));
                if (trough_enable_ball_save != null)
                    trough_enable_ball_save.DynamicInvoke(true);
            }
            return SWITCH_CONTINUE;
        }
    }
}
