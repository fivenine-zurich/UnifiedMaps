using Android.Gms.Maps.Model;

namespace fivenine.UnifiedMaps.Droid
{
    public static class GoogleMapsExtensions
    {
        public static BitmapDescriptor ToStandardMarkerIcon(this PinColor color)
        {
            float hue;

            switch(color)
            {
                case PinColor.Green:
                    hue = BitmapDescriptorFactory.HueGreen;
                    break;
                case PinColor.Purple:
                    hue = BitmapDescriptorFactory.HueAzure;
                    break;
                default:
                    hue = BitmapDescriptorFactory.HueRed;
                    break;
            }


            return BitmapDescriptorFactory.DefaultMarker(hue);
        }
    }
}

