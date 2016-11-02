using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using RubiksCube.OpenCV.TestCase.AugmentedReality;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RubiksCube.OpenCV.TestCase.PtamLikeApproach
{
    public class SimpleAdHocTracker
    {
        #region Fields

        private const double RansacThreshold = 0.99;
        private const int MinInliers = 10;

        private bool _bootstrapping;
        private bool _canCalcMvm;

        private Mat _prevGray;
        private Mat _currGray;

        private Mat _raux;
        private Mat _taux;

        private ORBDetector _detector;

        private VectorOfKeyPoint _bootstrapKp;
        private VectorOfKeyPoint _trackedFeatures;
        private VectorOfPoint3D32F _trackedFeatures3D;

        private CameraCalibrationInfo _calibrationInfo;

        #endregion

        #region .ctor

        public SimpleAdHocTracker(CameraCalibrationInfo calibrationInfo)
        {
            _calibrationInfo = calibrationInfo;

            _detector = new ORBDetector();

            _prevGray = new Mat();
            _currGray = new Mat();

            _raux = new Mat();
            _taux = new Mat();

            _bootstrapKp = new VectorOfKeyPoint();
            _trackedFeatures = new VectorOfKeyPoint();
            _trackedFeatures3D = new VectorOfPoint3D32F();
        }

        #endregion

        #region Public Methods

        public void Bootstrap(Mat img)
        {
            //Detect first features in the image (clear any current tracks)
            if (img.IsEmpty || !img.IsEmpty && img.NumberOfChannels != 3)
                throw new Exception("Image is not appropriate (Empty or wrong number of channels).");

            _bootstrapKp.Clear();
            _detector.DetectRaw(img, _bootstrapKp);

            _trackedFeatures = _bootstrapKp;

            Trace.Indent();
            Trace.WriteLine($"Bootstrap keypoints: { _trackedFeatures.Size}.");
            Trace.Unindent();
            Trace.WriteLine("--------------------------");

            CvInvoke.CvtColor(img, _prevGray, ColorConversion.Bgr2Gray);
        }

        public bool BootstrapTrack(Mat img)
        {
            Trace.WriteLine($"BootstrapTrack iteration ({ _trackedFeatures.Size}).");
            Trace.WriteLine("--------------------------");
            Trace.Indent();

            //Track detected features
            if (_prevGray.IsEmpty)
            {
                const string error = "Previous frame is empty. Bootstrap first.";
                Trace.TraceError(error);
                throw new Exception(error);
            }


            if (img.IsEmpty || !img.IsEmpty && img.NumberOfChannels != 3)
            {
                const string error = "Image is not appropriate (Empty or wrong number of channels).";
                Trace.TraceError(error);
                throw new Exception(error);
            }

            //VectorOfPointF corners = new VectorOfPointF();
            //VectorOfCvString status = new VectorOfCvString();
            //VectorOfFloat errors = new VectorOfFloat();
            //Mat currGray = new Mat();

            var corners = new VectorOfPointF();
            var status = new VectorOfByte();
            var errors = new VectorOfFloat();

            var currGray = new Mat();
            CvInvoke.CvtColor(img, currGray, ColorConversion.Bgr2Gray);

            CvInvoke.CalcOpticalFlowPyrLK(_prevGray, currGray, Utils.GetPointsVector(_trackedFeatures), corners, status, errors, new Size(11, 11), 3, new MCvTermCriteria(20, 0.03));
            currGray.CopyTo(_prevGray);

            Trace.WriteLine($"Tracked first point: ({_trackedFeatures[0].Point.X}, {_trackedFeatures[0].Point.Y}) / Found first corner = ({corners[0].X}, {corners[0].Y})");
            Trace.WriteLine($"Tracked second point: ({_trackedFeatures[1].Point.X}, {_trackedFeatures[1].Point.Y}) / Found second corner = ({corners[1].X}, {corners[1].Y})");
            Trace.WriteLine($"Tracked third point: ({_trackedFeatures[2].Point.X}, {_trackedFeatures[2].Point.Y}) / Found third corner = ({corners[2].X}, {corners[2].Y})");

            for (int j = 0; j < corners.Size; j++)
            {
                if (status[j] == 1)
                {
                    var p1 = new Point((int)_trackedFeatures[j].Point.X, (int)_trackedFeatures[j].Point.Y);
                    var p2 = new Point((int)corners[j].X, (int)corners[j].Y);

                    CvInvoke.Line(img, p1, p2, new MCvScalar(120, 10, 20));
                }
            }

            if (CvInvoke.CountNonZero(status) < status.Size * 0.8)
            {
                Trace.TraceError("Tracking failed.");
                throw new Exception("Tracking failed.");
            }

            _trackedFeatures = Utils.GetKeyPointsVector(corners);

            Utils.KeepVectorsByStatus(ref _trackedFeatures, ref _bootstrapKp, status);

            Trace.WriteLine($"{_trackedFeatures.Size} features survived optical flow.");

            if (_trackedFeatures.Size != _bootstrapKp.Size)
            {
                const string error = "Tracked features vector size is not equal to bootstrapped one.";
                Trace.TraceError(error);
                throw new Exception(error);
            }

            Trace.WriteLine($"Bootstrap first point: ({_bootstrapKp[0].Point.X}, {_bootstrapKp[0].Point.Y}) / Found first corner = ({corners[0].X}, {corners[0].Y})");
            Trace.WriteLine($"Bootstrap second point: ({_bootstrapKp[1].Point.X}, {_bootstrapKp[1].Point.Y}) / Found second corner = ({corners[1].X}, {corners[1].Y})");
            Trace.WriteLine($"Bootstrap third point: ({_bootstrapKp[2].Point.X}, {_bootstrapKp[2].Point.Y}) / Found third corner = ({corners[2].X}, {corners[2].Y})");

            //verify features with a homography
            var inlierMask = new VectorOfByte();
            var homography = new Mat();
            if (_trackedFeatures.Size > 4)
                CvInvoke.FindHomography(Utils.GetPointsVector(_trackedFeatures), Utils.GetPointsVector(_bootstrapKp), homography, HomographyMethod.Ransac, RansacThreshold, inlierMask);

            int inliersNum = CvInvoke.CountNonZero(inlierMask);

            var zeroIndecies = new StringBuilder();
            for (int i = 0; i < inlierMask.Size; i++)
            {
                if (inlierMask[i] == 0)
                    zeroIndecies.AppendFormat("{0}, ", i);
            }

            Trace.WriteLine($"Homography zero indecies: {zeroIndecies}.");

            var m = new Matrix<float>(homography.Rows, homography.Cols, homography.DataPointer);
            Trace.WriteLine($"Homography: [ [ {m[0, 0]}, {m[0, 1]}, {m[0, 2]} ] [ {m[1, 0]}, {m[1, 1]}, {m[1, 2]} ] [ {m[2, 0]}, {m[2, 1]}, {m[2, 2]} ] ].");
            m.Dispose();

            //var res = new float[homography.Rows * homography.Cols];
            //Marshal.Copy(homography.DataPointer, res, 0, homography.Rows * homography.Cols);
            //Trace.WriteLine($"Homography1: [ [ {res[0]}, {res[1]}, {res[2]} ] [ {res[3]}, {res[4]}, {res[5]} ] [ {res[6]}, {res[7]}, {res[8]} ] ].");

            //res = new float[homography.Rows * homography.Cols];
            //Marshal.Copy(homography.DataPointer, res, 0, homography.Rows * homography.Cols);
            //Trace.WriteLine($"Homography2: [ [ {res[0]}, {res[1]}, {res[2]} ] [ {res[3]}, {res[4]}, {res[5]} ] [ {res[6]}, {res[7]}, {res[8]} ] ].");

            Trace.WriteLine($"{inliersNum} features survived homography.");

            if (inliersNum != _trackedFeatures.Size && inliersNum >= 4 && !homography.IsEmpty)
            {
                Utils.KeepVectorsByStatus(ref _trackedFeatures, ref _bootstrapKp, inlierMask);
            }
            else if (inliersNum < MinInliers)
            {
                Trace.TraceError("Not enough features survived homography.");
                return false;
            }

            var bootstrapKpOrig = new VectorOfKeyPoint(_bootstrapKp.ToArray());
            var trackedFeaturesOrig = new VectorOfKeyPoint(_trackedFeatures.ToArray());

            Trace.WriteLine($"Track first point: ({_trackedFeatures[0].Point.X}, {_trackedFeatures[0].Point.Y}) / Bootstrap first point = ({_bootstrapKp[0].Point.X}, {_bootstrapKp[0].Point.Y})");
            Trace.WriteLine($"Track 10th point: ({_trackedFeatures[10].Point.X}, {_trackedFeatures[10].Point.Y}) / Bootstrap 10th point = ({_bootstrapKp[10].Point.X}, {_bootstrapKp[10].Point.Y})");
            Trace.WriteLine($"Track last point: ({_trackedFeatures[_trackedFeatures.Size-1].Point.X}, {_trackedFeatures[_trackedFeatures.Size - 1].Point.Y}" +
                            $") / Bootstrap third point = ({_bootstrapKp[_bootstrapKp.Size-1].Point.X}, {_bootstrapKp[_bootstrapKp.Size - 1].Point.Y})");

            //Attempt at 3D reconstruction (triangulation) if conditions are right
            var rigidT = CvInvoke.EstimateRigidTransform(Utils.GetPointsVector(_trackedFeatures).ToArray(), Utils.GetPointsVector(_bootstrapKp).ToArray(), false);
            var matrix = new Matrix<float>(rigidT.Rows, rigidT.Cols, rigidT.DataPointer);

            //var ppp = rigidT.
            Trace.WriteLine($"Rigid matrix: [ [ {matrix[0, 0]}, {matrix[0, 1]}, {matrix[0, 2]} ] [ {matrix[1, 0]}, {matrix[1, 1]}, {matrix[1, 2]} ] ].");

            Trace.WriteLine($"Rigid: {CvInvoke.Norm(matrix.GetCol(2))}");

            if (CvInvoke.Norm(matrix.GetCol(2)) > 100)
            {
                //camera motion is sufficient

                Mat p, p1;
                bool triangulationSucceeded = CameraPoseAndTriangulationFromFundamental(out p, out p1);

                return true;
            }

            Trace.Unindent();
            Trace.WriteLine("--------------------------");

            return false;
        }

        public bool Track(Mat img)
        {
            //Track detected features
            if (_prevGray.IsEmpty) { Trace.WriteLine("Can't track: empty prev frame."); return false; }

            var corners = new VectorOfPointF();
            var status = new VectorOfByte();
            var errors = new VectorOfFloat();
            CvInvoke.CvtColor(img, _currGray, ColorConversion.Bgr2Gray);

            CvInvoke.CalcOpticalFlowPyrLK(_prevGray, _currGray, Utils.GetPointsVector(_trackedFeatures), corners, status, errors, new Size(11, 11), 0, new MCvTermCriteria(100));
            _currGray.CopyTo(_prevGray);

            if (CvInvoke.CountNonZero(status) < status.Size * 0.8)
            {
                Trace.WriteLine("Tracking failed.");
                _bootstrapping = false;
                _canCalcMvm = false;
                return false;
            }

            _trackedFeatures = Utils.GetKeyPointsVector(corners);

            Utils.KeepVectorsByStatus(ref _trackedFeatures, ref _trackedFeatures3D, status);

            Console.WriteLine("tracking.");

            _canCalcMvm = (_trackedFeatures.Size >= MinInliers);

            if (_canCalcMvm)
            {
                //Perform camera pose estimation for AR
                var rvec = new Mat();
                var tvec = new Mat();

                CvInvoke.SolvePnP(_trackedFeatures3D, Utils.GetPointsVector(_trackedFeatures), _calibrationInfo.Intrinsic, _calibrationInfo.Distortion, _raux, _taux, !_raux.IsEmpty);

                _raux.ConvertTo(rvec, DepthType.Cv32F);
                _taux.ConvertTo(tvec, DepthType.Cv64F);

                var pts = new MCvPoint3D32f[] {
                    new MCvPoint3D32f(0.01f,0,0),
                    new MCvPoint3D32f(0, 0.01f, 0),
                    new MCvPoint3D32f(0, 0, 0.01f) };
                var axis = new VectorOfPoint3D32F(pts);

                var imgPoints = new VectorOfPointF();
                CvInvoke.ProjectPoints(axis, _raux, _taux, _calibrationInfo.Intrinsic, _calibrationInfo.Distortion, imgPoints);

                var centerPoint = new Point((int)_trackedFeatures[0].Point.X, (int)_trackedFeatures[0].Point.Y);

                var xPoint = new Point((int)imgPoints[0].X, (int)imgPoints[0].Y);
                var yPoint = new Point((int)imgPoints[1].X, (int)imgPoints[1].Y);
                var zPoint = new Point((int)imgPoints[2].X, (int)imgPoints[2].Y);

                CvInvoke.Line(img, centerPoint, xPoint, new MCvScalar(255, 0, 0), 5); //blue x-ax
                CvInvoke.Line(img, centerPoint, yPoint, new MCvScalar(0, 255, 0), 5); //green y-ax
                CvInvoke.Line(img, centerPoint, zPoint, new MCvScalar(0, 0, 255), 5); //red z-ax

                var rot = new Mat(3, 3, DepthType.Cv32F, 3);

                CvInvoke.Rodrigues(rvec, rot);
            }

            return true;
        }

        public void Process(Mat img, bool newmap)
        {
            bool result;
            if (newmap)
            {
                Trace.WriteLine("Bootstrapping.");
                Trace.WriteLine("--------------------------");

                Bootstrap(img);
                _bootstrapping = true;

                Trace.WriteLine("Bootstrap tracking.");
                Trace.WriteLine("--------------------------");
            }
            else if (_bootstrapping)
            {
                result = BootstrapTrack(img);
                if (result)
                {
                    _bootstrapping = false;
                    Trace.WriteLine("Tracking.");
                    Trace.WriteLine("--------------------------");
                }
            }
            else if (!newmap && !_bootstrapping)
            {
                result = Track(img);
                if (result)
                {
                    Trace.WriteLine("Tracked successfully.");
                    Trace.WriteLine("--------------------------");
                }
            }
        }

        #endregion

        #region Private Methods

        private bool DecomposeEtoRandT(Matrix<float> e, out Matrix<float> r1, out Matrix<float> r2, out Matrix<float> t1, out Matrix<float> t2)
        {
            r1 = null;
            r2 = null;
            t1 = null;
            t2 = null;

            //Using HZ E decomposition

            var w = new Mat();
            var u = new Mat();
            var v = new Mat();
            CvInvoke.SVDecomp(e, w, u, v, SvdFlag.ModifyA);

            var wvv = new VectorOfFloat();
            var uvv = new VectorOfFloat();
            var vvv = new VectorOfFloat();
            CvInvoke.SVDecomp(e, wvv, uvv, vvv, SvdFlag.ModifyA);

            var wm = new Matrix<float>(w.Rows, w.Cols, w.Ptr);
            var um = new Matrix<float>(u.Rows, u.Cols, u.Ptr);
            var vm = new Matrix<float>(v.Rows, v.Cols, v.Ptr);

            //check if first and second singular values are the same (as they should be)
            float singularValuesRatio = Math.Abs(wm[0, 0] / wm[0, 1]);
            if (singularValuesRatio > 1.0) singularValuesRatio = 1.0f / singularValuesRatio; // flip ratio to keep it [0,1]
            if (singularValuesRatio < 0.7)
            {
                Trace.WriteLine("Singular values of essential matrix are too far apart.");
                return false;
            }

            var wMat = new Matrix<float>(new float[,] {
                { 0, -1, 0}, //HZ 9.13
                { 1, 0, 0 },
                { 0, 0, 1 }
            });

            var wMatTranspose = new Matrix<float>(new float[,] {
                { 0, 1, 0},
                { -1, 0, 0 },
                { 0, 0, 1 }
            });

            // vm maybe should be transposed.
            r1 = um * wMat * vm.Transpose(); //HZ 9.19
            r2 = um * wMatTranspose * vm.Transpose(); //HZ 9.19
            t1 = um.GetCol(2); //u3
            t2 = um.GetCol(2); //u3
            Utils.Negotiate(ref t2);

            wMat.Dispose();
            wMatTranspose.Dispose();

            wm.Dispose();
            um.Dispose();
            vm.Dispose();

            return true;
        }

        private bool TriangulateAndCheckReproj(Matrix<float> p, Matrix<float> p1)
        {
            //undistort
            var normalizedTrackedPts = new VectorOfPointF();
            var normalizedBootstrapPts = new VectorOfPointF();

            CvInvoke.UndistortPoints(Utils.GetPointsVector(_trackedFeatures), normalizedTrackedPts, _calibrationInfo.Intrinsic, _calibrationInfo.Distortion);
            CvInvoke.UndistortPoints(Utils.GetPointsVector(_bootstrapKp), normalizedBootstrapPts, _calibrationInfo.Intrinsic, _calibrationInfo.Distortion);

            //triangulate
            var pt3Dh = new Mat();
            CvInvoke.TriangulatePoints(p, p1, normalizedBootstrapPts, normalizedTrackedPts, pt3Dh);
            var pt3D = new Matrix<MCvPoint3D32f>(pt3Dh.Rows, pt3Dh.Cols);
            CvInvoke.ConvertPointsFromHomogeneous(pt3Dh, pt3D);
            //Mat pt_3d; convertPointsFromHomogeneous(Mat(pt_3d_h.t()).reshape(4, 1), pt_3d);
            //    cout << pt_3d.size() << endl;
            //    cout << pt_3d.rowRange(0,10) << endl;

            var statusArray = new byte[pt3D.Rows];
            for (int i = 0; i < pt3D.Rows; i++)
            {
                statusArray[i] = (pt3D[i, 0].Z > 0) ? (byte)1 : (byte)0;
            }

            var status = new VectorOfByte(statusArray);
            int count = CvInvoke.CountNonZero(status);

            double percentage = count / (double)pt3D.Rows;
            //cout << count << "/" << pt_3d.rows << " = " << percentage * 100.0 << "% are in front of camera";
            if (percentage < 0.75)
                return false; //less than 75% of the points are in front of the camera


            //calculate reprojection
            var r = p.GetSubRect(new Rectangle(0, 0, 3, 3));
            var rvec = new VectorOfFloat(new float[] { 0, 0, 0 }); //Rodrigues(R ,rvec);
            var tvec = new VectorOfFloat(new float[] { 0, 0, 0 }); // = P.col(3);
            var reprojectedPtSet1 = new VectorOfPointF();
            CvInvoke.ProjectPoints(pt3D, rvec, tvec, _calibrationInfo.Intrinsic, _calibrationInfo.Distortion, reprojectedPtSet1);
            //    cout << Mat(reprojected_pt_set1).rowRange(0,10) << endl;
            var bootstrapPts = Utils.GetPointsVector(_bootstrapKp);
            //Mat bootstrapPts = new Mat(bootstrapPts_v);
            //    cout << bootstrapPts.rowRange(0,10) << endl;

            double reprojErr = CvInvoke.Norm(reprojectedPtSet1, bootstrapPts, NormType.L2) / bootstrapPts.Size;
            //cout << "reprojection Error " << reprojErr;
            if (reprojErr < 5)
            {
                statusArray = new byte[pt3D.Rows];
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
            var status = new VectorOfByte();
            CvInvoke.FindFundamentalMat(trackedFeaturesPts, bootstrapPts, f, FmType.Ransac, 0.006 * maxVal, 0.99, status);
            var fMat = new Matrix<float>(f.Rows, f.Cols, f.DataPointer);

            int inliersNum = CvInvoke.CountNonZero(status);

            Trace.WriteLine($"Fundamental keeping {inliersNum} / {status.Size}");

            Utils.KeepVectorsByStatus(ref _trackedFeatures, ref _bootstrapKp, status);

            if (inliersNum > MinInliers)
            {
                //Essential matrix: compute then extract cameras [R|t]
                var e = _calibrationInfo.Intrinsic.Transpose() * fMat * _calibrationInfo.Intrinsic; //according to HZ (9.12)

                //according to http://en.wikipedia.org/wiki/Essential_matrix#Properties_of_the_essential_matrix
                var determinant = Math.Abs(CvInvoke.Determinant(e));
                if (determinant > 1e-07)
                {
                    Console.WriteLine($"det(E) != 0 : {determinant}");
                    return false;
                }

                Matrix<float> r1;
                Matrix<float> r2;
                Matrix<float> t1;
                Matrix<float> t2;
                if (!DecomposeEtoRandT(e, out r1, out r2, out t1, out t2)) return false;

                determinant = Math.Abs(CvInvoke.Determinant(r1));
                if (determinant + 1.0 < 1e-09)
                {
                    //according to http://en.wikipedia.org/wiki/Essential_matrix#Showing_that_it_is_valid
                    Trace.WriteLine($"det(R) == -1 [{determinant}]: flip E's sign");
                    Utils.Negotiate(ref e);
                    if (!DecomposeEtoRandT(e, out r1, out r2, out t1, out t2)) return false;
                }
                if (Math.Abs(determinant) - 1.0 > 1e-07)
                {
                    Trace.WriteLine($"det(R) != +-1.0, this is not a rotation matrix");
                    return false;
                }

                var p = new Matrix<float>(3, 4);
                p.SetIdentity(new MCvScalar(1f));

                //TODO: there are 4 different combinations for P1...
                var pMat1 = new Matrix<float>(new float[3, 4] {
                    { r1[0,0], r1[0,1], r1[0,2], t1[0,0] },
                    { r1[1,0], r1[1,1], r1[1,2], t1[0,1]},
                    { r1[2,0], r1[2,1], r1[2,2], t1[0,2]}
                });

                bool triangulationSucceeded = true;
                if (!TriangulateAndCheckReproj(p, pMat1))
                {
                    pMat1 = new Matrix<float>(new float[3, 4] {
                        { r1[0,0], r1[0,1], r1[0,2], t2[0,0] },
                        { r1[1,0], r1[1,1], r1[1,2], t2[0,1]},
                        { r1[2,0], r1[2,1], r1[2,2], t2[0,2]}
                    });

                    if (!TriangulateAndCheckReproj(p, pMat1))
                    {
                        pMat1 = new Matrix<float>(new float[3, 4] {
                            { r2[0,0], r2[0,1], r2[0,2], t2[0,0] },
                            { r2[1,0], r2[1,1], r2[1,2], t2[0,1]},
                            { r2[2,0], r2[2,1], r2[2,2], t2[0,2]}
                        });

                        if (!TriangulateAndCheckReproj(p, pMat1))
                        {
                            pMat1 = new Matrix<float>(new float[3, 4] {
                                { r2[0,0], r2[0,1], r2[0,2], t1[0,0] },
                                { r2[1,0], r2[1,1], r2[1,2], t1[0,1]},
                                { r2[2,0], r2[2,1], r2[2,2], t1[0,2]}
                            });

                            if (!TriangulateAndCheckReproj(p, pMat1))
                            {
                                Trace.WriteLine("Can't find the right P matrix.");
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
