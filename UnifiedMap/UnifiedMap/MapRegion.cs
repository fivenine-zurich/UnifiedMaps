using System;
using System.Collections.Generic;

namespace fivenine.UnifiedMaps
{
    /// <summary>
    /// Defines a map region.
    /// </summary>
    public class MapRegion
    {
        private Position _center;
        private readonly double _width;
        private readonly double _height;

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
        public static MapRegion Empty()
        {
            return new MapRegion(0, 0, 0, 0);
        }

        /// <summary>
        /// Creates a map region that includes the whole world.
        /// </summary>
        /// <returns>The largest possible map region.</returns>
        public static MapRegion World()
        {
            return new MapRegion(-180, 90, 180, -90);
        }

        /// <summary>
        /// Discribes a rectangular region. This region usually encloses a set of geometries or represents a area of view.
        /// </summary>
        /// <param name="center">Center of the bounding box</param>
        /// <param name="width">Width of bounding box in degress</param>
        /// <param name="height">Height of bounding box in degress</param>
        public MapRegion(Position center, double width, double height)
        {
            _center = center;
            _width = ClampWidth(width);
            _height = ClampHeight(height);
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
            _height = ClampHeight(Math.Abs(maxY - minY));
            _width = ClampWidth(Math.Abs(maxX - minX));

            var cLat = maxY - _height / 2;
            var cLon = minX + _width / 2;

            _center = new Position(cLat, cLon);
        }

        /// <summary>
        /// The Center Coordinate.
        /// </summary>
        public Position Center => _center;

        /// <summary>
        /// North Latitude Coordinate
        /// </summary>
        public double MaxY => _center.Latitude + _height / 2;

        /// <summary>
        /// South Latitude Coordinate
        /// </summary>
        public double MinY => _center.Latitude - _height / 2;

        /// <summary>
        /// Most Easterly Longitude Coordinate (right side of bounding box)
        /// </summary>
        public double MaxX => _center.Longitude + _width / 2;

        /// <summary>
        /// Most Westerly Longitude Coordinate (left side of bounding box)
        /// </summary>
        public double MinX => _center.Longitude - _width / 2;

        /// <summary>
        /// Gets the top left coordinate of the bounding box
        /// </summary>
        public Position TopLeft => new Position(MaxY, MinX);

        /// <summary>
        /// Gets the bottom right coordinate of the bounding box
        /// </summary>
        public Position BottomRight => new Position(MinY, MaxX);

        /// <summary>
        /// Inflates the current region with the given dimensions.
        /// </summary>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        /// <returns>The new <see cref="MapRegion"/> instance.</returns>
        public MapRegion Inflate(float height, float width)
        {
            return new MapRegion(_center, _width + width, _height + height);
        }
        
        private double ClampWidth(double width)
        {
            return width.Clamp(-360, 360);
        }

        private double ClampHeight(double height)
        {
            return height.Clamp(-180, 180);
        }

        public void Include(Position item)
        {
        }
    }
}