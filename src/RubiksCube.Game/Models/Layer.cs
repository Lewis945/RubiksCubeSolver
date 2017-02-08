using System.Collections.Generic;

namespace RubiksCube.Game.Models
{
    public class Layer
    {
        #region Properties

        public List<Cube> Cubes { get; set; }

        public LayerType Type { get; set; }

        #endregion

        #region .ctor

        public Layer()
        {
            Cubes = new List<Cube>();
        }

        public Layer(LayerType type)
            : this()
        {
            Type = type;
        }

        #endregion

        #region Methods

        public void AddCube(Cube cube)
        {
            Cubes.Add(cube);
        }

        #endregion
    }
}
