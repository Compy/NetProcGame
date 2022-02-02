using NetProcGame.Game;
using NetProcGame.Pdb;
using NetProcGame.Tools;
using System.Collections.Generic;
using System.Linq;

namespace NetProcGame.lamps
{
    public static class LEDS
    {
        public static readonly List<PDLED> PDLEDS = new List<PDLED>();
    }

    /// <summary>
    /// Represents an LED LED in a pinball machine
    /// </summary>
    public class LED : GameItem
    {
        private readonly List<uint> addrs;
        private readonly uint boardAddress;
        private LEDScript[] _activeScript;
        private double _lastTimeChanged;
        private double _nextActionTime;
        private bool _scriptFinished;
        private int _scriptIndex;
        private bool _scriptRepeat;
        private int _scriptRuntime;
        private double _scriptStartTime;
        private PDLED board;
        private string function;
        private bool invert = true;
        private double lastTimeChanged;

        public LED(IGameController game, string name, ushort number, string strNumber = "") : base(game, name, number, strNumber)
        {
            //take the first number in the array to get address. A0-R0-G1-B2
            var crList = strNumber.Split('-');
            boardAddress = uint.Parse(crList[0].Substring(1));

            //get the colors
            addrs = new List<uint>();
            foreach (var item in crList.Skip(1))
            {
                addrs.Add(uint.Parse(item.Substring(1)));
            }

            System.Console.WriteLine($"Creating LED: {name}, board_addr: {boardAddress}, color_addrs: {string.Join(",", addrs)}");
            function = "none";

            //check if the led already exists in collection
            var pdLedExists = false;
            foreach (var pdLed in LEDS.PDLEDS)
            {
                if (pdLed.BoardAddress == this.boardAddress)
                {
                    pdLedExists = true;
                    this.board = pdLed;
                    continue;
                }
            }

            //add the board if wasn't found into PDLEDS collection
            if (!pdLedExists)
            {
                this.board = new PDLED(game.PROC, boardAddress);
                LEDS.PDLEDS.Add(board);
            }
        }

        public void ChangeColor(uint[] color)
        {
            for (int i = 0; i < 3; i++)
            {
                var newColor = NormalizeColor(color[i]);
                board.WriteColor(addrs[i], newColor);
            }
        }

        /// <summary>
        /// Change colour and clear any scripts. You need to use 0xFF for 255, not sure why, to test 255,255,0 for yellow will be red so you would need to do 0xFF,0xFF,0xFF
        /// </summary>
        /// <param name="color"></param>
        public void Color(uint[] color)
        {
            function = "none";
            ChangeColor(color);
        }

        /// <summary>
        /// Turns off the LED
        /// </summary>
        public void Disable()
        {
            function = "none";
            for (int i = 0; i < 3; i++)
            {
                var newColor = NormalizeColor(0);
                board.WriteColor(addrs[i], newColor);
            }
            lastTimeChanged = Time.GetTime();
        }

        /// <summary>
        /// Fades the LED disabling any scripts
        /// </summary>
        /// <param name="color"></param>
        /// <param name="time"></param>
        public void Fade(uint[] color, uint time)
        {
            function = "none";
            ChangeFade(color, time);
        }

        public long GetSriptDuration(LEDScript[] ledScript) => ledScript.Sum(x => x.Duration);

        /// <summary>
        /// 
        /// </summary>
        public void Script(LEDScript[] newScript, int runtime = 0, bool repeat = true)
        {
            _scriptIndex = -1;
            _activeScript = newScript;
            _scriptRuntime = runtime;
            function = "script";
            _scriptStartTime = Time.GetTime();
            _scriptFinished = false;
            _scriptRepeat = repeat;
            IterateScript();
        }

        public void Tick()
        {
            if (function == "script")
            {
                var time = Time.GetTime();
                if ((_scriptRuntime == 0 && !_scriptFinished) || (time - _scriptStartTime) < _scriptRuntime)
                {
                    if (time >= _nextActionTime)
                        IterateScript();
                }
                else
                    function = "none";
            }
        }

        private void ChangeFade(uint[] color, uint time)
        {
            board.WriteFadeTime(time * 0x100);
            for (int i = 0; i < 3; i++)
            {
                var newColor = NormalizeColor(color[i]);
                board.WriteFadeColor(addrs[i], newColor);
            }
        }

        private void IterateScript()
        {
            if (_scriptIndex == _activeScript.Length - 1) _scriptIndex = 0;
            else _scriptIndex++;

            //change the color or fade it
            var entry = _activeScript[_scriptIndex];
            if (entry.FadeTime == 0)
                ChangeColor(entry.Colour);
            else
                ChangeFade(entry.Colour, entry.FadeTime);

            _lastTimeChanged = Time.GetTime();
            _nextActionTime = Time.GetTime() + entry.Duration;

            if (_scriptIndex == _activeScript.Length - 1)
            {
                if (!_scriptRepeat)
                    _scriptFinished = true;
            }
        }
        private uint NormalizeColor(uint color)
        {
            if (this.invert) return 255 - color;
            else return color;
        }
    }

    public class LEDScript
    {
        public uint[] Colour { get; set; }
        public uint Duration { get; set; }
        public uint FadeTime { get; set; }
    }
}
