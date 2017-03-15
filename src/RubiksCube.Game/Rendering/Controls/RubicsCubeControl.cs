using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using RubiksCube.Game.GraphicsEngine;
using RubiksCube.Game.Rendering;
using ScrarchEngine.Libraries.RubiksCube.Models;

namespace RubiksCubeSolver.Cube.Rendering.Controls
{
    public partial class RubicsCubeControl : RenderingControl<Face3D>
    {
        #region Properties

        public Dictionary<LayerType, double> LayerRotation { get; private set; }

        public ScrarchEngine.Libraries.RubiksCube.Models.RubiksCubeModel RubiksCubeModel { get; private set; }

        #endregion

        public RubicsCubeControl() : this(new ScrarchEngine.Libraries.RubiksCube.Models.RubiksCubeModel()) { }

        public RubicsCubeControl(ScrarchEngine.Libraries.RubiksCube.Models.RubiksCubeModel rubiksCube)
            : base()
        {
            RubiksCubeModel = rubiksCube;

            ResetLayerRotation();
            StartRender();

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.UserPaint, true);
        }

        public void ResetCube()
        {
            RubiksCubeModel.Reset();
        }

        public override void Render(Graphics g, IEnumerable<Face3D> frame)
        {
            bool threeD = DrawingMode == DrawingMode.Mode3D;

            var point = PointToClient(Cursor.Position);

            PositionSpec selectedPos;
            if (DrawingMode == DrawingMode.Mode3D)
            {
                selectedPos = Render3D(g, frame, point);
            }
            else if (DrawingMode == DrawingMode.Mode2D)
            {
                //selectedPos = Render2D(g, frame, point);
            }

            g.DrawRectangle(Pens.Black, 0, 0, Width - 1, Height - 1);
        }

        public override void Update(Action<IEnumerable<Face3D>> setFrame)
        {
            setFrame(GenerateFacesProjected(Screen, zoom));
        }

        #region Private Methods

        private void ResetLayerRotation()
        {
            LayerRotation = new Dictionary<LayerType, double>();
            foreach (var rp in (LayerType[])Enum.GetValues(typeof(LayerType)))
            {
                LayerRotation[rp] = 0;
            }
        }

        private PositionSpec Render3D(Graphics g, IEnumerable<Face3D> frame, Point mousePos)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var pos = PositionSpec.Default;

            foreach (var face in frame)
            {
                var brush = new SolidBrush(face.Color);

                var vertice = face.Vertices.Select(p => new PointF((float)p.X, (float)p.Y)).ToArray();
                var facePos = new PositionSpec()
                {
                    Position = face.Position,
                    Positions = face.Positions
                };

                g.FillPolygon(brush, vertice);
                g.DrawPolygon(new Pen(Color.Black, 1), vertice);

                var gp = new GraphicsPath();
                gp.AddPolygon(vertice);
                if (gp.IsVisible(mousePos))
                {
                    pos = facePos;
                }
            }

            return pos;
        }

        //public PositionSpec Render2D(Graphics g, IEnumerable<Face3D> frame, Point mousePos)
        //{
        //    g.SmoothingMode = SmoothingMode.AntiAlias;
        //    var pos = PositionSpec.Default;

        //    int square = 0, borderX = 5, borderY = 5;
        //    if ((Screen.Width - 10) / (double)(Screen.Height - 10) > (4.0 / 3.0))
        //    {
        //        square = (int)(Screen.Height / 9.0);
        //        borderX = (Screen.Width - 12 * square) / 2;
        //    }
        //    else
        //    {
        //        square = (int)(Screen.Width / 12.0);
        //        borderY = (Screen.Height - 9 * square) / 2;
        //    }

        //    var faces = new List<Face3D>();
        //    foreach (var c in RubiksCube.GetCubes())
        //    {
        //        var fs = c.Faces.Select(f => new Face3D(null, f.Color, f.Position, c.Positions));
        //        faces.AddRange(fs);

        //        #region CalculatePoints

        //        //int x = 0, y = 0;
        //        //int xOffs = borderX, yOffs = borderY;

        //        //var position = Get2dFacePosition(GetFaceNumber(face.Positions, face.Position));

        //        //if (face.Position.Equals(FacePosition.Front))
        //        //{
        //        //    xOffs += 3 * square; yOffs += 3 * square;

        //        //    x = xOffs + (position.X + 1) * square;
        //        //    y = yOffs + (position.Y * (-1) + 1) * square;
        //        //}

        //        //if (face.Position.Equals(FacePosition.Top))
        //        //{
        //        //    xOffs += 3 * square;

        //        //    x = xOffs + (position.X + 1) * square;
        //        //    y = yOffs + (position.Y + 1) * square;
        //        //}

        //        //if (face.Position.Equals(FacePosition.Bottom))
        //        //{
        //        //    xOffs += 3 * square; yOffs += 6 * square;

        //        //    x = xOffs + (position.X + 1) * square;
        //        //    y = yOffs + (position.Y * (-1) + 1) * square;
        //        //}

        //        //if (face.Position.Equals(FacePosition.Left))
        //        //{
        //        //    yOffs += 3 * square;

        //        //    x = xOffs + (position.X + 1) * square;
        //        //    y = yOffs + (position.Y * (-1) + 1) * square;
        //        //}

        //        //if (face.Position.Equals(FacePosition.Right))
        //        //{
        //        //    xOffs += 6 * square; yOffs += 3 * square;

        //        //    x = xOffs + (position.X * (-1) + 1) * square;
        //        //    y = yOffs + (position.Y * (-1) + 1) * square;
        //        //}

        //        //if (face.Position.Equals(FacePosition.Back))
        //        //{
        //        //    xOffs += 9 * square; yOffs += 3 * square;

        //        //    x = xOffs + (position.X * (-1) + 1) * square;
        //        //    y = yOffs + (position.Y * (-1) + 1) * square;
        //        //}

        //        #endregion

        //        int x = 0, y = 0;
        //        int xOffs = borderX, yOffs = borderY;

        //        var position = Get2dFacePosition(GetFaceNumber(face.Positions, face.Position));

        //        if (face.Position.Equals(FacePosition.Front))
        //        {
        //            xOffs += 3 * square; yOffs += 3 * square;

        //            x = xOffs + (position.X + 1) * square;
        //            y = yOffs + (position.Y * (-1) + 1) * square;
        //        }

        //        if (face.Position.Equals(FacePosition.Top))
        //        {
        //            xOffs += 3 * square;

        //            x = xOffs + (position.X + 1) * square;
        //            y = yOffs + (position.Y + 1) * square;
        //        }

        //        if (face.Position.Equals(FacePosition.Bottom))
        //        {
        //            xOffs += 3 * square; yOffs += 6 * square;

        //            x = xOffs + (position.X + 1) * square;
        //            y = yOffs + (position.Y * (-1) + 1) * square;
        //        }

        //        if (face.Position.Equals(FacePosition.Left))
        //        {
        //            yOffs += 3 * square;

        //            x = xOffs + (position.X + 1) * square;
        //            y = yOffs + (position.Y * (-1) + 1) * square;
        //        }

        //        if (face.Position.Equals(FacePosition.Right))
        //        {
        //            xOffs += 6 * square; yOffs += 3 * square;

        //            x = xOffs + (position.X * (-1) + 1) * square;
        //            y = yOffs + (position.Y * (-1) + 1) * square;
        //        }

        //        if (face.Position.Equals(FacePosition.Back))
        //        {
        //            xOffs += 9 * square; yOffs += 3 * square;

        //            x = xOffs + (position.X * (-1) + 1) * square;
        //            y = yOffs + (position.Y * (-1) + 1) * square;
        //        }

        //        var parr = new Point[] { new Point(x, y), new Point(x, y + square), new Point(x + square, y + square), new Point(x + square, y) };

        //        var brush = new SolidBrush(face.Color);

        //        g.FillPolygon(brush, parr);
        //        g.DrawPolygon(new Pen(Color.Black, 1), parr);

        //        var gp = new GraphicsPath();
        //        gp.AddPolygon(parr);
        //    }

        //    frame = faces;

        //    return pos;
        //}

        /// <summary>
        /// From 1 to 9
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        //private int GetFaceNumber(List<LayerType> layers, FacePosition position)
        //{
        //    if (position == FacePosition.Front || position == FacePosition.Back)
        //    {
        //        if (layers.Contains(LayerType.TopLayer))
        //        {
        //            if (layers.Contains(LayerType.LeftSlice))
        //            {
        //                if (position == FacePosition.Front) return 1;
        //                else if (position == FacePosition.Back) return 3;
        //            }
        //            else if (layers.Contains(LayerType.MiddleSliceSides))
        //            {
        //                return 2;
        //            }
        //            else if (layers.Contains(LayerType.RightSlice))
        //            {
        //                if (position == FacePosition.Front) return 3;
        //                else if (position == FacePosition.Back) return 1;
        //            }
        //        }
        //        else if (layers.Contains(LayerType.MiddleLayer))
        //        {
        //            if (layers.Contains(LayerType.LeftSlice))
        //            {
        //                if (position == FacePosition.Front) return 4;
        //                else if (position == FacePosition.Back) return 6;
        //            }
        //            else if (layers.Contains(LayerType.MiddleSliceSides))
        //            {
        //                return 5;
        //            }
        //            else if (layers.Contains(LayerType.RightSlice))
        //            {
        //                if (position == FacePosition.Front) return 6;
        //                else if (position == FacePosition.Back) return 4;
        //            }
        //        }
        //        else if (layers.Contains(LayerType.BottomLayer))
        //        {
        //            if (layers.Contains(LayerType.LeftSlice))
        //            {
        //                if (position == FacePosition.Front) return 7;
        //                else if (position == FacePosition.Back) return 9;
        //            }
        //            else if (layers.Contains(LayerType.MiddleSliceSides))
        //            {
        //                return 8;
        //            }
        //            else if (layers.Contains(LayerType.RightSlice))
        //            {
        //                if (position == FacePosition.Front) return 9;
        //                else if (position == FacePosition.Back) return 7;
        //            }
        //        }
        //    }
        //    else if (position == FacePosition.Top || position == FacePosition.Bottom)
        //    {
        //        if (layers.Contains(LayerType.BackSlice))
        //        {
        //            if (layers.Contains(LayerType.LeftSlice))
        //            {
        //                if (position == FacePosition.Top) return 1;
        //                else if (position == FacePosition.Bottom) return 3;
        //            }
        //            else if (layers.Contains(LayerType.MiddleSlice))
        //            {
        //                return 2;
        //            }
        //            else if (layers.Contains(LayerType.RightSlice))
        //            {
        //                if (position == FacePosition.Top) return 3;
        //                else if (position == FacePosition.Bottom) return 1;
        //            }
        //        }
        //        else if (layers.Contains(LayerType.MiddleSliceSides))
        //        {
        //            if (layers.Contains(LayerType.LeftSlice))
        //            {
        //                if (position == FacePosition.Top) return 4;
        //                else if (position == FacePosition.Bottom) return 6;
        //            }
        //            else if (layers.Contains(LayerType.MiddleSlice))
        //            {
        //                return 5;
        //            }
        //            else if (layers.Contains(LayerType.RightSlice))
        //            {
        //                if (position == FacePosition.Top) return 6;
        //                else if (position == FacePosition.Bottom) return 4;
        //            }
        //        }
        //        else if (layers.Contains(LayerType.FrontSlice))
        //        {
        //            if (layers.Contains(LayerType.LeftSlice))
        //            {
        //                if (position == FacePosition.Top) return 7;
        //                else if (position == FacePosition.Bottom) return 9;
        //            }
        //            else if (layers.Contains(LayerType.MiddleSlice))
        //            {
        //                return 8;
        //            }
        //            else if (layers.Contains(LayerType.RightSlice))
        //            {
        //                if (position == FacePosition.Top) return 9;
        //                else if (position == FacePosition.Bottom) return 7;
        //            }
        //        }
        //    }
        //    else if (position == FacePosition.Left || position == FacePosition.Right)
        //    {
        //        if (layers.Contains(LayerType.TopLayer))
        //        {
        //            if (layers.Contains(LayerType.BackSlice))
        //            {
        //                if (position == FacePosition.Left) return 1;
        //                else if (position == FacePosition.Right) return 3;
        //            }
        //            else if (layers.Contains(LayerType.MiddleSlice))
        //            {
        //                return 2;
        //            }
        //            else if (layers.Contains(LayerType.FrontSlice))
        //            {
        //                if (position == FacePosition.Left) return 3;
        //                else if (position == FacePosition.Right) return 1;
        //            }
        //        }
        //        else if (layers.Contains(LayerType.MiddleLayer))
        //        {
        //            if (layers.Contains(LayerType.BackSlice))
        //            {
        //                if (position == FacePosition.Left) return 4;
        //                else if (position == FacePosition.Right) return 5;
        //            }
        //            else if (layers.Contains(LayerType.MiddleSlice))
        //            {
        //                return 5;
        //            }
        //            else if (layers.Contains(LayerType.FrontSlice))
        //            {
        //                if (position == FacePosition.Left) return 6;
        //                else if (position == FacePosition.Right) return 4;
        //            }
        //        }
        //        else if (layers.Contains(LayerType.BottomLayer))
        //        {
        //            if (layers.Contains(LayerType.LeftSlice))
        //            {
        //                if (position == FacePosition.Left) return 7;
        //                else if (position == FacePosition.Right) return 9;
        //            }
        //            else if (layers.Contains(LayerType.MiddleSlice))
        //            {
        //                return 8;
        //            }
        //            else if (layers.Contains(LayerType.RightSlice))
        //            {
        //                if (position == FacePosition.Left) return 9;
        //                else if (position == FacePosition.Right) return 7;
        //            }
        //        }
        //    }

        //    throw new Exception("");
        //}

        //private Point Get2dFacePosition(int faceNumber)
        //{
        //    if (faceNumber == 1)
        //    {
        //        return new Point(-1, -1);
        //    }
        //    else if (faceNumber == 2)
        //    {
        //        return new Point(0, -1);
        //    }
        //    else if (faceNumber == 3)
        //    {
        //        return new Point(1, -1);
        //    }
        //    else if (faceNumber == 4)
        //    {
        //        return new Point(-1, 0);
        //    }
        //    else if (faceNumber == 5)
        //    {
        //        return new Point(0, 0);
        //    }
        //    else if (faceNumber == 6)
        //    {
        //        return new Point(1, 0);
        //    }
        //    else if (faceNumber == 7)
        //    {
        //        return new Point(-1, 1);
        //    }
        //    else if (faceNumber == 8)
        //    {
        //        return new Point(0, 1);
        //    }
        //    else if (faceNumber == 9)
        //    {
        //        return new Point(1, 1);
        //    }
        //    else
        //    {
        //        throw new ArgumentException("faceNumber should be from 1 to 9");
        //    }
        //}

        private static RotationDirection GetOrientation(LayerType flag)
        {
            var xFlags = new List<LayerType>() { LayerType.Right, LayerType.Left, LayerType.MiddleFromLeft };
            var yFlags = new List<LayerType>() { LayerType.Top, LayerType.Bottom, LayerType.MiddleFromTop };
            var zFlags = new List<LayerType>() { LayerType.Front, LayerType.Back, LayerType.MiddleFromFront };

            if (xFlags.Contains(flag))
            {
                return RotationDirection.X;
            }
            else if (yFlags.Contains(flag))
            {
                return RotationDirection.Y;
            }
            else if (zFlags.Contains(flag))
            {
                return RotationDirection.Z;
            }
            else
            {
                throw new Exception("");
            }
        }

        private static int ToInt(LayerType flag)
        {
            if (GetOrientation(flag) == RotationDirection.X)
            {
                switch (flag)
                {
                    case LayerType.Right:
                        return 1;
                    case LayerType.MiddleFromLeft:
                        return 0;
                    default:
                        return -1;
                }
            }

            else if (GetOrientation(flag) == RotationDirection.Y)
            {
                switch (flag)
                {
                    case LayerType.Top:
                        return -1;
                    case LayerType.MiddleFromTop:
                        return 0;
                    default:
                        return 1;
                }
            }
            else if (GetOrientation(flag) == RotationDirection.Z)
            {
                switch (flag)
                {
                    case LayerType.Back:
                        return 1;
                    case LayerType.MiddleFromFront:
                        return 0;
                    default:
                        return -1;
                }
            }
            else if (flag == LayerType.None)
                return 0;
            else
                throw new Exception("Flag can not be converted to an integer");
        }

        private static LayerType GetLayer(List<LayerType> layers, RotationDirection type)
        {
            var xFlags = new List<LayerType>() { LayerType.Right, LayerType.Left, LayerType.MiddleFromLeft };
            var yFlags = new List<LayerType>() { LayerType.Top, LayerType.Bottom, LayerType.MiddleFromTop };
            var zFlags = new List<LayerType>() { LayerType.Front, LayerType.Back, LayerType.MiddleFromFront };

            if (type == RotationDirection.X)
            {
                return layers.FirstOrDefault(l => xFlags.Contains(l));
            }
            else if (type == RotationDirection.Y)
            {
                return layers.FirstOrDefault(l => yFlags.Contains(l));
            }
            else if (type == RotationDirection.Z)
            {
                return layers.FirstOrDefault(l => zFlags.Contains(l));
            }
            else
            {
                throw new Exception("");
            }
        }

        private List<Cube3D> GenerateCubes3D()
        {
            var cubes = RubiksCubeModel.GetCubies();

            var cubes3D = new List<Cube3D>();

            double d = 2.0 / 3.0;
            foreach (var c in cubes)
            {
                var cr = new Cube3D(new Point3D(d * ToInt(GetLayer(c.Layers, RotationDirection.X)), d * ToInt(GetLayer(c.Layers, RotationDirection.Y)), d * ToInt(GetLayer(c.Layers, RotationDirection.Z))), d / 2, c);
                if (cr.Positions.Contains(LayerType.Top))
                {
                    cr = cr.Rotate(RotationDirection.Y, LayerRotation[LayerType.Top], new Point3D(0, d, 0));
                }
                if (cr.Positions.Contains(LayerType.MiddleFromTop))
                {
                    cr = cr.Rotate(RotationDirection.Y, LayerRotation[LayerType.MiddleFromTop], new Point3D(0, 0, 0));
                }
                if (cr.Positions.Contains(LayerType.Bottom))
                {
                    cr = cr.Rotate(RotationDirection.Y, LayerRotation[LayerType.Bottom], new Point3D(0, -d, 0));
                }
                if (cr.Positions.Contains(LayerType.Front))
                {
                    cr = cr.Rotate(RotationDirection.Z, LayerRotation[LayerType.Front], new Point3D(0, 0, d));
                }
                if (cr.Positions.Contains(LayerType.MiddleFromFront))
                {
                    cr = cr.Rotate(RotationDirection.Z, LayerRotation[LayerType.MiddleFromFront], new Point3D(0, 0, 0));
                }
                if (cr.Positions.Contains(LayerType.Back))
                {
                    cr = cr.Rotate(RotationDirection.Z, LayerRotation[LayerType.Back], new Point3D(0, 0, -d));
                }
                if (cr.Positions.Contains(LayerType.Left))
                {
                    cr = cr.Rotate(RotationDirection.X, LayerRotation[LayerType.Left], new Point3D(-d, 0, 0));
                }
                if (cr.Positions.Contains(LayerType.MiddleFromLeft))
                {
                    cr = cr.Rotate(RotationDirection.X, LayerRotation[LayerType.MiddleFromLeft], new Point3D(0, 0, 0));
                }
                if (cr.Positions.Contains(LayerType.Right))
                {
                    cr = cr.Rotate(RotationDirection.X, LayerRotation[LayerType.Right], new Point3D(d, 0, 0));
                }

                cr = cr.Rotate(RotationDirection.Y, Rotation[1], new Point3D(0, 0, 0));
                cr = cr.Rotate(RotationDirection.Z, Rotation[2], new Point3D(0, 0, 0));
                cr = cr.Rotate(RotationDirection.X, Rotation[0], new Point3D(0, 0, 0));
                cubes3D.Add(cr);
            }
            return cubes3D;
        }

        public IEnumerable<Face3D> GenerateFacesProjected(Rectangle screen, double scale)
        {
            var cubesRender = GenerateCubes3D();
            var facesProjected = cubesRender.SelectMany(c => c.Project(screen.Width, screen.Height, 100, 4, scale).Polygons);
            facesProjected = facesProjected.OrderBy(f => f.Vertices.ElementAt(0).Z).Reverse();
            return facesProjected;
        }

        #endregion
    }
}
