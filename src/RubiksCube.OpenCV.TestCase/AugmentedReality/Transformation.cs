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
    public struct Transformation
    {
        public Transformation(OpenTK.Matrix3 m, OpenTK.Vector3 v)
        {
            m_rotation = m;
            m_translation = v;
        }

        public OpenTK.Matrix4 getMat44() { return new OpenTK.Matrix4(); }

        public Transformation getInverted() { return new Transformation(); }

        private OpenTK.Matrix3 m_rotation;
        private OpenTK.Vector3 m_translation;

        public OpenTK.Matrix3 r { get { return m_rotation; } }
        public OpenTK.Vector3 t { get { return m_translation; } }

    }
}
