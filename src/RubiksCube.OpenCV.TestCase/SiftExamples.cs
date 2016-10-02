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
    public static class SiftExamples
    {
        public static void Run(Mat img)
        {
            Stopwatch watch;

            var modelKeyPoints = new VectorOfKeyPoint();

            var result = new UMat();

            using (UMat uModelImage = img.ToUMat(AccessType.Read))
            {
                watch = Stopwatch.StartNew();

                SIFT siftfCPU = new SIFT();
                //extract features from the object image
                UMat modelDescriptors = new UMat();
                //surfCPU.DetectAndCompute(uModelImage, null, modelKeyPoints, modelDescriptors, false);
                siftfCPU.DetectRaw(uModelImage, modelKeyPoints);

                Features2DToolbox.DrawKeypoints(img, modelKeyPoints, result, new Bgr(Color.Red), Features2DToolbox.KeypointDrawType.NotDrawSinglePoints);

                watch.Stop();
            }

            long detectTimeMs = watch.ElapsedMilliseconds;
            float detectTimeS = detectTimeMs / 1000f;

            ImageViewer.Show(result, $"SIFT Example ({detectTimeS} s.)");
        }
    }
}
