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
        public void Bfs_Testing()
        {
            var rootNode = new Node() { Result1 = 0, Result2 = 0, Result3 = 0 };
            var endNode = new Node() { Result1 = 3, Result2 = 20, Result3 = 20 };

            var finalNodes = new ConcurrentDictionary<Node, Node>();

            int limit = 20 * 6 - 20;

            Action<Node, Node> dfs = (start, end) =>
            {
                var stack = new Stack<Node>();
                stack.Push(start);

                while (stack.Any() && stack.Count < limit)
                {
                    var node = stack.Pop();

                    if (node.Result1 == end.Result1 && node.Result2 == end.Result2 && node.Result3 == end.Result3)
                    {
                        finalNodes.TryAdd(start, node);
                        break;
                    }

                    var ch = GetChildren(node);
                    foreach (var child in ch)
                        stack.Push(child);
                }
            };

            var tasks = new List<Task>();
            var nodes = GetChildren(rootNode);
            foreach (var child in nodes)
            {
                var task = Task.Run(() => dfs(child, endNode));
                tasks.Add(task);
            }

            Task.WaitAny(tasks.ToArray());

            var result = finalNodes.First();
            Assert.AreEqual(20, result.Value.Result2);
        }
    }
}
