using sldc.Stores;
using sldc.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sldc.Commands.NavigateViewModelCommands
{
    class BLNavigateCommand : CommandBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly NavigateCommand _navigationCommand;
        public BLNavigateCommand(MainWindowViewModel mainWindowViewModel, NavigationStore navigationStore, Func<BLViewModel> createBLViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _navigationCommand = new NavigateCommand(navigationStore, createBLViewModel, mainWindowViewModel.TriggerEnterViewModelAnimation, mainWindowViewModel.TriggerExitViewModelAnimation);
        }
        public override void Execute(object? parameter)
        {
            _navigationCommand.ChangeViewModel();
        }
    }
}
