using System.Collections.Generic;
using System.Linq;
using CollectiveMass.ImageTracerUnity.Extensions;
using UnityEngine;

namespace CollectiveMass.ImageTracerUnity
{
    internal class ColorReference
    {
        public Color32 Color { get; }

        public ColorReference(Color32 color)
        {
            Color = color;
        }

        public ColorReference(byte alpha, byte red, byte green, byte blue)
            : this(new Color32(red, green, blue, alpha))
        {
        }

        public byte A => Color.a;
        public byte R => Color.r;
        public byte G => Color.g;
        public byte B => Color.b;

        public int CalculateRectilinearDistance(ColorReference other)
        {
            //return Color.CalculateRectilinearDistance(other.Color);
            return ColorUtils.CalculateRectilinearDistance(Color, other.Color);
        }

        private ColorReference() { }
        public static ColorReference Empty { get; } = new ColorReference();

        // find closest color from palette by measuring (rectilinear) color distance between this pixel and all palette colors
        // In my experience, https://en.wikipedia.org/wiki/Rectilinear_distance works better than https://en.wikipedia.org/wiki/Euclidean_distance
        public ColorReference FindClosest(IReadOnlyList<ColorReference> palette)
        {
            var distance = 256 * 4;
            var paletteColor = palette.First();
            foreach (var color in palette)
            {
                var newDistance = color.CalculateRectilinearDistance(this);
                if (newDistance >= distance) continue;

                distance = newDistance;
                paletteColor = color;
            }
            return paletteColor;
        }

        public string ToSvgString()
        {
            return $"fill=\"rgb({R},{G},{B})\" stroke=\"rgb({R},{G},{B})\" stroke-width=\"1\" opacity=\"{A / 255.0}\" ";
        }
    }
}
