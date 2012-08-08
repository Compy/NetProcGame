using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;

namespace NetProcGame.game
{
    public delegate bool SwitchAcceptedHandler(Switch sw);
    public delegate bool DelayedHandler(object param);
    public delegate void AnonDelayedHandler();
    public class Mode : IComparable
    {
        // Properties
        /// <summary>
        /// Reference to the hosting GameController object / descendant
        /// </summary>
        public GameController Game { get; set; }
        /// <summary>
        /// The priority of the mode in the queue
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// List of delayed switch handlers
        /// TODO: Consider renaming this to something that makes more sense
        /// </summary>
        private List<AcceptedSwitch> _accepted_switches = new List<AcceptedSwitch>();

        /// <summary>
        /// A list of delayed events/callbacks
        /// </summary>
        private List<Delayed> _delayed = new List<Delayed>();

        // Constants
        public const bool SWITCH_STOP = true;
        public const bool SWITCH_CONTINUE = false;

        public dmd.Layer layer = null;


        /// <summary>
        /// Creates a new Mode
        /// </summary>
        /// <param name="game">The parent GameController object</param>
        /// <param name="priority">The priority of this mode in the queue</param>
        public Mode(GameController game, int priority)
        {
            this.Game = game;
            this.Priority = priority;

            // Scan all switch handlers
            scan_switch_handlers();
            
        }

        /// <summary>
        /// Scan all statically defined switch handlers in mode classes and wire up handling events
        /// </summary>
        private void scan_switch_handlers()
        {
            // Get all methods in the mode class that match a certain regular expression
            Type t = this.GetType();
            MethodInfo[] methods = t.GetMethods();
            string regexPattern = "sw_(?<name>[a-zA-Z0-9]+)_(?<state>open|closed|active|inactive)(?<after>_for_(?<time>[0-9]+)(?<units>ms|s))?";
            Regex pattern = new Regex(regexPattern);
            foreach (MethodInfo m in methods)
            {
                MatchCollection matches = pattern.Matches(m.Name);
                string switchName = "";
                string switchState = "";
                bool hasTimeSpec = false;
                double switchTime = 0;
                string switchUnits = "";
                foreach (Match match in matches)
                {
                    int i = 0;
                    foreach (Group group in match.Groups)
                    {
                        if (group.Success == true)
                        {
                            string gName = pattern.GroupNameFromNumber(i);
                            string gValue = group.Value;
                            if (gName == "name")
                            {
                                switchName = gValue;
                            }
                            if (gName == "state")
                                switchState = gValue;

                            if (gName == "after")
                                hasTimeSpec = true;

                            if (gName == "time")
                                switchTime = Int32.Parse(gValue);

                            if (gName == "units")
                                switchUnits = gValue;

                        }
                        i++;
                    }
                }
                if (switchName != "" && switchState != "")
                {
                    if (hasTimeSpec && switchUnits == "ms")
                        switchTime = switchTime / 1000.0;

                    

                    SwitchAcceptedHandler swh = (SwitchAcceptedHandler)Delegate.CreateDelegate(typeof(SwitchAcceptedHandler), this, m);
                    add_switch_handler(switchName, switchState, switchTime, swh);
                }
            }
        }

        /// <summary>
        /// Adds a switch handler to the list
        /// </summary>
        /// <param name="Name">Switch Name</param>
        /// <param name="Event_Type">'open', 'closed', 'active' or 'inactive'</param>
        /// <param name="Delay">float number of seconds that the state should be held before invoking the handler, 
        /// or None if it should be invoked immediately.</param>
        /// <param name="Handler">The handler to invoke</param>
        public void add_switch_handler(string Name, string Event_Type, double Delay = 0, SwitchAcceptedHandler Handler = null)
        {
            EventType adjusted_event_type;
            if (Event_Type == "active")
            {
                if (Game.Switches[Name].Type == SwitchType.NO)
                    adjusted_event_type = EventType.SwitchClosedDebounced;
                else
                    adjusted_event_type = EventType.SwitchOpenDebounced;
            }
            else if (Event_Type == "inactive")
            {
                if (Game.Switches[Name].Type == SwitchType.NO)
                    adjusted_event_type = EventType.SwitchOpenDebounced;
                else
                    adjusted_event_type = EventType.SwitchClosedDebounced;
            }
            else if (Event_Type == "closed")
                adjusted_event_type = EventType.SwitchClosedDebounced;
            else
                adjusted_event_type = EventType.SwitchOpenDebounced;

            Switch sw = Game.Switches[Name];
            AcceptedSwitch asw = new AcceptedSwitch(Name, adjusted_event_type, Delay, Handler, sw);
            if (!_accepted_switches.Contains(asw))
                _accepted_switches.Add(asw);
        }

        /// <summary>
        /// Delay an event for the specified period of time
        /// </summary>
        /// <param name="Name">The name of the delayed event</param>
        /// <param name="Event_Type">The type of event to delay</param>
        /// <param name="Delay">The delay in ms before the callback is fired</param>
        /// <param name="Handler">The callback to fire</param>
        /// <param name="Param">The parameters to the given callback</param>
        public void delay(string Name, EventType Event_Type, double Delay, Delegate Handler, object Param = null)
        {
            Game.Logger.Log(String.Format("Adding delay name={0} Event_Type={1} delay={2}",
                Name,
                Event_Type,
                Delay));
            Delayed d = new Delayed(Name, tools.Time.GetTime() + Delay, Handler, Event_Type, Param);
            _delayed.Add(d);
            _delayed.Sort();
        }

        /// <summary>
        /// Cancel a delayed event
        /// </summary>
        /// <param name="Name">The name of the delay to cancel</param>
        public void cancel_delayed(string Name)
        {
            _delayed = _delayed.Where<Delayed>(x => x.Name != Name).ToList<Delayed>();
        }

        /// <summary>
        /// Handles a switch event
        /// 
        /// This is called each time that an event is read in from the controller board
        /// </summary>
        /// <param name="evt">The event and type that was read in from the PROC</param>
        /// <returns>true if the event was handled and should not be propagated, false to propagate to other modes</returns>
        public bool handle_event(Event evt)
        {
            string sw_name = Game.Switches[(ushort)evt.Value].Name;
            bool handled = false;
            /// Filter out all of the delayed events that have been disqualified by this state change.
            /// Remove all items that are for this switch (sw_name) but for a different state (type).
            /// In other words, keep delayed items pertaining to other switches, plus delayed items pertaining
            /// to this switch for another state

            //var newDelayed = from d in _delayed
            //                 where d.Name != sw_name && (int)d.Event_Type != (int)evt.Type
            //                 select d;

            //_delayed = newDelayed.ToList<Delayed>();
            _delayed = _delayed.FindAll(d => d.Name != sw_name);

            //_delayed = _delayed.Where<Delayed>(x => sw_name == x.Name && x.Event_Type != evt.Type).ToList<Delayed>();

            foreach (AcceptedSwitch asw in 
                _accepted_switches.Where<AcceptedSwitch>(
                    accepted => 
                        accepted.Event_Type == evt.Type 
                        && accepted.Name == sw_name).ToList<AcceptedSwitch>())
            {
                if (asw.Delay == 0)
                {
                    bool result = asw.Handler(Game.Switches[asw.Name]);

                    if (result == SWITCH_STOP)
                        handled = true;
                }
                else
                {
                    delay(sw_name, asw.Event_Type, asw.Delay, asw.Handler, asw.Param);
                }
            }

            return handled;
        }

        /// <summary>
        /// Notifies the mode that it is now active on the mode queue.
        /// </summary>
        public virtual void mode_started()
        {
            // This method should not be invoked directly; it is called by the GameController run loop
        }

        /// <summary>
        /// Notifies the mode that it has been removed from the mode queue
        /// </summary>
        public virtual void mode_stopped()
        {
            // This method should not be invoked directly. It is called by the GameController run loop
        }

        /// <summary>
        /// Notifies the mode that it is now the topmost mode on the mode queue
        /// </summary>
        public virtual void mode_topmost()
        {
            // This method should not be invoked directly, it is called by the GameController run loop
        }

        public virtual void mode_tick()
        {
            // Called by the game controller run loop during each loop when the mode is running
        }

        /// <summary>
        /// Called by the GameController to dispatch any delayed events
        /// </summary>
        public void dispatch_delayed()
        {
            double t = tools.Time.GetTime();
            int cnt = _delayed.Count;
            for (int i = 0; i < cnt; i++)
            {
                if (_delayed[i].Time <= t)
                {
                    Game.Logger.Log("dispatch_delayed() " + _delayed[i].Name + " " + _delayed[i].Time.ToString() + " <= " + t.ToString());
                    if (_delayed[i].Param != null)
                        _delayed[i].Handler.DynamicInvoke(_delayed[i].Param);
                    else
                        _delayed[i].Handler.DynamicInvoke(null);
                }
            }
            _delayed = _delayed.Where<Delayed>(x => x.Time > t).ToList<Delayed>();
        }

        public virtual void update_lamps()
        {
            // Called by the GameController to re-apply active lamp schedules
        }

        public override string ToString()
        {
            return String.Format("{0}   pri={1}", this.GetType().Name, this.Priority);
        }

        class AcceptedSwitch : IEquatable<AcceptedSwitch>
        {
            public string Name { get; set; }
            public EventType Event_Type { get; set; }
            public double Delay { get; set; }
            public SwitchAcceptedHandler Handler { get; set; }
            public object Param { get; set; }

            public AcceptedSwitch(string Name, EventType Event_Type, double Delay, SwitchAcceptedHandler Handler = null, object Param = null)
            {
                this.Name = Name;
                this.Event_Type = Event_Type;
                this.Delay = Delay;
                this.Handler = Handler;
                this.Param = Param;
            }

            public bool Equals(AcceptedSwitch other)
            {
                if (other.Delay == this.Delay && other.Name == this.Name
                    && other.Event_Type == this.Event_Type && other.Handler == this.Handler)
                {
                    return true;
                }
                return false;
            }

            public override string ToString()
            {
                return String.Format("<name={0} event_type={1} delay={2}>", this.Name, this.Event_Type, this.Delay);
            }
        }

        class Delayed : IComparable<Delayed>
        {
            public string Name { get; set; }
            public double Time { get; set; }
            public Delegate Handler { get; set; }
            public EventType Event_Type { get; set; }
            public object Param { get; set; }

            public Delayed(string Name, double Time, Delegate Handler = null, EventType Event_Type = EventType.SwitchClosedDebounced, object Param = null)
            {
                this.Name = Name;
                this.Time = Time;
                this.Handler = Handler;
                this.Event_Type = Event_Type;
                this.Param = Param;
            }

            public int CompareTo(Delayed other)
            {
                return other.Time.CompareTo(this.Time);
            }

            public override string ToString()
            {
                return String.Format("name={0} time={1} event_type={2}", this.Name, this.Time, this.Event_Type);
            }
        }

        public int CompareTo(object obj)
        {
            if (obj is Mode)
                return ((Mode)obj).Priority.CompareTo(Priority);
            else
                return -1;
        }
    }
}
