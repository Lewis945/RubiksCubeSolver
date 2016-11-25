using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
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

        private bool _newMap;

        public PtamWindow(CameraCalibrationInfo calibration, Mat img)
            : base(img.Width, img.Height, GraphicsMode.Default, "PTAM",
            GameWindowFlags.Default, DisplayDevice.Default,
            3, 0, GraphicsContextFlags.ForwardCompatible)
        {
            _calibration = calibration;
            _backgroundImage = img;

            _newMap = true;

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
            Title = $"PTAM (Vsync: {VSync} FPS: {1f / e.Time:0.}";

            if (Capture != null)
            {
                var image = Capture.QueryFrame();
                _backgroundImage = ProcessFrame(image);
            }
            else
            {
                _backgroundImage = ProcessFrame(_backgroundImage);
            }
        }

        private Mat ProcessFrame(Mat frame)
        {
            var img = frame.Clone();

            Algorithm.Process(img, _newMap);

            if (Algorithm.IsBootstrapping)
            {
                _newMap = false;
                return img;
            }
            if (Algorithm.IsBootstrapping || !Algorithm.IsTracking)
                return img;

            //Thread.Sleep(1000);

            #region Draw axis

            //var axis = new VectorOfPoint3D32F(new[]
            //{
            //    new MCvPoint3D32f(0.01f, 0, 0),
            //    new MCvPoint3D32f(0, 0.01f, 0),
            //    new MCvPoint3D32f(0, 0, 0.01f)
            //});

            //var imgPoints = new VectorOfPointF();
            //CvInvoke.ProjectPoints(axis, Algorithm.Raux, Algorithm.Taux, _calibration.Intrinsic, _calibration.Distortion, imgPoints);

            //var centers = new VectorOfPoint3D32F(new[] { new MCvPoint3D32f(0, 0, 0) });
            //var centerPoints = new VectorOfPointF();
            //CvInvoke.ProjectPoints(centers, Algorithm.Raux, Algorithm.Taux, _calibration.Intrinsic, _calibration.Distortion,
            //    centerPoints);

            //CvInvoke.Line(img, new Point((int)centerPoints[0].X, (int)centerPoints[0].Y), new Point((int)imgPoints[0].X, (int)imgPoints[0].Y), new MCvScalar(255, 0, 0), 5);
            //CvInvoke.Line(img, new Point((int)centerPoints[0].X, (int)centerPoints[0].Y), new Point((int)imgPoints[1].X, (int)imgPoints[1].Y), new MCvScalar(0, 255, 0), 5);
            //CvInvoke.Line(img, new Point((int)centerPoints[0].X, (int)centerPoints[0].Y), new Point((int)imgPoints[2].X, (int)imgPoints[2].Y), new MCvScalar(0, 0, 255), 5);

            #endregion

            #region Draw keypoints and projected 3D points

            var projected3DfeaturesrPoints = new VectorOfPointF();
            CvInvoke.ProjectPoints(Algorithm.TrackedFeatures3D, Algorithm.Raux, Algorithm.Taux, _calibration.Intrinsic,
                _calibration.Distortion, projected3DfeaturesrPoints);
            for (int i = 0; i < projected3DfeaturesrPoints.Size; i++)
            {
                var feature = projected3DfeaturesrPoints[i];
                CvInvoke.Circle(img, new Point((int)feature.X, (int)feature.Y), 2,
                    new MCvScalar(0, 255, 0), 2);
            }

            for (int i = 0; i < Algorithm.TrackedFeatures.Size; i++)
            {
                var feature = Algorithm.TrackedFeatures[i];
                CvInvoke.Circle(img, new Point((int)feature.Point.X, (int)feature.Point.Y), 1,
                    new MCvScalar(0, 0, 255), 1);
            }

            #endregion

            _newMap = true;

            return img;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.ClearColor(Color4.Beige);
            GL.ClearDepth(1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Render background
            DrawCameraFrame(_backgroundImage);

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
    }
}