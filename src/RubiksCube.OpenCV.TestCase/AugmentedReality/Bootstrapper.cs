using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace RubiksCube.OpenCV.TestCase.AugmentedReality
{
    public static class Bootstrapper
    {
        public static void Run(string path, string patternPath, SourceType type)
        {
            //using (var a = new GameWindow())
            //{
            //    a.Run(30);
            //}

            var viewer = new ImageViewer();
            viewer.Text = path;

            // Change this calibration to yours:
            var calibration = new CameraCalibrationInfo(526.58037684199849f, 524.65577209994706f, 318.41744018680112f, 202.96659047014398f);

            var patternImage = CvInvoke.Imread(patternPath, Emgu.CV.CvEnum.LoadImageType.Unchanged);

            if (type == SourceType.Image)
            {
                ProcessImage(path, patternImage, viewer);
            }
            else if (type == SourceType.Video)
            {
                ProcessVideo(path, patternImage, viewer);
            }

            viewer.ShowDialog();
        }

        private static void ProcessImage(string path, Mat patternImage, ImageViewer viewer)
        {
            var img = CvInvoke.Imread(path, Emgu.CV.CvEnum.LoadImageType.Grayscale);
            int sleepTime = (int)Math.Round(1000 / 6f);

            Task.Run(() =>
            {
                bool run = true;
                while (run)
                {
                    if (img != null)
                        viewer.Image = ProcessFrame(img, patternImage);
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
                        viewer.Image = ProcessFrame(frame, patternImage);
                    else
                        capture = new Capture(path);

                    Thread.Sleep(sleepTime);
                }
            });
        }

        private static Mat ProcessFrame(Mat frame, Mat pattern)
        {
            long time;
            frame = DrawMatches.Draw(pattern, frame, out time);
            return frame;
        }
    }
}
