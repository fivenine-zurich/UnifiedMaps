using System.Threading.Tasks;
using CoreLocation;
using MapKit;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

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

        /// <summary>
        /// Converts an <see cref="ImageSource"/> to the native iOS <see cref="UIImage"/>
        /// </summary>
        /// <param name="source">Self intance</param>
        /// <returns>The UIImage</returns>
        public static async Task<UIImage> ToImage(this ImageSource source)
        {
            if (source is FileImageSource)
            {
                return await new FileImageSourceHandler().LoadImageAsync(source);
            }

            if (source is UriImageSource)
            {
                return await new ImageLoaderSourceHandler().LoadImageAsync(source);
            }

            if (source is StreamImageSource)
            {
                return await new StreamImagesourceHandler().LoadImageAsync(source);
            }

            return null;
        }
    }
}