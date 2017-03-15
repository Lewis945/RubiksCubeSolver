using Newtonsoft.Json;
using ScrarchEngine.Libraries.RubiksCube.Models;
using System.Collections.Generic;
using System.Text;
using ScrarchEngine.Libraries.RubiksCube.Json;

namespace ScrarchEngine.Libraries.RubiksCube.Solver.Methods.Beginners
{
    public class MoveAlgorithm
    {
        public string Name { get; set; }

        public Phase Phase { get; set; }
        public bool IsFinal { get; set; }

        public bool IsFlip { get; set; }
        public FlipAxis Axis { get; set; }
        public RotationType RotationType { get; set; }

        [JsonConverter(typeof(CustomStateConverter))]
        public Dictionary<FaceType, FaceType?[,]> StateFrom { get; set; }

        [JsonConverter(typeof(CustomMoveConverter))]
        public List<Move> Moves { get; set; }

        public MoveAlgorithm()
        {
            StateFrom = new Dictionary<FaceType, FaceType?[,]>();
            Moves = new List<Move>();
        }

        public MoveAlgorithm(FlipAxis axis, RotationType type)
        {
            IsFlip = true;
            Axis = axis;
            RotationType = type;
        }

        private int Process(Move move, Move nextMove, int i, LayerType layer, string layerLetter, StringBuilder builder)
        {
            if (move.Layer == layer)
            {
                if (move.Rotation == RotationType.CounterClockwise)
                    builder.Append($"{layerLetter}'");
                else if (nextMove != null && nextMove.Layer == layer)
                {
                    builder.Append($"{layerLetter}2");
                    i++;
                }
                else
                    builder.Append(layerLetter);
            }

            return i;
        }

        public override string ToString()
        {
            var algorithm = new StringBuilder();
            if (Moves != null)
                for (int i = 0; i < Moves.Count; i++)
                {
                    var move = Moves[i];
                    var nextMove = i < Moves.Count - 1 ? Moves[i + 1] : null;

                    i = Process(move, nextMove, i, LayerType.Front, "F", algorithm);
                    i = Process(move, nextMove, i, LayerType.Back, "B", algorithm);
                    i = Process(move, nextMove, i, LayerType.Top, "U", algorithm);
                    i = Process(move, nextMove, i, LayerType.Bottom, "D", algorithm);
                    i = Process(move, nextMove, i, LayerType.Left, "L", algorithm);
                    i = Process(move, nextMove, i, LayerType.Right, "R", algorithm);
                }
            else if(IsFlip)
                algorithm.Append($"Flip {Axis.ToString()} in {RotationType.ToString()}");

            return algorithm.ToString();
        }
    }
}
