using MapKit;
using ObjCRuntime;
using UIKit;

namespace fivenine.UnifiedMaps.iOS
{
    internal class UnifiedMapDelegate : MKMapViewDelegate
    {
        private const string PinIdentifier = "unifiedPin";
        private readonly UnifiedMapRenderer _renderer;

        public UnifiedMapDelegate(UnifiedMapRenderer renderer)
        {
            _renderer = renderer;
        }

        public override MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            if (Runtime.GetNSObject(annotation.Handle) is MKUserLocation)
            {
                return null;
            }

            var pinAnnotation = annotation as UnifiedPointAnnotation;
            if (pinAnnotation != null)
            {
                var mapPin = (MKPinAnnotationView) mapView.DequeueReusableAnnotation(PinIdentifier) ??
                             new MKPinAnnotationView(annotation, PinIdentifier)
                             {
                                 CanShowCallout = string.IsNullOrWhiteSpace(pinAnnotation.Data.Title) == false,
                                 PinColor = pinAnnotation.Data.Color.ToMKPinAnnotationColor()
                             };

                if (_renderer.Element.PinCalloutTappedCommand != null && pinAnnotation.Data != null )
                {
                    mapPin.RightCalloutAccessoryView = UIButton.FromType(UIButtonType.DetailDisclosure);
                }

                mapPin.Annotation = annotation;
                return mapPin;
            }

            return null;
        }

        public override void DidSelectAnnotationView(MKMapView mapView, MKAnnotationView view)
        {
            //var pinAnnotation = view.Annotation as UnifiedPointAnnotation;
            //pinAnnotation?.Select();
        }

        public override void CalloutAccessoryControlTapped(MKMapView mapView, MKAnnotationView view, UIControl control)
        {
            var pinAnnotation = view.Annotation as UnifiedPointAnnotation;
            if (pinAnnotation != null)
            {
                var pinSelectedCommand = _renderer.Element.PinCalloutTappedCommand;
                if (pinSelectedCommand.CanExecute(pinAnnotation.Data))
                {
                    pinSelectedCommand.Execute(pinAnnotation.Data);
                }
            }
        }
    }
}