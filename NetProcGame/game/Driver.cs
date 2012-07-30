using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetProcGame.tools;

namespace NetProcGame.game
{
    /// <summary>
    /// Represents a driver in a pinball machine, such as a lamp, coil/solenoid, or flasher.
    /// </summary>
    public class Driver : GameItem
    {
        /// <summary>
        /// Default number of milliseconds to pulse this driver.
        /// </summary>
        protected byte _default_pulse_time = (byte)30;

        /// <summary>
        /// Last time this driver's state was modified
        /// </summary>
        protected double _last_time_changed = 0;

        public Driver(GameController game, string name, ushort number)
            : base(game, name, number)
        {
            
        }

        /// <summary>
        /// Disables/turns off the driver
        /// </summary>
        public void Disable()
        {

            this._game.PROC.driver_disable(this._number);
            this._last_time_changed = Time.GetTime();
        }

        /// <summary>
        /// Enables the driver for the specified number of milliseconds.
        /// 
        /// If no parameters are provided, then the default pulse time is used.
        /// </summary>
        /// <param name="milliseconds">The number of milliseconds to pulse the coil</param>
        public void Pulse(int milliseconds = -1)
        {
            if (milliseconds < 0) milliseconds = _default_pulse_time;

            if (milliseconds > 255)
                throw new ArgumentOutOfRangeException("Milliseconds must be in range 0-255");

            this._game.PROC.driver_pulse(this._number, (byte)milliseconds);
            this._last_time_changed = Time.GetTime();
        }

        public void FuturePulse(int milliseconds = -1, UInt16 futureTime = 100)
        {
            if (milliseconds < 0) milliseconds = _default_pulse_time;

            if (milliseconds > 255)
                throw new ArgumentOutOfRangeException("Milliseconds must be in range 0-255");

            this._game.PROC.driver_future_pulse(this._number, (byte)milliseconds, futureTime);
            this._last_time_changed = Time.GetTime();
        }

        /// <summary>
        /// Enables a pitter-patter sequence.
        /// 
        /// It starts by activating the driver for 'orig_on_time' milliseconds.
        /// Then it repeatedly turns the driver on for 'on_time' milliseconds and off for 'off_time' milliseconds
        /// </summary>
        /// <param name="on_time"></param>
        /// <param name="off_time"></param>
        /// <param name="orig_on_time"></param>
        public void Patter(ushort on_time = 10, ushort off_time = 10, ushort orig_on_time = 0)
        {
            if (off_time < 0 || off_time > 255)
                throw new ArgumentOutOfRangeException("off_time must be in range 0-255");
            if (on_time < 0 || on_time > 255)
                throw new ArgumentOutOfRangeException("on_time must be in range 0-255");
            if (orig_on_time < 0 || orig_on_time > 255)
                throw new ArgumentOutOfRangeException("orig_on_time must be in range 0-255");

            this._game.PROC.driver_patter(this._number, off_time, on_time, orig_on_time);
            this._last_time_changed = Time.GetTime();
        }

        /// <summary>
        /// Enables a pitter-patter response that runs for 'run_time' milliseconds
        /// 
        /// Until it ends, the sequence repeatedly turns the driver on for 'on_time' milliseconds and off for 'off_time' milliseconds.
        /// </summary>
        /// <param name="on_time"></param>
        /// <param name="off_time"></param>
        /// <param name="run_time"></param>
        public void PulsedPatter(ushort on_time = 10, ushort off_time = 10, ushort run_time = 0)
        {
            if (off_time < 0 || off_time > 127)
                throw new ArgumentOutOfRangeException("off_time must be in range 0-127");
            if (on_time < 0 || on_time > 127)
                throw new ArgumentOutOfRangeException("on_time must be in range 0-127");
            if (run_time < 0 || run_time > 255)
                throw new ArgumentOutOfRangeException("run_time must be in range 0-255");

            this._game.PROC.driver_pulsed_patter(this._number, off_time, on_time, run_time);
            this._last_time_changed = Time.GetTime();
        }


        /// <summary>
        /// Schedules this driver to be enabled according to the given 'schedule' bitmask
        /// </summary>
        /// <param name="schedule"></param>
        /// <param name="cycle_seconds"></param>
        /// <param name="now"></param>
        public void Schedule(uint schedule, int cycle_seconds = 0, bool now = true)
        {
            this._game.PROC.driver_schedule(this._number, schedule, (byte)cycle_seconds, now);
            this._last_time_changed = Time.GetTime();
        }

        /// <summary>
        /// Enables this driver indefinitely.
        /// 
        /// WARNING!!!
        /// Never use this method with high voltage drivers such as coils and flashers!
        /// Instead, use time-limited methods such as 'pulse' and 'schedule'.
        /// </summary>
        public void Enable()
        {
            this.Schedule(0xffffffff, 0, true);
        }

        public void Tick()
        {
        }

        public DriverState State
        {
            get
            {
                return this._game.PROC.driver_get_state(this._number);
            }
            set
            {
            }
        }

        public double last_time_changed
        {
            get { return _last_time_changed; }
        }

        public override string ToString()
        {
            return String.Format("<Driver name={0} number={1}>", this.Name, this.Number);
        }

        public void reconfigure(bool polarity)
        {
            DriverState state = this.State;
            state.Polarity = polarity;
            _game.PROC.driver_update_state(ref state);
        }
    }
}
