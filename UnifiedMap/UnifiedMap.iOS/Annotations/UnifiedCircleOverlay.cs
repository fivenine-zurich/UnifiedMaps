using MapKit;
using Xamarin.Forms.Platform.iOS;

namespace fivenine.UnifiedMaps.iOS
{
    public class UnifiedCircleOverlay : MKCircle, IUnifiedOverlay
    {
        private MKCircleRenderer _renderer;
        private ICircleOverlay _data;

        public UnifiedCircleOverlay(MKCircle circle)
            : base(circle.Handle)
        {
        }

        IMapOverlay IUnifiedOverlay.Data => _data;

        public ICircleOverlay Data 
        {
            get { return _data; }
            set { _data = value;}
        }

        public MKOverlayRenderer GetRenderer()
        {
            if (_renderer == null)
            {
                _renderer = new MKCircleRenderer(this);
                Update();
            }

            return _renderer;
        }

        public void Update()
        {
            _renderer.StrokeColor = _data.Color.ToUIColor();
            _renderer.LineWidth = _data.LineWidth;
            _renderer.FillColor = _data.FillColor.ToUIColor();
        }
    }
}
