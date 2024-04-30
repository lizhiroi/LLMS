using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LLMS.Helper
{
    public class DecimalToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((decimal)value).ToString("F2");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (decimal.TryParse((string)value, out decimal result))
            {
                return result;
            }
            return DependencyProperty.UnsetValue; 
        }
    }
}
