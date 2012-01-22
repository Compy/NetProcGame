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

namespace XNAPinProc
{
    public class HeaderDisplay
    {
        // Ball         Player/Score        Credits
        /*
        private Vector2 ballTitlePos = new Vector2(10, 0);
        private Vector2 scoreTitlePos = new Vector2(200, 0);
        private Vector2 creditTitlePos = new Vector2(400, 0);
        private Vector2 ballPos = new Vector2(10, 20);
        private Vector2 scorePos = new Vector2(200, 20);
        private Vector2 creditPos = new Vector2(400, 20);
        */

        private Vector2 ballTitlePos = new Vector2(10, 0);
        private Vector2 scoreTitlePos = new Vector2(200, 0);
        private Vector2 creditTitlePos = new Vector2(400, 0);
        private Vector2 ballPos = new Vector2(10, 20);
        private Vector2 scorePos = new Vector2(200, 20);
        private Vector2 creditPos = new Vector2(400, 20);

        private Vector2 ballOrigin = new Vector2(0, 0);
        private Vector2 scoreOrigin = new Vector2(0, 0);
        private Vector2 creditOrigin = new Vector2(0, 0);

        private float drawScale = 1;
        private SpriteEffects drawEffects = SpriteEffects.None;

        private float alphaValue = 1;
        private float fadeIncrement = 0.02f;
        private double fadeDelay = 0.035;

        private SpriteFont font, scoreFont;

        public HeaderDisplay()
        {
            font = XNAPinProcGame.instance.Content.Load<SpriteFont>(@"Fonts\Arial");
            scoreFont = XNAPinProcGame.instance.Content.Load<SpriteFont>(@"Fonts\ArialBig");

            scoreTitlePos = new Vector2((XNAPinProcGame.ScreenWidth / 2) - font.MeasureString("Player 1").X / 2, 0);
            creditTitlePos = new Vector2(XNAPinProcGame.ScreenWidth - font.MeasureString("Credits").X - 10, 0);
            scorePos = new Vector2((XNAPinProcGame.ScreenWidth / 2) - font.MeasureString("Player 1").X / 2, 20);

            if (XNAPinProcGame.instance.FlipScreen)
            {
                drawEffects = SpriteEffects.FlipHorizontally;
            }
        }

        public void Update(GameTime gameTime)
        {
            // Decrement the delay by the number of seconds that have elapsed since the last time
            // the update method was called
            fadeDelay -= gameTime.ElapsedGameTime.TotalSeconds;

            // If the fade delay has dropped below zero, then its time to fade in/fade out the image
            if (fadeDelay <= 0)
            {
                // Reset the fade delay
                fadeDelay += 0.035;

                // Increment / Decrement the fade value for the image
                alphaValue += fadeIncrement;

                // If the alpha value is equal to or above the max alpha value or has dropped below
                // or equal to the min alpha value then reverse the fade
                if (alphaValue >= 1)
                    fadeIncrement = -0.04f;
                if (alphaValue <= 0.4)
                    fadeIncrement = 0.04f;

                if (XNAPinProcGame.instance.FlipScreen)
                {
                    drawEffects = SpriteEffects.FlipHorizontally;
                }
                else
                {
                    drawEffects = SpriteEffects.None;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 testOrigin = new Vector2(50, 50);

            spriteBatch.DrawString(font, 
                "Ball", 
                ballTitlePos,
                Color.White * alphaValue,
                0, new Vector2(0, 0), 1, drawEffects, 0);
            spriteBatch.DrawString(font,
                "1",
                ballPos,
                Color.Red * alphaValue,
                0, new Vector2(0, 0), 1, drawEffects, 0);

            spriteBatch.DrawString(font,
                "Player 1",
                scoreTitlePos,
                Color.White * alphaValue,
                0, new Vector2(0, 0), 1, drawEffects, 0);
            spriteBatch.DrawString(scoreFont,
                "2,032,523",
                scorePos,
                Color.Aqua * alphaValue,
                0, new Vector2(0, 0), 1, drawEffects, 0);

            spriteBatch.DrawString(font,
                "Credits",
                creditTitlePos,
                Color.White * alphaValue,
                0, new Vector2(0, 0), 1, drawEffects, 0);
        }
    }
}
