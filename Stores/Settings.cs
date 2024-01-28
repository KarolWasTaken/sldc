using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
