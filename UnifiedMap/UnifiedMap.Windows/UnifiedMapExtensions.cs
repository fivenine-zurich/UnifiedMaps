using Windows.Devices.Geolocation;

namespace fivenine.UnifiedMaps.Windows
{
    public static class UnifiedMapExtensions
    {
        public static BasicGeoposition Convert(this Position pos)
        {
            return new BasicGeoposition
            {
                Latitude = pos.Latitude,
                Longitude = pos.Longitude
            };
        }
    }
}
