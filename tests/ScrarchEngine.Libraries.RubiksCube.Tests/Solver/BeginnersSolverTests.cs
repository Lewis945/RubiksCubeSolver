using NUnit.Framework;
using ScrarchEngine.Libraries.RubiksCube.Models;
using ScrarchEngine.Libraries.RubiksCube.Solver.Methods.Beginners;
using System;
using System.Collections.Generic;
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
        }

        #endregion

        #region SetUp

        [SetUp]
        public void Init()
        {
            _cube = new RubiksCubeModel();
            _cube.Rotate90Degrees(LayerType.Right, RotationType.Clockwise);
            _cube.Rotate90Degrees(LayerType.Top, RotationType.Clockwise);
            _cube.Rotate90Degrees(LayerType.Left, RotationType.Clockwise);
            _cube.Rotate90Degrees(LayerType.Bottom, RotationType.Clockwise);
            _cube.Rotate90Degrees(LayerType.Right, RotationType.Clockwise);
            _cube.Rotate90Degrees(LayerType.Top, RotationType.CounterClockwise);
            _cube.Rotate90Degrees(LayerType.Left, RotationType.CounterClockwise);
            _cube.Rotate90Degrees(LayerType.Bottom, RotationType.CounterClockwise);
            _cube.Rotate90Degrees(LayerType.Left, RotationType.Clockwise);
            _cube.Rotate90Degrees(LayerType.Left, RotationType.Clockwise);
        }

        [TearDown]
        public void Cleanup()
        {
        }

        #endregion

        #region Tests

        #endregion

        [Test]
        public void Solve_Cross()
        {
            var solver = new BeginnersSolver(_cube, (f) => File.ReadAllText(f));
            var solution = solver.SolveCross();
        }
    }
}
