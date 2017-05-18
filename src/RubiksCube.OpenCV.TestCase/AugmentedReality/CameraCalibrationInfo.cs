using Emgu.CV;
using Emgu.CV.Util;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubiksCube.OpenCV.TestCase.AugmentedReality
{
    public class CameraCalibrationInfo
    {
        #region Fields

        /// <summary>
        /// 
        /// </summary>
        private readonly Matrix<double> _intrinsic;

        /// <summary>
        /// 
        /// </summary>
        private readonly VectorOfDouble _distortion;

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public Matrix<double> Intrinsic => _intrinsic;

        /// <summary>
        /// 
        /// </summary>
        public VectorOfDouble Distortion => _distortion;

        /// <summary>
        /// 
        /// </summary>
        public double Fx
        {
            get { return _intrinsic[0, 0]; }
            set { _intrinsic[0, 0] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public double Fy
        {
            get { return _intrinsic[1, 1]; }
            set { _intrinsic[1, 1] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public double Cx
        {
            get { return _intrinsic[0, 2]; }
            set { _intrinsic[0, 2] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public double Cy
        {
            get { return _intrinsic[1, 2]; }
            set { _intrinsic[1, 2] = value; }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fx"></param>
        /// <param name="fy"></param>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="distorsionCoeff"></param>
        public CameraCalibrationInfo(double fx, double fy, double cx, double cy, double[] distorsionCoeff)
        {
            _intrinsic = new Matrix<double>(3, 3);
            _intrinsic.SetIdentity();
            _distortion = new VectorOfDouble(distorsionCoeff);

            Fx = fx;
            Fy = fy;
            Cx = cx;
            Cy = cy;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fx"></param>
        /// <param name="fy"></param>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        public CameraCalibrationInfo(double fx, double fy, double cx, double cy)
            : this(fx, fy, cx, cy, new double[] { 0, 0, 0, 0, 0 })
        {
        }
    }
}
