using sldc.Commands;
using sldc.Model;
using sldc.Stores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static sldc.Stores.DRPClientStore;
using System.Windows.Input;
using System.Windows.Threading;
using System.Diagnostics.Contracts;
using static sldc.Model.GameDeathDataSerialiser;
using System.Diagnostics.Eventing.Reader;
using System.Windows;

namespace sldc.ViewModel
{
    public class BLViewModel : NonHookGameViewModelBase
    {
        public BLViewModel(StreamerWindowStore streamerWindowStore,
            DRPClientStore discordRpcClientStore,
            HookStore hookStore,
            NonHookGamesDataStore nonHookGamesDataStore,
            ENVTokens envToken)
            : base(streamerWindowStore,
                  discordRpcClientStore,
                  hookStore,
                  nonHookGamesDataStore,
                  envToken,
                  new BLHook(),
                  LoadData(NON_HOOKABLE_GAME.BLOODBORNE),
                  NON_HOOKABLE_GAME.BLOODBORNE)
        { }
    }
}
