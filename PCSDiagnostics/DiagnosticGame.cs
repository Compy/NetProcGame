using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetProcGame;
using NetProcGame.config;
using NetProcGame.game;
using NetProcGame.modes;
using NetProcGame.tools;

namespace PCSDiagnostics
{
    public class DiagnosticGame : GameController
    {
        public Trough trough;
        public BallSave ball_save;
        public bool auto_launch_next_ball = false;

        private DiagnosticMode diagMode;

        public DiagnosticGame(ILogger logger = null)
            : base(MachineType.WPC, logger)
        {
            this.Logger = logger;
            setup();
        }

        public void setup()
        {
            LoadConfig(@"C:\Users\Jimmy\Documents\Pinball\dm_reloaded\config\machine.json");

            string[] trough_switchnames = new string[5] { "trough1", "trough2", "trough3", "trough4", "trough5" };

            trough = new Trough(this,
                                trough_switchnames,
                                "trough5",
                                "trough",
                                new string[] { "leftOutlane", "rightOutlane" },
                                "shooterLane");
            diagMode = new DiagnosticMode(this);
            ball_save = new BallSave(this, "ballSave", "shooterLane");

            // Link ball save to trough
            trough.ball_save_callback = new AnonDelayedHandler(ball_save.launch_callback);
            trough.num_balls_to_save = new GetNumBallsToSaveHandler(ball_save.get_num_balls_to_save);
            ball_save.trough_enable_ball_save = new BallSaveEnable(trough.enable_ball_save);

            _modes.Add(diagMode);
            _modes.Add(trough);
            _modes.Add(ball_save);
        }

        public void close_divertor()
        {
            safe_drive_coil("divertorMain", 50);
            safe_drive_coil("divertorHold", 0);
        }

        public void open_divertor()
        {
            safe_disable_coil("divertorMain");
            safe_disable_coil("divertorHold");
        }
    }
}
