using System.Collections;
using System.Collections.Generic;

namespace fivenine.UnifiedMaps
{
    /// <summary>
    /// A map polyline annotation.
    /// </summary>
    public class MapPolyline : MapItem, IEnumerable<Position>
    {
        private readonly LinkedList<Position> _items;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapPolyline"/> class.
        /// </summary>
        public MapPolyline()
        {
            _items = new LinkedList<Position>();
        }

        /// <summary>
        /// Adds the specified position to the polyline.
        /// </summary>
        /// <param name="item">The position to add.</param>
        public void Add(Position item)
        {
            _items.AddLast(item);
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