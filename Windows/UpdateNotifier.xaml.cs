using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for UpdateNotifier.xaml
    /// </summary>
    public partial class UpdateNotifier : Window
    {
        public UpdateNotifier()
        {
            InitializeComponent();
        }

        private void UpdateNowClicked(object sender, RoutedEventArgs e)
        {
            OpenUrlInBrowser("https://github.com/KarolWasTaken/sldc/releases/latest");
            this.Close();
        }

        private void DontShowAgainClicked(object sender, RoutedEventArgs e)
        {
            Helper.ReturnSettings();
            Helper.settings.DontShowAgain = true;
            Helper.UpdateSettings();
            this.Close();
        }

        static void OpenUrlInBrowser(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true // Ensures the default browser is used
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error");
            }
        }
    }
}
