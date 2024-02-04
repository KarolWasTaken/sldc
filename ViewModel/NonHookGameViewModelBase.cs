using sldc.Stores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static sldc.Stores.DRPClientStore;
using System.Windows.Threading;
using sldc.Model;
using System.Windows.Input;
using sldc.Commands;
using static sldc.Model.GameDeathDataSerialiser;
using sldc.Commands.PlaythroughSubViewCommands;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Media;
using System.Reflection.Metadata;

namespace sldc.ViewModel
{
    public class NonHookGameViewModelBase : ViewModelBase
    {
        public ImageSimilarityBase ImageSimilarityNotifier;
        private Stopwatch stopwatch;
        private DispatcherTimer timer;
        internal NonHookGamesDataStore NonHookGamesDataStore;
        internal NON_HOOKABLE_GAME Game;
        internal DRPClientStore DiscordRpcClientStore;
        internal ENVTokens _envToken;
        public HookStore hookStore;

        private ViewModelBase currentSubView;
        public ViewModelBase CurrentSubView
        {
            get { return currentSubView; }
            set
            {
                if (currentSubView != value)
                {
                    currentSubView = value;
                    OnPropertyChanged(nameof(CurrentSubView));
                }
            }
        }
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
                    // start scan
                    Task deathUpdaterTask = Task.Run(() => ImageSimilarityNotifier.AsyncScanIndefinite(150, 3000));
                    ImageSimilarityNotifier.OnDeath += OnDeath;
                    // start timer to show elasped time
                    StartRecording();
                    // if drp enabled, create a client
                    if (Helper.ReturnSettings().IsDRPEnabled == true)
                    {
                        DiscordRpcClientStore.CreateClient(ENVTokens.BL_TOKEN);
                        //update drp
                        DiscordRpcClientStore.UpdatePresence(Playthroughs[SelectedPlaythroughName]);
                    }
                }
                else
                {
                    // reset Hooked label
                    hookStore.HookedGame = null;
                    hookStore.HookedPlaythroughName = null;

                    //// stop checking deaths
                    ImageSimilarityNotifier.IsSearchingForDeaths = false;
                    // reset death counter and covenant
                    DeathCount = 0;
                    OnPropertyChanged(nameof(DeathCountText));
                    // reset playthrough name text
                    SelectedPlaythroughName = "";
                    OnPropertyChanged(nameof(SelectedPlaythroughName));
                    // dispose DRP client
                    if (DiscordRpcClientStore.Client != null)
                        DiscordRpcClientStore.DisposeClient();
                    // stop timer to show elasped time
                    StopRecording();
                }
                OnPropertyChanged(nameof(IsConnectedToGame));
            }
        }

        public Dictionary<string, int> Playthroughs;
        public string? SelectedPlaythroughName { get; set; }
        private bool _isPlaythroughSelected;
        public bool IsPlaythroughSelected
        {
            get
            {
                return _isPlaythroughSelected;
            }
            set
            {
                _isPlaythroughSelected = value;
                OnPropertyChanged(nameof(IsPlaythroughSelected));
            }
        }

        public ICommand ConnectGameScreenCommand { get; }
        public ICommand OpenStreamerWindowCommand { get; }
        public ICommand SelectPlaythroughCommand { get; }
        public RelayCommand ResetDeathCount { get; private set; }
        public RelayCommand SelectCaptureDevice { get; private set; }

        public NonHookGameViewModelBase(StreamerWindowStore streamerWindowStore,
            DRPClientStore discordRpcClientStore,
            HookStore hookStore,
            NonHookGamesDataStore nonHookGamesDataStore,
            ENVTokens envToken,
            ImageSimilarityBase imageSimilarity,
            Dictionary<string, int> gamePlaythrough,
            NON_HOOKABLE_GAME game)
        {
            // force imageSimilarity to be implemented
            ImageSimilarityNotifier = imageSimilarity ?? throw new ArgumentNullException(nameof(imageSimilarity));
            Playthroughs = gamePlaythrough ?? throw new ArgumentNullException(nameof(gamePlaythrough));
            Game = game;

            // set up fields
            DiscordRpcClientStore = discordRpcClientStore;
            _envToken = envToken;
            this.NonHookGamesDataStore = nonHookGamesDataStore;
            this.hookStore = hookStore;
            IsPlaythroughSelected = false;


            // set up timer for elapsed time
            stopwatch = new Stopwatch();
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(1);

            // set up commands
            OpenStreamerWindowCommand = new OpenStreamerWindowComand(streamerWindowStore);
            ConnectGameScreenCommand = new ConnectGameScreenCommand(this, DiscordRpcClientStore, ref ImageSimilarityNotifier);
            SelectPlaythroughCommand = new NonHookGameSubViewNavigateCommand(this);
            ResetDeathCount = new RelayCommand(ResetDeathCountCommand);
            SelectCaptureDevice = new RelayCommand(SelectCaptureDeviceCommand);

        }
        private void SelectCaptureDeviceCommand(object paramters)
        {   if(!isConnectedToGame)
                CurrentSubView = new SelectCaptureDeviceViewModel(this);
            else
            {
                IsConnectedToGame = false;
                ImageSimilarityNotifier.IsSearchingForDeaths = false;
            }
        }
        private void ResetDeathCountCommand(object parameter)
        {
            // reset deaths
            Playthroughs[SelectedPlaythroughName] = 0;
            ResetDeaths();
            // update json
            SavePlaythroughs(Game);
            //OnPlaythroughUpdate();
            // update drp
            DiscordRpcClientStore.UpdatePresence(DeathCount);
        }

















        //private void OnPlaythroughUpdate()
        //{
        //    // event to notify code-behind to update the playthroughs list
        //    UpdatePlaythroughList.Invoke();
        //}

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
            DiscordRpcClientStore.ElaspedTime = TimeSpan.Zero;
        }
        private void Timer_Tick(object? sender, EventArgs e)
        {
            string formattedTime = $"{stopwatch.Elapsed.Hours:D2}:{stopwatch.Elapsed.Minutes:D2}:{stopwatch.Elapsed.Seconds:D2}";
            // Update the ElapsedTime property with the formatted time
            ElapsedTime = TimeSpan.ParseExact(formattedTime, "hh\\:mm\\:ss", null);
            DiscordRpcClientStore.ElaspedTime = ElapsedTime;
        }
        public void ResetDeaths()
        {
            DeathCount = 0;
            OnPropertyChanged(nameof(DeathCountText));
        }
        public void UpdateDeathCount(int deaths)
        {
            DeathCount = deaths;
            OnPropertyChanged(nameof(DeathCountText));
        }
        public void UpdatePlaythroughName(string name)
        {
            SelectedPlaythroughName = name;
            OnPropertyChanged(nameof(SelectedPlaythroughName));
        }
        public void SavePlaythroughs(NON_HOOKABLE_GAME game)
        {
            if (Game != game)
                throw new Exception("games dont match");
            SaveData(NonHookGamesDataStore, Playthroughs, game);
            NonHookGamesDataStore = LoadData();
        }
        public void UpdateDeathCountInDRP()
        {
            DiscordRpcClientStore.UpdatePresence(DeathCount);
        }
        private void OnDeath()
        {
            UpdateDeathCount(DeathCount + 1);
            UpdateDeathCountInDRP();
            Playthroughs[SelectedPlaythroughName] = DeathCount;
            SavePlaythroughs(Game);
        }
    }
}
