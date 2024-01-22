using sldc.Stores;
using sldc.Themes;
using sldc.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using static sldc.Stores.DRPClientStore;

namespace sldc.Commands
{
    public class CommittSettingsChangesCommand : CommandBase
    {
        private SettingsViewModel _settingsViewModel;

        public CommittSettingsChangesCommand(SettingsViewModel settingsViewModel)
        {
            _settingsViewModel = settingsViewModel;
        }
        // refactor later
        public override void Execute(object? parameter)
        {
            // disable apply button
            _settingsViewModel.ApplyButtonEnabled = false;

            // update settings config
            Helper.settings.Theme = _settingsViewModel.SelectedTheme;
            Helper.settings.IsDRPEnabled = _settingsViewModel.DRPStatus;
            Helper.UpdateSettings();

            // dpr stuff
            if (_settingsViewModel.DRPStatus == true)
            {
                // drp turn on from off
                if (_settingsViewModel._drpClientStore.Client == null && _settingsViewModel._hookStore.hook != null)
                {
                    ENVTokens envToken = SettingsViewModel.GetEnvTokenFromHook(_settingsViewModel._hookStore.hook);
                    _settingsViewModel._drpClientStore.CreateClient(envToken);
                    _settingsViewModel._drpClientStore.UpdatePresence(envToken, _settingsViewModel._hookStore.hook.Death);
                }
            }
            else
            {
                // drp off from on
                if (_settingsViewModel._drpClientStore.Client != null)
                {
                    _settingsViewModel._drpClientStore.DisposeClient();
                }
            }

            // change themes
            if (_settingsViewModel.SelectedTheme == null)
                return;
            switch(_settingsViewModel.SelectedTheme)
            {
                case ThemeType.LightTheme:
                    AppThemeChanger.ChangeTheme(new Uri("Themes/LightTheme.xaml", UriKind.Relative));
                    break;
                case ThemeType.DarkTheme:
                    AppThemeChanger.ChangeTheme(new Uri("Themes/DarkTheme.xaml", UriKind.Relative));
                    break;
            }
            // change credit mode
            Settings s = Helper.ReturnSettings();
            bool reloadDRP = s.EnableDRPCredit != _settingsViewModel.EnableDRPCredit && _settingsViewModel._drpClientStore.Client != null;
            s.EnableDRPCredit = _settingsViewModel.EnableDRPCredit;
            Helper.settings = s;
            Helper.UpdateSettings();
            if (reloadDRP)
            {
                ENVTokens envToken = SettingsViewModel.GetEnvTokenFromHook(_settingsViewModel._hookStore.hook);
                _settingsViewModel._drpClientStore.DisposeClient();
                _settingsViewModel._drpClientStore.CreateClient(envToken);
                _settingsViewModel._drpClientStore.UpdatePresence(envToken, _settingsViewModel._hookStore.hook.Death);
            }

        }
    }
}
