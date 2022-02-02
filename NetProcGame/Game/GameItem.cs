namespace NetProcGame.Game
{
    /// <summary>
    /// Base class for 'Driver' and 'Switch'
    /// </summary>
    public class GameItem
    {
        /// <summary>
        /// GameController instance to which this item belongs
        /// </summary>
        protected IGameController _game = null;
        /// <summary>
        /// Name of this item
        /// </summary>
        protected string _name = "";

        /// <summary>
        /// Integer value for this item that provides a mapping to the hardware
        /// </summary>
        protected ushort _number = 0;

        /// <summary>
        /// LED string number
        /// </summary>

        protected string _strNumber;


        public GameItem(IGameController game, string name, ushort number, string strNumber = "")
        {
            this._game = game;
            this._name = name;
            this._number = number;
            _strNumber = strNumber;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public ushort Number
        {
            get { return _number; }
            set { _number = value; }
        }
    }
}
