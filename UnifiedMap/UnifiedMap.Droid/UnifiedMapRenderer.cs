using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Util;
using fivenine.UnifiedMaps;
using fivenine.UnifiedMaps.Droid;
using Java.Lang;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof (UnifiedMap), typeof (UnifiedMapRenderer))]

namespace fivenine.UnifiedMaps.Droid
{
    public class UnifiedMapRenderer : ViewRenderer<UnifiedMap, MapView>, GoogleMap.IOnCameraChangeListener,
        IOnMapReadyCallback, GoogleMap.IOnInfoWindowClickListener, IUnifiedMapRenderer
    {
        private static Bundle _bundle;
        private readonly RendererBehavior _behavior;
        private readonly LinkedList<Tuple<Marker, MapPin>> _markers;
        private readonly Dictionary<MapPolyline, Polyline> _polylines;
        
        private GoogleMap _googleMap;

        public UnifiedMapRenderer()
        {
            AutoPackage = false;
            _markers = new LinkedList<Tuple<Marker, MapPin>>();
            _polylines = new Dictionary<MapPolyline, Polyline>();
            _behavior = new RendererBehavior(this);
        }

        internal static Bundle Bundle
        {
            set { _bundle = value; }
        }

        protected virtual Thickness MapPadding { get; } = new Thickness(48);

        public void OnCameraChange(CameraPosition position)
        {
        }

        public void OnInfoWindowClick(Marker marker)
        {
            var mapPin = _markers
                .Where(tuple => tuple.Item1.Id == marker.Id)
                .Select(tuple => tuple.Item2)
                .FirstOrDefault();

            var command = Element.PinCalloutTappedCommand;
            if (command != null && command.CanExecute(mapPin))
            {
                command.Execute(mapPin);
            }
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            _googleMap = googleMap;

            // Register listeners
            _googleMap.SetOnInfoWindowClickListener(this);

            ApplyPadding();
            _behavior.Initialize();
        }

        public UnifiedMap Map => Element;

        public void MoveToRegion(MapRegion region, bool animated)
        {
            if (_googleMap == null)
            {
                return;
            }

            var cameraUpdate = CameraUpdateFactory.NewLatLngBounds(region.ToBounds(), 0);

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
            catch (IllegalStateException)
            {
            }
        }

        public void AddPin(MapPin pin)
        {
            if (_googleMap == null)
                return;

            var mapPin = new MarkerOptions();

            mapPin.SetTitle(pin.Title);
            mapPin.SetPosition(new LatLng(pin.Location.Latitude, pin.Location.Longitude));
            mapPin.SetIcon(pin.Color.ToStandardMarkerIcon());
            mapPin.SetSnippet(pin.Snippet);

            var marker = _googleMap.AddMarker(mapPin);
            _markers.AddLast(new Tuple<Marker, MapPin>(marker, pin));
        }

        public void RemovePin(MapPin pin)
        {
            if (_googleMap == null || pin == null)
                return;

            var markers = _markers
                .Where(kv => kv.Item2 == pin)
                .ToList();

            foreach (var marker in markers)
            {
                marker.Item1.Remove();
                _markers.Remove(marker);
            }
        }

        public void AddPolyline(MapPolyline line)
        {
            if (_googleMap != null)
            {
                var options = new PolylineOptions();
                options.Add(line.Select(p => p.ToLatLng()).ToArray());
                options.InvokeColor(line.StrokeColor.ToAndroid().ToArgb());
                options.InvokeWidth(line.LineWidth);

                var polyline = _googleMap.AddPolyline(options);
                _polylines.Add(line, polyline);
            }
        }

        public void RemovePolyline(MapPolyline line)
        {
            Polyline polyline;
            if (_polylines.TryGetValue(line, out polyline))
            {
                polyline.Remove();
                _polylines.Remove(line);
            }
        }

        public void FitAllAnnotations(bool animated)
        {
            var region = _behavior.GetRegionForAllAnnotations();
            MoveToRegion(region, animated);
        }

        public void ApplyHasZoomEnabled()
        {
            if (_googleMap != null)
            {
                _googleMap.UiSettings.ZoomGesturesEnabled = Element.HasZoomEnabled;
                _googleMap.UiSettings.ZoomControlsEnabled = Element.HasZoomEnabled;
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
                _googleMap.UiSettings.MyLocationButtonEnabled = Element.IsShowingUser;
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

        protected override void OnElementChanged(ElementChangedEventArgs<UnifiedMap> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                var element = e.OldElement;

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
                    _googleMap.Dispose();
                    _googleMap = null;
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

        private void ApplyPadding()
        {
            var padding = MapPadding;
            _googleMap.SetPadding((int) padding.Left, (int) padding.Top, (int) padding.Right, (int) padding.Bottom);
        }
    }
}