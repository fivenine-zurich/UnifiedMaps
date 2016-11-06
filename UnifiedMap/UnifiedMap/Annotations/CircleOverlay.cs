using System;
using Xamarin.Forms;

namespace fivenine.UnifiedMaps
{
    public class CircleOverlay : ICircleOverlay
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:fivenine.UnifiedMaps.CircleOverlay"/> class.
        /// </summary>
        public CircleOverlay()
        {
            // Defaults
            LineWidth = 1.0f;
            StrokeColor = Color.Blue;
            FillColor = Color.Transparent;
        }

        /// <summary>
        /// Gets the location of the circle.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        public Position Location { get; set; }

        /// <summary>
        /// Gets the color of the circle.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public Color Color { get; set; }

        /// <summary>
        /// Gets the fill color of the circle.
        /// </summary>
        /// <value>
        /// The fill color.
        /// </value>
        public Color FillColor { get; set; }

        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        /// <value>The radius.</value>
        public float Radius { get; set; }

        /// <summary>
        /// Gets or sets the color of the stroke.
        /// </summary>
        /// <value>
        /// The color of the stroke.
        /// </value>
        public Color StrokeColor { get; set; }

        /// <summary>
        /// Gets or sets the stroke thickness.
        /// </summary>
        /// <value>
        /// The stroke thickness.
        /// </value>
        public float LineWidth { get; set; }

        /// <summary>
        /// Determines whether the specified <see cref="IMapAnnotation"/> is equal to the current <see cref="T:fivenine.UnifiedMaps.CircleOverlay"/>.
        /// </summary>
        /// <param name="other">The <see cref="IMapAnnotation"/> to compare with the current <see cref="T:fivenine.UnifiedMaps.CircleOverlay"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="IMapAnnotation"/> is equal to the current
        /// <see cref="T:fivenine.UnifiedMaps.CircleOverlay"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(IMapAnnotation other)
        {
            var that = other as ICircleOverlay;
            if (that == null)
            {
                return false;
            }

            return Location == that.Location 
               && Color == that.Color 
               && Math.Abs(Radius - that.Radius) < float.Epsilon;
        }
    }
}
