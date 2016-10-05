using System;
using Xamarin.Forms;

namespace fivenine.UnifiedMaps
{
    /// <summary>
    /// A map pin annotation.
    /// </summary>
    public class MapPin : IMapPin
    {
        /// <summary>
        /// Gets or sets the title of the MapPin.
        /// </summary>
        /// <value>
        /// The title of the pin.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the snippet (sub-title) of the MapPin callout window.
        /// </summary>
        /// <value>
        /// The snippet text.
        /// </value>
        public string Snippet { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        public Position Location { get; set; }

        /// <summary>
        /// Gets or sets the color of the MapPin.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public Color Color { get; set; }

        /// <summary>
        /// Determines whether the specified <see cref="fivenine.UnifiedMaps.IMapAnnotation"/> is equal to the current <see cref="T:fivenine.UnifiedMaps.MapPin"/>.
        /// </summary>
        /// <param name="other">The <see cref="fivenine.UnifiedMaps.IMapAnnotation"/> to compare with the current <see cref="T:fivenine.UnifiedMaps.MapPin"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="fivenine.UnifiedMaps.IMapAnnotation"/> is equal to the current
        /// <see cref="T:fivenine.UnifiedMaps.MapPin"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(IMapAnnotation other)
        {
            var that = other as IMapPin;
            if(that == null)
            {
                return false;
            }

            return Location == that.Location &&
               string.Compare(Title, that.Title, StringComparison.OrdinalIgnoreCase) == 0 &&
               string.Compare(Snippet, that.Snippet, StringComparison.OrdinalIgnoreCase) == 0 &&
               Color == that.Color;
        }
    }
}