// ReSharper disable once CheckNamespace

using System;
using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.OS;
using fivenine.UnifiedMaps.Droid;
using Android.Gms.Common;

namespace fivenine
{
    public static class UnifiedMap
    {
        public static bool IsInitialized { get; private set; }

        public static Context Context { get; private set; }

        public static void Init(Activity activity, Bundle bundle)
        {
            if (IsInitialized)
                return;
            
            IsInitialized = true;
            Context = activity;
            UnifiedMapRenderer.Bundle = bundle;

            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(Context);
            if (resultCode == ConnectionResult.Success)
            {
                try
                {
                    MapsInitializer.Initialize(Context);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Google Play Services Not Found");
                    Console.WriteLine("Exception: {0}", ex);
                }
            }
            else
            {
                Console.WriteLine("Google Play services not available");
            }
        }
    }
}