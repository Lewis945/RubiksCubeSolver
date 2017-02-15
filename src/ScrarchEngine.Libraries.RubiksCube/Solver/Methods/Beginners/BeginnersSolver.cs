using ScrarchEngine.Libraries.RubiksCube.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrarchEngine.Libraries.RubiksCube.Solver.Methods.Beginners
{
    public class BeginnersSolver : BaseSolver
    {
        private RubiksCube.Models.RubiksCubeModel _model;

        private List<MoveAlgorithm> _algorithms;

        public BeginnersSolver(RubiksCube.Models.RubiksCubeModel model)
        {
            //AddMove("F", s => RotateFront(s, 1));
            _model = model;

            #region Algorithms

            _algorithms = new List<MoveAlgorithm>();

            var a1 = new MoveAlgorithm(Phase.FirstCross);
            // Default
            //a1.SetFaceState(FaceType.Top, new FacePieceType?[3, 3] {
            //    { FacePieceType.White, FacePieceType.White, FacePieceType.White },
            //    { FacePieceType.White, FacePieceType.White, FacePieceType.White },
            //    { FacePieceType.White, FacePieceType.White, FacePieceType.White }
            //});
            //a1.SetFaceState(FaceType.Bottom, new FacePieceType?[3, 3] {
            //    { FacePieceType.Yellow, FacePieceType.Yellow, FacePieceType.Yellow },
            //    { FacePieceType.Yellow, FacePieceType.Yellow, FacePieceType.Yellow },
            //    { FacePieceType.Yellow, FacePieceType.Yellow, FacePieceType.Yellow }
            //});
            //a1.SetFaceState(FaceType.Front, new FacePieceType?[3, 3] {
            //    { FacePieceType.Blue, FacePieceType.Blue, FacePieceType.Blue },
            //    { FacePieceType.Blue, FacePieceType.Blue, FacePieceType.Blue },
            //    { FacePieceType.Blue, FacePieceType.Blue, FacePieceType.Blue }
            //});
            //a1.SetFaceState(FaceType.Back, new FacePieceType?[3, 3] {
            //    { FacePieceType.Green, FacePieceType.Green, FacePieceType.Green },
            //    { FacePieceType.Green, FacePieceType.Green, FacePieceType.Green },
            //    { FacePieceType.Green, FacePieceType.Green, FacePieceType.Green }
            //});
            //a1.SetFaceState(FaceType.Left, new FacePieceType?[3, 3] {
            //    { FacePieceType.Orange, FacePieceType.Orange, FacePieceType.Orange },
            //    { FacePieceType.Orange, FacePieceType.Orange, FacePieceType.Orange },
            //    { FacePieceType.Orange, FacePieceType.Orange, FacePieceType.Orange }
            //});
            //a1.SetFaceState(FaceType.Right, new FacePieceType?[3, 3] {
            //    { FacePieceType.Red, FacePieceType.Red, FacePieceType.Red },
            //    { FacePieceType.Red, FacePieceType.Red, FacePieceType.Red },
            //    { FacePieceType.Red, FacePieceType.Red, FacePieceType.Red }
            //});
            // State
            a1.SetFaceState(FaceType.Top, new FacePieceType?[3, 3] { 
                { FacePieceType.White, FacePieceType.White, FacePieceType.White }, 
                { FacePieceType.White, FacePieceType.White, FacePieceType.White }, 
                { FacePieceType.White, FacePieceType.White, FacePieceType.White }
            });
            a1.SetFaceState(FaceType.Bottom, new FacePieceType?[3, 3] {
                { FacePieceType.Yellow, FacePieceType.Yellow, FacePieceType.Yellow },
                { FacePieceType.Yellow, FacePieceType.Yellow, FacePieceType.Yellow },
                { FacePieceType.Yellow, FacePieceType.Yellow, FacePieceType.Yellow }
            });
            a1.SetFaceState(FaceType.Front, new FacePieceType?[3, 3] {
                { FacePieceType.Blue, FacePieceType.Blue, FacePieceType.Blue },
                { FacePieceType.Blue, FacePieceType.Blue, FacePieceType.Blue },
                { FacePieceType.Blue, FacePieceType.Blue, FacePieceType.Blue }
            });
            a1.SetFaceState(FaceType.Back, new FacePieceType?[3, 3] {
                { FacePieceType.Green, FacePieceType.Green, FacePieceType.Green },
                { FacePieceType.Green, FacePieceType.Green, FacePieceType.Green },
                { FacePieceType.Green, FacePieceType.Green, FacePieceType.Green }
            });
            a1.SetFaceState(FaceType.Left, new FacePieceType?[3, 3] {
                { FacePieceType.Orange, FacePieceType.Orange, FacePieceType.Orange },
                { FacePieceType.Orange, FacePieceType.Orange, FacePieceType.Orange },
                { FacePieceType.Orange, FacePieceType.Orange, FacePieceType.Orange }
            });
            a1.SetFaceState(FaceType.Right, new FacePieceType?[3, 3] {
                { FacePieceType.Red, FacePieceType.Red, FacePieceType.Red },
                { FacePieceType.Red, FacePieceType.Red, FacePieceType.Red },
                { FacePieceType.Red, FacePieceType.Red, FacePieceType.Red }
            });
            // Moves
            a1.AddMove(new Move(LayerType.Top, RotationType.Clockwise, () => _model.Rotate90Degrees(LayerType.Top, RotationType.Clockwise)));
            a1.AddMove(new Move(LayerType.Top, RotationType.Clockwise, () => _model.Rotate90Degrees(LayerType.Top, RotationType.Clockwise)));
            a1.AddMove(new Move(LayerType.Top, RotationType.Clockwise, () => _model.Rotate90Degrees(LayerType.Top, RotationType.Clockwise)));
            a1.AddMove(new Move(LayerType.Top, RotationType.Clockwise, () => _model.Rotate90Degrees(LayerType.Top, RotationType.Clockwise)));
            _algorithms.Add(a1);

            #endregion
        }

        private void BuildCross()
        {
            var crossAlgorithms = _algorithms.Where(a => a.Phase == Phase.FirstCross);

            var cubiesInPlace = new List<Cubie>();

            var topCollor = _model.Faces.FirstOrDefault(f => f.Type == FaceType.Top)?.Field[1, 1];
            if (topCollor == null)
                throw new Exception("");

            var frontCollor = _model.Faces.FirstOrDefault(f => f.Type == FaceType.Front)?.Field[1, 1];
            if (frontCollor == null)
                throw new Exception("");

            var backCollor = _model.Faces.FirstOrDefault(f => f.Type == FaceType.Back)?.Field[1, 1];
            if (backCollor == null)
                throw new Exception("");

            var leftCollor = _model.Faces.FirstOrDefault(f => f.Type == FaceType.Left)?.Field[1, 1];
            if (leftCollor == null)
                throw new Exception("");

            var rigthCollor = _model.Faces.FirstOrDefault(f => f.Type == FaceType.Right)?.Field[1, 1];
            if (rigthCollor == null)
                throw new Exception("");

            while (true)
            {
                var cubies = _model.GetCubies();
                var cubieWithTopColor = cubies.FirstOrDefault(c => !cubies.Contains(c) && c.Pieces.Any(p => p.CurrentType.HasValue && p.CurrentType.Value == topCollor));
                if (!cubieWithTopColor.Equals(default(Cubie)))
                {
                    if (cubieWithTopColor.Layers.Contains(LayerType.Front) && cubieWithTopColor.Layers.Contains(LayerType.Top))
                    {
                        var topPiece = cubieWithTopColor.Pieces.FirstOrDefault(p => p.Face == FaceType.Top);
                        if (topPiece.CurrentType.Value != topCollor)
                        {
                            if (topPiece.CurrentType.Value == frontCollor)
                            {
                                _model.Rotate90Degrees(LayerType.Front, RotationType.CounterClockwise);
                                _model.Rotate90Degrees(LayerType.Left, RotationType.CounterClockwise);
                                _model.Rotate90Degrees(LayerType.Top, RotationType.CounterClockwise);
                                _model.Rotate90Degrees(LayerType.Left, RotationType.Clockwise);
                            }
                            else if (topPiece.CurrentType.Value == leftCollor)
                            {
                                _model.Rotate90Degrees(LayerType.Front, RotationType.CounterClockwise);
                                _model.Rotate90Degrees(LayerType.Left, RotationType.CounterClockwise);
                                _model.Rotate90Degrees(LayerType.Top, RotationType.CounterClockwise);
                                _model.Rotate90Degrees(LayerType.Left, RotationType.Clockwise);
                            }
                            else if (topPiece.CurrentType.Value == backCollor)
                            {
                                _model.Rotate90Degrees(LayerType.Front, RotationType.CounterClockwise);
                                _model.Rotate90Degrees(LayerType.Left, RotationType.CounterClockwise);
                                _model.Rotate90Degrees(LayerType.Top, RotationType.CounterClockwise);
                                _model.Rotate90Degrees(LayerType.Left, RotationType.Clockwise);
                            }
                            else if (topPiece.CurrentType.Value == rigthCollor)
                            {
                                _model.Rotate90Degrees(LayerType.Front, RotationType.CounterClockwise);
                                _model.Rotate90Degrees(LayerType.Left, RotationType.CounterClockwise);
                                _model.Rotate90Degrees(LayerType.Top, RotationType.CounterClockwise);
                                _model.Rotate90Degrees(LayerType.Left, RotationType.Clockwise);
                            }
                        }

                        cubiesInPlace.Add(cubieWithTopColor);
                    }
                }
            }
        }
    }
}