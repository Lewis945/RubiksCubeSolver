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
        public static List<Color> Colors = new List<Color>
        {
            Color.Blue,
            Color.White,
            Color.Yellow,
            Color.Red,
            Color.Orange,
            Color.Green
        };

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

                    var color = Color.FromArgb(red, green, blue);
                    result.SetColor(i.ToString() + "-" + j.ToString(), color);
                }
            }

            return result;
        }

        #region Retrieve Color Methods

        /// <summary>
        /// Closed match for hues only
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Color ClosestColorHue(Color target)
        {
            var hue1 = target.GetHue();
            var diffs = Colors.Select(n => GetHueDistance(n.GetHue(), hue1));
            var diffMin = diffs.Min(n => n);
            var index = diffs.ToList().FindIndex(n => n == diffMin);
            return Colors[index];
        }

        /// <summary>
        /// Closed match in RGB space
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Color ClosestColorRgb(Color target)
        {
            var colorDiffs = Colors.Select(n => ColorDiff(n, target)).Min(n => n);
            var index = Colors.FindIndex(n => ColorDiff(n, target) == colorDiffs);
            return Colors[index];
        }

        /// <summary>
        /// Weighed distance using hue, saturation and brightness
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Color ClosestColorHsb(Color target)
        {
            float hue1 = target.GetHue();
            var num1 = ColorNum(target);
            var diffs = Colors.Select(n => Math.Abs(ColorNum(n) - num1) +
                                           GetHueDistance(n.GetHue(), hue1));
            var diffMin = diffs.Min(x => x);
            var index = diffs.ToList().FindIndex(n => n == diffMin);

            return Colors[index];
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Color brightness as perceived
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static float GetBrightness(Color c)
        {
            return (c.R * 0.299f + c.G * 0.587f + c.B * 0.114f) / 256f;
        }

        /// <summary>
        /// Distance between two hues
        /// </summary>
        /// <param name="hue1"></param>
        /// <param name="hue2"></param>
        /// <returns></returns>
        private static float GetHueDistance(float hue1, float hue2)
        {
            float d = Math.Abs(hue1 - hue2); return d > 180 ? 360 - d : d;
        }

        /// <summary>
        /// Weighed only by saturation and brightness (from my trackbars)
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static float ColorNum(Color c)
        {
            float factorSat = 1;
            float factorBri = 1;

            return c.GetSaturation() * factorSat +
                        GetBrightness(c) * factorBri;
        }

        /// <summary>
        /// Distance in RGB space
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        private static int ColorDiff(Color c1, Color c2)
        {
            return (int)Math.Sqrt((c1.R - c2.R) * (c1.R - c2.R)
                                   + (c1.G - c2.G) * (c1.G - c2.G)
                                   + (c1.B - c2.B) * (c1.B - c2.B));
        }

        #endregion
    }
}
