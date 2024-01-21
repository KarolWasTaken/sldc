using DiscordRPC;
using Microsoft.Extensions.Configuration;
using sldc.Commands.NavigateViewModelCommands;
using sldc.Model;
using sldc.Stores;
using sldc.View;
using sldc.ViewModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
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
        private readonly StreamerWindowStore _streamerWindowStore;
        private readonly DRPClientStore _discordRpcClientStore;
        private readonly HookStore _hookStore;

        // viewmodels
        private DSREViewModel _dSREViewModel;
        private DS2SoTFSViewModel _dS2SoTFSViewModel;
        private DS3ViewModel _dS3ViewModel;
        private ERViewModel _eRViewModel;
        public App()
        {
            _navigationStore = new NavigationStore();
            _streamerWindowStore = new StreamerWindowStore();
            _discordRpcClientStore = new DRPClientStore();
            _hookStore = new HookStore();

            // create viewmodels once. This means, when we re-enter them, they
            // will remain the same and wont be re-instantiated.
            _dSREViewModel = new DSREViewModel();
            _dS2SoTFSViewModel = new DS2SoTFSViewModel(_streamerWindowStore, _discordRpcClientStore, _hookStore);
            _dS3ViewModel = new DS3ViewModel();
            _eRViewModel = new ERViewModel();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            // load environmental variables (discord tokens)
            var root = Directory.GetCurrentDirectory();
            var dotenv = Path.Combine(root, ".env");
            DotEnv.Load(dotenv);

            MainWindow mainWindow = new MainWindow()
            {
                DataContext = new MainWindowViewModel
                (
                    _navigationStore,
                    CreateDSREViewModel,
                    CreateDS2SoTFSViewModel,
                    CreateDS3ViewModel,
                    CreateERViewModel,
                    CreateSettingsViewModel
                )
            };
            mainWindow.Show();

            base.OnStartup(e);  
        }

        private DSREViewModel CreateDSREViewModel()
        {
            return _dSREViewModel;
        }
        private DS2SoTFSViewModel CreateDS2SoTFSViewModel()
        {
            return _dS2SoTFSViewModel;
        }
        private DS3ViewModel CreateDS3ViewModel()
        {
            return _dS3ViewModel;
        }
        private ERViewModel CreateERViewModel()
        {
            return _eRViewModel;
        }
        private SettingsViewModel CreateSettingsViewModel()
        {
            return new SettingsViewModel(_discordRpcClientStore, _hookStore);
        }
    }

}
