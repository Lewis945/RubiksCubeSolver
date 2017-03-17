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
using ScrarchEngine.Libraries.RubiksCube.Extensions;

namespace ScrarchEngine.Libraries.RubiksCube.Tests.Solver
{
    [TestFixture]
    public class BeginnersSolverTests
    {
        #region Fields

        private RubiksCubeModel _cube;

        private const int RetriesCount = 10000;

        #endregion

        #region .ctor

        public BeginnersSolverTests()
        {
            _cube = new RubiksCubeModel();

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

        #region Tests

        [Test]
        public void Solve_Cube()
        {
            int i = 0;
            while (i < RetriesCount)
            {
                _cube.Reset();
                _cube.Shuffle();

                var cube = _cube.CloneJson();
                var cubeFallback = _cube.CloneJson();

                var solver = new BeginnersSolver(cube, (f) => GetFiles(f));

                List<MoveAlgorithm> solution;
                bool success;
                solution = solver.SolveCross(out success);

                if (!success)
                    LogError("First cross", i, cubeFallback, solution);

                LogSuccess("First cross");

                cubeFallback = cube.CloneJson();
                solution = solver.SolveFirstLayer(out success);

                if (!success)
                    LogError("First layer", i, cubeFallback, solution);

                LogSuccess("First layer");

                cubeFallback = cube.CloneJson();
                solution = solver.SolveSecondLayer(out success);

                if (!success)
                    LogError("Second layer", i, cubeFallback, solution);

                LogSuccess("Second layer");

                cubeFallback = cube.CloneJson();
                solution = solver.SolveSecondFlatCross(out success);

                if (!success)
                    LogError("Second flat cross", i, cubeFallback, solution);

                LogSuccess("Second flat cross");

                cubeFallback = cube.CloneJson();
                solution = solver.SolveSecondCross(out success);

                if (!success)
                    LogError("Second cross", i, cubeFallback, solution);

                LogSuccess("Second cross");

                cubeFallback = cube.CloneJson();
                solution = solver.SolveThirdLayerCubiesLocations(out success);

                if (!success)
                    LogError("Third layer cubies locations", i, cubeFallback, solution);

                LogSuccess("Third layer cubies locations");

                cubeFallback = cube.CloneJson();
                solution = solver.SolveThirdLayer(out success);

                if (!success)
                    LogError("Third layer", i, cubeFallback, solution);

                LogSuccess("Third layer");

                i++;
            }

            Assert.Pass($"Rubiks cube solving passed for {RetriesCount} tries.");
        }

        #endregion

        #region Private Methods

        private void LogError(string stage, int tryNumber, RubiksCubeModel cube, List<MoveAlgorithm> solution)
        {
            Trace.WriteLine($"{stage} solving failed on {tryNumber} try out of {RetriesCount}.");
            Trace.WriteLine($"Cube front face: {cube.GetFace(FaceType.Front).ToString()}");
            Trace.WriteLine($"Cube back face: {cube.GetFace(FaceType.Back).ToString()}");
            Trace.WriteLine($"Cube left face: {cube.GetFace(FaceType.Left).ToString()}");
            Trace.WriteLine($"Cube right face: {cube.GetFace(FaceType.Right).ToString()}");
            Trace.WriteLine($"Cube up face: {cube.GetFace(FaceType.Up).ToString()}");
            Trace.WriteLine($"Cube down face: {cube.GetFace(FaceType.Down).ToString()}");
            Trace.WriteLine($"Solution: {string.Join(", ", solution.Select(a => a.ToString()))}");

            Assert.Fail($"{stage} solving failed on {tryNumber} try out of {RetriesCount}.");
        }

        private void LogSuccess(string stage)
        {
            Trace.WriteLine($"{stage} solving passed.");
        }

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

        #endregion
    }
}
