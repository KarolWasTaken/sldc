﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace sldc.Themes
{
    public class AppThemeChanger
    {

        private static List<Uri> ADDITIONAL_STYLES_TO_LOAD = new List<Uri>()
        {
            new Uri("Themes/ElementThemes/PrimaryButtonStyle.xaml", UriKind.Relative),
            new Uri("Themes/ElementThemes/SecondaryButtonStyle.xaml", UriKind.Relative),
        };


        public static void ChangeTheme(Uri themeuri)
        {
            ResourceDictionary theme = new ResourceDictionary()
            {
                Source = themeuri
            };
            // adds additional themes like button styles.
            foreach (Uri additonalStyle in ADDITIONAL_STYLES_TO_LOAD)
            {
                ResourceDictionary additionalTheme = new ResourceDictionary()
                {
                    Source = additonalStyle
                };
                theme.MergedDictionaries.Add(additionalTheme);
            }
            //foreach (var key in theme.Keys)
            //{
            //    MessageBox.Show(key.ToString());
            //}
            // clears all previous themes and resources
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Clear();
            // adds the new one (light or dark theme)
            Application.Current.Resources.MergedDictionaries.Add(theme);
        }


    }
}
