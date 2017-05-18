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
using System.Drawing;

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
            //Bootstrapper.Main();
            var colors = Bootstrapper.GetFaceColors();
            MessageBox.Show("6 faces found.");
        }

        #region Face mappings

        private static Dictionary<Color, FacePieceType> _colorToTypeMappings = new Dictionary<Color, FacePieceType>
        {
            { Color.Blue, FacePieceType.Blue },
            { Color.Green, FacePieceType.Green },
            { Color.Orange, FacePieceType.Orange },
            { Color.Red, FacePieceType.Red },
            { Color.White, FacePieceType.White },
            { Color.Yellow, FacePieceType.Yellow }
        };

        private static Dictionary<FacePieceType, Color> _typeToColorMappings = new Dictionary<FacePieceType, Color>
        {
            {  FacePieceType.Blue ,Color.Blue},
            {  FacePieceType.Green,Color.Green },
            { FacePieceType.Orange , Color.Orange},
            { FacePieceType.Red , Color.Red},
            { FacePieceType.White ,  Color.White},
            { FacePieceType.Yellow , Color.Yellow}
        };

        private static Dictionary<FacePieceType, List<FacePieceType>> _mappingClockwiseOrder = new Dictionary<FacePieceType, List<FacePieceType>>
        {
            { FacePieceType.Blue, new List<FacePieceType> { FacePieceType.White, FacePieceType.Green, FacePieceType.Yellow, FacePieceType.Red, FacePieceType.Orange } },
            { FacePieceType.Green, new List<FacePieceType> { FacePieceType.White, FacePieceType.Blue, FacePieceType.Yellow, FacePieceType.Orange, FacePieceType.Red } },

            { FacePieceType.Orange, new List<FacePieceType> { FacePieceType.White, FacePieceType.Red, FacePieceType.Yellow, FacePieceType.Blue, FacePieceType.Green }  },
            { FacePieceType.Red, new List<FacePieceType> { FacePieceType.White, FacePieceType.Orange, FacePieceType.Yellow, FacePieceType.Green, FacePieceType.Blue }  },

            { FacePieceType.White, new List<FacePieceType> { FacePieceType.Red, FacePieceType.Yellow, FacePieceType.Orange, FacePieceType.Blue, FacePieceType.Green }  },
            { FacePieceType.Yellow, new List<FacePieceType> { FacePieceType.Red, FacePieceType.White, FacePieceType.Orange, FacePieceType.Green, FacePieceType.Blue }  }
        };

        private void MapFacesToModel(List<List<Color>> facesColors)
        {
            var firstFace = facesColors.FirstOrDefault();
            rubicsCubeControl.RubiksCubeModel.SetFaceColors(FaceType.Front, MapColors(firstFace));

            var middlePiece = firstFace[4];
            var middlePieceType = _colorToTypeMappings[middlePiece];

            facesColors.Remove(firstFace);

            var order = _mappingClockwiseOrder[middlePieceType];

            int i = 0;
            foreach (var item in order)
            {
                var color = _typeToColorMappings[item];
                var face = facesColors.FirstOrDefault(f => f[4] == color);

                if (i == 0)
                    rubicsCubeControl.RubiksCubeModel.SetFaceColors(FaceType.Up, MapColors(face));

                if (i == 1)
                    rubicsCubeControl.RubiksCubeModel.SetFaceColors(FaceType.Back, MapColors(face));

                if (i == 2)
                    rubicsCubeControl.RubiksCubeModel.SetFaceColors(FaceType.Down, MapColors(face));

                if (i == 3)
                    rubicsCubeControl.RubiksCubeModel.SetFaceColors(FaceType.Left, MapColors(face));

                if (i == 4)
                    rubicsCubeControl.RubiksCubeModel.SetFaceColors(FaceType.Right, MapColors(face));

                i++;
            }
        }

        private FacePieceType[,] MapColors(List<Color> colors)
        {
            var facePieceTypeArray = new FacePieceType[3, 3];

            int k = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    facePieceTypeArray[i, j] = _colorToTypeMappings[colors[k]];
                    k++;
                }
            }
            return facePieceTypeArray;
        }

        #endregion
    }
}
