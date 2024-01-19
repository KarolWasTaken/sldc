using Microsoft.VisualBasic;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace sldc.Windows
{
    /// <summary>
    /// Interaction logic for StreamerWindow.xaml
    /// </summary>
    public partial class StreamerWindow : Window
    {
        public StreamerWindow()
        {
            InitializeComponent();
        }

        // removes the streamwer window from the store when it's being closed.
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // grabs current datacontext (the viewmodel)
            StreamerWindowViewModel swvm = ((FrameworkElement)sender).DataContext as StreamerWindowViewModel;
            swvm.StreamerWindowStore.streamerWindow = null;
        }
    }
}
