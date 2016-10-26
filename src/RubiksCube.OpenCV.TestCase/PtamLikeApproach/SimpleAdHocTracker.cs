using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubiksCube.OpenCV.TestCase.PtamLikeApproach
{
    class SimpleAdHocTracker
    {
        public static double RANSAC_THRESHOLD = 3;
        public static int MIN_INLIERS = 10;

        private bool _bootstrapping;

        private Mat _prevGray;

        private ORBDetector _detector;

        private VectorOfKeyPoint _bootstrapKp;
        private VectorOfKeyPoint _trackedFeatures;
        private VectorOfPoint3D32F _trackedFeatures3D;

        public SimpleAdHocTracker()
        {
            _detector = new ORBDetector();

            _prevGray = new Mat();

            _bootstrapKp = new VectorOfKeyPoint();
            _trackedFeatures = new VectorOfKeyPoint();
            _trackedFeatures3D = new VectorOfPoint3D32F();
        }

        public void Bootstrap(Mat img)
        {
            //Detect first features in the image (clear any current tracks)
            if (img.IsEmpty || !img.IsEmpty && img.NumberOfChannels != 3)
                throw new Exception("Image is not appropriate (Empty or wrong number of channels).");

            _bootstrapKp.Clear();
            _detector.DetectRaw(img, _bootstrapKp);

            _trackedFeatures = _bootstrapKp;

            CvInvoke.CvtColor(img, _prevGray, ColorConversion.Bgr2Gray);

            _bootstrapping = true;
        }

        public void BootstrapTrack(Mat img)
        {
            //Track detected features
            if (_prevGray.IsEmpty)
                throw new Exception("Previous frame is empty. Bootstrap first.");

            if (img.IsEmpty || !img.IsEmpty && img.NumberOfChannels != 3)
                throw new Exception("Image is not appropriate (Empty or wrong number of channels).");

            //VectorOfPointF corners = new VectorOfPointF();
            //VectorOfCvString status = new VectorOfCvString();
            //VectorOfFloat errors = new VectorOfFloat();
            //Mat currGray = new Mat();

            var corners = new VectorOfPointF();
            var status = new VectorOfByte();
            var errors = new VectorOfFloat();
            var currGray = new Mat();

            CvInvoke.CvtColor(img, currGray, ColorConversion.Bgr2Gray);

            try
            {
                CvInvoke.CalcOpticalFlowPyrLK(_prevGray, currGray, Utils.GetPointsVector(_trackedFeatures), corners, status, errors, new Size(11, 11), 0, new MCvTermCriteria(100));
            }
            catch (Exception ex)
            {
                var t = ex;
            }

            currGray.CopyTo(_prevGray);

            if (CvInvoke.CountNonZero(status) < status.Size * 0.8)
            {
                _bootstrapping = false;
                throw new Exception("Tracking failed.");
            }

            _trackedFeatures = Utils.GetKeyPointsVector(corners);

            Utils.KeepVectorsByStatus(_trackedFeatures, _bootstrapKp, status);

            Console.WriteLine($"{_trackedFeatures.Size} features survived optical flow.");

            if (_trackedFeatures.Size != _bootstrapKp.Size)
                throw new Exception("Tracked features vector size is not equal to bootstrapped one.");

            //verify features with a homography
            var inlier_mask = new VectorOfByte();
            var homography = new Mat();
            if (_trackedFeatures.Size > 4)
            {
                CvInvoke.FindHomography(Utils.GetPointsVector(_trackedFeatures), Utils.GetPointsVector(_bootstrapKp), homography, HomographyMethod.Ransac, RANSAC_THRESHOLD, inlier_mask);
            }

            int inliers_num = CvInvoke.CountNonZero(inlier_mask);

            Console.WriteLine($"{inliers_num} features survived homography.");

            var m = new Matrix<float>(homography.Rows, homography.Cols, homography.Ptr);

            if (inliers_num != _trackedFeatures.Size && inliers_num >= 4 && !homography.IsEmpty)
            {
                Utils.KeepVectorsByStatus(_trackedFeatures, _bootstrapKp, inlier_mask);
            }
            else if (inliers_num < MIN_INLIERS)
            {
                Console.WriteLine("Not enough features survived homography.");
                _bootstrapping = false;
                return;
            }

            var bootstrap_kp_orig = new VectorOfKeyPoint(_bootstrapKp.ToArray());
            var trackedFeatures_orig = new VectorOfKeyPoint(_trackedFeatures.ToArray());

        }
    }
}
