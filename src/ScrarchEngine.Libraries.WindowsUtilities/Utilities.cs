using System.Diagnostics;

namespace ScrarchEngine.Libraries.WindowsUtilities
{
    public static class Utilities
    {
        public static void PrintMatrixToTrace<T>(T[,] matrix)
        {
            Trace.WriteLine("");
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                Trace.WriteLine("");
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Trace.Write(string.Format("{0} ", matrix[i, j]));
                }
            }
            Trace.WriteLine("");
        }
    }
}