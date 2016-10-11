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

namespace RubiksCube.OpenCV.TestCase
{
    /// <summary>
    /// http://www.emgu.com/wiki/index.php/Tutorial
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            //DrawMatches.Init();

            //RunForImage();
            //RunForVideo();

            RubiksCube.OpenCV.TestCase.AugmentedReality.Bootstrapper.Run("Images\\PyramidPatternTest.bmp", "Images\\PyramidPattern.jpg", AugmentedReality.SourceType.Image);
            //Bootstrapper.Run("Videos/cube.avi", SourceType.Video);

            Console.ReadKey();
        }

        private static void RunForImage()
        {
            var img = CvInvoke.Imread("Images\\KQWtX4GlUa4.jpg", Emgu.CV.CvEnum.LoadImageType.Grayscale);

            Stopwatch watch;
            watch = Stopwatch.StartNew();

            UMat result;

            result = Examples.SiftExamples.Run(img);
            //result = SurfExamples.Run(img);
            //result = FastExamples.Run(img);
            //result = OrbExamples.Run(img);
            //result = FreakExamples.Run(img);

            watch.Stop();

            long detectTimeMs = watch.ElapsedMilliseconds;
            float detectTimeS = detectTimeMs / 1000f;

            ImageViewer.Show(result, $"SURF Example ({detectTimeS} s.)");
        }

        private static void RunForVideo()
        {
            string fileName = "Videos/cube.avi";

            var viewer = new ImageViewer();
            var capture = new Capture(fileName);

            var fps = capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameCount);
            int sleepTime = (int)Math.Round(1000 / 12f);

            viewer.Text = fileName;

            Task.Run(() =>
            {
                bool run = true;
                while (run)
                {
                    var frame = capture.QueryFrame();
                    if (frame != null)
                    {
                        var result = Examples.FastExamples.Run(frame);
                        viewer.Image = result;
                    }
                    else
                    {
                        capture = new Capture(fileName);
                    }

                    Thread.Sleep(sleepTime);
                }
            });

            viewer.ShowDialog();
        }
    }
}
