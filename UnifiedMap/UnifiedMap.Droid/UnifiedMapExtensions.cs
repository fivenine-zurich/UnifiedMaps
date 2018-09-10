using Android.Gms.Maps.Model;

namespace fivenine.UnifiedMaps.Droid
{
    public static class UnifiedMapExtensions
    {
        public static LatLng ToLatLng(this Position position)
        {
            return new LatLng(position.Latitude, position.Longitude);
        }

        public static LatLngBounds ToBounds(this MapRegion region)
        {
            var southwest = new LatLng(region.MinY, region.MinX);
            var northeast = new LatLng(region.MaxY, region.MaxX);

            return new LatLngBounds.Builder().
                                       Include(southwest).
                                       Include(northeast).Build();
        }
    }
}