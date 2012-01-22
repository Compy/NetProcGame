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

using System.Net.NetworkInformation;

using XNAPinProc.Menus;

namespace XNAPinProc.Screens
{
    public class SystemInfoScreen : SettingsScreenBase
    {
        private SpriteFont infoFont;
        private string OSVersion, PCSVersion, NetworkIP, NetworkGW, NetworkDNS, NetworkSubnet;
        public SystemInfoScreen(GraphicsDevice device)
            : base(device, "SystemInfo")
        {
            Title = "System Information";
        }

        public override bool Init()
        {
            infoFont = XNAPinProcGame.instance.Content.Load<SpriteFont>(@"Fonts\Arial");
            RefreshInfo();
            return base.Init();
        }

        public void RefreshInfo()
        {
            OperatingSystem os = Environment.OSVersion;
            OSVersion = os.VersionString;
            PCSVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            if (adapters.Length < 1) return;
            foreach (NetworkInterface adapter in adapters)
            {
                if (adapter.OperationalStatus == OperationalStatus.Up && 
                    (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet || 
                    adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    NetworkDNS = properties.DnsAddresses[0].ToString();
                    NetworkIP = properties.UnicastAddresses[0].IPv4Mask.ToString();
                    NetworkGW = properties.GatewayAddresses[0].Address.ToString();
                    NetworkSubnet = properties.UnicastAddresses[0].IPv4Mask.ToString();

                    if (NetworkIP == "0.0.0.0" || NetworkSubnet == "0.0.0.0")
                        continue;

                    break;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _device.Clear(Color.White);
            spriteBatch.Begin(SpriteSortMode.Immediate,
                null, null, null, null, null, XNAPinProcGame.instance.camera.TransformMatrix);

            spriteBatch.DrawString(infoFont, "OS: " + OSVersion, new Vector2(20, 100), Color.Black);
            spriteBatch.DrawString(infoFont, "PCS Version: " + PCSVersion, new Vector2(20, 132), Color.Black);
            spriteBatch.DrawString(infoFont, "IP Address: " + NetworkIP, new Vector2(20, 196), Color.Black);
            spriteBatch.DrawString(infoFont, "Subnet: " + NetworkSubnet, new Vector2(20, 228), Color.Black);
            spriteBatch.DrawString(infoFont, "Gateway: " + NetworkGW, new Vector2(20, 260), Color.Black);
            spriteBatch.DrawString(infoFont, "DNS: " + NetworkDNS, new Vector2(20, 292), Color.Black);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
