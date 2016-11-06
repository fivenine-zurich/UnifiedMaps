using System.Collections;
using System.Collections.Generic;
using Xamarin.Forms;

namespace fivenine.UnifiedMaps
{
    /// <summary>
    /// A map polyline annotation.
    /// </summary>
    public class PolylineOverlay : IPolylineOverlay, IEnumerable<Position>
    {
        private readonly LinkedList<Position> _items;
        private readonly MapRegion _boundingBox;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolylineOverlay"/> class.
        /// </summary>
        public PolylineOverlay()
        {
            _items = new LinkedList<Position>();

            _boundingBox = MapRegion.Empty();

            // Defaults
            LineWidth = 1.0f;
            StrokeColor = Color.Blue;
        }

        /// <summary>
        /// Gets the bounding box of the current <see cref="PolylineOverlay"/>.
        /// </summary>
        /// <value>
        /// The bounding box of the current <see cref="PolylineOverlay"/>.
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

        /// <summary>
        /// Determines whether the specified <see cref="fivenine.UnifiedMaps.IMapAnnotation"/> is equal to the current <see cref="T:fivenine.UnifiedMaps.MapPolyline"/>.
        /// </summary>
        /// <param name="other">The <see cref="fivenine.UnifiedMaps.IMapAnnotation"/> to compare with the current <see cref="T:fivenine.UnifiedMaps.MapPolyline"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="fivenine.UnifiedMaps.IMapAnnotation"/> is equal to the current
        /// <see cref="T:fivenine.UnifiedMaps.MapPolyline"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(IMapAnnotation other)
        {
            var that = other as IPolylineOverlay;
            if (that == null)
            {
                return false;
            }

            return false;
        }
    }
}