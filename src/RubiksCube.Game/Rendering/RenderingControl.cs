using System.Windows.Forms;

namespace RubiksCube.Game.Rendering
{
    public abstract partial class RenderingControl<T> : UserControl
        where T : class
    {
        #region Properties

        #endregion

        #region .ctor

        public RenderingControl()
        {
            Rotation = new double[] { 0, 0, 0 };
            MouseHandling = false;
            DrawingMode = DrawingMode.Mode3D;
            RotationSpeed = 250;

            InitializeComponent();
        }

        #endregion
    }
}
