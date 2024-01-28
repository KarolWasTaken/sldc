using sldc.Commands;
using sldc.Model;
using sldc.Stores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static sldc.Stores.DRPClientStore;
using System.Windows.Input;
using System.Windows.Threading;
using System.Diagnostics.Contracts;
using static sldc.Model.GameDeathDataSerialiser;
using System.Diagnostics.Eventing.Reader;
using System.Windows;

namespace sldc.ViewModel
{
    public class BLViewModel : NonGameHookBaseViewModel
    {
        private BLHook _blHook;
        private Stopwatch stopwatch;
        private DispatcherTimer timer;
        private NonHookGamesDataStore _nonHookGamesDataStore;
        internal DRPClientStore _discordRpcClientStore;
        internal ENVTokens _envToken;
        public HookStore _hookStore;
        public static event Action UpdatePlaythroughList;

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
                    // start scan
                    Task deathUpdaterTask = Task.Run(() => _blHook.AsyncScanIndefinite());
                    
                    // start timer to show elasped time
                    StartRecording();
                    // if drp enabled, create a client
                    if (Helper.ReturnSettings().IsDRPEnabled == true)
                    {
                        _discordRpcClientStore.CreateClient(ENVTokens.BL_TOKEN);
                        //update drp
                        _discordRpcClientStore.UpdatePresence(Playthroughs[SelectedPlaythroughName]);
                    }
                }
                else
                {
                    // reset Hooked label
                    _hookStore.HookedGame = null;
                    _hookStore.HookedPlaythroughName = null;

                    //// stop checking deaths
                    _blHook.IsSearchingForDeaths = false;
                    // reset death counter and covenant
                    DeathCount = 0;
                    OnPropertyChanged(nameof(DeathCountText));
                    // dispose DRP client
                    if (_discordRpcClientStore.Client != null)
                        _discordRpcClientStore.DisposeClient();
                    // stop timer to show elasped time
                    StopRecording();
                }
                OnPropertyChanged(nameof(IsConnectedToGame));
            }
        }

        private string _createPlaythroughtName;
        public string CreatePlaythroughName
        {
            get
            {
                return _createPlaythroughtName;
            }
            set
            {
                _createPlaythroughtName = value;
                OnPropertyChanged(nameof(CreatePlaythroughName));
                OnPropertyChanged(nameof(CanCreatePlaythrough));
            }
        }
        private string _createPlaythroughInitialDeaths;
        public string CreatePlaythroughInitialDeaths
        {
            get
            {
                return _createPlaythroughInitialDeaths;
            }
            set
            {
                string tempHold = value.ToString();
                if (int.TryParse(tempHold, out int intvalue) || (tempHold == "" || tempHold == " "))
                {
                    _createPlaythroughInitialDeaths = value;
                }
                OnPropertyChanged(nameof(CreatePlaythroughInitialDeaths));
                OnPropertyChanged(nameof(CanCreatePlaythrough));
            }
        }

        public Dictionary<string, int> Playthroughs;
        public string? SelectedPlaythroughName { get; set; }
        private bool _isPlaythroughSelected;
        public bool IsChangePlaythroughDialogueOpen { get; set; }
        public bool IsCreatePlaythroughDialogueOpen { get; set; }
        public bool CanCreatePlaythrough =>
            CreatePlaythroughName.Length > 0 &&
            (CreatePlaythroughInitialDeaths != "" || CreatePlaythroughInitialDeaths != " ");
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
        public RelayCommand TogglePlaythroughDialogue { get; private set; }
        public RelayCommand ToggleCreatePlaythroughDialogue { get; private set; }
        public RelayCommand CreatePlaythrough { get; private set; }
        public RelayCommand ResetDeathCount { get; private set; }
        public BLViewModel(StreamerWindowStore streamerWindowStore, DRPClientStore discordRpcClientStore, HookStore hookStore, NonHookGamesDataStore nonHookGamesDataStore, ENVTokens envToken)
        {
            // set up fields
            _discordRpcClientStore = discordRpcClientStore;
            _envToken = envToken;
            _hookStore = hookStore;
            _blHook = new BLHook();
            _nonHookGamesDataStore = nonHookGamesDataStore;
            Playthroughs = _nonHookGamesDataStore.Bloodborne;
            IsPlaythroughSelected = false;
            IsChangePlaythroughDialogueOpen = false;
            IsCreatePlaythroughDialogueOpen = false;
            CreatePlaythroughName = "";
            CreatePlaythroughInitialDeaths = "0";

            // subscribe to on death event
            _blHook.OnDeath += _blHook_OnDeath;

            // set up timer for elapsed time
            stopwatch = new Stopwatch();
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(1);

            // set up commands
            OpenStreamerWindowCommand = new OpenStreamerWindowComand(streamerWindowStore);
            ConnectGameScreenCommand = new ConnectGameScreenCommand(this, _discordRpcClientStore, _blHook);
            TogglePlaythroughDialogue = new RelayCommand(TogglePlaythroughDialogueCommand);
            ToggleCreatePlaythroughDialogue = new RelayCommand(ToggleCreatePlaythroughDialogueCommand);
            CreatePlaythrough = new RelayCommand(CreatePlaythroughCommand);
            ResetDeathCount = new RelayCommand(ResetDeathCountCommand);

        }

        private void CreatePlaythroughCommand(object obj)
        {
            // add the playthrough
            Playthroughs.Add(CreatePlaythroughName, int.Parse(CreatePlaythroughInitialDeaths));
            // select it
            SelectPlaythrough(CreatePlaythroughName);
            // close dialogues
            TogglePlaythroughDialogueCommand();
            ToggleCreatePlaythroughDialogueCommand();
            // save playthroughs to json
            GameDeathDataSerialiser.SaveData(_nonHookGamesDataStore, Playthroughs, NON_HOOKABLE_GAME.BLOODBORNE);
            _nonHookGamesDataStore = GameDeathDataSerialiser.LoadData();
            OnPlaythroughUpdate();
            // update drp
            _discordRpcClientStore.UpdatePresence(DeathCount);
            // reset properties
            CreatePlaythroughName = "";
            CreatePlaythroughInitialDeaths = "0";
        }

        internal void TogglePlaythroughDialogueCommand(object parameter = null)
        {
            IsChangePlaythroughDialogueOpen = !IsChangePlaythroughDialogueOpen;
            OnPropertyChanged(nameof(IsChangePlaythroughDialogueOpen));
        }
        internal void ToggleCreatePlaythroughDialogueCommand(object parameter = null)
        {
            IsCreatePlaythroughDialogueOpen = !IsCreatePlaythroughDialogueOpen;
            OnPropertyChanged(nameof(IsCreatePlaythroughDialogueOpen));
        }
        private void ResetDeathCountCommand(object parameter)
        {
            // reset deaths
            Playthroughs[SelectedPlaythroughName] = 0;
            DeathCount = 0;
            OnPropertyChanged(nameof(DeathCountText));
            // update json
            GameDeathDataSerialiser.SaveData(_nonHookGamesDataStore, Playthroughs, NON_HOOKABLE_GAME.BLOODBORNE);
            _nonHookGamesDataStore = GameDeathDataSerialiser.LoadData();
            OnPlaythroughUpdate();
            // update drp
            _discordRpcClientStore.UpdatePresence(DeathCount);
        }
        private void _blHook_OnDeath()
        {
            // change death count
            DeathCount += 1;
            OnPropertyChanged(nameof(DeathCountText));
            // change deaths in playthrough
            Playthroughs[SelectedPlaythroughName] = DeathCount;
            // update drp
            _discordRpcClientStore.UpdatePresence(DeathCount);
            // update json
            GameDeathDataSerialiser.SaveData(_nonHookGamesDataStore, Playthroughs, NON_HOOKABLE_GAME.BLOODBORNE);
            _nonHookGamesDataStore = GameDeathDataSerialiser.LoadData();

            OnPlaythroughUpdate();
        }
        internal void SelectPlaythrough(string key)
        {
            // disconnect from game if we are changing playthroughs connected
            if(SelectedPlaythroughName != key && SelectedPlaythroughName != null)
                IsConnectedToGame = false;
            IsPlaythroughSelected = true;
            SelectedPlaythroughName = key;
            DeathCount = Playthroughs[key];
            OnPropertyChanged(nameof(SelectedPlaythroughName));
            OnPropertyChanged(nameof(DeathCountText));
        }
        internal void DeletePlaythrough(string key)
        {
            if(SelectedPlaythroughName == key)
            {
                SelectedPlaythroughName = "";
                DeathCount = 0;
                OnPropertyChanged(nameof(SelectedPlaythroughName));
                OnPropertyChanged(nameof(DeathCountText));
                IsPlaythroughSelected = false;
                if(IsConnectedToGame)
                    IsConnectedToGame = false;
            }


            Playthroughs.Remove(key);
            GameDeathDataSerialiser.SaveData(_nonHookGamesDataStore, Playthroughs, NON_HOOKABLE_GAME.BLOODBORNE);
            _nonHookGamesDataStore = GameDeathDataSerialiser.LoadData();
        }
        private void OnPlaythroughUpdate()
        {
            // event to notify code-behind to update the playthroughs list
            UpdatePlaythroughList.Invoke();
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
