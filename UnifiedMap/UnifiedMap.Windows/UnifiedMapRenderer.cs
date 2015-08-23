using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Shapes;
using fivenine.UnifiedMaps;
using fivenine.UnifiedMaps.Windows;
using Xamarin.Forms.Platform.WinRT;

[assembly: ExportRenderer(typeof(UnifiedMap), typeof(UnifiedMapRenderer))]

namespace fivenine.UnifiedMaps.Windows
{
    public class UnifiedMapRenderer : ViewRenderer<UnifiedMap,MapControl>
    {
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
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
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
        }

        private void LoadPin(MapPin pin)
        {
            var mapPin = new MapIcon
            {
                Title = pin.Title,
                Location = new Geopoint(new BasicGeoposition
                {
                    Latitude = pin.Location.Latitude,
                    Longitude = pin.Location.Longitude
                })
            };

            Control.MapElements.Add(mapPin);
        }

        private void LoadPins()
        {
            foreach (var pin in Element.Pins)
            {
                LoadPin(pin);
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
