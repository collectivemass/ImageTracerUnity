using CollectiveMass.ImageTracerUnity.Vectorization.TraceTypes;

namespace CollectiveMass.ImageTracerUnity.Svg
{
    internal class ZPosition
    {
        public ColorReference color;
        public SegmentPath path;

        // Label (Z-index key) is the startpoint of the path, linearized
        public double label;
    }
}
