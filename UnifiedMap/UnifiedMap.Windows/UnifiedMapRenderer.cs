using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Maps;
using fivenine.UnifiedMaps;
using fivenine.UnifiedMaps.Windows;
using Xamarin.Forms.Platform.WinRT;
using Binding = Windows.UI.Xaml.Data.Binding;
using Thickness = Windows.UI.Xaml.Thickness;

[assembly: ExportRenderer(typeof(UnifiedMap), typeof(UnifiedMapRenderer))]

namespace fivenine.UnifiedMaps.Windows
{
    public class UnifiedMapRenderer : ViewRenderer<UnifiedMap,MapControl>, IUnifiedMapRenderer
    {
        private readonly RendererBehavior _behavior;
        private readonly Thickness _mapPadding = new Thickness(20);

        private InfoWindow _infoWindow;
        
        public UnifiedMapRenderer()
        {
            _behavior = new RendererBehavior(this);
        }

        public UnifiedMap Map => Element;

        protected virtual Thickness MapPadding => _mapPadding;

        protected override void OnElementChanged(ElementChangedEventArgs<UnifiedMap> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var map = new MapControl();
                map.Loaded += OnControlLoaded;

                SetNativeControl(map);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == UnifiedMap.MapTypeProperty.PropertyName)
            {
                UpdateMapType();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                RemoveEvents(Element);
                Control.Loaded -= OnControlLoaded;
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

            UpdateMapType();
            LoadPins();
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

        public void MoveToRegion(MapRegion region, bool animated)
        {
            Control.TrySetViewBoundsAsync(new GeoboundingBox(region.TopLeft.Convert(), region.BottomRight.Convert()),
                MapPadding, animated ? MapAnimationKind.Linear : MapAnimationKind.None).AsTask();
        }

        public void FitAllAnnotations(bool animated)
        {
            var region = _behavior.GetRegionForAllAnnotations();
            MoveToRegion(region, animated);
        }

        private void OnMapTapped(MapControl sender, MapInputEventArgs args)
        {
            var items = Control.FindMapElementsAtOffset(args.Position);
            if (items.Any() == false)
            {
                _infoWindow.Visibility = Visibility.Collapsed;
            }
        }

        private void OnMapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            var element = args.MapElements.FirstOrDefault() as MapIcon;
            if (element != null)
            {

                var mapPin = Element.Pins.FirstOrDefault(p => p.Id == element);

                _infoWindow.DataContext = mapPin;
                _infoWindow.Visibility = Visibility.Visible;

                MapControl.SetLocation(_infoWindow, element.Location);
            }
        }

        public void AddPin(MapPin pin)
        {
            var mapPin = new MapIcon
            {
                Location = new Geopoint(new BasicGeoposition
                {
                    Latitude = pin.Location.Latitude,
                    Longitude = pin.Location.Longitude
                }),
                Image = RandomAccessStreamReference.CreateFromUri(
                    new Uri("ms-appx:///UnifiedMap.Windows/Assets/pin_black.png"))
            };

            pin.Id = mapPin;
            Control.MapElements.Add(mapPin);
        }

        private void LoadPins()
        {
            foreach (var pin in Element.Pins)
            {
                AddPin(pin);
            }
        }

        public void RemovePin(MapPin pin)
        {
            var pins = Control.MapElements
                .OfType<MapIcon>()
                .Where(point => point == pin.Id)
                .ToArray();

            foreach (var icon in pins)
            {
                Control.MapElements.Remove(icon);
            }
        }

        private void UpdateMapType()
        {
            switch (Element.MapType)
            {
                case MapType.Street:
                    Control.Style = MapStyle.Road;
                    break;
                case MapType.Satellite:
                    Control.Style = MapStyle.Aerial;
                    break;
                case MapType.Hybrid:
                    Control.Style = MapStyle.AerialWithRoads;
                    break;
                default:
                {
                    Control.Style = MapStyle.Road;
                    Debug.Fail($"The map type {Element.MapType} is not supported on Windows, falling back to Road");
                    break;
                }
            }
        }
    }
}
