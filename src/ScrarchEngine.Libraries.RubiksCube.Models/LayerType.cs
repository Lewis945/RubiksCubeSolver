using System;

namespace ScrarchEngine.Libraries.RubiksCube.Models
{
    /// <summary>
	/// Defines the layers of a cube
	/// </summary>
    [Flags]
    public enum LayerType
    {
        None = 1,
        Top = 1 << 1,
        MiddleFromTop = 1 << 2,
        Bottom = 1 << 3,
        Front = 1 << 4,
        MiddleFromFront = 1 << 5,
        Back = 1 << 6,
        Left = 1 << 7,
        MiddleFromLeft = 1 << 8,
        Right = 1 << 9
    }
}