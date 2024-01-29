﻿using AForge.Imaging;
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

namespace sldc.Model
{
    public abstract class ImageSimilarityBase
    {
        internal string _processName = "";
        public bool IsSearchingForDeaths = false;
        public event Action OnDeath;
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

                bool deathfound = Scan();
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
        public abstract bool Scan();

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
        internal virtual Bitmap CaptureGame(
            int beginSearchXCoord,
            int beginSearchYCoord,
            int captureSizeX,
            int captureSizeY)
        {
            // Find the process by name
            Process[] processes = Process.GetProcessesByName(_processName);

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
            int windowStartSearchOffsetX = (int)Math.Ceiling((float)beginSearchXCoord * xRatio);
            int windowStartSearchOffsetY = (int)Math.Ceiling((float)beginSearchYCoord * yRatio);
            int windowSearchSizeX = (int)Math.Ceiling(captureSizeX * xRatio);
            int windowSearchSizeY = (int)Math.Ceiling(captureSizeY * yRatio);

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
            Bitmap screenCaptureResized = new Bitmap(screenCapture, new Size(captureSizeX, captureSizeY));
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
    }
}
