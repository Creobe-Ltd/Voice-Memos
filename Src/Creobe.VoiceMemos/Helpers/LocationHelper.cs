using Microsoft.Phone.Maps.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace Creobe.VoiceMemos.Helpers
{
    public class LocationHelper
    {
        private static Geolocator _locator = new Geolocator();

        public static async Task<Geoposition> GetLocationAsync()
        {

            if (_locator != null && _locator.LocationStatus != PositionStatus.Disabled)
            {
                var position = await _locator.GetGeopositionAsync(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
                return position;
            }

            return null;
        }

        public static async Task<string> GetAddressAsync(Geoposition position)
        {
            var reverseGeocodeQuery = new ReverseGeocodeQuery();

            reverseGeocodeQuery.GeoCoordinate = position.Coordinate.ToGeoCoordinate();
            var locations = await reverseGeocodeQuery.GetMapLocationsAsync();

            if (locations != null && locations.Count > 0)
            {
                var location = locations.FirstOrDefault();

                if (location.Information != null && location.Information.Address != null)
                {
                    var address = location.Information.Address;

                    string streetAddress = string.Format("{0} {1}", address.Street, address.HouseNumber).Trim();
                    string cityAddress = string.Format("{0} {1}", address.PostalCode, address.City).Trim();
                    string countryAddress = address.Country;

                    return string.Format("{0}, {1}, {2}", streetAddress, cityAddress, countryAddress)
                        .Trim(new char[] { ' ', ',' });
                }
            }

            return null;

        }

        public static void StartTracking()
        {
            try
            {
                if (_locator == null)
                    _locator = new Geolocator();

                _locator.MovementThreshold = 1000;
                _locator.PositionChanged += _locator_PositionChanged;
            }
            catch
            {
                //do nothing
            }
        }

        public static void StopTracking()
        {
            _locator.PositionChanged -= _locator_PositionChanged;
        }

        private static void _locator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            //do nothing
        }
    }
}
