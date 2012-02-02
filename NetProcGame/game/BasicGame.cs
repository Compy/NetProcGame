using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetProcGame;
using NetProcGame.game;
using NetProcGame.dmd;
using NetProcGame.tools;
using NetProcGame.modes;

namespace NetProcGame.game
{
    /// <summary>
    /// BasicGame is a subclass of GameController that includes and configures various useful
    /// helper classes to provide:
    ///     - 
    /// </summary>
    /// 
    public class BasicGame : GameController
    {
        public DisplayController dmd = null;
        //public ScoreDisplay score_display = null;
        public ScoreDisplay score_display = null;

        public BasicGame(MachineType machine_type, ILogger logger)
            : base(machine_type, logger)
        {
            FontManager manager = new FontManager(@"fonts\");
            if (machine_type == MachineType.WPCAlphanumeric)
            {
                // Create alphanumeric display
            }
            else
            {
                this.dmd = new DisplayController(this, 128, 32, manager.font_named("Font07x5.dmd"));
            }

            this.score_display = new ScoreDisplay(this, 50);

            // The below code is for showing frames on the desktop
            //if (this.dmd != null) this.dmd.frame_handlers.Add(new DMDFrameHandler(this.set_last_frame));

            // Set up key map configs
        }

        /// <summary>
        /// Reset all core functionality and add the ScoreDisplay mode to the mode queue
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            this.Modes.Add(this.score_display);
        }

        /// <summary>
        /// Updates the DMD via 'DisplayController'
        /// </summary>
        public override void dmd_event()
        {
            if (this.dmd != null) this.dmd.update();
        }

        public override void tick()
        {
            base.tick();
            //this.show_last_frame();
        }

        public override void game_started()
        {
            score_display.layer.enabled = true;
            base.game_started();
        }

        public void score(int points)
        {
            Player p = this.current_player();
            p.score += points;
        }
    }
}
