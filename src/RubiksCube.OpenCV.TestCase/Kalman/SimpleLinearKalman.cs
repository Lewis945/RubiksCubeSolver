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
        private Matrix<double> _A;

        /// <summary>
        /// Control matrix.
        /// </summary>
        private Matrix<double> _B;

        /// <summary>
        /// Observation matrix.
        /// </summary>
        private Matrix<double> _H;

        /// <summary>
        /// Estimated error in process.
        /// </summary>
        private Matrix<double> _Q;

        /// <summary>
        /// Estimated error in measurements.
        /// </summary>
        private Matrix<double> _R;

        /// <summary>
        /// Initial state estimate.
        /// </summary>
        private Matrix<double> _currentStateEstimate;

        /// <summary>
        /// Initial covariance estimate.
        /// </summary>
        private Matrix<double> _currentProbEstimate;

        public SimpleLinearKalman(Matrix<double> a, Matrix<double> b, Matrix<double> h, Matrix<double> q, Matrix<double> r, Matrix<double> currentStateEstimate, Matrix<double> currentProbEstimate)
        {
            _A = a;
            _B = b;
            _H = h;
            _Q = q;
            _R = r;
            _currentProbEstimate = currentProbEstimate;
            _currentStateEstimate = currentStateEstimate;
        }

        public Matrix<double> GetCurrentState()
        {
            return _currentStateEstimate;
        }

        public void Step(Matrix<double> controlVector, Matrix<double> measurementVector)
        {
            //---------------------------Prediction step-----------------------------

            var predictedStateEstimate = _A * _currentStateEstimate + _B * controlVector;
            var predictedProbEstimate = (_A * _currentProbEstimate) * _A.Transpose() + _Q;

            //--------------------------Observation step-----------------------------

            var innovation = measurementVector - _H * predictedStateEstimate;

            var innovationCovariance = _H * predictedProbEstimate * _H.Transpose() + _R;
            //-----------------------------Update step-------------------------------

            Matrix<double> innovationCovarianceInv = new Matrix<double>(innovationCovariance.Rows, innovationCovariance.Cols);
            CvInvoke.Invert(innovationCovariance, innovationCovarianceInv, Emgu.CV.CvEnum.DecompMethod.LU);

            var kalmanGain = predictedProbEstimate * _H.Transpose() * innovationCovarianceInv;
            _currentStateEstimate = predictedStateEstimate + kalmanGain * innovation;

            // We need the size of the matrix so we can make an identity matrix.

            var size = _currentProbEstimate.Size.Width;

            // eye(n) = nxn identity matrix.
            var eye = new Matrix<double>(size, size);
            _currentProbEstimate = (eye - kalmanGain * _H) * predictedProbEstimate;
        }
    }
}
