using Android.App;
using Android.OS;
using Xamarin.Forms.Platform.Android;
using Sample;
using fivenine;
using Android.Content.PM;

namespace SampleApp.Droid
{
	[Activity (Label = "SampleApp.Droid", MainLauncher = true, Icon = "@drawable/icon",
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : FormsApplicationActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			Xamarin.Forms.Forms.Init (this, bundle);
			UnifiedMap.Init (this, bundle);

			LoadApplication (new App ());
		}
	}
}