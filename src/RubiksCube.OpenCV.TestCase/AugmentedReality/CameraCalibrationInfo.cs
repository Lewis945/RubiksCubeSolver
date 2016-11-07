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
        private Matrix<float> _intrinsic;

        /// <summary>
        /// 
        /// </summary>
        private VectorOfFloat _distortion;

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public Matrix<float> Intrinsic { get { return _intrinsic; } }

        /// <summary>
        /// 
        /// </summary>
        public VectorOfFloat Distortion { get { return _distortion; } }

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
        /// <param name="_fx"></param>
        /// <param name="_fy"></param>
        /// <param name="_cx"></param>
        /// <param name="_cy"></param>
        /// <param name="distorsionCoeff"></param>
        public CameraCalibrationInfo(float _fx, float _fy, float _cx, float _cy, float[] distorsionCoeff)
        {
            _intrinsic = new Matrix<float>(3, 3);
            _distortion = new VectorOfFloat(distorsionCoeff);

            Fx = _fx;
            Fy = _fy;
            Cx = _cx;
            Cy = _cy;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_fx"></param>
        /// <param name="_fy"></param>
        /// <param name="_cx"></param>
        /// <param name="_cy"></param>
        public CameraCalibrationInfo(float _fx, float _fy, float _cx, float _cy)
            : this(_fx, _fy, _cx, _cy, new float[] { 0, 0, 0, 0, 0 })
        {
        }
    }
}
