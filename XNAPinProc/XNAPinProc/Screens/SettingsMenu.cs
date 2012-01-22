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
    public class SettingsMenu : SettingsScreenBase
    {
        private MenuList homeMenu;

        public SettingsMenu(GraphicsDevice device)
            : base(device, "SettingsMenu")
        {
            Title = "System Menu";
        }

        public override bool Init()
        {
            homeMenu = new MenuList(_device);

            homeMenu.Position = new Vector2(20, 100);
            homeMenu.AddItem("Diagnostics");
            homeMenu.AddItem("Adjustments");
            homeMenu.AddItem("Bookkeeping");
            homeMenu.AddItem("Utilities");
            homeMenu.AddItem("Printouts");
            homeMenu.AddItem("System Information", new MenuItemSelectedHandler(delegate(string objName)
                {
                    SCREEN_MANAGER.goto_screen("SystemInfo");
                }));
            return base.Init();
        }

        public override void Update(GameTime gameTime)
        {
            homeMenu.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _device.Clear(Color.White);
            spriteBatch.Begin(SpriteSortMode.Immediate,
                null, null, null, null, null, XNAPinProcGame.instance.camera.TransformMatrix);

            homeMenu.Draw(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
