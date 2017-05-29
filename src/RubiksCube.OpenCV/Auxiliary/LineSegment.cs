using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubiksCube.OpenCV.Auxiliary
{
    public class LineSegment
    {
        private Point m_P1;

        private Point m_P2;

        private double m_Gradient;

        private double m_YIntersect;

        #region Straight line properties

        public double YIntersect
        {
            get
            {
                m_YIntersect = P1.Y - (Gradient * P1.X);
                return m_YIntersect;
            }
        }

        public double Gradient
        {
            get
            {
                var div = (P1.X - P2.X);
                m_Gradient = div != 0 ? (P1.Y - P2.Y) / (P1.X - P2.X) : 0;
                return m_Gradient;
            }
        }

        public Point P1
        {
            get
            {
                return m_P1;
            }
            set
            {
                m_P1 = value;
            }
        }

        public Point P2
        {
            get
            {
                return m_P2;
            }
            set
            {
                m_P2 = value;
            }
        }

        #endregion

        public LineSegment(Point p1, Point p2)
        {
            P1 = p1;
            P2 = p2;
        }

        public override string ToString()
        {
            if (YIntersect > 0)
            {
                return "y=" + Gradient.ToString() + "x+" + YIntersect.ToString();
            }
            else
            {
                return "y=" + Gradient.ToString() + "x" + YIntersect.ToString();
            }
        }

        public static double GetAngle(LineSegment l1, LineSegment l2)
        {
            double tanOfAngle = (l2.Gradient - l1.Gradient) / (1 + l1.Gradient * l2.Gradient);

            double angle = Math.Atan(tanOfAngle) * 180 / Math.PI;

            return angle;
        }
    }
}
