using sldc.Themes;
using sldc.ViewModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
        public MainWindow()
        {
            InitializeComponent();

            // Call the method asynchronously to allow for the delay after InitializeComponent
            // if it works, it works
            InitializeViewModelAsync();
        }

        private async Task InitializeViewModelAsync()
        {
            // Wait for 0.25 seconds to ensure DataContext is set
            await Task.Delay(250);  

            var viewModel = (MainWindowViewModel)DataContext;
            viewModel.PlayErrorAnimation += OnPlayErrorAnimation;
            viewModel.EnterViewModelAnimation += EnterViewModel;
            viewModel.ExitViewModelAnimation += ExitViewModel;
        }

        // using code-behind with mvvm is bad but this is also the easiest way to do this.
        private void close_errormsg_popup(object sender, MouseButtonEventArgs e)
        {
            var storyboard = (Storyboard)FindResource("CloseErrorAnimation");
            storyboard.Begin(this);
        }
        private void OnPlayErrorAnimation()
        {
            var storyboard = (Storyboard)FindResource("OpenErrorAnimation");
            storyboard.Begin(this);
        }

        private void EnterViewModel()
        {
            var storyboard = (Storyboard)FindResource("EnterViewModel");
            storyboard.Begin(this);
        }
        private void ExitViewModel()
        {
            var storyboard = (Storyboard)FindResource("ExitViewModel");
            storyboard.Begin(this);
        }

        private void SettingsButtonClicked(object sender, RoutedEventArgs e)
        {
            // Uncheck all RadioButtons
            DSRENavigateButton.IsChecked = false;
            DS2SoTFSNavigateButton.IsChecked = false;
            DS3NavigateButton.IsChecked = false;
            BLNavigateButton.IsChecked = false;
            ERNavigateButton.IsChecked = false;
        }
    }
}