using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Util;
using fivenine.UnifiedMaps;
using fivenine.UnifiedMaps.Droid;
using Java.Lang;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(UnifiedMap), typeof(UnifiedMapRenderer))]

namespace fivenine.UnifiedMaps.Droid
{
    public class UnifiedMapRenderer : ViewRenderer<UnifiedMap, MapView>,
        GoogleMap.IOnCameraIdleListener,
        IOnMapReadyCallback, 
        GoogleMap.IOnInfoWindowClickListener,
        GoogleMap.IOnInfoWindowLongClickListener,
        GoogleMap.IOnMarkerClickListener,
        GoogleMap.IOnMarkerDragListener,
        GoogleMap.IOnMapClickListener,
        GoogleMap.IOnMapLongClickListener,
        IUnifiedMapRenderer
    {
        private static Bundle _bundle;
        private readonly RendererBehavior _behavior;

        private readonly Dictionary<IMapAnnotation, Marker> _markers;
        private readonly Dictionary<IPolylineOverlay, Polyline> _polylines;
        private readonly Dictionary<ICircleOverlay, Circle> _circles;

        private GoogleMap _googleMap;
        private bool _requestedShowUserLocation;

		public UnifiedMapRenderer() : base()
        {
            AutoPackage = false;
            _markers = new Dictionary<IMapAnnotation, Marker>();
            _polylines = new Dictionary<IPolylineOverlay, Polyline>();
            _circles = new Dictionary<ICircleOverlay, Circle>();

            _behavior = new RendererBehavior(this);
        }

        public bool RequestedShowUserLocation => _requestedShowUserLocation;

		public UnifiedMapRenderer(IntPtr javaReference, global::Android.Runtime.JniHandleOwnership transfer) : this() { }

        internal static Bundle Bundle
        {
            set { _bundle = value; }
        }

        protected virtual Thickness MapPadding { get; } = new Thickness(48);

        protected IMapPin GetMapPinFromMarker(Marker marker)
        {
            return _markers
                .Where(kv => kv.Value.Id.Equals(marker.Id))
                .Select(kv => kv.Key as IMapPin)
                .FirstOrDefault();
        }

        void OnMyLocationChanged(object sender, GoogleMap.MyLocationChangeEventArgs e)
        {
            if (RequestedShowUserLocation)
            {
                MoveToRegion(MapRegion.FromPositions(new List<Position>{
                    new Position(e.Location.Latitude, e.Location.Longitude)
                }), true);
                ResetShowUserLocation();
            }
        }

        public void ResetShowUserLocation()
        {
            _requestedShowUserLocation = false;
        }

        public void OnInfoWindowClick(Marker marker)
        {
            if (GetMapPinFromMarker(marker) is IMapPin mapPin)
            {
                var command = Element.PinCalloutTappedCommand;
                if (command != null && command.CanExecute(mapPin))
                {
                    command.Execute(mapPin);
                }
                Map.SendPinInfoViewClicked(mapPin);
            }
        }

        public void OnInfoWindowLongClick(Marker marker)
        {
            if (GetMapPinFromMarker(marker) is IMapPin mapPin)
            {
                Map.SendPinInfoViewLongClicked(mapPin);
            }
        }

        public void OnMapClick(LatLng point)
		{
			// Add a conditional property for touch deselection
			if (Map != null)
			{
                // Fix a discrepancy with iOS version. iOS deselects pin when map is tapped, but not on Android.
                if (Map.ShouldDeselectOnMapTouch)
                    Map.SelectedItem = null;

                Map.SendMapClicked(new Position(point.Latitude, point.Longitude));
            }
		}

        public void OnMapLongClick(LatLng point)
        {
            if (Map != null)
            {
                Map.SendMapLongClicked(new Position(point.Latitude, point.Longitude));
            }
        }

        public bool OnMarkerClick(Marker marker)
        {
            var mapPin = GetMapPinFromMarker(marker);

            var selectedPin = Map.SelectedItem;
            if (selectedPin != mapPin)
            {
                if (selectedPin != null && _markers.TryGetValue(selectedPin, out var selectedMarker))
                {
                    UpdateMarkerImage(selectedPin as IMapPin, selectedMarker, false)
                    .HandleExceptions();
                }

                Map.SelectedItem = mapPin;

                // Workaround for the Android limitation of showing info window on marker clicked
                RemovePin(mapPin);
                AddPinAsync(mapPin).HandleExceptions();
            }
            else {
                marker.ShowInfoWindow();
            }
            Map.SendPinClicked(mapPin);
            return true; // return true to bypass default android behavior
        }

        public void OnMarkerDragStart(Marker marker)
        {
            if (GetMapPinFromMarker(marker) is IMapPin mapPin)
            {
                if (mapPin.Draggable)
                {
                    mapPin.Location = new Position(marker.Position.Latitude, marker.Position.Longitude);
                    Map.SendPinDragStart(mapPin);
                }
                else
                {
                    marker.Position = new LatLng(mapPin.Location.Latitude, mapPin.Location.Longitude);
                    Map.SendPinLongClicked(mapPin);
                }
            }
        }

        public void OnMarkerDrag(Marker marker)
        {
            if (GetMapPinFromMarker(marker) is IMapPin mapPin)
            {
                if (mapPin.Draggable)
                {
                    mapPin.Location = new Position(marker.Position.Latitude, marker.Position.Longitude);
                    Map.SendPinDragging(mapPin);
                }
                else
                {
                    marker.Position = new LatLng(mapPin.Location.Latitude, mapPin.Location.Longitude);
                }
            }
        }

        public void OnMarkerDragEnd(Marker marker)
        {
            if (GetMapPinFromMarker(marker) is IMapPin mapPin)
            {
                if (mapPin.Draggable)
                {
                    mapPin.Location = new Position(marker.Position.Latitude, marker.Position.Longitude);
                    Map.SendPinDragEnd(mapPin);
                }
                else
                {
                    marker.Position = new LatLng(mapPin.Location.Latitude, mapPin.Location.Longitude);
                }
            }
        }

        public void OnCameraIdle()
        {
            if (_googleMap != null)
            {
                var mapRegion = _googleMap.Projection.VisibleRegion.LatLngBounds;
                var region = new MapRegion(mapRegion.Southwest.Longitude, mapRegion.Northeast.Latitude,
                                           mapRegion.Northeast.Longitude, mapRegion.Southwest.Latitude);

                Map.VisibleRegion = region;
            }
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            _googleMap = googleMap;

            RegisterListeners();

            _googleMap.MyLocationChange += OnMyLocationChanged;

            ApplyPadding();
            _behavior.Initialize();

            if(Map.VisibleRegion != null) 
            {
                MoveToRegion(Map.VisibleRegion, false);    
            }
        }

        public UnifiedMap Map => Element;

        public void MoveToRegion(MapRegion region, bool animated)
        {
            if (_googleMap == null)
                return;

            var cameraUpdate = CameraUpdateFactory.NewLatLngBounds(region.ToBounds(), 0);
            if (Map.ZoomLevel != -1)
                cameraUpdate = CameraUpdateFactory.NewLatLngZoom(new LatLng(region.Center.Latitude, region.Center.Longitude), Map.ZoomLevel);
            else if (_requestedShowUserLocation)
                cameraUpdate = CameraUpdateFactory.NewLatLngZoom(new LatLng(region.Center.Latitude, region.Center.Longitude), 16);

            try
            {
                if (animated)
                {
                    _googleMap.AnimateCamera(cameraUpdate);
                }
                else
                {
                    _googleMap.MoveCamera(cameraUpdate);
                }
            }
            catch (IllegalStateException e)
            {
                Console.WriteLine("MoveToRegion exception: " + e);
            }
        }

        public void MoveToUserLocation(bool animated)
        {
            _googleMap.MyLocationEnabled = false;
            _googleMap.MyLocationEnabled = true;
            _requestedShowUserLocation = true;
        }

        public void AddPin(IMapPin pin)
        {
            if (_googleMap == null)
                return;

            AddPinAsync(pin).HandleExceptions();
        }

        public void RemovePin(IMapPin pin)
        {
            if (_googleMap == null || pin == null)
                return;

            Marker marker = null;
            if (_markers.TryGetValue(pin, out marker))
            {
                marker.Remove();
                _markers.Remove(pin);
            }
        }

        public void AddOverlay(IMapOverlay item)
        {
            if (_googleMap == null)
            {
                return;
            }

            if (item is ICircleOverlay)
            {
                var circle = (ICircleOverlay)item;

                var options = new CircleOptions();
                options.InvokeCenter(circle.Location.ToLatLng());
                options.InvokeRadius(circle.Radius);
                options.InvokeStrokeWidth(circle.LineWidth);
                options.InvokeStrokeColor(circle.StrokeColor.ToAndroid().ToArgb());
                options.InvokeFillColor(circle.FillColor.ToAndroid().ToArgb());

                var mapCircle = _googleMap.AddCircle(options);
                _circles.Add(circle, mapCircle);
                return;
            }

            if (item is IPolylineOverlay)
            {
                var polyline = (IPolylineOverlay)item;

                var options = new PolylineOptions();
                options.Add(polyline.Select(p => p.ToLatLng()).ToArray());
                options.InvokeColor(polyline.StrokeColor.ToAndroid());
                options.InvokeWidth(polyline.LineWidth);

                var mapPolyline = _googleMap.AddPolyline(options);
                _polylines.Add(polyline, mapPolyline);
                return;
            }
        }

        public void RemoveOverlay(IMapOverlay item)
        {
            if (item is ICircleOverlay)
            {
                var circle = (ICircleOverlay)item;
                Circle mapCircle;
                if (_circles.TryGetValue(circle, out mapCircle))
                {
                    _circles.Remove(circle);
                    mapCircle.Remove();
                }

                return;
            }

            if (item is IPolylineOverlay)
            {
                var polyline = (IPolylineOverlay)item;
                Polyline mapPolyline;
                if (_polylines.TryGetValue(polyline, out mapPolyline))
                {
                    _polylines.Remove(polyline);
                    mapPolyline.Remove();
                }

                return;
            }
        }

        public void FitAllAnnotations(bool animated)
        {
            var region = _behavior.GetRegionForAllAnnotations();
            MoveToRegion(region, false);
        }

        public void ApplyHasZoomEnabled()
        {
            if (_googleMap != null)
            {
                _googleMap.UiSettings.ZoomGesturesEnabled = Element.HasZoomEnabled;
            }
        }

        public void ApplyHasScrollEnabled()
        {
            if (_googleMap != null)
            {
                _googleMap.UiSettings.ScrollGesturesEnabled = Element.HasScrollEnabled;
            }
        }

        public void ApplyIsShowingUser()
        {
            if (_googleMap != null)
            {
                _googleMap.MyLocationEnabled = Element.IsShowingUser;
            }
        }

        public void ApplyMapType()
        {
            if (_googleMap != null)
            {
                switch (Element.MapType)
                {
                    case MapType.Street:
                        _googleMap.MapType = GoogleMap.MapTypeNormal;
                        break;
                    case MapType.Satellite:
                        _googleMap.MapType = GoogleMap.MapTypeSatellite;
                        break;
                    case MapType.Hybrid:
                        _googleMap.MapType = GoogleMap.MapTypeHybrid;
                        break;
                    default:
                        {
                            _googleMap.MapType = GoogleMap.MapTypeNormal;
                            Log.Error("error",
                                $"The map type {Element.MapType} is not supported on Android, falling back to Street");
                            break;
                        }
                }
            }
        }

        public void SetSelectedAnnotation()
        {
            if (Map.SelectedItem is IMapPin selectedItem && _markers.TryGetValue(selectedItem, out var selectedMarker))
            {
                // Refacto -> OnMarkerClick is calling twice, cause of the Marker Click listener
                //OnMarkerClick(selectedMarker);
            }
            else
            {
				// fix an issue where pins are not reset to default state when SelectedItem = null
				DeselectPins(); 
            }
        }

		public void ApplyDisplayNativeControls()
		{
			if (_googleMap != null)
			{
				_googleMap.UiSettings.ZoomControlsEnabled = Element.ShouldDisplayNativeControls;
				_googleMap.UiSettings.MyLocationButtonEnabled = Element.ShouldDisplayNativeControls;
			}
		}

        public void ResetPins()
        {
            foreach (var marker in _markers)
            {
                var markerView = marker.Value;
                markerView.Remove();
            }
            _markers.Clear();
            _googleMap.Clear();
        }

        protected override void OnElementChanged(ElementChangedEventArgs<UnifiedMap> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                var element = e.OldElement;

                _googleMap.MyLocationChange -= OnMyLocationChanged;
                _googleMap?.Dispose();
                RemoveEvents(element);
            }

            if (e.NewElement != null)
            {
                RegisterEvents(e.NewElement);

                if (Control == null)
                {
                    var mapView = new MapView(Context);

                    mapView.OnCreate(_bundle);
                    mapView.OnResume();

                    SetNativeControl(mapView);

                    mapView.GetMapAsync(this);
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

                if (_googleMap != null)
                {
                    RemoveListeners();
                    _googleMap.Dispose();
                    _googleMap = null;
                }
            }

            base.Dispose(disposing);
        }

        private async Task AddPinAsync(IMapPin pin)
        {
            if (_markers.ContainsKey(pin))
            {
                return;
            }

            var mapPin = new MarkerOptions();

            mapPin.InvokeZIndex(pin.ZIndex);

            // For draggable and LongPress Events
            mapPin.Draggable(true);

            if (!string.IsNullOrWhiteSpace(pin.Title))
            {
                mapPin.SetTitle(pin.Title);
            }

            if (!string.IsNullOrWhiteSpace(pin.Snippet))
            {
                mapPin.SetSnippet(pin.Snippet);
            }

            mapPin.SetPosition(new LatLng(pin.Location.Latitude, pin.Location.Longitude));

            if (pin.Image != null)
            {
                mapPin.Anchor((float)pin.Anchor.X, (float)pin.Anchor.Y);
            }

            var selected = pin.EqualsSafe(Map.SelectedItem);
            mapPin.SetIcon(await DeterminMarkerImage(pin, selected));

			if (_markers.ContainsKey(pin))
			{
				return;
			}

            var markerView = _googleMap.AddMarker(mapPin);
            _markers.Add(pin, markerView);

            if (selected && Map.CanShowCalloutOnTap)
            {
                markerView.ShowInfoWindow();
            }
            else
            {
                markerView.HideInfoWindow();
            }
        }

        private void RegisterListeners()
        {
            _googleMap.SetOnInfoWindowClickListener(this);
            _googleMap.SetOnInfoWindowLongClickListener(this);
            _googleMap.SetOnMarkerClickListener(this); 
            _googleMap.SetOnMarkerDragListener(this);
            _googleMap.SetOnCameraIdleListener(this);
            _googleMap.SetOnMapClickListener(this);
            _googleMap.SetOnMapLongClickListener(this);
        }

        private void RemoveListeners()
        {
            _googleMap.SetOnInfoWindowClickListener(null);
            _googleMap.SetOnInfoWindowLongClickListener(null);
            _googleMap.SetOnMarkerClickListener(null);
            _googleMap.SetOnMarkerDragListener(null);
            _googleMap.SetOnCameraIdleListener(null);
            _googleMap.SetOnMapClickListener(null);
            _googleMap.SetOnMapLongClickListener(null);
        }

        private void RegisterEvents(UnifiedMap map)
        {
            _behavior.RegisterEvents(map);
        }

        private void RemoveEvents(UnifiedMap map)
        {
            _behavior.RemoveEvents(map);
        }

        private void ApplyPadding()
        {
            var padding = MapPadding;
            _googleMap.SetPadding((int)padding.Left, (int)padding.Top, (int)padding.Right, (int)padding.Bottom);
        }

        private async Task<BitmapDescriptor> DeterminMarkerImage(IMapPin pin, bool selected)
        {
            if (pin == null)
            {
                return null;
            }

            BitmapDescriptor icon;

            if (pin.Image != null)
            {
                var image = selected && pin.SelectedImage != null ? pin.SelectedImage : pin.Image;
                icon = BitmapDescriptorFactory.FromBitmap(await image.ToBitmap(Context));
            }
            else
            {
                var color = selected ? pin.SelectedColor : pin.Color;
                icon = color.ToStandardMarkerIcon();
            }

            if (icon == null)
            {
                icon = BitmapDescriptorFactory.DefaultMarker();
            }
            return icon;
        }

        private async Task UpdateMarkerImage(IMapPin pin, Marker mapPin, bool selected)
        {
            if (pin == null)
            {
                return;
            }

            try
            {
                mapPin.SetIcon(await DeterminMarkerImage(pin, selected));
            }
            catch
            {
                // ignore
            }
        }

		private async void DeselectPins()
		{
			var safeList = _markers.ToList();
			foreach (var marker in safeList)
			{
				marker.Value.HideInfoWindow();
				var pin = GetMapPinFromMarker(marker.Value);
				await UpdateMarkerImage(pin, marker.Value, false); // force false
			}
		}
    }
}
