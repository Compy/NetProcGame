using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetProcGame.game
{
    /// <summary>
    /// Base class for 'Driver' and 'Switch'
    /// </summary>
    public class GameItem
    {
        /// <summary>
        /// GameController instance to which this item belongs
        /// </summary>
        protected GameController _game = null;
        /// <summary>
        /// Name of this item
        /// </summary>
        protected string _name = "";

        /// <summary>
        /// Integer value for this item that provides a mapping to the hardware
        /// </summary>
        protected ushort _number = 0;

        protected string _strNumber;


        public GameItem(GameController game, string name, ushort number)
        {
            this._game = game;
            this._name = name;
            this._number = number;
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
