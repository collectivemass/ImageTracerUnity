using System;
using System.Collections.Generic;
using CollectiveMass.ImageTracerUnity.OptionTypes;
using CollectiveMass.ImageTracerUnity.Vectorization.Points;
using SplinePointCalculation = System.Func<double, double, double, double, double>;
using CoordMethod = System.Func<double, double>;

namespace CollectiveMass.ImageTracerUnity.Vectorization.Segments
{
    internal class SplineSegment : Segment
    {
        public Point<double> Mid { get; set; }

        private static Point<double> CreateSplinePoint(double pseudoIndex, Point<double> first, Point<double> second, Point<double> third, bool isMidPoint = false)
        {
            // Static Term Calculations
            Func<double, double> t1Calc = t => (1.0 - t) * (1.0 - t);
            Func<double, double> t2Calc = t => 2.0 * (1.0 - t) * t;
            Func<double, double> t3Calc = t => Math.Pow(t, 2);

            // Static Point Calculations
            SplinePointCalculation midPointCalc =
                (i, start, end, fit) => (t1Calc(i) * start + t3Calc(i) * end - fit) / -t2Calc(i);

            SplinePointCalculation finalPointCalc =
                (i, start, mid, end) => t1Calc(i) * start + t2Calc(i) * mid + t3Calc(i) * end;

            Func<double, Point<double>, Point<double>, Point<double>, SplinePointCalculation, Point<double>> createPoint =
                (i, p1, p2, p3, func) => new Point<double>
                {
                    X = func(i, p1.X, p2.X, p3.X),
                    Y = func(i, p1.Y, p2.Y, p3.Y)
                };

            return createPoint(pseudoIndex, first, second, third, isMidPoint ? midPointCalc : finalPointCalc);
        }

        // 5.4. Fit a quadratic spline through this point, measure errors on every point in the sequence
        // helpers and projecting to get control point
        public static Segment Fit(IReadOnlyList<InterpolationPoint> path, double threshold, SequenceIndices sequence, int sequenceLength, ref int errorIndex, SvgRendering rendering)
        {
            var startPoint = path[sequence.Start];
            var endPoint = path[sequence.End];
            var fitPoint = path[errorIndex];

            Func<int, double> pseudoIndexCalc = i => (i - sequence.Start) / (double)sequenceLength;
            var midPoint = CreateSplinePoint(pseudoIndexCalc(errorIndex), startPoint, endPoint, fitPoint, true);

            // Check every point
            var isSpline = Fit(i => path[i], i => CreateSplinePoint(pseudoIndexCalc(i), startPoint, midPoint, endPoint), threshold,
                sequence.Start + 1, i => i != sequence.End, i => (i + 1) % path.Count, ref errorIndex);

            return isSpline ? new SplineSegment { Start = startPoint, Mid = midPoint, End = endPoint, Radius = rendering.quadraticControlPointRadius, RoundDecimalPlaces = rendering.roundCoords } : null;
        }

        public override Segment Scale(double scale)
        {
            Mid = ScalePoint(Mid, scale);
            return base.Scale(scale);
        }

        public override string ToPathString()
        {
            return $"Q {RoundCoordinates(Mid.X)} {RoundCoordinates(Mid.Y)} {RoundCoordinates(End.X)} {RoundCoordinates(End.Y)} ";
        }

        public override string ToControlPointString()
        {
            return
                $"<circle cx=\"{Mid.X}\" cy=\"{Mid.Y}\" r=\"{Radius}\" fill=\"cyan\" stroke-width=\"{Radius * 0.2}\" stroke=\"black\" />" +
                $"<circle cx=\"{End.X}\" cy=\"{End.Y}\" r=\"{Radius}\" fill=\"white\" stroke-width=\"{Radius * 0.2}\" stroke=\"black\" />" +
                $"<line x1=\"{Start.X}\" y1=\"{Start.Y}\" x2=\"{Mid.X}\" y2=\"{Mid.Y}\" stroke-width=\"{Radius * 0.2}\" stroke=\"cyan\" />" +
                $"<line x1=\"{Mid.X}\" y1=\"{Mid.Y}\" x2=\"{End.X}\" y2=\"{End.Y}\" stroke-width=\"{Radius * 0.2}\" stroke=\"cyan\" />";
        }
    }
}
