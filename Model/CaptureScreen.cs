using sldc.Stores;
using sldc.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace sldc.Model
{
    class CaptureScreen
    {
        private NonHookGameViewModelBase _gameViewModel;
        private DRPClientStore _discordRpcClientStore;
        private ImageSimilarityBase _imageSimilarityHook;
        public CaptureScreen(NonHookGameViewModelBase gameViewModel, DRPClientStore discordRpcClientStore, ref ImageSimilarityBase imageSimilarityHook)
        {
            _gameViewModel = gameViewModel;
            _discordRpcClientStore = discordRpcClientStore;
            _imageSimilarityHook = imageSimilarityHook;
        }

        public void CaptureRemotePlay(string gameToken)
        {
            if (_gameViewModel.hookStore.HookedGame != gameToken && _gameViewModel.hookStore.HookedGame != null)
            {
                MessageBox.Show("You are currently connected to a game.\nDisconnect from it first.");
                return;
            }

            // connect to bl
            switch (gameToken)
            {
                case "BL":
                    _imageSimilarityHook = new BLHook();
                    _imageSimilarityHook = (BLHook)_imageSimilarityHook;
                    break;
                case "DS-PS3":
                    throw new NotImplementedException();
                case "DS-PS5":
                    throw new NotImplementedException();
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
            _gameViewModel.hookStore.HookedGame = gameToken;
            _gameViewModel.hookStore.HookedPlaythroughName = _gameViewModel.SelectedPlaythroughName;
        }

        public void CaptureCaptureCard(string deviceMoniker)
        {
            BLHook bl = new BLHook();
            bl.BeginCapturingCard(deviceMoniker);
        }
    }
}
