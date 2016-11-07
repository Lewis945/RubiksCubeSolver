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
        private readonly Matrix<float> _intrinsic;

        /// <summary>
        /// 
        /// </summary>
        private readonly VectorOfFloat _distortion;

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public Matrix<float> Intrinsic => _intrinsic;

        /// <summary>
        /// 
        /// </summary>
        public VectorOfFloat Distortion => _distortion;

        /// <summary>
        /// 
        /// </summary>
        public float Fx
        {
            get { return _intrinsic[0, 0]; }
            set { _intrinsic[0, 0] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public float Fy
        {
            get { return _intrinsic[1, 1]; }
            set { _intrinsic[1, 1] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public float Cx
        {
            get { return _intrinsic[0, 2]; }
            set { _intrinsic[0, 2] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public float Cy
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
        public CameraCalibrationInfo(float fx, float fy, float cx, float cy, float[] distorsionCoeff)
        {
            _intrinsic = new Matrix<float>(3, 3);
            _intrinsic.SetIdentity();
            _distortion = new VectorOfFloat(distorsionCoeff);

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
        public CameraCalibrationInfo(float fx, float fy, float cx, float cy)
            : this(fx, fy, cx, cy, new float[] { 0, 0, 0, 0, 0 })
        {
        }
    }
}
