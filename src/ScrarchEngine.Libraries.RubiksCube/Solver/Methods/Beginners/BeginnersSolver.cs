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
        #region Fields

        private RubiksCubeModel _model;

        private List<MoveAlgorithm> _algorithms;

        #endregion

        #region .ctor

        public BeginnersSolver(RubiksCubeModel model, Func<string, string> getContent)
        {
            _model = model.CloneJson(model);
            _algorithms = JsonConvert.DeserializeObject<List<MoveAlgorithm>>(getContent(@"D:\Projects\RubiksCube\src\ScrarchEngine.Libraries.RubiksCube\Solver\Methods\Beginners\patterns.json"));
        }

        #endregion

        #region Public Methods

        public List<MoveAlgorithm> SolveCross()
        {
            var solution = new List<MoveAlgorithm>();

            var crossAlgorithms = _algorithms.Where(a => a.Phase == Phase.FirstCross).ToList();

            while (!IsCrossReady())
            {
                var alg = crossAlgorithms.FirstOrDefault(a => DoesStateMatch(a.StateFrom));
                if (alg == null)
                {
                    solution.Add(new MoveAlgorithm(FlipAxis.Vertical, RotationType.Clockwise));
                    _model.FlipCube(FlipAxis.Vertical, RotationType.Clockwise);
                    continue;
                }

                solution.Add(alg);
                foreach (var move in alg.Moves)
                    _model.Rotate90Degrees(move.Layer, move.Rotation);

                if (alg.IsFinal)
                {
                    solution.Add(new MoveAlgorithm(FlipAxis.Vertical, RotationType.Clockwise));
                    _model.FlipCube(FlipAxis.Vertical, RotationType.Clockwise);
                }
            }

            return solution;
        }

        public List<MoveAlgorithm> SolveFirstLayer()
        {
            var solution = new List<MoveAlgorithm>();

            var firstLayerAlgorithms = _algorithms.Where(a => a.Phase == Phase.FirstLayer).ToList();

            while (!IsFirstLayerReady())
            {
                var alg = firstLayerAlgorithms.FirstOrDefault(a => DoesStateMatch(a.StateFrom));
                if (alg == null)
                {
                    solution.Add(new MoveAlgorithm(FlipAxis.Vertical, RotationType.Clockwise));
                    _model.FlipCube(FlipAxis.Vertical, RotationType.Clockwise);
                    continue;
                }

                solution.Add(alg);
                foreach (var move in alg.Moves)
                    _model.Rotate90Degrees(move.Layer, move.Rotation);

                if (alg.IsFinal)
                {
                    solution.Add(new MoveAlgorithm(FlipAxis.Vertical, RotationType.Clockwise));
                    _model.FlipCube(FlipAxis.Vertical, RotationType.Clockwise);
                }
            }

            return solution;
        }

        public List<MoveAlgorithm> SolveSecondLayer()
        {
            var solution = new List<MoveAlgorithm>();

            solution.Add(new MoveAlgorithm(FlipAxis.Vertical, RotationType.Clockwise));
            solution.Add(new MoveAlgorithm(FlipAxis.Vertical, RotationType.Clockwise));
            solution.Add(new MoveAlgorithm(FlipAxis.Horizontal, RotationType.Clockwise));
            solution.Add(new MoveAlgorithm(FlipAxis.Horizontal, RotationType.Clockwise));

            _model.FlipCube(FlipAxis.Vertical, RotationType.Clockwise);
            _model.FlipCube(FlipAxis.Vertical, RotationType.Clockwise);
            _model.FlipCube(FlipAxis.Horizontal, RotationType.Clockwise);
            _model.FlipCube(FlipAxis.Horizontal, RotationType.Clockwise);

            var secondLayerAlgorithms = _algorithms.Where(a => a.Phase == Phase.SecondLayer).ToList();

            while (!IsSecondLayerReady())
            {
                var alg = secondLayerAlgorithms.FirstOrDefault(a => DoesStateMatch(a.StateFrom));
                if (alg == null)
                {
                    solution.Add(new MoveAlgorithm(FlipAxis.Vertical, RotationType.Clockwise));
                    _model.FlipCube(FlipAxis.Vertical, RotationType.Clockwise);
                    continue;
                }

                solution.Add(alg);
                foreach (var move in alg.Moves)
                    _model.Rotate90Degrees(move.Layer, move.Rotation);

                if (alg.IsFinal)
                {
                    solution.Add(new MoveAlgorithm(FlipAxis.Vertical, RotationType.Clockwise));
                    _model.FlipCube(FlipAxis.Vertical, RotationType.Clockwise);
                }
            }

            return solution;
        }

        public List<MoveAlgorithm> SolveSecondFlatCross()
        {
            var solution = new List<MoveAlgorithm>();

            var secondCrossAlgorithms = _algorithms.Where(a => a.Phase == Phase.SecondFlatCross).ToList();

            int flips = 0;
            while (!IsCrossReady())
            {
                if (flips > 4)
                {
                    flips = 0;
                    var initAlg = secondCrossAlgorithms.FirstOrDefault(a => a.Name == "Init cross on Up");
                    solution.Add(initAlg);
                    foreach (var move in initAlg.Moves)
                        _model.Rotate90Degrees(move.Layer, move.Rotation);
                }

                var alg = secondCrossAlgorithms.FirstOrDefault(a => DoesStateMatch(a.StateFrom));
                if (alg == null)
                {
                    solution.Add(new MoveAlgorithm(FlipAxis.Vertical, RotationType.Clockwise));
                    _model.FlipCube(FlipAxis.Vertical, RotationType.Clockwise);
                    flips++;
                    continue;
                }

                flips = 0;

                solution.Add(alg);
                foreach (var move in alg.Moves)
                    _model.Rotate90Degrees(move.Layer, move.Rotation);

                if (alg.IsFinal)
                {
                    solution.Add(new MoveAlgorithm(FlipAxis.Vertical, RotationType.Clockwise));
                    _model.FlipCube(FlipAxis.Vertical, RotationType.Clockwise);
                }
            }

            return solution;
        }

        public List<MoveAlgorithm> Solve()
        {
            var solution = new List<MoveAlgorithm>();

            solution.AddRange(SolveCross());
            solution.AddRange(SolveFirstLayer());
            solution.AddRange(SolveSecondLayer());
            solution.AddRange(SolveSecondFlatCross());

            return solution;
        }

        #endregion

        #region Private Methods

        private bool DoesStateMatch(Dictionary<FaceType, FaceType?[,]> state)
        {
            var result = new List<bool>();

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

            return result.All(s => s) && state.Count > 0;
        }

        private bool IsCrossReady()
        {
            var face = _model.GetFace(FaceType.Up);
            return face[0, 1] == face.PieceType && face[1, 0] == face.PieceType && face[1, 1] == face.PieceType && face[1, 2] == face.PieceType && face[2, 1] == face.PieceType;
        }

        private bool IsFirstLayerReady()
        {
            var upFace = _model.GetFace(FaceType.Up);

            bool result = true;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    result &= upFace[i, j] == upFace.PieceType;

            var frontFace = _model.GetFace(FaceType.Front);
            var rightFace = _model.GetFace(FaceType.Right);
            var backFace = _model.GetFace(FaceType.Back);
            var leftFace = _model.GetFace(FaceType.Left);

            for (int k = 0; k < 3; k++)
            {
                result &= frontFace[k] == frontFace.PieceType;
                result &= rightFace[k] == rightFace.PieceType;
                result &= backFace[k] == backFace.PieceType;
                result &= leftFace[k] == leftFace.PieceType;
            }

            return result;
        }

        private bool IsSecondLayerReady()
        {
            bool result = true;

            var frontFace = _model.GetFace(FaceType.Front);
            var rightFace = _model.GetFace(FaceType.Right);
            var backFace = _model.GetFace(FaceType.Back);
            var leftFace = _model.GetFace(FaceType.Left);

            for (int k = 3; k < 6; k++)
            {
                result &= frontFace[k] == frontFace.PieceType;
                result &= rightFace[k] == rightFace.PieceType;
                result &= backFace[k] == backFace.PieceType;
                result &= leftFace[k] == leftFace.PieceType;
            }

            return result;
        }

        #endregion
    }
}