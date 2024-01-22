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
using System.Windows.Threading;
using static sldc.Stores.DRPClientStore;

namespace sldc.ViewModel
{
    public abstract class GameHookBaseViewModel : ViewModelBase
    {
        internal BaseHook hook { get; set; }
        internal HookStore _hookStore;
        internal DRPClientStore _discordRpcClientStore;
        private Stopwatch stopwatch;
        private DispatcherTimer timer;
        internal ENVTokens _envToken;
        internal event Action ConnectedToGame;
        internal event Action DisconnectedToGame;

        // elapsed time for time counter.
        private TimeSpan elapsedTime;
        public TimeSpan ElapsedTime
        {
            get { return elapsedTime; }
            set
            {
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
            set
            {
                isConnectedToGame = value;

                if (isConnectedToGame == true)
                {
                    // make connect button say disconnect
                    ConnectToGameButtonSwap(true);

                    // if we conncted to game
                    // subscribe to the deathcountchanged event
                    // which runs when the death count changes
                    hook.Start();
                    hook.CheckForDeaths = true;
                    // subscribe to death count changed event
                    hook.DeathCountChanged += OnDeathCountChanged;
                    // start timer to show elasped time
                    StartRecording();
                    // for children to perform special connect logic
                    ConnectedToGame.Invoke();
                }
                else
                {
                    // maek connect button say connect again
                    ConnectToGameButtonSwap(false);

                    // reset hooked label
                    _hookStore.HookedGame = null;

                    // stop checking deaths
                    hook.Stop();
                    hook.CheckForDeaths = false;
                    // reset death counter
                    DeathCount = 0;
                    OnPropertyChanged(nameof(DeathCountText));
                    // dispose DRP client
                    _discordRpcClientStore.DisposeClient();
                    // stop timer to show elasped time
                    StopRecording();
                    //// for children to perform special disconnect logic
                    //DisconnectedToGame.Invoke();
                }
                OnPropertyChanged(nameof(IsConnectedToGame));
            }
        }
        private void OnDeathCountChanged()
        {
            // change death count
            DeathCount = hook.Death;
            OnPropertyChanged(nameof(DeathCountText));
            // update drp
            _discordRpcClientStore.UpdatePresence(_envToken, DeathCount);
        }

        public ICommand ConnectGameCommand { get; }
        public ICommand OpenStreamerWindowCommand { get; }

        public GameHookBaseViewModel(StreamerWindowStore streamerWindowStore, DRPClientStore discordRpcClientStore, HookStore hookStore, ENVTokens envToken)
        {
            _discordRpcClientStore = discordRpcClientStore;
            _hookStore = hookStore;
            _envToken = envToken;

            // set up timer for elapsed time
            stopwatch = new Stopwatch();
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(1);

            // set up commands
            OpenStreamerWindowCommand = new OpenStreamerWindowComand(hookStore, streamerWindowStore);
            ConnectGameCommand = new ConnectGameCommand(this, _discordRpcClientStore, _hookStore);

            // set up "Connect" button
            ConnectToGameButtonSwap(false);
        }
        
        public string ConnectionButtonText { get; set; }
        private void ConnectToGameButtonSwap(bool connectionStatus)
        {
            if (connectionStatus == true)
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

