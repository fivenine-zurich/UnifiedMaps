using System;
using System.Collections.Specialized;
using System.Linq;
using Xamarin.Forms;

namespace fivenine.UnifiedMaps
{
    internal interface IUnifiedMapRenderer
    {
        UnifiedMap Map { get; }

        void MoveToRegion(MapRegion region, bool animated);

        void AddPin(MapPin item);

        void RemovePin(MapPin item);

        void FitAllAnnotations(bool animated);
    }

    internal class RendererBehavior
    {
        private readonly IUnifiedMapRenderer _renderer;

        public RendererBehavior(IUnifiedMapRenderer renderer)
        {
            _renderer = renderer;
        }

        internal void RegisterEvents(UnifiedMap map)
        {
            if (map.Pins != null)
            {
                map.Pins.CollectionChanged += OnPinsCollectionChanged;
            }

            MessagingCenter.Subscribe<UnifiedMap, Tuple<MapRegion, bool>>(this, UnifiedMap.MessageMapMoveToRegion,
                (unifiedMap, span) => MoveToRegion(span.Item1, span.Item2));
        }
        
        internal void RemoveEvents(UnifiedMap map)
        {
            MessagingCenter.Unsubscribe<UnifiedMap, Tuple<MapRegion, bool>>(this, UnifiedMap.MessageMapMoveToRegion);

            if (map.Pins != null)
            {
                map.Pins.CollectionChanged -= OnPinsCollectionChanged;
            }
        }

        internal MapRegion GetRegionForAllAnnotations()
        {
            var allPinPositions = _renderer.Map.Pins
                .Select(p => p.Location);

            return MapRegion.FromPositions(allPinPositions);
        }

        private void MoveToRegion(MapRegion mapRegion, bool animated)
        {
            if (mapRegion == null)
            {
                // No region specified, fit all annotations
                _renderer.FitAllAnnotations(animated);
            }
            else
            {
                _renderer.MoveToRegion(mapRegion, animated);
            }
        }

        private void OnPinsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                {
                    foreach (var item in e.NewItems)
                    {
                        _renderer.AddPin((MapPin) item);
                    }

                    break;
                }
                case NotifyCollectionChangedAction.Remove:
                {
                    foreach (var item in e.OldItems)
                    {
                        _renderer.RemovePin((MapPin) item);
                    }

                    break;
                }
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                    throw new NotSupportedException($"The operation {e.Action} is not supported for MapPins");
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (_renderer.Map.AutoFitAllAnnotations)
            {
                _renderer.FitAllAnnotations(true);
            }
        }
    }
}
