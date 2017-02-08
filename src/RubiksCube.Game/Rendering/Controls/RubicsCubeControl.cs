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
using RubiksCube.Game.Models;

namespace RubiksCubeSolver.Cube.Rendering.Controls
{
    public partial class RubicsCubeControl : RenderingControl<Face3D>
    {
        #region Properties

        public RubiksCube.Game.Models.RubiksCube RubiksCube { get; private set; }

        public Dictionary<LayerType, double> LayerRotation { get; private set; }

        public ScrarchEngine.Libraries.RubiksCube.Models.RubiksCubeModel RubiksCubeModel { get; private set; }

        #endregion

        public RubicsCubeControl() : this(new RubiksCube.Game.Models.RubiksCube()) { }

        public RubicsCubeControl(RubiksCube.Game.Models.RubiksCube rubiksCube)
            : base()
        {
            RubiksCube = rubiksCube;
            RubiksCubeModel = new ScrarchEngine.Libraries.RubiksCube.Models.RubiksCubeModel();

            ResetLayerRotation();
            StartRender();

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.UserPaint, true);
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
                    FacePosition = face.Position,
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

        private static RotationType GetOrientation(LayerType flag)
        {
            var xFlags = new List<LayerType>() { LayerType.RightSlice, LayerType.LeftSlice, LayerType.MiddleSliceSides };
            var yFlags = new List<LayerType>() { LayerType.TopLayer, LayerType.BottomLayer, LayerType.MiddleLayer };
            var zFlags = new List<LayerType>() { LayerType.FrontSlice, LayerType.BackSlice, LayerType.MiddleSlice };

            if (xFlags.Contains(flag))
            {
                return RotationType.X;
            }
            else if (yFlags.Contains(flag))
            {
                return RotationType.Y;
            }
            else if (zFlags.Contains(flag))
            {
                return RotationType.Z;
            }
            else
            {
                throw new Exception("");
            }
        }

        private static int ToInt(LayerType flag)
        {
            if (GetOrientation(flag) == RotationType.X)
            {
                switch (flag)
                {
                    case LayerType.RightSlice:
                        return 1;
                    case LayerType.MiddleSliceSides:
                        return 0;
                    default:
                        return -1;
                }
            }

            else if (GetOrientation(flag) == RotationType.Y)
            {
                switch (flag)
                {
                    case LayerType.TopLayer:
                        return -1;
                    case LayerType.MiddleLayer:
                        return 0;
                    default:
                        return 1;
                }
            }
            else if (GetOrientation(flag) == RotationType.Z)
            {
                switch (flag)
                {
                    case LayerType.BackSlice:
                        return 1;
                    case LayerType.MiddleSlice:
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

        private static LayerType GetLayer(List<LayerType> layers, RotationType type)
        {
            var xFlags = new List<LayerType>() { LayerType.RightSlice, LayerType.LeftSlice, LayerType.MiddleSliceSides };
            var yFlags = new List<LayerType>() { LayerType.TopLayer, LayerType.BottomLayer, LayerType.MiddleLayer };
            var zFlags = new List<LayerType>() { LayerType.FrontSlice, LayerType.BackSlice, LayerType.MiddleSlice };

            if (type == RotationType.X)
            {
                return layers.FirstOrDefault(l => xFlags.Contains(l));
            }
            else if (type == RotationType.Y)
            {
                return layers.FirstOrDefault(l => yFlags.Contains(l));
            }
            else if (type == RotationType.Z)
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
            var cubes = GetCubes(RubiksCubeModel);

            var cubes3D = new List<Cube3D>();

            double d = 2.0 / 3.0;
            foreach (var c in cubes)
            {
                var cr = new Cube3D(new Point3D(d * ToInt(GetLayer(c.Positions, RotationType.X)), d * ToInt(GetLayer(c.Positions, RotationType.Y)), d * ToInt(GetLayer(c.Positions, RotationType.Z))), d / 2, c.Positions, c.Faces);
                if (cr.Positions.Contains(LayerType.TopLayer))
                {
                    cr = cr.Rotate(RotationType.Y, LayerRotation[LayerType.TopLayer], new Point3D(0, d, 0));
                }
                if (cr.Positions.Contains(LayerType.MiddleLayer))
                {
                    cr = cr.Rotate(RotationType.Y, LayerRotation[LayerType.MiddleLayer], new Point3D(0, 0, 0));
                }
                if (cr.Positions.Contains(LayerType.BottomLayer))
                {
                    cr = cr.Rotate(RotationType.Y, LayerRotation[LayerType.BottomLayer], new Point3D(0, -d, 0));
                }
                if (cr.Positions.Contains(LayerType.FrontSlice))
                {
                    cr = cr.Rotate(RotationType.Z, LayerRotation[LayerType.FrontSlice], new Point3D(0, 0, d));
                }
                if (cr.Positions.Contains(LayerType.MiddleSlice))
                {
                    cr = cr.Rotate(RotationType.Z, LayerRotation[LayerType.MiddleSlice], new Point3D(0, 0, 0));
                }
                if (cr.Positions.Contains(LayerType.BackSlice))
                {
                    cr = cr.Rotate(RotationType.Z, LayerRotation[LayerType.BackSlice], new Point3D(0, 0, -d));
                }
                if (cr.Positions.Contains(LayerType.LeftSlice))
                {
                    cr = cr.Rotate(RotationType.X, LayerRotation[LayerType.LeftSlice], new Point3D(-d, 0, 0));
                }
                if (cr.Positions.Contains(LayerType.MiddleSliceSides))
                {
                    cr = cr.Rotate(RotationType.X, LayerRotation[LayerType.MiddleSliceSides], new Point3D(0, 0, 0));
                }
                if (cr.Positions.Contains(LayerType.RightSlice))
                {
                    cr = cr.Rotate(RotationType.X, LayerRotation[LayerType.RightSlice], new Point3D(d, 0, 0));
                }

                cr = cr.Rotate(RotationType.Y, Rotation[1], new Point3D(0, 0, 0));
                cr = cr.Rotate(RotationType.Z, Rotation[2], new Point3D(0, 0, 0));
                cr = cr.Rotate(RotationType.X, Rotation[0], new Point3D(0, 0, 0));
                cubes3D.Add(cr);
            }
            return cubes3D;
        }

        private List<RubiksCube.Game.Models.Cube> GetCubes(ScrarchEngine.Libraries.RubiksCube.Models.RubiksCubeModel model)
        {
            var cubes = new List<RubiksCube.Game.Models.Cube>();

            foreach (var cubie in model.GetCubies())
            {
                var faces = new Face[cubie.Pieces.Count];
                for (int i = 0; i < cubie.Pieces.Count; i++)
                {
                    var piece = cubie.Pieces[i];

                    var color = Color.White;
                    switch (piece.CurrentType)
                    {
                        case ScrarchEngine.Libraries.RubiksCube.Models.FacePieceType.White:
                            color = Color.White;
                            break;
                        case ScrarchEngine.Libraries.RubiksCube.Models.FacePieceType.Blue:
                            color = Color.Blue;
                            break;
                        case ScrarchEngine.Libraries.RubiksCube.Models.FacePieceType.Green:
                            color = Color.Green;
                            break;
                        case ScrarchEngine.Libraries.RubiksCube.Models.FacePieceType.Orange:
                            color = Color.Orange;
                            break;
                        case ScrarchEngine.Libraries.RubiksCube.Models.FacePieceType.Red:
                            color = Color.Red;
                            break;
                        case ScrarchEngine.Libraries.RubiksCube.Models.FacePieceType.Yellow:
                            color = Color.Yellow;
                            break;
                        default:
                            break;
                    }

                    var position = FacePosition.None;
                    switch (piece.Face)
                    {
                        case ScrarchEngine.Libraries.RubiksCube.Models.FaceType.Top:
                            position = FacePosition.Top;
                            break;
                        case ScrarchEngine.Libraries.RubiksCube.Models.FaceType.Bottom:
                            position = FacePosition.Bottom;
                            break;
                        case ScrarchEngine.Libraries.RubiksCube.Models.FaceType.Left:
                            position = FacePosition.Left;
                            break;
                        case ScrarchEngine.Libraries.RubiksCube.Models.FaceType.Right:
                            position = FacePosition.Right;
                            break;
                        case ScrarchEngine.Libraries.RubiksCube.Models.FaceType.Back:
                            position = FacePosition.Back;
                            break;
                        case ScrarchEngine.Libraries.RubiksCube.Models.FaceType.Front:
                            position = FacePosition.Front;
                            break;
                        default:
                            break;
                    }

                    faces[i] = new Face(color, position);
                }

                var layers = new List<LayerType>();
                foreach (var layerType in cubie.Layers)
                {
                    var layer = LayerType.None;

                    switch (layerType)
                    {
                        case ScrarchEngine.Libraries.RubiksCube.Models.LayerType.None:
                            layer = LayerType.None;
                            break;
                        case ScrarchEngine.Libraries.RubiksCube.Models.LayerType.Top:
                            layer = LayerType.TopLayer;
                            break;
                        case ScrarchEngine.Libraries.RubiksCube.Models.LayerType.MiddleFromTop:
                            layer = LayerType.MiddleLayer;
                            break;
                        case ScrarchEngine.Libraries.RubiksCube.Models.LayerType.Bottom:
                            layer = LayerType.BottomLayer;
                            break;
                        case ScrarchEngine.Libraries.RubiksCube.Models.LayerType.Front:
                            layer = LayerType.FrontSlice;
                            break;
                        case ScrarchEngine.Libraries.RubiksCube.Models.LayerType.MiddleFromFront:
                            layer = LayerType.MiddleSlice;
                            break;
                        case ScrarchEngine.Libraries.RubiksCube.Models.LayerType.Back:
                            layer = LayerType.BackSlice;
                            break;
                        case ScrarchEngine.Libraries.RubiksCube.Models.LayerType.Left:
                            layer = LayerType.LeftSlice;
                            break;
                        case ScrarchEngine.Libraries.RubiksCube.Models.LayerType.MiddleFromLeft:
                            layer = LayerType.MiddleSliceSides;
                            break;
                        case ScrarchEngine.Libraries.RubiksCube.Models.LayerType.Right:
                            layer = LayerType.RightSlice;
                            break;
                        default:
                            break;
                    }

                    layers.Add(layer);
                }

                cubes.Add(new RubiksCube.Game.Models.Cube()
                {
                    Faces = faces,
                    Positions = layers
                });
            }

            return cubes;
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
