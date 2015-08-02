using Android.App;
using Android.OS;
using fivenine;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace Sample.Droid
{
	[Activity (Label = "Sample.Droid", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : FormsApplicationActivity
	{
	    protected override void OnCreate(Bundle bundle)
	    {
	        base.OnCreate(bundle);

	        Forms.Init(this, bundle);
            UnifiedMap.Init(this,bundle);

	        LoadApplication(new App());
	    }
	}
}


