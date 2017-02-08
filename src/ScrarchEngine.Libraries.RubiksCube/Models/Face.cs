using System.Collections.Generic;

namespace ScrarchEngine.Libraries.RubiksCube.Models
{
    public class Face
    {
        public static Dictionary<FaceType, FacePieceType> FacePieceTypeMap = new Dictionary<FaceType, FacePieceType>() {
            { FaceType.Front, FacePieceType.Blue },
            { FaceType.Left, FacePieceType.Red },
            { FaceType.Back, FacePieceType.Green },
            { FaceType.Right, FacePieceType.Orange },
            { FaceType.Top, FacePieceType.White },
            { FaceType.Bottom, FacePieceType.Yellow }
        };

        public FacePieceType[,] _field;

        public FacePieceType this[int index]
        {
            get
            {
                int x = 0;
                int y = 0;
                GetIndecies(index,out x, out y);

                return _field[x, y];
            }
            set
            {
                int x = 0;
                int y = 0;
                GetIndecies(index, out x, out y);

                _field[x, y] = value;
            }
        }

        public FacePieceType this[int x, int y]
        {
            get
            {
                return _field[x, y];
            }
            set
            {
                _field[x, y] = value;
            }
        }

        public FaceType Type { get; private set; }

        public Face(FaceType type)
        {
            Type = type;
            _field = new FacePieceType[3, 3];

            for (int i = 0; i < _field.GetLength(0); i++)
                for (int j = 0; j < _field.GetLength(1); j++)
                    _field[i, j] = FacePieceTypeMap[type];
        }

        public LayerType GetLayer(int index)
        {
            int x = 0;
            int y = 0;
            GetIndecies(index, out x, out y);

            return GetLayer(x, y);
        }

        public LayerType GetLayer(int x, int y)
        {
            var layerType = LayerType.None;

            if (Type == FaceType.Top || Type == FaceType.Bottom)
            {
                if (x == 0)
                    layerType |= LayerType.Back;
                else if (x == 1)
                    layerType |= LayerType.MiddleFromFront;
                else if (x == 2)
                    layerType |= LayerType.Front;
            }
            else
            {
                if (x == 0)
                    layerType |= LayerType.Top;
                else if (x == 1)
                    layerType |= LayerType.MiddleFromTop;
                else if (x == 2)
                    layerType |= LayerType.Bottom;
            }

            if (y == 0)
            {
                if (Type == FaceType.Front || Type == FaceType.Top)
                    layerType |= LayerType.Left;
                else if (Type == FaceType.Back || Type == FaceType.Bottom)
                    layerType |= LayerType.Right;

                else if (Type == FaceType.Left)
                    layerType |= LayerType.Back;
                else if (Type == FaceType.Right)
                    layerType |= LayerType.Front;
            }
            else if (y == 1)
            {
                if (Type == FaceType.Front || Type == FaceType.Back || Type == FaceType.Top || Type == FaceType.Bottom)
                    layerType |= LayerType.MiddleFromLeft;

                else if (Type == FaceType.Left || Type == FaceType.Right)
                    layerType |= LayerType.MiddleFromFront;
            }
            else if (y == 2)
            {
                if (Type == FaceType.Front || Type == FaceType.Top)
                    layerType |= LayerType.Right;
                else if (Type == FaceType.Back || Type == FaceType.Bottom)
                    layerType |= LayerType.Left;

                else if (Type == FaceType.Left)
                    layerType |= LayerType.Front;
                else if (Type == FaceType.Right)
                    layerType |= LayerType.Back;
            }

            return layerType;
        }

        public void Rotate90Degrees(RotationType direction)
        {
            if (direction == RotationType.Clockwise)
                _field = Utilities.RotateMatrixClockwise(_field);
            else
                _field = Utilities.RotateMatrixCounterClockwise(_field);
        }

        public FacePieceType[,] GetField()
        {
            return _field;
        }

        private void GetIndecies(int index, out int x, out int y)
        {
            x = 0;
            y = 0;

            if (index > 0 && index < 3)
            {
                x = index;
            }
            else if (index > 2 && index < 6)
            {
                y = 1;
                x = index - 3;
            }
            else if (index > 5 && index < 9)
            {
                y = 2;
                x = index - 6;
            }
        }
    }
}