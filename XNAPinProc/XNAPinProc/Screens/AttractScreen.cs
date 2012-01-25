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
//    public class AttractScreen : Screen
//    {
//        private SpriteFont font;
//        private SpriteBatch spriteBatch;
//        private HeaderDisplay header;

//        public AttractScreen(GraphicsDevice device)
//            : base(device, "AttractScreen")
//        {
//            spriteBatch = new SpriteBatch(device);
//        }

//        public override bool Init()
//        {
//            font = XNAPinProcGame.instance.Content.Load<SpriteFont>(@"Fonts\Arial");

//            header = new HeaderDisplay();

//            return base.Init();
//        }

//        public override void Update(GameTime gameTime)
//        {
//            header.Update(gameTime);
//            base.Update(gameTime);
//        }

//        public override void Draw(GameTime gameTime)
//        {
//            _device.Clear(Color.Black);
//            spriteBatch.Begin(SpriteSortMode.Immediate,
//                null, null, null, null, null, XNAPinProcGame.instance.camera.TransformMatrix);

//            header.Draw(spriteBatch);

//            spriteBatch.End();
//            base.Draw(gameTime);
//        }
//    }
//}
