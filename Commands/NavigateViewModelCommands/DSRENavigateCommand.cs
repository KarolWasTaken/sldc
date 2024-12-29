using sldc.Commands;
using sldc.Stores;
using sldc.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sldc.Commands.NavigateViewModelCommands
{
    class DSRENavigateCommand : CommandBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly NavigateCommand _navigationCommand;
        public DSRENavigateCommand(MainWindowViewModel mainWindowViewModel, NavigationStore navigationStore, Func<DSREViewModel> createDSREViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _navigationCommand = new NavigateCommand(navigationStore, createDSREViewModel, mainWindowViewModel.TriggerEnterViewModelAnimation, mainWindowViewModel.TriggerExitViewModelAnimation);
        }
        public override void Execute(object? parameter)
        {
            _navigationCommand.ChangeViewModel();
        }
    }
}
