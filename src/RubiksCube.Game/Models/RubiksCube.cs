using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace RubiksCube.Game.Models
{
    public class RubiksCube
    {
        #region Properties

        public List<Layer> Layers { get; private set; }

        public Dictionary<FacePosition, Color> Colors { get; private set; }

        #endregion

        #region .ctor

        public RubiksCube()
            : this(Color.Orange, Color.Red, Color.Yellow, Color.White, Color.Blue, Color.Green)
        {
        }

        public RubiksCube(Color frontColor, Color backColor, Color topColor, Color bottomColor, Color rightColor, Color leftColor)
        {
            SetColors(frontColor, backColor, topColor, bottomColor, rightColor, leftColor);
            CreateLayers();
            FillLayers();
            SetFacesColors();
        }

        #endregion

        #region Private Methods

        private void SetColors(Color frontColor, Color backColor, Color topColor, Color bottomColor, Color rightColor, Color leftColor)
        {
            Colors = new Dictionary<FacePosition, Color>();
            Colors[FacePosition.Front] = frontColor;
            Colors[FacePosition.Back] = backColor;
            Colors[FacePosition.Top] = topColor;
            Colors[FacePosition.Bottom] = bottomColor;
            Colors[FacePosition.Right] = rightColor;
            Colors[FacePosition.Left] = leftColor;
        }

        private void CreateLayers()
        {
            Layers = new List<Layer>();

            // left to right
            Layers.Add(new Layer(LayerType.LeftSlice));
            Layers.Add(new Layer(LayerType.MiddleSliceSides));
            Layers.Add(new Layer(LayerType.RightSlice));

            // top to bottom
            Layers.Add(new Layer(LayerType.TopLayer));
            Layers.Add(new Layer(LayerType.MiddleLayer));
            Layers.Add(new Layer(LayerType.BottomLayer));

            // front to back
            Layers.Add(new Layer(LayerType.FrontSlice));
            Layers.Add(new Layer(LayerType.MiddleSlice));
            Layers.Add(new Layer(LayerType.BackSlice));
        }

        private void FillLayers()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        Layer layer = null;

                        var cube = new Cube();

                        // left to right
                        if (i == 0)
                        {
                            layer = Layers.FirstOrDefault(l => l.Type == LayerType.LeftSlice);
                            cube.Positions.Add(LayerType.LeftSlice);
                        }
                        else if (i == 1)
                        {
                            layer = Layers.FirstOrDefault(l => l.Type == LayerType.MiddleSliceSides);
                            cube.Positions.Add(LayerType.MiddleSliceSides);
                        }
                        else if (i == 2)
                        {
                            layer = Layers.FirstOrDefault(l => l.Type == LayerType.RightSlice);
                            cube.Positions.Add(LayerType.RightSlice);
                        }

                        layer.AddCube(cube);

                        // top to bottom
                        if (j == 0)
                        {
                            layer = Layers.FirstOrDefault(l => l.Type == LayerType.TopLayer);
                            cube.Positions.Add(LayerType.TopLayer);
                        }
                        else if (j == 1)
                        {
                            layer = Layers.FirstOrDefault(l => l.Type == LayerType.MiddleLayer);
                            cube.Positions.Add(LayerType.MiddleLayer);
                        }
                        else if (j == 2)
                        {
                            layer = Layers.FirstOrDefault(l => l.Type == LayerType.BottomLayer);
                            cube.Positions.Add(LayerType.BottomLayer);
                        }

                        layer.AddCube(cube);

                        // front to back
                        if (k == 0)
                        {
                            layer = Layers.FirstOrDefault(l => l.Type == LayerType.FrontSlice);
                            cube.Positions.Add(LayerType.FrontSlice);
                        }
                        else if (k == 1)
                        {
                            layer = Layers.FirstOrDefault(l => l.Type == LayerType.MiddleSlice);
                            cube.Positions.Add(LayerType.MiddleSlice);
                        }
                        else if (k == 2)
                        {
                            layer = Layers.FirstOrDefault(l => l.Type == LayerType.BackSlice);
                            cube.Positions.Add(LayerType.BackSlice);
                        }

                        layer.AddCube(cube);
                    }
                }
            }
        }

        private void SetFaceColor(LayerType type, FacePosition facePosition)
        {
            var faces = Layers.Where(l => l.Type == type)
                .SelectMany(l => l.Cubes)
                .SelectMany(c => c.Faces.Where(f => f.Position == facePosition));

            faces.ToList().ForEach(f => { f.Color = Colors[facePosition]; });
        }

        private void SetFacesColors()
        {
            SetFaceColor(LayerType.FrontSlice, FacePosition.Front);
            SetFaceColor(LayerType.BackSlice, FacePosition.Back);
            SetFaceColor(LayerType.TopLayer, FacePosition.Top);
            SetFaceColor(LayerType.BottomLayer, FacePosition.Bottom);
            SetFaceColor(LayerType.RightSlice, FacePosition.Right);
            SetFaceColor(LayerType.LeftSlice, FacePosition.Left);
        }

        #endregion

        #region Methods

        public IEnumerable<Cube> GetCubes()
        {
            return Layers.SelectMany(l => l.Cubes);
        }

        public IEnumerable<Face> GetFaces()
        {
            return GetCubes().SelectMany(c => c.Faces);
        }

        #endregion
    }
}
