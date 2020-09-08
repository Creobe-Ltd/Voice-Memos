using System;
using System.Windows;
using System.Windows.Data;

namespace Creobe.VoiceMemos.ValueConverters
{
    public class EmptyToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter == null)
                throw new ArgumentOutOfRangeException("parameter must not be null");

            if (parameter.GetType() != typeof(String))
                throw new ArgumentOutOfRangeException("parameter must be a string");

            if (!parameter.ToString().ToLower().Equals("visible") && !parameter.ToString().ToLower().Equals("collapsed"))
                throw new ArgumentOutOfRangeException("parameter value must be either Visible or Collapsed");

            Visibility desiredVisibility = Visibility.Collapsed;
            Visibility opositeVisibility = Visibility.Visible;

            if (parameter.ToString().ToLower().Equals("visible"))
            {
                desiredVisibility = Visibility.Visible;
                opositeVisibility = Visibility.Collapsed;
            }


            if (value != null)
            {
                if (value is int)
                    return ((int)value) <= 0? desiredVisibility: opositeVisibility;

                if (value is string)
                    return string.IsNullOrWhiteSpace(value.ToString()) ? desiredVisibility : opositeVisibility;
            }

            return desiredVisibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
