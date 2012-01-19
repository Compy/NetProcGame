using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetProcGame;
using NetProcGame.game;
using NetProcGame.tools;

namespace PCSDiagnostics
{
    public class DiagnosticMode : Mode
    {
        public DiagnosticMode(GameController game)
            : base(game, 1)
        {
            Game.FlippersEnabled = true;
        }

        public bool sw_shooterLane_active_for_1s(Switch sw)
        {
            Game.auto_launch_next_ball = false;
            Game.Coils["ballLaunch"].Pulse();
            return SWITCH_CONTINUE;
        }

        public bool sw_eject_active_for_1s(Switch sw)
        {
            Game.Coils["eject"].Pulse();
            return SWITCH_CONTINUE;
        }

        public bool sw_bottomPopper_active_for_500ms(Switch sw)
        {
            Game.Coils["bottomPopper"].Pulse();
            return SWITCH_CONTINUE;
        }

        public bool sw_topPopper_active_for_500ms(Switch sw)
        {
            Game.Coils["topPopper"].Pulse();
            return SWITCH_CONTINUE;
        }

        public bool sw_elevatorHold_active_for_1s(Switch sw)
        {
            Game.Coils["clawRight"].Pulse(254);
            return SWITCH_CONTINUE;
        }

        public new DiagnosticGame Game
        {
            get { return (DiagnosticGame)base.Game; }
        }
    }
}
