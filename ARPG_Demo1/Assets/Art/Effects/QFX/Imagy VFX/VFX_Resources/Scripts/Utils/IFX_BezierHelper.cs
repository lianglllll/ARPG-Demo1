using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    public static class IFX_BezierHelper
    {
        public static BezierCurveData GetBezierCurve(Transform[] controlPoints, int segmentCount)
        {
            int numPositions = 0;
            var positions = new Vector3[segmentCount];
            var curveCount = controlPoints.Length / 3;

            for (int j = 0; j < curveCount; j++)
            {
                for (int i = 1; i <= segmentCount; i++)
                {
                    var t = i / (float)segmentCount;
                    var nodeIndex = j * 3;
                    var pixel = CalculateCubicBezierPoint(t, controlPoints[nodeIndex].position, controlPoints[nodeIndex + 1].position, controlPoints[nodeIndex + 2].position, controlPoints[nodeIndex + 3].position);

                    numPositions = j * segmentCount + i;
                    positions[j * segmentCount + (i - 1)] = pixel;
                }

            }

            return new BezierCurveData
            {
                Positions = positions,
                PositionCount = numPositions
            };
        }

        private static Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 p = uuu * p0;
            p += 3 * uu * t * p1;
            p += 3 * u * tt * p2;
            p += ttt * p3;

            return p;
        }

        public class BezierCurveData
        {
            public Vector3[] Positions;
            public int PositionCount;
        }
    }
}
