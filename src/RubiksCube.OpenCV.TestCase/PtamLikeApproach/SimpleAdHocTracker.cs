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

            _trackedFeatures = new VectorOfKeyPoint(_bootstrapKp.ToArray());

            #region Trace

            Trace.Indent();
            Trace.WriteLine($"Bootstrap keypoints: { _trackedFeatures.Size}.");
            Trace.Unindent();
            Trace.WriteLine("--------------------------");

            #endregion

            CvInvoke.CvtColor(img, _prevGray, ColorConversion.Bgr2Gray);
        }

        public bool BootstrapTrack(Mat img)
        {
            #region Trace

            Trace.WriteLine($"BootstrapTrack iteration ({ _trackedFeatures.Size}).");
            Trace.WriteLine("--------------------------");
            Trace.Indent();

            #endregion

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

            var corners = new VectorOfPointF();
            var status = new VectorOfByte();
            var errors = new VectorOfFloat();

            var currGray = new Mat();
            CvInvoke.CvtColor(img, currGray, ColorConversion.Bgr2Gray);

            CvInvoke.CalcOpticalFlowPyrLK(_prevGray, currGray, Utils.GetPointsVector(_trackedFeatures), corners,
                status, errors, new Size(11, 11), 3, new MCvTermCriteria(20, 0.03));
            currGray.CopyTo(_prevGray);

            #region Trace

            Trace.WriteLine($"Tracked first point: ({_trackedFeatures[0].Point.X}, {_trackedFeatures[0].Point.Y}) / Found first corner = ({corners[0].X}, {corners[0].Y})");
            Trace.WriteLine($"Tracked second point: ({_trackedFeatures[1].Point.X}, {_trackedFeatures[1].Point.Y}) / Found second corner = ({corners[1].X}, {corners[1].Y})");
            Trace.WriteLine($"Tracked third point: ({_trackedFeatures[2].Point.X}, {_trackedFeatures[2].Point.Y}) / Found third corner = ({corners[2].X}, {corners[2].Y})");

            #endregion

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

            #region Trace

            Trace.WriteLine($"Bootstrap first point: ({_bootstrapKp[0].Point.X}, {_bootstrapKp[0].Point.Y}) / Found first corner = ({corners[0].X}, {corners[0].Y})");
            Trace.WriteLine($"Bootstrap second point: ({_bootstrapKp[1].Point.X}, {_bootstrapKp[1].Point.Y}) / Found second corner = ({corners[1].X}, {corners[1].Y})");
            Trace.WriteLine($"Bootstrap third point: ({_bootstrapKp[2].Point.X}, {_bootstrapKp[2].Point.Y}) / Found third corner = ({corners[2].X}, {corners[2].Y})");

            #endregion

            //verify features with a homography
            var inlierMask = new VectorOfByte();
            var homography = new Mat();
            if (_trackedFeatures.Size > 4)
                CvInvoke.FindHomography(Utils.GetPointsVector(_trackedFeatures), Utils.GetPointsVector(_bootstrapKp), homography, HomographyMethod.Ransac, RansacThreshold, inlierMask);

            int inliersNum = CvInvoke.CountNonZero(inlierMask);

            var m = new Matrix<double>(homography.Rows, homography.Cols, homography.DataPointer);

            m.Dispose();

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

            //Attempt at 3D reconstruction (triangulation) if conditions are right
            var rigidT = CvInvoke.EstimateRigidTransform(Utils.GetPointsVector(_trackedFeatures).ToArray(), Utils.GetPointsVector(_bootstrapKp).ToArray(), false);
            var matrix = new Matrix<double>(rigidT.Rows, rigidT.Cols, rigidT.DataPointer);

            #region Trace

            Trace.WriteLine($"Track first point: ({_trackedFeatures[0].Point.X}, {_trackedFeatures[0].Point.Y}) / Bootstrap first point = ({_bootstrapKp[0].Point.X}, {_bootstrapKp[0].Point.Y})");
            Trace.WriteLine($"Track 10th point: ({_trackedFeatures[10].Point.X}, {_trackedFeatures[10].Point.Y}) / Bootstrap 10th point = ({_bootstrapKp[10].Point.X}, {_bootstrapKp[10].Point.Y})");
            Trace.WriteLine($"Track last point: ({_trackedFeatures[_trackedFeatures.Size - 1].Point.X}, {_trackedFeatures[_trackedFeatures.Size - 1].Point.Y}" +
                            $") / Bootstrap third point = ({_bootstrapKp[_bootstrapKp.Size - 1].Point.X}, {_bootstrapKp[_bootstrapKp.Size - 1].Point.Y})");

            Trace.WriteLine($"Rigid matrix: [ [ {matrix[0, 0]}, {matrix[0, 1]}, {matrix[0, 2]} ] [ {matrix[1, 0]}, {matrix[1, 1]}, {matrix[1, 2]} ] ].");
            Trace.WriteLine($"Rigid: {CvInvoke.Norm(matrix.GetCol(2))}");

            #endregion

            if (CvInvoke.Norm(matrix.GetCol(2)) > 100)
            {
                //camera motion is sufficient
                var p1 = new Matrix<double>(3,4);
                p1.SetIdentity();
                var result = OpenCvUtilities.CameraPoseAndTriangulationFromFundamental(_calibrationInfo, _trackedFeatures, _bootstrapKp, p1);

                _trackedFeatures = result.FilteredTrackedFeaturesKp;
                _bootstrapKp = result.FilteredBootstrapKp;

                if (result.Result)
                {
                    _trackedFeatures3D = result.TrackedFeatures3D;
                    var trackedFeatures3Dm = Utils.Get3dPointsMat(_trackedFeatures3D);

                    var eigenvectors = new Mat();
                    var mean = new Mat();
                    CvInvoke.PCACompute(trackedFeatures3Dm, mean, eigenvectors);
                    var eigenvectorsMatrix = new Matrix<double>(eigenvectors.Rows, eigenvectors.Cols, eigenvectors.DataPointer);
                    
                    int numInliers = 0;
                    var normalOfPlane = eigenvectorsMatrix.GetRow(2).ToUMat().ToMat(AccessType.Fast);
                    //eigenvectors.GetRow(2).CopyTo(normalOfPlane);
                    CvInvoke.Normalize(normalOfPlane, normalOfPlane);

                    var normalOfPlaneMatrix = new Matrix<double>(normalOfPlane.Rows, normalOfPlane.Cols, normalOfPlane.DataPointer);
                    Trace.WriteLine($"normal of plane: {normalOfPlaneMatrix[0, 0]}");
                    //cv::Vec3d x0 = pca.mean;
                    //double p_to_plane_thresh = sqrt(pca.eigenvalues.at<double>(2));
                }

                return true;
            }

            #region Trace

            Trace.Unindent();
            Trace.WriteLine("--------------------------");

            #endregion

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
    }
}
