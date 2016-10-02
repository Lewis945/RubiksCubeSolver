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
        public static UMat Run(Mat img)
        {
            var modelKeyPoints = new VectorOfKeyPoint();
            var result = new UMat();

            using (UMat uModelImage = img.ToUMat(AccessType.Read))
            {
                SIFT siftfCPU = new SIFT();
                UMat modelDescriptors = new UMat();
                siftfCPU.DetectRaw(uModelImage, modelKeyPoints);
                Features2DToolbox.DrawKeypoints(img, modelKeyPoints, result, new Bgr(Color.Red), Features2DToolbox.KeypointDrawType.NotDrawSinglePoints);
            }

            return result;
        }
    }
}
