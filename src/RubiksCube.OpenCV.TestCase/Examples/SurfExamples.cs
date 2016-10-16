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

namespace RubiksCube.OpenCV.TestCase.Examples
{
    /// <summary>
    /// http://www.emgu.com/wiki/index.php/SURF_feature_detector_in_CSharp
    /// </summary>
    public static class SurfExamples
    {
        public static UMat Run(Mat img)
        {
            double hessianThresh = 500;

            var modelKeyPoints = new VectorOfKeyPoint();
            var result = new UMat();

            using (UMat uModelImage = img.ToUMat(AccessType.Read))
            {
                SURF surfCPU = new SURF(hessianThresh);
                //extract features from the object image
                UMat modelDescriptors = new UMat();
                //surfCPU.DetectAndCompute(uModelImage, null, modelKeyPoints, modelDescriptors, false);
                surfCPU.DetectRaw(uModelImage, modelKeyPoints);

                Features2DToolbox.DrawKeypoints(img, modelKeyPoints, result, new Bgr(Color.Red), Features2DToolbox.KeypointDrawType.NotDrawSinglePoints);
            }

            return result;
        }
    }
}
