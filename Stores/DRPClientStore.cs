using DiscordRPC;
using sldc.Converter.CovenantConverters;
using sldc.Exceptions;
using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace sldc.Stores
{
    public class DRPClientStore
    {
        public enum ENVTokens
        {
            DS1_TOKEN,
            DS2_TOKEN,
            DS3_TOKEN,
            ER_TOKEN,
            BL_TOKEN
        }
        public ENVTokens CurrentClientToken;

        public DiscordRpcClient Client;
        private RichPresence Presence;
        public TimeSpan ElaspedTime;
        public void CreateClient(ENVTokens ENVToken)
        {
            string gameName = "null game name";
            // create client
            Client = new DiscordRpcClient(Environment.GetEnvironmentVariable(ENVToken.ToString()));
            Client.Initialize();
            CurrentClientToken = ENVToken;

            // get title for drp
            switch (ENVToken)
            {
                case ENVTokens.DS1_TOKEN:
                    gameName = "DARK SOULS™ REMASTERED";
                    break;
                case ENVTokens.DS2_TOKEN:
                    gameName = "DARK SOULS™ II SoTFS";
                    break;
                case ENVTokens.DS3_TOKEN:
                    gameName = "DARK SOULS™ III";
                    break;
                case ENVTokens.BL_TOKEN:
                    gameName = "Bloodborne™";
                    break;
            }

            // if time elapsed is not 0, set time elasped to the time that has elapsed
            Timestamps timeElasped;
            if (ElaspedTime.TotalMilliseconds > 100)
            {
                //Timestamps startTime = Timestamps.FromTimeSpan(ElaspedTime);
                Timestamps startTime = new Timestamps()
                {
                    Start = DateTime.UtcNow - ElaspedTime,
                    End = null
                };
                timeElasped = startTime;

            }
            else
            {
                timeElasped = Timestamps.Now;
            }


            Assets assets = new Assets()
            {
                LargeImageKey = "large-image",
                LargeImageText = gameName,
            };

            // if we display credit is on, display credit
            if (Helper.ReturnSettings().EnableDRPCredit == true) 
            {
                assets = new Assets()
                {
                    LargeImageKey = "large-image",
                    LargeImageText = gameName,
                    SmallImageKey = "small-image",
                    SmallImageText = "By Karoll :)"
                };
            }
            // if cov display is on, 
            if(Helper.settings.EnableCovenantDisplay == true) 
            {
                // do nothing for now

                //string SmallImageKey = CovenantNameToSmallImageKeyConverter.Convert(CovenantName);
                //assets = new Assets()
                //{
                //    LargeImageKey = "large-image",
                //    LargeImageText = gameName,
                //    SmallImageKey = SmallImageKey,
                //    SmallImageText = CovenantName
                //};
            }
            


            Presence = new RichPresence()
            {
                Details = $"Death #0",
                Timestamps = timeElasped,
                Assets = assets
            };
            Client.SetPresence(Presence);
        }

        public void DisposeClient()
        {
            Client.Dispose();
            Client = null;
            Presence = null;
        }

        public void UpdatePresence(int deathCount)
        {
            if (Client == null)
            {
                return;
            }

            
            if (Presence != null)
            {
                Presence.Details = $"Death #{deathCount}";
                //Presence = new RichPresence()
                //{
                //    Details = $"Death #{deathCount}",
                //    Timestamps = Presence.Timestamps, // Use the same timestamp
                //    Assets = Presence.Assets
                //};
            }
            Client.SetPresence(Presence);
        }
        public void UpdatePresence(string covenantName)
        {
            if (Client == null)
            {
                return;
                //throw new Exception("Discord client is null");
            }

            // grab cov image key (file name on developer portal)
            string smallImageKey = CovenantNameToSmallImageKeyConverter.Convert(covenantName);

            // create new assets for drp
            Assets assets = new Assets()
            {
                LargeImageKey = Presence.Assets.LargeImageKey,
                LargeImageText = Presence.Assets.LargeImageText,
                SmallImageKey = smallImageKey,
                SmallImageText = covenantName
            };
            // set the assets
            Presence.Assets = assets;
            // set the presence
            Client.SetPresence(Presence);
        }
    }
}
