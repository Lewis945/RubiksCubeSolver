using System;
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

        public FacePieceType[,] Field { get; set; }

        public FacePieceType this[int index]
        {
            get
            {
                int x = 0;
                int y = 0;
                GetIndecies(index, out x, out y);

                return Field[x, y];
            }
            set
            {
                int x = 0;
                int y = 0;
                GetIndecies(index, out x, out y);

                Field[x, y] = value;
            }
        }

        public FacePieceType this[int x, int y]
        {
            get
            {
                return Field[x, y];
            }
            set
            {
                Field[x, y] = value;
            }
        }

        public FaceType Type { get; private set; }

        public Face(FaceType type)
        {
            Type = type;
            Field = new FacePieceType[3, 3];

            for (int i = 0; i < Field.GetLength(0); i++)
                for (int j = 0; j < Field.GetLength(1); j++)
                    Field[i, j] = FacePieceTypeMap[type];
        }

        public Face(FacePieceType[,] faces)
        {
            if (faces.GetLength(0) != 3 && faces.GetLength(1) != 3)
                throw new ArgumentException("Array dimensions are not appropriate", nameof(faces));

            Field = faces;
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
                Field = Utilities.RotateMatrixClockwise(Field);
            else
                Field = Utilities.RotateMatrixCounterClockwise(Field);
        }

        public FacePieceType[,] GetField()
        {
            return Field;
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