using Accord.Math;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubiksCube.OpenCV.TestCase.Tests
{
    [TestFixture]
    public class DotProductTests
    {
        public DotProductTests()
        {
            //Normal: [-0.646031086055159, 0.751958713237675, 0.131156126186813]
            //Normal: [0.525444735828459, 0.850455524374977, -0.0251640745954108]
            //Normal: [0.316921188919316, 0.941573750649253, 0.114016806227265]

            //Normal: [-0.0375129683159171, 0.989581901354632, 0.138997977393486]
            //Normal: [-0.305450683442706, 0.950639820670283, -0.0546242742776077]
            //Normal: [0.296329979471642, 0.952581596888181, 0.069114720113011]
        }

        [Test]
        public void Test()
        {
            //var v1 = new Vector3(-0.646031086055159f, 0.751958713237675f, 0.131156126186813f);
            //var v2 = new Vector3(0.525444735828459f, 0.850455524374977f, -0.0251640745954108f);
            //var v3 = new Vector3(0.316921188919316f, 0.941573750649253f, 0.114016806227265f);

            var v1 = new double[] { -0.646031086055159, 0.751958713237675, 0.131156126186813 };
            var v2 = new double[] { 0.525444735828459, 0.850455524374977, -0.0251640745954108 };
            var v3 = new double[] { 0.316921188919316, 0.941573750649253, 0.114016806227265 };

            var v1Xv2 = v1.Dot(v2);
            var v1Xv3 = v1.Dot(v3);
            var v2Xv3 = v2.Dot(v3);

            var q1 = (v2[0] * v3[0] + v2[1] * v3[1] + v2[2] * v3[2]) / (Math.Sqrt(v2[0] * v2[0] + v2[1] * v2[1] + v2[2] * v2[2]) * Math.Sqrt(v3[0] * v3[0] + v3[1] * v3[1] + v3[2] * v3[2]));
            var angle = RadianToDegree(Math.Acos(q1));
            var angle1 = RadianToDegree(Math.Acos(v1Xv3));
            var angle2 = RadianToDegree(Math.Acos(v1Xv2));

            var t1 = new double[] { -0.305450683442706, 0.950639820670283, -0.0546242742776077 };
            var t2 = new double[] { 0.296329979471642, 0.952581596888181, 0.069114720113011 };
            var t1Xt2 = t1.Dot(t2);
            var angle5 = RadianToDegree(Math.Acos(t1Xt2));

        }

        private double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        private double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }
    }
}
