using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubiksCube.OpenCV
{
    public class Cube
    {
        // helper function:
        // finds a cosine of angle between vectors
        // from pt0->pt1 and from pt0->pt2
        static double Angle(Point pt1, Point pt2, Point pt0)
        {
            double dx1 = pt1.X - pt0.X;
            double dy1 = pt1.Y - pt0.Y;
            double dx2 = pt2.X - pt0.X;
            double dy2 = pt2.Y - pt0.Y;
            return (dx1 * dx2 + dy1 * dy2) / Math.Sqrt((dx1 * dx1 + dy1 * dy1) * (dx2 * dx2 + dy2 * dy2) + 1e-10);
        }

        public static void DrawSquares(Mat image, List<List<Point>> squares)
        {
            for (int i = 0; i < squares.Count; i++)
            {
                Point p = squares[i][0];
                int n = (int)squares[i].Count;
                int shift = 1;

                Rect r = Cv2.BoundingRect(InputArray.Create<Point>(squares[i]));
                r.X = r.X + r.Width / 4;
                r.Y = r.Y + r.Height / 4;
                r.Width = r.Width / 2;
                r.Height = r.Height / 2;

                Mat roi = new Mat(image, r);
                Scalar color = Scalar.Red;// Cv2.Mean(roi);
                Cv2.Polylines(image, squares, true, color, 2, LineTypes.AntiAlias, shift);

                var center = new Point(r.X + r.Width / 2, r.Y + r.Height / 2);
                Cv2.Ellipse(image, center, new Size(r.Width / 2, r.Height / 2), 0, 0, 360, color, 2, LineTypes.AntiAlias);
            }
        }

        // returns sequence of squares detected on the image.
        // the sequence is stored in the specified memory storage
        public static void FindSquares(Mat image, List<List<Point>> squares, bool inv = false)
        {
            squares.Clear();

            Mat grey = image.CvtColor(ColorConversionCodes.BGR2GRAY);
            Mat blur = grey.GaussianBlur(new Size(7, 7), 1.5, 1.5);
            Mat canny = blur.Canny(0, 30, 3);

            // find contours and store them all as a list
            Point[][] contours;
            HierarchyIndex[] hierarchIndex;
            canny.FindContours(out contours, out hierarchIndex, RetrievalModes.List, ContourApproximationModes.ApproxSimple);

            // test each contour
            for (int i = 0; i < contours.Count(); i++)
            {
                // approximate contour with accuracy proportional
                // to the contour perimeter
                //List<Point> approx;
                var approx = new List<Point>(contours[i]);
                Cv2.ApproxPolyDP(approx, 9, true);

                // square contours should have 4 vertices after approximation
                // relatively large area (to filter out noisy contours)
                // and be convex.
                // Note: absolute value of an area is used because
                // area may be positive or negative - in accordance with the
                // contour orientation
                if (approx.Count == 4 && Math.Abs(Cv2.ContourArea(approx)) > 5 && Cv2.IsContourConvex(approx))
                {
                    double maxCosine = 0;

                    for (int j = 2; j < 5; j++)
                    {
                        // find the maximum cosine of the angle between joint edges
                        double cosine = Math.Abs(Angle(approx[j % 4], approx[j - 2], approx[j - 1]));
                        maxCosine = Math.Max(maxCosine, cosine);
                    }

                    // if cosines of all angles are small
                    // (all angles are ~90 degree) then write quandrange
                    // vertices to resultant sequence
                    if (maxCosine < 0.3)
                        squares.Add(approx);
                }
            }
        }

    }
}