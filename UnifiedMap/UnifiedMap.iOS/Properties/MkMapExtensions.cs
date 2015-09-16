using MapKit;

namespace fivenine.UnifiedMaps.iOS
{
    public static class MkMapExtensions
    {
        public static MKPinAnnotationColor ToMKPinAnnotationColor(this PinColor color)
        {
            switch (color)
            {
                case PinColor.Purple:
                    return MKPinAnnotationColor.Purple;
                case PinColor.Green:
                    return MKPinAnnotationColor.Green;
                default:
                    return MKPinAnnotationColor.Red;
            }
        }
    }
}