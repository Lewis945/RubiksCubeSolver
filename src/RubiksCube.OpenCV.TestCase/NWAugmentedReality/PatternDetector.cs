using Emgu.CV;
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

namespace RubiksCube.OpenCV.TestCase.NWAugmentedReality
{
    public class PatternDetector
    {
        #region Fields

        private VectorOfKeyPoint m_queryKeypoints = new VectorOfKeyPoint();
        private Mat m_queryDescriptors = new Mat();
        private VectorOfDMatch m_matches = new VectorOfDMatch();
        private VectorOfVectorOfDMatch m_knnMatches = new VectorOfVectorOfDMatch();

        private Mat m_grayImg = new Mat();
        private Mat m_warpedImg = new Mat();
        private Mat m_roughHomography = new Mat();
        private Mat m_refinedHomography = new Mat();

        private Pattern m_pattern;
        //private cv::Ptr<cv::FeatureDetector> m_detector;
        //private cv::Ptr<cv::DescriptorExtractor> m_extractor;
        //private cv::Ptr<cv::DescriptorMatcher> m_matcher;
        private ORBDetector m_detector;
        private Freak m_extractor;
        private BFMatcher m_matcher;

        #endregion

        #region Properties

        public bool enableRatioTest;
        public bool enableHomographyRefinement;
        public float homographyReprojectionThreshold;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public PatternDetector()
        {
            m_detector = new ORBDetector(1000);
            m_extractor = new Freak(false, false);
            m_matcher = new BFMatcher(DistanceType.Hamming, true);
            enableRatioTest = false;
            enableHomographyRefinement = true;
            homographyReprojectionThreshold = 3;
        }

        /// <summary>
        /// Initialize a pattern detector with specified feature detector, descriptor extraction and matching algorithm
        /// </summary>
        /// <param name="detector"></param>
        /// <param name="extractor"></param>
        /// <param name="matcher"></param>
        /// <param name="ratioTest"></param>
        public PatternDetector(ORBDetector detector, Freak extractor, BFMatcher matcher, bool ratioTest)
        //public PatternDetector(Ptr<FeatureDetector> detector = new ORB(1000), Ptr<DescriptorExtractor> extractor = new FREAK(false, false), Ptr<DescriptorMatcher> matcher = new BFMatcher(NORM_HAMMING, true), bool enableRatioTest = false)
        {
            m_detector = detector;
            m_extractor = extractor;
            m_matcher = matcher;
            enableRatioTest = ratioTest;
            enableHomographyRefinement = true;
            homographyReprojectionThreshold = 3;
        }

        public void train(Pattern pattern)
        {
            // Store the pattern object
            m_pattern = pattern;

            // Then we add vector of descriptors (each descriptors matrix describe one image). 
            // This allows us to perform search across multiple images:
            m_matcher.Add(pattern.descriptors.Clone());

            // After adding train data perform actual train:
            //m_matcher->train();
        }

        /// <summary>
        /// Initialize Pattern structure from the input image.
        /// This function finds the feature points and extract descriptors for them.
        /// </summary>
        public void buildPatternFromImage(Mat image, Pattern pattern)
        {
            float step = (float)Math.Sqrt(2);

            // Store original image in pattern structure
            pattern.size = new Size(image.Cols, image.Rows);
            pattern.frame = image.Clone();
            getGray(image, pattern.grayImg);

            // Build 2d and 3d contours (3d contour lie in XY plane since it's planar)
            pattern.points2d = new VectorOfPoint(4);
            pattern.points3d = new VectorOfPoint3D32F(4);

            // Image dimensions
            int w = image.Cols;
            int h = image.Rows;

            // Normalized dimensions:
            float maxSize = Math.Max(w, h);
            float unitW = w / maxSize;
            float unitH = h / maxSize;

            pattern.points2d.Clear();
            pattern.points2d.Push(new Point[] {
                new Point(0, 0),
                new Point(w, 0),
                new Point(w, h),
                new Point(0, h)
            });

            pattern.points3d.Clear();
            pattern.points3d.Push(new MCvPoint3D32f[] {
                new MCvPoint3D32f(-unitW, -unitH, 0),
                new MCvPoint3D32f(unitW, -unitH, 0),
                new MCvPoint3D32f(unitW, unitH, 0),
                 new MCvPoint3D32f(-unitW, unitH, 0)
            });

            extractFeatures(pattern.grayImg, pattern.keypoints, pattern.descriptors);
        }

        /// <summary>
        /// Tries to find a @pattern object on given @image. 
        /// The function returns true if succeeded and store the result(pattern 2d location, homography) in @info.
        /// </summary>
        /// <returns></returns>
        public bool findPattern(Mat image, PatternTrackingInfo info)
        {
            // Convert input image to gray
            getGray(image, m_grayImg);

            // Extract feature points from input gray image
            extractFeatures(m_grayImg, m_queryKeypoints, m_queryDescriptors);

            // Get matches with current pattern
            getMatches(m_queryDescriptors, m_matches);

            // Find homography transformation and detect good matches
            bool homographyFound = refineMatchesWithHomography(
                m_queryKeypoints,
                m_pattern.keypoints,
                homographyReprojectionThreshold,
                m_matches,
                m_roughHomography);

            if (homographyFound)
            {
                // If homography refinement enabled improve found transformation
                if (enableHomographyRefinement)
                {
                    // Warp image using found homography
                    CvInvoke.WarpPerspective(m_grayImg, m_warpedImg, m_roughHomography, m_pattern.size, Emgu.CV.CvEnum.Inter.Cubic, Emgu.CV.CvEnum.Warp.InverseMap);

                    // Get refined matches:
                    VectorOfKeyPoint warpedKeypoints = new VectorOfKeyPoint();
                    VectorOfDMatch refinedMatches = new VectorOfDMatch();

                    // Detect features on warped image
                    extractFeatures(m_warpedImg, warpedKeypoints, m_queryDescriptors);

                    // Match with pattern
                    getMatches(m_queryDescriptors, refinedMatches);

                    // Estimate new refinement homography
                    homographyFound = refineMatchesWithHomography(
                        warpedKeypoints,
                        m_pattern.keypoints,
                        homographyReprojectionThreshold,
                        refinedMatches,
                        m_refinedHomography);

                    // Get a result homography as result of matrix product of refined and rough homographies:

                    var roughHomography = new Matrix<Byte>(m_roughHomography.Rows, m_roughHomography.Cols, m_roughHomography.NumberOfChannels);
                    m_roughHomography.CopyTo(roughHomography);
                    var refinedHomography = new Matrix<Byte>(m_refinedHomography.Rows, m_refinedHomography.Cols, m_refinedHomography.NumberOfChannels);
                    m_refinedHomography.CopyTo(refinedHomography);

                    info.homography = (roughHomography * refinedHomography).ToUMat().ToMat(Emgu.CV.CvEnum.AccessType.ReadWrite);

                    // Transform contour with precise homography
                    CvInvoke.PerspectiveTransform(m_pattern.points2d, info.points2d, info.homography);
                }
                else
                {
                    info.homography = m_roughHomography;

                    // Transform contour with rough homography
                    CvInvoke.PerspectiveTransform(m_pattern.points2d, info.points2d, m_roughHomography);
                }
            }

            return homographyFound;
        }

        protected bool extractFeatures(Mat image, VectorOfKeyPoint keypoints, Mat descriptors)
        {
            //keypoints = new VectorOfKeyPoint(m_detector.Detect(image));
            //if (keypoints.Size == 0)
            //    return false;

            //m_extractor.Compute(image, keypoints, descriptors);
            //if (keypoints.Size == 0)
            //    return false;

            m_extractor.DetectAndCompute(image, null, keypoints, descriptors, false);

            if (keypoints.Size == 0)
                return false;

            return true;
        }

        protected void getMatches(Mat queryDescriptors, VectorOfDMatch matches)
        {
            matches.Clear();

            if (enableRatioTest)
            {
                // To avoid NaN's when best match has zero distance we will use inversed ratio. 
                const float minRatio = 1.0f / 1.5f;

                // KNN match will return 2 nearest matches for each query descriptor
                m_matcher.KnnMatch(queryDescriptors, m_knnMatches, 2, null);

                for (int i = 0; i < m_knnMatches.Size; i++)
                {
                    MDMatch bestMatch = m_knnMatches[i][0];
                    MDMatch betterMatch = m_knnMatches[i][1];

                    float distanceRatio = bestMatch.Distance / betterMatch.Distance;

                    // Pass only matches where distance ratio between 
                    // nearest matches is greater than 1.5 (distinct criteria)
                    if (distanceRatio < minRatio)
                    {
                        matches.Push(new MDMatch[] { bestMatch });
                    }
                }
            }
            else
            {
                // Perform regular match
                //m_matcher.match(queryDescriptors, matches);
            }
        }

        /// <summary>
        /// Get the gray image from the input image.
        /// Function performs necessary color conversion if necessary
        /// Supported input images types - 1 channel(no conversion is done), 3 channels(assuming BGR) and 4 channels(assuming BGRA).
        /// </summary>
        protected static void getGray(Mat image, Mat gray)
        {
            if (image.NumberOfChannels == 3)
                CvInvoke.CvtColor(image, gray, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
            else if (image.NumberOfChannels == 4)
                CvInvoke.CvtColor(image, gray, Emgu.CV.CvEnum.ColorConversion.Bgra2Gray);
            else if (image.NumberOfChannels == 1)
                gray = image;
        }

        protected static bool refineMatchesWithHomography(VectorOfKeyPoint queryKeypoints, VectorOfKeyPoint trainKeypoints, float reprojectionThreshold, VectorOfDMatch matches, Mat homography)
        {
            const int minNumberMatchesAllowed = 8;

            if (matches.Size < minNumberMatchesAllowed)
                return false;

            // Prepare data for cv::findHomography
            PointF[] srcPoints = new PointF[matches.Size];
            PointF[] dstPoints = new PointF[matches.Size];

            for (int i = 0; i < matches.Size; i++)
            {
                srcPoints[i] = trainKeypoints[matches[i].TrainIdx].Point;
                dstPoints[i] = queryKeypoints[matches[i].QueryIdx].Point;
            }

            // Find homography matrix and get inliers mask
            VectorOfByte mask = new VectorOfByte(srcPoints.Length);
            CvInvoke.FindHomography(srcPoints, dstPoints, homography, Emgu.CV.CvEnum.HomographyMethod.Ransac, reprojectionThreshold, mask);

            VectorOfDMatch inliers = new VectorOfDMatch();
            for (int i = 0; i < mask.Size; i++)
            {
                if (mask[i] > 0)
                    inliers.Push(new MDMatch[] { matches[i] });
            }

            matches = inliers;

            return matches.Size > minNumberMatchesAllowed;
        }
    }
}
