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

        public static FaceCorners GetFaceCorners(Mat src, Mat tresh, Window w = null)
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
                //foreach (var c in group)
                //{
                //    Cv2.PutText(dst, "g" + ((int)i).ToString() + " " + group.Key, new Point2f(c[0].X + 20, c[0].Y + 20), HersheyFonts.HersheyPlain, 1, Scalar.SpringGreen);
                //}
                //i++;

                var all = group.SelectMany(g => g);

                if (group.Count() == 9)
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
                    bitmap.Save("Results\\face-corners-" + Guid.NewGuid() + ".jpg", ImageFormat.Jpeg);
                }
            }

            if (w == null) Cv2.ImShow("src", dst);
            else w.ShowImage(dst);

            return result;
        }
    }
}
