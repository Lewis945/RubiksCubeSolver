using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using RubiksCube.OpenCV.TestCase.AugmentedReality;

namespace RubiksCube.OpenCV.TestCase.PtamLikeApproach
{
    public sealed class PtamWindow : OpenTK.GameWindow
    {
        public Capture Capture { get; set; }
        public PtamLikeAlgorithm Algorithm { get; set; }

        private bool _isTextureInitialized;
        private uint _backgroundTextureId;
        private readonly CameraCalibrationInfo _calibration;
        private Mat _backgroundImage;

        private ProcessedFrame _currentProcessedFrame;

        public PtamWindow(CameraCalibrationInfo calibration, Mat img)
            // set window resolution, title, and default behaviour
            : base(img.Width, img.Height, GraphicsMode.Default, "PTAM",
            GameWindowFlags.Default, DisplayDevice.Default,
            // ask for an OpenGL 3.0 forward compatible context
            3, 0, GraphicsContextFlags.ForwardCompatible)
        {
            _calibration = calibration;
            _backgroundImage = img;

            Console.WriteLine("gl version: " + GL.GetString(StringName.Version));
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
        }

        protected override void OnLoad(EventArgs e)
        {
            // this is called when the window starts running
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            Title = "PTAM (Vsync: " + VSync + ") " + "  FPS: " + (1f / e.Time).ToString("0.");

            if (Capture != null)
            {
                var image = Capture.QueryFrame();
                var result = Algorithm.BootstrapTrack(image);
                if (result)
                {
                    _currentProcessedFrame = ProcessFrame(image);
                    _backgroundImage = _currentProcessedFrame.Image;
                    Capture = null;
                }
                else
                {
                    _currentProcessedFrame = new ProcessedFrame { Image = image.Clone() };
                    _backgroundImage = _currentProcessedFrame.Image;
                }
            }
            else
            {
                _currentProcessedFrame = ProcessFrame(_backgroundImage);
            }
        }

        private struct ProcessedFrame
        {
            public bool IsPatternPresent { get; set; }
            public Transformation PatternPose { get; set; }
            public Mat Image { get; set; }
        }

        private ProcessedFrame ProcessFrame(Mat frame)
        {
            var img = frame.Clone();

            var info = new TrackingInfo();

            var points3D = Algorithm.TrackedFeatures3D;
            var points2D = Utils.GetPointsVector(Algorithm.TrackedFeatures);

            VectorOfFloat raux;
            VectorOfFloat taux;

            var patternPose = info.ComputePose(points3D, points2D, _calibration, out raux, out taux);

            var axis = new VectorOfPoint3D32F(new[]
            {
                new MCvPoint3D32f(-3, 3, -1),
                new MCvPoint3D32f(-2, 4, -1),
                new MCvPoint3D32f(-2, 3, -2)
            });

            var imgPoints = new VectorOfPointF();
            CvInvoke.ProjectPoints(axis, raux, taux, _calibration.Intrinsic, _calibration.Distortion, imgPoints);

            var centers = new VectorOfPoint3D32F(new[] { new MCvPoint3D32f(-2, 3, -1) });
            var centerPoints = new VectorOfPointF();
            CvInvoke.ProjectPoints(centers, raux, taux, _calibration.Intrinsic, _calibration.Distortion, centerPoints);

            CvInvoke.Line(_backgroundImage, new Point((int)centerPoints[0].X, (int)centerPoints[0].Y), new Point((int)imgPoints[0].X, (int)imgPoints[0].Y), new MCvScalar(255, 0, 0), 5);
            CvInvoke.Line(_backgroundImage, new Point((int)centerPoints[0].X, (int)centerPoints[0].Y), new Point((int)imgPoints[1].X, (int)imgPoints[1].Y), new MCvScalar(0, 255, 0), 5);
            CvInvoke.Line(_backgroundImage, new Point((int)centerPoints[0].X, (int)centerPoints[0].Y), new Point((int)imgPoints[2].X, (int)imgPoints[2].Y), new MCvScalar(0, 0, 255), 5);

            return new ProcessedFrame { IsPatternPresent = true, PatternPose = patternPose, Image = img };
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.ClearColor(Color4.Beige);
            GL.ClearDepth(1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Render background
            DrawCameraFrame(_backgroundImage);
            //DrawAugmentedScene(_currentProcessedFrame, _backgroundImage);

            SwapBuffers();
        }

        private void DrawCameraFrame(Mat image)
        {
            // Initialize texture for background image
            if (!_isTextureInitialized)
            {
                GL.GenTextures(1, out _backgroundTextureId);
                GL.BindTexture(TextureTarget.Texture2D, _backgroundTextureId);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                _isTextureInitialized = true;
            }

            int w = image.Cols;
            int h = image.Rows;

            GL.PixelStore(PixelStoreParameter.PackAlignment, 1);
            GL.BindTexture(TextureTarget.Texture2D, _backgroundTextureId);

            // Upload new texture data:
            if (image.NumberOfChannels == 3)
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, w, h, 0, PixelFormat.Bgr, PixelType.UnsignedByte, image.DataPointer);
            else if (image.NumberOfChannels == 4)
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, w, h, 0, PixelFormat.Bgra, PixelType.UnsignedByte, image.DataPointer);
            else if (image.NumberOfChannels == 1)
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, w, h, 0, PixelFormat.Luminance, PixelType.UnsignedByte, image.DataPointer);

            var bgTextureVertices = new float[] { 0, 0, w, 0, 0, h, w, h };
            var bgTextureCoords = new float[] { 1, 0, 1, 1, 0, 0, 0, 1 };
            var proj = new[] { 0, -2.0f / w, 0, 0, -2.0f / h, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1 };

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(proj);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, _backgroundTextureId);

            // Update attribute values.
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);

            GL.VertexPointer(2, VertexPointerType.Float, 0, bgTextureVertices);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, bgTextureCoords);

            GL.Color4(1f, 1f, 1f, 1f);
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);

            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.TextureCoordArray);
            GL.Disable(EnableCap.Texture2D);
        }

        private void DrawAugmentedScene(ProcessedFrame processedFrame, Mat image)
        {
            // Init augmentation projection
            int w = image.Cols;
            int h = image.Rows;
            var projectionMatrix = BuildProjectionMatrix(_calibration, w, h);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projectionMatrix);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            if (processedFrame.IsPatternPresent)
            {
                // Set the pattern transformation
                var glMatrix = processedFrame.PatternPose.GetMat44();
                GL.LoadMatrix(ref glMatrix);
            }
        }

        private static Matrix4 BuildProjectionMatrix(CameraCalibrationInfo calibration, int screenWidth, int screenHeight)
        {
            float nearPlane = 0.01f;  // Near clipping distance
            float farPlane = 100.0f;  // Far clipping distance

            // Camera parameters
            double fX = calibration.Fx; // Focal length in x axis
            double fY = calibration.Fy; // Focal length in y axis (usually the same?)
            double cX = calibration.Cx; // Camera primary point x
            double cY = calibration.Cy; // Camera primary point y

            var projectionMatrix = new Matrix4(
                (float)(-2.0 * fX / screenWidth),
                0.0f,
                0.0f,
                0.0f,
                //----------
                0.0f,
                (float)(2.0f * fY / screenHeight),
                0.0f,
                0.0f,
                //----------
                (float)(2.0f * cX / screenWidth - 1.0f),
                (float)(2.0f * cY / screenHeight - 1.0f),
                -(farPlane + nearPlane) / (farPlane - nearPlane),
                -1.0f,
                //----------
                0.0f,
                0.0f,
                -2.0f * farPlane * nearPlane / (farPlane - nearPlane),
                0.0f
            );

            return projectionMatrix;
        }
    }
}