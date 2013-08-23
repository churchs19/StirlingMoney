using System;

namespace Shane.Church.Utility.Core.Extensions
{
    public static class RadianMathExtensions
    {
        public static double DegreeToRadian(this double angle)
        {
            return Math.PI * angle / 180.0;
        }

        public static double RadianToDegree(this double angle)
        {
            return angle * (180.0 / Math.PI);
        }
    }
}
