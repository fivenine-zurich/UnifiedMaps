using System;
using Foundation;
using MapKit;
using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace fivenine.UnifiedMaps.iOS
{
    [Preserve(AllMembers = true)]
    internal class UnifiedPolylineOverlay : MKPolyline, IUnifiedOverlay
    {
        private MKPolylineRenderer _renderer;
        private IPolylineOverlay _data;

        public UnifiedPolylineOverlay(MKPolyline line)
            : base(line.Handle)
        {
        }

        IMapOverlay IUnifiedOverlay.Data => _data;

        public IPolylineOverlay Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public MKOverlayRenderer GetRenderer()
        {
            if (_renderer == null)
            {
                _renderer = new MKPolylineRenderer(this);
                Update();
            }

            return _renderer;
        }

        public void Update()
        {
            _renderer.StrokeColor = _data.StrokeColor.ToUIColor();
            _renderer.LineWidth = _data.LineWidth;
        }
    }
}