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
            { FaceType.Up, FacePieceType.White },
            { FaceType.Down, FacePieceType.Yellow }
        };

        public FacePieceType[,] Field { get; set; }

        public FacePieceType this[int index]
        {
            get
            {
                int i = 0;
                int j = 0;
                GetIndecies(index, out i, out j);

                return Field[i, j];
            }
            set
            {
                int i = 0;
                int j = 0;
                GetIndecies(index, out i, out j);

                Field[i, j] = value;
            }
        }

        public FacePieceType this[int i, int j]
        {
            get
            {
                return Field[i, j];
            }
            set
            {
                Field[i, j] = value;
            }
        }

        public FaceType Type { get; private set; }

        public FacePieceType PieceType { get { return Field[1, 1]; } }

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
            int i = 0;
            int j = 0;
            GetIndecies(index, out i, out j);

            return GetLayer(i, j);
        }

        public LayerType GetLayer(int x, int y)
        {
            var layerType = LayerType.None;

            if (Type == FaceType.Up || Type == FaceType.Down)
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
                if (Type == FaceType.Front || Type == FaceType.Up)
                    layerType |= LayerType.Left;
                else if (Type == FaceType.Back || Type == FaceType.Down)
                    layerType |= LayerType.Right;

                else if (Type == FaceType.Left)
                    layerType |= LayerType.Back;
                else if (Type == FaceType.Right)
                    layerType |= LayerType.Front;
            }
            else if (y == 1)
            {
                if (Type == FaceType.Front || Type == FaceType.Back || Type == FaceType.Up || Type == FaceType.Down)
                    layerType |= LayerType.MiddleFromLeft;

                else if (Type == FaceType.Left || Type == FaceType.Right)
                    layerType |= LayerType.MiddleFromFront;
            }
            else if (y == 2)
            {
                if (Type == FaceType.Front || Type == FaceType.Up)
                    layerType |= LayerType.Right;
                else if (Type == FaceType.Back || Type == FaceType.Down)
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

        private void GetIndecies(int index, out int i, out int j)
        {
            i = 0;
            j = 0;

            if (index > 0 && index < 3)
            {
                j = index;
            }
            else if (index > 2 && index < 6)
            {
                i = 1;
                j = index - 3;
            }
            else if (index > 5 && index < 9)
            {
                i = 2;
                j = index - 6;
            }
        }
    }
}