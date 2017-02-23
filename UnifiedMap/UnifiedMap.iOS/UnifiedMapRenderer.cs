using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using CoreLocation;
using fivenine.UnifiedMaps;
using fivenine.UnifiedMaps.iOS;
using Foundation;
using MapKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof (UnifiedMap), typeof (UnifiedMapRenderer))]

namespace fivenine.UnifiedMaps.iOS
{
    [Preserve(AllMembers = true)]
    public class UnifiedMapRenderer : ViewRenderer<UnifiedMap, MKMapView>, IUnifiedMapRenderer
    {
        private readonly RendererBehavior _behavior;
        private CLLocationManager _locationManager;

        public UnifiedMapRenderer()
        {
            _behavior = new RendererBehavior(this);
        }

        public UnifiedMap Map => Element;

        public IMapAnnotation SelectedItem
        {
            get { return Element.SelectedItem; }
            set 
            {
                if (!Element.SelectedItem.EqualsSafe(value))
                {
                    Element.SelectedItem = value;
                }
            }
        }

        private UnifiedMapDelegate UnifiedDelegate => (UnifiedMapDelegate)Control.Delegate;

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

        public void ApplyHasZoomEnabled()
        {
            Control.ZoomEnabled = Element.HasZoomEnabled;
        }

        public void ApplyHasScrollEnabled()
        {
            Control.ScrollEnabled = Element.HasScrollEnabled;
        }

        public void ApplyMapType()
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

        public void ApplyIsShowingUser()
        {
            if (Element.IsShowingUser && _locationManager == null)
            {
                // Request location access permission once
                _locationManager = new CLLocationManager();
                _locationManager.RequestWhenInUseAuthorization();
            }

            Control.ShowsUserLocation = Element.IsShowingUser;
        }

        public void AddPin(IMapPin pin)
        {
            var mapPin = new UnifiedPointAnnotation
            {
                Data = pin,
                Title = pin.Title,
                Subtitle = pin.Snippet,
                Coordinate = new CLLocationCoordinate2D(pin.Location.Latitude, pin.Location.Longitude)
            };

            // pin.Id = mapPin;
            Control.AddAnnotation(mapPin);
        }

        public void RemovePin(IMapPin pin)
        {
            var pins = Control.Annotations
                .OfType<UnifiedPointAnnotation>()
                .Where(point => point.Data == pin)
                .Cast<IMKAnnotation>()
                .ToArray();

            Control.RemoveAnnotations(pins);
        }

        public void AddOverlay(IMapOverlay item)
        {
            IMKOverlay overlay = null;

            if (item is ICircleOverlay)
            {
                var circle = (ICircleOverlay)item;
                overlay = new UnifiedCircleOverlay(MKCircle.Circle(circle.Location.ToCoordinate(), circle.Radius))
                {
                    Data = circle
                };

                Control.AddOverlay(overlay);
                return;
            }

            if (item is IPolylineOverlay)
            {
                var polyline = (IPolylineOverlay)item;
                var coordinates = polyline
                    .Select(p => new CLLocationCoordinate2D(p.Latitude, p.Longitude))
                    .ToArray();

                overlay = new UnifiedPolylineOverlay(MKPolyline.FromCoordinates(coordinates))
                {
                    Data = polyline
                };

                Control.AddOverlay(overlay);
                return;
            }
        }

        public void RemoveOverlay(IMapOverlay item)
        {
            var overlays = Control.Overlays
                .OfType<IUnifiedOverlay>()
                .Where(x => x.Data.Equals(item))
                .Cast<IMKOverlay>()
                .ToArray();

            Control.RemoveOverlays(overlays);
        }

        public void SetSelectedAnnotation()
        {
            var newItem = Control.Annotations
                .OfType<IUnifiedAnnotation>()
                .FirstOrDefault(point => point.Data == SelectedItem) 
                 as IMKAnnotation;
            
            if(newItem == null) return;

            Control.SelectAnnotation(newItem, true);
        }

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
                _behavior.Initialize();
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
                _behavior.Destroy();

                if (Control != null)
                {
                    Control.Delegate = null;
                }

                if (_locationManager != null)
                {
                    _locationManager.Dispose();
                    _locationManager = null;
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
    }
}
