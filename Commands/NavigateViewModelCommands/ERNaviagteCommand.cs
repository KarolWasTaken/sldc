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
    class ERNaviagteCommand : CommandBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly NavigateCommand _navigationCommand;
        public ERNaviagteCommand(MainWindowViewModel mainWindowViewModel, NavigationStore navigationStore, Func<ERViewModel> createERViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _navigationCommand = new NavigateCommand(navigationStore, createERViewModel);
        }
        public override void Execute(object? parameter)
        {
            _navigationCommand.ChangeViewModel();
        }
    }
}
