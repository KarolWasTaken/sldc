using DiscordRPC;
using sldc.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sldc.Stores
{
    public class DRPClientStore
    {
        public enum ENVTokens
        {
            DS1_TOKEN,
            DS2_TOKEN,
            DS3_TOKEN
        }
        public ENVTokens CurrentClientToken;

        public DiscordRpcClient Client;
        private RichPresence Presence;
        public TimeSpan ElaspedTime;

        public void CreateClient(ENVTokens ENVToken)
        {
            string gameName = "null game name";

            Client = new DiscordRpcClient(Environment.GetEnvironmentVariable(ENVToken.ToString()));
            Client.Initialize();
            CurrentClientToken = ENVToken;
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
            }
            Timestamps timeElasped;
            if (ElaspedTime.TotalMilliseconds > 100)
            {
                Timestamps startTime = Timestamps.FromTimeSpan(ElaspedTime);
                Timestamps test = new Timestamps()
                {
                    Start = DateTime.UtcNow - ElaspedTime,
                    End = null
                };
                timeElasped = test;

            }
            else
            {
                timeElasped = Timestamps.Now;
            }

            Assets assets;
            if(Helper.ReturnSettings().EnableDRPCredit == true) 
            {
                assets = new Assets()
                {
                    LargeImageKey = "large-image",
                    LargeImageText = gameName,
                    SmallImageKey = "small-image",
                    SmallImageText = "By Karoll :)"
                };
            }
            else 
            {
                assets = new Assets()
                {
                    LargeImageKey = "large-image",
                    LargeImageText = gameName,
                };
            }

            Presence = new RichPresence()
            {
                Details = $"Death #0",
                Timestamps = timeElasped,
                Assets = assets
            };
            Client.SetPresence(Presence);
        }
        public void ChangeGameInstance()
        {

        }

        public void DisposeClient()
        {
            Client.Dispose();
            Client = null;
            Presence = null;
        }

        public void UpdatePresence(ENVTokens ENVToken, int deathCount)
        {
            if (ENVToken != CurrentClientToken)
            {
                throw new UnmatchedDRPTokenException("Incomming token doesnt match token of established client. Dispose old client first.");
            }
            if (Client == null)
            {
                throw new Exception("Discord client is null");
            }

            Presence = new RichPresence()
            {
                Details = $"Death #{deathCount}",
                Timestamps = Presence.Timestamps, // Use the same timestamp
                Assets = Presence.Assets
            };
            Client.SetPresence(Presence);
        }
    }
}
