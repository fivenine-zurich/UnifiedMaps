using MapKit;
using ObjCRuntime;
using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace fivenine.UnifiedMaps.iOS
{
    internal class UnifiedMapDelegate : MKMapViewDelegate
    {
        private const string PinIdentifier = "unifiedPin";
        private readonly UnifiedMapRenderer _renderer;

        public UnifiedMapDelegate (UnifiedMapRenderer renderer)
        {
            _renderer = renderer;
        }

        public override MKAnnotationView GetViewForAnnotation (MKMapView mapView, IMKAnnotation annotation)
        {
            if (Runtime.GetNSObject (annotation.Handle) is MKUserLocation) {
                return null;
            }

            var pinAnnotation = annotation as UnifiedPointAnnotation;
            if (pinAnnotation != null) {
                var mapPin = (MKPinAnnotationView)mapView.DequeueReusableAnnotation (PinIdentifier) ??
                             new MKPinAnnotationView (annotation, PinIdentifier) {
                                 CanShowCallout = string.IsNullOrWhiteSpace (pinAnnotation.Data.Title) == false,
                                 PinTintColor = pinAnnotation.Data.Color.ToUIColor ()
                             };

                if (_renderer.Element.PinCalloutTappedCommand != null && pinAnnotation.Data != null) {
                    mapPin.RightCalloutAccessoryView = UIButton.FromType (UIButtonType.DetailDisclosure);
                }

                mapPin.Annotation = annotation;
                return mapPin;
            }

            return null;
        }

        public override MKOverlayRenderer OverlayRenderer (MKMapView mapView, IMKOverlay overlay)
        {
            var polylineOverlay = overlay as UnifiedPolylineAnnotation;
            if (polylineOverlay != null) {
                var renderer = new MKPolylineRenderer (polylineOverlay) {
                    StrokeColor = polylineOverlay.StrokeColor,
                    LineWidth = polylineOverlay.LineWidth,
                    Alpha = polylineOverlay.Alpha
                };

                return renderer;
            }

            return null;
        }

        public override void CalloutAccessoryControlTapped (MKMapView mapView, MKAnnotationView view, UIControl control)
        {
            var pinAnnotation = view.Annotation as UnifiedPointAnnotation;
            if (pinAnnotation != null) {
                var pinSelectedCommand = _renderer.Element.PinCalloutTappedCommand;
                if (pinSelectedCommand.CanExecute (pinAnnotation.Data)) {
                    pinSelectedCommand.Execute (pinAnnotation.Data);
                }
            }
        }
    }
}