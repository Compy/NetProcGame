using NetProc;
using NetProc.Dmd;
using NetProcGame.Events;
using NetProcGame.Game;
using System;

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

		const int LP_RIGHT_FLASHER = 31;
		const int LP_LEFT_FLASHER = 14;
		const int LP_UPPER_LEFT_FLASHER = 41;
		const byte GRP_FLASHERS = 0x1;
		const byte GRP_LAMPS = 0x2;
		const byte GRP_ODD_LAMPS = 0x3;
		const byte GRP_EVEN_LAMPS = 0x4;

        public Attract(StarterGame game)
            : base(game, 1)
        {

        }
		public void alternateAllLampsAtSpeed(int level) {
			uint a = 0;
			uint b = 0;
			if (level == 0) {
				a = 0xFFFF0000;
				b = 0x0000FFFF;
			} else if (level == 1) {
				a = 0xFF00FF00;
				b = 0x00FF00FF;
			} else if (level == 2) {
				a = 0xCCCCCCCC;
				b = 0x33333333;
			} else if (level == 3) {
				a = 0x55555555;
				b = 0xAAAAAAAA;
			}
			Game.ledDriver.GroupSchedule (GRP_EVEN_LAMPS, a, true);
			Game.ledDriver.GroupSchedule (GRP_ODD_LAMPS, b, true);

			Game.ledDriver.ScheduleLamp (LP_LEFT_FLASHER, 0xFFFFFFFF);
			Game.ledDriver.FadeLedToColor (LP_LEFT_FLASHER, 255, 0, 255, 32);
			Game.ledDriver.ScheduleLamp (LP_RIGHT_FLASHER, 0xFFFFFFFF);
			Game.ledDriver.FadeLedToColor (LP_RIGHT_FLASHER, 0, 0, 255, 32);
			Game.ledDriver.ScheduleLamp (LP_UPPER_LEFT_FLASHER, 0xFFFFFFFF);
			Game.ledDriver.FadeLedToColor (LP_UPPER_LEFT_FLASHER, 255, 255, 255, 32);
		}
		public void RunFlasherRoutine()
		{
			//don't run theese shows when null
			if(Game.ledDriver != null)
            {
				Game.ledDriver.FadeAllToColor(0, 0, 0, 0);

				// Turn on the flasher motors
				//Game.spinning_flashers_on ();

				//Game.ledDriver.ScheduleLamp (LP_LEFT_FLASHER, 0xFFFFFFFF);
				//Game.ledDriver.ScheduleLamp (LP_RIGHT_FLASHER, 0xFFFFFFFF);
				//Game.ledDriver.ScheduleLamp (LP_UPPER_LEFT_FLASHER, 0xFFFFFFFF);

				Game.ledDriver.AssignLamp((byte)LP_LEFT_FLASHER, GRP_FLASHERS);
				Game.ledDriver.AssignLamp((byte)LP_RIGHT_FLASHER, GRP_FLASHERS);
				Game.ledDriver.AssignLamp((byte)LP_UPPER_LEFT_FLASHER, GRP_FLASHERS);

				for (int i = 0; i < 50; i++)
				{
					if (i != LP_RIGHT_FLASHER && i != LP_LEFT_FLASHER && i != LP_UPPER_LEFT_FLASHER)
					{
						byte destGroup = (i % 2 == 0) ? GRP_EVEN_LAMPS : GRP_ODD_LAMPS;
						Console.WriteLine("Assigning " + i.ToString() + " to " + destGroup.ToString());
						Game.ledDriver.AssignLamp((byte)i, destGroup);
					}
				}

				Game.ledDriver.GroupSchedule(GRP_FLASHERS, 0xFFFFFFFF, true);
				Game.ledDriver.FadeLedToColor(LP_LEFT_FLASHER, 255, 0, 255, 32);
				Game.ledDriver.FadeLedToColor(LP_RIGHT_FLASHER, 0, 0, 255, 32);
				Game.ledDriver.FadeLedToColor(LP_UPPER_LEFT_FLASHER, 255, 255, 255, 32);

				Game.ledDriver.GroupFadeToColor(GRP_EVEN_LAMPS, 255, 255, 255, 32);
				Game.ledDriver.GroupFadeToColor(GRP_ODD_LAMPS, 255, 255, 255, 32);

				//Game.ledDriver.GroupSchedule (GRP_EVEN_LAMPS, 0x55555555, true);
				//Game.ledDriver.GroupSchedule (GRP_ODD_LAMPS, 0xAAAAAAAA, true);
				alternateAllLampsAtSpeed(0);
				this.Delay("flasherEvent", EventType.None, 1, new AnonDelayedHandler(delegate () {
					alternateAllLampsAtSpeed(1);
				}));

				this.Delay("flasherEvent2", EventType.None, 2, new AnonDelayedHandler(delegate () {
					alternateAllLampsAtSpeed(2);
				}));

				this.Delay("flasherEvent3", EventType.None, 3, new AnonDelayedHandler(delegate () {
					alternateAllLampsAtSpeed(3);
				}));

				this.Delay("flasherEvent4", EventType.None, 5, new AnonDelayedHandler(delegate () {
					Game.ledDriver.ScheduleAll(0x0);
					Game.spinning_flashers_off();
				}));
			}
			
			Game.spinning_flashers_on ();

			

			//Game.ledDriver.GroupSchedule (GRP_FLASHERS, 0xFFFFFFFF, true);
			/*
			this.delay("flasherEvent", EventType.None, 5, new AnonDelayedHandler(delegate() {
				if (Game == null) Console.WriteLine("OH SNAP1");
				Game.spinning_flashers_off();
				if (Game.ledDriver == null) Console.WriteLine("OH SNAP2");
				Game.ledDriver.GroupFadeToColor(GRP_LAMPS, 0, 0, 0, 0);
			}));
			*/

			/*
			// Do a color fade
			Game.ledDriver.ScheduleLamp (11, 0xFFFFFFFF);
			Game.ledDriver.FadeLedToColor (11, 255, 0, 0, 32);
			this.delay ("flasherEvent1", EventType.None, 1.0, new AnonDelayedHandler (delegate() {
				Game.ledDriver.FadeLedToColor (11, 0, 0, 255, 32);
				this.delay ("flasherEvent2", EventType.None, 1.0, new AnonDelayedHandler (delegate() {
					Game.ledDriver.FadeLedToColor (11, 255, 255, 255, 32);
					this.delay ("flasherEvent3", EventType.None, 1.0, new AnonDelayedHandler (delegate() {
						Game.ledDriver.ScheduleLamp (11, 0xCCCCCCCC);
						this.delay ("flasherEventFinal", EventType.None, 1.0, new AnonDelayedHandler (delegate() {
							Game.spinning_flashers_off ();
							Game.ledDriver.ScheduleLamp (11, 0x0);
						}));
					}));
				}));
			}));
			*/
			/*
			// 31 - right spinning flasher
			// 14 - left spinning flasher
			const int LP_RIGHT_FLASHER = 31;

			// EFFECT 2 -- Blink then fade out
			Game.ledDriver.FadeLedToColor (LP_RIGHT_FLASHER, 0, 255, 0, 0);
			Game.ledDriver.ScheduleLamp (LP_RIGHT_FLASHER, 0xCCCCCCCC);
			Attract self = this;
			this.delay ("flasherEvent1", EventType.None, 0.75, new AnonDelayedHandler (delegate() {
				Game.ledDriver.ScheduleLamp (LP_RIGHT_FLASHER, 0xFFFFFFFF);
				Game.ledDriver.FadeLedToColor (LP_RIGHT_FLASHER, 0, 0, 0, 32);
				self.delay ("flasherEvent2", EventType.None, 1.0, new AnonDelayedHandler (delegate() {
					Game.spinning_flashers_off ();
					Game.ledDriver.ScheduleLamp (LP_RIGHT_FLASHER, 0x0);
				}));
			}));
			*/
		}

        public override void ModeStarted()
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
