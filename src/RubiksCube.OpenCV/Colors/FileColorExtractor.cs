using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;

namespace RubiksCube.OpenCV.Colors
{
    public static class FileColorExtractor
    {
        public static List<List<object>> _colors;
        public static List<List<string>> _shades;

        public static void Init()
        {
            if (_colors == null)
            {
                using (var file = File.OpenText(@"Colors\colors.json"))
                {
                    var serializer = new JsonSerializer();
                    _colors = (List<List<object>>)serializer.Deserialize(file, typeof(List<List<object>>));
                }

                //var toDelete = new List<int>();
                for (var i = 0; i < _colors.Count; i++)
                {
                    //if (_colors[i][2].Equals("Violet") || _colors[i][2].Equals("Brown"))
                    //{
                    //    toDelete.Add(i);
                    //    continue;
                    //}

                    var color = "#" + _colors[i][0];
                    var rgb = GetRgb(color);
                    var hsl = GetHsl(color);

                    _colors[i].Add(rgb.Item1);
                    _colors[i].Add(rgb.Item2);
                    _colors[i].Add(rgb.Item3);
                    _colors[i].Add(hsl.Item1);
                    _colors[i].Add(hsl.Item2);
                    _colors[i].Add(hsl.Item3);
                }

                //_colors.RemoveAll(v => toDelete.Contains(_colors.IndexOf(v)));
            }

            if (_shades == null)
            {
                using (var file = File.OpenText(@"Colors\shades.json"))
                {
                    var serializer = new JsonSerializer();
                    _shades = (List<List<string>>)serializer.Deserialize(file, typeof(List<List<string>>));
                }
            }
        }

        public static Tuple<string, string, string, string, bool> GetColor(string color)
        {
            Init();

            color = color.ToUpper();

            if ((color.Length < 3) || (color.Length > 7))
                throw new Exception("Invalid Color: " + color);
            if (color.Length % 3 == 0)
                color = "#" + color;
            if (color.Length == 4)
                color = "#" + color.Substring(1, 1) + color.Substring(1, 1) + color.Substring(2, 1) +
                        color.Substring(2, 1) + color.Substring(3, 1) + color.Substring(3, 1);

            var rgb = GetRgb(color);
            var r = rgb.Item1;
            var g = rgb.Item2;
            var b = rgb.Item3;
            var hsl = GetHsl(color);
            var h = hsl.Item1;
            var s = hsl.Item2;
            var l = hsl.Item3;
            var cl = -1;
            var df = -1;

            for (var i = 0; i < _colors.Count; i++)
            {
                if (color == "#" + _colors[i][0])
                    return new Tuple<string, string, string, string, bool>("#" + _colors[i][0], (string)_colors[i][1],
                        ShadeRgb((string)_colors[i][2]), (string)_colors[i][2], true);

                var ndf1 =
                    (int)
                    (Math.Pow(r - (int)_colors[i][3], 2) + Math.Pow(g - (int)_colors[i][4], 2) +
                     Math.Pow(b - (int)_colors[i][5], 2));
                var ndf2 =
                    (int)
                    (Math.Abs(Math.Pow(h - (int)_colors[i][6], 2)) + Math.Pow(s - (int)_colors[i][7], 2) +
                     Math.Abs(Math.Pow(l - (int)_colors[i][8], 2)));
                var ndf = ndf1 + ndf2 * 2;
                if ((df < 0) || (df > ndf))
                {
                    df = ndf;
                    cl = i;
                }
            }

            if (cl < 0)
                throw new Exception("Invalid Color: " + color);

            return new Tuple<string, string, string, string, bool>("#" + _colors[cl][0], (string)_colors[cl][1],
                ShadeRgb((string)_colors[cl][2]), (string)_colors[cl][2], false);
        }

        public static Color GetColor(Tuple<string, string, string, string, bool> data)
        {
            if (data.Item4.Equals("White") || data.Item4.Equals("Grey") || data.Item4.Equals("Brown") || data.Item4.Equals("Black"))
                return Color.White;

            if (data.Item4.Equals("Red"))
                return Color.Red;

            if (data.Item4.Equals("Green"))
                return Color.Green;

            if (data.Item4.Equals("Blue"))
                return Color.Blue;

            if (data.Item4.Equals("Yellow"))
                return Color.Yellow;

            if (data.Item4.Equals("Orange"))
                return Color.Orange;

            throw new Exception("Color not found!");
        }

        // adopted from: Farbtastic 1.2
        // http://acko.net/dev/farbtastic
        public static Tuple<int, int, int> GetHsl(string color)
        {
            var rgb = GetRgb(color);
            var r = rgb.Item1 / 255d;
            var g = rgb.Item2 / 255d;
            var b = rgb.Item3 / 255d;

            var min = Math.Min(r, Math.Min(g, b));
            var max = Math.Max(r, Math.Max(g, b));
            var delta = max - min;
            var l = (min + max) / 2;

            double s = 0;
            if ((l > 0) && (l < 1))
                s = delta / (l < 0.5 ? 2 * l : 2 - 2 * l);

            double h = 0;
            if (delta > 0)
            {
                if ((max == r) && (max != g)) h += (g - b) / delta;
                if ((max == g) && (max != b)) h += 2 + (b - r) / delta;
                if ((max == b) && (max != r)) h += 4 + (r - g) / delta;
                h /= 6;
            }

            return new Tuple<int, int, int>((int)Math.Floor(h * 255), (int)Math.Floor(s * 255), (int)Math.Floor(l * 255));
        }

        // adopted from: Farbtastic 1.2
        // http://acko.net/dev/farbtastic
        public static Tuple<int, int, int> GetRgb(string color)
        {
            var red = int.Parse(color.Substring(1, 2), NumberStyles.AllowHexSpecifier);
            var green = int.Parse(color.Substring(3, 2), NumberStyles.AllowHexSpecifier);
            var blue = int.Parse(color.Substring(5, 2), NumberStyles.AllowHexSpecifier);

            return new Tuple<int, int, int>(red, green, blue);
        }

        public static string ShadeRgb(string shadename)
        {
            foreach (var t in _shades)
                if (shadename == t[1])
                    return "#" + t[0];
            return "#000000";
        }
    }
}
