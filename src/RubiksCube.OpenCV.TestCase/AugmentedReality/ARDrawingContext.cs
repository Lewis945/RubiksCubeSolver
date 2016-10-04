using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubiksCube.OpenCV.TestCase.AugmentedReality
{
    public class ARDrawingContext
    {
        #region Fields

        private bool m_isTextureInitialized;
        private uint m_backgroundTextureId;
        private CameraCalibrationInfo m_calibration;
        private Mat m_backgroundImage;
        private string m_windowName;

        #endregion

        #region Properties

        public bool isPatternPresent;
        public Transformation patternPose;

        #endregion

        public ARDrawingContext(string windowName, Size frameSize, CameraCalibrationInfo c)
        {
            m_isTextureInitialized = false;
            m_calibration = c;
            m_windowName = windowName;

            // Create window with OpenGL support
            CvInvoke.NamedWindow(windowName, Emgu.CV.CvEnum.NamedWindowType.Opengl);

            // Resize it exactly to video size
            //CvInvoke.ResizeWindow(windowName, frameSize.width, frameSize.height);
        }
    }
}
