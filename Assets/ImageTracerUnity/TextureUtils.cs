using System.Collections.Generic;
using UnityEngine;

namespace ImageTracerUnity
{
    internal static class TextureUtils
    {
        //internal static IEnumerable<ColorReference> TextureToColorReferences(Texture2D pTexture)
        //{
        //    Color32[] colors = pTexture.GetPixels32(0);
        //    for (int i = 0; i < colors.Length; ++i)
        //    {
        //        yield return new ColorReference(colors[i]);
        //    }
        //}

        internal static ColorReference[] TextureToColorReferences(Texture2D pTexture) {
            Color32[] colors = pTexture.GetPixels32(0);
            ColorReference[] output = new ColorReference[colors.Length];
            for (int i = 0; i < colors.Length; ++i) {
                output[i] = new ColorReference(colors[i]);
            }
            return output;
        }

    }
}