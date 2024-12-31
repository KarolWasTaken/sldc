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
        public ConnectGameCommand(GameHookBaseViewModel gameViewModel, DRPClientStore discordRpcClientStore)
        {
            _gameViewModel = gameViewModel;
            _discordRpcClientStore = discordRpcClientStore;
        }

        public override void Execute(object? parameter)
        {
            if(_gameViewModel._hookStore.HookedGame != parameter.ToString() && _gameViewModel._hookStore.HookedGame != null)
            {
                //MessageBox.Show("You are currently connected to a game.\nDisconnect from it first.");
                _gameViewModel.SendErrorMessage("ERROR", "You are currently connected to a \ngame. Disconnect from it and\ntry again.");
                return;
            }

            // connect to ds2
            BaseHook hook;
            switch(parameter.ToString())
            {
                case "DSR":
                    hook = new DSRHook();
                    hook = (DSRHook)hook;
                    break;
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
            //// if already connected to game, disconnect
            if (_gameViewModel.IsConnectedToGame == true)
            {
                _gameViewModel.IsConnectedToGame = false;
                hook.Stop();
                return;
            }
            // if sllvl is not 0, we are in game and we can connect.
            if (hook.slLvl != 0)
            {
                DRPClientStore.ENVTokens envToken;
                switch (parameter.ToString())
                {
                    case "DSR":
                        _gameViewModel.Hook = new DSRHook();
                        envToken = DRPClientStore.ENVTokens.DS1_TOKEN;
                        break;
                    case "DS2":
                        _gameViewModel.Hook = new DS2Hook();
                        envToken = DRPClientStore.ENVTokens.DS2_TOKEN;
                        break;
                    case "DS3":
                        _gameViewModel.Hook = new DS3Hook();
                        envToken = DRPClientStore.ENVTokens.DS3_TOKEN;
                        break;
                    default:
                        throw new NotImplementedException();
                }
                _gameViewModel.IsConnectedToGame = true;
                _gameViewModel._hookStore.HookedGame = parameter.ToString();
                if (Helper.ReturnSettings().IsDRPEnabled == true)
                    _discordRpcClientStore.CreateClient(envToken);
            }
            else
            {
                // couldnt connect to game
                _gameViewModel.SendErrorMessage("ERROR", "Error connecting to game!\nPlease connect while in-game.");
            }
            hook.Stop();
        }
    }
}
