using System;
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
    public class UnifiedMapRenderer : ViewRenderer<UnifiedMap, MKMapView>, IUnifiedMapRenderer
    {
        private readonly RendererBehavior _behavior;

        public UnifiedMapRenderer()
        {
            _behavior = new RendererBehavior(this);
        }

        public UnifiedMap Map => Element;

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
            _behavior.RegisterEvents(map);
        }

        private void RemoveEvents(UnifiedMap map)
        {
            _behavior.RemoveEvents(map);
        }

        void IUnifiedMapRenderer.MoveToRegion(MapRegion mapRegion, bool animated)
        {
            Control.SetRegion(new MKCoordinateRegion(
                mapRegion.Center.ToCoordinate(),
                mapRegion.ToSpan()), animated);
        }

        public void FitAllAnnotations(bool animated)
        {
            Control.ShowAnnotations(Control.Annotations, animated);
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

        public void AddPin(MapPin pin)
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

        public void RemovePin(MapPin pin)
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
                AddPin(pin);
            }
        }
    }
}
