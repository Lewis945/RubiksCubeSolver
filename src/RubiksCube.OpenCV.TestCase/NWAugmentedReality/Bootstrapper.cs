using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace RubiksCube.OpenCV.TestCase.NWAugmentedReality
{
    public static class Bootstrapper
    {
        public static void Run(string path, string patternPath, SourceType type)
        {
            //using (var a = new GameWindow())
            //{
            //    a.Run(30);
            //}

            var viewer = new ImageViewer();
            viewer.Text = path;

            var calibration = new CameraCalibrationInfo(560.764656335266f, 562.763179958161f, 295.849138757436f, 255.022208986073f);

            var patternImage = CvInvoke.Imread(patternPath, Emgu.CV.CvEnum.LoadImageType.Unchanged);

            var pipeline = new ARPipeline(patternImage, calibration);
            //var drawingCtx = new ARDrawingContext("Markerless AR", frameSize, calibration);

            if (type == SourceType.Image)
            {
                ProcessImage(path, patternImage, viewer, pipeline);
            }
            else if (type == SourceType.Video)
            {
                ProcessVideo(path, patternImage, viewer, pipeline);
            }

            viewer.ShowDialog();
        }

        private static void ProcessImage(string path, Mat patternImage, ImageViewer viewer, ARPipeline pipeline)
        {
            var img = CvInvoke.Imread(path, Emgu.CV.CvEnum.LoadImageType.Unchanged);
            int sleepTime = (int)Math.Round(1000 / 6f);

            Task.Run(() =>
            {
                bool run = true;
                while (run)
                {
                    if (img != null)
                        viewer.Image = ProcessFrame(img, patternImage, pipeline);
                    else
                        run = false;

                    Thread.Sleep(sleepTime);
                }
            });
        }

        private static void ProcessVideo(string path, Mat patternImage, ImageViewer viewer, ARPipeline pipeline)
        {
            var capture = new Capture(path);
            int sleepTime = (int)Math.Round(1000 / 6f);

            Task.Run(() =>
            {
                bool run = true;
                while (run)
                {
                    var frame = capture.QueryFrame();
                    if (frame != null)
                        viewer.Image = ProcessFrame(frame, patternImage, pipeline);
                    else
                        capture = new Capture(path);

                    Thread.Sleep(sleepTime);
                }
            });
        }

        private static Mat ProcessFrame(Mat frame, Mat pattern, ARPipeline pipeline)
        {
            //long time;
            //frame = DrawMatches.Draw(pattern, frame, out time);

            var img = frame.Clone();

            // Draw information:
            if (pipeline.m_patternDetector.enableHomographyRefinement)
                CvInvoke.PutText(img, "Pose refinement: On   ('h' to switch off)", new Point(10, 15), FontFace.HersheyPlain, 1, new MCvScalar(0, 200, 0));
            else
                CvInvoke.PutText(img, "Pose refinement: Off  ('h' to switch on)", new Point(10, 15), FontFace.HersheyPlain, 1, new MCvScalar(0, 200, 0));

            CvInvoke.PutText(img, "RANSAC threshold: "+ pipeline.m_patternDetector.homographyReprojectionThreshold + "( Use'-'/'+' to adjust)", new Point(10, 30), FontFace.HersheyPlain, 1, new MCvScalar(0, 200, 0));

            //// Set a new camera frame:
            //drawingCtx.updateBackground(img);

            //// Find a pattern and update it's detection status:
            //drawingCtx.isPatternPresent = pipeline.processFrame(cameraFrame);

            //// Update a pattern pose:
            //drawingCtx.patternPose = pipeline.getPatternLocation();

            //// Request redraw of the window:
            //drawingCtx.updateWindow();

            //if (keyCode == '+' || keyCode == '=')
            //{
            //    pipeline.m_patternDetector.homographyReprojectionThreshold += 0.2f;
            //    pipeline.m_patternDetector.homographyReprojectionThreshold = std::min(10.0f, pipeline.m_patternDetector.homographyReprojectionThreshold);
            //}
            //else if (keyCode == '-')
            //{
            //    pipeline.m_patternDetector.homographyReprojectionThreshold -= 0.2f;
            //    pipeline.m_patternDetector.homographyReprojectionThreshold = std::max(0.0f, pipeline.m_patternDetector.homographyReprojectionThreshold);
            //}
            //else if (keyCode == 'h')
            //{
            //    pipeline.m_patternDetector.enableHomographyRefinement = !pipeline.m_patternDetector.enableHomographyRefinement;
            //}

            return img;
        }
    }
}
