namespace fivenine.UnifiedMaps
{
    /// <summary>
    /// Enumeration that specifies the display style of the map.
    /// </summary>
    public enum MapType
    {
        /// <summary>
        /// The street map.
        /// </summary>
        /// <remarks>Available on all platformss.</remarks>
        Street,

        /// <summary>
        /// The satellite map.
        /// </summary>
        /// <remarks>Available on all platformss.</remarks>
        Satellite,

        /// <summary>
        /// The combined satellite and street map.
        /// </summary>
        /// <remarks>Available on all platformss.</remarks>
        Hybrid,
    }
}