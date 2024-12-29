using sldc.Stores;
using sldc.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sldc.Commands.NavigateViewModelCommands
{
    class SettingsNavigateCommand : CommandBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly NavigateCommand _navigationCommand;
        public SettingsNavigateCommand(MainWindowViewModel mainWindowViewModel, NavigationStore navigationStore, Func<SettingsViewModel> createSettingsViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _navigationCommand = new NavigateCommand(navigationStore, createSettingsViewModel, mainWindowViewModel.TriggerEnterViewModelAnimation, mainWindowViewModel.TriggerExitViewModelAnimation);
        }
        public override void Execute(object? parameter)
        {
            _navigationCommand.ChangeViewModel();
        }
    }
}
