# UnifiedMaps
*A platform independent map implementation for Xamarin.Forms.*

UnifiedMaps uses the native map APIs on each platform. This provides a fast, familiar maps experience for users, but means that some configuration steps are needed to adhere to each platforms specific API requirements. Once configured, the Map control works just like any other Xamarin.Forms element in common code.

## Maps Initialization
After installing the NuGet package, the following initialization code is required in each application project:

```c#
fivenine.UnifiedMap.Init();
```
T
his call should be made after the Xamarin.Forms.Forms.Init() method call. The Xamarin.Forms templates have this call in the following files for each platform:

 * iOS - AppDelegate.cs file, in the FinishedLaunching method.
 * Android - MainActivity.cs file, in the OnCreate method.
 * Windows Store - MainPage.xaml.cs file, in the MainPage constructor.
Once the NuGet package has been added and the initialization method called inside each application, the UnifiedMaps APIs can be used in the common PCL or Shared Project code.

## Platform Configuration
Additional configuation steps are required on some platforms before the map will display.

### iOS
Add the following keys to the  _Info.plist_ file:
 * NSLocationAlwaysUsageDescription
 * NSLocationWhenInUseUsageDescription


The XML representation is shown below - you should update the string values to reflect how your application is using the location information:

```xml
<key>NSLocationAlwaysUsageDescription</key>
    <string>Can we use your location</string>
<key>NSLocationWhenInUseUsageDescription</key>
    <string>We are using your location</string>
```

### Android
To use the Google Maps API v2 on Android you must generate an API key and add it to your Android project. Follow the instructions in the Xamarin doc on [obtaining a Google Maps API v2 key](http://developer.xamarin.com/guides/android/platform_features/maps_and_location/maps/obtaining_a_google_maps_api_key/) . After following those instructions, paste the API key in the **Properties/AndroidManifest.xml** file.

```xml
<meta-data android:name="com.google.android.maps.v2.API_KEY" android:value="AbCdEfGhIjKlMnOpQrStUvWValueGoesHere" />
```

Without a valid API key the maps control will display as a grey box on Android.

In addition to the API key the following permissions are required:

```xml
<!-- Google Maps for Android v2 requires OpenGL ES v2 -->
<uses-feature android:glEsVersion="0x00020000" android:required="true" />
<!-- Google Maps for Android v2 requires OpenGL ES v2 -->
<uses-feature android:glEsVersion="0x00020000" android:required="true" />
<!-- We need to be able to download map tiles and access Google Play Services-->
<uses-permission android:name="android.permission.INTERNET" />
<!-- Allow the application to access Google web-based services. -->
<uses-permission android:name="com.google.android.providers.gsf.permission.READ_GSERVICES" />
<!-- Google Maps for Android v2 will cache map tiles on external storage -->
<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
<!-- Google Maps for Android v2 needs this permission so that it may check the connection state as it must download data -->
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
<!-- These are optional, but recommended. They will allow Maps to use the My Location provider. -->
<uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
<!-- Notice here that we have the package name of our application as a prefix on the permissions. -->
<uses-permission android:name="ch.fivenine.unifiedmaps.sample.permission.MAPS_RECEIVE" />
<permission android:name="ch.fivenine.unifiedmaps.sample.permission.MAPS_RECEIVE" android:protectionLevel="signature" />
```

### Windows Universal Application
To use the Bing Maps API for Universal Windows Applications you must generate an API key and add it to your Windows Universal project. Follow the instructions on the Windows Dev Center  [Request a maps authentication key](https://msdn.microsoft.com/en-us/library/windows/apps/mt219694.aspx). After generating the *service token*, make sure to call the `fivenine.UnifiedMaps.Init()` method and pass it the generated token.

```c#
fivenine.UnifiedMap.Init("your-map-service-token");
```

In addition you need to declare the location capability in your apps *package.appxmanifest*.
Double click on *package.appxmanifest* in *Solution Explorer* and select the Capabilities tab. Then check *Location* in the *Capabilities list*. This adds the Location device capability to the package manifest file.

```xml
<Capabilities>
  <!-- DeviceCapability elements must follow Capability elements (if present) -->
  <DeviceCapability Name="location"/>
</Capabilities>
```

## Using Maps

### Map Type

### Map Pins

## Using Xaml
