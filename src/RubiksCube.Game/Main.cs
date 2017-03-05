using ScrarchEngine.Libraries.RubiksCube.Models;
using ScrarchEngine.Libraries.RubiksCube.Solver.Methods.Beginners;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;

namespace RubiksCube.Game
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void radioButton3dCube_CheckedChanged(object sender, EventArgs e)
        {
            //rubicsCubeControl.DrawingMode = DrawingMode.Mode3D;
        }

        private List<MoveAlgorithm> _solution;
        private void radioButton2dCube_CheckedChanged(object sender, EventArgs e)
        {
            //rubicsCubeControl.DrawingMode = DrawingMode.Mode2D;

            //rubicsCubeControl.RubiksCubeModel.Shuffle();

            rubicsCubeControl.RubiksCubeModel.Rotate90Degrees(LayerType.Right, RotationType.Clockwise);
            rubicsCubeControl.RubiksCubeModel.Rotate90Degrees(LayerType.Top, RotationType.Clockwise);
            rubicsCubeControl.RubiksCubeModel.Rotate90Degrees(LayerType.Left, RotationType.Clockwise);
            rubicsCubeControl.RubiksCubeModel.Rotate90Degrees(LayerType.Bottom, RotationType.Clockwise);
            rubicsCubeControl.RubiksCubeModel.Rotate90Degrees(LayerType.Right, RotationType.Clockwise);
            rubicsCubeControl.RubiksCubeModel.Rotate90Degrees(LayerType.Top, RotationType.CounterClockwise);
            rubicsCubeControl.RubiksCubeModel.Rotate90Degrees(LayerType.Left, RotationType.CounterClockwise);
            rubicsCubeControl.RubiksCubeModel.Rotate90Degrees(LayerType.Bottom, RotationType.CounterClockwise);
            rubicsCubeControl.RubiksCubeModel.Rotate90Degrees(LayerType.Left, RotationType.Clockwise);
            rubicsCubeControl.RubiksCubeModel.Rotate90Degrees(LayerType.Left, RotationType.Clockwise);

            var solver = new BeginnersSolver(rubicsCubeControl.RubiksCubeModel, (f) => File.ReadAllText(f));
            _solution = solver.Solve();

            //rubicsCubeControl.RubiksCubeModel.FlipCube(FlipAxis.Vertical, RotationType.CounterClockwise);

            System.Threading.Tasks.Task.Run(() =>
            {
                //Task.Delay(2000).Wait();

                //_solution = solver.Solve();

                //foreach (var alg in _solution)
                //{
                //    if (alg.IsFlip)
                //    {
                //        rubicsCubeControl.RubiksCubeModel.FlipCube(alg.Axis, alg.RotationType);
                //        continue;
                //    }

                //    foreach (var move in alg.Moves)
                //        rubicsCubeControl.RubiksCubeModel.Rotate90Degrees(move.Layer, move.Rotation);
                //}

                //delay:2000
                //rubicsCubeControl.RubiksCubeModel.Shuffle();
            });
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            var alg = _solution.FirstOrDefault();
            if (alg == null)
            {
                MessageBox.Show("Done!");
                return;
            }

            _solution.Remove(alg);

            if (alg.IsFlip)
            {
                rubicsCubeControl.RubiksCubeModel.FlipCube(alg.Axis, alg.RotationType);
                return;
            }

            foreach (var move in alg.Moves)
                rubicsCubeControl.RubiksCubeModel.Rotate90Degrees(move.Layer, move.Rotation);
        }
    }
}
