using System.Collections.Generic;
using System.Linq;
using CollectiveMass.ImageTracerUnity.Extensions;

namespace CollectiveMass.ImageTracerUnity.Vectorization
{
    //https://en.wikipedia.org/wiki/Indexed_color
    // Container for the color-indexed image before and tracedata after vectorizing
    internal static class ColorGrouping
    {
        // array[x][y] of palette colors
        //public IEnumerable<ColorGroup> ColorGroups { get; }
        //public int ImageWidth { get; }
        //public int ImageHeight { get; }

        // array[palettelength][4] RGBA color palette
        //public IReadOnlyList<ColorReference> Palette { get; }
        // tracedata
        //public IReadOnlyList<Layer<SegmentPath>> Layers { get; set; }

        //public PaddedPaletteImage(IEnumerable<ColorReference> colors, int height, int width, IReadOnlyList<ColorReference> palette)
        //{
        //    Palette = palette;
        //    ImageWidth = width;
        //    ImageHeight = height;

        //    ColorGroups = ConvertToPaddedPaletteColorGroups(colors);
        //}

        
        //private int PaddedWidth => ImageWidth + 2;
        //private int PaddedHeight => ImageHeight + 2;

        // Creating indexed color array arr which has a boundary filled with -1 in every direction
        // Imagine the -1's being ColorReference.Empty and the 0's being null.
        // Example: 4x4 image becomes a 6x6 matrix:
        // -1 -1 -1 -1 -1 -1
        // -1  0  0  0  0 -1
        // -1  0  0  0  0 -1
        // -1  0  0  0  0 -1
        // -1  0  0  0  0 -1
        // -1 -1 -1 -1 -1 -1
        private static IEnumerable<ColorReference[]> CreatePaddedColorMatrix(int width, int height)
        {
            return new ColorReference[height][].Initialize(i =>
            i == 0 || i == height - 1
                ? new ColorReference[width].Initialize(j => ColorReference.Empty)
                : new ColorReference[width].Initialize(j => ColorReference.Empty, 0, width - 1));
        }

        public static IEnumerable<ColorGroup> Convert(IEnumerable<ColorReference> colors, int width, int height, IReadOnlyList<ColorReference> palette)
        {
            // Indexed color array requires +2 to the original width and height
            int paddedWidth = width + 2;
            int paddedHeight = height + 2;

            var imageColorQueue = new Queue<ColorReference>(colors.AsParallel()
                .AsOrdered()
                .Select(c => ColorUtils.FindClosest(c.Color, palette)));

            var colorMatrix = CreatePaddedColorMatrix(paddedWidth, paddedHeight)
                .SelectMany(c => c)
                .Select(c => c ?? imageColorQueue.Dequeue())
                .ToList();

            for (int row = 1; row < paddedHeight - 1; row++)
            {
                for (int column = 1; column < paddedWidth - 1; column++)
                {
                    yield return new ColorGroup(colorMatrix, row, column, paddedWidth);
                }
            }
        }
    }
}
