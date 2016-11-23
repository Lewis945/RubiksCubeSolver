using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;
using Accord.Statistics;
using Accord.Statistics.Analysis;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using RubiksCube.OpenCV.TestCase.AugmentedReality;

namespace RubiksCube.OpenCV.TestCase.PtamLikeApproach
{
    public class PtamLikeAlgorithm
    {
        #region Fields

        private const double RansacThreshold = 0.99;
        private const int MinInliers = 10;

        private bool _bootstrapping;
        private bool _canCalcMvm;

        private readonly Mat _prevGray;
        private readonly Mat _currGray;

        private Mat _raux;
        private Mat _taux;

        private readonly ORBDetector _detector;

        private VectorOfKeyPoint _bootstrapKp;
        private VectorOfKeyPoint _trackedFeatures;
        private VectorOfPoint3D32F _trackedFeatures3D;

        private readonly CameraCalibrationInfo _calibrationInfo;

        #endregion

        #region Properties

        public VectorOfPoint3D32F TrackedFeatures3D => _trackedFeatures3D;
        public VectorOfKeyPoint TrackedFeatures => _trackedFeatures;

        #endregion

        #region .ctor

        public PtamLikeAlgorithm(CameraCalibrationInfo calibrationInfo)
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

            CvInvoke.CvtColor(img, _prevGray, ColorConversion.Bgr2Gray);
        }

        public bool BootstrapTrack(Mat img)
        {
            //Track detected features
            if (_prevGray.IsEmpty)
            {
                const string error = "Previous frame is empty. Bootstrap first.";
                throw new Exception(error);
            }

            if (img.IsEmpty || !img.IsEmpty && img.NumberOfChannels != 3)
            {
                const string error = "Image is not appropriate (Empty or wrong number of channels).";
                throw new Exception(error);
            }

            var corners = new VectorOfPointF();
            var status = new VectorOfByte();
            var errors = new VectorOfFloat();

            CvInvoke.CvtColor(img, _currGray, ColorConversion.Bgr2Gray);

            CvInvoke.CalcOpticalFlowPyrLK(_prevGray, _currGray, Utils.GetPointsVector(_trackedFeatures), corners,
                status, errors, new Size(11, 11), 3, new MCvTermCriteria(20, 0.03));
            _currGray.CopyTo(_prevGray);

            for (int j = 0; j < corners.Size; j++)
            {
                if (status[j] == 1)
                    CvInvoke.Line(img, new Point((int)_trackedFeatures[j].Point.X, (int)_trackedFeatures[j].Point.Y), new Point((int)corners[j].X, (int)corners[j].Y), new MCvScalar(120, 10, 20));
            }

            if (CvInvoke.CountNonZero(status) < status.Size * 0.8)
            {
                throw new Exception("Tracking failed.");
            }

            _trackedFeatures = Utils.GetKeyPointsVector(corners);

            Utils.KeepVectorsByStatus(ref _trackedFeatures, ref _bootstrapKp, status);

            if (_trackedFeatures.Size != _bootstrapKp.Size)
            {
                const string error = "Tracked features vector size is not equal to bootstrapped one.";
                throw new Exception(error);
            }

            //verify features with a homography
            var inlierMask = new VectorOfByte();
            var homography = new Mat();
            if (_trackedFeatures.Size > 4)
                CvInvoke.FindHomography(Utils.GetPointsVector(_trackedFeatures), Utils.GetPointsVector(_bootstrapKp), homography, HomographyMethod.Ransac, 0.99,
                    inlierMask);

            var homographyMatrix = new Matrix<double>(homography.Rows, homography.Cols, homography.DataPointer);

            int inliersNum = CvInvoke.CountNonZero(inlierMask);

            if (inliersNum != _trackedFeatures.Size && inliersNum >= 4 && !homography.IsEmpty)
            {
                Utils.KeepVectorsByStatus(ref _trackedFeatures, ref _bootstrapKp, inlierMask);
            }
            else if (inliersNum < 10)
            {
                throw new Exception("Not enough features survived homography.");
            }

            var bootstrapKpOrig = new VectorOfKeyPoint(_bootstrapKp.ToArray());
            var trackedFeaturesOrig = new VectorOfKeyPoint(_trackedFeatures.ToArray());

            //TODO: Compare all these to c++ version
            //Attempt at 3D reconstruction (triangulation) if conditions are right
            var rigidT = CvInvoke.EstimateRigidTransform(Utils.GetPointsVector(_trackedFeatures).ToArray(), Utils.GetPointsVector(_bootstrapKp).ToArray(), false);
            var matrix = new Matrix<double>(rigidT.Rows, rigidT.Cols, rigidT.DataPointer);

            if (CvInvoke.Norm(matrix.GetCol(2)) > 100)
            {
                //camera motion is sufficient
                var result = OpenCvUtilities.CameraPoseAndTriangulationFromFundamental(_calibrationInfo, _trackedFeatures, _bootstrapKp);

                _trackedFeatures = result.FilteredTrackedFeaturesKp;
                _bootstrapKp = result.FilteredBootstrapKp;

                if (result.Result)
                {
                    _trackedFeatures3D = result.TrackedFeatures3D;

                    var tf3D = new double[_trackedFeatures3D.Size, 3];
                    var trackedFeatures3Dm = new Matrix<double>(_trackedFeatures3D.Size, 3);
                    for (int k = 0; k < _trackedFeatures3D.Size; k++)
                    {
                        trackedFeatures3Dm[k, 0] = _trackedFeatures3D[k].X;
                        trackedFeatures3Dm[k, 1] = _trackedFeatures3D[k].Y;
                        trackedFeatures3Dm[k, 2] = _trackedFeatures3D[k].Z;

                        tf3D[k, 0] = _trackedFeatures3D[k].X;
                        tf3D[k, 1] = _trackedFeatures3D[k].Y;
                        tf3D[k, 2] = _trackedFeatures3D[k].Z;
                    }

                    var eigenvectors = new Mat();
                    var mean = new Mat();
                    CvInvoke.PCACompute(trackedFeatures3Dm, mean, eigenvectors);
                    var eigenvectorsMatrix = new Matrix<double>(eigenvectors.Rows, eigenvectors.Cols, eigenvectors.DataPointer);

                    double[] meanArr = tf3D.Mean(0);
                    var method = PrincipalComponentMethod.Center;
                    var pca = new PrincipalComponentAnalysis(method);
                    pca.Learn(tf3D.ToJagged());

                    int numInliers = 0;
                    var normalOfPlane = eigenvectorsMatrix.GetRow(2).ToUMat().ToMat(AccessType.Fast);
                    CvInvoke.Normalize(normalOfPlane, normalOfPlane);
                    var normalOfPlaneMatrix = new Matrix<double>(normalOfPlane.Rows, normalOfPlane.Cols, normalOfPlane.DataPointer);

                    double pToPlaneThresh = Math.Sqrt(pca.Eigenvalues.ElementAt(2));

                    var statusArray = new byte[_trackedFeatures3D.Size];
                    for (int k = 0; k < _trackedFeatures3D.Size; k++)
                    {
                        var t1 = new double[] { _trackedFeatures3D[k].X, _trackedFeatures3D[k].Y, _trackedFeatures3D[k].Z };
                        var t2 = t1.Subtract(meanArr);
                        var w = new Matrix<double>(new[,] { { t2[0], t2[1], t2[2] } });
                        double d = Math.Abs(normalOfPlane.Dot(w));
                        if (d < pToPlaneThresh)
                        {
                            numInliers++;
                            statusArray[k] = 1;
                        }
                    }

                    var statusVector = new VectorOfByte(statusArray);

                    var bootstrapping = numInliers / (double)_trackedFeatures3D.Size < 0.75;
                    if (!bootstrapping)
                    {
                        //enough features are coplanar, keep them and flatten them on the XY plane
                        Utils.KeepVectorsByStatus(ref _trackedFeatures, ref _trackedFeatures3D, statusVector);

                        //the PCA has the major axes of the plane
                        var projected = new Mat();
                        CvInvoke.PCAProject(trackedFeatures3Dm, mean, eigenvectors, projected);
                        var projectedMatrix = new Matrix<double>(projected.Rows, projected.Cols, projected.DataPointer);
                        projectedMatrix.GetCol(2).SetValue(0);
                        projectedMatrix.CopyTo(trackedFeatures3Dm);

                        return true;
                    }
                    else
                    {
                        //cerr << "not enough features are coplanar" << "\n";
                        _bootstrapKp = bootstrapKpOrig;
                        _trackedFeatures = trackedFeaturesOrig;
                    }
                }
            }

            return false;
        }

        //public void Process(Mat img, bool newmap)
        //{
        //    bool result;
        //    if (newmap)
        //    {
        //        Bootstrap(img);
        //        _bootstrapping = true;
        //    }
        //    else if (_bootstrapping)
        //    {
        //        result = BootstrapTrack(img);
        //        if (result)
        //        {
        //            _bootstrapping = false;
        //        }
        //    }
        //    else if (!newmap && !_bootstrapping)
        //    {
        //        result = Track(img);
        //        if (result)
        //        {
        //        }
        //    }
        //}

        #endregion
    }
}
