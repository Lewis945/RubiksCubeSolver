using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubiksCube.OpenCV.TestCase.AugmentedReality
{
    public class ARPipeline
    {

        private CameraCalibrationInfo m_calibration;
        private Pattern m_pattern;
        private PatternTrackingInfo m_patternInfo;

        public PatternDetector m_patternDetector;

        public ARPipeline(Mat patternImage, CameraCalibrationInfo calibration)
        {
            m_calibration = calibration;
            m_patternDetector.buildPatternFromImage(patternImage, m_pattern);
            m_patternDetector.train(m_pattern);
        }


        public bool processFrame(Mat inputFrame)
        {
            bool patternFound = m_patternDetector.findPattern(inputFrame, m_patternInfo);

            if (patternFound)
            {
                m_patternInfo.computePose(m_pattern, m_calibration);
            }

            return patternFound;
        }

        public Transformation getPatternLocation()
        {
            return m_patternInfo.pose3d;
        }
    }
}
