using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using RubiksCube.OpenCV.TestCase.AugmentedReality;

namespace RubiksCube.OpenCV.TestCase.PtamLikeApproach
{
    public class TrackingInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public TrackingInfo()
        {
        }

        /// <summary>
        /// Compute pattern pose using PnP algorithm
        /// </summary>
        /// <param name="points"></param>
        /// <param name="calibration"></param>
        /// <param name="points3D"></param>
        /// <param name="raux"></param>
        /// <param name="taux"></param>
        public Transformation ComputePose(VectorOfPoint3D32F points3D, VectorOfPointF points, CameraCalibrationInfo calibration, out VectorOfFloat raux, out VectorOfFloat taux)
        {
            var pose3D = new Transformation();

            var rotationVector32F = new VectorOfFloat();
            var translationVector32F = new VectorOfFloat();
            var rotationVector = new Mat();
            var translationVector = new Mat();

            CvInvoke.SolvePnP(points3D, points, calibration.Intrinsic, calibration.Distortion, rotationVector, translationVector);

            rotationVector.ConvertTo(rotationVector32F, DepthType.Cv32F);
            translationVector.ConvertTo(translationVector32F, DepthType.Cv32F);

            raux = rotationVector32F;
            taux = translationVector32F;

            var rotationMat = new Mat();
            CvInvoke.Rodrigues(rotationVector32F, rotationMat);
            var rotationMatrix = new Matrix<double>(rotationMat.Rows, rotationMat.Cols, rotationMat.DataPointer);

            // Copy to transformation matrix
            for (int col = 0; col < 3; col++)
            {
                for (int row = 0; row < 3; row++)
                {
                    pose3D.SetRotationMatrixValue(row, col, (float)rotationMatrix[row, col]); // Copy rotation component
                }
                pose3D.SetTranslationVectorValue(col, translationVector32F[col]); // Copy translation component
            }

            // Since solvePnP finds camera location, w.r.t to marker pose, to get marker pose w.r.t to the camera we invert it.
            return pose3D.GetInverted();
        }
    }
}
