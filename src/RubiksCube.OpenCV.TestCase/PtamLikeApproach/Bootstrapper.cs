using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.UI;
using Emgu.CV.Util;
using RubiksCube.OpenCV.TestCase.AugmentedReality;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RubiksCube.OpenCV.TestCase.PtamLikeApproach
{
    public class Bootstrapper
    {
        public static void Run()
        {
            Run(null);
        }

        public static void Run(string path)
        {
            var calibration = new CameraCalibrationInfo(560.764656335266f, 562.763179958161f, 295.849138757436f, 255.022208986073f);
            var algorithm = new PtamLikeAlgorithm(calibration);

            var capture = path != null ? new Capture(path) : new Capture();
            var image = capture.QueryFrame();

            ShowWindow(image, calibration, algorithm, capture);
        }

        private static void ShowWindow(Mat img, CameraCalibrationInfo calibration, PtamLikeAlgorithm algorithm, Capture capture = null)
        {
            double fps = capture?.GetCaptureProperty(CapProp.Fps) ?? 30;
            using (var window = new PtamWindow(calibration, img))
            {
                window.Capture = capture;
                if (capture != null)
                {
                    for (int i = 0; i < 40; i++)
                        capture.QueryFrame();
                }
                window.Algorithm = algorithm;
                window.Run(fps);
            }
        }
    }
}
