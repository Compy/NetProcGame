using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NetProcGame.game;
using NetProcGame.tools;

namespace NetProcGame.lamps
{
    /// <summary>
    /// Controller object that encapsulates a LampShow class and helps to restore lamp drivers to their
    /// prior state.
    /// </summary>
    public class LampController
    {
        struct LampStateRecord
        {
            public double time;
            public DriverState state;

            public LampStateRecord(double time, DriverState state)
            {
                this.time = time;
                this.state = state;
            }
        }

        struct SavedLampState
        {
            public Dictionary<string, LampStateRecord> lamp_states;
            public double time_saved;
        }

        /// <summary>
        /// LampShowMode that must be added to the mode queue
        /// </summary>
        LampShowMode show = null;

        /// <summary>
        /// True if a show is currently playing
        /// </summary>
        public bool show_playing = false;

        /// <summary>
        /// List of shows to load (key, filepath). 
        /// Used to create LampShow objects
        /// </summary>
        public Dictionary<string, string> shows;

        private Dictionary<string, SavedLampState> saved_state_dicts;

        private GameController game;
        public bool resume_state = false;
        public string resume_key = "";

        public LampController(GameController game)
        {
            this.game = game;
            this.show = new LampShowMode(game);
            this.saved_state_dicts = new Dictionary<string, SavedLampState>();
            this.shows = new Dictionary<string, string>();
        }

        public void register_show(string key, string show_file)
        {
            this.shows.Add(key, show_file);
        }

        public void play_show(string key, bool repeat = false, Delegate callback = null)
        {
            // Always stop any previously running show first
            this.stop_show();
            this.show.load(this.shows[key], repeat, callback);
            this.game.Modes.Add(this.show);
            this.show_playing = true;
        }

        public void restore_callback()
        {
            this.resume_state = false;
            this.restore_state(this.resume_key);
            //this.callback() ?
        }

        public void stop_show()
        {
            if (show_playing)
                this.game.Modes.Remove(this.show);
            this.show_playing = false;
        }

        public void save_state(string key)
        {
            SavedLampState state = new SavedLampState();
            state.time_saved = Time.GetTime();
            foreach (Driver lamp in this.game.Lamps.Values)
            {
                state.lamp_states.Add(lamp.Name, new LampStateRecord(lamp.last_time_changed, lamp.State));
            }

            if (this.saved_state_dicts.ContainsKey(key))
            {
                this.saved_state_dicts[key] = state;
            }
            else
                this.saved_state_dicts.Add(key, state);
        }

        public void restore_state(string key)
        {
            if (this.saved_state_dicts.ContainsKey(key))
            {
                int duration = 0;
                SavedLampState state = saved_state_dicts[key];
                foreach (string lamp_name in state.lamp_states.Keys)
                {
                    if (!lamp_name.StartsWith("gi0"))
                    {
                        double time_remaining = (state.lamp_states[lamp_name].state.OutputDriveTime + state.lamp_states[lamp_name].time) - state.time_saved;
                        // Disable the lamp if it has never been used or if there would have been
                        // less than 1 second of drive time when the state was saved
                        if ((state.lamp_states[lamp_name].time == 0 || time_remaining < 1.0)
                            && state.lamp_states[lamp_name].state.OutputDriveTime != 0)
                            this.game.Lamps[lamp_name].Disable();
                        else
                        {
                            // Otherwise, resume the lamp
                            if (state.lamp_states[lamp_name].state.OutputDriveTime == 0)
                            {
                                duration = 0;
                            }
                            else
                            {
                                duration = (int)time_remaining;
                            }
                            if (state.lamp_states[lamp_name].state.Timeslots == 0)
                                this.game.Lamps[lamp_name].Disable();
                            else
                            {
                                this.game.Lamps[lamp_name].Schedule(state.lamp_states[lamp_name].state.Timeslots,
                                    duration,
                                    state.lamp_states[lamp_name].state.WaitForFirstTimeSlot);
                            }
                        }
                    }
                }
            }
        }
    }
}
