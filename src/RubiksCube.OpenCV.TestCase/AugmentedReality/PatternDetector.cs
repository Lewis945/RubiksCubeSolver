using Emgu.CV;
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
        private PatternTrackingInfo _patternInfo;

        public Pattern Pattern { get { return _pattern; } }
        public PatternTrackingInfo PatternTrackingInfo { get { return _patternInfo; } }

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

            VectorOfVectorOfDMatch matches;
            Mat homography;

            FeaturesUtils.GetMatches(image, keypoints, descriptors, _pattern.Keypoints, _pattern.Descriptors, out matches, out homography);

            _patternInfo.homography = homography;

            var pts = Array.ConvertAll<Point, PointF>(_pattern.Points2d.ToArray(), (a) => { return a; });
            pts = CvInvoke.PerspectiveTransform(pts, homography);
            var points = Array.ConvertAll(pts, Point.Round);

            _patternInfo.points2d = new VectorOfPoint(points);

            _patternInfo.Draw2dContour(image, new MCvScalar(0, 200, 0));

            return true;
        }

        protected Pattern BuildPatternFromImage(Mat image)
        {
            _pattern = new Pattern();

            // Store original image in pattern structure
            _pattern.Size = new Size(image.Cols, image.Rows);
            _pattern.Frame = image.Clone();
            _pattern.GrayImg = GetGray(image);

            // Build 2d and 3d contours (3d contour lie in XY plane since it's planar)
            _pattern.Points2d = new VectorOfPoint(4);
            _pattern.Points3d = new VectorOfPoint3D32F(4);

            // Image dimensions
            int w = image.Cols;
            int h = image.Rows;

            // Normalized dimensions:
            float maxSize = Math.Max(w, h);
            float unitW = w / maxSize;
            float unitH = h / maxSize;

            _pattern.Points2d.Clear();
            _pattern.Points2d.Push(new Point[] {
                new Point(0, 0),
                new Point(w, 0),
                new Point(w, h),
                new Point(0, h)
            });

            _pattern.Points3d.Clear();
            _pattern.Points3d.Push(
                new MCvPoint3D32f[] {
                    new MCvPoint3D32f(-unitW, -unitH, 0),
                    new MCvPoint3D32f(unitW, -unitH, 0),
                    new MCvPoint3D32f(unitW, unitH, 0),
                    new MCvPoint3D32f(-unitW, unitH, 0)
            });

            FeaturesUtils.ExtractFeatures(_pattern.GrayImg, out _pattern.keypoints, out _pattern.descriptors);

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
