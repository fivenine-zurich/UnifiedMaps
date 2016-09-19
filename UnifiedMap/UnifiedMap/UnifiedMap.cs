using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Xamarin.Forms;

namespace fivenine.UnifiedMaps
{
    /// <summary>
    /// A map control for Xamarin.Froms applications that supports all major platforms.
    /// </summary>
    public class UnifiedMap : View
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly ObservableCollection<IMapPin> _pins;
        
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
        /// The bindable pins property.
        /// </summary>
        public static readonly BindableProperty PinsProperty = BindableProperty.Create("Pins",
            typeof (ObservableCollection<IMapPin>), typeof (UnifiedMap), new ObservableCollection<IMapPin>());

        /// <summary>
        /// The bindable polylines property.
        /// </summary>
        public static readonly BindableProperty PolylinesProperty = BindableProperty.Create("Polylines",
            typeof(ObservableCollection<MapPolyline>), typeof(UnifiedMap), new ObservableCollection<MapPolyline>());

        /// <summary>
        /// The bindable pin callout tapped command property.
        /// </summary>
        public static readonly BindableProperty PinCalloutTappedCommandProperty =
            BindableProperty.Create("PinCalloutTappedCommand", typeof (Command<IMapPin>), typeof (UnifiedMap), null);

        /// <summary>
        /// The autofitallannotations property.
        /// </summary>
        public static readonly BindableProperty AutoFitAllAnnotationsProperty = BindableProperty.Create("AutoFitAllAnnotations",
                typeof (bool), typeof (UnifiedMap), true);

        /// <summary>
        /// The is showing user property.
        /// </summary>
        public static readonly BindableProperty IsShowingUserProperty = BindableProperty.Create("IsShowingUser",
                typeof(bool), typeof(UnifiedMap), false);

        /// <summary>
        /// The has scroll enabled property
        /// </summary>
        public static readonly BindableProperty HasScrollEnabledProperty = BindableProperty.Create("HasScrollEnabled",
                typeof(bool), typeof(UnifiedMap), true);

        /// <summary>
        /// The has zoom enabled property
        /// </summary>
        public static readonly BindableProperty HasZoomEnabledProperty = BindableProperty.Create("HasZoomEnabled",
                typeof(bool), typeof(UnifiedMap), true);

        /// <summary>
        /// Initializes a new instance of the <see cref="UnifiedMap"/> class.
        /// </summary>
        public UnifiedMap()
        {
            HorizontalOptions = LayoutOptions.FillAndExpand;
            VerticalOptions = LayoutOptions.FillAndExpand;

            _pins = new ObservableCollection<IMapPin>();
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
        public ObservableCollection<IMapPin> Pins
        {
            get { return (ObservableCollection<IMapPin>)GetValue(PinsProperty); }
            set { SetValue(PinsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the pin callout tapped command.
        /// </summary>
        /// <value>
        /// The pin callout tapped command.
        /// </value>
        public Command<IMapPin> PinCalloutTappedCommand
        {
            get { return (Command<IMapPin>)GetValue(PinCalloutTappedCommandProperty); }
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
        /// Gets or sets whether to automatically fit the view to display all annotations.
        /// </summary>
        /// <value>
        /// <c>true</c> if auto fitting should be enabled; otherwise, <c>false</c>.
        /// </value>
        public bool AutoFitAllAnnotations
        {
            get { return (bool)GetValue(AutoFitAllAnnotationsProperty); }
            set { SetValue(AutoFitAllAnnotationsProperty, value); }
        }


        /// <summary>
        /// Gets or sets whether to display the current location of the user on the map.
        /// </summary>
        /// <value>
        /// <c>true</c> if the user location should be show; otherwise, <c>false</c>.
        /// </value>
        public bool IsShowingUser
        {
            get { return (bool)GetValue(IsShowingUserProperty); }
            set { SetValue(IsShowingUserProperty, value); }
        }

        /// <summary>
        /// Gets or sets if scrolling is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has scroll enabled; otherwise, <c>false</c>.
        /// </value>
        public bool HasScrollEnabled
        {
            get { return (bool)GetValue(HasScrollEnabledProperty); }
            set { SetValue(HasScrollEnabledProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has zoom enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has zoom enabled; otherwise, <c>false</c>.
        /// </value>
        public bool HasZoomEnabled
        {
            get { return (bool)GetValue(HasZoomEnabledProperty); }
            set { SetValue(HasZoomEnabledProperty, value); }
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
        /// Makes the map move to the new region. If no region is specified the map will be moved
        /// to fit all annotations that are currently displayed.
        /// </summary>
        /// <param name="region">The region to display.</param>
        /// <param name="animated">Wether to animate the move.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void MoveToRegion(MapRegion region = null, bool animated = false)
        {
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
