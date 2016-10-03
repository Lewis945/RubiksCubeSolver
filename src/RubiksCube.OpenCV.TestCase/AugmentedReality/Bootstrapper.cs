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
        public static void Run(string path, SourceType type)
        {
            var viewer = new ImageViewer();
            viewer.Text = path;

            if (type == SourceType.Image)
            {
                ProcessImage(path, viewer);
            }
            else if (type == SourceType.Video)
            {
                ProcessVideo(path, viewer);
            }

            viewer.ShowDialog();
        }

        private static void ProcessImage(string path, ImageViewer viewer)
        {
            var img = CvInvoke.Imread(path, Emgu.CV.CvEnum.LoadImageType.Grayscale);
            int sleepTime = (int)Math.Round(1000 / 6f);

            Task.Run(() =>
            {
                bool run = true;
                while (run)
                {
                    if (img != null)
                        viewer.Image = ProcessFrame(img);
                    else
                        run = false;

                    Thread.Sleep(sleepTime);
                }
            });
        }

        private static void ProcessVideo(string path, ImageViewer viewer)
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
                        viewer.Image = ProcessFrame(frame);
                    else
                        capture = new Capture(path);

                    Thread.Sleep(sleepTime);
                }
            });
        }

        private static Mat ProcessFrame(Mat frame)
        {
            return frame;
        }
    }
}
