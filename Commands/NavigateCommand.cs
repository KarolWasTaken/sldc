
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
        private readonly Action _triggerEnterViewModelAnimation;
        private readonly Action _triggerExitViewModelAnimation;

        public NavigateCommand(NavigationStore navigationStore, Func<ViewModelBase> createViewModel, Action triggerEnterViewModelAnimation, Action triggerExitViewModelAnimation)
        {
            _navigationStore = navigationStore;
            _createViewModel = createViewModel;
            _triggerEnterViewModelAnimation = triggerEnterViewModelAnimation;
            _triggerExitViewModelAnimation = triggerExitViewModelAnimation;
        }

        public async Task ChangeViewModel()
        {
            // Introduce a delay of 1 seconds
            _triggerExitViewModelAnimation.Invoke();
            await Task.Delay(200);
            // when this is ran (when a navcommand is ran), it'll tell all the subscribers of this event
            // to execute their subscribed event (MainWindowViewModel.cs OnCurrentViewModelChanged())
            _navigationStore.CurrentViewModel = _createViewModel();
            _triggerEnterViewModelAnimation.Invoke();
        }
    }
}
