using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace diplom.Components
{
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            string visibilityParameter = parameter as string;

            return value == null ? (visibilityParameter == "Visible" ? Visibility.Visible : Visibility.Hidden) : Visibility.Collapsed;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
