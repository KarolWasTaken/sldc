using sldc.Commands;
using sldc.Model;
using sldc.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace sldc.ViewModel
{
    class DS2SoTFSViewModel : ViewModelBase
    {
        private DS2Hook ds2h = new DS2Hook();





        private int DeathCount;
        private string deathCountText;
        public string DeathCountText
        {
            get
            {
                return $"Deaths: {DeathCount}";
            }
        }
        private bool isConnectedToGame;
        public bool IsConnectedToGame
        {
            get { return isConnectedToGame; }
            set {
                isConnectedToGame = value; 

                if(isConnectedToGame == true)
                {
                    // make connect button say disconnect
                    ConnectToGameButtonSwap(true);

                    // if we conncted to game
                    // subscribe to the deathcountchanged event
                    // which runs when the death count changes
                    ds2h.Start();
                    ds2h.CheckForDeaths = true;
                    // subscribe to death count changed event
                    ds2h.DeathCountChanged += DS2_DeathCountChanged;
                }
                else
                {
                    // maek connect button say connect again
                    ConnectToGameButtonSwap(false);

                    // stop checking deaths
                    ds2h.Stop();
                    ds2h.CheckForDeaths = false;
                    // reset death counter
                    DeathCount = 0;
                    OnPropertyChanged(nameof(DeathCountText));
                }
            }
        }
        private void DS2_DeathCountChanged()
        {
            DeathCount = ds2h.Death;
            OnPropertyChanged(nameof(DeathCountText));
        }

        public ICommand ConnectDS2Command { get; }
        public ICommand OpenStreamerWindowCommand {  get; }
        public DS2SoTFSViewModel(StreamerWindowStore streamerWindowStore)
        {
            ConnectDS2Command = new ConnectDS2Command(this);
            OpenStreamerWindowCommand = new OpenStreamerWindowComand(ds2h, streamerWindowStore);

            // set up "Connect" button
            ConnectToGameButtonSwap(false);
        }

        public string ConnectionButtonText { get; set; }
        private void ConnectToGameButtonSwap(bool connectionStatus)
        {
            if(connectionStatus == true)
                ConnectionButtonText = "Disconnect";
            else
                ConnectionButtonText = "Connect";
            OnPropertyChanged(nameof(ConnectionButtonText));
        }
    }
}
