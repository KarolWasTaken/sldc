using AForge.Video;
using AForge.Video.DirectShow;
using sldc.Model;
using sldc.ViewModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Brush = System.Windows.Media.Brush;
using FontFamily = System.Windows.Media.FontFamily;

namespace sldc.View
{
    /// <summary>
    /// Interaction logic for SelectCaptureDeviceView.xaml
    /// </summary>
    public partial class SelectCaptureDeviceView : UserControl
    {
        private static SelectCaptureDeviceViewModel _scdvm;
        private static int frameCount = 0;
        public SelectCaptureDeviceView()
        {
            InitializeComponent();
            this.Loaded += SelectCaptureDeviceView_Loaded;
        }

        private void SelectCaptureDeviceView_Loaded(object sender, RoutedEventArgs e)
        {
            // grab vm
            _scdvm = (SelectCaptureDeviceViewModel)this.DataContext;
            // just need an instance of ImageSimilarityBase to use DoesProcessExist
            BLHook isb = new BLHook();
            // check if remote play is on
            bool IsRemotePlayOn = isb.DoesProcessExist("RemotePlay");
            // add remote play
            if(IsRemotePlayOn)
            {
                var test = new Bitmap("Resources/PS-Remote-Logo.png");
                AddStackPanelToGrid("PS Remote", "PS Remote", new Bitmap("Resources/PS-Remote-Logo.png"), Stretch.Uniform);
            }

            // add capture cards
            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count > 0)
            {
                foreach (FilterInfo videoDevice in videoDevices)
                {

                    // Create VideoCaptureDevice using the current FilterInfo
                    VideoCaptureDevice videoCaptureDevice = new VideoCaptureDevice(videoDevice.MonikerString);

                    // Set NewFrame event handler to capture the first frame
                    videoCaptureDevice.NewFrame += async (sender, eventArgs) =>
                    {
                        // Save the first frame as an image asynchronously
                        bool framecaptured = await SaveSnapshotAsync();
                        if(framecaptured) 
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                this.AddStackPanelToGrid(videoDevice.Name, videoDevice.MonikerString, (Bitmap)eventArgs.Frame.Clone(), Stretch.Uniform);
                                frameCount = 0;
                                //Bitmap test = (Bitmap)eventArgs.Frame.Clone();
                                //test.Save("Testing.png");
                            });
                            videoCaptureDevice.SignalToStop();
                        }
                    };
                    // Start the video source
                    videoCaptureDevice.Start();
                }
            }
        }
        static async Task<bool> SaveSnapshotAsync()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                frameCount += 1;
            });

            // first and second frame always returns black for some reason 
            if (frameCount >= 4)
            {
                return true;
            }
            return false;
        }
        private void AddStackPanelToGrid(string captureName, string commandParameter, Bitmap image, Stretch stretchMode)
        {
            Grid grid = new Grid();
            // Create StackPanel
            StackPanel stackPanel = new StackPanel();

            // Create Border
            Border border = new Border
            {
                Margin = new Thickness(0, 0, 0, 15),
                Width = 240,
                Height = 100,
                Style = (Style)FindResource("CardBorderStyle")
            };
            border.CornerRadius = new CornerRadius(5);

            // Create ImageBrush
            ImageBrush imageBrush = new ImageBrush
            {
                Stretch = stretchMode
            };

            // Convert Bitmap to BitmapImage  
            BitmapImage bitmapImage = ConvertBitmapToBitmapImage(image);

            // Set ImageSource to ImageBrush
            imageBrush.ImageSource = bitmapImage;
            // Set ImageBrush as Background for the Border
            border.Background = imageBrush;

            // Add Border to StackPanel
            stackPanel.Children.Add(border);

            // Create TextBlock
            TextBlock textBlock = new TextBlock
            {
                Text = captureName,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = (Brush)FindResource("TextColour"), // Make sure to replace "TextColour" with the actual resource key
                FontFamily = (FontFamily)FindResource("Inter"), // Make sure to replace "Inter" with the actual resource key
                FontWeight = FontWeights.Regular,
                FontSize = 20,
                Margin = new Thickness(0, 0, 0, 10)
            };

            // Add TextBlock to StackPanel
            stackPanel.Children.Add(textBlock);
            // create select button
            Button selectButton = new Button
            {
                Opacity = 0,
                Command = new RelayCommand(SelectPlaythrough),
                CommandParameter = commandParameter
            };
            grid.Children.Add(stackPanel);
            grid.Children.Add(selectButton);
            // Add StackPanel to your Grid
            CaptureDeviceList.Children.Add(grid); // Make sure to replace "YourGrid" with the actual name of your Grid
        }
        public void SelectPlaythrough(object parameter)
        {
            _scdvm.SelectCaptureDevice(parameter.ToString());
        }
        private BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Save the bitmap to the memory stream
                bitmap.Save(memoryStream, ImageFormat.Png);

                // Create a new BitmapImage
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();

                // Set the stream source to the memory stream
                bitmapImage.StreamSource = new MemoryStream(memoryStream.ToArray());

                // End initialization
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }
    }
}
