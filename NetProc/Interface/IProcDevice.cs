using NetProc.Dmd;
using System;

namespace NetProc
{
    public interface IProcDevice
    {
        ILogger Logger { get; set; }

        void AuxSendCommands(ushort address, ushort aux_commands);
        void Close();
        void DmdDraw(byte[] bytes);
        void DmdDraw(Frame frame);
        void DmdUpdateConfig(ushort[] high_cycles);
        DriverState DriverStatePulsedPatter(DriverState state, ushort milliseconds_on, ushort milliseconds_off, ushort milliseconds_overall_patter_time);

        DriverState driver_state_schedule(DriverState state, uint schedule, byte seconds, bool now);

        void driver_update_global_config(bool enable, bool polarity, bool use_clear, bool strobe_start_select,
            byte start_strobe_time, byte matrix_row_enable_index0, byte matrix_row_enable_index1,
            bool active_low_matrix_rows, bool tickle_stern_watchdog, bool encode_enables, bool watchdog_expired,
            bool watchdog_enable, ushort watchdog_reset_time);

        void driver_update_group_config(byte group_num, ushort slow_time, byte enable_index, byte row_activate_index,
            byte row_enable_select, bool matrixed, bool polarity, bool active, bool disable_strobe_after);

        void driver_update_state(ref DriverState driver);

        void DriverDisable(ushort number);
        Result DriverFuturePulse(ushort number, byte milliseconds, UInt16 futureTime);
        DriverState DriverGetState(ushort number);
        void DriverGroupDisable(byte number);
        void DriverPatter(ushort number, ushort milliseconds_on, ushort milliseconds_off, ushort original_on_time);
        Result DriverPulse(ushort number, byte milliseconds);
        void DriverPulsedPatter(ushort number, ushort milliseconds_on, ushort milliseconds_off, ushort milliseconds_overall_patter_time);
        void DriverSchedule(ushort number, uint schedule, ushort cycle_seconds, bool now);

        /// <summary>
        /// Disables (turns off) the given driver.
        /// This function is provided for convenience. See PRDriverStateDisable() for a full description.</summary>
        /// <param name="state"></param>
        /// <returns></returns>
        DriverState DriverStateDisable(DriverState state);
        DriverState DriverStateFuturePulse(DriverState state, byte milliseconds, UInt16 futureTime);
        DriverState DriverStatePatter(DriverState state, ushort milliseconds_on, ushort milliseconds_off, ushort original_on_time);
        DriverState DriverStatePulse(DriverState state, byte milliseconds);
        void flush();
        Event[] Getevents();
        void i2c_write8(uint address, uint register, uint value);

        void initialize_i2c(uint address);

        Result ReadData(uint module, uint address, ref uint data);

        void Reset(uint flags);
        void SetDmdColorMapping(byte[] mapping);
        EventType[] SwitchGetStates();
        void switch_update_rule(ushort number, EventType event_type, SwitchRule rule, DriverState[] linked_drivers, bool drive_outputs_now);
        void WatchDogTickle();

		Result WriteData(uint module, uint address, ref uint data);
    }
}
