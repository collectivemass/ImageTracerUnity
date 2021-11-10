using System.Collections.Generic;
using CollectiveMass.ImageTracerUnity.Vectorization.Segments;

namespace CollectiveMass.ImageTracerUnity.Vectorization.TraceTypes
{
    internal class SegmentPath
    {
        public IReadOnlyList<Segment> Segments { get; set; }
    }
}
