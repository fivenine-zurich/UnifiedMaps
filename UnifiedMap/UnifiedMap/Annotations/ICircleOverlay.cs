using System;
using Xamarin.Forms;

namespace fivenine.UnifiedMaps
{
    public interface ICircleOverlay : IMapOverlay
    {
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
        /// Gets the fill color of the circle.
        /// </summary>
        /// <value>The fill color.</value>
        Color FillColor { get; }

        /// <summary>
        /// Gets the radius.
        /// </summary>
        /// <value>The radius.</value>
        float Radius { get; }

        /// <summary>
        /// Gets or sets the color of the stroke.
        /// </summary>
        /// <value>
        /// The color of the stroke.
        /// </value>
        Color StrokeColor { get; }

        /// <summary>
        /// Gets or sets the stroke thickness.
        /// </summary>
        /// <value>
        /// The stroke thickness.
        /// </value>
        float LineWidth { get; }
    }
}
