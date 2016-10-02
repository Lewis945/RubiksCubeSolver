using OpenCvSharp;
using System;

namespace RubiksCube.OpenCV.Utils
{
    public class TresholdingUtil
    {
        public static Mat AdaptiveTresholding(Mat greyImage, int c = 2, double perc = 0.001)
        {
            //var spix = (int)Math.Sqrt(greyImage.Width * greyImage.Height * perc);
            //spix = spix % 2 == 1 ? spix : spix + 1;
            //spix = spix < 3 ? 3 : spix;

            var thresh = greyImage.AdaptiveThreshold(255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, 11, c);
            return thresh;
        }

        public static Mat Tresholding(Mat greyImage)
        {
            var thresh = greyImage.Threshold(100, 255, ThresholdTypes.Binary);
            return thresh;
        }
    }
}
