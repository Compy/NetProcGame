using System;
using System.Collections.Generic;
using NetProcGame.tools;
using System.Threading;
namespace NetProcGame
{
    /// <summary>
    /// Wrapper for netpinproc (libpinproc native interface).
    /// </summary>
    public class ProcDevice
    {
        public IntPtr ProcHandle;
        public MachineType g_machineType;
        private static bool firstTime = true;
        private bool dmdConfigured = false;
        private int dmdMappingSize = 16;
        char[] dmdMapping;

        private object procSyncObject = new object();

        public ILogger Logger { get; set; }

        public ProcDevice(MachineType machineType, ILogger logger)
        {
            this.Logger = logger;

            Logger.Log("Initializing P-ROC device...");

            dmdMapping = new char[dmdMappingSize];
            for (int i = 0; i < dmdMappingSize; i++)
                dmdMapping[i] = (char)i;

            g_machineType = machineType;

            dmdConfigured = false;

            ProcHandle = PinProc.PRCreate(machineType);
            if (ProcHandle == IntPtr.Zero)
                throw new InvalidOperationException(PinProc.PRGetLastErrorText());
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

        ///////////////////////////////////////////////////////////////////////////////
        /// DRIVER FUNCTIONS
        ///////////////////////////////////////////////////////////////////////////////

        public Result driver_pulse(ushort number, byte milliseconds)
        {
            DriverState state = this.driver_get_state(number);
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

        public void driver_schedule(ushort number, uint schedule, ushort cycle_seconds, bool now)
        {
            DriverState state = this.driver_get_state(number);
            lock (procSyncObject)
            {
                PinProc.PRDriverStateSchedule(ref state, schedule, (byte)cycle_seconds, now);
                PinProc.PRDriverUpdateState(ProcHandle, ref state);
                PinProc.PRFlushWriteData(ProcHandle);
            }
        }

        public void driver_patter(ushort number, ushort milliseconds_on, ushort milliseconds_off, ushort original_on_time)
        {
            DriverState state = this.driver_get_state(number);
            lock (procSyncObject)
            {
                PinProc.PRDriverStatePatter(ref state, milliseconds_on, milliseconds_off, original_on_time);
                PinProc.PRDriverUpdateState(ProcHandle, ref state);
                PinProc.PRFlushWriteData(ProcHandle);
            }
        }

        public void driver_pulsed_patter(ushort number, ushort milliseconds_on, ushort milliseconds_off, ushort milliseconds_overall_patter_time)
        {
            DriverState state = this.driver_get_state(number);
            lock (procSyncObject)
            {
                PinProc.PRDriverStatePulsedPatter(ref state, milliseconds_on, milliseconds_off, milliseconds_overall_patter_time);
                PinProc.PRDriverUpdateState(ProcHandle, ref state);
                PinProc.PRFlushWriteData(ProcHandle);
            }
        }

        public void driver_disable(ushort number)
        {
            DriverState state = this.driver_get_state(number);
            lock (procSyncObject)
            {
                PinProc.PRDriverStateDisable(ref state);
                PinProc.PRDriverUpdateState(ProcHandle, ref state);
                PinProc.PRFlushWriteData(ProcHandle);
            }
        }

        public DriverState driver_get_state(ushort number)
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

        public DriverState driver_state_pulse(DriverState state, byte milliseconds)
        {
            lock (procSyncObject)
            {
                PinProc.PRDriverStatePulse(ref state, milliseconds);
            }
            return state;
        }

        public DriverState driver_state_disable(DriverState state)
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

        public DriverState driver_state_patter(DriverState state, ushort milliseconds_on, ushort milliseconds_off, ushort original_on_time)
        {
            lock (procSyncObject)
            {
                PinProc.PRDriverStatePatter(ref state, milliseconds_on, milliseconds_off, original_on_time);
            }
            return state;
        }

        public DriverState driver_state_pulsed_patter(DriverState state, ushort milliseconds_on, ushort milliseconds_off, ushort milliseconds_overall_patter_time)
        {
            lock (procSyncObject)
            {
                PinProc.PRDriverStatePulsedPatter(ref state, milliseconds_on, milliseconds_off, milliseconds_overall_patter_time);
            }
            return state;
        }

        ///////////////////////////////////////////////////////////////////////////////
        /// SWITCH FUNCTIONS
        ///////////////////////////////////////////////////////////////////////////////

        public EventType[] switch_get_states()
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
        /// Not implemented yet
        /// </summary>
        /// <param name="address"></param>
        /// <param name="aux_commands"></param>
        public void aux_send_commands(ushort address, ushort aux_commands)
        {
            throw new NotImplementedException();
        }

        ///////////////////////////////////////////////////////////////////////////////
        /// PROC BOARD INTERACTIONS
        ///////////////////////////////////////////////////////////////////////////////

        public void watchdog_tickle()
        {
            lock (procSyncObject)
            {
                PinProc.PRDriverWatchdogTickle(ProcHandle);
                PinProc.PRFlushWriteData(ProcHandle);
            }
        }

        public Event[] get_events()
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

        public void reset(uint flags)
        {
            lock (procSyncObject)
            {
                PinProc.PRReset(ProcHandle, flags);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////
        /// DMD FUNCTIONS
        ///////////////////////////////////////////////////////////////////////////////
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

        public void dmd_draw(byte[] bytes)
        {
            if (!dmdConfigured)
            {
                DMDConfig dmdConfig = new DMDConfig(kDMDColumns,kDMDRows);
                DMDConfigPopulateDefaults(ref dmdConfig);
                PinProc.PRDMDUpdateConfig(ProcHandle, ref dmdConfig);
                dmdConfigured = true;
            }
            PinProc.PRDMDDraw(ProcHandle, bytes);
        }

        public void dmd_draw(dmd.Frame frame)
        {
            dmd_draw(frame.get_data());
        }

        public void set_dmd_color_mapping()
        {
        }

        public void dmd_update_config(ushort[] high_cycles)
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
    }
}

