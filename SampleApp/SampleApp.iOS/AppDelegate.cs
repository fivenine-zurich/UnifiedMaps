using Foundation;
using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace Sample.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
	[Register ("AppDelegate")]
	public class AppDelegate : FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Xamarin.Forms.Forms.Init();
            fivenine.UnifiedMap.Init();

            var formsApp = new App();
            LoadApplication(formsApp);

	        return base.FinishedLaunching(app,options);
	    }
    }
}


