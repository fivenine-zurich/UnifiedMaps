using Xamarin.Forms;

namespace fivenine.UnifiedMaps
{
    /// <summary>
    /// A definition of a map pin annotation.
    /// </summary>
    public interface IMapPin : IMapAnnotation
    {
        /// <summary>
        /// Gets the title of the pin.
        /// </summary>
        /// <value>
        /// The title of the pin.
        /// </value>
        string Title { get; }

        /// <summary>
        /// Gets the snippet (sub-title) of the map pin callout window.
        /// </summary>
        /// <value>
        /// The snippet text.
        /// </value>
        string Snippet { get; }

        /// <summary>
        /// Gets the location of the pin.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        Position Location { get; }

        /// <summary>
        /// Gets the color of the map pin.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        Color Color { get; }

        /// <summary>
        /// Gets the color for the selected state.
        /// </summary>
        /// <value>
        /// The color of a selected map pin.
        /// </value>
        Color SelectedColor { get; }

        /// <summary>
        /// Gets the pin annotation image or <c>null</c> if not used.
        /// </summary>
        /// <value>The image source.</value>
        ImageSource Image { get; }

        /// <summary>
        /// Gets the pin annotation image for the selected state; or <c>null</c> if not used.
        /// </summary>
        /// <value>The image source for the selected state.</value>
        ImageSource SelectedImage { get; }

        /// <summary>
        /// Gets the anchor point of the map pin.
        /// </summary>
        /// <value>The anchor point.</value>
        Point Anchor { get; }
    }
}
