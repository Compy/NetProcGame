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
            // Lamp show
            change_lampshow();

            Game.score_display.layer.enabled = false;

            Animation anim = new Animation().load(@"animations/williams_animated.dmd");
            this.williams_logo = new AnimatedLayer(false, true, false, 1, anim.frames.ToArray());

            anim = new Animation().load(@"animations/ballcross.dmd");
            this.ballcross = new AnimatedLayer(false, true, false, 1, anim.frames.ToArray());

            anim = new Animation().load(@"animations/dm_logo.dmd");
            this.dm_logo = new AnimatedLayer(false, true, false, 1, anim.frames.ToArray());

            anim = new Animation().load(@"animations/pcc_logo.dmd");
            this.pcc_logo = new AnimatedLayer(false, true, false, 1, anim.frames.ToArray());

            anim = new Animation().load(@"animations/github_fork.dmd");
            this.github_logo = new AnimatedLayer(false, true, false, 1, anim.frames.ToArray());

            presents_layer = new TextLayer(128 / 2, 0, FontManager.instance.font_named("Font09Bx7.dmd"), FontJustify.Center, true);
            presents_layer.set_text("PRESENTS");
            //testfontlayer_04B37 = new TextLayer(128 / 2, 0, FontManager.instance.font_named("04B-03-7px.dmd"), FontJustify.Center, true);
            //testfontlayer_07x4 = new TextLayer(128 / 2, 0, FontManager.instance.font_named("Font07x4.dmd"), FontJustify.Center, true);
            //testfontlayer_07x5 = new TextLayer(128 / 2, 0, FontManager.instance.font_named("Font07x5.dmd"), FontJustify.Center, true);
            //testfontlayer_09Bx7 = new TextLayer(128 / 2, 0, FontManager.instance.font_named("Font09Bx7.dmd"), FontJustify.Center, true);
            //testfontlayer_09x5 = new TextLayer(128 / 2, 0, FontManager.instance.font_named("Font09x5.dmd"), FontJustify.Center, true);
            //testfontlayer_09x6 = new TextLayer(128 / 2, 0, FontManager.instance.font_named("Font09x6.dmd"), FontJustify.Center, true);
            //testfontlayer_09x7 = new TextLayer(128 / 2, 0, FontManager.instance.font_named("Font09x7.dmd"), FontJustify.Center, true);
            //testfontlayer_14x10 = new TextLayer(128 / 2, 0, FontManager.instance.font_named("Font14x10.dmd"), FontJustify.Center, true);
            //testfontlayer_14x8 = new TextLayer(128 / 2, 0, FontManager.instance.font_named("Font14x8.dmd"), FontJustify.Center, true);
            //testfontlayer_14x9 = new TextLayer(128 / 2, 0, FontManager.instance.font_named("Font14x9.dmd"), FontJustify.Center, true);
            //testfontlayer_18x10 = new TextLayer(128 / 2, 0, FontManager.instance.font_named("Font18x10.dmd"), FontJustify.Center, true);
            //testfontlayer_18x11 = new TextLayer(128 / 2, 0, FontManager.instance.font_named("Font18x11.dmd"), FontJustify.Center, true);
            //testfontlayer_18x12 = new TextLayer(128 / 2, 0, FontManager.instance.font_named("Font18x12.dmd"), FontJustify.Center, true);
            //testfontlayer_eurostile = new TextLayer(128 / 2, 0, FontManager.instance.font_named("eurostile.dmd"), FontJustify.Center, true);

            //testfontlayer_04B37.set_text("FONT04B037");
            //testfontlayer_07x4.set_text("FONT07x4");
            //testfontlayer_07x5.set_text("FONT07x5");
            //testfontlayer_09Bx7.set_text("FONT09Bx7");
            //testfontlayer_09x5.set_text("FONT09x5");
            //testfontlayer_09x6.set_text("FONT09x6");
            //testfontlayer_09x7.set_text("FONT09x7");
            //testfontlayer_14x10.set_text("FONT14x10");
            //testfontlayer_14x8.set_text("FONT14x8");
            //testfontlayer_14x9.set_text("FONT14x9");
            //testfontlayer_18x10.set_text("FONT18x10");
            //testfontlayer_18x11.set_text("FONT18x11");
            //testfontlayer_18x12.set_text("FONT18x12");
            //testfontlayer_eurostile.set_text("Eurostile 123");

            MarkupGenerator gen = new MarkupGenerator();
            gen.font_plain = FontManager.instance.font_named("Font09x7.dmd");
            gen.font_bold = FontManager.instance.font_named("Font09Bx7.dmd");
            
            Frame credits_frame = gen.frame_for_markup(@"

[CREDITS]

[Game Rules and Coding]
[Jimmy Lipham]

[Special Thanks]
[Gerry Stellenberg]
[Adam Preble]");

            this.credits_layer = new PanningLayer(128, 32, credits_frame, new Pair<int, int>(0, 0),
                new Pair<int, int>(0, 1), false);
            this.credits_layer.composite_op = DMDBlendMode.DMDBlendModeCopy;

            List<Pair<int, Layer>> script = new List<Pair<int, Layer>>();

            script.Add(new Pair<int, Layer>(7, williams_logo));
            script.Add(new Pair<int, Layer>(4, presents_layer));
            script.Add(new Pair<int, Layer>(10, dm_logo));
            script.Add(new Pair<int, Layer>(1, ballcross));
            script.Add(new Pair<int, Layer>(5, pcc_logo));
            script.Add(new Pair<int, Layer>(5, github_logo));
            //script.Add(new Pair<int, Layer>(5, testfontlayer_eurostile));
            //script.Add(new Pair<int, Layer>(5, testfontlayer_04B37));
            //script.Add(new Pair<int, Layer>(5, testfontlayer_07x4));
            //script.Add(new Pair<int, Layer>(5, testfontlayer_07x5));
            //script.Add(new Pair<int, Layer>(5, testfontlayer_09Bx7));
            //script.Add(new Pair<int, Layer>(5, testfontlayer_09x5));
            //script.Add(new Pair<int, Layer>(5, testfontlayer_09x6));
            //script.Add(new Pair<int, Layer>(5, testfontlayer_09x7));
            //script.Add(new Pair<int, Layer>(5, testfontlayer_14x10));
            //script.Add(new Pair<int, Layer>(5, testfontlayer_14x8));
            //script.Add(new Pair<int, Layer>(5, testfontlayer_14x9));
            //script.Add(new Pair<int, Layer>(5, testfontlayer_18x10));
            //script.Add(new Pair<int, Layer>(5, testfontlayer_18x11));
            //script.Add(new Pair<int, Layer>(5, testfontlayer_18x12));
            //script.Add(new Pair<int,Layer>(30, credits_layer));

            this.layer = new ScriptedLayer(128, 32, script);

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

        public bool sw_buyIn_active(Switch sw)
        {
            Game.Coils["sideRampFlasher"].FuturePulse(30, 500);
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
