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

namespace RubiksCube.OpenCV.TestCase
{
    public static class OrbExamples
    {
        public static void Run(Mat img)
        {
            Stopwatch watch;

            var modelKeyPoints = new VectorOfKeyPoint();

            var result = new UMat();

            using (UMat uModelImage = img.ToUMat(AccessType.Read))
            {
                watch = Stopwatch.StartNew();

                ORBDetector orbCPU = new ORBDetector();
                //extract features from the object image
                UMat modelDescriptors = new UMat();
                orbCPU.DetectRaw(uModelImage, modelKeyPoints);

                Features2DToolbox.DrawKeypoints(img, modelKeyPoints, result, new Bgr(Color.Red), Features2DToolbox.KeypointDrawType.NotDrawSinglePoints);

                watch.Stop();
            }

            long detectTimeMs = watch.ElapsedMilliseconds;
            float detectTimeS = detectTimeMs / 1000f;

            ImageViewer.Show(result, $"ORB Example ({detectTimeS} s.)");
        }
    }
}
