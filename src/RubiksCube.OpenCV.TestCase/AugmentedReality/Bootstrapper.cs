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
        private static CameraCalibrationInfo _calibration;

        public static void Run(string path, string patternPath, SourceType type)
        {
            //using (var a = new GameWindow())
            //{
            //    a.Run(30);
            //}

            var viewer = new ImageViewer();
            viewer.Text = path;

            _calibration = new CameraCalibrationInfo(560.764656335266f, 562.763179958161f, 295.849138757436f, 255.022208986073f);

            var patternImage = CvInvoke.Imread(patternPath, Emgu.CV.CvEnum.LoadImageType.Unchanged);
            var patternDetector = new PatternDetector(patternImage);

            if (type == SourceType.Image)
            {
                ProcessImage(path, patternImage, patternDetector, viewer);
            }
            else if (type == SourceType.Video)
            {
                ProcessVideo(path, patternImage, viewer);
            }

            viewer.ShowDialog();
        }

        private static void ProcessImage(string path, Mat patternImage, PatternDetector patternDetector, ImageViewer viewer)
        {
            var img = CvInvoke.Imread(path, Emgu.CV.CvEnum.LoadImageType.Unchanged);
            int sleepTime = (int)Math.Round(1000 / 6f);

            Task.Run(() =>
            {
                bool run = true;
                while (run)
                {
                    if (img != null)
                        viewer.Image = ProcessFrame(img, patternImage, patternDetector);
                    else
                        run = false;

                    Thread.Sleep(sleepTime);
                }
            });
        }

        private static void ProcessVideo(string path, Mat patternImage, ImageViewer viewer)
        {
            var capture = new Capture(path);
            int sleepTime = (int)Math.Round(1000 / 6f);

            Task.Run(() =>
            {
                bool run = true;
                while (run)
                {
                    var frame = capture.QueryFrame();
                    if (frame != null)
                        viewer.Image = ProcessFrame(frame, patternImage, null);
                    else
                        capture = new Capture(path);

                    Thread.Sleep(sleepTime);
                }
            });
        }

        private static Mat ProcessFrame(Mat frame, Mat pattern, PatternDetector patternDetector)
        {
            //long time;
            //frame = DrawMatches.Draw(pattern, frame, out time);

            var img = frame.Clone();

            patternDetector.FindPattern(img);
            patternDetector.PatternTrackingInfo.computePose(patternDetector.Pattern, _calibration);

            return img;
        }
    }
}
