using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using fivenine.UnifiedMaps;
using Sample.Annotations;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using System;

namespace Sample
{
	[Preserve(AllMembers = true)]
    public class UnifiedMapViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly ICommand _inInfoWindowsClickCommand;
        private readonly ICommand _changeMapTypeCommand;
        private readonly Command _addPinCommand;
        private readonly Command _removePinCommand;
        private readonly ICommand _moveToRegionCommand;
        private readonly ICommand _moveToUserLocationcommand;
        private readonly ICommand _visibleRegionChangedCommand;

        private readonly Command _addPolylineCommand;
        private readonly Command _removePolylineCommand;
        private readonly Command _selectCommand;
        private readonly Command _clearSelectionCommand;

        private readonly LinkedList<IMapPin> _allPins;
        private readonly LinkedList<IMapOverlay> _allPolylines;

        private MapType _mapType = MapType.Street;
        private bool _hasScrollEnabled = true;
        private bool _hasZoomEnabled = true;
        private bool _isShowingUserLocation;
        private bool _cameraAnmiationEnabled = true;
        private IMapAnnotation _selectedItem;

        public UnifiedMapViewModel ()
        {
            _inInfoWindowsClickCommand =
                new Command<IMapPin>(pin => Debug.WriteLine($"The Pin {pin.Title} Info Window has been click"));

            _changeMapTypeCommand =
                new Command<MapType>(m => MapDisplayType = m);

            _addPinCommand =
                new Command(AddPin, o => _allPins.Any());

            _removePinCommand =
                new Command(RemovePin, o => Pins.Any());

            _moveToRegionCommand =
                new Command(() => Map.MoveToRegion(new MapRegion(new Position(47.389097, 8.517756), 400)));

            _addPolylineCommand =
                new Command(AddPolyline, o => _allPolylines.Any());

            _removePolylineCommand =
                new Command(RemovePolyline, o => Overlays.Any());

            _selectCommand =
                new Command<int>(SetSelectedItem, (arg) => Pins.Count > 0);

            _clearSelectionCommand =
                new Command(() => SelectedItem = null);

            _moveToUserLocationcommand =
                new Command(() => Map.MoveToUserLocation(false));

            _visibleRegionChangedCommand =
                new Command<MapRegion>((region) => RegionChanged(region));

            _allPins = new LinkedList<IMapPin> (
                new []
                {
                    new MapPin
                    {
                        Title = "Brändlen",
                        Location = new Position(46.904829, 8.409724),
                        Color = Color.Red
                    },
                    new MapPin
                    {
                        Title = "Wolfenschiessen",
                        Snippet = "... nothing to see here",
                        Location = new Position(46.905180, 8.398110),
                        Color = Color.Blue
                    },
                    new MapPin
                    {
                        Title = "Klewenalp",
                        Location = new Position(46.939898, 8.475217),
                        Color = Color.Fuchsia,
                    },
                    new MapPin
                    {
                        Title = "Beckenried NW",
                        Location = new Position(46.963876, 8.482078),
                        Color = Color.Green,
                    },
                    new MapPin
                    {
                        //Title = "Zürich",
                        //Snippet = "It's awesome",
                        Location = new Position(47.3667, 8.5500),
                        Image = "pin_icon",
                        SelectedImage = "pin_icon_active",
                        Anchor = new Point(0.5, 1)
                    },
                    new MapPin
                    {
                        Title = "fivenine",
                        Snippet = "fivenine GmbH",
                        Location = new Position(47.389097, 8.517756),
                        Image = "pin_icon",
                        SelectedImage = "pin_icon_active",
                        Anchor = new Point(0.5, 1),
                        Draggable = true
                    },
                });

            Pins = new ObservableCollection<IMapPin> ();

            _allPolylines = new LinkedList<IMapOverlay> ();

            var polyline = new PolylineOverlay ();
            foreach (var pin in _allPins) {
                polyline.Add (pin.Location);
            }

            _allPolylines.AddLast (polyline);

            Overlays = new ObservableCollection<IMapOverlay> ();

            Overlays.Add(new CircleOverlay
            {
                Location = new Position(47.389097, 8.517756),
                Radius = 400,
                Color = Color.Navy.MultiplyAlpha(0.2),
                FillColor = Color.Blue.MultiplyAlpha(0.2)
            });

            // Add some sample pins
            AddPin (null);
            AddPin (null);

            // Add some polylines
            AddPolyline (null);

            SelectedItem = Pins.LastOrDefault();
        }

        private void RegionChanged(MapRegion region)
        {
            Debug.WriteLine($"Region changed Lat: {region?.Center.Latitude} Lng: {region?.Center.Longitude}");
        }

        internal UnifiedMap Map { get; set; }

        public ObservableCollection<IMapPin> Pins { get; set; }

        public ObservableCollection<IMapOverlay> Overlays { get; set; }

        public IMapAnnotation SelectedItem 
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        public MapType MapDisplayType 
        {
            get { return _mapType; }
            set {
                _mapType = value;
                OnPropertyChanged ();
            }
        }

        public bool HasScrollEnabled 
        {
            get { return _hasScrollEnabled; }
            set 
            {
                _hasScrollEnabled = value;
                OnPropertyChanged ();
            }
        }

        public bool HasZoomEnabled 
        {
            get { return _hasZoomEnabled; }
            set 
            {
                _hasZoomEnabled = value;
                OnPropertyChanged ();
            }
        }

        public bool IsShowingUserLocation {
            get { return _isShowingUserLocation; }
            set 
            {
                _isShowingUserLocation = value;
                OnPropertyChanged ();
            }
        }

        public bool CameraAnimationEnabled {
            get { return _cameraAnmiationEnabled; }
            set{
                _cameraAnmiationEnabled = value;
                OnPropertyChanged();
            }
        }

        public ICommand PinSelectedCommand => _inInfoWindowsClickCommand;

        public ICommand ChangeMapTypeCommand => _changeMapTypeCommand;

        public ICommand AddPinCommand => _addPinCommand;

        public ICommand RemovePinCommand => _removePinCommand;

        public ICommand MoveToRegionCommand => _moveToRegionCommand;

        public ICommand AddPolylineCommand => _addPolylineCommand;

        public ICommand RemovePolylineCommand => _removePolylineCommand;

        public ICommand SelectCommand => _selectCommand;

        public ICommand ClearSelectionCommand => _clearSelectionCommand;

        public ICommand MoveToUserLocationCommand => _moveToUserLocationcommand;

        public ICommand VisibleRegionChangedCommand => _visibleRegionChangedCommand;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void AddPin(object o)
        {
            if (_allPins.Last == null)
            {
                return;
            }

            var pin = _allPins.Last.Value;
            _allPins.RemoveLast();

            Pins.Add(pin);

            _removePinCommand.ChangeCanExecute();
            _addPinCommand.ChangeCanExecute();
            _selectCommand.ChangeCanExecute();
        }

        private void RemovePin(object o)
        {
            if (Pins.Count == 0)
            {
                return;
            }

            var pin = Pins.LastOrDefault();
            _allPins.AddLast(pin);

            Pins.Remove(pin);

            _removePinCommand.ChangeCanExecute();
            _addPinCommand.ChangeCanExecute();
            _selectCommand.ChangeCanExecute();
        }

        private void AddPolyline(object o)
        {
            if (_allPolylines.Last == null)
            {
                return;
            }

            var polyline = _allPolylines.Last.Value;
            _allPolylines.RemoveLast();

            Overlays.Add(polyline);

            _removePolylineCommand.ChangeCanExecute();
            _addPolylineCommand.ChangeCanExecute();
        }

        private void RemovePolyline(object o)
        {
            if (Overlays.Count == 0)
            {
                return;
            }

            var polyline = Overlays.LastOrDefault();
            _allPolylines.AddLast(polyline);

            Overlays.Remove(polyline);

            _removePolylineCommand.ChangeCanExecute();
            _addPolylineCommand.ChangeCanExecute();
        }

        private void SetSelectedItem(int direction)
        {
            if (SelectedItem == null)
            {
                SelectedItem = Pins.FirstOrDefault();
            }

            var index = Pins.IndexOf(SelectedItem as IMapPin);
            var newIndex = (index + direction) % (Pins.Count);
            if (newIndex < 0)
            {
                newIndex = Pins.Count - 1;
            }

            SelectedItem = Pins[newIndex];
        }
    }
}