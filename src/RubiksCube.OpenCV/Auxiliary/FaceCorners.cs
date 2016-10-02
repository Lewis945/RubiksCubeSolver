using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubiksCube.OpenCV.Auxiliary
{
    public class FaceCorners
    {
        /// <summary>
        /// Top left or top most
        /// </summary>
        public Point TopLeft { get; set; }

        /// <summary>
        /// Bottom right or Left most
        /// </summary>
        public Point BottomRight { get; set; }

        /// <summary>
        /// Top right or Right most
        /// </summary>
        public Point TopRight { get; set; }

        /// <summary>
        /// Bottom left or Bottom most
        /// </summary>
        public Point BottomLeft { get; set; }

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
    }
}
