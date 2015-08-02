using System;
using System.Collections.Specialized;
using System.Drawing;
using CoreLocation;
using fivenine.UnifiedMaps;
using fivenine.UnifiedMaps.iOS;
using Foundation;
using MapKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

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
                if (Element.Pins != null)
                {
                    Element.Pins.CollectionChanged -= PinsOnCollectionChanged;
                }
            }

            if( e.NewElement == null)
            {
                return;
            }

            if (Control == null)
            {
                var map = new MKMapView(RectangleF.Empty)
                {
                    Delegate = new UnifiedMapDelegate(this)
                };

                SetNativeControl(map);

                MessagingCenter.Subscribe<UnifiedMap, MapRegion>(this, UnifiedMap.MessageMapMoveToRegion,
                    (unifiedMap, span) =>
                    {
                        if (unifiedMap.LastMoveToRegion != null)
                        {
                            MoveToRegion(unifiedMap.LastMoveToRegion, false);
                        }
                    });

                UpdateMapType();

                if (Element.Pins != null)
                {
                    Element.Pins.CollectionChanged += PinsOnCollectionChanged;
                }

                LoadPins();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Element != null)
                {
                    var unifiedMap = Element;

                    MessagingCenter.Unsubscribe<UnifiedMap, MapRegion>(this, UnifiedMap.MessageMapMoveToRegion);
                    unifiedMap.Pins.CollectionChanged -= PinsOnCollectionChanged;
                }

                if (Control != null)
                {
                    Control.Delegate = null;
                }
            }

            base.Dispose(disposing);
        }

        private void MoveToRegion(MapRegion mapRegion, bool animated = true)
        {
            Position center = mapRegion.Center;

            //Control.SetRegion(new MKCoordinateRegion(
            //    new CLLocationCoordinate2D(center.Latitude, center.Longitude), 
            //    new MKCoordinateSpan(MapRegion.LatitudeDegrees, MapRegion.LongitudeDegrees)), 
            //    animated);
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
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CreatePin(MapPin pin)
        {
            var mapPin = new UnifiedPointAnnotation
            {
                Data = pin,
                Title = pin.Title,
                Coordinate = new CLLocationCoordinate2D(pin.Location.Latitude, pin.Location.Longitude)
            };

            pin.Id = mapPin;
            Control.AddAnnotation(mapPin);
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
    }
}
