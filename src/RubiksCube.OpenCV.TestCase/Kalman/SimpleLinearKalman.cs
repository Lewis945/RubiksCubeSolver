using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubiksCube.OpenCV.TestCase.Kalman
{
    class SimpleLinearKalman
    {
        /// <summary>
        /// State transition matrix.
        /// </summary>
        private Matrix<float> _A;

        /// <summary>
        /// Control matrix.
        /// </summary>
        private Matrix<float> _B;

        /// <summary>
        /// Observation matrix.
        /// </summary>
        private Matrix<float> _H;

        /// <summary>
        /// Estimated error in process.
        /// </summary>
        private Matrix<float> _Q;

        /// <summary>
        /// Estimated error in measurements.
        /// </summary>
        private Matrix<float> _R;

        /// <summary>
        /// Initial state estimate.
        /// </summary>
        private Matrix<float> _currentStateEstimate;

        /// <summary>
        /// Initial covariance estimate.
        /// </summary>
        private Matrix<float> _currentProbEstimate;

        public SimpleLinearKalman(Matrix<float> a, Matrix<float> b, Matrix<float> h, Matrix<float> q, Matrix<float> r, Matrix<float> currentStateEstimate, Matrix<float> currentProbEstimate)
        {
            _A = a;
            _B = b;
            _H = h;
            _Q = q;
            _R = r;
            _currentProbEstimate = currentProbEstimate;
            _currentStateEstimate = currentStateEstimate;
        }

        public Matrix<float> GetCurrentState()
        {
            return _currentStateEstimate;
        }

        public void Step(Matrix<float> controlVector, Matrix<float> measurementVector)
        {
            //---------------------------Prediction step-----------------------------

            var predictedStateEstimate = _A * _currentStateEstimate + _B * controlVector;
            var predictedProbEstimate = (_A * _currentProbEstimate) * _A.Transpose() + _Q;

            //--------------------------Observation step-----------------------------

            var innovation = measurementVector - _H * predictedStateEstimate;

            var innovationCovariance = _H * predictedProbEstimate * _H.Transpose() + _R;
            //-----------------------------Update step-------------------------------

            Matrix<float> innovationCovarianceInv = new Matrix<float>(innovationCovariance.Rows, innovationCovariance.Cols);
            CvInvoke.Invert(innovationCovariance, innovationCovarianceInv, Emgu.CV.CvEnum.DecompMethod.LU);

            var kalmanGain = predictedProbEstimate * _H.Transpose() * innovationCovarianceInv;
            _currentStateEstimate = predictedStateEstimate + kalmanGain * innovation;

            // We need the size of the matrix so we can make an identity matrix.

            var size = _currentProbEstimate.Size.Width;

            // eye(n) = nxn identity matrix.
            var eye = new Matrix<float>(size, size);
            _currentProbEstimate = (eye - kalmanGain * _H) * predictedProbEstimate;
        }
    }
}
