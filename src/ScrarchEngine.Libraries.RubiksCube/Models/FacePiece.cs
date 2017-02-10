namespace ScrarchEngine.Libraries.RubiksCube.Models
{
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
}
