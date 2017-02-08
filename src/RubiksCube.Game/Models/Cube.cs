using System.Collections.Generic;
using System.Drawing;

namespace RubiksCube.Game.Models
{
    public class Cube
    {
        #region Properties

        public IEnumerable<Face> Faces { get; set; }

        public List<LayerType> Positions { get; set; }

        #endregion

        #region .ctor

        public Cube()
        {
            Faces = GenerateFaces();
            Positions = new List<LayerType>();
        }

        #endregion

        #region Static Methods

        public static IEnumerable<Face> GenerateFaces()
        {
            return new Face[] {
                new Face(Color.Black, FacePosition.Front),
                new Face(Color.Black, FacePosition.Back),
                new Face(Color.Black, FacePosition.Top),
                new Face(Color.Black, FacePosition.Bottom),
                new Face(Color.Black, FacePosition.Right),
                new Face(Color.Black, FacePosition.Left)
              };
        }

        #endregion

        #region Public Methods

        #endregion
    }
}
