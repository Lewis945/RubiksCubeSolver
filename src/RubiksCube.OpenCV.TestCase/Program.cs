using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;
using RubiksCube.OpenCV.TestCase.Examples;
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
        [STAThread]
        public static void Main(string[] args)
        {
            //RubiksCube.OpenCV.TestCase.AugmentedReality.Bootstrapper.Run("Images\\PyramidPatternTest.bmp", "Images\\PyramidPattern.jpg", AugmentedReality.SourceType.Image);
            //RubiksCube.OpenCV.TestCase.AugmentedReality.Bootstrapper.Run("Videos\\napkins-video.mp4", "Images\\napkins-pattern.jpg", AugmentedReality.SourceType.Video);

            //RubiksCube.OpenCV.TestCase.Kalman.Bootstrapper.Run();

            RubiksCube.OpenCV.TestCase.PtamLikeApproach.Bootstrapper.Run("Videos\\cube2.avi");

            //UMat image;
            //image = SiftExamples.Run(new Mat("Images\\PyramidPatternTest.bmp", LoadImageType.Grayscale));
            //image = FastExamples.Run(new Mat("Images\\PyramidPatternTest.bmp", LoadImageType.Grayscale));
            //image = FreakExamples.Run(new Mat("Images\\PyramidPatternTest.bmp", LoadImageType.Grayscale));
            //image = OrbExamples.Run(new Mat("Images\\PyramidPatternTest.bmp", LoadImageType.Grayscale));
            //image = SurfExamples.Run(new Mat("Images\\PyramidPatternTest.bmp", LoadImageType.Grayscale));
            //ImageViewer.Show(image, "Image");
        }
    }
}
