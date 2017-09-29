using System;
using MapKit;

namespace fivenine.UnifiedMaps.iOS
{

    internal class UnifiedPointAnnotation : MKPointAnnotation, IUnifiedAnnotation
    {
        private IMapPin _data;

        public IMapPin Data 
        {
            get { return _data;  }
            set { this._data = value; }
        }

        IMapAnnotation IUnifiedAnnotation.Data => _data;
    }
}
