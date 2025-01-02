using sldc.Stores;
using sldc.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace sldc.Commands
{
    public class RevertSettingsToDefaultCommand : CommandBase
    {
        private SettingsViewModel _settingsViewModel;
        private Action<string> _onPropertyChanged;
        public RevertSettingsToDefaultCommand(SettingsViewModel settingsViewModel, Action<string> onPropertyChanged)
        {
            _settingsViewModel = settingsViewModel;
            _onPropertyChanged = onPropertyChanged;
        }
        public override void Execute(object? parameter)
        {
            _settingsViewModel.SelectedTheme = ThemeType.DarkTheme;
            _settingsViewModel.SelectedThemeIndex = (int)ThemeType.DarkTheme;
            _onPropertyChanged.Invoke(nameof(_settingsViewModel.SelectedThemeIndex));
            _settingsViewModel.EnableMinimiseToToolbar = false;
            _onPropertyChanged.Invoke(nameof(_settingsViewModel.EnableMinimiseToToolbar));
            _settingsViewModel.DRPStatus = true;
            _onPropertyChanged.Invoke(nameof(_settingsViewModel.DRPStatus));
            _settingsViewModel.EnableCovenantDisplay = true;
            _onPropertyChanged.Invoke(nameof(_settingsViewModel.EnableCovenantDisplay));
            _settingsViewModel.EnableDRPCredit = false;
            _onPropertyChanged.Invoke(nameof(_settingsViewModel.EnableDRPCredit));
            _settingsViewModel.SelectedFontFamily = new FontFamily("Arial");
            _settingsViewModel.SelectedFontWeight = (FontWeight)FontWeights.Normal;
            _settingsViewModel.SelectedFontStyle = (FontStyle)FontStyles.Normal;
            _settingsViewModel.SelectedFontStretch = (FontStretch)FontStretches.Normal;
            _settingsViewModel.FontColourHex = "#FFFFFF";

        }
    }
}
