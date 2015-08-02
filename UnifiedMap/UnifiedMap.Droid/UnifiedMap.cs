// ReSharper disable once CheckNamespace

using System;
using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.OS;
using fivenine.UnifiedMaps.Droid;
using static Android.Gms.Common.GooglePlayServicesUtil;

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
            if (IsGooglePlayServicesAvailable(Context) == 0)
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
        }
    }
}