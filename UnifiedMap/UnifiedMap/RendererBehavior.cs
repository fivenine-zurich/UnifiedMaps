using System;
using System.Collections.Specialized;
using System.Linq;
using Xamarin.Forms;

namespace fivenine.UnifiedMaps
{
    internal interface IUnifiedMapRenderer
    {
        UnifiedMap Map { get; }

        void AddPin(IMapPin item);

        void RemovePin(IMapPin item);

        void AddOverlay(IMapOverlay item);

        void RemoveOverlay(IMapOverlay item);

        void FitAllAnnotations(bool animated);

        void MoveToRegion(MapRegion region, bool animated);

        void MoveToUserLocation(bool animated);

        void ApplyHasZoomEnabled();

        void ApplyHasScrollEnabled();

        void ApplyIsShowingUser();

        void ApplyMapType();

        void SetSelectedAnnotation();

		void ApplyDisplayNativeControls();
        
        void ResetPins();
    }

    internal class RendererBehavior
    {
        private readonly IUnifiedMapRenderer _renderer;

        private INotifyCollectionChanged _observablePins;
        private INotifyCollectionChanged _observableOverlays;

        public RendererBehavior(IUnifiedMapRenderer renderer)
        {
            _renderer = renderer;
        }

        public bool CameraAnimationEnabled => _renderer.Map.CameraAnimationEnabled;

        internal void RegisterEvents(UnifiedMap map)
        {
            RegisterPinEvents(map);
            RegisterOverlayEvents(map);

            MessagingCenter.Subscribe<UnifiedMap, Tuple<MapRegion, bool>>(this, map.GetMoveToRegionMessage(),
                (unifiedMap, span) => MoveToRegion(span.Item1, span.Item2));

            MessagingCenter.Subscribe<UnifiedMap, bool>(this, map.GetMoveToUserLocationMessage(),
                (unifiedMap, animated) => MoveToUserLocation(animated));
            
        }

        internal void RemoveEvents(UnifiedMap map)
        {
            MessagingCenter.Unsubscribe<UnifiedMap, Tuple<MapRegion, bool>>(this, map.GetMoveToRegionMessage());
            MessagingCenter.Unsubscribe<UnifiedMap, bool>(this, map.GetMoveToUserLocationMessage());

            RemovePinEvents();
            RemoveOverlayEvents();
        }

        internal void RegisterPinEvents(UnifiedMap map)
        {
            var pinsObservable = map.Pins as INotifyCollectionChanged;
            if (pinsObservable != null && pinsObservable != _observablePins)
            {
                if (_observablePins != null)
                {
                    _observablePins.CollectionChanged -= OnPinsCollectionChanged;
                }

                pinsObservable.CollectionChanged += OnPinsCollectionChanged;
                _observablePins = pinsObservable;
            }
        }

        internal void RemovePinEvents()
        {
            if (_observablePins != null)
            {
                _observablePins.CollectionChanged -= OnPinsCollectionChanged;
            }
        }

        internal void RegisterOverlayEvents(UnifiedMap map)
        {
            var overlaysObservable = map.Overlays as INotifyCollectionChanged;
            if (overlaysObservable != null && overlaysObservable != _observableOverlays)
            {
                if (_observableOverlays != null)
                {
                    _observableOverlays.CollectionChanged -= OnOverlaysCollectionChanged;
                }

                overlaysObservable.CollectionChanged += OnOverlaysCollectionChanged;
                _observableOverlays = overlaysObservable;
            }
        }

        internal void RemoveOverlayEvents()
        {
            if (_observableOverlays != null)
            {
                _observableOverlays.CollectionChanged -= OnOverlaysCollectionChanged;
            }
        }

        internal MapRegion GetRegionForAllAnnotations()
        {
            var allPinPositions = _renderer.Map.Pins?.OfType<IMapPin>()?.Select(p => p.Location);

            return allPinPositions == null || allPinPositions.Count() == 0 ? MapRegion.Empty() : MapRegion.FromPositions(allPinPositions);
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


        private void MoveToUserLocation(bool animated)
        {
            _renderer.MoveToUserLocation(animated);
        }


        private void AddAllPins()
        {
            // Todo Clear Pins
            var pins = _renderer.Map.Pins;
            if (pins != null)
            {
                foreach (var pin in pins.OfType<IMapPin>())
                {
                    _renderer.AddPin(pin);
                }
            }

            if (_renderer.Map.AutoFitAllAnnotations)
            {
                _renderer.FitAllAnnotations(CameraAnimationEnabled);
            }
        }

        private void AddAllOverlays()
        {
            var overlays = _renderer.Map.Overlays;
            if (overlays != null)
            {
                foreach (var overlay in overlays.OfType<IMapOverlay>())
                {
                    _renderer.AddOverlay(overlay);
                }
            }

            if (_renderer.Map.AutoFitAllAnnotations)
            {
                _renderer.FitAllAnnotations(false);
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
                        _renderer.AddPin((IMapPin) item);
                    }

                    break;
                }
                case NotifyCollectionChangedAction.Remove:
                {
                    foreach (var item in e.OldItems)
                    {
                        _renderer.RemovePin((IMapPin) item);
                    }

                    break;
                }
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                    _renderer.ResetPins();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (_renderer.Map.AutoFitAllAnnotations)
            {
                _renderer.FitAllAnnotations(CameraAnimationEnabled);
            }
        }

        private void OnOverlaysCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                {
                    foreach (var item in e.NewItems)
                    {
                        _renderer.AddOverlay((IMapOverlay) item);
                    }

                    break;
                }
                case NotifyCollectionChangedAction.Remove:
                {
                    foreach (var item in e.OldItems)
                    {
                        _renderer.RemoveOverlay((IMapOverlay) item);
                    }

                    break;
                }
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                    throw new NotSupportedException($"The operation {e.Action} is not supported for Overlays");
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (_renderer.Map.AutoFitAllAnnotations)
            {
                _renderer.FitAllAnnotations(CameraAnimationEnabled);
            }
        }

        internal void ElementProperyChanged(string propertyName)
        {
            if (propertyName == UnifiedMap.MapTypeProperty.PropertyName)
            {
                _renderer.ApplyMapType();
            }

			if (propertyName == UnifiedMap.ShouldDisplayNativeControlsProperty.PropertyName)
			{
				_renderer.ApplyDisplayNativeControls();
			}

            if (propertyName == UnifiedMap.IsShowingUserProperty.PropertyName)
            {
                _renderer.ApplyIsShowingUser();
            }

            if (propertyName == UnifiedMap.HasZoomEnabledProperty.PropertyName)
            {
                _renderer.ApplyHasZoomEnabled();
            }

            if (propertyName == UnifiedMap.HasScrollEnabledProperty.PropertyName)
            {
                _renderer.ApplyHasScrollEnabled();
            }

            if (propertyName == UnifiedMap.PinsProperty.PropertyName)
            {
                RegisterPinEvents(_renderer.Map);
                AddAllPins();
            }

            if (propertyName == UnifiedMap.OverlaysProperty.PropertyName)
            {
                RegisterOverlayEvents(_renderer.Map);
                AddAllOverlays();
            }

            if (propertyName == UnifiedMap.SelectedItemProperty.PropertyName)
            {
                _renderer.SetSelectedAnnotation();
            }

            if (propertyName == VisualElement.WidthProperty.PropertyName 
                || propertyName == VisualElement.HeightProperty.PropertyName) 
            {
                if (_renderer.Map.AutoFitAllAnnotations)
                {
                    _renderer.FitAllAnnotations(false);
                }
            }
        }

        internal void Initialize()
        {
            _renderer.ApplyMapType();
            _renderer.ApplyHasScrollEnabled();
            _renderer.ApplyHasZoomEnabled();
            _renderer.ApplyIsShowingUser();
            _renderer.ApplyDisplayNativeControls();

            AddAllPins();
            AddAllOverlays();
        }

        internal void Destroy()
        {
            var observablePins = _renderer.Map.Pins as INotifyCollectionChanged;
            if (observablePins != null)
            {
                observablePins.CollectionChanged -= OnPinsCollectionChanged;
            }
        }
    }
}