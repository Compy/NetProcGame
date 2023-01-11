using NetProc;
using NetProc.Game;

namespace NetProcGame.RgbLeds
{
    internal class Game : GameController
    {
        public Game(MachineType machineType, ILogger logger, bool Simulated = false) : base(machineType, logger, Simulated)
        {

        }

        internal void Setup()
        {
            LoadConfig(@"machine.json");
        }        
    }
}
