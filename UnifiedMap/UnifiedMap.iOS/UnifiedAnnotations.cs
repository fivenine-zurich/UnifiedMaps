using System;
using MapKit;
using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace fivenine.UnifiedMaps.iOS
{
    internal class UnifiedPointAnnotation : MKPointAnnotation
    {
        public MapPin Data { get; set; }
    }

    internal class UnifiedPolylineAnnotation : MKPolyline
    {
        public UnifiedPolylineAnnotation(MKPolyline line)
            : base(line.Handle)
        {
        }

        public MapPolyline Data { get; set; }

        public UIColor StrokeColor => Data.StrokeColor.ToUIColor();

        public nfloat LineWidth => Data.LineWidth;

        public nfloat Alpha => Data.Alpha;
    }
}