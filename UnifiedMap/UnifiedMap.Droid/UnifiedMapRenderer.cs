using Android.Gms.Maps;
using Android.OS;
using fivenine.UnifiedMaps;
using fivenine.UnifiedMaps.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.App;
using Android.Runtime;
using System;

[assembly: ExportRenderer(typeof(UnifiedMap), typeof(UnifiedMapRenderer))]

namespace fivenine.UnifiedMaps.Droid
{
    public class UnifiedMapRenderer : ViewRenderer, GoogleMap.IOnCameraChangeListener, IJavaObject, IDisposable
    {
        private static Bundle _bundle;

        public UnifiedMapRenderer()
        {
            AutoPackage = false;
        }

        protected GoogleMap NativeMap
        {
            get
            {
                return ((MapView) this.Control).Map;
            }
        }

        protected UnifiedMap Map
        {
            get
            {
                return (UnifiedMap) Element;
            }
        }

        internal static Bundle Bundle
        {
            set { _bundle = value; }
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            
        }

        public void OnCameraChange(Android.Gms.Maps.Model.CameraPosition position)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            MapView mapView1 = (MapView)this.Control;
            MapView mapView2 = new MapView(((Android.Views.View) this).Context);

            mapView2.OnCreate(_bundle);
            mapView2.OnResume();
            SetNativeControl(mapView2);
            if (e.OldElement != null)
            {
                if (mapView1.Map != null)
                {
                }
                ((Java.Lang.Object) mapView1).Dispose();
            }
            GoogleMap nativeMap = this.NativeMap;
            if (nativeMap != null)
            {
                nativeMap.SetOnCameraChangeListener((GoogleMap.IOnCameraChangeListener) this);
                GoogleMap googleMap = nativeMap;
                bool isShowingUser;
            }
        }
    }
}