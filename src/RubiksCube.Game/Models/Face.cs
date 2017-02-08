using System.Collections.Generic;
using System.Drawing;

namespace RubiksCube.Game.Models
{
    public class Face
    {
        #region Properties

        public Color Color { get; set; }

        public FacePosition Position { get; set; }

        #endregion

        #region .ctor

        public Face() { }

        public Face(Color color, FacePosition position)
        {
            Color = color;
            Position = position;
        }

        #endregion
    }
}
