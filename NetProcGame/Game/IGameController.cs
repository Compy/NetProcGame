using NetProcGame.Config;
using NetProcGame.lamps;
using NetProcGame.Lamps;
using NetProcGame.Modes;
using NetProcGame.Tools;
using System.Collections.Generic;

namespace NetProcGame.Game
{
    public interface IGameController
    {
        int ball { get; set; }
        AttrCollection<ushort, string, IDriver> Coils { get; set; }
        MachineConfiguration Config { get; }
        int CurrentPlayerIndex { get; }
        bool FlippersEnabled { get; set; }
        AttrCollection<ushort, string, IDriver> GI { get; set; }
        ILampController LampController { get; set; }
        AttrCollection<ushort, string, IDriver> Lamps { get; set; }
        AttrCollection<ushort, string, LED> LEDS { get; set; }
        ILogger Logger { get; set; }
        ModeQueue Modes { get; set; }
        List<Player> Players { get; set; }
        IProcDevice PROC { get; }
        AttrCollection<ushort, string, Switch> Switches { get; set; }
        Player AddPlayer();
        void BallEnded();
        void BallStarting();
        Player create_player(string name);
        Player CurrentPlayer();
        void DmdEvent();
        void EndBall();
        void EndGame();
        void EndRunLoop();
        void GameEnded();
        void GameStarted();
        double GetBallTime();
        Event[] GetEvents();
        double GetGameTime(int player);
        void LinkFlipperSwitch(string switch_name, string[] linked_coils);
        void LoadConfig(string PathToFile);
        void ProcessEvent(Event evt);
        void Reset();
        void RunLoop();
        void SafeDisableCoil(string coilName);
        void SafeDriveCoil(string coilName, ushort pulse_time = 30);
        void SaveBallStartTime();
        void ShootAgain();
        void StartBall();
        void StartGame();
        void Tick();
        void TickVirtualDrivers();
        void UpdateLamps();
    }
}