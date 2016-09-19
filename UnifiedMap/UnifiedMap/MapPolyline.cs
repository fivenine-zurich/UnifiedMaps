using System.Collections;
using System.Collections.Generic;
using Xamarin.Forms;

namespace fivenine.UnifiedMaps
{
    /// <summary>
    /// A map polyline annotation.
    /// </summary>
    public class MapPolyline : IMapAnnotation, IEnumerable<Position>
    {
        private readonly LinkedList<Position> _items;
        private readonly MapRegion _boundingBox;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapPolyline"/> class.
        /// </summary>
        public MapPolyline()
        {
            _items = new LinkedList<Position>();

            _boundingBox = MapRegion.Empty();

            // Defaults
            Alpha = 1.0f;
            LineWidth = 1.0f;
            StrokeColor = Color.Blue;
        }

        /// <summary>
        /// Gets the bounding box of the current <see cref="MapPolyline"/>.
        /// </summary>
        /// <value>
        /// The bounding box of the current <see cref="MapPolyline"/>.
        /// </value>
        public MapRegion BoundingBox => _boundingBox;

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
        /// Gets or sets the alpha.
        /// </summary>
        /// <value>
        /// The alpha.
        /// </value>
        public float Alpha { get; set; }

        /// <summary>
        /// Adds the specified position to the polyline.
        /// </summary>
        /// <param name="item">The position to add.</param>
        public void Add(Position item)
        {
            _items.AddLast(item);
            _boundingBox.Include(item);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<Position> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}