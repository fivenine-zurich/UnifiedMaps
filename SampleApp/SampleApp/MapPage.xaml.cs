using System;
using Xamarin.Forms;

namespace Sample
{
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
