using System;
using System.Drawing;
using System.Windows.Forms;

namespace RubiksCube.Game.Rendering
{
    public abstract partial class RenderingControl<T>
    {
        #region Fields

        private Point _oldMousePos = new Point(-1, -1);

        #endregion

        #region Properties

        public bool MouseHandling { get; protected set; }

        #endregion

        #region Override

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_oldMousePos.X != -1 && _oldMousePos.Y != -1)
            {
                if (e.Button == MouseButtons.Right)
                {
                    Cursor = Cursors.SizeAll;
                    int dx = e.X - _oldMousePos.X;
                    int dy = e.Y - _oldMousePos.Y;

                    int min = Math.Min(ClientRectangle.Height, ClientRectangle.Width);
                    double scale = min / (double)400 * 6.0;

                    Rotation[1] -= (dx / scale) % 360;
                    Rotation[0] += (dy / scale) % 360;
                }
                else
                {
                    Cursor = Cursors.Arrow;
                }
            }

            _oldMousePos = e.Location;
            base.OnMouseMove(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            zoom = Math.Min(Math.Max(0.2, zoom + e.Delta / 100.0), 10.0);
            base.OnMouseWheel(e);
        }

        #endregion

        #region Private Methods

        #endregion
    }
}
