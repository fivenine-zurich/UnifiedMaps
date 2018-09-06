using System;
using System.Diagnostics;
using fivenine.UnifiedMaps;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Sample
{
	[Preserve(AllMembers = true)]
    public partial class MapPage : ContentPage
    {
        public MapPage()
        {
            InitializeComponent();

            Map.PinClicked += (sender, e) => Debug.WriteLine("Pin Clicked");
            Map.InfoWindowClicked += (sender, e) => Debug.WriteLine("Info Window Clicked");
            Map.MapClicked += (sender, e) => Debug.WriteLine($"Map Clicked, {{latitude: {((Position)e.Value).Latitude} longitude: {((Position)e.Value).Longitude}}}");
            Map.MapLongClicked += (sender, e) => Debug.WriteLine($"Map Long Clicked, {{latitude: {((Position)e.Value).Latitude} longitude: {((Position)e.Value).Longitude}}}");
        }

        private void MapPage_OnAppearing(object sender, EventArgs e)
        {
            Map.MoveToRegion();
            ((UnifiedMapViewModel) BindingContext).Map = Map;
        }
    }
}
