using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetProcGame;
using NetProcGame.game;
using NetProcGame.tools;
using NetProcGame.modes;
using System.Timers;

namespace PinprocTest
{
    public class TestGame : GameController
    {
        public TestGame(ILogger logger)
            : base(MachineType.WPC, logger)
        {
            LoadConfig(@"C:\Users\Jimmy\Documents\Pinball\dm_reloaded\config\machine.json");

            //_lamps["startMultiball"].Schedule(0x30003000, 20, true);


            // Lets try this
            //_coils["bottomJet"].Pulse();
            //_proc.switch_update_rule(_switches["bottomJet"].Number, EventType.SwitchClosedDebounced,
            //    new SwitchRule { NotifyHost = false, ReloadActive = false }, new DriverState[] { _coils["bottomJet"].State }, false);
            FindAndEjectBall();
            Dictionary<string, Driver> _trap_switch_coils = new Dictionary<string, Driver>();

            _trap_switch_coils.Add("shooterLane", Coils["ballLaunch"]);
            _trap_switch_coils.Add("eject", Coils["eject"]);
            _trap_switch_coils.Add("topPopper", Coils["topPopper"]);
            _trap_switch_coils.Add("bottomPopper", Coils["bottomPopper"]);
            //_trap_switch_coils.Add("trough1", Coils["trough"]);

            TestMode m = new TestMode(this, _trap_switch_coils);
            _modes.Add(m);
        }

        ~TestGame()
        {
        }

        public void FindAndEjectBall()
        {
            foreach (Switch s in _switches.Values)
            {
                if (s.IsActive())
                {
                    Console.WriteLine("Switch " + s.Name + " ACTIVE");
                }
            }
        }

        public override void Reset()
        {
            base.Reset();
            this.FlippersEnabled = false;
        }
    }
}
