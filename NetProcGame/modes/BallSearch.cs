using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetProcGame.game;

namespace NetProcGame.modes
{
    public class BallSearch : Mode
    {
        public int countdown_time;
        public string[] coils;
        public string[] reset_switches;
        public string[] stop_switches;
        public string[] enable_switch_names;
        public string[] special_handler_modes;

        public BallSearch(GameController game, int countdown_time,
            string[] coils = null, string[] reset_switches = null, string[] stop_switches = null,
            string[] enable_switch_names = null, string[] special_handler_modes = null)
            : base(game, 8)
        {
            if (stop_switches == null) this.stop_switches = new string[] { };
            else this.stop_switches = stop_switches;
            if (coils == null) this.coils = new string[] { };
            else this.coils = coils;
            if (reset_switches == null) this.reset_switches = new string[] { };
            else this.reset_switches = reset_switches;
            if (enable_switch_names == null) this.enable_switch_names = new string[] { };
            else this.enable_switch_names = enable_switch_names;
            if (special_handler_modes == null) this.special_handler_modes = new string[] { };
            else this.special_handler_modes = enable_switch_names;

        }
    }
}
