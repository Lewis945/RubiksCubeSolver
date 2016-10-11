using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace RubiksCube.OpenCV.TestCase.AugmentedReality
{
    public struct Transformation
    {
        private OpenTK.Matrix3 _rotation;
        private OpenTK.Vector3 _translation;

        public OpenTK.Matrix3 r { get { return _rotation; } }
        public OpenTK.Vector3 t { get { return _translation; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <param name="v"></param>
        public Transformation(OpenTK.Matrix3 m, OpenTK.Vector3 v)
        {
            _rotation = m;
            _translation = v;
        }

        public OpenTK.Matrix4 getMat44()
        {
            OpenTK.Matrix4 res = OpenTK.Matrix4.Identity;

            for (int col = 0; col < 3; col++)
            {
                for (int row = 0; row < 3; row++)
                {
                    // Copy rotation component
                    res[row, col] = _rotation[row, col];
                }

                // Copy translation component
                res[3, col] = _translation[col];
            }

            return res;
        }

        public Transformation GetInverted()
        {
            var rot = OpenTK.Matrix3.Transpose(_rotation);
            return new Transformation(rot, -_translation);
        }

        public void SetRotationMatrixValue(int i, int j, float value)
        {
            _rotation[i, j] = value;
        }

        public void SetTranslationVectorValue(int i, float value)
        {
            _translation[i] = value;
        }
    }
}
