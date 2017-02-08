using RubiksCube.Game.Models;
using System.Collections.Generic;

namespace RubiksCube.Game.Rendering
{
    public struct PositionSpec
    {
        #region Properties

        public List<LayerType> Positions { get; set; }

        public FacePosition FacePosition { get; set; }

        public bool IsDefault
        {
            get
            {
                return (Positions == null || FacePosition == Models.FacePosition.None);
            }
        }

        #endregion

        #region Static Methods

        public static PositionSpec Default
        {
            get
            {
                return new PositionSpec() { Positions = null, FacePosition = Models.FacePosition.None };
            }
        }

        #endregion

        #region Methods

        public bool Equals(PositionSpec compare)
        {
            return (compare.Positions == Positions && compare.FacePosition == FacePosition);
        }

        #endregion
    }
}
