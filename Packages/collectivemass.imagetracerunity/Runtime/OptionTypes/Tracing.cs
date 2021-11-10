using System;

namespace CollectiveMass.ImageTracerUnity.OptionTypes
{
    [Serializable]
    public class Tracing
    {
        // LineThreshold
        public double LTres { get; set; } = 1f;

        // QuadraticSplineThreshold!
        public double QTres { get; set; } = 2f;
        public int PathOmit { get; set; } = 8;
    }
}
