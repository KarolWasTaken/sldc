using DiscordRPC;
using sldc.Model;
using sldc.Stores;
using sldc.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace sldc.Commands
{
    class ConnectGameCommand : CommandBase
    {
        private GameHookBaseViewModel _gameViewModel;
        private DRPClientStore _discordRpcClientStore;
        private HookStore _hookStore;
        public ConnectGameCommand(GameHookBaseViewModel gameViewModel, DRPClientStore discordRpcClientStore, HookStore hookStore)
        {
            _gameViewModel = gameViewModel;
            _discordRpcClientStore = discordRpcClientStore;
            _hookStore = hookStore;
        }

        public override void Execute(object? parameter)
        {
            if(_gameViewModel._hookStore.HookedGame != parameter.ToString() && _gameViewModel._hookStore.HookedGame != null)
            {
                MessageBox.Show("You are currently connected to a game.\nDisconnect from it first.");
                return;
            }
            _gameViewModel._hookStore.HookedGame = parameter.ToString();

            // connect to ds2
            BaseHook hook;
            switch(parameter.ToString())
            {
                case "DS2":
                    hook = new DS2Hook();
                    hook = (DS2Hook)hook;
                    break;
                case "DS3":
                    hook = new DS3Hook();
                    hook = (DS3Hook)hook;
                    break;
                default:
                    throw new NotImplementedException();
            }


            
            hook.Start();
            Thread.Sleep(150);
            // if already connected to game, disconnect
            if(_gameViewModel.IsConnectedToGame == true)
            {
                _gameViewModel.IsConnectedToGame = false;
                hook.Stop();
                return;
            }
            // if sllvl is not 0, we are in game and we can connect.
            if (hook.slLvl != 0)
            {
                _hookStore.hook = hook;
                //_dS2SoTFSViewModel.hook = (DS2Hook)_hookStore.hook;
                DRPClientStore.ENVTokens envToken;
                switch (parameter.ToString())
                {
                    case "DS2":
                        _gameViewModel.hook = new DS2Hook();
                        envToken = DRPClientStore.ENVTokens.DS2_TOKEN;
                        break;
                    case "DS3":
                        _gameViewModel.hook = new DS3Hook();
                        envToken = DRPClientStore.ENVTokens.DS3_TOKEN;
                        break;
                    default:
                        throw new NotImplementedException();
                }
                _gameViewModel.IsConnectedToGame = true;
                if(Helper.ReturnSettings().IsDRPEnabled == true)
                    _discordRpcClientStore.CreateClient(envToken);
            }
            else
            {
                // couldnt connect to game
                MessageBox.Show("couldnt connect to game");
            }
            hook.Stop();
        }
    }
}
