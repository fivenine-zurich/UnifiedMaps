using System;
using System.Collections;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace fivenine.UnifiedMaps
{
    /// <summary>
    /// A map control for Xamarin.Froms applications that supports all major platforms.
    /// </summary>
	[Preserve(AllMembers = true)]
    public class UnifiedMap : View
    {
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
            typeof (IEnumerable), typeof (UnifiedMap), null);

        /// <summary>
        /// The bindable polylines property.
        /// </summary>
        public static readonly BindableProperty OverlaysProperty = BindableProperty.Create("Overlays",
            typeof(IEnumerable), typeof(UnifiedMap), null);

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
        /// The selected item property.
        /// </summary>
        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create("SelectedItem",
                typeof(IMapAnnotation), typeof(UnifiedMap), null, propertyChanged: OnSelectedItemPropertyChanged);

        /// <summary>
        /// The selection changed command property.
        /// </summary>
        public static readonly BindableProperty SelectionChangedCommandProperty = BindableProperty.Create("SelectionChangedCommand",
                typeof(Command<IMapAnnotation>), typeof(UnifiedMap), null);

        public static readonly BindableProperty VisibleRegionProperty = BindableProperty.Create("VisibleRegion",
                typeof(MapRegion), typeof(UnifiedMap), null, propertyChanged: OnVisibleRegionChanged);

        public static readonly BindableProperty VisibleRegionChangedCommandProperty = BindableProperty.Create("VisibleRegionChangedCommand",
                typeof(Command<MapRegion>), typeof(UnifiedMap), null);

        /// <summary>
        /// Occurs when the selected annotation changes.
        /// </summary>
        public event EventHandler<MapEventArgs<IMapAnnotation>> SelectionChanged;

        public event EventHandler<MapEventArgs<MapRegion>> VisibleRegionChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnifiedMap"/> class.
        /// </summary>
        public UnifiedMap()
        {
            HorizontalOptions = LayoutOptions.FillAndExpand;
            VerticalOptions = LayoutOptions.FillAndExpand;
        }

        static void OnSelectedItemPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var map = bindable as UnifiedMap;
            if (map != null && !newValue.EqualsSafe(oldValue))
            {
                map.SelectionChanged?.Invoke(map, new MapEventArgs<IMapAnnotation>(newValue as IMapAnnotation));
                map.SelectionChangedCommand?.Execute(newValue);
            }
        }

        static void OnVisibleRegionChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var map = bindable as UnifiedMap;
            if (map != null && !newValue.EqualsSafe(oldValue))
            {
                map.VisibleRegionChanged?.Invoke(map, new MapEventArgs<MapRegion>(newValue as MapRegion));
                map.VisibleRegionChangedCommand?.Execute(newValue);
            }
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
        public IEnumerable Pins
        {
            get { return (IEnumerable)GetValue(PinsProperty); }
            set { SetValue(PinsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the overlays.
        /// </summary>
        /// <value>
        /// The overlays.
        /// </value>
        public IEnumerable Overlays
        {
            get { return (IEnumerable)GetValue(OverlaysProperty); }
            set { SetValue(OverlaysProperty, value); }
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
            get { return (MapRegion)GetValue(VisibleRegionProperty); }
            set { SetValue(VisibleRegionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        public IMapAnnotation SelectedItem
        {
            get { return (IMapAnnotation) GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// Gets or sets the selection changed command.
        /// </summary>
        /// <value>The selection changed command.</value>
        public Command<IMapAnnotation> SelectionChangedCommand
        {
            get { return (Command<IMapAnnotation>)GetValue(SelectionChangedCommandProperty); }
            set { SetValue(SelectionChangedCommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets the visible region changed command.
        /// </summary>
        /// <value>The visible region changed command.</value>
        public Command<MapRegion> VisibleRegionChangedCommand
        {
            get { return (Command<MapRegion>)GetValue(VisibleRegionChangedCommandProperty); }
            set { SetValue(VisibleRegionChangedCommandProperty, value); }
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
    }
}
