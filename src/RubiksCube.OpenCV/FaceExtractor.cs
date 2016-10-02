using OpenCvSharp;
using RubiksCube.OpenCV.Auxiliary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubiksCube.OpenCV
{
    public static class FaceExtractor
    {
        public static Mat Extract(Mat src, FaceCorners corners)
        {
            var widthA = Math.Sqrt(Math.Pow(corners.BottomRight.X - corners.BottomLeft.X, 2) + Math.Pow(corners.BottomRight.Y - corners.BottomLeft.Y, 2));
            var widthB = Math.Sqrt(Math.Pow(corners.TopRight.X - corners.TopLeft.X, 2) + Math.Pow(corners.TopRight.Y - corners.TopLeft.Y, 2));
            var maxWidth = Math.Max(widthA, widthB);

            var heightA = Math.Sqrt(Math.Pow(corners.TopRight.X - corners.BottomRight.X, 2) + Math.Pow(corners.TopRight.Y - corners.BottomRight.Y, 2));
            var heightB = Math.Sqrt(Math.Pow(corners.TopLeft.X - corners.BottomLeft.X, 2) + Math.Pow(corners.TopLeft.Y - corners.BottomLeft.Y, 2));
            var maxHeight = Math.Max(heightA, heightB);

            var perspectiveTransformationMatrix = Cv2.GetPerspectiveTransform(new List<Point2f> { corners.TopLeft, corners.TopRight, corners.BottomRight, corners.BottomLeft }, new List<Point2f> {
                new Point2f(0, 0),
                new Point2f((float)maxWidth-1, 0),
                new Point2f((float)maxWidth-1, (float)maxHeight-1),
                new Point2f(0, (float)maxHeight-1),
            });

            Mat warped = new Mat(new Size((float)maxWidth, (float)maxHeight), MatType.CV_16S);
            Cv2.WarpPerspective(src, warped, perspectiveTransformationMatrix, new Size((float)maxWidth, (float)maxHeight));
            return warped;
        }
    }
}
