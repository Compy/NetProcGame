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
    public class MenuList
    {
        public Vector2 Position { get; set; }

        private Texture2D pixel;
        private GraphicsDevice device;
        private KeyboardState oldKeyboardState;
        private List<MenuItem> items;
        private int currentIdx = 0;

        private SpriteFont menuFont;

        public MenuList(GraphicsDevice device)
        {
            this.device = device;
            items = new List<MenuItem>();
            menuFont = XNAPinProcGame.instance.Content.Load<SpriteFont>(@"Fonts\DiagnosticTitle");
            oldKeyboardState = Keyboard.GetState();
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Down) && oldKeyboardState.IsKeyUp(Keys.Down))
            {
                NextItem();
            }
            else if (state.IsKeyDown(Keys.Up) && oldKeyboardState.IsKeyUp(Keys.Up))
                PrevItem();
            else if (state.IsKeyDown(Keys.Enter) && oldKeyboardState.IsKeyUp(Keys.Enter))
                ItemSelected();


            oldKeyboardState = state;
        }

        public void AddItem(string text, MenuItemSelectedHandler callback = null)
        {
            // Add item to the end, but calculate new position
            MenuItem i = new MenuItem(device, callback);
            i.Font = menuFont;
            i.Text = text;
            i.Position = new Vector2(Position.X, Position.Y + (i.Height * items.Count));
            i.Selected = (items.Count == 0);
            items.Add(i);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < items.Count; i++)
                items[i].Draw(spriteBatch);
        }

        private void NextItem()
        {
            if (items.Count == 0) return;
            items[currentIdx].Selected = false;
            if (currentIdx == items.Count - 1) currentIdx = 0;
            else currentIdx++;
            items[currentIdx].Selected = true;
        }
        private void PrevItem()
        {
            if (items.Count == 0) return;
            items[currentIdx].Selected = false;
            if (currentIdx == 0) currentIdx = items.Count - 1;
            else currentIdx--;
            items[currentIdx].Selected = true;
        }
        private void ItemSelected()
        {
            if (items.Count == 0) return;
            items[currentIdx].ItemSelected();
        }
    }
}
