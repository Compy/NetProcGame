using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetProcGame;
using NetProcGame.modes;
using NetProcGame.game;

namespace PinprocTest.StarterGame
{
    public delegate void VoidDelegateNoArgs();
    public class Attract : Mode
    {
        public Attract(StarterGame game)
            : base(game, 1)
        {

        }

        public override void mode_started()
        {
            // Lamp show
            change_lampshow();

            this.Game.set_status("This is a test", 30);

            // Blinky start button
            Game.Lamps["startButton"].Schedule(0x00ff00ff, 0, false);
        }

        public bool sw_startButton_active(Switch sw)
        {
            if (Game.trough.is_full())
            {
                // Remove attract mode from queue
                Game.Modes.Remove(this);

                // Initialize game
                Game.start_game();

                // Add first player
                Game.add_player();
                Game.start_ball();
            }
            else
            {
                // Perform ball search
                this.Game.set_status("Locating pinballs...", 5);
                Game.Logger.Log("BALL SEARCH");
            }
            return SWITCH_CONTINUE;
        }

        public bool sw_coinDoor_open(Switch sw)
        {
            Game.Logger.Log("COINDOOR OPEN");
            Game.set_status("COIN DOOR OPEN");
            return SWITCH_CONTINUE;
        }

        public bool sw_bottomPopper_active_for_1s(Switch sw)
        {
            Game.Coils["bottomPopper"].Pulse();
            return SWITCH_CONTINUE;
        }

        public bool sw_topPopper_active_for_1s(Switch sw)
        {
            Game.Coils["topPopper"].Pulse();
            return SWITCH_CONTINUE;
        }

        public bool sw_eject_active_for_1s(Switch sw)
        {
            Game.Coils["eject"].Pulse();
            return SWITCH_CONTINUE;
        }

        public bool sw_leftInlane_active_for_1s(Switch sw)
        {
            Game.open_divertor();
            return SWITCH_CONTINUE;
        }

        public bool sw_rightInlane_active_for_1s(Switch sw)
        {
            Game.close_divertor();
            return SWITCH_CONTINUE;
        }

        public new StarterGame Game
        {
            get { return (StarterGame)base.Game; }
        }

        public void change_lampshow()
        {
            Random r = new Random();
            List<string> lampshow_keys = Enumerable.ToList<string>(Game.lampctrl.shows.Keys);
            string next_show = lampshow_keys[r.Next(lampshow_keys.Count - 1)];
            Game.lampctrl.play_show(next_show, true);
            this.delay("lampshow", EventType.None, 10, new VoidDelegateNoArgs(change_lampshow));
        }
    }
}
