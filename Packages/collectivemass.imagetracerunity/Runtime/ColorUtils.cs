using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CollectiveMass.ImageTracerUnity
{
    internal static class ColorUtils
    {
        internal static byte[] ToRgbaByteArray(Color[] colors) {
            return colors.Select(c => ToRgbaByteArray(c)).SelectMany(b => b).ToArray();
        }

        internal static byte[] ToRgbaByteArray(Color32 color) {
            return new[] { color.r, color.g, color.b, color.a };
        }

        private static int CalculateRectilinearDistance(Color pFirst, Color pSecond) {
            var firstArray = ToRgbaByteArray(pFirst);
            var secondArray = ToRgbaByteArray(pSecond);

            // weighted alpha seems to help images with transparency
            return firstArray.Zip(
                secondArray,
                (f, s) => Math.Abs(f - s))
                    .Select((d, i) => i == 3 ? d * 4 : d)
                    .Sum();
        }

        // find closest color from palette by measuring (rectilinear) color distance between this pixel and all palette colors
        // In my experience, https://en.wikipedia.org/wiki/Rectilinear_distance works better than https://en.wikipedia.org/wiki/Euclidean_distance
        internal static ColorReference FindClosest(Color pColor, IReadOnlyList<ColorReference> pPalette) {
            var distance = 256 * 4;
            var paletteColor = pPalette.First();
            foreach (var color in pPalette) {
                var newDistance = CalculateRectilinearDistance(pColor, color.Color);
                if (newDistance >= distance) continue;

                distance = newDistance;
                paletteColor = color;
            }
            return paletteColor;
        }

        internal static string ToSvgString(Color32 pColor) {
            return $"fill=\"rgb({pColor.r},{pColor.g},{pColor.b})\" stroke=\"rgb({pColor.r},{pColor.g},{pColor.b})\" stroke-width=\"1\" opacity=\"{pColor.a / 255f}\" ";
        }

    }
}