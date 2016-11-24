using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using OpenTK.Graphics.OpenGL;
using RubiksCube.OpenCV.TestCase.AugmentedReality;

namespace RubiksCube.OpenCV.TestCase.PtamLikeApproach
{
    public struct TriangulateAndCheckReprojResult
    {
        public double Error { get; set; }
        public VectorOfKeyPoint FilteredTrackedFeaturesKp { get; set; }
        public VectorOfKeyPoint FilteredBootstrapKp { get; set; }
        public VectorOfPoint3D32F TrackedFeatures3D { get; set; }
        public bool Result { get; set; }
    }

    public struct CameraPoseAndTriangulationFromFundamentalResult
    {
        public Matrix<double> P1 { get; set; }
        public Matrix<double> P2 { get; set; }

        public Matrix<double> Esential { get; set; }

        public float Min { get; set; }
        public float Max { get; set; }

        public VectorOfKeyPoint FilteredTrackedFeaturesKp { get; set; }
        public VectorOfKeyPoint FilteredBootstrapKp { get; set; }
        public VectorOfPoint3D32F TrackedFeatures3D { get; set; }

        public bool Result { get; set; }
    }

    public class OpenCvUtilities
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static bool DecomposeEtoRandT(IInputArray e, out Matrix<double> r1, out Matrix<double> r2, out Matrix<double> t1, out Matrix<double> t2)
        {
            r1 = null;
            r2 = null;
            t1 = new Matrix<double>(3, 1);
            t2 = new Matrix<double>(3, 1);

            //Using HZ E decomposition

            var w = new Mat();
            var u = new Mat();
            var vt = new Mat();
            CvInvoke.SVDecomp(e, w, u, vt, SvdFlag.ModifyA);

            using (var wm = new Matrix<double>(w.Rows, w.Cols, w.DataPointer))
            using (var um = new Matrix<double>(u.Rows, u.Cols, u.DataPointer))
            using (var vtm = new Matrix<double>(vt.Rows, vt.Cols, vt.DataPointer))
            {
                //check if first and second singular values are the same (as they should be)
                double singularValuesRatio = Math.Abs(wm[0, 0] / wm[1, 0]);
                if (singularValuesRatio > 1.0) singularValuesRatio = 1.0f / singularValuesRatio; // flip ratio to keep it [0,1]
                if (singularValuesRatio < 0.7)
                {
                    Trace.WriteLine("Singular values of essential matrix are too far apart.");
                    return false;
                }

                var wMat = new Matrix<double>(new double[,] {
                    { 0, -1, 0}, //HZ 9.13
                    { 1, 0, 0 },
                    { 0, 0, 1 }
                });

                var wMatTranspose = new Matrix<double>(new double[,] {
                    { 0, 1, 0},
                    { -1, 0, 0 },
                    { 0, 0, 1 }
                });

                // vm maybe should be transposed.
                r1 = um * wMat * vtm; //HZ 9.19
                r2 = um * wMatTranspose * vtm; //HZ 9.19
                um.GetCol(2).CopyTo(t1); //u3
                um.GetCol(2).CopyTo(t2); //u3
                Utils.Negotiate(ref t2);

                wMat.Dispose();
                wMatTranspose.Dispose();

                w.Dispose();
                u.Dispose();
                vt.Dispose();
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="calibrationInfo"></param>
        /// <param name="trackedFeaturesKp"></param>
        /// <param name="bootstrapKp"></param>
        /// <param name="p"></param>
        /// <param name="p1"></param>
        /// <returns></returns>
        public static TriangulateAndCheckReprojResult TriangulateAndCheckReproj(CameraCalibrationInfo calibrationInfo, VectorOfKeyPoint trackedFeaturesKp, VectorOfKeyPoint bootstrapKp, Matrix<double> p, Matrix<double> p1)
        {
            var result = new TriangulateAndCheckReprojResult();

            var trackedFeaturesPoints = Utils.GetPointsVector(trackedFeaturesKp);
            var bootstrapPoints = Utils.GetPointsVector(bootstrapKp);

            //undistort
            var normalizedTrackedPts = new VectorOfPointF();
            var normalizedBootstrapPts = new VectorOfPointF();

            CvInvoke.UndistortPoints(trackedFeaturesPoints, normalizedTrackedPts, calibrationInfo.Intrinsic, calibrationInfo.Distortion);
            CvInvoke.UndistortPoints(bootstrapPoints, normalizedBootstrapPts, calibrationInfo.Intrinsic, calibrationInfo.Distortion);

            //triangulate
            var pt3Dh = new Mat();
            CvInvoke.TriangulatePoints(p, p1, normalizedBootstrapPts, normalizedTrackedPts, pt3Dh);
            var pt3DhMatrix = new Matrix<float>(pt3Dh.Rows, pt3Dh.Cols, pt3Dh.DataPointer);

            var pt3DMat = new Mat();
            CvInvoke.ConvertPointsFromHomogeneous(pt3DhMatrix.Transpose(), pt3DMat);
            var pt3D = new Matrix<float>(pt3DMat.Rows, pt3DMat.Cols * pt3DMat.NumberOfChannels, pt3DMat.DataPointer);

            var statusArray = new byte[pt3D.Rows];
            for (int i = 0; i < pt3D.Rows; i++)
            {
                statusArray[i] = (pt3D[i, 2] > 0) ? (byte)1 : (byte)0;
            }

            var status = new VectorOfByte(statusArray);
            int count = CvInvoke.CountNonZero(status);

            double percentage = count / (double)pt3D.Rows;
            if (percentage > 0.75)
            {
                //calculate reprojection
                var rvec = new VectorOfFloat(new float[] { 0, 0, 0 });
                var tvec = new VectorOfFloat(new float[] { 0, 0, 0 });
                var reprojectedPtSet1 = new VectorOfPointF();
                CvInvoke.ProjectPoints(pt3D, rvec, tvec, calibrationInfo.Intrinsic, calibrationInfo.Distortion,
                    reprojectedPtSet1);

                double reprojErr = CvInvoke.Norm(reprojectedPtSet1, bootstrapPoints) / bootstrapPoints.Size;
                if (reprojErr < 5)
                {
                    statusArray = new byte[bootstrapPoints.Size];
                    for (int i = 0; i < bootstrapPoints.Size; ++i)
                    {
                        var pointsDiff = Utils.SubstarctPoints(bootstrapPoints[i], reprojectedPtSet1[i]);
                        var vectorOfPoint = new VectorOfPointF(new[] { pointsDiff });
                        statusArray[i] = CvInvoke.Norm(vectorOfPoint) < 20.0 ? (byte)1 : (byte)0;
                    }

                    status = new VectorOfByte(statusArray);

                    var trackedFeatures3D = new VectorOfPoint3D32F(Utils.Get3dPointsArray(pt3D));

                    Utils.KeepVectorsByStatus(ref trackedFeaturesKp, ref trackedFeatures3D, status);

                    result.Error = reprojErr;
                    result.TrackedFeatures3D = new VectorOfPoint3D32F(trackedFeatures3D.ToArray());
                    result.FilteredTrackedFeaturesKp = new VectorOfKeyPoint(trackedFeaturesKp.ToArray());
                    result.FilteredBootstrapKp = new VectorOfKeyPoint(bootstrapKp.ToArray());
                    result.Result = true;
                }

                rvec.Dispose();
                tvec.Dispose();
                reprojectedPtSet1.Dispose();
            }

            normalizedTrackedPts.Dispose();
            normalizedBootstrapPts.Dispose();
            pt3Dh.Dispose();
            pt3DhMatrix.Dispose();
            pt3DMat.Dispose();
            pt3D.Dispose();
            status.Dispose();

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="calibrationInfo"></param>
        /// <param name="trackedFeaturesKp"></param>
        /// <param name="bootstrapKp"></param>
        /// <param name="initialP1"></param>
        /// <param name="minInliers"></param>
        /// <returns></returns>
        public static CameraPoseAndTriangulationFromFundamentalResult CameraPoseAndTriangulationFromFundamental(CameraCalibrationInfo calibrationInfo, VectorOfKeyPoint trackedFeaturesKp, 
            VectorOfKeyPoint bootstrapKp, Matrix<double> initialP1, int minInliers = 10)
        {
            var result = new CameraPoseAndTriangulationFromFundamentalResult();

            //find fundamental matrix
            double minVal, maxVal;
            var minIdx = new int[2];
            var maxIdx = new int[2];
            var trackedFeaturesPts = Utils.GetPointsVector(trackedFeaturesKp);
            var bootstrapPts = Utils.GetPointsVector(bootstrapKp);

            CvInvoke.MinMaxIdx(Utils.GetPointsMatrix(trackedFeaturesPts), out minVal, out maxVal, minIdx, maxIdx);
            result.Min = (float)minVal;
            result.Max = (float)maxVal;

            var f = new Mat();
            var status = new VectorOfByte();
            CvInvoke.FindFundamentalMat(trackedFeaturesPts, bootstrapPts, f, FmType.Ransac, 0.006 * maxVal, 0.99, status);
            var fMat = new Matrix<double>(f.Rows, f.Cols, f.DataPointer);

            int inliersNum = CvInvoke.CountNonZero(status);

            Trace.WriteLine($"Fundamental keeping {inliersNum} / {status.Size}");

            Utils.KeepVectorsByStatus(ref trackedFeaturesKp, ref bootstrapKp, status);

            if (inliersNum > minInliers)
            {
                //Essential matrix: compute then extract cameras [R|t]
                var e = calibrationInfo.Intrinsic.Transpose() * fMat * calibrationInfo.Intrinsic; //according to HZ (9.12)
                result.Esential = e;

                //according to http://en.wikipedia.org/wiki/Essential_matrix#Properties_of_the_essential_matrix
                var determinant = Math.Abs(CvInvoke.Determinant(e));
                if (determinant > 1e-07)
                {
                    Console.WriteLine($"det(E) != 0 : {determinant}");
                    return result;
                }

                Matrix<double> r1;
                Matrix<double> r2;
                Matrix<double> t1;
                Matrix<double> t2;
                if (!DecomposeEtoRandT(e, out r1, out r2, out t1, out t2))
                    return result;

                determinant = CvInvoke.Determinant(r1);
                if (determinant + 1.0 < 1e-09)
                {
                    //according to http://en.wikipedia.org/wiki/Essential_matrix#Showing_that_it_is_valid
                    Trace.WriteLine($"det(R) == -1 [{determinant}]: flip E's sign");
                    Utils.Negotiate(ref e);
                    if (!DecomposeEtoRandT(e, out r1, out r2, out t1, out t2))
                        return result;
                }
                if (Math.Abs(determinant) - 1.0 > 1e-07)
                {
                    Trace.WriteLine($"det(R) != +-1.0, this is not a rotation matrix");
                    return result;
                }

                var trPts = Utils.GetPointsVector(trackedFeaturesKp);
                var btPts = Utils.GetPointsVector(bootstrapKp);

                var p = initialP1.Clone();

                //TODO: there are 4 different combinations for P1...
                var pMat1 = new Matrix<double>(new[,] {
                    { r1[0,0], r1[0,1], r1[0,2], t1[0,0] },
                    { r1[1,0], r1[1,1], r1[1,2], t1[1,0] },
                    { r1[2,0], r1[2,1], r1[2,2], t1[2,0] }
                });

                var triangulationResult = TriangulateAndCheckReproj(calibrationInfo, trackedFeaturesKp, bootstrapKp, p, pMat1);
                if (!triangulationResult.Result)
                {
                    pMat1 = new Matrix<double>(new[,] {
                        { r1[0,0], r1[0,1], r1[0,2], t2[0,0] },
                        { r1[1,0], r1[1,1], r1[1,2], t2[1,0] },
                        { r1[2,0], r1[2,1], r1[2,2], t2[2,0] }
                    });

                    triangulationResult = TriangulateAndCheckReproj(calibrationInfo, trackedFeaturesKp, bootstrapKp, p, pMat1);
                    if (!triangulationResult.Result)
                    {
                        pMat1 = new Matrix<double>(new[,] {
                            { r2[0,0], r2[0,1], r2[0,2], t2[0,0] },
                            { r2[1,0], r2[1,1], r2[1,2], t2[1,0] },
                            { r2[2,0], r2[2,1], r2[2,2], t2[2,0] }
                        });

                        triangulationResult = TriangulateAndCheckReproj(calibrationInfo, trackedFeaturesKp, bootstrapKp, p, pMat1);
                        if (!triangulationResult.Result)
                        {
                            pMat1 = new Matrix<double>(new[,] {
                                { r2[0,0], r2[0,1], r2[0,2], t1[0,0] },
                                { r2[1,0], r2[1,1], r2[1,2], t1[1,0] },
                                { r2[2,0], r2[2,1], r2[2,2], t1[2,0] }
                            });

                            triangulationResult = TriangulateAndCheckReproj(calibrationInfo, trackedFeaturesKp, bootstrapKp, p, pMat1);
                            if (!triangulationResult.Result)
                            {
                                Trace.WriteLine("Can't find the right P matrix.");
                            }
                        }

                    }
                }

                result.P1 = p;
                result.P2 = pMat1;
                result.FilteredTrackedFeaturesKp = triangulationResult.FilteredTrackedFeaturesKp;
                result.FilteredBootstrapKp = triangulationResult.FilteredBootstrapKp;
                result.TrackedFeatures3D = triangulationResult.TrackedFeatures3D;
                result.Result = triangulationResult.Result;
                return result;
            }

            return result;
        }
    }
}
