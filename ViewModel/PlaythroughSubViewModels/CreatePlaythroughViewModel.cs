using sldc.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static sldc.Model.GameDeathDataSerialiser;

namespace sldc.ViewModel.PlaythroughSubViewModels
{
    public class CreatePlaythroughViewModel : ViewModelBase
    {
        private SelectPlaythroughViewModel _selectPlaythroughViewModel;
        private NonHookGameViewModelBase _nonHookGameViewModel;
        private NON_HOOKABLE_GAME _game;

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

        public bool CanCreatePlaythrough =>
            CreatePlaythroughName.Length > 0 &&
            (CreatePlaythroughInitialDeaths != "" || CreatePlaythroughInitialDeaths != " ");
        public RelayCommand CloseDialogue { get; private set; }
        public RelayCommand CreatePlaythrough { get; private set; }
        public CreatePlaythroughViewModel(ref SelectPlaythroughViewModel selectPlaythroughViewModel, ref NonHookGameViewModelBase nonHookGameViewModel, NON_HOOKABLE_GAME game)
        {
            // set up fields
            _selectPlaythroughViewModel = selectPlaythroughViewModel;
            _nonHookGameViewModel = nonHookGameViewModel;
            _game = game;
            CreatePlaythroughName = "";
            CreatePlaythroughInitialDeaths = "0";

            CloseDialogue = new RelayCommand(CloseDialogueCommand);
            CreatePlaythrough = new RelayCommand(CreatePlaythroughCommand);
        }


        private void CreatePlaythroughCommand(object obj)
        {
            NonHookGameViewModelBase nhgvm = _nonHookGameViewModel;

            // add the playthrough
            nhgvm.Playthroughs.Add(CreatePlaythroughName, int.Parse(CreatePlaythroughInitialDeaths));
            // select it
            _selectPlaythroughViewModel.SelectPlaythrough(CreatePlaythroughName);
            // save playthroughs to json
            nhgvm.SavePlaythroughs(_game);
            // update drp
            nhgvm.UpdateDeathCountInDRP();
            // close dialogues
            _nonHookGameViewModel.CurrentSubView = null;
            _selectPlaythroughViewModel.CurrentSubView = null;
            // reset properties
            CreatePlaythroughName = "";
            CreatePlaythroughInitialDeaths = "0";
        }





        public void CloseDialogueCommand(object parameter = null)
        {
            _selectPlaythroughViewModel.CurrentSubView = null;
        }
    }
}
