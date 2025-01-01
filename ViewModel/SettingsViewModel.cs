using Newtonsoft.Json.Linq;
using sldc.Commands;
using sldc.Model;
using sldc.Stores;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using FontFamily = System.Windows.Media.FontFamily;
using FontStyle = System.Windows.FontStyle;

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
                AreChangesMade = true;
                OnPropertyChanged(nameof(AreChangesMade));
            }
        }
        public bool EnableDRPCredit { get; set; }
        public bool DRPStatus { get; set; }
        public bool EnableCovenantDisplay { get; set; }

        // font properties
        private FontFamily _selectedFontFamily;
        private FontWeight _selectedFontWeight;
        private FontStyle _selectedFontStyle;
        private FontStretch _selectedFontStretch;
        public FontFamily SelectedFontFamily
        {
            get => _selectedFontFamily;
            set
            {
                _selectedFontFamily = value;
                OnPropertyChanged(nameof(SelectedFontFamily));
                AreChangesMade = true;
            }
        }
        public FontWeight SelectedFontWeight
        {
            get => _selectedFontWeight;
            set
            {
                _selectedFontWeight = value;
                OnPropertyChanged(nameof(SelectedFontWeight));
            }
        }
        public FontStyle SelectedFontStyle
        {
            get => _selectedFontStyle;
            set
            {
                _selectedFontStyle = value;
                OnPropertyChanged(nameof(SelectedFontStyle));
            }
        }
        public FontStretch SelectedFontStretch
        {
            get => _selectedFontStretch;
            set
            {
                _selectedFontStretch = value;
                OnPropertyChanged(nameof(SelectedFontStretch));
            }
        }
        private FamilyTypeface _selectedTypeface;
        public FamilyTypeface SelectedTypeface
        {
            get => _selectedTypeface;
            set
            {
                _selectedTypeface = value;

                if (_selectedTypeface != null)
                {
                    SelectedFontWeight = _selectedTypeface.Weight;
                    SelectedFontStyle = _selectedTypeface.Style;
                    SelectedFontStretch = _selectedTypeface.Stretch;
                }
                OnPropertyChanged(nameof(SelectedTypeface));
                AreChangesMade = true;
            }
        }

        private string _fontColourHex;
        public string FontColourHex
        {
            get
            {
                return _fontColourHex;
            }
            set
            {
                AreChangesMade = true;
                _fontColourHex = value;

                RemoveError(nameof(FontColourHex));

                if (value.Length > 0)
                {
                    if (!HexValidator.ValidateHexCode(value))
                        AddError(nameof(FontColourHex), "Please enter a valid HEX code");
                    else
                    {
                        if(value.Length != 1)
                            FinalFontColour = value;
                    }
                }
                
                OnPropertyChanged(nameof(FontColourHex));
                OnPropertyChanged(nameof(CanCommitChanges));
            }
        }
        private bool _areChangesMade;
        public bool AreChangesMade
        {
            get
            {
                return _areChangesMade;
            }
            set
            {
                _areChangesMade = value;
                OnPropertyChanged(nameof(AreChangesMade));
                OnPropertyChanged(nameof(CanCommitChanges));
            }
        }
        private string _finalFontColour;
        public string FinalFontColour
        {
            get
            {
                return _finalFontColour;
            }
            set
            {
                _finalFontColour = value;
                OnPropertyChanged(nameof(FinalFontColour));
            }
        }
        public bool CanCommitChanges => AreChangesMade && !HasErrors;
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

            // setup font
            // also guard against someone editing json to enter invalid font
            if (!HexValidator.ValidateHexCode(settings.FontColour))
            {
                FinalFontColour = "#FFFFFF";
            }
            else
                FinalFontColour = settings.FontColour;

            // setup drp button
            DRPStatus = settings.IsDRPEnabled == true;
            OnPropertyChanged(nameof(DRPStatus));

            // setup credit
            EnableDRPCredit = settings.EnableDRPCredit == true;
            OnPropertyChanged(nameof(EnableDRPCredit));


            // set up commands
            RevertSettingsToDefaultCommand = new RevertSettingsToDefaultCommand(this, OnPropertyChanged);
            CommittSettingsChangesCommand = new CommittSettingsChangesCommand(this);
            ToggleDRPCommand = new RelayCommand(ToggleDRP);
            ToggleDRPCreditCommand = new RelayCommand(ToggleDRPCredit);
            ToggleCovenantDisplayCommand = new RelayCommand(ToggleCovenantDisplay);

            AreChangesMade = false;
        }

        private void ToggleDRP(object paramters)
        {
            DRPStatus = !DRPStatus;
            OnPropertyChanged(nameof(DRPStatus));
            AreChangesMade = true;
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
            AreChangesMade = true;
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
            AreChangesMade = true;
        }
    }
}
