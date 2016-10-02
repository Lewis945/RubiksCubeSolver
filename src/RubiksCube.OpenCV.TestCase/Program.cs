using Emgu.CV;
using Emgu.CV.UI;
using System;

namespace RubiksCube.OpenCV.TestCase
{
    /// <summary>
    /// http://www.emgu.com/wiki/index.php/Tutorial
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            RunForImage();
            //RunForVideo();

            Console.ReadKey();
        }

        private static void RunForImage()
        {
            //DrawMatches.Init();

            var img = CvInvoke.Imread("Images\\KQWtX4GlUa4.jpg", Emgu.CV.CvEnum.LoadImageType.Grayscale);

            //SiftExamples.Run(img);
            //SurfExamples.Run(img);
            //FastExamples.Run(img);
            //OrbExamples.Run(img);
            //FreakExamples.Run(img);
        }

        private static void RunForVideo()
        {
        }
    }
}
