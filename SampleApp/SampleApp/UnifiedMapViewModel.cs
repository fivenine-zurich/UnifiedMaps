﻿using System.Collections.ObjectModel;
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

        private readonly LinkedList<MapPin> _allPins;
        private MapType _mapType = MapType.Street;

        public UnifiedMapViewModel()
        {
            _pinSelectedCommand =
                new Command<MapPin>(pin => Debug.WriteLine($"Pin {pin.Title} was selected"));

            _changeMapTypeCommand =
                new Command<MapType>(m => MapDisplayType = m);

            _addPinCommand =
                new Command(AddPin, o => _allPins.Any());

            _removePinCommand =
                new Command(RemovePin, o => Pins.Any());

            _moveToRegionCommand =
                new Command(() => Map.MoveToRegion(animated: true));

            _allPins = new LinkedList<MapPin>(
                new []
                {

                    new MapPin
                    {
                        Title = "Zürich",
                        Snippet = "It's awesome",
                        Location = new Position(47.3667, 8.5500),
                        Color = PinColor.Purple
                    },
                    new MapPin
                    {
                        Title = "Brändlen",
                        Location = new Position(46.904829, 8.409724),
                        Color = PinColor.Red
                    },
                    new MapPin
                    {
                        Title = "Wolfenschiessen",
                        Snippet = "... nothing to see here",
                        Location = new Position(46.905180, 8.398110),
                        Color = PinColor.Green
                    },
                    new MapPin
                    {
                        Title = "Klewenalp",
                        Location = new Position(46.939898, 8.475217),
                        Color = PinColor.Green
                    },
                    new MapPin
                    {
                        Title = "Beckenried NW",
                        Location = new Position(46.963876, 8.482078),
                        Color = PinColor.Red
                    }
                });

            Pins = new ObservableCollection<MapPin>();

            // Add some sample pins
            AddPin(null);
            AddPin(null);
        }

        internal UnifiedMap Map { get; set; }

        public ObservableCollection<MapPin> Pins { get; set; }

        public MapType MapDisplayType
        {
            get{ return _mapType;}
            set 
            {
                _mapType = value;
                OnPropertyChanged();
            }
        }

        public ICommand PinSelectedCommand => _pinSelectedCommand;

        public ICommand ChangeMapTypeCommand => _changeMapTypeCommand;

        public ICommand AddPinCommand => _addPinCommand;

        public ICommand RemovePinCommand => _removePinCommand;

        public ICommand MoveToRegionCommand => _moveToRegionCommand;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void AddPin(object o)
        {
            if( _allPins.Last == null )
            {
                return;
            }

            var pin = _allPins.Last.Value;
            _allPins.RemoveLast();

            Pins.Add(pin);

            _removePinCommand.ChangeCanExecute();
            _addPinCommand.ChangeCanExecute();
        }

        private void RemovePin(object o)
        {
            if( Pins.Count == 0 )
            {
                return;
            }

            var pin = Pins.LastOrDefault();
            _allPins.AddLast(pin);

            Pins.Remove(pin);

            _removePinCommand.ChangeCanExecute();
            _addPinCommand.ChangeCanExecute();
        }
    }
}