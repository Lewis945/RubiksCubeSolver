using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;
using RubiksCube.OpenCV.TestCase.NWAugmentedReality;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace RubiksCube.OpenCV.TestCase.NWAugmentedReality
{
    public class PatternTrackingInfo
    {
        public Mat homography;
        public VectorOfPoint points2d;
        public Transformation pose3d;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="color"></param>
        public void draw2dContour(Mat image, MCvScalar color)
        {
            for (int i = 0; i < points2d.Size; i++)
            {
                CvInvoke.Line(image, points2d[i], points2d[(i + 1) % points2d.Size], color, 2, Emgu.CV.CvEnum.LineType.AntiAlias);
            }
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

            CvInvoke.SolvePnP(pattern.points3d, points2d, calibration.getIntrinsic(), calibration.getDistorsion(), rotationVector, translationVector);
            rotationVector.ConvertTo(rotationVector32f, DepthType.Cv32F);
            translationVector.ConvertTo(translationVector32f, DepthType.Cv32F);

            Matrix<float> rotationMat = new Matrix<float>(3, 3);
            CvInvoke.Rodrigues(rotationVector32f, rotationMat);

            // Copy to transformation matrix
            for (int col = 0; col < 3; col++)
            {
                for (int row = 0; row < 3; row++)
                {
                    var r = pose3d.r;
                    r[row,col] = rotationMat[row, col]; // Copy rotation component
                }
                var t = pose3d.t;
                t[col] = translationVector32f[col]; // Copy translation component
            }

            // Since solvePnP finds camera location, w.r.t to marker pose, to get marker pose w.r.t to the camera we invert it.
            pose3d = pose3d.getInverted();
        }
    }
}
