using System.Collections.Generic;
using System.Linq;
using System.Text;
using CollectiveMass.ImageTracerUnity.OptionTypes;
using CollectiveMass.ImageTracerUnity.Vectorization.Segments;

namespace CollectiveMass.ImageTracerUnity.Svg
{
    internal static class SvgGeneration
    {
        // Converting tracedata to an SVG string, paths are drawn according to a Z-index
        // the optional lcpr and qcpr are linear and quadratic control point radiuses
        public static string ToSvgString(this TracedImage image, SvgRendering options)
        {
            // SVG start
            var scaledWidth = (int)(image.Width * options.scale);
            var scaledHeight = (int)(image.Height * options.scale);
            var viewBoxOrViewPort = options.viewbox ?
                $"viewBox=\"0 0 {scaledWidth} {scaledHeight}\"" :
                $"width=\"{scaledWidth}\" height=\"{scaledHeight}\"";

            var stringBuilder = new StringBuilder($"<svg {viewBoxOrViewPort} version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" >");
            // Creating Z-index
            // Only selecting the first segment of each path for sorting.
            // Sorting Z-index is not required, TreeMap is sorted automatically
            return image.Layers
                .SelectMany(cs => cs.Value.Paths.Select(p =>
                {
                    var firstSegmentStart = p.Segments.First().Start;
                    var label = firstSegmentStart.Y * scaledWidth + firstSegmentStart.X;
                    return new ZPosition { label = label, color = cs.Key, path = p };
                })).OrderBy(z => z.label)
                .Aggregate(stringBuilder, (sb, z) =>
                {
                    var scaledSegments = z.path.Segments.Select(s => s.Scale(options.scale)).ToList();
                    return AppendSegments(sb, scaledSegments, z.color);
                }).Append("</svg>").ToString();
        }

        // Getting SVG path element string from a traced path
        internal static StringBuilder AppendSegments(StringBuilder stringBuilder, IReadOnlyList<Segment> segments, ColorReference color)
        {
            // Path
            stringBuilder.Append($"<path {ColorUtils.ToSvgString(color.Color)}d=\"M {segments.First().Start.X} {segments.First().Start.Y} ");
            //http://stackoverflow.com/a/217814/294804
            segments.Aggregate(stringBuilder, (sb, segment) => sb.Append(segment.ToPathString())).Append("Z\" />");

            // Rendering control points
            return segments.Where(s => s.Radius > 0).Aggregate(stringBuilder, (sb, segment) => sb.Append(segment.ToControlPointString()));
        }
    }
}
