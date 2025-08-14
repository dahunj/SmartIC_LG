using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SmartICAVI.UserControls
{
    [ValueConversion(typeof(double), typeof(int))]
    class DoubleToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)(double)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)(int)value;
        }
    }
}
