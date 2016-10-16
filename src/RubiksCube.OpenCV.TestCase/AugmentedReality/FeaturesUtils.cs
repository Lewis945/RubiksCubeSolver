using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.XFeatures2D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubiksCube.OpenCV.TestCase.AugmentedReality
{
    public static class FeaturesUtils
    {
        private static Feature2D _featureDetector;
        private static Feature2D _descriptorsComputer;

        public static void Init()
        {
            _featureDetector = new SURF(500);
            //_featureDetector = new ORBDetector();
            //_featureDetector = new FastDetector(10, true);
            //_featureDetector = new SIFT();

            //_descriptorsComputer = _featureDetector;
            _descriptorsComputer = new Freak();
        }

        public static void ExtractFeatures(Mat image, out VectorOfKeyPoint keypoints, out Mat descriptors)
        {
            keypoints = new VectorOfKeyPoint();
            descriptors = new Mat();

            using (var uModelImage = image.ToUMat(AccessType.Read))
            {
                //_featureDetector.DetectAndCompute(uModelImage, null, keypoints, descriptors, false);

                _featureDetector.DetectRaw(uModelImage, keypoints);
                _descriptorsComputer.Compute(uModelImage, keypoints, descriptors);
            }
        }

        public static void GetMatches(VectorOfKeyPoint imageKeypoints, IInputArray imageDescriptors, VectorOfKeyPoint patternKeypoints, IInputArray patternDescriptors, out VectorOfVectorOfDMatch matches, out Mat homography)
        {
            int k = 2;
            double uniquenessThreshold = 0.8;

            homography = null;

            matches = new VectorOfVectorOfDMatch();

            var matcher = new BFMatcher(DistanceType.L2);
            matcher.Add(patternDescriptors);
            matcher.KnnMatch(imageDescriptors, matches, k, null);

            var mask = new Mat(matches.Size, 1, DepthType.Cv8U, 1);
            mask.SetTo(new MCvScalar(255));
            Features2DToolbox.VoteForUniqueness(matches, uniquenessThreshold, mask);

            int nonZeroCount = CvInvoke.CountNonZero(mask);
            if (nonZeroCount >= 4)
            {
                nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(patternKeypoints, imageKeypoints, matches, mask, 1.5, 20);
                if (nonZeroCount >= 4)
                    homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(patternKeypoints, imageKeypoints, matches, mask, 2);
            }
        }

        public static void FindMatch(Mat modelImage, Mat observedImage, out VectorOfKeyPoint modelKeyPoints, out VectorOfKeyPoint observedKeyPoints, VectorOfVectorOfDMatch matches, out Mat mask, out Mat homography)
        {
            int k = 2;
            double uniquenessThreshold = 0.8;
            double hessianThresh = 500;

            homography = null;

            modelKeyPoints = new VectorOfKeyPoint();
            observedKeyPoints = new VectorOfKeyPoint();

            using (UMat uModelImage = modelImage.ToUMat(AccessType.Read))
            using (UMat uObservedImage = observedImage.ToUMat(AccessType.Read))
            {
                SURF surfCPU = new SURF(hessianThresh);
                //extract features from the object image
                UMat modelDescriptors = new UMat();
                surfCPU.DetectAndCompute(uModelImage, null, modelKeyPoints, modelDescriptors, false);

                // extract features from the observed image
                UMat observedDescriptors = new UMat();
                surfCPU.DetectAndCompute(uObservedImage, null, observedKeyPoints, observedDescriptors, false);
                BFMatcher matcher = new BFMatcher(DistanceType.L2);
                matcher.Add(modelDescriptors);

                matcher.KnnMatch(observedDescriptors, matches, k, null);
                mask = new Mat(matches.Size, 1, DepthType.Cv8U, 1);
                mask.SetTo(new MCvScalar(255));
                Features2DToolbox.VoteForUniqueness(matches, uniquenessThreshold, mask);

                int nonZeroCount = CvInvoke.CountNonZero(mask);
                if (nonZeroCount >= 4)
                {
                    nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints,
                       matches, mask, 1.5, 20);
                    if (nonZeroCount >= 4)
                        homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints,
                           observedKeyPoints, matches, mask, 2);
                }
            }
        }

        public static Mat Draw(Mat modelImage, Mat observedImage)
        {
            Mat homography;
            VectorOfKeyPoint modelKeyPoints;
            VectorOfKeyPoint observedKeyPoints;
            using (VectorOfVectorOfDMatch matches = new VectorOfVectorOfDMatch())
            {
                Mat mask;
                FindMatch(modelImage, observedImage, out modelKeyPoints, out observedKeyPoints, matches, out mask, out homography);

                //Draw the matched keypoints
                //Mat result = new Mat();
                //Features2DToolbox.DrawKeypoints(observedImage, observedKeyPoints, result, new Bgr(0, 200, 0), Features2DToolbox.KeypointDrawType.NotDrawSinglePoints);
                var result = observedImage.Clone();

                #region draw the projected region on the image

                if (homography != null)
                {
                    //draw a rectangle along the projected model
                    Rectangle rect = new Rectangle(Point.Empty, modelImage.Size);
                    PointF[] pts = new PointF[]
                    {
                        new PointF(rect.Left, rect.Bottom),
                        new PointF(rect.Right, rect.Bottom),
                        new PointF(rect.Right, rect.Top),
                        new PointF(rect.Left, rect.Top)
                    };
                    pts = CvInvoke.PerspectiveTransform(pts, homography);

                    Point[] points = Array.ConvertAll(pts, Point.Round);
                    using (VectorOfPoint vp = new VectorOfPoint(points))
                    {
                        CvInvoke.Polylines(result, vp, true, new MCvScalar(255, 0, 0, 255), 5);
                    }
                }

                #endregion

                return result;

            }
        }
    }
}
