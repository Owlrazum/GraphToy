using System.Collections.Generic;
using UnityEngine;

namespace RopeNamespace 
{
    public enum ClockOrderEnum  // Enum added in the end to distinguish between field ClockOrder and Type ClockOrderEnum
    {
        None,
        Clockwise,
        CounterClockwise
    }

    public static class EnumConverter
    {
        public static ClockOrderEnum ConvertSignToClockOrder(float crossProdSign)
        {
            ClockOrderEnum clockOrder = crossProdSign > 0 ? ClockOrderEnum.Clockwise : ClockOrderEnum.CounterClockwise;
            return clockOrder;
        }
    }
}
