using NetProc.Pdb;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetProc.FakeProcDevice
{
    class Program
    {
        static CancellationTokenSource source = new CancellationTokenSource();
        private static AttrCollection<ushort, string, Switch> _switches;
        private static AttrCollection<ushort, string, LED> _leds;
        private static AttrCollection<ushort, string, IDriver> _coils;
        static IFakeProcDevice PROC = null;
        static async Task Main(string[] args)
        {            
            try
            {
                Console.WriteLine("Creating FakePinProc");
                PROC = new FakePinProc(MachineType.PDB);
                await Task.Delay(100);
                PROC?.Reset(1);

                //load machine config.
                var config = MachineConfiguration.FromFile("machine.json");

                //create collections to pass into the setup.
                //The GameController does this for you in LoadConfig, but this is to test without a game
                _switches = new AttrCollection<ushort, string, Switch>();
                _leds = new AttrCollection<ushort, string, LED>();
                _coils = new AttrCollection<ushort, string, IDriver>();

                //setup machine items to be able to process events.
                PROC.SetupProcMachine(config, coils: _coils, switches: _switches, lamps:null, leds: _leds, gi:null);

                var states = PROC.SwitchGetStates();

                //listen for cancel keypress and end run loop
                Console.CancelKeyPress += (sender, eventArgs) =>
                {
                    Console.WriteLine("ctrl+C triggered");
                    source.Cancel();
                    eventArgs.Cancel = true;
                };

                Task.Run(() =>
                {
                    string line = "";
                    while ((line = Console.ReadLine()) != null)
                    {
                        ushort.TryParse(line, out var number);
                        if(number > 0)
                        {
                            PROC.AddSwitchEvent(number, EventType.SwitchClosedDebounced);
                        }
                    }                          
                });

                //run game loop
                await RunLoop(PROC);

                //close console
                Console.WriteLine("netprocgame closing...");
                await Task.Delay(500);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static async Task RunLoop(IFakeProcDevice proc)
        {
            long loops = 0;
            Event[] events;
            //_coils["trough"].Pulse(255);
            //var flasher = _coils["flasher"];
            //flasher.Pulse(255);
            //flasher.Schedule(0x4F4F4F4F, 10); //schedule for 10 sec
            //flasher.Patter(0, 0, 0); // on 1, off 1
            //flasher.Patter(125, 0, 0); // on 10ms, off 1
            //flasher.Patter(10, 255); //flash fast
            //flasher.Patter(255, 255); //flash med
            //flasher.Patter(10, 10); //always on (pretty much)
            //flasher.Enable();

            while (!source.IsCancellationRequested)
            {
                loops++;
                events = proc.Getevents(false);
                if (events != null)
                {
                    foreach (Event evt in events)
                    {
                        switch (evt.Type)
                        {
                            case EventType.SwitchClosedDebounced:
                            case EventType.SwitchOpenDebounced:
                            case EventType.SwitchClosedNondebounced:
                            case EventType.SwitchOpenNondebounced:
                                ProcessSwitchEvent(evt);
                                break;
                            case EventType.DMDFrameDisplayed:
                            case EventType.None:
                            case EventType.Invalid:
                            default:
                                break;
                        }
                    }
                }

                proc.WatchDogTickle();

                //adding this delay makes CPU usage 0. Memory 10mb
                await Task.Delay(1);
            }

            proc.Close();
        }

        private static void ProcessSwitchEvent(Event evt)
        {
            var evtVal = (ushort)evt.Value;
            if (_switches.ContainsKey(evtVal))
            {
                Switch sw = _switches[evtVal];
                bool recvd_state = evt.Type == EventType.SwitchClosedDebounced;
                if (!sw.IsState(recvd_state))
                {
                    Console.WriteLine($"{sw.Name} {recvd_state}");
                    sw.SetState(recvd_state);
                }
                Console.WriteLine($"{sw.Name} ({sw.Number}) | {evt.Type}");
            }
            else
            {
                Console.WriteLine("WARNING: no switch found under " + evtVal);
            }                       
        }
    }
}
