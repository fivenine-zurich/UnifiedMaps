using System;
using CoreGraphics;
using Foundation;
using MapKit;
using ObjCRuntime;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace fivenine.UnifiedMaps.iOS
{
    [Preserve(AllMembers = true)]
    internal class UnifiedMapDelegate : MKMapViewDelegate
    {
        private const string MKAnnotationIdentifier = "unifiedAnnotation";
        private const string MKPinAnnotationIdentifier = "unifiedPin";

        private readonly UnifiedMapRenderer _renderer;

        private IUnifiedAnnotation _selectedAnnotation;
        private MKAnnotationView _selectedAnnotationView;

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

            var unifiedAnnotation = annotation as IUnifiedAnnotation;
            if (unifiedAnnotation == null)
            {
                return null;
            }

            var pinAnnotation = annotation as UnifiedPointAnnotation;
            if (pinAnnotation != null)
            {
                var data = pinAnnotation.Data;
                MKAnnotationView annotationView = null;

                if (data.Image == null)
                {
                    // Handle standard pins
                    var pinAnnotationView = (MKPinAnnotationView)mapView.DequeueReusableAnnotation(MKPinAnnotationIdentifier) ??
                        new MKPinAnnotationView(annotation, MKPinAnnotationIdentifier);

                    pinAnnotationView.PinTintColor = data.Color.ToUIColor();
                    annotationView = pinAnnotationView;
                }
                else {
                    // Handle pins with an image as pin icon
                    annotationView = mapView.DequeueReusableAnnotation(MKAnnotationIdentifier) ??
                        new MKAnnotationView(annotation, MKAnnotationIdentifier);

                    UpdateImage(annotationView, pinAnnotation.Data);
                }

                // Only show the callout if there is something to display
                annotationView.CanShowCallout = _renderer.Element.CanShowCalloutOnTap && !string.IsNullOrWhiteSpace(pinAnnotation.Data.Title);

                if (annotationView.CanShowCallout
                    && _renderer.Element.PinCalloutTappedCommand != null
                    && pinAnnotation.Data != null)
                {
                    annotationView.RightCalloutAccessoryView = UIButton.FromType(UIButtonType.DetailDisclosure);
                }

                annotationView.Annotation = annotation;
                return annotationView;
            }

            return null;
        }

        public override MKOverlayRenderer OverlayRenderer(MKMapView mapView, IMKOverlay overlay)
        {
            var unifiedOverlay = overlay as IUnifiedOverlay;
            if (unifiedOverlay != null)
            {
                return unifiedOverlay.GetRenderer();
            }

            return null;
        }

        public override void DidSelectAnnotationView(MKMapView mapView, MKAnnotationView view)
        {
            DeselectPin();

            var unifiedPoint = view?.Annotation as UnifiedPointAnnotation;
            _selectedAnnotation = unifiedPoint;
            _selectedAnnotationView = view;

            if (unifiedPoint != null)
            {
                _renderer.SelectedItem = unifiedPoint.Data;
                var isSelected = unifiedPoint.Data?.SelectedImage != null;

                UpdateImage(view, unifiedPoint.Data, isSelected);
                UpdatePinColor(view, unifiedPoint.Data, true);
            }
        }

		public override void DidDeselectAnnotationView(MKMapView mapView, MKAnnotationView view)
		{
			// Fix issue where Pins already deselected internally but it's still highlighted on UI
			DeselectPin();
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

        public override void RegionChanged(MKMapView mapView, bool animated)
        {
            var mkregion = MKCoordinateRegion.FromMapRect(mapView.VisibleMapRect);

            var region = new MapRegion(new Position(mkregion.Center.Latitude, mkregion.Center.Longitude), 
                                       mkregion.Span.LatitudeDelta, mkregion.Span.LongitudeDelta);
            _renderer.Map.VisibleRegion = region;
        }

        internal void SetSelectedAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            var annotationView = mapView.ViewForAnnotation(annotation);
            DidSelectAnnotationView(mapView, annotationView);
        }

        private void UpdatePinColor(MKAnnotationView annotationView, IMapPin customAnnotation, bool selected = false)
        {
            var pinAnnotationView = annotationView as MKPinAnnotationView;
            if (pinAnnotationView != null)
            {
                var color = selected ? customAnnotation.SelectedColor : customAnnotation.Color;
                pinAnnotationView.PinTintColor = color.ToUIColor();
            }
        }

        private void UpdateImage(MKAnnotationView annotationView, IMapPin customAnnotation, bool selected = false)
        {
            if (customAnnotation.Image != null)
            {
                annotationView.Layer.AnchorPoint = new CGPoint(customAnnotation.Anchor.X, customAnnotation.Anchor.Y);

                var newImage = selected ? customAnnotation.SelectedImage : customAnnotation.Image;
                newImage.ToImage()
                    .ContinueWith((image) =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        annotationView.Image = image.Result;
                    });
                });
            }
        }

		private void DeselectPin()
		{
			if (_selectedAnnotation is UnifiedPointAnnotation)
			{
				var prevAnnotation = (UnifiedPointAnnotation)_selectedAnnotation;
				UpdateImage(_selectedAnnotationView, prevAnnotation.Data, false);
				UpdatePinColor(_selectedAnnotationView, prevAnnotation.Data, false);
			}
		}
    }
}