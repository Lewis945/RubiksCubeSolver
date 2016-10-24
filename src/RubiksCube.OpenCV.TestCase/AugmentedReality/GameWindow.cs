using Emgu.CV;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubiksCube.OpenCV.TestCase.AugmentedReality
{
    /// <summary>
    /// https://github.com/amulware/genericgamedev-opentk-intro/tree/master/src
    /// </summary>
    public sealed class GameWindow : OpenTK.GameWindow
    {
        public Capture Capture { get; set; }
        public PatternDetector PatternDetector { get; set; }
        public Mat Pattern { get; set; }

        //public bool _isPatternPresent;
        //public Transformation _patternPose;

        private bool _isTextureInitialized;
        private uint _backgroundTextureId;
        private CameraCalibrationInfo _calibration;
        private Mat _backgroundImage = new Mat();

        private bool _render;
        private ConcurrentQueue<ProcessedFrame> _captureBuffer;

        public GameWindow(CameraCalibrationInfo calibration, Mat img)
            // set window resolution, title, and default behaviour
            : base(img.Width, img.Height, GraphicsMode.Default, "OpenTK Intro",
            GameWindowFlags.Default, DisplayDevice.Default,
            // ask for an OpenGL 3.0 forward compatible context
            3, 0, GraphicsContextFlags.ForwardCompatible)
        {
            _calibration = calibration;
            _backgroundImage = img;

            // init capture buffer
            _captureBuffer = new ConcurrentQueue<ProcessedFrame>();

            Task.Run(() =>
            {
                int counter = 0;
                Transformation lastPose = new Transformation();
                while (true)
                {
                    if (Capture != null)
                    {
                        var frame = Capture.QueryFrame();
                        if (frame == null) break;
                        if (counter % 8 == 0)
                        {
                            var processedFrame = ProcessFrame(frame);
                            lastPose = processedFrame.PatternPose;
                            _captureBuffer.Enqueue(processedFrame);
                        }
                        else
                        {
                            _captureBuffer.Enqueue(new ProcessedFrame { PatternPose = lastPose, IsPatternPresent = true, Image = frame });
                        }

                        counter++;

                        if (_captureBuffer.Count > 50)
                        {
                            _render = true;
                        }
                    }
                }
            });

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

        private bool _isInit = false;
        private ProcessedFrame _currentProcessedFrame;

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            Title = "GameWindowSimple (Vsync: " + VSync.ToString() + ") " + "  FPS: " + (1f / e.Time).ToString("0.");

            // this is called every frame, put game logic here
            if (Capture != null)
            {
                if (_render)
                {
                    if (_captureBuffer.TryDequeue(out _currentProcessedFrame))
                        _backgroundImage = _currentProcessedFrame.Image;
                }
            }
            else
            {
                if (!_isInit)
                {
                    _currentProcessedFrame = ProcessFrame(_backgroundImage);
                    _isInit = true;
                }
            }
        }

        struct ProcessedFrame
        {
            public bool IsPatternPresent { get; set; }
            public Transformation PatternPose { get; set; }
            public Mat Image { get; set; }
        }

        private ProcessedFrame ProcessFrame(Mat frame)
        {
            var img = frame.Clone();

            PatternDetector.FindPattern(img);
            PatternDetector.PatternTrackingInfo.ComputePose(PatternDetector.Pattern, _calibration);

            var isPatternPresent = true;
            var patternPose = PatternDetector.PatternTrackingInfo.Pose3d;

            return new ProcessedFrame { IsPatternPresent = isPatternPresent, PatternPose = patternPose, Image = img };
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.ClearColor(Color4.Beige);
            GL.ClearDepth(1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Render background
            DrawCameraFrame(_backgroundImage);
            DrawAugmentedScene(_currentProcessedFrame, _backgroundImage);

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
            var proj = new float[] { 0, -2.0f / w, 0, 0, -2.0f / h, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1 };

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
            Matrix4 projectionMatrix;
            int w = image.Cols;
            int h = image.Rows;
            projectionMatrix = BuildProjectionMatrix(_calibration, w, h);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projectionMatrix);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            if (processedFrame.IsPatternPresent)
            {
                // Set the pattern transformation
                Matrix4 glMatrix = processedFrame.PatternPose.getMat44();
                GL.LoadMatrix(ref glMatrix);

                // Render model
                DrawCoordinateAxis();
                DrawCubeModel();
            }
        }

        private Matrix4 BuildProjectionMatrix(CameraCalibrationInfo calibration, int screen_width, int screen_height)
        {
            float nearPlane = 0.01f;  // Near clipping distance
            float farPlane = 100.0f;  // Far clipping distance

            // Camera parameters
            float f_x = calibration.Fx; // Focal length in x axis
            float f_y = calibration.Fy; // Focal length in y axis (usually the same?)
            float c_x = calibration.Cx; // Camera primary point x
            float c_y = calibration.Cy; // Camera primary point y

            var projectionMatrix = new Matrix4(
                -2.0f * f_x / screen_width,
                0.0f,
                0.0f,
                0.0f,
                //----------
                0.0f,
                2.0f * f_y / screen_height,
                0.0f,
                0.0f,
                //----------
                2.0f * c_x / screen_width - 1.0f,
                2.0f * c_y / screen_height - 1.0f,
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

        private void DrawCoordinateAxis()
        {
            float[] lineX = new float[] { 0, 0, 0, 1, 0, 0 };
            float[] lineY = new float[] { 0, 0, 0, 0, 1, 0 };
            float[] lineZ = new float[] { 0, 0, 0, 0, 0, 1 };

            GL.LineWidth(2);

            GL.Begin(PrimitiveType.Lines);

            GL.Color3(1f, 0f, 0f);
            GL.Vertex3(lineX);
            GL.Vertex3(1f, 0f, 0f);

            GL.Color3(0f, 1f, 0f);
            GL.Vertex3(lineY);
            GL.Vertex3(0f, 1f, 0f);

            GL.Color3(0f, 0f, 1f);
            GL.Vertex3(lineZ);
            GL.Vertex3(0f, 0f, 1f);

            GL.End();
        }

        private void DrawCubeModel()
        {
            var LightAmbient = new float[] { 0.25f, 0.25f, 0.25f, 1.0f };    // Ambient Light Values
            var LightDiffuse = new float[] { 0.1f, 0.1f, 0.1f, 1.0f };    // Diffuse Light Values
            var LightPosition = new float[] { 0.0f, 0.0f, 2.0f, 1.0f };    // Light Position

            GL.PushAttrib(AttribMask.ColorBufferBit | AttribMask.CurrentBit | AttribMask.EnableBit | AttribMask.LightingBit | AttribMask.PolygonBit);

            GL.Color4(0.2f, 0.35f, 0.3f, 0.75f);// Full Brightness, 50% Alpha ( NEW )
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);// Blending Function For Translucency Based On Source Alpha 
            GL.Enable(EnableCap.Blend);

            GL.ShadeModel(ShadingModel.Smooth);

            GL.Enable(EnableCap.Lighting);
            GL.Disable(EnableCap.Light0);
            GL.Enable(EnableCap.Light1);
            GL.Light(LightName.Light1, LightParameter.Ambient, LightAmbient);
            GL.Light(LightName.Light1, LightParameter.Diffuse, LightDiffuse);
            GL.Light(LightName.Light1, LightParameter.Position, LightPosition);
            GL.Enable(EnableCap.ColorMaterial);

            GL.Scale(0.25f, 0.25f, 0.25f);
            GL.Translate(0, 0, 1);

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Begin(PrimitiveType.Quads);

            // Front Face
            GL.Normal3(0.0f, 0.0f, 1.0f);    // Normal Pointing Towards Viewer
            GL.Vertex3(-1.0f, -1.0f, 1.0f);  // Point 1 (Front)
            GL.Vertex3(1.0f, -1.0f, 1.0f);  // Point 2 (Front)
            GL.Vertex3(1.0f, 1.0f, 1.0f);  // Point 3 (Front)
            GL.Vertex3(-1.0f, 1.0f, 1.0f);  // Point 4 (Front)

            // Back Face
            GL.Normal3(0.0f, 0.0f, -1.0f);    // Normal Pointing Away From Viewer
            GL.Vertex3(-1.0f, -1.0f, -1.0f);  // Point 1 (Back)
            GL.Vertex3(-1.0f, 1.0f, -1.0f);  // Point 2 (Back)
            GL.Vertex3(1.0f, 1.0f, -1.0f);  // Point 3 (Back)
            GL.Vertex3(1.0f, -1.0f, -1.0f);  // Point 4 (Back)

            // Top Face
            GL.Normal3(0.0f, 1.0f, 0.0f);    // Normal Pointing Up
            GL.Vertex3(-1.0f, 1.0f, -1.0f);  // Point 1 (Top)
            GL.Vertex3(-1.0f, 1.0f, 1.0f);  // Point 2 (Top)
            GL.Vertex3(1.0f, 1.0f, 1.0f);  // Point 3 (Top)
            GL.Vertex3(1.0f, 1.0f, -1.0f);  // Point 4 (Top)

            // Bottom Face
            GL.Normal3(0.0f, -1.0f, 0.0f);    // Normal Pointing Down
            GL.Vertex3(-1.0f, -1.0f, -1.0f);  // Point 1 (Bottom)
            GL.Vertex3(1.0f, -1.0f, -1.0f);  // Point 2 (Bottom)
            GL.Vertex3(1.0f, -1.0f, 1.0f);  // Point 3 (Bottom)
            GL.Vertex3(-1.0f, -1.0f, 1.0f);  // Point 4 (Bottom)

            // Right face
            GL.Normal3(1.0f, 0.0f, 0.0f);    // Normal Pointing Right
            GL.Vertex3(1.0f, -1.0f, -1.0f);  // Point 1 (Right)
            GL.Vertex3(1.0f, 1.0f, -1.0f);  // Point 2 (Right)
            GL.Vertex3(1.0f, 1.0f, 1.0f);  // Point 3 (Right)
            GL.Vertex3(1.0f, -1.0f, 1.0f);  // Point 4 (Right)

            // Left Face
            GL.Normal3(-1.0f, 0.0f, 0.0f);    // Normal Pointing Left
            GL.Vertex3(-1.0f, -1.0f, -1.0f);  // Point 1 (Left)
            GL.Vertex3(-1.0f, -1.0f, 1.0f);  // Point 2 (Left)
            GL.Vertex3(-1.0f, 1.0f, 1.0f);  // Point 3 (Left)
            GL.Vertex3(-1.0f, 1.0f, -1.0f);  // Point 4 (Left)

            GL.End();

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.Color4(0.2f, 0.65f, 0.3f, 0.35f);// Full Brightness, 50% Alpha ( NEW )
            GL.Begin(PrimitiveType.Quads);

            // Front Face
            GL.Normal3(0.0f, 0.0f, 1.0f);    // Normal Pointing Towards Viewer
            GL.Vertex3(-1.0f, -1.0f, 1.0f);  // Point 1 (Front)
            GL.Vertex3(1.0f, -1.0f, 1.0f);  // Point 2 (Front)
            GL.Vertex3(1.0f, 1.0f, 1.0f);  // Point 3 (Front)
            GL.Vertex3(-1.0f, 1.0f, 1.0f);  // Point 4 (Front)

            // Back Face
            GL.Normal3(0.0f, 0.0f, -1.0f);    // Normal Pointing Away From Viewer
            GL.Vertex3(-1.0f, -1.0f, -1.0f);  // Point 1 (Back)
            GL.Vertex3(-1.0f, 1.0f, -1.0f);  // Point 2 (Back)
            GL.Vertex3(1.0f, 1.0f, -1.0f);  // Point 3 (Back)
            GL.Vertex3(1.0f, -1.0f, -1.0f);  // Point 4 (Back)

            // Top Face
            GL.Normal3(0.0f, 1.0f, 0.0f);    // Normal Pointing Up
            GL.Vertex3(-1.0f, 1.0f, -1.0f);  // Point 1 (Top)
            GL.Vertex3(-1.0f, 1.0f, 1.0f);  // Point 2 (Top)
            GL.Vertex3(1.0f, 1.0f, 1.0f);  // Point 3 (Top)
            GL.Vertex3(1.0f, 1.0f, -1.0f);  // Point 4 (Top)

            // Bottom Face
            GL.Normal3(0.0f, -1.0f, 0.0f);    // Normal Pointing Down
            GL.Vertex3(-1.0f, -1.0f, -1.0f);  // Point 1 (Bottom)
            GL.Vertex3(1.0f, -1.0f, -1.0f);  // Point 2 (Bottom)
            GL.Vertex3(1.0f, -1.0f, 1.0f);  // Point 3 (Bottom)
            GL.Vertex3(-1.0f, -1.0f, 1.0f);  // Point 4 (Bottom)

            // Right face
            GL.Normal3(1.0f, 0.0f, 0.0f);    // Normal Pointing Right
            GL.Vertex3(1.0f, -1.0f, -1.0f);  // Point 1 (Right)
            GL.Vertex3(1.0f, 1.0f, -1.0f);  // Point 2 (Right)
            GL.Vertex3(1.0f, 1.0f, 1.0f);  // Point 3 (Right)
            GL.Vertex3(1.0f, -1.0f, 1.0f);  // Point 4 (Right)

            // Left Face
            GL.Normal3(-1.0f, 0.0f, 0.0f);    // Normal Pointing Left
            GL.Vertex3(-1.0f, -1.0f, -1.0f);  // Point 1 (Left)
            GL.Vertex3(-1.0f, -1.0f, 1.0f);  // Point 2 (Left)
            GL.Vertex3(-1.0f, 1.0f, 1.0f);  // Point 3 (Left)
            GL.Vertex3(-1.0f, 1.0f, -1.0f);  // Point 4 (Left)

            GL.End();

            GL.PopAttrib();
        }
    }
}
