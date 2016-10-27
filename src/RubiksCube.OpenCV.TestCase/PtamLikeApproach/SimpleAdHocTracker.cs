using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using RubiksCube.OpenCV.TestCase.AugmentedReality;
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

        private CameraCalibrationInfo _calibrationInfo;

        public SimpleAdHocTracker(CameraCalibrationInfo calibrationInfo)
        {
            _calibrationInfo = calibrationInfo;

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

            //Attempt at 3D reconstruction (triangulation) if conditions are right
            var rigidT = CvInvoke.EstimateRigidTransform(Utils.GetPointsVector(_trackedFeatures), Utils.GetPointsVector(_bootstrapKp), false);

            var matrix = new Matrix<float>(rigidT.Rows, rigidT.Cols, rigidT.Ptr);
            if (CvInvoke.Norm(matrix.GetCol(2)) > 100)
            {
                //camera motion is sufficient

                Mat P, P1;
                bool triangulationSucceeded = CameraPoseAndTriangulationFromFundamental(out P, out P1);
            }
        }

        #region Private Methods

        private bool DecomposeEtoRandT(Matrix<float> E, out Matrix<float> R1, out Matrix<float> R2, out Matrix<float> t1, out Matrix<float> t2)
        {
            R1 = null;
            R2 = null;
            t1 = null;
            t2 = null;

            //Using HZ E decomposition

            var w = new Mat();
            var u = new Mat();
            var v = new Mat();
            CvInvoke.SVDecomp(E, w, u, v, SvdFlag.ModifyA);

            var wvv = new VectorOfFloat();
            var uvv = new VectorOfFloat();
            var vvv = new VectorOfFloat();
            CvInvoke.SVDecomp(E, wvv, uvv, vvv, SvdFlag.ModifyA);

            var wm = new Matrix<float>(w.Rows, w.Cols, w.Ptr);
            var um = new Matrix<float>(u.Rows, u.Cols, u.Ptr);
            var vm = new Matrix<float>(v.Rows, v.Cols, v.Ptr);

            //check if first and second singular values are the same (as they should be)
            float singular_values_ratio = Math.Abs(wm[0, 0] / wm[0, 1]);
            if (singular_values_ratio > 1.0) singular_values_ratio = 1.0f / singular_values_ratio; // flip ratio to keep it [0,1]
            if (singular_values_ratio < 0.7)
            {
                Console.WriteLine("Singular values of essential matrix are too far apart.");
                return false;
            }

            var W = new Matrix<float>(new float[,] {
                { 0, -1, 0}, //HZ 9.13
                { 1, 0, 0 },
                { 0, 0, 1 }
            });

            var Wt = new Matrix<float>(new float[,] {
                { 0, 1, 0},
                { -1, 0, 0 },
                { 0, 0, 1 }
            });

            // vm maybe should be transposed.
            R1 = um * W * vm.Transpose(); //HZ 9.19
            R2 = um * Wt * vm.Transpose(); //HZ 9.19
            t1 = um.GetCol(2); //u3
            t2 = um.GetCol(2); //u3
            Utils.Negotiate(t2);

            return true;
        }

        private bool TriangulateAndCheckReproj(Matrix<float> P, Matrix<float> P1)
        {
            //undistort
            var normalizedTrackedPts = new VectorOfPointF();
            var normalizedBootstrapPts = new VectorOfPointF();

            CvInvoke.UndistortPoints(Utils.GetPointsVector(_trackedFeatures), normalizedTrackedPts, _calibrationInfo.Intrinsic, _calibrationInfo.Distortion);
            CvInvoke.UndistortPoints(Utils.GetPointsVector(_bootstrapKp), normalizedBootstrapPts, _calibrationInfo.Intrinsic, _calibrationInfo.Distortion);

            //triangulate
            Mat pt_3d_h = new Mat();
            CvInvoke.TriangulatePoints(P, P1, normalizedBootstrapPts, normalizedTrackedPts, pt_3d_h);
            Matrix<MCvPoint3D32f> pt_3d = new Matrix<MCvPoint3D32f>(pt_3d_h.Rows, pt_3d_h.Cols);
            CvInvoke.ConvertPointsFromHomogeneous(pt_3d_h, pt_3d);
            //Mat pt_3d; convertPointsFromHomogeneous(Mat(pt_3d_h.t()).reshape(4, 1), pt_3d);
            //    cout << pt_3d.size() << endl;
            //    cout << pt_3d.rowRange(0,10) << endl;

            var statusArray = new byte[pt_3d.Rows];
            for (int i = 0; i < pt_3d.Rows; i++)
            {
                statusArray[i] = (pt_3d[i, 0].Z > 0) ? (byte)1 : (byte)0;
            }

            var status = new VectorOfByte(statusArray);
            int count = CvInvoke.CountNonZero(status);

            double percentage = count / (double)pt_3d.Rows;
            //cout << count << "/" << pt_3d.rows << " = " << percentage * 100.0 << "% are in front of camera";
            if (percentage < 0.75)
                return false; //less than 75% of the points are in front of the camera


            //calculate reprojection
            var R = P.GetSubRect(new Rectangle(0, 0, 3, 3));
            var rvec = new VectorOfFloat(new float[] { 0, 0, 0 }); //Rodrigues(R ,rvec);
            var tvec = new VectorOfFloat(new float[] { 0, 0, 0 }); // = P.col(3);
            var reprojected_pt_set1 = new VectorOfPointF();
            CvInvoke.ProjectPoints(pt_3d, rvec, tvec, _calibrationInfo.Intrinsic, _calibrationInfo.Distortion, reprojected_pt_set1);
            //    cout << Mat(reprojected_pt_set1).rowRange(0,10) << endl;
            var bootstrapPts = Utils.GetPointsVector(_bootstrapKp);
            //Mat bootstrapPts = new Mat(bootstrapPts_v);
            //    cout << bootstrapPts.rowRange(0,10) << endl;

            double reprojErr = CvInvoke.Norm(reprojected_pt_set1, bootstrapPts, NormType.L2) / bootstrapPts.Size;
            //cout << "reprojection Error " << reprojErr;
            if (reprojErr < 5)
            {
                statusArray = new byte[pt_3d.Rows];
                for (int i = 0; i < bootstrapPts.Size; ++i)
                {
                    //statusArray[i] = (CvInvoke.Norm(Utils.SubstarctPoints(bootstrapPts[i], reprojected_pt_set1[i])) < 20.0);
                }

                status = new VectorOfByte(statusArray);

                _trackedFeatures3D.Clear();
                //_trackedFeatures3D.Resize(pt_3d.Rows);
                //pt_3d.CopyTo(_trackedFeatures3D);

                //Utils.KeepVectorsByStatus(_trackedFeatures, _trackedFeatures3D, status);
                //cout << "keeping " << trackedFeatures.size() << " nicely reprojected points";
                _bootstrapping = false;
                return true;
            }
            return false;
        }

        private bool CameraPoseAndTriangulationFromFundamental(out Mat p1, out Mat p2)
        {
            p1 = null;
            p2 = null;

            //find fundamental matrix
            double minVal, maxVal;
            var minIdx = new int[2];
            var maxIdx = new int[2];
            var trackedFeaturesPts = Utils.GetPointsVector(_trackedFeatures);
            var bootstrapPts = Utils.GetPointsVector(_bootstrapKp);

            CvInvoke.MinMaxIdx(trackedFeaturesPts, out minVal, out maxVal, minIdx, maxIdx);

            var f = new Mat();
            VectorOfByte status = new VectorOfByte();
            CvInvoke.FindFundamentalMat(trackedFeaturesPts, bootstrapPts, f, FmType.Ransac, 0.006 * maxVal, 0.99, status);
            var F = new Matrix<float>(f.Rows, f.Cols, f.Ptr);

            int inliers_num = CvInvoke.CountNonZero(status);
            Console.WriteLine($"Fundamental keeping {inliers_num} / {status.Size}");
            Utils.KeepVectorsByStatus(_trackedFeatures, _bootstrapKp, status);

            if (inliers_num > MIN_INLIERS)
            {
                //Essential matrix: compute then extract cameras [R|t]
                Matrix<float> E = _calibrationInfo.Intrinsic.Transpose() * F * _calibrationInfo.Intrinsic; //according to HZ (9.12)

                //according to http://en.wikipedia.org/wiki/Essential_matrix#Properties_of_the_essential_matrix
                var determinant = Math.Abs(CvInvoke.Determinant(E));
                if (determinant > 1e-07)
                {
                    Console.WriteLine($"det(E) != 0 : {determinant}");
                    return false;
                }

                Matrix<float> R1;
                Matrix<float> R2;
                Matrix<float> t1;
                Matrix<float> t2;
                if (!DecomposeEtoRandT(E, out R1, out R2, out t1, out t2)) return false;

                determinant = Math.Abs(CvInvoke.Determinant(R1));
                if (determinant + 1.0 < 1e-09)
                {
                    //according to http://en.wikipedia.org/wiki/Essential_matrix#Showing_that_it_is_valid
                    Console.WriteLine($"det(R) == -1 [{determinant}]: flip E's sign");
                    Utils.Negotiate(E);
                    if (!DecomposeEtoRandT(E, out R1, out R2, out t1, out t2)) return false;
                }
                if (Math.Abs(determinant) - 1.0 > 1e-07)
                {
                    Console.WriteLine($"det(R) != +-1.0, this is not a rotation matrix");
                    return false;
                }

                var P = new Matrix<float>(3, 4);
                P.SetIdentity(new MCvScalar(1f));

                //TODO: there are 4 different combinations for P1...
                var P1 = new Matrix<float>(new float[3, 4] {
                    { R1[0,0], R1[0,1], R1[0,2], t1[0,0] },
                    { R1[1,0], R1[1,1], R1[1,2], t1[0,1]},
                    { R1[2,0], R1[2,1], R1[2,2], t1[0,2]}
                });

                bool triangulationSucceeded = true;
                if (!TriangulateAndCheckReproj(P, P1))
                {
                    P1 = new Matrix<float>(new float[3, 4] {
                        { R1[0,0], R1[0,1], R1[0,2], t2[0,0] },
                        { R1[1,0], R1[1,1], R1[1,2], t2[0,1]},
                        { R1[2,0], R1[2,1], R1[2,2], t2[0,2]}
                    });

                    if (!TriangulateAndCheckReproj(P, P1))
                    {
                        P1 = new Matrix<float>(new float[3, 4] {
                            { R2[0,0], R2[0,1], R2[0,2], t2[0,0] },
                            { R2[1,0], R2[1,1], R2[1,2], t2[0,1]},
                            { R2[2,0], R2[2,1], R2[2,2], t2[0,2]}
                        });

                        if (!TriangulateAndCheckReproj(P, P1))
                        {
                            P1 = new Matrix<float>(new float[3, 4] {
                                { R2[0,0], R2[0,1], R2[0,2], t1[0,0] },
                                { R2[1,0], R2[1,1], R2[1,2], t1[0,1]},
                                { R2[2,0], R2[2,1], R2[2,2], t1[0,2]}
                            });

                            if (!TriangulateAndCheckReproj(P, P1))
                            {
                                Console.WriteLine("can't find the right P matrix");
                                triangulationSucceeded = false;
                            }
                        }

                    }
                }
                return triangulationSucceeded;
            }

            p1 = new Mat();
            p2 = new Mat();

            return true;
        }

        #endregion
    }
}
