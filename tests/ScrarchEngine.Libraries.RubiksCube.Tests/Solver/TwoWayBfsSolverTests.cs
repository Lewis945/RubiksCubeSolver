using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrarchEngine.Libraries.RubiksCube.Tests.Solver
{
    [TestFixture]
    public class TwoWayBfsSolverTests
    {
        private IEnumerable<int> GetChildren(int node)
        {
            return new List<int>()
            {
                node, node, node, node, node, node
            };
        }

        [Test]
        public void Bfs_Testing()
        {
            var rootNode = 0;
            var endNode = 0;

            var queue1 = new ConcurrentQueue<int>();
            var queue2 = new ConcurrentQueue<int>();

            queue1.Enqueue(rootNode);
            queue2.Enqueue(endNode);

            int level = 5;
            int count = 1;
            for (int i = 1; i < level; i++)
                count += (int)Math.Pow(6, i);

            var task1 = Task.Run(() =>
            {
                for (int i = 0; i < count; i++)
                {
                    int currentNode;
                    if (queue1.TryDequeue(out currentNode))
                    {
                        var children = GetChildren(currentNode + 1);
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
                    int currentNode;
                    if (queue2.TryDequeue(out currentNode))
                    {
                        var children = GetChildren(currentNode + 1);
                        foreach (var child in children)
                        {
                            queue2.Enqueue(child);
                        }
                    }
                }
            });

            Task.WaitAll(task1, task2);

            CollectionAssert.AreEqual(queue1.ToArray(), queue2.ToArray());
        }
    }
}
