using System;
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
        public static void ChangeTheme(Uri themeuri)
        {
            ResourceDictionary theme = new ResourceDictionary()
            {
                Source = themeuri
            };
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(theme);

            foreach (Window window in Application.Current.Windows)
            {
                // Assuming controls need to be refreshed in each window
                foreach (var control in FindVisualChildren<Control>(window))
                {
                    control.ApplyTemplate(); // Force re-application of templates
                }
            }
        }
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T t)
                    {
                        yield return t;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}
