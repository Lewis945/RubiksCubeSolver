using NUnit.Framework;
using ScrarchEngine.Libraries.RubiksCube.Models;

namespace ScrarchEngine.Libraries.RubiksCube.Tests.Models
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

           var rotatedArr = new int[,]
           {
                { 6, 3, 0 },
                { 7, 4, 1 },
                { 8, 5, 2 }
           };

            var rotated = Utilities.RotateMatrixClockwise(arr);

            CollectionAssert.AreEqual(rotatedArr, rotated);
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

           var rotatedArr = new int[,]
           {
                { 2, 5, 8 },
                { 1, 4, 7 },
                { 0, 3, 6 }
           };

            var rotated = Utilities.RotateMatrixCounterClockwise(arr);

            CollectionAssert.AreEqual(rotatedArr, rotated);
        }
    }
}