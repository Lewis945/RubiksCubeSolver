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
            //RubiksCube.OpenCV.TestCase.AugmentedReality.Bootstrapper.Run("Images\\PyramidPatternTest.bmp", "Images\\PyramidPattern.jpg", AugmentedReality.SourceType.Image);
            RubiksCube.OpenCV.TestCase.AugmentedReality.Bootstrapper.Run("Videos\\napkins-video.mp4", "Images\\napkins-pattern.jpg", AugmentedReality.SourceType.Video);

            Console.ReadKey();
        }
    }
}
