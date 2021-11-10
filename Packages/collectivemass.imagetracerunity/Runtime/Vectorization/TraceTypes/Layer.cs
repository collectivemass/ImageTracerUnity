using System.Collections.Generic;

namespace CollectiveMass.ImageTracerUnity.Vectorization.TraceTypes
{
    internal class Layer<T> where T : class
    {
        public IReadOnlyList<T> Paths { get; set; }
    }
}
