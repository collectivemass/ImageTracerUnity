using System.Collections.Generic;

namespace CollectiveMass.ImageTracerUnity.Vectorization.TraceTypes
{
    internal class SequencePath
    {
        public InterpolationPointPath Path { get; set; }
        public IReadOnlyList<SequenceIndices> Sequences { get; set; }
    }
}
