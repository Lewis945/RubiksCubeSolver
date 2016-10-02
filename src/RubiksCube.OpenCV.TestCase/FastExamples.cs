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
    /// <summary>
    /// http://www.emgu.com/wiki/index.php/FAST_feature_detector_in_CSharp
    /// </summary>
    public static class FastExamples
    {
        public static UMat Run(Mat img)
        {
            var modelKeyPoints = new VectorOfKeyPoint();
            var result = new UMat();

            using (UMat uModelImage = img.ToUMat(AccessType.Read))
            {
                FastDetector fastCPU = new FastDetector(10, true);
                UMat modelDescriptors = new UMat();
                fastCPU.DetectRaw(uModelImage, modelKeyPoints);
                Features2DToolbox.DrawKeypoints(img, modelKeyPoints, result, new Bgr(Color.Red), Features2DToolbox.KeypointDrawType.NotDrawSinglePoints);
            }

            return result;
        }
    }
}
