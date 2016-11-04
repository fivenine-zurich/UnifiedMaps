using System;
namespace fivenine.UnifiedMaps
{
    /// <summary>
    /// An <see cref="EventArgs"/> providing a generic playload.
    /// </summary>
    public class MapEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Gets the value of the <see cref="T:MapEventArgs`1"/>.
        /// </summary>
        public T Value { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MapEventArgs`1"/> class.
        /// </summary>
        /// <param name="value">Value.</param>
        public MapEventArgs(T value)
        {
            Value = value;
        }
    }
}
