using sldc.Model;
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
    class ConnectDS2Command : CommandBase
    {
        private DS2SoTFSViewModel _dS2SoTFSViewModel;
        public ConnectDS2Command(ViewModel.DS2SoTFSViewModel dS2SoTFSViewModel)
        {
            _dS2SoTFSViewModel = dS2SoTFSViewModel;
        }

        public override void Execute(object? parameter)
        {
            // connect to ds2
            DS2Hook ds2h = new DS2Hook();
            ds2h.Start();
            Thread.Sleep(150);
            // if already connected to game, disconnect
            if(_dS2SoTFSViewModel.IsConnectedToGame == true)
            {
                _dS2SoTFSViewModel.IsConnectedToGame = false;
                ds2h.Stop();
                return;
            }
            // if sllvl is not 0, we are in game and we can connect.
            if (ds2h.slLvl != 0)
                _dS2SoTFSViewModel.IsConnectedToGame = true;
            else
            {
                // couldnt connect to game
                MessageBox.Show("couldnt connect to game");
            }
            ds2h.Stop();
        }
    }
}
