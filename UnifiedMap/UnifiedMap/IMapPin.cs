using System;
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
    }
}
