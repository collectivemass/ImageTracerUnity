using System.Collections.Generic;
using CollectiveMass.ImageTracerUnity.Vectorization.Points;

namespace CollectiveMass.ImageTracerUnity.Vectorization.TraceTypes
{
    internal class InterpolationPointPath
    {
        public IReadOnlyList<InterpolationPoint> Points { get; set; }
    }
}
