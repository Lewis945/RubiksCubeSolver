using OpenCvSharp;
using RubiksCube.OpenCV.Auxiliary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubiksCube.OpenCV
{
    public static class ColorsExtractor
    {
        public static FaceColors Extract(Mat face)
        {
            var result = new FaceColors();

            var subWidth = face.Width / Config.FaceDimension;
            var subHeight = face.Height / Config.FaceDimension;

            var centerX = subWidth / 2;
            var centerY = subHeight / 2;

            for (int i = 0; i < Config.FaceDimension; i++)
            {
                for (int j = 0; j < Config.FaceDimension; j++)
                {
                    var sub = new Mat(face, new Rect(i * subWidth, j * subHeight, subWidth, subHeight));

                    var bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(sub);
                    bitmap.Save("Results\\sub-" + i + j + ".png", ImageFormat.Png);

                    var intensity = sub.At<Vec3b>(centerX, centerY);

                    byte blue = intensity.Item0;
                    byte green = intensity.Item1;
                    byte red = intensity.Item2;

                    Color c = Color.FromArgb(red, green, blue);
                    result.SetColor(i.ToString() + "-" + j.ToString(), c);
                }
            }

            return result;
        }
    }
}
