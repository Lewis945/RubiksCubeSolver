using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrarchEngine.Libraries.RubiksCube.Models
{
    public class RubiksCubeModel
    {
        public Face[] Faces { get; private set; }

        public RubiksCubeModel()
            : this(new Face[] {
                new Face(FaceType.Front),
                new Face(FaceType.Left),
                new Face(FaceType.Back),
                new Face(FaceType.Right),
                new Face(FaceType.Top),
                new Face(FaceType.Bottom)
            })
        {
            //              | 0 | 1 | 2 | 
            //              | 4 | 5 | 6 |
            //              | 7 | 8 | 9 |
            //              -------------
            //| 0 | 1 | 2 |-| 0 | 1 | 2 |-| 0 | 1 | 2 |-| 0 | 1 | 2 |
            //| 3 | 4 | 5 |-| 3 | 4 | 5 |-| 3 | 4 | 5 |-| 3 | 4 | 5 |
            //| 6 | 7 | 8 |-| 6 | 7 | 8 |-| 6 | 7 | 8 |-| 6 | 7 | 8 |
            //              -------------
            //              | 0 | 1 | 2 |
            //              | 3 | 4 | 5 |
            //              | 6 | 7 | 8 |
        }

        public RubiksCubeModel(Face[] faces)
        {
            Faces = faces;
        }

        private static FaceType[] _topBottomClockwiseOrder = new FaceType[] { FaceType.Front, FaceType.Left, FaceType.Back, FaceType.Right };
        private static FaceType[] _frontBackClockwiseOrder = new FaceType[] { FaceType.Top, FaceType.Right, FaceType.Bottom, FaceType.Left };
        private static FaceType[] _leftRightClockwiseOrder = new FaceType[] { FaceType.Front, FaceType.Top, FaceType.Back, FaceType.Bottom };

        private static Dictionary<LayerType, FaceType> _layerFaceMap = new Dictionary<LayerType, FaceType> {
            { LayerType.Front, FaceType.Front },
            { LayerType.Left, FaceType.Left },
            { LayerType.Back, FaceType.Back },
            { LayerType.Right, FaceType.Right },
            { LayerType.Top, FaceType.Top },
            { LayerType.Bottom, FaceType.Bottom }
        };

        private class RotationIndex
        {
            public FaceType Face { get; private set; }
            public int Index { get; private set; }
            public bool IsX { get; private set; }

            public RotationIndex(FaceType face, int index, bool isX)
            {
                Face = face;
                Index = index;
                IsX = isX;
            }
        }

        private static Dictionary<LayerType, RotationIndex[]> _facesNearestLayers =
            new Dictionary<LayerType, RotationIndex[]>
            {
                { LayerType.Front,  new RotationIndex[] { new RotationIndex(FaceType.Top, 2,true), new RotationIndex(FaceType.Right, 0, false), new RotationIndex(FaceType.Bottom, 0, true), new RotationIndex(FaceType.Left, 2, false) } },
                { LayerType.Back,  new RotationIndex[] { new RotationIndex(FaceType.Top, 0, true), new RotationIndex(FaceType.Left, 0, false), new RotationIndex(FaceType.Bottom, 2, true), new RotationIndex(FaceType.Right, 2, false) } },

                { LayerType.Left, new RotationIndex[] { new RotationIndex(FaceType.Front, 0, false), new RotationIndex(FaceType.Bottom, 0, false), new RotationIndex(FaceType.Back, 2, false), new RotationIndex(FaceType.Top, 0, false) } },
                { LayerType.Right, new RotationIndex[] { new RotationIndex(FaceType.Front, 2, false), new RotationIndex(FaceType.Top, 2, false), new RotationIndex(FaceType.Back, 0, false), new RotationIndex(FaceType.Bottom, 2, false) } },

                { LayerType.Top, new RotationIndex[] { new RotationIndex(FaceType.Front, 0, true), new RotationIndex(FaceType.Left, 0, true), new RotationIndex(FaceType.Back, 0, true), new RotationIndex(FaceType.Right, 0, true) } },
                { LayerType.Bottom, new RotationIndex[] {new RotationIndex(FaceType.Front, 2, true), new RotationIndex(FaceType.Right, 2, true), new RotationIndex(FaceType.Back, 2, true), new RotationIndex(FaceType.Left, 2, true) } }
            };

        public Face GetFace(FaceType type)
        {
            var face = Faces.FirstOrDefault(f => f.Type == type);

            if (face == null)
                throw new Exception("Face with the specified type doesnot exist.");

            return face;
        }

        public void Rotate90Degrees(LayerType layer, RotationType direction)
        {
            // rotate face component of a layer
            var faceComponent = Faces.FirstOrDefault(f => f.Type == _layerFaceMap[layer]);
            if (faceComponent == null) throw new Exception("Face that you are trying to rotate does not exist on the list");
            faceComponent.Rotate90Degrees(direction);

            var rotationOrder = direction == RotationType.Clockwise ? _facesNearestLayers[layer] : _facesNearestLayers[layer].Reverse().ToArray();

            Rotate90NearestLayers(rotationOrder, layer);
        }

        private void Rotate90NearestLayers(RotationIndex[] rotationOrder, LayerType layer)
        {
            var faceTypes = rotationOrder.Select(o => o.Face);

            var faces = (from ft in faceTypes
                         join f in Faces
                         on ft equals f.Type
                         select f).ToArray();

            FacePieceType? prev1 = null;
            FacePieceType? prev2 = null;
            FacePieceType? prev3 = null;

            for (int i = 0; i <= faces.Length; i++)
            {
                var face = faces[i % faces.Length];
                var rotationItem = rotationOrder[i % faces.Length];

                if (rotationItem.IsX)
                {
                    var y1 = face[rotationItem.Index, 0];
                    var y2 = face[rotationItem.Index, 1];
                    var y3 = face[rotationItem.Index, 2];

                    if (i == 0)
                    {
                        prev1 = y1;
                        prev2 = y2;
                        prev3 = y3;
                    }
                    else
                    {
                        face[rotationItem.Index, 0] = prev1.Value;
                        face[rotationItem.Index, 1] = prev2.Value;
                        face[rotationItem.Index, 2] = prev3.Value;

                        prev1 = y1;
                        prev2 = y2;
                        prev3 = y3;
                    }
                }
                else
                {
                    var x1 = face[0, rotationItem.Index];
                    var x2 = face[1, rotationItem.Index];
                    var x3 = face[2, rotationItem.Index];

                    if (i == 0)
                    {
                        prev1 = x1;
                        prev2 = x2;
                        prev3 = x3;
                    }
                    else
                    {
                        face[0, rotationItem.Index] = prev1.Value;
                        face[1, rotationItem.Index] = prev2.Value;
                        face[2, rotationItem.Index] = prev3.Value;

                        prev1 = x1;
                        prev2 = x2;
                        prev3 = x3;
                    }
                }
            }
        }

        public Tuple<int, int> GetOpositeCoordinates(int x, int y, FaceType face)
        {
            Tuple<int, int> result = null;

            int n = 3;
            if (face == FaceType.Front || face == FaceType.Back || face == FaceType.Left || face == FaceType.Right)
                result = Tuple.Create(x, n - y - 1);
            else
                result = Tuple.Create(n - x - 1, y);

            return result;
        }
    }
}