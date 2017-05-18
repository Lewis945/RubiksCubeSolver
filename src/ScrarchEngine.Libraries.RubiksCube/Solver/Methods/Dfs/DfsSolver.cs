using ScrarchEngine.Libraries.RubiksCube.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
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

        private Func<Face[], Dictionary<FaceType, int[,]>> getState;
        private Func<Dictionary<FaceType, int[,]>, Face[]> getFaces;
        private Func<Face[], LayerType, RotationType, Node> getChildNode;

        #endregion

        #region .ctor

        public DfsSolver(RubiksCubeModel model)
        {
            _model = model;

            getState = (f) =>
            {
                var state = new Dictionary<FaceType, int[,]>();
                foreach (var faceItem in f)
                {
                    var face = new int[3, 3];
                    for (int i = 0; i < 3; i++)
                        for (int j = 0; j < 3; j++)
                            face[i, j] = (int)faceItem[i, j];
                    state.Add(faceItem.Type, face);
                }
                return state;
            };

            getFaces = (s) =>
            {
                var state = new Face[6];
                int k = 0;
                foreach (var stateItem in s)
                {
                    var face = new Face(stateItem.Key);
                    for (int i = 0; i < 3; i++)
                        for (int j = 0; j < 3; j++)
                            face[i, j] = (FacePieceType)stateItem.Value[i, j];
                    state[k] = face;
                    k++;
                }
                return state;
            };

            getChildNode = (f, l, r) =>
            {
                var childNode = new Node(getState(f));
                childNode.Layer = l;
                childNode.Rotation = r;
                return childNode;
            };
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
            public Dictionary<FaceType, int[,]> State { get; set; }
            public LayerType Layer { get; set; }
            public RotationType Rotation { get; set; }

            public Node(Dictionary<FaceType, int[,]> state)
            {
                State = state;
                Layer = LayerType.None;
                Rotation = RotationType.Clockwise;
            }
        }

        private List<Node> GetChildren(Node node)
        {
            var children = new List<Node>();

            _model.Faces = getFaces(node.State);
            _model.Rotate90Degrees(LayerType.Top, RotationType.Clockwise);
            children.Add(getChildNode(_model.Faces, LayerType.Top, RotationType.Clockwise));
            _model.Faces = getFaces(node.State);
            _model.Rotate90Degrees(LayerType.Top, RotationType.CounterClockwise);
            children.Add(getChildNode(_model.Faces, LayerType.Top, RotationType.CounterClockwise));

            _model.Faces = getFaces(node.State);
            _model.Rotate90Degrees(LayerType.Bottom, RotationType.Clockwise);
            children.Add(getChildNode(_model.Faces, LayerType.Bottom, RotationType.Clockwise));
            _model.Faces = getFaces(node.State);
            _model.Rotate90Degrees(LayerType.Bottom, RotationType.CounterClockwise);
            children.Add(getChildNode(_model.Faces, LayerType.Bottom, RotationType.CounterClockwise));

            _model.Faces = getFaces(node.State);
            _model.Rotate90Degrees(LayerType.Left, RotationType.Clockwise);
            children.Add(getChildNode(_model.Faces, LayerType.Left, RotationType.Clockwise));
            _model.Faces = getFaces(node.State);
            _model.Rotate90Degrees(LayerType.Left, RotationType.CounterClockwise);
            children.Add(getChildNode(_model.Faces, LayerType.Left, RotationType.CounterClockwise));

            _model.Faces = getFaces(node.State);
            _model.Rotate90Degrees(LayerType.Right, RotationType.Clockwise);
            children.Add(getChildNode(_model.Faces, LayerType.Right, RotationType.Clockwise));
            _model.Faces = getFaces(node.State);
            _model.Rotate90Degrees(LayerType.Right, RotationType.CounterClockwise);
            children.Add(getChildNode(_model.Faces, LayerType.Right, RotationType.CounterClockwise));

            _model.Faces = getFaces(node.State);
            _model.Rotate90Degrees(LayerType.Front, RotationType.Clockwise);
            children.Add(getChildNode(_model.Faces, LayerType.Front, RotationType.Clockwise));
            _model.Faces = getFaces(node.State);
            _model.Rotate90Degrees(LayerType.Front, RotationType.CounterClockwise);
            children.Add(getChildNode(_model.Faces, LayerType.Front, RotationType.CounterClockwise));

            _model.Faces = getFaces(node.State);
            _model.Rotate90Degrees(LayerType.Back, RotationType.Clockwise);
            children.Add(getChildNode(_model.Faces, LayerType.Back, RotationType.Clockwise));
            _model.Faces = getFaces(node.State);
            _model.Rotate90Degrees(LayerType.Back, RotationType.CounterClockwise);
            children.Add(getChildNode(_model.Faces, LayerType.Back, RotationType.CounterClockwise));

            return children;
        }

        private bool IsStateFinal(Node node)
        {
            foreach (var face in node.State)
            {
                bool equal = face.Value.Cast<int>().All(x => x == face.Value[0, 0]);
                if (!equal)
                    return false;
            }

            return true;
        }

        public List<Node> FindSolution()
        {
            var rootNode = new Node(getState(_model.Faces));

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
                        break;

                    var kids = GetChildren(node);
                    if (kids.Count() == 0)
                        solution.Remove(node);
                    foreach (var child in kids)
                        stack.Push(child);
                }

                if (IsStateFinal(solution.Last()))
                    return solution;
                return null;
            };

            var tasks = new List<Task<List<Node>>>();
            var children = GetChildren(rootNode);
            foreach (var child in children.Where(c => c.Layer == LayerType.Top && c.Rotation == RotationType.CounterClockwise))
            {
                var task = Task.Run(() => dfs(child));
                tasks.Add(task);
            }

            while (true)
            {
                foreach (var task in tasks)
                    if (task.IsCompleted && task.Result != null)
                        return task.Result;

                Task.Delay(100).Wait();
            }
        }

        #endregion
    }
}
