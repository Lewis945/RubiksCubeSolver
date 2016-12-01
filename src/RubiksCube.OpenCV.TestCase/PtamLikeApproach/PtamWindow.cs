using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Accord.Math;
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

        public List<PlaneInfo> Planes { get; }

        public PtamWindow(CameraCalibrationInfo calibration, Mat img)
            : base(img.Width, img.Height, GraphicsMode.Default, "PTAM",
            GameWindowFlags.Default, DisplayDevice.Default,
            3, 0, GraphicsContextFlags.ForwardCompatible)
        {
            _calibration = calibration;
            _backgroundImage = img;

            _newMap = true;

            Planes = new List<PlaneInfo>();

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
                //_backgroundImage = ProcessFrame(_backgroundImage);
            }
        }

        private bool _new = true;
        private int _i = 0;
        private Mat ProcessFrame(Mat frame)
        {
            var img = frame.Clone();
            CvInvoke.PutText(img, _i.ToString(), new Point(30, 30), FontFace.HersheyPlain, 1, new MCvScalar(255, 0, 0), 2);
            _i++;

            try
            {
                Algorithm.Process(img, _newMap, (normal, points, homography) =>
            {
                Planes.Add(new PlaneInfo
                {
                    Normal = normal,
                    Points3D = points
                });

                Capture = null;

                #region Rectangle

                //var rect = CvInvoke.BoundingRectangle(Utils.GetPointsVector(Algorithm.TrackedFeatures));
                //var rectPoints = new[]
                //{
                //    new PointF(rect.Location.X, rect.Location.Y + rect.Y),
                //    rect.Location,
                //    new PointF(rect.Location.X + rect.X, rect.Location.Y),
                //    new PointF(rect.Location.X + rect.X, rect.Location.Y + rect.Y)
                //};
                //rectPoints = CvInvoke.PerspectiveTransform(rectPoints, homography);
                //var points2D = Array.ConvertAll(rectPoints, Point.Round);

                ////CvInvoke.CvtColor(img, img, ColorConversion.Bgr2Gray);
                //var edges = new Mat();
                //CvInvoke.Canny(img, edges, 0.1, 99);

                //var contours = new VectorOfVectorOfPoint();
                //var hierarchy = new Mat();

                //CvInvoke.FindContours(edges, contours, hierarchy, RetrType.Tree, ChainApproxMethod.ChainApproxSimple);

                //int largestContourIndex = 0;
                //double largestArea = 0;
                //VectorOfPoint largestContour;

                //for (int i = 0; i < contours.Size; i++)
                //{
                //    var approx = new Mat();
                //    CvInvoke.ApproxPolyDP(contours[i], approx, 9, true);

                //    var approxMat = new Matrix<double>(approx.Rows, approx.Cols, approx.DataPointer);

                //    double a = Math.Abs(CvInvoke.ContourArea(approx));
                //    if (approxMat.Rows == 4 && Math.Abs(CvInvoke.ContourArea(approx)) > 500 &&
                //            CvInvoke.IsContourConvex(approx))
                //    {
                //        CvInvoke.DrawContours(img, contours, i, new MCvScalar(0, 255, 0), 2);
                //    }
                //}

                //while (true)
                //{
                //    rect.Inflate(20, 20);
                //    if (rect.Size.Width > 200 || rect.Height > 200)
                //        break;
                //}

                #endregion

                #region MyRegion

                var rotationVector32F = new VectorOfFloat();
                var translationVector32F = new VectorOfFloat();
                var rotationVector = new Mat();
                var translationVector = new Mat();

                CvInvoke.SolvePnP(Algorithm.TrackedFeatures3D, Utils.GetPointsVector(Algorithm.TrackedFeatures), _calibration.Intrinsic, _calibration.Distortion, rotationVector, translationVector);

                rotationVector.ConvertTo(rotationVector32F, DepthType.Cv32F);
                translationVector.ConvertTo(translationVector32F, DepthType.Cv32F);

                var rotationMat = new Mat();
                CvInvoke.Rodrigues(rotationVector32F, rotationMat);
                var rotationMatrix = new Matrix<double>(rotationMat.Rows, rotationMat.Cols, rotationMat.DataPointer);

                var tvec = translationVector32F.ToArray().Select(i => (double)i).ToArray();

                var cameraPosition = rotationMatrix.Transpose() * new Matrix<double>(tvec);
                var cameraPositionPoint = new MCvPoint3D32f((float)cameraPosition[0, 0], (float)cameraPosition[1, 0], (float)cameraPosition[2, 0]);

                var cameraVector = Algorithm.TrackedFeatures3D[0] - cameraPositionPoint;

                Func<double, double> radianToDegree = angle => angle * (180.0 / Math.PI);

                double dotProduct = new double[] { cameraVector.X, cameraVector.Y, cameraVector.Z }.Dot(new[] { normal[0, 0], normal[0, 1], normal[0, 2] });
                double acos = Math.Acos(dotProduct);
                double angle5 = radianToDegree(acos);

                double t = dotProduct;

                //var projected = CvInvoke.ProjectPoints(Algorithm.TrackedFeatures3D.ToArray(), rotationVector32F, translationVector32F, _calibration.Intrinsic, _calibration.Distortion);

                //foreach (var pr in projected)
                //{
                //    CvInvoke.Circle(img, new Point((int)pr.X, (int)pr.Y), 2, new MCvScalar(0, 255, 0), 2, LineType.AntiAlias);
                //}

                #endregion

                //CvInvoke.Rectangle(img, rect, new MCvScalar(0, 0, 255), 3, LineType.AntiAlias);
                //CvInvoke.Polylines(img, points2D, true, new MCvScalar(0, 255, 0), 3, LineType.AntiAlias);

                Console.WriteLine($"Normal: [{normal.Data[0, 0]}, {normal.Data[0, 1]}, {normal.Data[0, 2]}]");
                Console.WriteLine($"Angle: {angle5}");
                Console.WriteLine($"Dot product: {dotProduct}");
                //Console.WriteLine($"Points: [{string.Join(",", points.ToArray().Select(p => $"[{p.X}, {p.Y}, {p.Z}]").ToArray())}]");
            });
            }
            catch (Exception ex)
            {
                _newMap = true;
                Algorithm.ResetAlgorithm();
            }

            if (Algorithm.IsBootstrapping)
            {
                _newMap = false;
                return img;
            }
            if (Algorithm.IsBootstrapping || !Algorithm.IsTracking)
            {
                _newMap = true;
                return img;
            }

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

            //Thread.Sleep(1000);
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