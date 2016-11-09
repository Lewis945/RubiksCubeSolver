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
        private Mat _homography;
        private VectorOfPoint _points2d;
        private Transformation _pose3d;

        /// <summary>
        /// 
        /// </summary>
        public Mat Homography
        {
            get { return _homography; }
            set { _homography = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public VectorOfPoint Points2d
        {
            get { return _points2d; }
            set { _points2d = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Transformation Pose3d { get { return _pose3d; } }

        /// <summary>
        /// 
        /// </summary>
        public PatternTrackingInfo()
        {
            _homography = new Mat();
            _points2d = new VectorOfPoint();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="color"></param>
        public void Draw2dContour(Mat image, MCvScalar color)
        {
            CvInvoke.Polylines(image, _points2d, true, color, 2, Emgu.CV.CvEnum.LineType.AntiAlias);
        }

        /// <summary>
        /// Compute pattern pose using PnP algorithm
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="calibration"></param>
        public void ComputePose(Pattern pattern, CameraCalibrationInfo calibration)
        {
            VectorOfFloat rotationVector32f = new VectorOfFloat();
            VectorOfFloat translationVector32f = new VectorOfFloat();
            Mat rotationVector = new Mat();
            Mat translationVector = new Mat();

            var px1 = pattern.Points3d.ToArray();
            var px2 = Array.ConvertAll<Point, PointF>(_points2d.ToArray(), (a) => { return a; });

            CvInvoke.SolvePnP(px1, px2, calibration.Intrinsic, calibration.Distortion, rotationVector, translationVector);

            //var p1 = new Matrix<double>(rotationVector.Rows, rotationVector.Cols, rotationVector.Ptr);
            //var p2 = new Matrix<double>(translationVector.Rows, translationVector.Cols, translationVector.Ptr);

            rotationVector.ConvertTo(rotationVector32f, DepthType.Cv32F);
            translationVector.ConvertTo(translationVector32f, DepthType.Cv32F);

            Matrix<double> rotationMat = new Matrix<double>(3, 3);
            CvInvoke.Rodrigues(rotationVector32f, rotationMat);

            // Copy to transformation matrix
            for (int col = 0; col < 3; col++)
            {
                for (int row = 0; row < 3; row++)
                {
                    _pose3d.SetRotationMatrixValue(row, col, (float)rotationMat[row, col]); // Copy rotation component
                }
                _pose3d.SetTranslationVectorValue(col, translationVector32f[col]); // Copy translation component
            }

            // Since solvePnP finds camera location, w.r.t to marker pose, to get marker pose w.r.t to the camera we invert it.
            _pose3d = _pose3d.GetInverted();
        }
    }
}
