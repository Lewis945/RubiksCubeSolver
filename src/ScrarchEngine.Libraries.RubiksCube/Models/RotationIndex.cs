namespace ScrarchEngine.Libraries.RubiksCube.Models
{
    internal struct RotationIndex
    {
        public FaceType Face { get; private set; }
        public int[] Indecies { get; private set; }

        public RotationIndex(FaceType face, params int[] indecies)
        {
            Face = face;
            Indecies = indecies;
        }
    }
}
