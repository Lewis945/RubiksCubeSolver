using ScrarchEngine.Libraries.RubiksCube.Models;
using System;

namespace ScrarchEngine.Libraries.RubiksCube.Solver
{
    public class Move
    {
        public LayerType Layer { get; private set; }
        public RotationType Rotation { get; private set; }

        public Move(LayerType layer, RotationType rotation)
        {
            Layer = layer;
            Rotation = rotation;
        }
    }
}
