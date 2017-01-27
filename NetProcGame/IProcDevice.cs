using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetProcGame.tools;

namespace NetProcGame
{
    public interface IProcDevice
    {
        void aux_send_commands(ushort address, ushort aux_commands);
        void Close();
        void dmd_draw(byte[] bytes);
        void dmd_draw(dmd.Frame frame);
        void dmd_update_config(ushort[] high_cycles);
        void driver_disable(ushort number);
        Result driver_future_pulse(ushort number, byte milliseconds, UInt16 futureTime);
        DriverState driver_get_state(ushort number);
        void driver_group_disable(byte number);
        void driver_patter(ushort number, ushort milliseconds_on, ushort milliseconds_off, ushort original_on_time);
        Result driver_pulse(ushort number, byte milliseconds);
        void driver_pulsed_patter(ushort number, ushort milliseconds_on, ushort milliseconds_off, ushort milliseconds_overall_patter_time);
        void driver_schedule(ushort number, uint schedule, ushort cycle_seconds, bool now);
        DriverState driver_state_disable(DriverState state);
        DriverState driver_state_future_pulse(DriverState state, byte milliseconds, UInt16 futureTime);
        DriverState driver_state_patter(DriverState state, ushort milliseconds_on, ushort milliseconds_off, ushort original_on_time);
        DriverState driver_state_pulse(DriverState state, byte milliseconds);
        DriverState driver_state_pulsed_patter(DriverState state, ushort milliseconds_on, ushort milliseconds_off, ushort milliseconds_overall_patter_time);
        DriverState driver_state_schedule(DriverState state, uint schedule, byte seconds, bool now);
        void driver_update_global_config(bool enable, bool polarity, bool use_clear, bool strobe_start_select,
            byte start_strobe_time, byte matrix_row_enable_index0, byte matrix_row_enable_index1,
            bool active_low_matrix_rows, bool tickle_stern_watchdog, bool encode_enables, bool watchdog_expired,
            bool watchdog_enable, ushort watchdog_reset_time);
        void driver_update_group_config(byte group_num, ushort slow_time, byte enable_index, byte row_activate_index,
            byte row_enable_select, bool matrixed, bool polarity, bool active, bool disable_strobe_after);
        void driver_update_state(ref DriverState driver);
        void flush();
        Event[] get_events();
        ILogger Logger { get; set; }
        void reset(uint flags);
        void set_dmd_color_mapping(byte[] mapping);
        EventType[] switch_get_states();
        void switch_update_rule(ushort number, EventType event_type, SwitchRule rule, DriverState[] linked_drivers, bool drive_outputs_now);
        void watchdog_tickle();

		Result write_data(uint module, uint address, ref uint data);
		Result read_data(uint module, uint address, ref uint data);

    }
}
