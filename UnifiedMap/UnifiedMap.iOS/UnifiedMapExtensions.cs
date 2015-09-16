using CoreLocation;
using MapKit;

namespace fivenine.UnifiedMaps.iOS
{
    public static class UnifiedMapExtensions
    {
        public static CLLocationCoordinate2D ToCoordinate(this Position pos)
        {
            return new CLLocationCoordinate2D(pos.Latitude, pos.Longitude);
        }

        public static MKCoordinateSpan ToSpan(this MapRegion region)
        {
            var lonDegrees = region.MaxX - region.MinX;
            var latDegrees = region.MaxY - region.MinY;

            return new MKCoordinateSpan(latDegrees, lonDegrees);
        }
    }
}