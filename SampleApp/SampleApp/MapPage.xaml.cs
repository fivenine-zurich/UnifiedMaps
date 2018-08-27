using System;
using System.Diagnostics;
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
        }

        private void MapPage_OnAppearing(object sender, EventArgs e)
        {
            Map.MoveToRegion();
            ((UnifiedMapViewModel) BindingContext).Map = Map;
        }
    }
}
