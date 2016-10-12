using Emgu.CV;
using Emgu.CV.Util;
using System.Drawing;

namespace RubiksCube.OpenCV.TestCase.AugmentedReality
{
    public class Pattern
    {
        #region Fields

        /// <summary>
        /// 
        /// </summary>
        public Size Size { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Mat Frame { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Mat GrayImg { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public VectorOfKeyPoint Keypoints { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Mat Descriptors { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public VectorOfPoint Points2d { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public VectorOfPoint3D32F Points3d { get; set; }

        #endregion

        #region .ctor

        /// <summary>
        /// 
        /// </summary>
        public Pattern()
        {
            Frame = new Mat();
            GrayImg = new Mat();
            Keypoints = new VectorOfKeyPoint();
            Descriptors = new Mat();

            Points2d = new VectorOfPoint();
            Points3d = new VectorOfPoint3D32F();
        }

        #endregion
    }
}
