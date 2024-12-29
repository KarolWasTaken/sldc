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
    // the reason all these navigate command are separate is incase i need a given navigate command to do something special 
    // that the other commands dont need to do.
    class DS2SoTFSNavigateCommand : CommandBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly NavigateCommand _navigationCommand;
        public DS2SoTFSNavigateCommand(MainWindowViewModel mainWindowViewModel, NavigationStore navigationStore, Func<DS2SoTFSViewModel> createDS2SoTFSViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _navigationCommand = new NavigateCommand(navigationStore, createDS2SoTFSViewModel, mainWindowViewModel.TriggerEnterViewModelAnimation, mainWindowViewModel.TriggerExitViewModelAnimation);
        }
        public override void Execute(object? parameter)
        {
            _navigationCommand.ChangeViewModel();
        }
    }
}
