﻿using Newtonsoft.Json;
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

        public List<MoveAlgorithm> SolveSecondCross()
        {
            var solution = new List<MoveAlgorithm>();

            var crossAlgorithms = _algorithms.Where(a => a.Phase == Phase.SecondCross).ToList();

            int rotations = 0;
            while (!IsSecondCrossReady())
            {
                var alg = crossAlgorithms.FirstOrDefault(a => DoesStateMatch(a.StateFrom));
                if (alg == null)
                {
                    if (rotations < 4)
                    {
                        solution.Add(new MoveAlgorithm() { Moves = new List<Move> { new Move(LayerType.Top, RotationType.Clockwise) } });
                        _model.Rotate90Degrees(LayerType.Top, RotationType.Clockwise);
                        rotations++;
                    }
                    else
                    {
                        solution.Add(new MoveAlgorithm(FlipAxis.Vertical, RotationType.Clockwise));
                        _model.FlipCube(FlipAxis.Vertical, RotationType.Clockwise);
                        rotations = 0;
                    }
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

        public List<MoveAlgorithm> SolveThirdLayerCubiesLocations()
        {
            var solution = new List<MoveAlgorithm>();

            var thirdLayerAlgorithms = _algorithms.Where(a => a.Phase == Phase.ThirdLayer).ToList();
            var initAlg = thirdLayerAlgorithms.FirstOrDefault(a => a.Name == "Init");

            while (!IsAnyThirdLayerCubieInPlace())
            {
                solution.Add(initAlg);
                foreach (var move in initAlg.Moves)
                    _model.Rotate90Degrees(move.Layer, move.Rotation);
            }

            while (!IsCubieInPlaceInTheTopRightCorner())
            {
                solution.Add(new MoveAlgorithm(FlipAxis.Vertical, RotationType.Clockwise));
                _model.FlipCube(FlipAxis.Vertical, RotationType.Clockwise);
            }

            while (!AreThirdLayerCubiesInPlace())
            {
                solution.Add(initAlg);
                foreach (var move in initAlg.Moves)
                    _model.Rotate90Degrees(move.Layer, move.Rotation);
            }

            return solution;
        }

        public List<MoveAlgorithm> SolveThirdLayer()
        {
            var solution = new List<MoveAlgorithm>();

            while (!IsCubieInPlaceInTheTopRightCorner())
            {
                solution.Add(new MoveAlgorithm(FlipAxis.Vertical, RotationType.Clockwise));
                _model.FlipCube(FlipAxis.Vertical, RotationType.Clockwise);
            }

            solution.Add(new MoveAlgorithm(FlipAxis.Horizontal, RotationType.Clockwise));
            _model.FlipCube(FlipAxis.Horizontal, RotationType.Clockwise);
            solution.Add(new MoveAlgorithm(FlipAxis.Vertical, RotationType.CounterClockwise));
            _model.FlipCube(FlipAxis.Vertical, RotationType.CounterClockwise);
            solution.Add(new MoveAlgorithm(FlipAxis.Horizontal, RotationType.CounterClockwise));
            _model.FlipCube(FlipAxis.Horizontal, RotationType.CounterClockwise);

            var fourAlg = _algorithms.FirstOrDefault(a => a.Phase == Phase.ThirdLayer && a.Name == "Four");

            while (!IsLeftLayerReady())
            {
                while (!IsCubieMatchInTheTopLeftCorner())
                {
                    solution.Add(fourAlg);
                    foreach (var move in fourAlg.Moves)
                        _model.Rotate90Degrees(move.Layer, move.Rotation);
                }

                solution.Add(new MoveAlgorithm() { Moves = new List<Move> { new Move(LayerType.Left, RotationType.Clockwise) } });
                _model.Rotate90Degrees(LayerType.Left, RotationType.Clockwise);
            }

            while (!IsLeftLayerInPlace())
            {
                solution.Add(new MoveAlgorithm() { Moves = new List<Move> { new Move(LayerType.Left, RotationType.Clockwise) } });
                _model.Rotate90Degrees(LayerType.Left, RotationType.Clockwise);
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
            solution.AddRange(SolveSecondCross());
            solution.AddRange(SolveThirdLayerCubiesLocations());
            solution.AddRange(SolveThirdLayer());

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

        private bool IsSecondCrossReady()
        {
            bool result = true;

            var frontFace = _model.GetFace(FaceType.Front);
            var rightFace = _model.GetFace(FaceType.Right);
            var backFace = _model.GetFace(FaceType.Back);
            var leftFace = _model.GetFace(FaceType.Left);

            result &= frontFace[1] == frontFace.PieceType;
            result &= rightFace[1] == rightFace.PieceType;
            result &= backFace[1] == backFace.PieceType;
            result &= leftFace[1] == leftFace.PieceType;

            return result;
        }

        private bool IsAnyThirdLayerCubieInPlace()
        {
            var upFace = _model.GetFace(FaceType.Up);
            var frontFace = _model.GetFace(FaceType.Front);
            var rightFace = _model.GetFace(FaceType.Right);
            var backFace = _model.GetFace(FaceType.Back);
            var leftFace = _model.GetFace(FaceType.Left);

            var result1 = true;
            var result2 = true;
            var result3 = true;
            var result4 = true;

            result1 &= upFace[8] == frontFace.PieceType || upFace[8] == rightFace.PieceType || upFace[8] == upFace.PieceType;
            result1 &= frontFace[2] == upFace.PieceType || frontFace[2] == rightFace.PieceType || frontFace[2] == frontFace.PieceType;
            result1 &= rightFace[0] == upFace.PieceType || rightFace[0] == frontFace.PieceType || rightFace[0] == rightFace.PieceType;

            result2 &= upFace[6] == frontFace.PieceType || upFace[6] == leftFace.PieceType || upFace[6] == upFace.PieceType;
            result2 &= frontFace[0] == upFace.PieceType || frontFace[0] == leftFace.PieceType || frontFace[0] == frontFace.PieceType;
            result2 &= leftFace[2] == upFace.PieceType || leftFace[2] == frontFace.PieceType || leftFace[2] == leftFace.PieceType;

            result3 &= upFace[0] == backFace.PieceType || upFace[0] == leftFace.PieceType || upFace[0] == leftFace.PieceType;
            result3 &= backFace[2] == upFace.PieceType || backFace[2] == leftFace.PieceType || backFace[2] == leftFace.PieceType;
            result3 &= leftFace[0] == upFace.PieceType || leftFace[0] == backFace.PieceType || leftFace[0] == backFace.PieceType;

            result4 &= upFace[2] == backFace.PieceType || upFace[2] == rightFace.PieceType || upFace[2] == upFace.PieceType;
            result4 &= backFace[0] == upFace.PieceType || backFace[0] == rightFace.PieceType || backFace[0] == backFace.PieceType;
            result4 &= rightFace[2] == upFace.PieceType || rightFace[2] == backFace.PieceType || rightFace[2] == rightFace.PieceType;

            return result1 || result2 || result3 || result4;
        }

        private bool IsCubieInPlaceInTheTopRightCorner()
        {
            var upFace = _model.GetFace(FaceType.Up);
            var frontFace = _model.GetFace(FaceType.Front);
            var rightFace = _model.GetFace(FaceType.Right);
            var backFace = _model.GetFace(FaceType.Back);
            var leftFace = _model.GetFace(FaceType.Left);

            var result = true;

            result &= upFace[8] == frontFace.PieceType || upFace[8] == rightFace.PieceType || upFace[8] == upFace.PieceType;
            result &= frontFace[2] == upFace.PieceType || frontFace[2] == rightFace.PieceType || frontFace[2] == frontFace.PieceType;
            result &= rightFace[0] == upFace.PieceType || rightFace[0] == frontFace.PieceType || rightFace[0] == rightFace.PieceType;

            return result;
        }

        private bool AreThirdLayerCubiesInPlace()
        {
            var upFace = _model.GetFace(FaceType.Up);
            var frontFace = _model.GetFace(FaceType.Front);
            var rightFace = _model.GetFace(FaceType.Right);
            var backFace = _model.GetFace(FaceType.Back);
            var leftFace = _model.GetFace(FaceType.Left);

            var result1 = true;
            var result2 = true;
            var result3 = true;
            var result4 = true;

            result1 &= upFace[8] == frontFace.PieceType || upFace[8] == rightFace.PieceType || upFace[8] == upFace.PieceType;
            result1 &= frontFace[2] == upFace.PieceType || frontFace[2] == rightFace.PieceType || frontFace[2] == frontFace.PieceType;
            result1 &= rightFace[0] == upFace.PieceType || rightFace[0] == frontFace.PieceType || rightFace[0] == rightFace.PieceType;

            result2 &= upFace[6] == frontFace.PieceType || upFace[6] == leftFace.PieceType || upFace[6] == upFace.PieceType;
            result2 &= frontFace[0] == upFace.PieceType || frontFace[0] == leftFace.PieceType || frontFace[0] == frontFace.PieceType;
            result2 &= leftFace[2] == upFace.PieceType || leftFace[2] == frontFace.PieceType || leftFace[2] == leftFace.PieceType;

            result3 &= upFace[0] == backFace.PieceType || upFace[0] == leftFace.PieceType || upFace[0] == leftFace.PieceType;
            result3 &= backFace[2] == upFace.PieceType || backFace[2] == leftFace.PieceType || backFace[2] == leftFace.PieceType;
            result3 &= leftFace[0] == upFace.PieceType || leftFace[0] == backFace.PieceType || leftFace[0] == backFace.PieceType;

            result4 &= upFace[2] == backFace.PieceType || upFace[2] == rightFace.PieceType || upFace[2] == upFace.PieceType;
            result4 &= backFace[0] == upFace.PieceType || backFace[0] == rightFace.PieceType || backFace[0] == backFace.PieceType;
            result4 &= rightFace[2] == upFace.PieceType || rightFace[2] == backFace.PieceType || rightFace[2] == rightFace.PieceType;

            return result1 && result2 && result3 && result4;
        }

        private bool IsCubieInPlaceInTheTopLeftCorner()
        {
            var upFace = _model.GetFace(FaceType.Up);
            var frontFace = _model.GetFace(FaceType.Front);
            var leftFace = _model.GetFace(FaceType.Left);

            var result = true;

            result &= upFace[6] == frontFace.PieceType || upFace[6] == leftFace.PieceType || upFace[6] == upFace.PieceType;
            result &= frontFace[0] == upFace.PieceType || frontFace[0] == leftFace.PieceType || frontFace[0] == frontFace.PieceType;
            result &= leftFace[2] == upFace.PieceType || leftFace[2] == frontFace.PieceType || leftFace[2] == leftFace.PieceType;

            return result;
        }

        private bool IsCubieMatchInTheTopLeftCorner()
        {
            //var upFace = _model.GetFace(FaceType.Up);
            //var frontFace = _model.GetFace(FaceType.Front);
            var leftFace = _model.GetFace(FaceType.Left);

            var result = true;

            //result &= upFace[6] == upFace.PieceType;
            //result &= frontFace[0] == frontFace.PieceType;
            result &= leftFace[2] == leftFace.PieceType;

            return result;
        }

        private bool IsLeftLayerReady()
        {
            var leftFace = _model.GetFace(FaceType.Left);

            bool result = true;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    result &= leftFace[i, j] == leftFace.PieceType;

            var frontFace = _model.GetFace(FaceType.Front);
            var bottomFace = _model.GetFace(FaceType.Down);
            var backFace = _model.GetFace(FaceType.Back);
            var upFace = _model.GetFace(FaceType.Up);

            result &= frontFace[3] == frontFace[0] && frontFace[6] == frontFace[0];
            result &= bottomFace[3] == bottomFace[0] && bottomFace[6] == bottomFace[0];
            result &= backFace[5] == backFace[2] && backFace[8] == backFace[2];
            result &= upFace[1] == upFace[0] && upFace[2] == upFace[0];

            return result;
        }

        private bool IsLeftLayerInPlace()
        {
            var leftFace = _model.GetFace(FaceType.Left);

            bool result = true;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    result &= leftFace[i, j] == leftFace.PieceType;

            var frontFace = _model.GetFace(FaceType.Front);
            var bottomFace = _model.GetFace(FaceType.Down);
            var backFace = _model.GetFace(FaceType.Back);
            var upFace = _model.GetFace(FaceType.Up);

            result &= frontFace[0] == frontFace.PieceType;
            result &= frontFace[3] == frontFace.PieceType;
            result &= frontFace[6] == frontFace.PieceType;

            result &= bottomFace[0] == bottomFace.PieceType;
            result &= bottomFace[3] == bottomFace.PieceType;
            result &= bottomFace[6] == bottomFace.PieceType;

            result &= backFace[2] == backFace.PieceType;
            result &= backFace[5] == backFace.PieceType;
            result &= backFace[8] == backFace.PieceType;

            result &= upFace[0] == upFace.PieceType;
            result &= upFace[1] == upFace.PieceType;
            result &= upFace[2] == upFace.PieceType;

            return result;
        }

        #endregion
    }
}