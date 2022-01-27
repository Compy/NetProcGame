using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetProcGame;
using NetProcGame.game;
using NetProcGame.tools;

namespace NetProcGameTest.StarterGame
{
    public class TestMode : Mode
    {
        private const int STROBE_INTERVAL = 3;

        private Dictionary<string, Driver> _trap_switch_coils;

        public TestMode(GameController game, Dictionary<string, Driver> drivers)
            : base(game, 90)
        {
            if (drivers != null)
                _trap_switch_coils = drivers;
            else
                _trap_switch_coils = new Dictionary<string, Driver>();

            /*
            this.add_switch_handler("trough1", "closed", 3, 
                new SwitchAcceptedHandler(delegate(Switch s) {
                    game.Coils["trough"].Pulse(); 
                    return SWITCH_CONTINUE; 
                }));
             */
        }

        public override void mode_started()
        {
            // Illuminate GI
            flicker_gi();
        }

        public override void mode_tick()
        {
            /* double currentTime = Time.GetTime();
            if ((currentTime - _last_strobe) >= STROBE_INTERVAL && !_is_checking)
            {
                _is_checking = true;
                strobe_game();
                _last_strobe = currentTime;
                _is_checking = false;
            }
             */
        }

        private void strobe_game()
        {
            foreach (string switch_name in _trap_switch_coils.Keys)
            {
                if (Game.Switches[switch_name].IsActive())
                {
                    Game.Logger.Log(switch_name + " is active. Ejecting ball.");
                    _trap_switch_coils[switch_name].Pulse();
                }
            }
        }

        public bool sw_leftInlane_active_for_3s(Switch sw)
        {
            return SWITCH_CONTINUE;
        }

        public bool sw_ballLaunch_active(Switch sw)
        {
            if (Game.Switches["shooterLane"].IsActive())
            {
                // Flicker all GI lights
                foreach (Driver d in Game.GI.Values)
                {
                    d.Schedule(0xaaaaaaaa, 1, true);
                }
                delay("all_gi_on", EventType.Invalid, 1, new AnonDelayedHandler(all_gi_on), null);
                
                Game.Coils["ballLaunch"].Pulse();
            }

            return SWITCH_CONTINUE;
        }

        public void all_gi_on()
        {
            foreach (Driver d in Game.GI.Values)
            {
                d.Enable();
            }
        }
        public void all_gi_off()
        {
            foreach (Driver d in Game.GI.Values)
            {
                d.Disable();
            }
        }

        public bool sw_startButton_active(Switch sw)
        {
            Game.FlippersEnabled = true;
            Game.Coils["trough"].Pulse();

            return SWITCH_CONTINUE;
        }

        public bool sw_buyIn_active(Switch sw)
        {
            Game.FlippersEnabled = false;
            return SWITCH_CONTINUE;
        }

        public bool sw_bottomPopper_active(Switch sw)
        {
            this.all_gi_off();
            Game.Coils["sideRampFlasher"].Schedule(0x0000aaaa, 1, true);
            Game.Coils["rightRampFlasher"].Schedule(0x00009999, 1, true);
            return SWITCH_CONTINUE;
        }

        public bool sw_bottomPopper_active_for_500ms(Switch sw)
        {
            this.all_gi_on();
            Game.Coils["bottomPopper"].Pulse();
            return SWITCH_CONTINUE;
        }

        public bool sw_topPopper_active_for_500ms(Switch sw)
        {
            Game.Coils["topPopper"].Pulse();
            return SWITCH_CONTINUE;
        }

        public bool sw_eject_active(Switch sw)
        {
            Game.Coils["ejectFlasher"].Schedule(0x0000aaaa, 1, true);
            return SWITCH_CONTINUE;
        }

        public bool sw_eject_active_for_500ms(Switch sw)
        {
            Game.Coils["eject"].Pulse();
            return SWITCH_CONTINUE;
        }

        public bool sw_standUp5_active(Switch sw)
        {
            toggle_lamp(Game.Lamps["standup5"]);
            return SWITCH_CONTINUE;
        }

        public bool sw_standUp4_active(Switch sw)
        {
            toggle_lamp(Game.Lamps["standup4"]);
            return SWITCH_CONTINUE;
        }

        public bool sw_standUp3_active(Switch sw)
        {
            toggle_lamp(Game.Lamps["standup3"]);
            return SWITCH_CONTINUE;
        }
        public bool sw_standUp2_active(Switch sw)
        {
            toggle_lamp(Game.Lamps["standup2"]);
            return SWITCH_CONTINUE;
        }
        public bool sw_standUp1_active(Switch sw)
        {
            toggle_lamp(Game.Lamps["standup1"]);
            return SWITCH_CONTINUE;
        }
        public bool sw_leftInlane_active(Switch sw)
        {
            toggle_lamp(Game.Lamps["accessClaw"]);
            return SWITCH_CONTINUE;
        }
        public bool sw_rightInlane_active(Switch sw)
        {
            toggle_lamp(Game.Lamps["lightQuickFreeze"]);
            return SWITCH_CONTINUE;
        }

        public void flicker_gi()
        {
            foreach (Driver d in Game.GI.Values)
            {
                d.Schedule(0xefffefff, 20, true);
            }
        }

        private void toggle_lamp(Driver d)
        {
            if (d.State.State)
            {
                d.Disable();
            }
            else
            {
                d.Enable();
            }
        }
    }
}
