using System;
using System.Collections.Specialized;
using System.Drawing;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using CoreLocation;
using MapKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using fivenine.UnifiedMaps;
using fivenine.UnifiedMaps.iOS;

[assembly: ExportRenderer(typeof(UnifiedMap), typeof(UnifiedMapRenderer))]

namespace fivenine.UnifiedMaps.iOS
{
    public class UnifiedMapRenderer : ViewRenderer<UnifiedMap, MKMapView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<UnifiedMap> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                RemoveEvents(e.OldElement);
            }

            if (e.NewElement != null)
            {
                RegisterEvents(e.NewElement);
            }

            if (Control == null)
            {
                var map = new MKMapView(RectangleF.Empty)
                {
                    Delegate = new UnifiedMapDelegate(this)
                };

                SetNativeControl(map);

                UpdateMapType();

                LoadPins();
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

                if (Control != null)
                {
                    Control.Delegate = null;
                }
            }

            base.Dispose(disposing);
        }

        private void RegisterEvents(UnifiedMap map)
        {
            if (map.Pins != null)
            {
                map.Pins.CollectionChanged += PinsOnCollectionChanged;
            }

            MessagingCenter.Subscribe<UnifiedMap, Tuple<MapRegion, bool>>(this, UnifiedMap.MessageMapMoveToRegion,
                (unifiedMap, span) => MoveToRegion(span.Item1, span.Item2));
        }

        private void RemoveEvents(UnifiedMap map)
        {
            MessagingCenter.Unsubscribe<UnifiedMap, MapRegion>(this, UnifiedMap.MessageMapMoveToRegion);

            if (map.Pins != null)
            {
                map.Pins.CollectionChanged -= PinsOnCollectionChanged;
            }
        }

        private void MoveToRegion(MapRegion mapRegion, bool animated = true)
        {
            Control.SetRegion(new MKCoordinateRegion(
                mapRegion.Center.ToCoordinate(),
                mapRegion.ToSpan()), animated);
        }

        private void UpdateMapType()
        {
            switch (Element.MapType)
            {
                case MapType.Street:
                    Control.MapType = MKMapType.Standard;
                    break;
                case MapType.Satellite:
                    Control.MapType = MKMapType.Satellite;
                    break;
                case MapType.Hybrid:
                    Control.MapType = MKMapType.Hybrid;
                    break;
                default:
                {
                    Control.MapType = MKMapType.Standard;
                    Debug.Fail($"The map type {Element.MapType} is not supported on iOS, falling back to Standard");
                    break;
                }
            }
        }

        private void CreatePin(MapPin pin)
        {
            var mapPin = new UnifiedPointAnnotation
            {
                Data = pin,
                Title = pin.Title,
                Subtitle = pin.Snippet,
                Coordinate = new CLLocationCoordinate2D(pin.Location.Latitude, pin.Location.Longitude),
            };

            pin.Id = mapPin;
            Control.AddAnnotation(mapPin);
        }

        private void RemovePin(MapPin pin)
        {
            var pins = Control.Annotations
                .OfType<UnifiedPointAnnotation>()
                .Where( point => point.Data == pin)
                .ToArray();

            Control.RemoveAnnotations(pins);
        }

        private void LoadPins()
        {
            foreach (var pin in Element.Pins)
            {
                CreatePin(pin);
            }
        }

        private void PinsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                {
                    foreach (var item in e.NewItems)
                    {
                        CreatePin((MapPin) item);
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
    }
}
