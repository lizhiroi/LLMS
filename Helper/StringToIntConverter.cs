using System;
using System.Globalization;
using System.Windows.Data;

namespace LLMS.Helper
{
    public class StringToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
    
            if (string.IsNullOrEmpty(value as string))
            {
                return 0; 
            }

            int number;
            if (int.TryParse(value as string, out number))
            {
                return number;
            }

            return 0; 
        }
    }
}
