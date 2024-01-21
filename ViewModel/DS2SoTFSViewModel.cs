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
using System.Windows.Threading;

namespace sldc.ViewModel
{
    class DS2SoTFSViewModel : ViewModelBase
    {
        internal DS2Hook ds2h;
        private DRPClientStore _discordRpcClientStore;
        private Stopwatch stopwatch;
        private DispatcherTimer timer;


        // elapsed time for time counter.
        private TimeSpan elapsedTime;
        public TimeSpan ElapsedTime
        {
            get { return elapsedTime; }
            set {
                elapsedTime = value;
                OnPropertyChanged(nameof(ElapsedTime));
            }
        }
        private int DeathCount;
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
                    // start timer to show elasped time
                    StartRecording();
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
                    // dispose DRP client
                    _discordRpcClientStore.DisposeClient();
                    // stop timer to show elasped time
                    StopRecording();
                }
                OnPropertyChanged(nameof(IsConnectedToGame));
            }
        }
        private void DS2_DeathCountChanged()
        {
            // change death count
            DeathCount = ds2h.Death;
            OnPropertyChanged(nameof(DeathCountText));
            // update drp
            _discordRpcClientStore.UpdatePresence(DRPClientStore.ENVTokens.DS2_TOKEN, DeathCount);
        }

        public ICommand ConnectDS2Command { get; }
        public ICommand OpenStreamerWindowCommand {  get; }
        public DS2SoTFSViewModel(StreamerWindowStore streamerWindowStore, DRPClientStore discordRpcClientStore, HookStore _hookStore)
        {
            _discordRpcClientStore = discordRpcClientStore;
            
            // set up timer for elapsed time
            stopwatch = new Stopwatch();
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(1);



            // set up commands
            ConnectDS2Command = new ConnectDS2Command(this, discordRpcClientStore, _hookStore);
            OpenStreamerWindowCommand = new OpenStreamerWindowComand(_hookStore, streamerWindowStore);

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
        private void StartRecording()
        {
            stopwatch.Start();
            timer.Start();
        }
        private void StopRecording()
        {
            stopwatch.Stop();
            timer.Stop();
            stopwatch.Restart();
            ElapsedTime = TimeSpan.Zero;
            _discordRpcClientStore.ElaspedTime = TimeSpan.Zero;
        }
        private void Timer_Tick(object? sender, EventArgs e)
        {
            //string formattedTime2 = $"{elasped.Hours:D2}:{elasped.Minutes:D2}:{elasped.Seconds:D2}";

            string formattedTime = $"{stopwatch.Elapsed.Hours:D2}:{stopwatch.Elapsed.Minutes:D2}:{stopwatch.Elapsed.Seconds:D2}";
            // Update the ElapsedTime property with the formatted time
            ElapsedTime = TimeSpan.ParseExact(formattedTime, "hh\\:mm\\:ss", null);
            _discordRpcClientStore.ElaspedTime = ElapsedTime;
        }
    }
}
