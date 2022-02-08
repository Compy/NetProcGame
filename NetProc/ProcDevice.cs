using NetProc.Dmd;
using NetProc.Pdb;
using System;
using System.Collections.Generic;
using System.Threading;

namespace NetProc
{
    /// <summary>
    /// Wrapper for NetProcGame (libpinproc native interface).
    /// </summary>
    public class ProcDevice : IProcDevice
    {
        public IntPtr ProcHandle;
        public MachineType g_machineType;
        private static bool firstTime = true;
        private bool dmdConfigured = false;
        private int dmdMappingSize = 16;
        byte[] dmdMapping;

        byte[] testFrame = new byte[128 * 32];

        public bool swCoindoor = false;

        private object procSyncObject = new object();

        /// <summary>
        /// TODO: implement coil driver object list
        /// </summary>
        protected internal AttrCollection<ushort, string, IDriver> Coils;

        public ILogger Logger { get; set; }

        public ProcDevice(MachineType machineType, ILogger logger = null)
        {
            this.Logger = logger;

            Logger?.Log("Initializing P-ROC device...");

            dmdMapping = new byte[dmdMappingSize];
            for (int i = 0; i < dmdMappingSize; i++)
                dmdMapping[i] = (byte)i;

            g_machineType = machineType;

            dmdConfigured = false;

            ProcHandle = PinProc.PRCreate(machineType);
            if (ProcHandle == IntPtr.Zero)
                throw new InvalidOperationException(PinProc.PRGetLastErrorText());

            this.Coils = new AttrCollection<ushort, string, IDriver>();
        }

        public void Close()
        {
            if (ProcHandle != IntPtr.Zero)
                PinProc.PRDelete(ProcHandle);

            ProcHandle = IntPtr.Zero;
        }

        public void flush()
        {
            lock (procSyncObject)
            {
                PinProc.PRFlushWriteData(ProcHandle);
            }
        }


        #region DRIVER FUNCTIONS
        public Result DriverPulse(ushort number, byte milliseconds)
        {
            DriverState state = this.DriverGetState(number);
            Result res;
            lock (procSyncObject)
            {
                PinProc.PRDriverStatePulse(ref state, milliseconds);
                res = PinProc.PRDriverUpdateState(ProcHandle, ref state);
            }

            if (res == Result.Success)
            {
                lock (procSyncObject)
                {
                    res = PinProc.PRDriverWatchdogTickle(ProcHandle);
                    res = PinProc.PRFlushWriteData(ProcHandle);
                }
            }
            return res;
        }

        public Result DriverFuturePulse(ushort number, byte milliseconds, UInt16 futureTime)
        {
            DriverState state = this.DriverGetState(number);
            Result res;

            lock (procSyncObject)
            {
                PinProc.PRDriverStateFuturePulse(ref state, milliseconds, futureTime);
                res = PinProc.PRDriverUpdateState(ProcHandle, ref state);
            }

            if (res == Result.Success)
            {
                lock (procSyncObject)
                {
                    res = PinProc.PRDriverWatchdogTickle(ProcHandle);
                    res = PinProc.PRFlushWriteData(ProcHandle);
                }
            }
            return res;
        }

        public void DriverSchedule(ushort number, uint schedule, ushort cycle_seconds, bool now)
        {
            DriverState state = this.DriverGetState(number);
            lock (procSyncObject)
            {
                PinProc.PRDriverStateSchedule(ref state, schedule, (byte)cycle_seconds, now);
                PinProc.PRDriverUpdateState(ProcHandle, ref state);
                PinProc.PRFlushWriteData(ProcHandle);
            }
        }

        public void DriverPatter(ushort number, ushort milliseconds_on, ushort milliseconds_off, ushort original_on_time)
        {
            DriverState state = this.DriverGetState(number);
            lock (procSyncObject)
            {
                PinProc.PRDriverStatePatter(ref state, milliseconds_on, milliseconds_off, original_on_time);
                PinProc.PRDriverUpdateState(ProcHandle, ref state);
                PinProc.PRFlushWriteData(ProcHandle);
            }
        }

        public void DriverPulsedPatter(ushort number, ushort milliseconds_on, ushort milliseconds_off, ushort milliseconds_overall_patter_time)
        {
            DriverState state = this.DriverGetState(number);
            lock (procSyncObject)
            {
                PinProc.PRDriverStatePulsedPatter(ref state, milliseconds_on, milliseconds_off, milliseconds_overall_patter_time);
                PinProc.PRDriverUpdateState(ProcHandle, ref state);
                PinProc.PRFlushWriteData(ProcHandle);
            }
        }

        public void DriverGroupDisable(byte number)
        {
            lock (procSyncObject)
            {
                PinProc.PRDriverGroupDisable(ProcHandle, number);
            }
        }

        public void DriverDisable(ushort number)
        {
            DriverState state = this.DriverGetState(number);
            lock (procSyncObject)
            {
                PinProc.PRDriverStateDisable(ref state);
                PinProc.PRDriverUpdateState(ProcHandle, ref state);
                PinProc.PRFlushWriteData(ProcHandle);
            }
        }

        public DriverState DriverGetState(ushort number)
        {
            DriverState ds = new DriverState();
            lock (procSyncObject)
            {
                PinProc.PRDriverGetState(ProcHandle, (byte)number, ref ds);
            }
            return ds;
        }

        public void driver_update_state(ref DriverState driver)
        {
            lock (procSyncObject)
            {
                PinProc.PRDriverUpdateState(ProcHandle, ref driver);
            }
        }

        public void driver_update_global_config(bool enable, bool polarity, bool use_clear, bool strobe_start_select,
            byte start_strobe_time, byte matrix_row_enable_index0, byte matrix_row_enable_index1,
            bool active_low_matrix_rows, bool tickle_stern_watchdog, bool encode_enables, bool watchdog_expired,
            bool watchdog_enable, ushort watchdog_reset_time)
        {
            lock (procSyncObject)
            {
                DriverGlobalConfig globals = new DriverGlobalConfig();
                globals.EnableOutputs = enable;
                globals.GlobalPolarity = polarity;
                globals.UseClear = use_clear;
                globals.StrobeStartSelect = strobe_start_select;
                globals.StartStrobeTime = start_strobe_time;
                globals.MatrixRowEnableIndex0 = matrix_row_enable_index0;
                globals.MatrixRowEnableIndex1 = matrix_row_enable_index1;
                globals.ActiveLowMatrixRows = active_low_matrix_rows;
                globals.TickleSternWatchdog = tickle_stern_watchdog;
                globals.EncodeEnables = encode_enables;
                globals.WatchdogExpired = watchdog_expired;
                globals.WatchdogEnable = watchdog_enable;
                globals.WatchdogResetTime = watchdog_reset_time;

                PinProc.PRDriverUpdateGlobalConfig(ProcHandle, ref globals);
            }
        }

        public void driver_update_group_config(byte group_num, ushort slow_time, byte enable_index, byte row_activate_index,
            byte row_enable_select, bool matrixed, bool polarity, bool active, bool disable_strobe_after)
        {
            lock (procSyncObject)
            {
                DriverGroupConfig group = new DriverGroupConfig();
                group.GroupNum = group_num;
                group.SlowTime = slow_time;
                group.EnableIndex = enable_index;
                group.RowActivateIndex = row_activate_index;
                group.RowEnableSelect = row_enable_select;
                group.Matrixed = matrixed;
                group.Polarity = polarity;
                group.Active = active;
                group.DisableStrobeAfter = disable_strobe_after;

                PinProc.PRDriverUpdateGroupConfig(ProcHandle, ref group);
            }
        }

        public DriverState DriverStatePulse(DriverState state, byte milliseconds)
        {
            lock (procSyncObject)
            {
                PinProc.PRDriverStatePulse(ref state, milliseconds);
            }
            return state;
        }

        public DriverState DriverStateFuturePulse(DriverState state, byte milliseconds, UInt16 futureTime)
        {
            lock (procSyncObject)
            {
                PinProc.PRDriverStateFuturePulse(ref state, milliseconds, futureTime);
            }
            return state;
        }

        public DriverState DriverStateDisable(DriverState state)
        {
            lock (procSyncObject)
            {
                PinProc.PRDriverStateDisable(ref state);
            }
            return state;
        }

        public DriverState driver_state_schedule(DriverState state, uint schedule, byte seconds, bool now)
        {
            lock (procSyncObject)
            {
                PinProc.PRDriverStateSchedule(ref state, schedule, seconds, now);
            }
            return state;
        }

        public DriverState DriverStatePatter(DriverState state, ushort milliseconds_on, ushort milliseconds_off, ushort original_on_time)
        {
            lock (procSyncObject)
            {
                PinProc.PRDriverStatePatter(ref state, milliseconds_on, milliseconds_off, original_on_time);
            }
            return state;
        }

        public DriverState DriverStatePulsedPatter(DriverState state, ushort milliseconds_on, ushort milliseconds_off, ushort milliseconds_overall_patter_time)
        {
            lock (procSyncObject)
            {
                PinProc.PRDriverStatePulsedPatter(ref state, milliseconds_on, milliseconds_off, milliseconds_overall_patter_time);
            }
            return state;
        } 
        #endregion

        ///////////////////////////////////////////////////////////////////////////////
        /// SWITCH FUNCTIONS
        ///////////////////////////////////////////////////////////////////////////////

        public EventType[] SwitchGetStates()
        {
            ushort numSwitches = PinProc.kPRSwitchPhysicalLast + 1;
            EventType[] procSwitchStates = new EventType[numSwitches];
            lock (procSyncObject)
            {
                PinProc.PRSwitchGetStates(ProcHandle, procSwitchStates, numSwitches);
            }
            return procSwitchStates;
        }

        public void switch_update_rule(ushort number, EventType event_type, SwitchRule rule, DriverState[] linked_drivers, bool drive_outputs_now)
        {
            int numDrivers = 0;
            if (linked_drivers != null)
                numDrivers = linked_drivers.Length;

            bool use_column_8 = g_machineType == MachineType.WPC;

            if (firstTime)
            {
                firstTime = false;
                SwitchConfig switchConfig = new SwitchConfig();
                switchConfig.Clear = false;
                switchConfig.UseColumn8 = use_column_8;
                switchConfig.UseColumn9 = false; // No WPC machines actually use this
                switchConfig.HostEventsEnable = true;
                switchConfig.DirectMatrixScanLoopTime = 2; // Milliseconds
                switchConfig.PulsesBeforeCheckingRX = 10;
                switchConfig.InactivePulsesAfterBurst = 12;
                switchConfig.PulsesPerBurst = 6;
                switchConfig.PulseHalfPeriodTime = 13; // Milliseconds
                lock (procSyncObject)
                {
                    PinProc.PRSwitchUpdateConfig(ProcHandle, ref switchConfig);
                }
            }
            Result r;
            lock (procSyncObject)
            {
                r = PinProc.PRSwitchUpdateRule(ProcHandle, (byte)number, event_type, ref rule, linked_drivers, numDrivers, drive_outputs_now);
            }
            if (r == Result.Success)
            {
                // Possibly we should flush the write data here
            }
            else
            {
                Logger.Log(String.Format("SwitchUpdateRule FAILED for #{0} event_type={1} numDrivers={2} drive_outputs_now={3}",
                    number, event_type.ToString(), numDrivers, drive_outputs_now));
            }
        }

        /// <summary>
        /// TODO: Send aux commands Not implemented yet
        /// </summary>
        /// <param name="address"></param>
        /// <param name="aux_commands"></param>
        public void AuxSendCommands(ushort address, ushort aux_commands)
        {
            throw new NotImplementedException();
        }

        ///////////////////////////////////////////////////////////////////////////////
        /// PROC BOARD INTERACTIONS
        ///////////////////////////////////////////////////////////////////////////////

        public void WatchDogTickle()
        {
            lock (procSyncObject)
            {
                PinProc.PRDriverWatchdogTickle(ProcHandle);
                PinProc.PRFlushWriteData(ProcHandle);
            }
        }

        public Event[] Getevents()
        {
            const int batchSize = 16; // Pyprocgame uses 16
            Event[] events = new Event[batchSize];

            int numEvents;

            lock (procSyncObject)
            {
                numEvents = PinProc.PRGetEvents(ProcHandle, events, batchSize);
            }

            if (numEvents <= 0) return null;

            return events;
        }

        public void Reset(uint flags)
        {
            lock (procSyncObject)
            {
                PinProc.PRReset(ProcHandle, flags);
            }
        }

		public Result WriteData(uint module, uint startingAddr, ref uint data)
		{
			Result r;
			lock (procSyncObject) {
				//Logger.Log(String.Format("write_data - thread: {0}", System.Threading.Thread.CurrentThread.Name));

				r = PinProc.PRWriteData(ProcHandle, module, startingAddr, 1, ref data);
				//Logger.Log(String.Format ("write_data module: {0} start_addr: {1} data: {2}",
				//                          module, startingAddr, data));
			}
			return r;
		}

		public Result ReadData(uint module, uint startingAddr, ref uint data)
		{
			Result r;

			lock (procSyncObject) {
				//Logger.Log(String.Format("read_data - thread: {0}", System.Threading.Thread.CurrentThread.Name));
				r = PinProc.PRReadData(ProcHandle, module, startingAddr, 1, ref data);
				//Logger.Log(String.Format ("read_data module: {0} start_addr: {1} data: {2}",
				//                          module, startingAddr, data));
			}
			return r;
		}

		public void i2c_write8(uint address, uint register, uint value)
		{
			this.WriteData (7, address << 9 | register, ref value);
		}

		public void initialize_i2c(uint address)
		{
			this.i2c_write8 (address, 0x00, 0x11); // Set sleep
			this.i2c_write8 (address, 0x01, 0x04); // Configure output
			//this.i2c_write8 (address, 0xFE, 130); // Set to 50Hz
			this.i2c_write8 (address, 0xFE, 102); // Set to 60Hz
			Thread.Sleep(10); // Sleep needed to sync PLL
			this.i2c_write8 (address, 0x00, 0x01); // No more sleeping
			Thread.Sleep(10);
		}


        #region DMD FUNCTIONS
        static ushort kDMDColumns = 128;
        static byte kDMDRows = 32;
        static byte kDMDSubFrames = 4;
        static byte kDMDFrameBuffers = 3;

        private void DMDConfigPopulateDefaults(ref DMDConfig dmdConfig)
        {
            dmdConfig.EnableFrameEvents = true;
            dmdConfig.NumRows = kDMDRows;
            dmdConfig.NumColumns = kDMDColumns;
            dmdConfig.NumSubFrames = kDMDSubFrames;
            dmdConfig.NumFrameBuffers = kDMDFrameBuffers;
            dmdConfig.AutoIncBufferWrPtr = true;

            for (int i = 0; i < dmdConfig.NumSubFrames; i++)
            {
                dmdConfig.RclkLowCycles[i] = 15;
                dmdConfig.LatchHighCycles[i] = 15;
                dmdConfig.DotclkHalfPeriod[i] = 1;
            }
            dmdConfig.DeHighCycles[0] = 90;
            dmdConfig.DeHighCycles[1] = 190;
            dmdConfig.DeHighCycles[2] = 50;
            dmdConfig.DeHighCycles[3] = 377;
        }

        public void DmdDraw(byte[] bytes)
        {
            if (!dmdConfigured)
            {
                DMDConfig dmdConfig = new DMDConfig(kDMDColumns, kDMDRows);
                DMDConfigPopulateDefaults(ref dmdConfig);
                PinProc.PRDMDUpdateConfig(ProcHandle, ref dmdConfig);
                dmdConfigured = true;
            }
            PinProc.PRDMDDraw(ProcHandle, bytes);
        }

        public void DmdDraw(Frame frame)
        {
            if (!dmdConfigured)
            {
                DMDConfig dmdConfig = new DMDConfig(kDMDColumns, kDMDRows);
                DMDConfigPopulateDefaults(ref dmdConfig);
                PinProc.PRDMDUpdateConfig(ProcHandle, ref dmdConfig);
                dmdConfigured = true;
            }
            //dmd_draw(testFrame);
            byte[] dots = new byte[4 * kDMDColumns * kDMDRows / 8];
            DMDGlobals.DMDFrameCopyPROCSubframes(ref frame.frame, dots, kDMDColumns, kDMDRows, 4, dmdMapping);
            DmdDraw(dots);
        }

        public void SetDmdColorMapping(byte[] mapping)
        {
        }

        public void DmdUpdateConfig(ushort[] high_cycles)
        {
            DMDConfig dmdConfig = new DMDConfig();
            DMDConfigPopulateDefaults(ref dmdConfig);
            if (high_cycles == null || high_cycles.Length != 4)
                return;

            for (int i = 0; i < 4; i++)
            {
                dmdConfig.DeHighCycles[i] = high_cycles[i];
            }
            lock (procSyncObject)
            {
                PinProc.PRDMDUpdateConfig(ProcHandle, ref dmdConfig);
            }
            dmdConfigured = true;

        }
        #endregion

        /// <summary>
        /// Sets up all PROC driver and switch rules from a machine config. Pass in attribute collections which are populated here.
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
            bool polarity = (g_machineType == MachineType.SternWhitestar || g_machineType == MachineType.SternSAM || g_machineType == MachineType.PDB);

            PDBConfig pdb_config = null;
            if (g_machineType == MachineType.PDB)
                pdb_config = new PDBConfig(this, config);

            //process and add coils, add virtual driver, drivers
            if (_coils != null)
            {
                foreach (CoilConfigFileEntry ce in config.PRCoils)
                {
                    Driver d;
                    int number;
                    if (g_machineType == MachineType.PDB && pdb_config != null)
                    {
                        number = pdb_config.GetProcNumber("PRCoils", ce.Number);

                        if (number == -1)
                        {
                            Console.WriteLine("Coil {0} cannot be controlled by the P-ROC. Ignoring...", ce.Name);
                            continue;
                        }
                    }
                    else
                        number = PinProc.PRDecode(g_machineType, ce.Number);

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
            if (g_machineType == MachineType.PDB && _leds != null)
            {
                ushort i = 0;
                foreach (LampConfigFileEntry le in config.PRLeds)
                {
                    LED led = new LED(this, le.Name, i, le.Number);
                    string number;
                    number = le.Number;
                    //todo: polarity
                    _leds.Add(i, led.Name, led);
                    i++;
                }
            }

            if(_switches != null)
            {
                foreach (SwitchConfigFileEntry se in config.PRSwitches)
                {
                    //Log (se.Number);
                    var s = new Switch(this, se.Name, PinProc.PRDecode(g_machineType, se.Number), se.Type);

                    ushort number = 0;
                    if (g_machineType == MachineType.PDB)
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
                        number = PinProc.PRDecode(g_machineType, se.Number);
                    }

                    s.Number = number;
                    switch_update_rule(number,
                        EventType.SwitchClosedDebounced,
                        new SwitchRule { NotifyHost = true, ReloadActive = false },
                        null,
                        false
                    );
                    switch_update_rule(number,
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
                    Driver d = new Driver(this, ge.Name, PinProc.PRDecode(g_machineType, ge.Number));
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

