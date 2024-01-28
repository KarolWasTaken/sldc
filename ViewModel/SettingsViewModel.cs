using sldc.Commands;
using sldc.Model;
using sldc.Stores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Input;

namespace sldc.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        internal DRPClientStore _drpClientStore;
        internal HookStore _hookStore;

        private List<string> _themeList = new List<string>(Enum.GetNames(typeof(ThemeType)));
        public List<string> ThemeList
        {
            get { return _themeList; }
            set { _themeList = value; }
        }
        public int SelectedThemeIndex { get; set; }
        private ThemeType _selectedTheme;
        public ThemeType SelectedTheme
        {
            get
            {
                return _selectedTheme;
            }
            set
            {
                _selectedTheme = value;
                OnPropertyChanged(nameof(SelectedTheme));
                ApplyButtonEnabled = true;
                OnPropertyChanged(nameof(ApplyButtonEnabled));
            }
        }
        public bool EnableDRPCredit { get; set; }
        public bool DRPStatus { get; set; }
        public bool EnableCovenantDisplay { get; set; }

        private bool _applyButtonEnabled;
        public bool ApplyButtonEnabled
        {
            get
            {
                return _applyButtonEnabled;
            }
            set
            {
                _applyButtonEnabled = value;
                OnPropertyChanged(nameof(ApplyButtonEnabled));
            }
        }
        public ICommand RevertSettingsToDefaultCommand { get; }
        public ICommand CommittSettingsChangesCommand { get; }
        public RelayCommand ToggleDRPCommand { get; private set; }
        public RelayCommand ToggleDRPCreditCommand { get; private set; }
        public RelayCommand ToggleCovenantDisplayCommand { get; private set; }
        public SettingsViewModel(DRPClientStore _discordRpcClientStore, HookStore hookStore)
        {
            // store set up
            _drpClientStore = _discordRpcClientStore;
            _hookStore = hookStore;
            // grab settings
            Settings settings = Helper.ReturnSettings();
            EnableCovenantDisplay = settings.EnableCovenantDisplay;

            // set enum from settings
            ThemeType currentTheme = ThemeType.DarkTheme;
            if (Enum.TryParse<ThemeType>(Helper.Config["Theme"], out ThemeType EnumString))
            {
                currentTheme = EnumString;
            }
            SelectedThemeIndex = (int)currentTheme;
            SelectedTheme = currentTheme;
            OnPropertyChanged(nameof(SelectedThemeIndex));

            // setup drp button
            DRPStatus = settings.IsDRPEnabled == true;
            OnPropertyChanged(nameof(DRPStatus));

            // setup credit
            EnableDRPCredit = settings.EnableDRPCredit == true;
            OnPropertyChanged(nameof(EnableDRPCredit));


            // set up commands
            RevertSettingsToDefaultCommand = new RevertSettingsToDefaultCommand();
            CommittSettingsChangesCommand = new CommittSettingsChangesCommand(this);
            ToggleDRPCommand = new RelayCommand(ToggleDRP);
            ToggleDRPCreditCommand = new RelayCommand(ToggleDRPCredit);
            ToggleCovenantDisplayCommand = new RelayCommand(ToggleCovenantDisplay);

            ApplyButtonEnabled = false;
        }

        private void ToggleDRP(object paramters)
        {
            DRPStatus = !DRPStatus;
            OnPropertyChanged(nameof(DRPStatus));
            ApplyButtonEnabled = true;
        }
        private void ToggleDRPCredit(object paramters)
        { 
            EnableDRPCredit = !EnableDRPCredit;
            if(EnableCovenantDisplay)
            {
                EnableCovenantDisplay = false;
                OnPropertyChanged(nameof(EnableCovenantDisplay));
            }
            OnPropertyChanged(nameof(EnableDRPCredit));
            ApplyButtonEnabled = true;
        }
        private void ToggleCovenantDisplay(object paramters)
        {
            EnableCovenantDisplay = !EnableCovenantDisplay;
            if(EnableDRPCredit) 
            {
                EnableDRPCredit = false;
                OnPropertyChanged(nameof(EnableDRPCredit));
            }
            OnPropertyChanged(nameof(EnableCovenantDisplay));
            ApplyButtonEnabled = true;
        }
    }
}
