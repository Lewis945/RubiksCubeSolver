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
            #region AugmentedReality

            //RubiksCube.OpenCV.TestCase.AugmentedReality.Bootstrapper.Run("Images\\PyramidPatternTest.bmp", "Images\\PyramidPattern.jpg", AugmentedReality.SourceType.Image);
            //RubiksCube.OpenCV.TestCase.AugmentedReality.Bootstrapper.Run("Videos\\napkins-video.mp4", "Images\\napkins-pattern.jpg", AugmentedReality.SourceType.Video);

            #endregion

            #region PtamLikeApproach

            RubiksCube.OpenCV.TestCase.PtamLikeApproach.Bootstrapper.Run("Videos\\cube2.avi");
            //RubiksCube.OpenCV.TestCase.PtamLikeApproach.Bootstrapper.Run("Videos\\rubik1.avi");
            //RubiksCube.OpenCV.TestCase.PtamLikeApproach.Bootstrapper.Run();

            #endregion

            //Utilities.WriteVideo("Videos\\rubik.avi");
        }
    }
}
