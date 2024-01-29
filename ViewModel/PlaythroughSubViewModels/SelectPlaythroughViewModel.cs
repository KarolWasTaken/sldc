using sldc.Commands.PlaythroughSubViewCommands;
using sldc.Model;
using sldc.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static sldc.Model.GameDeathDataSerialiser;

namespace sldc.ViewModel.PlaythroughSubViewModels
{
    public class SelectPlaythroughViewModel : ViewModelBase
    {
        private ViewModelBase _currentSubView;
        public ViewModelBase CurrentSubView
        {
            get
            {
                return _currentSubView;
            }
            set
            {
                _currentSubView = value;
                OnPropertyChanged(nameof(CurrentSubView));
            }
        }
        public NON_HOOKABLE_GAME PlaythroughsToShow;
        public NonHookGameViewModelBase NonHookGameViewModelBase;
        private NonHookGamesDataStore _nonHookGamesDataStore;
        public ICommand CreatePlaythroughCommand { get; }
        public RelayCommand CloseDialogue { get; private set; }
        public SelectPlaythroughViewModel(NON_HOOKABLE_GAME playthroughGame,
            ref NonHookGameViewModelBase nonHookGameViewModelBase,
            ref NonHookGamesDataStore nonHookGamesDataStore)
        {
            // set up fields
            PlaythroughsToShow = playthroughGame;
            NonHookGameViewModelBase = nonHookGameViewModelBase;
            _nonHookGamesDataStore = nonHookGamesDataStore;
            
            // set up commands
            CreatePlaythroughCommand = new NonHookGameSubViewNavigateCommand(this);
            CloseDialogue = new RelayCommand(CloseDialogueCommand);
        }

        public void CloseDialogueCommand(object parameter = null)
        {
            NonHookGameViewModelBase.CurrentSubView = null;
        }

        internal void DeletePlaythrough(string? key)
        {
            NonHookGameViewModelBase nhgvm = NonHookGameViewModelBase;
            if (nhgvm.SelectedPlaythroughName == key)
            {
                nhgvm.UpdatePlaythroughName("");
                nhgvm.ResetDeaths();
                nhgvm.IsPlaythroughSelected = false;
                if (nhgvm.IsConnectedToGame)
                    nhgvm.IsConnectedToGame = false;
            }

            nhgvm.Playthroughs.Remove(key);
            nhgvm.SavePlaythroughs(PlaythroughsToShow);
        }
        internal void SelectPlaythrough(string? key)
        {
            NonHookGameViewModelBase nhgvm = NonHookGameViewModelBase;
            if (nhgvm.SelectedPlaythroughName != key && nhgvm.SelectedPlaythroughName != null)
                nhgvm.IsConnectedToGame = false;
            nhgvm.IsPlaythroughSelected = true;
            nhgvm.UpdatePlaythroughName(key);
            nhgvm.UpdateDeathCount(nhgvm.Playthroughs[key]);
            OnPropertyChanged(nameof(nhgvm.SelectedPlaythroughName));
        }
    }
}
