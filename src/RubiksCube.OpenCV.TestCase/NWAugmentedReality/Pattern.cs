using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;
using RubiksCube.OpenCV.TestCase.NWAugmentedReality;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace RubiksCube.OpenCV.TestCase.NWAugmentedReality
{
    public class Pattern
    {
        #region Fields

        public Size size;
        public Mat frame;
        public Mat grayImg;

        public VectorOfKeyPoint keypoints;
        public Mat descriptors;

        public VectorOfPoint points2d;
        public VectorOfPoint3D32F points3d;

        #endregion

        public Pattern()
        {
            frame = new Mat();
            grayImg = new Mat();
            keypoints = new VectorOfKeyPoint();
            descriptors=new Mat();

            points2d = new VectorOfPoint();
            points3d = new VectorOfPoint3D32F();
        }

    }
}
