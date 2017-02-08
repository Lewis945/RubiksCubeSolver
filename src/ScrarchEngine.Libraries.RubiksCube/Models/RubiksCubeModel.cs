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
            //              | 3 | 4 | 5 |
            //              | 6 | 7 | 8 |
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

        private static Dictionary<FaceType, LayerType> _faceLayerMap = new Dictionary<FaceType, LayerType> {
            { FaceType.Front, LayerType.Front },
            { FaceType.Left, LayerType.Left },
            { FaceType.Back, LayerType.Back },
            { FaceType.Right, LayerType.Right },
            { FaceType.Top, LayerType.Top },
            { FaceType.Bottom, LayerType.Bottom }
        };

        public struct FacePiece
        {
            public FaceType Face { get; private set; }
            public int Index { get; private set; }

            public FacePieceType? CurrentType { get; set; }

            public FacePiece(FaceType face, int index)
            {
                Face = face;
                Index = index;
                CurrentType = null;
            }
        }

        public struct Cubie
        {
            public List<FacePiece> Pieces { get; private set; }
            public List<LayerType> Layers { get; private set; }

            public Cubie(List<FacePiece> pieces, List<LayerType> layers)
            {
                Pieces = pieces;
                Layers = layers;
            }
        }

        private static List<List<FacePiece>> _cubieMappings = new List<List<FacePiece>> {
           new List<FacePiece> { new FacePiece(FaceType.Front, 0), new FacePiece(FaceType.Left, 2), new FacePiece(FaceType.Top, 6) },
           new List<FacePiece> { new FacePiece(FaceType.Front, 1), new FacePiece(FaceType.Top, 7) },
           new List<FacePiece> { new FacePiece(FaceType.Front, 2), new FacePiece(FaceType.Right, 0), new FacePiece(FaceType.Top, 8) },
           new List<FacePiece> { new FacePiece(FaceType.Front, 3), new FacePiece(FaceType.Left, 5) },
           new List<FacePiece> { new FacePiece(FaceType.Front, 4) },
           new List<FacePiece> { new FacePiece(FaceType.Front, 5), new FacePiece(FaceType.Right, 3) },
           new List<FacePiece> { new FacePiece(FaceType.Front, 6), new FacePiece(FaceType.Left, 8), new FacePiece(FaceType.Bottom, 0) },
           new List<FacePiece> { new FacePiece(FaceType.Front, 7), new FacePiece(FaceType.Bottom, 1) },
           new List<FacePiece> { new FacePiece(FaceType.Front, 8), new FacePiece(FaceType.Right, 6), new FacePiece(FaceType.Bottom, 2) },

           new List<FacePiece> { new FacePiece(FaceType.Back, 0), new FacePiece(FaceType.Right, 2), new FacePiece(FaceType.Top, 2) },
           new List<FacePiece> { new FacePiece(FaceType.Back, 1), new FacePiece(FaceType.Top, 1) },
           new List<FacePiece> { new FacePiece(FaceType.Back, 2), new FacePiece(FaceType.Left, 0), new FacePiece(FaceType.Top, 0) },
           new List<FacePiece> { new FacePiece(FaceType.Back, 3), new FacePiece(FaceType.Right, 5) },
           new List<FacePiece> { new FacePiece(FaceType.Back, 4) },
           new List<FacePiece> { new FacePiece(FaceType.Back, 5), new FacePiece(FaceType.Left, 3) },
           new List<FacePiece> { new FacePiece(FaceType.Back, 6), new FacePiece(FaceType.Right, 8), new FacePiece(FaceType.Bottom, 8) },
           new List<FacePiece> { new FacePiece(FaceType.Back, 7), new FacePiece(FaceType.Bottom, 7) },
           new List<FacePiece> { new FacePiece(FaceType.Back, 8), new FacePiece(FaceType.Left, 6), new FacePiece(FaceType.Bottom, 6) },

           new List<FacePiece> { new FacePiece(FaceType.Top, 4) },
           new List<FacePiece> { new FacePiece(FaceType.Bottom, 4) },
           new List<FacePiece> { new FacePiece(FaceType.Left, 4) },
           new List<FacePiece> { new FacePiece(FaceType.Right, 4) },

           new List<FacePiece> { new FacePiece(FaceType.Top, 5), new FacePiece(FaceType.Right, 1) },
           new List<FacePiece> { new FacePiece(FaceType.Top, 3), new FacePiece(FaceType.Left, 1) },
           new List<FacePiece> { new FacePiece(FaceType.Bottom, 5), new FacePiece(FaceType.Right, 7) },
           new List<FacePiece> { new FacePiece(FaceType.Bottom, 3), new FacePiece(FaceType.Left, 7) }
        };

        private struct RotationIndex
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
                { LayerType.Front,  new RotationIndex[] { new RotationIndex(FaceType.Top, 2, false), new RotationIndex(FaceType.Right, 0, true), new RotationIndex(FaceType.Bottom, 0, false), new RotationIndex(FaceType.Left, 2, true) } },
                { LayerType.Back,  new RotationIndex[] { new RotationIndex(FaceType.Top, 0, false), new RotationIndex(FaceType.Left, 0, true), new RotationIndex(FaceType.Bottom, 2, false), new RotationIndex(FaceType.Right, 2, true) } },

                { LayerType.Left, new RotationIndex[] { new RotationIndex(FaceType.Front, 0, true), new RotationIndex(FaceType.Bottom, 0, true), new RotationIndex(FaceType.Back, 2, true), new RotationIndex(FaceType.Top, 0, true) } },
                { LayerType.Right, new RotationIndex[] { new RotationIndex(FaceType.Front, 2, true), new RotationIndex(FaceType.Top, 2, true), new RotationIndex(FaceType.Back, 0, true), new RotationIndex(FaceType.Bottom, 2, true) } },

                { LayerType.Top, new RotationIndex[] { new RotationIndex(FaceType.Front, 0, false), new RotationIndex(FaceType.Left, 0, false), new RotationIndex(FaceType.Back, 0, false), new RotationIndex(FaceType.Right, 0, false) } },
                { LayerType.Bottom, new RotationIndex[] {new RotationIndex(FaceType.Front, 2, false), new RotationIndex(FaceType.Right, 2, false), new RotationIndex(FaceType.Back, 2, false), new RotationIndex(FaceType.Left, 2, false) } }
            };

        public Face GetFace(FaceType type)
        {
            var face = Faces.FirstOrDefault(f => f.Type == type);

            if (face == null)
                throw new Exception("Face with the specified type does not exist.");

            return face;
        }

        public void Rotate90Degrees(LayerType layer, RotationType direction)
        {
            // rotate face component of a layer
            var faceComponent = Faces.FirstOrDefault(f => f.Type == _layerFaceMap[layer]);
            if (faceComponent == null) throw new Exception("Face that you are trying to rotate does not exist in the list.");
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

        public List<Cubie> GetCubies()
        {
            var result = new List<Cubie>();

            foreach (var faces in _cubieMappings)
            {
                var pieces = new List<FacePiece>();
                var layers = new List<LayerType>();

                foreach (var piece in faces)
                {
                    var face = GetFace(piece.Face);

                    var newPiece = piece;
                    newPiece.CurrentType = face[piece.Index];
                    pieces.Add(newPiece);

                    layers.Add(_faceLayerMap[face.Type]);

                    if (face.Type == FaceType.Top || face.Type == FaceType.Bottom)
                    {
                        if (piece.Index == 1 || piece.Index == 4 || piece.Index == 7)
                            layers.Add(LayerType.MiddleFromLeft);
                        if (piece.Index == 3 || piece.Index == 4 || piece.Index == 5)
                            layers.Add(LayerType.MiddleFromFront);
                    }

                    if (face.Type == FaceType.Left || face.Type == FaceType.Right)
                    {
                        if (piece.Index == 1 || piece.Index == 4 || piece.Index == 7)
                            layers.Add(LayerType.MiddleFromFront);
                        if (piece.Index == 3 || piece.Index == 4 || piece.Index == 5)
                            layers.Add(LayerType.MiddleFromTop);
                    }

                    if (face.Type == FaceType.Front || face.Type == FaceType.Back)
                    {
                        if (piece.Index == 1 || piece.Index == 4 || piece.Index == 7)
                            layers.Add(LayerType.MiddleFromLeft);
                        if (piece.Index == 3 || piece.Index == 4 || piece.Index == 5)
                            layers.Add(LayerType.MiddleFromTop);
                    }
                }

                result.Add(new Cubie(pieces, layers));
            }

            return result;
        }
    }
}