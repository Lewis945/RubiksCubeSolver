using ScrarchEngine.Libraries.RubiksCube.Models;
using System.Collections.Generic;

namespace ScrarchEngine.Libraries.RubiksCube.Solver
{
    public abstract class BaseSolver
    {
        Dictionary<string, Move> _allMoves;

        public BaseSolver()
        {
            _allMoves = new Dictionary<string, Move>();
        }

        public Move GetMove(string move)
        {
            return _allMoves[move];
        }

        public bool IsValidMove(string move)
        {
            return _allMoves.ContainsKey(move);
        }

        protected void AddMove(string name, Move.MoveFunction action)
        {
            _allMoves.Add(name, new Move(name, action));
        }
    }
}