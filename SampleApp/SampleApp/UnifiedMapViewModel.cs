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

namespace Sample
{
    public class UnifiedMapViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly ICommand _pinSelectedCommand;
        private readonly ICommand _changeMapTypeCommand;
        private readonly Command _addPinCommand;
        private readonly Command _removePinCommand;
        private readonly ICommand _moveToRegionCommand;

        private readonly Command _addPolylineCommand;
        private readonly Command _removePolylineCommand;

        private readonly LinkedList<IMapPin> _allPins;
        private readonly LinkedList<MapPolyline> _allPolylines;

        private MapType _mapType = MapType.Street;
        private bool _hasScrollEnabled = true;
        private bool _hasZoomEnabled = true;
        private bool _isShowingUserLocation;
        private IMapAnnotation _selectedItem;

        public UnifiedMapViewModel ()
        {
            _pinSelectedCommand =
                new Command<MapPin> (pin => Debug.WriteLine ($"Pin {pin.Title} was selected"));

            _changeMapTypeCommand =
                new Command<MapType> (m => MapDisplayType = m);

            _addPinCommand =
                new Command (AddPin, o => _allPins.Any ());

            _removePinCommand =
                new Command (RemovePin, o => Pins.Any ());

            _moveToRegionCommand =
                new Command (() => Map.MoveToRegion (animated: true));

            _addPolylineCommand =
                new Command (AddPolyline, o => _allPolylines.Any ());

            _removePolylineCommand =
                new Command (RemovePolyline, o => Polylines.Any ());

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
                        Anchor = new Point(0.5, 0)
                    },
                    new MapPin
                    {
                        Title = "fivenine",
                        Snippet = "fivenine GmbH",
                        Location = new Position(47.389097, 8.517756),
                        Image = "pin_icon",
                        SelectedImage = "pin_icon_active",
                        Anchor = new Point(0.5, 0.6)
                    },
                });

            Pins = new ObservableCollection<IMapPin> ();

            _allPolylines = new LinkedList<MapPolyline> ();

            var polyline = new MapPolyline ();
            foreach (var pin in _allPins) {
                polyline.Add (pin.Location);
            }

            _allPolylines.AddLast (polyline);

            Polylines = new ObservableCollection<MapPolyline> ();

            // Add some sample pins
            AddPin (null);
            AddPin (null);

            // Add some polylines
            AddPolyline (null);

            SelectedItem = Pins.LastOrDefault();
        }

        internal UnifiedMap Map { get; set; }

        public ObservableCollection<IMapPin> Pins { get; set; }

        public ObservableCollection<MapPolyline> Polylines { get; set; }

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

        public ICommand PinSelectedCommand => _pinSelectedCommand;

        public ICommand ChangeMapTypeCommand => _changeMapTypeCommand;

        public ICommand AddPinCommand => _addPinCommand;

        public ICommand RemovePinCommand => _removePinCommand;

        public ICommand MoveToRegionCommand => _moveToRegionCommand;

        public ICommand AddPolylineCommand => _addPolylineCommand;

        public ICommand RemovePolylineCommand => _removePolylineCommand;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged ([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName));
        }

        private void AddPin (object o)
        {
            if (_allPins.Last == null) {
                return;
            }

            var pin = _allPins.Last.Value;
            _allPins.RemoveLast ();

            Pins.Add (pin);

            _removePinCommand.ChangeCanExecute ();
            _addPinCommand.ChangeCanExecute ();
        }

        private void RemovePin (object o)
        {
            if (Pins.Count == 0) {
                return;
            }

            var pin = Pins.LastOrDefault ();
            _allPins.AddLast (pin);

            Pins.Remove (pin);

            _removePinCommand.ChangeCanExecute ();
            _addPinCommand.ChangeCanExecute ();
        }

        private void AddPolyline (object o)
        {
            if (_allPolylines.Last == null) {
                return;
            }

            var polyline = _allPolylines.Last.Value;
            _allPolylines.RemoveLast ();

            Polylines.Add (polyline);

            _removePolylineCommand.ChangeCanExecute ();
            _addPolylineCommand.ChangeCanExecute ();
        }

        private void RemovePolyline (object o)
        {
            if (Polylines.Count == 0) {
                return;
            }

            var polyline = Polylines.LastOrDefault ();
            _allPolylines.AddLast (polyline);

            Polylines.Remove (polyline);

            _removePolylineCommand.ChangeCanExecute ();
            _addPolylineCommand.ChangeCanExecute ();
        }
    }
}