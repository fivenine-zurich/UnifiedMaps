using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Data;
using fivenine.UnifiedMaps;
using fivenine.UnifiedMaps.Windows;
using Xamarin.Forms.Platform.WinRT;
using Binding = Windows.UI.Xaml.Data.Binding;

[assembly: ExportRenderer(typeof(UnifiedMap), typeof(UnifiedMapRenderer))]

namespace fivenine.UnifiedMaps.Windows
{
    public class UnifiedMapRenderer : ViewRenderer<UnifiedMap,MapControl>
    {
        private InfoWindow _infoWindow;

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

        private void OnPinsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                {
                    foreach (var item in e.NewItems)
                    {
                        LoadPin((MapPin) item);
                    }

                    break;
                }

                case NotifyCollectionChangedAction.Remove:
                {
                    foreach (var item in e.OldItems)
                    {
                        RemovePin((MapPin) item);
                    }
                    break;
                }

                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                    throw new NotSupportedException($"The operation {e.Action} is not supported for MapPins");

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnControlLoaded(object sender, global::Windows.UI.Xaml.RoutedEventArgs e)
        {
            var serviceToken = fivenine.UnifiedMap.AuthenticationToken;

            if (string.IsNullOrWhiteSpace(serviceToken) == false)
            {
                Control.MapServiceToken = serviceToken;
            }

            Element.Pins.CollectionChanged += OnPinsCollectionChanged;

            UpdateMapType();
            LoadPins();

            _infoWindow = new InfoWindow
            {
                Visibility = Visibility.Collapsed,
                DataContext = new MapPin()
            };

            Control.Children.Add(_infoWindow);

            _infoWindow.SetBinding(InfoWindow.TitleProperty,
                new Binding { Path = new PropertyPath(MapPin.TitleProperty.PropertyName) });

            _infoWindow.SetBinding(InfoWindow.SnippetProperty,
                new Binding { Path = new PropertyPath(MapPin.SnippetProperty.PropertyName) });

            _infoWindow.SelectedCommand = Element.PinCalloutTappedCommand;

            Control.MapElementClick += Control_MapElementClick;
            Control.MapTapped += OnMapTapped;
        }

        private void OnMapTapped(MapControl sender, MapInputEventArgs args)
        {
            var items = Control.FindMapElementsAtOffset(args.Position);
            if (items.Any() == false)
            {
                _infoWindow.Visibility = Visibility.Collapsed;
            }
        }

        private void Control_MapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            var element = args.MapElements.FirstOrDefault() as MapIcon;

            var mapPin = Element.Pins.FirstOrDefault(p => p.Id == element);

            _infoWindow.DataContext = mapPin;
            _infoWindow.Visibility = Visibility.Visible;
            
            MapControl.SetLocation(_infoWindow, element.Location);
        }

        private void LoadPin(MapPin pin)
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
                LoadPin(pin);
            }
        }

        private void RemovePin(MapPin pin)
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
