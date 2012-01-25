//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Audio;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Media;

//namespace XNAPinProc.Screens
//{
//    public class LoadingScreen : Screen
//    {
//        private SpriteFont font;
//        private SpriteBatch spriteBatch;
//        private string OSVersionString;
//        public LoadingScreen(GraphicsDevice device)
//            : base(device, "LoadingScreen")
//        {
//            spriteBatch = new SpriteBatch(device);
//            OperatingSystem os = Environment.OSVersion;
//            OSVersionString = os.Version.ToString();
//        }

//        public override bool Init()
//        {
//            font = XNAPinProcGame.instance.Content.Load<SpriteFont>(@"Fonts\Arial");
//            return base.Init();
//        }

//        public override void Draw(GameTime gameTime)
//        {
//            _device.Clear(Color.Black);
//            spriteBatch.Begin(SpriteSortMode.Immediate,
//                null, null, null, null, null, XNAPinProcGame.instance.camera.TransformMatrix);

//            spriteBatch.DrawString(font,
//                "LOADING...",
//                new Vector2(XNAPinProcGame.ScreenWidth / 2 - (font.MeasureString("LOADING...").X / 2)
//                    , XNAPinProcGame.ScreenHeight / 2 - (font.MeasureString("LOADING...").Y / 2)),
//                Color.Red);

//            spriteBatch.DrawString(font,
//                OSVersionString,
//                new Vector2(300, 100),
//                Color.Red);

//            spriteBatch.End();
//            base.Draw(gameTime);
//        }
//    }
//}
