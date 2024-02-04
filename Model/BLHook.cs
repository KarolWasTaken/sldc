using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AForge.Imaging;
using AForge.Imaging.Filters;
using Size = System.Drawing.Size;

namespace sldc.Model
{
    public class BLHook : ImageSimilarityBase
    {
        private const int BLOODBORNE_BEGIN_SEARCH_1080P_X = 589;
        private const int BLOODBORNE_BEGIN_SEARCH_1080P_Y = 179;
        private const int BLOODBORNE_CAPTURE_SEARCH_SIZE_X = 747;
        private const int BLOODBORNE_CAPTURE_SEARCH_SIZE_Y = 156;

        private const string REFERENC_IMAGE_PATH = "Resources/ImageSimilarity/Bloodborne/bloodborne_death.png";
        private const string MASK_IMAGE_PATH = "Resources/ImageSimilarity/Bloodborne/bloodborne_death_mask.png";
        public override bool ScanRemotePlay()
        {
            Bitmap screenCapture = CaptureRemotePlay(
                    BLOODBORNE_BEGIN_SEARCH_1080P_X,
                    BLOODBORNE_BEGIN_SEARCH_1080P_Y,
                    BLOODBORNE_CAPTURE_SEARCH_SIZE_X,
                    BLOODBORNE_CAPTURE_SEARCH_SIZE_Y
                );

            Bitmap referenceImage = new Bitmap(REFERENC_IMAGE_PATH);

            Bitmap imageMask = new Bitmap(MASK_IMAGE_PATH);

            bool areImagesSimilar = CompareImagesWithMask(screenCapture, referenceImage, imageMask, 0.982f); //CompareImages(screenCapture, referenceImage);
            return areImagesSimilar;
        }

    }
}
