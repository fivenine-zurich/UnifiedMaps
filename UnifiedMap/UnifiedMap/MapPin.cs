namespace fivenine.UnifiedMaps
{
    /// <summary>
    /// A map pin annotation.
    /// </summary>
    public class MapPin : IMapPin
    {
        /// <summary>
        /// Gets or sets the title of the MapPin.
        /// </summary>
        /// <value>
        /// The title of the pin.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the snippet (sub-title) of the MapPin callout window.
        /// </summary>
        /// <value>
        /// The snippet text.
        /// </value>
        public string Snippet { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        public Position Location { get; set; }

        /// <summary>
        /// Gets or sets the color of the MapPin.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public PinColor Color { get; set; }
    }
}