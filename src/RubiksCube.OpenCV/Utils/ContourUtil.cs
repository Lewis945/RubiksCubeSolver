using OpenCvSharp;
using RubiksCube.OpenCV;
using RubiksCube.OpenCV.Auxiliary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubiksCube.OpenCV.Utils
{
    public static class ContourUtil
    {
        public static bool CheckRectCorners(Point[] contour)
        {
            var epsilon = 0.1 * Cv2.ArcLength(contour, true);
            var approximation = Cv2.ApproxPolyDP(contour, epsilon, true);

            var hasFourCorners = approximation.Length == 4;
            var isConvex = Cv2.IsContourConvex(approximation);

            if (hasFourCorners && isConvex) return true;
            else return false;
        }

        public static List<Point[]> FindContours(Mat tresh)
        {
            var contours = tresh.FindContoursAsArray(RetrievalModes.List, ContourApproximationModes.ApproxSimple);

            var imageSize = tresh.Width * tresh.Height;

            var areas = new List<double>();
            var matchedCountours = new List<Point[]>();

            foreach (var contour in contours)
            {
                var area = Cv2.ContourArea(contour);
                var p = area / imageSize;
                if (p > Config.LowerThresh && p < Config.Upperthresh && CheckRectCorners(contour))
                {
                    matchedCountours.Add(contour);
                    areas.Add(area);
                }
            }

            return matchedCountours;
        }

        public static double GetAngle(Point[] contour, Mat dst = null)
        {
            var tl = contour.FirstOrDefault(pt => pt.X + pt.Y == contour.Min(p => p.X + p.Y));
            var br = contour.FirstOrDefault(pt => pt.X + pt.Y == contour.Max(p => p.X + p.Y));
            var tr = contour.FirstOrDefault(pt => pt.X - pt.Y == contour.Min(p => p.X - p.Y));
            var bl = contour.FirstOrDefault(pt => pt.X - pt.Y == contour.Max(p => p.X - p.Y));

            //Cv2.Line(dst, tl, br, Scalar.Black);

            //var angle = Math.Atan((tl.Y - br.Y) / (tl.X - br.X)) * 180 / Math.PI;

            //Cv2.Line(dst, tr, bl, Scalar.Black);

            var l1 = new LineSegment(tl, br);
            if (dst != null)
                Cv2.Line(dst, l1.P1, l1.P2, Scalar.Black);

            var l2 = new LineSegment(new Point(0, 5), new Point(600, 5));
            if (dst != null)
                Cv2.Line(dst, l2.P1, l2.P2, Scalar.Black);

            var angle = LineSegment.GetAngle(l1, l2);
            //if (dst != null)
            //    Cv2.PutText(dst, ((int)angle).ToString(), new Point2f(tl.X + 200, tl.Y + 5), HersheyFonts.HersheyPlain, 1, Scalar.YellowGreen);

            return angle;
        }
    }
}
