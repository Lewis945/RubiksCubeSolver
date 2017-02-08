using System;

namespace RubiksCube.Game.GraphicsEngine
{
    public class Point3D
    {
        #region Properties

        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        #endregion

        #region .ctor

        public Point3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        #endregion

        #region Methods

        public void Rotate(RotationType type, double angleInDeg)
        {
            // Rotation matrix: http://de.wikipedia.org/wiki/Drehmatrix
            double rad = angleInDeg * Math.PI / 180;
            double cosa = Math.Cos(rad);
            double sina = Math.Sin(rad);

            var old = new Point3D(X, Y, Z);

            switch (type)
            {
                case RotationType.X:
                    Y = old.Y * cosa - old.Z * sina;
                    Z = old.Y * sina + old.Z * cosa;
                    break;
                case RotationType.Y:
                    X = old.Z * sina + old.X * cosa;
                    Z = old.Z * cosa - old.X * sina;
                    break;
                case RotationType.Z:
                    X = old.X * cosa - old.Y * sina;
                    Y = old.X * sina + old.Y * cosa;
                    break;
            }
        }

        public Point3D Project(int viewWidth, int viewHeight, int fov, int viewDistance, double scale)
        {
            double factor = fov / (viewDistance + Z) * scale;
            double Xn = X * factor + viewWidth / 2;
            double Yn = Y * factor + viewHeight / 2;
            return new Point3D(Xn, Yn, Z);
        }

        #endregion
    }
}
