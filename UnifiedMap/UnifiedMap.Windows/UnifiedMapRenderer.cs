using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using fivenine.UnifiedMaps;
using fivenine.UnifiedMaps.Windows;
using Xamarin.Forms.Platform.WinRT;
using WinMap = Windows.UI.Xaml.Controls.Maps;

[assembly: ExportRenderer(typeof (UnifiedMap), typeof (UnifiedMapRenderer))]

namespace fivenine.UnifiedMaps.Windows
{
    public class UnifiedMapRenderer : ViewRenderer<UnifiedMap, WinMap.MapControl>, IUnifiedMapRenderer
    {
        private readonly RendererBehavior _behavior;
        private InfoWindow _infoWindow;
        private Geolocator _locator;
        private Ellipse _userLocationIndicator;

        public UnifiedMapRenderer()
        {
            _behavior = new RendererBehavior(this);
        }

        protected virtual Thickness MapPadding { get; } = new Thickness(20);
        public UnifiedMap Map => Element;

        public void MoveToRegion(MapRegion region, bool animated)
        {
            Control.TrySetViewBoundsAsync(new GeoboundingBox(region.TopLeft.Convert(), region.BottomRight.Convert()),
                MapPadding, animated ? WinMap.MapAnimationKind.Linear : WinMap.MapAnimationKind.None).AsTask();
        }

        public void FitAllAnnotations(bool animated)
        {
            var region = _behavior.GetRegionForAllAnnotations();
            MoveToRegion(region, animated);
        }

        public void ApplyHasZoomEnabled()
        {
            Control.ZoomInteractionMode = Element.HasZoomEnabled
                ? WinMap.MapInteractionMode.Auto
                : WinMap.MapInteractionMode.Disabled;
        }

        public void ApplyHasScrollEnabled()
        {
            Control.PanInteractionMode = Element.HasScrollEnabled
                ? WinMap.MapPanInteractionMode.Auto
                : WinMap.MapPanInteractionMode.Disabled;
        }

        public async void ApplyIsShowingUser()
        {
            if (Element.IsShowingUser)
            {
                var accessStatus = await Geolocator.RequestAccessAsync();
                switch (accessStatus)
                {
                    case GeolocationAccessStatus.Allowed:
                    {
                        _locator = new Geolocator();
                        _locator.PositionChanged += OnUserPositionChanged;
                        _locator.StatusChanged += OnLocatorStatusChanged;

                        var userPosition = await _locator.GetGeopositionAsync();
                        if (userPosition?.Coordinate != null)
                        {
                            DisplayUserPosition(userPosition.Coordinate);
                        }

                        break;
                    }
                }
            }
            else
            {
                DisplayUserPosition(null);
            }
        }

        public void ApplyMapType()
        {
            switch (Element.MapType)
            {
                case MapType.Street:
                    Control.Style = WinMap.MapStyle.Road;
                    break;
                case MapType.Satellite:
                    Control.Style = WinMap.MapStyle.Aerial;
                    break;
                case MapType.Hybrid:
                    Control.Style = WinMap.MapStyle.AerialWithRoads;
                    break;
                default:
                {
                    Control.Style = WinMap.MapStyle.Road;
                    Debug.Fail($"The map type {Element.MapType} is not supported on Windows, falling back to Road");
                    break;
                }
            }
        }

        public void AddPin(MapPin pin)
        {
            var mapPin = new WinMap.MapIcon
            {
                Location = new Geopoint(new BasicGeoposition
                {
                    Latitude = pin.Location.Latitude,
                    Longitude = pin.Location.Longitude
                }),
                Image = RandomAccessStreamReference.CreateFromUri(
                    new Uri("ms-appx:///UnifiedMap.Windows/Assets/pin_black.png")),
                NormalizedAnchorPoint = new Point(0.5, 1)
            };

            pin.Id = mapPin;
            Control.MapElements.Add(mapPin);
        }

        public void RemovePin(MapPin pin)
        {
            var pins = Control.MapElements
                .OfType<WinMap.MapIcon>()
                .Where(point => point == pin.Id)
                .ToArray();

            foreach (var icon in pins)
            {
                Control.MapElements.Remove(icon);
            }
        }

        public void AddPolyline(MapPolyline line)
        {
            var positions = line.Select(p => new BasicGeoposition {Latitude = p.Latitude, Longitude = p.Longitude});

            var polyline = new WinMap.MapPolyline
            {
                Path = new Geopath(positions),
                StrokeColor = line.StrokeColor.ToWinRT(),
                StrokeThickness = line.LineWidth
            };

            // Store the map-polyline object in an attached property
            PolylineData.SetData(polyline, line);

            Control.MapElements.Add(polyline);
        }

        public void RemovePolyline(MapPolyline line)
        {
            var polylines = Control.MapElements
                .OfType<WinMap.MapPolyline>()
                .Where(polyline => PolylineData.GetData(polyline) == line)
                .ToArray();

            foreach (var polyline in polylines)
            {
                Control.MapElements.Remove(polyline);
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<UnifiedMap> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var map = new WinMap.MapControl();
                map.Loaded += OnControlLoaded;

                SetNativeControl(map);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            _behavior.ElementProperyChanged(e.PropertyName);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                RemoveEvents(Element);
                Control.Loaded -= OnControlLoaded;

                if (_locator != null)
                {
                    _locator.PositionChanged -= OnUserPositionChanged;
                    _locator.StatusChanged -= OnLocatorStatusChanged;
                    _locator = null;
                }
            }

            base.Dispose(disposing);
        }

        private void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            var serviceToken = fivenine.UnifiedMap.AuthenticationToken;

            if (string.IsNullOrWhiteSpace(serviceToken) == false)
            {
                Control.MapServiceToken = serviceToken;
            }

            RegisterEvents(Element);

            _behavior.Initialize();
        }

        private void RegisterEvents(UnifiedMap map)
        {
            _behavior.RegisterEvents(map);

            if (Control != null)
            {
                _infoWindow = new InfoWindow
                {
                    Visibility = Visibility.Collapsed,
                    DataContext = new MapPin()
                };

                Control.Children.Add(_infoWindow);

                _infoWindow.SetBinding(InfoWindow.TitleProperty,
                    new Binding {Path = new PropertyPath(MapPin.TitleProperty.PropertyName)});

                _infoWindow.SetBinding(InfoWindow.SnippetProperty,
                    new Binding {Path = new PropertyPath(MapPin.SnippetProperty.PropertyName)});

                _infoWindow.SelectedCommand = Element.PinCalloutTappedCommand;

                Control.MapElementClick += OnMapElementClick;
                Control.MapTapped += OnMapTapped;
            }
        }

        private void RemoveEvents(UnifiedMap map)
        {
            _behavior.RemoveEvents(map);

            if (Control != null)
            {
                _infoWindow = null;
                Control.MapElementClick -= OnMapElementClick;
                Control.MapTapped -= OnMapTapped;
            }
        }

        private void DisplayUserPosition(Geocoordinate coordinate)
        {
            if (coordinate == null && _userLocationIndicator != null)
            {
                Control.Children.Remove(_userLocationIndicator);
                _userLocationIndicator = null;
            }

            if (coordinate != null)
            {
                if (_userLocationIndicator == null)
                {
                    _userLocationIndicator = new Ellipse
                    {
                        Fill = new SolidColorBrush(Colors.Blue),
                        Height = 20,
                        Width = 20,
                        Opacity = 50
                    };

                    Control.Children.Add(_userLocationIndicator);
                }

                WinMap.MapControl.SetLocation(_userLocationIndicator, coordinate.Point);
            }
        }

        private void OnLocatorStatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
        }

        private async void OnUserPositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            await
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () => { DisplayUserPosition(args.Position?.Coordinate); });
        }

        private void OnMapTapped(WinMap.MapControl sender, WinMap.MapInputEventArgs args)
        {
            var items = Control.FindMapElementsAtOffset(args.Position);
            if (items.Any() == false)
            {
                _infoWindow.Visibility = Visibility.Collapsed;
            }
        }

        private void OnMapElementClick(WinMap.MapControl sender, WinMap.MapElementClickEventArgs args)
        {
            var element = args.MapElements.FirstOrDefault() as WinMap.MapIcon;
            if (element != null)
            {
                var mapPin = Element.Pins.FirstOrDefault(p => p.Id == element);

                _infoWindow.DataContext = mapPin;
                _infoWindow.Visibility = Visibility.Visible;

                WinMap.MapControl.SetLocation(_infoWindow, element.Location);
            }
        }
    }

    public class PolylineData : DependencyObject
    {
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.RegisterAttached("Data", typeof (MapPolyline), typeof (WinMap.MapPolyline),
                new PropertyMetadata(null));

        public static MapPolyline GetData(DependencyObject d)
        {
            return (MapPolyline) d.GetValue(DataProperty);
        }

        public static void SetData(DependencyObject d, MapPolyline data)
        {
            d.SetValue(DataProperty, data);
        }
    }
}