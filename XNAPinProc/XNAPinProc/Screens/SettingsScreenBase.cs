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

using XNAPinProc.Menus;

namespace XNAPinProc.Screens
{
    public class SettingsScreenBase : Screen
    {
        protected SpriteFont titleFont, font;
        protected SpriteBatch spriteBatch;
        protected Texture2D pixel, wmsLogo;
        protected double timeSinceLastUpdate = 0;
        protected string dateTimeString = "";

        public string Title { get; set; }

        public SettingsScreenBase(GraphicsDevice device, string ScreenName)
            : base(device, ScreenName)
        {
            spriteBatch = new SpriteBatch(device);
        }

        public override bool Init()
        {
            font = XNAPinProcGame.instance.Content.Load<SpriteFont>(@"Fonts\Arial");
            titleFont = XNAPinProcGame.instance.Content.Load<SpriteFont>(@"Fonts\DiagnosticTitle");
            pixel = new Texture2D(_device, 1, 1, true, SurfaceFormat.Color);
            pixel.SetData<Color>(new Color[] { Color.White });
            wmsLogo = XNAPinProcGame.instance.Content.Load<Texture2D>(@"Images\williamslogo");
            return base.Init();
        }

        public override void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.TotalMilliseconds - timeSinceLastUpdate >= 500)
            {
                dateTimeString = DateTime.Now.ToString("dddd, dd MMMM yyyy, h:mm:ss tt");
                timeSinceLastUpdate = gameTime.TotalGameTime.TotalMilliseconds;
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate,
                null, null, null, null, null, XNAPinProcGame.instance.camera.TransformMatrix);

            spriteBatch.Draw(pixel, new Rectangle(0, 0, XNAPinProcGame.ScreenWidth, 72), new Color(0, 51, 204));
            spriteBatch.DrawString(titleFont,
                Title,
                new Vector2(96, 0),
                Color.White);

            spriteBatch.DrawString(font,
                dateTimeString,
                new Vector2(96, titleFont.MeasureString("S").Y + 2),
                Color.White);

            spriteBatch.Draw(wmsLogo, new Vector2(10, 5), Color.White);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
