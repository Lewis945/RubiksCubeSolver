using NUnit.Framework;
using ScrarchEngine.Libraries.RubiksCube.Models;
using System;
using System.Collections.Generic;
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
        }
    }
}
