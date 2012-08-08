using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetProcGame.game;
using NetProcGame.tools;

namespace NetProcGame
{
    public struct FakeSwitchRule
    {
        public bool NotifyHost;
        public bool ReloadActive;
        public List<Driver> Drivers;
    }
    public class FakePinProc : IProcDevice
    {
        private AttrCollection<ushort, string, VirtualDriver> drivers;
        private List<Event> switch_events = new List<Event>();
        private FakeSwitchRule[] switch_rules = new FakeSwitchRule[1024];
        private double now;
        private double last_dmd_event = 0;
        private int frames_per_second = 60;

        public FakePinProc(MachineType machineType)
        {
            this.drivers = new AttrCollection<ushort, string, VirtualDriver>();
            // Make 256 drivers
            for (ushort i = 0; i < 256; i++)
            {
                string name = "driver" + i.ToString();
                drivers.Add(i, name, new VirtualDriver(null, name, i, true));
            }
            // Instantiate default switch rules
            for (int j = 0; j < 1024; j++)
            {
                switch_rules[j] = new FakeSwitchRule() { NotifyHost = false, ReloadActive = false, Drivers = new List<Driver>() };
            }
        }
        public void aux_send_commands(ushort address, ushort aux_commands)
        {
        }

        public void Close()
        {
        }

        public void dmd_draw(byte[] bytes)
        {
        }

        public void dmd_draw(dmd.Frame frame)
        {
        }

        public void dmd_update_config(ushort[] high_cycles)
        {
        }

        public void driver_disable(ushort number)
        {
            this.drivers[number].Disable();
        }

        public DriverState driver_get_state(ushort number)
        {
            return this.drivers[number].State;
        }

        public void driver_group_disable(byte number)
        {
        }

        public void driver_patter(ushort number, ushort milliseconds_on, ushort milliseconds_off, ushort original_on_time)
        {
        }

        public Result driver_pulse(ushort number, byte milliseconds)
        {
            this.drivers[number].Pulse(milliseconds);
            return Result.Success;
        }

        public void driver_pulsed_patter(ushort number, ushort milliseconds_on, ushort milliseconds_off, ushort milliseconds_overall_patter_time)
        {
        }

        public void driver_schedule(ushort number, uint schedule, ushort cycle_seconds, bool now)
        {
            this.drivers[number].Schedule(schedule, cycle_seconds, now);
        }

        public DriverState driver_state_disable(DriverState state)
        {
            return state;
        }

        public DriverState driver_state_future_pulse(DriverState state, byte milliseconds, ushort futureTime)
        {
            return state;
        }

        public DriverState driver_state_patter(DriverState state, ushort milliseconds_on, ushort milliseconds_off, ushort original_on_time)
        {
            return state;
        }

        public DriverState driver_state_pulse(DriverState state, byte milliseconds)
        {
            return state;
        }

        public DriverState driver_state_pulsed_patter(DriverState state, ushort milliseconds_on, ushort milliseconds_off, ushort milliseconds_overall_patter_time)
        {
            return state;
        }

        public DriverState driver_state_schedule(DriverState state, uint schedule, byte seconds, bool now)
        {
            return state;
        }

        public void driver_update_global_config(bool enable, bool polarity, bool use_clear, bool strobe_start_select, byte start_strobe_time, byte matrix_row_enable_index0, byte matrix_row_enable_index1, bool active_low_matrix_rows, bool tickle_stern_watchdog, bool encode_enables, bool watchdog_expired, bool watchdog_enable, ushort watchdog_reset_time)
        {
        }

        public void driver_update_group_config(byte group_num, ushort slow_time, byte enable_index, byte row_activate_index, byte row_enable_select, bool matrixed, bool polarity, bool active, bool disable_strobe_after)
        {
        }

        public void driver_update_state(ref DriverState driver)
        {
            this.drivers[driver.DriverNum].State = driver;
        }

        public void flush()
        {
        }

        public Event[] get_events()
        {
            List<Event> events = new List<Event>();
            events.AddRange(this.switch_events);
            this.switch_events.Clear();
            now = Time.GetTime();
            double seconds_since_last_dmd_event = now - this.last_dmd_event;
            int missed_dmd_events = Math.Min((int)seconds_since_last_dmd_event * this.frames_per_second, 16);
            if (missed_dmd_events > 0)
            {
                this.last_dmd_event = now;
                for (int i = 0; i < missed_dmd_events; i++)
                    events.Add(new Event() { Type = EventType.DMDFrameDisplayed, Value = 0 });
            }
            return events.ToArray();
        }

        public tools.ILogger Logger
        {
            get;
            set;
        }

        public void reset(uint flags)
        {
        }

        public void set_dmd_color_mapping(byte[] mapping)
        {
        }

        public EventType[] switch_get_states()
        {
            EventType[] result = new EventType[256];
            for (int i = 0; i < 256; i++)
                result[i] = EventType.None;

            return result;
        }

        public void switch_update_rule(ushort number, EventType event_type, SwitchRule rule, DriverState[] linked_drivers, bool drive_outputs_now)
        {
            int rule_index = ((int)event_type * 256) + number;
            List<Driver> d = new List<Driver>();
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

        public void watchdog_tickle()
        {
            foreach (Driver d in this.drivers.Values) d.Tick();
        }

        public void add_switch_event(ushort number, EventType event_type)
        {
            int rule_index = (((int)event_type - 1) * 256) + number;

            if (this.switch_rules[rule_index].NotifyHost)
            {
                Event evt = new Event() { Type = event_type, Value = number };
                this.switch_events.Add(evt);
            }

            List<Driver> dlist = this.switch_rules[rule_index].Drivers;
            foreach (Driver drv in dlist)
                this.drivers[drv.Number].State = drv.State;
        }

        public Result driver_future_pulse(ushort number, byte milliseconds, UInt16 futureTime)
        {
            return Result.Success;
        }
    }
}
