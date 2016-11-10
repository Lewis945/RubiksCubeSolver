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
    class Bootstrapper
    {
        static int speed = 100;
        static Mat currentFrame;
        //int startinPoint = 220;
        static int startinPoint = 40;
        static int i = 0;

        public static void Run(string path)
        {
            Trace.Listeners.Add(new TextWriterTraceListener($"../../Logs/log {DateTime.Now:H-mm-ss dd-MM-yyyy}.txt", "tracelog"));

            var calibration = new CameraCalibrationInfo(560.764656335266f, 562.763179958161f, 295.849138757436f, 255.022208986073f);

            var capture = new Capture(path);

            //var image1Gray = new Mat();
            //var image2Gray = new Mat();

            //CvInvoke.CvtColor(image1, image1Gray, ColorConversion.Rgb2Gray);
            //CvInvoke.CvtColor(image2, image2Gray, ColorConversion.Rgb2Gray);

            var tracker = new SimpleAdHocTracker(calibration);

            var imageViewer = new ImageViewer();

            new Task(() =>
            {
                do
                {
                    currentFrame = capture.QueryFrame();
                    if (currentFrame == null)
                        break;

                    if (i == startinPoint)
                    {
                        tracker.Process(currentFrame, true);
                    }
                    else if (i > startinPoint)
                    {
                        tracker.Process(currentFrame, false);
                    }

                    CvInvoke.PutText(currentFrame, i.ToString(), new Point(20, 20), FontFace.HersheyPlain, 2, new Emgu.CV.Structure.MCvScalar(100, 10, 100));

                    i++;

                    imageViewer.Image = currentFrame;

                    Thread.Sleep(1000 / 15);
                }
                while (true);

            }).Start();

            while (i < 75)
            {
            }
            Trace.Flush();
            //imageViewer.ShowDialog();
        }
    }
}
