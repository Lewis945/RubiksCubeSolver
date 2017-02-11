using ScrarchEngine.Libraries.RubiksCube.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrarchEngine.Libraries.RubiksCube.Solver.Methods.TwoWayBfs
{
    public class TwoWayBfsSolver
    {
        #region Fields

        private RubiksCubeModel _model;

        private const int _godNumber = 20;

        #endregion

        #region .ctor

        public TwoWayBfsSolver(RubiksCubeModel model)
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

        private struct Node
        {
            public List<Node> Children { get; set; }
        }

        private IEnumerable<Node> GetChildren(Node node)
        {
            return new List<Node>();
        }

        public void FindSolution()
        {
            var rootNode = new Node();
            var endNode = new Node();

            var queue1 = new ConcurrentQueue<Node>();
            var queue2 = new ConcurrentQueue<Node>();

            queue1.Enqueue(rootNode);
            queue2.Enqueue(endNode);

            int count = 0;
            for (int i = 1; i < (_godNumber / 2); i++)
                count += (int)Math.Pow(6, i);

            var task1 = Task.Run(() =>
            {
                for (int i = 0; i < count; i++)
                {
                    Node currentNode;
                    if (queue1.TryDequeue(out currentNode))
                    {
                        var children = GetChildren(currentNode);
                        foreach (var child in children)
                        {
                            queue1.Enqueue(child);
                        }
                    }
                }
            });

            var task2 = Task.Run(() =>
            {
                for (int i = 0; i < count; i++)
                {
                    Node currentNode;
                    if (queue2.TryDequeue(out currentNode))
                    {
                        var children = GetChildren(currentNode);
                        foreach (var child in children)
                        {
                            queue2.Enqueue(child);
                        }
                    }
                }
            });

            Task.WaitAll(task1, task2);
        }

        #endregion
    }
}
