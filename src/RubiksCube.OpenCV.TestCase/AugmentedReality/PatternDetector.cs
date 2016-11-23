﻿using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.XFeatures2D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubiksCube.OpenCV.TestCase.AugmentedReality
{
    public class PatternDetector
    {
        private Pattern _pattern;
        private readonly PatternTrackingInfo _patternInfo;

        /// <summary>
        /// 
        /// </summary>
        public Pattern Pattern => _pattern;

        /// <summary>
        /// 
        /// </summary>
        public PatternTrackingInfo PatternTrackingInfo => _patternInfo;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="patternImage"></param>
        public PatternDetector(Mat patternImage)
        {
            _pattern = BuildPatternFromImage(patternImage);
            _patternInfo = new PatternTrackingInfo();
        }

        public bool FindPattern(Mat image)
        {
            VectorOfKeyPoint keypoints;
            Mat descriptors;

            var gray = GetGray(image);

            FeaturesUtils.ExtractFeatures(gray, out keypoints, out descriptors);

            Features2DToolbox.DrawKeypoints(gray, keypoints, image, new Bgr(Color.Red), Features2DToolbox.KeypointDrawType.NotDrawSinglePoints);

            VectorOfVectorOfDMatch matches;
            Mat homography;

            FeaturesUtils.GetMatches(keypoints, descriptors, _pattern.Keypoints, _pattern.Descriptors, out matches, out homography);

            _patternInfo.Homography = homography;

            var pts = Array.ConvertAll<Point, PointF>(_pattern.Points2d.ToArray(), a => a);
            pts = CvInvoke.PerspectiveTransform(pts, homography);
            var points = Array.ConvertAll(pts, Point.Round);

            _patternInfo.Points2d = new VectorOfPoint(points);

            _patternInfo.Draw2dContour(image, new MCvScalar(0, 200, 0));

            return true;
        }

        protected Pattern BuildPatternFromImage(Mat image)
        {
            _pattern = new Pattern
            {
                Size = new Size(image.Cols, image.Rows),
                Frame = image.Clone(),
                GrayImg = GetGray(image),
                Points2d = new VectorOfPoint(4),
                Points3d = new VectorOfPoint3D32F(4)
            };

            // Build 2d and 3d contours (3d contour lie in XY plane since it's planar)

            // Image dimensions
            int w = image.Cols;
            int h = image.Rows;

            // Normalized dimensions:
            float maxSize = Math.Max(w, h);
            float unitW = w / maxSize;
            float unitH = h / maxSize;

            _pattern.Points2d.Clear();
            _pattern.Points2d.Push(new[] {
                new Point(0, 0),
                new Point(w, 0),
                new Point(w, h),
                new Point(0, h)
            });

            _pattern.Points3d.Clear();
            _pattern.Points3d.Push(
                new[] {
                    new MCvPoint3D32f(-unitW, -unitH, 0),
                    new MCvPoint3D32f(unitW, -unitH, 0),
                    new MCvPoint3D32f(unitW, unitH, 0),
                    new MCvPoint3D32f(-unitW, unitH, 0)
            });

            VectorOfKeyPoint keypoints;
            Mat descriptors;

            FeaturesUtils.ExtractFeatures(_pattern.GrayImg, out keypoints, out descriptors);

            _pattern.Keypoints = keypoints;
            _pattern.Descriptors = descriptors;

            return _pattern;
        }

        protected static Mat GetGray(Mat image)
        {
            var gray = new Mat();

            if (image.NumberOfChannels == 3)
                CvInvoke.CvtColor(image, gray, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
            else if (image.NumberOfChannels == 4)
                CvInvoke.CvtColor(image, gray, Emgu.CV.CvEnum.ColorConversion.Bgra2Gray);
            else if (image.NumberOfChannels == 1)
                gray = image;

            return gray;
        }
    }
}
