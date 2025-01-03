using Hardcodet.Wpf.TaskbarNotification;
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
        private TaskbarIcon tbi;
        public MainWindow()
        {
            InitializeComponent();
            // Call the method asynchronously to allow for the delay after InitializeComponent
            // if it works, it works
            InitializeViewModelAsync();

            // for minimising to tooltips
            tbi = new TaskbarIcon();
            tbi.Visibility = Visibility.Collapsed;
            tbi.Icon = new System.Drawing.Icon(Application.GetResourceStream(new Uri("pack://application:,,,/sldc;component/icon.ico")).Stream);
            tbi.ToolTipText = "SLDC";
            tbi.DoubleClickCommand = new RelayCommand(DoubleClickOpenWindow);
            tbi.ContextMenu = CreateContextMenu();
            tbi.MenuActivation = PopupActivationMode.RightClick;
        }

        private ContextMenu CreateContextMenu()
        {
            // Create the ContextMenu
            ContextMenu contextMenu = new ContextMenu();

            // Create the "Open" MenuItem
            MenuItem openMenuItem = new MenuItem
            {
                Header = "Open"
            };
            openMenuItem.Click += (s, e) =>
            {
                this.ShowInTaskbar = true;
                this.WindowState = WindowState.Normal;
            };

            // Create the "Close" MenuItem
            MenuItem closeMenuItem = new MenuItem
            {
                Header = "Close"
            };
            closeMenuItem.Click += (s, e) =>
            {
                Application.Current.Shutdown(); // Example: Close the application
            };

            contextMenu.Items.Add(openMenuItem);
            contextMenu.Items.Add(closeMenuItem);

            return contextMenu;
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
            //BLNavigateButton.IsChecked = false;
            //ERNavigateButton.IsChecked = false;
        }

        private void WindowStateChanged(object sender, EventArgs e)
        {
            Helper.ReturnSettings();
            if (!Helper.settings.EnableMinimiseToToolbar)
                return;

            if(this.WindowState == WindowState.Minimized)
            {
                tbi.Visibility = Visibility.Visible;
                this.ShowInTaskbar = false;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                this.ShowInTaskbar = true;
                tbi.Visibility = Visibility.Collapsed;
            }
        }
        private void DoubleClickOpenWindow(object param)
        {
            this.WindowState = WindowState.Normal;
        }
    }
}