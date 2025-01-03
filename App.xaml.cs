﻿using DiscordRPC;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using sldc.Commands.NavigateViewModelCommands;
using sldc.Model;
using sldc.Stores;
using sldc.Themes;
using sldc.View;
using sldc.ViewModel;
using sldc.Windows;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Policy;
using System.Text.Json;
using System.Text.RegularExpressions;
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

        // subscribers (talk between viewmodel and mainwindow viewmodel)
        public delegate void ErrorMessageHandler(string header, string body);
        public delegate void ConnectedToGameHandler(bool status);
        public App()
        {
            _navigationStore = new NavigationStore();
            _streamerWindowStore = new StreamerWindowStore();
            _discordRpcClientStore = new DRPClientStore();
            _hookStore = new HookStore();
            _nonHookGamesDataStore = GameDeathDataSerialiser.LoadData();


            // create viewmodels once. This means, when we re-enter them, they
            // will remain the same and wont be re-instantiated.
            _dSREViewModel = new DSREViewModel(_streamerWindowStore, _discordRpcClientStore, _hookStore, DRPClientStore.ENVTokens.DS1_TOKEN);
            _dS2SoTFSViewModel = new DS2SoTFSViewModel(_streamerWindowStore, _discordRpcClientStore, _hookStore, DRPClientStore.ENVTokens.DS2_TOKEN);
            _dS3ViewModel = new DS3ViewModel(_streamerWindowStore, _discordRpcClientStore, _hookStore, DRPClientStore.ENVTokens.DS3_TOKEN);
            _eRViewModel = new ERViewModel();
            _bLViewModel = new BLViewModel(_streamerWindowStore, _discordRpcClientStore, _hookStore, _nonHookGamesDataStore, DRPClientStore.ENVTokens.BL_TOKEN);
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            // load environmental variables (discord tokens)
            DotEnv.Load();
            Helper.ReturnSettings();

            // set your theme to the saved theme
            // refactor this with commit changes command
            switch (Helper.settings.Theme)
            {
                case ThemeType.LightTheme:
                    AppThemeChanger.ChangeTheme(new Uri("Themes/LightTheme.xaml", UriKind.Relative));
                    break;
                case ThemeType.DarkTheme:
                    AppThemeChanger.ChangeTheme(new Uri("Themes/DarkTheme.xaml", UriKind.Relative));
                    break;
            }
            // check for tampering with fontcolour
            if(!HexValidator.ValidateHexCode(Helper.settings.FontColour))
            {
                Helper.settings.FontColour = "#FFFFFF";
                Helper.UpdateSettings();
            }
            // set startup window
            _navigationStore.CurrentViewModel = _dS3ViewModel;

            //string test = "";
            //try
            //{
            //    var dict = new ResourceDictionary
            //    {
            //        Source = new Uri("Themes/ElementThemes/GameSelectButtons.xaml", UriKind.Relative)
            //    };
            //    Application.Current.Resources.MergedDictionaries.Add(dict);
            //}
            //catch (Exception ex)
            //{
            //    Debug.WriteLine($"Failed to load GameSelectButtons: {ex.Message}");
            //}

            //foreach (ResourceDictionary dictionary in Application.Current.Resources.MergedDictionaries)
            //{
            //    foreach (var key in dictionary.Keys)
            //    {

            //        test += $"key: {key}\n";
            //    }
            //}
            //MessageBox.Show(test);

            MainWindowViewModel mainWindowViewModel = new MainWindowViewModel ( _navigationStore,
                                                                                CreateDSREViewModel,
                                                                                CreateDS2SoTFSViewModel,
                                                                                CreateDS3ViewModel,
                                                                                CreateERViewModel,
                                                                                CreateBLViewModel,
                                                                                CreateSettingsViewModel );
            // do this to all future viewmodel games
            _dS2SoTFSViewModel.OnErrorMessage += mainWindowViewModel.SendErrorMessageToViewModel;
            _dS3ViewModel.OnErrorMessage += mainWindowViewModel.SendErrorMessageToViewModel;
            _dSREViewModel.OnErrorMessage += mainWindowViewModel.SendErrorMessageToViewModel;
            // actually opening the mainwindow
            MainWindow mainWindow = new MainWindow()
            {
                DataContext = mainWindowViewModel
            };
            mainWindow.Show();


            if (!Helper.ReturnSettings().DontShowAgain)
                CheckForUpdates();

            base.OnStartup(e);  
        }

        private async Task CheckForUpdates()
        {
            // if there is no tag
            if (Helper.settings.TagName == null)
            {
                Helper.settings.TagName = await GetLatestVersionTag();
            }


            string newestTag = null;

            // grab settings
            Helper.ReturnSettings();

            try
            {
                newestTag = await GetLatestVersionTag();
            }
            catch (HttpRequestException e)
            {
                MessageBox.Show($"Request error: {e.Message}", "Error while checking for update");
            }

            if (newestTag == null)
                return;
            // Remove all alphabetic and non-numeric characters except dots and digits.
            string oldTagStripped = Regex.Replace(Helper.settings.TagName, "[^0-9.]", "");
            string newTagStripped = Regex.Replace(newestTag, "[^0-9.]", "");

            int comparisonCode = CompareVersion(oldTagStripped, newTagStripped);
            if (comparisonCode != -1)
                return;

            UpdateNotifier notifier = new UpdateNotifier();
            notifier.Show();
        }
        private async Task<string> GetLatestVersionTag()
        {
            string url = "https://api.github.com/repos/KarolWasTaken/sldc/releases/latest";
            using HttpClient client = new HttpClient();
            // set the User-Agent header (github requires this)
            client.DefaultRequestHeaders.Add("User-Agent", $"sldc/{Helper.settings.TagName}");

            // make the GET request
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode(); // throw if not a success code

            // Read the response as a string
            string jsonResponse = await response.Content.ReadAsStringAsync();

            // parse the json
            using JsonDocument doc = JsonDocument.Parse(jsonResponse);
            JsonElement root = doc.RootElement;

            if (root.TryGetProperty("tag_name", out JsonElement tagName))
                return tagName.GetString();
            else
                throw new Exception("tag_name was not found in github GET json");
        }
        static private int CompareVersion(string v1, string v2)
        {
            // Split version strings into individual components
            string[] version1Parts = v1.Split('.');
            string[] version2Parts = v2.Split('.');

            int maxLength = Math.Max(version1Parts.Length, version2Parts.Length);

            for (int i = 0; i < maxLength; i++)
            {
                // Parse each part as integers, defaulting to 0 if not present
                int value1 = i < version1Parts.Length ? int.Parse(version1Parts[i]) : 0;
                int value2 = i < version2Parts.Length ? int.Parse(version2Parts[i]) : 0;

                if (value1 > value2) return 1;
                if (value1 < value2) return -1;
            }
            
            // all components are equal
            return 0;
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
