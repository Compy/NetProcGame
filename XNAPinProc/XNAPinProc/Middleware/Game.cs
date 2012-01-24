using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NetProcGame;
using NetProcGame.config;
using NetProcGame.game;
using NetProcGame.modes;
using NetProcGame.tools;
using XNAPinProc.Middleware;
using XNAPinProc.Middleware.Modes;

namespace XNAPinProc.Middleware
{
    public class MiddlewareGame : GameController
    {
        public Attract attract;
        public BaseGameMode _base_game_mode;
        public BallSave ball_save;
        public Trough trough;
        public SystemBase system;

        public KeyboardController keyboardController;

        public bool ball_being_saved = false;

        public MiddlewareGame(ILogger logger)
            : base(MachineType.WPC, logger)
        {
            keyboardController = new KeyboardController();
        }

        /// <summary>
        /// Overridden functionality that gets events from the proc as well as the keyboard triggered switch events
        /// </summary>
        /// <returns>A list of keyboard and playfield switch triggered events</returns>
        public override Event[] get_events()
        {
            Event[] events = _proc.get_events();
            List<Event> eventList = new List<Event>(events);
            return events;
        }

        public void setup()
        {
            LoadConfig(@"Configuration\machine.json");
            setup_ball_search();

            // Intantiate basic game features
            system = new SystemBase(this);
            attract = new Attract(this);
            _base_game_mode = new BaseGameMode(this);

            string[] trough_switchnames = new string[5] { "trough1", "trough2", "trough3", "trough4", "trough5" };

            trough = new Trough(this,
                                trough_switchnames,
                                "trough5",
                                "trough",
                                new string[] { "leftOutlane", "rightOutlane" },
                                "shooterLane");
            ball_save = new BallSave(this, "ballSave", "shooterLane");

            // Link ball save to trough
            trough.ball_save_callback = new AnonDelayedHandler(ball_save.launch_callback);
            trough.num_balls_to_save = new GetNumBallsToSaveHandler(ball_save.get_num_balls_to_save);
            ball_save.trough_enable_ball_save = new BallSaveEnable(trough.enable_ball_save);
            
            // Add keyboard switch maps
            keyboardController.KeySwitchMap.Add(Microsoft.Xna.Framework.Input.Keys.S, Switches["startButton"].Number);
            keyboardController.KeySwitchMap.Add(Microsoft.Xna.Framework.Input.Keys.Enter, Switches["enter"].Number);
            keyboardController.KeySwitchMap.Add(Microsoft.Xna.Framework.Input.Keys.X, Switches["exit"].Number);
            keyboardController.KeySwitchMap.Add(Microsoft.Xna.Framework.Input.Keys.U, Switches["up"].Number);
            keyboardController.KeySwitchMap.Add(Microsoft.Xna.Framework.Input.Keys.D, Switches["down"].Number);

            // Instead of resetting everything here as well as when a user initiated reset occurs, do everything in
            // this.reset and call it now and during a user initiated reset
            this.Reset();
        }

        public void on_ball_saved()
        {
            ball_being_saved = true;
        }

        public override void Reset()
        {
            base.Reset();
            // Add the basic modes to the queue
            _modes.Add(system);
            _modes.Add(attract);
            _modes.Add(ball_save);
            _modes.Add(trough);
            //_modes.Add(_ball_search);

            // Disable the flippers
            this.FlippersEnabled = false;
        }

        /// <summary>
        /// Empty callback just incase a ball drains into the trough before another
        /// drain_callback can be installed by a gameplay mode
        /// </summary>
        public void drain_callback()
        {
        }

        public override void ball_starting()
        {
            base.ball_starting();
            _modes.Add(_base_game_mode);
        }

        public override void ball_ended()
        {
            _modes.Remove(_base_game_mode);
            base.ball_ended();
        }

        public override void game_ended()
        {
            base.game_ended();
            _modes.Add(attract);
        }

        public void extra_ball()
        {
            Player p = current_player();
            p.extra_balls++;
        }


        public void setup_ball_search()
        {
        }
    }
}
