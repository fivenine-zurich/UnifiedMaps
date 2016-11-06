using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace fivenine.UnifiedMaps
{
    /// <summary>
    /// Defines a map region.
    /// </summary>
    public class MapRegion
    {
        private Position _topLeft;
        private Position _bottomRight;

        private Position _center;
        private double _width;
        private double _height;

        /// <summary>
        /// Creates a new map region that includes the specified positons.
        /// </summary>
        /// <param name="positions">The positions.</param>
        /// <returns>A new MapRegion instance.</returns>
        public static MapRegion FromPositions(IEnumerable<Position> positions)
        {
            bool hasValues = false;

            double maxX, maxY, minX, minY;

            maxX = maxY = double.MinValue;
            minX = minY = double.MaxValue;

            foreach (var position in positions)
            {
                hasValues = true;

                maxX = Math.Max(maxX, position.Longitude);
                minX = Math.Min(minX, position.Longitude);
                maxY = Math.Max(maxY, position.Latitude);
                minY = Math.Min(minY, position.Latitude);
            }

            return hasValues ? new MapRegion(minX, maxY, maxX, minY) : World();
        }

        /// <summary>
        /// Returns an empty map region.
        /// </summary>
        /// <returns>The smallest possible map region.</returns>
        public static MapRegion Empty() => new MapRegion(0, 0, 0, 0);

        /// <summary>
        /// Creates a map region that includes the whole world.
        /// </summary>
        /// <returns>The largest possible map region.</returns>
        public static MapRegion World() => new MapRegion(-180, 90, 180, -90);

        /// <summary>
        /// Discribes a rectangular region. This region usually encloses a set of geometries or represents a area of view.
        /// </summary>
        /// <param name="topLeft">The top left coordinates of the bounding box.</param>
        /// <param name="bottomRight">The bottom right coordinate of the bounding box.</param>
        public MapRegion(Position topLeft, Position bottomRight)
            : this( topLeft.Longitude, topLeft.Latitude, bottomRight.Longitude, bottomRight.Latitude )
        {
        }

        /// <summary>
        /// Discribes a rectangular region. This region usually encloses a set of geometries or represents a area of view.
        /// </summary>
        /// <param name="center">Center of the bounding box</param>
        /// <param name="width">Width of bounding box in degress</param>
        /// <param name="height">Height of bounding box in degress</param>
        public MapRegion(Position center, double width, double height)
            : this(
                  center.Longitude - width/2, 
                  center.Latitude + height/2, 
                  center.Longitude + width/2,
                  center.Latitude - height/2)
        {
        }

        /// <summary>
        /// Discribes a rectangular region. This region usually encloses a set of geometries or represents a area of view.
        /// </summary>
        /// <param name="minX">Mininium X value (longitude), left most coordinate.</param>
        /// <param name="maxY">Maximium Y value (laitude), northern most coordinate.</param>
        /// <param name="maxX">Maximium X value (longitude), right most coordinate.</param>
        /// <param name="minY">Minimium Y value (latitude), southern most coordinate.</param>
        public MapRegion(double minX, double maxY, double maxX, double minY)
        {
            var lminX = ClampWidth(Math.Min(minX, maxX));
            var lmaxX = ClampWidth(Math.Max(minX, maxX));

            var lminY = ClampHeight(Math.Min(minY, maxY));
            var lmaxY = ClampHeight(Math.Max(minY, maxY));

            _topLeft = new Position(lmaxY, lminX);
            _bottomRight = new Position(lminY, lmaxX);

            RecalculateDimensionsAndCenter();
        }

        /// <summary>
        /// The Center Coordinate.
        /// </summary>
        public Position Center => _center;

        /// <summary>
        /// North Latitude Coordinate
        /// </summary>
        public double MaxY => _topLeft.Latitude;

        /// <summary>
        /// South Latitude Coordinate
        /// </summary>
        public double MinY => _bottomRight.Latitude;

        /// <summary>
        /// Most Easterly Longitude Coordinate (right side of bounding box)
        /// </summary>
        public double MaxX => _bottomRight.Longitude;

        /// <summary>
        /// Most Westerly Longitude Coordinate (left side of bounding box)
        /// </summary>
        public double MinX => _topLeft.Longitude;

        /// <summary>
        /// Gets the top left coordinate of the bounding box
        /// </summary>
        public Position TopLeft => _topLeft;

        /// <summary>
        /// Gets the bottom right coordinate of the bounding box
        /// </summary>
        public Position BottomRight => _bottomRight;

        /// <summary>
        /// Inflates the current region with the given dimensions.
        /// </summary>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        /// <returns>The new <see cref="MapRegion"/> instance.</returns>
        public MapRegion Inflate(float height, float width) => new MapRegion(_center, _width + width, _height + height);

        public void Include(Position item)
        {
            if (Contains(item) == false)
            {
                var lMinX = Math.Min(MinX, item.Longitude);
                var lMaxX = Math.Max(MaxX, item.Longitude);

                var lMinY = Math.Min(MinY, item.Latitude);
                var lMaxY = Math.Max(MaxY, item.Latitude);

                var newRegion = new MapRegion(lMinX, lMaxY, lMaxX, lMinY);
                _topLeft = newRegion._topLeft;
                _bottomRight = newRegion._bottomRight;

                RecalculateDimensionsAndCenter();
            }
        }

        /// <summary>
        /// Makes a copy of the current instance.
        /// </summary>
        /// <returns>The copyied instance.</returns>
        public MapRegion Clone() => new MapRegion(_topLeft, _bottomRight);

        public bool Contains(Position position) => position.Longitude >= MinX && position.Longitude <= MaxX && 
                                                   position.Latitude >= MinY && position.Latitude <= MaxY;

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            var other = obj as MapRegion;
            return Equals(other);
        }

        /// <summary>
        /// Serves as the default hash function. 
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode() => _topLeft.GetHashCode() * 397 ^ _bottomRight.GetHashCode();

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString() => $"Region Latitude [{MinY} to {MaxY}] Longitude [{MinX} to {MaxX}]";

        /// <summary>
        /// Determines whether the specified <see cref="MapRegion"/> is equal to the current.
        /// </summary>
        /// <returns>
        /// True if the specified MapRegion is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="other">The object to compare with the current object. </param><filterpriority>2</filterpriority>
        protected bool Equals(MapRegion other)
            => other != null && _topLeft.Equals(other._topLeft) && _bottomRight.Equals(other._bottomRight);


        private double ClampWidth(double width) => width.Clamp(-360, 360);

        private double ClampHeight(double height) => height.Clamp(-180, 180);

        private void RecalculateDimensionsAndCenter()
        {
            _width = MaxX + Math.Abs(MinX);
            _height = MaxY + Math.Abs(MinY);

            _center = new Position(_height/2, _width/2);
        }
    }
}