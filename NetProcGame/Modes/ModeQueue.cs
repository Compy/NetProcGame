using NetProcGame.Game;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetProcGame.Modes
{
    public class ModeQueue
    {
        protected IGameController _game;
        protected List<IMode> _modes;
        protected object _mode_lock_obj = new object();
        public ModeQueue(IGameController game)
        {
            _game = game;
            _modes = new List<IMode>();
        }

        public void Add(IMode mode)
        {
            if (_modes.Contains(mode))
                throw new Exception("Attempted to add mode " + mode.ToString() + ", already in mode queue.");

            lock (_mode_lock_obj)
            {
                _modes.Add(mode);
            }
            //self.modes.sort(lambda x, y: y.priority - x.priority)
            _modes.Sort();
            mode.ModeStarted();

            if (mode == _modes[0])
                mode.ModeTopMost();
        }

        public void Remove(IMode mode)
        {
            mode.ModeStopped();
            lock (_mode_lock_obj)
            {
                _modes.Remove(mode);
            }

            if (_modes.Count > 0)
                _modes[0].ModeTopMost();
        }

        public void handle_event(Event evt)
        {
            IMode[] modes = new IMode[_modes.Count()];
            _modes.CopyTo(modes);
            for (int i = 0; i < modes.Length; i++)
            {
                bool handled = modes[i].HandleEvent(evt);
                if (handled)
                    break;
            }
        }

        public void Clear()
        {
            _modes.Clear();
        }

        public void tick()
        {
            IMode[] modes;
            lock (_mode_lock_obj)
            {
                modes = new IMode[_modes.Count()];
                _modes.CopyTo(modes);
            }
            for (int i = 0; i < modes.Length; i++)
            {
                modes[i].DispatchDelayed();
                modes[i].ModeTick();
            }
        }

        public List<IMode> Modes
        {
            get { return _modes; }
        }
    }
}
