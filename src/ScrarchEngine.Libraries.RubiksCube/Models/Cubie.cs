using System.Collections.Generic;

namespace ScrarchEngine.Libraries.RubiksCube.Models
{
    public struct Cubie
    {
        public List<FacePiece> Pieces { get; private set; }
        public List<LayerType> Layers { get; private set; }

        public Cubie(List<FacePiece> pieces, List<LayerType> layers)
        {
            Pieces = pieces;
            Layers = layers;
        }
    }
}
