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
using OpenTK.Graphics.ES11;
using RubiksCube.OpenCV.TestCase.AugmentedReality;

namespace RubiksCube.OpenCV.TestCase.PtamLikeApproach
{
    public class PtamLikeAlgorithm
    {
        #region Fields

        private const double RansacThreshold = 0.99;
        private const int MinInliers = 10;

        private bool _canCalcMvm;

        private readonly Mat _prevGray;
        private readonly Mat _currGray;

        private VectorOfFloat _raux;
        private VectorOfFloat _taux;

        private readonly ORBDetector _detector;

        private VectorOfKeyPoint _bootstrapKp;
        private VectorOfKeyPoint _trackedFeatures;
        private VectorOfPoint3D32F _trackedFeatures3D;

        private readonly CameraCalibrationInfo _calibrationInfo;

        #endregion

        #region Properties

        public VectorOfPoint3D32F TrackedFeatures3D => _trackedFeatures3D;
        public VectorOfKeyPoint TrackedFeatures => _trackedFeatures;

        public bool IsBootstrapping { get; private set; }
        public bool IsTracking { get; private set; }

        public VectorOfFloat Raux => _raux;
        public VectorOfFloat Taux => _taux;

        public Matrix<double> InitialP1 { get; private set; }

        #endregion

        #region .ctor

        public PtamLikeAlgorithm(CameraCalibrationInfo calibrationInfo)
        {
            _calibrationInfo = calibrationInfo;

            _detector = new ORBDetector();

            _prevGray = new Mat();
            _currGray = new Mat();

            _raux = new VectorOfFloat();
            _taux = new VectorOfFloat();

            _bootstrapKp = new VectorOfKeyPoint();
            _trackedFeatures = new VectorOfKeyPoint();
            _trackedFeatures3D = new VectorOfPoint3D32F();

            InitialP1 = new Matrix<double>(3, 4);
            InitialP1.SetIdentity();
        }

        #endregion

        #region Public Methods

        public void ResetAlgorithm()
        {
            IsBootstrapping = false;
            IsTracking = false;
        }

        public void Bootstrap(Mat img)
        {
            ValidateImages(null, img);

            _bootstrapKp.Clear();
            _detector.DetectRaw(img, _bootstrapKp);

            _trackedFeatures = new VectorOfKeyPoint(_bootstrapKp.ToArray());

            _trackedFeatures3D.Clear();

            CvInvoke.CvtColor(img, _prevGray, ColorConversion.Bgr2Gray);
        }

        public bool BootstrapTrack(Mat img, Action<Matrix<double>, VectorOfPoint3D32F, Point[]> onPlaneFound)
        {
            ValidateImages(_prevGray, img);

            CvInvoke.CvtColor(img, _currGray, ColorConversion.Bgr2Gray);

            ComputeOpticalFlowAndValidate(_prevGray, _currGray, ref _trackedFeatures, ref _bootstrapKp, img);

            Matrix<double> homography;
            ComputeHomographyAndValidate(ref _trackedFeatures, ref _bootstrapKp, out homography);

            var bootstrapKpOrig = new VectorOfKeyPoint(_bootstrapKp.ToArray());
            var trackedFeaturesOrig = new VectorOfKeyPoint(_trackedFeatures.ToArray());

            //Attempt at 3D reconstruction (triangulation) if conditions are right
            var rigidT = CvInvoke.EstimateRigidTransform(Utils.GetPointsVector(_trackedFeatures).ToArray(), Utils.GetPointsVector(_bootstrapKp).ToArray(), false);
            var matrix = new Matrix<double>(rigidT.Rows, rigidT.Cols, rigidT.DataPointer);

            if (CvInvoke.Norm(matrix.GetCol(2)) > 100)
            {
                //camera motion is sufficient
                var result = OpenCvUtilities.CameraPoseAndTriangulationFromFundamental(_calibrationInfo, _trackedFeatures, _bootstrapKp, InitialP1);
                if (result.Result)
                {
                    _trackedFeatures = result.FilteredTrackedFeaturesKp;
                    _bootstrapKp = result.FilteredBootstrapKp;

                    _trackedFeatures3D = result.TrackedFeatures3D;

                    int numInliers;
                    Matrix<double> trackedFeatures3Dm;
                    Mat mean;
                    Mat eigenvectors;
                    Matrix<double> normalOfPlaneMatrix;
                    var statusVector = ComputeNormalAndPlaneInliers(_trackedFeatures3D, out trackedFeatures3Dm, out mean, out eigenvectors, out numInliers, out normalOfPlaneMatrix);

                    bool bootstrapping = numInliers / (double)_trackedFeatures3D.Size < 0.75;
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

                        //InitialP1 = result.P2;

                        VectorOfFloat raux;
                        VectorOfFloat taux;

                        ComputeRotationAndTranslation(_trackedFeatures3D, _trackedFeatures, _calibrationInfo, out raux, out taux);

                        double angle = ComputeAngleBetweenCameraNormAndPlaneNorm(_trackedFeatures3D, normalOfPlaneMatrix, raux, taux);
                        if (angle > 65 && angle < 110)
                        {
                            var points2D = FindFaceCorners(_currGray, _trackedFeatures);
                            onPlaneFound(normalOfPlaneMatrix.Clone(), new VectorOfPoint3D32F(_trackedFeatures3D.ToArray()), points2D.Points);

                            var face = ExtractFace(img, points2D);
                            face.Save("C:\\Users\\zakharov\\Documents\\Repos\\Mine\\Rc\\src\\RubiksCube.OpenCV.TestCase\\extracted.jpg");
                        }

                        return true;
                    }

                    //cerr << "not enough features are coplanar" << "\n";
                    _bootstrapKp = bootstrapKpOrig;
                    _trackedFeatures = trackedFeaturesOrig;
                }
            }

            return false;
        }

        public bool Track(Mat img)
        {
            ValidateImages(_prevGray, img);

            CvInvoke.CvtColor(img, _currGray, ColorConversion.Bgr2Gray);

            ComputeOpticalFlowAndValidate(_prevGray, _currGray, ref _trackedFeatures, ref _bootstrapKp);

            _canCalcMvm = _trackedFeatures.Size >= MinInliers;

            if (!_canCalcMvm) return false;

            ComputeRotationAndTranslation(_trackedFeatures3D, _trackedFeatures, _calibrationInfo, out _raux, out _taux);

            var rotationMat = new Mat();
            CvInvoke.Rodrigues(_raux, rotationMat);
            var rotationMatrix = new Matrix<double>(rotationMat.Rows, rotationMat.Cols, rotationMat.DataPointer);

            var cvToGl = new Matrix<double>(4, 4);
            cvToGl.SetZero();

            //cvToGl[0, 0] = 1.0f;
            //cvToGl[1, 1] = -1.0f;// Invert the y axis
            //cvToGl[2, 2] = -1.0f;// invert the z axis
            //cvToGl[3, 3] = 1.0f;

            // [R | t] matrix
            //Mat_<double> para = Mat_ < double >::eye(4, 4);
            //Rot.convertTo(para(cv::Rect(0, 0, 3, 3)), CV_64F);
            //Tvec.copyTo(para(cv::Rect(3, 0, 1, 3)));
            //para = cvToGl * para;

            var pose3D = new Transformation();
            // Copy to transformation matrix
            for (int col = 0; col < 3; col++)
            {
                for (int row = 0; row < 3; row++)
                {
                    pose3D.SetRotationMatrixValue(row, col, (float)rotationMatrix[row, col]); // Copy rotation component
                }
                pose3D.SetTranslationVectorValue(col, _taux[col]); // Copy translation component
            }

            // Since solvePnP finds camera location, w.r.t to marker pose, to get marker pose w.r.t to the camera we invert it.
            pose3D = pose3D.GetInverted();

            //Mat(para.t()).copyTo(modelview_matrix); // transpose to col-major for OpenGL

            return true;
        }

        public void Process(Mat img, bool newMap, Action<Matrix<double>, VectorOfPoint3D32F, Point[]> onPlaneFound)
        {
            bool result;
            if (newMap)
            {
                Bootstrap(img);
                IsBootstrapping = true;
            }
            else if (IsBootstrapping)
            {
                result = BootstrapTrack(img, onPlaneFound);
                if (result)
                {
                    IsBootstrapping = false;
                }
            }
            //else if (!IsBootstrapping)
            //{
            //    result = Track(img);
            //    if (result)
            //    {
            //        IsTracking = true;
            //    }
            //}
        }

        #endregion

        #region Private Methods

        private static void ValidateImages(Mat prevGray, Mat img)
        {
            if (prevGray != null && prevGray.IsEmpty)
            {
                const string error = "Previous frame is empty. Bootstrap first.";
                throw new Exception(error);
            }

            if (img != null && img.IsEmpty || !img.IsEmpty && img.NumberOfChannels != 3)
            {
                const string error = "Image is not appropriate (Empty or wrong number of channels).";
                throw new Exception(error);
            }
        }

        private static void ComputeOpticalFlowAndValidate(Mat prevGray, Mat currGray, ref VectorOfKeyPoint trackedFeatures, ref VectorOfKeyPoint bootstrapKp, Mat img = null)
        {
            var corners = new VectorOfPointF();
            var status = new VectorOfByte();
            var errors = new VectorOfFloat();

            CvInvoke.CalcOpticalFlowPyrLK(prevGray, currGray, Utils.GetPointsVector(trackedFeatures), corners,
              status, errors, new Size(11, 11), 3, new MCvTermCriteria(30, 0.01));
            currGray.CopyTo(prevGray);

            if (img != null)
            {
                for (int j = 0; j < corners.Size; j++)
                {
                    if (status[j] == 1)
                        CvInvoke.Line(img, new Point((int)trackedFeatures[j].Point.X, (int)trackedFeatures[j].Point.Y),
                            new Point((int)corners[j].X, (int)corners[j].Y), new MCvScalar(120, 10, 20));
                }
            }

            if (CvInvoke.CountNonZero(status) < status.Size * 0.8)
                throw new Exception("Tracking failed.");

            trackedFeatures = Utils.GetKeyPointsVector(corners);

            Utils.KeepVectorsByStatus(ref trackedFeatures, ref bootstrapKp, status);

            if (trackedFeatures.Size != bootstrapKp.Size)
            {
                const string error = "Tracked features vector size is not equal to bootstrapped one.";
                throw new Exception(error);
            }
        }

        private static void ComputeHomographyAndValidate(ref VectorOfKeyPoint trackedFeatures, ref VectorOfKeyPoint bootstrapKp, out Matrix<double> homographyMatrix)
        {
            //verify features with a homography
            var inlierMask = new VectorOfByte();
            var homography = new Mat();
            if (trackedFeatures.Size > 4)
                CvInvoke.FindHomography(Utils.GetPointsVector(trackedFeatures), Utils.GetPointsVector(bootstrapKp), homography, HomographyMethod.Ransac, RansacThreshold,
                    inlierMask);

            homographyMatrix = new Matrix<double>(homography.Rows, homography.Cols, homography.DataPointer);

            int inliersNum = CvInvoke.CountNonZero(inlierMask);

            if (inliersNum != trackedFeatures.Size && inliersNum >= 4 && !homography.IsEmpty)
            {
                Utils.KeepVectorsByStatus(ref trackedFeatures, ref bootstrapKp, inlierMask);
            }
            else if (inliersNum < 10)
            {
                throw new Exception("Not enough features survived homography.");
            }
        }

        private static VectorOfByte ComputeNormalAndPlaneInliers(VectorOfPoint3D32F trackedFeatures3D, out Matrix<double> trackedFeatures3Dm, out Mat mean, out Mat eigenvectors, out int numInliers, out Matrix<double> normalOfPlaneMatrix)
        {
            //var tf3D = new double[_trackedFeatures3D.Size, 3];
            //var trackedFeatures3Dm = new Matrix<double>(trackedFeatures3D.Size, 3);
            trackedFeatures3Dm = new Matrix<double>(trackedFeatures3D.Size, 3);
            for (int k = 0; k < trackedFeatures3D.Size; k++)
            {
                trackedFeatures3Dm[k, 0] = trackedFeatures3D[k].X;
                trackedFeatures3Dm[k, 1] = trackedFeatures3D[k].Y;
                trackedFeatures3Dm[k, 2] = trackedFeatures3D[k].Z;

                //tf3D[k, 0] = _trackedFeatures3D[k].X;
                //tf3D[k, 1] = _trackedFeatures3D[k].Y;
                //tf3D[k, 2] = _trackedFeatures3D[k].Z;
            }

            //var eigenvectors = new Mat();
            //var mean = new Mat();
            eigenvectors = new Mat();
            mean = new Mat();
            CvInvoke.PCACompute(trackedFeatures3Dm, mean, eigenvectors);
            var eigenvectorsMatrix = new Matrix<double>(eigenvectors.Rows, eigenvectors.Cols, eigenvectors.DataPointer);

            //double[] meanArr = tf3D.Mean(0);
            var meanArr = trackedFeatures3Dm.Data.Mean(0);
            var pca = new PrincipalComponentAnalysis(PrincipalComponentMethod.Center);
            pca.Learn(trackedFeatures3Dm.Data.ToJagged());

            //int numInliers = 0;
            numInliers = 0;
            var normalOfPlane = eigenvectorsMatrix.GetRow(2).ToUMat().ToMat(AccessType.Fast);
            CvInvoke.Normalize(normalOfPlane, normalOfPlane);
            //var normalOfPlaneMatrix = new Matrix<double>(normalOfPlane.Rows, normalOfPlane.Cols, normalOfPlane.DataPointer);
            normalOfPlaneMatrix = new Matrix<double>(normalOfPlane.Rows, normalOfPlane.Cols, normalOfPlane.DataPointer);

            double pToPlaneThresh = Math.Sqrt(pca.Eigenvalues.ElementAt(2));
            var statusArray = new byte[trackedFeatures3D.Size];
            for (int k = 0; k < trackedFeatures3D.Size; k++)
            {
                var t1 = new double[] { trackedFeatures3D[k].X, trackedFeatures3D[k].Y, trackedFeatures3D[k].Z };
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
            return statusVector;
        }

        private static void ComputeRotationAndTranslation(VectorOfPoint3D32F trackedFeatures3D, VectorOfKeyPoint trackedFeatures, CameraCalibrationInfo calibrationInfo, out VectorOfFloat raux, out VectorOfFloat taux)
        {
            var rotationVector32F = new VectorOfFloat();
            var translationVector32F = new VectorOfFloat();
            var rotationVector = new Mat();
            var translationVector = new Mat();

            CvInvoke.SolvePnP(trackedFeatures3D, Utils.GetPointsVector(trackedFeatures), calibrationInfo.Intrinsic, calibrationInfo.Distortion, rotationVector, translationVector);

            rotationVector.ConvertTo(rotationVector32F, DepthType.Cv32F);
            translationVector.ConvertTo(translationVector32F, DepthType.Cv32F);

            raux = rotationVector32F;
            taux = translationVector32F;
        }

        private static double ComputeAngleBetweenCameraNormAndPlaneNorm(VectorOfPoint3D32F trackedFeatures3D, Matrix<double> normal, VectorOfFloat raux, VectorOfFloat taux)
        {
            var tvec = taux.ToArray().Select(i => (double)i).ToArray();

            var rotationMat = new Mat();
            CvInvoke.Rodrigues(raux, rotationMat);
            var rotationMatrix = new Matrix<double>(rotationMat.Rows, rotationMat.Cols, rotationMat.DataPointer);

            // ???
            Utils.Negotiate(ref rotationMatrix);

            var cameraPosition = rotationMatrix * new Matrix<double>(tvec);
            var cameraPositionPoint = new MCvPoint3D32f((float)cameraPosition[0, 0], (float)cameraPosition[1, 0], (float)cameraPosition[2, 0]);

            var cameraVector = trackedFeatures3D[0] - cameraPositionPoint;

            Func<double, double> radianToDegree = angle => angle * (180.0 / Math.PI);

            double dotProduct = new double[] { cameraVector.X, cameraVector.Y, cameraVector.Z }.Dot(new[] { normal[0, 0], normal[0, 1], normal[0, 2] });
            double acos = Math.Acos(dotProduct);
            double anglResult = radianToDegree(acos);

            Console.WriteLine($"Normal: [{normal.Data[0, 0]}, {normal.Data[0, 1]}, {normal.Data[0, 2]}]");
            Console.WriteLine($"Angle: {anglResult}");
            Console.WriteLine($"Dot product: {dotProduct}");

            return anglResult;
        }

        private static FaceCorners FindFaceCorners(Mat img, VectorOfKeyPoint trackedFeatures)
        {
            var rect = CvInvoke.BoundingRectangle(Utils.GetPointsVector(trackedFeatures));

            var edges = new Mat();
            CvInvoke.Canny(img, edges, 0.1, 99);

            var contours = new VectorOfVectorOfPoint();
            var hierarchy = new Mat();

            var matchedContours = new List<VectorOfPoint>();

            CvInvoke.FindContours(edges, contours, hierarchy, RetrType.Tree, ChainApproxMethod.ChainApproxSimple);

            for (int i = 0; i < contours.Size; i++)
            {
                var approx = new Mat();
                CvInvoke.ApproxPolyDP(contours[i], approx, 9, true);

                var approxMat = new Matrix<double>(approx.Rows, approx.Cols, approx.DataPointer);

                double a = Math.Abs(CvInvoke.ContourArea(approx));
                if (approxMat.Rows == 4 && Math.Abs(CvInvoke.ContourArea(approx)) > 3000 &&
                        CvInvoke.IsContourConvex(approx))
                {
                    matchedContours.Add(contours[i]);
                }
            }

            var contoursArray = matchedContours.ToArray().SelectMany(c => c.ToArray()).ToArray();

            var contour = matchedContours.FirstOrDefault();
            var bbox = CvInvoke.BoundingRectangle(contour);

            int minX = contoursArray.Min(p => p.X);
            int maxX = contoursArray.Max(p => p.X);

            if (!(maxX - minX > 2.9 * bbox.Width))
                return new FaceCorners();

            while (true)
            {
                bool sutisfied = contoursArray.All(p => p.X > rect.X && p.Y > rect.Y && p.X < (rect.X + rect.Width) && p.Y < (rect.Y + rect.Height));
                if (sutisfied)
                    break;

                if (contoursArray.Any(c => c.X < rect.X))
                    rect.X -= 5;

                if (contoursArray.Any(c => c.Y < rect.Y))
                    rect.Y -= 5;

                if (contoursArray.Any(c => c.X > (rect.X + rect.Width)))
                    rect.Width += 5;

                if (contoursArray.Any(c => c.Y > (rect.Y + rect.Height)))
                    rect.Height += 5;
            }

            var rectPoints = new[]
            {
                rect.Location,
                new PointF(rect.X, rect.Y + rect.Height),
                new PointF(rect.X + rect.Width, rect.Y),
                new PointF(rect.X + rect.Width, rect.Y + rect.Height)
            };
            var points2D = Array.ConvertAll(rectPoints, Point.Round);

            var corners = new FaceCorners(points2D);

            return corners;
        }

        private static Mat ExtractFace(Mat img, FaceCorners corners)
        {
            double widthA = Math.Sqrt(Math.Pow(corners.BottomRight.X - corners.BottomLeft.X, 2) + Math.Pow(corners.BottomRight.Y - corners.BottomLeft.Y, 2));
            double widthB = Math.Sqrt(Math.Pow(corners.TopRight.X - corners.TopLeft.X, 2) + Math.Pow(corners.TopRight.Y - corners.TopLeft.Y, 2));
            double maxWidth = Math.Max(widthA, widthB);

            double heightA = Math.Sqrt(Math.Pow(corners.TopRight.X - corners.BottomRight.X, 2) + Math.Pow(corners.TopRight.Y - corners.BottomRight.Y, 2));
            double heightB = Math.Sqrt(Math.Pow(corners.TopLeft.X - corners.BottomLeft.X, 2) + Math.Pow(corners.TopLeft.Y - corners.BottomLeft.Y, 2));
            double maxHeight = Math.Max(heightA, heightB);

            var a = Array.ConvertAll(corners.Points, p => (PointF)p);

            var perspectiveTransformationMatrix = CvInvoke.GetPerspectiveTransform(new VectorOfPointF(a), new VectorOfPointF(new[] {
                new PointF(0, 0),
                new PointF((int)maxWidth - 1, 0),
                new PointF((int)maxWidth - 1, (int)maxHeight - 1),
                new PointF(0, (int)maxHeight - 1),
            }));

            var warped = new Mat((int)maxWidth, (int)maxHeight, DepthType.Cv32F, 1);
            CvInvoke.WarpPerspective(img, warped, perspectiveTransformationMatrix, new Size((int)maxWidth, (int)maxHeight));

            return warped;
        }

        #endregion
    }
}
