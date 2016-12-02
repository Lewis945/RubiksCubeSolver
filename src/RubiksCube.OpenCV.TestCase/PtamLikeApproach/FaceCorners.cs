using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubiksCube.OpenCV.TestCase.PtamLikeApproach
{
    /// <summary>
    /// 
    /// </summary>
    public class FaceCorners
    {
        #region Fields

        private Point[] _points;

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public Point[] Points => _points;

        /// <summary>
        /// Top left or top most
        /// </summary>
        public Point TopLeft => _points[0];

        /// <summary>
        /// Top right or Right most
        /// </summary>
        public Point TopRight => _points[1];

        /// <summary>
        /// Bottom right or Left most
        /// </summary>
        public Point BottomRight => _points[2];

        /// <summary>
        /// Bottom left or Bottom most
        /// </summary>
        public Point BottomLeft => _points[3];

        /// <summary>
        /// 
        /// </summary>
        public bool Rotated { get; set; }

        /// <summary>
        /// Top most
        /// </summary>
        public Point TopMost { get; set; }

        /// <summary>
        /// Left most
        /// </summary>
        public Point LeftMost { get; set; }

        /// <summary>
        /// Right most
        /// </summary>
        public Point RightMost { get; set; }

        /// <summary>
        /// Bottom most
        /// </summary>
        public Point BottomMost { get; set; }

        #endregion

        #region .ctor

        /// <summary>
        /// 
        /// </summary>
        public FaceCorners()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        public FaceCorners(Point[] points)
        {
            _points = new Point[4];

            int minY = points.Min(p => p.Y);
            int minX = points.Min(p => p.X);
            int maxY = points.Max(p => p.Y);
            int maxX = points.Max(p => p.X);

            var minPointsY = points.Where(p => p.Y == minY).ToList();
            var minPointsX = points.Where(p => p.X == minX).ToList();
            var maxPointsY = points.Where(p => p.Y == maxY).ToList();
            var maxPointsX = points.Where(p => p.X == maxX).ToList();

            if (minPointsY.Count == 1 && minPointsX.Count == 1 && maxPointsY.Count == 1 && maxPointsX.Count == 1)
            {
                var topLeft = minPointsX[0];
                var topRight = minPointsY[0];
                var bottomRight = maxPointsX[0];
                var bottomLeft = maxPointsY[0];
                _points[0] = topLeft;
                _points[1] = topRight;
                _points[2] = bottomRight;
                _points[3] = bottomLeft;
            }
            else
            {
                if (minPointsY.Count == 2)
                {
                    var topLeft = minPointsY[0].X < minPointsY[1].X ? minPointsY[0] : minPointsY[1];
                    var topRight = minPointsY[0].X > minPointsY[1].X ? minPointsY[0] : minPointsY[1];

                    var bottomRight = maxPointsY[0].X > maxPointsY[1].X ? maxPointsY[0] : maxPointsY[1];
                    var bottomLeft = maxPointsY[0].X < maxPointsY[1].X ? maxPointsY[0] : maxPointsY[1];

                    _points[0] = topLeft;
                    _points[1] = topRight;
                    _points[2] = bottomRight;
                    _points[3] = bottomLeft;
                }
                //var topLeft = minPointsX[0];
                //var topRight = minPointsY[0];
                //var bottomRight = maxPointsX[0];
                //var bottomLeft = maxPointsY[0];
                //_points[0] = topLeft;
                //_points[1] = topRight;
                //_points[2] = bottomRight;
                //_points[3] = bottomLeft;
            }

            //var topLeft = points.OrderBy(p => p.X).ThenBy(x => x.Y).FirstOrDefault();
            //var bottomRight = points.OrderByDescending(p => p.X).ThenByDescending(x => x.Y).FirstOrDefault();

            //var bottomLeft = points.OrderBy(p => p.X).ThenByDescending(x => x.Y).FirstOrDefault();
            //var topRight = points.OrderByDescending(p => p.X).ThenBy(x => x.Y).FirstOrDefault();

            //_points[0] = topLeft;
            //_points[1] = topRight;
            //_points[2] = bottomRight;
            //_points[3] = bottomLeft;
        }

        #endregion
    }
}
