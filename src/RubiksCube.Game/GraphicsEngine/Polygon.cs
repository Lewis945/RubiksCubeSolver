using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace RubiksCube.Game.GraphicsEngine
{
    public class Polygon<T>
        where T : class
    {
        #region Properties

        public IEnumerable<Point3D> Vertices { get; protected set; }

        public Color Color { get; set; }

        #endregion

        #region .ctor

        public Polygon(IEnumerable<Point3D> vertices)
        {
            Vertices = vertices;
        }

        public Polygon(IEnumerable<Point3D> vertices, Color color)
        {
            Vertices = vertices;
            Color = color;
        }

        #endregion

        #region Methods

        public void Rotate(RotationDirection type, double angleInDeg)
        {
            Vertices.ToList().ForEach(v => v.Rotate(type, angleInDeg));
        }

        public Point3D GetCenter()
        {
            return new Point3D(Vertices.Average(v => v.X), Vertices.Average(v => v.Y), Vertices.Average(v => v.Z));
        }

        public virtual T Project(int viewWidth, int viewHeight, int fov, int viewDistance, double scale)
        {
            var parr = Vertices.Select(v => v.Project(viewWidth, viewHeight, fov, viewDistance, scale));
            double mid = parr.Average(v => v.Z);
            parr = parr.Select(p => new Point3D(p.X, p.Y, mid));
            return (T)Activator.CreateInstance(typeof(T), parr, Color);
        }

        #endregion
    }
}
