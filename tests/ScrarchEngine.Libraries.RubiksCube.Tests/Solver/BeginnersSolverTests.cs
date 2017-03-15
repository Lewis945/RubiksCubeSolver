using NUnit.Framework;
using ScrarchEngine.Libraries.RubiksCube.Models;
using ScrarchEngine.Libraries.RubiksCube.Solver.Methods.Beginners;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrarchEngine.Libraries.RubiksCube.Tests.Solver
{
    [TestFixture]
    public class BeginnersSolverTests
    {
        #region Fields

        private RubiksCubeModel _cube;

        #endregion

        #region .ctor

        public BeginnersSolverTests()
        {
            var now = DateTime.Now;
            Trace.Listeners.Add(new TextWriterTraceListener($@"D:\Projects\RubiksCube\tests\ScrarchEngine.Libraries.RubiksCube.Tests\bin\Debug\logs\{now.Day}d{now.Month}m{now.Year}y and {now.Hour}h{now.Minute}m{now.Second}s.txt"));
            Trace.AutoFlush = true;
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

        private string GetFiles(string path)
        {
            var builder = new StringBuilder();
            var dir = new DirectoryInfo(path);
            foreach (var file in dir.GetFiles())
            {
                string content = File.ReadAllText(file.FullName);
                content = content.Remove(content.IndexOf("["), 1);
                content = content.Remove(content.LastIndexOf("]"), 1);
                builder.Append(content);
                builder.Append(",");
            }
            return $"[{builder.ToString()}]";
        }

        #region Tests

        #endregion

        [Test]
        public void Solve_Cross()
        {
            int count = 50;

            int i = 0;
            while (i < count)
            {
                _cube = new RubiksCubeModel();
                _cube.Shuffle();

                var solver = new BeginnersSolver(_cube, (f) => GetFiles(f));

                List<MoveAlgorithm> solution;
                bool success;

                solution = solver.SolveCross(out success);

                if (success)
                {
                    Trace.WriteLine($"Cross solving passed.");
                }
                else
                {
                    Trace.WriteLine($"Cross solving failed on {i} try out of {count}.");

                    Trace.WriteLine($"Cube front face: {_cube.GetFace(FaceType.Front).ToString()}");
                    Trace.WriteLine($"Cube back face: {_cube.GetFace(FaceType.Back).ToString()}");
                    Trace.WriteLine($"Cube left face: {_cube.GetFace(FaceType.Left).ToString()}");
                    Trace.WriteLine($"Cube right face: {_cube.GetFace(FaceType.Right).ToString()}");
                    Trace.WriteLine($"Cube up face: {_cube.GetFace(FaceType.Up).ToString()}");
                    Trace.WriteLine($"Cube down face: {_cube.GetFace(FaceType.Down).ToString()}");

                    Trace.WriteLine($"Solution: {string.Join(", ", solution.Select(a => a.ToString()))}");

                    Assert.Fail($"Cross solving failed on {i} try out of {count}.");
                }

                i++;
            }
        }
    }
}
