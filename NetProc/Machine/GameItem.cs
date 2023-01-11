namespace NetProc
{
    /// <summary>
    /// Base class for 'Driver' and 'Switch'
    /// </summary>
    public class GameItem : IGameItem
    {
        protected readonly IProcDevice proc;

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


        public GameItem(IProcDevice proc, string name, ushort number, string strNumber = "")
        {
            this.proc = proc;
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
