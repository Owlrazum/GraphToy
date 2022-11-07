using UnityEngine;

namespace Orazum.Math
{
    public enum ClockOrderType  // Enum added in the end to distinguish between field ClockOrder and Type ClockOrderEnum
    {
        Clockwise,
        CounterClockwise
    }

    public static class MathUtilities
    {
        public static float GetPointLineSegmentDistance(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            return Vector3.Cross((p0 - p1), (p0 - p2)).magnitude / (p2 - p1).magnitude;
        }

        public static float GetCrossProdSign(Vector3 lhs, Vector3 rhs)
        {
            return Mathf.Sign(Vector3.Cross(lhs, rhs).y);
        }

        public static ClockOrderType ConvertSignToClockOrder(float crossProdSign)
        {
            ClockOrderType clockOrder = crossProdSign > 0 ? ClockOrderType.Clockwise : ClockOrderType.CounterClockwise;
            return clockOrder;
        }
    }
}