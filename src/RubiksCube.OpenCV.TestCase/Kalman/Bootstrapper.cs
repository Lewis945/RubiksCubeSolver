using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;
using RubiksCube.OpenCV.TestCase.AugmentedReality;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubiksCube.OpenCV.TestCase.Kalman
{
    class Bootstrapper
    {
        //public static void Run(string path)
        //{
        //    var calibration = new CameraCalibrationInfo(560.764656335266f, 562.763179958161f, 295.849138757436f, 255.022208986073f);

        //    var capture = new Capture(path);
        //    var image1 = capture.QueryFrame();
        //    var image2 = capture.QueryFrame();

        //    var image1Gray = new Mat();
        //    var image2Gray = new Mat();

        //    CvInvoke.CvtColor(image1, image1Gray, ColorConversion.Rgb2Gray);
        //    CvInvoke.CvtColor(image2, image2Gray, ColorConversion.Rgb2Gray);

        //    VectorOfKeyPoint keypoints1, keypoints2;
        //    Mat descriptors1, descriptors2;

        //    FeaturesUtils.ExtractFeatures(image1Gray, out keypoints1, out descriptors1);
        //    FeaturesUtils.ExtractFeatures(image2Gray, out keypoints2, out descriptors2);

        //    Mat homography;
        //    VectorOfVectorOfDMatch matches;
        //    FeaturesUtils.GetMatches(keypoints1, descriptors1, keypoints2, descriptors2, out matches, out homography);

        //    //var points1 = keypoints1.ToArray().Select(k => k.Point);
        //    //var points2 = keypoints2.ToArray().Select(k => k.Point);

        //    var points1 = new List<PointF>();
        //    var points2 = new List<PointF>();

        //    for (int i = 0; i < matches.Size; i++)
        //    {
        //        var a = matches[i].ToArray();
        //        foreach (var e in a)
        //        {
        //            points1.Add(keypoints1[e.QueryIdx].Point);
        //            points2.Add(keypoints2[e.TrainIdx].Point);
        //        }
        //    }
        //}

        public static void Run1()
        {
            var calibration = new CameraCalibrationInfo(560.764656335266f, 562.763179958161f, 295.849138757436f, 255.022208986073f);

            var mat = new Mat(new Size(500, 500), DepthType.Cv32F, 3);

            //CvInvoke.Circle(mat, new Point(200, 200), 20, new MCvScalar(125, 10, 10), 1, LineType.AntiAlias, 0);

            var A = new Matrix<float>(new float[] { 1 });
            var H = new Matrix<float>(new float[] { 1 });
            var B = new Matrix<float>(new float[] { 0 });
            var Q = new Matrix<float>(new float[] { 0.00001f });
            var R = new Matrix<float>(new float[] { 0.1f });
            var xhat = new Matrix<float>(new float[] { 3 });
            var P = new Matrix<float>(new float[] { 1 });

            var filter = new SimpleLinearKalman(a: A, b: B, h: H, q: Q, r: R, currentStateEstimate: xhat, currentProbEstimate: P);
            var v = new Voltmeter(220, 20);

            var measuredvoltage = new List<float>();
            var truevoltage = new List<float>();
            var kalman = new List<float>();

            for (int i = 0; i < 60; i++)
            {
                var measured = v.GetVoltageWithNoise();
                measuredvoltage.Add(measured);
                truevoltage.Add(v.GetVoltage());
                kalman.Add(filter.GetCurrentState()[0, 0]);
                filter.Step(new Matrix<float>(new float[] { 0 }), new Matrix<float>(new float[] { measured }));
            }

            List<Point> p1 = new List<Point>();
            List<Point> p2 = new List<Point>();
            List<Point> p3 = new List<Point>();

            int x = 10;
            for (int i = 0; i < 60; i++)
            {
                var measuredV = measuredvoltage[i];
                var trueV = truevoltage[i];
                var kalmanV = kalman[i];

                p1.Add(new Point(x + (i * 10), (int)measuredV));
                p2.Add(new Point(x + (i * 10), (int)trueV));
                p3.Add(new Point(x + (i * 10), (int)kalmanV));

                //CvInvoke.Circle(mat, new Point(x + (i * 10), (int)measuredV), 2, new MCvScalar(125, 10, 10), 1, LineType.AntiAlias, 0);
                //CvInvoke.Circle(mat, new Point(x + (i * 10), (int)trueV), 2, new MCvScalar(125, 10, 10), 1, LineType.AntiAlias, 0);
                //CvInvoke.Circle(mat, new Point(x + (i * 10), (int)kalmanV), 2, new MCvScalar(125, 10, 10), 1, LineType.AntiAlias, 0);
            }

            CvInvoke.Polylines(mat, p1.ToArray(), false, new MCvScalar(100, 10, 10));
            CvInvoke.Polylines(mat, p2.ToArray(), false, new MCvScalar(10, 100, 10));
            CvInvoke.Polylines(mat, p3.ToArray(), false, new MCvScalar(10, 10, 100));

            ImageViewer.Show(mat, "Plot");
        }

        public static void Run()
        {
            var mat = new Mat(new Size(500, 500), DepthType.Cv32F, 3);

            var kal = new KalmanFilter(1, 1, 0);
            var v = new Voltmeter(220, 20);

            var measuredvoltage = new List<float>();
            var truevoltage = new List<float>();
            var kalman = new List<float>();

            var c1 = new Matrix<float>(new float[] { 0 });
            var control = new Mat(new int[] { 0 }, DepthType.Cv32F, c1.Ptr);

            var t3 = new Matrix<float>(kal.TransitionMatrix.Rows, kal.TransitionMatrix.Cols, kal.TransitionMatrix.Ptr);
            t3[0, 0] = 1;

            for (int i = 0; i < 60; i++)
            {
                var measured = v.GetVoltageWithNoise();
                measuredvoltage.Add(measured);
                truevoltage.Add(v.GetVoltage());

                var predicted = kal.Predict(control);
                var t = new Matrix<float>(predicted.Rows, predicted.Cols, predicted.Ptr);
                kalman.Add(t[0, 0]);

                var m = new Matrix<float>(new float[] { measured });
                var c = m.ToUMat().ToMat(AccessType.ReadWrite);

                kal.Correct(c);
            }

            List<Point> p1 = new List<Point>();
            List<Point> p2 = new List<Point>();
            List<Point> p3 = new List<Point>();

            int x = 10;
            for (int i = 0; i < 60; i++)
            {
                var measuredV = measuredvoltage[i];
                var trueV = truevoltage[i];
                var kalmanV = kalman[i];

                p1.Add(new Point(x + (i * 10), (int)measuredV));
                p2.Add(new Point(x + (i * 10), (int)trueV));
                p3.Add(new Point(x + (i * 10), (int)kalmanV));

                //CvInvoke.Circle(mat, new Point(x + (i * 10), (int)measuredV), 2, new MCvScalar(125, 10, 10), 1, LineType.AntiAlias, 0);
                //CvInvoke.Circle(mat, new Point(x + (i * 10), (int)trueV), 2, new MCvScalar(125, 10, 10), 1, LineType.AntiAlias, 0);
                //CvInvoke.Circle(mat, new Point(x + (i * 10), (int)kalmanV), 2, new MCvScalar(125, 10, 10), 1, LineType.AntiAlias, 0);
            }

            CvInvoke.Polylines(mat, p1.ToArray(), false, new MCvScalar(100, 10, 10));
            CvInvoke.Polylines(mat, p2.ToArray(), false, new MCvScalar(10, 100, 10));
            CvInvoke.Polylines(mat, p3.ToArray(), false, new MCvScalar(10, 10, 100));

            ImageViewer.Show(mat, "Plot");
        }
    }
}
