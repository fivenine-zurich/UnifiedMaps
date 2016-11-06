using MapKit;

namespace fivenine.UnifiedMaps.iOS
{
    internal interface IUnifiedOverlay
    {
        IMapOverlay Data { get; }

        MKOverlayRenderer GetRenderer();
    }
}