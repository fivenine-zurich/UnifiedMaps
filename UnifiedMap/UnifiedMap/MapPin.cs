using Xamarin.Forms;

namespace fivenine.UnifiedMaps
{
    /// <summary>
    /// Defines the available pin colors.
    /// </summary>
    public enum PinColor
    {
        /// <summary>
        /// The default pin color.
        /// </summary>
        Default,

        /// <summary>
        /// A red pin.
        /// </summary>
        Red,

        /// <summary>
        /// A green pin.
        /// </summary>
        Green,

        /// <summary>
        /// A purple pin.
        /// </summary>
        Purple
    }

    /// <summary>
    /// A map pin annotation.
    /// </summary>
    public class MapPin : MapItem
    {
        public static readonly BindableProperty PositionProperty = BindableProperty.Create("Location",
            typeof(Position), typeof(MapPin), new Position());

        public static readonly BindableProperty TitleProperty = BindableProperty.Create("Title",
            typeof (string), typeof (MapPin), null);

        public static readonly BindableProperty ColorProperty = BindableProperty.Create("Color",
            typeof(PinColor), typeof(MapPin), PinColor.Default);

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
        public PinColor Color
        {
            get { return (PinColor) GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
    }
}