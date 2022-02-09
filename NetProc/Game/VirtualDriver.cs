using NetProc.Tools;
using System;

namespace NetProc
{
    /// <summary>
    /// Represents a driver in a pinball machine such as a lamp, coil/solenoid or flasher that should be driven by Aux Port logic
    /// rather than directly by P-ROC hardware. This means any automatic logic to determine when to turn on or off the driver is
    /// handled by software in this class.
    /// </summary>
    public class VirtualDriver : Driver, IVirtualDriver
    {
        /// <summary>
        /// The current state of the driver. Active = True, Inactive = False
        /// </summary>
        protected bool _currentState = false;

        /// <summary>
        /// The current value of the driver taking into account the desired state and polarity
        /// </summary>
        protected bool _currentValue = false;

        /// <summary>
        /// The currently assigned function (pulse, schedule, patter, pulsed_patter
        /// </summary>
        protected string _function = "";

        /// <summary>
        /// Whether or not a function is currently active
        /// </summary>
        protected bool _functionActive = false;

        /// <summary>
        /// The next time the driver's state should change.
        /// </summary>
        protected double _nextActionTimeMs = 0;

        /// <summary>
        /// Virtual drivers have their own internal DriverState object since they are not
        /// actually on the board.
        /// </summary>
        protected DriverState _state;

        /// <summary>
        /// Function to be called when the driver needs to change state
        /// </summary>
        protected Delegate _stateChangeHandler = null;

        /// <summary>
        /// The time the drivers currently active function should end.
        /// </summary>
        protected double _timeMs = 0;

        public VirtualDriver(IProcDevice proc, string name, ushort number, bool polarity)
            : base(proc, name, number)
        {
            this._state = new DriverState();
            this._state.Polarity = polarity;
            this._state.Timeslots = 0x0;
            this._state.PatterEnable = false;
            this._state.DriverNum = number;
            this._state.PatterOnTime = 0;
            this._state.PatterOffTime = 0;
            this._state.State = false;
            this._state.OutputDriveTime = 0;
            this._state.WaitForFirstTimeSlot = false;

            this._currentValue = !(_currentState ^ this._state.Polarity);

        }

        public new DriverState State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
            }
        }

        public void ChangeState(bool newState)
        {
            this._currentState = newState;
            this._currentValue = !(this._currentState ^ this._state.Polarity);
            this._last_time_changed = Time.GetTime();
            if (this._stateChangeHandler != null) _stateChangeHandler.DynamicInvoke();
        }

        /// <summary>
        /// Disables (turns off) this driver.
        /// </summary>
        public new void Disable()
        {
            this._functionActive = false;
            this.ChangeState(false);
        }

        public new void Enable() => this.Schedule(0xffffffff, 0, true);

        public void IncSchedule()
        {
            this._nextActionTimeMs += 0.0325;
            // Does our state need to change?
            bool next_state = Convert.ToBoolean((this._state.Timeslots >> 1) & 0x1);
            if (next_state != this._currentState) this.ChangeState(next_state);

            // Rotate the schedule down
            this._state.Timeslots = this._state.Timeslots >> 1 | ((this._state.Timeslots << 31) & 0x80000000);
        }

        /// <summary>
        /// Enables this driver for 'milliseconds'
        /// </summary>
        /// <param name="milliseconds"></param>
        public new void Pulse(int milliseconds = -1)
        {
            this._function = "pulse";
            this._functionActive = true;
            if (milliseconds == -1)
                milliseconds = this._default_pulse_time;

            this.ChangeState(true);
            if (milliseconds == 0) this._timeMs = 0;
            else this._timeMs = Time.GetTime() + milliseconds / 1000;
        }

        /// <summary>
        /// Schedules the driver. todo: implement the now parameter
        /// </summary>
        /// <param name="schedule"></param>
        /// <param name="cycle_seconds"></param>
        /// <param name="now"></param>
        public new void Schedule(uint schedule, int cycle_seconds = 0, bool now = true)
        {
            this._function = "schedule";
            this._functionActive = true;
            this._state.Timeslots = schedule;
            if (cycle_seconds == 0) this._timeMs = 0;
            else this._timeMs = Time.GetTime() + cycle_seconds;

            uint test = (schedule & 0x1);
            this.ChangeState(Convert.ToBoolean(test));
            this._nextActionTimeMs = Time.GetTime() + 0.03125;
        }

        public new void Tick()
        {
            if (this._functionActive)
            {
                // Check for time expired. time_ms == 0 is a special case that never expires.
                if (Time.GetTime() >= this._timeMs && this._timeMs > 0)
                {
                    this.Disable();
                }
                else if (this._function == "schedule")
                {
                    if (Time.GetTime() >= this._nextActionTimeMs)
                    {
                        this.IncSchedule();
                    }
                }
            }
        }

        /// <summary>
        /// Generic state change request that represents the P-ROC's PRDriverUpdateState function
        /// </summary>
        /// <param name="newState"></param>
        public void UpdateState(DriverState newState)
        {
            // Copy the newState object to the current local state
            this._state.DriverNum = newState.DriverNum;
            this._state.OutputDriveTime = newState.OutputDriveTime;
            this._state.PatterEnable = newState.PatterEnable;
            this._state.PatterOffTime = newState.PatterOffTime;
            this._state.PatterOnTime = newState.PatterOnTime;
            this._state.Polarity = newState.Polarity;
            this._state.State = newState.State;
            this._state.Timeslots = newState.Timeslots;
            this._state.WaitForFirstTimeSlot = newState.WaitForFirstTimeSlot;

            if (!newState.State) this.Disable();
            else if (newState.Timeslots == 0) this.Pulse(newState.OutputDriveTime);
            else this.Schedule(newState.Timeslots, newState.OutputDriveTime, newState.WaitForFirstTimeSlot);
        }
    }
}
