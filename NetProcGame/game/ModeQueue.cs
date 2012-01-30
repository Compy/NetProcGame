using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetProcGame.game
{
    public class ModeQueue
    {
        protected GameController _game;
        protected List<Mode> _modes;
        public ModeQueue(GameController game)
        {
            _game = game;
            _modes = new List<Mode>();
        }

        public void Add(Mode mode)
        {
            if (_modes.Contains(mode))
                throw new Exception("Attempted to add mode " + mode.ToString() + ", already in mode queue.");

            _modes.Add(mode);
            //self.modes.sort(lambda x, y: y.priority - x.priority)
            mode.mode_started();

            if (mode == _modes[0])
                mode.mode_topmost();
        }

        public void Remove(Mode mode)
        {
            mode.mode_stopped();
            _modes.Remove(mode);

            if (_modes.Count > 0)
                _modes[0].mode_topmost();
        }

        public void handle_event(Event evt)
        {
            Mode[] modes = new Mode[_modes.Count()];
            _modes.CopyTo(modes);
            for (int i = 0; i < modes.Length; i++)
            {
                bool handled = modes[i].handle_event(evt);
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
            //Mode[] modes = new Mode[_modes.Count()];
            //_modes.CopyTo(modes);
            for (int i = 0; i < _modes.Count; i++)
            {
                _modes[i].dispatch_delayed();
                _modes[i].mode_tick();
            }
        }

        public List<Mode> Modes
        {
            get { return _modes; }
        }
    }
}
