using OpenCvSharp;
using RubiksCube.OpenCV.Auxiliary;
using RubiksCube.OpenCV.Colors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubiksCube.OpenCV
{
    public static class ColorsExtractor
    {
        public static List<Color> Colors = new List<Color>
        {
            Color.Black,
            Color.Blue,
            Color.White,
            Color.Yellow,
            Color.Red,
            Color.Orange,
            Color.Green
        };

        public static FaceColors Extract(Mat face, Guid jobId)
        {
            var faceId = Guid.NewGuid();

            var dir = new DirectoryInfo($"Results\\{jobId}\\Extracted\\Unique\\{faceId}");
            if (!dir.Exists) dir.Create();

            var bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(face);
            bitmap.Save($"Results\\{jobId}\\Extracted\\Unique\\{faceId}\\face.png", ImageFormat.Png);
            bitmap.Save($"Results\\{jobId}\\Extracted\\Unique\\{faceId}.png", ImageFormat.Png);

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

                    //var colors = new List<Color>();
                    //for (int k = centerX - 20; k < centerX + 20; k++)
                    //    for (int p = centerY - 20; p < centerY + 20; p++)
                    //    {
                    //        var intensity = sub.At<Vec3b>(k, p);

                    //        byte blue = intensity.Item0;
                    //        byte green = intensity.Item1;
                    //        byte red = intensity.Item2;

                    //        var color1 = Color.FromArgb(red, green, blue);
                    //        color1 = ClosestColorRgb(color1);
                    //        colors.Add(color1);
                    //    }

                    //var color = colors.GroupBy(s => s)
                    //         .OrderByDescending(s => s.Count())
                    //         .First().Key;

                    var intensity = sub.At<Vec3b>(centerX, centerY);

                    byte blue = intensity.Item0;
                    byte green = intensity.Item1;
                    byte red = intensity.Item2;

                    var color = Color.FromArgb(red, green, blue);

                    var c1 = ClosestColorRgb(color);
                    var c2 = ClosestColorHue(color);
                    var c3 = ClosestColorHsb(color);

                    var list = new List<Color> { c1, c2, c3 };
                    var max = list.GroupBy(s => s)
                                 .OrderByDescending(s => s.Count())
                                 .First().Key;

                    var newColor = FileColorExtractor.GetColor(FileColorExtractor.GetColor(color.ToHexString()));

                    //if (max == Color.Orange && newColor != Color.Orange)
                    //    newColor = Color.Orange;

                    result.SetColor(i.ToString() + "-" + j.ToString(), newColor);

                    bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(sub);
                    //bitmap.Save($"Results\\{jobId}\\Extracted\\Unique\\{faceId}\\sub-{i}-{j}({newColor.Name}).png", ImageFormat.Png);
                    bitmap.Save($"Results\\{jobId}\\Extracted\\Unique\\{faceId}\\sub-{i}-{j}({newColor.Name})({c1.Name}-{c2.Name}-{c3.Name})({color.R}-{color.G}-{color.B}).png", ImageFormat.Png);

                }
            }

            return result;
        }

        public static Color ExtractMiddleColor(Mat face)
        {
            var subWidth = face.Width / Config.FaceDimension;
            var subHeight = face.Height / Config.FaceDimension;

            var centerX = subWidth / 2;
            var centerY = subHeight / 2;

            int i = (Config.FaceDimension - 1) / 2;
            int j = (Config.FaceDimension - 1) / 2;

            var sub = new Mat(face, new Rect(i * subWidth, j * subHeight, subWidth, subHeight));

            var intensity = sub.At<Vec3b>(centerX, centerY);

            byte blue = intensity.Item0;
            byte green = intensity.Item1;
            byte red = intensity.Item2;

            var color = Color.FromArgb(red, green, blue);

            var c1 = ClosestColorRgb(color);
            var c2 = ClosestColorHue(color);
            var c3 = ClosestColorHsb(color);

            var list = new List<Color> { c1, c2, c3 };
            var max = list.GroupBy(s => s)
                         .OrderByDescending(s => s.Count())
                         .First().Key;

            var cl = FileColorExtractor.GetColor(color.ToHexString());
            var newColor = FileColorExtractor.GetColor(cl);

            //if (max == Color.Orange && newColor != Color.Orange)
            //    newColor = Color.Orange;

            return newColor;
        }

        #region Retrieve Color Methods

        public static string ToHexString(this Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

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
            var color = Colors[index];
            return color == Color.Black ? Color.White : color;
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
