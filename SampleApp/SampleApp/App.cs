using Xamarin.Forms;

namespace Sample
{
    public class App : Application
    {
        public App()
        {
            BindingContext = new UnifiedMapViewModel();
            MainPage = new HomeView();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}