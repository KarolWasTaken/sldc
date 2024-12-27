using DiscordRPC;
using Microsoft.Extensions.Configuration;
using sldc.Commands.NavigateViewModelCommands;
using sldc.Model;
using sldc.Stores;
using sldc.Themes;
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
        private readonly NonHookGamesDataStore _nonHookGamesDataStore;

        // viewmodels
        private DSREViewModel _dSREViewModel;
        private DS2SoTFSViewModel _dS2SoTFSViewModel;
        private DS3ViewModel _dS3ViewModel;
        private ERViewModel _eRViewModel;
        private BLViewModel _bLViewModel;
        public App()
        {
            _navigationStore = new NavigationStore();
            _streamerWindowStore = new StreamerWindowStore();
            _discordRpcClientStore = new DRPClientStore();
            _hookStore = new HookStore();
            _nonHookGamesDataStore = GameDeathDataSerialiser.LoadData();


            // create viewmodels once. This means, when we re-enter them, they
            // will remain the same and wont be re-instantiated.
            _dSREViewModel = new DSREViewModel();
            _dS2SoTFSViewModel = new DS2SoTFSViewModel(_streamerWindowStore, _discordRpcClientStore, _hookStore, DRPClientStore.ENVTokens.DS2_TOKEN);
            _dS3ViewModel = new DS3ViewModel(_streamerWindowStore, _discordRpcClientStore, _hookStore, DRPClientStore.ENVTokens.DS3_TOKEN);
            _eRViewModel = new ERViewModel();
            _bLViewModel = new BLViewModel(_streamerWindowStore, _discordRpcClientStore, _hookStore, _nonHookGamesDataStore, DRPClientStore.ENVTokens.BL_TOKEN);
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            // load environmental variables (discord tokens)
            var root = Directory.GetCurrentDirectory();
            var dotenv = Path.Combine(root, ".env");
            DotEnv.Load(dotenv);

            // set your theme to the saved theme
            // refactor this with commit changes command
            switch (Helper.ReturnSettings().Theme)
            {
                case ThemeType.LightTheme:
                    AppThemeChanger.ChangeTheme(new Uri("Themes/LightTheme.xaml", UriKind.Relative));
                    break;
                case ThemeType.DarkTheme:
                    AppThemeChanger.ChangeTheme(new Uri("Themes/DarkTheme.xaml", UriKind.Relative));
                    break;
            }

            // set startup window
            _navigationStore.CurrentViewModel = _dS3ViewModel;

            string test = "";

            try
            {
                var dict = new ResourceDictionary
                {
                    Source = new Uri("Themes/ElementThemes/GameSelectButtons.xaml", UriKind.Relative)
                };
                Application.Current.Resources.MergedDictionaries.Add(dict);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load GameSelectButtons: {ex.Message}");
            }

            //foreach (ResourceDictionary dictionary in Application.Current.Resources.MergedDictionaries)
            //{
            //    foreach (var key in dictionary.Keys)
            //    {

            //        test += $"key: {key}\n";
            //    }
            //}
            //MessageBox.Show(test);


            MainWindow mainWindow = new MainWindow()
            {
                DataContext = new MainWindowViewModel
                (
                    _navigationStore,
                    CreateDSREViewModel,
                    CreateDS2SoTFSViewModel,
                    CreateDS3ViewModel,
                    CreateERViewModel,
                    CreateBLViewModel,
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
        private BLViewModel CreateBLViewModel()
        {
            return _bLViewModel;
        }
        private SettingsViewModel CreateSettingsViewModel()
        {
            return new SettingsViewModel(_discordRpcClientStore, _hookStore);
        }
    }

}
