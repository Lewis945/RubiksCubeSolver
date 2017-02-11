using ScrarchEngine.Libraries.RubiksCube.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScrarchEngine.Libraries.RubiksCube.Solver.Methods.Dfs
{
    public class DfsSolver
    {
        #region Fields

        private RubiksCubeModel _model;

        private const int _godNumber = 20;

        #endregion

        #region .ctor

        public DfsSolver(RubiksCubeModel model)
        {
            _model = model;
        }

        #endregion

        #region Public Methods

        public void Solve()
        {

        }

        #endregion

        #region Private Methods

        public struct Node
        {
            public List<FacePieceType[,]> State { get; set; }

            public List<Node> Children { get; set; }
        }

        private IEnumerable<Node> GetChildren(Node node)
        {
            return new List<Node>();
        }

        private bool IsStateFinal(Node node)
        {
            foreach (var face in node.State)
            {
                bool equal = true;
                for (int i = 1; i < 3; i++)
                    for (int j = 1; j < 3; j++)
                        equal = face[i - 1, j - 1] == face[i, j];

                if (!equal)
                    return false;
            }

            return true;
        }

        public List<Node> FindSolution()
        {
            var rootNode = new Node() { State = _model.Faces.Select(f => f.Field).ToList() };

            var finalNodes = new ConcurrentDictionary<Node, Node>();

            int limit = 20 * 6;

            Func<Node, List<Node>> dfs = (start) =>
            {
                var solution = new List<Node>();

                var stack = new Stack<Node>();
                stack.Push(start);

                while (stack.Any() && stack.Count <= limit)
                {
                    var node = stack.Pop();
                    solution.Add(node);

                    if (IsStateFinal(node))
                    {
                        finalNodes.TryAdd(start, node);
                        break;
                    }

                    var children = GetChildren(node);
                    if (children.Count() == 0)
                        solution.Remove(node);
                    foreach (var child in children)
                        stack.Push(child);
                }

                return solution;
            };

            var tasks = new List<Task<List<Node>>>();
            var nodes = GetChildren(rootNode);
            foreach (var child in nodes)
            {
                var task = Task.Run(() => dfs(child));
                tasks.Add(task);
            }

            while (true)
            {
                foreach (var task in tasks)
                    if (task.IsCompleted)
                        if (finalNodes.Count > 0)
                            return task.Result;

                Task.Delay(100).Wait();
            }
        }

        #endregion
    }
}
