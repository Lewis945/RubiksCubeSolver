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
    public struct Transformation
    {
        public Transformation(OpenTK.Matrix3 m, OpenTK.Vector3 v)
        {
            m_rotation = m;
            m_translation = v;
        }

        public OpenTK.Matrix4 getMat44()
        {
            OpenTK.Matrix4 res = OpenTK.Matrix4.Identity;

            for (int col = 0; col < 3; col++)
            {
                for (int row = 0; row < 3; row++)
                {
                    // Copy rotation component
                    res[row, col] = m_rotation[row, col];
                }

                // Copy translation component
                res[3, col] = m_translation[col];
            }

            return res;
        }

        public Transformation getInverted()
        {
            var rot = OpenTK.Matrix3.Transpose(m_rotation);
            return new Transformation(rot, -m_translation);
        }

        private OpenTK.Matrix3 m_rotation;
        private OpenTK.Vector3 m_translation;

        public OpenTK.Matrix3 r { get { return m_rotation; } }
        public OpenTK.Vector3 t { get { return m_translation; } }

    }
}
