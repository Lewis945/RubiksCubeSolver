using NUnit.Framework;
using ScrarchEngine.Libraries.RubiksCube.Solver.Methods.Dfs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrarchEngine.Libraries.RubiksCube.Tests.Solver
{
    [TestFixture]
    public class DfsTests
    {
        private struct Node
        {
            public int Result1 { get; set; }
            public int Result2 { get; set; }
            public int Result3 { get; set; }

            public List<Node> Children { get; set; }
        }

        private IEnumerable<Node> GetChildren(Node node)
        {
            int r3 = 20;

            return new List<Node>() {
                new Node { Result1 = 1, Result2 = node.Result2 + 1, Result3 = r3 },
                new Node { Result1 = 2, Result2 = node.Result2 + 2, Result3 = r3 },
                new Node { Result1 = 3, Result2 = 20, Result3 = r3 },
                new Node { Result1 = 4, Result2 = node.Result2 + 4, Result3 = r3 },
                new Node { Result1 = 5, Result2 = node.Result2 + 5, Result3 = r3 },
                new Node { Result1 = 6, Result2 = node.Result2 + 6, Result3 = r3 }
            };
        }

        [Test]
        public void Dfs_Testing()
        {
            var model = new RubiksCube.Models.RubiksCubeModel();

            model.Rotate90Degrees(RubiksCube.Models.LayerType.Top, RubiksCube.Models.RotationType.Clockwise);
            model.Rotate90Degrees(RubiksCube.Models.LayerType.Bottom, RubiksCube.Models.RotationType.Clockwise);

            var solver = new DfsSolver(model);
            var solution = solver.FindSolution();

            Assert.AreEqual(2, solution.Count);
        }
    }
}
