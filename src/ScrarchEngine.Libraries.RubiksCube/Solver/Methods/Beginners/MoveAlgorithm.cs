using ScrarchEngine.Libraries.RubiksCube.Models;
using System.Collections.Generic;

namespace ScrarchEngine.Libraries.RubiksCube.Solver.Methods.Beginners
{
    public class MoveAlgorithm
    {
        public Phase Phase { get; set; }
        public Dictionary<FaceType, FacePieceType?[,]> StateFrom { get; set; }
        public List<Move> Moves { get; set; }

        public MoveAlgorithm(Phase phase)
        {
            Phase = Phase;
            StateFrom = new Dictionary<FaceType, FacePieceType?[,]>();
            Moves = new List<Move>();
        }

        public void SetFaceState(FaceType type, FacePieceType?[,] state)
        {
            StateFrom.Add(type, state);
        }

        public void AddMove(Move move)
        {
            Moves.Add(move);
        }
    }
}
