using System;
using System.Linq;
using UnityEngine;

namespace ImageTracerUnity
{
    internal static class ColorUtils
    {
        internal static byte[] ToRgbaByteArray(Color[] colors) {
            return colors.Select(c => ToRgbaByteArray(c)).SelectMany(b => b).ToArray();
        }

        internal static byte[] ToRgbaByteArray(Color32 color) {
            return new[] { color.r, color.g, color.b, color.a };
        }

        internal static int CalculateRectilinearDistance(Color pFirst, Color pSecond) {
            var firstArray = ToRgbaByteArray(pFirst);
            var secondArray = ToRgbaByteArray(pSecond);

            // weighted alpha seems to help images with transparency
            return firstArray.Zip(
                secondArray,
                (f, s) => Math.Abs(f - s))
                    .Select((d, i) => i == 3 ? d * 4 : d)
                    .Sum();
        }

    }
}