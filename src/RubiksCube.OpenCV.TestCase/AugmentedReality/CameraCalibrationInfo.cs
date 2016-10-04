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

        Matrix<float> m_intrinsic;
        VectorOfFloat m_distortion;

        #endregion

        #region Properties

        float fx { get { return m_intrinsic[1, 1]; } set { m_intrinsic[1, 1] = value; } }
        float fy { get { return m_intrinsic[0, 0]; } set { m_intrinsic[0, 0] = value; } }

        float cx { get { return m_intrinsic[0, 2]; } set { m_intrinsic[0, 2] = value; } }
        float cy { get { return m_intrinsic[1, 2]; } set { m_intrinsic[1, 2] = value; } }

        #endregion

        public CameraCalibrationInfo(float _fx, float _fy, float _cx, float _cy, float[] distorsionCoeff)
        {
            m_intrinsic = new Matrix<float>(3, 3);
            m_distortion = new VectorOfFloat(distorsionCoeff);

            fx = _fx;
            fy = _fy;
            cx = _cx;
            cy = _cy;
        }

        public CameraCalibrationInfo(float _fx, float _fy, float _cx, float _cy)
            : this(_fx, _fy, _cx, _cy, new float[] { 0, 0, 0, 0, 0 })
        {
        }

        public Matrix<float> getIntrinsic()
        {
            return m_intrinsic;
        }

        public VectorOfFloat getDistorsion()
        {
            return m_distortion;
        }
    }
}
