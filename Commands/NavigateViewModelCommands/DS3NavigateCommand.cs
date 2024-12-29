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
    class DS3NavigateCommand : CommandBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly NavigateCommand _navigationCommand;
        public DS3NavigateCommand(MainWindowViewModel mainWindowViewModel, NavigationStore navigationStore, Func<DS3ViewModel> createDS3ViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _navigationCommand = new NavigateCommand(navigationStore, createDS3ViewModel, mainWindowViewModel.TriggerEnterViewModelAnimation, mainWindowViewModel.TriggerExitViewModelAnimation);
        }
        public override void Execute(object? parameter)
        {
            _navigationCommand.ChangeViewModel();
        }
    }
}
