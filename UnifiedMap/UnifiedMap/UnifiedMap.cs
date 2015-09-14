using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Xamarin.Forms;

namespace fivenine.UnifiedMaps
{
    /// <summary>
    /// A map control for Xamarin.Froms applications that supports all major platforms.
    /// </summary>
    public class UnifiedMap : View
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly ObservableCollection<MapPin> _pins;
        
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly ObservableCollection<MapPolyline> _polylines;

        private MapRegion _visibleRegion;

        internal const string MessageMapMoveToRegion = "MapMoveToRegion";

        /// <summary>
        /// Identifies the <see cref="MapType"/> bindable property.
        /// </summary>
        public static readonly BindableProperty MapTypeProperty = BindableProperty.Create("MapType", typeof (MapType),
            typeof (UnifiedMap), MapType.Street);

        /// <summary>
        /// The bindable pins property
        /// </summary>
        public static readonly BindableProperty PinsProperty = BindableProperty.Create("Pins",
            typeof (ObservableCollection<MapPin>), typeof (UnifiedMap), new ObservableCollection<MapPin>());

        /// <summary>
        /// The bindable pin callout tapped command property
        /// </summary>
        public static readonly BindableProperty PinCalloutTappedCommandProperty =
            BindableProperty.Create("PinCalloutTappedCommand", typeof (Command<MapPin>), typeof (UnifiedMap), null);

        /// <summary>
        /// The bindable polylines property
        /// </summary>
        public static readonly BindableProperty PolylinesProperty = BindableProperty.Create("Polylines",
            typeof(ObservableCollection<MapPolyline>), typeof(UnifiedMap), new ObservableCollection<MapPolyline>());

        /// <summary>
        /// Initializes a new instance of the <see cref="UnifiedMap"/> class.
        /// </summary>
        public UnifiedMap()
        {
            HorizontalOptions = LayoutOptions.FillAndExpand;
            VerticalOptions = LayoutOptions.FillAndExpand;

            _pins = new ObservableCollection<MapPin>();
            _polylines = new ObservableCollection<MapPolyline>();

            _pins.CollectionChanged += OnPinsCollectionChanged;
            _polylines.CollectionChanged += OnPolylinesCollectionChanged;
        }

        /// <summary>
        /// The <see cref="MapType"/> display style of this <see cref="UnifiedMap"/>.
        /// </summary>
        public MapType MapType
        {
            get { return (MapType) GetValue(MapTypeProperty); }
            set { SetValue(MapTypeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the pins.
        /// </summary>
        /// <value>
        /// The pins.
        /// </value>
        public ObservableCollection<MapPin> Pins
        {
            get { return (ObservableCollection<MapPin>)GetValue(PinsProperty); }
            set { SetValue(PinsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the pin callout tapped command.
        /// </summary>
        /// <value>
        /// The pin callout tapped command.
        /// </value>
        public Command<MapPin> PinCalloutTappedCommand
        {
            get { return (Command<MapPin>)GetValue(PinCalloutTappedCommandProperty); }
            set { SetValue(PinCalloutTappedCommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets the polylines.
        /// </summary>
        /// <value>
        /// The polylines.
        /// </value>
        public ObservableCollection<MapPolyline> Polylines
        {
            get { return (ObservableCollection<MapPolyline>)GetValue(PolylinesProperty); }
            set { SetValue(PolylinesProperty, value); }
        }

        /// <summary>
        /// Gets the visible region.
        /// </summary>
        /// <value>
        /// The visible region.
        /// </value>
        /// <exception cref="System.ArgumentNullException"></exception>
        public MapRegion VisibleRegion
        {
            get { return _visibleRegion; }
            internal set
            {
                if (_visibleRegion == value)
                {
                    return;
                }

                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                OnPropertyChanging();
                _visibleRegion = value;
                OnPropertyChanged();
            }
        }

        internal MapRegion LastMoveToRegion { get; private set; }

        /// <summary>
        /// Makes the map move to the new region.
        /// </summary>
        /// <param name="region">The region to display.</param>
        /// <param name="animated">Animate the move.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void MoveToRegion(MapRegion region = null, bool animated = false)
        {
            if (region == null)
            {
                region = MapRegion.FromPositions(
                    Pins.Select(p => p.Location).ToArray());
            }

            LastMoveToRegion = region;

            // Send the move message to the platform renderer
            MessagingCenter.Send(this, MessageMapMoveToRegion, new Tuple<MapRegion, bool>(region, animated));
        }

        private void OnPinsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Check the newly added items
        }

        private void OnPolylinesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Check the newly added items
        }
    }
}
