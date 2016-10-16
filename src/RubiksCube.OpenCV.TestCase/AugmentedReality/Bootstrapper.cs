using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;
using Emgu.CV.XFeatures2D;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace RubiksCube.OpenCV.TestCase.AugmentedReality
{
    public static class Bootstrapper
    {
        public static void Run(string path, string patternPath, SourceType type)
        {
            FeaturesUtils.Init();

            var calibration = new CameraCalibrationInfo(560.764656335266f, 562.763179958161f, 295.849138757436f, 255.022208986073f);

            var patternImage = CvInvoke.Imread(patternPath, Emgu.CV.CvEnum.LoadImageType.Unchanged);
            var patternDetector = new PatternDetector(patternImage);

            if (type == SourceType.Image)
            {
                var image = CvInvoke.Imread(path, Emgu.CV.CvEnum.LoadImageType.Unchanged);
                ShowWindow(image, patternImage, patternDetector, calibration);
            }
            else if (type == SourceType.Video)
            {
                var capture = new Capture(path);
                var image = capture.QueryFrame();
                ShowWindow(image, patternImage, patternDetector, calibration, capture);
            }
        }

        private static void ShowWindow(Mat img, Mat patternImage, PatternDetector patternDetector, CameraCalibrationInfo calibration, Capture capture = null)
        {
            double fps = capture.GetCaptureProperty(CapProp.Fps);
            using (var window = new GameWindow(calibration, img))
            {
                window.PatternDetector = patternDetector;
                window.Pattern = patternImage;
                window.Capture = capture;
                window.Run(fps);
            }
        }
    }
}
