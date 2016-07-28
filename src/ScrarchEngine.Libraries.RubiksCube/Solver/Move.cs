using ScrarchEngine.Libraries.RubiksCube.Models;

namespace ScrarchEngine.Libraries.RubiksCube.Solver
{
    public class Move
    {
        public string _name;

        public delegate void MoveFunction(RubiksCubeModel s);
        public MoveFunction _action;

        public Move(string name, MoveFunction action)
        {
            _name = name;
            _action = action;
        }
    }
}