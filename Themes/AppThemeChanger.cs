using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace sldc.Themes
{
    public class AppThemeChanger
    {
        public static void ChangeTheme(Uri themeuri)
        {
            ResourceDictionary theme = new ResourceDictionary()
            {
                Source = themeuri
            };
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(theme);
        }
    }
}
