using Creobe.VoiceMemos.Helpers;
using System;
using System.Windows.Data;

namespace Creobe.VoiceMemos.ValueConverters
{
    public class IsolatedStoragePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string path = value as string;

            if (!string.IsNullOrWhiteSpace(path))
            {
                return StorageHelper.GetFilePath(path);
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
