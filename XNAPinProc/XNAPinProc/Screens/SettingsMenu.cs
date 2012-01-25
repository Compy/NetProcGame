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
//    public class SettingsMenu : SettingsScreenBase
//    {

//        public SettingsMenu(GraphicsDevice device)
//            : base(device, "SettingsMenu")
//        {
//            Title = "System Menu";
//        }

//        public override bool Init()
//        {
//            return base.Init();
//        }

//        public override void Update(GameTime gameTime)
//        {
//            base.Update(gameTime);
//        }

//        public override void Draw(GameTime gameTime)
//        {
//            _device.Clear(Color.White);
//            spriteBatch.Begin(SpriteSortMode.Immediate,
//                null, null, null, null, null, XNAPinProcGame.instance.camera.TransformMatrix);

//            spriteBatch.End();
//            base.Draw(gameTime);
//        }
//    }
//}
