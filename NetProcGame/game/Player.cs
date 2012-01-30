using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetProcGame.game
{
    /// <summary>
    /// Represents a player in the game.
    /// 
    /// The game maintains a collection of players in GameController.Players
    /// </summary>
    public class Player
    {
        /// <summary>
        /// This player's score
        /// </summary>
        public int score { get; set; }

        /// <summary>
        /// This players name (optional)
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// The number of extra balls this player has accumulated
        /// </summary>
        public int extra_balls { get; set; }

        /// <summary>
        /// The number of seconds that this player has had the ball in play.
        /// </summary>
        public double game_time { get; set; }

        public Player(string name)
        {
            this.name = name;
        }
    }
}
