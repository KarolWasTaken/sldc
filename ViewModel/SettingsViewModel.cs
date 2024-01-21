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
        public bool DRPStatus { get; set; }
        private string _drpButtonText;
        public string DRPButtonText
        {
            get { return _drpButtonText; }
            set { 
                _drpButtonText = value; 
                OnPropertyChanged(nameof(DRPButtonText));
                ApplyButtonEnabled = true;
                OnPropertyChanged(nameof(ApplyButtonEnabled));
            }
        }


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
        public SettingsViewModel(DRPClientStore _discordRpcClientStore, HookStore hookStore)
        {
            // store set up
            _drpClientStore = _discordRpcClientStore;
            _hookStore = hookStore;

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
            if (Helper.Config["IsDRPEnabled"] == "True")
            {
                DRPStatus = true;
                DRPButtonText = "Disable";
            }
            else
            {
                DRPStatus = false;
                DRPButtonText = "Enable";
            }
            OnPropertyChanged(nameof(DRPStatus));
            // set up commands
            RevertSettingsToDefaultCommand = new RevertSettingsToDefaultCommand();
            CommittSettingsChangesCommand = new CommittSettingsChangesCommand(this);
            ToggleDRPCommand = new RelayCommand(ToggleDRP);

            ApplyButtonEnabled = false;
        }

        private void ToggleDRP(object paramters)
        {
            DRPStatus = !DRPStatus;
            OnPropertyChanged(nameof(DRPStatus));
            if (DRPStatus == true)
            {
                // drp turn on from off
                DRPButtonText = "Disable";
            }
            else
            {
                // drp off from on
                DRPButtonText = "Enable";
            }
        }

        public static DRPClientStore.ENVTokens GetEnvTokenFromHook(BaseHook hook)
        {
            switch (hook)
            {
                case DS2Hook ds2h:
                    return DRPClientStore.ENVTokens.DS2_TOKEN;
                default: 
                    throw new NotImplementedException();
            }
        }
    }
}
