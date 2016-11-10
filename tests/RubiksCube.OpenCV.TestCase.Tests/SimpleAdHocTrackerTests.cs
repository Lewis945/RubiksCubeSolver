using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using NUnit.Framework;
using RubiksCube.OpenCV.TestCase.AugmentedReality;
using RubiksCube.OpenCV.TestCase.PtamLikeApproach;

namespace RubiksCube.OpenCV.TestCase.Tests
{
    [TestFixture]
    public class SimpleAdHocTrackerTests
    {
        private readonly CameraCalibrationInfo _calibration;
        private readonly SimpleAdHocTracker _tracker;

        private static readonly int StartinPoint = 40;
        private static readonly int BootstrappedPoint = 95;

        public SimpleAdHocTrackerTests()
        {
            _calibration = new CameraCalibrationInfo(560.764656335266f, 562.763179958161f, 295.849138757436f, 255.022208986073f);
            _tracker = new SimpleAdHocTracker(_calibration);

            Trace.Listeners.Add(new TextWriterTraceListener($"C:/Users/zakharov/Documents/Repos/Mine/Rc/tests/RubiksCube.OpenCV.TestCase.Tests/Logs/log {DateTime.Now:H-mm-ss dd-MM-yyyy}.txt", "tracelog"));
        }

        [Test]
        public void Bootstrap_Track_Test()
        {
            var capture = new Capture(@"C:\Users\zakharov\Documents\Repos\Mine\Rc\src\RubiksCube.OpenCV.TestCase\Videos\cube2.avi");

            int i = 0;
            do
            {
                using (var currentFrame = capture.QueryFrame())
                {
                    if (currentFrame == null)
                        break;

                    if (i == StartinPoint)
                    {
                        _tracker.Process(currentFrame, true);
                    }
                    else if (i > StartinPoint)
                    {
                       _tracker.Process(currentFrame, false);

                        if (i == BootstrappedPoint)
                        {
                            CvInvoke.PutText(currentFrame, i.ToString(), new Point(20, 20), FontFace.HersheyPlain, 2,
                                new Emgu.CV.Structure.MCvScalar(100, 10, 100));

                            currentFrame.Save("output.jpg");

                            break;
                        }
                    }

                    i++;
                }
            }
            while (true);
        }
    }
}
