using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace fivenine.UnifiedMaps.Droid
{
    public static class GoogleMapsExtensions
    {
        public static float ToMarkerHue(this Xamarin.Forms.Color color)
        {
            // Uses the solution from http://stackoverflow.com/questions/3732046/how-do-you-get-the-hue-of-a-xxxxxx-colour/3732073#3732073
            float hue = (float)((float)Math.Atan2(1.732050808 * (color.G - color.B), (2 * color.R - color.G - color.B)) * 57.295779513);

            // With a small fix to make sure only positive values are returned.
            if (hue < 0)
                hue += 360;

            return hue;
        }

        public static BitmapDescriptor ToStandardMarkerIcon(this Xamarin.Forms.Color color)
        {
            return BitmapDescriptorFactory.DefaultMarker(color.ToMarkerHue());
        }

        /// <summary>
        /// Convert a <see cref="ImageSource"/> to the native Android <see cref="Bitmap"/>
        /// </summary>
        /// <param name="source">Self instance</param>
        /// <param name="context">Android Context</param>
        /// <returns>The Bitmap</returns>
        public static async Task<Bitmap> ToBitmap(this ImageSource source, Context context)
        {
            if (source is FileImageSource)
            {
                return await new FileImageSourceHandler().LoadImageAsync(source, context);
            }
            if (source is UriImageSource)
            {
                return await new ImageLoaderSourceHandler().LoadImageAsync(source, context);
            }
            if (source is StreamImageSource)
            {
                return await new StreamImagesourceHandler().LoadImageAsync(source, context);
            }
            return null;
        }

        public static void HandleExceptions(this Task other)
        {
            other.ContinueWith((arg) =>
            {
                if (arg.IsCanceled)
                {
                    return;
                }

                if (arg.Exception != null)
                {
                    Exception e = arg.Exception is AggregateException ? arg.Exception.InnerException : arg.Exception;

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        throw e;
                    });
                }
            });
        }
    }
}
