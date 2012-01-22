using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XNAPinProc
{
    /// <summary>
    /// Screen Manager
    /// Keeps a list of available screens
    /// so you can switch between them, 
    /// ie. jumping from the start screen to the game screen 
    /// </summary>
    public static class SCREEN_MANAGER
    {
        // Protected Members
        static private List<Screen> _screens = new List<Screen>();
        static private bool _started = false;
        static private Screen _previous = null;
        // Public Members
        static public Screen ActiveScreen = null;

        /// <summary>
        /// Add new Screen
        /// </summary>
        /// <param name="screen">New screen, name must be unique</param>
        static public void add_screen(Screen screen)
        {
            foreach (Screen scr in _screens)
            {
                if (scr.Name == screen.Name)
                {
                    return;
                }
            }
            _screens.Add(screen);
        }

        static public int get_screen_number()
        {
            return _screens.Count;
        }

        static public Screen get_screen(int idx)
        {
            return _screens[idx];
        }

        /// <summary>
        /// Go to screen
        /// </summary>
        /// <param name="name">Screen name</param>
        static public void goto_screen(string name)
        {
            foreach (Screen screen in _screens)
            {
                if (screen.Name == name)
                {
                    // Shutsdown Previous Screen           
                    _previous = ActiveScreen;
                    if (ActiveScreen != null)
                    {
                        ActiveScreen.Shutdown();
                    }
                    // Inits New Screen
                    ActiveScreen = screen;
                    if (_started) ActiveScreen.Init();
                    return;
                }
            }
        }

        /// <summary>
        /// Init Screen manager
        /// Only at this point is screen manager going to init the selected screen
        /// </summary>
        static public void Init()
        {
            _started = true;
            if (ActiveScreen != null)
            {
                ActiveScreen.Init();
            }
        }
        /// <summary>
        /// Falls back to previous selected screen if any
        /// </summary>
        static public void go_back()
        {
            if (_previous != null)
            {
                goto_screen(_previous.Name);
                return;
            }
        }


        /// <summary>
        /// Updates Active Screen
        /// </summary>
        /// <param name="elapsed">GameTime</param>
        static public void Update(GameTime gameTime)
        {
            if (_started == false) return;
            if (ActiveScreen != null)
            {
                ActiveScreen.Update(gameTime);
            }
        }

        static public void Draw(GameTime gameTime)
        {
            if (_started == false) return;
            if (ActiveScreen != null)
            {
                ActiveScreen.Draw(gameTime);
            }
        }
    }
}
