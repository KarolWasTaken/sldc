using sldc.Model;
using sldc.Stores;
using sldc.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace sldc.Commands
{
    public class ConnectGameScreenCommand : CommandBase
    {
        private BLViewModel _gameViewModel;
        private DRPClientStore _discordRpcClientStore;
        private BLHook _imageSimilarityHook;
        public ConnectGameScreenCommand(BLViewModel gameViewModel, DRPClientStore discordRpcClientStore, BLHook imageSimilarityHook)
        {
            _gameViewModel = gameViewModel;
            _discordRpcClientStore = discordRpcClientStore;
            _imageSimilarityHook = imageSimilarityHook;
        }
        public override void Execute(object? parameter)
        {
            if (_gameViewModel._hookStore.HookedGame != parameter.ToString() && _gameViewModel._hookStore.HookedGame != null)
            {
                MessageBox.Show("You are currently connected to a game.\nDisconnect from it first.");
                return;
            }

            // connect to bl
            switch (parameter.ToString())
            {
                case "BL":
                    _imageSimilarityHook = (BLHook)_imageSimilarityHook;
                    break;
                default:
                    throw new NotImplementedException();
            }
            bool DoesProcessExist = _imageSimilarityHook.DoesProcessExist("RemotePlay");
            if (!DoesProcessExist)
            {
                MessageBox.Show("Cannot find RemotePlay.exe");
                return;
            }
            
            // if already connected to game, disconnect
            if (_gameViewModel.IsConnectedToGame == true)
            {
                _gameViewModel.IsConnectedToGame = false;
                _imageSimilarityHook.IsSearchingForDeaths = false;
                return;
            }
            
            _gameViewModel.IsConnectedToGame = true;
            _gameViewModel._hookStore.HookedGame = parameter.ToString();
            _gameViewModel._hookStore.HookedPlaythroughName = _gameViewModel.SelectedPlaythroughName;
        }
    }
}
