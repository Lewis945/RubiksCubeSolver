using NUnit.Framework;
using ScrarchEngine.Libraries.RubiksCube.Models;

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

        #region Default tests

        [Test]
        public void RubiksCube_Default_Setup()
        {
            Assert.AreEqual(6, _cube.Faces.Length);
            Assert.AreEqual(FaceType.Front, _cube.Faces[0].Type);
            Assert.AreEqual(FaceType.Left, _cube.Faces[1].Type);
            Assert.AreEqual(FaceType.Back, _cube.Faces[2].Type);
            Assert.AreEqual(FaceType.Right, _cube.Faces[3].Type);
            Assert.AreEqual(FaceType.Top, _cube.Faces[4].Type);
            Assert.AreEqual(FaceType.Bottom, _cube.Faces[5].Type);

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

            // rotated
            Assert.AreEqual(FacePieceType.Orange, frontFace[0, 0]);
            Assert.AreEqual(FacePieceType.Orange, frontFace[1, 0]);
            Assert.AreEqual(FacePieceType.Orange, frontFace[2, 0]);

            // unchanged
            Assert.AreEqual(FacePieceType.Blue, frontFace[0, 1]);
            Assert.AreEqual(FacePieceType.Blue, frontFace[1, 1]);
            Assert.AreEqual(FacePieceType.Blue, frontFace[2, 1]);
            Assert.AreEqual(FacePieceType.Blue, frontFace[0, 2]);
            Assert.AreEqual(FacePieceType.Blue, frontFace[1, 2]);
            Assert.AreEqual(FacePieceType.Blue, frontFace[2, 2]);

            // rotated
            Assert.AreEqual(FacePieceType.Blue, leftFace[0, 0]);
            Assert.AreEqual(FacePieceType.Blue, leftFace[1, 0]);
            Assert.AreEqual(FacePieceType.Blue, leftFace[2, 0]);

            // unchanged
            Assert.AreEqual(FacePieceType.Red, leftFace[0, 1]);
            Assert.AreEqual(FacePieceType.Red, leftFace[1, 1]);
            Assert.AreEqual(FacePieceType.Red, leftFace[2, 1]);
            Assert.AreEqual(FacePieceType.Red, leftFace[0, 2]);
            Assert.AreEqual(FacePieceType.Red, leftFace[1, 2]);
            Assert.AreEqual(FacePieceType.Red, leftFace[2, 2]);

            // rotated
            Assert.AreEqual(FacePieceType.Red, backFace[0, 0]);
            Assert.AreEqual(FacePieceType.Red, backFace[1, 0]);
            Assert.AreEqual(FacePieceType.Red, backFace[2, 0]);

            // unchanged
            Assert.AreEqual(FacePieceType.Green, backFace[0, 1]);
            Assert.AreEqual(FacePieceType.Green, backFace[1, 1]);
            Assert.AreEqual(FacePieceType.Green, backFace[2, 1]);
            Assert.AreEqual(FacePieceType.Green, backFace[0, 2]);
            Assert.AreEqual(FacePieceType.Green, backFace[1, 2]);
            Assert.AreEqual(FacePieceType.Green, backFace[2, 2]);

            // rotated
            Assert.AreEqual(FacePieceType.Green, rightFace[0, 0]);
            Assert.AreEqual(FacePieceType.Green, rightFace[1, 0]);
            Assert.AreEqual(FacePieceType.Green, rightFace[2, 0]);

            // unchanged
            Assert.AreEqual(FacePieceType.Orange, rightFace[0, 1]);
            Assert.AreEqual(FacePieceType.Orange, rightFace[1, 1]);
            Assert.AreEqual(FacePieceType.Orange, rightFace[2, 1]);
            Assert.AreEqual(FacePieceType.Orange, rightFace[0, 1]);
            Assert.AreEqual(FacePieceType.Orange, rightFace[1, 1]);
            Assert.AreEqual(FacePieceType.Orange, rightFace[2, 1]);
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
            Assert.AreEqual(FacePieceType.Red, frontFace[0, 0]);
            Assert.AreEqual(FacePieceType.Red, frontFace[1, 0]);
            Assert.AreEqual(FacePieceType.Red, frontFace[2, 0]);

            // unchanged
            Assert.AreEqual(FacePieceType.Blue, frontFace[0, 1]);
            Assert.AreEqual(FacePieceType.Blue, frontFace[1, 1]);
            Assert.AreEqual(FacePieceType.Blue, frontFace[2, 1]);
            Assert.AreEqual(FacePieceType.Blue, frontFace[0, 2]);
            Assert.AreEqual(FacePieceType.Blue, frontFace[1, 2]);
            Assert.AreEqual(FacePieceType.Blue, frontFace[2, 2]);

            // rotated
            Assert.AreEqual(FacePieceType.Green, leftFace[0, 0]);
            Assert.AreEqual(FacePieceType.Green, leftFace[1, 0]);
            Assert.AreEqual(FacePieceType.Green, leftFace[2, 0]);

            // unchanged
            Assert.AreEqual(FacePieceType.Red, leftFace[0, 1]);
            Assert.AreEqual(FacePieceType.Red, leftFace[1, 1]);
            Assert.AreEqual(FacePieceType.Red, leftFace[2, 1]);
            Assert.AreEqual(FacePieceType.Red, leftFace[0, 2]);
            Assert.AreEqual(FacePieceType.Red, leftFace[1, 2]);
            Assert.AreEqual(FacePieceType.Red, leftFace[2, 2]);

            // rotated
            Assert.AreEqual(FacePieceType.Orange, backFace[0, 0]);
            Assert.AreEqual(FacePieceType.Orange, backFace[1, 0]);
            Assert.AreEqual(FacePieceType.Orange, backFace[2, 0]);

            // unchanged
            Assert.AreEqual(FacePieceType.Green, backFace[0, 1]);
            Assert.AreEqual(FacePieceType.Green, backFace[1, 1]);
            Assert.AreEqual(FacePieceType.Green, backFace[2, 1]);
            Assert.AreEqual(FacePieceType.Green, backFace[0, 2]);
            Assert.AreEqual(FacePieceType.Green, backFace[1, 2]);
            Assert.AreEqual(FacePieceType.Green, backFace[2, 2]);

            // rotated
            Assert.AreEqual(FacePieceType.Blue, rightFace[0, 0]);
            Assert.AreEqual(FacePieceType.Blue, rightFace[1, 0]);
            Assert.AreEqual(FacePieceType.Blue, rightFace[2, 0]);

            // unchanged
            Assert.AreEqual(FacePieceType.Orange, rightFace[0, 1]);
            Assert.AreEqual(FacePieceType.Orange, rightFace[1, 1]);
            Assert.AreEqual(FacePieceType.Orange, rightFace[2, 1]);
            Assert.AreEqual(FacePieceType.Orange, rightFace[0, 1]);
            Assert.AreEqual(FacePieceType.Orange, rightFace[1, 1]);
            Assert.AreEqual(FacePieceType.Orange, rightFace[2, 1]);
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
            Assert.AreEqual(FacePieceType.Red, frontFace[0, 2]);
            Assert.AreEqual(FacePieceType.Red, frontFace[1, 2]);
            Assert.AreEqual(FacePieceType.Red, frontFace[2, 2]);

            // rotated
            Assert.AreEqual(FacePieceType.Green, leftFace[0, 2]);
            Assert.AreEqual(FacePieceType.Green, leftFace[1, 2]);
            Assert.AreEqual(FacePieceType.Green, leftFace[2, 2]);

            // rotated
            Assert.AreEqual(FacePieceType.Orange, backFace[0, 2]);
            Assert.AreEqual(FacePieceType.Orange, backFace[1, 2]);
            Assert.AreEqual(FacePieceType.Orange, backFace[2, 2]);

            // rotated
            Assert.AreEqual(FacePieceType.Blue, rightFace[0, 2]);
            Assert.AreEqual(FacePieceType.Blue, rightFace[1, 2]);
            Assert.AreEqual(FacePieceType.Blue, rightFace[2, 2]);
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
            Assert.AreEqual(FacePieceType.Orange, frontFace[0, 2]);
            Assert.AreEqual(FacePieceType.Orange, frontFace[1, 2]);
            Assert.AreEqual(FacePieceType.Orange, frontFace[2, 2]);

            // rotated
            Assert.AreEqual(FacePieceType.Blue, leftFace[0, 2]);
            Assert.AreEqual(FacePieceType.Blue, leftFace[1, 2]);
            Assert.AreEqual(FacePieceType.Blue, leftFace[2, 2]);

            // rotated
            Assert.AreEqual(FacePieceType.Red, backFace[0, 2]);
            Assert.AreEqual(FacePieceType.Red, backFace[1, 2]);
            Assert.AreEqual(FacePieceType.Red, backFace[2, 2]);

            // rotated
            Assert.AreEqual(FacePieceType.Green, rightFace[0, 2]);
            Assert.AreEqual(FacePieceType.Green, rightFace[1, 2]);
            Assert.AreEqual(FacePieceType.Green, rightFace[2, 2]);
        }

        #endregion

        #region Left Layer

        [Test]
        public void Rotate_Left_Layer_90_Degrees_Clockwise()
        {
            _cube.Rotate90Degrees(LayerType.Left, RotationType.Clockwise);

            var frontFace = _cube.GetFace(FaceType.Front);
            var topFace = _cube.GetFace(FaceType.Top);
            var backFace = _cube.GetFace(FaceType.Back);
            var bottomFace = _cube.GetFace(FaceType.Bottom);

            // rotated
            Assert.AreEqual(FacePieceType.White, frontFace[0, 0]);
            Assert.AreEqual(FacePieceType.White, frontFace[0, 1]);
            Assert.AreEqual(FacePieceType.White, frontFace[0, 2]);

            // rotated
            Assert.AreEqual(FacePieceType.Green, topFace[0, 0]);
            Assert.AreEqual(FacePieceType.Green, topFace[0, 1]);
            Assert.AreEqual(FacePieceType.Green, topFace[0, 2]);

            // rotated
            Assert.AreEqual(FacePieceType.Yellow, backFace[2, 0]);
            Assert.AreEqual(FacePieceType.Yellow, backFace[2, 1]);
            Assert.AreEqual(FacePieceType.Yellow, backFace[2, 2]);

            // rotated
            Assert.AreEqual(FacePieceType.Blue, bottomFace[0, 0]);
            Assert.AreEqual(FacePieceType.Blue, bottomFace[0, 1]);
            Assert.AreEqual(FacePieceType.Blue, bottomFace[0, 2]);
        }

        [Test]
        public void Rotate_Left_Layer_90_Degrees_CounterClockwise()
        {
            _cube.Rotate90Degrees(LayerType.Left, RotationType.CounterClockwise);

            var frontFace = _cube.GetFace(FaceType.Front);
            var topFace = _cube.GetFace(FaceType.Top);
            var backFace = _cube.GetFace(FaceType.Back);
            var bottomFace = _cube.GetFace(FaceType.Bottom);

            // rotated
            Assert.AreEqual(FacePieceType.Yellow, frontFace[0, 0]);
            Assert.AreEqual(FacePieceType.Yellow, frontFace[0, 1]);
            Assert.AreEqual(FacePieceType.Yellow, frontFace[0, 2]);

            // rotated
            Assert.AreEqual(FacePieceType.Blue, topFace[0, 0]);
            Assert.AreEqual(FacePieceType.Blue, topFace[0, 1]);
            Assert.AreEqual(FacePieceType.Blue, topFace[0, 2]);

            // rotated
            Assert.AreEqual(FacePieceType.White, backFace[2, 0]);
            Assert.AreEqual(FacePieceType.White, backFace[2, 1]);
            Assert.AreEqual(FacePieceType.White, backFace[2, 2]);

            // rotated
            Assert.AreEqual(FacePieceType.Green, bottomFace[0, 0]);
            Assert.AreEqual(FacePieceType.Green, bottomFace[0, 1]);
            Assert.AreEqual(FacePieceType.Green, bottomFace[0, 2]);
        }

        #endregion

        #region Right Layer

        [Test]
        public void Rotate_Right_Layer_90_Degrees_Clockwise()
        {
            _cube.Rotate90Degrees(LayerType.Right, RotationType.Clockwise);

            var frontFace = _cube.GetFace(FaceType.Front);
            var topFace = _cube.GetFace(FaceType.Top);
            var backFace = _cube.GetFace(FaceType.Back);
            var bottomFace = _cube.GetFace(FaceType.Bottom);

            // rotated
            Assert.AreEqual(FacePieceType.Yellow, frontFace[2, 0]);
            Assert.AreEqual(FacePieceType.Yellow, frontFace[2, 1]);
            Assert.AreEqual(FacePieceType.Yellow, frontFace[2, 2]);

            // rotated
            Assert.AreEqual(FacePieceType.Blue, topFace[2, 0]);
            Assert.AreEqual(FacePieceType.Blue, topFace[2, 1]);
            Assert.AreEqual(FacePieceType.Blue, topFace[2, 2]);

            // rotated
            Assert.AreEqual(FacePieceType.White, backFace[0, 0]);
            Assert.AreEqual(FacePieceType.White, backFace[0, 1]);
            Assert.AreEqual(FacePieceType.White, backFace[0, 2]);

            // rotated
            Assert.AreEqual(FacePieceType.Green, bottomFace[2, 0]);
            Assert.AreEqual(FacePieceType.Green, bottomFace[2, 1]);
            Assert.AreEqual(FacePieceType.Green, bottomFace[2, 2]);
        }

        [Test]
        public void Rotate_Right_Layer_90_Degrees_CounterClockwise()
        {
            _cube.Rotate90Degrees(LayerType.Right, RotationType.CounterClockwise);

            var frontFace = _cube.GetFace(FaceType.Front);
            var topFace = _cube.GetFace(FaceType.Top);
            var backFace = _cube.GetFace(FaceType.Back);
            var bottomFace = _cube.GetFace(FaceType.Bottom);

            // rotated
            Assert.AreEqual(FacePieceType.White, frontFace[2, 0]);
            Assert.AreEqual(FacePieceType.White, frontFace[2, 1]);
            Assert.AreEqual(FacePieceType.White, frontFace[2, 2]);

            // rotated
            Assert.AreEqual(FacePieceType.Green, topFace[2, 0]);
            Assert.AreEqual(FacePieceType.Green, topFace[2,1]);
            Assert.AreEqual(FacePieceType.Green, topFace[2,2]);

            // rotated
            Assert.AreEqual(FacePieceType.Yellow, backFace[0, 0]);
            Assert.AreEqual(FacePieceType.Yellow, backFace[0, 1]);
            Assert.AreEqual(FacePieceType.Yellow, backFace[0, 2]);

            // rotated
            Assert.AreEqual(FacePieceType.Blue, bottomFace[2, 0]);
            Assert.AreEqual(FacePieceType.Blue, bottomFace[2, 1]);
            Assert.AreEqual(FacePieceType.Blue, bottomFace[2, 2]);
        }

        #endregion

        #region Front Layer

        [Test]
        public void Rotate_Front_Layer_90_Degrees_Clockwise()
        {
            _cube.Rotate90Degrees(LayerType.Front, RotationType.Clockwise);

            var topFace = _cube.GetFace(FaceType.Top);
            var leftFace = _cube.GetFace(FaceType.Left);
            var bootomFace = _cube.GetFace(FaceType.Bottom);
            var rightFace = _cube.GetFace(FaceType.Right);

            // rotated
            Assert.AreEqual(FacePieceType.Red, topFace[0, 2]);
            Assert.AreEqual(FacePieceType.Red, topFace[1, 2]);
            Assert.AreEqual(FacePieceType.Red, topFace[2, 2]);

            // rotated
            Assert.AreEqual(FacePieceType.Yellow, leftFace[2, 0]);
            Assert.AreEqual(FacePieceType.Yellow, leftFace[2, 1]);
            Assert.AreEqual(FacePieceType.Yellow, leftFace[2, 2]);

            // rotated
            Assert.AreEqual(FacePieceType.Orange, bootomFace[0, 0]);
            Assert.AreEqual(FacePieceType.Orange, bootomFace[1, 0]);
            Assert.AreEqual(FacePieceType.Orange, bootomFace[2, 0]);

            // rotated
            Assert.AreEqual(FacePieceType.White, rightFace[0, 0]);
            Assert.AreEqual(FacePieceType.White, rightFace[0, 1]);
            Assert.AreEqual(FacePieceType.White, rightFace[0, 2]);
        }

        [Test]
        public void Rotate_Front_Layer_90_Degrees_CounterClockwise()
        {
            _cube.Rotate90Degrees(LayerType.Front, RotationType.CounterClockwise);

            var topFace = _cube.GetFace(FaceType.Top);
            var leftFace = _cube.GetFace(FaceType.Left);
            var bootomFace = _cube.GetFace(FaceType.Bottom);
            var rightFace = _cube.GetFace(FaceType.Right);

            // rotated
            Assert.AreEqual(FacePieceType.Orange, topFace[0, 2]);
            Assert.AreEqual(FacePieceType.Orange, topFace[1, 2]);
            Assert.AreEqual(FacePieceType.Orange, topFace[2, 2]);

            // rotated
            Assert.AreEqual(FacePieceType.White, leftFace[2, 0]);
            Assert.AreEqual(FacePieceType.White, leftFace[2, 1]);
            Assert.AreEqual(FacePieceType.White, leftFace[2, 2]);

            // rotated
            Assert.AreEqual(FacePieceType.Red, bootomFace[0, 0]);
            Assert.AreEqual(FacePieceType.Red, bootomFace[1, 0]);
            Assert.AreEqual(FacePieceType.Red, bootomFace[2, 0]);

            // rotated
            Assert.AreEqual(FacePieceType.Yellow, rightFace[0, 0]);
            Assert.AreEqual(FacePieceType.Yellow, rightFace[0, 1]);
            Assert.AreEqual(FacePieceType.Yellow, rightFace[0, 2]);
        }

        #endregion

        #region Back Layer

        [Test]
        public void Rotate_Back_Layer_90_Degrees_Clockwise()
        {
            _cube.Rotate90Degrees(LayerType.Back, RotationType.Clockwise);

            var topFace = _cube.GetFace(FaceType.Top);
            var leftFace = _cube.GetFace(FaceType.Left);
            var bootomFace = _cube.GetFace(FaceType.Bottom);
            var rightFace = _cube.GetFace(FaceType.Right);

            // rotated
            Assert.AreEqual(FacePieceType.Orange, topFace[0, 0]);
            Assert.AreEqual(FacePieceType.Orange, topFace[1, 0]);
            Assert.AreEqual(FacePieceType.Orange, topFace[2, 0]);

            // rotated
            Assert.AreEqual(FacePieceType.White, leftFace[0, 0]);
            Assert.AreEqual(FacePieceType.White, leftFace[0, 1]);
            Assert.AreEqual(FacePieceType.White, leftFace[0, 2]);

            // rotated
            Assert.AreEqual(FacePieceType.Red, bootomFace[0, 2]);
            Assert.AreEqual(FacePieceType.Red, bootomFace[1, 2]);
            Assert.AreEqual(FacePieceType.Red, bootomFace[2, 2]);

            // rotated
            Assert.AreEqual(FacePieceType.Yellow, rightFace[2, 0]);
            Assert.AreEqual(FacePieceType.Yellow, rightFace[2, 1]);
            Assert.AreEqual(FacePieceType.Yellow, rightFace[2, 2]);
        }

        [Test]
        public void Rotate_Back_Layer_90_Degrees_CounterClockwise()
        {
            _cube.Rotate90Degrees(LayerType.Back, RotationType.CounterClockwise);

            var topFace = _cube.GetFace(FaceType.Top);
            var leftFace = _cube.GetFace(FaceType.Left);
            var bootomFace = _cube.GetFace(FaceType.Bottom);
            var rightFace = _cube.GetFace(FaceType.Right);

            // rotated
            Assert.AreEqual(FacePieceType.Red, topFace[0, 0]);
            Assert.AreEqual(FacePieceType.Red, topFace[1, 0]);
            Assert.AreEqual(FacePieceType.Red, topFace[2, 0]);

            // rotated
            Assert.AreEqual(FacePieceType.Yellow, leftFace[0, 0]);
            Assert.AreEqual(FacePieceType.Yellow, leftFace[0, 1]);
            Assert.AreEqual(FacePieceType.Yellow, leftFace[0, 2]);

            // rotated
            Assert.AreEqual(FacePieceType.Orange, bootomFace[0, 2]);
            Assert.AreEqual(FacePieceType.Orange, bootomFace[1, 2]);
            Assert.AreEqual(FacePieceType.Orange, bootomFace[2, 2]);

            // rotated
            Assert.AreEqual(FacePieceType.White, rightFace[2, 0]);
            Assert.AreEqual(FacePieceType.White, rightFace[2, 1]);
            Assert.AreEqual(FacePieceType.White, rightFace[2, 2]);
        }

        #endregion
    }
}