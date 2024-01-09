using sldc.Commands.NavigateViewModelCommands;
using sldc.Stores;
using sldc.View;
using sldc.ViewModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;

namespace sldc
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly NavigationStore _navigationStore;
        public App()
        {
            _navigationStore = new NavigationStore();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow mainWindow = new MainWindow()
            {
                DataContext = new MainWindowViewModel(_navigationStore, CreateDSREViewModel, CreateDS2SoTFSViewModel, CreateDS3ViewModel, CreateERViewModel)
            };
            mainWindow.Show();

            base.OnStartup(e);  
        }

        private DSREViewModel CreateDSREViewModel()
        {
            return new DSREViewModel();
        }
        private DS2SoTFSViewModel CreateDS2SoTFSViewModel()
        {
            return new DS2SoTFSViewModel();
        }
        private DS3ViewModel CreateDS3ViewModel()
        {
            return new DS3ViewModel();
        }
        private ERViewModel CreateERViewModel()
        {
            return new ERViewModel();
        }
    }

}
