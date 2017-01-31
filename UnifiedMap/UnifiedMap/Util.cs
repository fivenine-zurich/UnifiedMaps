using System;
using System.Linq;

namespace fivenine.UnifiedMaps
{
    /// <summary>
    /// Utility extension methods.
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Clamps the given value to the specified range.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>The clamped value.</returns>
        public static T Clamp<T>(this T value, T min, T max)
            where T : IComparable<T>
        {
            T result = value;
            if (value.CompareTo(max) > 0)
                result = max;
            if (value.CompareTo(min) < 0)
                result = min;
            return result;
        }

        public static Position FindCenter(params Position[] positions)
        {
            if (positions.Length <= 1)
            {
                return positions.FirstOrDefault();
            }

            double x = 0;
            double y = 0;
            double z = 0;

            foreach (var p in positions)
            {
                var latitude = p.Latitude * Math.PI / 180;
                var longitude = p.Longitude * Math.PI / 180;

                x += Math.Cos(latitude) * Math.Cos(longitude);
                y += Math.Cos(latitude) * Math.Sin(longitude);
                z += Math.Sin(latitude);
            }

            var total = positions.Length;

            x = x / total;
            y = y / total;
            z = z / total;

            var centralLongitude = Math.Atan2(y, x);
            var centralSquareRoot = Math.Sqrt(x * x + y * y);
            var centralLatitude = Math.Atan2(z, centralSquareRoot);

            return new Position(centralLatitude * 180 / Math.PI, centralLongitude * 180 / Math.PI);
        }

    }
}
