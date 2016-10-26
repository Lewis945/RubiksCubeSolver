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

        public static void KeepVectorsByStatus(VectorOfKeyPoint f1, VectorOfKeyPoint f2, VectorOfByte status)
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

        public static void KeepVectorsByStatus(VectorOfKeyPoint f1, VectorOfKeyPoint f2, VectorOfKeyPoint f3, VectorOfByte status)
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
    }
}
