using RubiksCube.Game.Rendering;
using System;
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
        }
    }
}
