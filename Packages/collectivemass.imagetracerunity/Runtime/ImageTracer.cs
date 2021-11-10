using System.Collections.Generic;
using System.Linq;
using CollectiveMass.ImageTracerUnity.OptionTypes;
using CollectiveMass.ImageTracerUnity.Svg;
using CollectiveMass.ImageTracerUnity.Vectorization;
using CollectiveMass.ImageTracerUnity.Vectorization.TraceTypes;
using UnityEngine;

namespace CollectiveMass.ImageTracerUnity
{
    public static class ImageTracer
    {
        //internal static readonly List<ColorReference> Palette = BitmapPalettes.Halftone256.Colors.Select
        //    (c => new ColorReference(c.A, c.R, c.G, c.B)).ToList();

        // We should really derive a custom pallet here or allow color data to be passed to the method
        internal static readonly List<ColorReference> Palette = new List<ColorReference>() {
            new ColorReference(Color.black),
            new ColorReference(Color.white)
        };

        public static string ImageToSvg(Texture2D pImage, Options pOptions)
        {
            return PaddedPaletteImageToTraceData(pImage, pOptions, pOptions.SvgRendering)
                .ToSvgString(pOptions.SvgRendering);
        }

        // Tracing ImageData, then returning PaddedPaletteImage with tracedata in layers
        private static TracedImage PaddedPaletteImageToTraceData(Texture2D pImage, Options pOptions, SvgRendering pRendering)
        {
			// Selective Gaussian blur preprocessing
			if (pOptions.Blur.BlurRadius > 0) {
				pImage = Blur(pImage, pOptions.Blur.BlurRadius, pOptions.Blur.BlurDelta);
			}

			// 1. Color quantization
			//var colors = image.ChangeFormat(PixelFormat.Format32bppArgb).ToColorReferences();
			var colors = TextureUtils.TextureToColorReferences(pImage);

            var colorGroups = ColorGrouping.Convert(colors, pImage.width, pImage.height, Palette);

            // 2. Layer separation and edge detection
            var rawLayers = Layering.Convert(colorGroups, pImage.width, pImage.height, Palette);

            // 3. Batch pathscan
            var pathPointLayers = rawLayers.ToDictionary(cl => cl.Key, cl => new Layer<PathPointPath>
            {
                Paths = Pathing.Scan(cl.Value, pOptions.Tracing.PathOmit).ToList()
            });

            // 4. Batch interpollation
            var interpolationPointLayers = pathPointLayers.ToDictionary(cp => cp.Key, cp => Interpolation.Convert(cp.Value));

            // 5. Batch tracing
            var sequenceLayers = interpolationPointLayers.ToDictionary(ci => ci.Key, ci => new Layer<SequencePath>
            {
                Paths = ci.Value.Paths.Select(path => new SequencePath
                {
                    Path = path,
                    Sequences = Sequencing.Create(path.Points.Select(p => p.Direction).ToList()).ToList()
                }).ToList()
            });

            var segmentLayers = sequenceLayers.ToDictionary(ci => ci.Key, ci => new Layer<SegmentPath>
            {
                Paths = ci.Value.Paths.Select(path => new SegmentPath
                {
                    Segments = path.Sequences.Select(s => Segmentation.Fit(path.Path.Points, s, pOptions.Tracing, pRendering)).SelectMany(s => s).ToList()
                }).ToList()
            });

            return new TracedImage(segmentLayers, pImage.width, pImage.height);
        }

		// Gaussian kernels for blur
		private static readonly float[][] Gks =
		{
			new []{0.27901f, 0.44198f, 0.27901f},
			new []{0.135336f, 0.228569f, 0.272192f, 0.228569f, 0.135336f},
			new []{0.086776f, 0.136394f, 0.178908f, 0.195843f, 0.178908f, 0.136394f, 0.086776f},
			new []{0.063327f, 0.093095f, 0.122589f, 0.144599f, 0.152781f, 0.144599f, 0.122589f, 0.093095f, 0.063327f},
			new []{0.049692f, 0.069304f, 0.089767f, 0.107988f, 0.120651f, 0.125194f, 0.120651f, 0.107988f, 0.089767f, 0.069304f, 0.049692f}
		};

		// Selective Gaussian blur for preprocessing
		public static Texture2D Blur(Texture2D pImage, int pRadius, float del)
		{
			int i, j, k;
			int idx;
			float racc, gacc, bacc, aacc, wacc;

			// radius and delta limits, this kernel
			var radius = pRadius;
			if (radius < 1) { return pImage; }
			if (radius > 5) { radius = 5; }

			var delta = (int)Mathf.Abs(del);
			if (delta > 1024) { delta = 1024; }

			var thisgk = Gks[radius - 1];

			Color[] source = pImage.GetPixels();
			Color[] destination = new Color[source.Length];

			// loop through all pixels, horizontal blur
			for (j = 0; j < pImage.height; j++)
			{
				for (i = 0; i < pImage.width; i++)
				{
					racc = 0; gacc = 0; bacc = 0; aacc = 0; wacc = 0;

					// gauss kernel loop
					for (k = -radius; k < radius + 1; k++)
					{
						// add weighted color values
						if ((i + k > 0) && (i + k < pImage.width))
						{
							idx = (j * pImage.width + i + k);
							racc += source[idx].r * thisgk[k + radius];
							gacc += source[idx].g * thisgk[k + radius];
							bacc += source[idx].b * thisgk[k + radius];
							aacc += source[idx].a * thisgk[k + radius];
							wacc += thisgk[k + radius];
						}
					}
					// The new pixel
					idx = (j * pImage.width + i);
					destination[idx].r = racc / wacc;
					destination[idx].g = gacc / wacc;
					destination[idx].b = bacc / wacc;
					destination[idx].a = aacc / wacc;

				}
			}

			// copying the half blurred imgd2
			Color[] himgd = destination.Clone() as Color[];

			// loop through all pixels, vertical blur
			for (j = 0; j < pImage.height; j++)
			{
				for (i = 0; i < pImage.width; i++)
				{
					racc = 0; gacc = 0; bacc = 0; aacc = 0; wacc = 0;
					// gauss kernel loop
					for (k = -radius; k < radius + 1; k++)
					{
						// add weighted color values
						if ((j + k > 0) && (j + k < pImage.height))
						{
							idx = ((j + k) * pImage.width + i);
							racc += himgd[idx].r * thisgk[k + radius];
							gacc += himgd[idx].g * thisgk[k + radius];
							bacc += himgd[idx].b * thisgk[k + radius];
							aacc += himgd[idx].a * thisgk[k + radius];
							wacc += thisgk[k + radius];
						}
					}
					// The new pixel
					idx = (j * pImage.width + i);
					destination[idx].r = racc / wacc;
					destination[idx].g = gacc / wacc;
					destination[idx].b = bacc / wacc;
					destination[idx].a = aacc / wacc;
				}
			}

			// Selective blur: loop through all pixels
			for (j = 0; j < pImage.height; j++) {
				for (i = 0; i < pImage.width; i++) {
					idx = j * pImage.width + i;

					// d is the difference between the blurred and the original pixel
					var d = Mathf.Abs(destination[idx].r - source[idx].r)
							+ Mathf.Abs(destination[idx].g - source[idx].g)
							+ Mathf.Abs(destination[idx].b - source[idx].b)
							+ Mathf.Abs(destination[idx].a - source[idx].a);

					// selective blur: if d>delta, put the original pixel back
					if (d > delta) {
						destination[idx].r = source[idx].r;
						destination[idx].g = source[idx].g;
						destination[idx].b = source[idx].b;
						destination[idx].a = source[idx].a;
					}
				}
			}

			var imgd2 = new Texture2D(pImage.width, pImage.height, TextureFormat.RGBA32, false);
			imgd2.SetPixels(destination);
			imgd2.Apply();

			return imgd2;
		}
	}
}
