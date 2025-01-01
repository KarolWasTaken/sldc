using sldc.Commands;
using sldc.Model;
using sldc.Stores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using static sldc.App;
using static sldc.Stores.DRPClientStore;

namespace sldc.ViewModel
{
    public abstract class GameHookBaseViewModel : ViewModelBase
    {
        private BaseHook _hook;

        public BaseHook Hook
        {
            get { return _hookStore.hook; }
            set { _hookStore.hook = value; }
        }

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
        private string _covenant;
        public string Covenant
        {
            get
            {
                if (_covenant == null)
                    return "Covenant: N/A";
                else
                    return $"Covenant: {_covenant}";
            }
            set
            {
                _covenant = value;
                OnPropertyChanged(nameof(Covenant));
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
                    // reset deathcount
                    DeathCount = 0;
                    // if we conncted to game
                    // subscribe to the deathcountchanged event
                    // which runs when the death count changes
                    Hook.Start();
                    Hook.CheckForDeaths = true;
                    // subscribe to death count changed event
                    Hook.DeathCountChanged += OnDeathCountChanged;
                    Hook.CovenantChanged += OnCovenantChanged;
                    // start timer to show elasped time
                    StartRecording();
                    OnGameConnected?.Invoke(true);
                }
                else
                {
                    // reset Hooked label
                    _hookStore.HookedGame = null;

                    // stop checking deaths
                    Hook.Stop();
                    Hook.CheckForDeaths = false;
                    // reset covenant
                    Covenant = null;
                    OnPropertyChanged(nameof(DeathCountText));
                    OnPropertyChanged(nameof(Covenant));
                    // dispose DRP client
                    if(_discordRpcClientStore.Client != null)
                        _discordRpcClientStore.DisposeClient();
                    // stop timer to show elasped time
                    StopRecording();
                    OnGameConnected?.Invoke(false);
                }
                OnPropertyChanged(nameof(IsConnectedToGame));
            }
        }

        private void OnCovenantChanged(string covenant)
        {
            // change cov on screen
            Covenant = covenant;
            // update dpr
            if(Helper.settings.EnableCovenantDisplay)
                _discordRpcClientStore.UpdatePresence(covenant);
        }
        private void OnDeathCountChanged(int death)
        {
            // change death count
            DeathCount = death;
            OnPropertyChanged(nameof(DeathCountText));
            // update drp
            _discordRpcClientStore.UpdatePresence(DeathCount);
        }

        public event ConnectedToGameHandler OnGameConnected;
        // pushing error messages
        public event ErrorMessageHandler OnErrorMessage;
        public void SendErrorMessage(string header, string body)
        {
            // raise event
            OnErrorMessage.Invoke(header, body);
        }

        public ICommand ConnectGameCommand { get; }
        public ICommand OpenStreamerWindowCommand { get; }

        public GameHookBaseViewModel(StreamerWindowStore streamerWindowStore, DRPClientStore discordRpcClientStore, HookStore hookStore, ENVTokens envToken)
        {
            // set up fields
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
            ConnectGameCommand = new ConnectGameCommand(this, _discordRpcClientStore);

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
            string formattedTime = $"{stopwatch.Elapsed.Hours:D2}:{stopwatch.Elapsed.Minutes:D2}:{stopwatch.Elapsed.Seconds:D2}";
            // Update the ElapsedTime property with the formatted time
            ElapsedTime = TimeSpan.ParseExact(formattedTime, "hh\\:mm\\:ss", null);
            _discordRpcClientStore.ElaspedTime = ElapsedTime;
        }
    }
}

