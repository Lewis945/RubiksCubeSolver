using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using RubiksCube.OpenCV.TestCase.AugmentedReality;

namespace RubiksCube.OpenCV.TestCase.PtamLikeApproach
{
    public class OpenCvUtilities
    {
        public static bool DecomposeEtoRandT(IInputArray e, out Matrix<float> r1, out Matrix<float> r2, out Matrix<float> t1, out Matrix<float> t2)
        {
            r1 = null;
            r2 = null;
            t1 = new Matrix<float>(3, 1);
            t2 = new Matrix<float>(3, 1);

            //Using HZ E decomposition

            var w = new Mat();
            var u = new Mat();
            var vt = new Mat();
            CvInvoke.SVDecomp(e, w, u, vt, SvdFlag.ModifyA);

            using (var wm = new Matrix<float>(w.Rows, w.Cols, w.DataPointer))
            using (var um = new Matrix<float>(u.Rows, u.Cols, u.DataPointer))
            using (var vtm = new Matrix<float>(vt.Rows, vt.Cols, vt.DataPointer))
            {
                //check if first and second singular values are the same (as they should be)
                float singularValuesRatio = Math.Abs(wm[0, 0] / wm[1, 0]);
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
                r1 = um * wMat * vtm; //HZ 9.19
                r2 = um * wMatTranspose * vtm; //HZ 9.19
                um.GetCol(2).CopyTo(t1); //u3
                um.GetCol(2).CopyTo(t2); //u3
                Utils.Negotiate(ref t2);

                wMat.Dispose();
                wMatTranspose.Dispose();
            }

            return true;
        }

        public static bool TriangulateAndCheckReproj(CameraCalibrationInfo calibrationInfo, VectorOfPointF trackedFeatures, VectorOfPointF bootstrapKp, Matrix<float> p, Matrix<float> p1, out float error)
        {
            error = 0;

            //undistort
            var normalizedTrackedPts = new VectorOfPointF();
            var normalizedBootstrapPts = new VectorOfPointF();

            CvInvoke.UndistortPoints(trackedFeatures, normalizedTrackedPts, calibrationInfo.Intrinsic, calibrationInfo.Distortion);
            CvInvoke.UndistortPoints(bootstrapKp, normalizedBootstrapPts, calibrationInfo.Intrinsic, calibrationInfo.Distortion);

            //triangulate
            var pt3Dh = new Mat();
            CvInvoke.TriangulatePoints(p, p1, normalizedBootstrapPts, normalizedTrackedPts, pt3Dh);
            var pt3DhMatrix = new Matrix<float>(pt3Dh.Rows, pt3Dh.Cols, pt3Dh.DataPointer);

            var pt3DMat = new Mat();
            CvInvoke.ConvertPointsFromHomogeneous(pt3DhMatrix.Transpose(), pt3DMat);
            var pt3D = new Matrix<float>(pt3DMat.Rows, pt3DMat.Cols * pt3DMat.NumberOfChannels, pt3DMat.DataPointer);

            //Mat pt_3d; convertPointsFromHomogeneous(Mat(pt_3d_h.t()).reshape(4, 1), pt_3d);
            //    cout << pt_3d.size() << endl;
            //    cout << pt_3d.rowRange(0,10) << endl;

            var statusArray = new byte[pt3D.Rows];
            for (int i = 0; i < pt3D.Rows; i++)
            {
                statusArray[i] = (pt3D[i, 2] > 0) ? (byte)1 : (byte)0;
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
            CvInvoke.ProjectPoints(pt3D, rvec, tvec, calibrationInfo.Intrinsic, calibrationInfo.Distortion, reprojectedPtSet1);
            //    cout << Mat(reprojected_pt_set1).rowRange(0,10) << endl;
            var bootstrapPts = new VectorOfPointF(bootstrapKp.ToArray());
            //Mat bootstrapPts = new Mat(bootstrapPts_v);
            //    cout << bootstrapPts.rowRange(0,10) << endl;

            float reprojErr = (float)CvInvoke.Norm(reprojectedPtSet1, bootstrapPts) / bootstrapPts.Size;
            error = reprojErr;
            //cout << "reprojection Error " << reprojErr;
            if (reprojErr < 5)
            {
                statusArray = new byte[bootstrapPts.Size];
                for (int i = 0; i < bootstrapPts.Size; ++i)
                {
                    var pointsDiff = Utils.SubstarctPoints(bootstrapPts[i], reprojectedPtSet1[i]);
                    var vectorOfPoint = new VectorOfPointF(new[] { pointsDiff });
                    statusArray[i] = CvInvoke.Norm(vectorOfPoint) < 20.0 ? (byte)1 : (byte)0;
                }

                status = new VectorOfByte(statusArray);

                //_trackedFeatures3D = new VectorOfPoint3D32F(pt3D.Rows);
                //pt3D.CopyTo(new Matrix<MCvPoint3D32f>(_trackedFeatures3D.Size, 1, _trackedFeatures3D.Ptr));

                //Utils.KeepVectorsByStatus(ref _trackedFeatures, ref _trackedFeatures3D, status);
                //cout << "keeping " << trackedFeatures.size() << " nicely reprojected points";
                return true;
            }
            return false;
        }
    }
}
