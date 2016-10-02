using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubiksCube.OpenCV.Utils
{
    public static class ImageUtil
    {
        public static Mat ProportionalImageResize(Mat image, int width)
        {
            var r = (float)width / image.Width;
            var dim = new Size(width, (image.Height * r));
            return image.Resize(dim, 0, 0, InterpolationFlags.Area);
        }

        public static Mat GetCopy(Mat img)
        {
            var dst = new Mat();
            img.CopyTo(dst);
            return dst;
        }
    }
}
