using NetProc.Interface;
using NetProc.Pdb;
using NetProc.Tools;
using System;
using System.Collections.Generic;

namespace NetProc
{
    public struct FakeSwitchRule
    {
        public bool NotifyHost;
        public bool ReloadActive;
        public List<IDriver> Drivers;
    }
    public class FakePinProc : IFakeProcDevice
    {
        private AttrCollection<ushort, string, IVirtualDriver> drivers;
        private MachineType _machineType;
        private List<Event> switch_events = new List<Event>();
        private FakeSwitchRule[] switch_rules = new FakeSwitchRule[1024];
        private double now;
        private double last_dmd_event = 0;
        private int frames_per_second = 60;

        /// <summary>
        /// Makes 1,024 fake switch_rules
        /// </summary>
        /// <param name="machineType"></param>
        public FakePinProc(MachineType machineType)
        {
            this.drivers = new AttrCollection<ushort, string, IVirtualDriver>();

            _machineType = machineType;

            //todo: Make 256 drivers
            for (ushort i = 0; i < 256; i++)
            {
                string name = "driver" + i.ToString();
                drivers.Add(i, name, new VirtualDriver(null, name, i, true));
            }

            // Instantiate default switch rules
            for (int j = 0; j < 1024; j++)
            {
                switch_rules[j] = new FakeSwitchRule() { NotifyHost = true, ReloadActive = false, Drivers = new List<IDriver>() };
            }
        }

        public void AuxSendCommands(ushort address, ushort aux_commands) { }
        public void Close() { }
        public void DmdDraw(byte[] bytes) { }
        public void DmdDraw(IFrame frame) { }
        public void DmdUpdateConfig(ushort[] high_cycles) { }
        public void DriverDisable(ushort number) => this.drivers[number].Disable();
        public DriverState DriverGetState(ushort number) => this.drivers[number].State;
        public void DriverGroupDisable(byte number) { }
        public void DriverPatter(ushort number, ushort milliseconds_on, ushort milliseconds_off, ushort original_on_time) { }
        public Result DriverPulse(ushort number, byte milliseconds)
        {
            this.drivers[number].Pulse(milliseconds);
            return Result.Success;
        }
        public void DriverPulsedPatter(ushort number, ushort milliseconds_on, ushort milliseconds_off, ushort milliseconds_overall_patter_time) { }
        public void DriverSchedule(ushort number, uint schedule, ushort cycle_seconds, bool now)
            => this.drivers[number].Schedule(schedule, cycle_seconds, now);
        public DriverState DriverStateDisable(DriverState state) => state;
        public DriverState DriverStateFuturePulse(DriverState state, byte milliseconds, ushort futureTime) => state;
        public DriverState DriverStatePatter(DriverState state, ushort milliseconds_on, ushort milliseconds_off, ushort original_on_time) => state;
        public DriverState DriverStatePulse(DriverState state, byte milliseconds) => state;
        public DriverState DriverStatePulsedPatter(DriverState state, ushort milliseconds_on, ushort milliseconds_off, ushort milliseconds_overall_patter_time) => state;
        public DriverState driver_state_schedule(DriverState state, uint schedule, byte seconds, bool now) => state;
        public void driver_update_global_config(bool enable, bool polarity, bool use_clear, bool strobe_start_select, byte start_strobe_time, byte matrix_row_enable_index0, byte matrix_row_enable_index1, bool active_low_matrix_rows, bool tickle_stern_watchdog, bool encode_enables, bool watchdog_expired, bool watchdog_enable, ushort watchdog_reset_time) { }
        public void driver_update_group_config(byte group_num, ushort slow_time, byte enable_index, byte row_activate_index, byte row_enable_select, bool matrixed, bool polarity, bool active, bool disable_strobe_after) { }
        public void driver_update_state(ref DriverState driver) => this.drivers[driver.DriverNum].State = driver;
        public void Flush() { }
        public Event[] Getevents(bool dmdEvents = true)
        {
            List<Event> events = new List<Event>();
            events.AddRange(this.switch_events);
            this.switch_events.Clear();

            if (dmdEvents)
            {
                now = Time.GetTime();
                double seconds_since_last_dmd_event = now - this.last_dmd_event;
                int missed_dmd_events = Math.Min((int)seconds_since_last_dmd_event * this.frames_per_second, 16);
                if (missed_dmd_events > 0)
                {
                    this.last_dmd_event = now;
                    for (int i = 0; i < missed_dmd_events; i++)
                        events.Add(new Event() { Type = EventType.DMDFrameDisplayed, Value = 0 });
                }
            }            
            return events.ToArray();
        }
        public ILogger Logger { get; set; }
        public void Reset(uint flags) { }
        public Result WriteData(uint module, uint startingAddr, ref uint data) => Result.Success;
        public Result ReadData(uint module, uint startingAddr, ref uint data) => Result.Success;
        public void SetDmdColorMapping(byte[] mapping) { }
        /// <summary>
        /// Set None to every switch
        /// </summary>
        /// <returns></returns>
        public EventType[] SwitchGetStates()
        {
            EventType[] result = new EventType[256];
            for (int i = 0; i < 256; i++)
                result[i] = EventType.None;

            return result;
        }
        public void SwitchUpdateRule(ushort number, EventType event_type, SwitchRule rule, DriverState[] linked_drivers, bool drive_outputs_now)
        {
            int rule_index = ((int)event_type * 256) + number;
            List<IDriver> d = new List<IDriver>();
            if (linked_drivers != null)
            {
                foreach (DriverState s in linked_drivers)
                {
                    d.Add(drivers[s.DriverNum]);
                }
            }
            if (rule_index >= switch_rules.Length)
                return;
            this.switch_rules[rule_index] = new FakeSwitchRule() { Drivers = d, NotifyHost = rule.NotifyHost };
        }
        public void WatchDogTickle()
        {
            foreach (IVirtualDriver d in this.drivers.Values) d.Tick();
        }
        /// <summary>
        /// Adds switch event to <see cref="switch_events"/>
        /// </summary>
        /// <param name="number"></param>
        /// <param name="event_type"></param>
        public void AddSwitchEvent(ushort number, EventType event_type)
        {
            int rule_index = (((int)event_type - 1) * 256) + number;

            if (this.switch_rules[rule_index].NotifyHost)
            {
                Event evt = new Event() { Type = event_type, Value = number };
                this.switch_events.Add(evt);
            }

            List<IDriver> dlist = this.switch_rules[rule_index].Drivers;
            foreach (IDriver drv in dlist)
                this.drivers[drv.Number].State = drv.State;
        }
        public Result DriverFuturePulse(ushort number, byte milliseconds, UInt16 futureTime) => Result.Success;
        public void i2c_write8(uint address, uint register, uint value) { }
		public void initialize_i2c(uint address) { }
        /// <summary>
        /// NOT USED, FAKE
        /// </summary>
        /// <param name="config"></param>
        /// <param name="_coils"></param>
        /// <param name="_switches"></param>
        /// <param name="_lamps"></param>
        /// <param name="_leds"></param>
        /// <param name="_gi"></param>
        public void SetupProcMachine(MachineConfiguration config, AttrCollection<ushort, string, IDriver> _coils = null,
            AttrCollection<ushort, string, Switch> _switches = null, AttrCollection<ushort, string, IDriver> _lamps = null, AttrCollection<ushort, string, LED> _leds = null, AttrCollection<ushort, string, IDriver> _gi = null)
        {
            List<VirtualDriver> new_virtual_drivers = new List<VirtualDriver>();
            bool polarity = (_machineType == MachineType.SternWhitestar || _machineType == MachineType.SternSAM || _machineType == MachineType.PDB);

            PDBConfig pdb_config = null;
            if (_machineType == MachineType.PDB)
                pdb_config = new PDBConfig(this, config);

            //process and add coils, add virtual driver, drivers
            if (_coils != null)
            {
                foreach (CoilConfigFileEntry ce in config.PRCoils)
                {
                    Driver d;
                    int number;
                    if (_machineType == MachineType.PDB && pdb_config != null)
                    {
                        number = pdb_config.GetProcNumber("PRCoils", ce.Number);

                        if (number == -1)
                        {
                            Console.WriteLine("Coil {0} cannot be controlled by the P-ROC. Ignoring...", ce.Name);
                            continue;
                        }
                    }
                    else
                        number = PinProc.PRDecode(_machineType, ce.Number);

                    if ((ce.Bus != null && ce.Bus == "AuxPort") || number >= PinProc.kPRDriverCount)
                    {
                        d = new VirtualDriver(this, ce.Name, (ushort)number, polarity);
                        new_virtual_drivers.Add((VirtualDriver)d);
                    }
                    else
                    {
                        d = new Driver(this, ce.Name, (ushort)number);
                        Logger?.Log("Adding driver " + d.ToString());
                        d.reconfigure(ce.Polarity);
                    }
                    _coils.Add(d.Number, d.Name, d);
                }
            }

            //process and add leds
            if (_machineType == MachineType.PDB && _leds != null)
            {
                ushort i = 0;
                foreach (LampConfigFileEntry le in config.PRLeds)
                {
                    LED led = new LED(this, le.Name, i, le.Number);
                    string number;
                    number = le.Number;
                    led.Polarity = le.Polarity;
                    _leds.Add(i, led.Name, led);
                    i++;
                }
            }

            if (_switches != null)
            {
                foreach (SwitchConfigFileEntry se in config.PRSwitches)
                {
                    //Log (se.Number);
                    var s = new Switch(this, se.Name, PinProc.PRDecode(_machineType, se.Number), se.Type);

                    ushort number = 0;
                    if (_machineType == MachineType.PDB)
                    {
                        var num = pdb_config.GetProcNumber("PRSwitches", se.Number);
                        if (num == -1)
                        {
                            Console.WriteLine("Switch {0} cannot be controlled by the P-ROC. Ignoring...", se.Name);
                            continue;
                        }
                        else
                        {
                            number = Convert.ToUInt16(num);
                        }
                    }
                    else
                    {
                        number = PinProc.PRDecode(_machineType, se.Number);
                    }

                    s.Number = number;
                    SwitchUpdateRule(number,
                        EventType.SwitchClosedDebounced,
                        new SwitchRule { NotifyHost = true, ReloadActive = false },
                        null,
                        false
                    );
                    SwitchUpdateRule(number,
                        EventType.SwitchOpenDebounced,
                        new SwitchRule { NotifyHost = true, ReloadActive = false },
                        null,
                        false
                    );
                    Logger?.Log("Adding switch " + s.ToString());
                    _switches.Add(s.Number, s.Name, s);
                }

                /// TODO: THIS SHOULD RETURN A LIST OF STATES
                EventType[] states = SwitchGetStates();
                foreach (Switch s in _switches.Values)
                {
                    s.SetState(states[s.Number] == EventType.SwitchClosedDebounced);
                }
            }

            if (_gi != null)
            {
                foreach (GIConfigFileEntry ge in config.PRGI)
                {
                    Driver d = new Driver(this, ge.Name, PinProc.PRDecode(_machineType, ge.Number));
                    Logger?.Log("Adding GI " + d.ToString());
                    _gi.Add(d.Number, d.Name, d);
                }
            }

            foreach (VirtualDriver virtual_driver in new_virtual_drivers)
            {
                int base_group_number = virtual_driver.Number / 8;
                List<Driver> items_to_remove = new List<Driver>();
                foreach (Driver d in _coils?.Values)
                {
                    if (d.Number / 8 == base_group_number)
                        items_to_remove.Add(d);
                }
                foreach (Driver d in items_to_remove)
                {
                    _coils.Remove(d.Name);
                    VirtualDriver vd = new VirtualDriver(this, d.Name, d.Number, polarity);
                    _coils.Add(d.Number, d.Name, d);
                }
                items_to_remove.Clear();
                foreach (Driver d in _lamps?.Values)
                {
                    if (d.Number / 8 == base_group_number)
                        items_to_remove.Add(d);
                }
                foreach (Driver d in items_to_remove)
                {
                    _lamps.Remove(d.Name);
                    VirtualDriver vd = new VirtualDriver(this, d.Name, d.Number, polarity);
                    _lamps.Add(d.Number, d.Name, d);
                }
            }
        }
    }
}
