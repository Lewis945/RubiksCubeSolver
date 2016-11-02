using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubiksCube.OpenCV.TestCase.PtamLikeApproach
{
    public class Utils
    {
        public static VectorOfPointF GetPointsVector(VectorOfKeyPoint keyPoints)
        {
            var points = new PointF[keyPoints.Size];

            for (int i = 0; i < keyPoints.Size; i++)
            {
                points[i] = keyPoints[i].Point;
            }

            return new VectorOfPointF(points);
        }

        public static VectorOfKeyPoint GetKeyPointsVector(VectorOfPointF points)
        {
            var keyPoints = new MKeyPoint[points.Size];

            for (int i = 0; i < points.Size; i++)
            {
                keyPoints[i] = new MKeyPoint()
                {
                    Point = points[i],
                    Size = 1,
                    Angle = 0,
                    Response = 0
                };
            }

            return new VectorOfKeyPoint(keyPoints);
        }

        public static void KeepVectorsByStatus(ref VectorOfKeyPoint f1, ref VectorOfKeyPoint f2, VectorOfByte status)
        {
            var newF1 = new VectorOfKeyPoint();
            var newF2 = new VectorOfKeyPoint();

            for (int i = 0; i < status.Size; i++)
            {
                if (status[i] > 0)
                {
                    newF1.Push(new MKeyPoint[] { f1[i] });
                    newF2.Push(new MKeyPoint[] { f2[i] });
                }
            }

            f1 = newF1;
            f2 = newF2;
        }

        public static void KeepVectorsByStatus(ref VectorOfKeyPoint f1, ref VectorOfPoint3D32F f2, VectorOfByte status)
        {
            var newF1 = new VectorOfKeyPoint();
            var newF2 = new VectorOfPoint3D32F();

            for (int i = 0; i < status.Size; i++)
            {
                if (status[i] > 0)
                {
                    newF1.Push(new MKeyPoint[] { f1[i] });
                    newF2.Push(new MCvPoint3D32f[] { f2[i] });
                }
            }

            f1 = newF1;
            f2 = newF2;
        }

        public static void KeepVectorsByStatus(ref VectorOfKeyPoint f1, ref VectorOfKeyPoint f2, ref VectorOfKeyPoint f3, VectorOfByte status)
        {
            var newF1 = new VectorOfKeyPoint();
            var newF2 = new VectorOfKeyPoint();
            var newF3 = new VectorOfKeyPoint();

            for (int i = 0; i < status.Size; i++)
            {
                if (status[i] > 0)
                {
                    newF1.Push(new MKeyPoint[] { f1[i] });
                    newF2.Push(new MKeyPoint[] { f2[i] });
                    newF3.Push(new MKeyPoint[] { f3[i] });
                }
            }

            f1 = newF1;
            f2 = newF2;
            f3 = newF3;
        }

        public static void Negotiate(ref Matrix<float> m)
        {
            for (int i = 0; i < m.Rows; i++)
                for (int j = 0; j < m.Cols; j++)
                    m[i, j] = -m[i, j];
        }

        public static PointF SubstarctPoints(PointF p1, PointF p2)
        {
            return new PointF(p1.X - p2.X, p1.Y - p2.Y);
        }
    }
}
