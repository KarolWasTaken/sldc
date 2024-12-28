using sldc.Commands;
using sldc.Commands.NavigateViewModelCommands;
using sldc.Model;
using sldc.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace sldc.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        public ViewModelBase CurrentViewModel => _navigationStore.CurrentViewModel;


        // for error messages
        private string _errorBodyMessage;
        public string ErrorBodyMessage
        {
            get { return _errorBodyMessage; }
            set
            {
                _errorBodyMessage = value;
                OnPropertyChanged(nameof(ErrorBodyMessage));
            }
        }
        private string _errorHeaderMessage;
        public string ErrorHeaderMessage
        {
            get { return _errorHeaderMessage; }
            set
            {
                _errorHeaderMessage = value;
                OnPropertyChanged(nameof(ErrorHeaderMessage));
            }
        }
        public event Action PlayErrorAnimation;
        public void SendErrorMessageToViewModel(string Header, string Body)
        {
            ErrorHeaderMessage = Header;
            ErrorBodyMessage = Body;
            PlayErrorAnimation?.Invoke();
        }



        // naviagtion command here
        public ICommand DSRENavigateCommand { get; }
        public ICommand DS2SoTFSNavigateCommand { get; }
        public ICommand DS3NavigateCommand { get; }
        public ICommand ERNavigateCommand { get; }
        public ICommand BLNavigateCommand { get; }
        public ICommand SettingsNavigateCommand { get; }
        public MainWindowViewModel(NavigationStore navigationStore,
            Func<DSREViewModel> createDSREViewModel,
            Func<DS2SoTFSViewModel> createDS2SoTFSViewModel,
            Func<DS3ViewModel> createDS3ViewModel,
            Func<ERViewModel> createERViewModel,
            Func<BLViewModel> createBLViewModel,
            Func<SettingsViewModel> createSettingsViewModel)
        {
            // navigation commands
            DSRENavigateCommand = new DSRENavigateCommand(this, navigationStore, createDSREViewModel);
            DS2SoTFSNavigateCommand = new DS2SoTFSNavigateCommand(this, navigationStore, createDS2SoTFSViewModel);
            DS3NavigateCommand = new DS3NavigateCommand(this, navigationStore, createDS3ViewModel);
            ERNavigateCommand = new ERNaviagteCommand(this, navigationStore, createERViewModel);
            BLNavigateCommand = new BLNavigateCommand(this, navigationStore, createBLViewModel);
            SettingsNavigateCommand = new SettingsNavigateCommand(this, navigationStore, createSettingsViewModel);

            _navigationStore = navigationStore;
            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;

        }

        /// <summary>
        /// reminds model to update, on the ui thread, the CurrentViewModel property - which updates the view and datacontext
        /// </summary>
        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}
