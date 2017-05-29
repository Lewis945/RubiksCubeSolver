using OpenCvSharp;
using RubiksCube.OpenCV.Utils;
using RubiksCube.OpenCV.Auxiliary;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubiksCube.OpenCV
{
    public static class FaceDetector
    {
        public static Point GetMassCenter(Point[] contour)
        {
            var moments = Cv2.Moments(contour, false);
            var massCenter = new Point(moments.M10 / moments.M00, moments.M01 / moments.M00);
            return massCenter;
        }

        public static FaceCorners GetFaceCorners(Mat src, Mat tresh, Guid jobId, Window w = null)
        {
            FaceCorners result = null;

            var dst = ImageUtil.GetCopy(src);
            var copy = ImageUtil.GetCopy(tresh);

            var matched_countours = ContourUtil.FindContours(copy);

            matched_countours = matched_countours.Distinct(new LambdaComparer<Point[]>((x, y) =>
            {
                var mx = GetMassCenter(x);
                var my = GetMassCenter(y);

                var dx = Math.Abs(mx.X - my.X);
                var dy = Math.Abs(mx.Y - my.Y);
                double distance = Math.Sqrt(dx * dx + dy * dy);

                return mx == my || distance < 5 && distance > 0;
            })).ToList();

            //Cv2.DrawContours(dst, matched_countours, -1, Scalar.Purple, 2);

            foreach (var c in matched_countours)
            {
                var angle = ContourUtil.GetAngle(c, dst);
            }

            var cts = matched_countours.GroupBy(c => ContourUtil.GetAngle(c));

            int i = 1;
            foreach (var group in cts)
            {
                var count = group.Count();

                //foreach (var c in group)
                //{
                //    Cv2.PutText(dst, "g" + ((int)i).ToString() + " " + group.Key, new Point2f(c[0].X + 20, c[0].Y + 20), HersheyFonts.HersheyPlain, 1, Scalar.SpringGreen);
                //}
                //i++;

                var all = group.SelectMany(g => g).ToList();

                if (count == 9)
                {
                    //tl
                    var p1 = all.FirstOrDefault(pt => pt.X + pt.Y == all.Min(p => p.X + p.Y));
                    //br
                    var p2 = all.FirstOrDefault(pt => pt.X + pt.Y == all.Max(p => p.X + p.Y));
                    //tr
                    var p3 = all.FirstOrDefault(pt => pt.X - pt.Y == all.Min(p => p.X - p.Y));
                    //bl
                    var p4 = all.FirstOrDefault(pt => pt.X - pt.Y == all.Max(p => p.X - p.Y));

                    var points = new List<Point>() { p1, p2, p3, p4 };

                    points = points.Distinct(new LambdaComparer<Point>((a, b) =>
                    {
                        var dx = Math.Abs(a.X - b.X);
                        var dy = Math.Abs(a.Y - b.Y);
                        double distance = Math.Sqrt(dx * dx + dy * dy);

                        return distance < 20 && distance > 0;
                    })).ToList();

                    // if two of the points from [tl,br,tr,bl] are ~ equal then we need to find alternatives
                    // like the most left or the most top element
                    if (points.Count < 4)
                    {
                        // top most
                        p1 = all.FirstOrDefault(pt => pt.Y == all.Min(p => p.Y));
                        // left most
                        p2 = all.FirstOrDefault(pt => pt.X == all.Min(p => p.X));
                        // right most
                        p3 = all.FirstOrDefault(pt => pt.X == all.Max(p => p.X));
                        // bottom most
                        p4 = all.FirstOrDefault(pt => pt.Y == all.Max(p => p.Y));

                        result = new FaceCorners { Rotated = true, TopMost = p1, LeftMost = p2, RightMost = p3, BottomMost = p4 };
                    }
                    else
                    {
                        result = new FaceCorners { TopLeft = p1, BottomRight = p2, TopRight = p3, BottomLeft = p4 };
                    }

                    Cv2.Circle(dst, p1, 10, Scalar.Red);
                    Cv2.Circle(dst, p2, 10, Scalar.Red);
                    Cv2.Circle(dst, p3, 10, Scalar.Red);
                    Cv2.Circle(dst, p4, 10, Scalar.Red);

                    var bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(dst);
                    bitmap.Save($"Results\\{jobId}\\Detected\\face-corners-" + Guid.NewGuid() + ".jpg", ImageFormat.Jpeg);
                }

                if (count >= 5 && count < 9)
                {
                    result = FindFaceCornersForContour(group.FirstOrDefault(), all.ToArray());

                    if (result != null)
                    {
                        Cv2.Circle(dst, result.TopLeft, 10, Scalar.Red);
                        Cv2.Circle(dst, result.BottomLeft, 10, Scalar.Red);
                        Cv2.Circle(dst, result.TopRight, 10, Scalar.Red);
                        Cv2.Circle(dst, result.BottomRight, 10, Scalar.Red);

                        var bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(dst);
                        bitmap.Save($"Results\\{jobId}\\Detected\\face-corners-" + Guid.NewGuid() + ".jpg", ImageFormat.Jpeg);
                    }
                }
            }

            if (w == null) Cv2.ImShow("src", dst);
            else w.ShowImage(dst);

            return result;
        }

        private static FaceCorners FindFaceCornersForContour(Point[] contour, Point[] contours)
        {
            var rect = Cv2.BoundingRect(contour);

            int cubieWidth = rect.Width;
            int cubieHeight = rect.Height;

            int minX = contours.Min(p => p.X);
            int maxX = contours.Max(p => p.X);

            if (!(maxX - minX > 2.9 * rect.Width))
                return null;

            int minY = contours.Min(p => p.Y);
            int maxY = contours.Max(p => p.Y);

            if (!(maxY - minY > 2.9 * rect.Height))
                return null;

            while (true)
            {
                bool sutisfied = contours.All(p => p.X > rect.X && p.Y > rect.Y && p.X < (rect.X + rect.Width) && p.Y < (rect.Y + rect.Height));
                if (sutisfied)
                    break;

                if (contours.Any(c => c.X <= rect.X))
                    rect.X -= 5;

                if (contours.Any(c => c.Y <= rect.Y))
                    rect.Y -= 5;

                if (contours.Any(c => c.X >= (rect.X + rect.Width)))
                    rect.Width += 5;

                if (contours.Any(c => c.Y >= (rect.Y + rect.Height)))
                    rect.Height += 5;
            }

            var rectPoints = new[]
            {
                rect.Location,
                new Point(rect.X, rect.Y + rect.Height),
                new Point(rect.X + rect.Width, rect.Y),
                new Point(rect.X + rect.Width, rect.Y + rect.Height)
            };

            var thresh = cubieWidth - cubieWidth / 10;

            for (int i = 0; i < rectPoints.Length; i++)
            {
                var point = rectPoints[i];
                var orderedByDistance = contours.OrderBy(x => GetDistance(x, point)).ToList();
                var closest = orderedByDistance.FirstOrDefault();
                var distance = GetDistance(closest, point);
                //while (distance > thresh)
                //{
                //    var difY = closest.Y - point.Y;
                //    var difX = closest.X - point.X;

                //    if (difX < 0 && difY < 0)
                //    {
                //        point.X -= 5;
                //        point.Y -= 5;
                //    }
                //    else if (difX > 0 && difY > 0)
                //    {
                //        point.X += 5;
                //        point.Y += 5;
                //    }
                //    else if (difX < 0)
                //    {
                //        point.X -= 5;
                //    }
                //    else if (difY < 0)
                //    {
                //        point.Y -= 5;
                //    }
                //    else if (difX > 0)
                //    {
                //        point.X += 5;
                //    }
                //    else if (difY > 0)
                //    {
                //        point.Y += 5;
                //    }

                //    distance = GetDistance(closest, point);
                //}

                //rectPoints[i] = point;

                if (distance > cubieWidth / 3)
                    continue;

                rectPoints[i] = closest;
            }

            return new FaceCorners(rectPoints);
        }

        private static Decimal GetDistance(Point p1, Point p2)
        {
            return Convert.ToDecimal(Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2)));
        }
    }
}
