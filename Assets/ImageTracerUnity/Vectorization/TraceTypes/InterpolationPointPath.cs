using System.Collections.Generic;
using ImageTracerUnity.Vectorization.Points;

namespace ImageTracerUnity.Vectorization.TraceTypes
{
    internal class InterpolationPointPath
    {
        public IReadOnlyList<InterpolationPoint> Points { get; set; }
    }
}
