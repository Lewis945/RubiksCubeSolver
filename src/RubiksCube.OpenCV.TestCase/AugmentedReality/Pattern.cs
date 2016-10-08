﻿using Emgu.CV;
using Emgu.CV.Util;
using System.Drawing;

namespace RubiksCube.OpenCV.TestCase.AugmentedReality
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

        #region .ctor

        public Pattern()
        {
            frame = new Mat();
            grayImg = new Mat();
            keypoints = new VectorOfKeyPoint();
            descriptors = new Mat();

            points2d = new VectorOfPoint();
            points3d = new VectorOfPoint3D32F();
        }

        #endregion
    }
}
