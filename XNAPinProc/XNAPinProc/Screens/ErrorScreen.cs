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
//    public class ErrorScreen : Screen
//    {
//        private SpriteBatch spriteBatch;
//        private SpriteFont errorTitleFont;
//        private SpriteFont errorTextFont;

//        private int borderWidth = 5;
//        private Texture2D borderTexture;

//        public string Title { get; set; }
//        public string ErrorText { get; set; }

//        public ErrorScreen(GraphicsDevice device)
//            : base(device, "ErrorScreen")
//        {
//            spriteBatch = new SpriteBatch(device);
//            borderTexture = new Texture2D(device, 1, 1);
//            borderTexture.SetData<Color>(new Color[] { Color.White });
//            errorTitleFont = XNAPinProcGame.instance.Content.Load<SpriteFont>(@"Fonts\ArialBig");
//            errorTextFont = XNAPinProcGame.instance.Content.Load<SpriteFont>(@"Fonts\Arial");
//        }

//        public override void Draw(GameTime gameTime)
//        {
//            _device.Clear(Color.Black);
//            spriteBatch.Begin(SpriteSortMode.Immediate,
//                null, null, null, null, null, XNAPinProcGame.instance.camera.TransformMatrix);

//            // Draw border
//            spriteBatch.Draw(borderTexture,
//                new Rectangle(0, 0, XNAPinProcGame.ScreenWidth, borderWidth),
//                Color.Red);
//            spriteBatch.Draw(borderTexture,
//                new Rectangle(XNAPinProcGame.ScreenWidth - borderWidth, 0, borderWidth, XNAPinProcGame.ScreenHeight),
//                Color.Red);
//            spriteBatch.Draw(borderTexture,
//                new Rectangle(0, XNAPinProcGame.ScreenHeight - borderWidth, XNAPinProcGame.ScreenWidth, borderWidth),
//                Color.Red);
//            spriteBatch.Draw(borderTexture,
//                new Rectangle(0, 0, borderWidth, XNAPinProcGame.ScreenHeight),
//                Color.Red);

//            spriteBatch.DrawString(errorTitleFont, Title, new Vector2((XNAPinProcGame.ScreenWidth / 2) - (errorTitleFont.MeasureString(Title).X / 2), 50), Color.Red);
//            string formattedText = WrapText(errorTextFont, ErrorText, XNAPinProcGame.ScreenWidth - 100);

//            spriteBatch.DrawString(errorTextFont, formattedText, new Vector2(10, 110), Color.White);

//            spriteBatch.End();
//            base.Draw(gameTime);
//        }

//        public string WrapText(SpriteFont spriteFont, string text, float maxLineWidth)
//        {
//            string[] words = text.Split(' ');

//            StringBuilder sb = new StringBuilder();

//            float lineWidth = 0f;

//            float spaceWidth = spriteFont.MeasureString(" ").X;

//            foreach (string word in words)
//            {
//                Vector2 size = spriteFont.MeasureString(word);

//                if (lineWidth + size.X < maxLineWidth)
//                {
//                    sb.Append(word + " ");
//                    lineWidth += size.X + spaceWidth;
//                }
//                else
//                {
//                    sb.Append("\n" + word + " ");
//                    lineWidth = size.X + spaceWidth;
//                }
//            }

//            return sb.ToString();
//        }
//    }
//}
