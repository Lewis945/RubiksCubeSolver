namespace ScrarchEngine.Libraries.RubiksCube.Models
{
    internal struct RotationIndex
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
}
