using Xamarin.Forms;

namespace fivenine.UnifiedMaps
{
    /// <summary>
    /// A map pin annotation.
    /// </summary>
    public class MapPin : MapItem
    {
        /// <summary>
        /// The position property.
        /// </summary>
        public static readonly BindableProperty PositionProperty = BindableProperty.Create("Location",
            typeof(Position), typeof(MapPin), new Position());

        /// <summary>
        /// The title property.
        /// </summary>
        public static readonly BindableProperty TitleProperty = BindableProperty.Create("Title",
            typeof (string), typeof (MapPin), null);

        /// <summary>
        /// The color property.
        /// </summary>
        public static readonly BindableProperty ColorProperty = BindableProperty.Create("Color",
            typeof(Color), typeof(MapPin), Xamarin.Forms.Color.Blue);

        /// <summary>
        /// The snippet property.
        /// </summary>
        public static readonly BindableProperty SnippetProperty = BindableProperty.Create("Snippet",
            typeof (string), typeof (MapPin), null);

        /// <summary>
        /// Gets or sets the title of the MapPin.
        /// </summary>
        /// <value>
        /// The title of the pin.
        /// </value>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }


        /// <summary>
        /// Gets or sets the snippet (sub-title) of the MapPin callout window.
        /// </summary>
        /// <value>
        /// The snippet text.
        /// </value>
        public string Snippet
        {
            get { return (string)GetValue(SnippetProperty); }
            set { SetValue(SnippetProperty, value); }
        }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        public Position Location
        {
            get { return (Position) GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the color of the MapPin.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public Color Color
        {
            get { return (Color) GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
    }
}