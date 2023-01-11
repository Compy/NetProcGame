namespace NetProc.Game
{
    /// <summary>
    /// Represents a player in the game. <para/>
    /// The game maintains a collection of players in GameController.Players
    /// </summary>
    public class Player : IPlayer
    {
        /// <summary>
        /// This player's score
        /// </summary>
        public long Score { get; set; }

        /// <summary>
        /// This players name (optional)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The number of extra balls this player has accumulated
        /// </summary>
        public int ExtraBalls { get; set; }

        /// <summary>
        /// The number of seconds that this player has had the ball in play.
        /// </summary>
        public double GameTime { get; set; }

        public Player(string name)
        {
            this.Name = name;
        }
    }
}
