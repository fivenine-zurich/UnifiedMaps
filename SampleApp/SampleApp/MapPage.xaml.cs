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
            Map.PinInfoViewClicked += (sender, e) => Debug.WriteLine("Info Window Clicked");
            Map.PinInfoViewLongClicked += (sender, e) => Debug.WriteLine("Info Window Long Clicked");
            Map.MapClicked += (sender, e) => Debug.WriteLine($"Map Clicked, {{latitude: {((Position)e.Value).Latitude} longitude: {((Position)e.Value).Longitude}}}");
            Map.MapLongClicked += (sender, e) => Debug.WriteLine($"Map Long Clicked, {{latitude: {((Position)e.Value).Latitude} longitude: {((Position)e.Value).Longitude}}}");
            Map.PinLongClicked += (sender, e) => Debug.WriteLine("Pin Long Clicked");
            Map.PinDragStart += (sender, e) => Debug.WriteLine($"Pin Drag Start, {{latitude: {((MapPin)e.Value).Location.Latitude} longitude: {((MapPin)e.Value).Location.Longitude}}}");
            Map.PinDragging += (sender, e) => Debug.WriteLine("Pin Dragging");
            Map.PinDragEnd += (sender, e) => Debug.WriteLine($"Pin Drag End, {{latitude: {((MapPin)e.Value).Location.Latitude} longitude: {((MapPin)e.Value).Location.Longitude}}}");
        }

        private void MapPage_OnAppearing(object sender, EventArgs e)
        {
            Map.MoveToRegion();
            ((UnifiedMapViewModel) BindingContext).Map = Map;
        }
    }
}
