using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using System.ComponentModel;

using XNAPinProc.Screens;
using XNAPinProc.Middleware;
using NetProcGame.tools;

namespace XNAPinProc
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class XNAPinProcGame : Microsoft.Xna.Framework.Game, ILogger
    {
        public static XNAPinProcGame instance;
        public static int ScreenWidth = 800;
        public static int ScreenHeight = 600;
        private double lastTimeChecked = 0;
        public static MiddlewareGame middlewareGame;
        private BackgroundWorker middlewareThread;
        private KeyboardState oldKeyboardState;

        private ErrorScreen errorScreen;

        private bool flipScreen = false;
        public bool FlipScreen
        {
            get { return flipScreen;  }
            set
            {
                if (value == true)
                {
                    camera.Rotation = MathHelper.ToRadians(180);
                    camera.Position = new Vector2(800, 600);
                }
                else
                {
                    camera.Rotation = 0;
                    camera.Position = new Vector2(0, 0);
                }
                flipScreen = value;
            }
        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        HeaderDisplay headerDisplay;
        public Camera camera;

        public XNAPinProcGame()
        {
            XNAPinProcGame.instance = this;
            graphics = new GraphicsDeviceManager(this);

            OperatingSystem os = Environment.OSVersion;

            graphics.IsFullScreen = (os.Version.Major == 5 && os.Version.Minor == 1);
            graphics.PreferredBackBufferWidth = ScreenWidth;
            graphics.PreferredBackBufferHeight = ScreenHeight;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.Exiting += new EventHandler<EventArgs>(XNAPinProcGame_Exiting);

            // TODO: Add your initialization logic here
            headerDisplay = new HeaderDisplay();
            camera = new Camera();
            camera.ScreenFlipped = flipScreen;
            errorScreen = new ErrorScreen(GraphicsDevice);
            SCREEN_MANAGER.add_screen(new LoadingScreen(GraphicsDevice));
            SCREEN_MANAGER.add_screen(new SettingsMenu(GraphicsDevice));
            SCREEN_MANAGER.add_screen(new AttractScreen(GraphicsDevice));
            SCREEN_MANAGER.add_screen(new SystemInfoScreen(GraphicsDevice));
            SCREEN_MANAGER.add_screen(errorScreen);
            SCREEN_MANAGER.goto_screen("LoadingScreen");

            oldKeyboardState = Keyboard.GetState();

            // Initialize the middleware thread to communicate with game hardware
            middlewareThread = new BackgroundWorker();
            middlewareThread.WorkerSupportsCancellation = true;
            middlewareThread.DoWork += new DoWorkEventHandler(middlewareThread_DoWork);
            middlewareThread.RunWorkerAsync();

            base.Initialize();
        }

        void middlewareThread_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                System.Threading.Thread.CurrentThread.Name = "p-roc thread";
                middlewareGame = new MiddlewareGame(this);
                middlewareGame.setup();
                middlewareGame.run_loop();
            }
            catch (Exception ex)
            {
                errorScreen.Title = "Device Communication Error";
                //errorScreen.ErrorText = "Communication with the P-ROC has been lost. Please make sure the device is plugged into the USB port and is powered on.\n\nPress 'Q' to exit PCS into Windows";
                errorScreen.ErrorText = ex.ToString();
                System.Threading.Thread.Sleep(1000);
                SCREEN_MANAGER.goto_screen("ErrorScreen");
            }
        }

        private void ProcessKeyboard()
        {
            KeyboardState kbState = Keyboard.GetState();
            if (kbState.IsKeyDown(Keys.F) && oldKeyboardState.IsKeyUp(Keys.F))
            {
                FlipScreen = !FlipScreen;
                camera.ScreenFlipped = FlipScreen;
            }
            else if (kbState.IsKeyDown(Keys.D) && oldKeyboardState.IsKeyUp(Keys.D))
            {
                SCREEN_MANAGER.goto_screen("SettingsMenu");
            }
            else if (kbState.IsKeyDown(Keys.B) && oldKeyboardState.IsKeyUp(Keys.B))
            {
                SCREEN_MANAGER.go_back();
            }
            else if (kbState.IsKeyDown(Keys.A) && oldKeyboardState.IsKeyUp(Keys.A))
                SCREEN_MANAGER.goto_screen("AttractScreen");
            else if (kbState.IsKeyDown(Keys.Q) && oldKeyboardState.IsKeyUp(Keys.Q))
            {
                System.Diagnostics.Process.Start(@"C:\Windows\explorer.exe");
                this.Exit();
            }

            oldKeyboardState = kbState;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            SCREEN_MANAGER.Init();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            SCREEN_MANAGER.Update(gameTime);

            ProcessKeyboard();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            

            SCREEN_MANAGER.Draw(gameTime);

            
            base.Draw(gameTime);
        }

        public void Log(string text)
        {
            System.Diagnostics.Trace.WriteLine(text);
        }

        private void XNAPinProcGame_Exiting(object sender, EventArgs e)
        {
            if (middlewareGame != null)
                middlewareGame.end_run_loop();
        }
    }
}
