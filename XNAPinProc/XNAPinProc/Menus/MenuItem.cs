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

namespace XNAPinProc.Menus
{
    public delegate void MenuItemSelectedHandler(string ItemName);
    public class MenuItem
    {
        public Vector2 Position { get; set; }
        public string Text { get; set; }
        public SpriteFont Font { get; set; }
        public bool Selected { get; set; }
        public event MenuItemSelectedHandler OnMenuItemSelected;
        public float Width
        {
            get
            {
                return Font.MeasureString(Text).X;
            }
        }
        public float Height
        {
            get
            {
                return Font.MeasureString(Text).Y;
            }
        }

        private Texture2D pixel;
        private GraphicsDevice device;
        public MenuItem(GraphicsDevice device, MenuItemSelectedHandler callback = null)
        {
            this.device = device;
            pixel = new Texture2D(device, 1, 1, true, SurfaceFormat.Color);
            pixel.SetData<Color>(new Color[] { Color.White });

            if (callback != null)
                OnMenuItemSelected += callback;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Selected)
            {
                spriteBatch.Draw(pixel, new Rectangle((int)Position.X - 2, (int)Position.Y - 2,
                                                (int)Font.MeasureString(Text).X + 4,
                                                (int)Font.MeasureString(Text).Y + 4), Color.Black);
                spriteBatch.DrawString(Font, Text, Position, Color.White);
            }
            else
            {
                spriteBatch.DrawString(Font, Text, Position, Color.Black);
            }
        }

        public void ItemSelected()
        {
            if (OnMenuItemSelected != null)
                OnMenuItemSelected(Text);
        }
    }
}
