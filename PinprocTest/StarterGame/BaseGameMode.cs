using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NetProcGame;
using NetProcGame.game;
using NetProcGame.modes;

namespace PinprocTest.StarterGame
{
    public class BaseGameMode : Mode
    {
        public bool ball_starting = false;

        public BaseGameMode(GameController game)
            : base(game, 2)
        {
            ball_starting = true;

        }

        public override void mode_started()
        {
            // Disable any previously active lamp
            foreach (Driver lamp in Game.Lamps.Values)
            {
                lamp.Disable();
            }

            foreach (Driver lamp in Game.GI.Values)
                lamp.Enable();
            
            // Enable flippers
            Game.FlippersEnabled = true;

            // Put the ball into play and start tracking it
            //Game.trough.onBallLaunched += new LaunchCallbackHandler(ball_launch_callback);
            Game.trough.launch_callback = new AnonDelayedHandler(ball_launch_callback);
            Game.trough.drain_callback = new AnonDelayedHandler(ball_drained_callback);
            Game.trough.ball_save_callback = new AnonDelayedHandler(ball_saved_callback);
            // Enable ball search in case a ball gets stuck

            // In case a higher priority mode doesn't install its own ball drain handler
            //Game.trough.onBallDrained += new DrainCallbackHandler(ball_drained_callback);

            // Each time this mode is added, ball_starting should be set to true
            ball_starting = true;

            Game.trough.launch_balls(1, null, false);
        }

        public void ball_saved_callback()
        {
            Game.ball_being_saved = true;
        }
        public void ball_launch_callback()
        {
            if (ball_starting)
                ((StarterGame)Game).ball_save.start_lamp();
        }

        public override void mode_stopped()
        {
            // Ensure flippers are disabled
            Game.FlippersEnabled = false;
            // Disable ball search
        }

        public void ball_drained_callback()
        {
            if (Game.trough.num_balls_in_play == 0)
            {
                finish_ball();
            }
        }

        public void finish_ball()
        {
            // Turn off tilt display if it was on now that the ball was drained
            end_ball();
        }

        public void end_ball()
        {
            // Tell the game object it can process the end of ball (to end the players turn or shoot again)
            Game.end_ball();
        }

        public bool sw_startButton_active(Switch sw)
        {
            if (Game.ball == 1)
            {
                Player p = Game.add_player();
                // Display a nice message saying the player has been added, or play a sound
            }
            return SWITCH_CONTINUE;
        }

        public bool sw_shooterLane_active_for_500ms(Switch sw)
        {
            if (Game.ball_being_saved)
            {
                Game.Coils["ballLaunch"].Pulse();
                Game.ball_being_saved = false;
            }

            return SWITCH_CONTINUE;
        }

        public bool sw_shooterLane_open_for_1s(Switch sw)
        {
            if (ball_starting)
            {
                ball_starting = false;
                Game.ball_save.start(1, 10, true, false);
            }
            return SWITCH_CONTINUE;
        }

        public bool sw_ballLaunch_active(Switch sw)
        {
            if (Game.Switches["shooterLane"].IsActive())
            {
                Game.Coils["ballLaunch"].Pulse();
            }

            return SWITCH_CONTINUE;
        }

        public bool sw_elevatorHold_open_for_1s(Switch sw)
        {
            Game.Modes.Add(((StarterGame)Game).claw);
            return SWITCH_CONTINUE;
        }

        public bool sw_topPopper_active_for_100ms(Switch sw)
        {
            Game.Coils["topPopper"].Pulse();
            return SWITCH_CONTINUE;
        }

        public bool sw_eject_active_for_1s(Switch sw)
        {
            Game.Coils["eject"].Pulse();
            return SWITCH_CONTINUE;
        }

        public bool sw_leftSlingshot_active(Switch sw) { Game.score(500); return SWITCH_CONTINUE; }
        public bool sw_rightSlingshot_active(Switch sw) { Game.score(500); return SWITCH_CONTINUE; }
        public bool sw_leftJet_active(Switch sw) { Game.score(1000); return SWITCH_CONTINUE; }
        public bool sw_rightJet_active(Switch sw) { Game.score(1000); return SWITCH_CONTINUE; }
        public bool sw_bottomJet_active(Switch sw) { Game.score(1000); return SWITCH_CONTINUE; }
        public bool sw_leftOutlane_active(Switch sw) { Game.score(100); return SWITCH_CONTINUE; }
        public bool sw_rightOutlane_active(Switch sw) { Game.score(100); return SWITCH_CONTINUE; }
        public bool sw_rightInlane_active(Switch sw) { Game.score(100); return SWITCH_CONTINUE; }
        public bool sw_standUp5_active(Switch sw) { Game.score(1500); return SWITCH_CONTINUE; }
        public bool sw_standUp4_active(Switch sw) { Game.score(1500); return SWITCH_CONTINUE; }
        public bool sw_standUp3_active(Switch sw) { Game.score(1500); return SWITCH_CONTINUE; }
        public bool sw_standUp2_active(Switch sw) { Game.score(1500); return SWITCH_CONTINUE; }
        public bool sw_standUp1_active(Switch sw) { Game.score(1500); return SWITCH_CONTINUE; }
        public bool sw_rightRampExit_active(Switch sw) { Game.score(5000); return SWITCH_CONTINUE; }
        public bool sw_leftRampExit_active(Switch sw) { Game.score(5000); return SWITCH_CONTINUE; }
        public bool sw_sideRampExit_active(Switch sw) { Game.score(5000); return SWITCH_CONTINUE; }
        public bool sw_leftRollOver_active(Switch sw) { Game.score(5000); return SWITCH_CONTINUE; }
        public bool sw_centerRollOver_active(Switch sw) { Game.score(5000); return SWITCH_CONTINUE; }
        public bool sw_rightRollOver_active(Switch sw) { Game.score(5000); return SWITCH_CONTINUE; }
        public bool sw_topPopper_active(Switch sw) { Game.score(500000); return SWITCH_CONTINUE; }
        public bool sw_eyeballStandup_active(Switch sw) { Game.score(1000000); return SWITCH_CONTINUE; }
        public bool sw_centerRamp_active(Switch sw) { Game.score(6000); return SWITCH_CONTINUE; }
        public bool sw_leftLoop_active(Switch sw) { Game.score(1000); return SWITCH_CONTINUE; }

        public bool sw_eject_active(Switch sw) {
            Game.score(50000);
            Game.Coils["ejectFlasher"].Schedule(0xaaaaaaaa, 1, true);
            return SWITCH_CONTINUE; 
        }

        public bool sw_bottomPopper_active(Switch sw) {
            ((StarterGame)Game).all_gi_off();
            Game.Coils["sideRampFlasher"].Schedule(0x0000aaaa, 1, true);
            Game.Coils["rightRampFlasher"].Schedule(0x00009999, 1, true);
            Game.score(800000); 
            return SWITCH_CONTINUE; 
        }
        public bool sw_bottomPopper_active_for_500ms(Switch sw)
        {
            ((StarterGame)Game).all_gi_on();
            Game.Coils["bottomPopper"].Pulse();
            Game.Lamps["accessClaw"].Enable();
            return SWITCH_CONTINUE;
        }

        public bool sw_leftInlane_active(Switch sw) { 
            Game.score(100); 

            // If access claw is lit, open the divertor if not already
            if (Game.Lamps["accessClaw"].State.State && !Game.Coils["divertorHold"].State.State)
            {
                // Open divertor
                Game.Coils["divertorHold"].Pulse(0);
                Game.Coils["divertorMain"].Pulse(30);

                // Turn off lamp
                Game.Lamps["accessClaw"].Disable();

            }

            return SWITCH_CONTINUE; 
        }

        public bool sw_chaseCar1_active(Switch sw)
        {
            Game.score(550);
            return SWITCH_CONTINUE;
        }

        public bool sw_chaseCar2_active(Switch sw)
        {
            Game.score(1100);
            Game.ball_save.add(1);
            return SWITCH_CONTINUE;
        }

        public new StarterGame Game
        {
            get { return (StarterGame)base.Game; }
        }
    }
}
