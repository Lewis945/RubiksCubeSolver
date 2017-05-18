using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Util;

namespace RubiksCube.OpenCV.TestCase.PtamLikeApproach
{
    public class PlaneInfo
    {
        public Matrix<double> Normal { get; set; }
        public VectorOfPoint3D32F Points3D { get; set; }

        public VectorOfFloat Raux { get; set; }
        public VectorOfFloat Taux { get; set; }
    }
}
