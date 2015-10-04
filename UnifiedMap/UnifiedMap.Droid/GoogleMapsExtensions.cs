using System;
using Android.Gms.Maps.Model;

namespace fivenine.UnifiedMaps.Droid
{
    public static class GoogleMapsExtensions
    {
        public static float ToMarkerHue(this Xamarin.Forms.Color color)
        {
            // Uses the solution from http://stackoverflow.com/questions/3732046/how-do-you-get-the-hue-of-a-xxxxxx-colour/3732073#3732073
            float hue = (float) ((float) Math.Atan2(1.732050808 * (color.G - color.B), (2 * color.R - color.G - color.B)) * 57.295779513);

            // With a small fix to make sure only positive values are returned.
            if (hue < 0)
                hue += 360;

            return hue;
        }

        public static BitmapDescriptor ToStandardMarkerIcon(this Xamarin.Forms.Color color)
        {
            return BitmapDescriptorFactory.DefaultMarker(color.ToMarkerHue());
        }
    }
}

