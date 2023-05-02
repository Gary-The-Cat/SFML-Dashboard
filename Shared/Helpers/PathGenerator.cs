using SFML.System;
using Shared.DataStructures;
using Shared.Maths.Spline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared.Helpers
{
    public static class PathGenerator
    {
        /// <summary>
        /// Generates a path using linear interpolation between the points provided.
        /// </summary>
        /// <param name="points">The waypoints along the desired path.</param>
        /// <param name="numPoints">The number of points in the output path.</param>
        /// <returns></returns>
        public static List<Vector2f> GenerateLinearPath(List<Vector2f> points, int numPoints)
        {
            var output = new List<Vector2f>();

            // Get all segments
            var segments = new List<LineSegment>();
            for(int i = 0; i < points.Count - 1; i++)
            {
                segments.Add(new LineSegment(points[i], points[i + 1]));
            }

            var totalSegmentLength = segments.Sum(s => s.GetMagnitude());

            var lerpDistance = totalSegmentLength / numPoints;

            output.Add(points.First());

            foreach (var segment in segments)
            {
                var stepsInSegment = segment.GetMagnitude() / lerpDistance;
                var unitStep = segment.NormalizedDirection;

                for (int step = 0; step < stepsInSegment; step++)
                {
                    var previousStep = output.Last();
                    Vector2f newStep;
                    if (stepsInSegment - step > 1)
                    {
                        newStep = previousStep + lerpDistance * unitStep;
                    }
                    else
                    {
                        newStep = segment.End;
                    }

                    output.Add(newStep);
                }
            }

            return output;
        }

        public static List<Vector2f> GenerateNaturalSplinePath(List<Vector2f> points, int numPoints)
        {
            var output = new List<Vector2f>();

            // Create the data to be fitted
            var x = points.Select(p => p.X).ToArray();
            var y = points.Select(p => p.Y).ToArray();

            CubicSpline.FitParametric(x, y, numPoints, out var xs, out var ys);

            for (int i = 0; i < numPoints; i++)
            {
                output.Add(new Vector2f(xs[i], ys[i]));
            }

            return output;
        }
    }
}
