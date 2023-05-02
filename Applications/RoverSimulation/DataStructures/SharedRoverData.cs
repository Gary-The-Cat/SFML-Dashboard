using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoverSimulation.DataStructures
{
    public class SharedRoverData
    {
        public float Gold { get; set; }

        public float Fuel { get; set; }

        public Action<float[,], float[,], List<Vector2u>> StartNewSimulation { get; set; }

        public SharedRoverData()
        {

        }
    }
}
