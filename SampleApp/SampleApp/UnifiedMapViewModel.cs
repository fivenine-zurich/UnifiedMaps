using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
        private MapType _mapType = MapType.Street;

        public UnifiedMapViewModel()
        {
            _pinSelectedCommand =
                new Command<MapPin>(pin => Debug.WriteLine($"Pin {pin.Title} was selected"));

            _changeMapTypeCommand =
                new Command<MapType>(m => MapDisplayType = m);

            Pins = new ObservableCollection<MapPin>(
                new[]
                {
                    new MapPin
                    {
                        Title = "Zürich",
                        Location = new Position(47.3667, 8.5500),
                    },
                    new MapPin
                    {
                        Title = "Brändlen",
                        Location = new Position(46.904829, 8.409724),
                    },
                    new MapPin
                    {
                        Title = "Wolfenschiessen",
                        Location = new Position(46.905180, 8.398110),
                    },
                    new MapPin
                    {
                        Title = "Klewenalp",
                        Location = new Position(46.939898, 8.475217),
                    },
                    new MapPin
                    {
                        Title = "Beckenried NW",
                        Location = new Position(46.963876, 8.482078),
                    }
                });
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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
    }
}