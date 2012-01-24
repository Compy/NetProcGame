using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using NetProcGame;

namespace XNAPinProc
{
    public class KeyboardController
    {
        public static KeyboardController Instance;
        public Dictionary<Keys, uint> KeySwitchMap { get; set; }

        private KeyboardState lastKeyboardState;

        public KeyboardController(Dictionary<Keys, uint> mappings = null)
        {
            if (mappings == null) KeySwitchMap = new Dictionary<Keys, uint>();
            else KeySwitchMap = mappings;
            Instance = this;

            lastKeyboardState = Keyboard.GetState();
        }

        public Event[] GetKeyboardEvents()
        {
            KeyboardState state = Keyboard.GetState();
            List<Event> events = new List<Event>();
            // Loop through all keys in the switch map
            // If the current entry is currently UP, and the last state showed the entry as DOWN, trigger a switchopen
            // If the current entry is currently DOWN, and the last state showed the entry as UP, trigger a switchclosed
            foreach (Keys key in KeySwitchMap.Keys)
            {
                if (state.IsKeyUp(key) && lastKeyboardState.IsKeyDown(key))
                {
                    // Switchopen event
                    Event e = new Event() { Type = EventType.SwitchOpenDebounced, Value = KeySwitchMap[key] };
                    events.Add(e);
                }
                else if (state.IsKeyDown(key) && lastKeyboardState.IsKeyUp(key))
                {
                    // Switchclosed event
                    Event e = new Event() { Type = EventType.SwitchClosedDebounced, Value = KeySwitchMap[key] };
                    events.Add(e);
                }
            }
            lastKeyboardState = state;

            return events.ToArray();
        }
    }
}
