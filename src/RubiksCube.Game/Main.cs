using ScrarchEngine.Libraries.RubiksCube.Models;
using ScrarchEngine.Libraries.RubiksCube.Solver.Methods.Beginners;
using System;
using System.IO;
using System.Windows.Forms;

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

        private void radioButton2dCube_CheckedChanged(object sender, EventArgs e)
        {
            //rubicsCubeControl.DrawingMode = DrawingMode.Mode2D;

            //rubicsCubeControl.RubiksCubeModel.Shuffle();
            //var solver = new BeginnersSolver(rubicsCubeControl.RubiksCubeModel, (f) => File.ReadAllText(f));
            //solver.Solve();

            System.Threading.Tasks.Task.Run(() =>
            {
                //delay:2000
                //rubicsCubeControl.RubiksCubeModel.Shuffle();

                //rubicsCubeControl.RubiksCubeModel.Rotate90Degrees(LayerType.Top, RotationType.Clockwise);
                //System.Threading.Thread.Sleep(2000);
                //rubicsCubeControl.RubiksCubeModel.Rotate90Degrees(LayerType.Top, RotationType.CounterClockwise);
                //System.Threading.Thread.Sleep(2000);

                //rubicsCubeControl.RubiksCubeModel.Rotate90Degrees(LayerType.Left, RotationType.Clockwise);
                //System.Threading.Thread.Sleep(2000);
                //rubicsCubeControl.RubiksCubeModel.Rotate90Degrees(LayerType.Left, RotationType.CounterClockwise);
                //System.Threading.Thread.Sleep(2000);

                //rubicsCubeControl.RubiksCubeModel.Rotate90Degrees(LayerType.Bottom, RotationType.Clockwise);
                //System.Threading.Thread.Sleep(2000);
                //rubicsCubeControl.RubiksCubeModel.Rotate90Degrees(LayerType.Bottom, RotationType.CounterClockwise);
                //System.Threading.Thread.Sleep(2000);

                //rubicsCubeControl.RubiksCubeModel.Rotate90Degrees(LayerType.Right, RotationType.Clockwise);
                //System.Threading.Thread.Sleep(2000);
                //rubicsCubeControl.RubiksCubeModel.Rotate90Degrees(LayerType.Right, RotationType.CounterClockwise);
                //System.Threading.Thread.Sleep(2000);

                //rubicsCubeControl.RubiksCubeModel.Rotate90Degrees(LayerType.Front, RotationType.Clockwise);
                //System.Threading.Thread.Sleep(2000);
                //rubicsCubeControl.RubiksCubeModel.Rotate90Degrees(LayerType.Front, RotationType.CounterClockwise);
                //System.Threading.Thread.Sleep(2000);

                //rubicsCubeControl.RubiksCubeModel.Rotate90Degrees(LayerType.Back, RotationType.Clockwise);
                //System.Threading.Thread.Sleep(2000);
                //rubicsCubeControl.RubiksCubeModel.Rotate90Degrees(LayerType.Back, RotationType.CounterClockwise);
                //System.Threading.Thread.Sleep(2000);
            });
        }
    }
}
