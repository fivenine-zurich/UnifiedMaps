using Android.Gms.Maps;
using Android.OS;
using fivenine.UnifiedMaps;
using fivenine.UnifiedMaps.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.App;

[assembly: ExportRenderer(typeof(UnifiedMap), typeof(UnifiedMapRenderer))]

namespace fivenine.UnifiedMaps.Droid
{
    public class UnifiedMapRenderer : ViewRenderer<UnifiedMap, MapView>, IOnMapReadyCallback
    {
        private static Bundle _bundle;

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
            
        }

        protected override void OnElementChanged(ElementChangedEventArgs<UnifiedMap> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
            }
            else
            {
                GoogleMapOptions mapOptions = new GoogleMapOptions()
                    .InvokeMapType(GoogleMap.MapTypeSatellite)
                    .InvokeZoomControlsEnabled(false)
                    .InvokeCompassEnabled(true);

                var fragment = MapFragment.NewInstance(mapOptions);
                fragment.GetMapAsync(this);

                var view = new MapView(Context, mapOptions);
            }
        }
    }
}