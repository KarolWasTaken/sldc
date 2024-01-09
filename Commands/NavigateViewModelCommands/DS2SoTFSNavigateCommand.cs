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
    class DS2SoTFSNavigateCommand : CommandBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly NavigateCommand _navigationCommand;
        public DS2SoTFSNavigateCommand(MainWindowViewModel mainWindowViewModel, NavigationStore navigationStore, Func<DS2SoTFSViewModel> createDS2SoTFSViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _navigationCommand = new NavigateCommand(navigationStore, createDS2SoTFSViewModel);
        }
        public override void Execute(object? parameter)
        {
            _navigationCommand.ChangeViewModel();
        }
    }
}
