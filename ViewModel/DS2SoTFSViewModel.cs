using sldc.Commands;
using sldc.Model;
using sldc.Stores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static sldc.Stores.DRPClientStore;

namespace sldc.ViewModel
{
    class DS2SoTFSViewModel : GameHookBaseViewModel
    {
        public DS2SoTFSViewModel(StreamerWindowStore streamerWindowStore, DRPClientStore discordRpcClientStore, HookStore hookStore, ENVTokens envToken)
            : base(streamerWindowStore, discordRpcClientStore, hookStore, envToken)
        { }
    }
}
