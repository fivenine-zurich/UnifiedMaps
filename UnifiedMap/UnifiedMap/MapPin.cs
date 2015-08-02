using System.Windows.Input;
using Xamarin.Forms;

namespace fivenine.UnifiedMaps
{
    /// <summary>
    /// A map pin annotation.
    /// </summary>
    public class MapPin : MapItem
    {
        public static readonly BindableProperty PositionProperty = BindableProperty.Create("Location",
            typeof(Position), typeof(MapPin), new Position());

        public static readonly BindableProperty TitleProperty = BindableProperty.Create("Title",
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
    }
}