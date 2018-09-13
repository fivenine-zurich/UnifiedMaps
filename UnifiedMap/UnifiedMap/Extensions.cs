using System;

namespace fivenine.UnifiedMaps
{
    public static class Extensions
    {
        public static bool EqualsSafe(this object a, object b)
        {
            if (a != null)
            {
                return a.Equals(b);
            }

            if (b != null)
            {
                return b.Equals(a);
            }

            // Both are null -> so equals
            return true;
        }

        public static Position GetPositionFromAngle(this Position center, double distance, double angle)
        {
            var earthRadius = 6371000;
            var lonRads = ToRadians(center.Longitude);
            var latRads = ToRadians(center.Latitude);
            var bearingRads = ToRadians(angle);

            var maxLatRads = Math.Asin(Math.Sin(latRads) * Math.Cos(distance / earthRadius) + Math.Cos(latRads) * Math.Sin(distance / earthRadius) * Math.Cos(bearingRads));
            var maxLonRads = lonRads + Math.Atan2((Math.Sin(bearingRads) * Math.Sin(distance / earthRadius) * Math.Cos(latRads)), (Math.Cos(distance / earthRadius) - Math.Sin(latRads) * Math.Sin(maxLatRads)));

            var maxLat = ToDegrees(maxLatRads);
            var maxLong = ToDegrees(maxLonRads);

            return new Position(maxLat, maxLong);
        }

        public static double ToRadians(double val)
        {
            return (Math.PI / 180) * val;
        }

        private static double ToDegrees(double angle)
        {
            return angle * (180.0 / Math.PI);
        }
    }
}
