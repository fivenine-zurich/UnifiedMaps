using Android.Gms.Maps;
using Android.OS;
using fivenine.UnifiedMaps;
using fivenine.UnifiedMaps.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(UnifiedMap), typeof(UnifiedMapRenderer))]

namespace fivenine.UnifiedMaps.Droid
{
    public class UnifiedMapRenderer : ViewRenderer<UnifiedMap,MapView>, GoogleMap.IOnCameraChangeListener, IOnMapReadyCallback
    {
        private static Bundle _bundle;
        private GoogleMap _googleMap;

        public UnifiedMapRenderer()
        {
            AutoPackage = false;
        }

        internal static Bundle Bundle
        {
            set { _bundle = value; }
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            _googleMap = googleMap;
        }

        public void OnCameraChange(Android.Gms.Maps.Model.CameraPosition position)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<UnifiedMap> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                if(_googleMap != null )
                {
                    _googleMap.Dispose();
                }
            }

            var mapView = new MapView(Context);

            mapView.OnCreate(_bundle);
            mapView.OnResume();

            SetNativeControl(mapView);

            mapView.GetMapAsync(this);
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            UpdateMapType();
        }

        private void UpdateMapType()
        {
            if (_googleMap == null)
            {
                return;
            }

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
                    break;
            }
        }
    }
}