using System;
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
        }

        private void MapPage_OnAppearing(object sender, EventArgs e)
        {
            Map.MoveToRegion();
            ((UnifiedMapViewModel) BindingContext).Map = Map;
        }
    }
}
