using System.Collections.Generic;
using System.Linq;
using CollectiveMass.ImageTracerUnity.Svg;
using CollectiveMass.ImageTracerUnity.Vectorization;
using CollectiveMass.ImageTracerUnity.Vectorization.TraceTypes;
using UnityEngine;

namespace CollectiveMass.ImageTracerUnity {
	public static class ImageTracer {
		//internal static readonly List<ColorReference> Palette = BitmapPalettes.Halftone256.Colors.Select
		//    (c => new ColorReference(c.A, c.R, c.G, c.B)).ToList();

		// We should really derive a custom pallet here or allow color data to be passed to the method
		internal static readonly List<ColorReference> Palette = new List<ColorReference>() {
			new ColorReference(Color.black),
			new ColorReference(Color.white)
		};

		public static string ImageToSvg(Texture2D pImage, Options pOptions) {
			return PaddedPaletteImageToTraceData(pImage, pOptions)
				.ToSvgString(pOptions.svgRendering);
		}

		// Tracing ImageData, then returning PaddedPaletteImage with tracedata in layers
		private static TracedImage PaddedPaletteImageToTraceData(Texture2D pImage, Options pOptions) {

			// Selective Gaussian blur preprocessing
			if (pOptions.blur.blurRadius > 0) {
				pImage = TextureUtils.SelectiveGaussianBlur(pImage, pOptions.blur.blurRadius, pOptions.blur.blurDelta);
			}

			// 1. Color quantization
			//var colors = image.ChangeFormat(PixelFormat.Format32bppArgb).ToColorReferences();
			var colors = TextureUtils.TextureToColorReferences(pImage);

			var colorGroups = ColorGrouping.Convert(colors, pImage.width, pImage.height, Palette);


			// 2. Layer separation and edge detection
			var rawLayers = Layering.Convert(colorGroups, pImage.width, pImage.height, Palette);


			// 3. Batch pathscan
			var pathPointLayers = rawLayers.ToDictionary(cl => cl.Key, cl => new Layer<PathPointPath> {
				Paths = Pathing.Scan(cl.Value, pOptions.tracing.pathOmit).ToList()
			});


			// 4. Batch interpollation
			var interpolationPointLayers = pathPointLayers.ToDictionary(cp => cp.Key, cp => Interpolation.Convert(cp.Value));


			// 5. Batch tracing
			var sequenceLayers = interpolationPointLayers.ToDictionary(ci => ci.Key, ci => new Layer<SequencePath> {
				Paths = ci.Value.Paths.Select(path => new SequencePath {
					Path = path,
					Sequences = Sequencing.Create(path.Points.Select(p => p.Direction).ToList()).ToList()
				}).ToList()
			});

			var segmentLayers = sequenceLayers.ToDictionary(ci => ci.Key, ci => new Layer<SegmentPath> {
				Paths = ci.Value.Paths.Select(path => new SegmentPath {
					Segments = path.Sequences.Select(s => Segmentation.Fit(path.Path.Points, s, pOptions.tracing, pOptions.svgRendering)).SelectMany(s => s).ToList()
				}).ToList()
			});

			return new TracedImage(segmentLayers, pImage.width, pImage.height);
		}
	}
}
