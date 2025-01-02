using sldc.Converter;
using sldc.Model;
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
using static sldc.Model.GameDeathDataSerialiser;

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
            _settingsViewModel.AreChangesMade = false;

            Helper.ReturnSettings();

            // check if drp needs to reload
            // check if drpcredit changed and if there is a discord client
            bool reloadDRP =
                (Helper.settings.EnableDRPCredit != _settingsViewModel.EnableDRPCredit || Helper.settings.EnableCovenantDisplay != _settingsViewModel.EnableCovenantDisplay)
                && _settingsViewModel._drpClientStore.Client != null;

            // update settings config
            Helper.settings.Theme = _settingsViewModel.SelectedTheme;
            Helper.settings.IsDRPEnabled = _settingsViewModel.DRPStatus;
            Helper.settings.EnableDRPCredit = _settingsViewModel.EnableDRPCredit;
            Helper.settings.EnableCovenantDisplay = _settingsViewModel.EnableCovenantDisplay;
            Helper.settings.FontColour = _settingsViewModel.FinalFontColour;
            Helper.settings.EnableMinimiseToToolbar = _settingsViewModel.EnableMinimiseToToolbar;
            // used null coalescing here later
            if (_settingsViewModel.SelectedFontFamily != null)
            {
                Helper.settings.StreamerWindowFontFamily = _settingsViewModel.SelectedFontFamily.Source;
                if(_settingsViewModel.SelectedTypeface != null)
                {
                    Helper.settings.StreamerWindowFontWeight = _settingsViewModel.SelectedFontWeight.ToString();
                    Helper.settings.StreamerWindowFontStyle = _settingsViewModel.SelectedFontStyle.ToString();
                    Helper.settings.StreamerWindowFontStretch = _settingsViewModel.SelectedFontStretch.ToString();
                }
                else
                {
                    Helper.settings.StreamerWindowFontWeight = "Normal";
                    Helper.settings.StreamerWindowFontStyle = "Normal";
                    Helper.settings.StreamerWindowFontStretch = "Normal";
                }
            }
            Helper.UpdateSettings();

            // drp reload if game is already on
            if (_settingsViewModel.DRPStatus == true)
            {
                // drp turn on from off
                if (_settingsViewModel._drpClientStore.Client == null && _settingsViewModel._hookStore.HookedGame != null)
                {
                    ENVTokens envToken = HookToENVTokenConverter.Convert(_settingsViewModel._hookStore.HookedGame);
                    _settingsViewModel._drpClientStore.CreateClient(envToken);
                    UpdatePresenceDeaths(envToken);
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

            // reload drp if needed
            if (reloadDRP)
            {
                ENVTokens envToken = HookToENVTokenConverter.Convert(_settingsViewModel._hookStore.HookedGame);
                _settingsViewModel._drpClientStore.DisposeClient();
                _settingsViewModel._drpClientStore.CreateClient(envToken);

                UpdatePresenceDeaths(envToken);
                if (Helper.settings.EnableCovenantDisplay && envToken != ENVTokens.BL_TOKEN)
                    _settingsViewModel._drpClientStore.UpdatePresence(_settingsViewModel._hookStore.hook.Covenant);
            }

        }
        public void UpdatePresenceDeaths(ENVTokens envToken)
        {
            if (envToken == ENVTokens.BL_TOKEN)
            {
                _settingsViewModel._drpClientStore.UpdatePresence(
                    GameDeathDataSerialiser.LoadData(
                        NON_HOOKABLE_GAME.BLOODBORNE)[_settingsViewModel._hookStore.HookedPlaythroughName]
                        );
            }
            else
            {
                _settingsViewModel._drpClientStore.UpdatePresence(_settingsViewModel._hookStore.hook.Death);
            }
        }
    }
}
