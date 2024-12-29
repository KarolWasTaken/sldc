using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FontStyle = System.Windows.FontStyle;

namespace sldc.Stores
{
    public enum ThemeType
    {
        DarkTheme,
        LightTheme
    }
    public class Settings
    {
        public ThemeType Theme;
        public bool IsDRPEnabled;
        public bool EnableDRPCredit;
        public bool EnableCovenantDisplay;
        public string? StreamerWindowFontFamily;
        public string? StreamerWindowFontWeight;
        public string? StreamerWindowFontStyle;
        public string? StreamerWindowFontStretch;
    }
}
