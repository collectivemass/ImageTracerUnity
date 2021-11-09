using System.Collections.Generic;
using ImageTracerUnity.Vectorization.Segments;

namespace ImageTracerUnity.Vectorization.TraceTypes
{
    internal class SegmentPath
    {
        public IReadOnlyList<Segment> Segments { get; set; }
    }
}
