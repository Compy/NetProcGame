using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NetProcGame.modes;
using NetProcGame.game;

namespace PinprocTest.StarterGame
{
    public class Claw : Mode
    {
        private bool isClawMoving, isOrienting, isModeEnding, isElevatorMoving = false;
        public Claw(GameController game)
            : base(game, 2)
        {

        }

        public override void mode_started()
        {
            this.isClawMoving = false;
            this.isModeEnding = false;
            this.isOrienting = false;
            this.isElevatorMoving = false;

            if (Game.ball != 0)
            {

                // Disable the flippers
                this.Game.FlippersEnabled = false;

                // Close the divertor
                this.Game.Coils["divertorHold"].Disable();
                // Move the claw all the way over to the right
                this.move_claw_alltheway_right();

                // Wait about 3 seconds and pick the ball up
                this.delay("pickBallUp", NetProcGame.EventType.None, 3, new AnonDelayedHandler(this.pickBallUp));
            }
        }

        public override void mode_stopped()
        {
        }

        /// <summary>
        /// Activate the coil until elevatorHold is closed
        /// </summary>
        public void orient()
        {
            this.isOrienting = true;
            // Run through a full cycle of elevator+claw+pickupball+dropball
            this.move_claw_alltheway_right();
            this.delay("pickBallUp", NetProcGame.EventType.None, 3, new AnonDelayedHandler(this.pickBallUp));
        }

        public void cycle_elevator()
        {
            this.isElevatorMoving = true;
            this.Game.Coils["elevatorMotor"].Pulse(0);
            this.delay("elevCallback", NetProcGame.EventType.None, 2, new AnonDelayedHandler(this.elevatorTimeout));
        }

        private void elevatorTimeout()
        {
            this.Game.Coils["elevatorMotor"].Disable();
            this.isElevatorMoving = false;
        }

        public bool sw_elevatorHold_closed(Switch sw)
        {
            this.Game.Coils["elevatorMotor"].Disable();
            return SWITCH_CONTINUE;
        }

        public void move_claw_alltheway_left()
        {
            if (this.Game.Switches["clawPos2"].IsActive()) return;
            this.Game.Coils["clawLeft"].Pulse(0);
            this.delay("safetyTimeout", NetProcGame.EventType.None, 4, new AnonDelayedHandler(this.safetyClawTimeout));
        }

        public void move_claw_alltheway_right()
        {
            if (this.Game.Switches["clawPos1"].IsActive()) return;
            this.Game.Coils["clawRight"].Pulse(0);
            this.delay("safetyTimeout", NetProcGame.EventType.None, 4, new AnonDelayedHandler(this.safetyClawTimeout));
        }

        private void safetyClawTimeout()
        {
            this.Game.Coils["clawRight"].Disable();
            this.Game.Coils["clawLeft"].Disable();
        }

        public bool sw_leftHandle_closed(Switch sw)
        {
            if (this.Game.Switches["clawPos2"].IsActive()) return SWITCH_CONTINUE;

            this.Game.Coils["clawLeft"].Pulse(0);
            return SWITCH_CONTINUE;
        }

        public bool sw_leftHandle_open(Switch sw)
        {
            this.Game.Coils["clawLeft"].Disable();
            return SWITCH_CONTINUE;
        }

        public bool sw_ballLaunch_closed(Switch sw)
        {
            dropBall();
            return SWITCH_CONTINUE;
        }

        public bool sw_rightHandle_closed(Switch sw)
        {
            if (this.Game.Switches["clawPos1"].IsActive()) return SWITCH_CONTINUE;

            this.Game.Coils["clawRight"].Pulse(0);
            return SWITCH_CONTINUE;
        }

        public bool sw_rightHandle_open(Switch sw)
        {
            this.Game.Coils["clawRight"].Disable();
            return SWITCH_CONTINUE;
        }

        public bool sw_clawPos2_open(Switch sw)
        {
            this.Game.Coils["clawLeft"].Disable();
            this.Game.Coils["clawRight"].Pulse(50);

            if (this.isOrienting)
            {
                this.dropBall();
                this.isOrienting = false;
                this.Game.Modes.Remove(this);
            }
            return SWITCH_CONTINUE;
        }

        public bool sw_clawPos1_open(Switch sw)
        {
            this.Game.Coils["clawRight"].Disable();
            return SWITCH_CONTINUE;
        }

        public bool sw_clawAcmag_closed(Switch sw)
        {
            this.endMode();
            return SWITCH_CONTINUE;
        }
        public bool sw_clawFreeze_closed(Switch sw)
        {
            this.endMode();
            return SWITCH_CONTINUE;
        }
        public bool sw_clawPrisonBreak_closed(Switch sw)
        {
            this.endMode();
            return SWITCH_CONTINUE;
        }
        public bool sw_clawSuperJets_closed(Switch sw)
        {
            this.endMode();
            return SWITCH_CONTINUE;
        }
        public bool sw_clawCaptureSimon_closed(Switch sw)
        {
            this.endMode();
            return SWITCH_CONTINUE;
        }

        public void endMode()
        {
            this.isModeEnding = true;
            this.move_claw_alltheway_left();
            this.delay("endMode", NetProcGame.EventType.None, 3, new AnonDelayedHandler(this.doEndMode));
        }

        private void doEndMode()
        {
            this.Game.Coils["clawLeft"].Disable();
            this.Game.Coils["clawRight"].Disable();
            this.Game.Coils["clawMagnet"].Disable();
            this.Game.Modes.Remove(this);
        }

        public void moveBall()
        {
            this.Game.Coils["clawLeft"].Pulse(0);

            if (this.isOrienting == false)
            {
                this.delay("stopClaw", NetProcGame.EventType.None, 1, new AnonDelayedHandler(this.Game.Coils["clawLeft"].Disable));
                this.delay("dropBall", NetProcGame.EventType.None, 8, new AnonDelayedHandler(this.dropBall));
            }
            else
            {
                this.delay("stopClaw", NetProcGame.EventType.None, 4, new AnonDelayedHandler(this.Game.Coils["clawLeft"].Disable));
                this.delay("dropBall", NetProcGame.EventType.None, 4, new AnonDelayedHandler(this.dropBall));
            }
        }

        public void pickBallUp()
        {
            // Enable claw magnet
            this.Game.Coils["clawMagnet"].Pulse(0);
            // Move elevator up and back down to grab the ball
            this.cycle_elevator();
            this.delay("moveBall", NetProcGame.EventType.None, 2, new AnonDelayedHandler(this.moveBall));
        }

        public void dropBall()
        {
            this.Game.Coils["clawMagnet"].Disable();
            if (this.Game.ball != 0)
            {
                this.Game.FlippersEnabled = true;
            }
        }
    }
}
