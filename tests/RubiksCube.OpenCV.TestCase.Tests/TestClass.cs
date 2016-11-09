using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using RubiksCube.OpenCV.TestCase.AugmentedReality;
using RubiksCube.OpenCV.TestCase.PtamLikeApproach;

namespace RubiksCube.OpenCV.TestCase.Tests
{
    [TestFixture]
    public class TestClass
    {
        private CameraCalibrationInfo _calibration;

        public TestClass()
        {
            _calibration = new CameraCalibrationInfo(560.764656335266f, 562.763179958161f, 295.849138757436f, 255.022208986073f);
        }

        [Test]
        public void Decompose_Eto_Rand()
        {
            Matrix<double> e;
            Matrix<double> r1;
            Matrix<double> r2;
            Matrix<double> t1;
            Matrix<double> t2;

            Matrix<double> r1Expected;
            Matrix<double> r2Expected;
            Matrix<double> t1Expected;
            Matrix<double> t2Expected;

            var comparer = Comparer<double>.Create((x, y) => Math.Abs(x - y) < 0.000001 ? 0 : 1);

            #region Case 1

            e = new Matrix<double>(new[,]
            {
                { -0.0793904, -15.101, 0.371601 },
                { 14.1124, 0.375567, -2.48662 },
                { -0.48423, 0.188653, 0.0805487 }
            });

            r1Expected = new Matrix<double>(new[,]
            {
                { -0.982481, -0.00737817, 0.186217 },
                { 0.0179944, -0.998303, 0.0553845 },
                { 0.185492, 0.0577651, 0.980946 }
            });
            r2Expected = new Matrix<double>(new[,]
            {
                { 0.987093, 0.00800034, -0.159948 },
                { -0.00611273, 0.999906, 0.01229 },
                { 0.160031, -0.0111536, 0.987049 }
            });
            t1Expected = new Matrix<double>(new[]
            {
                 0.013339, 0.0343641, 0.99932
            });
            t2Expected = new Matrix<double>(new[]
            {
                -0.013339, -0.0343641, -0.99932
            });

            OpenCvUtilities.DecomposeEtoRandT(e, out r1, out r2, out t1, out t2);

            CollectionAssert.AreEqual(r1Expected.Data, r1.Data, comparer);
            CollectionAssert.AreEqual(r2Expected.Data, r2.Data, comparer);
            CollectionAssert.AreEqual(t1Expected.Data, t1.Data, comparer);
            CollectionAssert.AreEqual(t2Expected.Data, t2.Data, comparer);

            #endregion

            #region Case 2

            e = new Matrix<double>(new[,]
            {
                { 0.4052652752518787, -12.45233689948151, 0.01348616168894345 },
                { 11.47229370932243, 0.4564241889679639, -1.831549257782737 },
                { -0.297551532077682, -0.001072849756781113, 0.04743653296950803 }
            });

            r1Expected = new Matrix<double>(new[,]
            {
                { -0.9870124281645626, -0.03547270126454876, 0.1566785055892807 },
                { 0.03193350943559628, -0.9991760497758493, -0.0250494017784099 },
                { -0.1574379802456221, 0.01972077633678765, -0.9873319468936744 }
            });
            r2Expected = new Matrix<double>(new[,]
            {
                { 0.9867407052466314, 0.03546201177308789, -0.1583831630268374 },
                { -0.04008650966451568, 0.9988553133911859, -0.02609855653312132 },
                { -0.1572763566220209, -0.0321015362747831, -0.9870327446526289 }
            });
            t1Expected = new Matrix<double>(new[]
            {
                 -0.0008631055249466995, -0.02589733540347029, -0.9996642351649142
            });
            t2Expected = new Matrix<double>(new[]
            {
               0.0008631055249466995, 0.02589733540347029, 0.9996642351649142
            });

            OpenCvUtilities.DecomposeEtoRandT(e, out r1, out r2, out t1, out t2);

            CollectionAssert.AreEqual(r1Expected.Data, r1.Data, comparer);
            CollectionAssert.AreEqual(r2Expected.Data, r2.Data, comparer);
            CollectionAssert.AreEqual(t1Expected.Data, t1.Data, comparer);
            CollectionAssert.AreEqual(t2Expected.Data, t2.Data, comparer);

            #endregion

            #region Case 3

            e = new Matrix<double>(new[,]
            {
                { -0.4052652752518787, 12.45233689948151, -0.01348616168894345 },
                { -11.47229370932243, -0.4564241889679639, 1.831549257782737 },
                { 0.297551532077682, 0.001072849756781113, -0.04743653296950803 }
            });

            r1Expected = new Matrix<double>(new[,]
            {
                { 0.9870124281645626, 0.03547270126454876, -0.1566785055892807 },
                { -0.03193350943559628, 0.9991760497758493, 0.0250494017784099 },
                { 0.1574379802456221, -0.01972077633678765, 0.9873319468936744 }
            });
            r2Expected = new Matrix<double>(new[,]
            {
                { -0.9867407052466314, -0.03546201177308789, 0.1583831630268374 },
                { 0.04008650966451568, -0.9988553133911859, 0.02609855653312132 },
                { 0.1572763566220209, 0.0321015362747831, 0.9870327446526289 }
            });
            t1Expected = new Matrix<double>(new[]
            {
                 0.0008631055249466995, 0.02589733540347029, 0.9996642351649142
            });
            t2Expected = new Matrix<double>(new[]
            {
                -0.0008631055249466995, -0.02589733540347029, -0.9996642351649142
            });

            OpenCvUtilities.DecomposeEtoRandT(e, out r1, out r2, out t1, out t2);

            CollectionAssert.AreEqual(r1Expected.Data, r1.Data, comparer);
            CollectionAssert.AreEqual(r2Expected.Data, r2.Data, comparer);
            CollectionAssert.AreEqual(t1Expected.Data, t1.Data, comparer);
            CollectionAssert.AreEqual(t2Expected.Data, t2.Data, comparer);

            #endregion
        }

        [Test]
        public void Triangulate_And_Check_Reproj()
        {
            Matrix<double> p;
            Matrix<double> p1;

            VectorOfPointF trackedFeatures;
            VectorOfPointF bootstrapKp;

            TriangulateAndCheckReprojResult result;

            #region Case 1

            p = new Matrix<double>(new double[3, 4]
            {
                { 1, 0, 0, 0 },
                { 0, 1, 0, 0 },
                { 0, 0, 1, 0 }
            });

            p1 = new Matrix<double>(new double[3, 4]
            {
                { -0.9824809681811199, -0.00737816829847441, 0.1862168354216374, 0.01333896289977746 },
                { 0.01799441939100593, -0.9983029386588546, 0.05538450627997073, 0.03436414273108072 },
                { 0.1854921778207113, 0.05776508718642179, 0.9809464035663084f, 0.9993203579248829 }
            });

            trackedFeatures = new VectorOfPointF(new[]
            {
               new PointF(427.13428f, 227.33339f),
                new PointF(391.91531f, 224.74487f),
                new PointF(407.59424f, 224.22755f),
                new PointF(429.16696f, 223.29608f),
                new PointF(421.02155f, 218.90135f),
                new PointF(418.12018f, 216.68893f),
                new PointF(372.59952f, 271.2688f),
                new PointF(374.59927f, 269.27325f),
                new PointF(421.33975f, 213.69167f),
                new PointF(421.7453f, 208.64569f),
                new PointF(406.97095f, 205.71231f),
                new PointF(373.56705f, 252.66739f),
                new PointF(419.0239f, 202.15726f),
                new PointF(410.15182f, 202.6559f),
                new PointF(414.73322f, 202.8203f),
                new PointF(421.86398f, 205.47487f),
                new PointF(402.73456f, 236.44148f),
                new PointF(385.00809f, 235.10345f),
                new PointF(381.72519f, 235.00421f),
                new PointF(384.38098f, 242.03709f),
                new PointF(400.35175f, 221.35245f),
                new PointF(375.55579f, 253.7218f),
                new PointF(405.49969f, 217.04935f),
                new PointF(382.35257f, 244.00104f),
                new PointF(397.18988f, 213.02864f),
                new PointF(423.28958f, 201.39389f),
                new PointF(403.97839f, 243.57945f),
                new PointF(371.52927f, 273.35638f),
                new PointF(402.63351f, 235.38774f),
                new PointF(421.15161f, 209.28316f),
                new PointF(421.27039f, 205.40399f),
                new PointF(407.20142f, 204.68332f),
                new PointF(418.98199f, 202.7654f),
                new PointF(413.95847f, 201.12106f),
                new PointF(375.23849f, 254.13487f),
                new PointF(426.56113f, 226.66367f),
                new PointF(428.95081f, 223.1118f),
                new PointF(399.58334f, 221.15453f),
                new PointF(384.38138f, 242.40669f),
                new PointF(421.60223f, 219.11192f),
                new PointF(382.37976f, 234.07773f),
                new PointF(372.77267f, 251.64935f),
                new PointF(408.25488f, 224.43033f),
                new PointF(418.13879f, 216.50366f),
                new PointF(420.66992f, 214.13902f),
                new PointF(405.49969f, 217.04935f),
                new PointF(381.91599f, 243.56929f),
                new PointF(407.97061f, 224.17844f),
                new PointF(428.24982f, 223.81142f),
                new PointF(406.58286f, 203.43124f),
                new PointF(425.33484f, 226.63785f),
                new PointF(422.5126f, 217.67339f),
                new PointF(419.67578f, 214.5631f),
                new PointF(381.633f, 233.09117f),
                new PointF(373.30225f, 251.47868f),
                new PointF(372.72458f, 270.70651f),
                new PointF(371.83621f, 248.51126f),
                new PointF(375.51776f, 269.3432f),
                new PointF(406.24478f, 215.43176f),
                new PointF(418.99274f, 202.52885f),
                new PointF(402.92493f, 234.41644f),
                new PointF(427.65225f, 224.37265f),
                new PointF(408.91357f, 225.19838f),
                new PointF(372.72458f, 270.70651f),
                new PointF(372.97241f, 251.12473f),
                new PointF(382.81277f, 232.57416f),
                new PointF(417.39114f, 213.46442f),
                new PointF(408.55203f, 203.72842f),
                new PointF(406.4628f, 216.28511f),
                new PointF(398.01672f, 220.56718f),
                new PointF(409.94516f, 225.57368f),
                new PointF(423.86615f, 217.82196f),
                new PointF(399.35309f, 220.36206f),
                new PointF(373.63873f, 250.76747f),
                new PointF(373.73434f, 270.03922f),
                new PointF(417.8584f, 211.4928f),
                new PointF(383.17978f, 230.55035f),
                new PointF(371.48196f, 274.57385f),
                new PointF(406.82678f, 214.84328f),
                new PointF(373.29251f, 249.99451f),
                new PointF(373.75876f, 270.42651f),
                new PointF(415.3963f, 209.66611f),
                new PointF(422.67966f, 217.35188f),
                new PointF(371.20831f, 272.03253f),
                new PointF(373.29816f, 250.51921f),
                new PointF(396.60257f, 224.0912f),
                new PointF(375.98041f, 247.96669f),
                new PointF(374.15375f, 273.72263f),
                new PointF(382.40717f, 219.19965f)
            });
            bootstrapKp = new VectorOfPointF(new[]
            {
                new PointF(333, 231),
                new PointF(300, 231),
                new PointF(315, 229),
                new PointF(335, 227),
                new PointF(327, 223),
                new PointF(324, 221),
                new PointF(285, 277),
                new PointF(287, 275),
                new PointF(327, 218),
                new PointF(327, 213),
                new PointF(313, 211),
                new PointF(284, 259),
                new PointF(324, 207),
                new PointF(316, 208),
                new PointF(320, 208),
                new PointF(327, 210),
                new PointF(311, 241),
                new PointF(294, 241),
                new PointF(291, 241),
                new PointF(294, 248),
                new PointF(308, 227),
                new PointF(286, 260),
                new PointF(312, 222),
                new PointF(292, 250),
                new PointF(304, 219),
                new PointF(328, 206),
                new PointF(313, 248),
                new PointF(284, 279),
                new PointF(310.80002f, 240.00002f),
                new PointF(326.40002f, 213.60001f),
                new PointF(326.40002f, 210.00002f),
                new PointF(313.20001f, 210.00002f),
                new PointF(324, 207.60001f),
                new PointF(319.20001f, 206.40001f),
                new PointF(285.60001f, 260.40002f),
                new PointF(332.40002f, 230.40001f),
                new PointF(334.80002f, 226.8f),
                new PointF(307.20001f, 226.8f),
                new PointF(294, 248.40001f),
                new PointF(327.60001f, 223.20001f),
                new PointF(291.60001f, 240.00002f),
                new PointF(283.20001f, 258),
                new PointF(315.60001f, 229.20001f),
                new PointF(324, 220.8f),
                new PointF(326.40002f, 218.40001f),
                new PointF(312, 222.00002f),
                new PointF(291.60001f, 249.60001f),
                new PointF(315.36002f, 228.96001f),
                new PointF(334.08002f, 227.52f),
                new PointF(312.48001f, 208.8f),
                new PointF(331.20001f, 230.40001f),
                new PointF(328.32001f, 221.76001f),
                new PointF(325.44f, 218.88f),
                new PointF(290.88f, 239.04001f),
                new PointF(283.68002f, 257.76001f),
                new PointF(285.12003f, 276.48001f),
                new PointF(282.24002f, 254.88f),
                new PointF(288, 275.04001f),
                new PointF(312.48001f, 220.32001f),
                new PointF(324f, 207.36002f),
                new PointF(311.04001f, 239.04001f),
                new PointF(333.50403f, 228.09602f),
                new PointF(316.22403f, 229.82402f),
                new PointF(285.12003f, 276.48004f),
                new PointF(283.39203f, 257.47202f),
                new PointF(292.03201f, 238.46402f),
                new PointF(323.13602f, 217.72803f),
                new PointF(314.49603f, 209.08801f),
                new PointF(312.76804f, 221.18402f),
                new PointF(305.85602f, 226.36803f),
                new PointF(317.26083f, 230.16963f),
                new PointF(329.70245f, 221.87523f),
                new PointF(306.89285f, 226.02243f),
                new PointF(284.08325f, 257.12643f),
                new PointF(286.15683f, 275.78885f),
                new PointF(323.48166f, 215.65443f),
                new PointF(292.37766f, 236.39043f),
                new PointF(284.08325f, 279.93604f),
                new PointF(313.11365f, 219.80164f),
                new PointF(283.66855f, 256.29703f),
                new PointF(286.15686f, 276.20358f),
                new PointF(320.99335f, 213.99557f),
                new PointF(328.45831f, 221.46053f),
                new PointF(283.66855f, 277.69659f),
                new PointF(283.66855f, 256.79468f),
                new PointF(304.57043f, 229.92084f),
                new PointF(286.65454f, 254.40591f),
                new PointF(286.65454f, 279.48819f),
                new PointF(290.23773f, 225.74046f)
            });

            result = OpenCvUtilities.TriangulateAndCheckReproj(_calibration, Utils.GetKeyPointsVector(trackedFeatures), Utils.GetKeyPointsVector(bootstrapKp), p, p1);

            Assert.That(false, Is.EqualTo(result.Result));
            Assert.That(0, Is.EqualTo(result.Error));

            #endregion

            #region Case 2

            p = new Matrix<double>(new double[3, 4]
            {
                { 1, 0, 0, 0 },
                { 0, 1, 0, 0 },
                { 0, 0, 1, 0 }
            });

            p1 = new Matrix<double>(new double[3, 4]
            {
                { 0.9870124281645626, 0.03547270126454876, -0.1566785055892807, -0.0008631055249466995 },
                { -0.03193350943559628, 0.9991760497758493, 0.0250494017784099, -0.02589733540347029 },
                { 0.1574379802456221, -0.01972077633678765, 0.9873319468936744, -0.9996642351649142 }
            });

            trackedFeatures = new VectorOfPointF(new[]
            {
                new PointF(426.03632f, 222.76335f),
                new PointF(390.96695f, 219.97858f),
                new PointF(406.54562f, 219.64514f),
                new PointF(428.09631f, 218.70369f),
                new PointF(420.02969f, 214.37195f),
                new PointF(417.18686f, 212.15242f),
                new PointF(371.31644f, 266.3609f),
                new PointF(373.28091f, 264.38715f),
                new PointF(420.39337f, 209.12727f),
                new PointF(420.87967f, 204.09981f),
                new PointF(406.13959f, 201.16556f),
                new PointF(372.52917f, 247.70665f),
                new PointF(418.207f, 197.58563f),
                new PointF(409.27347f, 198.09737f),
                new PointF(413.95377f, 198.25195f),
                new PointF(420.99988f, 200.95354f),
                new PointF(401.62491f, 231.76155f),
                new PointF(383.94525f, 230.31931f),
                new PointF(380.65768f, 230.22371f),
                new PointF(383.29068f, 237.19083f),
                new PointF(399.32339f, 216.67557f),
                new PointF(374.50339f, 248.74915f),
                new PointF(404.55908f, 212.4364f),
                new PointF(381.25662f, 239.14381f),
                new PointF(396.28745f, 208.28883f),
                new PointF(422.42389f, 196.84546f),
                new PointF(402.66537f, 238.85341f),
                new PointF(370.23953f, 268.43887f),
                new PointF(401.51538f, 230.76601f),
                new PointF(420.25861f, 204.6877f),
                new PointF(420.39197f, 200.87657f),
                new PointF(406.35147f, 200.13043f),
                new PointF(418.17007f, 198.2444f),
                new PointF(413.20557f, 196.53227f),
                new PointF(374.18121f, 249.16602f),
                new PointF(425.47458f, 222.13777f),
                new PointF(427.86325f, 218.52226f),
                new PointF(398.54398f, 216.46603f),
                new PointF(383.29218f, 237.52814f),
                new PointF(420.62109f, 214.55142f),
                new PointF(381.33087f, 229.29582f),
                new PointF(371.72778f, 246.67822f),
                new PointF(407.19f, 219.83838f),
                new PointF(417.2084f, 211.94316f),
                new PointF(419.71942f, 209.54953f),
                new PointF(404.55908f, 212.4364f),
                new PointF(380.8197f, 238.69392f),
                new PointF(406.92761f, 219.60931f),
                new PointF(427.15094f, 219.28215f),
                new PointF(405.73328f, 198.8499f),
                new PointF(424.24506f, 222.11359f),
                new PointF(421.53174f, 213.15207f),
                new PointF(418.71411f, 210.00742f),
                new PointF(380.58487f, 228.33238f),
                new PointF(372.25766f, 246.51025f),
                new PointF(371.43317f, 265.80927f),
                new PointF(370.80389f, 243.58653f),
                new PointF(374.17325f, 264.45847f),
                new PointF(405.3219f, 210.81206f),
                new PointF(418.19434f, 197.99078f),
                new PointF(401.79028f, 229.80133f),
                new PointF(426.57095f, 219.80702f),
                new PointF(407.84232f, 220.65297f),
                new PointF(371.43317f, 265.80927f),
                new PointF(371.91681f, 246.1552f),
                new PointF(381.74255f, 227.81966f),
                new PointF(416.45859f, 208.87207f),
                new PointF(407.67117f, 199.16803f),
                new PointF(405.53528f, 211.66692f),
                new PointF(397.01413f, 215.87862f),
                new PointF(408.84979f, 220.99838f),
                new PointF(422.86496f, 213.2979f),
                new PointF(398.32544f, 215.67107f),
                new PointF(372.59445f, 245.80473f),
                new PointF(372.43692f, 265.15585f),
                new PointF(416.97476f, 206.92216f),
                new PointF(382.06662f, 225.86298f),
                new PointF(370.20435f, 269.63144f),
                new PointF(405.93604f, 210.27585f),
                new PointF(372.24982f, 245.04134f),
                new PointF(372.46011f, 265.53363f),
                new PointF(414.5097f, 205.08484f),
                new PointF(421.71219f, 212.78195f),
                new PointF(369.8866f, 267.12936f),
                new PointF(372.25372f, 245.5535f),
                new PointF(395.57431f, 219.39975f),
                new PointF(374.96515f, 243.144f),
                new PointF(372.81671f, 268.81808f),
                new PointF(381.4924f, 214.46245f)
            });
            bootstrapKp = new VectorOfPointF(new[]
            {
                new PointF(333, 231),
                 new PointF(300, 231),
                 new PointF(315, 229),
                 new PointF(335, 227),
                 new PointF(327, 223),
                 new PointF(324, 221),
                 new PointF(285, 277),
                 new PointF(287, 275),
                 new PointF(327, 218),
                 new PointF(327, 213),
                 new PointF(313, 211),
                 new PointF(284, 259),
                 new PointF(324, 207),
                 new PointF(316, 208),
                 new PointF(320, 208),
                 new PointF(327, 210),
                 new PointF(311, 241),
                 new PointF(294, 241),
                 new PointF(291, 241),
                 new PointF(294, 248),
                 new PointF(308, 227),
                 new PointF(286, 260),
                 new PointF(312, 222),
                 new PointF(292, 250),
                 new PointF(304, 219),
                 new PointF(328, 206),
                 new PointF(313, 248),
                 new PointF(284, 279),
                 new PointF(310.80002f, 240.00002f),
                 new PointF(326.40002f, 213.60001f),
                 new PointF(326.40002f, 210.00002f),
                 new PointF(313.20001f, 210.00002f),
                 new PointF(324, 207.60001f),
                 new PointF(319.20001f, 206.40001f),
                 new PointF(285.60001f, 260.40002f),
                 new PointF(332.40002f, 230.40001f),
                 new PointF(334.80002f, 226.8f),
                 new PointF(307.20001f, 226.8f),
                 new PointF(294, 248.40001f),
                 new PointF(327.60001f, 223.20001f),
                 new PointF(291.60001f, 240.00002f),
                 new PointF(283.20001f, 258f),
                 new PointF(315.60001f, 229.20001f),
                 new PointF(324, 220.8f),
                 new PointF(326.40002f, 218.40001f),
                 new PointF(312, 222.00002f),
                 new PointF(291.60001f, 249.60001f),
                 new PointF(315.36002f, 228.96001f),
                 new PointF(334.08002f, 227.52f),
                 new PointF(312.48001f, 208.8f),
                 new PointF(331.20001f, 230.40001f),
                 new PointF(328.32001f, 221.76001f),
                 new PointF(325.44f, 218.88f),
                 new PointF(290.88f, 239.04001f),
                 new PointF(283.68002f, 257.76001f),
                 new PointF(285.12003f, 276.48001f),
                 new PointF(282.24002f, 254.88f),
                 new PointF(288, 275.04001f),
                 new PointF(312.48001f, 220.32001f),
                 new PointF(324, 207.36002f),
                 new PointF(311.04001f, 239.04001f),
                 new PointF(333.50403f, 228.09602f),
                 new PointF(316.22403f, 229.82402f),
                 new PointF(285.12003f, 276.48004f),
                 new PointF(283.39203f, 257.47202f),
                 new PointF(292.03201f, 238.46402f),
                 new PointF(323.13602f, 217.72803f),
                 new PointF(314.49603f, 209.08801f),
                 new PointF(312.76804f, 221.18402f),
                 new PointF(305.85602f, 226.36803f),
                 new PointF(317.26083f, 230.16963f),
                 new PointF(329.70245f, 221.87523f),
                 new PointF(306.89285f, 226.02243f),
                 new PointF(284.08325f, 257.12643f),
                 new PointF(286.15683f, 275.78885f),
                 new PointF(323.48166f, 215.65443f),
                 new PointF(292.37766f, 236.39043f),
                 new PointF(284.08325f, 279.93604f),
                 new PointF(313.11365f, 219.80164f),
                 new PointF(283.66855f, 256.29703f),
                 new PointF(286.15686f, 276.20358f),
                 new PointF(320.99335f, 213.99557f),
                 new PointF(328.45831f, 221.46053f),
                 new PointF(283.66855f, 277.69659f),
                 new PointF(283.66855f, 256.79468f),
                 new PointF(304.57043f, 229.92084f),
                 new PointF(286.65454f, 254.40591f),
                 new PointF(286.65454f, 279.48819f),
                 new PointF(290.23773f, 225.74046f)
            });

            result = OpenCvUtilities.TriangulateAndCheckReproj(_calibration, Utils.GetKeyPointsVector(trackedFeatures), Utils.GetKeyPointsVector(bootstrapKp), p, p1);

            Assert.That(true, Is.EqualTo(result.Result));
            Assert.AreEqual(result.Error, 2.90235f, 0.01);

            #endregion
        }

        [Test]
        public void Camera_Pose_And_Triangulation_From_Fundamental()
        {
            Matrix<double> p;
            Matrix<double> p1;
            Matrix<double> e;

            VectorOfPointF trackedFeatures;
            VectorOfPointF bootstrapKp;

            CameraPoseAndTriangulationFromFundamentalResult result;

            #region Case 1

            p = new Matrix<double>(new double[3, 4]
            {
                { 1, 0, 0, 0 },
                { 0, 1, 0, 0 },
                { 0, 0, 1, 0 }
            });

            p1 = new Matrix<double>(new double[3, 4]
            {
                { 0.9870124281645626, 0.03547270126454876, -0.1566785055892807, -0.0008631055249466995 },
                { -0.03193350943559628, 0.9991760497758493, 0.0250494017784099, -0.02589733540347029 },
                { 0.1574379802456221, -0.01972077633678765, 0.9873319468936744, -0.9996642351649142 }
            });

            e = new Matrix<double>(new double[3, 3]
            {
                {  0.4052652752518787, -12.45233689948151, 0.01348616168894345 },
                { 11.47229370932243, 0.4564241889679639, -1.831549257782737 },
                { -0.297551532077682, -0.001072849756781113, 0.04743653296950803 }
            });

            trackedFeatures = new VectorOfPointF(new[]
            {
                new PointF(426.03632f, 222.76335f),
                new PointF(390.96695f, 219.97858f),
                new PointF(406.54562f, 219.64514f),
                new PointF(428.09631f, 218.70369f),
                new PointF(420.02969f, 214.37195f),
                new PointF(417.18686f, 212.15242f),
                new PointF(371.31644f, 266.3609f),
                new PointF(373.28091f, 264.38715f),
                new PointF(420.39337f, 209.12727f),
                new PointF(420.87967f, 204.09981f),
                new PointF(406.13959f, 201.16556f),
                new PointF(372.52917f, 247.70665f),
                new PointF(418.207f, 197.58563f),
                new PointF(409.27347f, 198.09737f),
                new PointF(413.95377f, 198.25195f),
                new PointF(420.99988f, 200.95354f),
                new PointF(401.62491f, 231.76155f),
                new PointF(383.94525f, 230.31931f),
                new PointF(380.65768f, 230.22371f),
                new PointF(383.29068f, 237.19083f),
                new PointF(399.32339f, 216.67557f),
                new PointF(374.50339f, 248.74915f),
                new PointF(404.55908f, 212.4364f),
                new PointF(381.25662f, 239.14381f),
                new PointF(396.28745f, 208.28883f),
                new PointF(422.42389f, 196.84546f),
                new PointF(402.66537f, 238.85341f),
                new PointF(370.23953f, 268.43887f),
                new PointF(401.51538f, 230.76601f),
                new PointF(420.25861f, 204.6877f),
                new PointF(420.39197f, 200.87657f),
                new PointF(406.35147f, 200.13043f),
                new PointF(418.17007f, 198.2444f),
                new PointF(413.20557f, 196.53227f),
                new PointF(374.18121f, 249.16602f),
                new PointF(425.47458f, 222.13777f),
                new PointF(427.86325f, 218.52226f),
                new PointF(398.54398f, 216.46603f),
                new PointF(383.29218f, 237.52814f),
                new PointF(420.62109f, 214.55142f),
                new PointF(381.33087f, 229.29582f),
                new PointF(371.72778f, 246.67822f),
                new PointF(407.19f, 219.83838f),
                new PointF(417.2084f, 211.94316f),
                new PointF(419.71942f, 209.54953f),
                new PointF(404.55908f, 212.4364f),
                new PointF(380.8197f, 238.69392f),
                new PointF(406.92761f, 219.60931f),
                new PointF(427.15094f, 219.28215f),
                new PointF(405.73328f, 198.8499f),
                new PointF(424.24506f, 222.11359f),
                new PointF(421.53174f, 213.15207f),
                new PointF(418.71411f, 210.00742f),
                new PointF(380.58487f, 228.33238f),
                new PointF(372.25766f, 246.51025f),
                new PointF(371.43317f, 265.80927f),
                new PointF(370.80389f, 243.58653f),
                new PointF(374.17325f, 264.45847f),
                new PointF(405.3219f, 210.81206f),
                new PointF(418.19434f, 197.99078f),
                new PointF(401.79028f, 229.80133f),
                new PointF(426.57095f, 219.80702f),
                new PointF(407.84232f, 220.65297f),
                new PointF(371.43317f, 265.80927f),
                new PointF(371.91681f, 246.1552f),
                new PointF(381.74255f, 227.81966f),
                new PointF(416.45859f, 208.87207f),
                new PointF(407.67117f, 199.16803f),
                new PointF(405.53528f, 211.66692f),
                new PointF(397.01413f, 215.87862f),
                new PointF(408.84979f, 220.99838f),
                new PointF(422.86496f, 213.2979f),
                new PointF(398.32544f, 215.67107f),
                new PointF(372.59445f, 245.80473f),
                new PointF(372.43692f, 265.15585f),
                new PointF(416.97476f, 206.92216f),
                new PointF(382.06662f, 225.86298f),
                new PointF(370.20435f, 269.63144f),
                new PointF(405.93604f, 210.27585f),
                new PointF(372.24982f, 245.04134f),
                new PointF(372.46011f, 265.53363f),
                new PointF(414.5097f, 205.08484f),
                new PointF(421.71219f, 212.78195f),
                new PointF(369.8866f, 267.12936f),
                new PointF(372.25372f, 245.5535f),
                new PointF(395.57431f, 219.39975f),
                new PointF(374.96515f, 243.144f),
                new PointF(372.81671f, 268.81808f),
                new PointF(381.4924f, 214.46245f)
            });
            bootstrapKp = new VectorOfPointF(new[]
            {
                new PointF(333, 231),
                new PointF(300, 231),
                new PointF(315, 229),
                new PointF(335, 227),
                new PointF(327, 223),
                new PointF(324, 221),
                new PointF(285, 277),
                new PointF(287, 275),
                new PointF(327, 218),
                new PointF(327, 213),
                new PointF(313, 211),
                new PointF(284, 259),
                new PointF(324, 207),
                new PointF(316, 208),
                new PointF(320, 208),
                new PointF(327, 210),
                new PointF(311, 241),
                new PointF(294, 241),
                new PointF(291, 241),
                new PointF(294, 248),
                new PointF(308, 227),
                new PointF(286, 260),
                new PointF(312, 222),
                new PointF(292, 250),
                new PointF(304, 219),
                new PointF(328, 206),
                new PointF(313, 248),
                new PointF(284, 279),
                new PointF(310.80002f, 240.00002f),
                new PointF(326.40002f, 213.60001f),
                new PointF(326.40002f, 210.00002f),
                new PointF(313.20001f, 210.00002f),
                new PointF(324, 207.60001f),
                new PointF(319.20001f, 206.40001f),
                new PointF(285.60001f, 260.40002f),
                new PointF(332.40002f, 230.40001f),
                new PointF(334.80002f, 226.8f),
                new PointF(307.20001f, 226.8f),
                new PointF(294, 248.40001f),
                new PointF(327.60001f, 223.20001f),
                new PointF(291.60001f, 240.00002f),
                new PointF(283.20001f, 258),
                new PointF(315.60001f, 229.20001f),
                new PointF(324, 220.8f),
                new PointF(326.40002f, 218.40001f),
                new PointF(312, 222.00002f),
                new PointF(291.60001f, 249.60001f),
                new PointF(315.36002f, 228.96001f),
                new PointF(334.08002f, 227.52f),
                new PointF(312.48001f, 208.8f),
                new PointF(331.20001f, 230.40001f),
                new PointF(328.32001f, 221.76001f),
                new PointF(325.44f, 218.88f),
                new PointF(290.88f, 239.04001f),
                new PointF(283.68002f, 257.76001f),
                new PointF(285.12003f, 276.48001f),
                new PointF(282.24002f, 254.88f),
                new PointF(288, 275.04001f),
                new PointF(312.48001f, 220.32001f),
                new PointF(324, 207.36002f),
                new PointF(311.04001f, 239.04001f),
                new PointF(333.50403f, 228.09602f),
                new PointF(316.22403f, 229.82402f),
                new PointF(285.12003f, 276.48004f),
                new PointF(283.39203f, 257.47202f),
                new PointF(292.03201f, 238.46402f),
                new PointF(323.13602f, 217.72803f),
                new PointF(314.49603f, 209.08801f),
                new PointF(312.76804f, 221.18402f),
                new PointF(305.85602f, 226.36803f),
                new PointF(317.26083f, 230.16963f),
                new PointF(329.70245f, 221.87523f),
                new PointF(306.89285f, 226.02243f),
                new PointF(284.08325f, 257.12643f),
                new PointF(286.15683f, 275.78885f),
                new PointF(323.48166f, 215.65443f),
                new PointF(292.37766f, 236.39043f),
                new PointF(284.08325f, 279.93604f),
                new PointF(313.11365f, 219.80164f),
                new PointF(283.66855f, 256.29703f),
                new PointF(286.15686f, 276.20358f),
                new PointF(320.99335f, 213.99557f),
                new PointF(328.45831f, 221.46053f),
                new PointF(283.66855f, 277.69659f),
                new PointF(283.66855f, 256.79468f),
                new PointF(304.57043f, 229.92084f),
                new PointF(286.65454f, 254.40591f),
                new PointF(286.65454f, 279.48819f),
                new PointF(290.23773f, 225.74046f)
            });

            result = OpenCvUtilities.CameraPoseAndTriangulationFromFundamental(_calibration, Utils.GetKeyPointsVector(trackedFeatures), Utils.GetKeyPointsVector(bootstrapKp));

            Assert.AreEqual(196.532, result.Min, 0.1);
            Assert.AreEqual(428.096, result.Max, 0.1);

            Assert.That(result.Esential, Is.EqualTo(e));

            Assert.That(result.P1, Is.EqualTo(p));
            Assert.That(result.P2, Is.EqualTo(p1));

            Assert.That(result.Result, Is.EqualTo(true));

            #endregion
        }

        [Test]
        public void FundamentalMatrix()
        {
            #region Init

            //15 Points
            //p1 = [[245.77, 248.66, 263.39, 251.11, 249.88, 287.57, 240.93, 206.84,
            //       224.95, 231.47, 257.55, 264.71, 227.67, 246.01, 244.15],
            //      [169.57, 105.82, 182.45, 146.54, 197.99, 137.11, 113.15, 171.57,
            //       170.34, 170.21, 124.43, 176.16, 127.05, 113.26, 154.41]]

            //p2 = [[267.07, 252.92, 254.22, 284.33, 236.82, 220.04, 255.9, 259.09,
            //   241.73, 258.62, 277.83, 219.62, 262.91, 250.93, 287.09],
            //  [172.34, 105.02, 190.25, 145.1, 190.63, 135.11, 114.37, 165.9,
            //   164.27, 168.3, 112.98, 170.46, 129.03, 114.5, 155.89]]

            //15 Points
            var p1 = new List<PointF>()
            {
                new PointF(245.77f,169.57f),
                new PointF(248.66f,105.82f),
                new PointF(263.39f,182.45f),
                new PointF(251.11f,146.54f),
                new PointF(249.88f,197.99f),
                new PointF(287.57f,137.11f),
                new PointF(240.93f,113.15f),
                new PointF(206.84f,171.57f),
                new PointF(224.95f,170.34f),
                new PointF(231.47f,170.21f),
                new PointF(257.55f,124.43f),
                new PointF(264.71f,176.16f),
                new PointF(227.67f,127.05f),
                new PointF(246.01f,113.26f),
                new PointF(244.15f,154.41f)
            };

            var p2 = new List<PointF>()
            {
                new PointF(267.07f,172.34f),
                new PointF(252.92f,105.02f),
                new PointF(254.22f,190.25f),
                new PointF(284.33f,145.1f),
                new PointF(236.82f,190.63f),
                new PointF(220.04f,135.11f),
                new PointF(255.9f,114.37f),
                new PointF(259.09f,165.9f),
                new PointF(241.73f,164.27f),
                new PointF(258.62f,168.3f),
                new PointF(277.83f,112.98f),
                new PointF(219.62f,170.46f),
                new PointF(262.91f,129.03f),
                new PointF(250.93f,114.5f),
                new PointF(287.09f,155.89f)
            };

            #endregion

            var f1Expected = new Matrix<double>(new[,]
            {
                { -0.0, 0.000013, -0.002001 },
                { 0.000013, 0, -0.004486 },
                { -0.001997, -0.002184, 1  }
            });

            var comparer = Comparer<double>.Create((x, y) => Math.Abs(x - y) < 0.000001 ? 0 : 1);

            var p1Vec = new VectorOfPointF(p1.ToArray());
            var p2Vec = new VectorOfPointF(p2.ToArray());

            var f1 = new Mat();
            CvInvoke.FindFundamentalMat(p1Vec, p2Vec, f1);
            var f1Mat = new Matrix<double>(f1.Rows, f1.Cols, f1.DataPointer);

            CollectionAssert.AreEqual(f1Expected.Data, f1Mat.Data, comparer);

            var f2 = new Mat();
            CvInvoke.FindFundamentalMat(p2Vec, p1Vec, f2);
            var f2Mat = new Matrix<double>(f2.Rows, f2.Cols, f2.DataPointer);

            CollectionAssert.AreEqual(f1Mat.Transpose().Data, f2Mat.Data, comparer);
        }
    }
}
