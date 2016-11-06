using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace fivenine.UnifiedMaps
{
    public interface IPolylineOverlay : IMapOverlay, IEnumerable<Position>
    {
        /// <summary>
        /// Gets the bounding box of the current <see cref="PolylineOverlay"/>.
        /// </summary>
        /// <value>
        /// The bounding box of the current <see cref="PolylineOverlay"/>.
        /// </value>
        MapRegion BoundingBox { get; }

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

        /// <summary>
        /// Adds the specified position to the polyline.
        /// </summary>
        /// <param name="item">The position to add.</param>
        void Add(Position item);
    }
}
