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
        private static int startinPoint = 40;
        private static int i = 0;

        public static void Run(string path, bool preprocess)
        {
            var calibration = new CameraCalibrationInfo(560.764656335266f, 562.763179958161f, 295.849138757436f, 255.022208986073f);
            var algorithm = new PtamLikeAlgorithm(calibration);

            var capture = new Capture(path);

            Mat image;
            for (int j = 0; j < startinPoint; j++)
                image = capture.QueryFrame();

            image = capture.QueryFrame();
            algorithm.Bootstrap(image);

            if (preprocess)
            {
                while (true)
                {
                    image = capture.QueryFrame();
                    var result = algorithm.BootstrapTrack(image);
                    if (result)
                        break;
                }
            }

            ShowWindow(image, calibration, algorithm, preprocess, capture);
        }

        private static void ShowWindow(Mat img, CameraCalibrationInfo calibration, PtamLikeAlgorithm algorithm, bool preprocess, Capture capture = null)
        {
            double fps = capture?.GetCaptureProperty(CapProp.Fps) ?? 30;
            using (var window = new PtamWindow(calibration, img))
            {
                if (!preprocess)
                    window.Capture = capture;
                window.Algorithm = algorithm;
                window.Run(fps);
            }
        }
    }
}
