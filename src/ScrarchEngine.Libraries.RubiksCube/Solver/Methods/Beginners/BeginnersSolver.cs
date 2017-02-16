using Newtonsoft.Json;
using ScrarchEngine.Libraries.RubiksCube.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ScrarchEngine.Libraries.RubiksCube.Solver.Methods.Beginners
{
    public class BeginnersSolver : BaseSolver
    {
        private RubiksCube.Models.RubiksCubeModel _model;

        private List<MoveAlgorithm> _algorithms;

        public BeginnersSolver(RubiksCube.Models.RubiksCubeModel model, Func<string, string> getContent)
        {
            _model = model;
            _algorithms = JsonConvert.DeserializeObject<List<MoveAlgorithm>>(getContent(@"D:\Projects\RubiksCube\src\ScrarchEngine.Libraries.RubiksCube\Solver\Methods\Beginners\patterns.json"));
        }

        private bool DoesStateMatch(Dictionary<FaceType, FaceType?[,]> state)
        {
            var result = new List<bool>();

            var top = _model.GetFace(FaceType.Up);
            var topColor = top.PieceType;

            foreach (var stateItem in state)
            {
                var face = _model.GetFace(stateItem.Key);
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        var type = stateItem.Value[i, j];
                        if (!type.HasValue)
                            continue;
                        var faceToCompare = _model.GetFace(type.Value);
                        var faceToCompareColor = faceToCompare.PieceType;
                        result.Add(face[i, j] == faceToCompareColor);
                    }
                }
            }

            return result.Count(s => s) == state.Count;
        }

        private bool IsCrossReady()
        {
            var face = _model.GetFace(FaceType.Up);
            return face[0, 1] == FacePieceType.White && face[1, 0] == FacePieceType.White && face[1, 1] == FacePieceType.White && face[1, 2] == FacePieceType.White && face[2, 1] == FacePieceType.White;
        }

        private void BuildCross()
        {
            var crossAlgorithms = _algorithms.Where(a => a.Phase == Phase.FirstCross);

            while (!IsCrossReady())
            {
                var alg = crossAlgorithms.FirstOrDefault(a => DoesStateMatch(a.StateFrom));
                if (alg == null)
                {
                    // Rotate cube
                    continue;
                }
                foreach (var move in alg.Moves)
                    _model.Rotate90Degrees(move.Layer, move.Rotation);
            }
        }

        public void Solve()
        {
            BuildCross();
        }
    }
}