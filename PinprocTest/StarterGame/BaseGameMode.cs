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

        public bool sw_bottomPopper_active_for_100ms(Switch sw)
        {
            Game.Coils["bottomPopper"].Pulse();
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

        public bool sw_chaseCar2_active(Switch sw)
        {
            Game.ball_save.add(1);
            return SWITCH_CONTINUE;
        }

        public StarterGame Game
        {
            get { return (StarterGame)base.Game; }
        }
    }
}
