using ScrarchEngine.Libraries.RubiksCube.Models;
using System;

namespace ScrarchEngine.Libraries.RubiksCube.Solver
{
    public class Move
    {
        public LayerType Layer { get; private set; }
        public RotationType Rotation { get; private set; }
        public Action Motion { get; private set; }

        public Move(LayerType layer, RotationType rotation, Action motion)
        {
            Layer = layer;
            Rotation = rotation;
            Motion = motion;
        }
    }
}
