using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CollectiveMass.ImageTracerUnity.Palettes
{
    internal static class SmartPalette
    {
        public static Color[] Generate(Texture2D pImage, int pRows = 4, int pColumns = 4)
        {
            var blurred = BlurImage(pImage);
            var blocks = DivideImage(blurred, pRows, pColumns);
            return blocks.Select(AverageImageColor).ToArray();
        }

        private static Texture2D BlurImage(Texture2D pImage)
        {
            var blurredImage = new Texture2D(pImage.width, pImage.height);
            var gaussianBlur = new GaussianBlur();
            var rectangle = new RectInt(0, 0, pImage.width, pImage.height);
            gaussianBlur.Apply(blurredImage, pImage, rectangle, 0, pImage.height - 1);
            return blurredImage;
        }

        private static IEnumerable<Texture2D> DivideImage(Texture2D image, int rows = 4, int columns = 4)
        {
            // Will lose pixels because of naive divison when size doesn't divide evently.
            var blockHeight = image.height / rows;
            var blockWidth = image.width / columns;
            for (var i = 0; i < rows; ++i)
            {
                for (var j = 0; j < columns; ++j)
                {
                    var colorData = image.GetPixels(j * blockWidth, i * blockHeight, blockWidth, blockHeight);
                    var imageSlice = new Texture2D(blockWidth, blockHeight, image.format, false);
                    imageSlice.SetPixels(colorData);
                    imageSlice.Apply();
                    yield return imageSlice;
                    //yield return image.Clone(rectangle, image.format);
                }
            }
        }

        private static Color AverageImageColor(Texture2D image)
        {
            float RTotal = 0;
            float GTotal = 0;
            float BTotal = 0;
            float ATotal = 0;
            for (var i = 0; i < image.width; ++i)
            {
                for (var j = 0; j < image.height; ++j)
                {
                    var pixel = image.GetPixel(i, j);
                    RTotal += pixel.r;
                    GTotal += pixel.g;
                    BTotal += pixel.b;
                    ATotal += pixel.a;
                }
            }
            float totalPixels = image.width * image.height;
            return new Color(
                RTotal / totalPixels,
                GTotal / totalPixels,
                BTotal / totalPixels,
                ATotal / totalPixels
            );
        }
    }
}
