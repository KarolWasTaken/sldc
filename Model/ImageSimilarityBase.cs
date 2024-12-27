using AForge.Imaging;
using AForge.Video.DirectShow;
using AForge.Video;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Size = System.Drawing.Size;
using System.Security.Policy;
using System.DirectoryServices.ActiveDirectory;

namespace sldc.Model
{
    public abstract class ImageSimilarityBase
    {
        internal string _processName = "";
        public bool IsSearchingForDeaths = false;
        public Bitmap? CapturedFrame;
        public event Action OnDeath;
        
        private int _beginSearchXCoord;
        private int _beginSearchYCoord;
        private int _captureSizeX;
        private int _captureSizeY;

        public ImageSimilarityBase(int beginSearchXCoord, int beginSearchYCoord, int captureSizeX, int captureSizeY)
        {
            _beginSearchXCoord = beginSearchXCoord;
            _beginSearchYCoord = beginSearchYCoord;
            _captureSizeX = captureSizeX;
            _captureSizeY = captureSizeY;
        }

        public bool DoesProcessExist(string processName)
        {
            // Find the process by name
            Process[] processes = Process.GetProcessesByName(processName);

            if (processes.Length == 0)
            {
                // Handle the case where the process is not found
                return false;
            }
            _processName = processName;
            return true;
        }
        public async Task AsyncScanIndefinite(int ScanMillisecondDelay, int deathCaptureMillisecondDelay)
        {
            IsSearchingForDeaths = true;
            while (IsSearchingForDeaths)
            {

                bool deathfound = ScanRemotePlay();
                if (deathfound)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        OnDeath.Invoke();
                    });
                    // wait 2-3 seconds so we dont count an additional death
                    await Task.Delay(deathCaptureMillisecondDelay);
                }
                // delay to not throttle cpu
                await Task.Delay(ScanMillisecondDelay);
            }
        }
        /// <summary>
        /// Scan similarity. Make use of <see cref="CaptureGame(int, int, int, int)"/>
        /// </summary>
        /// <returns></returns>
        public abstract bool ScanRemotePlay();
        public abstract bool ScanCaptureCard();
        internal bool CompareImagesWithMask(Bitmap capturedImage, Bitmap referenceImage, Bitmap mask, float similarityThreshold = 0.92f)
        {
            // Check if the images have the same size
            if (capturedImage.Width != referenceImage.Width || capturedImage.Height != referenceImage.Height || capturedImage.Width != mask.Width || capturedImage.Height != mask.Height)
            {
                throw new ArgumentException("Images and mask must have the same size.");
            }

            // Apply mask to capturedImage
            capturedImage = ApplyMask(capturedImage, mask);
            // Convert images to the same pixel format (24bpp RGB)
            referenceImage = ConvertImageTo24bpp(referenceImage);
            capturedImage = ConvertImageTo24bpp(capturedImage);

            // Create an ExhaustiveTemplateMatching instance
            ExhaustiveTemplateMatching templateMatching = new ExhaustiveTemplateMatching(similarityThreshold);

            // Find similarities between the captured and reference images
            TemplateMatch[] matches = templateMatching.ProcessImage(capturedImage, referenceImage);

            // Check if a match is found
            bool isSimilar = matches.Length > 0;

            return isSimilar;
        }
        private Bitmap ApplyMask(Bitmap image, Bitmap mask)
        {
            Bitmap result = new Bitmap(image.Width, image.Height);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixelColor = image.GetPixel(x, y);
                    Color maskColor = mask.GetPixel(x, y);

                    // Apply the mask to the pixel
                    Color newColor = Color.FromArgb(pixelColor.A, pixelColor.R * maskColor.R / 255, pixelColor.G * maskColor.G / 255, pixelColor.B * maskColor.B / 255);

                    result.SetPixel(x, y, newColor);
                }
            }

            return result;
        }
        private Bitmap ConvertImageTo24bpp(Bitmap originalImage)
        {
            // Convert the image to 24bpp RGB format
            return AForge.Imaging.Image.Clone(originalImage, PixelFormat.Format24bppRgb);
        }
        internal virtual Bitmap CaptureRemotePlay()
        {
            // Find the process by name
            Process[] processes = Process.GetProcessesByName("RemotePlay");

            if (processes.Length == 0)
            {
                // Handle the case where the process is not found
                return null;
            }

            // Get the main window handle (HWND) of the process
            IntPtr mainWindowHandle = processes[0].MainWindowHandle;

            // Get the rectangle of the window
            RECT windowRect;
            GetWindowRect(mainWindowHandle, out windowRect);
            int width = windowRect.Right - windowRect.Left;
            int height = windowRect.Bottom - windowRect.Top;


            // create multipliers to find where to start capturing
            float xRatio = (float)width / (float)1920;
            float yRatio = (float)height / (float)1080;

            // get search offsets and search size box (grabbing region of interest)
            int windowStartSearchOffsetX = (int)Math.Ceiling((float)_beginSearchXCoord * xRatio);
            int windowStartSearchOffsetY = (int)Math.Ceiling((float)_beginSearchYCoord * yRatio);
            int windowSearchSizeX = (int)Math.Ceiling(_captureSizeX * xRatio);
            int windowSearchSizeY = (int)Math.Ceiling(_captureSizeY * yRatio);

            // Define the capture region
            Rectangle captureRegion = new Rectangle(
                windowRect.Left + windowStartSearchOffsetX,
                windowRect.Top + windowStartSearchOffsetY + 22,
                windowSearchSizeX,
                windowSearchSizeY);



            //Create a bitmap to store the screen capture
            Bitmap screenCapture = new Bitmap(captureRegion.Width, captureRegion.Height);

            using (Graphics g = Graphics.FromImage(screenCapture))
            {
                // Copy the specified region from the screen
                g.CopyFromScreen(captureRegion.X,
                                 captureRegion.Y,
                                 0, 0,
                                 captureRegion.Size,
                                 CopyPixelOperation.SourceCopy);
            }
            //screenCapture.Save("AAAbloodborneCaptureText.png");
            Bitmap screenCaptureResized = new Bitmap(screenCapture, new Size(_captureSizeX, _captureSizeY));
            //screenCaptureResized.Save("AAAbloodborneCaptureText_Resized.jpg");
            screenCapture = screenCaptureResized;
            return screenCapture;
        }

        // Import the necessary functions for getting window rectangle
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        // Define the RECT structure
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private VideoCaptureDevice videoSource;
        public void BeginCapturingCard(string deviceMoniker)
        {
            videoSource = new VideoCaptureDevice(deviceMoniker);
            videoSource.DesiredFrameRate = 1;
            videoSource.NewFrame += VideoSource_NewFrame;
            videoSource.Start();
            //Task frameLimiter = Task.Run(() => IndefiniteFrameLimiter(150));
        }
        private bool canUpdateFrames = false;
        private int framesRecieved = 0;
        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // frame limiter
            //if (!canUpdateFrames)
            //    return;
            //canUpdateFrames = false;
            //if (videoSource.FramesReceived % 5 != 0)
            //    return;
            //framesRecieved++;
            //if(framesRecieved % 60 != 0)
            //{
            //    return;
            //}

            Bitmap screenCapture2 = (Bitmap)eventArgs.Frame.Clone();
            screenCapture2.Save("CAPTURECARD.png");
            if (videoSource.VideoResolution == null)
                return;
            int width = videoSource.VideoResolution.FrameSize.Width;
            int height = videoSource.VideoResolution.FrameSize.Height;

            // create multipliers to find where to start capturing
            float xRatio = (float)width / (float)1920;
            float yRatio = (float)height / (float)1080;

            // get search offsets and search size box (grabbing region of interest)
            int windowSearchSizeX = (int)Math.Ceiling(_captureSizeX * xRatio);
            int windowSearchSizeY = (int)Math.Ceiling(_captureSizeY * yRatio);

            // Define the capture region
            Rectangle captureRegion = new Rectangle(
                0,
                0,
                windowSearchSizeX,
                windowSearchSizeY);



            //Create a bitmap to store the screen capture
            Bitmap screenCapture = (Bitmap)eventArgs.Frame.Clone();
            Bitmap screenCaptureResized = new Bitmap(screenCapture, new Size(_captureSizeX, _captureSizeY));
            screenCaptureResized.Save("capturecard_Resized.jpg");

            CapturedFrame = screenCaptureResized;
        }

        private async Task IndefiniteFrameLimiter(int limitMiliSeconds)
        {
            while (true)
            {
                bool localBool;
                Application.Current.Dispatcher.Invoke(() => { localBool = canUpdateFrames; });
                if (!canUpdateFrames)
                {
                    await Task.Delay(limitMiliSeconds);
                    Application.Current.Dispatcher.Invoke(() => { canUpdateFrames = true; });
                }
            }
        }
    }
}
