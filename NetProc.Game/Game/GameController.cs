using NetProc;
using NetProc.Interface;
using NetProc.Pdb;
using NetProc.Tools;
using NetProc.Game.Lamps;
using NetProc.Game.Modes;
using System;
using System.Collections.Generic;

namespace NetProc.Game
{
    /// <summary>
    /// Core object representing the game itself.
    /// Usually a game developer will create a new game by subclassing this class.
    /// Consider subclassing 'BasicGame' instead, as it makes use of several helpful modes and controllers. See Test game
    /// </summary>
    public class GameController : IGameController
    {
        public byte[] testFrame;
        /// <summary>
        /// Thread synchronization object for coils
        /// </summary>
        protected object _coil_lock_object = new object();

        /// <summary>
        /// TODO: implement coil driver object list
        /// </summary>
        protected AttrCollection<ushort, string, IDriver> _coils;

        /// <summary>
        /// The configuration object loaded by load_config()
        /// TODO: implement config handling
        /// </summary>
        protected MachineConfiguration _config;

        /// <summary>
        /// Run loop exit condition. This continues until true
        /// </summary>
        protected bool _done = false;

        /// <summary>
        /// Are the flippers and bumpers currently Enabled?
        /// </summary>
        protected bool _flippers_enabled = false;

        /// <summary>
        /// Contains information specific to the particular location installation (high scores, audits, etc).
        /// </summary>
        protected object _game_data;

        /// <summary>
        /// List of GI drivers
        /// </summary>
        protected AttrCollection<ushort, string, IDriver> _gi;

        /// <summary>
        /// TODO: implement lamp driver object list
        /// </summary>
        protected AttrCollection<ushort, string, IDriver> _lamps;

        protected AttrCollection<ushort, string, LED> _leds;
        /// <summary>
        /// Machine type used to configure the proc device
        /// </summary>
        protected MachineType _machineType;

        /// <summary>
        /// TODO: Implement mode queue logic
        /// </summary>
        protected IModeQueue _modes;

        /// <summary>
        /// The total number of balls in the machine
        /// </summary>
        protected int _num_balls_total = 5;

        /// <summary>
        /// A collection of old player objects if reset is called.
        /// </summary>
        protected List<IPlayer> _old_players;

        /// <summary>
        /// A collection of player objects
        /// </summary>
        protected List<IPlayer> _players;

        /// <summary>
        /// A pinproc class instance, created in the constructor with the Machine_Type attribute
        /// </summary>
        protected IProcDevice _proc;

        /// <summary>
        /// List of coils to drive that is manipulated from outside/UI threads
        /// TODO: This is set to be removed in favor of the UI process communication model
        /// </summary>
        protected List<SafeCoilDrive> _safe_coil_drive_queue = new List<SafeCoilDrive>();

        /// <summary>
        /// TODO: implement switch object lists
        /// </summary>
        protected AttrCollection<ushort, string, Switch> _switches;

        /// <summary>
        /// Contains local game information such as volume
        /// </summary>
        protected object _user_settings;

        /// <summary>
        /// The number of the current ball. A value of 1 represents the first ball; 0 indicates game over.
        /// </summary>
        protected int Ball = 0;

        /// <summary>
        /// The ending time of the current ball
        /// </summary>
        protected double BallEndTime;

        /// <summary>
        /// The number of balls per game
        /// </summary>
        protected int BallsPerGame = 3;

        /// <summary>
        /// The starting time of the current ball
        /// </summary>
        protected double BallStartTime;

        /// <summary>
        /// The date/time when the framework was started (machine powered up)
        /// </summary>
        protected double BootTime = 0;

        /// <summary>
        /// Creates a new GameController object with the given machine type and logging infrastructure
        /// </summary>
        /// <param name="machineType">Machine type to use (WPC, STERN, PDB etc)</param>
        /// <param name="logger">The logger interface to use</param>
        public GameController(MachineType machineType, ILogger logger, bool Simulated = false)
        {
            this.Logger = logger;
            this._machineType = machineType;
            if (Simulated)
            {
                this._proc = new FakePinProc(machineType);
                this._proc.Logger = logger;
                this._proc.Reset(1);
            }
            else
            {
                this._proc = new ProcDevice(machineType, Logger);
                this._proc.Reset(1);
            }
            this._modes = new ModeQueue(this);
            this.BootTime = Time.GetTime();
            this._coils = new AttrCollection<ushort, string, IDriver>();
            this._switches = new AttrCollection<ushort, string, Switch>();
            this._lamps = new AttrCollection<ushort, string, IDriver>();
            this._leds = new AttrCollection<ushort, string, LED>();
            this._gi = new AttrCollection<ushort, string, IDriver>();
            this._old_players = new List<IPlayer>();
            this._players = new List<IPlayer>();

            this.LampController = new LampController(this);

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
        /// The current ball the game is on
        /// </summary>
        public int ball { get; set; }

        /// <summary>
        /// All coils/solenoids within the game
        /// </summary>
        public AttrCollection<ushort, string, IDriver> Coils
        {
            get { return _coils; }
            set { _coils = value; }
        }

        /// <summary>
        /// Current machine configuration representation
        /// </summary>
        public MachineConfiguration Config
        {
            get { return _config; }
        }

        public int CurrentPlayerIndex { get; private set; }

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
                    LinkFlipperSwitch("flipperLwR", _config.PRFlipperRight.ToArray());
                    LinkFlipperSwitch("flipperLwL", _config.PRFlipperLeft.ToArray());
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
                else if (_machineType == MachineType.SternWhitestar || _machineType == MachineType.SternSAM || _machineType == MachineType.PDB)
                {
                    foreach (string flipper in _config.PRFlippers)
                    {
                        IDriver main_coil = _coils[flipper + "Main"];
                        ushort switch_num = PinProc.PRDecode(_machineType, _switches[flipper].Number.ToString());
                        DriverState[] drivers = new DriverState[1];
                        if (_flippers_enabled)
                            drivers[0] = _proc.DriverStatePatter(main_coil.State, 2, 18, 34);
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
                            drivers[0] = _proc.DriverStateDisable(main_coil.State);
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
                    IDriver coil = _coils[bumper];

                    DriverState[] drivers = new DriverState[1];
                    if (_flippers_enabled)
                        drivers[0] = _proc.DriverStatePulse(coil.State, 20);
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
        /// All GI drivers within the game
        /// </summary>
        public AttrCollection<ushort, string, IDriver> GI
        {
            get { return _gi; }
            set { _gi = value; }
        }

        public ILampController LampController { get; set; }

        /// <summary>
        /// All lamps within the game
        /// </summary>
        public AttrCollection<ushort, string, IDriver> Lamps
        {
            get { return _lamps; }
            set { _lamps = value; }
        }

        /// <summary>
        /// All leds within the game
        /// </summary>
        public AttrCollection<ushort, string, LED> LEDS
        {
            get { return _leds; }
            set { _leds = value; }
        }

        /// <summary>
        /// Public logging interface class. Make sure all games have a class that implements this interface
        /// </summary>
        public ILogger Logger { get; set; }
        /// <summary>
        /// The current list of modes that are active in the game
        /// </summary>
        public IModeQueue Modes
        {
            get { return _modes; }
            set { _modes = value; }
        }

        /// <summary>
        /// The list of players currently playing the game
        /// </summary>
        public List<IPlayer> Players
        {
            get { return _players; }
            set { _players = value; }
        }

        /// <summary>
        /// PROC device driver wrapper
        /// </summary>
        public IProcDevice PROC
        {
            get { return _proc; }
        }

        /// <summary>
        /// All switches and optos within the game
        /// </summary>
        public AttrCollection<ushort, string, NetProc.Switch> Switches
        {
            get { return _switches; }
            set { _switches = value; }
        }

        /// <summary>
        /// Adds a new player to 'Players' and auto-assigns a name
        /// </summary>
        /// <returns></returns>
        public virtual IPlayer AddPlayer()
        {
            IPlayer newPlayer = this.CreatePlayer("Player " + (_players.Count + 1).ToString());
            _players.Add(newPlayer);
            return newPlayer;
        }

        /// <summary>
        /// Called by the game framework when the current ball has ended
        /// </summary>
        public virtual void BallEnded()
        {

        }

        /// <summary>
        /// Called by the game framework when a new ball is starting
        /// </summary>
        public virtual void BallStarting()
        {
            this.SaveBallStartTime();
        }

        /// <summary>
        /// Creates a new player with a given name
        /// </summary>
        /// <param name="name">The name for the player to use, usually auto generated</param>
        /// <returns>A new player object</returns>
        public IPlayer create_player(string name)
        {
            return new Player(name);
        }

        /// <summary>
        /// Returns the current 'Player' object according to the current_player_index value
        /// </summary>
        /// <returns></returns>
        public IPlayer CurrentPlayer()
        {
            if (this._players.Count > this.CurrentPlayerIndex)
                return this._players[this.CurrentPlayerIndex];
            else
                return null;
        }

        /// <summary>
        /// Called by the GameController when a DMD event has been received.
        /// </summary>
        public virtual void DmdEvent()
        {
        }

        /// <summary>
        /// Called by the implementor to notify the game that the current ball has ended
        /// </summary>
        public void EndBall()
        {
            this.BallEndTime = Time.GetTime();
            this.CurrentPlayer().GameTime += this.GetBallTime();
            this.BallEnded();

            if (this.CurrentPlayer().ExtraBalls > 0)
            {
                this.CurrentPlayer().ExtraBalls -= 1;
                this.ShootAgain();
                return;
            }

            if (this.CurrentPlayerIndex + 1 == this._players.Count)
            {
                this.ball += 1;
                this.CurrentPlayerIndex = 0;
            }
            else
            {
                this.CurrentPlayerIndex += 1;
            }

            if (this.ball > this.BallsPerGame)
            {
                this.EndGame();
            }
            else
            {
                this.StartBall();
            }

        }

        /// <summary>
        /// Called by the implementor to notify the game that the game as ended
        /// </summary>
        public void EndGame()
        {
            this.GameEnded();
            this.ball = 0;
        }

        /// <summary>
        /// Set the exit condition for the run loop causing the game to terminate
        /// </summary>
        public void EndRunLoop()
        {
            _done = true;
        }

        /// <summary>
        /// Called by the GameController when the current game has ended
        /// </summary>
        public virtual void GameEnded()
        {
        }

        /// <summary>
        /// Called by the GameController when a new game is starting.
        /// </summary>
        public virtual void GameStarted()
        {
            this.ball = 1;
            this._players = new List<IPlayer>();
            this.CurrentPlayerIndex = 0;
        }

        /// <summary>
        /// The ball time for the current player
        /// </summary>
        /// <returns>The ball time (in seconds) that the current ball has been in play</returns>
        public double GetBallTime()
        {
            return this.BallEndTime - this.BallStartTime;
        }

        /// <summary>
        /// Retrieve all events from the PROC interface board
        /// </summary>
        /// <returns>A list of events from the PROC</returns>
        public virtual Event[] GetEvents()
        {
            return _proc.Getevents();
        }

        /// <summary>
        /// The game time for the given player index
        /// </summary>
        /// <param name="player">The player index to calculate the game time for</param>
        /// <returns>The time in seconds the player has been playing the entire game</returns>
        public double GetGameTime(int player)
        {
            return this._players[player].GameTime;
        }

        public void LinkFlipperSwitch(string switch_name, string[] linked_coils)
        {
            // Hold all of the linked coils * 2 (one for Main, one for Hold)
            DriverState[] drivers = new DriverState[linked_coils.Length * 2];
            IDriver main_coil, hold_coil;
            ushort switch_num = _switches[switch_name].Number;
            int driverIdx = 0;
            if (_flippers_enabled)
            {
                foreach (string coil in linked_coils)
                {
                    main_coil = _coils[coil + "Main"];
                    hold_coil = _coils[coil + "Hold"];
                    drivers[driverIdx] = _proc.DriverStatePulse(main_coil.State, 34);
                    driverIdx++;
                    drivers[driverIdx] = _proc.DriverStatePulse(hold_coil.State, 0);
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
                    drivers[driverIdx] = _proc.DriverStateDisable(main_coil.State);
                    driverIdx++;
                    drivers[driverIdx] = _proc.DriverStateDisable(hold_coil.State);
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
        /// Create a new machine configuration representation in memory from a file on disk. Uses <see cref="SetupProcMachine"/>
        /// </summary>
        /// <param name="PathToFile">The path to the configuration json file</param>
        public void LoadConfig(string PathToFile)
        {
            var config = MachineConfiguration.FromFile(PathToFile);
            LoadConfig(config);
        }

        /// <summary>
        /// Sets up the the machine from a MachineConfiguration. Uses <see cref="SetupProcMachine"/>
        /// </summary>
        /// <param name="PathToFile">The path to the configuration json file</param>
        public void LoadConfig(MachineConfiguration config)
        {
            _config = config;
            _num_balls_total = config.PRGame.NumBalls;

            //setup machine items
            if((PROC as ProcDevice) != null) PROC.SetupProcMachine(config, _coils, _switches, _lamps, _leds, _gi);
        }

        /// <summary>
        /// Process the retrieved event from the PROC interface board (switch/dmd events)
        /// </summary>
        /// <param name="evt">The event to process</param>
        public void ProcessEvent(Event evt)
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
                this.DmdEvent();
            }
            else
            {                                
                Switch sw = _switches[(ushort)evt.Value];
                bool recvd_state = evt.Type == EventType.SwitchClosedDebounced;

                Console.WriteLine($"{sw.Name}-{sw.Number} {evt.ToString()}");
                Log(evt.ToString());

                if (!sw.IsState(recvd_state))
                {
                    sw.SetState(recvd_state);
                    _modes.handle_event(evt);
                }
            }
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
            CurrentPlayerIndex = 0;
            _modes.Clear();
        }

        /// <summary>
        /// Main run loop of the program. Performs the following logic until the loop ends:
        ///     - Get events from PROC
        ///     - Process this list of events across all modes
        ///     - 'Tick' modes
        ///     - Tickle watchdog
        /// </summary>
        public void RunLoop()
        {
            long loops = 0;
            _done = false;
            DmdEvent();
            Event[] events;
            //try
            //{
            while (!_done)
            {
                loops++;
                events = GetEvents();
                if (events != null)
                {
                    foreach (Event evt in events)
                    {
                        ProcessEvent(evt);
                    }
                }

                this.Tick();
                TickVirtualDrivers();
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
                    _proc.WatchDogTickle();
                }
            }
            //}
            //catch (Exception ex)
            //{
            //    Logger.Log("RUN LOOP EXCEPTION: " + ex.ToString());
            //}
            //finally
            //{
            Logger?.Log("Run loop ended");
            if (loops != 0)
            {
                double dt = Time.GetTime() - BootTime;
            }
            _proc.Close();

            //}
        }

        /// <summary>
        /// Safely disable a coil from another thread
        /// </summary>
        /// <param name="coilName">The coil name to disable</param>
        public void SafeDisableCoil(string coilName)
        {
            SafeCoilDrive d = new SafeCoilDrive();
            d.coil_name = coilName;
            d.disable = true;
            lock (_coil_lock_object)
            {
                _safe_coil_drive_queue.Add(d);
            }
        }

        /// <summary>
        /// Safely drive a coil from another thread
        /// </summary>
        /// <param name="coilName">The coil name to drive</param>
        /// <param name="pulse_time">The time (ms) to pulse (default = 30ms)</param>
        public void SafeDriveCoil(string coilName, ushort pulse_time = 30)
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
        /// Save the ball start time into local memory
        /// </summary>
        public void SaveBallStartTime()
        {
            this.BallStartTime = Time.GetTime();
        }

        /// <summary>
        /// Called by the game framework when a new ball is starting which was the result of a stored extra ball.
        /// The default implementation calls ball_starting() which is not called by the framework in this case.
        /// </summary>
        public virtual void ShootAgain()
        {
            this.BallStarting();
        }

        /// <summary>
        /// Called by the implementor to notify the game that the first ball should be started.
        /// </summary>
        public void StartBall()
        {
            this.BallStarting();
        }

        /// <summary>
        /// Called by the implementor to notify the game that the game has started.
        /// </summary>
        public virtual void StartGame()
        {
            this.GameStarted();
        }

        public virtual void Tick()
        {
        }

        /// <summary>
        /// Propagate Tick events to all lamps and coils
        /// </summary>
        public void TickVirtualDrivers()
        {
            foreach (Driver coil in _coils.Values)
            {
                coil.Tick();
            }

            foreach (Driver lamp in _lamps.Values)
            {
                lamp.Tick();
            }

            foreach (LED led in _leds.Values)
            {
                led.Tick();
            }
        }

        public void UpdateLamps()
        {
            //for (int i = modes.length - 1; i >= 0; i--)
            //{
            //   modes[i].update_lamps();
            //}
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
    }
}
