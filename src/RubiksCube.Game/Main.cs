using ScrarchEngine.Libraries.RubiksCube.Models;
using ScrarchEngine.Libraries.RubiksCube.Solver.Methods.Beginners;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using ScrarchEngine.Libraries.RubiksCube.Extensions;
using RubiksCube.OpenCV;

namespace RubiksCube.Game
{
    public partial class Main : Form
    {
        private List<MoveAlgorithm> _solution;

        public Main()
        {
            InitializeComponent();
        }

        private void radioButton3dCube_CheckedChanged(object sender, EventArgs e)
        {
            //rubicsCubeControl.DrawingMode = DrawingMode.Mode3D;
        }

        private void radioButton2dCube_CheckedChanged(object sender, EventArgs e)
        {
            //rubicsCubeControl.DrawingMode = DrawingMode.Mode2D;

            System.Threading.Tasks.Task.Run(() =>
            {
                //Task.Delay(2000).Wait();

                //delay:2000
                //rubicsCubeControl.RubiksCubeModel.Shuffle();
            });
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            rubicsCubeControl.ResetCube();
            solutionListBox.Items.Clear();
        }

        private void shuffleButton_Click(object sender, EventArgs e)
        {
            rubicsCubeControl.RubiksCubeModel.Shuffle();

            //rubicsCubeControl.RubiksCubeModel.Rotate90Degrees(LayerType.Right, RotationType.Clockwise);
            //rubicsCubeControl.RubiksCubeModel.Rotate90Degrees(LayerType.Top, RotationType.Clockwise);
            //rubicsCubeControl.RubiksCubeModel.Rotate90Degrees(LayerType.Left, RotationType.Clockwise);
            //rubicsCubeControl.RubiksCubeModel.Rotate90Degrees(LayerType.Bottom, RotationType.Clockwise);
            //rubicsCubeControl.RubiksCubeModel.Rotate90Degrees(LayerType.Right, RotationType.Clockwise);
            //rubicsCubeControl.RubiksCubeModel.Rotate90Degrees(LayerType.Top, RotationType.CounterClockwise);
            //rubicsCubeControl.RubiksCubeModel.Rotate90Degrees(LayerType.Left, RotationType.CounterClockwise);
            //rubicsCubeControl.RubiksCubeModel.Rotate90Degrees(LayerType.Bottom, RotationType.CounterClockwise);
            //rubicsCubeControl.RubiksCubeModel.Rotate90Degrees(LayerType.Left, RotationType.Clockwise);
            //rubicsCubeControl.RubiksCubeModel.Rotate90Degrees(LayerType.Left, RotationType.Clockwise);
        }

        private bool ProcessSolutionStep()
        {
            var alg = _solution.FirstOrDefault();
            if (alg == null)
                return false;

            solutionListBox.Items.Add(alg);
            _solution.Remove(alg);

            if (alg.IsFlip)
            {
                rubicsCubeControl.RubiksCubeModel.FlipCube(alg.Axis, alg.RotationType);
                return true;
            }

            foreach (var move in alg.Moves)
                rubicsCubeControl.RubiksCubeModel.Rotate90Degrees(move.Layer, move.Rotation);

            return true;
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            if (_solution == null)
            {
                var cube = rubicsCubeControl.RubiksCubeModel.CloneJson();
                var solver = new BeginnersSolver(cube, (f) => GetFiles(f));
                _solution = solver.Solve();
            }

            if (!ProcessSolutionStep())
            {
                _solution = null;
                MessageBox.Show("Done!");
            }
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

        private void solveButton_Click(object sender, EventArgs e)
        {
            if (_solution == null)
            {
                var cube = rubicsCubeControl.RubiksCubeModel.CloneJson();
                var solver = new BeginnersSolver(cube, (f) => GetFiles(f));
                _solution = solver.Solve();
            }

            while (true)
            {
                if (!ProcessSolutionStep())
                    break;
            }

            _solution = null;
            MessageBox.Show("Done!");
        }

        private void recognizeButton_Click(object sender, EventArgs e)
        {
            Bootstrapper.Main();
            var colors = Bootstrapper.GetFaceColors();
            MessageBox.Show("6 faces found.");
        }
    }
}
