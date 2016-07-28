using System.Diagnostics;

namespace ScrarchEngine.Libraries.RubiksCube.Models
{
    public static class Utilities
    {
        public static T[,] RotateMatrixClockwise<T>(T[,] matrix)
        {
            var n = matrix.GetLength(0);

            var result = new T[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    result[i, j] = matrix[n - j - 1, i];
                }
            }

            return result;
        }

        public static T[,] RotateMatrixCounterClockwise<T>(T[,] matrix)
        {
            var n = matrix.GetLength(0);

            var result = new T[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    result[i, j] = matrix[j, n - i - 1];
                }
            }

            return result;
        }
    }
}