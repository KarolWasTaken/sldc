
using sldc.Stores;
using sldc.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sldc.Commands
{
    public class NavigateCommand
    {
        private readonly NavigationStore _navigationStore;
        private readonly Func<ViewModelBase> _createViewModel;

        public NavigateCommand(NavigationStore navigationStore, Func<ViewModelBase> createViewModel)
        {
            _navigationStore = navigationStore;
            _createViewModel = createViewModel;
        }

        public void ChangeViewModel()
        {
            // when this is ran (when a navcommand is ran), it'll tell all the subscribers of this event
            // to execute their subscribed event (MainWindowViewModel.cs OnCurrentViewModelChanged())
            _navigationStore.CurrentViewModel = _createViewModel();
        }
    }
}
