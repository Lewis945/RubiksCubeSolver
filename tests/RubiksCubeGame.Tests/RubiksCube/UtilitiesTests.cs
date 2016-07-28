using NUnit.Framework;
using RubiksCubeGame.RubiksCube;

namespace RubiksCubeGame.Tests.RubiksCube
{
    [TestFixture]
    public class UtilitiesTests
    {
        [Test]
        public void Rotate_Matrix_Clockwise()
        {
            var arr = new int[,]
            {
                { 0, 1, 2 },
                { 3, 4, 5 },
                { 6, 7, 8 }
            };

            var rotated = Utilities.RotateMatrixClockwise(arr);

            Assert.AreEqual(arr[0, 0], rotated[0, 2]);
            Assert.AreEqual(arr[2, 0], rotated[0, 0]);
        }

        [Test]
        public void Rotate_Matrix_CounterClockwise()
        {
            var arr = new int[,]
            {
                { 0, 1, 2 },
                { 3, 4, 5 },
                { 6, 7, 8 }
            };

            var rotated = Utilities.RotateMatrixCounterClockwise(arr);

            Assert.AreEqual(arr[0, 0], rotated[2, 0]);
            Assert.AreEqual(arr[2, 0], rotated[2, 2]);
        }
    }
}