using System;
using System.Collections.Generic;
using CoreGraphics;
using CoreLocation;
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
        UILongPressGestureRecognizer _PinInfoViewLongPress;

        public UnifiedMapDelegate(UnifiedMapRenderer renderer)
        {
            _renderer = renderer;
            _PinInfoViewLongPress = new UILongPressGestureRecognizer(HandlePinInfoViewLongPress);
        }

        public override MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            if (annotation is UnifiedPointAnnotation pinAnnotation)
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

                // For Draggable
                annotationView.Draggable = pinAnnotation.Data.Draggable;

                // z index
                annotationView.Layer.ZPosition = pinAnnotation.Data.ZIndex;

                // Only show the callout if there is something to display
                annotationView.CanShowCallout = _renderer.Element.CanShowCalloutOnTap && !string.IsNullOrWhiteSpace(pinAnnotation.Data.Title);

                if (annotationView.CanShowCallout && pinAnnotation.Data != null)
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
            if (overlay is IUnifiedOverlay unifiedOverlay)
            {
                return unifiedOverlay.GetRenderer();
            }
            return null;
        }

        public override void DidSelectAnnotationView(MKMapView mapView, MKAnnotationView view)
        {
            DeselectPin();

            if (view?.Annotation is UnifiedPointAnnotation unifiedPoint)
            {
                _selectedAnnotation = unifiedPoint;
                _selectedAnnotationView = view;

                _selectedAnnotationView.AddGestureRecognizer(_PinInfoViewLongPress);

                _renderer.SelectedItem = unifiedPoint.Data;
                var isSelected = unifiedPoint.Data?.SelectedImage != null;

                UpdateImage(view, unifiedPoint.Data, isSelected);
                UpdatePin(view, unifiedPoint.Data, true);
                view.Layer.ZPosition = int.MaxValue - 1;
            }
            else if (view.Class.Name == "MKModernUserLocationView")
            {
                // Ensure that when UserLocation pin is added it's always at the top
                view.Layer.ZPosition = int.MaxValue;
                // Remove InfoWindow from MKModernUserLocationView
                view.CanShowCallout = false;
            }
        }

        public override void DidDeselectAnnotationView(MKMapView mapView, MKAnnotationView view)
        {
            // Fix issue where Pins already deselected internally but it's still highlighted on UI
            DeselectPin();

            if (view?.Annotation is UnifiedPointAnnotation unifiedPoint)
            {
                _selectedAnnotationView.RemoveGestureRecognizer(_PinInfoViewLongPress);
            }
        }

        private void HandlePinInfoViewLongPress(UILongPressGestureRecognizer o)
        {
            if (o.State == UIGestureRecognizerState.Began
                && o.View is MKAnnotationView view
                && view.Annotation is UnifiedPointAnnotation unifiedPoint)
            {
                _renderer.Map.SendPinInfoViewLongClicked(unifiedPoint.Data);
            }
        }

        public override void ChangedDragState(MKMapView mapView, MKAnnotationView annotationView, MKAnnotationViewDragState newState, MKAnnotationViewDragState oldState)
        {
            if (annotationView?.Annotation is UnifiedPointAnnotation unifiedPoint)
            {
                switch (newState)
                {
                    case MKAnnotationViewDragState.Starting:
                        _renderer.Map.SendPinDragStart(unifiedPoint.Data);
                        break;
                    case MKAnnotationViewDragState.Dragging:
                        _renderer.Map.SendPinDragging(unifiedPoint.Data);
                        break;
                    case MKAnnotationViewDragState.Ending:
                        _renderer.Map.SendPinDragEnd(unifiedPoint.Data);
                        annotationView.SetDragState(MKAnnotationViewDragState.None, false);
                        break;
                    case MKAnnotationViewDragState.Canceling:
                        annotationView.SetDragState(MKAnnotationViewDragState.None, false);
                        break;
                    default:
                        break;
                }
                unifiedPoint.Data.Location = new Position(unifiedPoint.Coordinate.Latitude, unifiedPoint.Coordinate.Longitude);
            }
        }

        public override void DidAddAnnotationViews(MKMapView mapView, MKAnnotationView[] views)
        {
            foreach (var view in views)
            {
                if (view?.Annotation is UnifiedPointAnnotation unifiedPoint)
                {
                    view.Layer.ZPosition = unifiedPoint.Data.ZIndex;
                }
                else if (view.Class.Name == "MKModernUserLocationView")
                {
                    // Ensure that when UserLocation pin is added it's always at the top
                    view.Layer.ZPosition = int.MaxValue;
                    // Remove InfoWindow from MKModernUserLocationView
                    view.CanShowCallout = false;
                }
            }
        }

        public override void CalloutAccessoryControlTapped(MKMapView mapView, MKAnnotationView view, UIControl control)
        {
            if (view.Annotation is UnifiedPointAnnotation pinAnnotation)
            {
                var pinSelectedCommand = _renderer.Element.PinCalloutTappedCommand;
                if (pinSelectedCommand != null && pinSelectedCommand.CanExecute(pinAnnotation.Data))
                {
                    pinSelectedCommand.Execute(pinAnnotation.Data);
                }
                _renderer.Map.SendPinInfoViewClicked(pinAnnotation.Data);
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

        private void UpdatePin(MKAnnotationView annotationView, IMapPin customAnnotation, bool selected = false)
        {
            if (annotationView is MKPinAnnotationView pinAnnotationView)
            {
                var color = selected ? customAnnotation.SelectedColor : customAnnotation.Color;
                pinAnnotationView.PinTintColor = color.ToUIColor();
                pinAnnotationView.Layer.ZPosition = customAnnotation.ZIndex;
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
                UpdatePin(_selectedAnnotationView, prevAnnotation.Data, false);
            }
        }
    }
}