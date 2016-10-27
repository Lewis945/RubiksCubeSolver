﻿using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using RubiksCube.OpenCV.TestCase.AugmentedReality;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubiksCube.OpenCV.TestCase.PtamLikeApproach
{
    class Bootstrapper
    {
        public static void Run(string path)
        {
            var calibration = new CameraCalibrationInfo(560.764656335266f, 562.763179958161f, 295.849138757436f, 255.022208986073f);

            var capture = new Capture(path);
            var image1 = capture.QueryFrame();
            for (int i = 0; i < 100; i++)
            {
                capture.QueryFrame();
            }
            var image2 = capture.QueryFrame();

            //var image1Gray = new Mat();
            //var image2Gray = new Mat();

            //CvInvoke.CvtColor(image1, image1Gray, ColorConversion.Rgb2Gray);
            //CvInvoke.CvtColor(image2, image2Gray, ColorConversion.Rgb2Gray);

            var tracker = new SimpleAdHocTracker(calibration);
            tracker.Bootstrap(image1);
            tracker.BootstrapTrack(image2);
        }

        public static void Run1(string path)
        {
            var calibration = new CameraCalibrationInfo(560.764656335266f, 562.763179958161f, 295.849138757436f, 255.022208986073f);

            var capture = new Capture(path);
            var image1 = capture.QueryFrame();
            var image2 = capture.QueryFrame();

            var image1Gray = new Mat();
            var image2Gray = new Mat();

            CvInvoke.CvtColor(image1, image1Gray, ColorConversion.Rgb2Gray);
            CvInvoke.CvtColor(image2, image2Gray, ColorConversion.Rgb2Gray);

            VectorOfKeyPoint keypoints1, keypoints2;
            Mat descriptors1, descriptors2;

            FeaturesUtils.ExtractFeatures(image1Gray, out keypoints1, out descriptors1);
            FeaturesUtils.ExtractFeatures(image2Gray, out keypoints2, out descriptors2);

            Mat homography;
            VectorOfVectorOfDMatch matches;
            FeaturesUtils.GetMatches(keypoints1, descriptors1, keypoints2, descriptors2, out matches, out homography);

            //var points1 = keypoints1.ToArray().Select(k => k.Point);
            //var points2 = keypoints2.ToArray().Select(k => k.Point);

            var points1 = new List<PointF>();
            var points2 = new List<PointF>();

            for (int i = 0; i < matches.Size; i++)
            {
                var a = matches[i].ToArray();
                foreach (var e in a)
                {
                    points1.Add(keypoints1[e.QueryIdx].Point);
                    points2.Add(keypoints2[e.TrainIdx].Point);
                }
            }

            Mat f = new Mat();
            CvInvoke.FindFundamentalMat(new VectorOfPointF(points1.ToArray()), new VectorOfPointF(points2.ToArray()), f, FmType.Ransac, 0.1, 0.99);

            Matrix<float> fm = new Matrix<float>(f.Rows, f.Cols, f.Ptr);
            var E = calibration.Intrinsic.Transpose() * fm * calibration.Intrinsic;

            Mat u = new Mat();
            Mat w = new Mat();
            Mat v = new Mat();
            CvInvoke.SVDecomp(E, w, u, v, SvdFlag.ModifyA);

            //CvInvoke.Rodrigues(rotationVector32f, rotationMat);

            var points3d = new Mat();

            CvInvoke.TriangulatePoints(new Mat(), new Mat(), keypoints1, keypoints2, points3d);
        }
    }
}
