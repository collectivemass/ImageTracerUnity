using System;

namespace CollectiveMass.ImageTracerUnity.OptionTypes
{
    [Serializable]
    public class Tracing
    {
        // LineThreshold
        public double lineThreshold = 1f;

        // QuadraticSplineThreshold!
        public double quadraticSplineThreshold = 2f;
        public int pathOmit = 8;
    }
}
