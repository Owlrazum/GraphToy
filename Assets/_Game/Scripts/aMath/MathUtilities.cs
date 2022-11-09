using UnityEngine;

namespace Orazum.Math
{
    public enum ClockOrderType  // Enum added in the end to distinguish between field ClockOrder and Type ClockOrderEnum
    {
        CW,
        AntiCW
    }

    public enum LineEndType
    {
        Start,
        End
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
            ClockOrderType clockOrder = crossProdSign > 0 ? ClockOrderType.CW : ClockOrderType.AntiCW;
            return clockOrder;
        }

        public static ClockOrderType GetTriangleClockOrder(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float val = (p2.y - p1.y) * (p3.x - p2.x) - (p2.x - p1.x) * (p3.y - p2.y);
            return (val >= 0) ? ClockOrderType.CW : ClockOrderType.AntiCW;
        }
    }
}