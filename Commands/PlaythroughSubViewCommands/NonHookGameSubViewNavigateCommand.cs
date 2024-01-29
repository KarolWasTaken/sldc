using sldc.Model;
using sldc.ViewModel;
using sldc.ViewModel.PlaythroughSubViewModels;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static sldc.Model.GameDeathDataSerialiser;

namespace sldc.Commands.PlaythroughSubViewCommands
{
    public class NonHookGameSubViewNavigateCommand : CommandBase
    {

        private NonHookGameViewModelBase _nonHookGameViewModelBase;
        public NonHookGameSubViewNavigateCommand(NonHookGameViewModelBase nonHookGameViewModelBase)
        {
            _nonHookGameViewModelBase = nonHookGameViewModelBase;
        }
        private SelectPlaythroughViewModel _selectPlaythroughViewModel;
        public NonHookGameSubViewNavigateCommand(SelectPlaythroughViewModel selectPlaythroughViewModel)
        {
            _selectPlaythroughViewModel = selectPlaythroughViewModel;
        }

        public override void Execute(object? parameter)
        {
            switch(parameter.ToString())
            {
                case "SELECT":
                    _nonHookGameViewModelBase.CurrentSubView = new SelectPlaythroughViewModel(_nonHookGameViewModelBase.Game,
                                                                        ref _nonHookGameViewModelBase,
                                                                        ref _nonHookGameViewModelBase.NonHookGamesDataStore);
                    break;
                case "CREATE":
                    _selectPlaythroughViewModel.CurrentSubView = new CreatePlaythroughViewModel(ref _selectPlaythroughViewModel,
                                                                        ref _selectPlaythroughViewModel.NonHookGameViewModelBase,
                                                                            _selectPlaythroughViewModel.NonHookGameViewModelBase.Game);
                    break;
            }
        }
    }
}
