using System;
using System.Collections.Generic;
using System.Linq;

namespace RubiksCube.Game.GraphicsEngine
{
    public abstract class Mesh<TM, TP>
        where TM : Mesh<TM,TP>
        where TP : Polygon<TP>
    {
        #region Properties

        public IEnumerable<TP> Polygons { get; protected set; }

        public Point3D Location { get; protected set; }

        public double Scale { get; protected set; }

        #endregion

        #region .ctor

        public Mesh(Point3D location, double scale)
        {
            Location = location;
            Scale = scale;
        }

        public Mesh(IEnumerable<TP> poligons, Point3D location, double scale)
        {
            Polygons = poligons;
            Location = location;
            Scale = scale;
        }

        #endregion

        #region Methods

        public abstract TM Rotate(RotationDirection type, double angle, Point3D center);

        protected static void Rotate(TM mesh, RotationDirection type, double angle, Point3D center)
        {
            foreach (var poligon in mesh.Polygons)
            {
                poligon.Vertices.ToList().ForEach(e => { e.X -= center.X; e.Y -= center.Y; e.Z -= center.Z; });
                poligon.Rotate(type, angle);
                poligon.Vertices.ToList().ForEach(e => { e.X += center.X; e.Y += center.Y; e.Z += center.Z; });
            }
        }

        public virtual TM Project(int viewWidth, int viewHeight, int fov, int viewDistance, double scale)
        {
            return (TM)Activator.CreateInstance(typeof(TM), Polygons.Select(p => p.Project(viewWidth, viewHeight, fov, viewDistance, scale)), Location, Scale);
        }

        #endregion
    }
}
