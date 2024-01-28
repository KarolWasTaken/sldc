using DiscordRPC;
using sldc.Commands;
using sldc.Model;
using sldc.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using static sldc.Stores.DRPClientStore;

namespace sldc.ViewModel
{
    class DS3ViewModel : GameHookBaseViewModel
    {
		public DS3ViewModel(StreamerWindowStore streamerWindowStore, DRPClientStore discordRpcClientStore, HookStore hookStore, ENVTokens envToken)
            : base(streamerWindowStore, discordRpcClientStore, hookStore, envToken)
        { }
    }
}
