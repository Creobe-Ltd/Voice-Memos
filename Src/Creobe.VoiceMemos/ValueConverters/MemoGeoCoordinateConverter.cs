using Creobe.VoiceMemos.Data.Models;
using Creobe.VoiceMemos.Models;
using System;
using System.Device.Location;
using System.Windows.Data;

namespace Creobe.VoiceMemos.ValueConverters
{
    public class MemoGeoCoordinateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var memo = value as Memo;

            if (memo != null)
            {
                if (memo.Latitude.HasValue && memo.Longitude.HasValue)
                    return new GeoCoordinate(memo.Latitude.Value, memo.Longitude.Value);
                else
                    return null;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
