using sldc.Themes.AppThemes;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace sldc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool IsDarkTheme = true;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ChangeTheme()
        {
            if (IsDarkTheme)
            {
                AppThemeChanger.ChangeTheme(new Uri("Themes/AppThemes/LightTheme.xaml", UriKind.Relative));
            }
            else
            {
                AppThemeChanger.ChangeTheme(new Uri("Themes/AppThemes/DarkTheme.xaml", UriKind.Relative));
            }
            IsDarkTheme = !IsDarkTheme;
        }

        private void DarkThemePressed(object sender, RoutedEventArgs e)
        {
            ChangeTheme();
        }
        private void LightThemePressed(object sender, RoutedEventArgs e)
        {
            ChangeTheme();
        }
    }
}