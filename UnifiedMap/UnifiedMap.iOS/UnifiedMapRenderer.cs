using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using CoreLocation;
using fivenine.UnifiedMaps;
using fivenine.UnifiedMaps.iOS;
using Foundation;
using MapKit;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof (UnifiedMap), typeof (UnifiedMapRenderer))]

namespace fivenine.UnifiedMaps.iOS
{
    [Preserve(AllMembers = true)]
    public class UnifiedMapRenderer : ViewRenderer<UnifiedMap, MKMapView>, IUnifiedMapRenderer
    {
        private readonly RendererBehavior _behavior;
        private readonly List<IUnifiedOverlay> _overlays = new List<IUnifiedOverlay>();
        private CLLocationManager _locationManager;
		private bool _shouldNotDismiss;
        private bool _requestedShowUserLocation = false;

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

        public void MoveToRegion(MapRegion mapRegion, bool animated)
        {
            Control.SetRegion(new MKCoordinateRegion(
                mapRegion.Center.ToCoordinate(),
                mapRegion.ToSpan()), animated);
        }

        public void MoveToUserLocation(bool animated)
        {
            Control.ShowsUserLocation = true;
            Control.SetCenterCoordinate(Control.UserLocation.Coordinate, animated);

            _requestedShowUserLocation = true;
        }

        public bool RequestedShowUserLocation => _requestedShowUserLocation;

        public void ResetShowUserLocation() {
            _requestedShowUserLocation = false;
            Control.ShowsUserLocation = false;
        }

        public void FitAllAnnotations(bool animated)
        {
            if(Control.Annotations.Length == 1) 
            {
                var singleAnnotation = Control.Annotations[0];
                var region = new MapRegion(new Position(singleAnnotation.Coordinate.Latitude, singleAnnotation.Coordinate.Longitude), Element.IosSingleAnnotationZoom, Element.IosSingleAnnotationZoom);
                MoveToRegion(region, animated);
            } 
            else 
            {
                Control.ShowAnnotations(Control.Annotations, animated);    
            }
        }

        public void ApplyHasZoomEnabled()
        {
            Control.ZoomEnabled = Element.HasZoomEnabled;
        }

        public void ApplyHasScrollEnabled()
        {
            Control.ScrollEnabled = Element.HasScrollEnabled;
        }

		public void ApplyDisplayNativeControls()
		{
			// Does nothing as iOS does not have native zoom and location buttons
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
            if (item is ICircleOverlay)
            {
                var circle = (ICircleOverlay)item;
                var overlay = new UnifiedCircleOverlay(MKCircle.Circle(circle.Location.ToCoordinate(), circle.Radius))
                {
                    Data = circle
                };

                _overlays.Add(overlay);
                Control.AddOverlay(overlay);
                return;
            }

            if (item is IPolylineOverlay)
            {
                var polyline = (IPolylineOverlay)item;
                var coordinates = polyline
                    .Select(p => new CLLocationCoordinate2D(p.Latitude, p.Longitude))
                    .ToArray();

                var overlay = new UnifiedPolylineOverlay(MKPolyline.FromCoordinates(coordinates))
                {
                    Data = polyline
                };

                _overlays.Add(overlay);
                Control.AddOverlay(overlay);
                return;
            }
        }

        public void RemoveOverlay(IMapOverlay item)
        {
            var overlay = _overlays.FirstOrDefault(x => x.Data == item);

            if (overlay != null)
            {
                Control.RemoveOverlay((IMKOverlay)overlay);
                _overlays.Remove(overlay);
            }
        }

        public void SetSelectedAnnotation()
        {
            var newItem = Control.Annotations
                .OfType<IUnifiedAnnotation>()
                .FirstOrDefault(point => point.Data == SelectedItem) 
                 as IMKAnnotation;

            if (newItem == null)
            {
				// fix an issue where pins are not reset to default state when SelectedItem = null
				foreach (var annotation in Control.Annotations)
					Control.DeselectAnnotation(annotation, false);

                return;
            }
			
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

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			base.TouchesBegan(touches, evt);
			_shouldNotDismiss = false; // reset
		}

		public override void TouchesMoved(NSSet touches, UIEvent evt)
		{
			base.TouchesMoved(touches, evt);
			_shouldNotDismiss = true;
		}

		public override void TouchesCancelled(NSSet touches, UIEvent evt)
		{
			base.TouchesCancelled(touches, evt);
			_shouldNotDismiss = true;
		}

		public override void TouchesEnded(NSSet touches, UIEvent evt)
		{
			base.TouchesEnded(touches, evt);

			// Add a conditional guard to deselect on map touch
			if (Element != null && Element.ShouldDeselectOnMapTouch && _shouldNotDismiss)
			{
				var isAnnotation = false;
				var touch = touches.FirstOrDefault() as UITouch;
				if (touch != null && Control != null)
				{
					// Detect when an annotation is touched
					// May be enhanced in the future to detect when an MKCircle, MKPolyline or MKPointAnnotaiton is touched
					if (touch.View is MKAnnotationView)
					{
						isAnnotation = true;
					}
				}

				if (!isAnnotation)
				{
					// Deselect annotation when map is touched
					SelectedItem = null;
				}
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
