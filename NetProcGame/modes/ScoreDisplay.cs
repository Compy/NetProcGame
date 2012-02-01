using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetProcGame.game;
using NetProcGame.dmd;
using NetProcGame.tools;

namespace NetProcGame.modes
{
    /// <summary>
    /// A mode that provides a DMD layer containing a generic 1-4 player score display.
    /// To use 'ScoreDisplay' simply instantiate it and add it to the mode queue. A low priority
    /// is recommended.
    /// 
    /// When the layer is asked for its next_frame() the DMD frame is built based on the player score and the ball information
    /// contained in the GameController
    /// 
    /// 'ScoreDisplay' uses a number of fonts, the defaults of which are included in the shared DMD resources folder. If a font
    /// cannot be found then the score may not display properly in some states.
    /// </summary>
    public class ScoreDisplay : Mode
    {
        /// <summary>
        /// Font used for the bottom status line text (BALL 1 FREE PLAY) -- defaults to Font07x5.dmd
        /// </summary>
        public Font font_common;
        public Font font_18x12;
        public Font font_18x11;
        public Font font_18x10;
        public Font font_14x10;
        public Font font_14x9;
        public Font font_14x8;
        public Font font_09x5;
        public Font font_09x6;
        public Font font_09x7;
        public Dictionary<bool, List<Pair<int, int>>> score_posns;
        FontJustify[] score_justs;

        public ScoreDisplay(GameController game, int priority, FontJustify left_players_justify = FontJustify.Right)
            : base(game, priority)
        {
            this.layer = new ScoreLayer(128, 32, this);
            this.font_common = FontManager.instance.font_named("Font07x5.dmd");
            this.font_18x12 = FontManager.instance.font_named("Font18x12.dmd");
            this.font_18x11 = FontManager.instance.font_named("Font18x11.dmd");
            this.font_18x10 = FontManager.instance.font_named("Font18x10.dmd");
            this.font_14x10 = FontManager.instance.font_named("Font14x10.dmd");
            this.font_14x9 = FontManager.instance.font_named("Font14x9.dmd");
            this.font_14x8 = FontManager.instance.font_named("Font14x8.dmd");
            this.font_09x5 = FontManager.instance.font_named("Font09x5.dmd");
            this.font_09x6 = FontManager.instance.font_named("Font09x6.dmd");
            this.font_09x7 = FontManager.instance.font_named("Font09x7.dmd");

            this.score_posns = new Dictionary<bool, List<Pair<int, int>>>();

            this.set_left_players_justify(left_players_justify);
        }

        /// <summary>
        /// Call to set the justification of the left-hand players' scores in a multiplayer game.
        /// Valid values for left_player_justify are FontJustify.Left and FontJustify.Right
        /// </summary>
        public void set_left_players_justify(FontJustify left_players_justify)
        {
            List<Pair<int, int>> position_entries;
            this.score_posns.Clear();
            if (left_players_justify == FontJustify.Left)
            {
                position_entries = new List<Pair<int, int>>();
                position_entries.Add(new Pair<int, int>(0, 0));
                position_entries.Add(new Pair<int, int>(128, 0));
                position_entries.Add(new Pair<int, int>(0, 11));
                position_entries.Add(new Pair<int, int>(128, 11));
                this.score_posns.Add(true, position_entries);
                position_entries.Clear();
                position_entries.Add(new Pair<int, int>(0, -1));
                position_entries.Add(new Pair<int, int>(128, -1));
                position_entries.Add(new Pair<int, int>(0, 16));
                position_entries.Add(new Pair<int, int>(128, 16));
                this.score_posns.Add(false, position_entries);
            }
            else
            {
                position_entries = new List<Pair<int, int>>();
                position_entries.Add(new Pair<int, int>(75, 0));
                position_entries.Add(new Pair<int, int>(128, 0));
                position_entries.Add(new Pair<int, int>(75, 11));
                position_entries.Add(new Pair<int, int>(128, 11));
                this.score_posns.Add(true, position_entries);
                position_entries.Clear();
                position_entries.Add(new Pair<int, int>(52, -1));
                position_entries.Add(new Pair<int, int>(128, -1));
                position_entries.Add(new Pair<int, int>(52, 16));
                position_entries.Add(new Pair<int, int>(128, 16));
                this.score_posns.Add(false, position_entries);
            }
            this.score_justs = new FontJustify[4] { left_players_justify, FontJustify.Right, left_players_justify, FontJustify.Right };
        }

        /// <summary>
        /// Returns a string representation of the given score value
        /// </summary>
        public string format_score(long score)
        {
            if (score == 0) return "00";
            return score.ToString("#,##0");
        }

        /// <summary>
        /// Returns the font to be used for displaying the given numeric score value in a single-player game
        /// </summary>
        public Font font_for_score_single(long score)
        {
            if (score < 1e10)
                return this.font_18x12;
            else if (score < 1e11)
                return this.font_18x11;
            else
                return this.font_18x10;
        }

        /// <summary>
        /// Returns the font to be used for displaying the given numeric score in a 2,3, or 4 player game
        /// </summary>
        public Font font_for_score(long score, bool is_active_player)
        {
            if (is_active_player)
            {
                if (score < 1e7)
                    return this.font_14x10;
                if (score < 1e8)
                    return this.font_14x9;
                else
                    return this.font_14x8;
            }
            else
            {
                if (score < 1e7)
                    return this.font_09x7;
                if (score < 1e8)
                    return this.font_09x6;
                else
                    return this.font_09x5;
            }
        }

        public Pair<int, int> pos_for_player(int player_index, bool is_active_player)
        {
            return this.score_posns[is_active_player][player_index];
        }

        public FontJustify justify_for_player(int player_index)
        {
            return this.score_justs[player_index];
        }

        /// <summary>
        /// Called by the layer to update the score layer for the present game state.
        /// </summary>
        public void update_layer()
        {
            ((ScoreLayer)this.layer).layers.Clear();
            if (this.Game.Players.Count <= 1)
                this.update_layer_1p();
            else
                this.update_layer_4p();

            // Common: add the ball X ... FREE PLAY footer
            TextLayer common = new TextLayer(128 / 2, 32 - 6, this.font_common, FontJustify.Center);
            if (this.Game.ball == 0)
                common.set_text("FREE PLAY");
            else
                common.set_text(String.Format("BALL {0}      FREE PLAY", this.Game.ball));

            ((ScoreLayer)this.layer).layers.Add(common);
        }

        public void update_layer_1p()
        {
            long score;
            if (this.Game.current_player() == null)
                score = 0; // Small hack to make something show up on startup
            else
                score = this.Game.current_player().score;

            TextLayer layer = new TextLayer(128 / 2, 5, this.font_for_score_single(score), FontJustify.Center);
            layer.set_text(this.format_score(score));
            ((ScoreLayer)this.layer).layers.Add(layer);
        }

        public void update_layer_4p()
        {
            long score;
            bool is_active_player;
            Font font;
            Pair<int, int> pos;
            FontJustify justify;
            TextLayer layer;
            for (int i = 0; i < Game.Players.Count; i++)
            {
                score = Game.Players[i].score;
                is_active_player = (this.Game.ball > 0) && (i == this.Game.current_player_index);
                font = this.font_for_score(score, is_active_player);
                pos = this.pos_for_player(i, is_active_player);
                justify = this.justify_for_player(i);
                layer = new TextLayer(pos.First, pos.Second, font, justify);
                layer.set_text(this.format_score(score));
                ((ScoreLayer)this.layer).layers.Add(layer);
            }
        }
    }
}
