using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NetProcGame.config;
using NetProcGame.tools;

namespace NetProcGame.game
{
    /// <summary>
    /// Core object representing the game itself.
    /// 
    /// Usually a game developer will create a new game by subclassing this class.
    /// Consider subclassing 'BasicGame' instead, as it makes use of several helpful modes and controllers.
    /// </summary>
    public class GameController
    {
        /// <summary>
        /// Machine type used to configure the proc device
        /// </summary>
        protected MachineType _machineType;

        /// <summary>
        /// A pinproc class instance, created in the constructor with the Machine_Type attribute
        /// </summary>
        protected ProcDevice _proc;

        /// <summary>
        /// TODO: Implement mode queue logic
        /// </summary>
        protected ModeQueue _modes;

        /// <summary>
        /// TODO: implement coil driver object list
        /// </summary>
        protected AttrCollection<ushort, string, Driver> _coils;

        /// <summary>
        /// TODO: implement lamp driver object list
        /// </summary>
        protected AttrCollection<ushort, string, Driver> _lamps;

        /// <summary>
        /// List of GI drivers
        /// </summary>
        protected AttrCollection<ushort, string, Driver> _gi;

        /// <summary>
        /// TODO: implement switch object lists
        /// </summary>
        protected AttrCollection<ushort, string, Switch> _switches;

        /// <summary>
        /// The number of the current ball. A value of 1 represents the first ball; 0 indicates game over.
        /// </summary>
        protected int _ball = 0;

        /// <summary>
        /// A collection of player objects
        /// </summary>
        protected List<Player> _players;

        /// <summary>
        /// A collection of old player objects if reset is called.
        /// </summary>
        protected List<Player> _old_players;

        /// <summary>
        /// The index in _players of the current player
        /// TODO: implement this logic
        /// </summary>
        public int current_player_index = 0;

        /// <summary>
        /// The date/time when the framework was started (machine powered up)
        /// </summary>
        protected double _bootTime = 0;

        /// <summary>
        /// The configuration object loaded by load_config()
        /// TODO: implement config handling
        /// </summary>
        protected MachineConfiguration _config;

        /// <summary>
        /// The number of balls per game
        /// </summary>
        protected int _balls_per_game = 3;

        /// <summary>
        /// Contains information specific to the particular location installation (high scores, audits, etc).
        /// </summary>
        protected object _game_data;

        /// <summary>
        /// Contains local game information such as volume
        /// </summary>
        protected object _user_settings;

        /// <summary>
        /// The total number of balls in the machine
        /// </summary>
        protected int _num_balls_total = 5;

        /// <summary>
        /// Are the flippers and bumpers currently enabled?
        /// </summary>
        protected bool _flippers_enabled = false;

        /// <summary>
        /// Run loop exit condition. This continues until true
        /// </summary>
        protected bool _done = false;

        /// <summary>
        /// The ending time of the current ball
        /// </summary>
        protected double _ball_end_time;

        /// <summary>
        /// The starting time of the current ball
        /// </summary>
        protected double _ball_start_time;

        /// <summary>
        /// Public logging interface class. Make sure all games have a class that implements this interface
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Thread synchronization object for coils
        /// </summary>
        protected object _coil_lock_object = new object();

        /// <summary>
        /// List of coils to drive that is manipulated from outside/UI threads
        /// TODO: This is set to be removed in favor of the UI process communication model
        /// </summary>
        protected List<SafeCoilDrive> _safe_coil_drive_queue = new List<SafeCoilDrive>();

        /// <summary>
        /// Current machine configuration representation
        /// </summary>
        public MachineConfiguration Config
        {
            get { return _config; }
        }

        /// <summary>
        /// The current ball the game is on
        /// </summary>
        public int ball { get; set; }

        public byte[] testFrame;

        /// <summary>
        /// Creates a new GameController object with the given machine type and logging infrastructure
        /// </summary>
        /// <param name="machineType">Machine type to use (WPC, STERN, PDB etc)</param>
        /// <param name="logger">The logger interface to use</param>
        public GameController(MachineType machineType, ILogger logger)
        {
            this.Logger = logger;
            this._machineType = machineType;
            this._proc = new ProcDevice(machineType, Logger);
            this._proc.reset(1);
            this._modes = new ModeQueue(this);
            this._bootTime = Time.GetTime();
            this._coils = new AttrCollection<ushort, string, Driver>();
            this._switches = new AttrCollection<ushort, string, Switch>();
            this._lamps = new AttrCollection<ushort, string, Driver>();
            this._gi = new AttrCollection<ushort, string, Driver>();
            this._old_players = new List<Player>();
            this._players = new List<Player>();

            testFrame = new byte[128 * 32];
            for (int i = 0; i < (128 * 32); i++)
            {
                testFrame[i] = 0;
            }
        }

        ~GameController()
        {
            this._proc = null;
        }

        /// <summary>
        /// Reset the game state to normal (like a slam tilt)
        /// </summary>
        public virtual void Reset()
        {
            ball = 0;
            _old_players.Clear();
            _old_players.AddRange(_players);
            _players.Clear();
            current_player_index = 0;
            _modes.Clear();
        }

        /// <summary>
        /// Creates a new player with a given name
        /// </summary>
        /// <param name="name">The name for the player to use, usually auto generated</param>
        /// <returns>A new player object</returns>
        public Player create_player(string name)
        {
            return new Player(name);
        }

        /// <summary>
        /// Adds a new player to 'Players' and auto-assigns a name
        /// </summary>
        /// <returns></returns>
        public Player add_player()
        {
            Player newPlayer = this.CreatePlayer("Player " + (_players.Count + 1).ToString());
            _players.Add(newPlayer);
            return newPlayer;
        }

        /// <summary>
        /// Returns the current 'Player' object according to the current_player_index value
        /// </summary>
        /// <returns></returns>
        public Player current_player()
        {
            if (this._players.Count > this.current_player_index)
                return this._players[this.current_player_index];
            else
                return null;
        }

        /// <summary>
        /// The ball time for the current player
        /// </summary>
        /// <returns>The ball time (in seconds) that the current ball has been in play</returns>
        public double get_ball_time()
        {
            return this._ball_end_time - this._ball_start_time;
        }

        /// <summary>
        /// The game time for the given player index
        /// </summary>
        /// <param name="player">The player index to calculate the game time for</param>
        /// <returns>The time in seconds the player has been playing the entire game</returns>
        public double get_game_time(int player)
        {
            return this._players[player].game_time;
        }

        /// <summary>
        /// Save the ball start time into local memory
        /// </summary>
        public void save_ball_start_time()
        {
            this._ball_start_time = Time.GetTime();
        }

        /// <summary>
        /// Called by the implementor to notify the game that the first ball should be started.
        /// </summary>
        public void start_ball()
        {
            this.ball_starting();
        }

        /// <summary>
        /// Called by the game framework when a new ball is starting
        /// </summary>
        public virtual void ball_starting()
        {
            this.save_ball_start_time();
        }

        /// <summary>
        /// Called by the game framework when a new ball is starting which was the result of a stored extra ball.
        /// The default implementation calls ball_starting() which is not called by the framework in this case.
        /// </summary>
        public virtual void shoot_again()
        {
            this.ball_starting();
        }

        /// <summary>
        /// Called by the game framework when the current ball has ended
        /// </summary>
        public virtual void ball_ended()
        {

        }

        /// <summary>
        /// Called by the implementor to notify the game that the current ball has ended
        /// </summary>
        public void end_ball()
        {
            this._ball_end_time = Time.GetTime();
            this.current_player().game_time += this.get_ball_time();
            this.ball_ended();

            if (this.current_player().extra_balls > 0)
            {
                this.current_player().extra_balls -= 1;
                this.shoot_again();
                return;
            }

            if (this.current_player_index + 1 == this._players.Count)
            {
                this.ball += 1;
                this.current_player_index = 0;
            }
            else
            {
                this.current_player_index += 1;
            }

            if (this.ball > this._balls_per_game)
            {
                this.end_game();
            }
            else
            {
                this.start_ball();
            }

        }

        /// <summary>
        /// Called by the GameController when a new game is starting.
        /// </summary>
        public virtual void game_started()
        {
            this.ball = 1;
            this._players = new List<Player>();
            this.current_player_index = 0;
        }

        /// <summary>
        /// Called by the implementor to notify the game that the game has started.
        /// </summary>
        public virtual void start_game()
        {
            this.game_started();
        }

        /// <summary>
        /// Called by the GameController when the current game has ended
        /// </summary>
        public virtual void game_ended()
        {
        }

        /// <summary>
        /// Called by the implementor to notify the game that the game as ended
        /// </summary>
        public void end_game()
        {
            this.game_ended();
            this.ball = 0;
        }

        /// <summary>
        /// Create a new machine configuration representation in memory from a file on disk
        /// </summary>
        /// <param name="PathToFile">The path to the configuration XML file</param>
        public void LoadConfig(string PathToFile)
        {
            MachineConfiguration config = MachineConfiguration.FromFile(PathToFile);
            foreach (CoilConfigFileEntry ce in config.PRCoils)
            {
                Driver d = new Driver(this, ce.Name, PinProc.PRDecode(_machineType, ce.Number));
                Log("Adding driver " + d.ToString());
                _coils.Add(d.Number, d.Name, d);
            }

            foreach (LampConfigFileEntry le in config.PRLamps)
            {
                Driver d = new Driver(this, le.Name, PinProc.PRDecode(_machineType, le.Number));
                Log("Adding lamp " + d.ToString());
                _lamps.Add(d.Number, d.Name, d);
            }

            foreach (GIConfigFileEntry ge in config.PRGI)
            {
                Driver d = new Driver(this, ge.Name, PinProc.PRDecode(_machineType, ge.Number));
                _gi.Add(d.Number, d.Name, d);
            }

            foreach (SwitchConfigFileEntry se in config.PRSwitches)
            {

                Switch s = new Switch(this, se.Name, PinProc.PRDecode(_machineType, se.Number), se.Type);
                
                _proc.switch_update_rule(s.Number, 
                    EventType.SwitchClosedDebounced, 
                    new SwitchRule { NotifyHost = true, ReloadActive = false }, 
                    null,
                    false
                );
                _proc.switch_update_rule(s.Number,
                    EventType.SwitchOpenDebounced,
                    new SwitchRule { NotifyHost = true, ReloadActive = false },
                    null,
                    false
                );
                Log("Adding switch " + s.ToString());
                _switches.Add(s.Number, s.Name, s);
            }

            /// TODO: THIS SHOULD RETURN A LIST OF STATES
            EventType[] states = _proc.switch_get_states();
            foreach (Switch s in _switches.Values)
            {
                s.SetState(states[s.Number] == EventType.SwitchClosedDebounced);
            }

            _num_balls_total = config.PRGame.numBalls;
            _config = config;

            if (_config.PRGame.displayMonitor)
            {
            }
        }

        /// <summary>
        /// Instantiates and returns a new instance of class 'Player' with the given name
        /// 
        /// This method is called by 'AddPlayer'
        /// </summary>
        /// <param name="Name">The name of the player</param>
        /// <returns>A new player object</returns>
        private Player CreatePlayer(string Name)
        {
            return new Player(Name);
        }

        /// <summary>
        /// PROC device driver wrapper
        /// </summary>
        public ProcDevice PROC
        {
            get { return _proc; }
        }

        public void link_flipper_switch(string switch_name, string[] linked_coils)
        {
            // Hold all of the linked coils * 2 (one for Main, one for Hold)
            DriverState[] drivers = new DriverState[linked_coils.Length * 2];
            Driver main_coil, hold_coil;
            ushort switch_num = _switches[switch_name].Number;
            int driverIdx = 0;
            if (_flippers_enabled)
            {
                foreach (string coil in linked_coils)
                {
                    main_coil = _coils[coil + "Main"];
                    hold_coil = _coils[coil + "Hold"];
                    drivers[driverIdx] = _proc.driver_state_pulse(main_coil.State, 34);
                    driverIdx++;
                    drivers[driverIdx] = _proc.driver_state_pulse(hold_coil.State, 0);
                    driverIdx++;
                }
                // Add switch rule for activating flippers (when switch closes)
                _proc.switch_update_rule(switch_num,
                    EventType.SwitchClosedNondebounced,
                    new SwitchRule { NotifyHost = false, ReloadActive = false },
                    drivers,
                    false
                );

                // --------------------------------------------------------------
                // Now add the rule for open switches and disabling flippers
                // --------------------------------------------------------------
                driverIdx = 0;
                foreach (string coil in linked_coils)
                {
                    main_coil = _coils[coil + "Main"];
                    hold_coil = _coils[coil + "Hold"];
                    drivers[driverIdx] = _proc.driver_state_disable(main_coil.State);
                    driverIdx++;
                    drivers[driverIdx] = _proc.driver_state_disable(hold_coil.State);
                    driverIdx++;
                }
                _proc.switch_update_rule(switch_num,
                    EventType.SwitchOpenNondebounced,
                    new SwitchRule { NotifyHost = false, ReloadActive = false },
                    drivers,
                    false
                );
            }
            else
            {
                // Remove all switch linkages
                _proc.switch_update_rule(switch_num,
                    EventType.SwitchClosedNondebounced,
                    new SwitchRule { NotifyHost = false, ReloadActive = false },
                    null,
                    false
                );
                _proc.switch_update_rule(switch_num,
                    EventType.SwitchOpenNondebounced,
                    new SwitchRule { NotifyHost = false, ReloadActive = false },
                    null,
                    false
                );
                // Disable flippers
                foreach (string coil in linked_coils)
                {
                    main_coil = _coils[coil + "Main"];
                    hold_coil = _coils[coil + "Hold"];
                    main_coil.Disable();
                    hold_coil.Disable();
                }
            }
        }

        /// <summary>
        /// Enables/disables flippers and other high-power coils (bumpers)
        /// </summary>
        public bool FlippersEnabled
        {
            get { return _flippers_enabled; }
            set
            {
                if (_flippers_enabled == value) return;
                Log("Setting flippers_enabled to " + value.ToString());
                _flippers_enabled = value;
                if (_machineType == MachineType.WPC || _machineType == MachineType.WPC95 || _machineType == MachineType.WPCAlphanumeric)
                {
                    link_flipper_switch("flipperLwR", _config.PRFlipperRight.ToArray());
                    link_flipper_switch("flipperLwL", _config.PRFlipperLeft.ToArray());
                    /*
                    DriverState[] drivers;
                    drivers = new DriverState[2];
                    
                    foreach (string flipper in _config.PRFlippers)
                    {
                        Driver main_coil = _coils[flipper + "Main"];
                        Driver hold_coil = _coils[flipper + "Hold"];
                        ushort switch_num = _switches[flipper].Number;

                        if (_flippers_enabled)
                        {
                            drivers[0] = _proc.driver_state_pulse(main_coil.State, 34);
                            drivers[1] = _proc.driver_state_pulse(hold_coil.State, 0);
                        }
                        else
                        {
                            drivers = null;
                        }
                        _proc.switch_update_rule(
                            switch_num,
                            EventType.SwitchClosedNondebounced,
                            new SwitchRule { NotifyHost = false, ReloadActive = false },
                            drivers,
                            false
                        );

                        drivers = new DriverState[2];
                        if (_flippers_enabled)
                        {
                            drivers[0] = _proc.driver_state_disable(main_coil.State);
                            drivers[1] = _proc.driver_state_disable(hold_coil.State);
                        }
                        else
                        {
                            drivers = null;
                        }
                        _proc.switch_update_rule(switch_num,
                            EventType.SwitchOpenNondebounced,
                            new SwitchRule { NotifyHost = false, ReloadActive = false },
                            drivers,
                            false
                        );

                        if (!_flippers_enabled)
                        {
                            main_coil.Disable();
                            hold_coil.Disable();
                        }
                    }
                    */
                }
                // Enable the flipper relay on WPC alpha numeric machines
                if (_machineType == MachineType.WPCAlphanumeric)
                {
                    if (_flippers_enabled)
                        _coils[79].Pulse(0);
                    else
                        _coils[79].Disable();
                }
                else if (_machineType == MachineType.SternWhitestar || _machineType == MachineType.SternSAM)
                {
                    foreach (string flipper in _config.PRFlippers)
                    {
                        Driver main_coil = _coils[flipper + "Main"];
                        ushort switch_num = PinProc.PRDecode(_machineType, _switches[flipper].Number.ToString());
                        DriverState[] drivers = new DriverState[1];
                        if (_flippers_enabled)
                            drivers[0] = _proc.driver_state_patter(main_coil.State, 2, 18, 34);
                        else
                            drivers = null;

                        _proc.switch_update_rule(switch_num,
                            EventType.SwitchClosedNondebounced,
                            new SwitchRule { NotifyHost = false, ReloadActive = false },
                            drivers,
                            false
                        );

                        drivers = new DriverState[1];
                        if (_flippers_enabled)
                            drivers[0] = _proc.driver_state_disable(main_coil.State);
                        else
                            drivers = null;

                        _proc.switch_update_rule(switch_num,
                            EventType.SwitchOpenNondebounced,
                            new SwitchRule { NotifyHost = false, ReloadActive = false },
                            drivers,
                            false
                        );

                        if (!_flippers_enabled)
                            main_coil.Disable();
                    }
                }
                foreach (string bumper in _config.PRBumpers)
                {
                    ushort switch_num = _switches[bumper].Number;
                    Driver coil = _coils[bumper];

                    DriverState[] drivers = new DriverState[1];
                    if (_flippers_enabled)
                        drivers[0] = _proc.driver_state_pulse(coil.State, 20);
                    else
                        drivers = null;

                    _proc.switch_update_rule(switch_num,
                        EventType.SwitchClosedNondebounced,
                        new SwitchRule { NotifyHost = false, ReloadActive = true },
                        drivers,
                        false
                    );
                }
            }
        }

        /// <summary>
        /// Process the retrieved event from the PROC interface board (switch/dmd events)
        /// </summary>
        /// <param name="evt">The event to process</param>
        public void process_event(Event evt)
        {
            if (evt.Type == EventType.None)
            {
            }
            else if (evt.Type == EventType.Invalid)
            {
                // Invalid event type, end run loop perhaps
            }
            // DMD events
            else if (evt.Type == EventType.DMDFrameDisplayed)
            {
                this.dmd_event();
            }
            else
            {
                Switch sw = _switches[(ushort)evt.Value];
                bool recvd_state = evt.Type == EventType.SwitchClosedDebounced;

                if (!sw.IsState(recvd_state))
                {
                    sw.SetState(recvd_state);
                    _modes.handle_event(evt);
                }
            }
        }

        public void update_lamps()
        {
            //for (int i = modes.length - 1; i >= 0; i--)
            //{
            //   modes[i].update_lamps();
            //}
        }

        /// <summary>
        /// Set the exit condition for the run loop causing the game to terminate
        /// </summary>
        public void end_run_loop()
        {
            _done = true;
        }

        /// <summary>
        /// Retrieve all events from the PROC interface board
        /// </summary>
        /// <returns>A list of events from the PROC</returns>
        public virtual Event[] get_events()
        {
            return _proc.get_events();
        }

        /// <summary>
        /// Propagate Tick events to all lamps and coils
        /// </summary>
        public void tick_virtual_drivers()
        {
            foreach (Driver coil in _coils.Values)
            {
                coil.Tick();
            }

            foreach (Driver lamp in _lamps.Values)
            {
                lamp.Tick();
            }
        }

        public virtual void tick()
        {
        }

        /// <summary>
        /// Called by the GameController when a DMD event has been received.
        /// </summary>
        public virtual void dmd_event()
        {
        }

        /// <summary>
        /// Main run loop of the program. Performs the following logic until the loop ends:
        ///     - Get events from PROC
        ///     - Process this list of events across all modes
        ///     - 'Tick' modes
        ///     - Tickle watchdog
        /// </summary>
        public void run_loop()
        {
            long loops = 0;
            _done = false;
            dmd_event();
            Event[] events;
            try
            {
                while (!_done)
                {
                    loops++;
                    events = get_events();
                    if (events != null)
                    {
                        foreach (Event evt in events)
                        {
                            process_event(evt);
                            
                        }
                    }

                    this.tick();
                    //tick_virtual_drivers();
                    this._modes.tick();
                    //this.modes.tick();

                    // Do we have any events waiting such as pulses from the UI
                    lock (_coil_lock_object)
                    {
                        SafeCoilDrive c;
                        for (int i = 0; i < _safe_coil_drive_queue.Count; i++)
                        {
                            c = _safe_coil_drive_queue[i];
                            if (c.pulse)
                                Coils[c.coil_name].Pulse(c.pulse_time);
                            if (c.disable)
                                Coils[c.coil_name].Disable();
                        }
                        _safe_coil_drive_queue.Clear();
                    }

                    if (_proc != null)
                    {
                        _proc.watchdog_tickle();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("RUN LOOP EXCEPTION: " + ex.ToString());
            }
            finally
            {
                Logger.Log("Run loop ended");
                if (loops != 0)
                {
                    double dt = Time.GetTime() - _bootTime;
                }
                _proc.Close();

            }
        }

        /// <summary>
        /// All coils/solenoids within the game
        /// </summary>
        public AttrCollection<ushort, string, Driver> Coils
        {
            get { return _coils; }
            set { _coils = value; }
        }

        /// <summary>
        /// All lamps within the game
        /// </summary>
        public AttrCollection<ushort, string, Driver> Lamps
        {
            get { return _lamps; }
            set { _lamps = value; }
        }

        /// <summary>
        /// All GI drivers within the game
        /// </summary>
        public AttrCollection<ushort, string, Driver> GI
        {
            get { return _gi; }
            set { _gi = value; }
        }

        /// <summary>
        /// All switches and optos within the game
        /// </summary>
        public AttrCollection<ushort, string, Switch> Switches
        {
            get { return _switches; }
            set { _switches = value; }
        }

        /// <summary>
        /// The current list of modes that are active in the game
        /// </summary>
        public ModeQueue Modes
        {
            get { return _modes; }
            set { _modes = value; }
        }

        /// <summary>
        /// The list of players currently playing the game
        /// </summary>
        public List<Player> Players
        {
            get { return _players; }
            set { _players = value; }
        }

        /// <summary>
        /// Log the specified text to the given logger. If no logger is set up, log to the trace output
        /// </summary>
        /// <param name="text"></param>
        protected void Log(string text)
        {
            if (this.Logger != null)
                this.Logger.Log(text);
            else
                System.Diagnostics.Trace.WriteLine(text);
        }

        /// <summary>
        /// Safely drive a coil from another thread
        /// </summary>
        /// <param name="coilName">The coil name to drive</param>
        /// <param name="pulse_time">The time (ms) to pulse (default = 30ms)</param>
        public void safe_drive_coil(string coilName, ushort pulse_time = 30)
        {
            SafeCoilDrive d = new SafeCoilDrive();
            d.coil_name = coilName;
            d.pulse = true;
            d.pulse_time = pulse_time;
            lock (_coil_lock_object)
            {
                _safe_coil_drive_queue.Add(d);
            }
        }
        /// <summary>
        /// Safely disable a coil from another thread
        /// </summary>
        /// <param name="coilName">The coil name to disable</param>
        public void safe_disable_coil(string coilName)
        {
            SafeCoilDrive d = new SafeCoilDrive();
            d.coil_name = coilName;
            d.disable = true;
            lock (_coil_lock_object)
            {
                _safe_coil_drive_queue.Add(d);
            }
        }
    }
}
