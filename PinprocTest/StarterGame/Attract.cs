using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetProcGame;
using NetProcGame.modes;
using NetProcGame.game;
using NetProcGame.dmd;
using NetProcGame.tools;

namespace PinprocTest.StarterGame
{
    public delegate void VoidDelegateNoArgs();
    public class Attract : Mode
    {
        private TextLayer testfontlayer_04B37, testfontlayer_07x4, testfontlayer_07x5, testfontlayer_09Bx7,
            testfontlayer_09x5, testfontlayer_09x6, testfontlayer_09x7, testfontlayer_14x10,
            testfontlayer_14x8, testfontlayer_14x9, testfontlayer_18x10, testfontlayer_18x11,
            testfontlayer_18x12, testfontlayer_eurostile, presents_layer;
        private PanningLayer credits_layer;

        private AnimatedLayer williams_logo, ballcross, dm_logo, pcc_logo, github_logo;

        public Attract(StarterGame game)
            : base(game, 1)
        {

        }

        public override void mode_started()
        {
            // Blinky start button
            //Game.Lamps["startButton"].Schedule(0x00ff00ff, 0, false);
        }
		/*
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
        */

        

        public new StarterGame Game
        {
            get { return (StarterGame)base.Game; }
        }
    }
}
