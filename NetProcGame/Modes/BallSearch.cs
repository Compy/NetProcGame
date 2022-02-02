using NetProcGame.Events;
using NetProcGame.Game;
using System;
using System.Collections.Generic;

namespace NetProcGame.Modes
{
    public delegate void BallSearchDelayHandler(int completion_wait_time, Delegate completion_handler);
    public delegate void BallSearchCoilDelayHandler(string coil);
    public delegate void RemoveSpecialHandlerDelegate(Mode special_handler_mode);
    public class BallSearch : Mode
    {
        public int countdown_time;
        public string[] coils;
        public Dictionary<string, string> reset_switches;
        public Dictionary<string, string> stop_switches;
        public string[] enable_switch_names;
        public Mode[] special_handler_modes;
        public bool enabled;

        public BallSearch(GameController game, int countdown_time,
            string[] coils = null, Dictionary<string, string> reset_switches = null, Dictionary<string, string> stop_switches = null,
            string[] enable_switch_names = null, Mode[] special_handler_modes = null)
            : base(game, 8)
        {
            if (stop_switches == null) this.stop_switches = new Dictionary<string, string>();
            else this.stop_switches = stop_switches;
            if (coils == null) this.coils = new string[] { };
            else this.coils = coils;
            if (reset_switches == null) this.reset_switches = new Dictionary<string, string>();
            else this.reset_switches = reset_switches;
            if (enable_switch_names == null) this.enable_switch_names = new string[] { };
            else this.enable_switch_names = enable_switch_names;
            if (special_handler_modes == null) this.special_handler_modes = new Mode[] { };
            else this.special_handler_modes = special_handler_modes;

            this.enabled = false;

            foreach (string sw in reset_switches.Keys)
            {
                this.AddSwitchHandler(sw, reset_switches[sw], 0, new SwitchAcceptedHandler(this.reset));
            }
            foreach (string sw in stop_switches.Keys)
            {
                this.AddSwitchHandler(sw, stop_switches[sw], 0, new SwitchAcceptedHandler(this.stop));
            }

        }

        public void enable()
        {
            this.enabled = true;
            this.reset(null);
        }

        public void disable()
        {
            this.stop(null);
            this.enabled = false;
        }

        public bool reset(Switch sw)
        {
            if (this.enabled)
            {
                // Stop delayed coil activations in case a ball search has already started
                this.CancelDelayed("ball_search_coil1");
                this.CancelDelayed("start_special_handler_modes");
                bool schedule_search = true;
                foreach (string swc in this.stop_switches.Keys)
                {
                    if (sw == null) break;
                    // Dont restart the search countdown if a ball is resting on a stop_switch. First
                    // build the appropriate function call into the switch, then call it.
                    string state_str = this.stop_switches[swc];
                    if (state_str == "active" && sw.IsActive()) schedule_search = false;
                    if (state_str == "closed" && sw.IsClosed()) schedule_search = false;
                    if (state_str == "open" && sw.IsOpen()) schedule_search = false;
                    if (state_str == "inactive" && sw.IsInactive()) schedule_search = false;
                }
                if (schedule_search)
                {
                    this.CancelDelayed("ball_search_countdown");
                    this.Delay("ball_search_countdown", EventType.None, this.countdown_time, new BallSearchDelayHandler(this.perform_search));
                }
            }
            return SWITCH_CONTINUE;
        }

        public bool stop(Switch sw)
        {
            this.CancelDelayed("ball_search_countdown");
            return SWITCH_CONTINUE;
        }

        public void perform_search(int completion_wait_time = 0, Delegate completion_handler = null)
        {
            double delay = 0.150;
            foreach (string coil in this.coils)
            {
                this.Delay("ball_search_coil1", EventType.None, delay, new BallSearchCoilDelayHandler(this.pop_coil), coil);
                delay += 0.150;
            }
            this.Delay("start_special_handler_modes", EventType.None, delay, new AnonDelayedHandler(this.start_special_handler_modes));

            if (completion_wait_time != 0) return;
            else
            {
                this.CancelDelayed("ball_search_countdown");
                this.Delay("ball_search_countdown", EventType.None, this.countdown_time, new BallSearchDelayHandler(this.perform_search));
            }
        }

        public void pop_coil(string coil)
        {
            this.Game.Coils[coil].Pulse();
        }

        public void start_special_handler_modes()
        {
            foreach (Mode special_handler_mode in this.special_handler_modes)
            {
                this.Game.Modes.Add(special_handler_mode);
                this.Delay("remove_special_handler_mode", EventType.None, 7, new RemoveSpecialHandlerDelegate(remove_special_handler_mode), special_handler_mode);
            }
        }

        public void remove_special_handler_mode(Mode special_handler_mode)
        {
            this.Game.Modes.Remove(special_handler_mode);
        }
    }
}
