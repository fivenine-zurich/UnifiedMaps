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
        /// Initializes a new instance of the <see cref="T:fivenine.UnifiedMaps.MapPin"/> class.
        /// </summary>
        public MapPin()
        {
            Color = Color.Red;
            SelectedColor = Color;
        }

        /// <summary>
        /// Gets the title of the pin.
        /// </summary>
        /// <value>
        /// The title of the pin.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets the snippet (sub-title) of the map pin callout window.
        /// </summary>
        /// <value>
        /// The snippet text.
        /// </value>
        public string Snippet { get; set; }

        /// <summary>
        /// Gets the location of the pin.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        public Position Location { get; set; }

        /// <summary>
        /// Gets the color of the map pin.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public Color Color { get; set; }

        /// <summary>
        /// Gets the color for the selected state.
        /// </summary>
        /// <value>
        /// The color of a selected map pin.
        /// </value>
        public Color SelectedColor { get; set; }

        /// <summary>
        /// Gets the pin annotation image or <c>null</c> if not used.
        /// </summary>
        /// <value>The image source.</value>
        public ImageSource Image { get; set; }

        /// <summary>
        /// Gets the pin annotation image for the selected state; or <c>null</c> if not used.
        /// </summary>
        /// <value>The image source for the selected state.</value>
        public ImageSource SelectedImage { get; set; }

        /// <summary>
        /// Gets or sets the anchor point of the map pin.
        /// </summary>
        /// <value>The anchor point.</value>
        public Point Anchor { get; set; }

        /// <summary>
        /// Gets or sets the Z index.
        /// </summary>
        /// <value>The Z index.</value>
        public int ZIndex { get; set; }

        /// <summary>
        /// Determines whether the specified <see cref="IMapAnnotation"/> is equal to the current <see cref="T:fivenine.UnifiedMaps.MapPin"/>.
        /// </summary>
        /// <param name="other">The <see cref="IMapAnnotation"/> to compare with the current <see cref="T:fivenine.UnifiedMaps.MapPin"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="IMapAnnotation"/> is equal to the current
        /// <see cref="T:fivenine.UnifiedMaps.MapPin"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(IMapAnnotation other)
        {
            var that = other as IMapPin;
            if (that == null)
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