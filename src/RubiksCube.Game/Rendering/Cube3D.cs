using RubiksCube.Game.GraphicsEngine;
using RubiksCube.Game.Models;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace RubiksCube.Game.Rendering
{
    public class Cube3D : Mesh<Cube3D, Face3D>
    {
        #region Properties

        public List<LayerType> Positions { get; set; }

        #endregion

        #region .ctor

        public Cube3D(IEnumerable<Face3D> faces, Point3D location, double scale)
           : base(faces, location, scale)
        {
        }

        public Cube3D(IEnumerable<Face3D> faces, List<LayerType> positions, Point3D location, double scale)
            : base(faces, location, scale)
        {
            Positions = positions;
        }

        public Cube3D(Point3D location, double scale, List<LayerType> positions, IEnumerable<Face> faces)
            : base(location, scale)
        {
            Polygons = GenerateFaces3D(positions);
            Polygons.ToList().ForEach(f =>
            {
                var t = f.Position;
                var faceItem = faces.FirstOrDefault(face => face.Position == f.Position);

                f.Color = faceItem != null ? faceItem.Color : Color.Black;
                f.Vertices.ToList().ForEach(e =>
                {
                    e.X *= scale;
                    e.Y *= scale;
                    e.Z *= scale; //scale
                    e.X += location.X;
                    e.Y += location.Y;
                    e.Z += location.Z; //translate
                });
            });
            Positions = new List<LayerType>();
        }

        #endregion

        #region Static Methods

        private static IEnumerable<Face3D> GenerateFaces3D(List<LayerType> positions)
        {
            return new Face3D[] {
                new Face3D(new Point3D[] { new Point3D(-1, 1, -1), new Point3D(1, 1, -1), new Point3D(1, -1, -1), new Point3D(-1, -1, -1) }, Color.Black, FacePosition.Front, positions),
                new Face3D(new Point3D[] { new Point3D(-1, 1, 1), new Point3D(1, 1, 1), new Point3D(1, -1, 1), new Point3D(-1, -1, 1) }, Color.Black, FacePosition.Back, positions),
                new Face3D(new Point3D[] { new Point3D(-1, -1, -1), new Point3D(1, -1, -1), new Point3D(1, -1, 1), new Point3D(-1, -1, 1) }, Color.Black, FacePosition.Top, positions),
                new Face3D(new Point3D[] { new Point3D(-1, 1, -1), new Point3D(1, 1, -1), new Point3D(1, 1, 1), new Point3D(-1, 1, 1) }, Color.Black, FacePosition.Bottom, positions),
                new Face3D(new Point3D[] { new Point3D(1, 1, 1), new Point3D(1, 1, -1), new Point3D(1, -1, -1), new Point3D(1, -1, 1) }, Color.Black, FacePosition.Right, positions),
                new Face3D(new Point3D[] { new Point3D(-1, 1, 1), new Point3D(-1, 1, -1), new Point3D(-1, -1, -1), new Point3D(-1, -1, 1) }, Color.Black, FacePosition.Left, positions)
            };
        }

        #endregion

        #region Private Methods

        private Cube3D DeepClone()
        {
            //Deep Clone
            var faces = new List<Face3D>();
            foreach (var f in Polygons)
            {
                var edges = new List<Point3D>();
                foreach (Point3D p in f.Vertices)
                {
                    edges.Add(new Point3D(p.X, p.Y, p.Z));
                }
                faces.Add(new Face3D(edges, f.Color, f.Position, f.Positions));
            }
            return new Cube3D(faces, Positions, Location, Scale);
        }

        #endregion

        #region Methods

        public override Cube3D Rotate(RotationType type, double angle, Point3D center)
        {
            var cube3D = DeepClone();
            Rotate(cube3D, type, angle, center);
            return cube3D;
        }

        public override Cube3D Project(int viewWidth, int viewHeight, int fov, int viewDistance, double scale)
        {
            var cube3D = base.Project(viewWidth, viewHeight, fov, viewDistance, scale);
            cube3D.Positions = Positions;
            return cube3D;
        }

        #endregion
    }
}
