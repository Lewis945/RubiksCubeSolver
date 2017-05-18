using RubiksCube.Game.GraphicsEngine;
using ScrarchEngine.Libraries.RubiksCube.Models;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace RubiksCube.Game.Rendering
{
    public class Face3D : Polygon<Face3D>
    {
        #region Properties

        public FacePosition Position { get; set; }

        public List<LayerType> Positions { get; private set; }

        #endregion

        #region .ctor

        public Face3D(IEnumerable<Point3D> vertices, Color color)
            : base(vertices, color)
        {
        }

        public Face3D(IEnumerable<Point3D> vertices, Color color, FacePosition position, List<LayerType> layerType)
            : base(vertices, color)
        {
            Position = position;
            Positions = layerType;
        }

        #endregion

        #region Methods

        public override Face3D Project(int viewWidth, int viewHeight, int fov, int viewDistance, double scale)
        {
            var polygon = base.Project(viewWidth, viewHeight, fov, viewDistance, scale);
            polygon.Position = Position;
            polygon.Positions = Positions;
            return polygon;
        }

        #endregion
    }
}
