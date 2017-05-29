using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubiksCube.OpenCV
{
    //http://docs.OpenCV.org/2.4/doc/tutorials/imgproc/histograms/histogram_comparison/histogram_comparison.html
    public static class FaceUniquenessDetector
    {
        private static List<Mat> _faces;
        private static List<Color> _facesMiddleColors;

        public static void Init()
        {
            _faces = new List<Mat>();
            _facesMiddleColors = new List<Color>();
        }

        public static bool IsUnique(Mat face)
        {
            if (_faces.Count == 0)
            {
                var color1 = ColorsExtractor.ExtractMiddleColor(face);
                color1 = ColorsExtractor.ClosestColorRgb(color1);

                _faces.Add(face);
                _facesMiddleColors.Add(color1);
                return true;
            }

            /// Using 50 bins for hue and 60 for saturation
            int h_bins = 50;
            int s_bins = 60;
            int[] histSize = new int[2] { h_bins, s_bins };

            // hue varies from 0 to 179, saturation from 0 to 255
            var h_ranges = new Rangef(0, 180);
            var s_ranges = new Rangef(0, 256);

            var ranges = new Rangef[] { h_ranges, s_ranges };

            // Use the o-th and 1-st channels
            int[] channels = new int[2] { 0, 1 };

            var faceHsv = face.CvtColor(ColorConversionCodes.BGR2HSV);

            var faceHistogram = new Mat();
            Cv2.CalcHist(new Mat[] { faceHsv }, channels, new Mat(), faceHistogram, 2, histSize, ranges, true, false);
            Cv2.Normalize(faceHistogram, faceHistogram, 0, 1, NormTypes.MinMax, -1, new Mat());

            var color = ColorsExtractor.ExtractMiddleColor(face);

            foreach (var f in _faces)
            {
                var fHsv = f.CvtColor(ColorConversionCodes.BGR2HSV);

                var fHistogram = new Mat();
                Cv2.CalcHist(new Mat[] { fHsv }, channels, new Mat(), fHistogram, 2, histSize, ranges, true, false);
                Cv2.Normalize(fHistogram, fHistogram, 0, 1, NormTypes.MinMax, -1, new Mat());

                double correl = Cv2.CompareHist(faceHistogram, fHistogram, HistCompMethods.Correl);

                //if (correl > 0.5) return false;
                bool contains = _facesMiddleColors.Contains(color);
                if (correl > 0.4 && contains) return false;
                //if (correl > 0.65) return false;
            }

            color = ColorsExtractor.ExtractMiddleColor(face);

            _faces.Add(face);
            _facesMiddleColors.Add(color);
            return true;
        }
    }
}
