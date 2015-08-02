using Android.Gms.Maps;
using Android.OS;
using fivenine.UnifiedMaps;
using fivenine.UnifiedMaps.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(UnifiedMap), typeof(UnifiedMapRenderer))]

namespace fivenine.UnifiedMaps.Droid
{
    public class UnifiedMapRenderer : ViewRenderer<UnifiedMap, MapView>
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

        protected override void OnElementChanged(ElementChangedEventArgs<UnifiedMap> e)
        {
            base.OnElementChanged(e);

            Control.OnCreate(_bundle);
            Control.OnResume();
        }
    }
}