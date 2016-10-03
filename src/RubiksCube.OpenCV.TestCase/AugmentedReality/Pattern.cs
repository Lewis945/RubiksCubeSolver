using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;
using RubiksCube.OpenCV.TestCase.AugmentedReality;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

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

    }
}
