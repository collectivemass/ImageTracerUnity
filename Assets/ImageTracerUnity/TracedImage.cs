using System.Collections.Generic;
using ImageTracerUnity.Vectorization.TraceTypes;

namespace ImageTracerUnity
{
    internal class TracedImage
    {
        public int Width { get; }
        public int Height { get; }
        public Dictionary<ColorReference, Layer<SegmentPath>> Layers { get; }

        public TracedImage(Dictionary<ColorReference, Layer<SegmentPath>> layers, int width, int height)
        {
            Width = width;
            Height = height;
            Layers = layers;
        }
    }
}
