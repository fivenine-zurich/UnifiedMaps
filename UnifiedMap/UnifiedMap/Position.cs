using System;

namespace fivenine.UnifiedMaps
{
    /// <summary>
    /// A struct that has a latitude and longitude, stored as doubles.
    /// </summary>
    public struct Position
    {
        private const int LatitudeMax = 90;
        private const int LongitudeMax = 180;

        private readonly double _latitude;
        private readonly double _longitude;

        /// <summary>
        /// Clamps the latitude to the valid range.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <returns>A valid latitude value.</returns>
        public static double ClampLatitude(double latitude)
        {
            return latitude.Clamp(-LatitudeMax, LatitudeMax);
        }

        /// <summary>
        /// Clamps the longitude to the valid range.
        /// </summary>
        /// <param name="longitude">The longitude.</param>
        /// <returns>A valid longitude value.</returns>
        public static double ClampLongitude(double longitude)
        {
            return longitude.Clamp(-LongitudeMax, LongitudeMax);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Position"/> struct.
        /// </summary>
        /// <param name="latitude">The latitude degrees (y).</param>
        /// <param name="longitude">The longitude degrees (x).</param>
        public Position(double latitude, double longitude)
        {
            _latitude = ClampLatitude(latitude);
            _longitude = ClampLongitude(longitude);
        }

        /// <summary>
        /// Gets the latitude of this position in decimal degrees (y).
        /// </summary>
        public double Latitude => _latitude;

        /// <summary>
        /// Gets the longitude of this position in decimal degrees (x).
        /// </summary>
        public double Longitude => _longitude;

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Position left, Position right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Position left, Position right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false. 
        /// </returns>
        /// <param name="obj">The object to compare with the current instance. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj) || obj.GetType() != GetType())
                return false;

            var position = (Position) obj;

            return Math.Abs(_latitude - position._latitude) < double.Epsilon
                   && Math.Abs(_longitude - position._longitude) < double.Epsilon;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode() => _latitude.GetHashCode()*37 ^ _longitude.GetHashCode();

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> containing a fully qualified type name.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString() => $"Position [Latitude {Latitude}, Longitude {Longitude}]";
    }
}