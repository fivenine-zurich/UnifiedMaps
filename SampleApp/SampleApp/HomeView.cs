using System;
using Xamarin.Forms;

namespace Sample
{
    public class HomeView : MasterDetailPage
    {
        public HomeView()
        {
            Master = new MenuPage();
            Detail = new NavigationPage(new MapPage());
        }
    }
}

