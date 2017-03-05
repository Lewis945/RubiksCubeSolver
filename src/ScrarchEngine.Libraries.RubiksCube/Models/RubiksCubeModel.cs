using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrarchEngine.Libraries.RubiksCube.Models
{
    public class RubiksCubeModel
    {
        #region Private Static Fields

        private static FaceType[] _topBottomClockwiseOrder = new FaceType[] { FaceType.Front, FaceType.Left, FaceType.Back, FaceType.Right };
        private static FaceType[] _frontBackClockwiseOrder = new FaceType[] { FaceType.Up, FaceType.Right, FaceType.Down, FaceType.Left };
        private static FaceType[] _leftRightClockwiseOrder = new FaceType[] { FaceType.Front, FaceType.Up, FaceType.Back, FaceType.Down };

        private static Dictionary<LayerType, FaceType> _layerFaceMap = new Dictionary<LayerType, FaceType> {
            { LayerType.Front, FaceType.Front },
            { LayerType.Left, FaceType.Left },
            { LayerType.Back, FaceType.Back },
            { LayerType.Right, FaceType.Right },
            { LayerType.Top, FaceType.Up },
            { LayerType.Bottom, FaceType.Down }
        };

        private static Dictionary<FaceType, LayerType> _faceLayerMap = new Dictionary<FaceType, LayerType> {
            { FaceType.Front, LayerType.Front },
            { FaceType.Left, LayerType.Left },
            { FaceType.Back, LayerType.Back },
            { FaceType.Right, LayerType.Right },
            { FaceType.Up, LayerType.Top },
            { FaceType.Down, LayerType.Bottom }
        };

        private static List<List<FacePiece>> _cubieMappings = new List<List<FacePiece>> {
           new List<FacePiece> { new FacePiece(FaceType.Front, 0), new FacePiece(FaceType.Left, 2), new FacePiece(FaceType.Up, 6) },
           new List<FacePiece> { new FacePiece(FaceType.Front, 1), new FacePiece(FaceType.Up, 7) },
           new List<FacePiece> { new FacePiece(FaceType.Front, 2), new FacePiece(FaceType.Right, 0), new FacePiece(FaceType.Up, 8) },
           new List<FacePiece> { new FacePiece(FaceType.Front, 3), new FacePiece(FaceType.Left, 5) },
           new List<FacePiece> { new FacePiece(FaceType.Front, 4) },
           new List<FacePiece> { new FacePiece(FaceType.Front, 5), new FacePiece(FaceType.Right, 3) },
           new List<FacePiece> { new FacePiece(FaceType.Front, 6), new FacePiece(FaceType.Left, 8), new FacePiece(FaceType.Down, 0) },
           new List<FacePiece> { new FacePiece(FaceType.Front, 7), new FacePiece(FaceType.Down, 1) },
           new List<FacePiece> { new FacePiece(FaceType.Front, 8), new FacePiece(FaceType.Right, 6), new FacePiece(FaceType.Down, 2) },

           new List<FacePiece> { new FacePiece(FaceType.Back, 0), new FacePiece(FaceType.Right, 2), new FacePiece(FaceType.Up, 2) },
           new List<FacePiece> { new FacePiece(FaceType.Back, 1), new FacePiece(FaceType.Up, 1) },
           new List<FacePiece> { new FacePiece(FaceType.Back, 2), new FacePiece(FaceType.Left, 0), new FacePiece(FaceType.Up, 0) },
           new List<FacePiece> { new FacePiece(FaceType.Back, 3), new FacePiece(FaceType.Right, 5) },
           new List<FacePiece> { new FacePiece(FaceType.Back, 4) },
           new List<FacePiece> { new FacePiece(FaceType.Back, 5), new FacePiece(FaceType.Left, 3) },
           new List<FacePiece> { new FacePiece(FaceType.Back, 6), new FacePiece(FaceType.Right, 8), new FacePiece(FaceType.Down, 8) },
           new List<FacePiece> { new FacePiece(FaceType.Back, 7), new FacePiece(FaceType.Down, 7) },
           new List<FacePiece> { new FacePiece(FaceType.Back, 8), new FacePiece(FaceType.Left, 6), new FacePiece(FaceType.Down, 6) },

           new List<FacePiece> { new FacePiece(FaceType.Up, 4) },
           new List<FacePiece> { new FacePiece(FaceType.Down, 4) },
           new List<FacePiece> { new FacePiece(FaceType.Left, 4) },
           new List<FacePiece> { new FacePiece(FaceType.Right, 4) },

           new List<FacePiece> { new FacePiece(FaceType.Up, 5), new FacePiece(FaceType.Right, 1) },
           new List<FacePiece> { new FacePiece(FaceType.Up, 3), new FacePiece(FaceType.Left, 1) },
           new List<FacePiece> { new FacePiece(FaceType.Down, 5), new FacePiece(FaceType.Right, 7) },
           new List<FacePiece> { new FacePiece(FaceType.Down, 3), new FacePiece(FaceType.Left, 7) }
        };

        private static Dictionary<LayerType, RotationIndex[]> _facesNearestLayers =
            new Dictionary<LayerType, RotationIndex[]>
            {
                { LayerType.Front,  new RotationIndex[] { new RotationIndex(FaceType.Up, 6,7,8), new RotationIndex(FaceType.Right, 0,3,6), new RotationIndex(FaceType.Down, 2,1,0), new RotationIndex(FaceType.Left, 8,5,2) } },
                { LayerType.Back,  new RotationIndex[] { new RotationIndex(FaceType.Up, 0,1,2), new RotationIndex(FaceType.Left, 6,3,0), new RotationIndex(FaceType.Down, 8,7,6), new RotationIndex(FaceType.Right, 2,5,8) } },

                { LayerType.Left, new RotationIndex[] { new RotationIndex(FaceType.Front, 0,3,6), new RotationIndex(FaceType.Down, 0,3,6), new RotationIndex(FaceType.Back, 8,5,2), new RotationIndex(FaceType.Up, 0,3,6) } },
                { LayerType.Right, new RotationIndex[] { new RotationIndex(FaceType.Front, 2,5,8), new RotationIndex(FaceType.Up, 2,5,8), new RotationIndex(FaceType.Back, 6,3,0), new RotationIndex(FaceType.Down, 2,5,8) } },

                { LayerType.Top, new RotationIndex[] { new RotationIndex(FaceType.Front, 0,1,2), new RotationIndex(FaceType.Left, 0,1,2), new RotationIndex(FaceType.Back, 0,1,2), new RotationIndex(FaceType.Right, 0,1,2) } },
                { LayerType.Bottom, new RotationIndex[] {new RotationIndex(FaceType.Front, 6,7,8), new RotationIndex(FaceType.Right, 6, 7, 8), new RotationIndex(FaceType.Back, 6, 7, 8), new RotationIndex(FaceType.Left, 6, 7, 8) } }
            };

        #endregion

        #region Fields

        private Random _random;

        #endregion

        #region Properties

        public Face[] Faces { get; set; }

        #endregion

        #region .ctor

        public RubiksCubeModel()
            : this(new Face[] {
                new Face(FaceType.Front),
                new Face(FaceType.Left),
                new Face(FaceType.Back),
                new Face(FaceType.Right),
                new Face(FaceType.Up),
                new Face(FaceType.Down)
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

            _random = new Random();
        }

        #endregion

        #region Public Methods

        public Face GetFace(FaceType type)
        {
            var face = Faces.FirstOrDefault(f => f.Type == type);

            if (face == null)
                throw new Exception("Face with the specified type does not exist.");

            return face;
        }

        public void SetFaceColors(FaceType type, FacePieceType[,] colors)
        {
            var face = GetFace(type);
            int n = face.Field.GetLength(0);
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    face[i, j] = colors[i, j];
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

        public void FlipCube(FlipAxis axis, RotationType direction)
        {
            RotationIndex[] rotationOrder;

            if (axis == FlipAxis.Horizontal)
            {
                rotationOrder = direction == RotationType.Clockwise ? _facesNearestLayers[LayerType.Right] : _facesNearestLayers[LayerType.Right].Reverse().ToArray();
                GetFace(FaceType.Right).Rotate90Degrees(direction);
                GetFace(FaceType.Left).Rotate90Degrees(direction == RotationType.Clockwise ? RotationType.CounterClockwise : RotationType.Clockwise);
            }
            else
            {
                rotationOrder = direction == RotationType.Clockwise ? _facesNearestLayers[LayerType.Top] : _facesNearestLayers[LayerType.Top].Reverse().ToArray();
                GetFace(FaceType.Up).Rotate90Degrees(direction);
                GetFace(FaceType.Down).Rotate90Degrees(direction == RotationType.Clockwise ? RotationType.CounterClockwise : RotationType.Clockwise);
            }

            var faceTypes = rotationOrder.Select(o => o.Face);

            var faces = (from ft in faceTypes
                         join f in Faces
                         on ft equals f.Type
                         select f).ToArray();

            Face prevFace = null;
            FacePieceType[,] prevField = null;

            for (int i = 0; i <= faces.Length; i++)
            {
                var face = faces[i % faces.Length];
                var rotationItem = rotationOrder[i % faces.Length];

                var field = face.Field;

                if (i == 0)
                {
                    prevFace = face;
                    prevField = face.Field;
                }
                else
                {
                    if ((face.Type == FaceType.Back || prevFace.Type == FaceType.Back) && axis == FlipAxis.Horizontal)
                    {
                        var thisField = (FacePieceType[,])face.Field.Clone();
                        for (int k = 0; k < 9; k++)
                        {
                            int x = 0;
                            int y = 0;
                            Face.GetIndecies(8 - k, out x, out y);

                            face[k] = prevField[x, y];
                        }
                        prevField = thisField;
                    }
                    else
                    {
                        var thisField = (FacePieceType[,])face.Field.Clone();
                        face.Field = (FacePieceType[,])prevField.Clone();
                        prevField = thisField;
                    }
                    prevFace = face;
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

                    if (face.Type == FaceType.Up || face.Type == FaceType.Down)
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

        public void Shuffle(int moves = 100, int? delay = null)
        {
            for (int i = 0; i < moves; i++)
            {
                var type = (LayerType)_random.Next(1, 9);

                //TODO: temp workaround.
                if (type == LayerType.MiddleFromFront)
                    type = LayerType.Front;
                if (type == LayerType.MiddleFromLeft)
                    type = LayerType.Left;
                if (type == LayerType.MiddleFromTop)
                    type = LayerType.Top;

                var rotation = (RotationType)_random.Next((int)RotationType.Clockwise, (int)RotationType.CounterClockwise);

                Rotate90Degrees(type, rotation);

                if (delay.HasValue)
                    Task.Delay(delay.Value).Wait();
            }
        }

        #endregion

        #region Private Methods

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

                var y1 = face[rotationItem.Indecies[0]];
                var y2 = face[rotationItem.Indecies[1]];
                var y3 = face[rotationItem.Indecies[2]];

                if (i == 0)
                {
                    prev1 = y1;
                    prev2 = y2;
                    prev3 = y3;
                }
                else
                {
                    face[rotationItem.Indecies[0]] = prev1.Value;
                    face[rotationItem.Indecies[1]] = prev2.Value;
                    face[rotationItem.Indecies[2]] = prev3.Value;

                    prev1 = y1;
                    prev2 = y2;
                    prev3 = y3;
                }
            }
        }

        #endregion

        /// <summary>
        /// Perform a deep Copy of the object, using Json as a serialisation method. NOTE: Private members are not cloned using this method.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public T CloneJson<T>(T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (ReferenceEquals(source, null))
            {
                return default(T);
            }

            // initialize inner objects individually
            // for example in default constructor some list property initialized with some values,
            // but in 'source' these items are cleaned -
            // without ObjectCreationHandling.Replace default constructor values will be added to result
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

            var str = JsonConvert.SerializeObject(source);
            var obj = JsonConvert.DeserializeObject<T>(str, deserializeSettings);

            return obj;
        }
    }
}
