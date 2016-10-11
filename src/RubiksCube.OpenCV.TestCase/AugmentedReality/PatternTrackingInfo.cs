using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubiksCube.OpenCV.TestCase.AugmentedReality
{
    public class PatternTrackingInfo
    {
        public Mat homography;
        public VectorOfPoint points2d;
        public Transformation pose3d;

        public PatternTrackingInfo()
        {
            homography = new Mat();
            points2d = new VectorOfPoint();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="color"></param>
        public void Draw2dContour(Mat image, MCvScalar color)
        {
            CvInvoke.Polylines(image, points2d, true, color, 2, Emgu.CV.CvEnum.LineType.AntiAlias);
        }

        /// <summary>
        /// Compute pattern pose using PnP algorithm
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="calibration"></param>
        public void computePose(Pattern pattern, CameraCalibrationInfo calibration)
        {
            VectorOfFloat rotationVector32f = new VectorOfFloat();
            VectorOfFloat translationVector32f = new VectorOfFloat();
            Mat rotationVector = new Mat();
            Mat translationVector = new Mat();

            var t1 = calibration.Intrinsic;
            var t2 = calibration.Distortion;

            var px1 = pattern.points3d.ToArray();
            var px2 = Array.ConvertAll<Point, PointF>(points2d.ToArray(), (a) => { return a; });

            CvInvoke.SolvePnP(px1, px2, t1, t2, rotationVector, translationVector);

            var p1 = new Matrix<float>(rotationVector.Rows, rotationVector.Cols, rotationVector.Ptr);
            var p2 = new Matrix<float>(translationVector.Rows, translationVector.Cols, translationVector.Ptr);

            rotationVector.ConvertTo(rotationVector32f, DepthType.Cv32F);
            translationVector.ConvertTo(translationVector32f, DepthType.Cv32F);

            Matrix<float> rotationMat = new Matrix<float>(3, 3);
            CvInvoke.Rodrigues(rotationVector32f, rotationMat);

            // Copy to transformation matrix
            for (int col = 0; col < 3; col++)
            {
                for (int row = 0; row < 3; row++)
                {
                    pose3d.m_rotation[row, col] = rotationMat[row, col]; // Copy rotation component
                }
                pose3d.m_translation[col] = translationVector32f[col]; // Copy translation component
            }

            // Since solvePnP finds camera location, w.r.t to marker pose, to get marker pose w.r.t to the camera we invert it.
            pose3d = pose3d.getInverted();
        }
    }
}
