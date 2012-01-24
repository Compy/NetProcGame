using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NetProcGame;
using NetProcGame.config;
using NetProcGame.game;
using NetProcGame.modes;
using NetProcGame.tools;
using XNAPinProc.Screens;
using XNAPinProc.Middleware;

namespace XNAPinProc.Middleware.Modes
{
    /// <summary>
    /// System mode that runs beneath all other modes at all times
    /// 
    /// The primary function of this mode is to listen for switch events on the inner coindoor control switches
    /// </summary>
    public class SystemBase : Mode
    {
        public SystemBase(GameController game)
            : base(game, 98)
        {
        }

        public bool sw_enter_active(Switch sw)
        {
            return SWITCH_CONTINUE;
        }

        public bool sw_exit_active(Switch sw)
        {
            return SWITCH_CONTINUE;
        }

        public bool sw_up_active(Switch sw)
        {
            return SWITCH_CONTINUE;
        }

        public bool sw_down_active(Switch sw)
        {
            return SWITCH_CONTINUE;
        }

        public new MiddlewareGame Game
        {
            get { return (MiddlewareGame)base.Game; }
        }

    }
}
