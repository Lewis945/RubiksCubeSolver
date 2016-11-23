using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using NUnit.Framework;
using RubiksCube.OpenCV.TestCase.AugmentedReality;
using RubiksCube.OpenCV.TestCase.PtamLikeApproach;

namespace RubiksCube.OpenCV.TestCase.Tests
{
    [TestFixture]
    public class PtamLikeAlgorithmTests
    {
        private readonly CameraCalibrationInfo _calibration;
        private readonly PtamLikeAlgorithm _algorithm;

        private static readonly string TestCaseProjectPath = @"C:\Users\zakharov\Documents\Repos\Mine\Rc\src\RubiksCube.OpenCV.TestCase";
        //private static readonly string TestCaseProjectPath = @"D:\Projects\RubiksCube\src\RubiksCube.OpenCV.TestCase";
        private static readonly string TestCaseTestProjectPath = "C:/Users/zakharov/Documents/Repos/Mine/Rc/tests/RubiksCube.OpenCV.TestCase.Tests";
        //private static readonly string TestCaseTestProjectPath = @"D:\Projects\RubiksCube\tests\RubiksCube.OpenCV.TestCase.Tests";

        public PtamLikeAlgorithmTests()
        {
            _calibration = new CameraCalibrationInfo(560.764656335266f, 562.763179958161f, 295.849138757436f, 255.022208986073f);
            _algorithm = new PtamLikeAlgorithm(_calibration);
        }

        [Test]
        public void Bootstrap_Track_Test()
        {
            var capture = new Capture($@"{TestCaseProjectPath}\Videos\cube2.avi");
            for (int i = 0; i < 40; i++)
            {
                capture.QueryFrame();
            }

            var img = capture.QueryFrame();
            _algorithm.Bootstrap(img);

            for (int i = 41; i <= 95; i++)
            {
                img = capture.QueryFrame();
                var result = _algorithm.BootstrapTrack(img);
                if (result)
                {


                    //File.WriteAllLines($@"{TestCaseTestProjectPath}\cpluspluslogs\{item.Key}.txt", item.Value);
                }
            }
        }
    }
}
