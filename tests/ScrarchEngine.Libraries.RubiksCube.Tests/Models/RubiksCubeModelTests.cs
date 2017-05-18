using NUnit.Framework;
using ScrarchEngine.Libraries.RubiksCube.Models;
using System;
using System.Text;

namespace ScrarchEngine.Libraries.RubiksCube.Tests.Models
{
    [TestFixture]
    public class RubiksCubeModelTests
    {
        #region Fields

        private RubiksCubeModel _cube;

        #endregion

        #region .ctor

        public RubiksCubeModelTests()
        {
        }

        #endregion

        #region SetUp

        [SetUp]
        public void Init()
        {
            _cube = new RubiksCubeModel();
        }

        [TearDown]
        public void Cleanup()
        {
        }

        #endregion

        #region Private methods

        private string GetColor(int index, FaceType type)
        {
            var face = _cube.GetFace(type);
            var color = face[index];
            switch (color)
            {
                case FacePieceType.Red:
                    return "R";
                case FacePieceType.Green:
                    return "G";
                case FacePieceType.Blue:
                    return "B";
                case FacePieceType.White:
                    return "W";
                case FacePieceType.Orange:
                    return "O";
                case FacePieceType.Yellow:
                    return "Y";
                default:
                    return "-";
            }
        }

        private void PrintCubeState()
        {
            //              | 0 | 1 | 2 | 
            //              | 3 | 4 | 5 |
            //              | 6 | 7 | 8 |
            //              -------------
            //| 0 | 1 | 2 |-| 0 | 1 | 2 |-| 0 | 1 | 2 |-| 0 | 1 | 2 |
            //| 3 | 4 | 5 |-| 3 | 4 | 5 |-| 3 | 4 | 5 |-| 3 | 4 | 5 |
            //| 6 | 7 | 8 |-| 6 | 7 | 8 |-| 6 | 7 | 8 |-| 6 | 7 | 8 |
            //              -------------
            //              | 0 | 1 | 2 |
            //              | 3 | 4 | 5 |
            //              | 6 | 7 | 8 |

            var sb = new StringBuilder();

            sb.AppendLine($"              | {GetColor(0, FaceType.Up)} | {GetColor(1, FaceType.Up)} | {GetColor(2, FaceType.Up)} |");
            sb.AppendLine($"              | {GetColor(3, FaceType.Up)} | {GetColor(4, FaceType.Up)} | {GetColor(5, FaceType.Up)} |");
            sb.AppendLine($"              | {GetColor(6, FaceType.Up)} | {GetColor(7, FaceType.Up)} | {GetColor(8, FaceType.Up)} |");
            sb.AppendLine($"              -------------");
            sb.AppendLine($"| {GetColor(0, FaceType.Left)} | {GetColor(1, FaceType.Left)} | {GetColor(2, FaceType.Left)} |-| {GetColor(0, FaceType.Front)} | {GetColor(1, FaceType.Front)} | {GetColor(2, FaceType.Front)} |-| {GetColor(0, FaceType.Right)} | {GetColor(1, FaceType.Right)} | {GetColor(2, FaceType.Right)} |-| {GetColor(0, FaceType.Back)} | {GetColor(1, FaceType.Back)} | {GetColor(2, FaceType.Back)} |");
            sb.AppendLine($"| {GetColor(3, FaceType.Left)} | {GetColor(4, FaceType.Left)} | {GetColor(5, FaceType.Left)} |-| {GetColor(3, FaceType.Front)} | {GetColor(4, FaceType.Front)} | {GetColor(5, FaceType.Front)} |-| {GetColor(3, FaceType.Right)} | {GetColor(4, FaceType.Right)} | {GetColor(5, FaceType.Right)} |-| {GetColor(3, FaceType.Back)} | {GetColor(4, FaceType.Back)} | {GetColor(5, FaceType.Back)} |");
            sb.AppendLine($"| {GetColor(6, FaceType.Left)} | {GetColor(7, FaceType.Left)} | {GetColor(8, FaceType.Left)} |-| {GetColor(6, FaceType.Front)} | {GetColor(7, FaceType.Front)} | {GetColor(8, FaceType.Front)} |-| {GetColor(6, FaceType.Right)} | {GetColor(7, FaceType.Right)} | {GetColor(8, FaceType.Right)} |-| {GetColor(6, FaceType.Back)} | {GetColor(7, FaceType.Back)} | {GetColor(8, FaceType.Back)} |");
            sb.AppendLine($"              -------------");
            sb.AppendLine($"              | {GetColor(0, FaceType.Down)} | {GetColor(1, FaceType.Down)} | {GetColor(2, FaceType.Down)} |");
            sb.AppendLine($"              | {GetColor(3, FaceType.Down)} | {GetColor(4, FaceType.Down)} | {GetColor(5, FaceType.Down)} |");
            sb.AppendLine($"              | {GetColor(6, FaceType.Down)} | {GetColor(7, FaceType.Down)} | {GetColor(8, FaceType.Down)} |");

            Console.WriteLine(sb.ToString());
        }

        #endregion

        #region Default tests

        [Test]
        public void RubiksCube_Default_Setup()
        {
            Assert.AreEqual(6, _cube.Faces.Length);
            Assert.AreEqual(FaceType.Front, _cube.Faces[0].Type);
            Assert.AreEqual(FaceType.Left, _cube.Faces[1].Type);
            Assert.AreEqual(FaceType.Back, _cube.Faces[2].Type);
            Assert.AreEqual(FaceType.Right, _cube.Faces[3].Type);
            Assert.AreEqual(FaceType.Up, _cube.Faces[4].Type);
            Assert.AreEqual(FaceType.Down, _cube.Faces[5].Type);

            foreach (var face in _cube.Faces)
            {
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                        Assert.AreEqual(Face.FacePieceTypeMap[face.Type], face[i, j]);
            }
        }

        #endregion

        #region Top Layer

        [Test]
        public void Rotate_Top_Layer_90_Degrees_Clockwise()
        {
            _cube.Rotate90Degrees(LayerType.Top, RotationType.Clockwise);

            var frontFace = _cube.GetFace(FaceType.Front);
            var leftFace = _cube.GetFace(FaceType.Left);
            var backFace = _cube.GetFace(FaceType.Back);
            var rightFace = _cube.GetFace(FaceType.Right);

            PrintCubeState();

            // rotated
            Assert.AreEqual(FacePieceType.Orange, frontFace[0]);
            Assert.AreEqual(FacePieceType.Orange, frontFace[1]);
            Assert.AreEqual(FacePieceType.Orange, frontFace[2]);

            // unchanged
            Assert.AreEqual(FacePieceType.Blue, frontFace[3]);
            Assert.AreEqual(FacePieceType.Blue, frontFace[4]);
            Assert.AreEqual(FacePieceType.Blue, frontFace[5]);
            Assert.AreEqual(FacePieceType.Blue, frontFace[6]);
            Assert.AreEqual(FacePieceType.Blue, frontFace[7]);
            Assert.AreEqual(FacePieceType.Blue, frontFace[8]);

            // rotated
            Assert.AreEqual(FacePieceType.Blue, leftFace[0]);
            Assert.AreEqual(FacePieceType.Blue, leftFace[1]);
            Assert.AreEqual(FacePieceType.Blue, leftFace[2]);

            // unchanged
            Assert.AreEqual(FacePieceType.Red, leftFace[3]);
            Assert.AreEqual(FacePieceType.Red, leftFace[4]);
            Assert.AreEqual(FacePieceType.Red, leftFace[5]);
            Assert.AreEqual(FacePieceType.Red, leftFace[6]);
            Assert.AreEqual(FacePieceType.Red, leftFace[7]);
            Assert.AreEqual(FacePieceType.Red, leftFace[8]);

            // rotated
            Assert.AreEqual(FacePieceType.Red, backFace[0]);
            Assert.AreEqual(FacePieceType.Red, backFace[1]);
            Assert.AreEqual(FacePieceType.Red, backFace[2]);

            // unchanged
            Assert.AreEqual(FacePieceType.Green, backFace[3]);
            Assert.AreEqual(FacePieceType.Green, backFace[4]);
            Assert.AreEqual(FacePieceType.Green, backFace[5]);
            Assert.AreEqual(FacePieceType.Green, backFace[6]);
            Assert.AreEqual(FacePieceType.Green, backFace[7]);
            Assert.AreEqual(FacePieceType.Green, backFace[8]);

            // rotated
            Assert.AreEqual(FacePieceType.Green, rightFace[0]);
            Assert.AreEqual(FacePieceType.Green, rightFace[1]);
            Assert.AreEqual(FacePieceType.Green, rightFace[2]);

            // unchanged
            Assert.AreEqual(FacePieceType.Orange, rightFace[3]);
            Assert.AreEqual(FacePieceType.Orange, rightFace[4]);
            Assert.AreEqual(FacePieceType.Orange, rightFace[5]);
            Assert.AreEqual(FacePieceType.Orange, rightFace[6]);
            Assert.AreEqual(FacePieceType.Orange, rightFace[7]);
            Assert.AreEqual(FacePieceType.Orange, rightFace[8]);
        }

        [Test]
        public void Rotate_Top_Layer_90_Degrees_CounterClockwise()
        {
            _cube.Rotate90Degrees(LayerType.Top, RotationType.CounterClockwise);

            var frontFace = _cube.GetFace(FaceType.Front);
            var leftFace = _cube.GetFace(FaceType.Left);
            var backFace = _cube.GetFace(FaceType.Back);
            var rightFace = _cube.GetFace(FaceType.Right);

            // rotated
            Assert.AreEqual(FacePieceType.Red, frontFace[0]);
            Assert.AreEqual(FacePieceType.Red, frontFace[1]);
            Assert.AreEqual(FacePieceType.Red, frontFace[2]);

            // unchanged
            Assert.AreEqual(FacePieceType.Blue, frontFace[3]);
            Assert.AreEqual(FacePieceType.Blue, frontFace[4]);
            Assert.AreEqual(FacePieceType.Blue, frontFace[5]);
            Assert.AreEqual(FacePieceType.Blue, frontFace[6]);
            Assert.AreEqual(FacePieceType.Blue, frontFace[7]);
            Assert.AreEqual(FacePieceType.Blue, frontFace[8]);

            // rotated
            Assert.AreEqual(FacePieceType.Green, leftFace[0]);
            Assert.AreEqual(FacePieceType.Green, leftFace[1]);
            Assert.AreEqual(FacePieceType.Green, leftFace[2]);

            // unchanged
            Assert.AreEqual(FacePieceType.Red, leftFace[3]);
            Assert.AreEqual(FacePieceType.Red, leftFace[4]);
            Assert.AreEqual(FacePieceType.Red, leftFace[5]);
            Assert.AreEqual(FacePieceType.Red, leftFace[6]);
            Assert.AreEqual(FacePieceType.Red, leftFace[7]);
            Assert.AreEqual(FacePieceType.Red, leftFace[8]);

            // rotated
            Assert.AreEqual(FacePieceType.Orange, backFace[0]);
            Assert.AreEqual(FacePieceType.Orange, backFace[1]);
            Assert.AreEqual(FacePieceType.Orange, backFace[2]);

            // unchanged
            Assert.AreEqual(FacePieceType.Green, backFace[3]);
            Assert.AreEqual(FacePieceType.Green, backFace[4]);
            Assert.AreEqual(FacePieceType.Green, backFace[5]);
            Assert.AreEqual(FacePieceType.Green, backFace[6]);
            Assert.AreEqual(FacePieceType.Green, backFace[7]);
            Assert.AreEqual(FacePieceType.Green, backFace[8]);

            // rotated
            Assert.AreEqual(FacePieceType.Blue, rightFace[0]);
            Assert.AreEqual(FacePieceType.Blue, rightFace[1]);
            Assert.AreEqual(FacePieceType.Blue, rightFace[2]);

            // unchanged
            Assert.AreEqual(FacePieceType.Orange, rightFace[3]);
            Assert.AreEqual(FacePieceType.Orange, rightFace[4]);
            Assert.AreEqual(FacePieceType.Orange, rightFace[5]);
            Assert.AreEqual(FacePieceType.Orange, rightFace[6]);
            Assert.AreEqual(FacePieceType.Orange, rightFace[7]);
            Assert.AreEqual(FacePieceType.Orange, rightFace[8]);
        }

        #endregion

        #region Bottom Layer

        [Test]
        public void Rotate_Bottom_Layer_90_Degrees_Clockwise()
        {
            _cube.Rotate90Degrees(LayerType.Bottom, RotationType.Clockwise);

            var frontFace = _cube.GetFace(FaceType.Front);
            var leftFace = _cube.GetFace(FaceType.Left);
            var backFace = _cube.GetFace(FaceType.Back);
            var rightFace = _cube.GetFace(FaceType.Right);

            // rotated
            Assert.AreEqual(FacePieceType.Red, frontFace[6]);
            Assert.AreEqual(FacePieceType.Red, frontFace[7]);
            Assert.AreEqual(FacePieceType.Red, frontFace[8]);

            // rotated
            Assert.AreEqual(FacePieceType.Green, leftFace[6]);
            Assert.AreEqual(FacePieceType.Green, leftFace[7]);
            Assert.AreEqual(FacePieceType.Green, leftFace[8]);

            // rotated
            Assert.AreEqual(FacePieceType.Orange, backFace[6]);
            Assert.AreEqual(FacePieceType.Orange, backFace[7]);
            Assert.AreEqual(FacePieceType.Orange, backFace[8]);

            // rotated
            Assert.AreEqual(FacePieceType.Blue, rightFace[6]);
            Assert.AreEqual(FacePieceType.Blue, rightFace[7]);
            Assert.AreEqual(FacePieceType.Blue, rightFace[8]);
        }

        [Test]
        public void Rotate_Bottom_Layer_90_Degrees_CounterClockwise()
        {
            _cube.Rotate90Degrees(LayerType.Bottom, RotationType.CounterClockwise);

            var frontFace = _cube.GetFace(FaceType.Front);
            var leftFace = _cube.GetFace(FaceType.Left);
            var backFace = _cube.GetFace(FaceType.Back);
            var rightFace = _cube.GetFace(FaceType.Right);

            // rotated
            Assert.AreEqual(FacePieceType.Orange, frontFace[6]);
            Assert.AreEqual(FacePieceType.Orange, frontFace[7]);
            Assert.AreEqual(FacePieceType.Orange, frontFace[8]);

            // rotated
            Assert.AreEqual(FacePieceType.Blue, leftFace[6]);
            Assert.AreEqual(FacePieceType.Blue, leftFace[7]);
            Assert.AreEqual(FacePieceType.Blue, leftFace[8]);

            // rotated
            Assert.AreEqual(FacePieceType.Red, backFace[6]);
            Assert.AreEqual(FacePieceType.Red, backFace[7]);
            Assert.AreEqual(FacePieceType.Red, backFace[8]);

            // rotated
            Assert.AreEqual(FacePieceType.Green, rightFace[6]);
            Assert.AreEqual(FacePieceType.Green, rightFace[7]);
            Assert.AreEqual(FacePieceType.Green, rightFace[8]);
        }

        #endregion

        #region Left Layer

        [Test]
        public void Rotate_Left_Layer_90_Degrees_Clockwise()
        {
            _cube.Rotate90Degrees(LayerType.Left, RotationType.Clockwise);

            var frontFace = _cube.GetFace(FaceType.Front);
            var topFace = _cube.GetFace(FaceType.Up);
            var backFace = _cube.GetFace(FaceType.Back);
            var bottomFace = _cube.GetFace(FaceType.Down);

            // rotated
            Assert.AreEqual(FacePieceType.White, frontFace[0]);
            Assert.AreEqual(FacePieceType.White, frontFace[3]);
            Assert.AreEqual(FacePieceType.White, frontFace[6]);

            // rotated
            Assert.AreEqual(FacePieceType.Green, topFace[0]);
            Assert.AreEqual(FacePieceType.Green, topFace[3]);
            Assert.AreEqual(FacePieceType.Green, topFace[6]);

            // rotated
            Assert.AreEqual(FacePieceType.Yellow, backFace[2]);
            Assert.AreEqual(FacePieceType.Yellow, backFace[5]);
            Assert.AreEqual(FacePieceType.Yellow, backFace[8]);

            // rotated
            Assert.AreEqual(FacePieceType.Blue, bottomFace[0]);
            Assert.AreEqual(FacePieceType.Blue, bottomFace[3]);
            Assert.AreEqual(FacePieceType.Blue, bottomFace[6]);
        }

        [Test]
        public void Rotate_Left_Layer_90_Degrees_CounterClockwise()
        {
            _cube.Rotate90Degrees(LayerType.Left, RotationType.CounterClockwise);

            var frontFace = _cube.GetFace(FaceType.Front);
            var topFace = _cube.GetFace(FaceType.Up);
            var backFace = _cube.GetFace(FaceType.Back);
            var bottomFace = _cube.GetFace(FaceType.Down);

            // rotated
            Assert.AreEqual(FacePieceType.Yellow, frontFace[0]);
            Assert.AreEqual(FacePieceType.Yellow, frontFace[3]);
            Assert.AreEqual(FacePieceType.Yellow, frontFace[6]);

            // rotated
            Assert.AreEqual(FacePieceType.Blue, topFace[0]);
            Assert.AreEqual(FacePieceType.Blue, topFace[3]);
            Assert.AreEqual(FacePieceType.Blue, topFace[6]);

            // rotated
            Assert.AreEqual(FacePieceType.White, backFace[2]);
            Assert.AreEqual(FacePieceType.White, backFace[5]);
            Assert.AreEqual(FacePieceType.White, backFace[8]);

            // rotated
            Assert.AreEqual(FacePieceType.Green, bottomFace[0]);
            Assert.AreEqual(FacePieceType.Green, bottomFace[3]);
            Assert.AreEqual(FacePieceType.Green, bottomFace[6]);
        }

        #endregion

        #region Right Layer

        [Test]
        public void Rotate_Right_Layer_90_Degrees_Clockwise()
        {
            _cube.Rotate90Degrees(LayerType.Right, RotationType.Clockwise);

            var frontFace = _cube.GetFace(FaceType.Front);
            var topFace = _cube.GetFace(FaceType.Up);
            var backFace = _cube.GetFace(FaceType.Back);
            var bottomFace = _cube.GetFace(FaceType.Down);

            // rotated
            Assert.AreEqual(FacePieceType.Yellow, frontFace[2]);
            Assert.AreEqual(FacePieceType.Yellow, frontFace[5]);
            Assert.AreEqual(FacePieceType.Yellow, frontFace[8]);

            // rotated
            Assert.AreEqual(FacePieceType.Blue, topFace[2]);
            Assert.AreEqual(FacePieceType.Blue, topFace[5]);
            Assert.AreEqual(FacePieceType.Blue, topFace[8]);

            // rotated
            Assert.AreEqual(FacePieceType.White, backFace[0]);
            Assert.AreEqual(FacePieceType.White, backFace[3]);
            Assert.AreEqual(FacePieceType.White, backFace[6]);

            // rotated
            Assert.AreEqual(FacePieceType.Green, bottomFace[2]);
            Assert.AreEqual(FacePieceType.Green, bottomFace[5]);
            Assert.AreEqual(FacePieceType.Green, bottomFace[8]);
        }

        [Test]
        public void Rotate_Right_Layer_90_Degrees_CounterClockwise()
        {
            _cube.Rotate90Degrees(LayerType.Right, RotationType.CounterClockwise);

            var frontFace = _cube.GetFace(FaceType.Front);
            var topFace = _cube.GetFace(FaceType.Up);
            var backFace = _cube.GetFace(FaceType.Back);
            var bottomFace = _cube.GetFace(FaceType.Down);

            // rotated
            Assert.AreEqual(FacePieceType.White, frontFace[2]);
            Assert.AreEqual(FacePieceType.White, frontFace[5]);
            Assert.AreEqual(FacePieceType.White, frontFace[8]);

            // rotated
            Assert.AreEqual(FacePieceType.Green, topFace[2]);
            Assert.AreEqual(FacePieceType.Green, topFace[5]);
            Assert.AreEqual(FacePieceType.Green, topFace[8]);

            // rotated
            Assert.AreEqual(FacePieceType.Yellow, backFace[0]);
            Assert.AreEqual(FacePieceType.Yellow, backFace[3]);
            Assert.AreEqual(FacePieceType.Yellow, backFace[6]);

            // rotated
            Assert.AreEqual(FacePieceType.Blue, bottomFace[2]);
            Assert.AreEqual(FacePieceType.Blue, bottomFace[5]);
            Assert.AreEqual(FacePieceType.Blue, bottomFace[8]);
        }

        #endregion

        #region Front Layer

        [Test]
        public void Rotate_Front_Layer_90_Degrees_Clockwise()
        {
            _cube.Rotate90Degrees(LayerType.Front, RotationType.Clockwise);

            var topFace = _cube.GetFace(FaceType.Up);
            var leftFace = _cube.GetFace(FaceType.Left);
            var bootomFace = _cube.GetFace(FaceType.Down);
            var rightFace = _cube.GetFace(FaceType.Right);

            // rotated
            Assert.AreEqual(FacePieceType.Red, topFace[6]);
            Assert.AreEqual(FacePieceType.Red, topFace[7]);
            Assert.AreEqual(FacePieceType.Red, topFace[8]);

            // rotated
            Assert.AreEqual(FacePieceType.Yellow, leftFace[2]);
            Assert.AreEqual(FacePieceType.Yellow, leftFace[5]);
            Assert.AreEqual(FacePieceType.Yellow, leftFace[8]);

            // rotated
            Assert.AreEqual(FacePieceType.Orange, bootomFace[0]);
            Assert.AreEqual(FacePieceType.Orange, bootomFace[1]);
            Assert.AreEqual(FacePieceType.Orange, bootomFace[2]);

            // rotated
            Assert.AreEqual(FacePieceType.White, rightFace[0]);
            Assert.AreEqual(FacePieceType.White, rightFace[3]);
            Assert.AreEqual(FacePieceType.White, rightFace[6]);
        }

        [Test]
        public void Rotate_Front_Layer_90_Degrees_CounterClockwise()
        {
            _cube.Rotate90Degrees(LayerType.Front, RotationType.CounterClockwise);

            var topFace = _cube.GetFace(FaceType.Up);
            var leftFace = _cube.GetFace(FaceType.Left);
            var bootomFace = _cube.GetFace(FaceType.Down);
            var rightFace = _cube.GetFace(FaceType.Right);

            // rotated
            Assert.AreEqual(FacePieceType.Orange, topFace[6]);
            Assert.AreEqual(FacePieceType.Orange, topFace[7]);
            Assert.AreEqual(FacePieceType.Orange, topFace[8]);

            // rotated
            Assert.AreEqual(FacePieceType.White, leftFace[2]);
            Assert.AreEqual(FacePieceType.White, leftFace[5]);
            Assert.AreEqual(FacePieceType.White, leftFace[8]);

            // rotated
            Assert.AreEqual(FacePieceType.Red, bootomFace[0]);
            Assert.AreEqual(FacePieceType.Red, bootomFace[1]);
            Assert.AreEqual(FacePieceType.Red, bootomFace[2]);

            // rotated
            Assert.AreEqual(FacePieceType.Yellow, rightFace[0]);
            Assert.AreEqual(FacePieceType.Yellow, rightFace[3]);
            Assert.AreEqual(FacePieceType.Yellow, rightFace[6]);
        }

        #endregion

        #region Back Layer

        [Test]
        public void Rotate_Back_Layer_90_Degrees_Clockwise()
        {
            _cube.Rotate90Degrees(LayerType.Back, RotationType.Clockwise);

            var topFace = _cube.GetFace(FaceType.Up);
            var leftFace = _cube.GetFace(FaceType.Left);
            var bootomFace = _cube.GetFace(FaceType.Down);
            var rightFace = _cube.GetFace(FaceType.Right);

            // rotated
            Assert.AreEqual(FacePieceType.Orange, topFace[0]);
            Assert.AreEqual(FacePieceType.Orange, topFace[1]);
            Assert.AreEqual(FacePieceType.Orange, topFace[2]);

            // rotated
            Assert.AreEqual(FacePieceType.White, leftFace[0]);
            Assert.AreEqual(FacePieceType.White, leftFace[3]);
            Assert.AreEqual(FacePieceType.White, leftFace[6]);

            // rotated
            Assert.AreEqual(FacePieceType.Red, bootomFace[6]);
            Assert.AreEqual(FacePieceType.Red, bootomFace[7]);
            Assert.AreEqual(FacePieceType.Red, bootomFace[8]);

            // rotated
            Assert.AreEqual(FacePieceType.Yellow, rightFace[2]);
            Assert.AreEqual(FacePieceType.Yellow, rightFace[5]);
            Assert.AreEqual(FacePieceType.Yellow, rightFace[8]);
        }

        [Test]
        public void Rotate_Back_Layer_90_Degrees_CounterClockwise()
        {
            _cube.Rotate90Degrees(LayerType.Back, RotationType.CounterClockwise);

            var topFace = _cube.GetFace(FaceType.Up);
            var leftFace = _cube.GetFace(FaceType.Left);
            var bootomFace = _cube.GetFace(FaceType.Down);
            var rightFace = _cube.GetFace(FaceType.Right);

            // rotated
            Assert.AreEqual(FacePieceType.Red, topFace[0]);
            Assert.AreEqual(FacePieceType.Red, topFace[1]);
            Assert.AreEqual(FacePieceType.Red, topFace[2]);

            // rotated
            Assert.AreEqual(FacePieceType.Yellow, leftFace[0]);
            Assert.AreEqual(FacePieceType.Yellow, leftFace[3]);
            Assert.AreEqual(FacePieceType.Yellow, leftFace[6]);

            // rotated
            Assert.AreEqual(FacePieceType.Orange, bootomFace[6]);
            Assert.AreEqual(FacePieceType.Orange, bootomFace[7]);
            Assert.AreEqual(FacePieceType.Orange, bootomFace[8]);

            // rotated
            Assert.AreEqual(FacePieceType.White, rightFace[2]);
            Assert.AreEqual(FacePieceType.White, rightFace[5]);
            Assert.AreEqual(FacePieceType.White, rightFace[8]);
        }

        #endregion

        #region Shuffle

        [Test]
        public void Shuffle()
        {
            _cube.Shuffle();
        }

        #endregion
    }
}