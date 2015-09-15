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
            var southwest = new LatLng(region.MinX, region.MinY);
            var northeast = new LatLng(region.MaxX, region.MaxY);

            return new LatLngBounds(southwest, northeast);
        }
    }
}