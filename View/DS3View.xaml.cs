using sldc.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace sldc.View
{
    /// <summary>
    /// Interaction logic for DS3View.xaml
    /// </summary>
    public partial class DS3View : UserControl
    {
        public DS3View()
        {
            InitializeComponent();

            // Call the method asynchronously to allow for the delay after InitializeComponent
            // if it works, it works
            InitializeViewModelAsync();
        }

        private async Task InitializeViewModelAsync()
        {
            // Wait for 0.25 seconds to ensure DataContext is set
            await Task.Delay(1);

            var viewModel = (GameHookBaseViewModel)DataContext;
            viewModel.OnGameConnected += OnConnectedToGame;

            if(viewModel.IsConnectedToGame)
            {
                //var storyboard = (Storyboard)FindResource("InstantConnectedAnimation");
                //storyboard.Begin(this);
                OnConnectedToGame(true);
            }
        }
        public void OnConnectedToGame(bool status)
        {
            if (status) {
                var storyboard = (Storyboard)FindResource("ConnectedToGameAnimation");
                storyboard.Begin(this);
            }
            else
            {
                var storyboard = (Storyboard)FindResource("DisconnectedToGameAnimation");
                storyboard.Begin(this);
            }
        }
    }
}
