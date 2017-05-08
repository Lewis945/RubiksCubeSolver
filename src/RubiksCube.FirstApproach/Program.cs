using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubiksCube.FirstApproach
{
    class Program
    {
        private static Mat src;
        private static Mat grey;
        private static Mat tresh;

        //private static Mat dst;

        private static int t1 = 0;
        private static int t2 = 255;
        private static int asize = 3;

        private static Mat GetTresh()
        {
            var tr = grey.Canny(t1, t2, asize, false);
            //var tr = TresholdingUtil.AdaptiveTresholding(grey);
            return tr;
        }

        private static void ReadImage()
        {
            src = new Mat("Images/IMG_20160315_222740.jpg", ImreadModes.Unchanged);
            src = ImageUtil.ProportionalImageResize(src, 600);

            grey = src.CvtColor(ColorConversionCodes.BGR2GRAY);
            tresh = GetTresh();

            using (var w = new Window("src", src))
            {
                using (var wd = new Window("dst", tresh))
                {
                    wd.CreateTrackbar("t1", 0, 100, on_trackbart1);
                    wd.CreateTrackbar("t2", 100, 255, on_trackbart2);

                    Init();

                    Cv2.WaitKey();
                }
            }
        }

        private static void ReadVideo()
        {
            // Opens MP4 file (ffmpeg is probably needed)
            var capture = new VideoCapture("Videos/cube_003.avi");

            int sleepTime = (int)Math.Round(1000 / capture.Fps);

            using (var window = new Window("capture"))
            {
                using (var image = new Mat()) // Frame image buffer
                {
                    // When the movie playback reaches end, Mat.data becomes NULL.
                    while (true)
                    {
                        capture.Read(image); // same as cvQueryFrame
                        if (image.Empty())
                            break;

                        src = ImageUtil.GetCopy(image);
                        grey = image.CvtColor(ColorConversionCodes.BGR2GRAY);
                        tresh = GetTresh();

                        //window.ShowImage(tresh);

                        var corners = FaceDetector.GetFaceCorners(src, tresh, window);
                        if (corners != null && !corners.Rotated)
                        {
                            var face = FaceExtractor.Extract(src, corners);
                            if (FaceUniquenessDetector.IsUnique(face))
                            {
                                var bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(face);
                                bitmap.Save("Results\\face" + Guid.NewGuid() + ".png", ImageFormat.Png);

                                var colors = ColorsExtractor.Extract(face);
                                var t = colors.GetColor("0-2");
                            }
                        }

                        var k = Cv2.WaitKey(sleepTime);
                        if (k == 27) break;
                    }
                }
            }
        }

        private static void Init()
        {
            tresh = GetTresh();
            Cv2.ImShow("dst", tresh);
            FaceDetector.GetFaceCorners(src, tresh);
        }

        private static void on_trackbart1(int pos, object userdata)
        {
            t1 = pos;
            Init();
        }

        private static void on_trackbart2(int pos, object userdata)
        {
            t2 = pos;
            Init();
        }

        static void Main(string[] args)
        {
            //ReadImage();
            ReadVideo();
        }
    }
}
