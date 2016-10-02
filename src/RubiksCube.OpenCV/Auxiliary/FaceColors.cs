using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubiksCube.OpenCV.Auxiliary
{
    public class FaceColors
    {
        private Dictionary<string, Color> _colors;

        public FaceColors()
        {
            _colors = new Dictionary<string, Color>();
        }

        public void SetColor(string cubeName, Color color)
        {
            _colors.Add(cubeName, color);
        }

        public Color GetColor(string cubeName)
        {
            return _colors[cubeName];
        }
    }
}
